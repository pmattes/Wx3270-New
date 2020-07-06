// <copyright file="IHostPrefix.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Host prefix processor.
    /// </summary>
    public interface IHostPrefix : IBackEndEvent
    {
        /// <summary>
        /// Gets the host prefixes.
        /// </summary>
        string Prefixes { get; }
    }
}
