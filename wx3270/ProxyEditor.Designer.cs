namespace Wx3270
{
    partial class ProxyEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProxyEditor));
            this.proxyTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.portRequiredLabel = new System.Windows.Forms.Label();
            this.addressRequiredLabel = new System.Windows.Forms.Label();
            this.proxyComboBox = new System.Windows.Forms.ComboBox();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.addressLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.proxyTypeLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.helpContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayHelpInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.proxyTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.helpContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // proxyTableLayoutPanel
            // 
            this.proxyTableLayoutPanel.AutoSize = true;
            this.proxyTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.proxyTableLayoutPanel.ColumnCount = 1;
            this.proxyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.proxyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.proxyTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.proxyTableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.proxyTableLayoutPanel.Location = new System.Drawing.Point(4, 4);
            this.proxyTableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.proxyTableLayoutPanel.Name = "proxyTableLayoutPanel";
            this.proxyTableLayoutPanel.RowCount = 2;
            this.proxyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.proxyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.proxyTableLayoutPanel.Size = new System.Drawing.Size(405, 294);
            this.proxyTableLayoutPanel.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.portRequiredLabel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.addressRequiredLabel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.proxyComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.passwordTextBox, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.usernameTextBox, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.portLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.addressLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.portTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.addressTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.usernameLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.passwordLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.proxyTypeLabel, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(397, 242);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // portRequiredLabel
            // 
            this.portRequiredLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.portRequiredLabel.AutoSize = true;
            this.portRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portRequiredLabel.Location = new System.Drawing.Point(340, 128);
            this.portRequiredLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.portRequiredLabel.Name = "portRequiredLabel";
            this.portRequiredLabel.Size = new System.Drawing.Size(53, 15);
            this.portRequiredLabel.TabIndex = 122;
            this.portRequiredLabel.Text = "required";
            this.portRequiredLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // addressRequiredLabel
            // 
            this.addressRequiredLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addressRequiredLabel.AutoSize = true;
            this.addressRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressRequiredLabel.Location = new System.Drawing.Point(340, 80);
            this.addressRequiredLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressRequiredLabel.Name = "addressRequiredLabel";
            this.addressRequiredLabel.Size = new System.Drawing.Size(53, 15);
            this.addressRequiredLabel.TabIndex = 122;
            this.addressRequiredLabel.Text = "required";
            this.addressRequiredLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // proxyComboBox
            // 
            this.proxyComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.proxyComboBox.FormattingEnabled = true;
            this.proxyComboBox.Location = new System.Drawing.Point(137, 4);
            this.proxyComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.proxyComboBox.Name = "proxyComboBox";
            this.proxyComboBox.Size = new System.Drawing.Size(256, 24);
            this.proxyComboBox.TabIndex = 0;
            this.proxyComboBox.Tag = "type";
            this.proxyComboBox.SelectedIndexChanged += new System.EventHandler(this.ProxyTypeIndexChanged);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordTextBox.Location = new System.Drawing.Point(137, 198);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(256, 22);
            this.passwordTextBox.TabIndex = 4;
            this.passwordTextBox.Tag = "password";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usernameTextBox.Location = new System.Drawing.Point(137, 150);
            this.usernameTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(256, 22);
            this.usernameTextBox.TabIndex = 3;
            this.usernameTextBox.Tag = "username";
            // 
            // portLabel
            // 
            this.portLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(4, 105);
            this.portLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(31, 16);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Port";
            // 
            // addressLabel
            // 
            this.addressLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addressLabel.AutoSize = true;
            this.addressLabel.Location = new System.Drawing.Point(4, 57);
            this.addressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(58, 16);
            this.addressLabel.TabIndex = 0;
            this.addressLabel.Text = "Address";
            // 
            // portTextBox
            // 
            this.portTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portTextBox.Location = new System.Drawing.Point(137, 102);
            this.portTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(256, 22);
            this.portTextBox.TabIndex = 2;
            this.portTextBox.Tag = "port";
            // 
            // addressTextBox
            // 
            this.addressTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addressTextBox.Location = new System.Drawing.Point(137, 54);
            this.addressTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(256, 22);
            this.addressTextBox.TabIndex = 1;
            this.addressTextBox.Tag = "address";
            // 
            // usernameLabel
            // 
            this.usernameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(4, 153);
            this.usernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(70, 16);
            this.usernameLabel.TabIndex = 3;
            this.usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            this.passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(4, 201);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(67, 16);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "Password";
            // 
            // proxyTypeLabel
            // 
            this.proxyTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.proxyTypeLabel.AutoSize = true;
            this.proxyTypeLabel.Location = new System.Drawing.Point(4, 8);
            this.proxyTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.proxyTypeLabel.Name = "proxyTypeLabel";
            this.proxyTypeLabel.Size = new System.Drawing.Size(70, 16);
            this.proxyTypeLabel.TabIndex = 5;
            this.proxyTypeLabel.Text = "Proxy type";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.helpPictureBox);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(146, 254);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(255, 36);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.ContextMenuStrip = this.helpContextMenuStrip;
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(220, 4);
            this.helpPictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(31, 28);
            this.helpPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.helpPictureBox.TabIndex = 9;
            this.helpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.helpPictureBox, "Get help");
            this.helpPictureBox.Click += new System.EventHandler(this.HelpClick);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(112, 4);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 28);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "Save";
            this.toolTip1.SetToolTip(this.okButton, "Save proxy settings");
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(4, 4);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Abandon changes");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
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
            // ProxyEditor
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.proxyTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProxyEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Proxy Editor";
            this.Activated += new System.EventHandler(this.ProxyEditorActivated);
            this.Shown += new System.EventHandler(this.ProxyEditor_Shown);
            this.proxyTableLayoutPanel.ResumeLayout(false);
            this.proxyTableLayoutPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.helpContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel proxyTableLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox proxyComboBox;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label proxyTypeLabel;
        private System.Windows.Forms.Label portRequiredLabel;
        private System.Windows.Forms.Label addressRequiredLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip helpContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem displayHelpInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTourToolStripMenuItem;
    }
}