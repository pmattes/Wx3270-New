namespace Wx3270
{
    partial class Macros
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Macros));
            this.macroEditButton = new Wx3270.NoSelectButton();
            this.macroAddButton = new Wx3270.NoSelectButton();
            this.macroTestButton = new Wx3270.NoSelectButton();
            this.macroRemoveButton = new Wx3270.NoSelectButton();
            this.macrosLabel = new System.Windows.Forms.Label();
            this.macrosListBox = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.leftButtonFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.recordButton = new Wx3270.NoSelectButton();
            this.rightButtonsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.redoButton = new Wx3270.NoSelectButton();
            this.undoButton = new Wx3270.NoSelectButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.bottomFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.helpContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayHelpInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.leftButtonFlowLayoutPanel.SuspendLayout();
            this.rightButtonsFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.bottomFlowLayoutPanel.SuspendLayout();
            this.helpContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // macroEditButton
            // 
            this.macroEditButton.AutoSize = true;
            this.macroEditButton.Enabled = false;
            this.macroEditButton.Location = new System.Drawing.Point(4, 4);
            this.macroEditButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.macroEditButton.Name = "macroEditButton";
            this.macroEditButton.Size = new System.Drawing.Size(100, 32);
            this.macroEditButton.TabIndex = 101;
            this.macroEditButton.TabStop = false;
            this.macroEditButton.Text = "Edit";
            this.toolTip1.SetToolTip(this.macroEditButton, "Edit the selected macro");
            this.macroEditButton.UseVisualStyleBackColor = true;
            this.macroEditButton.Click += new System.EventHandler(this.MacroEditButton_Click);
            // 
            // macroAddButton
            // 
            this.macroAddButton.AutoSize = true;
            this.macroAddButton.ForeColor = System.Drawing.Color.Green;
            this.macroAddButton.Location = new System.Drawing.Point(0, 4);
            this.macroAddButton.Margin = new System.Windows.Forms.Padding(0, 4, 4, 4);
            this.macroAddButton.Name = "macroAddButton";
            this.macroAddButton.Size = new System.Drawing.Size(100, 32);
            this.macroAddButton.TabIndex = 104;
            this.macroAddButton.TabStop = false;
            this.macroAddButton.Text = "➕ New";
            this.toolTip1.SetToolTip(this.macroAddButton, "Create a new macro");
            this.macroAddButton.UseVisualStyleBackColor = true;
            this.macroAddButton.Click += new System.EventHandler(this.MacroAddButton_Click);
            // 
            // macroTestButton
            // 
            this.macroTestButton.AutoSize = true;
            this.macroTestButton.Enabled = false;
            this.macroTestButton.Location = new System.Drawing.Point(112, 4);
            this.macroTestButton.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.macroTestButton.Name = "macroTestButton";
            this.macroTestButton.Size = new System.Drawing.Size(100, 32);
            this.macroTestButton.TabIndex = 103;
            this.macroTestButton.TabStop = false;
            this.macroTestButton.Text = "Run";
            this.toolTip1.SetToolTip(this.macroTestButton, "Run the selected macro");
            this.macroTestButton.UseVisualStyleBackColor = true;
            this.macroTestButton.Click += new System.EventHandler(this.MacroTestButton_Click);
            // 
            // macroRemoveButton
            // 
            this.macroRemoveButton.AutoSize = true;
            this.macroRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.macroRemoveButton.Enabled = false;
            this.macroRemoveButton.ForeColor = System.Drawing.Color.Red;
            this.macroRemoveButton.Location = new System.Drawing.Point(219, 4);
            this.macroRemoveButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.macroRemoveButton.Name = "macroRemoveButton";
            this.macroRemoveButton.Size = new System.Drawing.Size(72, 26);
            this.macroRemoveButton.TabIndex = 105;
            this.macroRemoveButton.TabStop = false;
            this.macroRemoveButton.Text = "❌ Delete";
            this.toolTip1.SetToolTip(this.macroRemoveButton, "Delete the selected macro");
            this.macroRemoveButton.UseVisualStyleBackColor = true;
            this.macroRemoveButton.Click += new System.EventHandler(this.MacroRemoveButton_Click);
            // 
            // macrosLabel
            // 
            this.macrosLabel.AutoSize = true;
            this.macrosLabel.Location = new System.Drawing.Point(16, 11);
            this.macrosLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.macrosLabel.Name = "macrosLabel";
            this.macrosLabel.Size = new System.Drawing.Size(52, 16);
            this.macrosLabel.TabIndex = 102;
            this.macrosLabel.Text = "Macros";
            // 
            // macrosListBox
            // 
            this.macrosListBox.AllowDrop = true;
            this.macrosListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.macrosListBox.FormattingEnabled = true;
            this.macrosListBox.ItemHeight = 16;
            this.macrosListBox.Location = new System.Drawing.Point(16, 34);
            this.macrosListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.macrosListBox.Name = "macrosListBox";
            this.macrosListBox.Size = new System.Drawing.Size(736, 564);
            this.macrosListBox.TabIndex = 100;
            this.macrosListBox.SelectedIndexChanged += new System.EventHandler(this.MacrosListBox_SelectedIndexChanged);
            this.macrosListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.MacrosListBox_DragDrop);
            this.macrosListBox.DragOver += new System.Windows.Forms.DragEventHandler(this.MacrosListBox_DragOver);
            this.macrosListBox.DoubleClick += new System.EventHandler(this.MacrosListBox_DoubleClick);
            this.macrosListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MacrosListBox_MouseDown);
            this.macrosListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MacrosListBox_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(148, 76);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(147, 24);
            this.runToolStripMenuItem.Tag = "Run";
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(147, 24);
            this.editToolStripMenuItem.Tag = "Edit";
            this.editToolStripMenuItem.Text = "🖉 Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Red;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(147, 24);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "❌ Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainPanel.Controls.Add(this.leftButtonFlowLayoutPanel);
            this.mainPanel.Controls.Add(this.rightButtonsFlowLayoutPanel);
            this.mainPanel.Controls.Add(this.macrosListBox);
            this.mainPanel.Controls.Add(this.macrosLabel);
            this.mainPanel.Location = new System.Drawing.Point(17, 16);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(779, 656);
            this.mainPanel.TabIndex = 108;
            // 
            // leftButtonFlowLayoutPanel
            // 
            this.leftButtonFlowLayoutPanel.AutoSize = true;
            this.leftButtonFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftButtonFlowLayoutPanel.Controls.Add(this.macroAddButton);
            this.leftButtonFlowLayoutPanel.Controls.Add(this.recordButton);
            this.leftButtonFlowLayoutPanel.Controls.Add(this.macroRemoveButton);
            this.leftButtonFlowLayoutPanel.Location = new System.Drawing.Point(16, 603);
            this.leftButtonFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftButtonFlowLayoutPanel.Name = "leftButtonFlowLayoutPanel";
            this.leftButtonFlowLayoutPanel.Size = new System.Drawing.Size(295, 40);
            this.leftButtonFlowLayoutPanel.TabIndex = 108;
            // 
            // recordButton
            // 
            this.recordButton.AutoSize = true;
            this.recordButton.ForeColor = System.Drawing.Color.Green;
            this.recordButton.Location = new System.Drawing.Point(108, 4);
            this.recordButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(103, 32);
            this.recordButton.TabIndex = 106;
            this.recordButton.Text = "⏺ Record";
            this.toolTip1.SetToolTip(this.recordButton, "Create a new macro with the macro recorder");
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // rightButtonsFlowLayoutPanel
            // 
            this.rightButtonsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rightButtonsFlowLayoutPanel.AutoSize = true;
            this.rightButtonsFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.rightButtonsFlowLayoutPanel.Controls.Add(this.macroTestButton);
            this.rightButtonsFlowLayoutPanel.Controls.Add(this.macroEditButton);
            this.rightButtonsFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.rightButtonsFlowLayoutPanel.Location = new System.Drawing.Point(541, 603);
            this.rightButtonsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightButtonsFlowLayoutPanel.Name = "rightButtonsFlowLayoutPanel";
            this.rightButtonsFlowLayoutPanel.Size = new System.Drawing.Size(212, 40);
            this.rightButtonsFlowLayoutPanel.TabIndex = 107;
            // 
            // redoButton
            // 
            this.redoButton.Enabled = false;
            this.redoButton.Location = new System.Drawing.Point(181, 4);
            this.redoButton.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(47, 28);
            this.redoButton.TabIndex = 109;
            this.redoButton.TabStop = false;
            this.redoButton.Tag = "`";
            this.redoButton.Text = "↷";
            this.redoButton.UseVisualStyleBackColor = true;
            this.redoButton.Click += new System.EventHandler(this.RedoButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Enabled = false;
            this.undoButton.Location = new System.Drawing.Point(126, 4);
            this.undoButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(47, 28);
            this.undoButton.TabIndex = 110;
            this.undoButton.TabStop = false;
            this.undoButton.Tag = "`";
            this.undoButton.Text = "↶";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.ContextMenuStrip = this.helpContextMenuStrip;
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(236, 4);
            this.helpPictureBox.Margin = new System.Windows.Forms.Padding(8, 4, 0, 4);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(31, 28);
            this.helpPictureBox.TabIndex = 111;
            this.helpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.helpPictureBox, "Get help");
            this.helpPictureBox.Click += new System.EventHandler(this.Help_Click);
            // 
            // bottomFlowLayoutPanel
            // 
            this.bottomFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bottomFlowLayoutPanel.Controls.Add(this.helpPictureBox);
            this.bottomFlowLayoutPanel.Controls.Add(this.redoButton);
            this.bottomFlowLayoutPanel.Controls.Add(this.undoButton);
            this.bottomFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.bottomFlowLayoutPanel.Location = new System.Drawing.Point(529, 679);
            this.bottomFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bottomFlowLayoutPanel.Name = "bottomFlowLayoutPanel";
            this.bottomFlowLayoutPanel.Size = new System.Drawing.Size(267, 42);
            this.bottomFlowLayoutPanel.TabIndex = 112;
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
            // Macros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(115)))), ((int)(((byte)(191)))));
            this.ClientSize = new System.Drawing.Size(813, 720);
            this.Controls.Add(this.bottomFlowLayoutPanel);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Macros";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Macros";
            this.Activated += new System.EventHandler(this.Macros_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Macros_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.leftButtonFlowLayoutPanel.ResumeLayout(false);
            this.leftButtonFlowLayoutPanel.PerformLayout();
            this.rightButtonsFlowLayoutPanel.ResumeLayout(false);
            this.rightButtonsFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.bottomFlowLayoutPanel.ResumeLayout(false);
            this.helpContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Wx3270.NoSelectButton macroEditButton;
        private Wx3270.NoSelectButton macroAddButton;
        private Wx3270.NoSelectButton macroTestButton;
        private Wx3270.NoSelectButton macroRemoveButton;
        private System.Windows.Forms.Label macrosLabel;
        private System.Windows.Forms.ListBox macrosListBox;
        private System.Windows.Forms.Panel mainPanel;
        private Wx3270.NoSelectButton redoButton;
        private Wx3270.NoSelectButton undoButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private Wx3270.NoSelectButton recordButton;
        private System.Windows.Forms.FlowLayoutPanel bottomFlowLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel leftButtonFlowLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel rightButtonsFlowLayoutPanel;
        private System.Windows.Forms.ContextMenuStrip helpContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem displayHelpInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTourToolStripMenuItem;
    }
}