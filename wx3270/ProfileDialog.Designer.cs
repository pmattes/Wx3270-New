namespace X3270
{
    partial class ProfileDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileDialog));
            this.profileHelpLabel = new System.Windows.Forms.Label();
            this.currentProfileLabel = new System.Windows.Forms.Label();
            this.SaveProfileDialog = new System.Windows.Forms.SaveFileDialog();
            this.LoadProfileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.autoConnectCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.profilesListBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.switchToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mergeFromButton = new System.Windows.Forms.Button();
            this.switchToButton = new System.Windows.Forms.Button();
            this.duplicateButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.renameButton = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.loadSavePanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.switchToGroupBox = new System.Windows.Forms.GroupBox();
            this.renameTextBox = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.feedbackLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.loadSavePanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.switchToGroupBox.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // profileHelpLabel
            // 
            this.profileHelpLabel.AutoSize = true;
            this.profileHelpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileHelpLabel.Location = new System.Drawing.Point(17, 343);
            this.profileHelpLabel.Name = "profileHelpLabel";
            this.profileHelpLabel.Size = new System.Drawing.Size(311, 12);
            this.profileHelpLabel.TabIndex = 10;
            this.profileHelpLabel.Text = "The \'Base\' profile is used when wx3270 is started without specifying a profile.";
            this.profileHelpLabel.Visible = false;
            // 
            // currentProfileLabel
            // 
            this.currentProfileLabel.AutoSize = true;
            this.currentProfileLabel.Location = new System.Drawing.Point(13, 11);
            this.currentProfileLabel.Name = "currentProfileLabel";
            this.currentProfileLabel.Size = new System.Drawing.Size(75, 13);
            this.currentProfileLabel.TabIndex = 12;
            this.currentProfileLabel.Text = "Current profile:";
            // 
            // SaveProfileDialog
            // 
            this.SaveProfileDialog.DefaultExt = "wx3270";
            this.SaveProfileDialog.Filter = "wx3270 profiles|*.wx3270|All files|*.*";
            // 
            // LoadProfileDialog
            // 
            this.LoadProfileDialog.DefaultExt = "wx3270";
            this.LoadProfileDialog.FileName = "openFileDialog1";
            this.LoadProfileDialog.Filter = "wx3270 profiles|*.wx3270|All files|*.*";
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainPanel.Location = new System.Drawing.Point(13, 13);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(0, 0);
            this.mainPanel.TabIndex = 16;
            // 
            // autoConnectCheckBox
            // 
            this.autoConnectCheckBox.AutoSize = true;
            this.autoConnectCheckBox.Checked = true;
            this.autoConnectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoConnectCheckBox.Location = new System.Drawing.Point(6, 47);
            this.autoConnectCheckBox.Name = "autoConnectCheckBox";
            this.autoConnectCheckBox.Size = new System.Drawing.Size(185, 17);
            this.autoConnectCheckBox.TabIndex = 15;
            this.autoConnectCheckBox.Text = "Auto-connect if specified in profile";
            this.autoConnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Profiles:";
            // 
            // profilesListBox
            // 
            this.profilesListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.profilesListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.profilesListBox.FormattingEnabled = true;
            this.profilesListBox.Location = new System.Drawing.Point(19, 34);
            this.profilesListBox.Name = "profilesListBox";
            this.profilesListBox.Size = new System.Drawing.Size(399, 303);
            this.profilesListBox.Sorted = true;
            this.profilesListBox.TabIndex = 18;
            this.profilesListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ProfilesListBox_DrawItem);
            this.profilesListBox.SelectedIndexChanged += new System.EventHandler(this.ProfilesListBox_SelectedIndexChanged);
            this.profilesListBox.DoubleClick += new System.EventHandler(this.ProfilesListBox_DoubleClick);
            this.profilesListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProfilesListBox_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.switchToToolStripMenuItem,
            this.duplicateToolStripMenuItem,
            this.mergeFromToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(140, 136);
            // 
            // switchToToolStripMenuItem
            // 
            this.switchToToolStripMenuItem.Name = "switchToToolStripMenuItem";
            this.switchToToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.switchToToolStripMenuItem.Tag = "SwitchTo";
            this.switchToToolStripMenuItem.Text = "Switch To";
            this.switchToToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuStrip_Clicked);
            // 
            // duplicateToolStripMenuItem
            // 
            this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
            this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.duplicateToolStripMenuItem.Tag = "Duplicate";
            this.duplicateToolStripMenuItem.Text = "Duplicate";
            this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuStrip_Clicked);
            // 
            // mergeFromToolStripMenuItem
            // 
            this.mergeFromToolStripMenuItem.Name = "mergeFromToolStripMenuItem";
            this.mergeFromToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.mergeFromToolStripMenuItem.Tag = "MergeFrom";
            this.mergeFromToolStripMenuItem.Text = "Merge From";
            this.mergeFromToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuStrip_Clicked);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.renameToolStripMenuItem.Tag = "Rename";
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuStrip_Clicked);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Red;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "❌ Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuStrip_Clicked);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.exportToolStripMenuItem.Tag = "Export";
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuStrip_Clicked);
            // 
            // mergeFromButton
            // 
            this.mergeFromButton.Enabled = false;
            this.mergeFromButton.Location = new System.Drawing.Point(100, 440);
            this.mergeFromButton.Name = "mergeFromButton";
            this.mergeFromButton.Size = new System.Drawing.Size(75, 23);
            this.mergeFromButton.TabIndex = 25;
            this.mergeFromButton.Text = "Merge From";
            this.toolTip1.SetToolTip(this.mergeFromButton, "Merge selected attributes from the selected profile");
            this.mergeFromButton.UseVisualStyleBackColor = true;
            this.mergeFromButton.Click += new System.EventHandler(this.MergeFromButton_Click);
            // 
            // switchToButton
            // 
            this.switchToButton.Location = new System.Drawing.Point(6, 14);
            this.switchToButton.Name = "switchToButton";
            this.switchToButton.Size = new System.Drawing.Size(75, 23);
            this.switchToButton.TabIndex = 21;
            this.switchToButton.Text = "Switch To";
            this.toolTip1.SetToolTip(this.switchToButton, "Switch to the selected profile");
            this.switchToButton.UseVisualStyleBackColor = true;
            this.switchToButton.Click += new System.EventHandler(this.SwitchToButton_Click);
            // 
            // duplicateButton
            // 
            this.duplicateButton.Enabled = false;
            this.duplicateButton.Location = new System.Drawing.Point(19, 440);
            this.duplicateButton.Name = "duplicateButton";
            this.duplicateButton.Size = new System.Drawing.Size(75, 23);
            this.duplicateButton.TabIndex = 24;
            this.duplicateButton.Text = "Duplicate";
            this.toolTip1.SetToolTip(this.duplicateButton, "Duplicate the selected profile");
            this.duplicateButton.UseVisualStyleBackColor = true;
            this.duplicateButton.Click += new System.EventHandler(this.DuplicateButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Enabled = false;
            this.deleteButton.ForeColor = System.Drawing.Color.Red;
            this.deleteButton.Location = new System.Drawing.Point(343, 440);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(26, 23);
            this.deleteButton.TabIndex = 22;
            this.deleteButton.Text = "❌";
            this.toolTip1.SetToolTip(this.deleteButton, "Delete the selected profile");
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.ProfileDeleteButton_Click);
            // 
            // renameButton
            // 
            this.renameButton.Enabled = false;
            this.renameButton.Location = new System.Drawing.Point(181, 440);
            this.renameButton.Name = "renameButton";
            this.renameButton.Size = new System.Drawing.Size(75, 23);
            this.renameButton.TabIndex = 23;
            this.renameButton.Text = "Rename";
            this.toolTip1.SetToolTip(this.renameButton, "Rename the selected profile");
            this.renameButton.UseVisualStyleBackColor = true;
            this.renameButton.Click += new System.EventHandler(this.RenameButton_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(48, 522);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 27;
            this.button7.Text = "Import";
            this.toolTip1.SetToolTip(this.button7, "Import a profile from a non-standard location");
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.ProfileImportButton_Click);
            // 
            // newButton
            // 
            this.newButton.ForeColor = System.Drawing.Color.Green;
            this.newButton.Location = new System.Drawing.Point(17, 522);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(25, 23);
            this.newButton.TabIndex = 26;
            this.newButton.Text = "➕";
            this.toolTip1.SetToolTip(this.newButton, "Create a new profile with default values");
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(262, 440);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(75, 23);
            this.exportButton.TabIndex = 30;
            this.exportButton.Text = "Export";
            this.toolTip1.SetToolTip(this.exportButton, "Copy the selected profile to a non-standard location");
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ProfileSaveAsButton_Click);
            // 
            // loadSavePanel
            // 
            this.loadSavePanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.loadSavePanel.Controls.Add(this.groupBox1);
            this.loadSavePanel.Controls.Add(this.button7);
            this.loadSavePanel.Controls.Add(this.newButton);
            this.loadSavePanel.Controls.Add(this.panel3);
            this.loadSavePanel.Controls.Add(this.currentProfileLabel);
            this.loadSavePanel.Location = new System.Drawing.Point(13, 13);
            this.loadSavePanel.Name = "loadSavePanel";
            this.loadSavePanel.Size = new System.Drawing.Size(481, 561);
            this.loadSavePanel.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.switchToGroupBox);
            this.groupBox1.Controls.Add(this.exportButton);
            this.groupBox1.Controls.Add(this.renameTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.profilesListBox);
            this.groupBox1.Controls.Add(this.profileHelpLabel);
            this.groupBox1.Controls.Add(this.mergeFromButton);
            this.groupBox1.Controls.Add(this.duplicateButton);
            this.groupBox1.Controls.Add(this.deleteButton);
            this.groupBox1.Controls.Add(this.renameButton);
            this.groupBox1.Location = new System.Drawing.Point(16, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(443, 481);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            // 
            // switchToGroupBox
            // 
            this.switchToGroupBox.Controls.Add(this.switchToButton);
            this.switchToGroupBox.Controls.Add(this.autoConnectCheckBox);
            this.switchToGroupBox.Enabled = false;
            this.switchToGroupBox.Location = new System.Drawing.Point(19, 358);
            this.switchToGroupBox.Name = "switchToGroupBox";
            this.switchToGroupBox.Size = new System.Drawing.Size(200, 76);
            this.switchToGroupBox.TabIndex = 31;
            this.switchToGroupBox.TabStop = false;
            // 
            // renameTextBox
            // 
            this.renameTextBox.Location = new System.Drawing.Point(246, 402);
            this.renameTextBox.Name = "renameTextBox";
            this.renameTextBox.Size = new System.Drawing.Size(174, 20);
            this.renameTextBox.TabIndex = 29;
            this.renameTextBox.Visible = false;
            this.renameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RenameTextBox_KeyPress);
            this.renameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.RenameTextBox_Validating);
            this.renameTextBox.Validated += new System.EventHandler(this.RenameTextBox_Validated);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.feedbackLabel);
            this.panel3.Location = new System.Drawing.Point(305, 11);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(154, 19);
            this.panel3.TabIndex = 20;
            // 
            // feedbackLabel
            // 
            this.feedbackLabel.AutoSize = true;
            this.feedbackLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.feedbackLabel.ForeColor = System.Drawing.Color.Black;
            this.feedbackLabel.Location = new System.Drawing.Point(51, 0);
            this.feedbackLabel.Name = "feedbackLabel";
            this.feedbackLabel.Size = new System.Drawing.Size(103, 13);
            this.feedbackLabel.TabIndex = 19;
            this.feedbackLabel.Text = "What just happened";
            this.feedbackLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ProfileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(58)))), ((int)(((byte)(132)))));
            this.ClientSize = new System.Drawing.Size(513, 590);
            this.Controls.Add(this.loadSavePanel);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProfileDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Profile Management";
            this.Activated += new System.EventHandler(this.ProfileDialog_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProfileDialog_FormClosing);
            this.Shown += new System.EventHandler(this.ProfileDialog_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.loadSavePanel.ResumeLayout(false);
            this.loadSavePanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.switchToGroupBox.ResumeLayout(false);
            this.switchToGroupBox.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label currentProfileLabel;
        private System.Windows.Forms.Label profileHelpLabel;
        private System.Windows.Forms.SaveFileDialog SaveProfileDialog;
        private System.Windows.Forms.OpenFileDialog LoadProfileDialog;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel loadSavePanel;
        private System.Windows.Forms.Label feedbackLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox profilesListBox;
        private System.Windows.Forms.CheckBox autoConnectCheckBox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button mergeFromButton;
        private System.Windows.Forms.Button duplicateButton;
        private System.Windows.Forms.Button renameButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button switchToButton;
        private System.Windows.Forms.TextBox renameTextBox;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.GroupBox switchToGroupBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem switchToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeFromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateToolStripMenuItem;
    }
}
