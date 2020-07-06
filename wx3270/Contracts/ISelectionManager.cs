// <copyright file="ISelectionManager.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
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
        /// <param name="row1">1-origin row</param>
        /// <param name="column1">1-origin column</param>
        /// <param name="shift">True if shift key is presses</param>
        void MouseDown(int row1, int column1, bool shift);

        /// <summary>
        /// Process a mouse move event.
        /// </summary>
        /// <param name="row">1-origin row</param>
        /// <param name="column">1-origin column</param>
        void MouseMove(int row, int column);

        /// <summary>
        /// Process a mouse up event.
        /// </summary>
        /// <param name="row">1-origin row</param>
        /// <param name="column">1-origin column</param>
        void MouseUp(int row, int column);

        /// <summary>
        /// Clear the selection.
        /// </summary>
        void Clear();
    }
}
