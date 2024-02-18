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
    /// Create a new profile, or add to an existing one.
    /// </summary>
    public enum CreateAdd
    {
        /// <summary>
        /// Create a new one.
        /// </summary>
        CreateNewProfile,

        /// <summary>
        /// Add to existing.
        /// </summary>
        AddToExistingProfile,
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
        /// Create/add radio buttons.
        /// </summary>
        private readonly RadioEnum<CreateAdd> createAdd;

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
        /// True if this window has been activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostEditor"/> class.
        /// </summary>
        /// <param name="editingMode">Editing mode.</param>
        /// <param name="hostEntry">Optional existing host entry.</param>
        /// <param name="profile">Profile to associate with new or modified entry.</param>
        /// <param name="app">Application context.</param>
        /// <param name="spec">Profile creation spec.</param>
        public HostEditor(HostEditingMode editingMode, HostEntry hostEntry, Profile profile, Wx3270App app, ProfileSpec spec = null)
        {
            this.InitializeComponent();

            this.editingMode = editingMode;
            this.profile = profile;
            this.createAdd = new RadioEnum<CreateAdd>(this.profileTableLayoutPanel);
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
                this.connectionNameTextBox.Text = hostEntry.Name;
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

            // Populate profile-related fields.
            if (hostEntry == null || spec != null)
            {
                // This is an add of a new connection, or a continuation of an add after macro recording.
                // Set up the info about the profile we're adding to.
                this.addToProfileNameLabel.Text = profile.Name;
                this.addToProfileModelLabel.Text = profile.Model.ToString();
                if (this.app != null)
                {
                    this.addToProfileRowsLabel.Text = this.app.ModelsDb.Models[profile.Model].Rows.ToString();
                    this.addToProfileColumnsLabel.Text = this.app.ModelsDb.Models[profile.Model].Columns.ToString();
                }

                // Set up the fields for creating a new profile.
                this.newProfileNameTextBox.Width = this.connectionNameTextBox.Width;
                if (this.app != null)
                {
                    var modelNames = this.app.ModelsDb.Models.Keys.Cast<object>().ToArray();
                    this.modelComboBox.Items.AddRange(modelNames);
                    if (spec != null)
                    {
                        this.NewProfileSpec = spec;
                    }
                }

                this.createAdd.Changed += this.CreateAddChanged;
                this.createAdd.Value = spec != null ? CreateAdd.CreateNewProfile : CreateAdd.AddToExistingProfile;

                // No need for redundant display of the profile name.
                this.profileLabel.Visible = false;
                this.profileNameLabel.Visible = false;
            }
            else
            {
                // This is an edit, get rid of the add-related fields.
                this.profileGroupBox.RemoveFromParent();

                // We still don't want Save validating the profile name.
                this.createAdd.Value = CreateAdd.AddToExistingProfile;
            }
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
                    Name = this.connectionNameTextBox.Text,
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
        /// Gets the new profile to create, if the user selected that.
        /// </summary>
        public ProfileSpec NewProfileSpec
        {
            get
            {
                if (this.createAdd.Value == CreateAdd.AddToExistingProfile)
                {
                    return null;
                }

                var db = this.app.ModelsDb.Models[(int)this.modelComboBox.SelectedItem];
                var oversize = (int)this.rowsUpDown.Value != db.Rows || (int)this.columnsUpDown.Value != db.Columns;
                return new ProfileSpec(
                    this.newProfileNameTextBox.Text,
                    (int)this.modelComboBox.SelectedItem,
                    oversize ? (int)this.rowsUpDown.Value : 0,
                    oversize ? (int)this.columnsUpDown.Value : 0);
            }

            private set
            {
                this.newProfileNameTextBox.Text = value.ProfileName;
                this.modelComboBox.SelectedItem = value.Model;
                if (value.Rows != 0 || value.Columns != 0)
                {
                    this.rowsUpDown.Value = value.Rows;
                    this.columnsUpDown.Value = value.Columns;
                }
            }
        }

        /// <summary>
        /// Gets the macro editor state.
        /// </summary>
        public MacroEditor.EditorState MacroEditorState { get; private set; }

        /// <summary>
        /// Gets or sets the recorded text to insert into the login macro.
        /// </summary>
        public (string insertText, MacroEditor.EditorState)? LoginMacroInsert { get; set; }

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
            I18n.LocalizeGlobal(Message.InvalidProfileName, "Invalid profile name");
            I18n.LocalizeGlobal(Message.ProfileAlreadyExists, "Profile already exists");
            I18n.LocalizeGlobal(Message.PortCharacter, "Port can only contain alphanumeric, dash or underscore characters");
            I18n.LocalizeGlobal(Message.LuNameCharacter, "LU names can only contain alphanumeric, dash or underscore characters");
            I18n.LocalizeGlobal(Message.MustSpecifyProfile, "Must specify a profile name");
            I18n.LocalizeGlobal(Message.MustSpecifyHost, "Must specify a host name");
            I18n.LocalizeGlobal(Message.MustSpecifyCommand, "Must specify a command");
            I18n.LocalizeGlobal(Message.AcceptHostCharacter, "Accept host name cannot contain a space or any of these characters");
            I18n.LocalizeGlobal(Message.MustSpecifyLu, "Must specify an LU name");
            I18n.LocalizeGlobal(Message.TranslatedPrefix, "Translating host prefix to option");
            I18n.LocalizeGlobal(Message.HostPrefix, "host prefix");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global step 1.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), 1), "Tour: Connection Editor (new connection mode)");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), 1),
