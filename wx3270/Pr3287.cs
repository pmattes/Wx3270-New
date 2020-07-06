// <copyright file="Pr3287.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// <code>pr3287</code> keywords.
    /// </summary>
    public class Pr3287
    {
        /// <summary>
        /// Command line options.
        /// </summary>
        public class CommandLineOption
        {
            /// <summary>
            /// Trace option.
            /// </summary>
            public const string Trace = "-trace";

            /// <summary>
            /// Trace directory option.
            /// </summary>
            public const string TraceDir = "-tracedir";

            /// <summary>
            /// Regular expression for removing the trace options.
            /// </summary>
            public const string TraceRegex = @"\-trace\b";
        }
    }
}
