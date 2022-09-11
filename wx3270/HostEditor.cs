// <copyright file="HostEditor.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Host editing mode.
    /// </summary>
    public enum HostEditingMode
    {
        /// <summary>
        /// Save host (saved with profile).
        /// </summary>
        SaveHost,

        /// <summary>
        /// Quick host, with Connect as the accept button.
        /// </summary>
        QuickConnect,
    }

    /// <summary>
    /// Host editing result.
    /// </summary>
    [Flags]
    public enum HostEditingResult
    {
        /// <summary>
        /// Cancel editing.
        /// </summary>
        Cancel,

        /// <summary>
        /// Successful edit.
        /// </summary>
        Ok = 0x1,

        /// <summary>
        /// Save the result.
        /// </summary>
        Save = 0x2,

        /// <summary>
        /// Start login macro recording.
        /// </summary>
        Record = 0x4,

        /// <summary>
        /// Connect to host.
        /// </summary>
        Connect = 0x8,
    }

    /// <summary>
    /// The host editor.
    /// </summary>
    public partial class HostEditor : Form
    {
        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(HostEditor));

        /// <summary>
        /// Message group for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(HostEditor));

        /// <summary>
        /// Prefix-to-check-box sets.
        /// </summary>
        private readonly Dictionary<string, CheckPrefix> sets;

        /// <summary>
        /// The editing mode.
        /// </summary>
        private readonly HostEditingMode editingMode;

        /// <summary>
        /// Auto-connect radio buttons.
        /// </summary>
        private readonly RadioEnum<AutoConnect> autoConnect;

        /// <summary>
        /// Host type radio buttons.
        /// </summary>
        private readonly RadioEnum<HostType> hostType;

        /// <summary>
        /// Printer session type radio buttons.
        /// </summary>
        private readonly RadioEnum<PrinterSessionType> printerSessionType;

        /// <summary>
        /// Connection type radio buttons.
        /// </summary>
        private readonly RadioEnum<ConnectionType> connectionType;

        /// <summary>
        /// Input type radio buttons.
        /// </summary>
        private readonly RadioEnum<B3270.NoTelnetInputType> inputType;

        /// <summary>
        /// Current profile.
        /// </summary>
        private readonly Profile profile;

        /// <summary>
        /// The application context.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostEditor"/> class.
        /// </summary>
        /// <param name="editingMode">Editing mode.</param>
        /// <param name="hostEntry">Optional existing host entry.</param>
        /// <param name="profile">Profile to associate with new or modified entry.</param>
        /// <param name="app">Application context.</param>
        public HostEditor(HostEditingMode editingMode, HostEntry hostEntry, Profile profile, Wx3270App app)
        {
            this.InitializeComponent();

            this.editingMode = editingMode;
            this.profile = profile;
            this.autoConnect = new RadioEnum<AutoConnect>(this.loadTableLayoutPanel);
            this.hostType = new RadioEnum<HostType>(this.hostTypeTableLayoutPanel);
            this.printerSessionType = new RadioEnum<PrinterSessionType>(this.printerSessionTableLayoutPanel);
            this.connectionType = new RadioEnum<ConnectionType>(this.connectionTypeTableLayoutPanel);
            this.inputType = new RadioEnum<B3270.NoTelnetInputType>(this.localProcessInputTableLayoutPanel);
            this.app = app;

            // Map prefixes onto options.
            this.sets = new Dictionary<string, CheckPrefix>();
            foreach (var control in this.optionsLayoutPanel.Controls)
            {
                if (!(control is CheckBox checkBox) || !(checkBox.Tag is string tag))
                {
                    continue;
                }

                if (tag.StartsWith("~"))
                {
                    this.sets[tag.Substring(1, 1)] = new CheckPrefix(checkBox, false);
                }
                else
                {
                    this.sets[tag.Substring(0, 1)] = new CheckPrefix(checkBox, true);
                }
            }

            // Create checkboxes for unknown options.
            if (this.app != null)
            {
                // Determine the row and tab index of the last item.
                var row = 0;
                var tab = 0;
                foreach (var checkBox in this.optionsLayoutPanel.Controls.Cast<CheckBox>())
                {
                    tab = Math.Max(checkBox.TabIndex, tab);
                    if (this.optionsLayoutPanel.GetColumn(checkBox) == 1)
                    {
                        row = Math.Max(this.optionsLayoutPanel.GetRow(checkBox), row);
                    }
                }

                // Add unknown prefixes as options.
                foreach (var prefix in this.app.HostPrefix.Prefixes)
                {
                    if (!this.sets.ContainsKey(prefix.ToString()))
                    {
                        var checkBox = new CheckBox
                        {
                            AutoSize = true,
                            Location = new System.Drawing.Point(3, 3),
                            Name = "prefix" + prefix + "CheckBox",
                            Size = new System.Drawing.Size(112, 17),
                            TabIndex = ++tab,
                            Tag = prefix + ":",
                            Text = string.Empty,
                            UseVisualStyleBackColor = true,
                        };
                        this.optionsLayoutPanel.Controls.Add(checkBox, 1, ++row);
                        this.sets[prefix.ToString()] = new CheckPrefix(checkBox, true);
                    }
                }
            }

            if (hostEntry != null)
            {
                this.connectionType.Value = hostEntry.ConnectionType;
                this.NicknameTextBox.Text = hostEntry.Name;
                this.HostNameTextBox.Text = hostEntry.Host;
                this.PortTextBox.Text = hostEntry.Port;
                this.LuNamesTextBox.Text = hostEntry.LuNames;
                this.acceptTextBox.Text = hostEntry.AcceptHostName;
                this.clientCertTextBox.Text = hostEntry.ClientCertificateName;
                this.LoginMacroTextBox.Text = hostEntry.LoginMacro;
                this.descriptionTextBox.Text = hostEntry.Description;
                this.titleTextBox.Text = hostEntry.WindowTitle;
                this.commandTextBox.Text = hostEntry.Command;
                this.commandLineOptionsTextBox.Text = hostEntry.CommandLineOptions;

                if (!string.IsNullOrEmpty(hostEntry.Prefixes))
                {
                    foreach (var prefix in hostEntry.Prefixes.Select(p => p.ToString()))
                    {
                        if (this.sets.TryGetValue(prefix, out CheckPrefix set))
                        {
                            set.CheckBox.Checked = set.IsChecked;
                        }
                    }
                }

                this.starttlsCheckBox.Checked = hostEntry.AllowStartTls;
                this.autoConnect.Value = hostEntry.AutoConnect;
                this.hostType.Value = hostEntry.HostType;

                this.printerSessionType.Value = hostEntry.PrinterSessionType;
                this.specificLuTextBox.Text = hostEntry.PrinterSessionLu;
                this.specificLuTextBox.Enabled = hostEntry.PrinterSessionType == PrinterSessionType.SpecificLu;

                this.starttlsCheckBox.Enabled = this.telnetCheckBox.Checked;
                this.tn3270eCheckBox.Enabled = this.telnetCheckBox.Checked;

                this.inputType.Value = hostEntry.NoTelnetInputType;

                this.ConnectionTypeChanged(hostEntry.ConnectionType == ConnectionType.Host);
            }

            if (editingMode == HostEditingMode.QuickConnect)
            {
                this.ActiveControl = this.HostNameTextBox;
            }

            if (editingMode == HostEditingMode.SaveHost)
            {
                // Save hosts do not have a connect button.
                this.connectButton.Visible = false;
                this.connectRecordButton.Visible = false;
            }

            if (editingMode == HostEditingMode.QuickConnect)
            {
                this.AcceptButton = this.connectButton;
            }

            this.printerSessionType.Changed += (sender, args) =>
            {
                var e = (RadioEnum<PrinterSessionType>)sender;
                if (e.Value == PrinterSessionType.SpecificLu)
                {
                    this.specificLuTextBox.Enabled = true;
                    this.specificLuTextBox.Focus();
                }
                else
                {
                    this.specificLuTextBox.Enabled = false;
                    this.specificLuTextBox.Text = string.Empty;
                }
            };

            this.connectionType.Changed += (sender, args) =>
            {
                var e = (RadioEnum<ConnectionType>)sender;
                this.ConnectionTypeChanged(e.Value == ConnectionType.Host);
            };

            this.localProcessInputGroupBox.Enabled = this.connectionType.Value == ConnectionType.LocalProcess || !this.telnetCheckBox.Checked;

            if (this.app != null && this.app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);
            foreach (var checkPrefix in this.sets.Values)
            {
                if (checkPrefix.CheckBox.Text == string.Empty)
                {
                    checkPrefix.CheckBox.Text = (checkPrefix.CheckBox.Tag as string) + " " + I18n.Get(Message.HostPrefix);
                }
            }

            // Substitute.
            VersionSpecific.Substitute(this);

            // Set the profile name label, which we do not want localized.
            this.profileNameLabel.Text = hostEntry != null ? hostEntry.Profile.Name : profile.Name;
        }

        /// <summary>
        /// Gets the completed host entry.
        /// </summary>
        public HostEntry HostEntry
        {
            get
            {
                return new HostEntry
                {
                    Profile = this.profile,
                    Name = this.NicknameTextBox.Text,
                    ConnectionType = this.connectionType.Value,
                    Host = this.HostNameTextBox.Text,
                    Port = this.PortTextBox.Text,
                    LuNames = this.LuNamesTextBox.Text,
                    AcceptHostName = this.acceptTextBox.Text,
                    ClientCertificateName = this.clientCertTextBox.Text,
                    LoginMacro = this.LoginMacroTextBox.Text,
                    Description = this.descriptionTextBox.Text,
                    WindowTitle = this.titleTextBox.Text,
                    AllowStartTls = this.starttlsCheckBox.Checked,
                    AutoConnect = this.autoConnect.Value,
                    HostType = this.hostType.Value,
                    PrinterSessionType = this.printerSessionType.Value,
                    PrinterSessionLu = this.specificLuTextBox.Text,
                    Prefixes = string.Join(
                        string.Empty,
                        this.sets
                            .Where(kv => kv.Value.CheckBox.Checked == kv.Value.IsChecked)
                            .Select(kv => kv.Key)),
                    Command = this.commandTextBox.Text,
                    CommandLineOptions = this.commandLineOptionsTextBox.Text,
                    NoTelnetInputType = this.inputType.Value,
                };
            }
        }

        /// <summary>
        /// Gets the editing result.
        /// </summary>
        public HostEditingResult Result { get; private set; } = HostEditingResult.Cancel;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.HostName, "Host Name");
            I18n.LocalizeGlobal(Title.Port, "Port");
            I18n.LocalizeGlobal(Title.LuNames, "LU Names");
            I18n.LocalizeGlobal(Title.ConnectionEditor, "Connection Editor");
            I18n.LocalizeGlobal(Title.AcceptHostName, "Accept Host Name");
            I18n.LocalizeGlobal(Title.LuName, "LU Name");
            I18n.LocalizeGlobal(Title.LoginMacro, "login macro");

            I18n.LocalizeGlobal(Message.HostNameCharacter, "Host name cannot contain a space or any of these characters");
            I18n.LocalizeGlobal(Message.InvalidIpv6, "Invalid IPv6 address");
            I18n.LocalizeGlobal(Message.PortCharacter, "Port can only contain alphanumeric, dash or underscore characters");
            I18n.LocalizeGlobal(Message.LuNameCharacter, "LU names can only contain alphanumeric, dash or underscore characters");
            I18n.LocalizeGlobal(Message.MustSpecifyHost, "Must specify a host name");
            I18n.LocalizeGlobal(Message.MustSpecifyCommand, "Must specify a command");
            I18n.LocalizeGlobal(Message.AcceptHostCharacter, "Accept host name cannot contain a space or any of these characters");
            I18n.LocalizeGlobal(Message.MustSpecifyLu, "Must specify an LU name");
            I18n.LocalizeGlobal(Message.TranslatedPrefix, "Translating host prefix to option");
            I18n.LocalizeGlobal(Message.HostPrefix, "host prefix");
        }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void FormLocalize()
        {
            new HostEditor(HostEditingMode.SaveHost, null, Profile.DefaultProfile, null).Dispose();
        }

        /// <summary>
        /// Adjust the form to a changed connection type.
        /// </summary>
        /// <param name="isHost">True if this is a host connection.</param>
        private void ConnectionTypeChanged(bool isHost)
        {
            this.hostNameLabel.Enabled = isHost;
            this.HostNameTextBox.Enabled = isHost;
            this.portLabel.Enabled = isHost;
            this.PortTextBox.Enabled = isHost;
            this.luNamesLabel.Enabled = isHost;
            this.LuNamesTextBox.Enabled = isHost;
            this.acceptLabel.Enabled = isHost;
            this.acceptTextBox.Enabled = isHost;
            this.clientCertificateLabel.Enabled = isHost;
            this.clientCertTextBox.Enabled = isHost;
            this.optionsGroupBox.Enabled = isHost;
            this.hostTypeGroupBox.Enabled = isHost;
            this.printerSessionGroupBox.Enabled = isHost;

            this.commandLabel.Enabled = !isHost;
            this.commandTextBox.Enabled = !isHost;
            this.commandLineOptionsLabel.Enabled = !isHost;
            this.commandLineOptionsTextBox.Enabled = !isHost;
            this.localProcessInputGroupBox.Enabled = !isHost || !this.telnetCheckBox.Checked;

            if (isHost)
            {
                this.commandTextBox.Text = string.Empty;
                this.commandLineOptionsTextBox.Text = string.Empty;
            }
            else
            {
                this.HostNameTextBox.Text = string.Empty;
                this.PortTextBox.Text = string.Empty;
                this.LuNamesTextBox.Text = string.Empty;
                this.acceptTextBox.Text = string.Empty;
                this.clientCertTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Validation for the host name text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HostNameTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(sender is TextBox textBox))
            {
                return;
            }

            // Check for invalid characters.
            string error = null;
            var badChars = " @,=[]";
            var text = textBox.Text.Trim(new[] { ' ' });
            textBox.Text = text;
            if (text.Any(c => badChars.Contains(c)))
            {
                error = I18n.Get(Message.HostNameCharacter) + ":" + badChars;
            }

            // Check for (most) mistaken prefixes and (some) mistaken ports.
            if (error == null && text.Contains(":") && !IPAddress.TryParse(text, out _))
            {
                error = I18n.Get(Message.InvalidIpv6);
            }

            if (error != null)
            {
                if (this.ActiveControl.Equals(this.CancelButton))
                {
                    // Allow the form to be closed, but get rid of the bad text so it does not haunt us later.
                    textBox.Text = string.Empty;
                }
                else
                {
                    // Complain.
                    ErrorBox.Show(error, I18n.Get(Title.HostName));
                    e.Cancel = true;
                }

                return;
            }
        }

        /// <summary>
        /// Validation for the port text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PortTextBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = (TextBox)sender;

            // Remove empty lines and spaces at the beginning and end of each line.
            var text = textBox.Text.Trim(new[] { ' ' });
            textBox.Text = text;

            if (text.Any(c => !Utils.IsAlphaOrDash(c)) && !this.ActiveControl.Equals(this.CancelButton))
            {
                // Complain.
                ErrorBox.Show(I18n.Get(Message.PortCharacter), I18n.Get(Title.Port));
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Validation for the LU names text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LuNamesTextBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = (TextBox)sender;

            // Translate commas and spaces to newlines, then remove empty lines and spaces at the beginning and end of each line.
            var entries = textBox.Text
                .Replace(" ", Environment.NewLine)
                .Replace(",", Environment.NewLine)
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim(new[] { ' ' }));

            if (entries.Any(entry => entry.Any(c => !Utils.IsAlphaOrDash(c))))
            {
                if (!this.ActiveControl.Equals(this.CancelButton))
                {
                    ErrorBox.Show(I18n.Get(Message.LuNameCharacter), I18n.Get(Title.LuNames));
                    e.Cancel = true;
                }

                return;
            }

            textBox.Text = string.Join(Environment.NewLine, entries);
        }

        /// <summary>
        /// The macro edit button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LoginMacroEditButton_Click(object sender, EventArgs e)
        {
            var title = I18n.Get(Title.LoginMacro);
            if (!string.IsNullOrEmpty(this.NicknameTextBox.Text))
            {
                title = this.NicknameTextBox.Text + " " + title;
            }
            else if (!string.IsNullOrEmpty(this.HostNameTextBox.Text))
            {
                title = this.HostNameTextBox.Text + " " + title;
            }

            using var editor = new MacroEditor(this.HostEntry.LoginMacro, title, false, this.app);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.LoginMacroTextBox.Text = editor.MacroText;
            }
            else if (result == DialogResult.Retry)
            {
                this.StartRecordingLoginMacro(title);
            }
        }

        /// <summary>
        /// Start recording a login macro.
        /// </summary>
        /// <param name="title">Macro title.</param>
        private void StartRecordingLoginMacro(string title)
        {
            // Don't save it yet -- we're just recording.
            this.Result = HostEditingResult.Ok | HostEditingResult.Record;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// A key was pressed in the login macro text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LoginMacroEditButton_Click(object sender, KeyPressEventArgs e)
        {
            this.LoginMacroEditButton_Click(sender, (EventArgs)e);
        }

        /// <summary>
        /// The cancel button was pressed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Result = HostEditingResult.Cancel;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Validate that the entry can be completed.
        /// </summary>
        /// <param name="result">Dialog result to set, if successful.</param>
        /// <param name="editingResult">Editing result to return, if successful.</param>
        private void ValidateDone(DialogResult result, HostEditingResult editingResult)
        {
            if (this.connectionType.Value == ConnectionType.Host)
            {
                if (string.IsNullOrEmpty(this.HostNameTextBox.Text))
                {
                    ErrorBox.Show(I18n.Get(Message.MustSpecifyHost), I18n.Get(Title.ConnectionEditor));
                }

                // XXX: There should be a way to make this unique, instead of failing the edit/add later.
                if (string.IsNullOrEmpty(this.NicknameTextBox.Text))
                {
                    this.NicknameTextBox.Text = this.HostNameTextBox.Text;
                }
            }

            if (this.connectionType.Value == ConnectionType.LocalProcess)
            {
                if (string.IsNullOrEmpty(this.commandTextBox.Text))
                {
                    ErrorBox.Show(I18n.Get(Message.MustSpecifyCommand), I18n.Get(Title.ConnectionEditor));
                }

                if (string.IsNullOrEmpty(this.NicknameTextBox.Text))
                {
                    this.NicknameTextBox.Text = Path.GetFileNameWithoutExtension(this.commandTextBox.Text);
                }
            }

            this.Result = editingResult;
            this.DialogResult = result;
            this.Close();
        }

        /// <summary>
        /// The OK button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            this.ValidateDone(DialogResult.OK, HostEditingResult.Ok | HostEditingResult.Save);
        }

        /// <summary>
        /// The connect button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            this.ValidateDone(DialogResult.OK, HostEditingResult.Ok | HostEditingResult.Save | HostEditingResult.Connect);
        }

        /// <summary>
        /// Validation event for the session name text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NicknameTextBox_Validating(object sender, CancelEventArgs e)
        {
            // Strip white space.
            this.NicknameTextBox.Text = this.NicknameTextBox.Text.Trim(' ');
        }

        /// <summary>
        /// Checked change event handler for the TLS checkbox.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TlsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Checked change event handler for the verify cert checkbox.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void VerifyCertCheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Validation for the accept host name text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AcceptTextBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            string error = null;
            var badChars = " @,:[]=";
            var text = textBox.Text.Trim(new[] { ' ' });
            textBox.Text = text;
            if (text.Any(c => badChars.Contains(c)))
            {
                error = I18n.Get(Message.AcceptHostCharacter) + ":" + badChars;
            }

            if (error != null)
            {
                if (this.ActiveControl.Equals(this.CancelButton))
                {
                    // Allow the form to be closed, but get rid of the bad text so it does not haunt us later.
                    // Note that we delete the illegal characters before removing prefixes, in case removing them
                    // causes a prefix to be formed.
                    text = new string(text.Where(c => !badChars.Contains(c)).ToArray());
                    textBox.Text = text;
                }
                else
                {
                    // Complain.
                    ErrorBox.Show(error, I18n.Get(Title.AcceptHostName));
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// The Help picture box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Click(object sender, EventArgs e)
        {
            Wx3270App.GetHelp("ConnectionEditor");
        }

        /// <summary>
        /// Validate the printer session specific-LU text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SpecificLuTextBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text.Any(c => !Utils.IsAlphaOrDash(c)))
            {
                if (!this.ActiveControl.Equals(this.CancelButton))
                {
                    ErrorBox.Show(I18n.Get(Message.LuNameCharacter), I18n.Get(Title.LuName));
                    e.Cancel = true;
                }

                return;
            }
        }

        /// <summary>
        /// Validate the printer session group box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterSessionGroupBox_Validating(object sender, CancelEventArgs e)
        {
            if (this.printerSessionType.Value == PrinterSessionType.SpecificLu && string.IsNullOrWhiteSpace(this.specificLuTextBox.Text))
            {
                if (!this.ActiveControl.Equals(this.CancelButton))
                {
                    ErrorBox.Show(I18n.Get(Message.MustSpecifyLu), I18n.Get(Title.LuName));
                    e.Cancel = true;
                }

                return;
            }
        }

        /// <summary>
        /// The TELNET check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TelnetCheckBox_Click(object sender, EventArgs e)
        {
            var isChecked = this.telnetCheckBox.Checked;
            this.starttlsCheckBox.Checked = isChecked;
            this.starttlsCheckBox.Enabled = isChecked;
            this.tn3270eCheckBox.Checked = isChecked;
            this.tn3270eCheckBox.Enabled = isChecked;
            this.localProcessInputGroupBox.Enabled = !isChecked || this.connectionType.Value == ConnectionType.LocalProcess;
        }

        /// <summary>
        /// The command text box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CommandTextBox_Click(object sender, EventArgs e)
        {
            var initialDir = string.Empty;
            if (!string.IsNullOrEmpty(this.commandTextBox.Text))
            {
                initialDir = Path.GetDirectoryName(this.commandTextBox.Text);
            }

            if (string.IsNullOrEmpty(initialDir))
            {
               initialDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }

            this.commandOpenFileDialog.InitialDirectory = initialDir;
            this.commandOpenFileDialog.FileName = this.commandTextBox.Text;
            var ret = this.commandOpenFileDialog.ShowDialog(this);
            if (ret == DialogResult.Cancel)
            {
                return;
            }

            this.commandTextBox.Text = this.commandOpenFileDialog.FileName;
        }

        /// <summary>
        /// A key was pressed in the command text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CommandTextBox_Click(object sender, KeyPressEventArgs e)
        {
            this.CommandTextBox_Click(sender, (EventArgs)e);
        }

        /// <summary>
        /// The Connect+Record button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectRecordButton_Click(object sender, EventArgs e)
        {
            this.ValidateDone(DialogResult.OK, HostEditingResult.Ok | HostEditingResult.Save | HostEditingResult.Connect | HostEditingResult.Record);
        }

        /// <summary>
        /// A check prefix.
        /// </summary>
        private class CheckPrefix : Tuple<CheckBox, bool>
        {
            public CheckPrefix(CheckBox checkBox, bool isChecked)
                : base(checkBox, isChecked)
            {
            }

            /// <summary>
            /// Gets the check box.
            /// </summary>
            public CheckBox CheckBox => this.Item1;

            /// <summary>
            /// Gets a value indicating whether the item should be checked when the prefix is selected.
            /// </summary>
            public bool IsChecked => this.Item2;
        }

        /// <summary>
        /// Localized message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Host name error.
            /// </summary>
            public static readonly string HostName = I18n.Combine(TitleName, "hostName");

            /// <summary>
            /// Port error.
            /// </summary>
            public static readonly string Port = I18n.Combine(TitleName, "port");

            /// <summary>
            /// LU names error.
            /// </summary>
            public static readonly string LuNames = I18n.Combine(TitleName, "luNames");

            /// <summary>
            /// Save/connect error.
            /// </summary>
            public static readonly string ConnectionEditor = I18n.Combine(TitleName, "connectionEditor");

            /// <summary>
            /// Accept host name.
            /// </summary>
            public static readonly string AcceptHostName = I18n.Combine(TitleName, "acceptHostName");

            /// <summary>
            /// (Single) LU name.
            /// </summary>
            public static readonly string LuName = I18n.Combine(TitleName, "luName");

            /// <summary>
            /// Login macro editor.
            /// </summary>
            public static readonly string LoginMacro = I18n.Combine(TitleName, "loginMacro");
        }

        /// <summary>
        /// Localized message box messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Bad host name character.
            /// </summary>
            public static readonly string HostNameCharacter = I18n.Combine(MessageName, "hostNameCharacter");

            /// <summary>
            /// Bad host name prefix.
            /// </summary>
            public static readonly string InvalidIpv6 = I18n.Combine(MessageName, "invalidIpv6");

            /// <summary>
            /// Bad port character.
            /// </summary>
            public static readonly string PortCharacter = I18n.Combine(MessageName, "portCharacter");

            /// <summary>
            /// Bad LU name character.
            /// </summary>
            public static readonly string LuNameCharacter = I18n.Combine(MessageName, "luNameCharacter");

            /// <summary>
            /// Must specify a host name.
            /// </summary>
            public static readonly string MustSpecifyHost = I18n.Combine(MessageName, "mustSpecifyHost");

            /// <summary>
            /// Must specify a command.
            /// </summary>
            public static readonly string MustSpecifyCommand = I18n.Combine(MessageName, "mustSpecifyCommand");

            /// <summary>
            /// Bad accept host character.
            /// </summary>
            public static readonly string AcceptHostCharacter = I18n.Combine(MessageName, "acceptHostCharacter");

            /// <summary>
            /// Must specify LU.
            /// </summary>
            public static readonly string MustSpecifyLu = I18n.Combine(MessageName, "mustSpecifyLu");

            /// <summary>
            /// Prefix was translated to option.
            /// </summary>
            public static readonly string TranslatedPrefix = I18n.Combine(MessageName, "translatedPrefix");

            /// <summary>
            /// Host prefix label.
            /// </summary>
            public static readonly string HostPrefix = I18n.Combine(MessageName, "hostPrefix");
        }
    }
}
