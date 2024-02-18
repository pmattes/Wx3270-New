// <copyright file="MergeDialog.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using I18nBase;

    /// <summary>
    /// What kind of configuration information to merge.
    /// </summary>
    [Flags]
    public enum ImportType
    {
        /// <summary>
        /// No import.
        /// </summary>
        None = 0,

        /// <summary>
        /// Keyboard mapping (merge).
        /// </summary>
        KeyboardMerge = 0x1,

        /// <summary>
        /// Keyboard mapping (replace).
        /// </summary>
        KeyboardReplace = 0x2,

        /// <summary>
        /// Keypad mapping (merge).
        /// </summary>
        KeypadMerge = 0x4,

        /// <summary>
        /// Keypad mapping (replace).
        /// </summary>
        KeypadReplace = 0x8,

        /// <summary>
        /// Host list (merge).
        /// </summary>
        HostsMerge = 0x10,

        /// <summary>
        /// Host list (replace).
        /// </summary>
        HostsReplace = 0x20,

        /// <summary>
        /// Macros list (merge).
        /// </summary>
        MacrosMerge = 0x40,

        /// <summary>
        /// Macros list (replace).
        /// </summary>
        MacrosReplace = 0x80,

        /// <summary>
        /// 3278 and 3279 colors.
        /// </summary>
        ColorsReplace = 0x100,

        /// <summary>
        /// Font configuration.
        /// </summary>
        FontReplace = 0x200,

        /// <summary>
        /// Other settings.
        /// </summary>
        OtherSettingsReplace = 0x400,
    }

    /// <summary>
    /// Profile merge dialog.
    /// </summary>
    public partial class MergeDialog : Form
    {
        /// <summary>
        /// Label name for localization.
        /// </summary>
        private static readonly string LabelName = I18n.StringName(nameof(MergeDialog));

        /// <summary>
        /// The profile dialog.
        /// </summary>
        private readonly Form profileDialog;

        /// <summary>
        /// True if the form has been activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeDialog"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="profileDialog">Parent profile dialog.</param>
        /// <param name="sourceProfileName">Source profile name.</param>
        /// <param name="destinationProfileName">Destination profile name.</param>
        /// <param name="profileType">Profile type.</param>
        public MergeDialog(Wx3270App app, Form profileDialog, string sourceProfileName, string destinationProfileName, ProfileType profileType)
        {
            this.InitializeComponent();
            this.profileDialog = profileDialog;
            this.sourceProfileLabel.Text = I18n.Get(Label.SourceProfile) + ": " + sourceProfileName
                + Environment.NewLine + Environment.NewLine
                + I18n.Get(Label.DestinationProfile) + ": " + destinationProfileName;

            switch (profileType)
            {
                case ProfileType.Full:
                default:
                    break;
                case ProfileType.KeyboardMapTemplate:
                    this.ToggleMerge(this.keyboardMapPanel);
                    break;
                case ProfileType.KeypadMapTemplate:
                    this.ToggleMerge(this.keypadMapPanel);
                    break;
            }

            if (app != null && sourceProfileName == ProfileManager.DefaultValuesName)
            {
                this.hostsCheckBox.Enabled = false;
                this.macrosCheckBox.Enabled = false;
            }

            // Handle restrictions.
            if (app != null && app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);
        }

        /// <summary>
        /// Gets the imports.
        /// </summary>
        public ImportType Imports { get; private set; }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Start.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MergeDialog)), "Tour: Merge Window");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MergeDialog)),
@"This window provides options for merging the contents of one profile into another.");

            // Profile names.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MergeDialog), nameof(sourceProfileLabel)), "Source and destination profiles");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MergeDialog), nameof(sourceProfileLabel)),
@"The source profile is the profile that settings will be read from.

The destination profile is the profile that will be modified.");

            // Category checkboxes.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MergeDialog), nameof(keyboardCheckBox)), "Category check boxes");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MergeDialog), nameof(keyboardCheckBox)),
@"Use these check boxes to select the categories of settings to copy over.");

            // Replace button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MergeDialog), nameof(keyboardReplaceRadioButton)), "Replace/Merge option");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MergeDialog), nameof(keyboardReplaceRadioButton)),
@"For each selected category, select whether to replace the settings or (for most categories) merge them.

The 'Replace' option means that all of the settings in the destination profile will be replaced by the settings in the source profile.

The 'Merge' option means that settings which overlap existing settings will be overwritten, and settings that are not yet in the destination profile will be added, but other existing settings will be kept.");

            // Merge button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MergeDialog), nameof(mergeButton)), "Merge button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MergeDialog), nameof(mergeButton)),