@"Use the Connection Editor to define or change a connection to a host.

A connection has basic parameters like the host name/address and TCP port.

If there is a setting you want to change but you do not see here, such as colors or keyboard mappings, those are per-profile parameters and are changed from the Settings window.");

            // Global step 1, editing mode.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), 10), "Tour: Connection Editor (editing mode)");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), 10),
@"Use the Connection Editor to change a connection to a host.

A connection has basic parameters like the host name/address and TCP port.

If there is a setting you want to change but you do not see here, such as colors or keyboard mappings, those are per-profile parameters and are changed from the Settings window.");

            // Host name.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(HostNameTextBox)), "Basics, Step 1 of 4: Host name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(HostNameTextBox)),
@"Enter the host's name or numeric address here.");

            // Port.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(PortTextBox)), "Basics, Step 2 of 4: TCP port");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(PortTextBox)),
@"Enter the TCP port to use to connect to the host here.");

            // TLS tunnel.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(tlsTunnelCheckBox)), "Basics, Step 3 of 4: TLS tunnel");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(tlsTunnelCheckBox)),
@"Check this box if you need wx3270 to create a TLS tunnel to the host.");

            // Connect.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(connectButton)), "Basics, Step 4 of 4: Connect button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(connectButton)),
@"Click to connect to the host.");

            // Profile selection.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), 2), "More options: New profile");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), 2),
@"By default, the Connection Editor will add this connection to the current profile (or to the profile currently selected in the Profiles and Connections window). You can also choose to create a new profile and add this connection to it.

A common reason for doing this is to have different screen dimensions.

The new profile will be a copy of the default profile, containing only this connection.

Do this before clicking on the Connect button.");

            // New profile.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(newProfileRadioButton)), "New profile, Step 1 of 4");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(newProfileRadioButton)),
@"Select Create new profile.");

            // New profile name.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(newProfileNameTextBox)), "New profile, Step 2 of 4: Profile name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(newProfileNameTextBox)),
@"Enter the name of the new profile.");

            // New profile model.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(modelComboBox)), "New profile, Step 3 of 4: Model number");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(modelComboBox)),
@"Select the 3270 model number, which gives the minimum rows and columns.");

            // New profile oversize.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(rowsUpDown)), "New profile, Step 4 of 4: Oversize rows and columns");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(rowsUpDown)),
@"Optionally, you can choose larger dimensions than the minimum for the model here.");

            // Connection to something other than a host.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), 3), "Additional options");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), 3),
@"Here are some miscellaneous options that can be set.");

            // Connection name.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(connectionNameTextBox)), "Additional options: Connection name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(connectionNameTextBox)),
@"You can give this connection a name with this field. Otherwise the name will be constructed from the hostname and port.");

            // LU names.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(LuNamesTextBox)), "Additional options: LU names");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(LuNamesTextBox)),
@"If you need to connect to a specific Logical Unit (LU), enter its name or a list of LU names here.");

            // Login macro.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(LoginMacroTextBox)), "Additional options: Login macro");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(LoginMacroTextBox)),
@"A login macro is a series of actions that will be performed as soon as the connection is fully established.

Click here to create or edit the login macro with the Macro Editor.");

            // Description.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(descriptionTextBox)), "Additional options: Connection description");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(descriptionTextBox)),
