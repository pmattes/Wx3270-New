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
            this.recordButton = new Wx3270.NoSelectButton();
            this.redoButton = new Wx3270.NoSelectButton();
            this.undoButton = new Wx3270.NoSelectButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.bottomFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.bottomFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // macroEditButton
            // 
            this.macroEditButton.Enabled = false;
            this.macroEditButton.Location = new System.Drawing.Point(409, 493);
            this.macroEditButton.Name = "macroEditButton";
            this.macroEditButton.Size = new System.Drawing.Size(75, 23);
            this.macroEditButton.TabIndex = 101;
            this.macroEditButton.TabStop = false;
            this.macroEditButton.Text = "Edit";
            this.macroEditButton.UseVisualStyleBackColor = true;
            this.macroEditButton.Click += new System.EventHandler(this.MacroEditButton_Click);
            // 
            // macroAddButton
            // 
            this.macroAddButton.ForeColor = System.Drawing.Color.Green;
            this.macroAddButton.Location = new System.Drawing.Point(12, 493);
            this.macroAddButton.Name = "macroAddButton";
            this.macroAddButton.Size = new System.Drawing.Size(75, 23);
            this.macroAddButton.TabIndex = 104;
            this.macroAddButton.TabStop = false;
            this.macroAddButton.Text = "➕ New";
            this.macroAddButton.UseVisualStyleBackColor = true;
            this.macroAddButton.Click += new System.EventHandler(this.MacroAddButton_Click);
            // 
            // macroTestButton
            // 
            this.macroTestButton.Enabled = false;
            this.macroTestButton.Location = new System.Drawing.Point(490, 493);
            this.macroTestButton.Name = "macroTestButton";
            this.macroTestButton.Size = new System.Drawing.Size(75, 23);
            this.macroTestButton.TabIndex = 103;
            this.macroTestButton.TabStop = false;
            this.macroTestButton.Text = "Run";
            this.macroTestButton.UseVisualStyleBackColor = true;
            this.macroTestButton.Click += new System.EventHandler(this.MacroTestButton_Click);
            // 
            // macroRemoveButton
            // 
            this.macroRemoveButton.Enabled = false;
            this.macroRemoveButton.ForeColor = System.Drawing.Color.Red;
            this.macroRemoveButton.Location = new System.Drawing.Point(177, 493);
            this.macroRemoveButton.Margin = new System.Windows.Forms.Padding(1);
            this.macroRemoveButton.Name = "macroRemoveButton";
            this.macroRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.macroRemoveButton.TabIndex = 105;
            this.macroRemoveButton.TabStop = false;
            this.macroRemoveButton.Text = "❌ Delete";
            this.macroRemoveButton.UseVisualStyleBackColor = true;
            this.macroRemoveButton.Click += new System.EventHandler(this.MacroRemoveButton_Click);
            // 
            // macrosLabel
            // 
            this.macrosLabel.AutoSize = true;
            this.macrosLabel.Location = new System.Drawing.Point(12, 9);
            this.macrosLabel.Name = "macrosLabel";
            this.macrosLabel.Size = new System.Drawing.Size(42, 13);
            this.macrosLabel.TabIndex = 102;
            this.macrosLabel.Text = "Macros";
            // 
            // macrosListBox
            // 
            this.macrosListBox.AllowDrop = true;
            this.macrosListBox.ContextMenuStrip = this.contextMenuStrip1;
            this.macrosListBox.FormattingEnabled = true;
            this.macrosListBox.Location = new System.Drawing.Point(12, 28);
            this.macrosListBox.Name = "macrosListBox";
            this.macrosListBox.Size = new System.Drawing.Size(553, 459);
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
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(123, 70);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.runToolStripMenuItem.Tag = "Run";
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.editToolStripMenuItem.Tag = "Edit";
            this.editToolStripMenuItem.Text = "🖉 Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Red;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.deleteToolStripMenuItem.Tag = "Delete";
            this.deleteToolStripMenuItem.Text = "❌ Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainPanel.Controls.Add(this.recordButton);
            this.mainPanel.Controls.Add(this.macrosListBox);
            this.mainPanel.Controls.Add(this.macroEditButton);
            this.mainPanel.Controls.Add(this.macrosLabel);
            this.mainPanel.Controls.Add(this.macroAddButton);
            this.mainPanel.Controls.Add(this.macroTestButton);
            this.mainPanel.Controls.Add(this.macroRemoveButton);
            this.mainPanel.Location = new System.Drawing.Point(13, 13);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(584, 533);
            this.mainPanel.TabIndex = 108;
            // 
            // recordButton
            // 
            this.recordButton.ForeColor = System.Drawing.Color.Green;
            this.recordButton.Location = new System.Drawing.Point(94, 493);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(75, 23);
            this.recordButton.TabIndex = 106;
            this.recordButton.Text = "⏺ Record";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // redoButton
            // 
            this.redoButton.Enabled = false;
            this.redoButton.Location = new System.Drawing.Point(136, 3);
            this.redoButton.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(35, 23);
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
            this.undoButton.Location = new System.Drawing.Point(95, 3);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(35, 23);
            this.undoButton.TabIndex = 110;
            this.undoButton.TabStop = false;
            this.undoButton.Tag = "`";
            this.undoButton.Text = "↶";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(177, 3);
            this.helpPictureBox.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(23, 23);
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
            this.bottomFlowLayoutPanel.Location = new System.Drawing.Point(397, 552);
            this.bottomFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bottomFlowLayoutPanel.Name = "bottomFlowLayoutPanel";
            this.bottomFlowLayoutPanel.Size = new System.Drawing.Size(200, 34);
            this.bottomFlowLayoutPanel.TabIndex = 112;
            // 
            // Macros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(115)))), ((int)(((byte)(191)))));
            this.ClientSize = new System.Drawing.Size(610, 585);
            this.Controls.Add(this.bottomFlowLayoutPanel);
            this.Controls.Add(this.mainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.bottomFlowLayoutPanel.ResumeLayout(false);
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
    }
}