// <copyright file="IUpdate.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Control update notification interface.
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// Signal a screen update.
        /// </summary>
        /// <param name="updateType">Update type</param>
        void ScreenUpdate(ScreenUpdateType updateType);
    }
}