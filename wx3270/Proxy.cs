// <copyright file="Proxy.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using I18nBase;

    /// <summary>
    /// Proxy description.
    /// </summary>
    public class Proxy : Tuple<string, int?, bool>
    {
        /// <summary>
        /// Name of the model dimensions string.
        /// </summary>
        private static readonly string NoneName = I18n.Combine("Proxy", "none");

        /// <summary>
        /// Initializes a new instance of the <see cref="Proxy"/> class.
        /// </summary>
        /// <param name="name">Model number.</param>
        /// <param name="defaultPort">Default TCP port.</param>
        /// <param name="takesUsername">True if the proxy takes a user name parameter.</param>
        public Proxy(string name, int? defaultPort, bool takesUsername)
            : base(name, defaultPort, takesUsername)
        {
        }

        /// <summary>
        /// Gets the name of the "none" element.
        /// </summary>
        public static string None => I18n.Get(NoneName);

        /// <summary>
        /// Gets the proxy name.
        /// </summary>
        public string Name => this.Item1;

        /// <summary>
        /// Gets the default port.
        /// </summary>
        public int? DefaultPort => this.Item2;

        /// <summary>
        /// Gets a value indicating whether the proxy takes a user name parameter.
        /// </summary>
        public bool TakesUsername => this.Item3;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(NoneName, "none");
        }

        /// <summary>
        /// Converts a <see cref="Proxy"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
