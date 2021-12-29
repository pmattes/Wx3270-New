// <copyright file="IOiaState.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Types of keyboard lock.
    /// </summary>
    public enum Lock
    {
        /// <summary>
        /// Not locked.
        /// </summary>
        Unlocked,

        /// <summary>
        /// Deferred (350 millisecond delay after host unlock).
        /// </summary>
        Deferred,

        /// <summary>
        /// Keyboard disabled by a script.
        /// </summary>
        Disabled,

        /// <summary>
        /// Inhibit (X Inhibit, after Query Reply).
        /// </summary>
        Inhibit,

        /// <summary>
        /// Minus (X -f).
        /// </summary>
        Minus,

        /// <summary>
        /// Not connected.
        /// </summary>
        NotConnected,

        /// <summary>
        /// System (X SYSTEM, when host answers the AID without unlocking).
        /// </summary>
        SysWait,

        /// <summary>
        /// Time wait (X clock, AID sent with no response yet).
        /// </summary>
        TimeWait,

        /// <summary>
        /// Operator error, specific value in another field.
        /// </summary>
        OperatorError,

        /// <summary>
        /// Screen scrolled, number of screen in another field.
        /// </summary>
        Scrolled,

        /// <summary>
        /// Reconnect in progress.
        /// </summary>
        Reconnecting,

        /// <summary>
        /// Name resolution in progress.
        /// </summary>
        Resolving,

        /// <summary>
        /// TCP connection pending.
        /// </summary>
        TcpPending,

        /// <summary>
        /// Proxy negotiation pending.
        /// </summary>
        ProxyPending,

        /// <summary>
        /// TLS negotiation pending.
        /// </summary>
        TlsPending,

        /// <summary>
        /// TELNET negotiation pending.
        /// </summary>
        TelnetPending,

        /// <summary>
        /// TN3270E negotiation pending.
        /// </summary>
        Tn3270EPending,

        /// <summary>
        /// Waiting for the host to format the screen.
        /// </summary>
        Field,

        /// <summary>
        /// Waiting for a file transfer to complete.
        /// </summary>
        FileTransfer,

        /// <summary>
        /// Unknown value.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// Operator error types.
    /// </summary>
    public enum OperatorError
    {
        /// <summary>
        /// No operator error.
        /// </summary>
        None,

        /// <summary>
        /// Invalid DBCS operation.
        /// </summary>
        DBCS,

        /// <summary>
        /// Non-numeric character typed into numeric field.
        /// </summary>
        Numeric,

        /// <summary>
        /// Character inserted into full field.
        /// </summary>
        Overflow,

        /// <summary>
        /// Character typed into protected field.
        /// </summary>
        Protected,

        /// <summary>
        /// Unknown value.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// Host connection state.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Not connected.
        /// </summary>
        NotConnected,

        /// <summary>
        /// Re-connection pending.
        /// </summary>
        Reconnecting,

        /// <summary>
        /// Resolving host name.
        /// </summary>
        Resolving,

        /// <summary>
        /// TCP connection pending.
        /// </summary>
        TcpPending,

        /// <summary>
        /// TLS negotiation pending.
        /// </summary>
        TlsPending,

        /// <summary>
        /// Proxy negotiation pending.
        /// </summary>
        ProxyPending,

        /// <summary>
        /// TELNET negotiation in progress.
        /// </summary>
        TelnetPending,

        /// <summary>
        /// Connected in TN3270 NVT line mode.
        /// </summary>
        ConnectedNvt,

        /// <summary>
        /// Connected in TN3270 NVT character-at-a-time mode.
        /// </summary>
        ConnectedNvtCharmode,

        /// <summary>
        /// Connected in TN3270 3270 mode.
        /// </summary>
        Connected3270,

        /// <summary>
        /// Connected in TN3270E unbound mode.
        /// </summary>
        ConnectedUnbound,

        /// <summary>
        /// Connected in TN3270E NVT mode.
        /// </summary>
        ConnectedEnvt,

        /// <summary>
        /// Connected in TN3270E SSCP-LU mode.
        /// </summary>
        ConnectedEsscp,

        /// <summary>
        /// Connected in TN3270E 3270 mode.
        /// </summary>
        ConnectedTn3270e,
    }

    /// <summary>
    /// Host network state (the middle part of the network indicator).
    /// </summary>
    public enum HostNetworkState
    {
        /// <summary>
        /// Display an underscored A (TN3270E mode).
        /// </summary>
        UnderscoreA,

        /// <summary>
        /// Display an underscored B (TN3270 mode).
        /// </summary>
        UnderscoreB,

        /// <summary>
        /// Display a blank (connection in flux).
        /// </summary>
        Blank,
    }

    /// <summary>
    /// Local network state (right-hand side of the network indicator).
    /// </summary>
    public enum LocalNetworkState
    {
        /// <summary>
        /// Display an N (NVT mode).
        /// </summary>
        Nvt,

        /// <summary>
        /// Display a solid block (3270 mode).
        /// </summary>
        BoxSolid,

        /// <summary>
        /// Display an S (SSCP-LU mode).
        /// </summary>
        SscpLu,

        /// <summary>
        /// Display a boxed question mark (unknown mode).
        /// </summary>
        BoxQuestion,
    }

    /// <summary>
    /// Operator Information Area (OIA) state.
    /// </summary>
    public interface IOiaState
    {
        /// <summary>
        /// Gets the type of keyboard lock.
        /// </summary>
        Lock Lock { get; }

        /// <summary>
        /// Gets the type of operator error.
        /// </summary>
        OperatorError OperatorError { get; }

        /// <summary>
        /// Gets the number of screens scrolled back.
        /// </summary>
        int ScrollCount { get; }

        /// <summary>
        /// Gets the Logical Unit name, or null.
        /// </summary>
        string Lu { get; }

        /// <summary>
        /// Gets a value indicating whether there is type-ahead.
        /// </summary>
        bool Typeahead { get; }

        /// <summary>
        /// Gets a value indicating whether insert mode is set.
        /// </summary>
        bool Insert { get; }

        /// <summary>
        /// Gets the timing string, or null.
        /// </summary>
        string Timing { get; }

        /// <summary>
        /// Gets a value indicating whether the cursor is enabled.
        /// </summary>
        bool CursorEnabled { get; }

        /// <summary>
        /// Gets the cursor row.
        /// </summary>
        int CursorRow { get; }

        /// <summary>
        /// Gets the cursor column.
        /// </summary>
        int CursorColumn { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is secure.
        /// </summary>
        bool Secure { get; }

        /// <summary>
        /// Gets a value indicating whether the TLS host is verified.
        /// </summary>
        bool Verified { get; }

        /// <summary>
        /// Gets the host connection state.
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Gets a value indicating whether the connection was started by the UI.
        /// </summary>
        bool ConnectionIsUi { get; }

        /// <summary>
        /// Gets the middle communication state.
        /// </summary>
        HostNetworkState HostNetworkState { get; }

        /// <summary>
        /// Gets the right-hand communication state.
        /// </summary>
        LocalNetworkState LocalNetworkState { get; }

        /// <summary>
        /// Gets the screen trace count.
        /// </summary>
        int ScreenTrace { get; }

        /// <summary>
        /// Gets a value indicating whether there is an active printer session.
        /// </summary>
        bool PrinterSession { get; }

        /// <summary>
        /// Gets a value indicating whether a script is running.
        /// </summary>
        bool Script { get; }

        /// <summary>
        /// Gets a value indicating whether reverse input mode is active.
        /// </summary>
        bool ReverseInput { get; }
    }
}
