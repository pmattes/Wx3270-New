// <copyright file="Thumb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// Scrollbar thumb state.
    /// </summary>
    public class Thumb
    {
        /// <summary>
        /// Gets or sets the position of the top of the thumb, as a fraction.
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// Gets or sets the fraction of the scrollbar taken by the thumb.
        /// </summary>
        public float Shown { get; set; }

        /// <summary>
        /// Gets or sets the number of lines saved.
        /// </summary>
        public int Saved { get; set; }

        /// <summary>
        /// Gets or sets the number of lines on a screen.
        /// </summary>
        public int Screen { get; set; }

        /// <summary>
        /// Gets or sets the number of lines currently scrolled back.
        /// </summary>
        public int Back { get; set; }

        /// <summary>
        /// Clones a thumb.
        /// </summary>
        /// <returns>Cloned thumb</returns>
        public Thumb Clone()
        {
            return (Thumb)this.MemberwiseClone();
        }
    }
}
