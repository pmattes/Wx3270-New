﻿// <copyright file="Settings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using I18nBase;

    using Wx3270.Contracts;

    /// <summary>
    /// Settings dialog.
    /// </summary>
    public partial class Settings : Form
    {
        /// <summary>
        /// Keywords and English text for undo/redo operations.
        /// </summary>
        private static readonly Dictionary<string, string> ExpandedToggleName = new Dictionary<string, string>
        {
            { B3270.Setting.AltCursor, "cursor type" },
            { B3270.Setting.CodePage, "host code page" },
            { B3270.Setting.Crosshair, "crosshair cursor" },
            { B3270.Setting.CursorBlink, "cursor blink" },
            { B3270.Setting.MonoCase, "monocase" },
            { B3270.Setting.NopSeconds, "TELNET NOP option" },
            { B3270.Setting.PreferIpv4, "prefer IPv4 addresses" },
            { B3270.Setting.PreferIpv6, "prefer IPv6 addresses" },
            { B3270.Setting.PrinterCodePage, "printer code page" },
            { B3270.Setting.PrinterName, "printer name" },
            { B3270.Setting.PrinterOptions, "printer options" },
            { B3270.Setting.Proxy, "proxy options" },
            { B3270.Setting.Retry, "retry connection" },
            { B3270.Setting.TermName, "terminal name" },
            { B3270.Setting.Typeahead, "typeahead" },
            { B3270.Setting.ShowTiming, "show timing" },
            { B3270.Setting.AlwaysInsert, "default to insert mode" },
            { ChangeKeyword.Maximize, "maximize window" },
            { ChangeKeyword.Model, "model" },
            { ChangeKeyword.OversizeMode, "oversize mode" },
            { ChangeKeyword.OversizeRows, "oversize rows" },
            { ChangeKeyword.OversizeColumns, "oversize columns" },
            { ChangeKeyword.ColorMode, "color mode" },
            { ChangeKeyword.ExtendedMode, "extended mode" },
            { ChangeKeyword.Color, "color" },
            { ChangeKeyword.KeyboardClick, "keyboard click" },
            { ChangeKeyword.AudibleBell, "audible bell" },
            { ChangeKeyword.Font, "font" },
            { ChangeKeyword.ListeningPort, "listening port" },
            { ChangeKeyword.ListenMode, "listen mode" },
            { ChangeKeyword.Address, "address" },
            { ChangeKeyword.Port, "port" },
            { ChangeKeyword.Misc, "miscellaneous setting" },
            { ChangeKeyword.Description, "profile description" },
            { ChangeKeyword.WindowTitle, "window title" },
            { ChangeKeyword.ScrollBar, "scroll bar" },
            { ChangeKeyword.PrinterType, "printer type" },
            { ChangeKeyword.PrinterSavePath, "printer save path" },
            { ChangeKeyword.MenuBar, "hide menu bar" },
        };

        /// <summary>
        /// Tour dictionary.
        /// </summary>
        private readonly Dictionary<TabPage, IEnumerable<(Control, int?, Orientation)>> tours = new Dictionary<TabPage, IEnumerable<(Control, int?, Orientation)>>();

        /// <summary>
        /// The keypad pop-up.
        /// </summary>
        private Keypad keypad;

        /// <summary>
        /// The APL keypad pop-up.
        /// </summary>
        private AplKeypad aplKeypad;

        /// <summary>
        /// The set of controls being modified programmatically.
        /// </summary>
        private HashSet<Control> lockedControls = new HashSet<Control>();

        /// <summary>
        /// The window handle.
        /// </summary>
        private IntPtr windowHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="mainScreen">Main screen.</param>
        /// <param name="keypad">Keypad pop-up.</param>
        /// <param name="aplKeypad">APL keypad pop-up.</param>
        public Settings(Wx3270App app, MainScreen mainScreen, Keypad keypad, AplKeypad aplKeypad)
        {
            this.InitializeComponent();

            this.BaseInit(app, mainScreen, keypad, aplKeypad);
        }

        /// <summary>
        /// Gets a value indicating whether 3279 (color) mode is in effect.
        /// </summary>
        public bool ColorMode => this.colorMode.Value == ColorModeEnum.Color;

        /// <summary>
        /// Gets the cursor type.
        /// </summary>
        public CursorType CursorType => this.cursorType.Value;

        /// <summary>
        /// Gets the sound manager.
        /// </summary>
        private ISound Sound => this.app.Sound;

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private IBackEnd BackEnd => this.app.BackEnd;

        /// <summary>
        /// Returns the localized string for changing a setting.
        /// </summary>
        /// <param name="setting">Setting name.</param>
        /// <returns>Localized string.</returns>
        public static string ChangeName(string setting)
        {
            return Wx3270.ProfileManager.ChangeName(I18n.Get(SettingPath(setting)));
        }

        /// <summary>
        /// Returns the global localization path for a setting.
        /// </summary>
        /// <param name="setting">Toggle to localize.</param>
        /// <returns>Global path name.</returns>
        public static string SettingPath(string setting)
        {
            return I18n.Combine(nameof(Settings), "setting", setting);
        }

        /// <summary>
        /// Create the sample screen image.
        /// </summary>
        /// <param name="color">True if in 3279 (color) mode.</param>
        /// <param name="withExtras">True to include extra characters.</param>
        /// <returns>Sample image.</returns>
        public ScreenImage CreateSampleImage(bool color, bool withExtras = false)
        {
            var image = new ScreenImage
            {
                MaxRows = withExtras ? 8 : 5,
                MaxColumns = withExtras ? 32 : 29,
                LogicalRows = withExtras ? 8 : 5,
                LogicalColumns = withExtras ? 32 : 29,
                ColorMode = color,
                CursorEnabled = true,
                CursorRow1 = 4,
                CursorColumn1 = 2,
                Image = withExtras ? new Cell[8, 32] : new Cell[5, 29],
                Settings = new SettingsDictionary(),
            };
            for (var row = 0; row < image.MaxRows; row++)
            {
                for (var column = 0; column < image.MaxColumns; column++)
                {
                    image.Image[row, column] = new Cell
                    {
                        Text = ' ',
                        HostBackground = HostColor.NeutralBlack,
                        HostForeground = color ? HostColor.Blue : HostColor.NeutralWhite,
                        GraphicRendition = GraphicRendition.None,
                    };
                }
            }

            // Set up the fields.
            PaintImage(
                image,
                0,
                color ? HostColor.Green : HostColor.NeutralWhite,
                GraphicRendition.None,
                this.LocalizeSample("Unprotected Field"));
            PaintImage(
                image,
                1,
                color ? HostColor.Green : HostColor.NeutralWhite,
                GraphicRendition.Selected,
                this.LocalizeSample("Selected Text"));
            PaintImage(
                image,
                2,
                color ? HostColor.Red : HostColor.NeutralWhite,
                color ? GraphicRendition.None : GraphicRendition.Highlight,
                this.LocalizeSample("Intensified Unprotected Field"));
            PaintImage(
                image,
                3,
                color ? HostColor.Blue : HostColor.NeutralWhite,
                GraphicRendition.None,
                this.LocalizeSample("Protected Field"));
            PaintImage(
                image,
                4,
                color ? HostColor.NeutralWhite : HostColor.NeutralWhite,
                color ? GraphicRendition.None : GraphicRendition.Highlight,
                this.LocalizeSample("Intensified Protected Field"));
            if (withExtras)
            {
                PaintImage(
                    image,
                    6,
                    color ? HostColor.Blue : HostColor.NeutralWhite,
                    GraphicRendition.None,
                    "01234567879!@#¬$€£%^&*()[]{}<>«»");
                PaintImage(
                    image,
                    7,
                    color ? HostColor.Blue : HostColor.NeutralWhite,
                    GraphicRendition.None,
                    "_=+-\\|;:'\",./?¿ÁáÆæÈèÏïÑñÔôØøßÚú");
            }

            return image;
        }

        /// <summary>
        /// Initialize the settings form.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="mainScreen">Main screen form.</param>
        /// <param name="keypad">Keypad form.</param>
        /// <param name="aplKeypad">APL keypad form.</param>
        public void BaseInit(
            Wx3270App app,
            MainScreen mainScreen,
            Keypad keypad,
            AplKeypad aplKeypad)
        {
            this.app = app;
            this.mainScreen = mainScreen;
            this.keypad = keypad;
            this.aplKeypad = aplKeypad;

            // Register the undo/redo buttons.
            this.ProfileManager.RegisterUndoRedo(this.undoButton, this.redoButton, this.toolTip1);

            // Process restrictions.
            if (this.app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);

            // Substitute.
            VersionSpecific.Substitute(this);

            // Do secondary init, now that we are called on demand.
            this.SecondaryInit();
        }

        /// <summary>
        /// Paint a row of text onto an image.
        /// </summary>
        /// <param name="image">Image to modify.</param>
        /// <param name="row">Row to write.</param>
        /// <param name="hostForeground">Foreground color.</param>
        /// <param name="graphicRendition">Graphic rendition.</param>
        /// <param name="text">Text string.</param>
        private static void PaintImage(ScreenImage image, int row, HostColor hostForeground, GraphicRendition graphicRendition, string text)
        {
            for (var column = 0; column < text.Length && column < image.MaxColumns; column++)
            {
                var cell = image.Image[row, column];
                cell.HostForeground = hostForeground;
                cell.Text = text[column];
                cell.GraphicRendition = graphicRendition;
            }
        }

        /// <summary>
        /// Localize a sample field.
        /// </summary>
        /// <param name="text">Text to localize.</param>
        /// <returns>Localized text.</returns>
        private string LocalizeSample(string text)
        {
            return I18n.Localize(this, I18n.Combine("sample", text), text);
        }

        /// <summary>
        /// Secondary initialization.
        /// </summary>
        private void SecondaryInit()
        {
            this.windowHandle = this.Handle;

            // Localize undo/redo.
            foreach (var t in ExpandedToggleName)
            {
                I18n.LocalizeGlobal(SettingPath(t.Key), t.Value);
            }

            // Set up the read-only message and save-as button.
            this.ProfileManager.AddChangeTo((oldProfile, newProfile) => this.readOnlyFlowLayoutPanel.Visible = newProfile.ReadOnly);

            // Set up the tabs.
            this.OptionsTabInit();
            this.SoundsTabInit();
            this.FontTabInit();
            this.ColorTabInit();
            this.KeypadTabInit(this.keypad, this.aplKeypad);
            this.KeyboardTabInit();
            this.ListenTabInit();
            this.ProxyTabInit();
            this.MiscTabInit();
        }

        /// <summary>
        /// Safely modify a control, without any side-effects.
        /// </summary>
        /// <param name="control">Check box to modify.</param>
        /// <param name="action">Action to perform.</param>
        private void SafeControlModify(Control control, Action action)
        {
            try
            {
                this.lockedControls.Add(control);
                action();
            }
            finally
            {
                this.lockedControls.Remove(control);
            }
        }

        /// <summary>
        /// Safely modify a control, without any side-effects.
        /// </summary>
        /// <param name="controls">Controls to modify.</param>
        /// <param name="action">Action to perform.</param>
        private void SafeControlModify(IEnumerable<Control> controls, Action action)
        {
            try
            {
                foreach (var control in controls)
                {
                    this.lockedControls.Add(control);
                }

                action();
            }
            finally
            {
                foreach (var control in controls)
                {
                    this.lockedControls.Remove(control);
                }
            }
        }

        /// <summary>
        /// The Undo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void UndoButtonClick(object sender, EventArgs e)
        {
            this.ProfileManager.Undo();
        }

        /// <summary>
        /// The Redo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RedoButtonClick(object sender, EventArgs e)
        {
            this.ProfileManager.Redo();
        }

        /// <summary>
        /// Paint a sample screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        /// <param name="color">True if in color mode.</param>
        /// <param name="screenBox">Screen box (screen except for status line).</param>
        /// <param name="layoutPanel">Layout panel (background).</param>
        /// <param name="statusLine">Status line.</param>
        /// <param name="separator">Separator between screen and status line.</param>
        /// <param name="withExtras">True to include extra text.</param>
        private void SamplePaint(
            object sender,
            PaintEventArgs e,
            bool color,
            ScreenBox screenBox,
            TableLayoutPanel layoutPanel,
            Label statusLine,
            PictureBox separator,
            bool withExtras)
        {
            // Set up the sample status line.
            layoutPanel.BackColor = color ? this.editedColors.HostColors[HostColor.NeutralBlack] : this.editedColors.MonoColors.Background;
            statusLine.ForeColor = color ? this.editedColors.HostColors[HostColor.Blue] : this.editedColors.MonoColors.Normal;
            separator.BackColor = statusLine.ForeColor;

            var sampleImage = this.CreateSampleImage(color: color, withExtras: withExtras);
            if (this.monoCaseCheckBox.Checked)
            {
                sampleImage.Settings.Add(B3270.Setting.MonoCase, true);
            }

            if (this.cursorBlinkCheckBox.Checked)
            {
                sampleImage.Settings.Add(B3270.Setting.CursorBlink, true);
            }

            sampleImage.Settings.Add(B3270.Setting.AltCursor, this.CursorType == CursorType.Underscore);
            sampleImage.Settings.Add(B3270.Setting.Crosshair, this.crosshairCursorCheckBox.Checked);

            screenBox.ScreenDraw(sender, e, sampleImage, this.editedColors);
        }

        /// <summary>
        /// Paint a sample screen image.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        /// <param name="screenSample">Sample box.</param>
        /// <param name="color">Optional color mode override.</param>
        private void SamplePaint(object sender, PaintEventArgs e, ScreenSample screenSample, bool? color = null, bool withExtras = false)
        {
            this.SamplePaint(sender, e, color.HasValue ? color.Value : this.ColorMode, screenSample.ScreenBox, screenSample.LayoutPanel, screenSample.StatusLine, screenSample.Separator, withExtras);
        }

        /// <summary>
        /// Handler for playing the keyboard click sound sample.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyClickPlayButton_Click(object sender, EventArgs e)
        {
            this.KeyClickPlayButtonClick(sender, e);
        }

        /// <summary>
        /// Handler for playing the console bell sound sample.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConsoleBellPlayButton_Click(object sender, EventArgs e)
        {
            this.ConsoleBellPlayButtonClick(sender, e);
        }

        /// <summary>
        /// Handler for a white-on-black radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void WhiteOnBlack_Click(object sender, EventArgs e)
        {
            this.WhiteOnBlackClick(sender, e);
        }

        /// <summary>
        /// Handler for the black-on-white radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BlackOnWhite_Click(object sender, EventArgs e)
        {
            this.BlackOnWhiteCheckedClick(sender, e);
        }

        /// <summary>
        /// Handler for clicking on any of the color editing buttons.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Color_Click(object sender, EventArgs e)
        {
            this.ColorClick(sender, e);
        }

        /// <summary>
        /// Change the selection background color.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SelectedButton_Click(object sender, EventArgs e)
        {
            this.ChangeSelectionColor();
        }

        /// <summary>
        /// The list index changed for the background color selection.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BackgroundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BackgroundListSelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// The value of the oversize rows changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RowsUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.RowsUpDownValueChanged(sender, e);
        }

        /// <summary>
        /// The value of the oversize columns changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColumnsUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.ColumnsUpDownValueChanged(sender, e);
        }

        /// <summary>
        /// The oversize check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OversizeCheckBox_Click(object sender, EventArgs e)
        {
            this.OversizeCheckBoxClick(sender, e);
        }

        /// <summary>
        /// The terminal name override check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverrideCheckBox_Click(object sender, EventArgs e)
        {
            this.OverrideCheckBoxClick(sender, e);
        }

        /// <summary>
        /// The Extended check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ExtendedCheckBox_Click(object sender, EventArgs e)
        {
            this.ExtendedCheckBoxClick(sender, e);
        }

        /// <summary>
        /// Handler for the green-on-black radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void GreenOnBlackButton_Click(object sender, EventArgs e)
        {
            this.GreenOnBlackButtonClick(sender, e);
        }

        /// <summary>
        /// Handler for the green-on-white radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void GreenOnWhiteButton_Click(object sender, EventArgs e)
        {
            this.GreenOnWhiteButtonClick(sender, e);
        }

        /// <summary>
        /// The list index changed for the monochrome background color selection.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoBackgroundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.MonoBackgroundListSelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Handler for clicking on any of the monochrome color editing buttons.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Mono_Click(object sender, EventArgs e)
        {
            this.MonoClick(sender, e);
        }

        /// <summary>
        /// Paint method for the options screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenPictureBox_Paint(object sender, PaintEventArgs e)
        {
            this.ScreenPictureBoxPaint(sender, e);
        }

        /// <summary>
        /// A miscellaneous checkbox changed state.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MiscCheckBox_Changed(object sender, EventArgs e)
        {
            this.MiscCheckBoxCheckedChanged(sender, e);
        }

        /// <summary>
        /// A miscellaneous local checkbox changed state.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MiscLocalCheckBox_Changed(object sender, EventArgs e)
        {
            this.MiscLocalCheckBoxChanged(sender, e);
        }

        /// <summary>
        /// Paint method for the monochrome preview box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoPreviewScreenPictureBox_Paint(object sender, PaintEventArgs e)
        {
            this.MonoPreviewScreenPictureBoxPaint(sender, e);
        }

        /// <summary>
        /// Paint method for the color preview box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColorPreviewScreenPictureBox_Paint(object sender, PaintEventArgs e)
        {
            this.ColorPreviewScreenPictureBoxPaint(sender, e);
        }

        /// <summary>
        /// Paint method for the font preview screen.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontPreviewScreenPictureBox_Paint(object sender, PaintEventArgs e)
        {
            this.FontPreviewScreenPictureBoxPaint(sender, e);
        }

        /// <summary>
        /// Change the crosshair cursor color.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CrosshairColorButton_Click(object sender, EventArgs e)
        {
            this.CrosshairColorButtonClick(sender, e);
        }

        /// <summary>
        /// One of the keypad modifier check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadModifier_Click(object sender, EventArgs e)
        {
            this.KeypadModifierClick(sender, e);
        }

        /// <summary>
        /// The actions text box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EditedButtonActionsTextBox_Click(object sender, EventArgs e)
        {
            this.EditedButtonActionsTextBoxClick(sender, e);
        }

        /// <summary>
        /// The label text changed for a keypad map.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadRemoveButton_Click(object sender, EventArgs e)
        {
            this.KeypadRemoveButtonClick(sender, e);
        }

        /// <summary>
        /// One of the keyboard action buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardAction_Click(object sender, EventArgs e)
        {
            this.KeyboardActionClick(sender, e);
        }

        /// <summary>
        /// One of the keyboard actions text boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardActions_Click(object sender, EventArgs e)
        {
            this.KeyboardActionsClick(sender, e);
        }

        /// <summary>
        /// The label text changed for a keypad map.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EditedButtonTextTextBox_TextChanged(object sender, EventArgs e)
        {
            this.KeypadTextChanged(sender, e);
        }

        /// <summary>
        /// One of the background image radio buttons changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BackgroundImage_Click(object sender, EventArgs e)
        {
            this.BackgroundImageClick(sender, e);
        }

        /// <summary>
        /// One of the keyboard modifier check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardModifier_Click(object sender, EventArgs e)
        {
            this.KeyboardModifierClick(sender, e);
        }

        /// <summary>
        /// The code page index (UI control) changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CodePageListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CodePageChanged(sender, e);
        }

        /// <summary>
        /// The keyboard click checkbox was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardClick_Click(object sender, EventArgs e)
        {
            this.KeyboardClickClick(sender, e);
        }

        /// <summary>
        /// The audible bell checkbox was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AudibleBell_Click(object sender, EventArgs e)
        {
            this.AudibleBellClick(sender, e);
        }

        /// <summary>
        /// The Undo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            this.UndoButtonClick(sender, e);
        }

        /// <summary>
        /// The Redo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RedoButton_Click(object sender, EventArgs e)
        {
            this.RedoButtonClick(sender, e);
        }

        /// <summary>
        /// The Settings dialog is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.Owner.BringToFront();
        }

        /// <summary>
        /// The display keyboard map layout button was checked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardPictureButton_Click(object sender, EventArgs e)
        {
            this.KeyboardPictureButtonClick(sender, e);
        }

        /// <summary>
        /// The settings dialog was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Settings_Activated(object sender, EventArgs e)
        {
            if (!Tour.IsComplete(this.settingsTabs.SelectedTab))
            {
                this.RunTour(this.settingsTabs.SelectedTab);
            }
        }

        /// <summary>
        /// Run the tour for a particular tab page.
        /// </summary>
        /// <param name="selectedTab">Selected tab.</param>
        /// <param name="isExplicit">True if invoked explicitly.</param>
        private void RunTour(TabPage selectedTab, bool isExplicit = false)
        {
            var nodes = new List<(Control, int?, Orientation)>();
            if (this.tours.TryGetValue(selectedTab, out IEnumerable<(Control, int?, Orientation)> tabNodes))
            {
                nodes.AddRange(tabNodes);
            }

            var commonButtonNodes = new[]
            {
                ((Control)this, (int?)99, Orientation.Centered),
                (this.readOnlyFlowLayoutPanel, null, Orientation.LowerLeftTight),
                (this.setToDefaultsButton, null, Orientation.LowerRight),
                (this.undoButton, null, Orientation.LowerRight),
                (this.helpPictureBox, null, Orientation.LowerRight),
            };
            nodes.AddRange(commonButtonNodes);
            Tour.Navigate(selectedTab, nodes, isExplicit: isExplicit);
        }

        /// <summary>
        /// The label size text box was changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadTextSize_Validating(object sender, CancelEventArgs e)
        {
            this.KeypadTextSizeValidating(sender, e);
        }

        /// <summary>
        /// The contents of the terminal override text box have been validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverrideTextBox_Validated(object sender, EventArgs e)
        {
            this.OverrideTextBoxValidated(sender, e);
        }

        /// <summary>
        /// The Defaults button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            this.ProfileManager.SetToDefaults();
        }

        /// <summary>
        /// The Help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// The keypad label text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EditedButtonTextTextBox_Validated(object sender, EventArgs e)
        {
            this.EditedButtonTextTextBoxValidated(sender, e);
        }

        /// <summary>
        /// One of the listener check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ListenerCheckBox_Click(object sender, EventArgs e)
        {
            this.ListenerCheckBoxClick(sender, e);
        }

        /// <summary>
        /// The printer selection changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.PrinterComboBoxSelectionChangeCommitted(sender, e);
        }

        /// <summary>
        /// The printer code page changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterCodePage_validated(object sender, EventArgs e)
        {
            this.PrinterCodePageValidated(sender, e);
        }

        /// <summary>
        /// The printer options changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterOptions_validated(object sender, EventArgs e)
        {
            this.PrinterOptionsValidated(sender, e);
        }

        /// <summary>
        /// The NOP check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Nop_Click(object sender, EventArgs e)
        {
            this.NopClick(sender, e);
        }

        /// <summary>
        /// One of the emulator mode check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EmulatorMode_Click(object sender, EventArgs e)
        {
            this.EmulatorModeClick(sender, e);
        }

        /// <summary>
        /// One of the match type radio buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MatchType_Click(object sender, EventArgs e)
        {
            this.MatchTypeClick(sender, e);
        }

        /// <summary>
        /// The input language changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keyboard_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            this.KeyboardInputLanguageChanged(sender, e);
        }

        /// <summary>
        /// The selection index on the chord combo box changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ChordComboBox_IndexChanged(object sender, EventArgs e)
        {
            this.ChordComboBoxIndexChanged(sender, e);
        }

        /// <summary>
        /// One of the chord-mode radio buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Chord_Click(object sender, EventArgs e)
        {
            this.ChordClick(sender, e);
        }

        /// <summary>
        /// The proxy edit button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyEditButton_Click(object sender, EventArgs e)
        {
            this.ProxyEditButtonClick(sender, e);
        }

        /// <summary>
        /// The proxy type text box became active.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyTypeTextBox_Enter(object sender, EventArgs e)
        {
            this.ProxyTypeTextBoxEnter(sender, e);
        }

        /// <summary>
        /// The opacity track bar was scrolled.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Opacity_Scroll(object sender, EventArgs e)
        {
            this.OpacityScroll(sender, e);
        }

        /// <summary>
        /// The opacity timer ticked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OpacityTimer_Tick(object sender, EventArgs e)
        {
            this.OpacityTimerTick(sender, e);
        }

        /// <summary>
        /// One of the edit buttons on the Servers tab was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ServerEdit_Click(object sender, EventArgs e)
        {
            this.ServerEditClick(sender, e);
        }

        /// <summary>
        /// One of the server text boxes got focus.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ServerTextBox_Enter(object sender, EventArgs e)
        {
            this.ServerTextBoxEnter(sender, e);
        }

        /// <summary>
        /// The description text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DescriptionTextBox_Validated(object sender, EventArgs e)
        {
            this.DescriptionTextBoxValidated(sender, e);
        }

        /// <summary>
        /// The window title text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TitleTextBox_Validated(object sender, EventArgs e)
        {
            this.TitleTextBoxValidated(sender, e);
        }

        /// <summary>
        /// The exact match check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ExactMatchCheckBox_Click(object sender, EventArgs e)
        {
            this.ExactMatchCheckBoxClick(sender, e);
        }

        /// <summary>
        /// The save directory text box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveDirectory_Click(object sender, EventArgs e)
        {
            this.SaveDirectoryClick(sender, e);
        }

        /// <summary>
        /// The 'Save a copy' button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveACopyButtonClick(object sender, EventArgs e)
        {
            this.mainScreen.DuplicateProfile();
        }

        /// <summary>
        /// The selected tab changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event options.</param>
        private void TabSelectedChanged(object sender, EventArgs e)
        {
            if (!Tour.IsComplete(this.settingsTabs.SelectedTab))
            {
                this.RunTour(this.settingsTabs.SelectedTab);
            }
        }

        /// <summary>
        /// Registers a tour for a tab.
        /// </summary>
        /// <param name="tabPage">Tab page.</param>
        /// <param name="nodes">Nodes for the tour.</param>
        private void RegisterTour(TabPage tabPage, IEnumerable<(Control, int?, Orientation)> nodes)
        {
            this.tours[tabPage] = nodes;
        }

        /// <summary>
        /// An entry in the help menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "Settings/" + Wx3270App.FormatHelpTag(this.settingsTabs.SelectedTab.Name), () => this.RunTour(this.settingsTabs.SelectedTab, isExplicit: true));
        }

        /// <summary>
        /// The Follower button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Follower_Click(object sender, EventArgs e)
        {
            this.FollowerClick(sender, e);
        }

        /// <summary>
        /// The settings window was loaded.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SettingsLoad(object sender, EventArgs e)
        {
            // For some reason, this window will not center on its parent without doing this explicitly.
            this.CenterToParent();
        }

        /// <summary>
        /// The font family selection changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontFamily_Changed(object sender, EventArgs e)
        {
            this.FontFamilyChanged(sender, e);
        }

        /// <summary>
        /// The font size changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontSize_Changed(object sender, EventArgs e)
        {
            this.FontSizeChanged(sender, e);
        }

        /// <summary>
        /// The font size is validating.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontSize_Validating(object sender, CancelEventArgs e)
        {
            this.FontSizeValidating(sender, e);
        }

        /// <summary>
        /// The Bold button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Bold_Click(object sender, EventArgs e)
        {
            this.BoldClick(sender, e);
        }

        /// <summary>
        /// The Italic button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Italic_Click(object sender, EventArgs e)
        {
            this.ItalicClick(sender, e);
        }

        /// <summary>
        /// The familyComboBox needs to be drawn.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event agruments.</param>
        private void FamilyComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            this.FamilyComboBoxDrawItem(sender, e);
        }

        /// <summary>
        /// The family combo box drop down was opened.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FamilyComboBox_DropDown(object sender, EventArgs e)
        {
            this.FamilyComboBoxDropDown(sender, e);
        }

        /// <summary>
        /// The family combo box drop down was closed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FamilyComboBox_DropDownClosed(object sender, EventArgs e)
        {
            this.FamilyComboBoxDropDownClosed(sender, e);
        }

        /// <summary>
        /// Names of settings that change without specific toggles.
        /// </summary>
        public class ChangeKeyword
        {
            /// <summary>
            /// Maximize the screen.
            /// </summary>
            public const string Maximize = "Maximize";

            /// <summary>
            /// Model number.
            /// </summary>
            public const string Model = "Model";

            /// <summary>
            /// Oversize mode.
            /// </summary>
            public const string OversizeMode = "OversizeMode";

            /// <summary>
            /// Oversize rows.
            /// </summary>
            public const string OversizeRows = "OversizeRows";

            /// <summary>
            /// Oversize columns.
            /// </summary>
            public const string OversizeColumns = "OversizeColumns";

            /// <summary>
            /// Color mode.
            /// </summary>
            public const string ColorMode = "ColorMode";

            /// <summary>
            /// Extended mode.
            /// </summary>
            public const string ExtendedMode = "ExtendedMode";

            /// <summary>
            /// Color mapping.
            /// </summary>
            public const string Color = "Color";

            /// <summary>
            /// Keyboard click.
            /// </summary>
            public const string KeyboardClick = "KeyboardClick";

            /// <summary>
            /// Audible bell.
            /// </summary>
            public const string AudibleBell = "AudibleBell";

            /// <summary>
            /// Font.
            /// </summary>
            public const string Font = "Font";

            /// <summary>
            /// Listening port.
            /// </summary>
            public const string ListeningPort = "ListeningPort";

            /// <summary>
            /// Listen mode.
            /// </summary>
            public const string ListenMode = "ListenMode";

            /// <summary>
            /// Listen address.
            /// </summary>
            public const string Address = "Address";

            /// <summary>
            /// Listen port.
            /// </summary>
            public const string Port = "Port";

            /// <summary>
            /// Miscellaneous setting.
            /// </summary>
            public const string Misc = "Misc";

            /// <summary>
            /// Profile description.
            /// </summary>
            public const string Description = "Description";

            /// <summary>
            /// Window title.
            /// </summary>
            public const string WindowTitle = "WindowTitle";

            /// <summary>
            /// Scroll bar.
            /// </summary>
            public const string ScrollBar = "ScrollBar";

            /// <summary>
            /// Printer type (real printer or save to files).
            /// </summary>
            public const string PrinterType = "PrinterType";

            /// <summary>
            /// Printer save path.
            /// </summary>
            public const string PrinterSavePath = "PrinterSavePath";

            /// <summary>
            /// Menu bar.
            /// </summary>
            public const string MenuBar = "MenuBar";
        }

        /// <summary>
        /// Context for a sample screen image.
        /// </summary>
        private class ScreenSample
        {
            /// <summary>
            /// The settings form.
            /// </summary>
            private Settings settings;

            /// <summary>
            /// Initializes a new instance of the <see cref="ScreenSample"/> class.
            /// </summary>
            /// <param name="settings">Settings object.</param>
            /// <param name="screenPictureBox">Picture box with screen image.</param>
            /// <param name="tableLayoutPanel">Table layout panel encompassing screen and status line.</param>
            /// <param name="statusLine">Status line.</param>
            /// <param name="separator">Separator between screen and status line.</param>
            /// <param name="colorMode">True if in 3279 mode.</param>
            /// <param name="withExtras">True to include extra characters.</param>
            public ScreenSample(
                Settings settings,
                PictureBox screenPictureBox,
                TableLayoutPanel tableLayoutPanel,
                Label statusLine,
                PictureBox separator,
                bool colorMode,
                bool withExtras)
            {
                this.settings = settings;
                this.ScreenBox = new ScreenBox("Sample", screenPictureBox);
                this.LayoutPanel = tableLayoutPanel;
                this.StatusLine = statusLine;
                this.Separator = separator;
                this.ScreenBox.ScreenNewFont(statusLine.Font, settings.CreateSampleImage(colorMode, withExtras));
                this.ScreenBox.Activated(true);
            }

            /// <summary>
            /// Gets the main screen image.
            /// </summary>
            public ScreenBox ScreenBox { get; private set; }

            /// <summary>
            /// Gets the background container.
            /// </summary>
            public TableLayoutPanel LayoutPanel { get; private set; }

            /// <summary>
            /// Gets the status line label.
            /// </summary>
            public Label StatusLine { get; private set; }

            /// <summary>
            /// Gets the separator line.
            /// </summary>
            public PictureBox Separator { get; private set; }

            /// <summary>
            /// Invalidate the screen so it gets redrawn.
            /// </summary>
            /// <param name="withExtras">True to include extra characters.</param>
            public void Invalidate(bool withExtras = false)
            {
                this.ScreenBox.ScreenNeedsDrawing("settings sample", true, this.settings.CreateSampleImage(this.settings.ColorMode, withExtras));
            }
        }
    }
}
