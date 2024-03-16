// <copyright file="Colors.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Drawing;

    /// <summary>
    /// Color definitions.
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Colors"/> class.
        /// </summary>
        public Colors()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Colors"/> class.
        /// </summary>
        /// <param name="other">Existing colors</param>
        public Colors(Colors other)
        {
            this.HostColors = new HostColors(other.HostColors);
            this.MonoColors = new MonoColors(other.MonoColors);
            this.SelectBackground = other.SelectBackground;
            this.CrosshairColor = other.CrosshairColor;
        }

        /// <summary>
        /// Gets or sets the host colors.
        /// </summary>
        public HostColors HostColors { get; set; }

        /// <summary>
        /// Gets or sets the monochrome colors.
        /// </summary>
        public MonoColors MonoColors { get; set; }

        /// <summary>
        /// Gets or sets the select background color.
        /// </summary>
        public Color SelectBackground { get; set; }

        /// <summary>
        /// Gets or sets the crosshair cursor color.
        /// </summary>
        public Color CrosshairColor { get; set; }

        /// <summary>
        /// Equality comparer.
        /// </summary>
        /// <param name="other">Other colors</param>
        /// <returns>True if the colors are equal</returns>
        public bool Equals(Colors other)
        {
            if (other == null)
            {
                return false;
            }

            return this.HostColors.Equals(other.HostColors)
                && this.MonoColors.Equals(other.MonoColors)
                && this.SelectBackground.Equals(other.SelectBackground)
                && this.CrosshairColor.Equals(other.CrosshairColor);
        }
    }
}
