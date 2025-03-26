// <copyright file="RealRegistry.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Microsoft.Win32;
    using Wx3270.Contracts;

    /// <summary>
    /// Interface to the real registry.
    /// </summary>
    public class RealRegistry : ISimplifiedRegistry
    {
        /// <inheritdoc/>
        public ISimplifiedRegistryKey CurrentUserCreateSubKey(string name, string baseName = null)
        {
            return new RealRegistryKey(name);
        }

        /// <summary>
        /// Registry key class that uses the real registry.
        /// </summary>
        private class RealRegistryKey : ISimplifiedRegistryKey
        {
            /// <summary>
            /// The real registry key.
            /// </summary>
            private RegistryKey key;

            /// <summary>
            /// Initializes a new instance of the <see cref="RealRegistryKey"/> class.
            /// </summary>
            /// <param name="name">Sub-key name.</param>
            public RealRegistryKey(string name)
            {
                this.key = Registry.CurrentUser.CreateSubKey(name);
            }

            /// <summary>
            /// Disposes the key.
            /// </summary>
            public void Dispose()
            {
                this.key?.Dispose();
                this.key = null;
            }

            /// <inheritdoc/>
            public object GetValue(string name)
            {
                return this.key.GetValue(name);
            }

            /// <inheritdoc/>
            public void SetValue(string name, object value)
            {
                this.key.SetValue(name, value);
            }

            /// <inheritdoc/>
            public void DeleteValue(string name)
            {
                this.key.DeleteValue(name);
            }

            /// <inheritdoc/>
            public string[] GetValueNames()
            {
                return this.key.GetValueNames();
            }

            /// <inheritdoc/>
            public void Close()
            {
                this.key.Close();
            }
        }
    }
}