// <copyright file="OptionsSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Printing;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// The Options tab in the settings dialog.
    /// </summary>
    public partial class Settings
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
        /// The sample image.
        /// </summary>
        private ScreenSample optionsScreenSample;

        /// <summary>
        /// The cursor type radio buttons.
        /// </summary>
        private RadioEnum<CursorType> cursorType;

        /// <summary>
        /// The printer type radio buttons.
        /// </summary>
        private RadioEnum<PrinterType> printerType;

        /// <summary>
        /// True if there is a host connection.
        /// </summary>
        private bool connected = false;

        /// <summary>
        /// True if this is a full profile.
        /// </summary>
        private bool isFullProfile = true;

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
        private bool UiIsOversize => OversizeCheck(this.UiModelDimensions, (int)this.rowsUpDown.Value, (int)this.columnsUpDown.Value);

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
        private int UiOversizeRows => this.oversizeCheckBox.Checked ? (int)this.rowsUpDown.Value : 0;

        /// <summary>
        /// Gets the oversize columns, according to the UI.
        /// </summary>
        private int UiOversizeColumns => this.oversizeCheckBox.Checked ? (int)this.columnsUpDown.Value : 0;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeOptionSettings()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(optionsTab)), "Tour: Option settings");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(optionsTab)),
@"Use this tab to change terminal emulation and display settings.");

            // Preview box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(optionsSampleGroupBox)), "Preview display");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(optionsSampleGroupBox)),
@"This miniature emulator widow displays the effect of the current set of options.");

            // Display box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(displayGroupBox)), "Color emulation");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(displayGroupBox)),
@"Choose between a 3278 (monochrome) and 3279 (color) display here.

This option cannot be changed while connected.");

            // Opacity box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(opacityGroupBox)), "Opacity");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(opacityGroupBox)),
@"Use the slider to change the opacity of the wx3270 screen and keypad windows.");

            // Display box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(modelGroupBox)), "Screen size");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(modelGroupBox)),
@"Choose the 3270 model here, which defines the basic screen dimensions (number of rows and columns).

Some hosts also allow an 'oversize' option, with a larger number of rows and columns than the basic size.

This option cannot be changed while connected.");

            // Terminal name box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(terminalNameGroupBox)), "Terminal name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(terminalNameGroupBox)),
@"As part of negotiation between the host and the 3270 emulator, the emulator tells the host what kind of terminal it is.

Normally this is constructed from the 3270 model, color option and oversize settings, but if necessary, you can override it here.

This option cannot be changed while connected.");

            // Miscellaneous settings box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(miscGroupBox)), "Miscellaneous options");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(miscGroupBox)),
@"This is an grab-bag of miscellaneous options. The x3270 Wiki is the best place to find the specific definitions of each.");

            // Cursor box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(cursorGroupBox)), "Cursor settings");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(cursorGroupBox)),
@"Change cursor settings here: Block versus underscore cursor, blinking cursor, and crosshair cursor.

The preview display above will show the effect of your choices.");

            // Code page box.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(codePageGroupBox)), "Host code page");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(codePageGroupBox)),
@"Define the EBCDIC code page used by the host here.

The code page defines the mappings between EBCDIC code points and displayed glyphs. The host does not tell the emulator which code page it is using, so it needs to be defined here.

The default of 'bracket' is a minor variation of the U.S. code page (cp037).");

            // Printer options.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(printerSessionGroupBox)), "Attached printer");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(printerSessionGroupBox)),
@"wx3270 can also emulate an IBM 3287 printer attached to the 3270 terminal, using the included pr3287 utility. Set up the general options here.

pr3287 can send output to a real printer connected to your workstation, or it can save the printer output as text files in a directory you specify. The latter is helpful because pr3287 generally doesn't support PDF printers, such as Microsoft's Print to PDF.

Also note that these are general pr3287 settings. Details about how pr3287 connects to the host are defined per connection, in the Connection Editor.");

            // Printer options.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(descriptionGroupBox)), "Description and window title");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(descriptionGroupBox)),
@"Set up a profile description here, which is the text that is displayed when you hover the mouse over this profile in the Profiles and Connections window.

