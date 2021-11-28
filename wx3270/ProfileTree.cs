// <copyright file="ProfileTree.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using I18nBase;
    using IWshRuntimeLibrary;
    using Wx3270.Contracts;
    using File = System.IO.File;

    /// <summary>
    /// Hierarchy of hosts in profiles.
    /// </summary>
    public partial class ProfileTree : Form
    {
        /// <summary>
        /// The separator string.
        /// </summary>
        public const string Separator = "*";

        /// <summary>
        /// String name, for localization.
        /// </summary>
        private static readonly string StringName = I18n.StringName(nameof(ProfileTree));

        /// <summary>
        /// Title name, for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(ProfileTree));

        /// <summary>
        /// Message name, for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(ProfileTree));

        /// <summary>
        /// Static copy of restrictions, for creating new windows.
        /// </summary>
        private static Restrictions restrictions;

        /// <summary>
        /// The application context.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// The main screen.
        /// </summary>
        private readonly MainScreen mainScreen;

        /// <summary>
        /// The font for normal entries.
        /// </summary>
        private readonly Font normalFont;

        /// <summary>
        /// The font for the current profile.
        /// </summary>
        private readonly Font currentProfileFont;

        /// <summary>
        /// Connection handler.
        /// </summary>
        private readonly Connect connect;

        /// <summary>
        /// The tree node that was right-clicked.
        /// </summary>
        private TreeNode rightClickNode;

        /// <summary>
        /// The selected node (while we maintain focus).
        /// </summary>
        private TreeNode selectedNode;

        /// <summary>
        /// True if we are connected.
        /// </summary>
        private bool connected;

        /// <summary>
        /// The path of a node to automatically rename when it appears.
        /// </summary>
        private string autoRenamePath;

        /// <summary>
        /// The path of a node to automatically select when the list is refreshed.
        /// </summary>
        private string autoSelectPath;

        /// <summary>
        /// True if we should auto-connect when switching profiles.
        /// </summary>
        private bool doAutoConnect;

        /// <summary>
        /// Where the left mouse button was pressed for drag-drop.
        /// </summary>
        private Point? mouseDownPoint;

        /// <summary>
        /// True if the form has ever been activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// The window handle.
        /// </summary>
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileTree"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="mainScreen">Main screen.</param>
        /// <param name="connect">Connection handler.</param>
        public ProfileTree(Wx3270App app, MainScreen mainScreen, Connect connect)
        {
            this.InitializeComponent();
            this.app = app;
            this.mainScreen = mainScreen;
            this.connect = connect;

            restrictions = app.Restrictions;

            this.normalFont = this.treeView.Font;
            this.currentProfileFont = new Font(this.normalFont, FontStyle.Underline);

            // Subscribe to tree changes and get the initial tree.
            this.handle = this.Handle;
            this.app.ProfileTracker.ProfileTreeChanged += (newTree) => this.Invoke(new MethodInvoker(() => this.TreeChanged(newTree)));
            this.TreeChanged(this.app.ProfileTracker.Tree);

            // Subscribe to profile changes.
            this.ProfileManager.ChangeTo += (previous, current) =>
                this.TreeChanged(this.app.ProfileTracker.Tree, previous == null || !previous.Name.Equals(current.Name));

            this.ProfileManager.ChangeFinal += (profile, _) =>
            {
                if (this.doAutoConnect)
                {
                    var autoConnectHost = profile.Hosts.FirstOrDefault(h => h.AutoConnect == AutoConnect.Connect || h.AutoConnect == AutoConnect.Reconnect);
                    if (autoConnectHost != null)
                    {
                        this.connect.ConnectToHost(autoConnectHost);
                    }

                    this.doAutoConnect = false;
                }
            };

            this.ProfileManager.DefaultProfileChanged += () => this.TreeChanged(this.app.ProfileTracker.Tree);

            // Subscribe to connection state events.
            mainScreen.ConnectionStateEvent += this.HostConnectionChange;

            // Set up the merge handler. This really isn't specific to this dialog, and should probably be define
            this.ProfileManager.RegisterMerge(ImportType.HostsMerge | ImportType.HostsReplace, MergeHandler);

            // Set up the undo and redo buttons.
            this.ProfileManager.RegisterUndoRedo(this.undoButton, this.redoButton, this.toolTip1);

            // Subscribe to refocus events.
            this.ProfileManager.RefocusEvent += (profileDir, profileName, hostName) =>
            {
                var pathList = new List<string>();
                if (profileDir != null)
                {
                    pathList.Add(profileDir);
                }

                if (profileName != null)
                {
                    pathList.Add(profileName);
                    if (hostName != null)
                    {
                        pathList.Add(hostName);
                    }
                }

                this.autoSelectPath = this.PathCombine(pathList);
            };

            // Set up UI actions.
            app.BackEnd.RegisterPassthru(Constants.Action.Connect, this.UiConnect);
            app.BackEnd.RegisterPassthru(Constants.Action.SwitchProfile, this.UiSwitchProfile);

            // Fix up some text.
            VersionSpecific.Substitute(this);
            VersionSpecific.Substitute(this.hostContextMenuStrip);
            VersionSpecific.Substitute(this.profileContextMenuStrip);
            VersionSpecific.Substitute(this.brokenProfileContextMenuStrip);
            VersionSpecific.Substitute(this.folderContextMenuStrip);
            VersionSpecific.Substitute(this.defaultsContextMenuStrip);

            // Handle restrictions.
            if (app.Restricted(Restrictions.ModifyProfiles))
            {
                this.commonDuplicateButton.RemoveFromParent();
                this.commonRenameButton.RemoveFromParent();
                this.commonDeleteButton.RemoveFromParent();

                this.profileNewButton.RemoveFromParent();
                this.profileMergeFromButton.RemoveFromParent();
                this.profileImportButton.RemoveFromParent();
                this.profileExportButton.RemoveFromParent();
                this.profileDefaultButton.RemoveFromParent();

                this.folderGroupBox.RemoveFromParent();

                this.profileDeleteToolStripMenuItem.RemoveFromOwner();
                this.profileDuplicateToolStripMenuItem.RemoveFromOwner();
                this.profileRenameToolStripMenuItem.RemoveFromOwner();
                this.profileMergeFromToolStripMenuItem.RemoveFromOwner();
                this.profileSetAsDefaultToolStripMenuItem.RemoveFromOwner();

                this.brokenProfileDeleteToolStripMenuItem.RemoveFromOwner();

                this.defaultsDuplicateToolStripMenuItem.RemoveFromOwner();

                this.folderNewProfileToolStripMenuItem.RemoveFromOwner();
                this.folderImportToolStripMenuItem.RemoveFromOwner();
                this.folderStopWatchingToolStripMenuItem.RemoveFromOwner();
            }

            if (this.app.Restricted(Restrictions.ModifyProfiles) || this.app.Restricted(Restrictions.ModifyHost))
            {
                this.topNewConnectionButton.RemoveFromParent();

                this.connectionNewButton.RemoveFromParent();

                this.hostEditToolStripMenuItem.RemoveFromOwner();
                this.hostDuplicateToolStripMenuItem.RemoveFromOwner();
                this.hostRenameToolStripMenuItem.RemoveFromOwner();
                this.hostDeleteToolStripMenuItem.RemoveFromOwner();
            }

            if (this.app.Restricted(Restrictions.ModifyProfiles) ||
                this.app.Restricted(Restrictions.ModifyHost) ||
                this.app.Restricted(Restrictions.ChangeSettings))
            {
                this.undoButton.RemoveFromParent();
                this.redoButton.RemoveFromParent();
            }

            if (this.app.Restricted(Restrictions.Disconnect))
            {
                this.topDisconnectButton.RemoveFromParent();
            }

            if (app.Restricted(Restrictions.ExternalFiles))
            {
                this.commonShortcutButton.RemoveFromParent();

                this.hostShortcutToolStripMenuItem.RemoveFromOwner();

                this.profileExportToolStripMenuItem.RemoveFromOwner();
                this.profileShortcutToolStripMenuItem.RemoveFromOwner();

                this.folderDeleteToolStripMenuItem.RemoveFromOwner();
            }

            if (app.Restricted(Restrictions.SwitchProfile))
            {
                this.profileNewButton.RemoveFromParent();
                this.profileSwitchToButton.RemoveFromParent();
                this.folderNewButton.RemoveFromParent();

                this.profileSwitchToToolStripMenuItem.RemoveFromOwner();
                this.defaultsDuplicateToolStripMenuItem.RemoveFromOwner();
            }

            if (app.Restricted(Restrictions.NewWindow))
            {
                this.useShiftLabel.RemoveFromParent();
            }

            if (app.Restricted(Restrictions.ModifyHost) && app.Restricted(Restrictions.SwitchProfile))
            {
                this.commonEditButton.RemoveFromParent();
            }

            if (app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // There may be nothing left of the common group box.
            if (!this.commonFlowLayoutPanel.Controls.OfType<Button>().Any())
            {
                this.commonGroupBox.RemoveFromParent();
            }

            // There may be nothing left of the profile group box.
            if (!this.profileFlowLayoutPanel.Controls.OfType<Button>().Any())
            {
                this.profileGroupBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(
                this,
                this.toolTip1,
                this.profileContextMenuStrip,
                this.brokenProfileContextMenuStrip,
                this.hostContextMenuStrip,
                this.defaultsContextMenuStrip,
                this.folderContextMenuStrip);
            this.exportProfileDialog.Title = I18n.Localize(this, "exportProfileDialog", this.exportProfileDialog.Title);
            this.shortcutDialog.Title = I18n.Localize(this, "shortcutDialog", this.shortcutDialog.Title);
            this.watchFolderDialog.Description = I18n.Localize(this, I18n.Combine("watchFolderDialog", "Description"), this.watchFolderDialog.Description);

            // Update some tool tips.
            if (this.app.Allowed(Restrictions.NewWindow))
            {
                var openText = Environment.NewLine + I18n.Get(Message.OpenInNewWindow);

                this.toolTip1.SetToolTip(this.profileSwitchToButton, this.toolTip1.GetToolTip(this.profileSwitchToButton) + openText);
                this.toolTip1.SetToolTip(this.commonEditButton, this.toolTip1.GetToolTip(this.commonEditButton) + openText);
                this.toolTip1.SetToolTip(this.connectionConnectButton, this.toolTip1.GetToolTip(this.connectionConnectButton) + openText);

                this.profileSwitchToToolStripMenuItem.ToolTipText += openText;
                this.profileEditToolStripMenuItem.ToolTipText += openText;
                this.hostConnectToolStripMenuItem.ToolTipText += openText;
            }
        }

        /// <summary>
        /// Image enumeration for icons in the tree view.
        /// </summary>
        private enum ImageEnum
        {
            /// <summary>
            /// Folder icon (default).
            /// </summary>
            Folder = 0,

            /// <summary>
            /// Plug icon.
            /// </summary>
            Host = 1,

            /// <summary>
            /// Red plug icon (auto-connect).
            /// </summary>
            AutoConnectHost = 4,

            /// <summary>
            /// Folder with an X through it (broken).
            /// </summary>
            BrokenFolder = 3,

            /// <summary>
            /// Green folder (used for actual folders).
            /// </summary>
            GreenFolder = 5,

            /// <summary>
            /// Folder with a star (default profile).
            /// </summary>
            StarFolder = 6,

            /// <summary>
            /// Keymap profile.
            /// </summary>
            KeymapFolder = 7,

            /// <summary>
            /// Keypad profile.
            /// </summary>
            KeypadFolder = 8,
        }

        /// <summary>
        /// Gets the display name of the default profile directory.
        /// </summary>
        public static string DefaultDirNodeName => DirNodeName(Wx3270.ProfileManager.ProfileDirectory);

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager => this.app.ProfileManager;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(String.Documents, "Documents");
            I18n.LocalizeGlobal(String.Desktop, "Desktop");

            I18n.LocalizeGlobal(Title.NewWindow, "New Window");
            I18n.LocalizeGlobal(Title.AddConnection, "Add Connection");
            I18n.LocalizeGlobal(Title.CopyConnection, "Copy Connection");
            I18n.LocalizeGlobal(Title.MoveConnection, "Move Connection");
            I18n.LocalizeGlobal(Title.RenameConnection, "Rename Connection");
            I18n.LocalizeGlobal(Title.DuplicateConnection, "Duplicate Connection");
            I18n.LocalizeGlobal(Title.DeleteConnection, "Delete Connection");
            I18n.LocalizeGlobal(Title.EditConnection, "Edit Connection");
            I18n.LocalizeGlobal(Title.CopyProfile, "Copy Profile");
            I18n.LocalizeGlobal(Title.UndoCopyProfile, "Undo Copy Profile");
            I18n.LocalizeGlobal(Title.RenameProfile, "Rename Profile");
            I18n.LocalizeGlobal(Title.UndoRenameProfile, "Undo Rename Profile");
            I18n.LocalizeGlobal(Title.DeleteProfile, "Delete Profile");
            I18n.LocalizeGlobal(Title.UndoDeleteProfile, "Undo Delete Profile");
            I18n.LocalizeGlobal(Title.DuplicateProfile, "Duplicate Profile");
            I18n.LocalizeGlobal(Title.UndoDuplicateProfile, "Undo Duplicate Profile");
            I18n.LocalizeGlobal(Title.CreateProfile, "Create Profile");
            I18n.LocalizeGlobal(Title.UndoCreateProfile, "Undo Create Profile");
            I18n.LocalizeGlobal(Title.ImportProfile, "Import Profile");
            I18n.LocalizeGlobal(Title.UndoImportProfile, "Undo Import Profile");
            I18n.LocalizeGlobal(Title.StopWatchingFolder, "Stop Watching Folder");
            I18n.LocalizeGlobal(Title.UndoWatchFolder, "Undo Watching Folder");
            I18n.LocalizeGlobal(Title.DeleteFolder, "Delete Folder");
            I18n.LocalizeGlobal(Title.UndoDeleteFolder, "Undo Folder Delete");

            I18n.LocalizeGlobal(Title.ImportProfileDialog, "Select profile to import into {0}");

            I18n.LocalizeGlobal(Message.NameAlreadyExists, "Name already exists");
            I18n.LocalizeGlobal(Message.ProfileSaveFailed, "Profile save failed");
            I18n.LocalizeGlobal(Message.InvalidFileType, "Invalid file type");
            I18n.LocalizeGlobal(Message.CannotStopWatching, "Cannot stop watching current folder");
            I18n.LocalizeGlobal(Message.CannotDeleteCurrentProfile, "Cannot delete current profile");
            I18n.LocalizeGlobal(Message.CannotOverwriteCurrentProfile, "Cannot overwrite current profile");
            I18n.LocalizeGlobal(Message.CannotRenameCurrentProfile, "Cannot rename current profile");

            I18n.LocalizeGlobal(Message.AddConnection, "add connection {0}");
            I18n.LocalizeGlobal(Message.MoveConnection, "move connection {0}");
            I18n.LocalizeGlobal(Message.RenameConnection, "rename connection {0} to {1}");
            I18n.LocalizeGlobal(Message.DuplicateConnection, "duplicate connection {0}");
            I18n.LocalizeGlobal(Message.DeleteConnection, "delete connection {0}");
            I18n.LocalizeGlobal(Message.EditConnection, "edit connection {0}");
            I18n.LocalizeGlobal(Message.CopyProfile, "copy profile '{0}' from '{1}' to '{2}'");
            I18n.LocalizeGlobal(Message.RenameProfile, "rename profile '{0}' to '{1}'");
            I18n.LocalizeGlobal(Message.DuplicateProfile, "duplicate profile '{0}' to '{1}'");
            I18n.LocalizeGlobal(Message.CreateProfile, "create new profile '{0}' in '{1}'");
            I18n.LocalizeGlobal(Message.ImportProfile, "import profile '{0}'");
            I18n.LocalizeGlobal(Message.DeleteProfile, "delete profile '{0}'");
            I18n.LocalizeGlobal(Message.WatchFolder, "watch folder '{0}'");
            I18n.LocalizeGlobal(Message.StopWatchingFolder, "stop watching folder '{0}'");
            I18n.LocalizeGlobal(Message.DeleteFolder, "delete folder '{0}'");

            I18n.LocalizeGlobal(Message.Copy, "Copy");
            I18n.LocalizeGlobal(Message.NewProfile, "New Profile");
            I18n.LocalizeGlobal(Message.NewKeymapTemplate, "New Keyboard Map Template");
            I18n.LocalizeGlobal(Message.NewKeypadTemplate, "New Keypad Map Template");
            I18n.LocalizeGlobal(Message.IsDefaultProfile, "Default profile");
            I18n.LocalizeGlobal(Message.IsCurrentProfile, "Current profile");
            I18n.LocalizeGlobal(Message.IsKeyboardMap, "Keyboard map template");
            I18n.LocalizeGlobal(Message.IsKeypadMap, "Keypad map template");

            I18n.LocalizeGlobal(Message.OpenInNewWindow, "Shift: Open in new window");
        }

        /// <summary>
        /// Map a directory path to a display name for a node.
        /// </summary>
        /// <param name="dirName">Directory path.</param>
        /// <returns>Display name.</returns>
        public static string DirNodeName(string dirName)
        {
            var specials = new Dictionary<Environment.SpecialFolder, string>
            {
                [Environment.SpecialFolder.MyDocuments] = I18n.Get(String.Documents),
                [Environment.SpecialFolder.Desktop] = I18n.Get(String.Desktop),
            };

            foreach (var kv in specials)
            {
                var special = Environment.GetFolderPath(kv.Key);
                if (dirName.Equals(special, StringComparison.InvariantCultureIgnoreCase))
                {
                    return kv.Value;
                }

                if (dirName.StartsWith(special + Path.DirectorySeparatorChar, StringComparison.InvariantCultureIgnoreCase))
                {
                    return kv.Value + dirName.Substring(special.Length);
                }
            }

            return dirName;
        }

        /// <summary>
        /// Start a new window.
        /// </summary>
        /// <param name="parentWindow">Parent window.</param>
        /// <param name="components">Parent form components.</param>
        /// <param name="profilePath">Profile path name.</param>
        /// <param name="host">Host name.</param>
        /// <param name="editMode">True to open in edit mode (suppress auto-connect, warn if read-only).</param>
        /// <param name="readWriteMode">True to open in read/write mode (allow auto-connect, warn if read-only).</param>
        public static void NewWindow(Form parentWindow, IContainer components, string profilePath, string host = null, bool editMode = false, bool readWriteMode = false)
        {
            if (restrictions.HasFlag(Restrictions.NewWindow))
            {
                return;
            }

            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = Application.ExecutablePath;
            var args = new List<string>
            {
                Constants.Option.Profile,
                "\"" + profilePath + "\"",
            };
            if (host != null)
            {
                args.Add(Constants.Option.Host);
                args.Add("\"" + host + "\"");
            }

            if (editMode)
            {
                args.Add(Constants.Option.Edit);
            }

            if (readWriteMode)
            {
                args.Add(Constants.Option.ReadWrite);
            }

            if (restrictions != Restrictions.None)
            {
                args.Add(Constants.Option.Restrict);
                args.Add(restrictions.ToString());
            }

            args.Add(Constants.Option.Topmost);

            p.StartInfo.Arguments = string.Join(" ", args);

            try
            {
                p.Start();

                // It takes a while for the new window to be displayed.
                // Switch to an AppStarting cursor, so there is immediate feedback.
                // Switch back after 2 seconds, which is rather arbitrary.
                // I tried using p.WaitForInputIdle to explicitly wait for the window to come up, but
                // it returned immediately, so apparently the window is 'available' almost immediately,
                // but not yet displayed.
                parentWindow.Cursor = Cursors.AppStarting;
                var t = new Timer(components) { Interval = 2000, Enabled = true };
                t.Tick += (sender, e) =>
                {
                    parentWindow.Cursor = Cursors.Default;
                    t.Stop();
                    t.Dispose(); // Dangerous?
                };
                t.Start();
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.NewWindow));
            }

            return;
        }

        /// <summary>
        /// Create a new host.
        /// </summary>
        /// <param name="profile">Profile to add host to.</param>
        /// <param name="existingHostEntry">Optional existing host entry (used for macro editor completion).</param>
        public void CreateHostDialog(Profile profile, HostEntry existingHostEntry = null)
        {
            // Pop up the dialog.
            using var editor = new HostEditor(HostEditingMode.QuickConnect, existingHostEntry, profile, this.app);
            HostEntry hostEntry = null;
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (editor.Result.HasFlag(HostEditingResult.Save))
                {
                    // Save the host.
                    hostEntry = editor.HostEntry;
                    if (profile.Hosts.Any(h => h.Name.Equals(hostEntry.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        hostEntry.Name = CreateUniqueName(hostEntry.Name, profile.Hosts.Select(p => p.Name));
                    }

                    var newEntryPath = this.PathCombine(profile.DisplayFolder, profile.Name, hostEntry.Name);
                    var refocus = new ProfileRefocus(
                        this.ProfileManager,
                        Separator,
                        this.PathCombine(profile.DisplayFolder, profile.Name),
                        newEntryPath);
                    try
                    {
                        this.autoSelectPath = newEntryPath;
                        this.ProfileManager.PushAndSave(
                            (current) =>
                            {
                                current.Hosts = current.Hosts.Concat(new[] { hostEntry }).ToArray();
                            },
                            string.Format(I18n.Get(Message.AddConnection), hostEntry.Name),
                            profile,
                            refocus);
                    }
                    catch (InvalidOperationException e)
                    {
                        ErrorBox.Show(e.Message, I18n.Get(Title.AddConnection));
                        this.autoSelectPath = null;
                        return;
                    }
                }

                if (editor.Result.HasFlag(HostEditingResult.Connect))
                {
                    // Connect to the host.
                    this.connect.ConnectToHost(hostEntry);
                    this.SafeHide();
                }

                if (editor.Result.HasFlag(HostEditingResult.Record))
                {
                    // Macro recorder started.
                    var editingMode = editor.Result.HasFlag(HostEditingResult.Save) ? HostEditingMode.SaveHost : HostEditingMode.QuickConnect;
                    this.app.MacroRecorder.Start(this.CreateHostMacroRecorderDone, (editor.HostEntry, profile, editingMode));
                    this.Hide();
                    this.mainScreen.Focus();
                }
            }
        }

        /// <summary>
        /// Load a profile, with auto-connect.
        /// </summary>
        /// <param name="profilePathName">Profile path name.</param>
        /// <param name="isShift">True if the shift key is down (new window).</param>
        /// <param name="doErrorPopups">True to do a pop-up if there is an error.</param>
        /// <returns>True if connect succeeded.</returns>
        public bool LoadWithAutoConnect(string profilePathName, bool isShift, bool doErrorPopups = true)
        {
            if (isShift)
            {
                // Open the profile in a new window, trying for read/write mode unless this is the current profile.
                NewWindow(this, this.components, profilePathName);
                return true;
            }

            // Explicitly check for switching to the same profile here, so we don't leave doAutoConnect dangling.
            if (this.ProfileManager.IsCurrentPathName(profilePathName))
            {
                return true;
            }

            this.doAutoConnect = true;
            if (!this.ProfileManager.Load(profilePathName, doErrorPopups: doErrorPopups))
            {
                this.doAutoConnect = false;
                return false;
            }

            // No more undo/redo.
            ////this.ProfileManager.FlushUndoRedo();

            return true;
        }

        /// <summary>
        /// Creates a unique version of <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Name to modify.</param>
        /// <param name="names">Existing names.</param>
        /// <returns>Unique name.</returns>
        private static string CreateUniqueName(string name, IEnumerable<string> names)
        {
            // If the name is already of the form 'xxx (n)', start incrementing n. Otherwise start with 1.
            string prefix;
            int n;
            var regex = new Regex(@"^(?<prefix>.*) \((?<n>\d+)\)$");
            var matches = regex.Match(name);
            if (matches.Success)
            {
                prefix = matches.Groups["prefix"].Value;
                n = int.Parse(matches.Groups["n"].Value);
            }
            else
            {
                prefix = name;
                n = 0;
            }

            // Spin until a unique name is found.
            string newName;
            while (names.Contains(newName = $"{prefix} ({++n})", StringComparer.InvariantCultureIgnoreCase))
            {
            }

            return newName;
        }

        /// <summary>
        /// Merge in the host definitions from another profile.
        /// </summary>
        /// <param name="toProfile">Current profile.</param>
        /// <param name="fromProfile">Merge profile.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if the list changed.</returns>
        private static bool MergeHandler(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.HostsReplace))
            {
                // Replace host definitions.
                if (!toProfile.Hosts.SequenceEqual(fromProfile.Hosts))
                {
                    toProfile.Hosts = fromProfile.Hosts.Select(host =>
                    {
                        var clone = host.Clone();
                        clone.Profile = toProfile;
                        return clone;
                    });
                    return true;
                }

                return false;
            }
            else
            {
                // Merge host definitions.
                var changed = false;
                var newHosts = toProfile.Hosts.ToDictionary(h => h.Name);
                foreach (var mergeHost in fromProfile.Hosts)
                {
                    if (!newHosts.TryGetValue(mergeHost.Name, out HostEntry found) || !mergeHost.Equals(found))
                    {
                        newHosts[mergeHost.Name] = mergeHost;
                        changed = true;
                    }
                }

                if (changed)
                {
                    toProfile.Hosts = newHosts.Values.Select(host =>
                    {
                        var clone = host.Clone();
                        clone.Profile = toProfile;
                        return clone;
                    });
                }

                return changed;
            }
        }

        /// <summary>
        /// Derive the compatible merge type for two profiles.
        /// </summary>
        /// <param name="profile1">First profile.</param>
        /// <param name="profile2">Second profile.</param>
        /// <returns>Compatible merge type, or null.</returns>
        private static ProfileType? CompatibleMergeType(Profile profile1, Profile profile2)
        {
            var type1 = profile1.ProfileType;
            var type2 = profile2.ProfileType;

            if (type1 == type2 || type2 == ProfileType.Full)
            {
                return type1;
            }

            if (type1 == ProfileType.Full)
            {
                return type2;
            }

            return null;
        }

        /// <summary>
        /// The macro recorder is complete for an added host.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="context">Context object.</param>
        private void CreateHostMacroRecorderDone(string text, object context)
        {
            var (entry, profile, mode) = (((HostEntry, Profile, HostEditingMode)?)context).Value;
            this.Show();
            entry.LoginMacro = text;
            if (mode == HostEditingMode.QuickConnect)
            {
                // Host entry has not been created yet.
                this.CreateHostDialog(profile, entry);
            }
            else
            {
                // Host entry has been created, we connected to the host and were recording a login macro.
                // This call assumes that the new entry is the selected node.
                this.EditHost(this.treeView.SelectedNode as HostTreeNode, entry);
            }
        }

        /// <summary>
        /// Execute an action for each tree node.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        /// <param name="seed">Base of tree or sub-tree to walk.</param>
        private void TreeViewForEach(Action<TreeNode> action, TreeNodeCollection seed = null)
        {
            if (seed == null)
            {
                this.TreeViewForEach(action, this.treeView.Nodes);
                return;
            }

            foreach (var child in seed.OfType<TreeNode>())
            {
                action(child);
                if (child.Nodes != null)
                {
                    this.TreeViewForEach(action, child.Nodes);
                }
            }
        }

        /// <summary>
        /// The profile tree changed. Update the UI.
        /// </summary>
        /// <param name="folderTree">New profile tree.</param>
        /// <param name="newProfile">True if the profile just switched.</param>
        private void TreeChanged(List<FolderWatchNode> folderTree, bool newProfile = false)
        {
            // Remember the current state.
            string selectedPath = this.treeView.SelectedNode != null ? this.treeView.SelectedNode.Name : string.Empty;
            var expanded = new HashSet<string>();
            this.TreeViewForEach((node) =>
            {
                if (node.IsExpanded)
                {
                    expanded.Add(node.Name);
                }
            });
            ProfileTreeNode currentProfileNode = null;

            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();

            foreach (var node in folderTree)
            {
                var treeNodeStack = new Stack<TreeNode>();
                node.ForEach(treeNodeStack, (n, stack) =>
                {
                    switch (n.Type)
                    {
                        case WatchNodeType.Folder:
                            var folder = n as FolderWatchNode;
                            if (this.app.Restricted(Restrictions.SwitchProfile)
                                && !folder.Any((w) => w is ProfileWatchNode && this.ProfileManager.IsCurrentPathName((w as ProfileWatchNode).PathName)))
                            {
                                return null;
                            }

                            var folderNode = new FolderTreeNode(stack.Any() ? folder.Name : DirNodeName(folder.PathName))
                            {
                                IsDefault = folder.Name.Equals(Wx3270.ProfileManager.ProfileDirectory, StringComparison.InvariantCultureIgnoreCase),
                                FolderName = folder.PathName,
                                ImageIndex = (int)ImageEnum.GreenFolder,
                                SelectedImageIndex = (int)ImageEnum.GreenFolder,
                                StateImageIndex = (int)ImageEnum.GreenFolder,
                                ContextMenuStrip = this.folderContextMenuStrip,
                            };

                            if (stack.Any())
                            {
                                stack.Peek().Nodes.Add(folderNode);
                            }
                            else
                            {
                                this.treeView.Nodes.Add(folderNode);
                            }

                            folderNode.Name = folderNode.JoinedFullPath();
                            if (folderNode.Name == selectedPath)
                            {
                                this.treeView.SelectedNode = folderNode;
                            }

                            return folderNode;

                        case WatchNodeType.Profile:
                            var profile = n as ProfileWatchNode;
                            var isCurrentProfile = this.ProfileManager.IsCurrentPathName(profile.PathName);
                            if (this.app.Restricted(Restrictions.SwitchProfile) && !isCurrentProfile)
                            {
                                // Can't switch profiles, no need to display a different one.
                                return null;
                            }

                            var isDefaults = profile.Name.Equals(Wx3270.ProfileManager.DefaultValuesName);
                            var profileNode = new ProfileTreeNode(profile.Name)
                            {
                                IsCurrent = isCurrentProfile,
                                IsBroken = profile.Broken,
                                IsDefaults = isDefaults,
                                Profile = profile.Profile,
                            };
                            profileNode.NodeFont = isCurrentProfile ? this.currentProfileFont : this.treeView.Font;
                            if (profile.Broken)
                            {
                                profileNode.ImageIndex = (int)ImageEnum.BrokenFolder;
                                profileNode.SelectedImageIndex = (int)ImageEnum.BrokenFolder;
                                profileNode.ContextMenuStrip = this.brokenProfileContextMenuStrip;
                            }
                            else if (isDefaults)
                            {
                                profileNode.ContextMenuStrip = this.defaultsContextMenuStrip;
                            }
                            else
                            {
                                profileNode.ContextMenuStrip = this.profileContextMenuStrip;

                                if (this.ProfileManager.IsDefaultPathName(profile.PathName))
                                {
                                    profileNode.ImageIndex = (int)ImageEnum.StarFolder;
                                    profileNode.SelectedImageIndex = (int)ImageEnum.StarFolder;
                                }
                                else if (profile.Profile.ProfileType == ProfileType.KeyboardMapTemplate)
                                {
                                    profileNode.ImageIndex = (int)ImageEnum.KeymapFolder;
                                    profileNode.SelectedImageIndex = (int)ImageEnum.KeymapFolder;
                                }
                                else if (profile.Profile.ProfileType == ProfileType.KeypadMapTemplate)
                                {
                                    profileNode.ImageIndex = (int)ImageEnum.KeypadFolder;
                                    profileNode.SelectedImageIndex = (int)ImageEnum.KeypadFolder;
                                }
                            }

                            if (isCurrentProfile)
                            {
                                currentProfileNode = profileNode;
                            }

                            stack.Peek().Nodes.Add(profileNode);
                            profileNode.Name = profileNode.JoinedFullPath();

                            if (profileNode.Name.Equals(selectedPath)
                                || (this.treeView.SelectedNode == null
                                    && profileNode.Name.Equals(this.PathCombine(this.PathSplit(selectedPath).Take(2)))))
                            {
                                // Preserve prior node selection, or select the profile that a host was deleted from.
                                this.treeView.SelectedNode = profileNode;
                            }

                            var toolTip = new List<string>();
                            if (profile.Profile != null && !string.IsNullOrWhiteSpace(profile.Profile.Description))
                            {
                                toolTip.Add(profile.Profile.Description);
                            }

                            if (isCurrentProfile || this.ProfileManager.IsDefaultPathName(profile.PathName))
                            {
                                if (isCurrentProfile)
                                {
                                    toolTip.Add(I18n.Get(Message.IsCurrentProfile));
                                }

                                if (this.ProfileManager.IsDefaultPathName(profile.PathName))
                                {
                                    toolTip.Add(I18n.Get(Message.IsDefaultProfile));
                                }
                            }
                            else if (profile.Profile != null && profile.Profile.ProfileType == ProfileType.KeyboardMapTemplate)
                            {
                                toolTip.Add(I18n.Get(Message.IsKeyboardMap));
                            }
                            else if (profile.Profile != null && profile.Profile.ProfileType == ProfileType.KeypadMapTemplate)
                            {
                                toolTip.Add(I18n.Get(Message.IsKeypadMap));
                            }

                            profileNode.ToolTipText = string.Join(Environment.NewLine, toolTip);
                            return profileNode;

                        case WatchNodeType.Host:
                            var host = n as HostWatchNode;
                            var isCurrentHost = this.connected
                                && (stack.Peek() as ProfileTreeNode).IsCurrent
                                && this.mainScreen.Connect.ConnectHostEntry != null
                                && this.mainScreen.Connect.ConnectHostEntry.Name.Equals(host.Name);
                            var hostNode = new HostTreeNode(host.Name)
                            {
                                Profile = (stack.Peek() as ProfileTreeNode).Profile,
                                AutoConnect = host.AutoConnect,
                                IsCurrentProfile = (stack.Peek() as ProfileTreeNode).IsCurrent,
                                IsCurrentHost = isCurrentHost,
                            };
                            hostNode.ImageIndex = (int)(host.AutoConnect ? ImageEnum.AutoConnectHost : ImageEnum.Host);
                            hostNode.SelectedImageIndex = (int)(host.AutoConnect ? ImageEnum.AutoConnectHost : ImageEnum.Host);
                            hostNode.ContextMenuStrip = this.hostContextMenuStrip;
                            if (isCurrentHost)
                            {
                                hostNode.NodeFont = this.currentProfileFont;
                            }

                            stack.Peek().Nodes.Add(hostNode);
                            hostNode.Name = hostNode.JoinedFullPath();
                            if (hostNode.Name == selectedPath)
                            {
                                this.treeView.SelectedNode = hostNode;
                            }

                            if (!string.IsNullOrWhiteSpace(host.HostEntry.Description))
                            {
                                hostNode.ToolTipText = host.HostEntry.Description;
                            }

                            return hostNode;

                        default:
                            return null;
                    }
                });
            }

            if (newProfile || this.treeView.SelectedNode == null)
            {
                // Nothing matched, jump to the current profile.
                this.treeView.SelectedNode = currentProfileNode;
            }

            // Add the defaults node at the end.
            if (this.app.Allowed(Restrictions.ModifyProfiles))
            {
                this.treeView.Nodes.Add(new ProfileTreeNode(Wx3270.ProfileManager.DefaultValuesName)
                {
                    IsDefaults = true,
                    Profile = Profile.DefaultProfile,
                    ContextMenuStrip = this.defaultsContextMenuStrip,
                    Name = Wx3270.ProfileManager.DefaultValuesName,
                });
            }

            // Do auto-rename after duplicating a host.
            var autoRenameNode = (TreeNode)null;
            if (this.autoRenamePath != null)
            {
                autoRenameNode = this.treeView.Nodes.Find(this.autoRenamePath, searchAllChildren: true).FirstOrDefault();
                if (autoRenameNode != null)
                {
                    this.treeView.SelectedNode = autoRenameNode;
                    this.selectedNode = autoRenameNode;
                }

                this.autoRenamePath = null;
            }

            // Do auto select.
            if (this.autoSelectPath != null)
            {
                var autoSelectNode = this.treeView.Nodes.Find(this.autoSelectPath, searchAllChildren: true).FirstOrDefault();
                if (autoSelectNode != null)
                {
                    this.treeView.SelectedNode = autoSelectNode;
                }

                this.autoSelectPath = null;
            }

            // Re-expand.
            this.TreeViewForEach((node) =>
            {
                if (node is ProfileTreeNode profileNode)
                {
                    if (profileNode.IsCurrent || (!newProfile && expanded.Contains(profileNode.Name)))
                    {
                        node.Expand();
                    }
                }
                else if (expanded.Contains(node.Name))
                {
                    node.Expand();
                }
            });

            if (this.treeView.SelectedNode != null)
            {
                this.treeView.SelectedNode.EnsureVisible();
            }

            this.treeView.EndUpdate();

            // You can't start the editing operation for a node inside the BeginUpdate/EndUpdate pair.
            if (autoRenameNode != null)
            {
                autoRenameNode.BeginEdit();
            }
        }

        /// <summary>
        /// Hide this window, without pushing our parent window to the back.
        /// </summary>
        private void SafeHide()
        {
            this.Hide();
            if (this.Owner != null)
            {
                this.Owner.BringToFront();
            }
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.SafeHide();
        }

        /// <summary>
        /// Connection change handler.
        /// </summary>
        private void HostConnectionChange()
        {
            var connectionState = this.app.ConnectionState;
            var connected = connectionState != ConnectionState.NotConnected;
            if (this.connected == connected)
            {
                return;
            }

            this.connected = connected;

            // Set up UI elements (top).
            this.topNewConnectionButton.Enabled = !connected;
            this.topDisconnectButton.Enabled = connected;
            this.topDisconnectButton.Invalidate(); // workaround for strange behavior when New Connection button has been deleted

            // Set up UI elements (bottom).
            ProfileTreeNode profileNode;
            var isFolder = this.treeView.SelectedNode is FolderTreeNode;
            var isHost = this.treeView.SelectedNode is HostTreeNode;
            var isProfile = (profileNode = this.treeView.SelectedNode as ProfileTreeNode) != null;
            var canSwitchTo = !connected
                && isProfile
                && !profileNode.IsBroken
                && !profileNode.IsDefaults;
            this.profileSwitchToButton.Enabled = isProfile && !profileNode.IsBroken && !profileNode.IsDefaults;
            this.profileMergeFromButton.Enabled = !connected && isProfile && !profileNode.IsBroken && !profileNode.IsCurrent;
            this.connectionNewButton.Enabled = !connected;
            this.commonEditButton.Enabled =
                (isHost && this.app.Allowed(Restrictions.ModifyHost))
                || (isProfile && this.app.Allowed(Restrictions.SwitchProfile) && !profileNode.IsBroken && !profileNode.IsDefaults);

            // Set up context menus.
            this.profileMergeFromToolStripMenuItem.Enabled = !connected;
            this.brokenProfileDeleteToolStripMenuItem.Enabled = !connected;

            // Underline the connected host.
            this.TreeChanged(this.app.ProfileTracker.Tree);
        }

        /// <summary>
        /// The tree view selection changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = this.treeView.SelectedNode;
            if (node == null)
            {
                return;
            }

            // Remember the selected node, to validate edit operations later.
            this.selectedNode = node;

            // Enable or disable various buttons.
            if (node is ProfileTreeNode)
            {
                // Profile node is active. Disable connection-specific buttons.
                this.connectionConnectButton.Enabled = false;

                // Set profile-specific buttons.
                var profileNode = node as ProfileTreeNode;
                this.profileSwitchToButton.Enabled = !this.connected && !profileNode.IsBroken && !profileNode.IsDefaults && !profileNode.IsCurrent;
                this.profileMergeFromButton.Enabled = !this.connected && !profileNode.IsBroken && !profileNode.IsCurrent;
                this.profileExportButton.Enabled = !profileNode.IsBroken && !profileNode.IsDefaults;
                this.profileDefaultButton.Enabled = !profileNode.IsBroken && !profileNode.IsDefaults;

                // Set common buttons.
                this.commonEditButton.Enabled = this.app.Allowed(Restrictions.SwitchProfile) && !profileNode.IsDefaults && !profileNode.IsCurrent && !profileNode.IsBroken;
                this.commonDuplicateButton.Enabled = !profileNode.IsBroken;
                this.commonRenameButton.Enabled = !profileNode.IsDefaults && !profileNode.IsCurrent && !profileNode.IsBroken;
                this.commonDeleteButton.Enabled = !profileNode.IsDefaults && !profileNode.IsCurrent;
                this.commonShortcutButton.Enabled = true;

                // Set connection buttons.
                this.connectionNewButton.Enabled = profileNode.Profile?.ProfileType == ProfileType.Full;
                if (!this.connected)
                {
                    this.topNewConnectionButton.Enabled = profileNode.Profile?.ProfileType == ProfileType.Full;
                }

                // Set folder buttons.
                this.folderUnwatchButton.Enabled = false;
                return;
            }

            if (node is HostTreeNode)
            {
                var hostTreeNode = node as HostTreeNode;

                // Host node is active. Disable profile-specific buttons.
                this.profileSwitchToButton.Enabled = false;
                this.profileMergeFromButton.Enabled = false;
                this.profileExportButton.Enabled = false;
                this.profileDefaultButton.Enabled = false;

                // Set connection-specific buttons.
                this.connectionConnectButton.Enabled = !this.connected &&
                    (this.app.Allowed(Restrictions.SwitchProfile) || hostTreeNode.Profile == this.ProfileManager.Current);

                // Set common buttons.
                this.commonEditButton.Enabled = this.app.Allowed(Restrictions.ModifyHost);
                this.commonDuplicateButton.Enabled = this.app.Allowed(Restrictions.ModifyHost);
                this.commonRenameButton.Enabled = this.app.Allowed(Restrictions.ModifyHost);
                this.commonDeleteButton.Enabled = this.app.Allowed(Restrictions.ModifyHost);
                this.commonShortcutButton.Enabled = true;

                // Set folder buttons.
                this.folderUnwatchButton.Enabled = false;

                // Set context menu items.
                this.hostConnectToolStripMenuItem.Enabled = !this.connected &&
                    (hostTreeNode.Profile == this.ProfileManager.Current || this.app.Allowed(Restrictions.SwitchProfile));

                return;
            }

            if (node is FolderTreeNode)
            {
                var folderTreeNode = node as FolderTreeNode;

                // Directory node is active. Everything off.
                this.profileSwitchToButton.Enabled = false;
                this.profileMergeFromButton.Enabled = false;
                this.profileExportButton.Enabled = false;
                this.profileDefaultButton.Enabled = false;
                this.connectionConnectButton.Enabled = false;
                this.commonEditButton.Enabled = false;
                this.commonDuplicateButton.Enabled = false;
                this.commonRenameButton.Enabled = false;
                this.commonDeleteButton.Enabled = node.Parent != null;
                this.commonShortcutButton.Enabled = false;

                this.folderUnwatchButton.Enabled =
                    folderTreeNode.Parent == null
                    && !folderTreeNode.IsDefault
                    && !this.ProfileManager.Current.DisplayFolder.Equals(node.Text, StringComparison.InvariantCultureIgnoreCase);
                return;
            }
        }

        /// <summary>
        /// Mouse button click on the tree view.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.mouseDownPoint = e.Location;
                return;
            }

            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var node = this.treeView.GetNodeAt(e.Location);
            if (node == null)
            {
                return;
            }

            this.rightClickNode = node;

            var profileTreeNode = node as ProfileTreeNode;
            if (profileTreeNode != null)
            {
                if (profileTreeNode.IsBroken || profileTreeNode.IsDefaults)
                {
                    // Not a profile node, or not anything that changes.
                    return;
                }

                // There are lots of things that make no sense from the current profile.
                this.profileDeleteToolStripMenuItem.Enabled = !profileTreeNode.IsCurrent;
                this.profileEditToolStripMenuItem.Enabled = !profileTreeNode.IsCurrent;
                this.profileRenameToolStripMenuItem.Enabled = !profileTreeNode.IsCurrent;
                this.profileMergeFromToolStripMenuItem.Enabled = !profileTreeNode.IsCurrent && !this.connected;

                return;
            }

            var folderTreeNode = node as FolderTreeNode;
            if (folderTreeNode != null)
            {
                this.folderStopWatchingToolStripMenuItem.Enabled =
                    node.Parent == null
                    && !folderTreeNode.IsDefault
                    && !this.ProfileManager.Current.DisplayFolder.Equals(node.Text, StringComparison.InvariantCultureIgnoreCase);
                this.folderDeleteToolStripMenuItem.Enabled = node.Parent != null;
            }
        }

        /// <summary>
        /// The mouse moved over the tree view.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.app.Restricted(Restrictions.ModifyProfiles))
            {
                return;
            }

            if (e.Button.HasFlag(MouseButtons.Left)
                && this.mouseDownPoint.HasValue
                && (Math.Abs(e.X - this.mouseDownPoint.Value.X) >= SystemInformation.DoubleClickSize.Width
                    || Math.Abs(e.Y - this.mouseDownPoint.Value.Y) >= SystemInformation.DoubleClickSize.Height))
            {
                var moveNode = this.treeView.GetNodeAt(this.mouseDownPoint.Value);
                var profileNode = moveNode as ProfileTreeNode;
                if (moveNode is HostTreeNode || (profileNode != null && !profileNode.IsBroken))
                {
                    this.treeView.DoDragDrop(moveNode.Name, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// Drag-over method for the tree view.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            // Assume it is not droppable.
            e.Effect = DragDropEffects.None;

            // Get the source node.
            if (!e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                return;
            }

            var sourcePath = (string)e.Data.GetData(DataFormats.Text);
            var sourceParts = this.PathSplit(sourcePath);
            var sourceNode = this.treeView.Nodes.Find(sourcePath, true).FirstOrDefault();
            if (sourceNode == null)
            {
                return;
            }

            // Get the destination node.
            var point = this.treeView.PointToClient(new Point(e.X, e.Y));
            var destNode = this.treeView.GetNodeAt(point);
            if (destNode == null)
            {
                return;
            }

            var destPath = destNode.Name;
            var destParts = this.PathSplit(destPath);

            // Make the destination node current.
            this.treeView.SelectedNode = destNode;
            this.treeView.SelectedNode.EnsureVisible();

            // Figure out dropability.
            if (sourceNode is HostTreeNode)
            {
                // Destination is a profile.
                if (destNode is ProfileTreeNode destProfile)
                {
                    if (!(destProfile.IsBroken || destProfile.IsDefaults || destParts.Take(2).SequenceEqual(sourceParts.Take(2)) || destProfile.Profile.ProfileType != ProfileType.Full))
                    {
                        // Copy to a different profile.
                        e.Effect = DragDropEffects.Copy;
                    }
                }
                else
                {
                    // Destination is a host.
                    if (!sourcePath.Equals(destPath))
                    {
                        if (destParts.Take(2).SequenceEqual(sourceParts.Take(2)))
                        {
                            // Different host within same profile.
                            e.Effect = DragDropEffects.Move;
                        }
                        else
                        {
                            // Host within different profile.
                            e.Effect = DragDropEffects.Copy;
                        }
                    }
                }
            }
            else if (sourceNode is ProfileTreeNode
                && destNode is FolderTreeNode
                && !destNode.Name.Equals(this.PathSplit(sourceNode.Name)[0]))
            {
                // Destination is a different folder.
                e.Effect = DragDropEffects.Copy;
            }
            else if (sourceNode is ProfileTreeNode
                && destNode is ProfileTreeNode
                && !(destNode as ProfileTreeNode).IsBroken
                && !(destNode as ProfileTreeNode).IsDefaults
                && !sourceNode.Name.Equals(destNode.Name)
                && CompatibleMergeType((sourceNode as ProfileTreeNode).Profile, (destNode as ProfileTreeNode).Profile) != null)
            {
                // Destination is a different profile (merge).
                e.Effect = DragDropEffects.Copy;
            }

            // Scroll the tree view, if necessary.
            if (point.Y < this.treeView.Font.Height / 3 && destNode.PrevVisibleNode != null)
            {
                destNode.PrevVisibleNode.EnsureVisible();
            }

            // This test needs to be against treeView.Font.Height instead of Height/3 because
            // any lower than that and (apparently) we find the hidden next node above with
            // GetNodeAt before this can kick in. I wish I understood this interaction better.
            //
            // At least this allows a drop onto the top and bottom visible nodes without having
            // auto-scroll kick in.
            if (point.Y > this.treeView.Height - this.treeView.Font.Height && destNode.NextVisibleNode != null)
            {
                destNode.NextVisibleNode.EnsureVisible();
            }
        }

        /// <summary>
        /// Drag-drop method for the tree view.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            // Get the source node.
            if (!e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                return;
            }

            var sourcePath = (string)e.Data.GetData(DataFormats.Text);
            var sourceTreeNode = this.treeView.Nodes.Find(sourcePath, true).FirstOrDefault();

            // Get the destination node.
            var point = this.treeView.PointToClient(new Point(e.X, e.Y));
            var destNode = this.treeView.GetNodeAt(point);
            if (destNode == null)
            {
                return;
            }

            if (sourceTreeNode is HostTreeNode)
            {
                this.DragDropDone(sourceTreeNode as HostTreeNode, destNode, e);
            }
            else if (sourceTreeNode is ProfileTreeNode)
            {
                this.DragDropDone(sourceTreeNode as ProfileTreeNode, destNode, e);
            }
        }

        /// <summary>
        /// Drag-drop method for host nodes.
        /// </summary>
        /// <param name="sourceNode">Source node.</param>
        /// <param name="destNode">Destination node.</param>
        /// <param name="e">Event arguments.</param>
        private void DragDropDone(HostTreeNode sourceNode, TreeNode destNode, DragEventArgs e)
        {
            var sourcePath = sourceNode.Name;
            var sourceParts = this.PathSplit(sourcePath);
            var sourceHostEntry = sourceNode.Profile.Hosts.FirstOrDefault(h => h.Name.Equals(sourceParts[2]));
            if (sourceHostEntry == null)
            {
                return;
            }

            // Get the destination node.
            var destPath = destNode.Name;
            var destParts = this.PathSplit(destPath);

            // Figure out dropability.
            if (destNode is ProfileTreeNode)
            {
                // Destination is a profile.
                var destProfile = (ProfileTreeNode)destNode;
                if (destProfile.IsBroken
                    || destProfile.IsDefaults
                    || destParts.Take(2).SequenceEqual(sourceParts.Take(2)))
                {
                    return;
                }

                // Copy to a different profile. Add to the end.
                var baseName = sourceParts[2];
                var newProfileName = baseName;
                var n = 0;
                while (destProfile.Profile.Hosts.Any(h => h.Name.Equals(newProfileName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    newProfileName = baseName + "(" + ++n + ")";
                }

                // Add the entry.
                var newDestPath = this.PathCombine(destParts[0], destParts[1], newProfileName);
                if (n > 0)
                {
                    this.autoRenamePath = newDestPath;
                }
                else
                {
                    this.autoSelectPath = newDestPath;
                }

                var newEntry = sourceHostEntry.Clone();
                newEntry.Name = newProfileName;
                newEntry.Profile = destProfile.Profile;
                var refocus = new ProfileRefocus(
                    this.ProfileManager,
                    Separator,
                    sourcePath,
                    newDestPath);
                try
                {
                    this.ProfileManager.PushAndSave(
                        (current) =>
                        {
                            current.Hosts = current.Hosts.Concat(new[] { newEntry }).ToArray();
                        },
                        string.Format(I18n.Get(Message.AddConnection), newProfileName),
                        destProfile.Profile,
                        refocus);
                }
                catch (InvalidOperationException ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.CopyConnection));
                    return;
                }
            }
            else
            {
                var destHost = (HostTreeNode)destNode;

                // Destination is a host.
                if (!sourcePath.Equals(destPath))
                {
                    if (destParts.Take(2).SequenceEqual(sourceParts.Take(2)))
                    {
                        // Different host within same profile. This is a move operation.
                        var hostNameList = destHost.Profile.Hosts.Select(h => h.Name).ToList();
                        var sourceIndex = hostNameList.IndexOf(sourceParts[2]);
                        var destIndex = hostNameList.IndexOf(destParts[2]);
                        var hostList = destHost.Profile.Hosts.ToList();
                        var sourceEntry = hostList[sourceIndex];

                        if (sourceIndex > destIndex)
                        {
                            // Move up, so move before.
                            hostList.RemoveAt(sourceIndex);
                            hostList.Insert(destIndex, sourceEntry);
                        }
                        else
                        {
                            // Move down, so move after.
                            hostList.Insert(destIndex + 1, sourceEntry);
                            hostList.RemoveAt(sourceIndex);
                        }

                        this.autoSelectPath = sourcePath;
                        var refocus = new ProfileRefocus(
                            this.ProfileManager,
                            Separator,
                            sourcePath,
                            sourcePath);
                        try
                        {
                            this.ProfileManager.PushAndSave(
                                (current) =>
                                {
                                    current.Hosts = hostList;
                                },
                                string.Format(I18n.Get(Message.MoveConnection), sourceParts[2]),
                                destHost.Profile,
                                refocus);
                        }
                        catch (InvalidOperationException ex)
                        {
                            ErrorBox.Show(ex.Message, I18n.Get(Title.MoveConnection));
                            return;
                        }
                    }
                    else
                    {
                        // Host within different profile. Copy to before that entry.
                        var baseName = sourceParts[2];
                        var newProfileName = baseName;
                        var n = 0;
                        while (destHost.Profile.Hosts.Any(h => h.Name.Equals(newProfileName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            newProfileName = baseName + "(" + ++n + ")";
                        }

                        var newDestPath = this.PathCombine(destParts[0], destParts[1], newProfileName);
                        if (n > 0)
                        {
                            this.autoRenamePath = newDestPath;
                        }
                        else
                        {
                            this.autoSelectPath = newDestPath;
                        }

                        var newEntry = sourceHostEntry.Clone();
                        newEntry.Name = newProfileName;
                        newEntry.Profile = destHost.Profile;
                        var refocus = new ProfileRefocus(
                            this.ProfileManager,
                            Separator,
                            sourcePath,
                            newDestPath);
                        try
                        {
                            this.ProfileManager.PushAndSave(
                                (current) =>
                                {
                                    current.Hosts = current.Hosts
                                        .TakeWhile(h => !h.Name.Equals(destParts[2]))
                                        .Concat(new[] { newEntry })
                                        .Concat(current.Hosts.SkipWhile(h => !h.Name.Equals(destParts[2])))
                                        .ToArray();
                                },
                                string.Format(I18n.Get(Message.AddConnection), newProfileName),
                                destHost.Profile,
                                refocus);
                        }
                        catch (InvalidOperationException ex)
                        {
                            ErrorBox.Show(ex.Message, I18n.Get(Title.CopyConnection));
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drag-drop method for profile nodes.
        /// </summary>
        /// <param name="sourceNode">Source node.</param>
        /// <param name="destNode">Destination node.</param>
        /// <param name="e">Event arguments.</param>
        private void DragDropDone(ProfileTreeNode sourceNode, TreeNode destNode, DragEventArgs e)
        {
            var folderNode = destNode as FolderTreeNode;
            if (folderNode != null)
            {
                // Target is a different folder.
                if (sourceNode.IsDefaults)
                {
                    // Create a new profile in the specified folder.
                    this.NewProfile(folderNode);
                    return;
                }

                // Copy a profile from one folder to another.
                var sourceParts = this.PathSplit(sourceNode.Name);
                var baseName = sourceParts[1];
                var newName = baseName;
                var n = 2;
                string destName;
                while (File.Exists(destName = Path.Combine(folderNode.FolderName, newName + Wx3270.ProfileManager.Suffix)))
                {
                    newName = baseName + "(" + n++ + ")";
                }

                this.autoSelectPath = this.PathCombine(destNode.Name, newName);
                try
                {
                    File.Copy(sourceNode.Profile.PathName, destName);
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.CopyProfile));
                }

                // Create an undo/redo record for it.
                this.ProfileManager.PushConfigAction(
                    new CopyProfileConfigAction(
                        string.Format(I18n.Get(Message.CopyProfile), baseName, sourceNode.Profile.DisplayFolder, DirNodeName(folderNode.FolderName)),
                        baseName,
                        sourceNode.Profile.PathName,
                        destName,
                        this.ProfileManager));

                return;
            }

            if (destNode is ProfileTreeNode destProfileNode)
            {
                // Merge a profile.
                if (this.connected && this.ProfileManager.IsCurrentPathName(destProfileNode.Profile.PathName))
                {
                    // But not the current profile, if connected.
                    return;
                }

                this.MergeFromProfile(destProfileNode.Profile, sourceNode.Profile);
            }
        }

        /// <summary>
        /// A node is about to be edited.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node is FolderTreeNode || (e.Node != this.rightClickNode && e.Node != this.selectedNode))
            {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// The connect tree form was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectTree_Activated(object sender, EventArgs e)
        {
            if (!this.everActivated)
            {
                this.everActivated = true;
                this.Location = MainScreen.CenteredOn(this.mainScreen, this);
            }

            this.treeView.Focus();
        }

        /// <summary>
        /// The connect tree form was deactivated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectTree_Deactivate(object sender, EventArgs e)
        {
            // Do not allow label editing when we are re-activated.
            // This works around a TreeView bug that causes a label edit operation when the (invisibly) selected node
            // in an inactive window is clicked on to make the window active.
            this.rightClickNode = null;
            this.selectedNode = null;
        }

        /// <summary>
        /// One of the top buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TopButtonClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            switch ((string)button.Tag)
            {
                case "NewConnection":
                    this.CreateHost();
                    break;
                case "Disconnect":
                    this.connect.Disconnect();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// One of the profile buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProfileButtonClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            var selectedProfileNode = this.treeView.SelectedNode as ProfileTreeNode;

            // Give the focus back to the tree view.
            this.treeView.Focus();

            switch ((string)button.Tag)
            {
                case "New":
                    // Create a new profile from defaults.
                    this.DuplicateProfile(Profile.DefaultProfile);
                    break;
                case "SwitchTo":
                    // Switch profiles, and auto-connect if defined.
                    if (selectedProfileNode == null)
                    {
                        return;
                    }

                    if (this.connected)
                    {
                        NewWindow(this, this.components, selectedProfileNode.Profile.PathName, editMode: true);
                    }
                    else
                    {
                        this.LoadWithAutoConnect(selectedProfileNode.Profile.PathName, ModifierKeys.HasFlag(Keys.Shift));
                    }

                    break;
                case "MergeFrom":
                    // Merge from a profile.
                    if (selectedProfileNode == null)
                    {
                        return;
                    }

                    this.MergeFromProfile(this.ProfileManager.Current, selectedProfileNode.Profile);
                    break;
                case "Import":
                    // Import a profile from an external folder.
                    this.ImportProfile(Path.GetDirectoryName(this.ProfileManager.Current.PathName));
                    break;
                case "Export":
                    // Export a profile to an external folder.
                    if (selectedProfileNode == null)
                    {
                        return;
                    }

                    this.ExportProfile(selectedProfileNode.Profile);
                    break;
                case "Default":
                    // Set this folder as the default.
                    if (selectedProfileNode == null)
                    {
                        return;
                    }

                    this.ProfileManager.SetDefaultProfile(selectedProfileNode.Profile);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// One of the common buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CommonButtonClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            // Give the focus back to the tree view.
            this.treeView.Focus();

            var selectedHostNode = this.treeView.SelectedNode as HostTreeNode;
            var selectedProfileNode = this.treeView.SelectedNode as ProfileTreeNode;
            var selectedFolderNode = this.treeView.SelectedNode as FolderTreeNode;
            if (selectedHostNode == null && selectedProfileNode == null && selectedFolderNode == null)
            {
                return;
            }

            switch ((string)button.Tag)
            {
                case "Edit":
                    if (selectedHostNode != null && this.app.Allowed(Restrictions.ModifyHost))
                    {
                        this.EditHost(selectedHostNode);
                    }
                    else if (selectedProfileNode != null)
                    {
                        if (this.connected)
                        {
                            // Open the profile in a new window with auto-connect, read/write.
                            NewWindow(this, this.components, selectedProfileNode.Profile.PathName, editMode: true);
                        }
                        else if (this.app.Allowed(Restrictions.SwitchProfile))
                        {
                            // Switch profiles without auto-connect.
                            this.LoadWithoutAutoConnect(selectedProfileNode.Profile.PathName, ModifierKeys.HasFlag(Keys.Shift));
                        }
                    }

                    break;
                case "Duplicate":
                    if (selectedHostNode != null)
                    {
                        this.DuplicateHost(selectedHostNode);
                    }
                    else if (selectedProfileNode != null)
                    {
                        this.DuplicateProfile(selectedProfileNode.Profile);
                    }

                    break;
                case "Rename":
                    this.treeView.SelectedNode.BeginEdit();
                    break;
                case "Shortcut":
                    // Create a shortcut.
                    if (selectedHostNode != null)
                    {
                        this.CreateShortcut(selectedHostNode.Profile, selectedHostNode.Text);
                    }
                    else
                    {
                        this.CreateShortcut(selectedProfileNode.Profile);
                    }

                    break;
                case "Delete":
                    if (selectedHostNode != null)
                    {
                        this.DeleteHost(selectedHostNode);
                    }
                    else if (selectedProfileNode != null)
                    {
                        this.DeleteProfile(selectedProfileNode);
                    }
                    else if (selectedFolderNode != null)
                    {
                        this.DeleteFolder(selectedFolderNode);
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// One of the connection (host) buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectionButtonClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            var selectedHostNode = this.treeView.SelectedNode as HostTreeNode;

            // Give the focus back to the tree view.
            this.treeView.Focus();

            switch ((string)button.Tag)
            {
                case "Connect":
                    // Connect to a host.
                    if (selectedHostNode == null)
                    {
                        return;
                    }

                    if (this.connected)
                    {
                        NewWindow(this, this.components, selectedHostNode.Profile.PathName, selectedHostNode.Text);
                    }
                    else
                    {
                        this.ConnectToHost(selectedHostNode.Profile, selectedHostNode.Text, ModifierKeys.HasFlag(Keys.Shift));
                        this.SafeHide();
                    }

                    break;
                case "Edit":
                    // Edit a host.
                    // Connect to a host.
                    if (selectedHostNode == null)
                    {
                        return;
                    }

                    this.EditHost(selectedHostNode);
                    break;
                case "Duplicate":
                    // Duplicate a host.
                    if (selectedHostNode == null)
                    {
                        return;
                    }

                    this.DuplicateHost(selectedHostNode);
                    break;
                case "Rename":
                    // Rename a host.
                    if (selectedHostNode == null)
                    {
                        return;
                    }

                    selectedHostNode.BeginEdit();
                    break;
                case "New":
                    // Create a new host.
                    this.CreateHost();
                    break;
                case "Delete":
                    // Delete a host.
                    if (selectedHostNode == null)
                    {
                        return;
                    }

                    this.DeleteHost(selectedHostNode);
                    break;
                case "Disconnect":
                    // Disconnect from the host.
                    this.connect.Disconnect();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// A node name has been edited.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node is ProfileTreeNode)
            {
                var profileNode = (ProfileTreeNode)e.Node;
                if (string.IsNullOrEmpty(e.Label) || !Nickname.ValidNickname(e.Label))
                {
                    // Empty label.
                    e.CancelEdit = true;
                    e.Node.EndEdit(true);
                    return;
                }

                // Try the rename.
                this.autoSelectPath = this.PathCombine(profileNode.Profile.DisplayFolder, e.Label);
                try
                {
                    File.Move(profileNode.Profile.PathName, profileNode.Profile.MappedPath(e.Label));
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.RenameProfile));
                    e.CancelEdit = true;
                    e.Node.EndEdit(true);
                    return;
                }

                // Create an undo/redo record for it.
                this.ProfileManager.PushConfigAction(
                    new ProfileRenameConfigAction(
                        string.Format(I18n.Get(Message.RenameProfile), e.Node.Text, e.Label),
                        e.Label,
                        profileNode.Profile,
                        this.ProfileManager));
            }

            if (e.Node is HostTreeNode)
            {
                if (string.IsNullOrEmpty(e.Label) || e.Label.Equals(e.Node.Text))
                {
                    e.CancelEdit = true;
                    e.Node.EndEdit(true);
                    return;
                }

                var hostNode = (HostTreeNode)e.Node;
                if (hostNode.Profile.Hosts.Any(h => h.Name.Equals(e.Label)))
                {
                    ErrorBox.Show(I18n.Get(Message.NameAlreadyExists), I18n.Get(Title.RenameConnection), MessageBoxIcon.Warning);
                    e.CancelEdit = true;
                    e.Node.EndEdit(true);
                    return;
                }

                var newPath = this.PathCombine(hostNode.Profile.DisplayFolder, hostNode.Profile.Name, e.Label);
                var refocus = new ProfileRefocus(
                    this.ProfileManager,
                    Separator,
                    e.Node.Name,
                    newPath);
                try
                {
                    this.autoSelectPath = newPath;
                    this.ProfileManager.PushAndSave(
                        (current) =>
                        {
                            current.Hosts = current.Hosts.Select(h => h.Name.Equals(e.Node.Text) ? new HostEntry(h) { Name = e.Label } : h).ToArray();
                        },
                        string.Format(I18n.Get(Message.RenameConnection), hostNode.Text, e.Label),
                        hostNode.Profile,
                        refocus);
                }
                catch (InvalidOperationException ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.RenameConnection));
                    this.autoSelectPath = null;
                    e.CancelEdit = true;
                    e.Node.EndEdit(true);
                    return;
                }

                return;
            }

            Trace.Line(Trace.Type.Profile, "AfterLabelEdit: unknown node type");
        }

        /// <summary>
        /// Delete a profile.
        /// </summary>
        /// <param name="profileNode">Profile to delete.</param>
        private void DeleteProfile(ProfileTreeNode profileNode)
        {
            var nextNode = profileNode.NextNode;
            string nextNodeName = null;
            if (nextNode != null && nextNode is ProfileTreeNode)
            {
                this.autoSelectPath = nextNode.Name;
                nextNodeName = nextNode.Text;
            }
            else
            {
                nextNode = profileNode.PrevNode;
                if (nextNode != null)
                {
                    if (nextNode is ProfileTreeNode)
                    {
                        this.autoSelectPath = nextNode.Name;
                        nextNodeName = nextNode.Text;
                    }
                }
                else if (nextNode is FolderTreeNode)
                {
                    this.autoSelectPath = nextNode.Name;
                }
            }

            try
            {
                File.Delete(profileNode.Profile.PathName);
            }
            catch (Exception ex)
            {
                ErrorBox.Show(ex.Message, I18n.Get(Title.DeleteProfile));
                return;
            }

            // Create an Undo record for it.
            if (!profileNode.IsBroken)
            {
                this.ProfileManager.PushConfigAction(
                    new ProfileDeleteConfigAction(
                        string.Format(I18n.Get(Message.DeleteProfile), profileNode.Profile.Name),
                        profileNode.Profile,
                        nextNodeName,
                        this.ProfileManager));
            }
        }

        /// <summary>
        /// Delete a sub-folder.
        /// </summary>
        /// <param name="folderNode">Folder to delete.</param>
        private void DeleteFolder(FolderTreeNode folderNode)
        {
            try
            {
                Directory.Delete(folderNode.FolderName);
                this.autoSelectPath = folderNode.Parent.Name;
            }
            catch (Exception ex)
            {
                ErrorBox.Show(ex.Message, I18n.Get(Title.DeleteFolder));
                this.autoSelectPath = folderNode.Name;
                return;
            }

            // Create an Undo record for it.
            this.ProfileManager.PushConfigAction(
                new FolderDeleteConfigAction(
                    string.Format(I18n.Get(Message.DeleteFolder), folderNode.FolderName),
                    folderNode.FolderName,
                    folderNode.Name,
                    folderNode.Parent.Name,
                    this.ProfileManager));
        }

        /// <summary>
        /// Duplicate a profile in the same folder.
        /// </summary>
        /// <param name="profile">Profile to duplicate.</param>
        /// <param name="profileType">Profile type.</param>
        private void DuplicateProfile(Profile profile, ProfileType profileType = ProfileType.Full)
        {
            var typeDict = new Dictionary<ProfileType, string>
            {
                { ProfileType.Full, Message.NewProfile },
                { ProfileType.KeyboardMapTemplate, Message.NewKeymapTemplate },
                { ProfileType.KeypadMapTemplate, Message.NewKeypadTemplate },
            };

            // Re-map the type if the profile is something other than defaults.
            var from = profile.Name;
            var isDefaults = from.Equals(Wx3270.ProfileManager.DefaultValuesName);
            if (!isDefaults && profileType == ProfileType.Full)
            {
                profileType = profile.ProfileType;
            }

            // Find a unique new name.
            string baseName = isDefaults ? I18n.Get(typeDict[profileType]) : from + " - " + I18n.Get(Message.Copy);
            var newName = baseName;
            var n = 2;
            while (File.Exists(profile.MappedPath(newName)))
            {
                newName = baseName + " (" + n++ + ")";
            }

            // Copy the file.
            if (isDefaults)
            {
                this.autoRenamePath = this.PathCombine(DefaultDirNodeName, newName);

                // For non-full profiles, clear out the corresponding config item.
                Profile saveProfile = Profile.DefaultProfile;
                if (profileType != ProfileType.Full)
                {
                    saveProfile = saveProfile.Clone();
                    saveProfile.ProfileType = profileType;
                    switch (profileType)
                    {
                        case ProfileType.KeyboardMapTemplate:
                            saveProfile.KeyboardMap = new KeyMap<KeyboardMap>();
                            break;
                        case ProfileType.KeypadMapTemplate:
                            saveProfile.KeypadMap = new KeyMap<KeypadMap>();
                            break;
                        default:
                            break;
                    }
                }

                if (!this.ProfileManager.Save(profile.MappedPath(newName), saveProfile))
                {
                    ErrorBox.Show(I18n.Get(Message.ProfileSaveFailed), I18n.Get(Title.DuplicateProfile));
                    this.autoRenamePath = null;
                    return;
                }
            }
            else
            {
                this.autoRenamePath = this.PathCombine(profile.DisplayFolder, newName);
                try
                {
                    File.Copy(profile.PathName, profile.MappedPath(newName));
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.DuplicateProfile));
                    this.autoRenamePath = null;
                    return;
                }
            }

            // Set up undo/redo.
            this.ProfileManager.PushConfigAction(
                new ProfileDuplicateConfigAction(
                    string.Format(I18n.Get(Message.DuplicateProfile), profile.Name, newName),
                    profile,
                    newName,
                    this.ProfileManager));
        }

        /// <summary>
        /// Create a new profile in a specific folder.
        /// </summary>
        /// <param name="folderTreeNode">Folder tree node.</param>
        private void NewProfile(FolderTreeNode folderTreeNode)
        {
            // Find a unique new name.
            string baseName = I18n.Get(Message.NewProfile);
            var newName = baseName;
            var n = 2;
            var newPath = Path.Combine(folderTreeNode.FolderName, newName + Wx3270.ProfileManager.Suffix);
            while (File.Exists(newPath))
            {
                newName = baseName + " (" + n++ + ")";
                newPath = Path.Combine(folderTreeNode.FolderName, newName + Wx3270.ProfileManager.Suffix);
            }

            // Copy defaults into the file.
            var folderDisplayName = DirNodeName(folderTreeNode.FolderName);
            this.autoRenamePath = this.PathCombine(folderDisplayName, newName);
            if (!this.ProfileManager.Save(newPath, Profile.DefaultProfile))
            {
                ErrorBox.Show(I18n.Get(Message.ProfileSaveFailed), I18n.Get(Title.CreateProfile));
                this.autoRenamePath = null;
                return;
            }

            // Set up undo/redo.
            this.ProfileManager.PushConfigAction(
                new NewProfileConfigAction(
                    string.Format(I18n.Get(Message.CreateProfile), newName, folderDisplayName),
                    newName,
                    newPath,
                    this.ProfileManager));
        }

        /// <summary>
        /// Merge from a profile.
        /// </summary>
        /// <param name="destProfile">Destination profile.</param>
        /// <param name="mergeProfile">Profile to merge from.</param>
        private void MergeFromProfile(Profile destProfile, Profile mergeProfile)
        {
            // Figure out the merge restrictions.
            var profileType = CompatibleMergeType(destProfile, mergeProfile);
            if (profileType == null)
            {
                return;
            }

            using (var mergeDialog = new MergeDialog(this.app, this, mergeProfile.Name, destProfile.Name, profileType.Value))
            {
                if (mergeDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var imports = mergeDialog.Imports;
                if (imports == ImportType.None)
                {
                    return;
                }

                if (mergeProfile.Name.Equals(Wx3270.ProfileManager.DefaultValuesName))
                {
                    this.ProfileManager.Merge(destProfile, Profile.DefaultProfile, imports);
                }
                else
                {
                    this.app.ProfileManager.Merge(destProfile, mergeProfile.PathName, imports);
                }
            }
        }

        /// <summary>
        /// Export a profile.
        /// </summary>
        /// <param name="profile">Profile to export.</param>
        private void ExportProfile(Profile profile)
        {
            this.exportProfileDialog.FileName = profile.Name + Wx3270.ProfileManager.Suffix;
            this.exportProfileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            switch (this.exportProfileDialog.ShowDialog())
            {
                default:
                case DialogResult.Cancel:
                    return;
                case DialogResult.OK:
                    break;
            }

            // Save the profile.
            this.ProfileManager.Save(this.exportProfileDialog.FileName, profile);
        }

        /// <summary>
        /// Import a profile.
        /// </summary>
        /// <param name="destFolderPath">Folder path.</param>
        private void ImportProfile(string destFolderPath)
        {
            this.importFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            this.importFileDialog.FileName = string.Empty;
            this.importFileDialog.Title = string.Format(I18n.Get(Title.ImportProfileDialog), DirNodeName(destFolderPath));
            var result = this.importFileDialog.ShowDialog();
            switch (result)
            {
                default:
                case DialogResult.Cancel:
                    return;
                case DialogResult.OK:
                    break;
            }

            var importName = this.importFileDialog.FileName;

            // Try to avoid a name conflict.
            string baseName = Path.GetFileNameWithoutExtension(importName);
            var newName = baseName;
            var n = 0;
            var destProfilePath = Path.Combine(destFolderPath, newName + Wx3270.ProfileManager.Suffix);
            while (File.Exists(destProfilePath))
            {
                newName = baseName + "(" + ++n + ")";
                destProfilePath = Path.Combine(destFolderPath, newName + Wx3270.ProfileManager.Suffix);
            }

            // Set up auto-rename.
            this.autoRenamePath = this.PathCombine(DirNodeName(destFolderPath), newName);

            if (Path.GetExtension(importName).Equals(Wx3270.ProfileManager.Suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                // Simple copy of wx3270 profile.
                try
                {
                    File.Copy(importName, destProfilePath);
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.ImportProfile));
                    this.autoRenamePath = null;
                    return;
                }

                // Set up undo/redo.
                this.ProfileManager.PushConfigAction(
                    new ProfileImportConfigAction(
                        string.Format(I18n.Get(Message.ImportProfile), Path.GetFileNameWithoutExtension(importName)),
                        importName,
                        newName,
                        destFolderPath,
                        this.ProfileManager,
                        this.app));

                return;
            }

            if (Path.GetExtension(importName).Equals(Wc3270Import.Suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                // Import of wc3270 profile.
                try
                {
                    var import = new Wc3270Import(this.app.CodePageDb);
                    import.Read(importName);
                    this.ProfileManager.Save(destProfilePath, import.Digest());
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(Title.ImportProfile));
                    this.autoRenamePath = null;
                    return;
                }

                // Set up undo/redo.
                this.ProfileManager.PushConfigAction(
                    new ProfileImportConfigAction(
                        string.Format(I18n.Get(Message.ImportProfile), Path.GetFileNameWithoutExtension(importName)),
                        importName,
                        newName,
                        destFolderPath,
                        this.ProfileManager,
                        this.app));

                return;
            }

            this.autoRenamePath = null;
            ErrorBox.Show(I18n.Get(Message.InvalidFileType), I18n.Get(Title.ImportProfile));
        }

        /// <summary>
        /// Create a shortcut.
        /// </summary>
        /// <param name="profile">Profile object.</param>
        /// <param name="host">Host name.</param>
        private void CreateShortcut(Profile profile, string host = null)
        {
            this.shortcutDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            this.shortcutDialog.FileName = (host ?? profile.Name) + ".lnk";
            switch (this.shortcutDialog.ShowDialog(this))
            {
                case DialogResult.OK:
                    break;
                default:
                    return;
            }

            var args = new List<string>
            {
                Constants.Option.Profile,
                "\"" + profile.PathName + "\"",
            };
            if (host != null)
            {
                args.Add(Constants.Option.Host);
                args.Add("\"" + host + "\"");
            }

            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(this.shortcutDialog.FileName);
            shortcut.Description = "wx3270 shortcut";
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.Arguments = string.Join(" ", args);
            shortcut.IconLocation = Application.ExecutablePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            shortcut.Save();
        }

        /// <summary>
        /// Duplicate a host.
        /// </summary>
        /// <param name="hostNode">Host node.</param>
        private void DuplicateHost(HostTreeNode hostNode)
        {
            // Find a unique new name.
            var from = hostNode.Text;
            string baseName = from + " - " + I18n.Get(Message.Copy);
            var newName = baseName;
            var n = 2;
            while (hostNode.Profile.Hosts.Any(h => h.Name.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
            {
                newName = baseName + " (" + n++ + ")";
            }

            // Set up auto-rename.
            var newPath = this.PathCombine(hostNode.Profile.DisplayFolder, hostNode.Profile.Name, newName);
            this.autoRenamePath = newPath;

            // Make the change.
            var refocus = new ProfileRefocus(
                this.ProfileManager,
                Separator,
                this.PathCombine(hostNode.Profile.DisplayFolder, hostNode.Profile.Name, hostNode.Text),
                newPath);
            try
            {
                this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        var existingEntry = hostNode.Profile.Hosts.FirstOrDefault(h => h.Name.Equals(hostNode.Text));
                        current.Hosts = hostNode.Profile.Hosts.Concat(new[] { new HostEntry(existingEntry) { Name = newName } }).ToArray();
                    },
                    string.Format(I18n.Get(Message.DuplicateConnection), hostNode.Text),
                    hostNode.Profile,
                    refocus);
            }
            catch (InvalidOperationException ex)
            {
                ErrorBox.Show(ex.Message, I18n.Get(Title.DuplicateConnection));
                return;
            }
        }

        /// <summary>
        /// Create a new host.
        /// </summary>
        private void CreateHost()
        {
            // Figure out which profile to add it to.
            Profile profile;
            if (this.treeView.SelectedNode is ProfileTreeNode)
            {
                // Profile node.
                var profileNode = (ProfileTreeNode)this.treeView.SelectedNode;
                if (profileNode.IsBroken || profileNode.IsDefaults)
                {
                    profile = this.ProfileManager.Current;
                }
                else
                {
                    profile = profileNode.Profile;
                }
            }
            else if (this.treeView.SelectedNode is HostTreeNode)
            {
                // Host node.
                profile = ((ProfileTreeNode)this.treeView.SelectedNode.Parent).Profile;
            }
            else
            {
                profile = this.ProfileManager.Current;
            }

            this.CreateHostDialog(profile);
        }

        /// <summary>
        /// Delete a host from a profile.
        /// </summary>
        /// <param name="hostTreeNode">Host tree node.</param>
        private void DeleteHost(HostTreeNode hostTreeNode)
        {
            var profile = hostTreeNode.Profile;
            var hostName = hostTreeNode.Text;
            string redoPath = this.PathCombine(profile.DisplayFolder, profile.Name);
            var nextNode = hostTreeNode.NextNode;
            if (nextNode != null && nextNode is HostTreeNode)
            {
                redoPath = this.PathCombine(profile.DisplayFolder, profile.Name, nextNode.Text);
            }
            else
            {
                nextNode = hostTreeNode.PrevNode;
                if (nextNode != null && nextNode is HostTreeNode)
                {
                    redoPath = this.PathCombine(profile.DisplayFolder, profile.Name, nextNode.Text);
                }
            }

            var refocus = new ProfileRefocus(
                this.ProfileManager,
                Separator,
                this.PathCombine(profile.DisplayFolder, profile.Name, hostName),
                redoPath);
            this.autoSelectPath = redoPath;
            try
            {
                this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        current.Hosts = current.Hosts.Where(h => !h.Name.Equals(hostName)).ToArray();
                    },
                    string.Format(I18n.Get(Message.DeleteConnection), hostName),
                    profile,
                    refocus);
            }
            catch (InvalidOperationException ex)
            {
                ErrorBox.Show(ex.Message, I18n.Get(Title.DeleteConnection));
                return;
            }
        }

        /// <summary>
        /// Connect to a host.
        /// </summary>
        /// <param name="profile">Profile containing the host.</param>
        /// <param name="hostName">Host entry name.</param>
        /// <param name="isShift">True if Shift key is pressed.</param>
        private void ConnectToHost(Profile profile, string hostName, bool isShift)
        {
            if (isShift)
            {
                NewWindow(this, this.components, profile.PathName, hostName);
                return;
            }

            if (!this.ProfileManager.IsCurrentPathName(profile.PathName))
            {
                // Switch profiles first.
                this.ProfileManager.Load(profile.PathName);
            }

            this.connect.ConnectToHost(profile.Hosts.FirstOrDefault(h => h.Name.Equals(hostName)));
        }

        /// <summary>
        /// Edit a host.
        /// </summary>
        /// <param name="hostNode">Host to edit.</param>
        /// <param name="editedEntry">Edited entry, for recording completion.</param>
        private void EditHost(HostTreeNode hostNode, HostEntry editedEntry = null)
        {
            var hostEntry = editedEntry;
            if (hostEntry == null)
            {
                // Find the existing host entry.
                hostEntry = hostNode.Profile.Hosts.FirstOrDefault(h => h.Name.Equals(hostNode.Text));
                if (hostEntry == null)
                {
                    return;
                }
            }

            // Pop up the dialog.
            using var editor = new HostEditor(HostEditingMode.SaveHost, hostEntry, hostNode.Profile, this.app);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (editor.Result.HasFlag(HostEditingResult.Save))
                {
                    var newHostEntry = editor.HostEntry;
                    if (!newHostEntry.Name.Equals(hostEntry.Name, StringComparison.InvariantCultureIgnoreCase)
                        && hostNode.Profile.Hosts.Any(h => h.Name.Equals(newHostEntry.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        newHostEntry.Name = CreateUniqueName(newHostEntry.Name, hostNode.Profile.Hosts.Select(p => p.Name));
                    }

                    var newName = this.PathCombine(hostNode.Profile.DisplayFolder, hostNode.Profile.Name, newHostEntry.Name);
                    this.autoSelectPath = newName;
                    var refocus = new ProfileRefocus(
                        this.ProfileManager,
                        Separator,
                        this.PathCombine(hostNode.Profile.DisplayFolder, hostNode.Profile.Name, hostNode.Text),
                        newName);
                    try
                    {
                        this.ProfileManager.PushAndSave(
                            (current) =>
                            {
                                current.Hosts = current.Hosts.Select(h => h.Name.Equals(hostEntry.Name, StringComparison.InvariantCultureIgnoreCase) ? newHostEntry : h).ToArray();
                            },
                            string.Format(I18n.Get(Message.EditConnection), hostEntry.Name),
                            hostNode.Profile,
                            refocus);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ErrorBox.Show(ex.Message, I18n.Get(Title.EditConnection));
                        return;
                    }
                }

                if (editor.Result.HasFlag(HostEditingResult.Record))
                {
                    this.app.MacroRecorder.Start(this.EditHostMacroRecorderComplete, (hostNode, editor.HostEntry));
                    this.Hide();
                    this.mainScreen.Focus();
                }
            }
        }

        /// <summary>
        /// Recording a login macro for an edited host is complete.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="context">Context object.</param>
        private void EditHostMacroRecorderComplete(string text, object context)
        {
            var (node, entry) = (((HostTreeNode, HostEntry)?)context).Value;
            entry.LoginMacro = text;

            // Restore this window and the dialog.
            this.Show();
            this.EditHost(node, entry);
        }

        /// <summary>
        /// A profile context menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProfileContextMenuClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menuItem))
            {
                return;
            }

            string tag = menuItem.Tag as string;
            if (tag == null)
            {
                return;
            }

            if (this.rightClickNode == null)
            {
                return;
            }

            var selectedProfileNode = this.rightClickNode as ProfileTreeNode;
            if (selectedProfileNode == null)
            {
                return;
            }

            switch (tag)
            {
                case "SwitchTo":
                    if (this.connected)
                    {
                        NewWindow(this, this.components, selectedProfileNode.Profile.PathName);
                    }
                    else
                    {
                        this.LoadWithAutoConnect(selectedProfileNode.Profile.PathName, ModifierKeys.HasFlag(Keys.Shift));
                    }

                    break;
                case "Edit":
                    if (this.connected)
                    {
                        NewWindow(this, this.components, selectedProfileNode.Profile.PathName, editMode: true);
                    }
                    else if (this.app.Allowed(Restrictions.SwitchProfile))
                    {
                        this.LoadWithoutAutoConnect(selectedProfileNode.Profile.PathName, ModifierKeys.HasFlag(Keys.Shift));
                    }

                    break;
                case "Duplicate":
                    this.DuplicateProfile(selectedProfileNode.Profile);
                    break;
                case "Rename":
                    // Rename a profile.
                    selectedProfileNode.BeginEdit();
                    break;
                case "MergeFrom":
                    this.MergeFromProfile(this.ProfileManager.Current, selectedProfileNode.Profile);
                    break;
                case "Export":
                    // Export a profile.
                    this.ExportProfile(selectedProfileNode.Profile);
                    break;
                case "Shortcut":
                    // Create a shortcut.
                    this.CreateShortcut(selectedProfileNode.Profile);
                    break;
                case "Default":
                    this.ProfileManager.SetDefaultProfile(selectedProfileNode.Profile);
                    break;
                case "Delete":
                    // Delete a profile.
                    this.DeleteProfile(selectedProfileNode);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// An item in the host context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HostContextMenuClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
            {
                return;
            }

            var tag = menuItem.Tag as string;
            if (tag == null)
            {
                return;
            }

            if (this.rightClickNode == null)
            {
                return;
            }

            var selectedHostNode = this.rightClickNode as HostTreeNode;
            if (selectedHostNode == null)
            {
                return;
            }

            switch (tag)
            {
                case "Connect":
                    // Connect to a host.
                    if (this.connected)
                    {
                        NewWindow(this, this.components, selectedHostNode.Profile.PathName, selectedHostNode.Text);
                    }
                    else
                    {
                        this.ConnectToHost(selectedHostNode.Profile, selectedHostNode.Text, ModifierKeys.HasFlag(Keys.Shift));
                        this.SafeHide();
                    }

                    break;
                case "Edit":
                    // Edit a host.
                    this.EditHost(selectedHostNode);
                    break;
                case "Duplicate":
                    // Duplicate a host.
                    this.DuplicateHost(selectedHostNode);
                    break;
                case "Rename":
                    // Rename a host.
                    selectedHostNode.BeginEdit();
                    break;
                case "Delete":
                    // Delete a host.
                    this.DeleteHost(selectedHostNode);
                    break;
                case "Shortcut":
                    // Create a shortcut.
                    this.CreateShortcut(selectedHostNode.Profile, selectedHostNode.Text);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// A node in the tree view was double-clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            var profileNode = this.treeView.SelectedNode as ProfileTreeNode;
            if (profileNode != null)
            {
                if (profileNode.IsDefaults || profileNode.IsCurrent || this.app.Restricted(Restrictions.SwitchProfile))
                {
                    return;
                }

                // Switch to that profile, with auto-connect.
                this.LoadWithAutoConnect(profileNode.Profile.PathName, this.connected || ModifierKeys.HasFlag(Keys.Shift));
                if (profileNode.Profile.Hosts.Any(h => h.AutoConnect != AutoConnect.None))
                {
                    this.SafeHide();
                }

                return;
            }

            var hostNode = this.treeView.SelectedNode as HostTreeNode;
            if (hostNode != null)
            {
                if (hostNode.Profile != this.ProfileManager.Current && this.app.Restricted(Restrictions.SwitchProfile))
                {
                    return;
                }

                if (this.connected)
                {
                    // Open in new window.
                    NewWindow(this, this.components, hostNode.Profile.PathName, hostNode.Text);
                }
                else
                {
                    // Connect and hide the window.
                    this.ConnectToHost(hostNode.Profile, hostNode.Text, ModifierKeys.HasFlag(Keys.Shift));
                    this.SafeHide();
                }
            }
        }

        /// <summary>
        /// Load a profile without auto-connect.
        /// </summary>
        /// <param name="profilePath">Profile path name.</param>
        /// <param name="isShift">True if Shift is pressed (new window).</param>
        private void LoadWithoutAutoConnect(string profilePath, bool isShift)
        {
            if (isShift)
            {
                // Open in a new window, in edit mode (no auto-connect, try for read/write).
                NewWindow(this, this.components, profilePath, editMode: true);
            }
            else
            {
                if (this.ProfileManager.Load(profilePath))
                {
                    ////this.ProfileManager.FlushUndoRedo();
                }
            }
        }

        /// <summary>
        /// The Undo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            this.treeView.Focus();
            this.ProfileManager.Undo();
        }

        /// <summary>
        /// The Redo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RedoButton_Click(object sender, EventArgs e)
        {
            this.treeView.Focus();
            this.ProfileManager.Redo();
        }

        /// <summary>
        /// A folder button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FolderButtonClick(object sender, EventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            var selectedFolderNode = this.treeView.SelectedNode as FolderTreeNode;

            // Give the focus back to the tree view.
            this.treeView.Focus();

            switch ((string)button.Tag)
            {
                case "WatchNew":
                    if (selectedFolderNode != null)
                    {
                        this.watchFolderDialog.SelectedPath = selectedFolderNode.FolderName;
                    }
                    else
                    {
                        var node = this.treeView.SelectedNode;
                        while (node != null && !(node is FolderTreeNode))
                        {
                            node = node.Parent;
                        }

                        this.watchFolderDialog.SelectedPath = (node != null) ? (node as FolderTreeNode).FolderName : string.Empty;
                    }

                    var result = this.watchFolderDialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        var newFolder = this.watchFolderDialog.SelectedPath;
                        if (this.app.ProfileTracker.IsWatched(newFolder))
                        {
                            // Nothing more to do here.
                            var existingDir = this.treeView.Nodes.Find(DirNodeName(newFolder), true).FirstOrDefault();
                            if (existingDir != null)
                            {
                                this.treeView.SelectedNode = existingDir;
                                this.treeView.SelectedNode.EnsureVisible();
                            }

                            return;
                        }

                        this.autoSelectPath = DirNodeName(newFolder);
                        this.app.ProfileTracker.Watch(newFolder);
                        this.ProfileManager.PushConfigAction(
                            new FolderWatchConfigAction(
                                string.Format(I18n.Get(Message.WatchFolder), newFolder),
                                newFolder,
                                this.app.ProfileTracker,
                                this.ProfileManager));
                    }

                    break;
                case "StopWatching":
                    this.UnwatchFolder(selectedFolderNode);
                    break;
            }
        }

        /// <summary>
        /// The Help picture box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Clicked(object sender, EventArgs e)
        {
            Wx3270App.GetHelp("Profiles");
        }

        /// <summary>
        /// An item on the folder context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FolderContextMenu_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
            {
                return;
            }

            if (this.rightClickNode == null)
            {
                return;
            }

            var selectedFolderNode = this.rightClickNode as FolderTreeNode;
            if (selectedFolderNode == null)
            {
                return;
            }

            switch ((string)menuItem.Tag)
            {
                case "New":
                    // Create a new profile here.
                    this.NewProfile(selectedFolderNode);
                    break;
                case "Import":
                    this.ImportProfile(selectedFolderNode.FolderName);
                    break;
                case "StopWatching":
                    // Stop watching this folder.
                    this.UnwatchFolder(selectedFolderNode);
                    break;
                case "Delete":
                    // Delete this folder.
                    this.DeleteFolder(selectedFolderNode);
                    break;
            }
        }

        /// <summary>
        /// A node in the tree view was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var hostTreeNode = e.Node as HostTreeNode;
            if (hostTreeNode == null)
            {
                return;
            }

            this.hostConnectToolStripMenuItem.Enabled = !this.connected &&
                    (hostTreeNode.Profile == this.ProfileManager.Current || this.app.Allowed(Restrictions.SwitchProfile));
        }

        /// <summary>
        /// Stop watching a folder.
        /// </summary>
        /// <param name="folderTreeNode">Folder tree node.</param>
        private void UnwatchFolder(FolderTreeNode folderTreeNode)
        {
            if (this.app.ProfileTracker.Unwatch(folderTreeNode.FolderName))
            {
                this.ProfileManager.PushConfigAction(
                    new FolderUnwatchConfigAction(
                        string.Format(I18n.Get(Message.StopWatchingFolder), folderTreeNode.Text),
                        folderTreeNode.FolderName,
                        this.app.ProfileTracker,
                        this.ProfileManager));
            }
        }

        /// <summary>
        /// Combine parts into a path.
        /// </summary>
        /// <param name="parts">Parts to combine.</param>
        /// <returns>Combined path.</returns>
        private string PathCombine(params string[] parts)
        {
            return string.Join(Separator, parts);
        }

        /// <summary>
        /// Combine parts into a path.
        /// </summary>
        /// <param name="parts">Parts to combine.</param>
        /// <returns>Combined path.</returns>
        private string PathCombine(IEnumerable<string> parts)
        {
            return string.Join(Separator, parts);
        }

        /// <summary>
        /// Split a path into parts.
        /// </summary>
        /// <param name="path">Path to split.</param>
        /// <returns>Path parts.</returns>
        private string[] PathSplit(string path)
        {
            return path.Split(new[] { Separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// The UI connect action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiConnect(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            var args = arguments.ToList();
            if (args.Count != 1)
            {
                result = Constants.Action.Connect + "() takes 1 argument";
                return PassthruResult.Failure;
            }

            var hostEntry = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
            if (hostEntry == null)
            {
                result = Constants.Action.Connect + "(): no such host defined in the current profile";
                return PassthruResult.Failure;
            }

            result = string.Empty;
            if (!this.connect.ConnectToHost(
                hostEntry,
                out string errorMessage,
                (success, r) =>
                {
                    // Asynchronous completion.
                    this.app.BackEnd.PassthruComplete(success, r, tag);
                }))
            {
                result = errorMessage;
                return PassthruResult.Failure;
            }

            // Connection is pending.
            return PassthruResult.Pending;
        }

        /// <summary>
        /// The UI switch profile action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiSwitchProfile(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            var args = arguments.ToList();
            if (args.Count != 1)
            {
                result = Constants.Action.SwitchProfile + "() takes 1 argument";
                return PassthruResult.Failure;
            }

            if (this.connected)
            {
                result = Constants.Action.SwitchProfile + "(): cannot switch profiles while connected";
                return PassthruResult.Failure;
            }

            var profile = args[0];
            if (!profile.EndsWith(Wx3270.ProfileManager.Suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                // No suffix. Add one.
                profile += Wx3270.ProfileManager.Suffix;
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(profile)))
            {
                // Relative path. Take it relative to the current folder.
                profile = this.ProfileManager.Current.MappedPath(profile);
            }

            if (!this.LoadWithAutoConnect(profile, isShift: false, doErrorPopups: false))
            {
                result = Constants.Action.SwitchProfile + "(): Cannot load " + profile;
                return PassthruResult.Failure;
            }

            result = string.Empty;
            return PassthruResult.Success;
        }

        /// <summary>
        /// One of the items on the New Profile context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NewProfileContextMenuClick(object sender, EventArgs e)
        {
            var profileType = (ProfileType)Enum.Parse(typeof(ProfileType), (sender as ToolStripMenuItem).Tag as string);
            this.DuplicateProfile(Profile.DefaultProfile, profileType);
        }

        /// <summary>
        /// Profile tree node.
        /// </summary>
        private class ProfileTreeNode : TreeNode
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileTreeNode"/> class.
            /// </summary>
            /// <param name="name">Node name.</param>
            public ProfileTreeNode(string name)
                : base(name)
            {
            }

            /// <summary>
            /// Gets or sets a value indicating whether this is the current profile.
            /// </summary>
            public bool IsCurrent { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the profile is broken.
            /// </summary>
            public bool IsBroken { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the profile is the dummy 'default values' profile.
            /// </summary>
            public bool IsDefaults { get; set; }

            /// <summary>
            /// Gets or sets the profile.
            /// </summary>
            public Profile Profile { get; set; }
        }

        /// <summary>
        /// Host tree node.
        /// </summary>
        private class HostTreeNode : TreeNode
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HostTreeNode"/> class.
            /// </summary>
            /// <param name="name">Node name.</param>
            public HostTreeNode(string name)
                : base(name)
            {
            }

            /// <summary>
            /// Gets or sets a value indicating whether the host is auto-connect.
            /// </summary>
            public bool AutoConnect { get; set; }

            /// <summary>
            /// Gets or sets the profile.
            /// </summary>
            public Profile Profile { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this host is part of the current profile.
            /// </summary>
            public bool IsCurrentProfile { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this is the currently connected host.
            /// </summary>
            public bool IsCurrentHost { get; set; }
        }

        /// <summary>
        /// Refocus handler for host operations.
        /// </summary>
        private class ProfileRefocus : IRefocus
        {
            /// <summary>
            /// The profile manager.
            /// </summary>
            private readonly IProfileManager profileManager;

            /// <summary>
            /// The path separator.
            /// </summary>
            private readonly string pathSeparator;

            /// <summary>
            /// The undo path.
            /// </summary>
            private readonly string undoPath;

            /// <summary>
            /// The redo path.
            /// </summary>
            private readonly string redoPath;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileRefocus"/> class.
            /// </summary>
            /// <param name="profileManager">Profile manager.</param>
            /// <param name="pathSeparator">Path separator.</param>
            /// <param name="undoPath">Pathname of node to focus on after an undo.</param>
            /// <param name="redoPath">Pathname of node to focus on after a redo.</param>
            public ProfileRefocus(IProfileManager profileManager, string pathSeparator, string undoPath, string redoPath)
            {
                this.profileManager = profileManager;
                this.pathSeparator = pathSeparator;
                this.undoPath = undoPath;
                this.redoPath = redoPath;
            }

            /// <summary>
            /// An undo or redo operation is about to be performed. Do a refocus.
            /// </summary>
            /// <param name="isUndo">True if undo.</param>
            public void Inform(bool isUndo)
            {
                string path = isUndo ? this.undoPath : this.redoPath;
                if (path != null)
                {
                    var parts = path.Split(new string[] { this.pathSeparator }, StringSplitOptions.None);
                    switch (parts.Length)
                    {
                        case 2:
                            // directory, profile
                            this.profileManager.Refocus(parts[0], parts[1]);
                            break;
                        case 3:
                            // directory, profile, host
                            this.profileManager.Refocus(parts[0], parts[1], parts[2]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Miscellaneous strings.
        /// </summary>
        private class String
        {
            /// <summary>
            /// Display name for My Documents.
            /// </summary>
            public static readonly string Documents = I18n.Combine(StringName, "documents");

            /// <summary>
            /// Display name for the Desktop.
            /// </summary>
            public static readonly string Desktop = I18n.Combine(StringName, "desktop");
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Profile edit.
            /// </summary>
            public static readonly string NewWindow = I18n.Combine(TitleName, "newWindow");

            /// <summary>
            /// Add a connection.
            /// </summary>
            public static readonly string AddConnection = I18n.Combine(TitleName, "addConnection");

            /// <summary>
            /// Copy a connection.
            /// </summary>
            public static readonly string CopyConnection = I18n.Combine(TitleName, "copyConnection");

            /// <summary>
            /// Move a connection.
            /// </summary>
            public static readonly string MoveConnection = I18n.Combine(TitleName, "moveConnection");

            /// <summary>
            /// Rename a connection.
            /// </summary>
            public static readonly string RenameConnection = I18n.Combine(TitleName, "renameConnection");

            /// <summary>
            /// Duplicate a connection.
            /// </summary>
            public static readonly string DuplicateConnection = I18n.Combine(TitleName, "duplicateConnection");

            /// <summary>
            /// Delete a connection.
            /// </summary>
            public static readonly string DeleteConnection = I18n.Combine(TitleName, "deleteConnection");

            /// <summary>
            /// Edit a connection.
            /// </summary>
            public static readonly string EditConnection = I18n.Combine(TitleName, "editConnection");

            /// <summary>
            /// Copy a profile.
            /// </summary>
            public static readonly string CopyProfile = I18n.Combine(TitleName, "copyProfile");

            /// <summary>
            /// Undo copying a profile.
            /// </summary>
            public static readonly string UndoCopyProfile = I18n.Combine(TitleName, "undoCopyProfile");

            /// <summary>
            /// Rename a profile.
            /// </summary>
            public static readonly string RenameProfile = I18n.Combine(TitleName, "renameProfile");

            /// <summary>
            /// Undo renaming a profile.
            /// </summary>
            public static readonly string UndoRenameProfile = I18n.Combine(TitleName, "undoRenameProfile");

            /// <summary>
            /// Delete a profile.
            /// </summary>
            public static readonly string DeleteProfile = I18n.Combine(TitleName, "deleteProfile");

            /// <summary>
            /// Undo deleting a profile.
            /// </summary>
            public static readonly string UndoDeleteProfile = I18n.Combine(TitleName, "undoDeleteProfile");

            /// <summary>
            /// Duplicate a profile.
            /// </summary>
            public static readonly string DuplicateProfile = I18n.Combine(TitleName, "duplicateProfile");

            /// <summary>
            /// Undo duplicating a profile.
            /// </summary>
            public static readonly string UndoDuplicateProfile = I18n.Combine(TitleName, "undoDuplicateProfile");

            /// <summary>
            /// Create a profile.
            /// </summary>
            public static readonly string CreateProfile = I18n.Combine(TitleName, "createProfile");

            /// <summary>
            /// Undo creating a profile.
            /// </summary>
            public static readonly string UndoCreateProfile = I18n.Combine(TitleName, "undoCreateProfile");

            /// <summary>
            /// Import a profile.
            /// </summary>
            public static readonly string ImportProfile = I18n.Combine(TitleName, "importProfile");

            /// <summary>
            /// Undo importing a profile.
            /// </summary>
            public static readonly string UndoImportProfile = I18n.Combine(TitleName, "undoImportProfile");

            /// <summary>
            /// Stop watching a profile folder.
            /// </summary>
            public static readonly string StopWatchingFolder = I18n.Combine(TitleName, "stopWatchingFolder");

            /// <summary>
            /// Undo watching a profile folder.
            /// </summary>
            public static readonly string UndoWatchFolder = I18n.Combine(TitleName, "undoWatchFolder");

            /// <summary>
            /// Delete a folder.
            /// </summary>
            public static readonly string DeleteFolder = I18n.Combine(TitleName, "deleteFolder");

            /// <summary>
            /// Undo a folder delete.
            /// </summary>
            public static readonly string UndoDeleteFolder = I18n.Combine(TitleName, "undoDeleteFolder");

            /// <summary>
            /// Import profile dialog.
            /// </summary>
            public static readonly string ImportProfileDialog = I18n.Combine(TitleName, "importProfileDialog");
        }

        /// <summary>
        /// Message text.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Name already exists.
            /// </summary>
            public static readonly string NameAlreadyExists = I18n.Combine(MessageName, "nameAlreadyExists");

            /// <summary>
            /// Profile save failed.
            /// </summary>
            public static readonly string ProfileSaveFailed = I18n.Combine(MessageName, "profileSaveFailed");

            /// <summary>
            /// Invalid file type.
            /// </summary>
            public static readonly string InvalidFileType = I18n.Combine(MessageName, "invalidFileType");

            /// <summary>
            /// Can't stop watching current folder.
            /// </summary>
            public static readonly string CannotStopWatching = I18n.Combine(MessageName, "cannotStopWatching");

            /// <summary>
            /// Can't delete current profile.
            /// </summary>
            public static readonly string CannotDeleteCurrentProfile = I18n.Combine(MessageName, "cannotDeleteCurrentProfile");

            /// <summary>
            /// Can't overwrite current profile.
            /// </summary>
            public static readonly string CannotOverwriteCurrentProfile = I18n.Combine(MessageName, "cannotOverwriteCurrentProfile");

            /// <summary>
            /// Can't rename current profile.
            /// </summary>
            public static readonly string CannotRenameCurrentProfile = I18n.Combine(MessageName, "cannotRenameCurrentProfile");

            /// <summary>
            /// Add connection undo/redo.
            /// </summary>
            public static readonly string AddConnection = I18n.Combine(MessageName, "addConnection");

            /// <summary>
            /// Move connection undo/redo.
            /// </summary>
            public static readonly string MoveConnection = I18n.Combine(MessageName, "moveConnection");

            /// <summary>
            /// Rename connection undo/redo.
            /// </summary>
            public static readonly string RenameConnection = I18n.Combine(MessageName, "renameConnection");

            /// <summary>
            /// Duplicate connection undo/redo.
            /// </summary>
            public static readonly string DuplicateConnection = I18n.Combine(MessageName, "duplicateConnection");

            /// <summary>
            /// Delete connection undo/redo.
            /// </summary>
            public static readonly string DeleteConnection = I18n.Combine(MessageName, "deleteConnection");

            /// <summary>
            /// Edit connection undo/redo.
            /// </summary>
            public static readonly string EditConnection = I18n.Combine(MessageName, "editConnection");

            /// <summary>
            /// Copy profile undo/redo.
            /// </summary>
            public static readonly string CopyProfile = I18n.Combine(MessageName, "copyProfile");

            /// <summary>
            /// Rename profile undo/redo.
            /// </summary>
            public static readonly string RenameProfile = I18n.Combine(MessageName, "renameProfile");

            /// <summary>
            /// Delete profile undo/redo.
            /// </summary>
            public static readonly string DeleteProfile = I18n.Combine(MessageName, "deleteProfile");

            /// <summary>
            /// Duplicate profile undo/redo.
            /// </summary>
            public static readonly string DuplicateProfile = I18n.Combine(MessageName, "duplicateProfile");

            /// <summary>
            /// Create profile undo/redo.
            /// </summary>
            public static readonly string CreateProfile = I18n.Combine(MessageName, "createProfile");

            /// <summary>
            /// Import profile undo/redo.
            /// </summary>
            public static readonly string ImportProfile = I18n.Combine(MessageName, "importProfile");

            /// <summary>
            /// Watch folder undo/redo.
            /// </summary>
            public static readonly string WatchFolder = I18n.Combine(MessageName, "watchFolder");

            /// <summary>
            /// Stop watching folder undo/redo.
            /// </summary>
            public static readonly string StopWatchingFolder = I18n.Combine(MessageName, "stopWatchingFolder");

            /// <summary>
            /// Delete folder undo/redo.
            /// </summary>
            public static readonly string DeleteFolder = I18n.Combine(MessageName, "deleteFolder");

            /// <summary>
            /// Copy suffix for duplicated items.
            /// </summary>
            public static readonly string Copy = I18n.Combine(MessageName, "copy");

            /// <summary>
            /// Name of a new profile.
            /// </summary>
            public static readonly string NewProfile = I18n.Combine(MessageName, "newProfile");

            /// <summary>
            /// Name of a new keymap template.
            /// </summary>
            public static readonly string NewKeymapTemplate = I18n.Combine(MessageName, "newKeymapTemplate");

            /// <summary>
            /// Name of a new keypad template.
            /// </summary>
            public static readonly string NewKeypadTemplate = I18n.Combine(MessageName, "newKeypadTemplate");

            /// <summary>
            /// Tool tip for the default profile.
            /// </summary>
            public static readonly string IsDefaultProfile = I18n.Combine(MessageName, "isDefaultProfile");

            /// <summary>
            /// Tool tip for the current profile.
            /// </summary>
            public static readonly string IsCurrentProfile = I18n.Combine(MessageName, "isCurrentProfile");

            /// <summary>
            /// Tool tip for keyboard maps.
            /// </summary>
            public static readonly string IsKeyboardMap = I18n.Combine(MessageName, "isKeyboardMap");

            /// <summary>
            /// Tool tip for keypad maps.
            /// </summary>
            public static readonly string IsKeypadMap = I18n.Combine(MessageName, "isKeypadMap");

            /// <summary>
            /// Tool tip extra text for opening a profile or connection in a new window.
            /// </summary>
            public static readonly string OpenInNewWindow = I18n.Combine(MessageName, "openInNewWindow");
        }
    }
}
