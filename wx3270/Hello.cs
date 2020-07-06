// <copyright file="Hello.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Wx3270.Contracts;

    /// <summary>
    /// Emulator hello message handler.
    /// </summary>
    public class Hello : BackEndEvent, IHello
    {
        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Backing field for <see cref="Version"/>.
        /// </summary>
        private string version = string.Empty;

        /// <summary>
        /// Backing field for <see cref="Build"/>.
        /// </summary>
        private string build = string.Empty;

        /// <summary>
        /// Backing field for <see cref="Copyright"/>.
        /// </summary>
        private string copyright = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hello"/> class.
        /// </summary>
        public Hello()
        {
            this.Def = new[] { new BackEndEventDef(B3270.Indication.Hello, this.StartHello) };
        }

        /// <inheritdoc />
        public string Version
        {
            get
            {
                lock (this.sync)
                {
                    return this.version;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.version = value;
                }
            }
        }

        /// <inheritdoc />
        public string Build
        {
            get
            {
                lock (this.sync)
                {
                    return this.build;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.build = value;
                }
            }
        }

        /// <inheritdoc />
        public string Copyright
        {
            get
            {
                lock (this.sync)
                {
                    return this.copyright;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.copyright = value;
                }
            }
        }

        /// <summary>
        /// Process a hello indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Element attributes.</param>
        private void StartHello(string name, AttributeDict attributes)
        {
            this.Version = attributes[B3270.Attribute.Version];
            this.Build = attributes[B3270.Attribute.Build];
            this.Copyright = attributes[B3270.Attribute.Copyright].Replace("\n", Environment.NewLine).Replace(Environment.NewLine + "\r", Environment.NewLine);

            // Signal the event.
            this.Change();
        }
    }
}
