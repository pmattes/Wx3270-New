// <copyright file="TlsHello.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;

    using Wx3270.Contracts;

    /// <summary>
    /// TLS hello information.
    /// </summary>
    public class TlsHello : BackEndEvent, ITlsHello
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TlsHello"/> class.
        /// </summary>
        public TlsHello()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.TlsHello, this.ProcessTlsHello),
            };
            this.Options = new List<string>();
            this.Provider = "None";
        }

        /// <inheritdoc />
        public bool Supported { get; private set; }

        /// <inheritdoc />
        public ICollection<string> Options { get; private set; }

        /// <inheritdoc />
        public string Provider { get; private set; }

        /// <summary>
        /// Process a TLS hello indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessTlsHello(string name, AttributeDict attrs)
        {
            this.Supported = attrs[B3270.Attribute.Supported] == B3270.Value.True;
            if (this.Supported)
            {
                this.Options = attrs[B3270.Attribute.Options].Split(' ');
            }

            this.Provider = attrs[B3270.Attribute.Provider];

            // Signal the event.
            this.Change();
        }
    }
}
