// <copyright file="ProfileDialog.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace X3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// The profile dialog.
    /// </summary>
    public partial class ProfileDialog : Form
    {
        /// <summary>
        /// Application instance.
        /// </summary>
        private X3270App app;

        /// <summary>
        /// The main screen.
        /// </summary>
        private MainScreen mainScreen;

        /// <summary>
        /// The host connect instance.
        /// </summary>
        private Connect connect;

        /// <summary>
        /// True if the dialog has ever been activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// The name of a profile to auto-rename, as soon as it appears.
        /// </summary>
        private string autoRename;

        /// <summary>
        /// The reason for auto-rename.
        /// </summary>
        private string autoRenameReason;

        /// <summary>
        /// The name of a profile to set the index to, when the list changes.
        /// </summary>
        private string autoIndex;

        /// <summary>
        /// The window handle.
        /// </summary>
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileDialog"/> class.
        /// </summary>
        /// <param name="app">Application instance</param>
        /// <param name="mainScreen">Main screen</param>
        /// <param name="connect">Connect machine</param>
        public ProfileDialog(X3270App app, MainScreen mainScreen, Connect connect)
        {
            this.InitializeComponent();

            this.app = app;
            this.mainScreen = mainScreen;
            this.connect = connect;

            // No feedback yet.
            this.feedbackLabel.Text = string.Empty;

            // Set up the profile list.
            this.profilesListBox.Items.AddRange(ProfileManager.ListProfiles().ToArray());

            // Set up the profile load dialog.
            this.mainScreen.loadMenuItem.DropDownItems.Clear();
            foreach (var profileName in ProfileManager.ListProfiles().Where(p => !p.Equals(ProfileManager.DefaultValuesName)))
            {
                this.mainScreen.loadMenuItem.DropDownItems.Add(profileName, null, this.LoadProfileFromMenu);
                this.mainScreen.loadMenuItem.DropDownItems[this.mainScreen.loadMenuItem.DropDownItems.Count - 1].Tag = profileName;
            }

            // Subscribe to profile change events -- last.
            this.ProfileManager.ChangeFinal += this.ProfileProfileChanged;

            // Subscribe to profile list changes.
            this.handle = this.Handle;
            this.ProfileManager.ListChange += (names) => { this.Invoke(new MethodInvoker(() => this.ProfileListChanged(names))); };

            // Subscribe to connection state events.
            this.mainScreen.ConnectionStateEvent += this.ProfileConnectionChange;
        }

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private ProfileManager ProfileManager
        {
            get { return this.app.ProfileManager; }
        }

        /// <summary>
        /// Set or add a name to the profile list.
        /// </summary>
        /// <param name="name">Profile name</param>
        /// <param name="addName">If true, add the name if it is not there</param>
        private void SetProfileName(string name, bool addName = true)
        {
            var index = this.profilesListBox.FindStringExact(name);
            if (index != ListBox.NoMatches)
            {
                this.profilesListBox.SetSelected(index, true);
            }
            else if (addName)
            {
                this.profilesListBox.Items.Add(name);
                this.profilesListBox.SetSelected(this.profilesListBox.FindStringExact(name), true);
            }
            else
            {
                this.profilesListBox.ClearSelected();
            }
        }

        /// <summary>
        /// Profile change handler for the Profile dialog.
        /// </summary>
        /// <param name="profile">New profile</param>
        private void ProfileProfileChanged(Profile profile)
        {
            // Change the selection in the list of profiles.
            this.SetProfileName(profile.Name);
            this.currentProfileLabel.Text = "Current profile: " + profile.Name;

            // Redraw the profiles list.
            this.profilesListBox.Invalidate();
#if false
            // Look for an auto-connect host.
            if (this.autoConnectCheckBox.Checked)
            {
                foreach (var entry in profile.Hosts)
                {
                    if (entry.AutoConnect == AutoConnect.Connect || entry.AutoConnect == AutoConnect.Reconnect)
                    {
                        this.connect.ConnectToHost(entry);
                        break;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Connection change handler for the Profile dialog.
        /// </summary>
        private void ProfileConnectionChange()
        {
            var connectionState = this.app.ConnectionState;
            var connected = connectionState != ConnectionState.NotConnected;

            this.switchToButton.Enabled = !connected;
            this.mergeFromButton.Enabled = !connected;
            this.deleteButton.Enabled = !connected;
        }

        /// <summary>
        /// The list of profiles changed.
        /// </summary>
        /// <param name="profileNames">New list</param>
        private void ProfileListChanged(IEnumerable<string> profileNames)
        {
            // Set up the profiles list.
            this.profilesListBox.Items.Clear();
            var namesArray = profileNames.ToArray();
            this.profilesListBox.Items.AddRange(namesArray);

            // Set the current profile (again?).
            this.SetProfileName(this.ProfileManager.Current.Name, false);

            // Set up the load menu.
            this.mainScreen.loadMenuItem.DropDownItems.Clear();
            foreach (var profileName in namesArray)
            {
                this.mainScreen.loadMenuItem.DropDownItems.Add(profileName, null, this.LoadProfileFromMenu);
                this.mainScreen.loadMenuItem.DropDownItems[this.mainScreen.loadMenuItem.DropDownItems.Count - 1].Tag = profileName;
            }

            // Do auto-rename or auto-index.
            if (this.autoRename != null)
            {
                var renameIndex = this.profilesListBox.FindStringExact(this.autoRename);
                if (renameIndex != ListBox.NoMatches)
                {
                    this.profilesListBox.SelectedIndex = renameIndex;
                    this.PopUpRenameBox(this.autoRename, this.autoRenameReason);
                    this.autoRename = null;
                }
            }
            else if (this.autoIndex != null)
            {
                var index = this.profilesListBox.FindStringExact(this.autoIndex);
                if (index != ListBox.NoMatches)
                {
                    this.profilesListBox.SelectedIndex = index;
                }

                this.autoIndex = null;
            }
        }

        /// <summary>
        /// Load a profile from the menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void LoadProfileFromMenu(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var profileName = item.Tag as string;
            if (this.ProfileManager.LoadSimple(profileName))
            {
                this.feedbackLabel.Text = "Profile loaded";
            }
        }

        /// <summary>
        /// The profile Import button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileImportButton_Click(object sender, EventArgs e)
        {
            this.LoadProfileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            this.LoadProfileDialog.FileName = string.Empty;
            var result = this.LoadProfileDialog.ShowDialog();
            switch (result)
            {
                default:
                case DialogResult.Cancel:
                    return;
                case DialogResult.OK:
                    break;
            }

            var importName = this.LoadProfileDialog.FileName;
            if (Path.GetDirectoryName(importName).Equals(ProfileManager.ProfileDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("Cannot import from profile directory", "Profile Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Path.GetExtension(importName).Equals(ProfileManager.Suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show("Invalid file name", "Profile Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                File.Copy(importName, Path.Combine(ProfileManager.ProfileDirectory, Path.GetFileName(importName)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Profile Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.feedbackLabel.Text = "Profile imported";
        }

        /// <summary>
        /// The profile delete button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileDeleteButton_Click(object sender, EventArgs e)
        {
            // Set up auto-index.
            if (this.profilesListBox.SelectedIndex < this.profilesListBox.Items.Count - 1)
            {
                // Item after current.
                this.autoIndex = this.profilesListBox.Items[profilesListBox.SelectedIndex + 1].ToString();
            }
            else
            {
                // Item before current.
                this.autoIndex = this.profilesListBox.Items[profilesListBox.SelectedIndex - 1].ToString();
            }

            // Delete the profile.
            try
            {
                File.Delete(ProfileManager.ProfilePath(this.profilesListBox.SelectedItem.ToString()));
            }
            catch (Exception exception)
            {
                this.autoIndex = null;
                MessageBox.Show(exception.Message, "Profile Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.feedbackLabel.Text = "Profile deleted";
        }

        /// <summary>
        /// The profile Export button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileSaveAsButton_Click(object sender, EventArgs e)
        {
            if (this.profilesListBox.SelectedIndex == -1)
            {
                return;
            }

            this.SaveProfileDialog.FileName = this.profilesListBox.SelectedItem.ToString() + ProfileManager.Suffix;
            this.SaveProfileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            switch (this.SaveProfileDialog.ShowDialog())
            {
                default:
                case DialogResult.Cancel:
                    return;
                case DialogResult.OK:
                    break;
            }

            // Save the profile.
            if (this.ProfileManager.Save(this.SaveProfileDialog.FileName))
            {
                this.feedbackLabel.Text = "Profile exported";
            }
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        /// <summary>
        /// One of the import check boxes changed state.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ImportCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;

            // Enable the radio buttons, and turn Replace on.
            var panel = checkBox.Parent;
            foreach (var child in panel.Controls)
            {
                var radioButton = child as RadioButton;
                if (radioButton != null)
                {
                    radioButton.Enabled = checkBox.Checked;
                    if (((string)radioButton.Tag).EndsWith("Replace"))
                    {
                        radioButton.Checked = true;
                    }
                }
            }

            // Enable or disable the import buttons.
            var any = false;
            foreach (var childPanel in panel.Parent.Controls.OfType<Panel>())
            {
                any |= childPanel.Controls.OfType<CheckBox>().Any(c => c.Checked);
            }
        }

        /// <summary>
        /// The profile dialog was shown.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileDialog_Shown(object sender, EventArgs e)
        {
            this.feedbackLabel.Text = string.Empty;
        }

        /// <summary>
        /// An entry in the delete profiles list was double-clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfilesListBox_DoubleClick(object sender, EventArgs e)
        {
            // Same as clicking on the load button.
            this.SwitchToButton_Click(sender, e);
        }

        /// <summary>
        /// The profile list box index changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var connected = this.app.ConnectionState != ConnectionState.NotConnected;

            var anySelected = this.profilesListBox.SelectedIndex >= 0;
            var selectedItem = anySelected ? this.profilesListBox.SelectedItem.ToString() : null;

            // Set up SwitchTo, Duplicate, Rename, MergeFrom, Export, Delete.
            var isDefaults = anySelected && selectedItem.Equals(ProfileManager.DefaultValuesName);
            var isBase = anySelected && selectedItem.Equals(ProfileManager.DefaultProfileName);
            var isCurrent = anySelected && selectedItem.Equals(this.ProfileManager.Current.Name);
            this.switchToGroupBox.Enabled = !connected && anySelected && !isDefaults && !isCurrent;
            this.duplicateButton.Enabled = anySelected;
            this.renameButton.Enabled = anySelected && !isDefaults && !isBase;
            this.mergeFromButton.Enabled = !connected && anySelected && !isCurrent;
            this.exportButton.Enabled = anySelected && !isDefaults;
            this.deleteButton.Enabled = anySelected && !isDefaults && !isBase && !isCurrent;

            // You can delete if the current profile is not selected.
            if (isBase)
            {
                this.profileHelpLabel.Text = string.Format("The '{0}' profile is used when wx3270 is started without specifying a profile", ProfileManager.DefaultProfileName);
                this.profileHelpLabel.Visible = true;
            }
            else if (isDefaults)
            {
                this.profileHelpLabel.Text = string.Format("Settings can be imported from '{0}', or it can be duplicated to create a new profile", ProfileManager.DefaultValuesName);
                this.profileHelpLabel.Visible = true;
            }
            else
            {
                this.profileHelpLabel.Visible = false;
            }
        }

        /// <summary>
        /// The selected tab changed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event parameters</param>
        private void ProfileTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When they switch tabs, start with a fresh slate.
            this.feedbackLabel.Text = string.Empty;
        }

        /// <summary>
        /// The profile dialog was activated.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfileDialog_Activated(object sender, EventArgs e)
        {
            if (!this.everActivated)
            {
                this.everActivated = true;
                this.Location = MainScreen.CenteredOn(this.mainScreen, this);
            }
        }

        /// <summary>
        /// Pop up the rename box.
        /// </summary>
        /// <param name="fileName">File name to start with</param>
        /// <param name="opcode">Operation to perform</param>
        private void PopUpRenameBox(string fileName, string opcode)
        {
            var rectangle = this.profilesListBox.GetItemRectangle(this.profilesListBox.SelectedIndex);
            var listBoxLocation = this.profilesListBox.Location;
            this.renameTextBox.Location = new Point(rectangle.X + listBoxLocation.X, rectangle.Y + listBoxLocation.Y);
            this.renameTextBox.Width = rectangle.Width + 2;
            this.renameTextBox.Text = fileName;
            this.renameTextBox.SelectAll();
            this.renameTextBox.Visible = true;
            this.renameTextBox.Tag = opcode;
            this.renameTextBox.Focus();
            this.feedbackLabel.Text = "Enter new profile name";
        }

        /// <summary>
        /// The Rename button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RenameButton_Click(object sender, EventArgs e)
        {
            // Pop up the rename box.
            this.PopUpRenameBox(this.profilesListBox.SelectedItem.ToString(), "renamed");
        }

        /// <summary>
        /// The rename text box was completed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RenameTextBox_Validated(object sender, EventArgs e)
        {
            this.renameTextBox.Visible = false;
            try
            {
                var from = ProfileManager.ProfilePath(this.profilesListBox.SelectedItem.ToString());
                var to = ProfileManager.ProfilePath(this.renameTextBox.Text);
                File.Move(from, to);
                if (from.Equals(this.ProfileManager.Current.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Current profile was renamed.
                    ProfileManager.Load(to);
                }

                this.feedbackLabel.Text = "Profile " + renameTextBox.Tag;
                this.autoIndex = this.renameTextBox.Text;
            }
            catch (Exception ex)
            {
                this.feedbackLabel.Text = string.Empty;
                MessageBox.Show(ex.Message, "Profile Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Validate the rename text box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RenameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var text = this.renameTextBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                this.feedbackLabel.Text = string.Empty;
                this.renameTextBox.Visible = false;
                this.renameTextBox.Tag = "nop";
                return;
            }

            if (!Nickname.ValidNickname(text))
            {
                MessageBox.Show("Invalid name", "Profile Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }

            if (!text.Equals(this.profilesListBox.SelectedItem.ToString(), StringComparison.InvariantCultureIgnoreCase)
                && ProfileManager.ListProfiles().Any(p => p.Equals(text, StringComparison.InvariantCultureIgnoreCase)))
            {
                MessageBox.Show("Duplicate name", "Profile Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// A key was pressed in the rename text box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RenameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\x1b':
                    // Get out.
                    this.renameTextBox.Text = string.Empty;
                    this.renameTextBox.Visible = false;
                    this.feedbackLabel.Text = string.Empty;
                    e.Handled = true;
                    break;
                case '\r':
                case '\n':
                    // Done.
                    this.profilesListBox.Focus();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// The Switch To button was pressed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void SwitchToButton_Click(object sender, EventArgs e)
        {
            if (this.profilesListBox.SelectedItem.ToString().Equals(ProfileManager.DefaultValuesName))
            {
                return;
            }

            if (this.ProfileManager.LoadSimple(this.profilesListBox.SelectedItem.ToString()))
            {
                this.feedbackLabel.Text = "Profile loaded";
            }
        }

        /// <summary>
        /// The Duplicate button was pressed.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void DuplicateButton_Click(object sender, EventArgs e)
        {
            // Find a unique new name.
            var from = this.profilesListBox.SelectedItem.ToString();
            string baseName;
            if (from.Equals(ProfileManager.DefaultValuesName))
            {
                baseName = "New Profile";
            }
            else
            {
                baseName = "Copy of " + from;
            }

            var newName = baseName;
            var n = 0;
            while (ProfileManager.ListProfiles().Any(p => p.Equals(newName, StringComparison.InvariantCultureIgnoreCase)))
            {
                newName = baseName + "(" + ++n + ")";
            }

            // Prime auto-rename when the watcher finds it.
            this.autoRenameReason = "duplicated";
            this.autoRename = newName;

            // Copy the file.
            if (from.Equals(ProfileManager.DefaultValuesName))
            {
                if (!ProfileManager.Save(ProfileManager.ProfilePath(newName), Profile.DefaultProfile))
                {
                    this.autoRename = null;
                    MessageBox.Show("Profile save failed", "Profile Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                try
                {
                    File.Copy(ProfileManager.ProfilePath(from), ProfileManager.ProfilePath(newName));
                }
                catch (Exception ex)
                {
                    this.autoRename = null;
                    MessageBox.Show(ex.Message, "Profile Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        /// <summary>
        /// Draw an item in the profiles list box.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfilesListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            using (var backBrush = new SolidBrush(e.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            var item = this.profilesListBox.Items[e.Index];
            var itemText = this.profilesListBox.GetItemText(item);
            const TextFormatFlags FormatFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
            var font = e.Font;
            var isCurrent = itemText.Equals(this.ProfileManager.Current.Name);
            if (isCurrent)
            {
                font = new Font(e.Font, FontStyle.Bold);
            }

            TextRenderer.DrawText(e.Graphics, itemText, font, e.Bounds, e.ForeColor, FormatFlags);
            if (isCurrent)
            {
                font.Dispose();
            }
        }

        /// <summary>
        /// The New button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void NewButton_Click(object sender, EventArgs e)
        {
            // Find a unique new name.
            var baseName = "New Profile";
            var name = baseName;
            var n = 0;
            while (ProfileManager.ListProfiles().Any(p => p.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                name = baseName + "(" + ++n + ")";
            }

            // Prime auto-rename.
            this.autoRenameReason = "created";
            this.autoRename = name;

            // Create it.
            if (!ProfileManager.Save(ProfileManager.ProfilePath(name), Profile.DefaultProfile))
            {
                this.autoRename = null;
                return; // Oops
            }
        }

        /// <summary>
        /// The Merge button was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void MergeFromButton_Click(object sender, EventArgs e)
        {
            var source = this.profilesListBox.SelectedItem.ToString();
            using (var m = new MergeDialog(this, source, this.app.ProfileManager.Current.Name))
            {
                if (m.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var imports = m.Imports;
                if (imports == ImportType.None)
                {
                    return;
                }

                // XXX: ProfileMerge operates live (eek!)
                if (source.Equals(ProfileManager.DefaultValuesName))
                {
                    if (this.ProfileManager.Merge(Profile.DefaultProfile, imports))
                    {
                        this.feedbackLabel.Text = "Default values merged";
                    }
                }
                else
                {
                    if (this.app.ProfileManager.MergeSimple(source, imports))
                    {
                        this.feedbackLabel.Text = "Profile merged";
                    }
                }
            }
        }

        /// <summary>
        /// The profiles list box was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ProfilesListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the item under the mouse pointer
                var index = this.profilesListBox.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    var connected = this.app.ConnectionState != ConnectionState.NotConnected;
                    var selectedItem = this.profilesListBox.Items[index].ToString();
                    var isBase = selectedItem.Equals(ProfileManager.DefaultProfileName);
                    var isDefaults = selectedItem.Equals(ProfileManager.DefaultValuesName);
                    var isCurrent = selectedItem.Equals(this.ProfileManager.Current.Name);
                    this.switchToToolStripMenuItem.Enabled = !connected && !isDefaults && !isCurrent;
                    this.renameToolStripMenuItem.Enabled = !isDefaults && !isBase;
                    this.mergeFromToolStripMenuItem.Enabled = !connected && !isCurrent;
                    this.exportToolStripMenuItem.Enabled = !isDefaults;
                    this.deleteToolStripMenuItem.Enabled = !isDefaults && !isBase && !isCurrent;

                    this.profilesListBox.SelectedIndex = index;
                    this.contextMenuStrip1.Show(e.Location);
                    this.contextMenuStrip1.Visible = true;
                }
            }
        }

        /// <summary>
        /// A tool strip menu item was clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ContextMenuStrip_Clicked(object sender, EventArgs e)
        {
            switch ((string)((ToolStripMenuItem)sender).Tag)
            {
                case "SwitchTo":
                    this.SwitchToButton_Click(sender, e);
                    break;
                case "Duplicate":
                    this.DuplicateButton_Click(sender, e);
                    break;
                case "MergeFrom":
                    this.MergeFromButton_Click(sender, e);
                    break;
                case "Rename":
                    this.RenameButton_Click(sender, e);
                    break;
                case "Delete":
                    this.ProfileDeleteButton_Click(sender, e);
                    break;
                case "Export":
                    this.ProfileSaveAsButton_Click(sender, e);
                    break;
            }
        }
    }
}
