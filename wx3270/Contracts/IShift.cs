// <copyright file="IShift.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Shift-key processing interface.
    /// </summary>
    public interface IShift
    {
        /// <summary>
        /// Gets the current keyboard modifier.
        /// </summary>
        KeyboardModifier Mod { get; }

        /// <summary>
        /// The keyboard modifiers changed.
        /// </summary>
        /// <param name="mod">New modifiers</param>
        /// <param name="mask">New modifier mask</param>
        void ModChanged(KeyboardModifier mod, KeyboardModifier mask);
    }
}