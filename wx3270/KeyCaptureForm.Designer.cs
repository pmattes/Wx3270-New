namespace Wx3270
{
    partial class KeyCaptureForm
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
            this.pressAnyKeyLabel = new System.Windows.Forms.Label();
            this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.layoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pressAnyKeyLabel
            // 
            this.pressAnyKeyLabel.AutoSize = true;
            this.pressAnyKeyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pressAnyKeyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pressAnyKeyLabel.ForeColor = System.Drawing.Color.White;
            this.pressAnyKeyLabel.Location = new System.Drawing.Point(0, 0);
            this.pressAnyKeyLabel.Margin = new System.Windows.Forms.Padding(0);
            this.pressAnyKeyLabel.Name = "pressAnyKeyLabel";
            this.pressAnyKeyLabel.Size = new System.Drawing.Size(170, 86);
            this.pressAnyKeyLabel.TabIndex = 0;
            this.pressAnyKeyLabel.Text = "Press any key\r\nor click to cancel";
            this.pressAnyKeyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pressAnyKeyLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.KeyCapture_MouseClick);
            // 
            // layoutPanel
            // 
            this.layoutPanel.BackColor = System.Drawing.Color.Red;
            this.layoutPanel.ColumnCount = 1;
            this.layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutPanel.Controls.Add(this.pressAnyKeyLabel, 0, 0);
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.RowCount = 1;
            this.layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layoutPanel.Size = new System.Drawing.Size(170, 86);
            this.layoutPanel.TabIndex = 4;
            // 
            // KeyCaptureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(252, 196);
            this.Controls.Add(this.layoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyCaptureForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "KeyCapture";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyCapture_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyCapture_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.KeyCapture_MouseClick);
            this.layoutPanel.ResumeLayout(false);
            this.layoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label pressAnyKeyLabel;
        private System.Windows.Forms.TableLayoutPanel layoutPanel;
    }
}