// <copyright file="UnpaddedLabel.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Class that implements labels with no padding. Useful if you want to mix fonts in what appears to be a single label,
    /// by putting a sequence of these into a FlowLayoutPanel.
    /// </summary>
    public class UnpaddedLabel : Label
    {
        /// <summary>
        /// True if there is an automatic resize operation in progress. Used to avoid unintended recursion.
        /// </summary>
        private bool resizing;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpaddedLabel"/> class.
        /// </summary>
        public UnpaddedLabel()
        {
            // Set triggers to do a resize whenever there is a manual size change, text change or font change.
            this.SizeChanged += (sender, args) => this.AutoResize(sender, args);
            this.TextChanged += (sender, args) => this.AutoResize(sender, args);
            this.FontChanged += (sender, args) => this.AutoResize(sender, args);

            // Turn off AutoSize, since it effectively ruins what we are trying to accomplish here.
            //
            // Users have to remember to turn off margins manually. That can't be done automatically here, because
            // users may want a left margin for the leftmost item and a right margin for the rightmost
            // item.
            //
            // Users also need to remember to turn off Padding in whatever container this is in.
            this.AutoSize = false;
        }

        /// <summary>
        /// Override for the Paint method.
        /// </summary>
        /// <param name="e">Paint event arguments.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Erase the background.
            using SolidBrush brush = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(brush, e.ClipRectangle);

            // Draw the text, with no padding.
            var proposedSize = new Size(int.MaxValue, int.MaxValue);
            Size unpaddedSize = TextRenderer.MeasureText(e.Graphics, this.Text, this.Font, proposedSize, TextFormatFlags.NoPadding);
            var drawRectangle = new Rectangle(new Point(0, 0), unpaddedSize);
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, drawRectangle, this.ForeColor, TextFormatFlags.NoPadding);
        }

        /// <summary>
        /// Do an automatic resize.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        private void AutoResize(object sender, EventArgs args)
        {
            if (this.resizing)
            {
                // Avoid recursion.
                return;
            }

            try
            {
                this.resizing = true;
                using Graphics g = this.CreateGraphics();
                var proposedSize = new Size(int.MaxValue, int.MaxValue);

                // Set the Size to the size of the text, without padding.
                this.Size = TextRenderer.MeasureText(g, this.Text, this.Font, proposedSize, TextFormatFlags.NoPadding);
            }
            finally
            {
                this.resizing = false;
            }
        }
    }
}
