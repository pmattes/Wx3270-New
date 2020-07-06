// <copyright file="ITlsHello.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// TLS hello information.
    /// </summary>
    public interface ITlsHello : IBackEndEvent
    {
        /// <summary>
        /// Gets a value indicating whether TLS is supported.
        /// </summary>
        bool Supported { get; }

        /// <summary>
        /// Gets the list of TLS options supported.
        /// </summary>
        ICollection<string> Options { get; }

        /// <summary>
        /// Gets the name of the TLS provider.
        /// </summary>
        string Provider { get; }
    }
}
