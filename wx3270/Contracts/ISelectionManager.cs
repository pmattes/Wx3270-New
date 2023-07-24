// <copyright file="ISelectionManager.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Drawing;

    /// <summary>
    /// Selection manager.
    /// </summary>
    public interface ISelectionManager
    {
        /// <summary>
        /// The containing form was activated.
        /// </summary>
        void Activated();

        /// <summary>
        /// Process a mouse button down event.
        /// </summary>
        /// <param name="row1">1-origin row.</param>
        /// <param name="column1">1-origin column.</param>
        /// <param name="shift">True if shift key is presses.</param>
        /// <param name="location">Untranslated location.</param>
        void MouseDown(int row1, int column1, bool shift, Point location);

        /// <summary>
        /// Process a mouse move event.
        /// </summary>
        /// <param name="cell">Cell measurer.</param>
        /// <param name="row1">1-origin row.</param>
        /// <param name="column1">1-origin column.</param>
        /// <param name="location">Untranslated location.</param>
        void MouseMove(ICell cell, int row1, int column1, Point location);

        /// <summary>
        /// Process a mouse up event.
        /// </summary>
        /// <param name="row1">1-origin row.</param>
        /// <param name="column1">1-origin column.</param>
        void MouseUp(int row1, int column1);

        /// <summary>
        /// Clear the selection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Copy to clipboard.
        /// </summary>
        /// <param name="result">Returned result.</param>
        /// <returns>Pass-through result.</returns>
        PassthruResult Copy(out string result);

        /// <summary>
        /// Paste from clipboard.
        /// </summary>
        /// <param name="result">Returned result.</param>
        /// <returns>Pass-through result.</returns>
        PassthruResult Paste(out string result);

        /// <summary>
        /// Cut to clipboard.
        /// </summary>
        /// <param name="result">Returned result.</param>
        /// <returns>Pass-through result.</returns>
        PassthruResult Cut(out string result);
    }
}
