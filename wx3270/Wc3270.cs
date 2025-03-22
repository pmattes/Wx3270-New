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
        /// <code>wc3270</code> profile resource names.
        /// </summary>
        public static class Resource
        {
            /// <summary>
            /// The underscore cursor toggle.
            /// </summary>
            public const string AltCursor = "altCursor";

            /// <summary>
            /// The bell mode.
            /// </summary>
            public const string BellMode = "bellMode";

            /// <summary>
            /// The host code page (old form).
            /// </summary>
            public const string Charset = "charset";

            /// <summary>
            /// The host code page (new form).
            /// </summary>
            public const string CodePage = "codePage";

            /// <summary>
            /// The Windows console color index for host color Neutral Black (15 for reverse video).
            /// </summary>
            public const string ConsoleColorForHostColorNeutralBlack = "consoleColorForHostColorNeutralBlack";

            /// <summary>
            /// The Windows console color index for host color Neutral White (0 for reverse video).
            /// </summary>
            public const string ConsoleColorForHostColorNeutralWhite = "consoleColorForHostColorNeutralWhite";

            /// <summary>
            /// The crosshair cursor toggle (overrides underscore cursor).
            /// </summary>
            public const string Crosshair = "crosshair";

            /// <summary>
            /// The host name.
            /// </summary>
            public const string Hostname = "hostname";

            /// <summary>
            /// Macro definitions.
            /// </summary>
            public const string Macros = "macros";

            /// <summary>
            /// The model number.
            /// </summary>
            public const string Model = "model";

            /// <summary>
            /// The oversize specification.
            /// </summary>
            public const string Oversize = "oversize";

            /// <summary>
            /// The Windows codepage to translate pr3287 printer session data to.
            /// </summary>
            public const string PrinterCodepage = "printer.codepage";

            /// <summary>
            /// The pr3287 printer session logical unit.
            /// </summary>
            public const string PrinterLu = "printerLu";

            /// <summary>
            /// The pr3287 printer session Windows local printer name.
            /// </summary>
            public const string PrinterName = "printer.name";

            /// <summary>
            /// The proxy specification.
            /// </summary>
            public const string Proxy = "proxy";

            /// <summary>
            /// The window title.
            /// </summary>
            public const string Title = "title";

            /// <summary>
            /// The toggle for verifying host TLS certificates.
            /// </summary>
            public const string VerifyHostCert = "verifyHostCert";

            /// <summary>
            /// Always revert to insert mode.
            /// </summary>
            public const string AlwaysInsert = "alwaysInsert";

            /// <summary>
            /// Formats an -xrm value.
            /// </summary>
            /// <param name="resource">Resource name.</param>
            /// <param name="value">Resource value.</param>
            /// <returns>Formatted -xrm string.</returns>
            public static string Format(string resource, string value)
            {
                return "wc3270." + resource + ": " + value;
            }
        }
    }
}
