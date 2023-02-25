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
        /// Tests whether a location is in the left third of a character cell.
        /// </summary>
        /// <param name="location">Location to test.</param>
        /// <returns>True if in the left third.</returns>
        public bool WithinLeftThird(Point location);

        /// <summary>
        /// Tests whether a location is in the right third of a character cell.
        /// </summary>
        /// <param name="location">Location to test.</param>
        /// <returns>True if in the right third.</returns>
        public bool WithinRightThird(Point location);

        /// <summary>
        /// Tests whether a location is in the left half of a character cell.
        /// </summary>
        /// <param name="location">Location to test.</param>
        /// <returns>True if in the left half.</returns>
        public bool WithinLeftHalf(Point location);

        /// <summary>
        /// Tests whether a location is in the right half of a character cell.
        /// </summary>
        /// <param name="location">Location to test.</param>
        /// <returns>True if in the right half.</returns>
        public bool WithinRightHalf(Point location);

        /// <summary>
        /// Tests whether a location is in the top half of a character cell.
        /// </summary>
        /// <param name="location">Location to test.</param>
        /// <returns>True if in the top half.</returns>
        public bool WithinTopHalf(Point location);

        /// <summary>
        /// Tests whether a location is in the bottom half of a character cell.
        /// </summary>
        /// <param name="location">Location to test.</param>
        /// <returns>True if in the bottom half.</returns>
        public bool WithinBottomHalf(Point location);

        /// <summary>
        /// Tests whether a column is the rightmost.
        /// </summary>
        /// <param name="column0">Column to test.</param>
        /// <returns>True if rightmost.</returns>
        public bool IsLastColumn0(int column0);
    }
}
