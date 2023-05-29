// <copyright file="OiaState.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    using Wx3270.Contracts;

    /// <summary>
    /// Operator Information Area (OIA) state.
    /// </summary>
    public class OiaState : IOiaState
    {
        /// <summary>
        /// Mapping from strings to connection state.
        /// </summary>
        private static readonly Dictionary<string, ConnectionState> ConnectionStateDictionary = new Dictionary<string, ConnectionState>
        {
            { string.Empty, ConnectionState.NotConnected },
            { B3270.ConnectionState.NotConnected, ConnectionState.NotConnected },
            { B3270.ConnectionState.Reconnecting, ConnectionState.Reconnecting },
            { B3270.ConnectionState.Resolving, ConnectionState.Resolving },
            { B3270.ConnectionState.TcpPending, ConnectionState.TcpPending },
            { B3270.ConnectionState.TlsPending, ConnectionState.TlsPending },
            { B3270.ConnectionState.ProxyPending, ConnectionState.ProxyPending },
            { B3270.ConnectionState.TelnetPending, ConnectionState.TelnetPending },
            { B3270.ConnectionState.ConnectedNvt, ConnectionState.ConnectedNvt },
            { B3270.ConnectionState.ConnectedNvtCharmode, ConnectionState.ConnectedNvtCharmode },
            { B3270.ConnectionState.Connected3270, ConnectionState.Connected3270 },
            { B3270.ConnectionState.ConnectedUnbound, ConnectionState.ConnectedUnbound },
            { B3270.ConnectionState.ConnectedEnvt, ConnectionState.ConnectedEnvt },
            { B3270.ConnectionState.ConnectedSscp, ConnectionState.ConnectedEsscp },
            { B3270.ConnectionState.ConnectedTn3270E, ConnectionState.ConnectedTn3270e },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OiaState"/> class.
        /// </summary>
        /// <param name="field">Field values from OIA indication.</param>
        /// <param name="cursorEnabled">True if cursor is enabled.</param>
        /// <param name="cursorRow">Cursor row.</param>
        /// <param name="cursorColumn">Cursor column.</param>
        /// <param name="secure">SSL secure flag.</param>
        /// <param name="verified">SSL host verification flag.</param>
        /// <param name="connectionState">Connection state field.</param>
        /// <param name="connectionIsUi">True if connection cause is UI.</param>
        public OiaState(
            Dictionary<string, string> field,
            bool cursorEnabled,
            int cursorRow,
            int cursorColumn,
            bool secure,
            bool verified,
            string connectionState,
            bool connectionIsUi)
        {
            if (connectionState != null)
            {
                this.ConnectionState = ConnectionStateDictionary[connectionState];
            }

            if (field.ContainsKey(B3270.OiaField.Lock))
            {
                var lockValue = field[B3270.OiaField.Lock];
                this.OperatorError = OperatorError.None;
                this.ScrollCount = 0;
                switch (lockValue.ToLowerInvariant())
                {
                    case B3270.OiaLock.Deferred:
                        this.Lock = Lock.Deferred;
                        break;
                    case B3270.OiaLock.Inhibit:
                        this.Lock = Lock.Inhibit;
                        break;
                    case B3270.OiaLock.Minus:
                        this.Lock = Lock.Minus;
                        break;
                    case B3270.OiaLock.NotConnected:
                        switch (this.ConnectionState)
                        {
                            case ConnectionState.Reconnecting:
                                this.Lock = Lock.Reconnecting;
                                break;
                            case ConnectionState.Resolving:
                                this.Lock = Lock.Resolving;
                                break;
                            case ConnectionState.TcpPending:
                                this.Lock = Lock.TcpPending;
                                break;
                            case ConnectionState.ProxyPending:
                                this.Lock = Lock.ProxyPending;
                                break;
                            case ConnectionState.TlsPending:
                                this.Lock = Lock.TlsPending;
                                break;
                            case ConnectionState.TelnetPending:
                                this.Lock = Lock.TelnetPending;
                                break;
                            case ConnectionState.ConnectedUnbound:
                                this.Lock = Lock.Tn3270EPending;
                                break;
                            default:
                                this.Lock = Lock.NotConnected;
                                break;
                        }

                        break;
                    case B3270.OiaLock.SysWait:
                        this.Lock = Lock.SysWait;
                        break;
                    case B3270.OiaLock.TWait:
                        this.Lock = Lock.TimeWait;
                        break;
                    case B3270.OiaLock.Disabled:
                        this.Lock = Lock.Disabled;
                        break;
                    case B3270.OiaLock.Field:
                        this.Lock = Lock.Field;
                        break;
                    case B3270.OiaLock.FileTransfer:
                        this.Lock = Lock.FileTransfer;
                        break;
                    default:
                        if (lockValue.StartsWith(B3270.OiaLock.Oerr + " ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.Lock = Lock.OperatorError;
                            switch (lockValue.Substring(5).ToLowerInvariant())
                            {
                                case B3270.OiaOerr.Dbcs:
                                    this.OperatorError = OperatorError.DBCS;
                                    break;
                                case B3270.OiaOerr.Numeric:
                                    this.OperatorError = OperatorError.Numeric;
                                    break;
                                case B3270.OiaOerr.Overflow:
                                    this.OperatorError = OperatorError.Overflow;
                                    break;
                                case B3270.OiaOerr.Protected:
                                    this.OperatorError = OperatorError.Protected;
                                    break;
                                default:
                                    this.OperatorError = OperatorError.Unknown;
                                    break;
                            }
                        }
                        else if (lockValue.StartsWith(B3270.OiaLock.Scrolled + " ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.Lock = Lock.Scrolled;
                            this.ScrollCount = int.Parse(lockValue.Substring(9));
                        }

                        break;
                }
            }

            if (field.TryGetValue(B3270.OiaField.Typeahead, out string b) && b.Equals(B3270.Value.True))
            {
                this.Typeahead = true;
            }

            if (field.TryGetValue(B3270.OiaField.Insert, out b) && b.Equals(B3270.Value.True))
            {
                this.Insert = true;
            }

            if (field.TryGetValue(B3270.OiaField.Lu, out string lu))
            {
                this.Lu = lu;
            }

            if (field.TryGetValue(B3270.OiaField.Timing, out string timing) && !string.IsNullOrEmpty(timing))
            {
                this.Timing = timing;
            }

            this.CursorEnabled = cursorEnabled;
            this.CursorRow = cursorRow;
            this.CursorColumn = cursorColumn;
            this.Secure = secure;
            this.Verified = verified;
            this.ConnectionIsUi = connectionIsUi;

            bool undera = !field.TryGetValue(B3270.OiaField.NotUnderA, out b) || !b.Equals(B3270.Value.True);
            if (undera)
            {
                switch (this.ConnectionState)
                {
                    case ConnectionState.ConnectedEnvt:
                    case ConnectionState.ConnectedEsscp:
                    case ConnectionState.ConnectedTn3270e:
                        this.HostNetworkState = HostNetworkState.UnderscoreB;
                        break;
                    default:
                        this.HostNetworkState = HostNetworkState.UnderscoreA;
                        break;
                }
            }
            else
            {
                this.HostNetworkState = HostNetworkState.Blank;
            }

            switch (this.ConnectionState)
            {
                case ConnectionState.ConnectedEnvt:
                case ConnectionState.ConnectedNvt:
                case ConnectionState.ConnectedNvtCharmode:
                    this.LocalNetworkState = LocalNetworkState.Nvt;
                    break;
                case ConnectionState.ConnectedTn3270e:
                case ConnectionState.Connected3270:
                    this.LocalNetworkState = LocalNetworkState.BoxSolid;
                    break;
                case ConnectionState.ConnectedEsscp:
                    this.LocalNetworkState = LocalNetworkState.SscpLu;
                    break;
                default:
                    this.LocalNetworkState = LocalNetworkState.BoxQuestion;
                    break;
            }

            if (field.TryGetValue(B3270.OiaField.Screentrace, out string screenTrace) && !string.IsNullOrEmpty(screenTrace))
            {
                this.ScreenTrace = int.Parse(screenTrace);
            }
            else
            {
                this.ScreenTrace = -1;
            }

            this.PrinterSession = field.TryGetValue(B3270.OiaField.PrinterSession, out b) && b.Equals(B3270.Value.True);
            this.Script = field.TryGetValue(B3270.OiaField.Script, out b) && b.Equals(B3270.Value.True);
            this.ReverseInput = field.TryGetValue(B3270.OiaField.ReverseInput, out b) && b.Equals(B3270.Value.True);
        }

        /// <inheritdoc />
        public Lock Lock { get; private set; }

        /// <inheritdoc />
        public OperatorError OperatorError { get; private set; }

        /// <inheritdoc />
        public int ScrollCount { get; private set; }

        /// <inheritdoc />
        public string Lu { get; private set; }

        /// <inheritdoc />
        public bool Typeahead { get; private set; }

        /// <inheritdoc />
        public bool Insert { get; private set; }

        /// <inheritdoc />
        public string Timing { get; private set; }

        /// <inheritdoc />
        public bool CursorEnabled { get; private set; }

        /// <inheritdoc />
        public int CursorRow { get; private set; }

        /// <inheritdoc />
        public int CursorColumn { get; private set; }

        /// <inheritdoc />
        public bool Secure { get; private set; }

        /// <inheritdoc />
        public bool Verified { get; private set; }

        /// <inheritdoc />
        public ConnectionState ConnectionState { get; private set; }

        /// <inheritdoc />
        public bool ConnectionIsUi { get; private set; }

        /// <inheritdoc />
        public HostNetworkState HostNetworkState { get; private set; }

        /// <inheritdoc />
        public LocalNetworkState LocalNetworkState { get; private set; }

        /// <inheritdoc />
        public int ScreenTrace { get; private set; }

        /// <inheritdoc />
        public bool PrinterSession { get; private set; }

        /// <inheritdoc />
        public bool Script { get; private set; }

        /// <inheritdoc />
        public bool ReverseInput { get; private set; }

        /// <summary>
        /// Translate a connection state string to a connection state.
        /// </summary>
        /// <param name="connectionState">Connection state string from emulator.</param>
        /// <returns>Translated state.</returns>
        public static ConnectionState ParseConnectionState(string connectionState)
        {
            return string.IsNullOrEmpty(connectionState) ? ConnectionState.NotConnected : ConnectionStateDictionary[connectionState];
        }
    }
}
