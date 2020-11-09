// <copyright file="HostName.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;

    /// <summary>
    /// Host name parser.
    /// </summary>
    public class HostName
    {
        /// <summary>
        /// Try to parse a host name, with simpler rules.
        /// </summary>
        /// <param name="hostName">Host name to parse.</param>
        /// <param name="prefixes">Returned list of prefixes.</param>
        /// <param name="lus">Returned list of LU names.</param>
        /// <param name="host">Return host name.</param>
        /// <param name="port">Returned port.</param>
        /// <param name="accept">Returned accept hostname.</param>
        /// <returns>True if parsed successfully.</returns>
        /// <remarks>
        /// A host name looks like: [prefix:...][lu[,lu]@]host[:port][=accept].
        /// Any character can be quoted with a backslash.
        /// The hostname part can be surrounded by square brackets to quote ':' characters inside it.
        /// The logic to enforce that only the hostname can be quoted with square brackets is sheer torture.
        /// </remarks>
        public static bool TryParse(string hostName, out List<char> prefixes, out List<string> lus, out string host, out string port, out string accept)
        {
            // Set up returns.
            prefixes = null;
            lus = null;
            host = null;
            port = null;
            accept = null;

            // Remove white space at the beginning and end.
            hostName = hostName.Trim();

            // If the string is gone or there is white space inside it, fail.
            if (hostName == string.Empty || hostName.Any(c => char.IsWhiteSpace(c)))
            {
                return false;
            }

            // Anything inside '[' and ']' is a quoted hostname.
            var chars = new List<QuotedChar>();
            var bracketed = false;
            foreach (var c in hostName)
            {
                chars.Add(new QuotedChar(c, bracketed && c == ':'));
                if (c == '[')
                {
                    bracketed = true;
                }
                else if (c == ']')
                {
                    bracketed = false;
                }
            }

            // Pick off prefixes.
            while (chars.Count >= 2)
            {
                var c = chars[0].C;
                if (!chars[0].Quoted &&
                    ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) &&
                    chars[1].Equals(new QuotedChar(':')))
                {
                    if (prefixes == null)
                    {
                        prefixes = new List<char> { c };
                    }
                    else
                    {
                        prefixes.Add(c);
                    }

                    chars = chars.Skip(2).ToList();
                }
                else
                {
                    break;
                }
            }

            // The permissible order is: @, [, ], :, =.
            var orderMap = new Dictionary<char, int>
            {
                { '@', 0 },
                { '[', 1 },
                { ']', 2 },
                { ':', 3 },
                { '=', 4 },
            };
            var position = new Dictionary<char, int>();
            var order = new Dictionary<int, int>();
            var index = -1;
            foreach (var c in chars)
            {
                index++;
                if (!c.Quoted && orderMap.Keys.Contains(c.C))
                {
                    if (position.ContainsKey(c.C))
                    {
                        // Duplicate.
                        return false;
                    }

                    position[c.C] = index;
                    order[orderMap[c.C]] = index;
                }
            }

            var last = -1;
            foreach (var k in order.Keys.OrderBy(k => k))
            {
                if (order[k] < last)
                {
                    // Out of order.
                    return false;
                }

                last = order[k];
            }

            // Pick the accept name off the end.
            if (position.TryGetValue('=', out int acceptIndex))
            {
                var acceptChars = chars.Skip(acceptIndex + 1);
                accept = new string(acceptChars.Select(c => c.C).ToArray());
                chars = chars.Take(acceptIndex).ToList();
            }

            // Pick the port off the end.
            if (position.TryGetValue(':', out int portIndex))
            {
                var portChars = chars.Skip(portIndex + 1);
                port = new string(portChars.Select(c => c.C).ToArray());
                chars = chars.Take(portIndex).ToList();
            }

            // Pick off LU names at the front.
            if (position.TryGetValue('@', out int luIndex))
            {
                lus = new List<string>();
                var luChars = chars.Take(luIndex).ToList();
                chars = chars.Skip(luIndex + 1).ToList();
                while (true)
                {
                    var commaIndex = luChars.IndexOf(new QuotedChar(','));
                    if (commaIndex == 0)
                    {
                        // Empty LU.
                        return false;
                    }

                    if (commaIndex > 0)
                    {
                        lus.Add(new string(luChars.Take(commaIndex).Select(c => c.C).ToArray()));
                        luChars = luChars.Skip(commaIndex + 1).ToList();
                    }
                    else
                    {
                        break;
                    }
                }

                if (luChars.Count == 0)
                {
                    // Empty LU.
                    return false;
                }

                lus.Add(new string(luChars.Select(c => c.C).ToArray()));
            }

            // What's left is the host.
            host = new string(chars.Select(c => c.C).ToArray());

            // Make sure there are no empty pieces, pieces (besides the host) containing '[' or ']', or a host containing a
            // misplaced '[' or ']'.
            // Duplicate '[' and ']' were detected elsewhere.
            if ((lus != null && (lus.Count == 0 || lus.Any(lu => lu.Contains('[') || lu.Contains(']')))) ||
                host == string.Empty || ((host.Contains('[') || host.Contains(']')) && (host.Length < 3 || !host.StartsWith("[") || !host.EndsWith("]"))) ||
                (port != null && (port == string.Empty || port.Contains('[') || port.Contains(']'))) ||
                (accept != null && (accept == string.Empty || accept.Contains('[') || accept.Contains(']'))))
            {
                return false;
            }

            // Success.
            return true;
        }

        /// <summary>
        /// Quoted character class.
        /// </summary>
        private class QuotedChar : Tuple<char, bool>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="QuotedChar"/> class.
            /// </summary>
            /// <param name="c">Character value.</param>
            /// <param name="quoted">True if quoted.</param>
            public QuotedChar(char c, bool quoted = false)
                : base(c, quoted)
            {
            }

            /// <summary>
            /// Gets the character value.
            /// </summary>
            public char C => this.Item1;

            /// <summary>
            /// Gets a value indicating whether the character is quoted.
            /// </summary>
            public bool Quoted => this.Item2;
        }
    }
}