@"Enter the connection descrition here, which will be displayed when the mouse is hovered over this connection in the Profiles and Connections window.");

            // Window title.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(titleTextBox)), "Additional options: Window title");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(titleTextBox)),
@"If you want a specific window title for this connection, enter it here.

This will override the wx3270 default of profile name and connection name, and will also override any per-profile window title defined in the Settings window.");

            // Load options.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(loadGroupBox)), "Additional options: Automatic connection");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(loadGroupBox)),
@"Use these radio buttons to choose whether to automatically make this connection when the profile that contains it is loaded.

The connection can be automatically made, and it can also be automatically reconnected if it disconnects.

Note that only one connection per profile can be designated this way.");

            // Host type for file transfers.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(hostTypeGroupBox)), "Additional options: Host type");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(hostTypeGroupBox)),
@"Use these radio buttons to define what kind of host this is. This setting is optional.

It is used to set up the options for IND$FILE file transfers.");

            // Printer session.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(printerSessionGroupBox)), "Additional options: Printer session");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(printerSessionGroupBox)),
@"wx3270 can also emulate an attached printer. Use these fields to set up an attached printer.");

            // TLS parameters.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), 4), "TLS options");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), 4),
@"Here are some additional options that are specific to Transport Layer Security (TLS).");

            // Accept hostname.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(acceptTextBox)), "TLS options: Accept hostname");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(acceptTextBox)),
@"If you are connecting to a host using TLS, and the name in the host's certificate is something other than the hostname/address you specified to connect to it, you can enter that name here.

This will allow TLS to successfully validate the host's certificate.");

            // Client certificate name.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(clientCertTextBox)), "TLS options: Client certificate name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(clientCertTextBox)),
@"If you are connecting to a host using TLS, and the host requires that you provide a certificate, enter the name of the certificate here.

wx3270 will look for the certifcate in the current user's Personal store.");

            // Verify cert checkbox.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(verifyCertCheckBox)), "TLS options: Verify host certificate");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(verifyCertCheckBox)),
@"If you are connecting to a host using TLS, and the connection keeps failing because host certificate validation fails, you can work around this issue by un-checking this option, which will turn off host certificate validation.");

            // Connection to something other than a host.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), 5), "Connect to a local process");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), 5),
@"wx3270 can also connect to a local process, rather than to a host. Examples of local processes would be cmd.exe or Powershell.");

            // Local process selection.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(localProcessRadioButton)), "Local process, Step 1 of 3");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(localProcessRadioButton)),
@"Select Local process here.");

            // Command.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(commandTextBox)), "Local process, Step 2 of 3: Command");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(commandTextBox)),
@"Click here to specify the program to execute.");

            // Command-line options.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(commandLineOptionsTextBox)), "Local process, Step 3 of 3: Command-line options");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(commandLineOptionsTextBox)),
@"Use this field to enter any command-line options to provide to the program.");

            // Connect+Record button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(connectRecordButton)), "Connect+Record button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(connectRecordButton)),
@"Click to start the connection and begin recording a login macro with the macro recorder.");

            // Cancel button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(cancelButton)), "Cancel button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(cancelButton)),
@"Click to abandon your changes.");

            // Save button, new-session variant.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(okButton)), "Save button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(okButton)),
@"Click to save the connection definition without starting the connection.");

            // Save button, edit variant.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(okButton), 1), "Basics, 4 of 4: Save button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(okButton), 1),
