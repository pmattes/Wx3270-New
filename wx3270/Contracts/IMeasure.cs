// <copyright file="IMeasure.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Drawing;

    /// <summary>
    /// Text measuring interface.
    /// </summary>
    public interface IMeasure
    {
        /// <summary>
        /// Measure a piece of text.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Text dimensions.</returns>
        public Size MeasureText(Graphics graphics, string text);
    }
}
