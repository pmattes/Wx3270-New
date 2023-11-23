// <copyright file="KnownSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Miscellaneous settings.
    /// </summary>
    public class KnownSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wx3270.KnownSettings"/> class.
        /// </summary>
        public KnownSettings()
        {
            var known = typeof(B3270.Setting)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(info => info.IsLiteral && !info.IsInitOnly)
                .Select(info => info.GetRawConstantValue())
                .OfType<string>();
            this.Settings = new HashSet<string>(known);
        }

        /// <summary>
        /// Gets the known settings.
        /// </summary>
        public HashSet<string> Settings { get; private set; }
    }
}
