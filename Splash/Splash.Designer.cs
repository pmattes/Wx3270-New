namespace Splash
{
    partial class SplashForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            this.nameLabel = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fadeInScreenPanel = new System.Windows.Forms.Panel();
            this.terminalPictureBox = new System.Windows.Forms.PictureBox();
            this.halfOnPictureBox = new System.Windows.Forms.PictureBox();
            this.offTerminalPictureBox = new System.Windows.Forms.PictureBox();
            this.innerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pidTimer = new System.Windows.Forms.Timer(this.components);
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.outermostTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fadeTimer = new System.Windows.Forms.Timer(this.components);
            this.outerTableLayoutPanel.SuspendLayout();
            this.fadeInScreenPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.terminalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.halfOnPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offTerminalPictureBox)).BeginInit();
            this.innerTableLayoutPanel.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            this.outermostTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.nameLabel.Location = new System.Drawing.Point(3, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(180, 36);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "wx3270 1.2";
            this.nameLabel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // CopyrightLabel
            // 
            this.CopyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CopyrightLabel.AutoSize = true;
            this.CopyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CopyrightLabel.ForeColor = System.Drawing.Color.Beige;
            this.CopyrightLabel.Location = new System.Drawing.Point(3, 36);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(274, 20);
            this.CopyrightLabel.TabIndex = 2;
            this.CopyrightLabel.Text = "Copyright © 2016-2023 Paul Mattes";
            this.CopyrightLabel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 2;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.Controls.Add(this.fadeInScreenPanel, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.innerTableLayoutPanel, 1, 0);
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(100, 100);
            this.outerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(100);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 1;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(344, 64);
            this.outerTableLayoutPanel.TabIndex = 3;
            this.outerTableLayoutPanel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // fadeInScreenPanel
            // 
            this.fadeInScreenPanel.AutoSize = true;
            this.fadeInScreenPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fadeInScreenPanel.Controls.Add(this.terminalPictureBox);
            this.fadeInScreenPanel.Controls.Add(this.halfOnPictureBox);
            this.fadeInScreenPanel.Controls.Add(this.offTerminalPictureBox);
            this.fadeInScreenPanel.Location = new System.Drawing.Point(0, 0);
            this.fadeInScreenPanel.Margin = new System.Windows.Forms.Padding(0);
            this.fadeInScreenPanel.Name = "fadeInScreenPanel";
            this.fadeInScreenPanel.Size = new System.Drawing.Size(64, 64);
            this.fadeInScreenPanel.TabIndex = 6;
            this.fadeInScreenPanel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // terminalPictureBox
            // 
            this.terminalPictureBox.Image = global::Splash.Properties.Resources.wc3270_icon;
            this.terminalPictureBox.Location = new System.Drawing.Point(0, 0);
            this.terminalPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.terminalPictureBox.Name = "terminalPictureBox";
            this.terminalPictureBox.Size = new System.Drawing.Size(64, 64);
            this.terminalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.terminalPictureBox.TabIndex = 1;
            this.terminalPictureBox.TabStop = false;
            this.terminalPictureBox.Visible = false;
            this.terminalPictureBox.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // halfOnPictureBox
            // 
            this.halfOnPictureBox.Image = global::Splash.Properties.Resources.wc3270_icon_half_on;
            this.halfOnPictureBox.Location = new System.Drawing.Point(0, 0);
            this.halfOnPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.halfOnPictureBox.Name = "halfOnPictureBox";
            this.halfOnPictureBox.Size = new System.Drawing.Size(64, 64);
            this.halfOnPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.halfOnPictureBox.TabIndex = 3;
            this.halfOnPictureBox.TabStop = false;
            this.halfOnPictureBox.Visible = false;
            this.halfOnPictureBox.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // offTerminalPictureBox
            // 
            this.offTerminalPictureBox.Image = global::Splash.Properties.Resources.wx3270_off;
            this.offTerminalPictureBox.Location = new System.Drawing.Point(0, 0);
            this.offTerminalPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.offTerminalPictureBox.Name = "offTerminalPictureBox";
            this.offTerminalPictureBox.Size = new System.Drawing.Size(64, 64);
            this.offTerminalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.offTerminalPictureBox.TabIndex = 2;
            this.offTerminalPictureBox.TabStop = false;
            this.offTerminalPictureBox.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // innerTableLayoutPanel
            // 
            this.innerTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.innerTableLayoutPanel.AutoSize = true;
            this.innerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.innerTableLayoutPanel.ColumnCount = 1;
            this.innerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.innerTableLayoutPanel.Controls.Add(this.nameLabel, 0, 0);
            this.innerTableLayoutPanel.Controls.Add(this.CopyrightLabel, 0, 1);
            this.innerTableLayoutPanel.Location = new System.Drawing.Point(64, 8);
            this.innerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.innerTableLayoutPanel.Name = "innerTableLayoutPanel";
            this.innerTableLayoutPanel.RowCount = 2;
            this.innerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.innerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.innerTableLayoutPanel.Size = new System.Drawing.Size(280, 56);
            this.innerTableLayoutPanel.TabIndex = 4;
            this.innerTableLayoutPanel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // pidTimer
            // 
            this.pidTimer.Interval = 500;
            this.pidTimer.Tick += new System.EventHandler(this.PidTimer_Tick);
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.AutoSize = true;
            this.mainTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTableLayoutPanel.BackColor = System.Drawing.Color.Black;
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.Controls.Add(this.outerTableLayoutPanel, 0, 0);
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(1, 1);
            this.mainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 1;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(544, 264);
            this.mainTableLayoutPanel.TabIndex = 4;
            this.mainTableLayoutPanel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // outermostTableLayoutPanel
            // 
            this.outermostTableLayoutPanel.AutoSize = true;
            this.outermostTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outermostTableLayoutPanel.ColumnCount = 1;
            this.outermostTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outermostTableLayoutPanel.Controls.Add(this.mainTableLayoutPanel, 0, 0);
            this.outermostTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outermostTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outermostTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outermostTableLayoutPanel.Name = "outermostTableLayoutPanel";
            this.outermostTableLayoutPanel.Padding = new System.Windows.Forms.Padding(1);
            this.outermostTableLayoutPanel.RowCount = 1;
            this.outermostTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outermostTableLayoutPanel.Size = new System.Drawing.Size(1678, 1131);
            this.outermostTableLayoutPanel.TabIndex = 5;
            this.outermostTableLayoutPanel.Click += new System.EventHandler(this.Anywhere_Click);
            // 
            // fadeTimer
            // 
            this.fadeTimer.Enabled = true;
            this.fadeTimer.Interval = 200;
            this.fadeTimer.Tick += new System.EventHandler(this.FadeTimer_Tick);
            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(1678, 1131);
            this.Controls.Add(this.outermostTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Splash";
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.fadeInScreenPanel.ResumeLayout(false);
            this.fadeInScreenPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.terminalPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.halfOnPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offTerminalPictureBox)).EndInit();
            this.innerTableLayoutPanel.ResumeLayout(false);
            this.innerTableLayoutPanel.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.outermostTableLayoutPanel.ResumeLayout(false);
            this.outermostTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.PictureBox terminalPictureBox;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel innerTableLayoutPanel;
        private System.Windows.Forms.Timer pidTimer;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel outermostTableLayoutPanel;
        private System.Windows.Forms.Panel fadeInScreenPanel;
        private System.Windows.Forms.PictureBox offTerminalPictureBox;
        private System.Windows.Forms.PictureBox halfOnPictureBox;
        private System.Windows.Forms.Timer fadeTimer;
    }
}