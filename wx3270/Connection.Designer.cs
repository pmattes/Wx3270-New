namespace X3270
{
    partial class Connection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Connection));
            this.profileHostRemoveButton = new System.Windows.Forms.Button();
            this.quickConnectButton = new System.Windows.Forms.Button();
            this.profileHostAddButton = new System.Windows.Forms.Button();
            this.profileHostEditButton = new System.Windows.Forms.Button();
            this.profileHostConnectButton = new System.Windows.Forms.Button();
            this.profileHostsListBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.UndoButton = new System.Windows.Forms.Button();
            this.RedoButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // profileHostRemoveButton
            // 
            this.profileHostRemoveButton.BackColor = System.Drawing.Color.Transparent;
            this.profileHostRemoveButton.Enabled = false;
            this.profileHostRemoveButton.ForeColor = System.Drawing.Color.Red;
            this.profileHostRemoveButton.Location = new System.Drawing.Point(44, 364);
            this.profileHostRemoveButton.Margin = new System.Windows.Forms.Padding(1);
            this.profileHostRemoveButton.Name = "profileHostRemoveButton";
            this.profileHostRemoveButton.Size = new System.Drawing.Size(23, 23);
            this.profileHostRemoveButton.TabIndex = 131;
            this.profileHostRemoveButton.TabStop = false;
            this.profileHostRemoveButton.Text = "❌";
            this.profileHostRemoveButton.UseVisualStyleBackColor = false;
            this.profileHostRemoveButton.Click += new System.EventHandler(this.ProfileHostRemoveButton_Click);
            // 
            // quickConnectButton
            // 
            this.quickConnectButton.BackColor = System.Drawing.Color.Transparent;
            this.quickConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quickConnectButton.ForeColor = System.Drawing.Color.Green;
            this.quickConnectButton.Location = new System.Drawing.Point(13, 9);
            this.quickConnectButton.Name = "quickConnectButton";
            this.quickConnectButton.Size = new System.Drawing.Size(103, 23);
            this.quickConnectButton.TabIndex = 129;
            this.quickConnectButton.Text = "🗲 Quick Connect";
            this.quickConnectButton.UseVisualStyleBackColor = false;
            this.quickConnectButton.Click += new System.EventHandler(this.QuickConnectButton_Click);
            // 
            // profileHostAddButton
            // 
            this.profileHostAddButton.BackColor = System.Drawing.Color.Transparent;
            this.profileHostAddButton.ForeColor = System.Drawing.Color.Green;
            this.profileHostAddButton.Location = new System.Drawing.Point(17, 364);
            this.profileHostAddButton.Name = "profileHostAddButton";
            this.profileHostAddButton.Size = new System.Drawing.Size(23, 23);
            this.profileHostAddButton.TabIndex = 127;
            this.profileHostAddButton.Text = "➕";
            this.profileHostAddButton.UseVisualStyleBackColor = false;
            this.profileHostAddButton.Click += new System.EventHandler(this.ProfileHostAddButton_Click);
            // 
            // profileHostEditButton
            // 
            this.profileHostEditButton.BackColor = System.Drawing.Color.Transparent;
            this.profileHostEditButton.Enabled = false;
            this.profileHostEditButton.Location = new System.Drawing.Point(200, 364);
            this.profileHostEditButton.Name = "profileHostEditButton";
            this.profileHostEditButton.Size = new System.Drawing.Size(75, 23);
            this.profileHostEditButton.TabIndex = 126;
            this.profileHostEditButton.Text = "Edit";
            this.profileHostEditButton.UseVisualStyleBackColor = false;
            this.profileHostEditButton.Click += new System.EventHandler(this.ProfileHostEditButton_Click);
            // 
            // profileHostConnectButton
            // 
            this.profileHostConnectButton.BackColor = System.Drawing.Color.Transparent;
            this.profileHostConnectButton.Enabled = false;
            this.profileHostConnectButton.Location = new System.Drawing.Point(281, 364);
            this.profileHostConnectButton.Name = "profileHostConnectButton";
            this.profileHostConnectButton.Size = new System.Drawing.Size(75, 23);
            this.profileHostConnectButton.TabIndex = 125;
            this.profileHostConnectButton.Text = "🗲 Connect";
            this.profileHostConnectButton.UseVisualStyleBackColor = false;
            this.profileHostConnectButton.Click += new System.EventHandler(this.ProfileHostConnectButton_Click);
            // 
            // profileHostsListBox
            // 
            this.profileHostsListBox.AllowDrop = true;
            this.profileHostsListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.profileHostsListBox.FormattingEnabled = true;
            this.profileHostsListBox.Location = new System.Drawing.Point(17, 16);
            this.profileHostsListBox.Name = "profileHostsListBox";
            this.profileHostsListBox.Size = new System.Drawing.Size(339, 342);
            this.profileHostsListBox.TabIndex = 124;
            this.profileHostsListBox.SelectedIndexChanged += new System.EventHandler(this.ProfileHostsListBox_SelectedIndexChanged);
            this.profileHostsListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.ProfileHostsListBox_DragDrop);
            this.profileHostsListBox.DragOver += new System.Windows.Forms.DragEventHandler(this.ProfileHostsListBox_DragOver);
            this.profileHostsListBox.DoubleClick += new System.EventHandler(this.ProfileHostsListBox_DoubleClick);
            this.profileHostsListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProfileHostsListBox_MouseDown);
            this.profileHostsListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ProfileHostsListBox_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 70);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.connectToolStripMenuItem.Tag = "Connect";
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.editToolStripMenuItem.Tag = "Edit";
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Red;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "❌ Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.ContextMenuItem_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.BackColor = System.Drawing.Color.Transparent;
            this.DisconnectButton.Enabled = false;
            this.DisconnectButton.ForeColor = System.Drawing.Color.Red;
            this.DisconnectButton.Location = new System.Drawing.Point(122, 9);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(75, 23);
            this.DisconnectButton.TabIndex = 115;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = false;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainPanel.Controls.Add(this.profileHostRemoveButton);
            this.mainPanel.Controls.Add(this.profileHostsListBox);
            this.mainPanel.Controls.Add(this.profileHostAddButton);
            this.mainPanel.Controls.Add(this.profileHostConnectButton);
            this.mainPanel.Controls.Add(this.profileHostEditButton);
            this.mainPanel.Location = new System.Drawing.Point(13, 42);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(378, 428);
            this.mainPanel.TabIndex = 137;
            // 
            // UndoButton
            // 
            this.UndoButton.Enabled = false;
            this.UndoButton.Location = new System.Drawing.Point(315, 476);
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.Size = new System.Drawing.Size(35, 23);
            this.UndoButton.TabIndex = 139;
            this.UndoButton.Text = "↶";
            this.UndoButton.UseVisualStyleBackColor = true;
            this.UndoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // RedoButton
            // 
            this.RedoButton.Enabled = false;
            this.RedoButton.Location = new System.Drawing.Point(356, 476);
            this.RedoButton.Name = "RedoButton";
            this.RedoButton.Size = new System.Drawing.Size(35, 23);
            this.RedoButton.TabIndex = 138;
            this.RedoButton.Text = "↷";
            this.RedoButton.UseVisualStyleBackColor = true;
            this.RedoButton.Click += new System.EventHandler(this.RedoButton_Click);
            // 
            // Connection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.ClientSize = new System.Drawing.Size(412, 521);
            this.Controls.Add(this.UndoButton);
            this.Controls.Add(this.RedoButton);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.quickConnectButton);
            this.Controls.Add(this.DisconnectButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Connection";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Host Connection";
            this.Activated += new System.EventHandler(this.Connection_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Connection_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button profileHostRemoveButton;
        private System.Windows.Forms.Button quickConnectButton;
        private System.Windows.Forms.Button profileHostAddButton;
        private System.Windows.Forms.Button profileHostEditButton;
        private System.Windows.Forms.Button profileHostConnectButton;
        private System.Windows.Forms.ListBox profileHostsListBox;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button UndoButton;
        private System.Windows.Forms.Button RedoButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}