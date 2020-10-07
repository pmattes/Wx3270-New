// <copyright file="Connect.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows.Forms;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for connection completion.
    /// </summary>
    /// <param name="success">True if connection completed successfully.</param>
    /// <param name="result">Result text.</param>"
    public delegate void ConnectComplete(bool success, string result);

    /// <summary>
    /// Connection management: connect, disconnect, login macro, reconnect.
    /// </summary>
    public class Connect
    {
        /// <summary>
        /// Localization category for message box titles.
        /// </summary>
        private static readonly string TitleName = I18n.TitleName(nameof(Connect));

        /// <summary>
        /// Localization category for message box messages.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Connect));

        /// <summary>
        /// The application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// The main screen.
        /// </summary>
        private readonly MainScreen mainScreen;

        /// <summary>
        /// The host we are connecting to.
        /// </summary>
        private HostEntry connectHostEntry;

        /// <summary>
        /// True if there is an asynchronous connection completion to run.
        /// </summary>
        private bool connectCompletePending;

        /// <summary>
        /// The pending connection completion.
        /// </summary>
        private ConnectComplete connectComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connect"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="mainScreen">Main screen.</param>
        public Connect(Wx3270App app, MainScreen mainScreen)
        {
            this.app = app;
            this.mainScreen = mainScreen;

            // Subscribe to connection state events.
            this.mainScreen.ConnectionStateEvent += this.HostConnectionChange;
        }

        /// <summary>
        /// Gets the host we are connecting to.
        /// </summary>
        public HostEntry ConnectHostEntry
        {
            get
            {
                return this.connectHostEntry;
            }

            private set
            {
                if (value != null)
                {
                    this.connectHostEntry = value;
                }
                else
                {
                    this.connectHostEntry = null;
                }
            }
        }

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private IBackEnd BackEnd => this.app.BackEnd;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.Connect, "Connect");
            I18n.LocalizeGlobal(Title.Disconnect, "Disconnect");

            I18n.LocalizeGlobal(Message.TlsNotSupported, "TLS is not supported");
            I18n.LocalizeGlobal(Message.TlsOptionsNotSupported, "TLS-specific option(s) ignored");
        }

        /// <summary>
        /// Connect to a host.
        /// </summary>
        /// <param name="entry">Host entry.</param>
        /// <returns>True if connection was successfully initiated.</returns>
        /// <param name="complete">Asynchronous completion delegate.</param>
        public bool ConnectToHost(HostEntry entry, ConnectComplete complete = null)
        {
            if (!this.app.TlsHello.Supported && entry.Prefixes.Contains(B3270.Prefix.TlsTunnel))
            {
                ErrorBox.Show(I18n.Get(Message.TlsNotSupported), I18n.Get(Title.Connect));
                return false;
            }

            var settings = new List<string>();

            switch (entry.PrinterSessionType)
            {
                case PrinterSessionType.None:
                    settings.AddRange(new[] { B3270.Setting.PrinterLu, string.Empty });
                    break;
                case PrinterSessionType.Associate:
                    settings.AddRange(new[] { B3270.Setting.PrinterLu, "." });
                    break;
                case PrinterSessionType.SpecificLu:
                    settings.AddRange(new[] { B3270.Setting.PrinterLu, entry.PrinterSessionLu });
                    break;
            }

            var tlsOptionsSupported = this.app.TlsHello.Options;
            var allAdded = AddToggleIfSupported(tlsOptionsSupported, settings, B3270.Setting.Tls, entry.AllowStartTls ? B3270.Value.True : B3270.Value.False)
                && AddToggleIfSupported(tlsOptionsSupported, settings, B3270.Setting.AcceptHostName, entry.AcceptHostName, required: true)
                && AddToggleIfSupported(tlsOptionsSupported, settings, B3270.Setting.ClientCert, entry.ClientCertificateName, required: true);
            if (!allAdded)
            {
                ErrorBox.Show(I18n.Get(Message.TlsOptionsNotSupported), I18n.Get(Title.Connect), MessageBoxIcon.Warning);
            }

            if (entry.AutoConnect == AutoConnect.Reconnect)
            {
                settings.AddRange(new[] { B3270.Setting.Reconnect, B3270.Value.True });
            }

            settings.AddRange(new[] { B3270.Setting.LoginMacro, CleanLoginMacro(entry.LoginMacro) });
            settings.AddRange(new[] { B3270.Setting.NoTelnetInputMode, entry.NoTelnetInputType.ToString() });

            var actions = new List<BackEndAction>() { new BackEndAction(B3270.Action.Set, settings) };
            if (entry.ConnectionType == ConnectionType.Host)
            {
                // Build up the host name. Add prefixes.
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(entry.Prefixes))
                {
                    sb.Append(string.Join(":", entry.Prefixes.ToCharArray().Select(c => c.ToString())) + ":");
                }

                // Add LU names.
                if (!string.IsNullOrEmpty(entry.LuNames))
                {
                    sb.Append(string.Join(",", entry.LuNames.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)) + "@");
                }

                // Add (possibly-bracketed) hostname.
                if (entry.Host.Contains(":"))
                {
                    sb.Append($"[{entry.Host}]");
                }
                else
                {
                    sb.Append(entry.Host);
                }

                // Add port.
                if (!string.IsNullOrEmpty(entry.Port))
                {
                    sb.Append(":" + entry.Port);
                }

                actions.Add(new BackEndAction(B3270.Action.Connect, sb.ToString()));
            }

            // Connect.
            this.mainScreen.ConnectHostType = entry.HostType; // for file transfers
            this.ConnectHostEntry = entry;
            this.connectComplete = complete;
            this.connectCompletePending = complete != null;
            this.app.BackEnd.RunActions(
                actions,
                (cookie, success, result) =>
                {
                    if (!success)
                    {
                        this.ConnectHostEntry = null;
                        if (this.connectCompletePending)
                        {
                            this.connectCompletePending = false;
                            this.connectComplete(false, result);
                            this.connectComplete = null;
                        }
                        else
                        {
                            ErrorBox.Show(result, I18n.Get(Title.Connect));
                        }
                    }
                    else
                    {
                        if (entry.ConnectionType == ConnectionType.LocalProcess)
                        {
                            // Connect to a local process.
                            this.app.Cmd.Connect(entry);
                        }
                    }
                });

            return true;
        }

        /// <summary>
        /// Disconnect from the host.
        /// </summary>
        public void Disconnect()
        {
            this.ConnectHostEntry = null;
            if (this.app.ConnectionState != ConnectionState.NotConnected)
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.Reconnect, B3270.Value.False), ErrorBox.Completion(I18n.Get(Title.Disconnect)));
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Disconnect), ErrorBox.Completion(I18n.Get(Title.Disconnect)));
            }
        }

        /// <summary>
        /// Clean a login macro before passing it to the back end.
        /// </summary>
        /// <param name="loginMacro">Login macro.</param>
        /// <returns>Cleaned macro.</returns>
        private static string CleanLoginMacro(string loginMacro)
        {
            if (string.IsNullOrEmpty(loginMacro))
            {
                return string.Empty;
            }

            return loginMacro.Replace(Environment.NewLine, " ");
        }

        /// <summary>
        /// Add a toggle to an action list if it is supported.
        /// </summary>
        /// <param name="supported">List of supported options.</param>
        /// <param name="settings">Settings list to add to.</param>
        /// <param name="option">Option (toggle) name.</param>
        /// <param name="value">New value.</param>
        /// <param name="required">True to report failure.</param>
        /// <returns>True if supported or not required.</returns>
        private static bool AddToggleIfSupported(ICollection<string> supported, List<string> settings, string option, string value, bool required = false)
        {
            if (supported.Contains(option))
            {
                settings.AddRange(new[] { option, value });
                return true;
            }

            // Not supported, or no value to set.
            return !required || string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Connection change handler.
        /// </summary>
        private void HostConnectionChange()
        {
            if (this.ConnectHostEntry == null)
            {
                return;
            }

            var connectionState = this.app.ConnectionState;
            if (connectionState == ConnectionState.NotConnected)
            {
                this.ConnectHostEntry = null;
                this.connectCompletePending = false;
                this.connectComplete = null;
                return;
            }
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Connection information.
            /// </summary>
            public static readonly string Connect = I18n.Combine(TitleName, "connect");

            /// <summary>
            /// Disconnect failed.
            /// </summary>
            public static readonly string Disconnect = I18n.Combine(TitleName, "disconnect");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// TLS not supported.
            /// </summary>
            public static readonly string TlsNotSupported = I18n.Combine(MessageName, "tlsNotSupported");

            /// <summary>
            /// TLS options not supported.
            /// </summary>
            public static readonly string TlsOptionsNotSupported = I18n.Combine(MessageName, "tlsOptionsNotSupported");
        }
    }
}
