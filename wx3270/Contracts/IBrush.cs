// <copyright file="IBrush.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Drawing;

    /// <summary>
    /// Brush interface.
    /// </summary>
    public interface IBrush
    {
        /// <summary>
        /// Gets the screen font.
        /// </summary>
        public Font ScreenFont { get; }

        /// <summary>
        /// Gets the underline font.
        /// </summary>
        public Font UnderlineFont { get; }

        /// <summary>
        /// Gets the cell size.
        /// </summary>
        public Size CellSize { get; }

        /// <summary>
        /// Gets a brush for a given color.
        /// </summary>
        /// <param name="color">Color for brush.</param>
        /// <returns>Brush for that color.</returns>
        public Brush BrushFromColor(Color color);
    }
}