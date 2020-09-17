// <copyright file="Oia.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for connection state changes.
    /// </summary>
    /// <param name="state">New state.</param>
    public delegate void ConnectionStateChange(ConnectionState state);

    /// <summary>
    /// Operator Information Area (OIA) processing.
    /// </summary>
    public class Oia : BackEndEvent
    {
        /// <summary>
        /// The number of milliseconds to delay before turning underscore-a back on.
        /// </summary>
        private const int UnderscoreAdelayMs = 200;

        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Dictionary of OIA element fields from the OIA indication.
        /// </summary>
        private readonly Dictionary<string, string> field = new Dictionary<string, string>();

        /// <summary>
        /// The event invocation interface.
        /// </summary>
        private readonly IUpdate invoke;

        /// <summary>
        /// Whether the cursor is enabled.
        /// </summary>
        private bool cursorEnabled;

        /// <summary>
        /// The current cursor row.
        /// </summary>
        private int cursorRow;

        /// <summary>
        /// The current cursor column.
        /// </summary>
        private int cursorColumn;

        /// <summary>
        /// Connection state.
        /// </summary>
        private string connection = "not-connected";

        /// <summary>
        /// Current host IP address, if connected.
        /// </summary>
        private string currentHostIp = string.Empty;

        /// <summary>
        /// SSL state.
        /// </summary>
        private bool secure;

        /// <summary>
        /// SSL host verification state.
        /// </summary>
        private bool verified;

        /// <summary>
        /// Timer to delay clearing the not-underscore-A indicator.
        /// </summary>
        private Timer underaTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Oia"/> class.
        /// </summary>
        /// <param name="invoke">Invoke interface for callbacks.</param>
        public Oia(IUpdate invoke)
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.Oia, this.ProcessOia),
                new BackEndEventDef(B3270.Indication.Cursor, this.ProcessCursor),
                new BackEndEventDef(B3270.Indication.Tls, this.ProcessTls),
                new BackEndEventDef(B3270.Indication.Connection, this.ProcessConnection),
            };
            this.invoke = invoke;
        }

        /// <summary>
        /// Connection state change event.
        /// </summary>
        public event ConnectionStateChange ConnectionStateChange = (state) => { };

        /// <summary>
        /// Gets the default OIA state.
        /// </summary>
        public static OiaState DefaultOiaState
        {
            get
            {
                return new OiaState(new Dictionary<string, string>(), false, 1, 1, false, false, string.Empty, false);
            }
        }

        /// <summary>
        /// Gets the connection state.
        /// </summary>
        public ConnectionState ConnectionState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current connection was initiated by the UI.
        /// </summary>
        public bool ConnectionIsUi { get; private set; } = false;

        /// <summary>
        /// Gets the current host IP address.
        /// </summary>
        public string CurrentHostIp => this.currentHostIp;

        /// <summary>
        /// Gets a snapshot of the OIA state.
        /// </summary>
        /// <returns>New snapshot.</returns>
        public OiaState OiaState
        {
            get
            {
                lock (this.sync)
                {
                    return new OiaState(
                        this.field,
                        this.cursorEnabled,
                        this.cursorRow,
                        this.cursorColumn,
                        this.secure,
                        this.verified,
                        this.connection,
                        this.ConnectionIsUi);
                }
            }
        }

        /// <summary>
        /// Returns whether a given state represents 3270 or SSCP-LU mode.
        /// </summary>
        /// <param name="state">Current state.</param>
        /// <returns>True if 3270 or SSCP-LU mode.</returns>
        public static bool StateIs3270orSscp(ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.Connected3270:
                case ConnectionState.ConnectedTn3270e:
                case ConnectionState.ConnectedEsscp:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns whether a given state represents NVT mode.
        /// </summary>
        /// <param name="state">Current state.</param>
        /// <returns>True if NVT mode.</returns>
        public static bool StateIsNvt(ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.ConnectedNvt:
                case ConnectionState.ConnectedNvtCharmode:
                case ConnectionState.ConnectedEnvt:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// The underscore-a timer expired. Turn it back on.
        /// </summary>
        /// <param name="o">Timer context.</param>
        private void UnderscoreAbackOn(object o)
        {
            // Set the state to false.
            this.field["not-undera"] = B3270.Value.False;

            // Tell the background worker.
            this.invoke.ScreenUpdate(ScreenUpdateType.Network, new UpdateState(this.OiaState));
        }

        /// <summary>
        /// Process an OIA indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessOia(string name, AttributeDict attrs)
        {
            lock (this.sync)
            {
                // Can get field*, value, char, type. Char and type are for composing, and we will ignore them.
                var field = attrs[B3270.Attribute.Field];
                string value = attrs.ContainsKey(B3270.Attribute.Value) ? attrs[B3270.Attribute.Value] : string.Empty;

                // Fields are compose, insert, lock, lu, reverse-input, screentrace, script, timing, typeahead, not-undera, printer-session.
                // Just remember whatever we got.
                this.field[field] = value;

                switch (field)
                {
                    case B3270.OiaField.Lock:
                        this.invoke.ScreenUpdate(ScreenUpdateType.Lock, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.Insert:
                        this.invoke.ScreenUpdate(ScreenUpdateType.Insert, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.Lu:
                        this.invoke.ScreenUpdate(ScreenUpdateType.LuName, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.Screentrace:
                        this.invoke.ScreenUpdate(ScreenUpdateType.ScreenTrace, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.Timing:
                        this.invoke.ScreenUpdate(ScreenUpdateType.Timing, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.NotUnderA:
                        // Without special handling, this field can change state too quickly to see
                        // it flash off and back on. This is because by the time the background worker
                        // wakes up to process turning it 'off', the global state that it samples has
                        // already been changed to 'on' again.
                        //
                        // So we insert a delay. When we get an 'off' indication from the emulator, we
                        // process it immediately. But when we get 'on', we set up a timeout to process
                        // it a bit later.
                        if (value == B3270.Value.True)
                        {
                            // Process this immediately.
                            if (this.underaTimer != null)
                            {
                                this.underaTimer.Dispose();
                                this.underaTimer = null;
                            }

                            this.invoke.ScreenUpdate(ScreenUpdateType.Network, new UpdateState(this.OiaState));
                        }
                        else
                        {
                            // Wait a while to process it.
                            this.field[field] = B3270.Value.True; // fake it back to the previous value
                            if (this.underaTimer == null)
                            {
                                this.underaTimer = new Timer(this.UnderscoreAbackOn, null, UnderscoreAdelayMs, 0);
                            }
                            else
                            {
                                this.underaTimer.Change(UnderscoreAdelayMs, 0);
                            }
                        }

                        break;
                    case B3270.OiaField.PrinterSession:
                        this.invoke.ScreenUpdate(ScreenUpdateType.PrinterSession, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.Typeahead:
                        this.invoke.ScreenUpdate(ScreenUpdateType.Typeahead, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.Script:
                        this.invoke.ScreenUpdate(ScreenUpdateType.Script, new UpdateState(this.OiaState));
                        break;
                    case B3270.OiaField.ReverseInput:
                        this.invoke.ScreenUpdate(ScreenUpdateType.ReverseInput, new UpdateState(this.OiaState));
                        break;
                }
            }
        }

        /// <summary>
        /// Process a cursor indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessCursor(string name, AttributeDict attrs)
        {
            lock (this.sync)
            {
                this.cursorEnabled = attrs[B3270.Attribute.Enabled].Equals(B3270.Value.True);
                if (this.cursorEnabled)
                {
                    this.cursorRow = int.Parse(attrs[B3270.Attribute.Row]);
                    this.cursorColumn = int.Parse(attrs[B3270.Attribute.Column]);
                    Trace.Line(Trace.Type.BackEnd, $"Cursor enabled {this.cursorRow}/{this.cursorColumn}");
                }
                else
                {
                    this.cursorRow = 0;
                    this.cursorColumn = 0;
                    Trace.Line(Trace.Type.BackEnd, "Cursor disabled");
                }
            }

            this.invoke.ScreenUpdate(ScreenUpdateType.OiaCursor, new UpdateState(this.OiaState));
        }

        /// <summary>
        /// Process a connection state indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessConnection(string name, AttributeDict attrs)
        {
            lock (this.sync)
            {
                this.connection = attrs[B3270.Attribute.State];
                if (!attrs.TryGetValue(B3270.Attribute.Host, out this.currentHostIp))
                {
                    this.currentHostIp = string.Empty;
                }

                this.ConnectionIsUi = attrs.TryGetValue(B3270.Attribute.Cause, out string cause) && cause.Equals(B3270.Cause.Ui);
                this.ConnectionState = OiaState.ParseConnectionState(this.connection);
            }

            // Propagate the connection state change.
            this.invoke.ScreenUpdate(ScreenUpdateType.Connection, new UpdateState(this.OiaState));
            this.ConnectionStateChange(this.ConnectionState);
        }

        /// <summary>
        /// Process a TLS indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessTls(string name, AttributeDict attrs)
        {
            this.secure = attrs[B3270.Attribute.Secure] == B3270.Value.True;
            string ver;
            this.verified = attrs.TryGetValue(B3270.Attribute.Verified, out ver) && ver == B3270.Value.True;
            this.invoke.ScreenUpdate(ScreenUpdateType.Ssl, new UpdateState(this.OiaState));
        }
    }
}
