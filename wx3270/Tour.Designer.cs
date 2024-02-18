namespace Wx3270
{
    partial class Tour
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tour));
            this.arrowPictureBox = new System.Windows.Forms.PictureBox();
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.borderPanel = new System.Windows.Forms.Panel();
            this.innerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.topRowTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.bodyTextBox = new System.Windows.Forms.TextBox();
            this.buttonsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.stopToursButton = new System.Windows.Forms.Button();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.arrowPictureBox)).BeginInit();
            this.outerTableLayoutPanel.SuspendLayout();
            this.borderPanel.SuspendLayout();
            this.innerTableLayoutPanel.SuspendLayout();
            this.topRowTableLayoutPanel.SuspendLayout();
            this.buttonsTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // arrowPictureBox
            // 
            this.arrowPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.arrowPictureBox.Image = global::Wx3270.Properties.Resources.arrow_left_up2;
            this.arrowPictureBox.Location = new System.Drawing.Point(0, 0);
            this.arrowPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.arrowPictureBox.Name = "arrowPictureBox";
            this.arrowPictureBox.Size = new System.Drawing.Size(48, 48);
            this.arrowPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.arrowPictureBox.TabIndex = 1;
            this.arrowPictureBox.TabStop = false;
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.AutoSize = true;
            this.outerTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.outerTableLayoutPanel.ColumnCount = 2;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.Controls.Add(this.borderPanel, 1, 1);
            this.outerTableLayoutPanel.Controls.Add(this.arrowPictureBox, 0, 0);
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 2;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(466, 337);
            this.outerTableLayoutPanel.TabIndex = 2;
            // 
            // borderPanel
            // 
            this.borderPanel.AutoSize = true;
            this.borderPanel.BackColor = System.Drawing.Color.Maroon;
            this.borderPanel.Controls.Add(this.innerTableLayoutPanel);
            this.borderPanel.ForeColor = System.Drawing.Color.Maroon;
            this.borderPanel.Location = new System.Drawing.Point(51, 51);
            this.borderPanel.Name = "borderPanel";
            this.borderPanel.Padding = new System.Windows.Forms.Padding(3);
            this.borderPanel.Size = new System.Drawing.Size(412, 283);
            this.borderPanel.TabIndex = 3;
            // 
            // innerTableLayoutPanel
            // 
            this.innerTableLayoutPanel.AutoSize = true;
            this.innerTableLayoutPanel.BackColor = System.Drawing.Color.Linen;
            this.innerTableLayoutPanel.ColumnCount = 1;
            this.innerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 406F));
            this.innerTableLayoutPanel.Controls.Add(this.topRowTableLayoutPanel, 0, 0);
            this.innerTableLayoutPanel.Controls.Add(this.bodyTextBox, 0, 1);
            this.innerTableLayoutPanel.Controls.Add(this.buttonsTableLayoutPanel, 0, 2);
            this.innerTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.innerTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.innerTableLayoutPanel.Name = "innerTableLayoutPanel";
            this.innerTableLayoutPanel.RowCount = 3;
            this.innerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.innerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.innerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.innerTableLayoutPanel.Size = new System.Drawing.Size(406, 277);
            this.innerTableLayoutPanel.TabIndex = 3;
            // 
            // topRowTableLayoutPanel
            // 
            this.topRowTableLayoutPanel.ColumnCount = 2;
            this.topRowTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topRowTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.topRowTableLayoutPanel.Controls.Add(this.closeButton, 1, 0);
            this.topRowTableLayoutPanel.Controls.Add(this.titleLabel, 0, 0);
            this.topRowTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topRowTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.topRowTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topRowTableLayoutPanel.Name = "topRowTableLayoutPanel";
            this.topRowTableLayoutPanel.RowCount = 1;
            this.topRowTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.topRowTableLayoutPanel.Size = new System.Drawing.Size(406, 45);
            this.topRowTableLayoutPanel.TabIndex = 3;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.closeButton.AutoSize = true;
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(378, 9);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(25, 26);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "X";
            this.toolTip1.SetToolTip(this.closeButton, "Stop this tour");
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.QuitTourButtonClick);
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(80, 13);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(214, 18);
            this.titleLabel.TabIndex = 5;
            this.titleLabel.Text = "`Tour: wx3270 Main Screen";
            // 
            // bodyTextBox
            // 
            this.bodyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bodyTextBox.BackColor = System.Drawing.Color.Linen;
            this.bodyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bodyTextBox.Location = new System.Drawing.Point(6, 48);
            this.bodyTextBox.Margin = new System.Windows.Forms.Padding(6, 3, 3, 6);
            this.bodyTextBox.Multiline = true;
            this.bodyTextBox.Name = "bodyTextBox";
            this.bodyTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.bodyTextBox.Size = new System.Drawing.Size(397, 191);
            this.bodyTextBox.TabIndex = 3;
            this.bodyTextBox.Text = resources.GetString("bodyTextBox.Text");
            this.bodyTextBox.Enter += new System.EventHandler(this.TextBoxEnter);
            // 
            // buttonsTableLayoutPanel
            // 
            this.buttonsTableLayoutPanel.AutoSize = true;
            this.buttonsTableLayoutPanel.ColumnCount = 3;
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.buttonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.buttonsTableLayoutPanel.Controls.Add(this.stopToursButton, 1, 0);
            this.buttonsTableLayoutPanel.Controls.Add(this.previousButton, 0, 0);
            this.buttonsTableLayoutPanel.Controls.Add(this.nextButton, 2, 0);
            this.buttonsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsTableLayoutPanel.Location = new System.Drawing.Point(0, 245);
            this.buttonsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonsTableLayoutPanel.Name = "buttonsTableLayoutPanel";
            this.buttonsTableLayoutPanel.RowCount = 1;
            this.buttonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonsTableLayoutPanel.Size = new System.Drawing.Size(406, 32);
            this.buttonsTableLayoutPanel.TabIndex = 3;
            // 
            // stopToursButton
            // 
            this.stopToursButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.stopToursButton.AutoSize = true;
            this.stopToursButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stopToursButton.Location = new System.Drawing.Point(155, 3);
            this.stopToursButton.Name = "stopToursButton";
            this.stopToursButton.Size = new System.Drawing.Size(94, 26);
            this.stopToursButton.TabIndex = 1;
            this.stopToursButton.Text = "Stop all tours";
            this.toolTip1.SetToolTip(this.stopToursButton, "Stop displaying tours automatically");
            this.stopToursButton.UseVisualStyleBackColor = true;
            this.stopToursButton.Click += new System.EventHandler(this.StopToursButtonClick);
            // 
            // previousButton
            // 
            this.previousButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.previousButton.AutoSize = true;
            this.previousButton.Location = new System.Drawing.Point(3, 3);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(86, 26);
            this.previousButton.TabIndex = 0;
            this.previousButton.Text = "← Previous";
            this.toolTip1.SetToolTip(this.previousButton, "Move to the previous item on the tour");
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.PreviousClick);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nextButton.AutoSize = true;
            this.nextButton.Location = new System.Drawing.Point(317, 3);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(86, 26);
            this.nextButton.TabIndex = 2;
            this.nextButton.Text = "Next →";
            this.toolTip1.SetToolTip(this.nextButton, "Move to the next item on the tour");
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.NextClick);
            // 
            // Tour
            // 
            this.AcceptButton = this.nextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImage = global::Wx3270.Properties.Resources.Transparent1;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(1077, 450);
            this.ControlBox = false;
            this.Controls.Add(this.outerTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Tour";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Tour";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TourKeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.arrowPictureBox)).EndInit();
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            this.borderPanel.ResumeLayout(false);
            this.borderPanel.PerformLayout();
            this.innerTableLayoutPanel.ResumeLayout(false);
            this.innerTableLayoutPanel.PerformLayout();
            this.topRowTableLayoutPanel.ResumeLayout(false);
            this.topRowTableLayoutPanel.PerformLayout();
            this.buttonsTableLayoutPanel.ResumeLayout(false);
            this.buttonsTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox arrowPictureBox;
        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel innerTableLayoutPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.TableLayoutPanel buttonsTableLayoutPanel;
        private System.Windows.Forms.TextBox bodyTextBox;
        private System.Windows.Forms.Panel borderPanel;
        private System.Windows.Forms.Button stopToursButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel topRowTableLayoutPanel;
        private System.Windows.Forms.Button closeButton;
    }
}