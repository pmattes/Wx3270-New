// <copyright file="Nickname.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Linq;

    /// <summary>
    /// Nickname generation and validation.
    /// </summary>
    public static class Nickname
    {
        /// <summary>
        /// Invalid names.
        /// </summary>
        private static readonly string[] BadNames = new[] { "CON", "PRN", "AUX", "NUL" };

        /// <summary>
        /// Invalid names if followed by a digit.
        /// </summary>
        private static readonly string[] BadNamesN = new[] { "COM", "LPT" };

        /// <summary>
        /// Gets the list of bad characters.
        /// </summary>
        public static char[] BadChars { get; } = new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' }
            .Concat(Enumerable.Range(0, 32).Select(r => (char)r))
            .ToArray();

        /// <summary>
        /// Check a nickname for validity.
        /// </summary>
        /// <param name="name">Name to check</param>
        /// <returns>True if a name is valid</returns>
        public static bool ValidNickname(string name)
        {
            return !(string.IsNullOrWhiteSpace(name)
                || BadNames.Any(badName => badName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                || (BadNamesN.Any(badNameN => name.StartsWith(badNameN, StringComparison.CurrentCultureIgnoreCase) && name.Length == badNameN.Length + 1)
                    && char.IsDigit(name.Last()))
                || BadChars.Any(badChar => name.Contains(badChar)));
        }

        /// <summary>
        /// Create a nickname from a hostname.
        /// </summary>
        /// <param name="hostName">Host name to test</param>
        /// <returns>Translated nickname</returns>
        public static string CreateNickname(string hostName)
        {
            if (BadNames.Any(badName => badName.Equals(hostName, StringComparison.InvariantCultureIgnoreCase))
                || (BadNamesN.Any(badNameN => badNameN.Equals(hostName.Substring(0, hostName.Length - 1), StringComparison.InvariantCultureIgnoreCase))
                    && char.IsDigit(hostName[-1])))
            {
                // Put an underscore in front of a device name.
                return "_" + hostName;
            }

            // Replace any prohibited character with an underscore.
            return new string(hostName.Select(ch => BadChars.Contains(ch) ? '_' : ch).ToArray());
        }
    }
}
