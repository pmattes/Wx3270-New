// <copyright file="IHostPrefixDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Host prefix processor.
    /// </summary>
    public interface IHostPrefixDb
    {
        /// <summary>
        /// Gets the host prefixes.
        /// </summary>
        string Prefixes { get; }
    }
}
