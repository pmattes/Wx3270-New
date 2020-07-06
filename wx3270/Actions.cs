// <copyright file="Actions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for mode-change events.
    /// </summary>
    /// <param name="isSet">True if mode is now set.</param>
    public delegate void ModeHandler(bool isSet);

    /// <summary>
    /// The actions dialog.
    /// </summary>
    public partial class Actions : Form
    {
        /// <summary>
        /// Title name, for localization.
        /// </summary>
        private static readonly string TitleName = I18n.TitleName(nameof(Actions));

        /// <summary>
        /// Message name, for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Actions));

        /// <summary>
        /// The application instance.
        /// </summary>
        private Wx3270App app;

        /// <summary>
        /// The main screen, for callbacks.
        /// </summary>
        private MainScreen mainScreen;

        /// <summary>
        /// Connection to cmd.exe.
        /// </summary>
        private Cmd cmd;

        /// <summary>
        /// The visible control codes document.
        /// </summary>
        private VisibleControls visibleControls;

        /// <summary>
        /// True if the actions dialog was ever activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// The window handle.
        /// </summary>
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Actions"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="mainScreen">Main screen.</param>
        public Actions(Wx3270App app, MainScreen mainScreen)
        {
            this.InitializeComponent();

            // Do basic set-up.
            this.app = app;
            this.mainScreen = mainScreen;
            this.cmd = new Cmd(this.BackEnd);
            this.handle = this.Handle;

            // Initialize the tabs.
            this.FileTransferTabInit();
            this.StatusTabInit();

            // Subscribe to connection changes.
            this.mainScreen.ConnectionStateEvent += () =>
            {
                this.cmdButton.Enabled = this.app.Allowed(Restrictions.Prompt) && this.app.ConnectionState == ConnectionState.NotConnected;
            };

            // Disable the debug console button if it is already allocated.
            if (this.app.ConsoleAttached)
            {
                this.consoleCheckBox.Enabled = false;
            }

            // Subscribe to toggle changes.
            this.app.SettingChange.Register(
                (settingName, settingDictionary) => this.Invoke(new MethodInvoker(() => this.SettingChanged(settingName, settingDictionary))),
                new[] { B3270.Setting.Trace, B3270.Setting.VisibleControl, B3270.Setting.ScreenTrace });

            // Apply restrictions.
            if (this.app.Restricted(Restrictions.FileTransfer))
            {
                this.fileTransferTab.RemoveFromParent();
            }

            if (this.app.Restricted(Restrictions.Prompt))
            {
                this.promptButton.RemoveFromParent();
                this.promptLabel.RemoveFromParent();
                this.consoleCheckBox.RemoveFromParent();
                this.cmdButton.RemoveFromParent();
                this.cmdExeLabel.RemoveFromParent();
            }

            this.fileRadioButton.Enabled = app.Allowed(Restrictions.ExternalFiles);

            if (this.app.Restricted(Restrictions.ExternalFiles))
            {
                this.traceCheckBox.RemoveFromParent();
                this.tracePr3287CheckBox.RemoveFromParent(); // How do we prevent pr3287 command line tracing?
            }

            if (this.developerGroupBox.Controls.Count == 0)
            {
                this.developerGroupBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);
            I18n.LocalizeGlobal(Title.ReEnableKeyboard, "Re-Enable Keyboard");
            I18n.LocalizeGlobal(Title.ModeChange, "Mode Change");
            I18n.LocalizeGlobal(Title.SaveLocalization, "Save Localization");

            // Localize the file transfer tab.
            FileTransferLocalize();

            // The About tab gets initialized after localization.
            this.AboutTabInit();
        }

        /// <summary>
        /// Gets or sets a value indicating whether tracing is active.
        /// </summary>
        public bool Tracing
        {
            get
            {
                return this.traceCheckBox.Checked;
            }

            set
            {
                this.traceCheckBox.Checked = value;
                this.TraceChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control characters are visible.
        /// </summary>
        public bool VisibleControl
        {
            get
            {
                return this.visibleControlCheckBox.Checked;
            }

            set
            {
                this.visibleControlCheckBox.Checked = value;
                this.ChangeVisibleControl(value);
            }
        }

        /// <summary>
        /// Gets the trace options for pr3287.
        /// </summary>
        public string Pr3287TraceOptions =>
            this.app.Allowed(Restrictions.ExternalFiles) && this.tracePr3287CheckBox.Checked ?
                Pr3287.CommandLineOption.Trace + " " + Pr3287.CommandLineOption.TraceDir + " \"" + Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\" " :
                string.Empty;

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private IBackEnd BackEnd => this.app.BackEnd;

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager => this.app.ProfileManager;

        /// <summary>
        /// Cancel pending scripts.
        /// </summary>
        public void CancelScripts()
        {
            this.BackEnd.RunAction(new BackEndAction(B3270.Action.Abort), ErrorBox.Ignore());
            if (this.app.ConnectionState != ConnectionState.NotConnected && this.app.ConnectionState < ConnectionState.ConnectedNvt)
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Disconnect), ErrorBox.Ignore());
            }
        }

        /// <summary>
        /// Re-enable the keyboard.
        /// </summary>
        public void ReenableKeyboard()
        {
            this.BackEnd.RunAction(
                new BackEndAction(B3270.Action.KeyboardDisable, B3270.Value.ForceEnable),
                ErrorBox.Completion(I18n.Get(Title.ReEnableKeyboard)));
        }

        /// <summary>
        /// The Print Text button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void PrintTextButton_Click(object sender, EventArgs e)
        {
            this.BackEnd.RunAction(
                new BackEndAction(B3270.Action.PrintText, B3270.Value.Gdi, B3270.Value.Dialog),
                Wx3270.BackEnd.Ignore());
        }

        /// <summary>
        /// Display the keymap.
        /// </summary>
        public void DisplayKeymap()
        {
            this.KeymapClick(null, null);
        }

        /// <summary>
        /// Change the state of screen tracing.
        /// </summary>
        /// <param name="tag">Menu item tag.</param>
        public void ToggleScreenTracing(string tag)
        {
            switch (tag)
            {
                case "Printer":
                    if (!this.traceScreenCheckBox.Checked)
                    {
                        this.printerRadioButton.Checked = true;
                        this.fileRadioButton.Checked = false;
                        this.traceScreenCheckBox.Checked = this.ChangeScreenTrace(true);
                    }

                    break;
                case "File":
                    if (!this.traceScreenCheckBox.Checked)
                    {
                        this.printerRadioButton.Checked = false;
                        this.fileRadioButton.Checked = true;
                        this.traceScreenCheckBox.Checked = this.ChangeScreenTrace(true);
                    }

                    break;
                case "Toggle":
                    if (this.traceScreenCheckBox.Checked)
                    {
                        this.traceScreenCheckBox.Checked = this.ChangeScreenTrace(false);
                    }

                    break;
            }
        }

        /// <summary>
        /// A setting value changed.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Setting dictionary.</param>
        private void SettingChanged(string settingName, SettingsDictionary settingDictionary)
        {
            switch (settingName)
            {
                case B3270.Setting.Trace:
                    this.traceCheckBox.Checked = settingDictionary.TryGetValue(B3270.Setting.Trace, out bool trace) && trace;
                    break;
                case B3270.Setting.VisibleControl:
                    this.visibleControlCheckBox.Checked = settingDictionary.TryGetValue(B3270.Setting.VisibleControl, out bool visibleControl) && visibleControl;
                    break;
                case B3270.Setting.ScreenTrace:
                    this.traceScreenCheckBox.Checked = settingDictionary.TryGetValue(B3270.Setting.ScreenTrace, out bool screenTrace) && screenTrace;
                    break;
            }
        }

        /// <summary>
        /// Hide this window, without pushing our parent to the back.
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
        /// The console button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConsoleButton_Click(object sender, EventArgs e)
        {
            this.app.Prompt.Start();
            this.SafeHide();
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.SafeHide();
        }

        /// <summary>
        /// The command window button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CmdButton_Click(object sender, EventArgs e)
        {
            if (!this.cmd.Live)
            {
                // We need a new one.
                this.cmd = new Cmd(this.BackEnd);
            }

            this.cmd.Connect();
            this.SafeHide();
        }

        /// <summary>
        /// Change tracing mode.
        /// </summary>
        /// <param name="value">True if tracing should now be on.</param>
        private void TraceChanged(bool value)
        {
            this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.Trace, B3270.ToggleArgument.Action(value)), ErrorBox.Ignore());
            if (value)
            {
                this.SafeHide();
            }
        }

        /// <summary>
        /// The trace check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TraceCheckBox_Click(object sender, EventArgs e)
        {
            this.TraceChanged(this.traceCheckBox.Checked);
        }

        /// <summary>
        /// The disconnect button was pressed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            this.mainScreen.Connect.Disconnect();
        }

        /// <summary>
        /// The file transfer local file name was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LocalFileTextBox_Click(object sender, EventArgs e)
        {
            this.FileTransferLocalFileTextBoxClick(sender, e);
        }

        /// <summary>
        /// The file transfer clear form button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferClearFormButton_Click(object sender, EventArgs e)
        {
            this.FileTransferClearFormButtonClick(sender, e);
        }

        /// <summary>
        /// One of the file transfer options changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferOptionChanged(object sender, EventArgs e)
        {
            this.EnableDisable();
        }

        /// <summary>
        /// Validation for the code page text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferNumeric_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.FileTransferNumericValidating(sender, e);
        }

        /// <summary>
        /// Validation for the local file text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LocalFileTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.LocalFileTextBoxValidating(sender, e);
        }

        /// <summary>
        /// Validation for the local file text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HostFileTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.HostFileTextBoxValidating(sender, e);
        }

        /// <summary>
        /// The transfer button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TransferButton_Click(object sender, EventArgs e)
        {
            this.TransferButtonClick(sender, e);
        }

        /// <summary>
        /// Change visible control character mode.
        /// </summary>
        /// <param name="isSet">True if the mode is set.</param>
        private void ChangeVisibleControl(bool isSet)
        {
            this.BackEnd.RunAction(
                new BackEndAction(B3270.Action.Set, B3270.Setting.VisibleControl, B3270.ToggleArgument.Action(isSet)),
                ErrorBox.Completion(I18n.Get(Title.ModeChange)));
        }

        /// <summary>
        /// The visible control characters check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void VisibleControlCheckBox_Clicked(object sender, EventArgs e)
        {
            this.ChangeVisibleControl(this.visibleControlCheckBox.Checked);
            this.SafeHide();
        }

        /// <summary>
        /// The control character codes button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ControlCharDocButton_Click(object sender, EventArgs e)
        {
            if (this.visibleControls == null)
            {
                this.visibleControls = new VisibleControls();
            }

            this.visibleControls.Show(this.mainScreen);
        }

        /// <summary>
        /// The connection timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectionTimer_Tick(object sender, EventArgs e)
        {
            this.StatusTimerTick(sender, e);
        }

        /// <summary>
        /// The cancel actions button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelActionsButton_Click(object sender, EventArgs e)
        {
            this.CancelScripts();
            this.SafeHide();
        }

        /// <summary>
        /// The console check box changed state.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConsoleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // A console can only be turned on, never off.
            this.app.AttachConsole();
            this.consoleCheckBox.Enabled = false;
            this.SafeHide();
        }

        /// <summary>
        /// The actions dialog was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Actions_Activated(object sender, EventArgs e)
        {
            if (!this.everActivated)
            {
                this.everActivated = true;
                this.Location = MainScreen.CenteredOn(this.mainScreen, this);
            }
        }

        /// <summary>
        /// Change screen trace mode.
        /// </summary>
        /// <param name="isOn">True if screen trace is enabled.</param>
        /// <returns>New value of screen trace mode.</returns>
        private bool ChangeScreenTrace(bool isOn)
        {
            var toggled = this.app.SettingChange.SettingDictionary.TryGetValue(B3270.Setting.ScreenTrace, out bool screenTrace) && screenTrace;

            if (isOn && !toggled)
            {
                // Turn screen tracing on.
                if (this.fileRadioButton.Checked)
                {
                    this.screenTraceFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    switch (this.screenTraceFileDialog.ShowDialog())
                    {
                        default:
                        case DialogResult.Cancel:
                            return false;
                        case DialogResult.OK:
                            break;
                    }

                    this.BackEnd.RunAction(
                        new BackEndAction(B3270.Action.ScreenTrace, "on", "file", this.screenTraceFileDialog.FileName),
                        (cookie, success, result) => { });
                }
                else
                {
                    this.BackEnd.RunAction(
                        new BackEndAction(B3270.Action.ScreenTrace, "on", "printer", "gdi", "dialog"),
                        (cookie, success, result) => { });
                }

                this.SafeHide();
            }
            else if (!isOn && toggled)
            {
                // Turn screen tracing off.
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.ScreenTrace, "off"),
                    (cookie, success, result) => { });

                this.SafeHide();
            }

            this.printerRadioButton.Checked = !isOn;
            this.printerRadioButton.Enabled = !isOn;
            this.fileRadioButton.Checked = false;
            this.fileRadioButton.Enabled = !isOn;
            return isOn;
        }

        /// <summary>
        /// The trace screen check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TraceScreen_Click(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                checkBox.Checked = this.ChangeScreenTrace(checkBox.Checked);
            }
        }

        /// <summary>
        /// The Help picture box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Clicked(object sender, EventArgs e)
        {
            if (sender is PictureBox box)
            {
                if (box.Tag is string tag)
                {
                    Wx3270App.GetHelp("Actions/" + Wx3270App.FormatHelpTag(tag));
                }
            }
        }

        /// <summary>
        /// The wx3270 prompt button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PromptCheckBox_Click(object sender, EventArgs e)
        {
            // Start the wx3270 prompt.
            this.app.Prompt.Start();
            this.SafeHide();
        }

        /// <summary>
        /// The file transfer Copy Action button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CopyActionButton_Click(object sender, EventArgs e)
        {
            this.CopyActionButtonClick(sender, e);
        }

        /// <summary>
        /// The Re-enable Keyboard button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ReenableButton_Click(object sender, EventArgs e)
        {
            this.ReenableKeyboard();
        }

        /// <summary>
        /// The pr3287 trace check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TracePr3287_Click(object sender, EventArgs e)
        {
            // Send the new pr3287 options.
            var options = this.Pr3287TraceOptions + this.ProfileManager.Current.PrinterOptions;
            if (this.app.Restricted(Restrictions.ExternalFiles))
            {
                // Remove the pr3287 trace option.
                options = Regex.Replace(options, @"\b" + Pr3287.CommandLineOption.Trace + @"\b", string.Empty);
            }

            this.BackEnd.RunAction(
                new BackEndAction(B3270.Action.Set, B3270.Setting.PrinterOptions, options),
                ErrorBox.Completion(I18n.Get(Title.ModeChange)));
        }

        /// <summary>
        /// The keymap display button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeymapClick(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this.SafeHide();
            }

            var keyboardModifier = this.app.AplMode ? KeyboardModifier.Apl : KeyboardModifier.None;
            if (Oia.StateIs3270orSscp(this.app.ConnectionState))
            {
                keyboardModifier |= KeyboardModifier.Mode3270;
            }
            else if (Oia.StateIsNvt(this.app.ConnectionState))
            {
                keyboardModifier |= KeyboardModifier.ModeNvt;
            }

            var k = new KeyboardPicture(this.ProfileManager.Current.KeyboardMap, PictureMode.Display, this.app)
            {
                KeyboardModifier = keyboardModifier,
            };

            k.Show(this.mainScreen);
        }

        /// <summary>
        /// Pop-up dialog titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Re-enable keyboard error.
            /// </summary>
            public static readonly string ReEnableKeyboard = I18n.Combine(TitleName, "reEnableKeyboard");

            /// <summary>
            /// Mode change.
            /// </summary>
            public static readonly string ModeChange = I18n.Combine(TitleName, "modeChange");

            /// <summary>
            /// Save localization.
            /// </summary>
            public static readonly string SaveLocalization = I18n.Combine(TitleName, "saveLocalization");
        }
    }
}
