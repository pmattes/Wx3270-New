// <copyright file="IFlash.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Windows.Forms;

    /// <summary>
    /// Window flashing class.
    /// </summary>
    public interface IFlash
    {
        /// <summary>
        /// Flash the window.
        /// </summary>
        void Flash();

        /// <summary>
        /// The activation state of this window changed.
        /// </summary>
        /// <param name="form">Form that changed state.</param>
        /// <param name="activated">True if activated, false if deactivated.</param>
        void ActivationChange(Form form, bool activated);
    }
}
