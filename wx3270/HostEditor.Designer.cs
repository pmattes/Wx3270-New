namespace Wx3270
{
    partial class HostEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostEditor));
            this.NicknameTextBox = new System.Windows.Forms.TextBox();
            this.sessionNameLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.LuNamesTextBox = new System.Windows.Forms.TextBox();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.HostNameTextBox = new System.Windows.Forms.TextBox();
            this.tlsTunnelCheckBox = new System.Windows.Forms.CheckBox();
            this.luNamesLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.hostNameLabel = new System.Windows.Forms.Label();
            this.noLoadConnectButton = new System.Windows.Forms.RadioButton();
            this.loadConnectButton = new System.Windows.Forms.RadioButton();
            this.loadGroupBox = new System.Windows.Forms.GroupBox();
            this.loadReconnectButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.luExplainLabel = new System.Windows.Forms.Label();
            this.hostNameRequiredLabel = new System.Windows.Forms.Label();
            this.hostTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.cicsRadioButton = new System.Windows.Forms.RadioButton();
            this.vmRadioButton = new System.Windows.Forms.RadioButton();
            this.tsoRadioButton = new System.Windows.Forms.RadioButton();
            this.unspecifiedRadioButton = new System.Windows.Forms.RadioButton();
            this.acceptLabel = new System.Windows.Forms.Label();
            this.acceptTextBox = new System.Windows.Forms.TextBox();
            this.nvtCheckBox = new System.Windows.Forms.CheckBox();
            this.VerifyCertCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.starttlsCheckBox = new System.Windows.Forms.CheckBox();
            this.tn3270eCheckBox = new System.Windows.Forms.CheckBox();
            this.telnetCheckBox = new System.Windows.Forms.CheckBox();
            this.loginScreenCheckBox = new System.Windows.Forms.CheckBox();
            this.rightTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.printerSessionGroupBox = new System.Windows.Forms.GroupBox();
            this.specificLuTextBox = new System.Windows.Forms.TextBox();
            this.associatePrinterRadioButton = new System.Windows.Forms.RadioButton();
            this.noPrinterRadioButton = new System.Windows.Forms.RadioButton();
            this.specificLuRadioButton = new System.Windows.Forms.RadioButton();
            this.leftLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.profileNameLabel = new System.Windows.Forms.Label();
            this.profileLabel = new System.Windows.Forms.Label();
            this.optionalLabel = new System.Windows.Forms.Label();
            this.loginMacroLabel = new System.Windows.Forms.Label();
            this.LoginMacroTextBox = new System.Windows.Forms.TextBox();
            this.clientCertificateLabel = new System.Windows.Forms.Label();
            this.clientCertTextBox = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.loadGroupBox.SuspendLayout();
            this.hostTypeGroupBox.SuspendLayout();
            this.optionsLayoutPanel.SuspendLayout();
            this.rightTableLayoutPanel.SuspendLayout();
            this.optionsGroupBox.SuspendLayout();
            this.printerSessionGroupBox.SuspendLayout();
            this.leftLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // NicknameTextBox
            // 
            this.NicknameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NicknameTextBox.Location = new System.Drawing.Point(120, 23);
            this.NicknameTextBox.Name = "NicknameTextBox";
            this.NicknameTextBox.Size = new System.Drawing.Size(215, 20);
            this.NicknameTextBox.TabIndex = 0;
            this.NicknameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.NicknameTextBox_Validating);
            // 
            // sessionNameLabel
            // 
            this.sessionNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sessionNameLabel.AutoSize = true;
            this.sessionNameLabel.Location = new System.Drawing.Point(3, 26);
            this.sessionNameLabel.Name = "sessionNameLabel";
            this.sessionNameLabel.Size = new System.Drawing.Size(73, 13);
            this.sessionNameLabel.TabIndex = 111;
            this.sessionNameLabel.Text = "Session name";
            // 
            // connectButton
            // 
            this.connectButton.ForeColor = System.Drawing.Color.ForestGreen;
            this.connectButton.Location = new System.Drawing.Point(459, 447);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // LuNamesTextBox
            // 
            this.LuNamesTextBox.AcceptsReturn = true;
            this.LuNamesTextBox.Location = new System.Drawing.Point(120, 131);
            this.LuNamesTextBox.Multiline = true;
            this.LuNamesTextBox.Name = "LuNamesTextBox";
            this.LuNamesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LuNamesTextBox.Size = new System.Drawing.Size(215, 45);
            this.LuNamesTextBox.TabIndex = 3;
            this.LuNamesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.LuNamesTextBox_Validating);
            // 
            // PortTextBox
            // 
            this.PortTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PortTextBox.Location = new System.Drawing.Point(120, 105);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(215, 20);
            this.PortTextBox.TabIndex = 2;
            this.PortTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PortTextBox_Validating);
            // 
            // HostNameTextBox
            // 
            this.HostNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HostNameTextBox.Location = new System.Drawing.Point(120, 64);
            this.HostNameTextBox.Name = "HostNameTextBox";
            this.HostNameTextBox.Size = new System.Drawing.Size(215, 20);
            this.HostNameTextBox.TabIndex = 1;
            this.HostNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.HostNameTextBox_Validating);
            // 
            // tlsTunnelCheckBox
            // 
            this.tlsTunnelCheckBox.AutoSize = true;
            this.tlsTunnelCheckBox.Location = new System.Drawing.Point(3, 3);
            this.tlsTunnelCheckBox.Name = "tlsTunnelCheckBox";
            this.tlsTunnelCheckBox.Size = new System.Drawing.Size(112, 17);
            this.tlsTunnelCheckBox.TabIndex = 0;
            this.tlsTunnelCheckBox.Tag = "L:";
            this.tlsTunnelCheckBox.Text = "Create TLS tunnel";
            this.tlsTunnelCheckBox.UseVisualStyleBackColor = true;
            this.tlsTunnelCheckBox.CheckedChanged += new System.EventHandler(this.TlsCheckBox_CheckedChanged);
            // 
            // luNamesLabel
            // 
            this.luNamesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.luNamesLabel.AutoSize = true;
            this.luNamesLabel.Location = new System.Drawing.Point(3, 147);
            this.luNamesLabel.Name = "luNamesLabel";
            this.luNamesLabel.Size = new System.Drawing.Size(55, 13);
            this.luNamesLabel.TabIndex = 112;
            this.luNamesLabel.Text = "LU names";
            // 
            // portLabel
            // 
            this.portLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(3, 108);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(26, 13);
            this.portLabel.TabIndex = 113;
            this.portLabel.Text = "Port";
            // 
            // hostNameLabel
            // 
            this.hostNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.hostNameLabel.AutoSize = true;
            this.hostNameLabel.Location = new System.Drawing.Point(3, 67);
            this.hostNameLabel.Name = "hostNameLabel";
            this.hostNameLabel.Size = new System.Drawing.Size(58, 13);
            this.hostNameLabel.TabIndex = 114;
            this.hostNameLabel.Text = "Host name";
            // 
            // noLoadConnectButton
            // 
            this.noLoadConnectButton.AutoSize = true;
            this.noLoadConnectButton.Checked = true;
            this.noLoadConnectButton.Location = new System.Drawing.Point(6, 19);
            this.noLoadConnectButton.Name = "noLoadConnectButton";
            this.noLoadConnectButton.Size = new System.Drawing.Size(153, 17);
            this.noLoadConnectButton.TabIndex = 0;
            this.noLoadConnectButton.TabStop = true;
            this.noLoadConnectButton.Tag = "None";
            this.noLoadConnectButton.Text = "Do not connect to this host";
            this.noLoadConnectButton.UseVisualStyleBackColor = true;
            // 
            // loadConnectButton
            // 
            this.loadConnectButton.AutoSize = true;
            this.loadConnectButton.Location = new System.Drawing.Point(6, 40);
            this.loadConnectButton.Name = "loadConnectButton";
            this.loadConnectButton.Size = new System.Drawing.Size(133, 17);
            this.loadConnectButton.TabIndex = 0;
            this.loadConnectButton.Tag = "Connect";
            this.loadConnectButton.Text = "🗲 Connect to this host";
            this.loadConnectButton.UseVisualStyleBackColor = true;
            // 
            // loadGroupBox
            // 
            this.loadGroupBox.Controls.Add(this.loadReconnectButton);
            this.loadGroupBox.Controls.Add(this.loadConnectButton);
            this.loadGroupBox.Controls.Add(this.noLoadConnectButton);
            this.loadGroupBox.Location = new System.Drawing.Point(3, 166);
            this.loadGroupBox.Name = "loadGroupBox";
            this.loadGroupBox.Size = new System.Drawing.Size(353, 88);
            this.loadGroupBox.TabIndex = 1;
            this.loadGroupBox.TabStop = false;
            this.loadGroupBox.Text = "When profile is loaded";
            // 
            // loadReconnectButton
            // 
            this.loadReconnectButton.AutoSize = true;
            this.loadReconnectButton.Location = new System.Drawing.Point(6, 63);
            this.loadReconnectButton.Name = "loadReconnectButton";
            this.loadReconnectButton.Size = new System.Drawing.Size(180, 17);
            this.loadReconnectButton.TabIndex = 1;
            this.loadReconnectButton.Tag = "Reconnect";
            this.loadReconnectButton.Text = "🗲+ Keep connecting to this host";
            this.loadReconnectButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(540, 447);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(621, 447);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // luExplainLabel
            // 
            this.luExplainLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.luExplainLabel.AutoSize = true;
            this.luExplainLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.luExplainLabel.Location = new System.Drawing.Point(174, 179);
            this.luExplainLabel.Name = "luExplainLabel";
            this.luExplainLabel.Size = new System.Drawing.Size(161, 12);
            this.luExplainLabel.TabIndex = 120;
            this.luExplainLabel.Text = "separate with commas or white space";
            this.luExplainLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // hostNameRequiredLabel
            // 
            this.hostNameRequiredLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hostNameRequiredLabel.AutoSize = true;
            this.hostNameRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hostNameRequiredLabel.Location = new System.Drawing.Point(297, 87);
            this.hostNameRequiredLabel.Name = "hostNameRequiredLabel";
            this.hostNameRequiredLabel.Size = new System.Drawing.Size(38, 12);
            this.hostNameRequiredLabel.TabIndex = 121;
            this.hostNameRequiredLabel.Text = "required";
            this.hostNameRequiredLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // hostTypeGroupBox
            // 
            this.hostTypeGroupBox.Controls.Add(this.cicsRadioButton);
            this.hostTypeGroupBox.Controls.Add(this.vmRadioButton);
            this.hostTypeGroupBox.Controls.Add(this.tsoRadioButton);
            this.hostTypeGroupBox.Controls.Add(this.unspecifiedRadioButton);
            this.hostTypeGroupBox.Location = new System.Drawing.Point(3, 260);
            this.hostTypeGroupBox.Name = "hostTypeGroupBox";
            this.hostTypeGroupBox.Size = new System.Drawing.Size(353, 66);
            this.hostTypeGroupBox.TabIndex = 2;
            this.hostTypeGroupBox.TabStop = false;
            this.hostTypeGroupBox.Text = "Host type (for file transfers)";
            // 
            // cicsRadioButton
            // 
            this.cicsRadioButton.AutoSize = true;
            this.cicsRadioButton.Location = new System.Drawing.Point(179, 42);
            this.cicsRadioButton.Name = "cicsRadioButton";
            this.cicsRadioButton.Size = new System.Drawing.Size(49, 17);
            this.cicsRadioButton.TabIndex = 3;
            this.cicsRadioButton.Tag = "Cics";
            this.cicsRadioButton.Text = "CICS";
            this.cicsRadioButton.UseVisualStyleBackColor = true;
            // 
            // vmRadioButton
            // 
            this.vmRadioButton.AutoSize = true;
            this.vmRadioButton.Location = new System.Drawing.Point(179, 19);
            this.vmRadioButton.Name = "vmRadioButton";
            this.vmRadioButton.Size = new System.Drawing.Size(69, 17);
            this.vmRadioButton.TabIndex = 2;
            this.vmRadioButton.Tag = "Vm";
            this.vmRadioButton.Text = "VM/CMS";
            this.vmRadioButton.UseVisualStyleBackColor = true;
            // 
            // tsoRadioButton
            // 
            this.tsoRadioButton.AutoSize = true;
            this.tsoRadioButton.Location = new System.Drawing.Point(6, 42);
            this.tsoRadioButton.Name = "tsoRadioButton";
            this.tsoRadioButton.Size = new System.Drawing.Size(47, 17);
            this.tsoRadioButton.TabIndex = 1;
            this.tsoRadioButton.Tag = "Tso";
            this.tsoRadioButton.Text = "TSO";
            this.tsoRadioButton.UseVisualStyleBackColor = true;
            // 
            // unspecifiedRadioButton
            // 
            this.unspecifiedRadioButton.AutoSize = true;
            this.unspecifiedRadioButton.Checked = true;
            this.unspecifiedRadioButton.Location = new System.Drawing.Point(6, 19);
            this.unspecifiedRadioButton.Name = "unspecifiedRadioButton";
            this.unspecifiedRadioButton.Size = new System.Drawing.Size(81, 17);
            this.unspecifiedRadioButton.TabIndex = 0;
            this.unspecifiedRadioButton.TabStop = true;
            this.unspecifiedRadioButton.Tag = "Unspecified";
            this.unspecifiedRadioButton.Text = "Unspecified";
            this.unspecifiedRadioButton.UseVisualStyleBackColor = true;
            // 
            // acceptLabel
            // 
            this.acceptLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.acceptLabel.AutoSize = true;
            this.acceptLabel.Location = new System.Drawing.Point(3, 200);
            this.acceptLabel.Name = "acceptLabel";
            this.acceptLabel.Size = new System.Drawing.Size(90, 13);
            this.acceptLabel.TabIndex = 123;
            this.acceptLabel.Text = "Accept hostname";
            // 
            // acceptTextBox
            // 
            this.acceptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.acceptTextBox.Location = new System.Drawing.Point(120, 197);
            this.acceptTextBox.Name = "acceptTextBox";
            this.acceptTextBox.Size = new System.Drawing.Size(215, 20);
            this.acceptTextBox.TabIndex = 4;
            this.acceptTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.AcceptTextBox_Validating);
            // 
            // nvtCheckBox
            // 
            this.nvtCheckBox.AutoSize = true;
            this.nvtCheckBox.Location = new System.Drawing.Point(3, 72);
            this.nvtCheckBox.Name = "nvtCheckBox";
            this.nvtCheckBox.Size = new System.Drawing.Size(77, 17);
            this.nvtCheckBox.TabIndex = 3;
            this.nvtCheckBox.Tag = "A:";
            this.nvtCheckBox.Text = "NVT mode";
            this.nvtCheckBox.UseVisualStyleBackColor = true;
            // 
            // VerifyCertCheckBox
            // 
            this.VerifyCertCheckBox.AutoSize = true;
            this.VerifyCertCheckBox.Checked = true;
            this.VerifyCertCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.VerifyCertCheckBox.Location = new System.Drawing.Point(3, 49);
            this.VerifyCertCheckBox.Name = "VerifyCertCheckBox";
            this.VerifyCertCheckBox.Size = new System.Drawing.Size(124, 17);
            this.VerifyCertCheckBox.TabIndex = 2;
            this.VerifyCertCheckBox.Tag = "~Y:";
            this.VerifyCertCheckBox.Text = "Verify host certificate";
            this.VerifyCertCheckBox.UseVisualStyleBackColor = true;
            this.VerifyCertCheckBox.CheckedChanged += new System.EventHandler(this.VerifyCertCheckBox_CheckedChanged);
            // 
            // optionsLayoutPanel
            // 
            this.optionsLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.optionsLayoutPanel.ColumnCount = 2;
            this.optionsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.optionsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.optionsLayoutPanel.Controls.Add(this.tlsTunnelCheckBox, 0, 0);
            this.optionsLayoutPanel.Controls.Add(this.starttlsCheckBox, 0, 1);
            this.optionsLayoutPanel.Controls.Add(this.nvtCheckBox, 0, 3);
            this.optionsLayoutPanel.Controls.Add(this.VerifyCertCheckBox, 0, 2);
            this.optionsLayoutPanel.Controls.Add(this.tn3270eCheckBox, 0, 4);
            this.optionsLayoutPanel.Controls.Add(this.telnetCheckBox, 1, 0);
            this.optionsLayoutPanel.Controls.Add(this.loginScreenCheckBox, 0, 6);
            this.optionsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.optionsLayoutPanel.Name = "optionsLayoutPanel";
            this.optionsLayoutPanel.RowCount = 7;
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsLayoutPanel.Size = new System.Drawing.Size(347, 138);
            this.optionsLayoutPanel.TabIndex = 0;
            // 
            // starttlsCheckBox
            // 
            this.starttlsCheckBox.AutoSize = true;
            this.starttlsCheckBox.Checked = true;
            this.starttlsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.starttlsCheckBox.Location = new System.Drawing.Point(3, 26);
            this.starttlsCheckBox.Name = "starttlsCheckBox";
            this.starttlsCheckBox.Size = new System.Drawing.Size(119, 17);
            this.starttlsCheckBox.TabIndex = 1;
            this.starttlsCheckBox.Text = "Accept STARTTLS";
            this.starttlsCheckBox.UseVisualStyleBackColor = true;
            // 
            // tn3270eCheckBox
            // 
            this.tn3270eCheckBox.AutoSize = true;
            this.tn3270eCheckBox.Checked = true;
            this.tn3270eCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tn3270eCheckBox.Location = new System.Drawing.Point(3, 95);
            this.tn3270eCheckBox.Name = "tn3270eCheckBox";
            this.tn3270eCheckBox.Size = new System.Drawing.Size(109, 17);
            this.tn3270eCheckBox.TabIndex = 4;
            this.tn3270eCheckBox.Tag = "~N:";
            this.tn3270eCheckBox.Text = "Accept TN3270E";
            this.tn3270eCheckBox.UseVisualStyleBackColor = true;
            // 
            // telnetCheckBox
            // 
            this.telnetCheckBox.AutoSize = true;
            this.telnetCheckBox.Checked = true;
            this.telnetCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.telnetCheckBox.Location = new System.Drawing.Point(176, 3);
            this.telnetCheckBox.Name = "telnetCheckBox";
            this.telnetCheckBox.Size = new System.Drawing.Size(90, 17);
            this.telnetCheckBox.TabIndex = 6;
            this.telnetCheckBox.Tag = "~T:";
            this.telnetCheckBox.Text = "Use TELNET";
            this.telnetCheckBox.UseVisualStyleBackColor = true;
            this.telnetCheckBox.Click += new System.EventHandler(this.TelnetCheckBox_Click);
            // 
            // loginScreenCheckBox
            // 
            this.loginScreenCheckBox.AutoSize = true;
            this.loginScreenCheckBox.Checked = true;
            this.loginScreenCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loginScreenCheckBox.Location = new System.Drawing.Point(3, 118);
            this.loginScreenCheckBox.Name = "loginScreenCheckBox";
            this.loginScreenCheckBox.Size = new System.Drawing.Size(123, 17);
            this.loginScreenCheckBox.TabIndex = 5;
            this.loginScreenCheckBox.Tag = "~C:";
            this.loginScreenCheckBox.Text = "Wait for login screen";
            this.loginScreenCheckBox.UseVisualStyleBackColor = true;
            // 
            // rightTableLayoutPanel
            // 
            this.rightTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightTableLayoutPanel.ColumnCount = 1;
            this.rightTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTableLayoutPanel.Controls.Add(this.hostTypeGroupBox, 0, 3);
            this.rightTableLayoutPanel.Controls.Add(this.loadGroupBox, 0, 2);
            this.rightTableLayoutPanel.Controls.Add(this.optionsGroupBox, 0, 0);
            this.rightTableLayoutPanel.Controls.Add(this.printerSessionGroupBox, 0, 4);
            this.rightTableLayoutPanel.Location = new System.Drawing.Point(367, 9);
            this.rightTableLayoutPanel.Name = "rightTableLayoutPanel";
            this.rightTableLayoutPanel.RowCount = 5;
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.Size = new System.Drawing.Size(359, 432);
            this.rightTableLayoutPanel.TabIndex = 1;
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Controls.Add(this.optionsLayoutPanel);
            this.optionsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(353, 157);
            this.optionsGroupBox.TabIndex = 0;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // printerSessionGroupBox
            // 
            this.printerSessionGroupBox.Controls.Add(this.specificLuTextBox);
            this.printerSessionGroupBox.Controls.Add(this.associatePrinterRadioButton);
            this.printerSessionGroupBox.Controls.Add(this.noPrinterRadioButton);
            this.printerSessionGroupBox.Controls.Add(this.specificLuRadioButton);
            this.printerSessionGroupBox.Location = new System.Drawing.Point(3, 332);
            this.printerSessionGroupBox.Name = "printerSessionGroupBox";
            this.printerSessionGroupBox.Size = new System.Drawing.Size(353, 95);
            this.printerSessionGroupBox.TabIndex = 3;
            this.printerSessionGroupBox.TabStop = false;
            this.printerSessionGroupBox.Text = "3287 printer session";
            this.printerSessionGroupBox.Validating += new System.ComponentModel.CancelEventHandler(this.PrinterSessionGroupBox_Validating);
            // 
            // specificLuTextBox
            // 
            this.specificLuTextBox.Enabled = false;
            this.specificLuTextBox.Location = new System.Drawing.Point(89, 64);
            this.specificLuTextBox.Name = "specificLuTextBox";
            this.specificLuTextBox.Size = new System.Drawing.Size(176, 20);
            this.specificLuTextBox.TabIndex = 3;
            this.specificLuTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.SpecificLuTextBox_Validating);
            // 
            // associatePrinterRadioButton
            // 
            this.associatePrinterRadioButton.AutoSize = true;
            this.associatePrinterRadioButton.Location = new System.Drawing.Point(6, 42);
            this.associatePrinterRadioButton.Name = "associatePrinterRadioButton";
            this.associatePrinterRadioButton.Size = new System.Drawing.Size(183, 17);
            this.associatePrinterRadioButton.TabIndex = 1;
            this.associatePrinterRadioButton.Tag = "Associate";
            this.associatePrinterRadioButton.Text = "Associate with interactive session";
            this.associatePrinterRadioButton.UseVisualStyleBackColor = true;
            // 
            // noPrinterRadioButton
            // 
            this.noPrinterRadioButton.AutoSize = true;
            this.noPrinterRadioButton.Checked = true;
            this.noPrinterRadioButton.Location = new System.Drawing.Point(6, 19);
            this.noPrinterRadioButton.Name = "noPrinterRadioButton";
            this.noPrinterRadioButton.Size = new System.Drawing.Size(51, 17);
            this.noPrinterRadioButton.TabIndex = 0;
            this.noPrinterRadioButton.TabStop = true;
            this.noPrinterRadioButton.Tag = "None";
            this.noPrinterRadioButton.Text = "None";
            this.noPrinterRadioButton.UseVisualStyleBackColor = true;
            // 
            // specificLuRadioButton
            // 
            this.specificLuRadioButton.AutoSize = true;
            this.specificLuRadioButton.Location = new System.Drawing.Point(6, 65);
            this.specificLuRadioButton.Name = "specificLuRadioButton";
            this.specificLuRadioButton.Size = new System.Drawing.Size(80, 17);
            this.specificLuRadioButton.TabIndex = 2;
            this.specificLuRadioButton.Tag = "SpecificLu";
            this.specificLuRadioButton.Text = "Specific LU";
            this.specificLuRadioButton.UseVisualStyleBackColor = true;
            // 
            // leftLayoutPanel
            // 
            this.leftLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftLayoutPanel.ColumnCount = 2;
            this.leftLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.leftLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftLayoutPanel.Controls.Add(this.luExplainLabel, 1, 7);
            this.leftLayoutPanel.Controls.Add(this.profileNameLabel, 1, 0);
            this.leftLayoutPanel.Controls.Add(this.LuNamesTextBox, 1, 6);
            this.leftLayoutPanel.Controls.Add(this.profileLabel, 0, 0);
            this.leftLayoutPanel.Controls.Add(this.hostNameRequiredLabel, 1, 4);
            this.leftLayoutPanel.Controls.Add(this.NicknameTextBox, 1, 1);
            this.leftLayoutPanel.Controls.Add(this.HostNameTextBox, 1, 3);
            this.leftLayoutPanel.Controls.Add(this.optionalLabel, 1, 2);
            this.leftLayoutPanel.Controls.Add(this.acceptTextBox, 1, 8);
            this.leftLayoutPanel.Controls.Add(this.sessionNameLabel, 0, 1);
            this.leftLayoutPanel.Controls.Add(this.hostNameLabel, 0, 3);
            this.leftLayoutPanel.Controls.Add(this.portLabel, 0, 5);
            this.leftLayoutPanel.Controls.Add(this.acceptLabel, 0, 8);
            this.leftLayoutPanel.Controls.Add(this.luNamesLabel, 0, 6);
            this.leftLayoutPanel.Controls.Add(this.PortTextBox, 1, 5);
            this.leftLayoutPanel.Controls.Add(this.loginMacroLabel, 0, 10);
            this.leftLayoutPanel.Controls.Add(this.LoginMacroTextBox, 1, 10);
            this.leftLayoutPanel.Controls.Add(this.clientCertificateLabel, 0, 9);
            this.leftLayoutPanel.Controls.Add(this.clientCertTextBox, 1, 9);
            this.leftLayoutPanel.Controls.Add(this.label1, 0, 11);
            this.leftLayoutPanel.Controls.Add(this.descriptionTextBox, 1, 11);
            this.leftLayoutPanel.Controls.Add(this.label2, 0, 12);
            this.leftLayoutPanel.Controls.Add(this.titleTextBox, 1, 12);
            this.leftLayoutPanel.Location = new System.Drawing.Point(12, 9);
            this.leftLayoutPanel.Name = "leftLayoutPanel";
            this.leftLayoutPanel.RowCount = 13;
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.leftLayoutPanel.Size = new System.Drawing.Size(338, 432);
            this.leftLayoutPanel.TabIndex = 0;
            // 
            // profileNameLabel
            // 
            this.profileNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileNameLabel.AutoSize = true;
            this.profileNameLabel.Location = new System.Drawing.Point(120, 3);
            this.profileNameLabel.Name = "profileNameLabel";
            this.profileNameLabel.Size = new System.Drawing.Size(70, 13);
            this.profileNameLabel.TabIndex = 131;
            this.profileNameLabel.Text = "`Profile Name";
            // 
            // profileLabel
            // 
            this.profileLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileLabel.AutoSize = true;
            this.profileLabel.Location = new System.Drawing.Point(3, 3);
            this.profileLabel.Name = "profileLabel";
            this.profileLabel.Size = new System.Drawing.Size(36, 13);
            this.profileLabel.TabIndex = 130;
            this.profileLabel.Text = "Profile";
            // 
            // optionalLabel
            // 
            this.optionalLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.optionalLabel.AutoSize = true;
            this.optionalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionalLabel.Location = new System.Drawing.Point(298, 46);
            this.optionalLabel.Name = "optionalLabel";
            this.optionalLabel.Size = new System.Drawing.Size(37, 12);
            this.optionalLabel.TabIndex = 122;
            this.optionalLabel.Text = "optional";
            this.optionalLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // loginMacroLabel
            // 
            this.loginMacroLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.loginMacroLabel.AutoSize = true;
            this.loginMacroLabel.Location = new System.Drawing.Point(3, 305);
            this.loginMacroLabel.Name = "loginMacroLabel";
            this.loginMacroLabel.Size = new System.Drawing.Size(65, 13);
            this.loginMacroLabel.TabIndex = 109;
            this.loginMacroLabel.Text = "Login macro";
            // 
            // LoginMacroTextBox
            // 
            this.LoginMacroTextBox.AcceptsReturn = true;
            this.LoginMacroTextBox.Location = new System.Drawing.Point(120, 249);
            this.LoginMacroTextBox.Multiline = true;
            this.LoginMacroTextBox.Name = "LoginMacroTextBox";
            this.LoginMacroTextBox.ReadOnly = true;
            this.LoginMacroTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LoginMacroTextBox.Size = new System.Drawing.Size(215, 125);
            this.LoginMacroTextBox.TabIndex = 6;
            this.LoginMacroTextBox.TabStop = false;
            this.LoginMacroTextBox.Click += new System.EventHandler(this.LoginMacroEditButton_Click);
            // 
            // clientCertificateLabel
            // 
            this.clientCertificateLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clientCertificateLabel.AutoSize = true;
            this.clientCertificateLabel.Location = new System.Drawing.Point(3, 226);
            this.clientCertificateLabel.Name = "clientCertificateLabel";
            this.clientCertificateLabel.Size = new System.Drawing.Size(111, 13);
            this.clientCertificateLabel.TabIndex = 125;
            this.clientCertificateLabel.Text = "Client certificate name";
            // 
            // clientCertTextBox
            // 
            this.clientCertTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clientCertTextBox.Location = new System.Drawing.Point(120, 223);
            this.clientCertTextBox.Name = "clientCertTextBox";
            this.clientCertTextBox.Size = new System.Drawing.Size(215, 20);
            this.clientCertTextBox.TabIndex = 5;
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(702, 447);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(23, 23);
            this.helpPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.helpPictureBox.TabIndex = 132;
            this.helpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.helpPictureBox, "Get help");
            this.helpPictureBox.Click += new System.EventHandler(this.Help_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 383);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 132;
            this.label1.Text = "Description";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(120, 380);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(215, 20);
            this.descriptionTextBox.TabIndex = 110;
            this.toolTip1.SetToolTip(this.descriptionTextBox, "Description of this host\r\nShown in the Profiles and Connections window");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 411);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 134;
            this.label2.Text = "Window title";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(120, 406);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(215, 20);
            this.titleTextBox.TabIndex = 111;
            this.toolTip1.SetToolTip(this.titleTextBox, "Window title override");
            // 
            // HostEditor
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(738, 477);
            this.Controls.Add(this.helpPictureBox);
            this.Controls.Add(this.leftLayoutPanel);
            this.Controls.Add(this.rightTableLayoutPanel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.connectButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "HostEditor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection Editor";
            this.loadGroupBox.ResumeLayout(false);
            this.loadGroupBox.PerformLayout();
            this.hostTypeGroupBox.ResumeLayout(false);
            this.hostTypeGroupBox.PerformLayout();
            this.optionsLayoutPanel.ResumeLayout(false);
            this.optionsLayoutPanel.PerformLayout();
            this.rightTableLayoutPanel.ResumeLayout(false);
            this.optionsGroupBox.ResumeLayout(false);
            this.printerSessionGroupBox.ResumeLayout(false);
            this.printerSessionGroupBox.PerformLayout();
            this.leftLayoutPanel.ResumeLayout(false);
            this.leftLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox NicknameTextBox;
        private System.Windows.Forms.Label sessionNameLabel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox LuNamesTextBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.TextBox HostNameTextBox;
        private System.Windows.Forms.CheckBox tlsTunnelCheckBox;
        private System.Windows.Forms.Label luNamesLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label hostNameLabel;
        private System.Windows.Forms.RadioButton noLoadConnectButton;
        private System.Windows.Forms.RadioButton loadConnectButton;
        private System.Windows.Forms.GroupBox loadGroupBox;
        private System.Windows.Forms.RadioButton loadReconnectButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label luExplainLabel;
        private System.Windows.Forms.Label hostNameRequiredLabel;
        private System.Windows.Forms.GroupBox hostTypeGroupBox;
        private System.Windows.Forms.RadioButton cicsRadioButton;
        private System.Windows.Forms.RadioButton vmRadioButton;
        private System.Windows.Forms.RadioButton tsoRadioButton;
        private System.Windows.Forms.RadioButton unspecifiedRadioButton;
        private System.Windows.Forms.Label acceptLabel;
        private System.Windows.Forms.TextBox acceptTextBox;
        private System.Windows.Forms.CheckBox nvtCheckBox;
        private System.Windows.Forms.CheckBox VerifyCertCheckBox;
        private System.Windows.Forms.TableLayoutPanel optionsLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel rightTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel leftLayoutPanel;
        private System.Windows.Forms.Label optionalLabel;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Label profileLabel;
        private System.Windows.Forms.Label profileNameLabel;
        private System.Windows.Forms.CheckBox starttlsCheckBox;
        private System.Windows.Forms.Label loginMacroLabel;
        private System.Windows.Forms.TextBox LoginMacroTextBox;
        private System.Windows.Forms.Label clientCertificateLabel;
        private System.Windows.Forms.TextBox clientCertTextBox;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox printerSessionGroupBox;
        private System.Windows.Forms.TextBox specificLuTextBox;
        private System.Windows.Forms.RadioButton specificLuRadioButton;
        private System.Windows.Forms.RadioButton associatePrinterRadioButton;
        private System.Windows.Forms.RadioButton noPrinterRadioButton;
        private System.Windows.Forms.CheckBox tn3270eCheckBox;
        private System.Windows.Forms.CheckBox loginScreenCheckBox;
        private System.Windows.Forms.CheckBox telnetCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox titleTextBox;
    }
}