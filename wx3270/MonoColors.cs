// <copyright file="MonoColors.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Drawing;

    /// <summary>
    /// Monochrome color definitions.
    /// </summary>
    public class MonoColors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoColors"/> class.
        /// </summary>
        public MonoColors()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoColors"/> class.
        /// </summary>
        /// <param name="colors">Existing dictionary</param>
        public MonoColors(MonoColors colors)
        {
            this.Normal = colors.Normal;
            this.Intensified = colors.Intensified;
            this.Background = colors.Background;
        }

        /// <summary>
        /// Gets or sets the normal color.
        /// </summary>
        public Color Normal { get; set; }

        /// <summary>
        /// Gets or sets the intensified color.
        /// </summary>
        public Color Intensified { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// Equality comparer.
        /// </summary>
        /// <param name="other">Other dictionary</param>
        /// <returns>True if the dictionaries are equal</returns>
        public bool Equals(MonoColors other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Normal == other.Normal
                && this.Intensified == other.Intensified
                && this.Background == other.Background;
        }
    }
}
