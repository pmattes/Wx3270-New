// <copyright file="IConnectAttempt.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Emulator connect-attempt message handler.
    /// </summary>
    public interface IConnectAttempt : IBackEndEvent
    {
        /// <summary>
        /// Gets the host address.
        /// </summary>
        string HostAddress { get; }

        /// <summary>
        /// Gets the host port.
        /// </summary>
        int HostPort { get; }
    }
}
