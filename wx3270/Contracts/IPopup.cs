// <copyright file="IPopup.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Wx3270.Contracts;

    /// <summary>
    /// Emulator pop-up message handler.
    /// </summary>
    public interface IPopup : IBackEndEvent
    {
        /// <summary>
        /// Asynchronous connect error event.
        /// </summary>
        event Action<string, bool> ConnectErrorEvent;
    }
}