@"Click to save the connection definition.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(HostEditor), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(HostEditor), nameof(helpPictureBox)),
@"Click to display context-dependent help from the wx3270 Wiki in your browser, or to restart this tour.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
        /// Adjusts the form to a changed create/add type.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CreateAddChanged(object sender, EventArgs e)
        {
            var creating = this.createAdd.Value == CreateAdd.CreateNewProfile;
            this.newProfileNameTextBox.Enabled = creating;
            this.modelComboBox.Enabled = creating;
            this.rowsUpDown.Enabled = creating;
            this.columnsUpDown.Enabled = creating;
            this.addToProfileNameLabel.Enabled = !creating;
            this.addToProfileModelLabel.Enabled = !creating;
            this.addToProfileRowsLabel.Enabled = !creating;
            this.addToProfileColumnsLabel.Enabled = !creating;

            if (!creating)
            {
                // Revert to defaults.
                this.newProfileNameTextBox.Text = string.Empty;
                if (this.app != null)
                {
                    var sourceProfile = this.app.ProfileManager.CopyDefaultProfile();
                    this.modelComboBox.SelectedItem = sourceProfile.Model;
                    this.RowsColsDefaults(sourceProfile.Model);
                }
            }
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
            this.acceptLabel.Enabled = isHost && (this.tlsTunnelCheckBox.Checked || this.starttlsCheckBox.Checked);
            this.acceptTextBox.Enabled = isHost && (this.tlsTunnelCheckBox.Checked || this.starttlsCheckBox.Checked);
            this.clientCertificateLabel.Enabled = isHost && (this.tlsTunnelCheckBox.Checked || this.starttlsCheckBox.Checked);
            this.clientCertTextBox.Enabled = isHost && (this.tlsTunnelCheckBox.Checked || this.starttlsCheckBox.Checked);
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
            this.StartMacroEditor();
        }

        /// <summary>
        /// Starts the macro editor for the login macro.
        /// </summary>
        /// <param name="ins">Login macro insert spec.</param>
        /// <returns>True if window was closed.</returns>
        private bool StartMacroEditor((string insertText, MacroEditor.EditorState state)? ins = null)
        {
            var title = I18n.Get(Title.LoginMacro);
            if (!string.IsNullOrEmpty(this.connectionNameTextBox.Text))
            {
                title = this.connectionNameTextBox.Text + " " + title;
            }
            else if (!string.IsNullOrEmpty(this.HostNameTextBox.Text))
            {
                title = this.HostNameTextBox.Text + " " + title;
            }

            using var editor = new MacroEditor(
                ins?.insertText ?? this.HostEntry.LoginMacro,
                title,
                false,
                this.app,
                ins?.state);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.LoginMacroTextBox.Text = editor.MacroText;
            }
            else if (result == DialogResult.Retry)
            {
                this.StartRecordingLoginMacro(editor.State);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Start recording a login macro.
        /// </summary>
        /// <param name="editorState">Macro editor state.</param>
        private void StartRecordingLoginMacro(MacroEditor.EditorState editorState)
        {
            // Don't save it yet -- we're just recording.
            this.Result = HostEditingResult.Ok | HostEditingResult.Record;
            this.MacroEditorState = editorState;
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
            if (this.createAdd.Value == CreateAdd.CreateNewProfile)
            {
                if (string.IsNullOrEmpty(this.newProfileNameTextBox.Text))
                {
                    ErrorBox.Show(I18n.Get(Message.MustSpecifyProfile), I18n.Get(Title.ConnectionEditor));
                    this.newProfileNameTextBox.Focus();
                    return;
                }
            }

            if (this.connectionType.Value == ConnectionType.Host)
            {
                if (string.IsNullOrEmpty(this.HostNameTextBox.Text))
                {
                    ErrorBox.Show(I18n.Get(Message.MustSpecifyHost), I18n.Get(Title.ConnectionEditor));
                    this.HostNameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(this.connectionNameTextBox.Text))
                {
                    this.connectionNameTextBox.Text = this.HostNameTextBox.Text + " " + this.PortTextBox.Text;
                }
            }

            if (this.connectionType.Value == ConnectionType.LocalProcess)
            {
                if (string.IsNullOrEmpty(this.commandTextBox.Text))
                {
                    ErrorBox.Show(I18n.Get(Message.MustSpecifyCommand), I18n.Get(Title.ConnectionEditor));
                    this.commandTextBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(this.connectionNameTextBox.Text))
                {
                    this.connectionNameTextBox.Text = Path.GetFileNameWithoutExtension(this.commandTextBox.Text);
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
            this.connectionNameTextBox.Text = this.connectionNameTextBox.Text.Trim(' ');
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
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
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
        /// The index of the model number combo box changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ModelIndexChanged(object sender, EventArgs e)
        {
            // The selected model changed. Take the up/downs back to defaults.
            var model = (int)this.modelComboBox.SelectedItem;
            this.RowsColsDefaults(model);
        }

        /// <summary>
        /// Set the rows and columns selectors to default valued.
        /// </summary>
        /// <param name="model">Model number.</param>
        private void RowsColsDefaults(int model)
        {
            var db = this.app.ModelsDb.Models[model];
            this.rowsUpDown.Minimum = db.Rows;
            this.rowsUpDown.Maximum = Settings.OversizeMax / db.Columns;
            this.rowsUpDown.Value = db.Rows;
            this.columnsUpDown.Minimum = db.Columns;
            this.columnsUpDown.Maximum = Settings.OversizeMax / db.Rows;
            this.columnsUpDown.Value = db.Columns;
        }

        /// <summary>
        /// One of the rows/columns selectors changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RowsColsValueChanged(object sender, EventArgs e)
        {
            if (!(sender is NumericUpDown numericUpDown))
            {
                return;
            }

            var value = (int)numericUpDown.Value;
            switch ((string)numericUpDown.Tag)
            {
                case "Rows":
                    this.columnsUpDown.Maximum = Settings.OversizeMax / value;
                    if (this.columnsUpDown.Value > this.columnsUpDown.Maximum)
                    {
                        this.columnsUpDown.Value = this.columnsUpDown.Maximum;
                    }

                    break;
                case "Columns":
                    this.rowsUpDown.Maximum = Settings.OversizeMax / value;
                    if (this.rowsUpDown.Value > this.rowsUpDown.Maximum)
                    {
                        this.rowsUpDown.Value = this.rowsUpDown.Maximum;
                    }

                    break;
            }
        }

        /// <summary>
        /// The new profile name is validating.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NewProfileValidating(object sender, CancelEventArgs e)
        {
            var text = this.newProfileNameTextBox.Text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (Path.GetInvalidFileNameChars().Any((c) => text.Contains(c)))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidProfileName), I18n.Get(Title.ConnectionEditor));
                e.Cancel = true;
                return;
            }

            var normalizedPath = ProfileManager.NormalizedPath(text, out string error);
            if (normalizedPath == null)
            {
                ErrorBox.Show(error, I18n.Get(Title.ConnectionEditor));
                e.Cancel = true;
                return;
            }

            if (File.Exists(normalizedPath))
            {
                ErrorBox.Show(I18n.Get(Message.ProfileAlreadyExists), I18n.Get(Title.ConnectionEditor));
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// The checked state of the TLS Tunnel or STARTTLS check box changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TlsCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var starttlsChecked = this.starttlsCheckBox.Checked;
            var tlsTunnelChecked = this.tlsTunnelCheckBox.Checked;

            this.acceptLabel.Enabled = starttlsChecked || tlsTunnelChecked;
            this.acceptTextBox.Enabled = starttlsChecked || tlsTunnelChecked;
            this.clientCertificateLabel.Enabled = starttlsChecked || tlsTunnelChecked;
            this.clientCertTextBox.Enabled = starttlsChecked || tlsTunnelChecked;
            this.verifyCertCheckBox.Enabled = starttlsChecked || tlsTunnelChecked;
            if (!starttlsChecked && !tlsTunnelChecked)
            {
                this.verifyCertCheckBox.Checked = false;
            }
        }

        /// <summary>
        /// Runs the tour.
        /// </summary>
        private void RunTour()
        {
            if (this.editingMode == HostEditingMode.QuickConnect)
            {
                var nodes = new[]
                {
                    (this, 1, Orientation.Centered),
                    ((Control)this.HostNameTextBox, (int?)null, Orientation.UpperLeft),
                    (this.PortTextBox, null, Orientation.UpperLeft),
                    (this.tlsTunnelCheckBox, null, Orientation.UpperRight),
                    (this.connectButton, null, Orientation.LowerRight),
                    (this, 2, Orientation.Centered),
                    (this.newProfileRadioButton, null, Orientation.UpperLeft),
                    (this.newProfileNameTextBox, null, Orientation.UpperLeft),
                    (this.modelComboBox, null, Orientation.UpperLeft),
                    (this.rowsUpDown, null, Orientation.UpperRight),
                    (this, 3, Orientation.Centered),
                    (this.connectionNameTextBox, null, Orientation.UpperLeft),
                    (this.LuNamesTextBox, null, Orientation.UpperLeft),
                    (this.LoginMacroTextBox, null, Orientation.LowerLeft),
                    (this.descriptionTextBox, null, Orientation.LowerLeft),
                    (this.titleTextBox, null, Orientation.LowerLeft),
                    (this.loadGroupBox, null, Orientation.UpperRight),
                    (this.hostTypeGroupBox, null, Orientation.LowerRight),
                    (this.printerSessionGroupBox, null, Orientation.LowerRight),
                    (this, 4, Orientation.Centered),
                    (this.acceptTextBox, null, Orientation.UpperLeft),
                    (this.clientCertTextBox, null, Orientation.UpperLeft),
                    (this.verifyCertCheckBox, null, Orientation.UpperRight),
                    (this, 5, Orientation.Centered),
                    (this.localProcessRadioButton, null, Orientation.UpperLeft),
                    (this.commandTextBox, null, Orientation.LowerLeft),
                    (this.commandLineOptionsTextBox, null, Orientation.LowerLeft),
                    (this.connectRecordButton, null, Orientation.LowerRight),
                    (this.cancelButton, null, Orientation.LowerRight),
                    (this.okButton, null, Orientation.LowerRight),
                    (this.helpPictureBox, null, Orientation.LowerRight),
                };
                Tour.Navigate(this, nodes);
            }
            else
            {
                var nodes = new[]
                {
                    (this, 10, Orientation.Centered),
                    ((Control)this.HostNameTextBox, (int?)null, Orientation.UpperLeft),
                    (this.PortTextBox, null, Orientation.UpperLeft),
                    (this.tlsTunnelCheckBox, null, Orientation.UpperRight),
                    (this.okButton, 1, Orientation.LowerRight),
                    (this, 3, Orientation.Centered),
                    (this.connectionNameTextBox, null, Orientation.UpperLeft),
                    (this.LuNamesTextBox, null, Orientation.UpperLeft),
                    (this.LoginMacroTextBox, null, Orientation.LowerLeft),
                    (this.descriptionTextBox, null, Orientation.LowerLeft),
                    (this.titleTextBox, null, Orientation.LowerLeft),
                    (this.loadGroupBox, null, Orientation.UpperRight),
                    (this.hostTypeGroupBox, null, Orientation.LowerRight),
                    (this.printerSessionGroupBox, null, Orientation.LowerRight),
                    (this, 4, Orientation.Centered),
                    (this.acceptTextBox, null, Orientation.UpperLeft),
                    (this.clientCertTextBox, null, Orientation.UpperLeft),
                    (this.verifyCertCheckBox, null, Orientation.UpperRight),
                    (this, 5, Orientation.Centered),
                    (this.localProcessRadioButton, null, Orientation.UpperLeft),
                    (this.commandTextBox, null, Orientation.LowerLeft),
                    (this.commandLineOptionsTextBox, null, Orientation.LowerLeft),
                    (this.cancelButton, null, Orientation.LowerRight),
                    (this.helpPictureBox, null, Orientation.LowerRight),
                };
                Tour.Navigate(this, nodes);
            }
        }

        /// <summary>
        /// An item on the help menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "ConnectionEditor", this.RunTour);
        }

        /// <summary>
        /// The connection editor was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectionEditorActivated(object sender, EventArgs e)
        {
            if (!this.everActivated)
            {
                this.everActivated = true;

                if (this.LoginMacroInsert != null)
                {
                    var ins = this.LoginMacroInsert;
                    this.LoginMacroInsert = null;
                    if (this.StartMacroEditor(ins))
                    {
                        return;
                    }
                }

                if (!Tour.IsComplete(this))
                {
                    this.RunTour();
                }
            }
        }

        /// <summary>
        /// Profile creation state.
        /// </summary>
        public class ProfileSpec
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileSpec"/> class.
            /// </summary>
            /// <param name="name">Profile name.</param>
            /// <param name="model">Model number.</param>
            /// <param name="rows">Desired number of rows.</param>
            /// <param name="columns">Desired number of columns.</param>
            public ProfileSpec(string name, int model, int rows, int columns)
            {
                this.ProfileName = name;
                this.Model = model;
                this.Rows = rows;
                this.Columns = columns;
            }

            /// <summary>
            /// Gets or sets the profile name.
            /// </summary>
            public string ProfileName { get; set; }

            /// <summary>
            /// Gets or sets the model number.
            /// </summary>
            public int Model { get; set; }

            /// <summary>
            /// Gets or sets the rows.
            /// </summary>
            public int Rows { get; set; }

            /// <summary>
            /// Gets or sets the columns.
            /// </summary>
            public int Columns { get; set; }
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
            /// Bad profile name.
            /// </summary>
            public static readonly string InvalidProfileName = I18n.Combine(MessageName, "invalidProfileName");

            /// <summary>
            /// Bad profile name.
            /// </summary>
            public static readonly string ProfileAlreadyExists = I18n.Combine(MessageName, "profileAlreadyExists");

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
            public static readonly string MustSpecifyProfile = I18n.Combine(MessageName, "mustSpecifyProfile");

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
