namespace Wx3270
{
    partial class ShortcutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortcutDialog));
            this.buttonsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.helpPictureBox = new System.Windows.Forms.PictureBox();
            this.helpContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayHelpInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.upperTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pathFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathDisplayLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.checkBoxFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.maximizedCheckBox = new System.Windows.Forms.CheckBox();
            this.fullScreenCheckBox = new System.Windows.Forms.CheckBox();
            this.locationFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.locationCheckBox = new System.Windows.Forms.CheckBox();
            this.xLabel = new System.Windows.Forms.Label();
            this.xNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.yLabel = new System.Windows.Forms.Label();
            this.yNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.readOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.detachedCheckBox = new System.Windows.Forms.CheckBox();
            this.outerFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.borderPanel = new System.Windows.Forms.Panel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.profileFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.profileLabel = new System.Windows.Forms.Label();
            this.profileNameLabel = new System.Windows.Forms.Label();
            this.connectionFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.connectionLabel = new System.Windows.Forms.Label();
            this.connectionNameLabel = new System.Windows.Forms.Label();
            this.profileSpacingLabel = new System.Windows.Forms.Label();
            this.connectionSpacingLabel = new System.Windows.Forms.Label();
            this.buttonsFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).BeginInit();
            this.helpContextMenuStrip.SuspendLayout();
            this.upperTableLayoutPanel.SuspendLayout();
            this.pathFlowLayoutPanel.SuspendLayout();
            this.checkBoxFlowLayoutPanel.SuspendLayout();
            this.locationFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumericUpDown)).BeginInit();
            this.outerFlowLayoutPanel.SuspendLayout();
            this.borderPanel.SuspendLayout();
            this.profileFlowLayoutPanel.SuspendLayout();
            this.connectionFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonsFlowLayoutPanel
            // 
            this.buttonsFlowLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonsFlowLayoutPanel.AutoSize = true;
            this.buttonsFlowLayoutPanel.Controls.Add(this.helpPictureBox);
            this.buttonsFlowLayoutPanel.Controls.Add(this.saveButton);
            this.buttonsFlowLayoutPanel.Controls.Add(this.cancelButton);
            this.buttonsFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonsFlowLayoutPanel.Location = new System.Drawing.Point(369, 254);
            this.buttonsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonsFlowLayoutPanel.Name = "buttonsFlowLayoutPanel";
            this.buttonsFlowLayoutPanel.Size = new System.Drawing.Size(255, 36);
            this.buttonsFlowLayoutPanel.TabIndex = 1;
            // 
            // helpPictureBox
            // 
            this.helpPictureBox.ContextMenuStrip = this.helpContextMenuStrip;
            this.helpPictureBox.Image = global::Wx3270.Properties.Resources.Question23c;
            this.helpPictureBox.Location = new System.Drawing.Point(220, 4);
            this.helpPictureBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.helpPictureBox.Name = "helpPictureBox";
            this.helpPictureBox.Size = new System.Drawing.Size(31, 28);
            this.helpPictureBox.TabIndex = 2;
            this.helpPictureBox.TabStop = false;
            this.toolTip1.SetToolTip(this.helpPictureBox, "Get help");
            this.helpPictureBox.Click += new System.EventHandler(this.HelpButtonClick);
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
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(112, 4);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 28);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.toolTip1.SetToolTip(this.saveButton, "Create the shortcut");
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(4, 4);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.toolTip1.SetToolTip(this.cancelButton, "Don\'t create a shortcut");
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelClick);
            // 
            // upperTableLayoutPanel
            // 
            this.upperTableLayoutPanel.AutoSize = true;
            this.upperTableLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            this.upperTableLayoutPanel.ColumnCount = 1;
            this.upperTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.upperTableLayoutPanel.Controls.Add(this.connectionFlowLayoutPanel, 0, 1);
            this.upperTableLayoutPanel.Controls.Add(this.pathFlowLayoutPanel, 0, 2);
            this.upperTableLayoutPanel.Controls.Add(this.checkBoxFlowLayoutPanel, 0, 3);
            this.upperTableLayoutPanel.Controls.Add(this.profileFlowLayoutPanel, 0, 0);
            this.upperTableLayoutPanel.Location = new System.Drawing.Point(4, 4);
            this.upperTableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.upperTableLayoutPanel.Name = "upperTableLayoutPanel";
            this.upperTableLayoutPanel.RowCount = 4;
            this.upperTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.upperTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.upperTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.upperTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.upperTableLayoutPanel.Size = new System.Drawing.Size(620, 242);
            this.upperTableLayoutPanel.TabIndex = 0;
            // 
            // pathFlowLayoutPanel
            // 
            this.pathFlowLayoutPanel.AutoSize = true;
            this.pathFlowLayoutPanel.Controls.Add(this.pathLabel);
            this.pathFlowLayoutPanel.Controls.Add(this.pathDisplayLabel);
            this.pathFlowLayoutPanel.Controls.Add(this.browseButton);
            this.pathFlowLayoutPanel.Location = new System.Drawing.Point(0, 56);
            this.pathFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.pathFlowLayoutPanel.Name = "pathFlowLayoutPanel";
            this.pathFlowLayoutPanel.Size = new System.Drawing.Size(620, 36);
            this.pathFlowLayoutPanel.TabIndex = 0;
            // 
            // pathLabel
            // 
            this.pathLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(4, 10);
            this.pathLabel.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(37, 16);
            this.pathLabel.TabIndex = 0;
            this.pathLabel.Text = "Path:";
            // 
            // pathDisplayLabel
            // 
            this.pathDisplayLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pathDisplayLabel.AutoEllipsis = true;
            this.pathDisplayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathDisplayLabel.Location = new System.Drawing.Point(41, 9);
            this.pathDisplayLabel.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.pathDisplayLabel.Name = "pathDisplayLabel";
            this.pathDisplayLabel.Size = new System.Drawing.Size(467, 18);
            this.pathDisplayLabel.TabIndex = 3;
            this.pathDisplayLabel.Text = "`path";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.browseButton.Location = new System.Drawing.Point(516, 4);
            this.browseButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(100, 28);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse";
            this.toolTip1.SetToolTip(this.browseButton, "Change the path");
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseClick);
            // 
            // checkBoxFlowLayoutPanel
            // 
            this.checkBoxFlowLayoutPanel.AutoSize = true;
            this.checkBoxFlowLayoutPanel.Controls.Add(this.maximizedCheckBox);
            this.checkBoxFlowLayoutPanel.Controls.Add(this.fullScreenCheckBox);
            this.checkBoxFlowLayoutPanel.Controls.Add(this.locationFlowLayoutPanel);
            this.checkBoxFlowLayoutPanel.Controls.Add(this.readOnlyCheckBox);
            this.checkBoxFlowLayoutPanel.Controls.Add(this.detachedCheckBox);
            this.checkBoxFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.checkBoxFlowLayoutPanel.Location = new System.Drawing.Point(4, 96);
            this.checkBoxFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxFlowLayoutPanel.Name = "checkBoxFlowLayoutPanel";
            this.checkBoxFlowLayoutPanel.Size = new System.Drawing.Size(377, 142);
            this.checkBoxFlowLayoutPanel.TabIndex = 1;
            // 
            // maximizedCheckBox
            // 
            this.maximizedCheckBox.AutoSize = true;
            this.maximizedCheckBox.Location = new System.Drawing.Point(4, 4);
            this.maximizedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.maximizedCheckBox.Name = "maximizedCheckBox";
            this.maximizedCheckBox.Size = new System.Drawing.Size(93, 20);
            this.maximizedCheckBox.TabIndex = 0;
            this.maximizedCheckBox.Tag = "Maximized";
            this.maximizedCheckBox.Text = "Maximized";
            this.toolTip1.SetToolTip(this.maximizedCheckBox, "Start the window maximized");
            this.maximizedCheckBox.UseVisualStyleBackColor = true;
            this.maximizedCheckBox.Click += new System.EventHandler(this.CheckBoxClick);
            // 
            // fullScreenCheckBox
            // 
            this.fullScreenCheckBox.AutoSize = true;
            this.fullScreenCheckBox.Location = new System.Drawing.Point(4, 32);
            this.fullScreenCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fullScreenCheckBox.Name = "fullScreenCheckBox";
            this.fullScreenCheckBox.Size = new System.Drawing.Size(94, 20);
            this.fullScreenCheckBox.TabIndex = 1;
            this.fullScreenCheckBox.Tag = "FullScreen";
            this.fullScreenCheckBox.Text = "Full screen";
            this.toolTip1.SetToolTip(this.fullScreenCheckBox, "Start in full-screen mode");
            this.fullScreenCheckBox.UseVisualStyleBackColor = true;
            this.fullScreenCheckBox.Click += new System.EventHandler(this.CheckBoxClick);
            // 
            // locationFlowLayoutPanel
            // 
            this.locationFlowLayoutPanel.AutoSize = true;
            this.locationFlowLayoutPanel.Controls.Add(this.locationCheckBox);
            this.locationFlowLayoutPanel.Controls.Add(this.xLabel);
            this.locationFlowLayoutPanel.Controls.Add(this.xNumericUpDown);
            this.locationFlowLayoutPanel.Controls.Add(this.yLabel);
            this.locationFlowLayoutPanel.Controls.Add(this.yNumericUpDown);
            this.locationFlowLayoutPanel.Location = new System.Drawing.Point(0, 56);
            this.locationFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.locationFlowLayoutPanel.Name = "locationFlowLayoutPanel";
            this.locationFlowLayoutPanel.Size = new System.Drawing.Size(377, 30);
            this.locationFlowLayoutPanel.TabIndex = 2;
            // 
            // locationCheckBox
            // 
            this.locationCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.locationCheckBox.AutoSize = true;
            this.locationCheckBox.Location = new System.Drawing.Point(4, 5);
            this.locationCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.locationCheckBox.Name = "locationCheckBox";
            this.locationCheckBox.Size = new System.Drawing.Size(106, 20);
            this.locationCheckBox.TabIndex = 0;
            this.locationCheckBox.Tag = "Location";
            this.locationCheckBox.Text = "Start location";
            this.toolTip1.SetToolTip(this.locationCheckBox, "Start at a fixed location on the screen");
            this.locationCheckBox.UseVisualStyleBackColor = true;
            this.locationCheckBox.Click += new System.EventHandler(this.CheckBoxClick);
            // 
            // xLabel
            // 
            this.xLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(118, 7);
            this.xLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(15, 16);
            this.xLabel.TabIndex = 1;
            this.xLabel.Text = "X";
            // 
            // xNumericUpDown
            // 
            this.xNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.xNumericUpDown.Location = new System.Drawing.Point(141, 4);
            this.xNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.xNumericUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.xNumericUpDown.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.xNumericUpDown.Name = "xNumericUpDown";
            this.xNumericUpDown.Size = new System.Drawing.Size(100, 22);
            this.xNumericUpDown.TabIndex = 2;
            // 
            // yLabel
            // 
            this.yLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.yLabel.AutoSize = true;
            this.yLabel.Location = new System.Drawing.Point(249, 7);
            this.yLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.yLabel.Name = "yLabel";
            this.yLabel.Size = new System.Drawing.Size(16, 16);
            this.yLabel.TabIndex = 3;
            this.yLabel.Text = "Y";
            // 
            // yNumericUpDown
            // 
            this.yNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.yNumericUpDown.Location = new System.Drawing.Point(273, 4);
            this.yNumericUpDown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.yNumericUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.yNumericUpDown.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.yNumericUpDown.Name = "yNumericUpDown";
            this.yNumericUpDown.Size = new System.Drawing.Size(100, 22);
            this.yNumericUpDown.TabIndex = 4;
            // 
            // readOnlyCheckBox
            // 
            this.readOnlyCheckBox.AutoSize = true;
            this.readOnlyCheckBox.Location = new System.Drawing.Point(4, 90);
            this.readOnlyCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.readOnlyCheckBox.Name = "readOnlyCheckBox";
            this.readOnlyCheckBox.Size = new System.Drawing.Size(92, 20);
            this.readOnlyCheckBox.TabIndex = 3;
            this.readOnlyCheckBox.Tag = "ReadOnly";
            this.readOnlyCheckBox.Text = "Read-only";
            this.toolTip1.SetToolTip(this.readOnlyCheckBox, "Run in read-only mode");
            this.readOnlyCheckBox.UseVisualStyleBackColor = true;
            this.readOnlyCheckBox.Click += new System.EventHandler(this.CheckBoxClick);
            // 
            // detachedCheckBox
            // 
            this.detachedCheckBox.AutoSize = true;
            this.detachedCheckBox.Location = new System.Drawing.Point(4, 118);
            this.detachedCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.detachedCheckBox.Name = "detachedCheckBox";
            this.detachedCheckBox.Size = new System.Drawing.Size(88, 20);
            this.detachedCheckBox.TabIndex = 4;
            this.detachedCheckBox.Tag = "Detached";
            this.detachedCheckBox.Text = "Detached";
            this.toolTip1.SetToolTip(this.detachedCheckBox, "Run in detached mode (if read-only)");
            this.detachedCheckBox.UseVisualStyleBackColor = true;
            this.detachedCheckBox.Click += new System.EventHandler(this.CheckBoxClick);
            // 
            // outerFlowLayoutPanel
            // 
            this.outerFlowLayoutPanel.AutoSize = true;
            this.outerFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerFlowLayoutPanel.Controls.Add(this.upperTableLayoutPanel);
            this.outerFlowLayoutPanel.Controls.Add(this.buttonsFlowLayoutPanel);
            this.outerFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.outerFlowLayoutPanel.Location = new System.Drawing.Point(15, 14);
            this.outerFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outerFlowLayoutPanel.Name = "outerFlowLayoutPanel";
            this.outerFlowLayoutPanel.Size = new System.Drawing.Size(628, 294);
            this.outerFlowLayoutPanel.TabIndex = 0;
            // 
            // borderPanel
            // 
            this.borderPanel.AutoSize = true;
            this.borderPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.borderPanel.Controls.Add(this.outerFlowLayoutPanel);
            this.borderPanel.Location = new System.Drawing.Point(0, 0);
            this.borderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Padding = new System.Windows.Forms.Padding(15, 14, 15, 14);
            this.borderPanel.Size = new System.Drawing.Size(658, 322);
            this.borderPanel.TabIndex = 0;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Shortcuts (*.lnk)|*.lnk";
            this.saveFileDialog.Title = "Create shortcut";
            // 
            // profileFlowLayoutPanel
            // 
            this.profileFlowLayoutPanel.AutoSize = true;
            this.profileFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.profileFlowLayoutPanel.Controls.Add(this.profileLabel);
            this.profileFlowLayoutPanel.Controls.Add(this.profileNameLabel);
            this.profileFlowLayoutPanel.Controls.Add(this.profileSpacingLabel);
            this.profileFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.profileFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.profileFlowLayoutPanel.Name = "profileFlowLayoutPanel";
            this.profileFlowLayoutPanel.Size = new System.Drawing.Size(213, 28);
            this.profileFlowLayoutPanel.TabIndex = 1;
            // 
            // profileLabel
            // 
            this.profileLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileLabel.AutoSize = true;
            this.profileLabel.Location = new System.Drawing.Point(3, 6);
            this.profileLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.profileLabel.Name = "profileLabel";
            this.profileLabel.Size = new System.Drawing.Size(48, 16);
            this.profileLabel.TabIndex = 0;
            this.profileLabel.Text = "Profile:";
            // 
            // profileNameLabel
            // 
            this.profileNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileNameLabel.AutoSize = true;
            this.profileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileNameLabel.Location = new System.Drawing.Point(51, 6);
            this.profileNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.profileNameLabel.Name = "profileNameLabel";
            this.profileNameLabel.Size = new System.Drawing.Size(99, 16);
            this.profileNameLabel.TabIndex = 1;
            this.profileNameLabel.Text = "`Profile name";
            // 
            // connectionFlowLayoutPanel
            // 
            this.connectionFlowLayoutPanel.AutoSize = true;
            this.connectionFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.connectionFlowLayoutPanel.Controls.Add(this.connectionLabel);
            this.connectionFlowLayoutPanel.Controls.Add(this.connectionNameLabel);
            this.connectionFlowLayoutPanel.Controls.Add(this.connectionSpacingLabel);
            this.connectionFlowLayoutPanel.Location = new System.Drawing.Point(0, 28);
            this.connectionFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.connectionFlowLayoutPanel.Name = "connectionFlowLayoutPanel";
            this.connectionFlowLayoutPanel.Size = new System.Drawing.Size(274, 28);
            this.connectionFlowLayoutPanel.TabIndex = 1;
            // 
            // connectionLabel
            // 
            this.connectionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectionLabel.AutoSize = true;
            this.connectionLabel.Location = new System.Drawing.Point(3, 6);
            this.connectionLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(77, 16);
            this.connectionLabel.TabIndex = 0;
            this.connectionLabel.Text = "Connection:";
            // 
            // connectionNameLabel
            // 
            this.connectionNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectionNameLabel.AutoSize = true;
            this.connectionNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionNameLabel.Location = new System.Drawing.Point(80, 6);
            this.connectionNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.connectionNameLabel.Name = "connectionNameLabel";
            this.connectionNameLabel.Size = new System.Drawing.Size(131, 16);
            this.connectionNameLabel.TabIndex = 1;
            this.connectionNameLabel.Text = "`Connection name";
            // 
            // profileSpacingLabel
            // 
            this.profileSpacingLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.profileSpacingLabel.AutoSize = true;
            this.profileSpacingLabel.Location = new System.Drawing.Point(156, 0);
            this.profileSpacingLabel.MinimumSize = new System.Drawing.Size(0, 28);
            this.profileSpacingLabel.Name = "profileSpacingLabel";
            this.profileSpacingLabel.Size = new System.Drawing.Size(54, 28);
            this.profileSpacingLabel.TabIndex = 2;
            this.profileSpacingLabel.Text = "`nothing";
            // 
            // connectionSpacingLabel
            // 
            this.connectionSpacingLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectionSpacingLabel.AutoSize = true;
            this.connectionSpacingLabel.Location = new System.Drawing.Point(217, 0);
            this.connectionSpacingLabel.MinimumSize = new System.Drawing.Size(0, 28);
            this.connectionSpacingLabel.Name = "connectionSpacingLabel";
            this.connectionSpacingLabel.Size = new System.Drawing.Size(54, 28);
            this.connectionSpacingLabel.TabIndex = 2;
            this.connectionSpacingLabel.Text = "`nothing";
            // 
            // ShortcutDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.IndianRed;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1497, 921);
            this.Controls.Add(this.borderPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShortcutDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Shortcut";
            this.Activated += new System.EventHandler(this.FormActivated);
            this.buttonsFlowLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.helpPictureBox)).EndInit();
            this.helpContextMenuStrip.ResumeLayout(false);
            this.upperTableLayoutPanel.ResumeLayout(false);
            this.upperTableLayoutPanel.PerformLayout();
            this.pathFlowLayoutPanel.ResumeLayout(false);
            this.pathFlowLayoutPanel.PerformLayout();
            this.checkBoxFlowLayoutPanel.ResumeLayout(false);
            this.checkBoxFlowLayoutPanel.PerformLayout();
            this.locationFlowLayoutPanel.ResumeLayout(false);
            this.locationFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yNumericUpDown)).EndInit();
            this.outerFlowLayoutPanel.ResumeLayout(false);
            this.outerFlowLayoutPanel.PerformLayout();
            this.borderPanel.ResumeLayout(false);
            this.borderPanel.PerformLayout();
            this.profileFlowLayoutPanel.ResumeLayout(false);
            this.profileFlowLayoutPanel.PerformLayout();
            this.connectionFlowLayoutPanel.ResumeLayout(false);
            this.connectionFlowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel buttonsFlowLayoutPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox helpPictureBox;
        private System.Windows.Forms.TableLayoutPanel upperTableLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel pathFlowLayoutPanel;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.FlowLayoutPanel checkBoxFlowLayoutPanel;
        private System.Windows.Forms.CheckBox maximizedCheckBox;
        private System.Windows.Forms.CheckBox fullScreenCheckBox;
        private System.Windows.Forms.FlowLayoutPanel locationFlowLayoutPanel;
        private System.Windows.Forms.CheckBox locationCheckBox;
        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.NumericUpDown xNumericUpDown;
        private System.Windows.Forms.Label yLabel;
        private System.Windows.Forms.NumericUpDown yNumericUpDown;
        private System.Windows.Forms.CheckBox readOnlyCheckBox;
        private System.Windows.Forms.CheckBox detachedCheckBox;
        private System.Windows.Forms.FlowLayoutPanel outerFlowLayoutPanel;
        private System.Windows.Forms.Panel borderPanel;
        private System.Windows.Forms.ContextMenuStrip helpContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem displayHelpInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startTourToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label pathDisplayLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.FlowLayoutPanel profileFlowLayoutPanel;
        private System.Windows.Forms.Label profileLabel;
        private System.Windows.Forms.Label profileNameLabel;
        private System.Windows.Forms.FlowLayoutPanel connectionFlowLayoutPanel;
        private System.Windows.Forms.Label connectionLabel;
        private System.Windows.Forms.Label connectionNameLabel;
        private System.Windows.Forms.Label connectionSpacingLabel;
        private System.Windows.Forms.Label profileSpacingLabel;
    }
}