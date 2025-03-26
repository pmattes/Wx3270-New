// <copyright file="ISimplifiedRegistryKey.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;

    /// <summary>
    /// Simplified registry key interface.
    /// </summary>
    public interface ISimplifiedRegistryKey : IDisposable
    {
        /// <summary>
        /// Gets the value of a sub-key.
        /// </summary>
        /// <param name="name">Sub-key name.</param>
        /// <returns>Key value.</returns>
        public object GetValue(string name);

        /// <summary>
        /// Sets the value of a sub-key.
        /// </summary>
        /// <param name="name">Sub-key name.</param>
        /// <param name="value">Key value.</param>
        public void SetValue(string name, object value);

        /// <summary>
        /// Deletes the value of a sub-key.
        /// </summary>
        /// <param name="name">Sub-key name.</param>
        public void DeleteValue(string name);

        /// <summary>
        /// Gets the set of value names.
        /// </summary>
        /// <returns>Array of value names.</returns>
        public string[] GetValueNames();

        /// <summary>
        /// Closes the sub-key.
        /// </summary>
        public void Close();
    }
}