@"Click to perform the merge operation.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MergeDialog), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MergeDialog), nameof(helpPictureBox)),
@"Click to display context-dependent help from the wx3270 Wiki in your browser, or to restart this tour.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void LocalizeForm()
        {
            I18n.LocalizeGlobal(Label.SourceProfile, "Source profile");
            I18n.LocalizeGlobal(Label.DestinationProfile, "Destination profile");
            new MergeDialog(null, null, "foo", "bar", ProfileType.Full).Dispose();
        }

        /// <summary>
        /// Toggles the merge options so that only one panel is available, and is selected.
        /// </summary>
        /// <param name="chosenPanel">Chosen panel.</param>
        private void ToggleMerge(Panel chosenPanel)
        {
            // Disable the other panels.
            foreach (var panel in this.categoriesTableLayoutPanel.Controls.OfType<Panel>())
            {
                if (panel != chosenPanel)
                {
                    panel.Enabled = false;
                }
            }

            // Set the checkbox in the chosen panel.
            foreach (var checkBox in chosenPanel.Controls.OfType<CheckBox>())
            {
                checkBox.Checked = true;
            }

            // Switch the default from Replace to Merge.
            foreach (var radioButton in chosenPanel.Controls.OfType<RadioButton>())
            {
                if (radioButton.Tag is string tag)
                {
                    if (tag.EndsWith("Replace"))
                    {
                        radioButton.Checked = false;
                        radioButton.Enabled = false;
                    }
                    else if (tag.EndsWith("Merge"))
                    {
                        radioButton.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Extract the types of imports requested.
        /// </summary>
        /// <returns>Set of imports.</returns>
        private ImportType SelectedImports()
        {
            // Extract the desired imports from the control states.
            var ret = ImportType.None;
            foreach (var panel in this.categoriesTableLayoutPanel.Controls.OfType<Panel>())
            {
                var panelImports = ImportType.None;
                var panelChecked = false;
                foreach (var child in panel.Controls)
                {
                    if (child is CheckBox checkBox)
                    {
                        if (!checkBox.Checked)
                        {
                            // Panel not enabled.
                            break;
                        }

                        panelChecked = true;
                        if (checkBox.Tag != null && !string.IsNullOrEmpty(checkBox.Tag as string))
                        {
                            // Panel without radio buttons.
                            panelImports = (ImportType)Enum.Parse(typeof(ImportType), (string)checkBox.Tag);
                            break;
                        }
                    }

                    if (child is RadioButton radioButton && radioButton.Checked)
                    {
                        panelImports = (ImportType)Enum.Parse(typeof(ImportType), (string)radioButton.Tag);
                    }
                }

                if (panelChecked)
                {
                    ret |= panelImports;
                }
            }

            return ret;
        }

        /// <summary>
        /// One of the import check boxes changed state.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MergeCheckBox_CheckedChanged(object sender, EventArgs e)
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

            this.mergeButton.Enabled = any;
        }

        /// <summary>
        /// The Merge button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            // Extract the desired imports from the control states.
            var imports = this.SelectedImports();
            if (imports == ImportType.None)
            {
                return;
            }

            this.Imports = imports;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// The merge dialog was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MergeDialog_Activated(object sender, EventArgs e)
        {
            this.Location = MainScreen.CenteredOn(this.profileDialog, this);

            if (!this.everActivated)
            {
                this.everActivated = true;
                if (!Tour.IsComplete(this))
                {
                    this.RunTour();
                }
            }
        }

        /// <summary>
        /// One of the help menu items was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "Merge", this.RunTour);
        }

        /// <summary>
        /// The help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// Run the tour.
        /// </summary>
        private void RunTour()
        {
            var nodes = new[]
            {
                ((Control)this, (int?)null, Orientation.Centered),
                (this.sourceProfileLabel, null, Orientation.UpperLeft),
                (this.keyboardCheckBox, null, Orientation.UpperLeft),
                (this.keyboardReplaceRadioButton, null, Orientation.UpperLeft),
                (this.mergeButton, null, Orientation.LowerLeft),
                (this.helpPictureBox, null, Orientation.LowerRight),
            };
            Tour.Navigate(this, nodes);
        }

        /// <summary>
        /// Tags for localized labels.
        /// </summary>
        private class Label
        {
            /// <summary>
            /// Source profile.
            /// </summary>
            public static readonly string SourceProfile = I18n.Combine(LabelName, "sourceProfile");

            /// <summary>
            /// Destination profile.
            /// </summary>
            public static readonly string DestinationProfile = I18n.Combine(LabelName, "destinationProfile");
        }
    }
}
