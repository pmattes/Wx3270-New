// <copyright file="TlsState.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// TLS state message handler.
    /// </summary>
    public class TlsState : BackEndEvent
    {
        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Backing field for <see cref="SessionInfo"/>.
        /// </summary>
        private string sessionInfo = string.Empty;

        /// <summary>
        /// Backing field for <see cref="HostCertificate"/>.
        /// </summary>
        private string hostCertificate = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TlsState"/> class.
        /// </summary>
        public TlsState()
        {
            this.Def = new[] { new BackEndEventDef("tls", this.StartTls) };
        }

        /// <summary>
        /// Gets the session information.
        /// </summary>
        public string SessionInfo
        {
            get
            {
                lock (this.sync)
                {
                    return this.sessionInfo;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.sessionInfo = value;
                }
            }
        }

        /// <summary>
        /// Gets the host certificate.
        /// </summary>
        public string HostCertificate
        {
            get
            {
                lock (this.sync)
                {
                    return this.hostCertificate;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.hostCertificate = value;
                }
            }
        }

        /// <summary>
        /// Process a TLS indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Element attributes.</param>
        private void StartTls(string name, AttributeDict attributes)
        {
            if (attributes[B3270.Attribute.Secure] == B3270.Value.True)
            {
                this.SessionInfo = attributes[B3270.Attribute.Session];
                this.HostCertificate = attributes[B3270.Attribute.HostCert];
            }
            else
            {
                this.SessionInfo = string.Empty;
                this.HostCertificate = string.Empty;
            }

            this.Change();
        }
    }
}
