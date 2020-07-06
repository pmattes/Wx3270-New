// <copyright file="EmulationSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// The emulation items in the options tab in the settings dialog.
    /// </summary>
    public partial class Settings : IOpacity
    {
        /// <summary>
        /// The maximum oversize rows times columns.
        /// </summary>
        public const int OversizeMax = 0x3fff;

        /// <summary>
        /// Name of the unknonw code page error.
        /// </summary>
        private static readonly string UnknownCodePageError = I18n.Combine(nameof(Settings), "error", "unknownCodePage");

        /// <summary>
        /// Models database.
        /// </summary>
        private static readonly ModelsDb ModelsDb = new ModelsDb();

        /// <summary>
        /// True if there is a host connection.
        /// </summary>
        private bool connected = false;

        /// <summary>
        /// The color mode radio buttons and UI state.
        /// </summary>
        private RadioEnum<ColorModeEnum> colorMode;

        /// <summary>
        /// Back-end initialization is complete.
        /// </summary>
        private bool ready;

        /// <summary>
        /// We are in extended mode (shadow for extended mode check box, which shows as indeterminate when override is in effect).
        /// </summary>
        private bool extendedMode;

        /// <summary>
        /// We are pushing a collection of related parameters controlled by the Model() action.
        /// </summary>
        private bool noUiPush;

        /// <summary>
        /// Event called when the opacity changes.
        /// </summary>
        public event Action<int> OpacityEvent = (percent) => { };

        /// <summary>
        /// Event called when the color mode changes.
        /// </summary>
        private event Action<bool> ColorModeChangedEvent = (color) => { };

        /// <summary>
        /// Color mode enumeration.
        /// </summary>
        private enum ColorModeEnum
        {
            /// <summary>
            /// Color (3279) mode.
            /// </summary>
            Color = 3279,

            /// <summary>
            /// Monochrome (3278) mode.
            /// </summary>
            Monochrome = 3278,
        }

        /// <summary>
        /// Model number enumeration.
        /// </summary>
        private enum ModelEnum
        {
            /// <summary>
            /// Model 2.
            /// </summary>
            Model2 = 2,

            /// <summary>
            /// Model 3.
            /// </summary>
            Model3 = 3,

            /// <summary>
            /// Model 4.
            /// </summary>
            Model4 = 4,

            /// <summary>
            /// Model 5.
            /// </summary>
            Model5 = 5,
        }

        /// <summary>
        /// Gets a value indicating whether oversize mode is enabled.
        /// </summary>
        private bool UiIsOversize => OversizeCheck(this.UiModelDimensions, (int)this.RowsUpDown.Value, (int)this.ColumnsUpDown.Value);

        /// <summary>
        /// Gets the model, according to the UI.
        /// </summary>
        private int UiModel => this.UiModelDimensions.Model;

        /// <summary>
        /// Gets the model dimensions, according to the UI.
        /// </summary>
        private ModelDimensions UiModelDimensions => this.modelComboBox.SelectedItem as ModelDimensions;

        /// <summary>
        /// Gets the oversize rows, according to the UI.
        /// </summary>
        private int UiOversizeRows => this.OversizeCheckBox.Checked ? (int)this.RowsUpDown.Value : 0;

        /// <summary>
        /// Gets the oversize columns, according to the UI.
        /// </summary>
        private int UiOversizeColumns => this.OversizeCheckBox.Checked ? (int)this.ColumnsUpDown.Value : 0;

        /// <summary>
        /// Return the default number of rows for a given model.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <returns>Number of rows.</returns>
        public static int DefaultRows(int model)
        {
            return DefaultDimensions(model).Rows;
        }

        /// <summary>
        /// Return the default number of columns for a given model.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <returns>Number of columns.</returns>
        public static int DefaultColumns(int model)
        {
            return DefaultDimensions(model).Columns;
        }

        /// <summary>
        /// Get the default dimensions for a model.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <returns>Default dimensions.</returns>
        private static ModelDimensions DefaultDimensions(int model)
        {
            return ModelsDb.Models[model];
        }

        /// <summary>
        /// Check for oversize.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        /// <returns>True if oversize.</returns>
        private static bool OversizeCheck(ModelDimensions model, int rows, int columns)
        {
            return rows != model.Rows || columns != model.Columns;
        }

        /// <summary>
        /// Construct a value for toggling the model.
        /// </summary>
        /// <param name="colorMode">Color mode.</param>
        /// <param name="model">Model number.</param>
        /// <param name="extendedMode">Extended mode.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        /// <returns>List of arguments.</returns>
        private static IEnumerable<string> ModelOptions(bool colorMode, int model, bool extendedMode, int rows, int columns)
        {
            var startupConfig = new StartupConfig
            {
                Model = model,
                ColorMode = colorMode,
                ExtendedMode = extendedMode,
                OversizeRows = rows,
                OversizeColumns = columns,
            };
            return new[]
            {
                B3270.Setting.Model, startupConfig.ModelParameter,
                B3270.Setting.Oversize, startupConfig.OversizeParameter,
            };
        }

        /// <summary>
        /// Initialize the emulation tab.
        /// </summary>
        private void EmulationTabInit()
        {
            this.BackEnd.Register(ModelsDb);
            ModelsDb.Done += () =>
            {
                this.modelComboBox.Items.Clear();
                this.modelComboBox.Items.AddRange(ModelsDb.Models.Values.ToArray());
            };
            this.app.CodePageDb.Done += () =>
            {
                this.CodePageListBox.Items.Clear();
                this.CodePageListBox.Items.AddRange(this.app.CodePageDb.All.ToArray());
            };

            // Set up the radio button enumerations.
            this.colorMode = new RadioEnum<ColorModeEnum>(this.DisplayGroupBox);
            this.colorMode.Changed += this.MonoButtonCheckedChanged;
            this.modelComboBox.SelectedValueChanged += this.ModelBoxChanged;

            // Subscribe to profile change and merge events.
            this.ProfileManager.ChangeTo += this.ProfileEmulationChanged;
            this.ProfileManager.RegisterMerge(ImportType.OtherSettingsReplace, this.MergeEmulation);

            // Subscribe to connection state events.
            this.mainScreen.ConnectionStateEvent += this.EmulationConnectionChange;

            // Subscribe to screen mode change events.
            this.mainScreen.ScreenModeEvent += this.EmulationScreenModeChange;

            // Subscribe to the ready event.
            this.app.BackEnd.OnReady += () =>
            {
                this.ready = true;
            };

            // Subscribe to terminal name changes.
            this.app.BackEnd.RegisterStart(
                B3270.Indication.TerminalName,
                (name, attrs) => this.BackEndChangedTerminalName(attrs));

            // Subscribe to setting changes.
            this.app.SettingChange.Register(
                (settingName, settingDictionary) => this.Invoke(new MethodInvoker(() => this.SettingChanged(settingName, settingDictionary))),
                new[] { B3270.Setting.CodePage, B3270.Setting.CodePage, B3270.Setting.CursorBlink, B3270.Setting.AltCursor, B3270.Setting.Crosshair, B3270.Setting.MonoCase });

            // Localize an error message.
            I18n.LocalizeGlobal(UnknownCodePageError, "Unknown host code page");
        }

        /// <summary>
        /// Process a change in connection state.
        /// </summary>
        private void EmulationConnectionChange()
        {
            this.connected = this.app.ConnectionState != ConnectionState.NotConnected;
            this.ModelGroupBox.Enabled = !this.connected;
            this.DisplayGroupBox.Enabled = !this.connected;
            this.TerminalNameGroupBox.Enabled = !this.connected;
        }

        /// <summary>
        /// Process an asynchronous change in screen mode (back-end indication).
        /// </summary>
        private void EmulationScreenModeChange()
        {
            // Get the current screen image.
            var image = this.app.ScreenImage;

            // Propagate model number, color mode and oversize.
            this.PropagateScreenModeToUI(image.Model, image.Oversize, image.MaxRows, image.MaxColumns, image.ColorMode, image.Extended);
            this.PushModelToProfile();
        }

        /// <summary>
        /// The profile has changed. Update the emulation defaults.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileEmulationChanged(Profile oldProfile, Profile newProfile)
        {
            // Set up the UI.
            this.EmulationSet(newProfile);

            // Tell the back end about the new screen settings.
            this.ApplyScreenSettingsFromProfile(oldProfile, newProfile);

            // Tell the back end about the code page.
            this.ApplyCodePageFromProfile(oldProfile, newProfile);
        }

        /// <summary>
        /// Apply the selected code page to the back end.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ApplyCodePageFromProfile(Profile oldProfile, Profile newProfile)
        {
            if (oldProfile == null || oldProfile.HostCodePage != newProfile.HostCodePage)
            {
                // Change the UI.
                var index = this.app.CodePageDb.Index(newProfile.HostCodePage);
                if (index < 0)
                {
                    this.CodePageListBox.ClearSelected();
                    return;
                }
                else if (this.CodePageListBox.SelectedIndex != index)
                {
                    this.CodePageListBox.SelectedIndex = index;
                }

                // Send it to the back end.
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, B3270.Setting.CodePage, newProfile.HostCodePage),
                    ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// Apply new screen settings from the profile.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ApplyScreenSettingsFromProfile(Profile oldProfile, Profile newProfile)
        {
            if (!this.connected)
            {
                if (oldProfile == null
                    || oldProfile.Model != newProfile.Model
                    || oldProfile.ColorMode != newProfile.ColorMode
                    || !oldProfile.Oversize.Equals(newProfile.Oversize)
                    || oldProfile.ExtendedMode != newProfile.ExtendedMode)
                {
                    // Send the model change command to the back end.
                    var modelOptions = ModelOptions(
                        newProfile.ColorMode,
                        newProfile.Model,
                        newProfile.ExtendedMode,
                        newProfile.Oversize.Rows,
                        newProfile.Oversize.Columns);
                    this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, modelOptions), ErrorBox.Completion(I18n.Get(Title.Settings)));
                }
            }
        }

        /// <summary>
        /// Paint new screen mode parameters into the UI.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <param name="oversize">Oversize mode.</param>
        /// <param name="oversizeRows">Oversize rows.</param>
        /// <param name="oversizeColumns">Oversize columns.</param>
        /// <param name="color">Color mode.</param>
        /// <param name="extended">Extended mode.</param>
        private void PropagateScreenModeToUI(int model, bool oversize, int oversizeRows, int oversizeColumns, bool color, bool extended)
        {
            // Don't push to the back end because of this.
            this.noUiPush = true;
            try
            {
                // Set the model.
                var modelDimensions = ModelsDb.Models.Values.FirstOrDefault(m => m.Model == model);
                this.modelComboBox.SelectedItem = modelDimensions;

                // Set oversize.
                this.OversizeCheckBox.Checked = oversize;
                this.OversizeCheckBox.Enabled = extended;
                this.RowsUpDown.Enabled = oversize;
                this.RowsUpDown.Minimum = modelDimensions.Rows;
                this.RowsUpDown.Value = oversize ? oversizeRows : modelDimensions.Rows;
                this.ColumnsUpDown.Enabled = oversize;
                this.ColumnsUpDown.Minimum = modelDimensions.Columns;
                this.ColumnsUpDown.Value = oversize ? oversizeColumns : modelDimensions.Columns;

                // Set color mode.
                this.colorMode.Value = color ? ColorModeEnum.Color : ColorModeEnum.Monochrome;

                // Only change the Extended check box if override is in effect.
                this.extendedMode = extended;
                if (!this.OverrideCheckBox.Checked)
                {
                    this.ExtendedCheckBox.Checked = extended;
                }
            }
            finally
            {
                this.noUiPush = false;
            }
        }

        /// <summary>
        /// Set the emulation settings from a given profile.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void EmulationSet(Profile profile)
        {
            // Handle the profile type.
            var isFull = profile.ProfileType == ProfileType.Full;
            this.DisplayGroupBox.Enabled = isFull;
            this.opacityGroupBox.Enabled = isFull;
            this.ModelGroupBox.Enabled = isFull;
            this.TerminalNameGroupBox.Enabled = isFull;
            this.MiscGroupBox.Enabled = isFull;
            this.CodePageGroupBox.Enabled = isFull;

            // Set the code page.
            var index = this.app.CodePageDb.Index(profile.HostCodePage);
            if (index < 0)
            {
                this.ProfileManager.ProfileError(I18n.Get(UnknownCodePageError) + $" '{profile.HostCodePage}'");
                index = this.app.CodePageDb.Index(Profile.DefaultProfile.HostCodePage);
            }

            if (this.CodePageListBox.SelectedIndex != index)
            {
                this.CodePageListBox.SelectedIndex = index;
            }

            // Set the opacity.
            this.opacityTrackBar.Value = profile.OpacityPercent;
            this.SetOpacity(profile.OpacityPercent);

            if (!this.connected)
            {
                // Set model, oversize, color mode.
                this.PropagateScreenModeToUI(
                    profile.Model,
                    profile.Oversize.HasValue(),
                    profile.Oversize.Rows,
                    profile.Oversize.Columns,
                    profile.ColorMode,
                    profile.ExtendedMode);
                this.PushModelToBackEnd();

                // Set terminal override.
                this.OverrideCheckBox.Checked = !string.IsNullOrEmpty(profile.TerminalNameOverride);
                this.OverrideTextBox.Enabled = this.OverrideCheckBox.Checked;
                this.ExtendedCheckBox.Enabled = !this.OverrideCheckBox.Checked;
                if (this.OverrideCheckBox.Checked)
                {
                    if (!this.OverrideTextBox.Text.Equals(profile.TerminalNameOverride))
                    {
                        this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.TermName, profile.TerminalNameOverride), ErrorBox.Completion(I18n.Get(Title.Settings)));
                    }

                    this.OverrideTextBox.Text = profile.TerminalNameOverride;
                }
                else
                {
                    this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.TermName, string.Empty), ErrorBox.Completion(I18n.Get(Title.Settings)));
                }
            }
        }

        /// <summary>
        /// The contents of the terminal override text box have been validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverrideTextBoxValidated(object sender, EventArgs e)
        {
            if (this.ProfileManager.PushAndSave((current) => current.TerminalNameOverride = this.OverrideTextBox.Text, this.ChangeName(B3270.Setting.TermName)))
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.TermName, this.OverrideTextBox.Text), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// Describes what changed in the model.
        /// </summary>
        /// <param name="current">Current profile.</param>
        /// <param name="model">Model number.</param>
        /// <param name="isOversize">Oversize mode.</param>
        /// <param name="rows">Oversize rows.</param>
        /// <param name="columns">Oversize columns.</param>
        /// <param name="colorMode">Color mode.</param>
        /// <param name="extendedMode">Extended mode.</param>
        /// <returns>List of items that changed.</returns>
        private string WhatChanged(Profile current, int model, bool isOversize, int rows, int columns, bool colorMode, bool extendedMode)
        {
            var whatChanged = new List<string>();
            if (current.Model != model)
            {
                whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.Model)));
            }

            var oversizeChanged = current.Oversize.HasValue() != isOversize;
            if (oversizeChanged)
            {
                whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.OversizeMode)));
            }

            if (oversizeChanged && isOversize)
            {
                // Oversize turned on. Non-default values count.
                if (rows != DefaultDimensions(model).Rows)
                {
                    whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.OversizeRows)));
                }

                if (columns != DefaultDimensions(model).Columns)
                {
                    whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.OversizeColumns)));
                }
            }
            else if (isOversize)
            {
                // Oversize stayed on. Compare to current.
                if (rows != current.Oversize.Rows)
                {
                    whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.OversizeRows)));
                }

                if (columns != current.Oversize.Columns)
                {
                    whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.OversizeColumns)));
                }
            }

            if (current.ColorMode != colorMode)
            {
                whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.ColorMode)));
            }

            if (current.ExtendedMode != extendedMode)
            {
                whatChanged.Add(I18n.Get(SettingPath(ChangeKeyword.ExtendedMode)));
            }

            return string.Join(", ", whatChanged);
        }

        /// <summary>
        /// Push a new model (model number, oversize, color mode) to the profile, and if the profile
        /// changed, to the back end.
        /// </summary>
        private void PushModelToProfile()
        {
            if (!this.ready)
            {
                // This is just initial back end state.
                return;
            }

            var whatChanged = this.WhatChanged(
                this.ProfileManager.Current,
                this.UiModel,
                this.OversizeCheckBox.Checked,
                this.UiOversizeRows,
                this.UiOversizeColumns,
                this.ColorMode,
                this.extendedMode);
            if (string.IsNullOrEmpty(whatChanged))
            {
                // Perhaps nothing has changed.
                return;
            }

            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    // Apply the model.
                    current.Model = this.UiModel;

                    // Apply oversize.
                    current.Oversize.Rows = this.UiOversizeRows;
                    current.Oversize.Columns = this.UiOversizeColumns;

                    // Apply monochrome/color mode.
                    current.ColorMode = this.ColorMode;

                    // Apply extended mode.
                    current.ExtendedMode = this.extendedMode;

                    // Set the size.
                    current.Size = this.mainScreen.Size;
                },
                this.ProfileManager.ChangeName(whatChanged));
        }

        /// <summary>
        /// Push model-related information to the back end.
        /// </summary>
        private void PushModelToBackEnd()
        {
            if (this.ready && !this.noUiPush)
            {
                var modelOptions = ModelOptions(
                        this.colorMode.Value == ColorModeEnum.Color,
                        this.UiModel,
                        this.extendedMode,
                        this.UiOversizeRows,
                        this.UiOversizeColumns);
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, modelOptions), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// Change handler for the Model combo box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ModelBoxChanged(object sender, EventArgs e)
        {
            // Turn off oversize first. This will keep oversize rows/columns changes from being propagated.
            this.OversizeCheckBox.Checked = false;

            var modelDimensions = this.modelComboBox.SelectedItem as ModelDimensions;
            this.RowsUpDown.Minimum = modelDimensions.Rows;
            this.RowsUpDown.Value = this.RowsUpDown.Minimum;
            this.ColumnsUpDown.Minimum = modelDimensions.Columns;
            this.ColumnsUpDown.Value = this.ColumnsUpDown.Minimum;

            this.PushModelToBackEnd();
        }

        /// <summary>
        /// The value of the oversize rows changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RowsUpDownValueChanged(object sender, EventArgs e)
        {
            this.ColumnsUpDown.Maximum = OversizeMax / this.RowsUpDown.Value;

            if (((NumericUpDown)sender).Enabled)
            {
                this.PushModelToBackEnd();
            }
        }

        /// <summary>
        /// The value of the oversize columns changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColumnsUpDownValueChanged(object sender, EventArgs e)
        {
            this.RowsUpDown.Maximum = OversizeMax / this.ColumnsUpDown.Value;

            if (((NumericUpDown)sender).Enabled)
            {
                this.PushModelToBackEnd();
            }
        }

        /// <summary>
        /// The Oversize check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OversizeCheckBoxClick(object sender, EventArgs e)
        {
            this.RowsUpDown.Enabled = this.OversizeCheckBox.Checked;
            this.ColumnsUpDown.Enabled = this.OversizeCheckBox.Checked;
            if (!this.OversizeCheckBox.Checked)
            {
                this.RowsUpDown.Minimum = this.UiModelDimensions.Rows;
                this.ColumnsUpDown.Minimum = this.UiModelDimensions.Columns;
                if (this.UiIsOversize)
                {
                    this.RowsUpDown.Value = this.UiModelDimensions.Rows;
                    this.ColumnsUpDown.Value = this.UiModelDimensions.Columns;
                }

                this.PushModelToBackEnd();
            }
        }

        /// <summary>
        /// The terminal name override check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverrideCheckBoxClick(object sender, EventArgs e)
        {
            if (this.OverrideCheckBox.Checked)
            {
                // Override on.
                this.OverrideTextBox.Enabled = true;
                this.ExtendedCheckBox.Enabled = false;
            }
            else
            {
                // Override off.
                this.OverrideTextBox.Enabled = false;
                this.ExtendedCheckBox.Enabled = true;
                this.ExtendedCheckBox.Checked = this.extendedMode;

                if (this.ProfileManager.PushAndSave((current) => current.TerminalNameOverride = string.Empty, this.ChangeName(B3270.Setting.TermName)))
                {
                    this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.TermName, string.Empty), ErrorBox.Completion(I18n.Get(Title.Settings)));
                }
            }
        }

        /// <summary>
        /// The Extended check box changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ExtendedCheckBoxClick(object sender, EventArgs e)
        {
            this.extendedMode = this.ExtendedCheckBox.Checked;
            this.PushModelToBackEnd();
        }

        /// <summary>
        /// The back end indicated a terminal name change.
        /// </summary>
        /// <param name="attrs">Dictionary of attributes.</param>
        private void BackEndChangedTerminalName(Dictionary<string, string> attrs)
        {
            var text = attrs[B3270.Attribute.Text];
            this.OverrideTextBox.Text = text;
            if (attrs[B3270.Attribute.Override].Equals(B3270.Value.True))
            {
                this.OverrideCheckBox.Checked = true;
                this.ProfileManager.PushAndSave((current) => current.TerminalNameOverride = text, this.ChangeName(B3270.Setting.TermName));
            }
            else
            {
                this.OverrideCheckBox.Checked = false;
                this.ProfileManager.PushAndSave((current) => current.TerminalNameOverride = string.Empty, this.ChangeName(B3270.Setting.TermName));
            }
        }

        /// <summary>
        /// The monochrome checkbox changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoButtonCheckedChanged(object sender, EventArgs e)
        {
            this.ColorModeChangedEvent(this.ColorMode);

            // Repaint the screen.
            this.mainScreen.Recolor(this.editedColors, this.ColorMode);

            // Update the profile and the host.
            this.PushModelToBackEnd();
        }

        /// <summary>
        /// The code page index (UI control) changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CodePageChanged(object sender, EventArgs e)
        {
            if (!(this.CodePageListBox.SelectedItem is string codePage))
            {
                return;
            }

            var canonicalName = this.app.CodePageDb.CanonicalName(codePage);
            if (canonicalName != codePage)
            {
                this.CodePageListBox.SelectedIndex = this.app.CodePageDb.Index(canonicalName);
            }

            if (this.ProfileManager.PushAndSave((current) => current.HostCodePage = canonicalName, this.ChangeName(B3270.Setting.CodePage)))
            {
                this.BackEnd.RunAction(
                        new BackEndAction(B3270.Action.Set, B3270.Setting.CodePage, codePage),
                        ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// A back-end setting changed.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="settingDictionary">Setting dictionary (values).</param>
        private void SettingChanged(string name, SettingsDictionary settingDictionary)
        {
            switch (name)
            {
                case B3270.Setting.CodePage:
                    settingDictionary.TryGetValue(B3270.Setting.CodePage, out string codePage);
                    var index = this.app.CodePageDb.Index(codePage);
                    if (index >= 0)
                    {
                        if (this.CodePageListBox.SelectedIndex != index)
                        {
                            this.CodePageListBox.SelectedIndex = index;
                        }

                        this.ProfileManager.PushAndSave((current) => current.HostCodePage = codePage, this.ChangeName(B3270.Setting.CodePage));
                    }

                    break;
                case B3270.Setting.CursorBlink:
                    this.cursorBlinkCheckBox.Checked = settingDictionary.TryGetValue(B3270.Setting.CursorBlink, out bool blink) && blink;
                    break;
                case B3270.Setting.AltCursor:
                    this.cursorType.Value = (settingDictionary.TryGetValue(B3270.Setting.AltCursor, out bool altCursor) && altCursor) ? CursorType.Underscore : CursorType.Block;
                    break;
                case B3270.Setting.Crosshair:
                    this.crosshairCursorCheckBox.Checked = settingDictionary.TryGetValue(B3270.Setting.Crosshair, out bool crosshair) && crosshair;
                    break;
                case B3270.Setting.MonoCase:
                    this.MonoCaseCheckBox.Checked = settingDictionary.TryGetValue(B3270.Setting.MonoCase, out bool monoCase) && monoCase;
                    break;
            }
        }

        /// <summary>
        /// Merge the miscellaneous settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        private bool MergeEmulation(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.Oversize.Equals(fromProfile.Oversize) &&
                toProfile.Model == fromProfile.Model &&
                toProfile.ColorMode == fromProfile.ColorMode &&
                toProfile.TerminalNameOverride == fromProfile.TerminalNameOverride &&
                toProfile.HostCodePage == fromProfile.HostCodePage &&
                toProfile.CursorBlink == fromProfile.CursorBlink &&
                toProfile.CursorType == fromProfile.CursorType &&
                toProfile.CrosshairCursor == fromProfile.CrosshairCursor &&
                toProfile.Monocase == fromProfile.Monocase &&
                toProfile.OpacityPercent == fromProfile.OpacityPercent)
            {
                return false;
            }

            toProfile.Oversize = fromProfile.Oversize.Clone();
            toProfile.Model = fromProfile.Model;
            toProfile.ColorMode = fromProfile.ColorMode;
            toProfile.TerminalNameOverride = fromProfile.TerminalNameOverride;
            toProfile.HostCodePage = fromProfile.HostCodePage;
            toProfile.CursorBlink = fromProfile.CursorBlink;
            toProfile.CursorType = fromProfile.CursorType;
            toProfile.CrosshairCursor = fromProfile.CrosshairCursor;
            toProfile.Monocase = fromProfile.Monocase;
            toProfile.OpacityPercent = fromProfile.OpacityPercent;
            return true;
        }

        /// <summary>
        /// Set the window opacity.
        /// </summary>
        /// <param name="percent">Opacity percent.</param>
        private void SetOpacity(int percent)
        {
            this.opacityLabel.Text = $"{percent}";
            this.mainScreen.Opacity = percent / 100.0;
            this.OpacityEvent(percent);
        }

        /// <summary>
        /// The opacity scroll changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OpacityScroll(object sender, EventArgs e)
        {
            var trackBar = sender as TrackBar;
            this.SetOpacity(trackBar.Value);

            // Set the opacity timer to go off 2 seconds after the last scroll event.
            if (this.opacityTimer.Enabled)
            {
                this.opacityTimer.Stop();
            }

            this.opacityTimer.Start();
        }

        /// <summary>
        /// The opacity timer ticked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OpacityTimerTick(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.OpacityPercent = this.opacityTrackBar.Value, this.ProfileManager.ChangeName(I18n.Get(I18n.Combine("settings", "opacity"))));
        }

        /// <summary>
        /// Handler for model indications from the back end.
        /// </summary>
        private class Model : BackEndEvent
        {
            /// <summary>
            /// The dictionary of models.
            /// </summary>
            private readonly Dictionary<int, ModelDimensions> models = new Dictionary<int, ModelDimensions>();

            /// <summary>
            /// Initializes a new instance of the <see cref="Model"/> class.
            /// </summary>
            public Model()
            {
                this.Def = new[] { new BackEndEventDef(B3270.Indication.Model, this.StartModel) };
            }

            /// <summary>
            /// Gets the dictionary of models.
            /// </summary>
            public IReadOnlyDictionary<int, ModelDimensions> Models => this.models;

            /// <summary>
            /// Processes a model indication.
            /// </summary>
            /// <param name="name">Indication name.</param>
            /// <param name="attributes">Indication attributes.</param>
            private void StartModel(string name, AttributeDict attributes)
            {
                var model = int.Parse(attributes[B3270.Attribute.Model]);
                this.models[model] = new ModelDimensions(
                    model,
                    int.Parse(attributes[B3270.Attribute.Rows]),
                    int.Parse(attributes[B3270.Attribute.Columns]));
            }
        }
    }
}