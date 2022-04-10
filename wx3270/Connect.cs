// <copyright file="Connect.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Connect));

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
        /// The pending connection completion.
        /// </summary>
        private ConnectComplete connectComplete;

        /// <summary>
        /// True if connection errors should produce a pop-up.
        /// </summary>
        private bool connectErrorPopups = true;

        /// <summary>
        /// The message box for connect errors.
        /// </summary>
        private NonModalMessageBox connectMessageBox;

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

            // Register for asynchronous connect error pop-ups.
            this.app.Popup.ConnectErrorEvent += this.ConnectErrorEvent;
        }

        /// <summary>
        /// Gets the host we are connecting to.
        /// </summary>
        public HostEntry ConnectHostEntry { get; private set; }

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
        public bool ConnectToHost(HostEntry entry)
        {
            return this.ConnectToHost(entry, out _, null);
        }

        /// <summary>
        /// Connect to a host.
        /// </summary>
        /// <param name="entry">Host entry.</param>
        /// <param name="errorMessage">Returned error message, if <paramref name="complete"/> is not null.</param>
        /// <param name="complete">Asynchronous completion delegate.</param>
        /// <returns>True if connection was successfully initiated.</returns>
        public bool ConnectToHost(HostEntry entry, out string errorMessage, ConnectComplete complete)
        {
            errorMessage = string.Empty;

            if (!this.app.TlsHello.Supported && entry.Prefixes.Contains(B3270.Prefix.TlsTunnel))
            {
                if (complete != null)
                {
                    errorMessage = I18n.Get(Message.TlsNotSupported);
                }
                else
                {
                    ErrorBox.Show(I18n.Get(Message.TlsNotSupported), I18n.Get(Title.Connect));
                }

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
            var allAdded = AddToggleIfSupported(tlsOptionsSupported, settings, B3270.Setting.StartTls, entry.AllowStartTls ? B3270.Value.True : B3270.Value.False)
                && AddToggleIfSupported(tlsOptionsSupported, settings, B3270.Setting.AcceptHostname, entry.AcceptHostName, required: true)
                && AddToggleIfSupported(tlsOptionsSupported, settings, B3270.Setting.ClientCert, entry.ClientCertificateName, required: true);
            if (!allAdded && complete == null)
            {
                ErrorBox.Show(I18n.Get(Message.TlsOptionsNotSupported), I18n.Get(Title.Connect), MessageBoxIcon.Warning);
            }

            var reconnect = entry.AutoConnect == AutoConnect.Reconnect;
            settings.AddRange(
                new[]
                {
                    B3270.Setting.Reconnect,
                    reconnect ? B3270.Value.True : B3270.Value.False,
                    B3270.Setting.LoginMacro,
                    CleanLoginMacro(entry.LoginMacro),
                    B3270.Setting.NoTelnetInputMode,
                    entry.NoTelnetInputType.ToString(),
                    B3270.Setting.Retry,
                    this.app.ProfileManager.Current.Retry ? B3270.Value.True : B3270.Value.False,
                });

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
            this.connectErrorPopups = true;
            this.app.BackEnd.RunActions(
                actions,
                (cookie, success, result, misc) =>
                {
                    var fromUconnect = this.connectComplete != null;
                    if (fromUconnect)
                    {
                        // Pass the result back to the uConnect action.
                        this.connectComplete(success, result);
                        this.connectComplete = null;
                    }

                    if (success)
                    {
                        if (entry.ConnectionType == ConnectionType.LocalProcess)
                        {
                            // Connect to the local process.
                            this.app.Cmd.Connect(entry);
                        }
                    }
                    else
                    {
                        this.ConnectHostEntry = null;
                        if (!fromUconnect && this.connectErrorPopups)
                        {
                            this.ConnectError(result, misc.ContainsKey(B3270.Attribute.Retrying) && bool.Parse(misc[B3270.Attribute.Retrying]));
                        }
                    }

                    this.connectErrorPopups = true;
                });

            return true;
        }

        /// <summary>
        /// Explcit disconnect from the host.
        /// </summary>
        public void Disconnect()
        {
            this.ConnectHostEntry = null;
            if (this.app.ConnectionState != ConnectionState.NotConnected)
            {
                // If the Connect() action is still pending, it will fail as a result of sending a Disconnect. That can be ignored.
                this.connectErrorPopups = false;

                // Stop reconnecting and disconnect.
                this.BackEnd.RunActions(
                    new[]
                    {
                        new BackEndAction(
                            B3270.Action.Set,
                            B3270.Setting.Reconnect,
                            B3270.Value.False,
                            B3270.Setting.Retry,
                            B3270.Value.False),
                        new BackEndAction(B3270.Action.Disconnect),
                    },
                    ErrorBox.Completion(I18n.Get(Title.Disconnect)));
            }

            // Pop down the connect message box.
            if (this.connectMessageBox != null)
            {
                this.connectMessageBox.Close();
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
        /// An asynchronous connect error was reported.
        /// </summary>
        /// <param name="text">Error text to display.</param>
        /// <param name="retrying">True if connection is being retried.</param>
        private void ConnectErrorEvent(string text, bool retrying)
        {
            this.ConnectError(text, retrying);
        }

        /// <summary>
        /// A connect error was reported.
        /// </summary>
        /// <param name="text">Error text.</param>
        /// <param name="reconnect">True if reconnecting.</param>
        private void ConnectError(string text, bool reconnect)
        {
            if (this.connectMessageBox != null)
            {
                // Message box is already showing. Simply update it.
                this.connectMessageBox.MessageText = text;
                return;
            }

            if (!this.connectErrorPopups)
            {
                // Resolve the race between aborting and retrying.
                return;
            }

            this.connectMessageBox = new NonModalMessageBox(
                I18n.Get(Title.Connect),
                text,
                retryAbort: reconnect && this.app.Allowed(Restrictions.Disconnect),
                this.ConnectErrorPopupComplete);
            this.connectMessageBox.Show(this.mainScreen);
        }

        /// <summary>
        /// The connect message box is complete.
        /// </summary>
        /// <param name="result">Result from button press or form closing.</param>
        private void ConnectErrorPopupComplete(DialogResult result)
        {
            // Throw away the message box.
            this.connectMessageBox.Dispose();
            this.connectMessageBox = null;

            if (result == DialogResult.Cancel && this.app.ConnectionState != ConnectionState.NotConnected)
            {
                // Stop reconnecting, and suppress further connect pop-ups until it takes effect.
                this.BackEnd.RunActions(
                    new[]
                    {
                        new BackEndAction(
                            B3270.Action.Set,
                            B3270.Setting.Reconnect,
                            B3270.Value.False,
                            B3270.Setting.Retry,
                            B3270.Value.False),
                        new BackEndAction(B3270.Action.Disconnect),
                    },
                    ErrorBox.Completion(I18n.Get(Title.Disconnect)));
                this.connectErrorPopups = false;
            }
        }

        /// <summary>
        /// Connection change handler.
        /// </summary>
        private void HostConnectionChange()
        {
            var connectionState = this.app.ConnectionState;
            if (connectionState == ConnectionState.NotConnected)
            {
                this.ConnectHostEntry = null;
                this.connectComplete = null;
                this.connectErrorPopups = true;
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.LoginMacro, string.Empty), ErrorBox.Ignore());
            }

            if (connectionState.ToString().StartsWith("Connected") && this.connectMessageBox != null)
            {
                // We are now connected. If there is a connect error pop-up, pop it down.
                this.connectMessageBox.Close();
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
