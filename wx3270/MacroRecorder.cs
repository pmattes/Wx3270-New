// <copyright file="MacroRecorder.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// One macro.
    /// </summary>
    public class MacroRecorder
    {
        /// <summary>
        /// The flash interval.
        /// </summary>
        private const int FlashMs = 750;

        /// <summary>
        /// The flash timer.
        /// </summary>
        private readonly Timer flashTimer = new Timer();

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
        /// Starts recording.
        /// </summary>
        public void Start()
        {
            if (!this.running)
            {
                this.running = true;
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
