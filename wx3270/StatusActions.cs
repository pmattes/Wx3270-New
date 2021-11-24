// <copyright file="StatusActions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Status tab of the actions dialog.
    /// </summary>
    public partial class Actions
    {
        /// <summary>
        /// Name of status translations.
        /// </summary>
        private static readonly string StatusTitle = I18n.Combine(nameof(Actions), "Status");

        /// <summary>
        /// Friendly names for connection states, not localized.
        /// </summary>
        private static readonly Dictionary<ConnectionState, string> UntranslatedConnectionNames = new Dictionary<ConnectionState, string>
        {
            { ConnectionState.NotConnected, "Not connected" },
            { ConnectionState.Reconnecting, "Reconnecting" },
            { ConnectionState.Resolving, "Resolving host name" },
            { ConnectionState.TcpPending, "Waiting for TCP connection to complete" },
            { ConnectionState.TlsPending, "Waiting for TLS negotiation to complete" },
            { ConnectionState.ProxyPending, "Waiting for proxy negotiation to complete" },
            { ConnectionState.TelnetPending, "Waiting for TELNET negotiation to complete" },
            { ConnectionState.ConnectedNvt, "NVT line mode" },
            { ConnectionState.ConnectedNvtCharmode, "NVT character mode" },
            { ConnectionState.Connected3270, "TN3270 3270 mode" },
            { ConnectionState.ConnectedUnbound, "TN3270E unbound mode" },
            { ConnectionState.ConnectedEnvt, "TN3270E NVT mode" },
            { ConnectionState.ConnectedEsscp, "TN3270E SSCP-LU mode" },
            { ConnectionState.ConnectedTn3270e, "TN3270E 3270 mode" },
        };

        /// <summary>
        /// Friendly names for connection states.
        /// </summary>
        private static readonly Dictionary<ConnectionState, string> ConnectionNames = new Dictionary<ConnectionState, string>();

        /// <summary>
        /// Timestamp of the start of the connection.
        /// </summary>
        private DateTime connectionTime;

        /// <summary>
        /// TLS state object.
        /// </summary>
        private TlsState tlsState;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            foreach (var kv in UntranslatedConnectionNames)
            {
                ConnectionNames[kv.Key] = I18n.LocalizeGlobal(I18n.Combine(nameof(Actions), "ConnectionState", kv.Key.ToString()), kv.Value);
            }

            I18n.LocalizeGlobal(TlsMode.SecureVerified, "Secure, host verified");
            I18n.LocalizeGlobal(TlsMode.SecureUnverified, "Secure, host not verified");
            I18n.LocalizeGlobal(TlsMode.NotSecure, "Not secure");
        }

        /// <summary>
        /// Initialize the status tab.
        /// </summary>
        private void StatusTabInit()
        {
            this.Clear();
            this.app.Stats.Add(this.mainScreen, this.OnStats);
            this.mainScreen.ConnectionStateEvent += this.OnConnect;
            this.mainScreen.SslEvent += this.OnTls;
            this.mainScreen.LuEvent += this.OnLu;
            this.app.ConnectAttempt.Add(this.mainScreen, this.OnConnectAttempt);

            this.app.BackEnd.Register(this.tlsState = new TlsState());
            this.tlsSessionTextBox.Text = string.Empty;
            this.hostCertificateTextBox.Text = string.Empty;
            this.tlsSessionGroupBox.Enabled = false;
            this.tlsState.Add(this, this.TlsSessionChanged);

            if (ConnectionNames.Count == 0)
            {
                foreach (var kv in UntranslatedConnectionNames)
                {
                    ConnectionNames[kv.Key] = I18n.Get(I18n.Combine(nameof(Actions), "ConnectionState", kv.Key.ToString()));
                }
            }
        }

        /// <summary>
        /// Clear the statistics.
        /// </summary>
        /// <param name="includingAddress">True to clear out the address fields, too.</param>
        private void Clear(bool includingAddress = true)
        {
            this.hostValueLabel.Text = string.Empty;
            this.timeValueLabel.Text = string.Empty;
            this.modeValueLabel.Text = string.Empty;
            this.tlsModeValueLabel.Text = string.Empty;
            this.luNameValueLabel.Text = string.Empty;
            if (includingAddress)
            {
                this.addressValueLabel.Text = string.Empty;
                this.portValueLabel.Text = string.Empty;
            }

            this.bytesReceivedValueLabel.Text = "0";
            this.recordsReceivedValueLabel.Text = "0";
            this.bytesSentValueLabel.Text = "0";
            this.recordsSentValueLabel.Text = "0";
            this.connectionTimer.Enabled = false;
        }

        /// <summary>
        /// The statistics changed.
        /// </summary>
        private void OnStats()
        {
            var stats = this.app.Stats;
            this.bytesReceivedValueLabel.Text = stats.BytesReceived.ToString();
            this.recordsReceivedValueLabel.Text = stats.RecordsReceived.ToString();
            this.bytesSentValueLabel.Text = stats.BytesSent.ToString();
            this.recordsSentValueLabel.Text = stats.RecordsSent.ToString();
        }

        /// <summary>
        /// The connection state changed.
        /// </summary>
        private void OnConnect()
        {
            var state = this.app.ConnectionState;
            var connected = state != ConnectionState.NotConnected;
            if (state <= ConnectionState.TcpPending)
            {
                this.Clear(!connected);
            }
            else
            {
                this.statusGroupBox.Enabled = true;
                this.hostValueLabel.Text = this.app.CurrentHostIp;
                this.SetTls();
                if (!this.connectionTimer.Enabled)
                {
                    this.timeValueLabel.Text = "0:00";
                    this.connectionTime = DateTime.UtcNow;
                    this.connectionTimer.Enabled = true;
                }
            }

            if (connected)
            {
                this.modeValueLabel.Text = ConnectionNames[state];
            }

            this.statusGroupBox.Enabled = connected;
        }

        /// <summary>
        /// Set the TLS state.
        /// </summary>
        private void SetTls()
        {
            if (this.app.OiaState.Secure)
            {
                if (this.app.OiaState.Verified)
                {
                    this.tlsModeValueLabel.Text = I18n.Get(TlsMode.SecureVerified);
                }
                else
                {
                    this.tlsModeValueLabel.Text = I18n.Get(TlsMode.SecureUnverified);
                }

                this.tlsSessionGroupBox.Enabled = true;
                this.hostCertificateGroupBox.Enabled = true;
            }
            else
            {
                if (this.app.ConnectionState <= ConnectionState.TcpPending)
                {
                    this.tlsModeValueLabel.Text = string.Empty;
                }
                else
                {
                    this.tlsModeValueLabel.Text = I18n.Get(TlsMode.NotSecure);
                }

                this.tlsSessionGroupBox.Enabled = false;
                this.hostCertificateGroupBox.Enabled = false;
            }
        }

        /// <summary>
        /// The TLS state changed.
        /// </summary>
        private void OnTls()
        {
            this.SetTls();
        }

        /// <summary>
        /// The LU name changed.
        /// </summary>
        private void OnLu()
        {
            this.luNameValueLabel.Text = this.app.OiaState.Lu;
        }

        /// <summary>
        /// A connection attempt occurred.
        /// </summary>
        private void OnConnectAttempt()
        {
            this.addressValueLabel.Text = this.app.ConnectAttempt.HostAddress;
            this.portValueLabel.Text = this.app.ConnectAttempt.HostPort.ToString();
        }

        /// <summary>
        /// The connection timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void StatusTimerTick(object sender, EventArgs e)
        {
            var t = (DateTime.UtcNow - this.connectionTime) + new TimeSpan(0, 0, 0, 0, 500);
            t = new TimeSpan(t.Days, t.Hours, t.Minutes, t.Seconds, 0);
            this.timeValueLabel.Text = t.ToString("c");
            this.connectionTimer.Enabled = true;
        }

        /// <summary>
        /// The TLS session and certificate information changed.
        /// </summary>
        private void TlsSessionChanged()
        {
            this.tlsSessionTextBox.Text = this.tlsState.SessionInfo.Replace("\n", Environment.NewLine);
            this.hostCertificateTextBox.Text = this.tlsState.HostCertificate.Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Names of TLS mode localization strings.
        /// </summary>
        private class TlsMode
        {
            /// <summary>
            /// TLS message for secure, unverified connection.
            /// </summary>
            public static readonly string SecureUnverified = I18n.Combine(StatusTitle, "SecureUnverified");

            /// <summary>
            /// TLS message for secure, unverified connection.
            /// </summary>
            public static readonly string SecureVerified = I18n.Combine(StatusTitle, "SecureVerified");

            /// <summary>
            /// TLS message for non-secure connection.
            /// </summary>
            public static readonly string NotSecure = I18n.Combine(StatusTitle, "NotSecure");
        }
    }
}
