// <copyright file="IBackEndDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Host prefix processor.
    /// </summary>
    public interface IBackEndDb
    {
        /// <summary>
        /// Gets the code page database.
        /// </summary>
        public ICodePageDb CodePageDb { get; }

        /// <summary>
        /// Gets the models database.
        /// </summary>
        public IModelsDb ModelsDb { get; }

        /// <summary>
        /// Gets the proxies database.
        /// </summary>
        public IProxiesDb ProxiesDb { get; }

        /// <summary>
        /// Gets the host prefixes.
        /// </summary>
        public IHostPrefixDb HostPrefixDb { get; }
    }
}
