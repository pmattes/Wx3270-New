// <copyright file="ScreenDraw.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The main screen class.
    /// </summary>
    public partial class MainScreen
    {
        /// <summary>
        /// The screen drawing object.
        /// </summary>
        private ScreenBox screenBox;

        /// <summary>
        /// Cause the screen to be drawn.
        /// </summary>
        /// <param name="why">Reason for redraw.</param>
        /// <param name="complete">True for complete redraw.</param>
        public void ScreenNeedsDrawing(string why, bool complete)
        {
            if (this.screenBox != null)
            {
                this.screenBox.ScreenNeedsDrawing($"ScreenDraw({why})", complete);
            }
        }

        /// <summary>
        /// Process a font change.
        /// </summary>
        /// <param name="font">New font.</param>
        private void ScreenNewFont(Font font)
        {
            this.screenBox.ScreenNewFont(font, this.App.ScreenImage);
        }

        /// <summary>
        /// The screen needs to be redrawn.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenDraw(object sender, PaintEventArgs e)
        {
            this.screenBox.ScreenDraw(
                sender,
                e,
                this.App.ScreenImage,
                this.colors);
        }

        /// <summary>
        /// The crosshair margin needs to be redrawn.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CrosshairDraw(object sender, PaintEventArgs e)
        {
            this.screenBox.CrosshairDraw(
                sender,
                e,
                this.App.ScreenImage,
                this.colors);
        }
    }
}
