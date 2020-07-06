// <copyright file="Connection.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace X3270
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// The host connection dialog.
    /// </summary>
    public partial class Connection : Form
    {
        /// <summary>
        /// Tag for profile hosts drag and drop entries.
        /// </summary>
        private const string ProfileTag = "profile";

        /// <summary>
        /// The application instance.
        /// </summary>
        private X3270App app;

        /// <summary>
        /// The main screen.
        /// </summary>
        private MainScreen mainScreen;

        /// <summary>
        /// The host connect logic.
        /// </summary>
        private Connect connect;

        /// <summary>
        /// The profile host list.
        /// </summary>
        private SyncedListBoxes<HostEntry> profileHosts;

        /// <summary>
        /// Drag and drop for the profile hosts.
        /// </summary>
        private DragAndDrop<HostEntry> profileDrag;

        /// <summary>
        /// True if the dialog has ever been activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="app">Application instance</param>
        /// <param name="mainScreen">Main screen</param>
        /// <param name="profileHosts">Profile hosts</param>
        /// <param name="connect">Connect machine</param>
        public Connection(X3270App app, MainScreen mainScreen, SyncedListBoxes<HostEntry> profileHosts, Connect connect)
        {
            this.InitializeComponent();

            this.app = app;
            this.mainScreen = mainScreen;
            this.connect = connect;

            // Add the profile hosts list box.
            this.profileHosts = profileHosts;
            this.profileHosts.AddListBox(this.profileHostsListBox);

            // Set up drag and drop.
            this.profileDrag = new DragAndDrop<HostEntry>(this, profileHosts, null, this.profileHostsListBox, ProfileTag, null);

            // Subscribe to profile change events.
            this.ProfileManager.Change += this.HostProfileChanged;

            // Set up the merge handler.
            this.ProfileManager.RegisterMerge(ImportType.HostsMerge | ImportType.HostsReplace, this.MergeHandler);

            // Subscribe to connection state events.
            this.mainScreen.ConnectionStateEvent += this.HostConnectionChange;

            // Set up the undo and redo buttons.
            this.ProfileManager.RegisterUndoRedo(this.UndoButton, this.RedoButton, this.toolTip1);
        }

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private ProfileManager ProfileManager
        {
            get { return this.app.ProfileManager; }
        }

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private BackEnd BackEnd
        {
            get { return this.app.BackEnd; }
        }

        /// <summary>
        /// Do a quick connect.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void QuickConnect(object sender, EventArgs e)
        {
            this.DoQuickHostAdd(sender, e, HostEditingMode.QuickConnect);
        }

        /// <summary>
        /// Connect to a profile host by name.
        /// </summary>
        /// <param name="entryName">Entry name</param>
        public void ConnectToProfileHost(string entryName)
        {
            var entry = this.profileHosts.Entries.FirstOrDefault(e => e.Name.Equals(entryName));
            if (entry == null)
            {
                return;
            }

            this.connect.ConnectToHost(entry);
        }

        /// <summary>
        /// Merge in the host definitions from another profile.
        /// </summary>
        /// <param name="current">Current profile</param>
        /// <param name="merge">Merge profile</param>
        /// <param name="importType">Import type</param>
        /// <returns>True if the list changed</returns>
        private bool MergeHandler(Profile current, Profile merge, ImportType importType)
        {
            if (importType.HasFlag(ImportType.HostsReplace))
            {
                // Replace host definitions.
                if (!current.Hosts.SequenceEqual(merge.Hosts))
                {
                    current.Hosts = merge.Hosts.Select(host => { var clone = host.Clone(); clone.Profile = current; return clone; });
                    return true;
                }

                return false;
            }
            else
            {
                // Merge host definitions.
                var changed = false;
                var newHosts = current.Hosts.ToDictionary(h => h.Name);
                foreach (var mergeHost in merge.Hosts)
                {
                    HostEntry found;
                    if (!newHosts.TryGetValue(mergeHost.Name, out found) || !mergeHost.Equals(found))
                    {
                        newHosts[mergeHost.Name] = mergeHost;
                        changed = true;
                    }
                }

                if (changed)
                {
                    current.Hosts = newHosts.Values.Select(host => { var clone = host.Clone(); clone.Profile = current; return clone; });
                }

                return changed;
            }
        }

        /// <summary>
        /// Profile change handler for the Host tab.
        /// </summary>
        /// <param name="profile">New profile</param>
        private void HostProfileChanged(Profile profile)
        {
            this.profileHosts.Entries = profile.Hosts;
        }

        /// <summary>
        /// Connection change handler for the Host tab.
        /// </summary>
        private void HostConnectionChange()
        {
            var connectionState = this.app.ConnectionState;

            // Set up UI elements.
            var connected = connectionState != ConnectionState.NotConnected;
            this.DisconnectButton.Enabled = connected;
            this.mainScreen.disconnectMenuItem.Enabled = connected;
            this.mainScreen.quickConnectMenuItem.Enabled = !connected;
            this.mainScreen.connectMenuItem.Enabled = !connected;
        }

        /// <summary>
        /// The quick connect button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void QuickConnectButton_Click(object sender, EventArgs e)
        {
            this.DoQuickHostAdd(sender, e, HostEditingMode.QuickConnect);
        }

        /// <summary>
        /// The Disconnect button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            this.connect.Disconnect();
        }

        /// <summary>
        /// The add button under the profile hosts list box was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostAddButton_Click(object sender, EventArgs e)
        {
            using (var editor = new HostEditor(HostEditingMode.SaveHost, null, this.ProfileManager.Current))
            {
                var result = editor.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    this.profileHosts.Add(editor.HostEntry);
                    this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "add host");
                }
            }
        }

        /// <summary>
        /// The edit button under the profile hosts list box was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostEditButton_Click(object sender, EventArgs e)
        {
            var index = this.profileHostsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            using (var editor = new HostEditor(HostEditingMode.SaveHost, this.profileHosts[index], this.ProfileManager.Current))
            {
                var result = editor.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    this.profileHosts[index] = editor.HostEntry;
                    this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "edit host");
                }
            }
        }

        /// <summary>
        /// The connect button in the profile hosts list box was double-clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostConnectButton_Click(object sender, EventArgs e)
        {
            var index = this.profileHostsListBox.SelectedIndex;
            if (index >= 0)
            {
                this.DoConnect(sender, e, this.profileHosts[index]);
            }
        }

        /// <summary>
        /// The remove button next to the profile hosts list box was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostRemoveButton_Click(object sender, EventArgs e)
        {
            this.profileHosts.Delete(this.profileHostsListBox);
            this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "remove host");
        }

        /// <summary>
        /// An entry in the profile hosts list box was double-clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBox_DoubleClick(object sender, EventArgs e)
        {
            var index = this.profileHostsListBox.SelectedIndex;
            if (index != ListBox.NoMatches)
            {
                this.DoConnect(sender, e, this.profileHosts[index]);
            }
        }

        /// <summary>
        /// The profile hosts list box selection index changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ProfileHostsListBoxSelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Drag-drop event for the profile hosts list box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBox_DragDrop(object sender, DragEventArgs e)
        {
            this.profileDrag.DragDrop(sender, e);
            this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "re-order hosts");
        }

        /// <summary>
        /// Drag-over event for the profile hosts list box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBox_DragOver(object sender, DragEventArgs e)
        {
            this.profileDrag.DragOver(sender, e);
        }

        /// <summary>
        /// The form is being closed (the close button on the border).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Connection_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        /// <summary>
        /// Mouse down event for the profile hosts list box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the item under the mouse pointer
                var index = this.profileHostsListBox.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    var connected = this.app.ConnectionState != ConnectionState.NotConnected;
                    connectToolStripMenuItem.Enabled = !connected;

                    this.profileHostsListBox.SelectedIndex = index;
                    this.contextMenuStrip1.Show(e.Location);
                    this.contextMenuStrip1.Visible = true;
                }
            }
            else
            {
                this.profileDrag.MouseDown(sender, e);
            }
        }

        /// <summary>
        /// Mouse move method for the profile hosts list box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.profileDrag.MouseMove(sender, e);
        }

        /// <summary>
        /// Connect to a host.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        /// <param name="entry">Host entry</param>
        private void DoConnect(object sender, EventArgs e, HostEntry entry)
        {
            this.connect.ConnectToHost(entry);
            this.Hide();
        }

        /// <summary>
        /// Do a quick host add operation.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        /// <param name="mode">Editing mode</param>
        private void DoQuickHostAdd(object sender, EventArgs e, HostEditingMode mode)
        {
            using (var editor = new HostEditor(mode, null, this.ProfileManager.Current))
            {
                var result = editor.ShowDialog(this);
                if (result == DialogResult.Yes || result == DialogResult.OK)
                {
                    // Save.
                    this.profileHosts.Add(editor.HostEntry, true);
                    this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "add host");
                }

                if (result == DialogResult.Yes)
                {
                    // Connect.
                    this.DoConnect(sender, e, editor.HostEntry);
                }
            }
        }

        /// <summary>
        /// The selection index on the profile hosts list box changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileHostsListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            var anySelected = this.profileHostsListBox.SelectedIndex >= 0;
            this.profileHostEditButton.Enabled = anySelected;
            this.profileHostConnectButton.Enabled = anySelected;
            this.profileHostRemoveButton.Enabled = anySelected;
        }

        /// <summary>
        /// The connection dialog was activated.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Connection_Activated(object sender, EventArgs e)
        {
            if (!this.everActivated)
            {
                this.everActivated = true;
                this.Location = MainScreen.CenteredOn(this.mainScreen, this);
            }
        }

        /// <summary>
        /// The Undo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            this.ProfileManager.Undo();
        }

        /// <summary>
        /// The Redo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RedoButton_Click(object sender, EventArgs e)
        {
            this.ProfileManager.Redo();
        }

        /// <summary>
        /// One of the context menu items was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ContextMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            switch ((string)item.Tag)
            {
                case "Connect":
                    this.DoConnect(sender, e, this.profileHosts[this.profileHostsListBox.SelectedIndex]);
                    break;
                case "Edit":
                    var index = this.profileHostsListBox.SelectedIndex;
                    using (var editor = new HostEditor(HostEditingMode.SaveHost, this.profileHosts[index], this.ProfileManager.Current))
                    {
                        var result = editor.ShowDialog(this);
                        if (result == DialogResult.OK)
                        {
                            this.profileHosts[index] = editor.HostEntry;
                            this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "edit host");
                        }
                    }

                    break;
                case "Delete":
                    this.profileHosts.Delete(this.profileHostsListBox);
                    this.ProfileManager.PushAndSave((current) => current.Hosts = this.profileHosts.Entries, "delete host");
                    break;
            }
        }
    }
}
