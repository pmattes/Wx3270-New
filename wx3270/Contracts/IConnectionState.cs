// <copyright file="IConnectionState.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Connection state interface.
    /// </summary>
    public interface IConnectionState
    {
        /// <summary>
        /// Gets the connection state.
        /// </summary>
        ConnectionState ConnectionState { get; }
    }
}