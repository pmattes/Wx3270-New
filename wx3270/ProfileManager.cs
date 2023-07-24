// <copyright file="ProfileManager.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    using I18nBase;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using Wx3270.Contracts;

    /// <summary>
    /// Profile manager.
    /// </summary>
    public class ProfileManager : IProfileManager
    {
        /// <summary>
        /// File name extension for profile files.
        /// </summary>
        public const string Suffix = ".wx3270";

        /// <summary>
        /// The icon name.
        /// </summary>
        private const string Icon = "wx3270.ico";

        /// <summary>
        /// Seed profile name, in English.
        /// </summary>
        private const string SeedProfileName = "Base";

        /// <summary>
        /// The seed profile folder name.
        /// </summary>
        private const string SeedFolderName = "wx3270";

        /// <summary>
        /// Registry value for default profile.
        /// </summary>
        private const string DefaultProfileRegistryValue = "DefaultProfile";

        /// <summary>
        /// Name of no display folder.
        /// </summary>
        private const string NoDisplayFolder = "<No Folder>";

        /// <summary>
        /// The name of titles, for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(ProfileManager));

        /// <summary>
        /// The name of strings, for localization.
        /// </summary>
        private static readonly string StringName = I18n.StringName(nameof(ProfileManager));

        /// <summary>
        /// The name of messages, for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(ProfileManager));

        /// <summary>
        /// Merge handlers.
        /// </summary>
        private readonly List<Tuple<ImportType, MergeHandler>> mergeHandlers = new List<Tuple<ImportType, MergeHandler>>();

        /// <summary>
        /// The stack of saved previous states.
        /// </summary>
        private readonly LimitedStack<ConfigAction> undoStack = new LimitedStack<ConfigAction>(500);

        /// <summary>
        /// The stack saved next states.
        /// </summary>
        private readonly Stack<ConfigAction> redoStack = new Stack<ConfigAction>();

        /// <summary>
        /// Suppressed errors.
        /// </summary>
        private readonly List<string> errors = new List<string>();

        /// <summary>
        /// The set of Undo controls.
        /// </summary>
        private readonly List<UndoRedoControl> undoControls = new List<UndoRedoControl>();

        /// <summary>
        /// The set of Redo controls.
        /// </summary>
        private readonly List<UndoRedoControl> redoControls = new List<UndoRedoControl>();

        /// <summary>
        /// True if error messages should be suppressed.
        /// </summary>
        private bool suppressErrors = true;

        /// <summary>
        /// True if a profile is being pushed out.
        /// </summary>
        private bool propagatingProfile;

        /// <summary>
        /// True if we have pushed out the initial profile.
        /// </summary>
        private bool pushedFirst;

        /// <summary>
        /// The set of current folder nodes.
        /// </summary>
        private List<FolderWatchNode> profileTree = new List<FolderWatchNode>();

        /// <summary>
        /// Open file stream for the current profile.
        /// </summary>
        private FileStream currentFileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileManager"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        public ProfileManager(Wx3270App app)
        {
            this.App = app;

            // Get the default profile path from the registry.
            var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.RegistryKey);
            var defaultProfilePath = (string)key.GetValue(DefaultProfileRegistryValue);
            if (defaultProfilePath == null)
            {
                // Use the default, and save it, in case they pick a different language later.
                defaultProfilePath = SeedProfilePath;
                key.SetValue(DefaultProfileRegistryValue, defaultProfilePath);
            }

            key.Close();

            // Set the static values everything else depends on.
            DefaultProfilePath = defaultProfilePath;
            DefaultProfileName = Path.GetFileNameWithoutExtension(defaultProfilePath);
            ProfileDirectory = Path.GetDirectoryName(defaultProfilePath);

            // Start with a default profile.
            this.Current = new Profile { Name = NoProfileName, ReadOnly = true, DisplayFolder = NoDisplayFolder };

            // Create the profile directory and the watcher.
            this.CreateProfileDirectoryAndProfile();
        }

        /// <inheritdoc />
        public event ChangeHandler Change = (profile) => { };

        /// <inheritdoc />
        public event ChangeToHandler ChangeTo = (from, to) => { };

        /// <inheritdoc />
        public event Action<Profile, bool> ChangeFinal = (profile, isNew) => { };

        /// <inheritdoc />
        public event ListChangeHandler ListChange = (names) => { };

        /// <inheritdoc />
        public event ChangesPendingHandler ChangesPending = (pending) => { };

        /// <inheritdoc />>
        public event SafetyCheckHandler SafetyCheck = (Profile oldProfile, Profile newProfile, ref bool safe) => { };

        /// <inheritdoc />
        public event RefocusHandler RefocusEvent = (profileDir, profileName, hostName) => { };

        /// <inheritdoc />
        public event Action DefaultProfileChanged = () => { };

        /// <inheritdoc />
        public event ChangeHandler NewProfileOpened = (profile) => { };

        /// <inheritdoc />
        public event ChangeHandler ProfileClosing = (profile) => { };

        /// <inheritdoc />
        public event OldVersionHandler OldVersion = (Profile.VersionClass oldVersion, ref bool saved) => { };

        /// <summary>
        /// Gets the directory where profiles are kept.
        /// </summary>
        /// <returns>Name of wx3270 profile directory.</returns>
        public static string ProfileDirectory { get; private set; }

        /// <summary>
        /// Gets the default profile name.
        /// </summary>
        public static string DefaultProfileName { get; private set; }

        /// <summary>
        /// Gets the name of the default values.
        /// </summary>
        public static string DefaultValuesName => "<" + I18n.Get(StringKey.DefaultValuesName) + ">";

        /// <summary>
        /// Gets the name of no profile.
        /// </summary>
        public static string NoProfileName => "<" + I18n.Get(StringKey.NoProfile) + ">";

        /// <summary>
        /// Gets the localized value of the "read-only" label.
        /// </summary>
        public static string ReadOnlyName => I18n.Get(StringKey.ReadOnly);

        /// <summary>
        /// Gets the seed name of directory where profiles are kept. The actual value comes from the registry.
        /// </summary>
        /// <returns>Name of wx3270 profile directory.</returns>
        public static string SeedProfileDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SeedFolderName);

        /// <inheritdoc />
        public Profile Current { get; private set; }

        /// <inheritdoc />
        public string ExternalText => I18n.Get(StringKey.External);

        /// <summary>
        /// Gets the seed full pathname of the default default profile. The actual value comes from the registry.
        /// </summary>
        private static string SeedProfilePath => Path.Combine(SeedProfileDirectory, I18n.Get(StringKey.Base) + Suffix);

        /// <summary>
        /// Gets or sets the full pathname of the default profile.
        /// </summary>
        private static string DefaultProfilePath { get; set; }

        /// <summary>
        /// Gets or sets the application context.
        /// </summary>
        private Wx3270App App { get; set; }

        /// <summary>
        /// Gets the number of Undo entries.
        /// </summary>
        private int UndoCount => this.undoStack.Count();

        /// <summary>
        /// Gets the number of Redo entries.
        /// </summary>
        private int RedoCount => this.redoStack.Count();

        /// <summary>
        /// Perform static localization.
        /// </summary>
        /// <remarks>
        /// This is called from localization, very early in initiailization, due to the <see cref="I18nInitAttribute"/> attribute.
        /// </remarks>
        [I18nInit]
        public static void Localize()
        {
            // Localize some strings.
            I18n.LocalizeGlobal(StringKey.Base, SeedProfileName);
            I18n.LocalizeGlobal(StringKey.Undo, "Undo {0}");
            I18n.LocalizeGlobal(StringKey.Redo, "Redo {0}");
            I18n.LocalizeGlobal(StringKey.Change, "change {0}");
            I18n.LocalizeGlobal(StringKey.External, "external");
            I18n.LocalizeGlobal(StringKey.MergeFrom, "merge from {0}");
            I18n.LocalizeGlobal(StringKey.SetToDefaults, "set to defaults");
            I18n.LocalizeGlobal(StringKey.ChangeDefaultProfile, "change default profile to '{0}':");
            I18n.LocalizeGlobal(StringKey.DefaultValuesName, "Default Values");
            I18n.LocalizeGlobal(StringKey.NoProfile, "No Profile");
            I18n.LocalizeGlobal(StringKey.ReadOnly, "Read-Only");
            I18n.LocalizeGlobal(StringKey.Disable, "disable {0}");

            I18n.LocalizeGlobal(Title.DefaultProfileChange, "Default Profile Change");
            I18n.LocalizeGlobal(Title.ProfileDirectoryError, "Profile Directory Error");
            I18n.LocalizeGlobal(Title.ProfileIconError, "Profile Icon Error");
            I18n.LocalizeGlobal(Title.ProfileDesktopIniError, "Profile Desktop.ini Error");
            I18n.LocalizeGlobal(Title.ProfileOpen, "Profile Open");
            I18n.LocalizeGlobal(Title.ProfileLoad, "Profile Load");
            I18n.LocalizeGlobal(Title.ProfileMerge, "Profile Merge");
            I18n.LocalizeGlobal(Title.ProfileSave, "Profile Save");
            I18n.LocalizeGlobal(Title.ProfileError, "Profile Error");
            I18n.LocalizeGlobal(Title.UndoRedoError, "Undo/Redo Error");

            I18n.LocalizeGlobal(Message.ProfileBusy, "Profile '{0}' is busy, opening read-only.");
            I18n.LocalizeGlobal(Message.CannotChangeProfile, "Profile cannot be changed");
            I18n.LocalizeGlobal(Message.CannotDeserializeProfile, "Cannot deserialize profile");
            I18n.LocalizeGlobal(Message.ProfileVersionMismatch, "Profile version ({0}) is newer than wx3270 version ({1})" + Environment.NewLine + "Unknown settings will be ignored");
        }

        /// <summary>
        /// Expand a simple profile name to its path.
        /// </summary>
        /// <param name="simpleName">Simple profile name.</param>
        /// <returns>Full path.</returns>
        public static string ProfilePath(string simpleName)
        {
            return Path.Combine(ProfileDirectory, simpleName + Suffix);
        }

        /// <summary>
        /// Ensures that the profile directory exists.
        /// </summary>
        /// <param name="forWindows">If true, create Windows artifacts.</param>
        /// <returns>True if directory now exists.</returns>
        public static bool CreateProfileDirectory(bool forWindows)
        {
            if (!Directory.Exists(ProfileDirectory))
            {
                try
                {
                    // Create the directory.
                    Directory.CreateDirectory(ProfileDirectory);
                }
                catch (Exception e)
                {
                    ErrorBox.Show(e.Message, I18n.Get(Title.ProfileDirectoryError));
                    return false;
                }
            }

            if (!forWindows)
            {
                return true;
            }

            // Set the read-only attribute on the profile directory, so file explorer looks for Desktop.ini.
            // Note that this doesn't actually make the directory read-only.
            // Also get rid of the System attribute, which might have been set by an earlier version of this code.
            try
            {
                File.SetAttributes(ProfileDirectory, (File.GetAttributes(ProfileDirectory) & ~FileAttributes.System) | FileAttributes.ReadOnly);
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ProfileDesktopIniError));
                return true;
            }

            var iniPath = Path.Combine(ProfileDirectory, "Desktop.ini");
            if (!File.Exists(iniPath))
            {
                try
                {
                    // Create Desktop.ini.
                    using (var outStream = File.Create(iniPath))
                    {
                        using var writer = new StreamWriter(outStream, new UnicodeEncoding());
                        writer.WriteLine("[.ShellClassInfo]");
                        writer.WriteLine("ConfirmFileOp=0");
                        writer.WriteLine("IconFile=" + Application.ExecutablePath);
                        writer.WriteLine("IconIndex=0");
                        writer.WriteLine("InfoTip=wx3270 Profiles");
                    }

                    File.SetAttributes(iniPath, File.GetAttributes(iniPath) | FileAttributes.System | FileAttributes.Hidden);
                }
                catch (Exception e)
                {
                    ErrorBox.Show(e.Message, I18n.Get(Title.ProfileDesktopIniError));
                    return true;
                }
            }

            // Set up a watch on the library, if there is one.
            var library = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Constants.Misc.Library);
            if (Directory.Exists(library) && !WatcherKey.Exists(library))
            {
                WatcherKey.Add(library);
            }

            return true;
        }

        /// <summary>
        /// Read a profile without returning a locked stream.
        /// </summary>
        /// <param name="profilePath">Profile name.</param>
        /// <param name="error">Returned error message.</param>
        /// <param name="warning">Returned warning message.</param>
        /// <param name="busy">Returned true if the file is busy.</param>
        /// <param name="notFound">Returned true if the file is not found.</param>
        /// <returns>Profile, or null.</returns>
        public static Profile Read(string profilePath, out string error, out string warning, out bool busy, out bool notFound)
        {
            var profile = Read(profilePath, out error, out warning, locked: false, out FileStream stream, out busy, out notFound, out _);
            stream?.Close();
            return profile;
        }

        /// <inheritdoc />
        public void SetProfileList(IProfileTracker profileTracker)
        {
            this.profileTree = profileTracker.Tree;
            profileTracker.ProfileTreeChanged += (nodes) => this.profileTree = nodes;
        }

        /// <inheritdoc />
        public void CreateProfileDirectoryAndProfile()
        {
            // Create the default profile directory.
            CreateProfileDirectory(this.App.IsWindows);

            // Create the base profile.
            if (!File.Exists(DefaultProfilePath))
            {
                this.Save(DefaultProfilePath, Profile.DefaultProfile);
            }
        }

        /// <inheritdoc />
        public bool Load(string profilePath, out string outProfilePath, bool readOnly = false, bool doErrorPopups = true)
        {
            return this.LoadInternal(profilePath, out outProfilePath, readOnly, doErrorPopups);
        }

        /// <inheritdoc />
        public bool Load(string profilePath, bool readOnly = false, bool doErrorPopups = true)
        {
            return this.LoadInternal(profilePath, out _, readOnly, doErrorPopups);
        }

        /// <inheritdoc />
        public bool Merge(Profile destProfile, string mergeProfilePath, ImportType importType)
        {
            // Read in the source profile.
            var mergeProfile = Read(mergeProfilePath, out string error, out string warning, out _, out _);
            if (mergeProfile == null)
            {
                ErrorBox.Show(error, I18n.Get(Title.ProfileMerge));

                return false;
            }

            if (warning != null)
            {
                ErrorBox.Show(warning, I18n.Get(Title.ProfileMerge), MessageBoxIcon.Warning);
            }

            // Do the merge.
            return this.Merge(destProfile, mergeProfile, importType);
        }

        /// <inheritdoc />
        public bool Merge(Profile destProfile, Profile mergeProfile, ImportType importType)
        {
            var previous = this.Current.Clone();
            if (this.PushAndSave(
                (current) =>
                {
                    foreach (var handler in this.mergeHandlers)
                    {
                        if ((handler.Item1 & importType) != ImportType.None)
                        {
                            handler.Item2(current, mergeProfile, importType);
                        }
                    }
                },
                string.Format(I18n.Get(StringKey.MergeFrom), mergeProfile.Name),
                destProfile))
            {
                this.PropagateExternalChange(previous);
            }

            return true;
        }

        /// <inheritdoc />
        public void Undo(bool skipInvert = false)
        {
            this.UndoRedo(this.undoStack, this.redoStack, skipInvert);
        }

        /// <inheritdoc />
        public void Redo()
        {
            this.UndoRedo(this.redoStack, this.undoStack);
        }

        /// <inheritdoc />
        public void PushFirst()
        {
            this.pushedFirst = true;
            try
            {
                this.propagatingProfile = true;
                this.NewProfileOpened(this.Current);
                this.Change(this.Current);
                this.ChangeTo(null, this.Current);
                this.ChangeFinal(this.Current, true);
            }
            finally
            {
                this.propagatingProfile = false;
            }
        }

        /// <inheritdoc />
        public bool Save(string profilePathName = null, Profile profile = null)
        {
            if (profilePathName == null)
            {
                profilePathName = this.Current.PathName;
            }

            if (profile == null)
            {
                profile = this.Current;
            }

            var isCurrent = this.IsCurrentPathName(profilePathName);
            if (isCurrent && profile.ReadOnly)
            {
                Trace.Line(Trace.Type.Profile, "Read-only profile '{0}': not saving", profile.Name);
                return true;
            }

            // Open the file stream.
            FileStream stream = null;
            Exception streamException = null;
            for (int tries = 0; tries < 5; tries++)
            {
                try
                {
                    if (isCurrent)
                    {
                        stream = this.currentFileStream;
                    }
                    else
                    {
                        stream = new FileStream(profilePathName, FileMode.Create);
                    }

                    break;
                }
                catch (Exception e)
                {
                    streamException = e;
                }

                Thread.Sleep(50);
            }

            if (stream == null)
            {
                ErrorBox.Show(streamException.Message, I18n.Get(Title.ProfileSave));
                return false;
            }

            // Serialize the profile.
            try
            {
                SerializeProfile(stream, profile);
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ProfileSave));
                return false;
            }
            finally
            {
                if (!isCurrent)
                {
                    stream.Close();
                }
            }

            return true;
        }

        /// <inheritdoc />
        public bool SaveDefault(out FileStream stream)
        {
            stream = null;
            try
            {
                // Create the file.
                stream = new FileStream(DefaultProfilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);

                // Serialize the profile into it.
                SerializeProfile(stream, Profile.DefaultProfile);
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ProfileSave));
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }

                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void ProfileError(string message)
        {
            if (this.suppressErrors)
            {
                this.errors.Add(message);
            }
            else
            {
                ErrorBox.Show(message, I18n.Get(Title.ProfileError));
            }
        }

        /// <inheritdoc />
        public void DumpErrors()
        {
            this.suppressErrors = false;
            if (this.errors.Any())
            {
                ErrorBox.Show(string.Join(Environment.NewLine, this.errors), I18n.Get(Title.ProfileError));
                this.errors.Clear();
            }
        }

        /// <inheritdoc />
        public bool PushAndSave(ChangeAction action, string what, Profile profileToChange = null, IRefocus refocus = null)
        {
            // Resolve which profile to modify.
            var isCurrent = profileToChange == null || this.IsCurrentPathName(profileToChange.PathName);
            var profile = isCurrent ? this.Current : profileToChange;

            // Run the action on a copy.
            var trial = profile.Clone();
            action(trial);
            if (!trial.Equals(profile))
            {
                if (isCurrent)
                {
                    // Short-circuit bad behavior.
                    if (this.propagatingProfile)
                    {
                        Trace.Line(Trace.Type.Profile, "Error: attempt to change profile while propagating profile");
                        return true;
                    }

                    // Short-circuit other bad behavior.
                    if (!this.pushedFirst)
                    {
                        Trace.Line(Trace.Type.Profile, "Error: attempt to push a change before first profile is pushed out");
                        return true;
                    }
                }

                // The profile was changed.
                // Save the current profile on the Undo stack.
                this.undoStack.Push(new ProfileChangeConfigAction(this, profile, what, refocus: refocus));
                if (isCurrent)
                {
                    this.Current = trial;
                }

                // No more redos.
                this.redoStack.Clear();

                // Undos and redos changed.
                this.UndoRedoChanged();

                // Save the new one.
                if (isCurrent)
                {
                    this.Save();
                }
                else
                {
                    this.Save(profileToChange.PathName, trial);
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void RegisterMerge(ImportType importType, MergeHandler handler)
        {
            this.mergeHandlers.Add(new Tuple<ImportType, MergeHandler>(importType, handler));
        }

        /// <inheritdoc />
        public void RegisterUndoRedo(Button undo, Button redo, ToolTip toolTip)
        {
            this.undoControls.Add(new UndoRedoControl(undo, toolTip));
            this.redoControls.Add(new UndoRedoControl(redo, toolTip));
        }

        /// <inheritdoc />
        public void RegisterUndoRedo(ToolStripMenuItem undo, ToolStripMenuItem redo, ToolTip toolTip)
        {
            this.undoControls.Add(new UndoRedoControl(undo, toolTip));
            this.redoControls.Add(new UndoRedoControl(redo, toolTip));
        }

        /// <inheritdoc />
        public void SetToDefaults()
        {
            if (!this.Current.Equals(Profile.DefaultProfile))
            {
                // Save the current settings.
                var oldCurrent = this.Current;
                this.undoStack.Push(new ProfileChangeConfigAction(this, this.Current, I18n.Get(StringKey.SetToDefaults)));

                // Replace with defaults, preserving macros and hosts.
                this.Current = Profile.DefaultProfile.Clone(this.Current.Name, this.Current.PathName);
                this.Current.Macros = oldCurrent.Macros.Select(m => new MacroEntry(m)).ToList();
                this.Current.Hosts = oldCurrent.Hosts.Select(h => new HostEntry(h)).ToList();

                // No more redos.
                this.redoStack.Clear();

                // Undos and redos changed.
                this.UndoRedoChanged();

                // Save the new one.
                this.Save();

                // Tell everyone.
                this.PropagateExternalChange(oldCurrent);
            }
        }

        /// <inheritdoc />
        public void FlushUndoRedo()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();
            this.UndoRedoChanged();
        }

        /// <inheritdoc />
        public void PushConfigAction(ConfigAction action)
        {
            this.undoStack.Push(action);
            this.redoStack.Clear();
            this.UndoRedoChanged();
        }

        /// <inheritdoc />
        public bool IsCurrentPathName(string profilePathName)
        {
            return profilePathName.Equals(this.Current.PathName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public bool IsDefaultPathName(string profilePathName)
        {
            var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.RegistryKey);
            var defaultProfile = (string)key.GetValue(DefaultProfileRegistryValue);
            key.Close();
            return defaultProfile != null && defaultProfile.Equals(profilePathName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public void Refocus(string profileDir, string profileName = null, string hostName = null)
        {
            this.RefocusEvent(profileDir, profileName, hostName);
        }

        /// <inheritdoc />
        public void SetDefaultProfile(Profile profile)
        {
            // Remember the old value and set the new one.
            var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.RegistryKey);
            var oldValue = (string)key.GetValue(DefaultProfileRegistryValue);
            key.SetValue(DefaultProfileRegistryValue, profile.PathName);
            key.Close();

            // Set up undo/redo.
            this.PushConfigAction(new DefaultProfileConfigAction(string.Format(I18n.Get(StringKey.ChangeDefaultProfile), profile.Name), oldValue ?? DefaultProfilePath, profile.PathName, this));

            // Notify.
            this.DefaultProfileChanged();
        }

        /// <inheritdoc />
        public void Close(Profile profile = null)
        {
            if (profile == null)
            {
                profile = this.Current;
            }

            // Close the current file stream.
            if (this.currentFileStream != null)
            {
                // If this is a writable profile, give folks a chance to modify it.
                if (!profile.ReadOnly)
                {
                    var trial = profile.Clone();
                    this.ProfileClosing(trial);
                    if (!trial.Equals(profile))
                    {
                        // Save the profile.
                        try
                        {
                            SerializeProfile(this.currentFileStream, trial);
                        }
                        catch (Exception e)
                        {
                            ErrorBox.Show(e.Message, I18n.Get(Title.ProfileSave));
                        }
                    }
                }

                this.currentFileStream.Close();
                this.currentFileStream = null;
            }
        }

        /// <inheritdoc />
        public string ChangeName(string text)
        {
            return string.Format(I18n.Get(StringKey.Change), text);
        }

        /// <inheritdoc />
        public string DisableName(string text)
        {
            return string.Format(I18n.Get(StringKey.Disable), text);
        }

        /// <summary>
        /// Read a profile.
        /// </summary>
        /// <param name="profilePath">Profile name.</param>
        /// <param name="error">Returned error message.</param>
        /// <param name="warning">Returned warning message.</param>
        /// <param name="locked">True to return a locked file stream.</param>
        /// <param name="openStream">Returned open stream.</param>
        /// <param name="busy">Returned true if the file is busy.</param>
        /// <param name="notFound">Returned true if the file was not found.</param>
        /// <param name="oldVersion">Returned old version if the file is from an old version and is opened read/write.</param>
        /// <returns>Profile, or null.</returns>
        private static Profile Read(string profilePath, out string error, out string warning, bool locked, out FileStream openStream, out bool busy, out bool notFound, out Profile.VersionClass oldVersion)
        {
            // No errors to begin with.
            error = null;
            warning = null;

            // No returned stream.
            openStream = null;
            busy = false;
            notFound = false;
            oldVersion = null;

            // Open the profile file. It might be busy, so try a few times.
            FileStream stream = null;
            for (var tries = 0; tries < 5; tries++)
            {
                try
                {
                    if (locked)
                    {
                        stream = new FileStream(profilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                    }
                    else
                    {
                        stream = new FileStream(profilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    }

                    break;
                }
                catch (DirectoryNotFoundException e)
                {
                    error = e.Message;
                    notFound = true;
                    return null;
                }
                catch (FileNotFoundException e)
                {
                    error = e.Message;
                    notFound = true;
                    return null;
                }
                catch (UnauthorizedAccessException e)
                {
                    // Save the error message in case the file can't be read at all, but switch
                    // to read-only mode and try again.
                    error = e.Message;
                    locked = false;
                    busy = true;
                }
                catch (Exception e)
                {
                    if (e is IOException && e.HResult == -2147024864)
                    {
                        busy = true;
                    }

                    error = e.Message;
                }

                Thread.Sleep(50);
            }

            if (stream == null)
            {
                return null;
            }

            Profile profile = null;
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                // Deserialize the stream.
                JsonSerializer serializer = new JsonSerializer();
                try
                {
                    profile = (Profile)serializer.Deserialize(reader, typeof(Profile));
                }
                catch (Exception e)
                {
                    error = profilePath + ":" + Environment.NewLine + e.Message;
                    stream.Close();
                    return null;
                }
            }

            if (profile == null)
            {
                error = I18n.Get(Message.CannotDeserializeProfile);
                stream.Close();
                return null;
            }

            // Handle version number mismatches.
            var thisVersion = Profile.VersionClass.GetThis();
            if (profile.Version > thisVersion)
            {
                warning = string.Format(I18n.Get(Message.ProfileVersionMismatch), profile.Version, thisVersion);
            }
            else if (profile.Version < thisVersion && locked)
            {
                oldVersion = profile.Version;
            }

            // Up- or downgrade the version number for saving later.
            profile.Version = thisVersion;

            // Remember the profile name and path.
            profile.Name = Path.GetFileNameWithoutExtension(profilePath);
            profile.PathName = profilePath;

            // Back-reference the profile.
            foreach (var host in profile.Hosts)
            {
                host.Profile = profile;
            }

            if (locked)
            {
                openStream = stream;
            }
            else
            {
                stream.Close();
            }

            return profile;
        }

        /// <summary>
        /// Compute the value to display for an Undo or Redo button.
        /// </summary>
        /// <param name="symbol">Undo or redo symbol.</param>
        /// <param name="count">Count to reflect.</param>
        /// <returns>Button text.</returns>
        private static string UndoValue(string symbol, int count)
        {
            if (count == 0)
            {
                return symbol;
            }

            if (count < 10)
            {
                return symbol + count;
            }

            return symbol + "+";
        }

        /// <summary>
        /// Serialize a profile.
        /// </summary>
        /// <param name="stream">Open stream to write to.</param>
        /// <param name="profile">Profile to serialize.</param>
        private static void SerializeProfile(FileStream stream, Profile profile)
        {
            // Lock the first byte, so readers won't see an incomplete file.
            stream.Lock(0, 1);

            // Rewind.
            stream.Position = 0;
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                // Serialize the stream, indented for human readability.
                var serializer = new JsonSerializer();
                var jsonWriter = new JsonTextWriter(writer)
                {
                    Formatting = Formatting.Indented,
                };
                serializer.Serialize(jsonWriter, profile);
            }

            // Truncate the file here, in case we are overwriting it.
            stream.SetLength(stream.Position);

            // Unlock.
            stream.Unlock(0, 1);
        }

        /// <summary>
        /// Load a profile, i.e., make some profile current.
        /// </summary>
        /// <param name="profilePath">Full profile pathname.</param>
        /// <param name="outProfilePath">Returned full profile path.</param>
        /// <param name="readOnly">If true, open read-only.</param>
        /// <param name="doErrorPopups">If true, do pop-ups for errors.</param>
        /// <returns>True if load was successful.</returns>
        private bool LoadInternal(string profilePath, out string outProfilePath, bool readOnly, bool doErrorPopups)
        {
            // Allow the profile to be under-specified.
            if (profilePath != null)
            {
                if (!profilePath.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                {
                    profilePath += Suffix;
                }

                if (!Path.IsPathRooted(profilePath))
                {
                    if (File.Exists(Path.Combine(SeedProfileDirectory, profilePath)))
                    {
                        profilePath = Path.Combine(SeedProfileDirectory, profilePath);
                    }
                    else
                    {
                        profilePath = Path.GetFullPath(profilePath);
                    }
                }
            }

            outProfilePath = profilePath;

            if (profilePath != null && this.IsCurrentPathName(profilePath))
            {
                // Re-loading the current profile is a successful no-op.
                return true;
            }

            // Remember the previous profile, whatever it is.
            var previous = this.Current?.Clone();

            if (profilePath == null)
            {
                profilePath = DefaultProfilePath;
            }

            var isDefault = profilePath.Equals(DefaultProfilePath, StringComparison.InvariantCultureIgnoreCase);

            var profileName = Path.GetFileNameWithoutExtension(profilePath);

            string error;
            string warning;
            FileStream stream = null;
            bool busy;
            bool notFound;
            Profile.VersionClass oldVersion = null;
            Profile profile;
            if (readOnly)
            {
                profile = Read(profilePath, out error, out warning, out busy, out notFound);
            }
            else
            {
                profile = Read(profilePath, out error, out warning, locked: true, out stream, out busy, out notFound, out oldVersion);
            }

            if (profile != null)
            {
                profile.ReadOnly = readOnly || stream == null;
                this.Current = profile;
            }
            else
            {
                if (busy)
                {
                    // Open read-only.
                    profile = Read(profilePath, out error, out warning, out busy, out notFound);
                    if (profile == null)
                    {
                        if (doErrorPopups)
                        {
                            ErrorBox.Show(error, I18n.Get(Title.ProfileOpen));
                        }

                        return false;
                    }

                    profile.ReadOnly = true;
                    this.Current = profile;
                }
                else if (isDefault && notFound)
                {
                    // Create the default profile.
                    if (!this.SaveDefault(out stream))
                    {
                        return false;
                    }

                    this.Current = Profile.DefaultProfile.Clone(DefaultProfileName, ProfilePath(DefaultProfileName));
                }
                else
                {
                    if (error != null)
                    {
                        if (doErrorPopups)
                        {
                            ErrorBox.Show(error, I18n.Get(Title.ProfileLoad));
                        }
                    }

                    if (stream != null)
                    {
                        stream.Close();
                    }

                    return false;
                }
            }

            if (warning != null)
            {
                ErrorBox.Show(warning, I18n.Get(Title.ProfileLoad), MessageBoxIcon.Warning);
            }

            // Pending macro recordings are a disaster waiting to happen when we switch profiles.
            this.App.MacroRecorder.Abort();

            // Close the previous profile.
            this.Close(previous);

            // Tell folks that we have opened a new profile.
            this.NewProfileOpened(this.Current);

            // Tell everyone else about it.
            this.PropagateExternalChange(previous, isNew: true);

            // Switch to the new one.
            this.currentFileStream = stream;

            // Tell interested parties that we've loaded up an old version.
            if (oldVersion != null)
            {
                var saved = false;
                this.OldVersion(oldVersion, ref saved);
                if (!saved)
                {
                    // Write the profile back out with the new version number applied.
                    this.Save();
                }
            }

            return true;
        }

        /// <summary>
        /// Undo or redo a change.
        /// </summary>
        /// <param name="from">Stack to get new state from.</param>
        /// <param name="to">Stack to add existing state to.</param>
        /// <param name="skipInvert">True to skip inversion.</param>
        private void UndoRedo(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert = false)
        {
            if (from.Any())
            {
                from.Peek().Apply(from, to, skipInvert);
                this.UndoRedoChanged();
            }
        }

        /// <summary>
        /// Change the Undo and Redo buttons.
        /// </summary>
        private void UndoRedoChanged()
        {
            var enabled = this.UndoCount > 0;
            var buttonText = UndoValue("↶", this.UndoCount);
            var toolTip = enabled ? string.Format(I18n.Get(StringKey.Undo), this.undoStack.Peek().What) : null;
            foreach (var undo in this.undoControls)
            {
                var button = undo.Button;
                if (button != null)
                {
                    button.Text = buttonText;
                    button.Enabled = enabled;
                    if (enabled)
                    {
                        undo.ToolTip.SetToolTip(button, toolTip);
                    }
                }
                else
                {
                    var menuItem = undo.MenuItem;
                    menuItem.Enabled = enabled;
                    menuItem.ToolTipText = toolTip;
                }
            }

            enabled = this.RedoCount > 0;
            buttonText = UndoValue("↷", this.RedoCount);
            toolTip = enabled ? string.Format(I18n.Get(StringKey.Redo), this.redoStack.Peek().What) : null;
            foreach (var redo in this.redoControls)
            {
                var button = redo.Button;
                if (button != null)
                {
                    button.Text = buttonText;
                    button.Enabled = enabled;
                    if (enabled)
                    {
                        redo.ToolTip.SetToolTip(button, toolTip);
                    }
                }
                else
                {
                    var menuItem = redo.MenuItem;
                    menuItem.Enabled = enabled;
                    menuItem.ToolTipText = toolTip;
                }
            }
        }

        /// <summary>
        /// Propagate an external profile change.
        /// </summary>
        /// <param name="previous">Previous profile.</param>
        /// <param name="isNew">True if the profile is new (false if re-reading).</param>
        private void PropagateExternalChange(Profile previous, bool isNew = false)
        {
            // Tell everyone about it.
            try
            {
                this.propagatingProfile = true;
                this.Change(this.Current);
                this.ChangeTo(previous, this.Current);
                this.ChangeFinal(this.Current, isNew);
            }
            finally
            {
                this.propagatingProfile = false;
            }
        }

        /// <summary>
        /// Undo/redo for a profile change.
        /// </summary>
        private class ProfileChangeConfigAction : ConfigAction
        {
            /// <summary>
            /// The refocus handler.
            /// </summary>
            private readonly IRefocus refocus;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileChangeConfigAction"/> class.
            /// </summary>
            /// <param name="profileManager">Profile manager.</param>
            /// <param name="profile">Affected profile.</param>
            /// <param name="what">Description of change.</param>
            /// <param name="qualify">True to qualify text with profile name.</param>
            /// <param name="refocus">Refocus handler.</param>
            public ProfileChangeConfigAction(ProfileManager profileManager, Profile profile, string what, bool qualify = true, IRefocus refocus = null)
                : base(what, qualify ? profile.Name : null)
            {
                this.ProfileManager = profileManager;
                this.Profile = profile;
                this.refocus = refocus;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets or sets the profile manager.
            /// </summary>
            public ProfileManager ProfileManager { get; set; }

            /// <summary>
            /// Gets or sets the profile.
            /// </summary>
            public Profile Profile { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this is an undo.
            /// </summary>
            private bool IsUndo { get; set; }

            /// <summary>
            /// Undo or redo a change.
            /// </summary>
            /// <param name="from">Stack to get new state from.</param>
            /// <param name="to">Stack to add new state to.</param>
            /// <param name="skipInvert">True to skip inversion.</param>
            public override void Apply(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert)
            {
                if (!from.Any())
                {
                    return;
                }

                // Figure out which profile we are operating on.
                var peekProfile = ((ProfileChangeConfigAction)from.Peek()).Profile;
                Profile profile = null;
                var isCurrent = this.ProfileManager.IsCurrentPathName(peekProfile.PathName);
                if (isCurrent)
                {
                    profile = this.ProfileManager.Current;
                }
                else
                {
                    foreach (var root in this.ProfileManager.profileTree)
                    {
                        root.ForEach((node) =>
                        {
                            if (node is ProfileWatchNode profileNode && profileNode.PathName == peekProfile.PathName)
                            {
                                profile = profileNode.Profile;
                            }
                        });
                        if (profile != null)
                        {
                            break;
                        }
                    }

                    if (profile == null)
                    {
                        Trace.Line(Trace.Type.Profile, "UndoRedo: no profile to modify");
                        return;
                    }
                }

                // Safety check. Not sure if this still works.
                bool safe = true;
                this.ProfileManager.SafetyCheck(profile, peekProfile, ref safe);
                if (!safe)
                {
                    ErrorBox.Show(I18n.Get(Message.CannotChangeProfile), I18n.Get(Title.UndoRedoError));
                    return;
                }

                // Move current to 'to', 'from' to current.
                if (!skipInvert)
                {
                    to.Push(new ProfileChangeConfigAction(this.ProfileManager, profile, from.Peek().What, qualify: false, refocus: this.refocus) { IsUndo = !this.IsUndo });
                }

                var newProfile = (from.Pop() as ProfileChangeConfigAction).Profile;
                if (isCurrent)
                {
                    var previous = this.ProfileManager.Current;
                    this.ProfileManager.Current = newProfile;

                    // Tell everyone.
                    this.ProfileManager.PropagateExternalChange(previous);
                }

                // Refocus.
                if (this.refocus != null)
                {
                    this.refocus.Inform(this.IsUndo);
                }

                // Save it.
                if (isCurrent)
                {
                    this.ProfileManager.Save();
                }
                else
                {
                    this.ProfileManager.Save(profile.PathName, newProfile);
                }
            }
        }

        /// <summary>
        /// Undo/redo for a default profile name change.
        /// </summary>
        private class DefaultProfileConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultProfileConfigAction"/> class.
            /// </summary>
            /// <param name="what">Description of change.</param>
            /// <param name="oldPath">Old path to default profile.</param>
            /// <param name="newPath">New path to default profile.</param>
            /// <param name="profileManager">Profile manager.</param>
            public DefaultProfileConfigAction(string what, string oldPath, string newPath, ProfileManager profileManager)
                : base(what)
            {
                this.OldPath = oldPath;
                this.NewPath = newPath;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets or sets the old path.
            /// </summary>
            public string OldPath { get; set; }

            /// <summary>
            /// Gets or sets the new path.
            /// </summary>
            public string NewPath { get; set; }

            /// <summary>
            /// Gets or sets the profile manager.
            /// </summary>
            public ProfileManager ProfileManager { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this is an undo.
            /// </summary>
            private bool IsUndo { get; set; }

            /// <summary>
            /// Undo or redo a change.
            /// </summary>
            /// <param name="from">Stack to get new state from.</param>
            /// <param name="to">Stack to add new state to.</param>
            /// <param name="skipInvert">True to skip creating the inverse.</param>
            public override void Apply(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert)
            {
                if (!from.Any())
                {
                    return;
                }

                // Figure out which profile we are operating on.
                var op = (DefaultProfileConfigAction)from.Pop();
                if (op.IsUndo)
                {
                    var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.RegistryKey);
                    key.SetValue(DefaultProfileRegistryValue, op.OldPath);
                    key.Close();
                }
                else
                {
                    var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.RegistryKey);
                    key.SetValue(DefaultProfileRegistryValue, op.NewPath);
                    key.Close();
                }

                // Update the profile tree, which displays the default profile.
                this.ProfileManager.DefaultProfileChanged();

                // Push and invert.
                if (!skipInvert)
                {
                    to.Push(new DefaultProfileConfigAction(this.What, this.OldPath, this.NewPath, this.ProfileManager) { IsUndo = !this.IsUndo });
                }
            }
        }

        /// <summary>
        /// Undo/Redo control.
        /// </summary>
        private class UndoRedoControl : Tuple<Button, ToolStripMenuItem, ToolTip>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UndoRedoControl"/> class.
            /// </summary>
            /// <param name="control">Button control.</param>
            /// <param name="toolTip">Tool tip.</param>
            public UndoRedoControl(Button control, ToolTip toolTip)
                : base(control, null, toolTip)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="UndoRedoControl"/> class.
            /// </summary>
            /// <param name="menuItem">Menu item.</param>
            /// <param name="toolTip">Tool tip.</param>
            public UndoRedoControl(ToolStripMenuItem menuItem, ToolTip toolTip)
                : base(null, menuItem, toolTip)
            {
            }

            /// <summary>
            /// Gets the button.
            /// </summary>
            public Button Button => this.Item1;

            /// <summary>
            /// Gets the menu item.
            /// </summary>
            public ToolStripMenuItem MenuItem => this.Item2;

            /// <summary>
            /// Gets the tool tip.
            /// </summary>
            public ToolTip ToolTip => this.Item3;
        }

        /// <summary>
        /// Localized string keys.
        /// </summary>
        private class StringKey
        {
            /// <summary>
            /// Base profile name.
            /// </summary>
            public static readonly string Base = I18n.Combine(StringName, "Base");

            /// <summary>
            /// Undo label.
            /// </summary>
            public static readonly string Undo = I18n.Combine(StringName, "undo");

            /// <summary>
            /// Redo label.
            /// </summary>
            public static readonly string Redo = I18n.Combine(StringName, "redo");

            /// <summary>
            /// Change label.
            /// </summary>
            public static readonly string Change = I18n.Combine(StringName, "change");

            /// <summary>
            /// External label.
            /// </summary>
            public static readonly string External = I18n.Combine(StringName, "external");

            /// <summary>
            /// Merge-from label.
            /// </summary>
            public static readonly string MergeFrom = I18n.Combine(StringName, "mergeFrom");

            /// <summary>
            /// Set to defaults label.
            /// </summary>
            public static readonly string SetToDefaults = I18n.Combine(StringName, "setToDefaults");

            /// <summary>
            /// Change default profile label.
            /// </summary>
            public static readonly string ChangeDefaultProfile = I18n.Combine(StringName, "changeDefaultProfile");

            /// <summary>
            /// Default values label.
            /// </summary>
            public static readonly string DefaultValuesName = I18n.Combine(StringName, "defaultValues");

            /// <summary>
            /// No-profile label.
            /// </summary>
            public static readonly string NoProfile = I18n.Combine(StringName, "noProfile");

            /// <summary>
            /// Read-only profile label.
            /// </summary>
            public static readonly string ReadOnly = I18n.Combine(StringName, "readOnly");

            /// <summary>
            /// Option disable.
            /// </summary>
            public static readonly string Disable = I18n.Combine(StringName, "disable");
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Default profile change.
            /// </summary>
            public static readonly string DefaultProfileChange = I18n.Combine(TitleName, "defaultProfileChange");

            /// <summary>
            /// Profile directory error.
            /// </summary>
            public static readonly string ProfileDirectoryError = I18n.Combine(TitleName, "profileDirectoryError");

            /// <summary>
            /// Profile icon error.
            /// </summary>
            public static readonly string ProfileIconError = I18n.Combine(TitleName, "profileIconError");

            /// <summary>
            /// Profile desktop INI error.
            /// </summary>
            public static readonly string ProfileDesktopIniError = I18n.Combine(TitleName, "profileDesktopIniError");

            /// <summary>
            /// Profile open error.
            /// </summary>
            public static readonly string ProfileOpen = I18n.Combine(TitleName, "profileOpen");

            /// <summary>
            /// Profile load error.
            /// </summary>
            public static readonly string ProfileLoad = I18n.Combine(TitleName, "profileLoad");

            /// <summary>
            /// Profile merge error.
            /// </summary>
            public static readonly string ProfileMerge = I18n.Combine(TitleName, "profileMerge");

            /// <summary>
            /// Profile save error.
            /// </summary>
            public static readonly string ProfileSave = I18n.Combine(TitleName, "profileSave");

            /// <summary>
            /// Profile error.
            /// </summary>
            public static readonly string ProfileError = I18n.Combine(TitleName, "profileError");

            /// <summary>
            /// Undo/redo error.
            /// </summary>
            public static readonly string UndoRedoError = I18n.Combine(TitleName, "undoRedoError");
        }

        /// <summary>
        /// Message box text.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Profile is busy.
            /// </summary>
            public static readonly string ProfileBusy = I18n.Combine(MessageName, "profileBusy");

            /// <summary>
            /// Profile cannot be changed.
            /// </summary>
            public static readonly string CannotChangeProfile = I18n.Combine(MessageName, "cannotChangeProfile");

            /// <summary>
            /// Profile cannot be changed.
            /// </summary>
            public static readonly string CannotDeserializeProfile = I18n.Combine(MessageName, "cannotDeserializeProfile");

            /// <summary>
            /// Profile version mismatch.
            /// </summary>
            public static readonly string ProfileVersionMismatch = I18n.Combine(MessageName, "profileVersionMismatch");
        }
    }
}
