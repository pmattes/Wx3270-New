// <copyright file="HostPrefixDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Linq;
    using Wx3270.Contracts;

    /// <summary>
    /// Host prefix processor.
    /// </summary>
    public class HostPrefixDb : BackEndEvent, IHostPrefixDb
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostPrefixDb"/> class.
        /// </summary>
        /// <param name="invoke">Invoke interface for callbacks.</param>
        public HostPrefixDb()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.Prefixes, this.ProcessPrefixes),
            };
        }

        /// <summary>
        /// Gets the set of prefixes.
        /// </summary>
        public string Prefixes { get; private set; }

        /// <summary>
        /// Process a prefixes indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessPrefixes(string name, AttributeDict attrs)
        {
            // Make sure the prefixes are uppercase and in sorted order.
            this.Prefixes = new string(attrs[B3270.Attribute.Value].ToUpperInvariant().ToCharArray().OrderBy(c => c).ToArray());

            // Signal the event.
            this.Change();
        }
    }
}
