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
            this.connectionNameTextBox = new System.Windows.Forms.TextBox();
            this.sessionNameLabel = new System.Windows.Forms.Label();
            this.LuNamesTextBox = new System.Windows.Forms.TextBox();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.HostNameTextBox = new System.Windows.Forms.TextBox();
            this.luNamesLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.hostNameLabel = new System.Windows.Forms.Label();
            this.noLoadConnectButton = new System.Windows.Forms.RadioButton();
            this.loadConnectButton = new System.Windows.Forms.RadioButton();
            this.loadGroupBox = new System.Windows.Forms.GroupBox();
            this.loadTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.loadReconnectButton = new System.Windows.Forms.RadioButton();
            this.hostTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.hostTypeTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.cicsRadioButton = new System.Windows.Forms.RadioButton();
            this.unspecifiedRadioButton = new System.Windows.Forms.RadioButton();
            this.vmRadioButton = new System.Windows.Forms.RadioButton();
            this.tsoRadioButton = new System.Windows.Forms.RadioButton();
            this.acceptLabel = new System.Windows.Forms.Label();
            this.acceptTextBox = new System.Windows.Forms.TextBox();
            this.rightTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.connectButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.helpContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayHelpInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectRecordButton = new System.Windows.Forms.Button();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tlsTunnelCheckBox = new System.Windows.Forms.CheckBox();
            this.starttlsCheckBox = new System.Windows.Forms.CheckBox();
            this.nvtCheckBox = new System.Windows.Forms.CheckBox();
            this.verifyCertCheckBox = new System.Windows.Forms.CheckBox();
            this.tn3270eCheckBox = new System.Windows.Forms.CheckBox();
            this.telnetCheckBox = new System.Windows.Forms.CheckBox();
            this.loginScreenCheckBox = new System.Windows.Forms.CheckBox();
            this.printerSessionGroupBox = new System.Windows.Forms.GroupBox();
            this.printerSessionTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.specificLuTextBox = new System.Windows.Forms.TextBox();
            this.associatePrinterRadioButton = new System.Windows.Forms.RadioButton();
            this.noPrinterRadioButton = new System.Windows.Forms.RadioButton();
            this.specificLuRadioButton = new System.Windows.Forms.RadioButton();
            this.localProcessInputGroupBox = new System.Windows.Forms.GroupBox();
            this.localProcessInputTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lineModeRadioButton = new System.Windows.Forms.RadioButton();
            this.characterModeRadioButton = new System.Windows.Forms.RadioButton();
            this.characterModeCrLfRadioButton = new System.Windows.Forms.RadioButton();
            this.profileNameLabel = new System.Windows.Forms.Label();
            this.profileLabel = new System.Windows.Forms.Label();
            this.loginMacroLabel = new System.Windows.Forms.Label();
            this.LoginMacroTextBox = new System.Windows.Forms.TextBox();
            this.clientCertificateLabel = new System.Windows.Forms.Label();
            this.clientCertTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.windowTitleLabel = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.commandLineOptionsTextBox = new System.Windows.Forms.TextBox();
            this.connectionTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.connectionTypeTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.hostRadioButton = new System.Windows.Forms.RadioButton();
            this.localProcessRadioButton = new System.Windows.Forms.RadioButton();
            this.overallTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.leftTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.basicParametersGroupBox = new System.Windows.Forms.GroupBox();
            this.basicParametersTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.commandLineOptionsLabel = new System.Windows.Forms.Label();
            this.commandLabel = new System.Windows.Forms.Label();
            this.commandOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.newProfileRadioButton = new System.Windows.Forms.RadioButton();
            this.addToProfileRadioButton = new System.Windows.Forms.RadioButton();
            this.profileTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.modelLabel = new System.Windows.Forms.Label();
            this.rowsLabel = new System.Windows.Forms.Label();
            this.columnsLabel = new System.Windows.Forms.Label();
            this.profileNameHeaderLabel = new System.Windows.Forms.Label();
            this.addToProfileNameLabel = new System.Windows.Forms.Label();
            this.newProfileNameTextBox = new System.Windows.Forms.TextBox();
            this.rowsUpDown = new System.Windows.Forms.NumericUpDown();
            this.columnsUpDown = new System.Windows.Forms.NumericUpDown();
            this.modelComboBox = new System.Windows.Forms.ComboBox();
            this.addToProfileModelLabel = new System.Windows.Forms.Label();
            this.addToProfileRowsLabel = new System.Windows.Forms.Label();
            this.addToProfileColumnsLabel = new System.Windows.Forms.Label();
            this.profileGroupBox = new System.Windows.Forms.GroupBox();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.loadGroupBox.SuspendLayout();
            this.loadTableLayoutPanel.SuspendLayout();
            this.hostTypeGroupBox.SuspendLayout();
            this.hostTypeTableLayoutPanel.SuspendLayout();
            this.rightTableLayoutPanel.SuspendLayout();
            this.buttonsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.helpContextMenuStrip.SuspendLayout();
            this.optionsGroupBox.SuspendLayout();
            this.optionsLayoutPanel.SuspendLayout();
            this.printerSessionGroupBox.SuspendLayout();
            this.printerSessionTableLayoutPanel.SuspendLayout();
            this.localProcessInputGroupBox.SuspendLayout();
            this.localProcessInputTableLayoutPanel.SuspendLayout();
            this.connectionTypeGroupBox.SuspendLayout();
            this.connectionTypeTableLayoutPanel.SuspendLayout();
            this.overallTableLayoutPanel.SuspendLayout();
            this.leftTableLayoutPanel.SuspendLayout();
            this.basicParametersGroupBox.SuspendLayout();
            this.basicParametersTableLayoutPanel.SuspendLayout();
            this.profileTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rowsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnsUpDown)).BeginInit();
            this.profileGroupBox.SuspendLayout();
            this.outerTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectionNameTextBox
            // 
            this.connectionNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionNameTextBox.Location = new System.Drawing.Point(124, 24);
            this.connectionNameTextBox.Name = "connectionNameTextBox";
            this.connectionNameTextBox.Size = new System.Drawing.Size(230, 20);
            this.connectionNameTextBox.TabIndex = 3;
            this.connectionNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.NicknameTextBox_Validating);
            // 
            // sessionNameLabel
            // 
            this.sessionNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sessionNameLabel.AutoSize = true;
            this.sessionNameLabel.Location = new System.Drawing.Point(3, 26);
            this.sessionNameLabel.Name = "sessionNameLabel";
            this.sessionNameLabel.Size = new System.Drawing.Size(104, 15);
            this.sessionNameLabel.TabIndex = 2;
            this.sessionNameLabel.Text = "Connection name";
            // 
            // LuNamesTextBox
            // 
            this.LuNamesTextBox.AcceptsReturn = true;
            this.LuNamesTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.LuNamesTextBox.Location = new System.Drawing.Point(124, 102);
            this.LuNamesTextBox.Multiline = true;
            this.LuNamesTextBox.Name = "LuNamesTextBox";
            this.LuNamesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LuNamesTextBox.Size = new System.Drawing.Size(230, 45);
            this.LuNamesTextBox.TabIndex = 9;
            this.LuNamesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.LuNamesTextBox_Validating);
            // 
            // PortTextBox
            // 
            this.PortTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PortTextBox.Location = new System.Drawing.Point(124, 76);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(230, 20);
            this.PortTextBox.TabIndex = 7;
            this.PortTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PortTextBox_Validating);
            // 
            // HostNameTextBox
            // 
            this.HostNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.HostNameTextBox.Location = new System.Drawing.Point(124, 50);
            this.HostNameTextBox.Name = "HostNameTextBox";
            this.HostNameTextBox.Size = new System.Drawing.Size(230, 20);
            this.HostNameTextBox.TabIndex = 5;
            this.HostNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.HostNameTextBox_Validating);
            // 
            // luNamesLabel
            // 
            this.luNamesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.luNamesLabel.AutoSize = true;
            this.luNamesLabel.Location = new System.Drawing.Point(3, 117);
            this.luNamesLabel.Name = "luNamesLabel";
            this.luNamesLabel.Size = new System.Drawing.Size(64, 15);
            this.luNamesLabel.TabIndex = 8;
            this.luNamesLabel.Text = "LU names";
            // 
            // portLabel
            // 
            this.portLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(3, 78);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(29, 15);
            this.portLabel.TabIndex = 6;
            this.portLabel.Text = "Port";
            // 
            // hostNameLabel
            // 
            this.hostNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.hostNameLabel.AutoSize = true;
            this.hostNameLabel.Location = new System.Drawing.Point(3, 52);
            this.hostNameLabel.Name = "hostNameLabel";
            this.hostNameLabel.Size = new System.Drawing.Size(72, 15);
            this.hostNameLabel.TabIndex = 4;
            this.hostNameLabel.Text = "Host name*";
            // 
            // noLoadConnectButton
            // 
            this.noLoadConnectButton.AutoSize = true;
            this.noLoadConnectButton.Checked = true;
            this.noLoadConnectButton.Location = new System.Drawing.Point(3, 3);
            this.noLoadConnectButton.Name = "noLoadConnectButton";
            this.noLoadConnectButton.Size = new System.Drawing.Size(183, 19);
            this.noLoadConnectButton.TabIndex = 0;
            this.noLoadConnectButton.TabStop = true;
            this.noLoadConnectButton.Tag = "None";
            this.noLoadConnectButton.Text = "Do not make this connection";
            this.noLoadConnectButton.UseVisualStyleBackColor = true;
            // 
            // loadConnectButton
            // 
            this.loadConnectButton.AutoSize = true;
            this.loadConnectButton.Location = new System.Drawing.Point(3, 28);
            this.loadConnectButton.Name = "loadConnectButton";
            this.loadConnectButton.Size = new System.Drawing.Size(159, 19);
            this.loadConnectButton.TabIndex = 0;
            this.loadConnectButton.Tag = "Connect";
            this.loadConnectButton.Text = "🗲 Make this connection";
            this.loadConnectButton.UseVisualStyleBackColor = true;
            // 
            // loadGroupBox
            // 
            this.loadGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadGroupBox.AutoSize = true;
            this.loadGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loadGroupBox.Controls.Add(this.loadTableLayoutPanel);
            this.loadGroupBox.Location = new System.Drawing.Point(3, 166);
            this.loadGroupBox.Name = "loadGroupBox";
            this.loadGroupBox.Size = new System.Drawing.Size(363, 94);
            this.loadGroupBox.TabIndex = 1;
            this.loadGroupBox.TabStop = false;
            this.loadGroupBox.Text = "When profile is loaded";
            // 
            // loadTableLayoutPanel
            // 
            this.loadTableLayoutPanel.AutoSize = true;
            this.loadTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loadTableLayoutPanel.ColumnCount = 1;
            this.loadTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.loadTableLayoutPanel.Controls.Add(this.loadReconnectButton, 0, 2);
            this.loadTableLayoutPanel.Controls.Add(this.loadConnectButton, 0, 1);
            this.loadTableLayoutPanel.Controls.Add(this.noLoadConnectButton, 0, 0);
            this.loadTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.loadTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.loadTableLayoutPanel.Name = "loadTableLayoutPanel";
            this.loadTableLayoutPanel.RowCount = 3;
            this.loadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.loadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.loadTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.loadTableLayoutPanel.Size = new System.Drawing.Size(357, 75);
            this.loadTableLayoutPanel.TabIndex = 0;
            // 
            // loadReconnectButton
            // 
            this.loadReconnectButton.AutoSize = true;
            this.loadReconnectButton.Location = new System.Drawing.Point(3, 53);
            this.loadReconnectButton.Name = "loadReconnectButton";
            this.loadReconnectButton.Size = new System.Drawing.Size(208, 19);
            this.loadReconnectButton.TabIndex = 1;
            this.loadReconnectButton.Tag = "Reconnect";
            this.loadReconnectButton.Text = "🗲+ Keep making this connection";
            this.loadReconnectButton.UseVisualStyleBackColor = true;
            // 
            // hostTypeGroupBox
            // 
            this.hostTypeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hostTypeGroupBox.AutoSize = true;
            this.hostTypeGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hostTypeGroupBox.Controls.Add(this.hostTypeTableLayoutPanel);
            this.hostTypeGroupBox.Location = new System.Drawing.Point(3, 266);
            this.hostTypeGroupBox.Name = "hostTypeGroupBox";
            this.hostTypeGroupBox.Size = new System.Drawing.Size(363, 69);
            this.hostTypeGroupBox.TabIndex = 2;
            this.hostTypeGroupBox.TabStop = false;
            this.hostTypeGroupBox.Text = "Host type for file transfers";
            // 
            // hostTypeTableLayoutPanel
            // 
            this.hostTypeTableLayoutPanel.AutoSize = true;
            this.hostTypeTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hostTypeTableLayoutPanel.ColumnCount = 2;
            this.hostTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.hostTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.hostTypeTableLayoutPanel.Controls.Add(this.cicsRadioButton, 1, 1);
            this.hostTypeTableLayoutPanel.Controls.Add(this.unspecifiedRadioButton, 0, 0);
            this.hostTypeTableLayoutPanel.Controls.Add(this.vmRadioButton, 1, 0);
            this.hostTypeTableLayoutPanel.Controls.Add(this.tsoRadioButton, 0, 1);
            this.hostTypeTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.hostTypeTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.hostTypeTableLayoutPanel.Name = "hostTypeTableLayoutPanel";
            this.hostTypeTableLayoutPanel.RowCount = 2;
            this.hostTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.hostTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.hostTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.hostTypeTableLayoutPanel.Size = new System.Drawing.Size(357, 50);
            this.hostTypeTableLayoutPanel.TabIndex = 0;
            // 
            // cicsRadioButton
            // 
            this.cicsRadioButton.AutoSize = true;
            this.cicsRadioButton.Location = new System.Drawing.Point(181, 28);
            this.cicsRadioButton.Name = "cicsRadioButton";
            this.cicsRadioButton.Size = new System.Drawing.Size(55, 19);
            this.cicsRadioButton.TabIndex = 3;
            this.cicsRadioButton.Tag = "Cics";
            this.cicsRadioButton.Text = "CICS";
            this.cicsRadioButton.UseVisualStyleBackColor = true;
            // 
            // unspecifiedRadioButton
            // 
            this.unspecifiedRadioButton.AutoSize = true;
            this.unspecifiedRadioButton.Checked = true;
            this.unspecifiedRadioButton.Location = new System.Drawing.Point(3, 3);
            this.unspecifiedRadioButton.Name = "unspecifiedRadioButton";
            this.unspecifiedRadioButton.Size = new System.Drawing.Size(93, 19);
            this.unspecifiedRadioButton.TabIndex = 0;
            this.unspecifiedRadioButton.TabStop = true;
            this.unspecifiedRadioButton.Tag = "Unspecified";
            this.unspecifiedRadioButton.Text = "Unspecified";
            this.unspecifiedRadioButton.UseVisualStyleBackColor = true;
            // 
            // vmRadioButton
            // 
            this.vmRadioButton.AutoSize = true;
            this.vmRadioButton.Location = new System.Drawing.Point(181, 3);
            this.vmRadioButton.Name = "vmRadioButton";
            this.vmRadioButton.Size = new System.Drawing.Size(76, 19);
            this.vmRadioButton.TabIndex = 2;
            this.vmRadioButton.Tag = "Vm";
            this.vmRadioButton.Text = "VM/CMS";
            this.vmRadioButton.UseVisualStyleBackColor = true;
            // 
            // tsoRadioButton
            // 
            this.tsoRadioButton.AutoSize = true;
            this.tsoRadioButton.Location = new System.Drawing.Point(3, 28);
            this.tsoRadioButton.Name = "tsoRadioButton";
            this.tsoRadioButton.Size = new System.Drawing.Size(52, 19);
            this.tsoRadioButton.TabIndex = 1;
            this.tsoRadioButton.Tag = "Tso";
            this.tsoRadioButton.Text = "TSO";
            this.tsoRadioButton.UseVisualStyleBackColor = true;
            // 
            // acceptLabel
            // 
            this.acceptLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.acceptLabel.AutoSize = true;
            this.acceptLabel.Location = new System.Drawing.Point(3, 155);
            this.acceptLabel.Name = "acceptLabel";
            this.acceptLabel.Size = new System.Drawing.Size(101, 15);
            this.acceptLabel.TabIndex = 10;
            this.acceptLabel.Text = "Accept hostname";
            // 
            // acceptTextBox
            // 
            this.acceptTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.acceptTextBox.Location = new System.Drawing.Point(124, 153);
            this.acceptTextBox.Name = "acceptTextBox";
            this.acceptTextBox.Size = new System.Drawing.Size(230, 20);
            this.acceptTextBox.TabIndex = 11;
            this.acceptTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.AcceptTextBox_Validating);
            // 
            // rightTableLayoutPanel
            // 
            this.rightTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightTableLayoutPanel.AutoSize = true;
            this.rightTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightTableLayoutPanel.ColumnCount = 1;
            this.rightTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTableLayoutPanel.Controls.Add(this.buttonsTableLayoutPanel, 0, 5);
            this.rightTableLayoutPanel.Controls.Add(this.optionsGroupBox, 0, 0);
            this.rightTableLayoutPanel.Controls.Add(this.loadGroupBox, 0, 1);
            this.rightTableLayoutPanel.Controls.Add(this.hostTypeGroupBox, 0, 2);
            this.rightTableLayoutPanel.Controls.Add(this.printerSessionGroupBox, 0, 3);
            this.rightTableLayoutPanel.Controls.Add(this.localProcessInputGroupBox, 0, 4);
            this.rightTableLayoutPanel.Location = new System.Drawing.Point(369, 0);
            this.rightTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightTableLayoutPanel.Name = "rightTableLayoutPanel";
            this.rightTableLayoutPanel.RowCount = 6;
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightTableLayoutPanel.Size = new System.Drawing.Size(369, 578);
            this.rightTableLayoutPanel.TabIndex = 1;
            // 
            // buttonsTableLayoutPanel
            // 
            this.buttonsTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsTableLayoutPanel.AutoSize = true;
            this.buttonsTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonsTableLayoutPanel.ColumnCount = 5;
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsTableLayoutPanel.Controls.Add(this.connectButton, 0, 0);
            this.buttonsTableLayoutPanel.Controls.Add(this.cancelButton, 2, 0);
            this.buttonsTableLayoutPanel.Controls.Add(this.okButton, 3, 0);
            this.buttonsTableLayoutPanel.Controls.Add(this.helpPictureBox, 4, 0);
            this.buttonsTableLayoutPanel.Controls.Add(this.connectRecordButton, 1, 0);
            this.buttonsTableLayoutPanel.Location = new System.Drawing.Point(3, 567);
            this.buttonsTableLayoutPanel.Name = "buttonsTableLayoutPanel";
            this.buttonsTableLayoutPanel.RowCount = 1;
            this.buttonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buttonsTableLayoutPanel.Size = new System.Drawing.Size(363, 32);
            this.buttonsTableLayoutPanel.TabIndex = 5;
            // 
            // connectButton
            // 
            this.connectButton.AutoSize = true;
            this.connectButton.ForeColor = System.Drawing.Color.ForestGreen;
            this.connectButton.Location = new System.Drawing.Point(3, 3);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 26);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.toolTip1.SetToolTip(this.connectButton, "Save all changes and connect");
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AutoSize = true;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(175, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 26);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Discard all changes");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.AutoSize = true;
            this.okButton.Location = new System.Drawing.Point(256, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 26);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "Save";
            this.toolTip1.SetToolTip(this.okButton, "Save all changes");
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.ContextMenuStrip = this.helpContextMenuStrip;
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(337, 3);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(23, 23);
            this.helpPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.helpPictureBox.TabIndex = 132;
            this.helpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.helpPictureBox, "Get help");
            this.helpPictureBox.Click += new System.EventHandler(this.Help_Click);
            // 
            // helpContextMenuStrip
            // 
            this.helpContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.helpContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayHelpInBrowserToolStripMenuItem,
            this.startTourToolStripMenuItem});
            this.helpContextMenuStrip.Name = "helpContextMenuStrip";
            this.helpContextMenuStrip.Size = new System.Drawing.Size(234, 52);
            // 
            // displayHelpInBrowserToolStripMenuItem
            // 
            this.displayHelpInBrowserToolStripMenuItem.Name = "displayHelpInBrowserToolStripMenuItem";
            this.displayHelpInBrowserToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
            this.displayHelpInBrowserToolStripMenuItem.Tag = "Help";
            this.displayHelpInBrowserToolStripMenuItem.Text = "Display help in browser";
            this.displayHelpInBrowserToolStripMenuItem.Click += new System.EventHandler(this.HelpMenuClick);
            // 
            // startTourToolStripMenuItem
            // 
            this.startTourToolStripMenuItem.Name = "startTourToolStripMenuItem";
            this.startTourToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
            this.startTourToolStripMenuItem.Tag = "Tour";
            this.startTourToolStripMenuItem.Text = "Start tour";
            this.startTourToolStripMenuItem.Click += new System.EventHandler(this.HelpMenuClick);
            // 
            // connectRecordButton
            // 
            this.connectRecordButton.AutoSize = true;
            this.connectRecordButton.ForeColor = System.Drawing.Color.ForestGreen;
            this.connectRecordButton.Location = new System.Drawing.Point(84, 3);
            this.connectRecordButton.Name = "connectRecordButton";
            this.connectRecordButton.Size = new System.Drawing.Size(85, 26);
            this.connectRecordButton.TabIndex = 1;
            this.connectRecordButton.Text = "Connect+⏺";
            this.toolTip1.SetToolTip(this.connectRecordButton, "Connect and start recording a login macro");
            this.connectRecordButton.UseVisualStyleBackColor = true;
            this.connectRecordButton.Click += new System.EventHandler(this.ConnectRecordButton_Click);
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.AutoSize = true;
            this.optionsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.optionsGroupBox.Controls.Add(this.optionsLayoutPanel);
            this.optionsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(363, 157);
            this.optionsGroupBox.TabIndex = 0;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Options";
            // 
            // optionsLayoutPanel
            // 
            this.optionsLayoutPanel.ColumnCount = 2;
            this.optionsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.optionsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.optionsLayoutPanel.Controls.Add(this.tlsTunnelCheckBox, 0, 0);
            this.optionsLayoutPanel.Controls.Add(this.starttlsCheckBox, 0, 1);
            this.optionsLayoutPanel.Controls.Add(this.nvtCheckBox, 0, 3);
            this.optionsLayoutPanel.Controls.Add(this.verifyCertCheckBox, 0, 2);
            this.optionsLayoutPanel.Controls.Add(this.tn3270eCheckBox, 0, 4);
            this.optionsLayoutPanel.Controls.Add(this.telnetCheckBox, 1, 0);
            this.optionsLayoutPanel.Controls.Add(this.loginScreenCheckBox, 0, 6);
            this.optionsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
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
            this.optionsLayoutPanel.Size = new System.Drawing.Size(357, 138);
            this.optionsLayoutPanel.TabIndex = 0;
            // 
            // tlsTunnelCheckBox
            // 
            this.tlsTunnelCheckBox.AutoSize = true;
            this.tlsTunnelCheckBox.Location = new System.Drawing.Point(3, 3);
            this.tlsTunnelCheckBox.Name = "tlsTunnelCheckBox";
            this.tlsTunnelCheckBox.Size = new System.Drawing.Size(127, 19);
            this.tlsTunnelCheckBox.TabIndex = 0;
            this.tlsTunnelCheckBox.Tag = "L:";
            this.tlsTunnelCheckBox.Text = "Create TLS tunnel";
            this.tlsTunnelCheckBox.UseVisualStyleBackColor = true;
            this.tlsTunnelCheckBox.CheckedChanged += new System.EventHandler(this.TlsCheckBoxCheckedChanged);
            // 
            // starttlsCheckBox
            // 
            this.starttlsCheckBox.AutoSize = true;
            this.starttlsCheckBox.Checked = true;
            this.starttlsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.starttlsCheckBox.Location = new System.Drawing.Point(3, 28);
            this.starttlsCheckBox.Name = "starttlsCheckBox";
            this.starttlsCheckBox.Size = new System.Drawing.Size(128, 19);
            this.starttlsCheckBox.TabIndex = 1;
            this.starttlsCheckBox.Text = "Accept STARTTLS";
            this.starttlsCheckBox.UseVisualStyleBackColor = true;
            this.starttlsCheckBox.CheckedChanged += new System.EventHandler(this.TlsCheckBoxCheckedChanged);
            // 
            // nvtCheckBox
            // 
            this.nvtCheckBox.AutoSize = true;
            this.nvtCheckBox.Location = new System.Drawing.Point(3, 78);
            this.nvtCheckBox.Name = "nvtCheckBox";
            this.nvtCheckBox.Size = new System.Drawing.Size(87, 19);
            this.nvtCheckBox.TabIndex = 3;
            this.nvtCheckBox.Tag = "A:";
            this.nvtCheckBox.Text = "NVT mode";
            this.nvtCheckBox.UseVisualStyleBackColor = true;
            // 
            // verifyCertCheckBox
            // 
            this.verifyCertCheckBox.AutoSize = true;
            this.verifyCertCheckBox.Checked = true;
            this.verifyCertCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verifyCertCheckBox.Location = new System.Drawing.Point(3, 53);
            this.verifyCertCheckBox.Name = "verifyCertCheckBox";
            this.verifyCertCheckBox.Size = new System.Drawing.Size(139, 19);
            this.verifyCertCheckBox.TabIndex = 2;
            this.verifyCertCheckBox.Tag = "~Y:";
            this.verifyCertCheckBox.Text = "Verify host certificate";
            this.verifyCertCheckBox.UseVisualStyleBackColor = true;
            this.verifyCertCheckBox.CheckedChanged += new System.EventHandler(this.VerifyCertCheckBox_CheckedChanged);
            // 
            // tn3270eCheckBox
            // 
            this.tn3270eCheckBox.AutoSize = true;
            this.tn3270eCheckBox.Checked = true;
            this.tn3270eCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tn3270eCheckBox.Location = new System.Drawing.Point(3, 103);
            this.tn3270eCheckBox.Name = "tn3270eCheckBox";
            this.tn3270eCheckBox.Size = new System.Drawing.Size(120, 19);
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
            this.telnetCheckBox.Location = new System.Drawing.Point(181, 3);
            this.telnetCheckBox.Name = "telnetCheckBox";
            this.telnetCheckBox.Size = new System.Drawing.Size(100, 19);
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
            this.loginScreenCheckBox.Location = new System.Drawing.Point(3, 128);
            this.loginScreenCheckBox.Name = "loginScreenCheckBox";
            this.loginScreenCheckBox.Size = new System.Drawing.Size(140, 19);
            this.loginScreenCheckBox.TabIndex = 5;
            this.loginScreenCheckBox.Tag = "~C:";
            this.loginScreenCheckBox.Text = "Wait for login screen";
            this.loginScreenCheckBox.UseVisualStyleBackColor = true;
            // 
            // printerSessionGroupBox
            // 
            this.printerSessionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.printerSessionGroupBox.AutoSize = true;
            this.printerSessionGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.printerSessionGroupBox.Controls.Add(this.printerSessionTableLayoutPanel);
            this.printerSessionGroupBox.Location = new System.Drawing.Point(3, 341);
            this.printerSessionGroupBox.Name = "printerSessionGroupBox";
            this.printerSessionGroupBox.Size = new System.Drawing.Size(363, 120);
            this.printerSessionGroupBox.TabIndex = 3;
            this.printerSessionGroupBox.TabStop = false;
            this.printerSessionGroupBox.Text = "3287 printer session";
            this.printerSessionGroupBox.Validating += new System.ComponentModel.CancelEventHandler(this.PrinterSessionGroupBox_Validating);
            // 
            // printerSessionTableLayoutPanel
            // 
            this.printerSessionTableLayoutPanel.AutoSize = true;
            this.printerSessionTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.printerSessionTableLayoutPanel.ColumnCount = 1;
            this.printerSessionTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.printerSessionTableLayoutPanel.Controls.Add(this.specificLuTextBox, 0, 3);
            this.printerSessionTableLayoutPanel.Controls.Add(this.associatePrinterRadioButton, 0, 1);
            this.printerSessionTableLayoutPanel.Controls.Add(this.noPrinterRadioButton, 0, 0);
            this.printerSessionTableLayoutPanel.Controls.Add(this.specificLuRadioButton, 0, 2);
            this.printerSessionTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.printerSessionTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.printerSessionTableLayoutPanel.Name = "printerSessionTableLayoutPanel";
            this.printerSessionTableLayoutPanel.RowCount = 4;
            this.printerSessionTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.printerSessionTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.printerSessionTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.printerSessionTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.printerSessionTableLayoutPanel.Size = new System.Drawing.Size(357, 101);
            this.printerSessionTableLayoutPanel.TabIndex = 0;
            // 
            // specificLuTextBox
            // 
            this.specificLuTextBox.Enabled = false;
            this.specificLuTextBox.Location = new System.Drawing.Point(3, 78);
            this.specificLuTextBox.Name = "specificLuTextBox";
            this.specificLuTextBox.Size = new System.Drawing.Size(233, 20);
            this.specificLuTextBox.TabIndex = 3;
            this.specificLuTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.SpecificLuTextBox_Validating);
            // 
            // associatePrinterRadioButton
            // 
            this.associatePrinterRadioButton.AutoSize = true;
            this.associatePrinterRadioButton.Location = new System.Drawing.Point(3, 28);
            this.associatePrinterRadioButton.Name = "associatePrinterRadioButton";
            this.associatePrinterRadioButton.Size = new System.Drawing.Size(208, 19);
            this.associatePrinterRadioButton.TabIndex = 1;
            this.associatePrinterRadioButton.Tag = "Associate";
            this.associatePrinterRadioButton.Text = "Associate with interactive session";
            this.associatePrinterRadioButton.UseVisualStyleBackColor = true;
            // 
            // noPrinterRadioButton
            // 
            this.noPrinterRadioButton.AutoSize = true;
            this.noPrinterRadioButton.Checked = true;
            this.noPrinterRadioButton.Location = new System.Drawing.Point(3, 3);
            this.noPrinterRadioButton.Name = "noPrinterRadioButton";
            this.noPrinterRadioButton.Size = new System.Drawing.Size(58, 19);
            this.noPrinterRadioButton.TabIndex = 0;
            this.noPrinterRadioButton.TabStop = true;
            this.noPrinterRadioButton.Tag = "None";
            this.noPrinterRadioButton.Text = "None";
            this.noPrinterRadioButton.UseVisualStyleBackColor = true;
            // 
            // specificLuRadioButton
            // 
            this.specificLuRadioButton.AutoSize = true;
            this.specificLuRadioButton.Location = new System.Drawing.Point(3, 53);
            this.specificLuRadioButton.Name = "specificLuRadioButton";
            this.specificLuRadioButton.Size = new System.Drawing.Size(90, 19);
            this.specificLuRadioButton.TabIndex = 2;
            this.specificLuRadioButton.Tag = "SpecificLu";
            this.specificLuRadioButton.Text = "Specific LU";
            this.specificLuRadioButton.UseVisualStyleBackColor = true;
            // 
            // localProcessInputGroupBox
            // 
            this.localProcessInputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localProcessInputGroupBox.AutoSize = true;
            this.localProcessInputGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.localProcessInputGroupBox.Controls.Add(this.localProcessInputTableLayoutPanel);
            this.localProcessInputGroupBox.Enabled = false;
            this.localProcessInputGroupBox.Location = new System.Drawing.Point(3, 467);
            this.localProcessInputGroupBox.Name = "localProcessInputGroupBox";
            this.localProcessInputGroupBox.Size = new System.Drawing.Size(363, 94);
            this.localProcessInputGroupBox.TabIndex = 4;
            this.localProcessInputGroupBox.TabStop = false;
            this.localProcessInputGroupBox.Text = "No-TELNET input mode";
            // 
            // localProcessInputTableLayoutPanel
            // 
            this.localProcessInputTableLayoutPanel.AutoSize = true;
            this.localProcessInputTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.localProcessInputTableLayoutPanel.ColumnCount = 1;
            this.localProcessInputTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.localProcessInputTableLayoutPanel.Controls.Add(this.lineModeRadioButton, 0, 0);
            this.localProcessInputTableLayoutPanel.Controls.Add(this.characterModeRadioButton, 0, 1);
            this.localProcessInputTableLayoutPanel.Controls.Add(this.characterModeCrLfRadioButton, 0, 2);
            this.localProcessInputTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.localProcessInputTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.localProcessInputTableLayoutPanel.Name = "localProcessInputTableLayoutPanel";
            this.localProcessInputTableLayoutPanel.RowCount = 3;
            this.localProcessInputTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.localProcessInputTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.localProcessInputTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.localProcessInputTableLayoutPanel.Size = new System.Drawing.Size(357, 75);
            this.localProcessInputTableLayoutPanel.TabIndex = 0;
            // 
            // lineModeRadioButton
            // 
            this.lineModeRadioButton.AutoSize = true;
            this.lineModeRadioButton.Checked = true;
            this.lineModeRadioButton.Location = new System.Drawing.Point(3, 3);
            this.lineModeRadioButton.Name = "lineModeRadioButton";
            this.lineModeRadioButton.Size = new System.Drawing.Size(52, 19);
            this.lineModeRadioButton.TabIndex = 1;
            this.lineModeRadioButton.TabStop = true;
            this.lineModeRadioButton.Tag = "Line";
            this.lineModeRadioButton.Text = "Line";
            this.lineModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // characterModeRadioButton
            // 
            this.characterModeRadioButton.AutoSize = true;
            this.characterModeRadioButton.Location = new System.Drawing.Point(3, 28);
            this.characterModeRadioButton.Name = "characterModeRadioButton";
            this.characterModeRadioButton.Size = new System.Drawing.Size(81, 19);
            this.characterModeRadioButton.TabIndex = 2;
            this.characterModeRadioButton.Tag = "Character";
            this.characterModeRadioButton.Text = "Character";
            this.characterModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // characterModeCrLfRadioButton
            // 
            this.characterModeCrLfRadioButton.AutoSize = true;
            this.characterModeCrLfRadioButton.Location = new System.Drawing.Point(3, 53);
            this.characterModeCrLfRadioButton.Name = "characterModeCrLfRadioButton";
            this.characterModeCrLfRadioButton.Size = new System.Drawing.Size(203, 19);
            this.characterModeCrLfRadioButton.TabIndex = 2;
            this.characterModeCrLfRadioButton.Tag = "CharacterCrLf";
            this.characterModeCrLfRadioButton.Text = "Character with CR/LF translation";
            this.characterModeCrLfRadioButton.UseVisualStyleBackColor = true;
            // 
            // profileNameLabel
            // 
            this.profileNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileNameLabel.AutoSize = true;
            this.profileNameLabel.Location = new System.Drawing.Point(124, 3);
            this.profileNameLabel.Name = "profileNameLabel";
            this.profileNameLabel.Size = new System.Drawing.Size(83, 15);
            this.profileNameLabel.TabIndex = 1;
            this.profileNameLabel.Text = "`Profile Name";
            // 
            // profileLabel
            // 
            this.profileLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileLabel.AutoSize = true;
            this.profileLabel.Location = new System.Drawing.Point(3, 3);
            this.profileLabel.Margin = new System.Windows.Forms.Padding(3);
            this.profileLabel.Name = "profileLabel";
            this.profileLabel.Size = new System.Drawing.Size(42, 15);
            this.profileLabel.TabIndex = 0;
            this.profileLabel.Text = "Profile";
            // 
            // loginMacroLabel
            // 
            this.loginMacroLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.loginMacroLabel.AutoSize = true;
            this.loginMacroLabel.Location = new System.Drawing.Point(3, 320);
            this.loginMacroLabel.Name = "loginMacroLabel";
            this.loginMacroLabel.Size = new System.Drawing.Size(76, 15);
            this.loginMacroLabel.TabIndex = 18;
            this.loginMacroLabel.Text = "Login macro";
            // 
            // LoginMacroTextBox
            // 
            this.LoginMacroTextBox.AcceptsReturn = true;
            this.LoginMacroTextBox.BackColor = System.Drawing.Color.White;
            this.LoginMacroTextBox.Location = new System.Drawing.Point(124, 265);
            this.LoginMacroTextBox.Multiline = true;
            this.LoginMacroTextBox.Name = "LoginMacroTextBox";
            this.LoginMacroTextBox.ReadOnly = true;
            this.LoginMacroTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LoginMacroTextBox.Size = new System.Drawing.Size(230, 125);
            this.LoginMacroTextBox.TabIndex = 19;
            this.LoginMacroTextBox.Click += new System.EventHandler(this.LoginMacroEditButton_Click);
            this.LoginMacroTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LoginMacroEditButton_Click);
            // 
            // clientCertificateLabel
            // 
            this.clientCertificateLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.clientCertificateLabel.AutoSize = true;
            this.clientCertificateLabel.Location = new System.Drawing.Point(3, 176);
            this.clientCertificateLabel.Name = "clientCertificateLabel";
            this.clientCertificateLabel.Size = new System.Drawing.Size(96, 30);
            this.clientCertificateLabel.TabIndex = 12;
            this.clientCertificateLabel.Text = "Client certificate name";
            // 
            // clientCertTextBox
            // 
            this.clientCertTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.clientCertTextBox.Location = new System.Drawing.Point(124, 179);
            this.clientCertTextBox.Name = "clientCertTextBox";
            this.clientCertTextBox.Size = new System.Drawing.Size(230, 20);
            this.clientCertTextBox.TabIndex = 13;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(3, 398);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(69, 15);
            this.descriptionLabel.TabIndex = 20;
            this.descriptionLabel.Text = "Description";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(124, 396);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(230, 20);
            this.descriptionTextBox.TabIndex = 21;
            this.toolTip1.SetToolTip(this.descriptionTextBox, "Description of this host\r\nShown in the Profiles and Connections window");
            // 
            // windowTitleLabel
            // 
            this.windowTitleLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.windowTitleLabel.AutoSize = true;
            this.windowTitleLabel.Location = new System.Drawing.Point(3, 424);
            this.windowTitleLabel.Name = "windowTitleLabel";
            this.windowTitleLabel.Size = new System.Drawing.Size(73, 15);
            this.windowTitleLabel.TabIndex = 22;
            this.windowTitleLabel.Text = "Window title";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleTextBox.Location = new System.Drawing.Point(124, 422);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(230, 20);
            this.titleTextBox.TabIndex = 23;
            this.toolTip1.SetToolTip(this.titleTextBox, "Window title override");
            // 
            // commandTextBox
            // 
            this.commandTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.commandTextBox.Enabled = false;
            this.commandTextBox.Location = new System.Drawing.Point(124, 209);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.ReadOnly = true;
            this.commandTextBox.Size = new System.Drawing.Size(230, 20);
            this.commandTextBox.TabIndex = 15;
            this.toolTip1.SetToolTip(this.commandTextBox, "Command to run");
            this.commandTextBox.Click += new System.EventHandler(this.CommandTextBox_Click);
            this.commandTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CommandTextBox_Click);
            // 
            // commandLineOptionsTextBox
            // 
            this.commandLineOptionsTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.commandLineOptionsTextBox.Enabled = false;
            this.commandLineOptionsTextBox.Location = new System.Drawing.Point(124, 235);
            this.commandLineOptionsTextBox.Name = "commandLineOptionsTextBox";
            this.commandLineOptionsTextBox.Size = new System.Drawing.Size(230, 20);
            this.commandLineOptionsTextBox.TabIndex = 17;
            this.toolTip1.SetToolTip(this.commandLineOptionsTextBox, "Command-line options");
            // 
            // connectionTypeGroupBox
            // 
            this.connectionTypeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionTypeGroupBox.AutoSize = true;
            this.connectionTypeGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.connectionTypeGroupBox.Controls.Add(this.connectionTypeTableLayoutPanel);
            this.connectionTypeGroupBox.Location = new System.Drawing.Point(3, 3);
            this.connectionTypeGroupBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.connectionTypeGroupBox.Name = "connectionTypeGroupBox";
            this.connectionTypeGroupBox.Size = new System.Drawing.Size(363, 69);
            this.connectionTypeGroupBox.TabIndex = 0;
            this.connectionTypeGroupBox.TabStop = false;
            this.connectionTypeGroupBox.Text = "Connection type";
            // 
            // connectionTypeTableLayoutPanel
            // 
            this.connectionTypeTableLayoutPanel.AutoSize = true;
            this.connectionTypeTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.connectionTypeTableLayoutPanel.ColumnCount = 1;
            this.connectionTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.connectionTypeTableLayoutPanel.Controls.Add(this.hostRadioButton, 0, 0);
            this.connectionTypeTableLayoutPanel.Controls.Add(this.localProcessRadioButton, 0, 1);
            this.connectionTypeTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionTypeTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.connectionTypeTableLayoutPanel.Name = "connectionTypeTableLayoutPanel";
            this.connectionTypeTableLayoutPanel.RowCount = 2;
            this.connectionTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.connectionTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.connectionTypeTableLayoutPanel.Size = new System.Drawing.Size(357, 50);
            this.connectionTypeTableLayoutPanel.TabIndex = 0;
            // 
            // hostRadioButton
            // 
            this.hostRadioButton.AutoSize = true;
            this.hostRadioButton.Checked = true;
            this.hostRadioButton.Location = new System.Drawing.Point(3, 3);
            this.hostRadioButton.Name = "hostRadioButton";
            this.hostRadioButton.Size = new System.Drawing.Size(53, 19);
            this.hostRadioButton.TabIndex = 0;
            this.hostRadioButton.TabStop = true;
            this.hostRadioButton.Tag = "Host";
            this.hostRadioButton.Text = "Host";
            this.hostRadioButton.UseVisualStyleBackColor = true;
            // 
            // localProcessRadioButton
            // 
            this.localProcessRadioButton.AutoSize = true;
            this.localProcessRadioButton.Location = new System.Drawing.Point(3, 28);
            this.localProcessRadioButton.Name = "localProcessRadioButton";
            this.localProcessRadioButton.Size = new System.Drawing.Size(104, 19);
            this.localProcessRadioButton.TabIndex = 1;
            this.localProcessRadioButton.Tag = "LocalProcess";
            this.localProcessRadioButton.Text = "Local process";
            this.localProcessRadioButton.UseVisualStyleBackColor = true;
            // 
            // overallTableLayoutPanel
            // 
            this.overallTableLayoutPanel.ColumnCount = 2;
            this.overallTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.overallTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.overallTableLayoutPanel.Controls.Add(this.rightTableLayoutPanel, 1, 0);
            this.overallTableLayoutPanel.Controls.Add(this.leftTableLayoutPanel, 0, 0);
            this.overallTableLayoutPanel.Location = new System.Drawing.Point(3, 100);
            this.overallTableLayoutPanel.Name = "overallTableLayoutPanel";
            this.overallTableLayoutPanel.RowCount = 1;
            this.overallTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.overallTableLayoutPanel.Size = new System.Drawing.Size(738, 578);
            this.overallTableLayoutPanel.TabIndex = 0;
            // 
            // leftTableLayoutPanel
            // 
            this.leftTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leftTableLayoutPanel.AutoSize = true;
            this.leftTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftTableLayoutPanel.ColumnCount = 1;
            this.leftTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftTableLayoutPanel.Controls.Add(this.basicParametersGroupBox, 0, 1);
            this.leftTableLayoutPanel.Controls.Add(this.connectionTypeGroupBox, 0, 0);
            this.leftTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.leftTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftTableLayoutPanel.Name = "leftTableLayoutPanel";
            this.leftTableLayoutPanel.RowCount = 2;
            this.leftTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.leftTableLayoutPanel.Size = new System.Drawing.Size(369, 549);
            this.leftTableLayoutPanel.TabIndex = 0;
            // 
            // basicParametersGroupBox
            // 
            this.basicParametersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.basicParametersGroupBox.AutoSize = true;
            this.basicParametersGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.basicParametersGroupBox.Controls.Add(this.basicParametersTableLayoutPanel);
            this.basicParametersGroupBox.Location = new System.Drawing.Point(3, 90);
            this.basicParametersGroupBox.Name = "basicParametersGroupBox";
            this.basicParametersGroupBox.Size = new System.Drawing.Size(363, 456);
            this.basicParametersGroupBox.TabIndex = 1;
            this.basicParametersGroupBox.TabStop = false;
            this.basicParametersGroupBox.Text = "Basic parameters";
            // 
            // basicParametersTableLayoutPanel
            // 
            this.basicParametersTableLayoutPanel.AutoSize = true;
            this.basicParametersTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.basicParametersTableLayoutPanel.ColumnCount = 2;
            this.basicParametersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.basicParametersTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66F));
            this.basicParametersTableLayoutPanel.Controls.Add(this.commandLineOptionsTextBox, 1, 8);
            this.basicParametersTableLayoutPanel.Controls.Add(this.commandTextBox, 1, 7);
            this.basicParametersTableLayoutPanel.Controls.Add(this.profileLabel, 0, 0);
            this.basicParametersTableLayoutPanel.Controls.Add(this.LuNamesTextBox, 1, 4);
            this.basicParametersTableLayoutPanel.Controls.Add(this.commandLineOptionsLabel, 0, 8);
            this.basicParametersTableLayoutPanel.Controls.Add(this.profileNameLabel, 1, 0);
            this.basicParametersTableLayoutPanel.Controls.Add(this.acceptTextBox, 1, 5);
            this.basicParametersTableLayoutPanel.Controls.Add(this.sessionNameLabel, 0, 1);
            this.basicParametersTableLayoutPanel.Controls.Add(this.commandLabel, 0, 7);
            this.basicParametersTableLayoutPanel.Controls.Add(this.clientCertTextBox, 1, 6);
            this.basicParametersTableLayoutPanel.Controls.Add(this.clientCertificateLabel, 0, 6);
            this.basicParametersTableLayoutPanel.Controls.Add(this.acceptLabel, 0, 5);
            this.basicParametersTableLayoutPanel.Controls.Add(this.HostNameTextBox, 1, 2);
            this.basicParametersTableLayoutPanel.Controls.Add(this.connectionNameTextBox, 1, 1);
            this.basicParametersTableLayoutPanel.Controls.Add(this.portLabel, 0, 3);
            this.basicParametersTableLayoutPanel.Controls.Add(this.luNamesLabel, 0, 4);
            this.basicParametersTableLayoutPanel.Controls.Add(this.hostNameLabel, 0, 2);
            this.basicParametersTableLayoutPanel.Controls.Add(this.PortTextBox, 1, 3);
            this.basicParametersTableLayoutPanel.Controls.Add(this.windowTitleLabel, 0, 11);
            this.basicParametersTableLayoutPanel.Controls.Add(this.titleTextBox, 1, 11);
            this.basicParametersTableLayoutPanel.Controls.Add(this.descriptionLabel, 0, 10);
            this.basicParametersTableLayoutPanel.Controls.Add(this.descriptionTextBox, 1, 10);
            this.basicParametersTableLayoutPanel.Controls.Add(this.loginMacroLabel, 0, 9);
            this.basicParametersTableLayoutPanel.Controls.Add(this.LoginMacroTextBox, 1, 9);
            this.basicParametersTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.basicParametersTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.basicParametersTableLayoutPanel.Name = "basicParametersTableLayoutPanel";
            this.basicParametersTableLayoutPanel.RowCount = 12;
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.basicParametersTableLayoutPanel.Size = new System.Drawing.Size(357, 445);
            this.basicParametersTableLayoutPanel.TabIndex = 0;
            // 
            // commandLineOptionsLabel
            // 
            this.commandLineOptionsLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.commandLineOptionsLabel.AutoSize = true;
            this.commandLineOptionsLabel.Enabled = false;
            this.commandLineOptionsLabel.Location = new System.Drawing.Point(3, 232);
            this.commandLineOptionsLabel.Name = "commandLineOptionsLabel";
            this.commandLineOptionsLabel.Size = new System.Drawing.Size(92, 30);
            this.commandLineOptionsLabel.TabIndex = 16;
            this.commandLineOptionsLabel.Text = "Command-line options";
            // 
            // commandLabel
            // 
            this.commandLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.commandLabel.AutoSize = true;
            this.commandLabel.Enabled = false;
            this.commandLabel.Location = new System.Drawing.Point(3, 211);
            this.commandLabel.Name = "commandLabel";
            this.commandLabel.Size = new System.Drawing.Size(70, 15);
            this.commandLabel.TabIndex = 14;
            this.commandLabel.Text = "Command*";
            // 
            // commandOpenFileDialog
            // 
            this.commandOpenFileDialog.DefaultExt = "exe";
            this.commandOpenFileDialog.Filter = "Programs|*.exe|All files|*.*";
            this.commandOpenFileDialog.Title = "Select command";
            // 
            // newProfileRadioButton
            // 
            this.newProfileRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.newProfileRadioButton.AutoSize = true;
            this.newProfileRadioButton.Location = new System.Drawing.Point(2, 41);
            this.newProfileRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.newProfileRadioButton.Name = "newProfileRadioButton";
            this.newProfileRadioButton.Size = new System.Drawing.Size(127, 19);
            this.newProfileRadioButton.TabIndex = 9;
            this.newProfileRadioButton.TabStop = true;
            this.newProfileRadioButton.Tag = "CreateNewProfile";
            this.newProfileRadioButton.Text = "Create new profile";
            this.newProfileRadioButton.UseVisualStyleBackColor = true;
            // 
            // addToProfileRadioButton
            // 
            this.addToProfileRadioButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addToProfileRadioButton.AutoSize = true;
            this.addToProfileRadioButton.Location = new System.Drawing.Point(2, 17);
            this.addToProfileRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.addToProfileRadioButton.Name = "addToProfileRadioButton";
            this.addToProfileRadioButton.Size = new System.Drawing.Size(144, 19);
            this.addToProfileRadioButton.TabIndex = 4;
            this.addToProfileRadioButton.TabStop = true;
            this.addToProfileRadioButton.Tag = "AddToExistingProfile";
            this.addToProfileRadioButton.Text = "Add to existing profile";
            this.addToProfileRadioButton.UseVisualStyleBackColor = true;
            // 
            // profileTableLayoutPanel
            // 
            this.profileTableLayoutPanel.AutoSize = true;
            this.profileTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.profileTableLayoutPanel.ColumnCount = 5;
            this.profileTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.profileTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.profileTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.profileTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.profileTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.profileTableLayoutPanel.Controls.Add(this.modelLabel, 2, 0);
            this.profileTableLayoutPanel.Controls.Add(this.rowsLabel, 3, 0);
            this.profileTableLayoutPanel.Controls.Add(this.newProfileRadioButton, 0, 2);
            this.profileTableLayoutPanel.Controls.Add(this.columnsLabel, 4, 0);
            this.profileTableLayoutPanel.Controls.Add(this.addToProfileRadioButton, 0, 1);
            this.profileTableLayoutPanel.Controls.Add(this.profileNameHeaderLabel, 1, 0);
            this.profileTableLayoutPanel.Controls.Add(this.addToProfileNameLabel, 1, 1);
            this.profileTableLayoutPanel.Controls.Add(this.newProfileNameTextBox, 1, 2);
            this.profileTableLayoutPanel.Controls.Add(this.rowsUpDown, 3, 2);
            this.profileTableLayoutPanel.Controls.Add(this.columnsUpDown, 4, 2);
            this.profileTableLayoutPanel.Controls.Add(this.modelComboBox, 2, 2);
            this.profileTableLayoutPanel.Controls.Add(this.addToProfileModelLabel, 2, 1);
            this.profileTableLayoutPanel.Controls.Add(this.addToProfileRowsLabel, 3, 1);
            this.profileTableLayoutPanel.Controls.Add(this.addToProfileColumnsLabel, 4, 1);
            this.profileTableLayoutPanel.Location = new System.Drawing.Point(4, 17);
            this.profileTableLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.profileTableLayoutPanel.Name = "profileTableLayoutPanel";
            this.profileTableLayoutPanel.RowCount = 3;
            this.profileTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.profileTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.profileTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.profileTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.profileTableLayoutPanel.Size = new System.Drawing.Size(564, 63);
            this.profileTableLayoutPanel.TabIndex = 0;
            // 
            // modelLabel
            // 
            this.modelLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.modelLabel.AutoSize = true;
            this.modelLabel.Location = new System.Drawing.Point(385, 0);
            this.modelLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.modelLabel.Name = "modelLabel";
            this.modelLabel.Size = new System.Drawing.Size(73, 15);
            this.modelLabel.TabIndex = 1;
            this.modelLabel.Text = "3270 model";
            // 
            // rowsLabel
            // 
            this.rowsLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rowsLabel.AutoSize = true;
            this.rowsLabel.Location = new System.Drawing.Point(462, 0);
            this.rowsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.rowsLabel.Name = "rowsLabel";
            this.rowsLabel.Size = new System.Drawing.Size(38, 15);
            this.rowsLabel.TabIndex = 2;
            this.rowsLabel.Text = "Rows";
            // 
            // columnsLabel
            // 
            this.columnsLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.columnsLabel.AutoSize = true;
            this.columnsLabel.Location = new System.Drawing.Point(504, 0);
            this.columnsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.columnsLabel.Name = "columnsLabel";
            this.columnsLabel.Size = new System.Drawing.Size(56, 15);
            this.columnsLabel.TabIndex = 3;
            this.columnsLabel.Text = "Columns";
            // 
            // profileNameHeaderLabel
            // 
            this.profileNameHeaderLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileNameHeaderLabel.AutoSize = true;
            this.profileNameHeaderLabel.Location = new System.Drawing.Point(150, 0);
            this.profileNameHeaderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.profileNameHeaderLabel.Name = "profileNameHeaderLabel";
            this.profileNameHeaderLabel.Size = new System.Drawing.Size(77, 15);
            this.profileNameHeaderLabel.TabIndex = 0;
            this.profileNameHeaderLabel.Text = "Profile name";
            // 
            // addToProfileNameLabel
            // 
            this.addToProfileNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addToProfileNameLabel.AutoSize = true;
            this.addToProfileNameLabel.Location = new System.Drawing.Point(150, 19);
            this.addToProfileNameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.addToProfileNameLabel.Name = "addToProfileNameLabel";
            this.addToProfileNameLabel.Size = new System.Drawing.Size(80, 15);
            this.addToProfileNameLabel.TabIndex = 5;
            this.addToProfileNameLabel.Text = "`profile name";
            // 
            // newProfileNameTextBox
            // 
            this.newProfileNameTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.newProfileNameTextBox.Enabled = false;
            this.newProfileNameTextBox.Location = new System.Drawing.Point(150, 40);
            this.newProfileNameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.newProfileNameTextBox.Name = "newProfileNameTextBox";
            this.newProfileNameTextBox.Size = new System.Drawing.Size(231, 20);
            this.newProfileNameTextBox.TabIndex = 10;
            this.newProfileNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.NewProfileValidating);
            // 
            // rowsUpDown
            // 
            this.rowsUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rowsUpDown.Enabled = false;
            this.rowsUpDown.Location = new System.Drawing.Point(462, 40);
            this.rowsUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.rowsUpDown.Name = "rowsUpDown";
            this.rowsUpDown.Size = new System.Drawing.Size(38, 20);
            this.rowsUpDown.TabIndex = 12;
            this.rowsUpDown.Tag = "Rows";
            this.rowsUpDown.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.rowsUpDown.ValueChanged += new System.EventHandler(this.RowsColsValueChanged);
            // 
            // columnsUpDown
            // 
            this.columnsUpDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.columnsUpDown.Enabled = false;
            this.columnsUpDown.Location = new System.Drawing.Point(514, 40);
            this.columnsUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.columnsUpDown.Name = "columnsUpDown";
            this.columnsUpDown.Size = new System.Drawing.Size(38, 20);
            this.columnsUpDown.TabIndex = 13;
            this.columnsUpDown.Tag = "Columns";
            this.columnsUpDown.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.columnsUpDown.ValueChanged += new System.EventHandler(this.RowsColsValueChanged);
            // 
            // modelComboBox
            // 
            this.modelComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.modelComboBox.Enabled = false;
            this.modelComboBox.FormattingEnabled = true;
            this.modelComboBox.Location = new System.Drawing.Point(385, 40);
            this.modelComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.modelComboBox.Name = "modelComboBox";
            this.modelComboBox.Size = new System.Drawing.Size(38, 21);
            this.modelComboBox.TabIndex = 11;
            this.modelComboBox.SelectedIndexChanged += new System.EventHandler(this.ModelIndexChanged);
            // 
            // addToProfileModelLabel
            // 
            this.addToProfileModelLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addToProfileModelLabel.AutoSize = true;
            this.addToProfileModelLabel.Location = new System.Drawing.Point(385, 19);
            this.addToProfileModelLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.addToProfileModelLabel.Name = "addToProfileModelLabel";
            this.addToProfileModelLabel.Size = new System.Drawing.Size(46, 15);
            this.addToProfileModelLabel.TabIndex = 6;
            this.addToProfileModelLabel.Text = "`model";
            // 
            // addToProfileRowsLabel
            // 
            this.addToProfileRowsLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addToProfileRowsLabel.AutoSize = true;
            this.addToProfileRowsLabel.Location = new System.Drawing.Point(462, 19);
            this.addToProfileRowsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.addToProfileRowsLabel.Name = "addToProfileRowsLabel";
            this.addToProfileRowsLabel.Size = new System.Drawing.Size(37, 15);
            this.addToProfileRowsLabel.TabIndex = 7;
            this.addToProfileRowsLabel.Text = "`rows";
            // 
            // addToProfileColumnsLabel
            // 
            this.addToProfileColumnsLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addToProfileColumnsLabel.AutoSize = true;
            this.addToProfileColumnsLabel.Location = new System.Drawing.Point(504, 19);
            this.addToProfileColumnsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.addToProfileColumnsLabel.Name = "addToProfileColumnsLabel";
            this.addToProfileColumnsLabel.Size = new System.Drawing.Size(58, 15);
            this.addToProfileColumnsLabel.TabIndex = 8;
            this.addToProfileColumnsLabel.Text = "`columns";
            // 
            // profileGroupBox
            // 
            this.profileGroupBox.Controls.Add(this.profileTableLayoutPanel);
            this.profileGroupBox.Location = new System.Drawing.Point(2, 2);
            this.profileGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.profileGroupBox.Name = "profileGroupBox";
            this.profileGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.profileGroupBox.Size = new System.Drawing.Size(736, 93);
            this.profileGroupBox.TabIndex = 0;
            this.profileGroupBox.TabStop = false;
            this.profileGroupBox.Text = "Profile";
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 1;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.Controls.Add(this.profileGroupBox, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.overallTableLayoutPanel, 0, 1);
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(9, 10);
            this.outerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 2;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(744, 681);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // HostEditor
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1806, 1212);
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HostEditor";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection Editor";
            this.toolTip1.SetToolTip(this, "Save and make the connection");
            this.Activated += new System.EventHandler(this.ConnectionEditorActivated);
            this.loadGroupBox.ResumeLayout(false);
            this.loadGroupBox.PerformLayout();
            this.loadTableLayoutPanel.ResumeLayout(false);
            this.loadTableLayoutPanel.PerformLayout();
            this.hostTypeGroupBox.ResumeLayout(false);
            this.hostTypeGroupBox.PerformLayout();
            this.hostTypeTableLayoutPanel.ResumeLayout(false);
            this.hostTypeTableLayoutPanel.PerformLayout();
            this.rightTableLayoutPanel.ResumeLayout(false);
            this.rightTableLayoutPanel.PerformLayout();
            this.buttonsTableLayoutPanel.ResumeLayout(false);
            this.buttonsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.helpContextMenuStrip.ResumeLayout(false);
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsLayoutPanel.ResumeLayout(false);
            this.optionsLayoutPanel.PerformLayout();
            this.printerSessionGroupBox.ResumeLayout(false);
            this.printerSessionGroupBox.PerformLayout();
            this.printerSessionTableLayoutPanel.ResumeLayout(false);
            this.printerSessionTableLayoutPanel.PerformLayout();
            this.localProcessInputGroupBox.ResumeLayout(false);
            this.localProcessInputGroupBox.PerformLayout();
            this.localProcessInputTableLayoutPanel.ResumeLayout(false);
            this.localProcessInputTableLayoutPanel.PerformLayout();
            this.connectionTypeGroupBox.ResumeLayout(false);
            this.connectionTypeGroupBox.PerformLayout();
            this.connectionTypeTableLayoutPanel.ResumeLayout(false);
            this.connectionTypeTableLayoutPanel.PerformLayout();
            this.overallTableLayoutPanel.ResumeLayout(false);
            this.overallTableLayoutPanel.PerformLayout();
            this.leftTableLayoutPanel.ResumeLayout(false);
            this.leftTableLayoutPanel.PerformLayout();
            this.basicParametersGroupBox.ResumeLayout(false);
            this.basicParametersGroupBox.PerformLayout();
            this.basicParametersTableLayoutPanel.ResumeLayout(false);
            this.basicParametersTableLayoutPanel.PerformLayout();
            this.profileTableLayoutPanel.ResumeLayout(false);
            this.profileTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rowsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnsUpDown)).EndInit();
            this.profileGroupBox.ResumeLayout(false);
            this.profileGroupBox.PerformLayout();
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox connectionNameTextBox;
        private System.Windows.Forms.Label sessionNameLabel;
        private System.Windows.Forms.TextBox LuNamesTextBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.TextBox HostNameTextBox;
        private System.Windows.Forms.Label luNamesLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label hostNameLabel;
        private System.Windows.Forms.RadioButton noLoadConnectButton;
        private System.Windows.Forms.RadioButton loadConnectButton;
        private System.Windows.Forms.GroupBox loadGroupBox;
        private System.Windows.Forms.RadioButton loadReconnectButton;
        private System.Windows.Forms.GroupBox hostTypeGroupBox;
        private System.Windows.Forms.RadioButton cicsRadioButton;
        private System.Windows.Forms.RadioButton vmRadioButton;
        private System.Windows.Forms.RadioButton tsoRadioButton;
        private System.Windows.Forms.RadioButton unspecifiedRadioButton;
        private System.Windows.Forms.Label acceptLabel;
        private System.Windows.Forms.TextBox acceptTextBox;
        private System.Windows.Forms.TableLayoutPanel rightTableLayoutPanel;
        private System.Windows.Forms.Label profileLabel;
        private System.Windows.Forms.Label profileNameLabel;
        private System.Windows.Forms.Label loginMacroLabel;
        private System.Windows.Forms.TextBox LoginMacroTextBox;
        private System.Windows.Forms.Label clientCertificateLabel;
        private System.Windows.Forms.TextBox clientCertTextBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox printerSessionGroupBox;
        private System.Windows.Forms.TextBox specificLuTextBox;
        private System.Windows.Forms.RadioButton specificLuRadioButton;
        private System.Windows.Forms.RadioButton associatePrinterRadioButton;
        private System.Windows.Forms.RadioButton noPrinterRadioButton;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label windowTitleLabel;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.GroupBox connectionTypeGroupBox;
        private System.Windows.Forms.TableLayoutPanel connectionTypeTableLayoutPanel;
        private System.Windows.Forms.RadioButton hostRadioButton;
        private System.Windows.Forms.RadioButton localProcessRadioButton;
        private System.Windows.Forms.GroupBox localProcessInputGroupBox;
        private System.Windows.Forms.TableLayoutPanel localProcessInputTableLayoutPanel;
        private System.Windows.Forms.RadioButton lineModeRadioButton;
        private System.Windows.Forms.RadioButton characterModeRadioButton;
        private System.Windows.Forms.RadioButton characterModeCrLfRadioButton;
        private System.Windows.Forms.TableLayoutPanel overallTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel loadTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel hostTypeTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel printerSessionTableLayoutPanel;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.GroupBox basicParametersGroupBox;
        private System.Windows.Forms.TableLayoutPanel basicParametersTableLayoutPanel;
        private System.Windows.Forms.TextBox commandLineOptionsTextBox;
        private System.Windows.Forms.TextBox commandTextBox;
        private System.Windows.Forms.Label commandLabel;
        private System.Windows.Forms.Label commandLineOptionsLabel;
        private System.Windows.Forms.TableLayoutPanel leftTableLayoutPanel;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.TableLayoutPanel optionsLayoutPanel;
        private System.Windows.Forms.CheckBox tlsTunnelCheckBox;
        private System.Windows.Forms.CheckBox starttlsCheckBox;
        private System.Windows.Forms.CheckBox nvtCheckBox;
        private System.Windows.Forms.CheckBox verifyCertCheckBox;
        private System.Windows.Forms.CheckBox tn3270eCheckBox;
        private System.Windows.Forms.CheckBox telnetCheckBox;
        private System.Windows.Forms.CheckBox loginScreenCheckBox;
        private System.Windows.Forms.TableLayoutPanel buttonsTableLayoutPanel;
        private System.Windows.Forms.OpenFileDialog commandOpenFileDialog;
        private System.Windows.Forms.Button connectRecordButton;
        private System.Windows.Forms.RadioButton newProfileRadioButton;
        private System.Windows.Forms.RadioButton addToProfileRadioButton;
        private System.Windows.Forms.TableLayoutPanel profileTableLayoutPanel;
        private System.Windows.Forms.Label modelLabel;
        private System.Windows.Forms.Label rowsLabel;
        private System.Windows.Forms.Label columnsLabel;
        private System.Windows.Forms.Label profileNameHeaderLabel;
        private System.Windows.Forms.Label addToProfileNameLabel;
        private System.Windows.Forms.TextBox newProfileNameTextBox;
        private System.Windows.Forms.NumericUpDown rowsUpDown;
        private System.Windows.Forms.NumericUpDown columnsUpDown;
        private System.Windows.Forms.ComboBox modelComboBox;
        private System.Windows.Forms.Label addToProfileModelLabel;
        private System.Windows.Forms.Label addToProfileRowsLabel;
        private System.Windows.Forms.Label addToProfileColumnsLabel;
        private System.Windows.Forms.GroupBox profileGroupBox;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.ContextMenuStrip helpContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem displayHelpInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTourToolStripMenuItem;
    }
}