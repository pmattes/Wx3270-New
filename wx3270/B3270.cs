// <copyright file="B3270.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// B3270 keywords.
    /// </summary>
    public class B3270
    {
        /// <summary>
        /// Private-use area base value.
        /// </summary>
        public const int PuaBase = 0xe000;

        /// <summary>
        /// Input types for no-TELNET connections.
        /// </summary>
        public enum NoTelnetInputType
        {
            /// <summary>
            /// Line mode.
            /// </summary>
            Line,

            /// <summary>
            /// Character mode.
            /// </summary>
            Character,

            /// <summary>
            /// Character mode with CR/LF translation.
            /// </summary>
            CharacterCrLf,
        }

        /// <summary>
        /// Class for constructing resource values and command-line arguments.
        /// </summary>
        public static class ResourceFormat
        {
            /// <summary>
            /// Constructs a resource option.
            /// </summary>
            /// <param name="name">Resource name.</param>
            /// <param name="value">Resource value.</param>
            /// <returns>Command line option.</returns>
            public static string Value(string name, string value)
            {
                return $"*{name}: {value}";
            }
        }

        /// <summary>
        /// Class for constructing setting actions.
        /// </summary>
        public static class ToggleArgument
        {
            /// <summary>
            /// Constructs a set or clear action based on a Boolean.
            /// </summary>
            /// <param name="set">True to set.</param>
            /// <returns>Argument string.</returns>
            public static string Action(bool set)
            {
                return set ? ToggleAction.Set : ToggleAction.Clear;
            }
        }

        /// <summary>
        /// XML start element names.
        /// </summary>
        public class StartElement
        {
            /// <summary>
            /// Start element sent to the emulator.
            /// </summary>
            public const string B3270In = "b3270-in";

            /// <summary>
            /// Start element send from the emulator.
            /// </summary>
            public const string B3270Out = "b3270-out";
        }

        /// <summary>
        /// Names of B3270 operations (outbound XML element names).
        /// </summary>
        public class Operation
        {
            /// <summary>
            /// The fail operation (fail a pass-through action).
            /// </summary>
            public const string Fail = "fail";

            /// <summary>
            /// The register operation (register a pass-through action).
            /// </summary>
            public const string Register = "register";

            /// <summary>
            /// The run operation (run a list of actions).
            /// </summary>
            public const string Run = "run";

            /// <summary>
            /// The succeed operation (complete a pass-through operation successfully).
            /// </summary>
            public const string Succeed = "succeed";
        }

        /// <summary>
        /// Names of B3270 indications (inbound XML element names).
        /// </summary>
        public class Indication
        {
            /// <summary>
            /// Screen attribute changed.
            /// </summary>
            public const string Attr = "attr";

            /// <summary>
            /// Ring the terminal bell.
            /// </summary>
            public const string Bell = "bell";

            /// <summary>
            /// New value for one cell on the screen.
            /// </summary>
            public const string Char = "char";

            /// <summary>
            /// Supported code page.
            /// </summary>
            public const string CodePage = "code-page";

            /// <summary>
            /// Set of supported code pages.
            /// </summary>
            public const string CodePages = "code-pages";

            /// <summary>
            /// Host connection in progress.
            /// </summary>
            public const string ConnectAttempt = "connect-attempt";

            /// <summary>
            /// Host connection state changed.
            /// </summary>
            public const string Connection = "connection";

            /// <summary>
            /// Cursor moved.
            /// </summary>
            public const string Cursor = "cursor";

            /// <summary>
            /// Screen was erased and possibly resized.
            /// </summary>
            public const string Erase = "erase";

            /// <summary>
            /// Screen switched to right-to-left mode.
            /// </summary>
            public const string Flipped = "flipped";

            /// <summary>
            /// <code>xterm</code> escape sequence specified a new font.
            /// </summary>
            public const string Font = "font";

            /// <summary>
            /// Screen changed state between formatted and unformatted.
            /// </summary>
            public const string Formatted = "formatted";

            /// <summary>
            /// File transfer state changed.
            /// </summary>
            public const string Ft = "ft";

            /// <summary>
            /// Initial indication, including copyright.
            /// </summary>
            public const string Hello = "hello";

            /// <summary>
            /// <code>xterm</code> escape sequence to change the icon name.
            /// </summary>
            public const string IconName = "icon-name";

            /// <summary>
            /// Initialization block.
            /// </summary>
            public const string Initialize = "initialize";

            /// <summary>
            /// Supported model.
            /// </summary>
            public const string Model = "model";

            /// <summary>
            /// Set of supported models.
            /// </summary>
            public const string Models = "models";

            /// <summary>
            /// Something in the Operator Information Area (status line) changed.
            /// </summary>
            public const string Oia = "oia";

            /// <summary>
            /// Pass-through action invoked.
            /// </summary>
            public const string Passthru = "passthru";

            /// <summary>
            /// Pop up a message.
            /// </summary>
            public const string Popup = "popup";

            /// <summary>
            /// Set of supported prefixes.
            /// </summary>
            public const string Prefixes = "prefixes";

            /// <summary>
            /// Set of supported proxies.
            /// </summary>
            public const string Proxies = "proxies";

            /// <summary>
            /// Supported proxy.
            /// </summary>
            public const string Proxy = "proxy";

            /// <summary>
            /// One row's worth of modified screen data.
            /// </summary>
            public const string Row = "row";

            /// <summary>
            /// A "run" operation completed.
            /// </summary>
            public const string RunResult = "run-result";

            /// <summary>
            /// The screen changed.
            /// </summary>
            public const string Screen = "screen";

            /// <summary>
            /// The screen mode (model number, oversize, color/monochrome, extended) changed.
            /// </summary>
            public const string ScreenMode = "screen-mode";

            /// <summary>
            /// Scroll the screen up one row.
            /// </summary>
            public const string Scroll = "scroll";

            /// <summary>
            /// A setting changed.
            /// </summary>
            public const string Setting = "setting";

            /// <summary>
            /// The terminal name changed.
            /// </summary>
            public const string TerminalName = "terminal-name";

            /// <summary>
            /// The vertical scrollbar thumb changed.
            /// </summary>
            public const string Thumb = "thumb";

            /// <summary>
            /// The TLS state changed.
            /// </summary>
            public const string Tls = "tls";

            /// <summary>
            /// TLS initial state information.
            /// </summary>
            public const string TlsHello = "tls-hello";

            /// <summary>
            /// The trace file changed.
            /// </summary>
            public const string TraceFile = "trace-file";

            /// <summary>
            /// An user interface error occurred (unknown operation, etc.)
            /// </summary>
            public const string UiError = "ui-error";

            /// <summary>
            /// The window title changed.
            /// </summary>
            public const string WindowTitle = "window-title";
        }

        /// <summary>
        /// XML attributes.
        /// </summary>
        public class Attribute
        {
            /// <summary>
            /// Pass-through action name.
            /// </summary>
            public const string Action = "action";

            /// <summary>
            /// Code page alias.
            /// </summary>
            public const string Alias = "alias";

            /// <summary>
            /// Pass-through argument (append 1 through n for each).
            /// </summary>
            public const string Arg = "arg";

            /// <summary>
            /// Number of screens scrolled back.
            /// </summary>
            public const string Back = "back";

            /// <summary>
            /// Background color.
            /// </summary>
            public const string Bg = "bg";

            /// <summary>
            /// Build string.
            /// </summary>
            public const string Build = "build";

            /// <summary>
            /// File transfer byte count.
            /// </summary>
            public const string Bytes = "bytes";

            /// <summary>
            /// Number of bytes received.
            /// </summary>
            public const string BytesReceived = "bytes-received";

            /// <summary>
            /// Number of bytes sent.
            /// </summary>
            public const string BytesSent = "bytes-sent";

            /// <summary>
            /// Cause of connection state change.
            /// </summary>
            public const string Cause = "cause";

            /// <summary>
            /// Cursor column.
            /// </summary>
            public const string Column = "column";

            /// <summary>
            /// Color (3279) mode.
            /// </summary>
            public const string Color = "color";

            /// <summary>
            /// Number of columns.
            /// </summary>
            public const string Columns = "columns";

            /// <summary>
            /// Copyright text.
            /// </summary>
            public const string Copyright = "copyright";

            /// <summary>
            /// Attribute count.
            /// </summary>
            public const string Count = "count";

            /// <summary>
            /// Enabled flag for the cursor.
            /// </summary>
            public const string Enabled = "enabled";

            /// <summary>
            /// Error flag for pop-up indications.
            /// </summary>
            public const string Error = "error";

            /// <summary>
            /// Extended mode.
            /// </summary>
            public const string Extended = "extended";

            /// <summary>
            /// Foreground color.
            /// </summary>
            public const string Fg = "fg";

            /// <summary>
            /// OIA field name.
            /// </summary>
            public const string Field = "field";

            /// <summary>
            /// Trace file name.
            /// </summary>
            public const string File = "file";

            /// <summary>
            /// Graphic rendition.
            /// </summary>
            public const string Gr = "gr";

            /// <summary>
            /// Host we are connected to.
            /// </summary>
            public const string Host = "host";

            /// <summary>
            /// Host certificate information.
            /// </summary>
            public const string HostCert = "host-cert";

            /// <summary>
            /// Host IP address.
            /// </summary>
            public const string HostIp = "host-ip";

            /// <summary>
            /// The number of logical columns.
            /// </summary>
            public const string LogicalColumns = "logical-columns";

            /// <summary>
            /// The number of logical rows.
            /// </summary>
            public const string LogicalRows = "logical-rows";

            /// <summary>
            /// Model number.
            /// </summary>
            public const string Model = "model";

            /// <summary>
            /// Name for toggles.
            /// </summary>
            public const string Name = "name";

            /// <summary>
            /// TLS options.
            /// </summary>
            public const string Options = "options";

            /// <summary>
            /// Override flag for terminal name.
            /// </summary>
            public const string Override = "override";

            /// <summary>
            /// Oversize specification.
            /// </summary>
            public const string Oversize = "oversize";

            /// <summary>
            /// Nested pass-through tag.
            /// </summary>
            public const string ParentRTag = "parent-r-tag";

            /// <summary>
            /// Connected port number.
            /// </summary>
            public const string Port = "port";

            /// <summary>
            /// TLS provider name.
            /// </summary>
            public const string Provider = "provider";

            /// <summary>
            /// Pass-through action tag.
            /// </summary>
            public const string PTag = "p-tag";

            /// <summary>
            /// Number of records received.
            /// </summary>
            public const string RecordsReceived = "records-received";

            /// <summary>
            /// Number of records send.
            /// </summary>
            public const string RecordsSent = "records-sent";

            /// <summary>
            /// True if connection is being retried.
            /// </summary>
            public const string Retrying = "retrying";

            /// <summary>
            /// Cursor row.
            /// </summary>
            public const string Row = "row";

            /// <summary>
            /// Number of rows.
            /// </summary>
            public const string Rows = "rows";

            /// <summary>
            /// Run operation tag.
            /// </summary>
            public const string RTag = "r-tag";

            /// <summary>
            /// Saved portion of the thumb.
            /// </summary>
            public const string Saved = "saved";

            /// <summary>
            /// Thumb screen count.
            /// </summary>
            public const string Screen = "screen";

            /// <summary>
            /// TLS secure connection state.
            /// </summary>
            public const string Secure = "secure";

            /// <summary>
            /// TLS session information.
            /// </summary>
            public const string Session = "session";

            /// <summary>
            /// Shown area of the thumb.
            /// </summary>
            public const string Shown = "shown";

            /// <summary>
            /// Success or failure Boolean.
            /// </summary>
            public const string Success = "success";

            /// <summary>
            /// TLS hello supported flag.
            /// </summary>
            public const string Supported = "supported";

            /// <summary>
            /// Connection state.
            /// </summary>
            public const string State = "state";

            /// <summary>
            /// Top of the thumb.
            /// </summary>
            public const string Top = "top";

            /// <summary>
            /// Text associated with an event.
            /// </summary>
            public const string Text = "text";

            /// <summary>
            /// Type of indication.
            /// </summary>
            public const string Type = "type";

            /// <summary>
            /// User name attribute.
            /// </summary>
            public const string Username = "username";

            /// <summary>
            /// TLS verified connection state.
            /// </summary>
            public const string Verified = "verified";

            /// <summary>
            /// Code version.
            /// </summary>
            public const string Version = "version";

            /// <summary>
            /// The value of an element.
            /// </summary>
            public const string Value = "value";
        }

        /// <summary>
        /// Types for the run action.
        /// </summary>
        public class RunType
        {
            /// <summary>
            /// Running a key map.
            /// </summary>
            public const string Keymap = "keymap";

            /// <summary>
            /// Running a mapping from the keypad.
            /// </summary>
            public const string Keypad = "keypad";

            /// <summary>
            /// Running a macro.
            /// </summary>
            public const string Macro = "macro";
        }

        /// <summary>
        /// Connection change causes.
        /// </summary>
        public class Cause
        {
            /// <summary>
            /// The user interface.
            /// </summary>
            public const string Ui = "ui";
        }

        /// <summary>
        /// XML attribute values.
        /// </summary>
        public class Value
        {
            /// <summary>
            /// False value for traditional toggles.
            /// </summary>
            public const string Clear = "Clear";

            /// <summary>
            /// File transfer cancel.
            /// </summary>
            public const string Cancel = "Cancel";

            /// <summary>
            /// Dialog option for the print text action.
            /// </summary>
            public const string Dialog = "dialog";

            /// <summary>
            /// Boolean false.
            /// </summary>
            public const string False = "false";

            /// <summary>
            /// Force-enable flag for the keyboard enable action.
            /// </summary>
            public const string ForceEnable = "ForceEnable";

            /// <summary>
            /// GDI option for the print text action.
            /// </summary>
            public const string Gdi = "gdi";

            /// <summary>
            /// True value for traditional toggles.
            /// </summary>
            public const string Set = "set";

            /// <summary>
            /// Boolean true.
            /// </summary>
            public const string True = "true";
        }

        /// <summary>
        /// Connection states.
        /// </summary>
        public class ConnectionState
        {
            /// <summary>
            /// Not connected.
            /// </summary>
            public const string NotConnected = "not-connected";

            /// <summary>
            /// Automatic reconnect on progress.
            /// </summary>
            public const string Reconnecting = "reconnecting";

            /// <summary>
            /// Name resolution in progress.
            /// </summary>
            public const string Resolving = "resolving";

            /// <summary>
            /// TCP connection pending.
            /// </summary>
            public const string TcpPending = "tcp-pending";

            /// <summary>
            /// TLS negotiation pending.
            /// </summary>
            public const string TlsPending = "tls-pending";

            /// <summary>
            /// Proxy negotiation pending.
            /// </summary>
            public const string ProxyPending = "proxy-pending";

            /// <summary>
            /// TELNET negotiation pending.
            /// </summary>
            public const string TelnetPending = "telnet-pending";

            /// <summary>
            /// Connected in NVT line mode.
            /// </summary>
            public const string ConnectedNvt = "connected-nvt";

            /// <summary>
            /// Connected in NVT character mode.
            /// </summary>
            public const string ConnectedNvtCharmode = "connected-nvt-charmode";

            /// <summary>
            /// Connected in TN3270 3270 mode.
            /// </summary>
            public const string Connected3270 = "connected-3270";

            /// <summary>
            /// Connected in TN3270E unbound mode.
            /// </summary>
            public const string ConnectedUnbound = "connected-unbound";

            /// <summary>
            /// Connected in TN3270E NVT mode.
            /// </summary>
            public const string ConnectedEnvt = "connected-e-nvt";

            /// <summary>
            /// Connected in TN3270E SSCP-LU mode.
            /// </summary>
            public const string ConnectedSscp = "connected-sscp";

            /// <summary>
            /// Connected in TN3270E 3270 mode.
            /// </summary>
            public const string ConnectedTn3270E = "connected-tn3270e";
        }

        /// <summary>
        /// OIA field names.
        /// </summary>
        public class OiaField
        {
            /// <summary>
            /// Insert mode.
            /// </summary>
            public const string Insert = "insert";

            /// <summary>
            /// Keyboard lock.
            /// </summary>
            public const string Lock = "lock";

            /// <summary>
            /// Logical unit name.
            /// </summary>
            public const string Lu = "lu";

            /// <summary>
            /// Underscore-A state.
            /// </summary>
            public const string NotUnderA = "not-undera";

            /// <summary>
            /// Printer session.
            /// </summary>
            public const string PrinterSession = "printer-session";

            /// <summary>
            /// Reverse input mode.
            /// </summary>
            public const string ReverseInput = "reverse-input";

            /// <summary>
            /// Screen tracing.
            /// </summary>
            public const string Screentrace = "screentrace";

            /// <summary>
            /// Script active.
            /// </summary>
            public const string Script = "script";

            /// <summary>
            /// AID timing.
            /// </summary>
            public const string Timing = "timing";

            /// <summary>
            /// Type-ahead state.
            /// </summary>
            public const string Typeahead = "typeahead";
        }

        /// <summary>
        /// OIA lock types.
        /// </summary>
        public class OiaLock
        {
            /// <summary>
            /// 350ms delay.
            /// </summary>
            public const string Deferred = "deferred";

            /// <summary>
            /// Keyboard disabled by script.
            /// </summary>
            public const string Disabled = "disabled";

            /// <summary>
            /// Waiting for the host to format the screen.
            /// </summary>
            public const string Field = "field";

            /// <summary>
            /// Waiting for a file transfer to complete.
            /// </summary>
            public const string FileTransfer = "file-transfer";

            /// <summary>
            /// Inhibit state.
            /// </summary>
            public const string Inhibit = "inhibit";

            /// <summary>
            /// Illegal function.
            /// </summary>
            public const string Minus = "minus";

            /// <summary>
            /// Not connected.
            /// </summary>
            public const string NotConnected = "not-connected";

            /// <summary>
            /// Operator error.
            /// </summary>
            public const string Oerr = "oerr";

            /// <summary>
            /// Scrolled.
            /// </summary>
            public const string Scrolled = "scrolled";

            /// <summary>
            /// System wait.
            /// </summary>
            public const string SysWait = "syswait";

            /// <summary>
            /// Time wait.
            /// </summary>
            public const string TWait = "twait";
        }

        /// <summary>
        /// OIA operator error types.
        /// </summary>
        public class OiaOerr
        {
            /// <summary>
            /// DBCS error.
            /// </summary>
            public const string Dbcs = "dbcs";

            /// <summary>
            /// Non-numeric character in numeric field.
            /// </summary>
            public const string Numeric = "numeric";

            /// <summary>
            /// Field overflow.
            /// </summary>
            public const string Overflow = "overflow";

            /// <summary>
            /// Protected field.
            /// </summary>
            public const string Protected = "protected";
        }

        /// <summary>
        /// Pop-up types.
        /// </summary>
        public class PopupType
        {
            /// <summary>
            /// Output from child process, except the printer session.
            /// </summary>
            public const string Child = "child";

            /// <summary>
            /// Asynchronous connection error.
            /// </summary>
            public const string ConnectionError = "connection-error";

            /// <summary>
            /// Back-end error.
            /// </summary>
            public const string Error = "error";

            /// <summary>
            /// Back-end informational message.
            /// </summary>
            public const string Info = "info";

            /// <summary>
            /// Printer session output.
            /// </summary>
            public const string Printer = "printer";

            /// <summary>
            /// Generic result (should not happen).
            /// </summary>
            public const string Result = "result";
        }

        /// <summary>
        /// Setting names.
        /// </summary>
        public class Setting
        {
            /// <summary>
            /// The accept host name setting.
            /// </summary>
            public const string AcceptHostname = "acceptHostname";

            /// <summary>
            /// The alternate (underscore) cursor setting.
            /// </summary>
            public const string AltCursor = "altCursor";

            /// <summary>
            /// The always insert mode setting.
            /// </summary>
            public const string AlwaysInsert = "alwaysInsert";

            /// <summary>
            /// APL mode.
            /// </summary>
            public const string AplMode = "aplMode";

            /// <summary>
            /// Host code page.
            /// </summary>
            public const string CodePage = "codePage";

            /// <summary>
            /// The client certificate name setting.
            /// </summary>
            public const string ClientCert = "clientCert";

            /// <summary>
            /// Extended data stream setting.
            /// </summary>
            public const string Extended = "extended";

            /// <summary>
            /// File transfer buffer size.
            /// </summary>
            public const string FtBufferSize = "ftBufferSize";

            /// <summary>
            /// HTTP listen setting.
            /// </summary>
            public const string Httpd = "httpd";

            /// <summary>
            /// Insert mode.
            /// </summary>
            public const string InsertMode = "insertMode";

            /// <summary>
            /// The model setting.
            /// </summary>
            public const string Model = "model";

            /// <summary>
            /// The mono-case setting.
            /// </summary>
            public const string MonoCase = "monoCase";

            /// <summary>
            /// The crosshair cursor setting.
            /// </summary>
            public const string Crosshair = "crosshair";

            /// <summary>
            /// The blinking cursor setting.
            /// </summary>
            public const string CursorBlink = "cursorBlink";

            /// <summary>
            /// The login macro.
            /// </summary>
            public const string LoginMacro = "loginMacro";

            /// <summary>
            /// The overlay paste setting.
            /// </summary>
            public const string OverlayPaste = "overlayPaste";

            /// <summary>
            /// The NOP interval setting.
            /// </summary>
            public const string NopSeconds = "nopSeconds";

            /// <summary>
            /// The no-TELNET input mode setting.
            /// </summary>
            public const string NoTelnetInputMode = "noTelnetInputMode";

            /// <summary>
            /// The oversize setting.
            /// </summary>
            public const string Oversize = "oversize";

            /// <summary>
            /// The IPv4 preference setting.
            /// </summary>
            public const string PreferIpv4 = "preferIpv4";

            /// <summary>
            /// The IPv6 preference setting.
            /// </summary>
            public const string PreferIpv6 = "preferIpv6";

            /// <summary>
            /// The printer code page setting.
            /// </summary>
            public const string PrinterCodePage = "printer.codepage";

            /// <summary>
            /// The printer session LU setting.
            /// </summary>
            public const string PrinterLu = "printerLu";

            /// <summary>
            /// The printer name setting.
            /// </summary>
            public const string PrinterName = "printer.name";

            /// <summary>
            /// The printer options setting.
            /// </summary>
            public const string PrinterOptions = "printer.options";

            /// <summary>
            /// Proxy setting.
            /// </summary>
            public const string Proxy = "proxy";

            /// <summary>
            /// Automatic re-connect.
            /// </summary>
            public const string Reconnect = "reconnect";

            /// <summary>
            /// Automatic retry.
            /// </summary>
            public const string Retry = "retry";

            /// <summary>
            /// Reverse input mode.
            /// </summary>
            public const string ReverseInputMode = "reverseInputMode";

            /// <summary>
            /// Right to left display mode.
            /// </summary>
            public const string RightToLeftMode = "rightToLeftMode";

            /// <summary>
            /// The screen tracing setting.
            /// </summary>
            public const string ScreenTrace = "screenTrace";

            /// <summary>
            /// The s3270 listening port setting.
            /// </summary>
            public const string ScriptPort = "scriptPort";

            /// <summary>
            /// The show timing setting.
            /// </summary>
            public const string ShowTiming = "showTiming";

            /// <summary>
            /// The STARTTLS setting.
            /// </summary>
            public const string StartTls = "startTls";

            /// <summary>
            /// The terminal name setting.
            /// </summary>
            public const string TermName = "termName";

            /// <summary>
            /// The trace setting.
            /// </summary>
            public const string Trace = "trace";

            /// <summary>
            /// The type-ahead setting.
            /// </summary>
            public const string Typeahead = "typeahead";

            /// <summary>
            /// The verify host certificate setting.
            /// </summary>
            public const string VerifyHostCert = "verifyHostCert";

            /// <summary>
            /// The visible control characters setting.
            /// </summary>
            public const string VisibleControl = "visibleControl";
        }

        /// <summary>
        /// Toggle actions.
        /// </summary>
        public class ToggleAction
        {
            /// <summary>
            /// Set the toggle.
            /// </summary>
            public const string Set = "set";

            /// <summary>
            /// Clear the toggle.
            /// </summary>
            public const string Clear = "clear";
        }

        /// <summary>
        /// File transfer states.
        /// </summary>
        public class FtState
        {
            /// <summary>
            /// Transfer is being canceled.
            /// </summary>
            public const string Aborting = "aborting";

            /// <summary>
            /// Awaiting start of transfer.
            /// </summary>
            public const string Awaiting = "awaiting";

            /// <summary>
            /// Transfer is complete.
            /// </summary>
            public const string Complete = "complete";

            /// <summary>
            /// Transfer is running.
            /// </summary>
            public const string Running = "running";
        }

        /// <summary>
        /// Emulator actions.
        /// </summary>
        public class Action
        {
            /// <summary>
            /// Abort action.
            /// </summary>
            public const string Abort = "Abort";

            /// <summary>
            /// Attention action.
            /// </summary>
            public const string Attn = "Attn";

            /// <summary>
            /// Back tab action.
            /// </summary>
            public const string BackTab = "BackTab";

            /// <summary>
            /// Clear region action.
            /// </summary>
            public const string ClearRegion = "ClearRegion";

            /// <summary>
            /// Clear action.
            /// </summary>
            public const string Clear = "Clear";

            /// <summary>
            /// Comment string.
            /// </summary>
            public const string Comment = "!";

            /// <summary>
            /// Connect action.
            /// </summary>
            public const string Connect = "Connect";

            /// <summary>
            /// Cursor select action.
            /// </summary>
            public const string CursorSelect = "CursorSelect";

            /// <summary>
            /// Delete action.
            /// </summary>
            public const string Delete = "Delete";

            /// <summary>
            /// Delete field action.
            /// </summary>
            public const string DeleteField = "DeleteField";

            /// <summary>
            /// Disconnect action.
            /// </summary>
            public const string Disconnect = "Disconnect";

            /// <summary>
            /// Down action.
            /// </summary>
            public const string Down = "Down";

            /// <summary>
            /// Dup action.
            /// </summary>
            public const string Dup = "Dup";

            /// <summary>
            /// Enter action.
            /// </summary>
            public const string Enter = "Enter";

            /// <summary>
            /// Erase action.
            /// </summary>
            public const string Erase = "Erase";

            /// <summary>
            /// Erase EOF action.
            /// </summary>
            public const string EraseEOF = "EraseEOF";

            /// <summary>
            /// Erase Input action.
            /// </summary>
            public const string EraseInput = "EraseInput";

            /// <summary>
            /// Field end action.
            /// </summary>
            public const string FieldEnd = "FieldEnd";

            /// <summary>
            /// Field mark action.
            /// </summary>
            public const string FieldMark = "FieldMark";

            /// <summary>
            /// Home action.
            /// </summary>
            public const string Home = "Home";

            /// <summary>
            /// Key action.
            /// </summary>
            public const string Key = "Key";

            /// <summary>
            /// Keyboard disable action.
            /// </summary>
            public const string KeyboardDisable = "KeyboardDisable";

            /// <summary>
            /// Left action.
            /// </summary>
            public const string Left = "Left";

            /// <summary>
            /// Move cursor action.
            /// </summary>
            public const string MoveCursor1 = "MoveCursor1";

            /// <summary>
            /// New line action.
            /// </summary>
            public const string Newline = "Newline";

            /// <summary>
            /// Next word action.
            /// </summary>
            public const string NextWord = "NextWord";

            /// <summary>
            /// Paste string action.
            /// </summary>
            public const string PasteString = "PasteString";

            /// <summary>
            /// The PA action.
            /// </summary>
            public const string PA = "PA";

            /// <summary>
            /// The PF action.
            /// </summary>
            public const string PF = "PF";

            /// <summary>
            /// Previous word action.
            /// </summary>
            public const string PreviousWord = "PreviousWord";

            /// <summary>
            /// Print text action.
            /// </summary>
            public const string PrintText = "PrintText";

            /// <summary>
            /// Prompt action.
            /// </summary>
            public const string Prompt = "Prompt";

            /// <summary>
            /// Query action.
            /// </summary>
            public const string Query = "Query";

            /// <summary>
            /// Quit action.
            /// </summary>
            public const string Quit = "Quit";

            /// <summary>
            /// Reset action.
            /// </summary>
            public const string Reset = "Reset";

            /// <summary>
            /// Restore input action.
            /// </summary>
            public const string RestoreInput = "RestoreInput";

            /// <summary>
            /// Right action.
            /// </summary>
            public const string Right = "Right";

            /// <summary>
            /// Save input action.
            /// </summary>
            public const string SaveInput = "SaveInput";

            /// <summary>
            /// Screen trace action.
            /// </summary>
            public const string ScreenTrace = "ScreenTrace";

            /// <summary>
            /// Script action.
            /// </summary>
            public const string Script = "Script";

            /// <summary>
            /// Scroll action.
            /// </summary>
            public const string Scroll = "Scroll";

            /// <summary>
            /// Set action.
            /// </summary>
            public const string Set = "Set";

            /// <summary>
            /// String action.
            /// </summary>
            public const string String = "String";

            /// <summary>
            /// Source action.
            /// </summary>
            public const string Source = "Source";

            /// <summary>
            /// System request action.
            /// </summary>
            public const string SysReq = "SysReq";

            /// <summary>
            /// Tab action.
            /// </summary>
            public const string Tab = "Tab";

            /// <summary>
            /// Terminal name action.
            /// </summary>
            public const string TerminalName = "TerminalName";

            /// <summary>
            /// Toggle action.
            /// </summary>
            public const string Toggle = "Toggle";

            /// <summary>
            /// File transfer action.
            /// </summary>
            public const string Transfer = "Transfer";

            /// <summary>
            /// Up action.
            /// </summary>
            public const string Up = "Up";

            /// <summary>
            /// Wait action.
            /// </summary>
            public const string Wait = "Wait";
        }

        /// <summary>
        /// Query types.
        /// </summary>
        public class Query
        {
            /// <summary>
            /// Model query.
            /// </summary>
            public const string Model = "Model";
        }

        /// <summary>
        /// Host modifier prefixes.
        /// </summary>
        public class Prefix
        {
            /// <summary>
            /// Create a TLS tunnel.
            /// </summary>
            public const string TlsTunnel = "L";

            /// <summary>
            /// Do not use TELNET.
            /// </summary>
            public const string NoTelnetHost = "T";

            /// <summary>
            /// Do not verify the host certificate.
            /// </summary>
            public const string NoVerifyCert = "Y";
        }

        /// <summary>
        /// Command line options.
        /// </summary>
        public class CommandLineOption
        {
            /// <summary>
            /// Minimum version option.
            /// </summary>
            public const string MinVersion = "-minversion";

            /// <summary>
            ///  Model option.
            /// </summary>
            public const string Model = "-model";

            /// <summary>
            /// Oversize option.
            /// </summary>
            public const string Oversize = "-oversize";

            /// <summary>
            /// Script port option.
            /// </summary>
            public const string ScriptPort = "-scriptport";

            /// <summary>
            /// Script port once option.
            /// </summary>
            public const string ScriptPortOnce = "-scriptportonce";

            /// <summary>
            /// Trace option.
            /// </summary>
            public const string Trace = "-trace";

            /// <summary>
            /// UTF8 option.
            /// </summary>
            public const string Utf8 = "-utf8";
        }

        /// <summary>
        /// Resource names.
        /// </summary>
        public class ResourceName
        {
            /// <summary>
            /// The HTTP listening specification.
            /// </summary>
            public const string Httpd = "httpd";

            /// <summary>
            /// Model number and options.
            /// </summary>
            public const string Model = "model";

            /// <summary>
            /// Oversize dimensions.
            /// </summary>
            public const string Oversize = "oversize";

            /// <summary>
            /// Listen for a script on the specified TCP port.
            /// </summary>
            public const string ScriptPort = "scriptPort";

            /// <summary>
            /// Exit after the script connection is broken.
            /// </summary>
            public const string ScriptPortOnce = "scriptPortOnce";

            /// <summary>
            /// Turn on tracing.
            /// </summary>
            public const string Trace = "trace";

            /// <summary>
            /// Trace directory.
            /// </summary>
            public const string TraceDir = "traceDir";

            /// <summary>
            /// Unlock delay.
            /// </summary>
            public const string UnlockDelay = "unlockDelay";
        }

        /// <summary>
        /// Options for the Script action.
        /// </summary>
        public class ScriptOption
        {
            /// <summary>
            /// Share the console between wx3270 and the child process.
            /// </summary>
            public const string ShareConsole = "-shareconsole";
        }
    }
}
