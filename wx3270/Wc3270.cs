// <copyright file="Wc3270.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// <code>wc3270</code> manifest constants.
    /// </summary>
    public class Wc3270
    {
        /// <summary>
        /// <code>wc3270</code>-specific profile resource names.
        /// </summary>
        public static class Resource
        {
            /// <summary>
            /// The bell mode.
            /// </summary>
            public const string BellMode = "bellMode";

            /// <summary>
            /// The host code page (old form).
            /// </summary>
            public const string Charset = "charset";

            /// <summary>
            /// The Windows console color index for host color Neutral Black (15 for reverse video).
            /// </summary>
            public const string ConsoleColorForHostColorNeutralBlack = "consoleColorForHostColorNeutralBlack";

            /// <summary>
            /// The Windows console color index for host color Neutral White (0 for reverse video).
            /// </summary>
            public const string ConsoleColorForHostColorNeutralWhite = "consoleColorForHostColorNeutralWhite";

            /// <summary>
            /// The host name.
            /// </summary>
            public const string Hostname = "hostname";

            /// <summary>
            /// Macro definitions.
            /// </summary>
            public const string Macros = "macros";

            /// <summary>
            /// The Windows codepage to translate pr3287 printer session data to.
            /// </summary>
            public const string PrinterCodepage = "printer.codepage";

            /// <summary>
            /// The window title.
            /// </summary>
            public const string Title = "title";
        }
    }
}
