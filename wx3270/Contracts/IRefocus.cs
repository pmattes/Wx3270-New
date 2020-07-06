// <copyright file="IRefocus.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Refocus interface. Class that is informed of undo/redo changes and updates the focus in the profile tree.
    /// </summary>
    public interface IRefocus
    {
        /// <summary>
        /// Inform the refocus that an undo or redo is about to happen.
        /// </summary>
        /// <param name="isUndo">True if undo, false if redo</param>
        void Inform(bool isUndo);
    }
}
