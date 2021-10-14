// <copyright file="Actions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for mode-change events.
    /// </summary>
    /// <param name="isSet">True if mode is now set.</param>
    public delegate void ModeHandler(bool isSet);

    /// <summary>
    /// Screen trace type enumeration.
    /// </summary>
    public enum ScreenTraceType
    {
        /// <summary>
        /// Trace to a file.
        /// </summary>
        File,

        /// <summary>
        /// Trace to the printer.
        /// </summary>
        Printer,

        /// <summary>
        /// Do not change the value.
        /// </summary>
        Nop,
    }

    /// <summary>
    /// The actions dialog.
    /// </summary>
    public partial class Actions : Form
    {
        /// <summary>
        /// Title name, for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Actions));

        /// <summary>
        /// Message name, for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Actions));

        /// <summary>
        /// The application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// The main screen, for callbacks.
        /// </summary>
        private readonly MainScreen mainScreen;

        /// <summary>
        /// The screen trace type.
        /// </summary>
        private readonly RadioEnum<ScreenTraceType> screenTraceType;

        /// <summary>
        /// The visible control codes document.
        /// </summary>
        private VisibleControls visibleControls;

        /// <summary>
        /// True if the actions dialog was ever activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// True if the UI trace check callback is running.
        /// </summary>
        private bool uiTraceCheckRunning;

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
            this.handle = this.Handle;
            this.screenTraceType = new RadioEnum<ScreenTraceType>(this.screenTraceTableLayoutPanel);

            // Initialize the tabs.
            this.FileTransferTabInit();
            this.StatusTabInit();

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
                this.promptPictureBox.RemoveFromParent();
                this.promptLabel.RemoveFromParent();
            }

            this.fileRadioButton.Enabled = app.Allowed(Restrictions.ExternalFiles);

            if (this.app.Restricted(Restrictions.ExternalFiles))
            {
                this.tracePictureBox.RemoveFromParent();
                this.traceCheckBox.RemoveFromParent();
                this.tracePr3287FlowLayoutPanel.RemoveFromParent(); // How do we prevent pr3287 command line tracing?
                this.uiTracePanel.RemoveFromParent();
            }

            if (this.app.Restricted(Restrictions.ChangeSettings))
            {
                // Ideally we would remove the row these controls are in, but the other rows are specific sizes
                // and this makes a mess.
                this.visibleControlFlowLayoutPanel.RemoveFromParent();
            }

            if (this.app.Allowed(Restrictions.ExternalFiles))
            {
                this.uiTraceCheckBox.Checked = Trace.Flags != Trace.Type.None;
                this.uiTraceCheckedListBox.Items.Add(Trace.Type.All, Trace.Flags == Trace.Type.All);
                this.uiTraceCheckedListBox.Items.Add(Trace.Type.None, Trace.Flags == Trace.Type.None);
                foreach (var t in Enum.GetValues(typeof(Trace.Type)).OfType<Trace.Type>().Where(t => t != Trace.Type.All && t != Trace.Type.None))
                {
                    this.uiTraceCheckedListBox.Items.Add(t, Trace.Flags.HasFlag(t));
                }

                // Enable the check event handler now.
                this.uiTraceCheckedListBox.ItemCheck += new ItemCheckEventHandler(this.UiTraceCheckedListBox_ItemCheck);
            }

            if (this.app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
                this.helpPictureBox2.RemoveFromParent();
                this.helpPictureBox3.RemoveFromParent();
                this.helpPictureBox4.RemoveFromParent();
            }

            if (this.app.Restricted(Restrictions.Printing))
            {
                this.printScreenPictureBox.RemoveFromParent();
                this.printScreenLabel.RemoveFromParent();
                this.printerRadioButton.Enabled = false;
            }

            // There might not be any screen tracing options left.
            if (!this.printerRadioButton.Enabled && !this.fileRadioButton.Enabled)
            {
                this.screenTracingPictureBox.RemoveFromParent();
                this.traceScreenCheckBox.RemoveFromParent();
                this.screenTraceTableLayoutPanel.RemoveFromParent();
            }

            if (this.screenImagesTableLayoutPanel.Controls.Count == 0)
            {
                this.screenImagesGroupBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);
            I18n.LocalizeGlobal(Title.ReEnableKeyboard, "Re-Enable Keyboard");
            I18n.LocalizeGlobal(Title.ModeChange, "Mode Change");
            I18n.LocalizeGlobal(Title.SaveLocalization, "Save Localization");
            I18n.LocalizeGlobal(Title.ScreenTrace, "Screen Tracing");

            // Localize the file transfer and about tabs.
            FileTransferLocalize();
            AboutLocalize();

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
        /// The Print Screen button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [Associated("printScreen")]
        public void PrintTextButton_Click(object sender, EventArgs e)
        {
            if (this.app.Allowed(Restrictions.Printing))
            {
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.PrintText, B3270.Value.Gdi, B3270.Value.Dialog),
                    Wx3270.BackEnd.Ignore());
            }
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
            if (!this.traceScreenCheckBox.Enabled)
            {
                // Still waiting for the back end to respond.
                return;
            }

            switch (tag)
            {
                case "Printer":
                    this.ChangeScreenTrace(true, ScreenTraceType.Printer);
                    break;
                case "File":
                    this.ChangeScreenTrace(true, ScreenTraceType.File);
                    break;
                case "Toggle":
                    this.ChangeScreenTrace(false, ScreenTraceType.Nop);
                    break;
            }
        }

        /// <summary>
        /// Recursively walks the children of a control.
        /// </summary>
        /// <param name="control">Control to walk.</param>
        /// <returns>All offspring.</returns>
        private static IEnumerable<Control> ChildControls(Control control)
        {
            var ret = new List<Control>();
            foreach (var child in control.Controls.OfType<Control>())
            {
                if (child.Controls.Count != 0)
                {
                    ret.AddRange(ChildControls(child));
                }
                else
                {
                    ret.Add(child);
                }
            }

            return ret;
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
                    this.traceScreenCheckBox.Enabled = true;
                    this.fileRadioButton.Enabled = !screenTrace && this.app.Allowed(Restrictions.ExternalFiles);
                    this.printerRadioButton.Enabled = !screenTrace;
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
        /// Change tracing mode.
        /// </summary>
        /// <param name="isOn">True if tracing should now be on.</param>
        /// <param name="fromCheckBox">True if the change came from the checkbox.</param>
        private void TraceChanged(bool isOn, bool fromCheckBox = false)
        {
            this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.Trace, B3270.ToggleArgument.Action(isOn)), ErrorBox.Ignore());
            if (isOn && !fromCheckBox && Trace.Flags == Trace.Type.None)
            {
                this.uiTraceCheckBox.Checked = true;
                this.ToggleUiTracing(true);
            }

            if (!isOn)
            {
                // Turn off UI tracing, too.
                this.uiTraceCheckBox.Checked = false;
                this.ToggleUiTracing(false);
            }
        }

        /// <summary>
        /// The trace check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [Associated("trace")]
        private void TraceCheckBox_Click(object sender, EventArgs e)
        {
            this.TraceChanged(this.traceCheckBox.Checked, fromCheckBox: true);
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
        [Associated("visibleControl")]
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
        [Associated("cancelActions")]
        private void CancelActionsButton_Click(object sender, EventArgs e)
        {
            this.CancelScripts();
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
        /// <param name="type">Screen trace type.</param>
        private void ChangeScreenTrace(bool isOn, ScreenTraceType type)
        {
            if (this.traceScreenCheckBox.Checked == isOn)
            {
                return;
            }

            if (isOn)
            {
                BackEndAction action;

                // Turn screen tracing on.
                if (type == ScreenTraceType.File)
                {
                    this.screenTraceFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    switch (this.screenTraceFileDialog.ShowDialog())
                    {
                        default:
                        case DialogResult.Cancel:
                            this.traceScreenCheckBox.Checked = false;
                            return;
                        case DialogResult.OK:
                            break;
                    }

                    action = new BackEndAction(B3270.Action.ScreenTrace, "on", "file", this.screenTraceFileDialog.FileName);
                }
                else
                {
                    action = new BackEndAction(B3270.Action.ScreenTrace, "on", "printer", "gdi", "dialog");
                }

                this.traceScreenCheckBox.Checked = true;
                this.traceScreenCheckBox.Enabled = false;
                this.screenTraceType.Value = type;
                this.printerRadioButton.Enabled = false;
                this.fileRadioButton.Enabled = false;
                this.BackEnd.RunAction(
                    action,
                    (cookie, success, result) =>
                    {
                        if (!success)
                        {
                            if (!string.IsNullOrEmpty(result))
                            {
                                ErrorBox.Show(result, I18n.Get(Title.ScreenTrace));
                            }

                            this.traceScreenCheckBox.Checked = false;
                            this.traceScreenCheckBox.Enabled = true;
                            this.printerRadioButton.Enabled = true;
                            this.fileRadioButton.Enabled = this.app.Allowed(Restrictions.ExternalFiles);
                        }
                    });
            }
            else
            {
                // Turn screen tracing off.
                this.traceScreenCheckBox.Checked = false;
                this.traceScreenCheckBox.Enabled = false;
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.ScreenTrace, "off"),
                    (cookie, success, result) => { });
            }
        }

        /// <summary>
        /// The trace screen check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [Associated("screenTracing")]
        private void TraceScreen_Click(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                // Invert the checkbox, then let ChangeScreenTrace set it back.
                checkBox.Checked = !checkBox.Checked;
                this.ChangeScreenTrace(!checkBox.Checked, this.screenTraceType.Value);
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
        [Associated("prompt")]
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
        [Associated("reenable")]
        private void ReenableButton_Click(object sender, EventArgs e)
        {
            this.ReenableKeyboard();
        }

        /// <summary>
        /// The pr3287 trace check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [Associated("tracePr3287")]
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
        [Associated("keymap")]
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
        /// Toggle UI tracing on or off.
        /// </summary>
        /// <param name="tracing">True if enabling UI tracing.</param>
        private void ToggleUiTracing(bool tracing)
        {
            this.uiTraceCheckedListBox.Enabled = tracing;

            // Set the trace flags.
            if (tracing && Trace.Flags == Trace.Type.None)
            {
                Trace.Flags = Trace.Type.All;
            }
            else if (!tracing && Trace.Flags != Trace.Type.None)
            {
                Trace.Flags = Trace.Type.None;
            }

            // Check or un-check all of the items.
            var items = this.uiTraceCheckedListBox.Items.OfType<object>().ToArray();
            foreach (var item in items)
            {
                if ((Trace.Type)item == Trace.Type.None)
                {
                    this.uiTraceCheckedListBox.SetItemChecked(this.uiTraceCheckedListBox.Items.IndexOf(item), !tracing);
                }
                else
                {
                    this.uiTraceCheckedListBox.SetItemChecked(this.uiTraceCheckedListBox.Items.IndexOf(item), tracing);
                }
            }

            this.uiTraceCheckedListBox.SelectedItem = tracing ? Trace.Type.All : Trace.Type.None;
        }

        /// <summary>
        /// The UI trace checkbox was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [Associated("uiTrace")]
        private void UiTraceCheckBox_Click(object sender, EventArgs e)
        {
            this.ToggleUiTracing(this.uiTraceCheckBox.Checked);
        }

        /// <summary>
        /// An item in the UI trace list box is about to be checked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameters.</param>
        private void UiTraceCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.uiTraceCheckRunning)
            {
                return;
            }

            this.uiTraceCheckRunning = true;
            try
            {
                var flag = (Trace.Type)this.uiTraceCheckedListBox.Items[e.Index];
                var value = e.NewValue;

                // Handle the All and None checkboxes.
                if ((flag == Trace.Type.All || flag == Trace.Type.None) && value == CheckState.Checked)
                {
                    // Turn everything on or off.
                    foreach (var t in Enum.GetValues(typeof(Trace.Type)).OfType<Trace.Type>().Where(t => t != Trace.Type.All && t != Trace.Type.None))
                    {
                        this.uiTraceCheckedListBox.SetItemChecked(this.uiTraceCheckedListBox.Items.IndexOf(t), flag == Trace.Type.All);
                    }

                    // Set the trace flags value.
                    Trace.Flags = (flag == Trace.Type.All) ? Trace.Type.All : Trace.Type.None;
                }

                // Set the new trace flags value.
                if (flag != Trace.Type.All && flag != Trace.Type.None)
                {
                    if (value == CheckState.Checked)
                    {
                        Trace.Flags |= flag;
                    }
                    else
                    {
                        Trace.Flags &= ~flag;
                    }
                }

                // Set All or None to match the new state.
                this.uiTraceCheckedListBox.SetItemChecked(this.uiTraceCheckedListBox.Items.IndexOf(Trace.Type.All), Trace.Flags == Trace.Type.All);
                this.uiTraceCheckedListBox.SetItemChecked(this.uiTraceCheckedListBox.Items.IndexOf(Trace.Type.None), Trace.Flags == Trace.Type.None);

                if (Trace.Flags != Trace.Type.None && !this.Tracing)
                {
                    // Start back end tracing so we can see the output.
                    this.TraceChanged(true, fromCheckBox: true);
                }
            }
            finally
            {
                this.uiTraceCheckRunning = false;
            }
        }

        /// <summary>
        /// An element associated with another element has been clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AssociatedClick(object sender, EventArgs e)
        {
            // Find the tag associated with this element. Then find other elements in the same container with the same tag.
            var element = (Control)sender;
            var tag = (string)element.Tag;
            if (tag == null)
            {
                return;
            }

            // Find the associated method.
            MethodInfo click = null;
            foreach (var methodInfo in typeof(Actions).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var attribute = methodInfo.GetCustomAttribute<Associated>();
                if (attribute != null && attribute.Tag == tag)
                {
                    click = methodInfo;
                    break;
                }
            }

            foreach (var control in ChildControls(element.Parent).Where(c => c != element && (string)c.Tag == tag && c.Enabled))
            {
                if (control is CheckBox checkBox)
                {
                    checkBox.Checked = !checkBox.Checked;
                }
                else if (control is RadioButton radioButton)
                {
                    if (!radioButton.Checked)
                    {
                        radioButton.Checked = true;
                    }
                }
            }

            click?.Invoke(this, new object[] { sender, e });
        }

        /// <summary>
        /// Associated event handler attrbutes.
        /// </summary>
        private class Associated : Attribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Associated"/> class.
            /// </summary>
            /// <param name="tag">Associated tag.</param>
            public Associated(string tag)
            {
                this.Tag = tag;
            }

            /// <summary>
            /// Gets or sets the tag.
            /// </summary>
            public string Tag { get; set; }
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

            /// <summary>
            /// Screen trace error.
            /// </summary>
            public static readonly string ScreenTrace = I18n.Combine(TitleName, "screenTrace");
        }
    }
}
