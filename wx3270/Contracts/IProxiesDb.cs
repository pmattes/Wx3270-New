// <copyright file="IProxiesDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;

    /// <summary>
    /// Handler for proxy indications from the back end.
    /// </summary>
    public interface IProxiesDb
    {
        /// <summary>
        /// Gets the dictionary of proxy definitions.
        /// </summary>
        public IReadOnlyDictionary<string, Proxy> Proxies { get; }
    }
}
