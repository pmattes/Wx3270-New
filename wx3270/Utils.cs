// <copyright file="Utils.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Tests a character for printable ASCII.
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>True if it is printable ASCII</returns>
        public static bool IsPrintableAscii(char c)
        {
            return c >= ' ' && c < 0x7f;
        }

        /// <summary>
        /// Tests a character for alphanumeric, underscore or dash.
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>True if it matches</returns>
        public static bool IsAlphaOrDash(char c)
        {
            return IsPrintableAscii(c) && (char.IsLetterOrDigit(c) || (c == '-' || c == '_'));
        }
    }
}
