// <copyright file="MergeDialog.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

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
        /// Initializes a new instance of the <see cref="MergeDialog"/> class.
        /// </summary>
        /// <param name="profileDialog">Parent profile dialog.</param>
        /// <param name="sourceProfileName">Source profile name.</param>
        /// <param name="destinationProfileName">Destination profile name.</param>
        /// <param name="profileType">Profile type.</param>
        public MergeDialog(Form profileDialog, string sourceProfileName, string destinationProfileName, ProfileType profileType)
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

            // Localize.
            I18n.Localize(this);
        }

        /// <summary>
        /// Gets the imports.
        /// </summary>
        public ImportType Imports { get; private set; }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void LocalizeForm()
        {
            I18n.LocalizeGlobal(Label.SourceProfile, "Source profile");
            I18n.LocalizeGlobal(Label.DestinationProfile, "Destination profile");
            new MergeDialog(null, "foo", "bar", ProfileType.Full).Dispose();
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

            this.importButton.Enabled = any;
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
