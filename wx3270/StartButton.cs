// <copyright file="StartButton.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Start button renderer.
    /// </summary>
    public class StartButton
    {
        /// <summary>
        /// The font name.
        /// </summary>
        private const string FontName = "Arial";

        /// <summary>
        /// The larger font size.
        /// </summary>
        private const float LargeFontSize = 4.75F;

        /// <summary>
        /// The smaller font size.
        /// </summary>
        private const float SmallFontSize = 3.5F;

        /// <summary>
        /// The minimum width of the start button.
        /// </summary>
        private const int MinButtonWidth = 24;

        /// <summary>
        /// The width of a moderately-expanded start button.
        /// </summary>
        private const int MediumButtonWidth = 32;

        /// <summary>
        /// The maximum width of the start button.
        /// </summary>
        private const int MaxButtonWidth = 48;

        /// <summary>
        /// Pixels to offset the text vertically.
        /// </summary>
        private const int Yoffset = 2;

        /// <summary>
        /// True if the resize computation has been completed.
        /// </summary>
        private bool scaled = false;

        /// <summary>
        /// Possibly-truncated text.
        /// </summary>
        private string text;

        /// <summary>
        /// Font size.
        /// </summary>
        private float fontSize = LargeFontSize;

        /// <summary>
        /// Render the start button.
        /// </summary>
        /// <param name="pictureBox">Picture box.</param>
        /// <param name="e">Event arguments.</param>
        /// <param name="localText">Localized text.</param>
        public void Render(PictureBox pictureBox, PaintEventArgs e, string localText)
        {
            Font font = null;
            if (!this.scaled)
            {
                font = new Font(FontName, this.fontSize);
                this.text = localText;

                // Possibly shrink the font.
                var textSize = Measure(e.Graphics, font, this.text);
                if (textSize.Width > MediumButtonWidth)
                {
                    this.fontSize = SmallFontSize;
                    font.Dispose();
                    font = new Font(FontName, this.fontSize);
                    textSize = Measure(e.Graphics, font, this.text);
                }

                // Expand the start button to fit.
                if (textSize.Width > MinButtonWidth)
                {
                    pictureBox.Width = Math.Min(MaxButtonWidth, textSize.Width);
                }
                else
                {
                    pictureBox.Width = MinButtonWidth;
                }

                this.scaled = true;
            }

            if (font == null)
            {
                font = new Font(FontName, this.fontSize);
            }

            // Render the text, centered without wrapping.
            e.Graphics.DrawString(
                this.text,
                font,
                Brushes.Moccasin,
                new RectangleF(e.ClipRectangle.X, e.ClipRectangle.Y + Yoffset, pictureBox.Width, e.ClipRectangle.Height - Yoffset),
                new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap });
            font.Dispose();
        }

        /// <summary>
        /// Measure a piece of text.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="font">Font to measure with.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Measured size.</returns>
        private static Size Measure(Graphics graphics, Font font, string text)
        {
            return TextRenderer.MeasureText(graphics, text, font, new Size(128, 128), TextFormatFlags.Left);
        }
    }
}
