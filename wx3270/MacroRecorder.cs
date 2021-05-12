// <copyright file="MacroRecorder.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Macro recorder.
    /// </summary>
    public class MacroRecorder
    {
        /// <summary>
        /// The flash interval.
        /// </summary>
        private const int FlashMs = 350;

        /// <summary>
        /// The flash timer.
        /// </summary>
        private readonly Timer flashTimer = new Timer();

        /// <summary>
        /// The accumulated actions.
        /// </summary>
        private readonly StringBuilder actions = new StringBuilder();

        /// <summary>
        /// True if the recorder is active.
        /// </summary>
        private bool running;

        /// <summary>
        /// True if the image is flashing.
        /// </summary>
        private bool flashing;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroRecorder"/> class.
        /// </summary>
        public MacroRecorder()
        {
        }

        /// <summary>
        /// Recording stop event.
        /// </summary>
        public event Action<string, string> StopEvent = (text, name) => { };

        /// <summary>
        /// Gets or sets the picture box to flash.
        /// </summary>
        public PictureBox FlashPicture { get; set; }

        /// <summary>
        /// Gets or sets the image to display when on (flashing).
        /// </summary>
        public Image OnImage { get; set; }

        /// <summary>
        /// Gets or sets the image to display when off (not flashing).
        /// </summary>
        public Image OffImage { get; set; }

        /// <summary>
        /// Gets a value indicating whether the recorder is running.
        /// </summary>
        public bool Running => this.running;

        /// <summary>
        /// Gets or sets the macro name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Starts recording.
        /// </summary>
        public void Start()
        {
            if (!this.running)
            {
                this.running = true;
                this.actions.Clear();
                this.FlashPicture.Image = this.OnImage;
                this.flashing = true;
                this.flashTimer.Interval = FlashMs;
                this.flashTimer.Tick += this.DoFlash;
                this.flashTimer.Start();
            }
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        public void Stop()
        {
            if (this.running)
            {
                this.flashTimer.Stop();
                this.flashTimer.Tick -= this.DoFlash;
                this.running = false;
                this.FlashPicture.Image = this.OffImage;
                this.flashing = false;

                this.StopEvent(this.actions.ToString(), this.Name);
                this.Name = string.Empty;
            }
        }

        /// <summary>
        /// Record a set of actions.
        /// </summary>
        /// <param name="actions">Actions to record.</param>
        public void Record(string actions)
        {
            if (this.running)
            {
                this.actions.Append(actions + Environment.NewLine);
            }
        }

        /// <summary>
        /// Processes a flash.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        private void DoFlash(object sender, EventArgs args)
        {
            this.FlashPicture.Image = this.flashing ? this.OffImage : this.OnImage;
            this.flashing = !this.flashing;
        }
    }
}
