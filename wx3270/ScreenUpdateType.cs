// <copyright file="ScreenUpdateType.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// Screen update types.
    /// </summary>
    public enum ScreenUpdateType
    {
        /// <summary>
        /// Incremental screen update.
        /// </summary>
        Screen,

        /// <summary>
        /// Full screen update.
        /// </summary>
        Repaint,

        /// <summary>
        /// Lock field.
        /// </summary>
        Lock,

        /// <summary>
        /// Insert-mode field.
        /// </summary>
        Insert,

        /// <summary>
        /// SSL state field.
        /// </summary>
        Ssl,

        /// <summary>
        /// Logical unit name field.
        /// </summary>
        LuName,

        /// <summary>
        /// Timing field.
        /// </summary>
        Timing,

        /// <summary>
        /// Cursor position.
        /// </summary>
        Cursor,

        /// <summary>
        /// Cursor position on the OIA.
        /// </summary>
        OiaCursor,

        /// <summary>
        /// Connection state.
        /// </summary>
        Connection,

        /// <summary>
        /// Network state.
        /// </summary>
        Network,

        /// <summary>
        /// Screen mode (model, color, oversize).
        /// </summary>
        ScreenMode,

        /// <summary>
        /// Scrollbar thumb state.
        /// </summary>
        Thumb,

        /// <summary>
        /// Screen trace count.
        /// </summary>
        ScreenTrace,

        /// <summary>
        /// Printer session state.
        /// </summary>
        PrinterSession,

        /// <summary>
        /// Type-ahead state.
        /// </summary>
        Typeahead,

        /// <summary>
        /// Trace file name.
        /// </summary>
        TraceFile,

        /// <summary>
        /// Script is running.
        /// </summary>
        Script,

        /// <summary>
        /// Scroll the screen.
        /// </summary>
        Scroll,

        /// <summary>
        /// Reverse input mode state.
        /// </summary>
        ReverseInput,
    }
}