namespace Wx3270
{
    partial class FileTransferProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileTransferProgress));
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.bytesLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.tapePictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.tapePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(206, 176);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(96, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel Transfer";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.progressLabel.Location = new System.Drawing.Point(23, 26);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(139, 15);
            this.progressLabel.TabIndex = 1;
            this.progressLabel.Text = "`File transfer in progress";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(23, 47);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(102, 15);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "`Status: Whatever";
            // 
            // bytesLabel
            // 
            this.bytesLabel.AutoSize = true;
            this.bytesLabel.Location = new System.Drawing.Point(23, 69);
            this.bytesLabel.Name = "bytesLabel";
            this.bytesLabel.Size = new System.Drawing.Size(129, 15);
            this.bytesLabel.TabIndex = 3;
            this.bytesLabel.Text = "`Bytes transferred: 127";
            // 
            // okButton
            // 
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(308, 176);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // animationTimer
            // 
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
            // 
            // tapePictureBox
            // 
            this.tapePictureBox.Image = global::Wx3270.Properties.Resources.TapePair1;
            this.tapePictureBox.Location = new System.Drawing.Point(26, 108);
            this.tapePictureBox.Name = "tapePictureBox";
            this.tapePictureBox.Size = new System.Drawing.Size(75, 91);
            this.tapePictureBox.TabIndex = 5;
            this.tapePictureBox.TabStop = false;
            this.tapePictureBox.Tag = "0";
            // 
            // FileTransferProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(131)))), ((int)(((byte)(91)))));
            this.ClientSize = new System.Drawing.Size(395, 220);
            this.Controls.Add(this.tapePictureBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.bytesLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.cancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileTransferProgress";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "File Transfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileTransferProgress_FormClosing);
            this.Load += new System.EventHandler(this.FileTransferProgressLoad);
            ((System.ComponentModel.ISupportInitialize)(this.tapePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label bytesLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox tapePictureBox;
        private System.Windows.Forms.Timer animationTimer;
    }
}