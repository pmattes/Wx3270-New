// <copyright file="ISimplifiedRegistry.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Simplified registry interface.
    /// </summary>
    public interface ISimplifiedRegistry
    {
        /// <summary>
        /// Creates a sub-key for the current user with the given name.
        /// </summary>
        /// <param name="name">Sub-key name.</param>
        /// <param name="baseName">Optional base name.</param>
        /// <returns>Created or existing key.</returns>
        public ISimplifiedRegistryKey CurrentUserCreateSubKey(string name, string baseName = null);
    }
}