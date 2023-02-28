// <copyright file="ICell.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Drawing;

    /// <summary>
    /// Character cell measurer.
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// Computes the horizontal fraction of the location within a cell.
        /// </summary>
        /// <param name="location">Coordinates to test.</param>
        /// <returns>Horizontal fraction, 0.0 (left edge) to 1.0 (right edge).</returns>
        public double HorizontalFraction(Point location);

        /// <summary>
        /// Computes the vertical fraction of the location within a cell.
        /// </summary>
        /// <param name="location">Coordinates to test.</param>
        /// <returns>Vertical fraction, 0.0 (top edge) to 1.0 (bottom edge).</returns>
        public double VerticalFraction(Point location);

        /// <summary>
        /// Tests whether a column is the rightmost.
        /// </summary>
        /// <param name="column0">Column to test.</param>
        /// <returns>True if rightmost.</returns>
        public bool IsLastColumn0(int column0);
    }
}
