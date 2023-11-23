// <copyright file="ITerminalName.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>using System;

namespace Wx3270.Contracts
{
    using System;

    /// <summary>
    /// Setting change processor.
    /// </summary>
    public interface ITerminalName
    {
        /// <summary>
        /// Gets the terminal name and override.
        /// </summary>
        (string, bool)? NameAndOverride { get; }

        /// <summary>
        /// Register a filtered setting handler.
        /// </summary>
        /// <param name="handler">Handler delegate.</param>
        void Register(Action<(string, bool)> handler);
    }
}
