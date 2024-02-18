// <copyright file="Oversize.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Oversize class.
    /// </summary>
    public class Oversize
    {
        /// <summary>
        /// The regular expression for parsing b3270 oversize specifications.
        /// </summary>
        private const string OversizeRegex = @"^(?<columns>\d+)x(?<rows>\d+)$";

        /// <summary>
        /// The maximum number of rows times columns.
        /// </summary>
        private const int MaxBufAddr = 0x4000;

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Parses a string into an <see cref="Oversize"/>.
        /// </summary>
        /// <param name="spec">Oversize specification.</param>
        /// <param name="oversize">Returned <see cref="Oversize"/>.</param>
        /// <returns>True if spec parsed successfully.</returns>
        public static bool TryParse(string spec, out Oversize oversize)
        {
            var regex = new Regex(OversizeRegex);
            var m = regex.Match(spec);
            if (!m.Success
                || !int.TryParse(m.Groups["rows"].Value, out int rows)
                || !int.TryParse(m.Groups["columns"].Value, out int columns)
                || rows == 0
                || columns == 0
                || rows * columns >= MaxBufAddr)
            {
                oversize = null;
                return false;
            }

            oversize = new Oversize { Rows = rows, Columns = columns };
            return true;
        }

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return $"{this.Columns}x{this.Rows}";
        }
    }
}
