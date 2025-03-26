// <copyright file="ShortcutDialog.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using I18nBase;

    /// <summary>
    /// Dialog for shortcut options.
    /// </summary>
    public partial class ShortcutDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutDialog"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="path">Initial pathname.</param>
        /// <param name="location">Initial location.</param>
        /// <param name="profileName">Profile name.</param>
        /// <param name="connectionName">Connection name.</param>
        public ShortcutDialog(Wx3270App app, string path, Point location, string profileName, string connectionName)
        {
            this.InitializeComponent();

            // Handle restrictions.
            if (app?.Restricted(Restrictions.GetHelp) == true)
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1, this.helpContextMenuStrip);

            // Initialize display fields.
            this.profileSpacingLabel.Text = string.Empty;
            this.connectionSpacingLabel.Text = string.Empty;
            this.profileNameLabel.Text = profileName;
            this.connectionNameLabel.Text = connectionName;
            this.pathDisplayLabel.Text = path;
            this.xNumericUpDown.Value = location.X;
            this.yNumericUpDown.Value = location.Y;
        }

        /// <summary>
        /// Gets the path name.
        /// </summary>
        public string PathName => this.pathDisplayLabel.Text;

        /// <summary>
        /// Gets a value indicating whether to start maximized.
        /// </summary>
        public bool Maximized => this.maximizedCheckBox.Checked;

        /// <summary>
        /// Gets a value indicating whether to start in full-screen mode.
        /// </summary>
        public bool FullScreen => this.fullScreenCheckBox.Checked;

        /// <summary>
        /// Gets the start location.
        /// </summary>
        public Point? StartLocation => this.locationCheckBox.Checked ? new Point((int)this.xNumericUpDown.Value, (int)this.yNumericUpDown.Value) : (Point?)null;

        /// <summary>
        /// Gets a value indicating whether to run in read-only mode.
        /// </summary>
        public bool ReadOnly => this.readOnlyCheckBox.Checked;

        /// <summary>
        /// Gets a value indicating whether to run in detached mdoe.
        /// </summary>
        public bool Detached => this.detachedCheckBox.Checked;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global step 1.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), null), "Tour: Create shortcut");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), null),
@"This window allows you to choose options when creating a desktop shortcut.");

            // Browse button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(browseButton)), "Browse button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(browseButton)),
@"Click to select a different name or location for the shortcut.");

            // Maximize check box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(maximizedCheckBox)), "Maximize option");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(maximizedCheckBox)),
@"Click to start wx3270 with the window maximized.");

            // Full-screen check box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(fullScreenCheckBox)), "Full-screen option");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(fullScreenCheckBox)),
@"Click to start wx3270 in full-screen mode.");

            // Location check box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(locationCheckBox)), "Fixed location option");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(locationCheckBox)),
@"Click to start wx3270 with the window in a particular location.

Enter the X and Y coordinates to the right.");

            // Read-only check box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(readOnlyCheckBox)), "Read-only option");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(readOnlyCheckBox)),
@"Click to run in read-only mode, which means that settings changes will not be saved.

Settings changed in a read/write window will be copied to the read-only window, unless 'Detached' is selected below.");

            // Detached check box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(detachedCheckBox)), "Read-only option");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(detachedCheckBox)),
@"Click to run in detached mode.

Detached mode, which only has effect if the window is also in read-only mode, prevents settings changes made in a read/write window from being copied to the read-only window.");

            // Cancel button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(cancelButton)), "Cancel button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(cancelButton)),
@"Click to cancel creating the shortcut.");

            // Save button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(saveButton)), "Save button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(saveButton)),
@"Click to create or update the shortcut.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ShortcutDialog), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ShortcutDialog), nameof(helpPictureBox)),
@"Click to display context-dependent help from the wx3270 Wiki in your browser, or to restart this tour.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Localizes the form.
        /// </summary>
        [I18nFormInit]
        public static void FormLocalize()
        {
            new ShortcutDialog(null, string.Empty, new Point(0, 0), string.Empty, string.Empty).Dispose();
        }

        /// <summary>
        /// Runs the tour.
        /// </summary>
        /// <param name="isExplicit">True if invoked explicitly.</param>
        private void RunTour(bool isExplicit = false)
        {
            var nodes = new[]
            {
                ((Control)this, (int?)null, Orientation.Centered),
                (this.browseButton, null, Orientation.UpperRight),
                (this.maximizedCheckBox, null, Orientation.UpperLeft),
                (this.fullScreenCheckBox, null, Orientation.UpperLeft),
                (this.locationCheckBox, null, Orientation.UpperLeft),
                (this.readOnlyCheckBox, null, Orientation.LowerLeft),
                (this.detachedCheckBox, null, Orientation.LowerLeft),
                (this.cancelButton, null, Orientation.LowerRight),
                (this.saveButton, null, Orientation.LowerRight),
                (this.helpPictureBox, null, Orientation.LowerRight),
            };
            Tour.Navigate(this, nodes, isExplicit: isExplicit);
        }

        /// <summary>
        /// One of the help menu items was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, System.EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "Shortcut Options", () => this.RunTour(isExplicit: true));
        }

        /// <summary>
        /// The help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpButtonClick(object sender, System.EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// The form was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FormActivated(object sender, System.EventArgs e)
        {
            if (!Tour.IsComplete(this))
            {
                this.RunTour();
            }
        }

        /// <summary>
        /// The Cancel button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelClick(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        /// <summary>
        /// The Save button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveClick(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        /// <summary>
        /// One of the check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CheckBoxClick(object sender, System.EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            switch ((string)checkBox.Tag)
            {
                case "Maximized":
                    if (checkBox.Checked)
                    {
                        this.fullScreenCheckBox.Checked = false;
                    }

                    this.fullScreenCheckBox.Enabled = !checkBox.Checked;
                    break;
                case "FullScreen":
                    if (checkBox.Checked)
                    {
                        this.maximizedCheckBox.Checked = false;
                    }

                    this.maximizedCheckBox.Enabled = !checkBox.Checked;
                    break;
            }
        }

        /// <summary>
        /// The browse button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BrowseClick(object sender, System.EventArgs e)
        {
            this.saveFileDialog.InitialDirectory = Path.GetDirectoryName(this.pathDisplayLabel.Text);
            this.saveFileDialog.FileName = Path.GetFileName(this.pathDisplayLabel.Text);
            switch (this.saveFileDialog.ShowDialog(this))
            {
                case DialogResult.OK:
                    this.pathDisplayLabel.Text = this.saveFileDialog.FileName;
                    break;
                default:
                    return;
            }
        }
    }
}