You can also define the window title here. This is a per-profile setting; you can also define the window title for each connection in the Connection Editor.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
        /// Initialize the Options tab.
        /// </summary>
        private void OptionsTabInit()
        {
            // Set up the databases that are populated from the back-end's init indication.
            this.app.ModelsDb.AddDone(() =>
            {
                this.modelComboBox.Items.Clear();
                this.modelComboBox.Items.AddRange(this.app.ModelsDb.Models.Values.ToArray());
            });

            this.app.CodePageDb.AddDone(() =>
            {
                this.SafeControlModify(this.codePageListBox, () =>
                {
                    this.codePageListBox.Items.Clear();
                    this.codePageListBox.Items.AddRange(this.app.CodePageDb.All.ToArray());
                });
            });

            // Set up the radio button enumerations.
            this.cursorType = new RadioEnum<CursorType>(this.cursorGroupBox);
            this.cursorType.Changed += this.CursorCheckedChanged;
            this.colorMode = new RadioEnum<ColorModeEnum>(this.displayGroupBox);
            this.colorMode.Changed += this.ColorModeChanged;
            this.printerType = new RadioEnum<PrinterType>(this.printerSessionGroupBox);
            this.printerType.Changed += this.PrinterTypeChanged;

            this.modelComboBox.SelectedValueChanged += this.ModelBoxChanged;

            // Set up the screen sample.
            this.optionsScreenSample = new ScreenSample(
                this,
                this.optionsPreviewScreenPictureBox,
                this.optionsPreviewLayoutPanel,
                this.optionsPreviewStatusLineLabel,
                this.optionsPreviewSeparatorPictureBox,
                this.ColorMode);

            // Set up the printer list.
            foreach (var printer in PrinterSettings.InstalledPrinters)
            {
                this.printerComboBox.Items.Add(printer);
            }

            // Set up the scroll bar and menu bar checkboxes.
            this.scrollbarCheckBox.Enabled = !this.app.NoScrollBar;
            this.menuBarCheckBox.Enabled = !this.app.NoButtons;

            // Subscribe to profile change events.
            this.ProfileManager.AddChangeTo(this.ProfileOptionsChanged);

            // Subscribe to color mode change events.
            this.ColorModeChangedEvent += (color) => this.optionsScreenSample.Invalidate();

            // Subscribe to connection state events.
            this.OptionsConnectionChange();
            this.mainScreen.ConnectionStateEvent += this.OptionsConnectionChange;

            // Subscribe to the ready event.
            if (!(this.ready = this.app.BackEnd.Ready))
            {
                this.app.BackEnd.OnReady += () => { this.ready = true; };
            }

            // Subscribe to terminal name changes.
            this.app.TerminalName.Register(this.BackEndChangedTerminalName);

            // Localize an error message.
            I18n.LocalizeGlobal(UnknownCodePageError, "Unknown host code page");

            // Register our tour.
            var nodes = new[]
            {
                ((Control)this.optionsTab, (int?)null, Orientation.Centered),
                (this.optionsSampleGroupBox, null, Orientation.UpperRight),
                (this.displayGroupBox, null, Orientation.UpperLeft),
                (this.opacityGroupBox, null, Orientation.UpperLeft),
                (this.modelGroupBox, null, Orientation.UpperLeft),
                (this.terminalNameGroupBox, null, Orientation.LowerLeft),
                (this.miscGroupBox, null, Orientation.LowerLeft),
                (this.cursorGroupBox, null, Orientation.UpperRight),
                (this.codePageGroupBox, null, Orientation.UpperRight),
                (this.printerSessionGroupBox, null, Orientation.LowerRight),
                (this.descriptionGroupBox, null, Orientation.LowerRight),
            };
            this.RegisterTour(this.optionsTab, nodes);
        }

        /// <summary>
        /// Process a change in connection state.
        /// </summary>
        private void OptionsConnectionChange()
        {
            this.connected = this.app.ConnectionState != ConnectionState.NotConnected;
            this.modelGroupBox.Enabled = this.isFullProfile && !this.connected;
            this.displayGroupBox.Enabled = this.isFullProfile && !this.connected;
            this.terminalNameGroupBox.Enabled = this.isFullProfile && !this.connected;

            string GroupBoxText(string groupBoxName) => I18n.Get(I18n.Combine(nameof(Settings), I18n.FormName, groupBoxName));
            string DynamicText(string groupBoxText) => this.connected ? groupBoxText + " - " + I18n.Get(Message.LockedWhileConnected) : groupBoxText;

            this.modelGroupBox.Text = DynamicText(GroupBoxText(nameof(this.modelGroupBox)));
            this.displayGroupBox.Text = DynamicText(GroupBoxText(nameof(this.displayGroupBox)));
            this.terminalNameGroupBox.Text = DynamicText(GroupBoxText(nameof(this.terminalNameGroupBox)));
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
            // Set the model.
            var modelDimensions = this.app.ModelsDb.Models.Values.FirstOrDefault(m => m.Model == model);
            if (modelDimensions == null)
            {
                modelDimensions = new ModelDimensions(2, 24, 80);
            }

            this.SafeControlModify(this.modelComboBox, () => this.modelComboBox.SelectedItem = modelDimensions);

            // Set oversize.
            this.oversizeCheckBox.Checked = oversize;
            this.oversizeCheckBox.Enabled = !this.connected && extended;
            this.rowsUpDown.Enabled = this.oversizeCheckBox.Enabled && this.oversizeCheckBox.Checked;
            this.rowsUpDown.Minimum = modelDimensions.Rows;
            this.SafeControlModify(this.rowsUpDown, () => this.rowsUpDown.Value = oversize ? oversizeRows : modelDimensions.Rows);
            this.columnsUpDown.Enabled = this.oversizeCheckBox.Enabled && this.oversizeCheckBox.Checked;
            this.columnsUpDown.Minimum = modelDimensions.Columns;
            this.SafeControlModify(this.columnsUpDown, () => this.columnsUpDown.Value = oversize ? oversizeColumns : modelDimensions.Columns);

            // Set color mode.
            this.colorMode.Value = color ? ColorModeEnum.Color : ColorModeEnum.Monochrome;

            // Only change the Extended check box if override is in effect.
            this.extendedMode = extended;
            if (!this.overrideCheckBox.Checked)
            {
                this.extendedCheckBox.Checked = extended;
            }
        }

        /// <summary>
        /// Set the emulation settings from a given profile.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void EmulationSet(Profile profile)
        {
            // Handle the profile type.
            this.isFullProfile = profile.ProfileType == ProfileType.Full;
            this.displayGroupBox.Enabled = this.isFullProfile && !this.connected;
            this.opacityGroupBox.Enabled = this.isFullProfile;
            this.modelGroupBox.Enabled = this.isFullProfile && !this.connected;
            this.terminalNameGroupBox.Enabled = this.isFullProfile && !this.connected;
            this.miscGroupBox.Enabled = this.isFullProfile;
            this.codePageGroupBox.Enabled = this.isFullProfile;

            // Set the code page.
            var index = this.app.CodePageDb.Index(profile.HostCodePage);
            if (index < 0)
            {
                this.ProfileManager.ProfileError(I18n.Get(UnknownCodePageError) + $" '{profile.HostCodePage}'");
                index = this.app.CodePageDb.Index(Profile.DefaultProfile.HostCodePage);
            }

            if (this.codePageListBox.SelectedIndex != index)
            {
                this.SafeControlModify(this.codePageListBox, () =>
                {
                    this.codePageListBox.SelectedIndex = index;
                });
            }

            // Set the opacity.
            this.opacityTrackBar.Value = profile.OpacityPercent;
            this.SetOpacity(profile.OpacityPercent);

            // Set model, oversize, color mode.
            this.PropagateScreenModeToUI(
                profile.Model,
                profile.Oversize.HasValue(),
                profile.Oversize.Rows,
                profile.Oversize.Columns,
                profile.ColorMode,
                profile.ExtendedMode);

            // Set terminal override.
            this.overrideCheckBox.Checked = !string.IsNullOrEmpty(profile.TerminalNameOverride);
            this.overrideTextBox.Enabled = !this.connected && this.overrideCheckBox.Checked;
            this.extendedCheckBox.Enabled = !this.connected && !this.overrideCheckBox.Checked;
            if (this.overrideCheckBox.Checked)
            {
                this.overrideTextBox.Text = profile.TerminalNameOverride;
            }

            this.cursorBlinkCheckBox.Checked = profile.CursorBlink;
            this.cursorType.Value = profile.CursorType;
            this.crosshairCursorCheckBox.Checked = profile.CrosshairCursor;
            this.monoCaseCheckBox.Checked = profile.Monocase;
        }

        /// <summary>
        /// The contents of the terminal override text box have been validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverrideTextBoxValidated(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.TerminalNameOverride = this.overrideTextBox.Text, ChangeName(B3270.Setting.TermName));
        }

        /// <summary>
        /// Change handler for the Model combo box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ModelBoxChanged(object sender, EventArgs e)
        {
            if (this.lockedControls.Contains(sender))
            {
                return;
            }

            var modelDimensions = this.modelComboBox.SelectedItem as ModelDimensions;
            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.Model = modelDimensions.Model;
                    current.Oversize = new Profile.OversizeClass();
                },
                ChangeName(ChangeKeyword.Model));
        }

        /// <summary>
        /// Updates the rows and columns in the profile.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        private void UpdateRowsCols(object sender)
        {
            if (!this.lockedControls.Contains(sender))
            {
                var newOversize = new Profile.OversizeClass { Rows = (int)this.rowsUpDown.Value, Columns = (int)this.columnsUpDown.Value };
                var dimensions = this.modelComboBox.SelectedItem as ModelDimensions;
                if (newOversize.Rows == dimensions.Rows && newOversize.Columns == dimensions.Columns)
                {
                    newOversize = new Profile.OversizeClass { Rows = 0, Columns = 0 };
                }

                this.ProfileManager.PushAndSave(
                    (current) => current.Oversize = newOversize,
                    ChangeName(ChangeKeyword.Model));
            }
        }

        /// <summary>
        /// The value of the oversize rows changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RowsUpDownValueChanged(object sender, EventArgs e)
        {
            this.columnsUpDown.Maximum = OversizeMax / this.rowsUpDown.Value;
            this.UpdateRowsCols(sender);
        }

        /// <summary>
        /// The value of the oversize columns changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColumnsUpDownValueChanged(object sender, EventArgs e)
        {
            this.rowsUpDown.Maximum = OversizeMax / this.columnsUpDown.Value;
            this.UpdateRowsCols(sender);
        }

        /// <summary>
        /// The Oversize check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OversizeCheckBoxClick(object sender, EventArgs e)
        {
            this.rowsUpDown.Enabled = this.oversizeCheckBox.Checked;
            this.columnsUpDown.Enabled = this.oversizeCheckBox.Checked;
            this.rowsUpDown.Minimum = this.UiModelDimensions.Rows;
            this.columnsUpDown.Minimum = this.UiModelDimensions.Columns;
            this.SafeControlModify(this.rowsUpDown, () => this.rowsUpDown.Value = this.UiModelDimensions.Rows);
            this.SafeControlModify(this.columnsUpDown, () => this.columnsUpDown.Value = this.UiModelDimensions.Columns);

            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    if (this.oversizeCheckBox.Checked)
                    {
                        current.Oversize = new Profile.OversizeClass
                        {
                            Rows = this.UiModelDimensions.Rows,
                            Columns = this.UiModelDimensions.Columns,
                        };
                    }
                    else
                    {
                        current.Oversize = new Profile.OversizeClass();
                    }
                },
                ChangeName(ChangeKeyword.Model));
        }

        /// <summary>
        /// The terminal name override check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverrideCheckBoxClick(object sender, EventArgs e)
        {
            if (this.overrideCheckBox.Checked)
            {
                // Override on.
                this.overrideTextBox.Enabled = true;
                this.extendedCheckBox.Enabled = false;
            }
            else
            {
                // Override off.
                this.overrideTextBox.Enabled = false;
                this.extendedCheckBox.Enabled = true;
                this.extendedCheckBox.Checked = this.extendedMode;

                this.ProfileManager.PushAndSave((current) => current.TerminalNameOverride = string.Empty, ChangeName(B3270.Setting.TermName));
            }
        }

        /// <summary>
        /// The Extended check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ExtendedCheckBoxClick(object sender, EventArgs e)
        {
            this.extendedMode = this.extendedCheckBox.Checked;
            this.ProfileManager.PushAndSave((current) => current.ExtendedMode = this.extendedMode, ChangeName(ChangeKeyword.Model));
        }

        /// <summary>
        /// The back end indicated a terminal name change.
        /// </summary>
        /// <param name="nameAndOverride">Terminal name and override.</param>
        private void BackEndChangedTerminalName((string, bool) nameAndOverride)
        {
            this.overrideTextBox.Text = nameAndOverride.Item1;      // name
            this.overrideCheckBox.Checked = nameAndOverride.Item2;  // override
        }

        /// <summary>
        /// The color mode changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColorModeChanged(object sender, EventArgs e)
        {
            // Tell everyone about the mode change.
            this.ColorModeChangedEvent(this.ColorMode);

            // Change the profile.
            this.ProfileManager.PushAndSave((current) => current.ColorMode = this.ColorMode, ChangeName(ChangeKeyword.ColorMode));
        }

        /// <summary>
        /// The code page index (UI control) changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CodePageChanged(object sender, EventArgs e)
        {
            if (!(this.codePageListBox.SelectedItem is string codePage))
            {
                return;
            }

            if (this.lockedControls.Contains(this.codePageListBox))
            {
                return;
            }

            var canonicalName = this.app.CodePageDb.CanonicalName(codePage);
            if (canonicalName != codePage)
            {
                this.codePageListBox.SelectedIndex = this.app.CodePageDb.Index(canonicalName);
            }

            this.ProfileManager.PushAndSave((current) => current.HostCodePage = canonicalName, ChangeName(B3270.Setting.CodePage));
        }

        /// <summary>
        /// Set the window opacity.
        /// </summary>
        /// <param name="percent">Opacity percent.</param>
        private void SetOpacity(int percent)
        {
            this.opacityLabel.Text = $"{percent}";
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
            this.ProfileManager.PushAndSave((current) => current.OpacityPercent = this.opacityTrackBar.Value, ChangeName("opacity"));
        }

        /// <summary>
        /// The profile has changed. Update the Options defaults.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileOptionsChanged(Profile oldProfile, Profile newProfile)
        {
            // Set up the GUI.
            this.EmulationSet(newProfile);
            this.OptionsSet(newProfile);
        }

        /// <summary>
        /// Set the printer name in the UI.
        /// </summary>
        /// <param name="printerName">Printer name.</param>
        private void SetPrinterName(string printerName)
        {
            if (!string.IsNullOrWhiteSpace(printerName))
            {
                int index;
                if ((index = this.printerComboBox.FindStringExact(printerName)) >= 0)
                {
                    this.printerComboBox.SelectedIndex = index;
                }
                else
                {
                    this.printerComboBox.SelectedIndex = this.printerComboBox.Items.Add(printerName);
                }
            }
            else
            {
                this.printerComboBox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Set the Options settings to a given profile.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void OptionsSet(Profile profile)
        {
            // Handle the profile type.
            var isFull = profile.ProfileType == ProfileType.Full;
            this.optionsSampleGroupBox.Enabled = isFull;
            this.cursorGroupBox.Enabled = isFull;
            this.printerSessionGroupBox.Enabled = isFull;
            this.windowTitleLabel.Enabled = isFull;
            this.titleTextBox.Enabled = isFull;

            // Set the cursor type.
            this.cursorType.Value = profile.CursorType;

            // Set the simple checkboxes.
            var checkBoxes = new[]
            {
                (this.crosshairCursorCheckBox, profile.CrosshairCursor),
                (this.cursorBlinkCheckBox, profile.CursorBlink),
                (this.monoCaseCheckBox, profile.Monocase),
                (this.typeaheadCheckBox, profile.Typeahead),
                (this.showTimingCheckBox, profile.ShowTiming),
                (this.alwaysInsertCheckBox, profile.AlwaysInsert),
            };
            foreach (var box in checkBoxes)
            {
                this.SafeControlModify(box.Item1, () => box.Item1.Checked = box.Item2);
            }

            // Redraw the sample.
            this.optionsScreenSample.Invalidate();

            // Set the printer type and name/path.
            this.printerType.Value = profile.PrinterType;
            if (this.printerType.Value == PrinterType.Printer)
            {
                this.SetPrinterName(profile.Printer);
                this.savePathTextBox.Text = string.Empty;
            }
            else
            {
                this.savePathTextBox.Text = profile.Printer;
                this.printerComboBox.SelectedIndex = -1;
            }

            this.savePathLabel.Enabled = this.printerType.Value == PrinterType.File;
            this.savePathTextBox.Enabled = this.printerType.Value == PrinterType.File;
            this.printerNameLabel.Enabled = this.printerType.Value == PrinterType.Printer;
            this.printerComboBox.Enabled = this.printerType.Value == PrinterType.Printer;

            // Set printer options.
            this.printerOptionsTextBox.Text = profile.PrinterOptions;

            // Set printer code page.
            this.printerCodePageTextBox.Text = profile.PrinterCodePage;

            // Set NOP interval.
            this.nopCheckBox.Checked = profile.NopInterval != 0;

            // Set retry.
            this.retryCheckBox.Checked = profile.Retry;

            // Set IPv4/Ipv6 preference.
            this.preferIpv4CheckBox.Checked = profile.PreferIpv4;
            this.preferIpv6CheckBox.Checked = profile.PreferIpv6;

            // Set description and window title.
            this.descriptionTextBox.Text = profile.Description;
            this.titleTextBox.Text = profile.WindowTitle;

            // Set scroll bar.
            this.scrollbarCheckBox.Checked = profile.ScrollBar;

            // Set menu bar.
            this.menuBarCheckBox.Checked = profile.MenuBar;
        }

        /// <summary>
        /// One of the cursor type buttons changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CursorCheckedChanged(object sender, EventArgs e)
        {
            // Redraw the sample fields.
            this.optionsScreenSample.Invalidate();

            // Change the profile.
            this.ProfileManager.PushAndSave((current) => current.CursorType = this.cursorType.Value, ChangeName(B3270.Setting.AltCursor));
        }

        /// <summary>
        /// One of the printer type radio buttons changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterTypeChanged(object sender, EventArgs e)
        {
            if (this.ProfileManager.Current.PrinterType == this.printerType.Value)
            {
                return;
            }

            // Change the profile.
            if (this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.PrinterType = this.printerType.Value;
                    current.Printer = string.Empty;
                },
                ChangeName(ChangeKeyword.PrinterType)))
            {
                // Change visibility and values.
                this.savePathLabel.Enabled = this.printerType.Value == PrinterType.File;
                this.savePathTextBox.Enabled = this.printerType.Value == PrinterType.File;
                this.printerNameLabel.Enabled = this.printerType.Value == PrinterType.Printer;
                this.printerComboBox.Enabled = this.printerType.Value == PrinterType.Printer;
                this.printerComboBox.SelectedIndex = -1;
                this.savePathTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// A miscellaneous checkbox state was changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MiscCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox checkBox))
            {
                return;
            }

            // Redraw the sample fields.
            var settingName = (string)checkBox.Tag;
            if (new[] { B3270.Setting.Crosshair, B3270.Setting.CursorBlink, B3270.Setting.MonoCase }.Contains(settingName))
            {
                this.optionsScreenSample.Invalidate();
            }

            // If this came from the user clicking on the UI, change the profile.
            if (!this.lockedControls.Contains(checkBox))
            {
                this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        switch (settingName)
                        {
                            case B3270.Setting.Crosshair:
                                current.CrosshairCursor = checkBox.Checked;
                                break;
                            case B3270.Setting.CursorBlink:
                                current.CursorBlink = checkBox.Checked;
                                break;
                            case B3270.Setting.MonoCase:
                                current.Monocase = checkBox.Checked;
                                break;
                            case B3270.Setting.Typeahead:
                                current.Typeahead = checkBox.Checked;
                                break;
                            case B3270.Setting.ShowTiming:
                                current.ShowTiming = checkBox.Checked;
                                break;
                            case B3270.Setting.AlwaysInsert:
                                current.AlwaysInsert = checkBox.Checked;
                                break;
                            case B3270.Setting.Retry:
                                current.Retry = checkBox.Checked;
                                break;
                            case B3270.Setting.PreferIpv4:
                                current.PreferIpv4 = checkBox.Checked;
                                break;
                            case B3270.Setting.PreferIpv6:
                                current.PreferIpv6 = checkBox.Checked;
                                break;
                        }
                    },
                    ChangeName(settingName));
            }
        }

        /// <summary>
        /// A miscellaneous local checkbox changed state.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MiscLocalCheckBoxChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox checkBox))
            {
                return;
            }

            var settingName = (string)checkBox.Tag;
            System.Drawing.Size? size = null;
            switch (settingName)
            {
                case ChangeKeyword.ScrollBar:
                    if (!this.app.NoScrollBar)
                    {
                        size = this.mainScreen.ToggleScrollBar(checkBox.Checked);
                        this.ProfileManager.PushAndSave(
                            (current) =>
                            {
                                current.ScrollBar = checkBox.Checked;
                                if (size != null)
                                {
                                    current.Size = size.Value;
                                }
                            },
                            ChangeName(settingName));
                    }

                    break;
                case ChangeKeyword.MenuBar:
                    if (!this.app.NoButtons)
                    {
                        this.mainScreen.FixedMenuBarSwitch(checkBox.Checked);
                    }

                    break;
            }
        }

        /// <summary>
        /// Paint method for the options screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenPictureBoxPaint(object sender, PaintEventArgs e)
        {
            // Use the standard method.
            this.SamplePaint(sender, e, this.optionsScreenSample);
        }

        /// <summary>
        /// The printer selection changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterComboBoxSelectionChangeCommitted(object sender, EventArgs e)
        {
            var printer = this.printerComboBox.SelectedItem.ToString();
            this.ProfileManager.PushAndSave((current) => current.Printer = printer, ChangeName(B3270.Setting.PrinterName));
        }

        /// <summary>
        /// The printer code page changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterCodePageValidated(object sender, EventArgs e)
        {
            var codePage = this.printerCodePageTextBox.Text.Trim();
            this.ProfileManager.PushAndSave((current) => current.PrinterCodePage = codePage, ChangeName(B3270.Setting.PrinterCodePage));
        }

        /// <summary>
        /// The printer options changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterOptionsValidated(object sender, EventArgs e)
        {
            var options = this.app.Pr3287TraceOptions + this.printerOptionsTextBox.Text.Trim();
            var sentOptions = options;
            if (this.app.Restricted(Restrictions.ExternalFiles))
            {
                // Remove the pr3287 trace option.
                sentOptions = Regex.Replace(options, Pr3287.CommandLineOption.TraceRegex, string.Empty);
            }

            this.ProfileManager.PushAndSave((current) => current.PrinterOptions = options, ChangeName(B3270.Setting.PrinterOptions));
        }

        /// <summary>
        /// The description text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DescriptionTextBoxValidated(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.Description = this.descriptionTextBox.Text, ChangeName(ChangeKeyword.Description));
        }

        /// <summary>
        /// The window title text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TitleTextBoxValidated(object sender, EventArgs e)
        {
            if (this.ProfileManager.PushAndSave((current) => current.WindowTitle = this.titleTextBox.Text, ChangeName(ChangeKeyword.WindowTitle)))
            {
                this.mainScreen.Retitle();
            }
        }

        /// <summary>
        /// The save directory text box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveDirectoryClick(object sender, EventArgs e)
        {
            switch (this.printerSaveFolderBrowserDialog.ShowDialog())
            {
                case DialogResult.Cancel:
                    return;
                case DialogResult.OK:
                    this.savePathTextBox.Text = this.printerSaveFolderBrowserDialog.SelectedPath;
                    this.ProfileManager.PushAndSave((current) => current.Printer = this.savePathTextBox.Text, ChangeName(ChangeKeyword.PrinterSavePath));
                    break;
            }
        }

        /// <summary>
        /// The NOP check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NopClick(object sender, EventArgs e)
        {
            var interval = this.nopCheckBox.Checked ? 30 : 0;
            this.ProfileManager.PushAndSave((current) => current.NopInterval = interval, ChangeName(B3270.Setting.NopSeconds));
        }
    }
}
