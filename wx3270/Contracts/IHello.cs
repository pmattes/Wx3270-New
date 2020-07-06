// <copyright file="IHello.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Emulator hello message handler.
    /// </summary>
    public interface IHello : IBackEndEvent
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the build string.
        /// </summary>
        string Build { get; }

        /// <summary>
        /// Gets the copyright notice.
        /// </summary>
        string Copyright { get; }
    }
}
