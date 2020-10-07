// <copyright file="ColorBackground.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Color background element.
    /// </summary>
    public class ColorBackground
    {
        /// <summary>
        /// Symbolic name for the selection background.
        /// </summary>
        private const string SelectBackground = "SelectBackground";

        /// <summary>
        /// The standard colors.
        /// </summary>
        private static Tuple<string, string>[] colors =
        {
            new Tuple<string, string>("Neutral Black (screen background)", HostColor.NeutralBlack.ToString()),
            new Tuple<string, string>("Neutral White", HostColor.NeutralWhite.ToString()),
            new Tuple<string, string>("Blue", HostColor.Blue.ToString()),
            new Tuple<string, string>("Red", HostColor.Red.ToString()),
            new Tuple<string, string>("Green", HostColor.Green.ToString()),
            new Tuple<string, string>("Turquoise", HostColor.Turquoise.ToString()),
            new Tuple<string, string>("Yellow", HostColor.Yellow.ToString()),
            new Tuple<string, string>("Pink", HostColor.Pink.ToString()),
            new Tuple<string, string>("Black", HostColor.Black.ToString()),
            new Tuple<string, string>("Deep Blue", HostColor.DeepBlue.ToString()),
            new Tuple<string, string>("Orange", HostColor.Orange.ToString()),
            new Tuple<string, string>("Purple", HostColor.Purple.ToString()),
            new Tuple<string, string>("Pale Green", HostColor.PaleGreen.ToString()),
            new Tuple<string, string>("Pale Turquoise", HostColor.PaleTurquoise.ToString()),
            new Tuple<string, string>("Gray", HostColor.Grey.ToString()),
            new Tuple<string, string>("White", HostColor.White.ToString()),
            new Tuple<string, string>("Selected text background", SelectBackground),
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
        public static IEnumerable<ColorBackground> Objects()
        {
            return colors.Select(c => new ColorBackground { DisplayName = c.Item1, InternalName = c.Item2 });
        }

        /// <summary>
        /// Get the color value.
        /// </summary>
        /// <param name="colors">Edited colors to select from.</param>
        /// <returns>Color value.</returns>
        public Color ColorValue(Colors colors)
        {
            if (this.InternalName == SelectBackground)
            {
                return colors.SelectBackground;
            }

            return colors.HostColors[(HostColor)Enum.Parse(typeof(HostColor), this.InternalName)];
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
