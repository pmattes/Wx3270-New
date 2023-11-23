// <copyright file="TerminalName.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>using System;

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wx3270.Contracts;

    /// <summary>
    /// Terminal name processor.
    /// </summary>
    public class TerminalName : ITerminalName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalName"/> class.
        /// </summary>
        /// <param name="backEnd">Back end.</param>
        public TerminalName(IBackEnd backEnd)
        {
            backEnd.RegisterStart(B3270.Indication.TerminalName, this.NewTerminalName);
        }

        /// <summary>
        /// The name change event.
        /// </summary>
        private event Action<(string, bool)> NameEvent = (state) => { };

        /// <inheritdoc />
        public (string, bool)? NameAndOverride { get; private set; } = null;

        /// <inheritdoc />
        public void Register(Action<(string, bool)> handler)
        {
            if (this.NameAndOverride.HasValue)
            {
                handler(this.NameAndOverride.Value);
            }

            this.NameEvent += handler;
        }

        /// <summary>
        /// Process a start indication for the terminal name.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Attribute dictionary.</param>
        private void NewTerminalName(string name, AttributeDict attributes)
        {
            this.NameAndOverride = (attributes[B3270.Attribute.Text], attributes[B3270.Attribute.Override].Equals(B3270.Value.True));
            this.NameEvent(this.NameAndOverride.Value);
        }
    }
}
