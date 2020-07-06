// <copyright file="MonoBackground.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Monochrome background element.
    /// </summary>
    public class MonoBackground
    {
        /// <summary>
        /// Symbolic name for the screen background.
        /// </summary>
        private const string ScreenBackground = "ScreenBackground";

        /// <summary>
        /// Symbolic name for the selection background.
        /// </summary>
        private const string SelectBackground = "SelectBackground";

        /// <summary>
        /// The standard colors.
        /// </summary>
        private static Tuple<string, string>[] colors =
        {
            new Tuple<string, string>("Screen background", ScreenBackground),
            new Tuple<string, string>("Selection background", SelectBackground),
        };

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the internal name.
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// Gets the set of color objects.
        /// </summary>
        /// <returns>Set of color objects.</returns>
        public static IEnumerable<MonoBackground> Objects()
        {
            return colors.Select(c => new MonoBackground { DisplayName = c.Item1, InternalName = c.Item2 });
        }

        /// <summary>
        /// Get the color value.
        /// </summary>
        /// <param name="colors">Edited colors to select from.</param>
        /// <returns>Color value.</returns>
        public Color ColorValue(Colors colors)
        {
            switch (this.InternalName)
            {
                default:
                case ScreenBackground:
                    return colors.MonoColors.Background;
                case SelectBackground:
                    return colors.SelectBackground;
            }
        }

        /// <summary>
        /// Convert to a string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
