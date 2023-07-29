// <copyright file="Splash.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Splash
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    public partial class SplashForm : Form
    {
        /// <summary>
        /// The parent's process ID, or -1.
        /// </summary>
        private readonly int pid = -1;

        /// <summary>
        /// The terminal fade-in phase.
        /// </summary>
        private int fadePhase = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashForm"/> class.
        /// </summary>
        /// <param name="pid">Process ID of parent process to watch.</param>
        /// <param name="version">Version number.</param>
        /// <param name="copyright">Copyright text.</param>
        public SplashForm(int pid = -1, string version = null, string copyright = null)
        {
            InitializeComponent();

            this.pid = pid;
            if (this.pid >= 0)
            {
                this.pidTimer.Start();
            }

            if (version != null)
            {
                this.nameLabel.Text = $"wx3270 {version}";
            }

            if (copyright != null)
            {
                this.CopyrightLabel.Text = $"{copyright}";
            }

            // Start with the off terminal.
            this.SetTerminalPhase();
        }

        /// <summary>
        /// Tick handler for the PID timer.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PidTimer_Tick(object sender, EventArgs e)
        {
            // If the process is gone, exit.
            try
            {
                var p = Process.GetProcessById(pid);
            }
            catch (ArgumentException)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Set the right terminal image to be visible.
        /// </summary>
        private void SetTerminalPhase()
        {
            this.fadeInScreenPanel.SuspendLayout();
            switch (this.fadePhase)
            {
                case 0:
                    this.offTerminalPictureBox.Visible = true;
                    this.halfOnPictureBox.Visible = false;
                    this.terminalPictureBox.Visible = false;
                    break;
                case 1:
                    this.halfOnPictureBox.Visible = true;
                    this.offTerminalPictureBox.Visible = false;
                    this.terminalPictureBox.Visible = false;
                    break;
                case 2:
                    this.terminalPictureBox.Visible = true;
                    this.offTerminalPictureBox.Visible = false;
                    this.halfOnPictureBox.Visible = false;
                    break;
            }

            this.fadeInScreenPanel.ResumeLayout();
        }

        /// <summary>
        /// Tick handler for the terminal image.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            this.SetTerminalPhase();
            if (this.fadePhase++ >= 2)
            {
                this.fadeTimer.Stop();
            }
        }

        /// <summary>
        /// The window was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Anywhere_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
