namespace Wx3270
{
    partial class ServerEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerEditor));
            this.proxyTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.portRequiredLabel = new System.Windows.Forms.Label();
            this.addressRequiredLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.addressLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.addressTextBox = new System.Windows.Forms.TextBox();
            this.serviceLabel = new System.Windows.Forms.Label();
            this.serviceValueLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.proxyTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // proxyTableLayoutPanel
            // 
            this.proxyTableLayoutPanel.AutoSize = true;
            this.proxyTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.proxyTableLayoutPanel.ColumnCount = 1;
            this.proxyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.proxyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.proxyTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.proxyTableLayoutPanel.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.proxyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.proxyTableLayoutPanel.Name = "proxyTableLayoutPanel";
            this.proxyTableLayoutPanel.RowCount = 2;
            this.proxyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.proxyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.proxyTableLayoutPanel.Size = new System.Drawing.Size(305, 162);
            this.proxyTableLayoutPanel.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.portRequiredLabel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.addressRequiredLabel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.portLabel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.addressLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.portTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.addressTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.serviceLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.serviceValueLabel, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(299, 121);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // portRequiredLabel
            // 
            this.portRequiredLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.portRequiredLabel.AutoSize = true;
            this.portRequiredLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portRequiredLabel.Location = new System.Drawing.Point(243, 106);
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
            this.addressRequiredLabel.Location = new System.Drawing.Point(243, 65);
            this.addressRequiredLabel.Name = "addressRequiredLabel";
            this.addressRequiredLabel.Size = new System.Drawing.Size(53, 15);
            this.addressRequiredLabel.TabIndex = 122;
            this.addressRequiredLabel.Text = "required";
            this.addressRequiredLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // portLabel
            // 
            this.portLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(3, 85);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(29, 15);
            this.portLabel.TabIndex = 1;
            this.portLabel.Text = "Port";
            // 
            // addressLabel
            // 
            this.addressLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.addressLabel.AutoSize = true;
            this.addressLabel.Location = new System.Drawing.Point(3, 44);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(51, 15);
            this.addressLabel.TabIndex = 0;
            this.addressLabel.Text = "Address";
            // 
            // portTextBox
            // 
            this.portTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.portTextBox.Location = new System.Drawing.Point(103, 83);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(193, 20);
            this.portTextBox.TabIndex = 2;
            this.portTextBox.Tag = "port";
            this.toolTip1.SetToolTip(this.portTextBox, "Listening port");
            // 
            // addressTextBox
            // 
            this.addressTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addressTextBox.Location = new System.Drawing.Point(103, 42);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(193, 20);
            this.addressTextBox.TabIndex = 1;
            this.addressTextBox.Tag = "address";
            this.toolTip1.SetToolTip(this.addressTextBox, "Listening address");
            // 
            // serviceLabel
            // 
            this.serviceLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serviceLabel.AutoSize = true;
            this.serviceLabel.Location = new System.Drawing.Point(3, 4);
            this.serviceLabel.Name = "serviceLabel";
            this.serviceLabel.Size = new System.Drawing.Size(47, 15);
            this.serviceLabel.TabIndex = 123;
            this.serviceLabel.Text = "Service";
            // 
            // serviceValueLabel
            // 
            this.serviceValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serviceValueLabel.AutoSize = true;
            this.serviceValueLabel.Location = new System.Drawing.Point(103, 4);
            this.serviceValueLabel.Name = "serviceValueLabel";
            this.serviceValueLabel.Size = new System.Drawing.Size(43, 15);
            this.serviceValueLabel.TabIndex = 124;
            this.serviceValueLabel.Tag = "`nolocal";
            this.serviceValueLabel.Text = "(none)";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.helpPictureBox);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(111, 130);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(191, 29);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(165, 3);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(23, 23);
            this.helpPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.helpPictureBox.TabIndex = 9;
            this.helpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.helpPictureBox, "Get help");
            this.helpPictureBox.Click += new System.EventHandler(this.HelpClick);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(84, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "Save";
            this.toolTip1.SetToolTip(this.okButton, "Save changes");
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(3, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Abandon edits");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // ServerEditor
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.proxyTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server Editor";
            this.Shown += new System.EventHandler(this.ListenerEditorShown);
            this.proxyTableLayoutPanel.ResumeLayout(false);
            this.proxyTableLayoutPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel proxyTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label portRequiredLabel;
        private System.Windows.Forms.Label addressRequiredLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label serviceLabel;
        private System.Windows.Forms.Label serviceValueLabel;
    }
}