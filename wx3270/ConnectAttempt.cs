// <copyright file="ConnectAttempt.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Emulator connect-attempt message handler.
    /// </summary>
    public class ConnectAttempt : BackEndEvent, IConnectAttempt
    {
        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Backing field for <see cref="HostAddress"/>.
        /// </summary>
        private string hostAddress = string.Empty;

        /// <summary>
        /// Backing field for <see cref="HostPort"/>.
        /// </summary>
        private int hostPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectAttempt"/> class.
        /// </summary>
        public ConnectAttempt()
        {
            this.Def = new[] { new BackEndEventDef(B3270.Indication.ConnectAttempt, this.StartConnectAttempt) };
        }

        /// <inheritdoc />
        public string HostAddress
        {
            get
            {
                lock (this.sync)
                {
                    return this.hostAddress;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.hostAddress = value;
                }
            }
        }

        /// <inheritdoc />
        public int HostPort
        {
            get
            {
                lock (this.sync)
                {
                    return this.hostPort;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.hostPort = value;
                }
            }
        }

        /// <summary>
        /// Process a connect-attempt indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Element attributes.</param>
        private void StartConnectAttempt(string name, AttributeDict attributes)
        {
            this.HostAddress = attributes[B3270.Attribute.HostIp];
            this.HostPort = int.Parse(attributes[B3270.Attribute.Port]);
            this.Change();
        }
    }
}
