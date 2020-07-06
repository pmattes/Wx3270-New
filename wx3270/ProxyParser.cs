// <copyright file="ProxyParser.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Proxy setting parser.
    /// </summary>
    public class ProxyParser
    {
        /// <summary>
        /// List of proxy names.
        /// </summary>
        private readonly List<string> names;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyParser"/> class.
        /// </summary>
        /// <param name="names">List of proxy names.</param>
        public ProxyParser(IEnumerable<string> names)
        {
            this.names = names.ToList();
        }

        /// <summary>
        /// Parse a proxy setting.
        /// </summary>
        /// <param name="text">Setting text.</param>
        /// <param name="proxyType">Returned proxy type.</param>
        /// <param name="address">Returned address.</param>
        /// <param name="port">Returned port.</param>
        /// <param name="username">Returned user name.</param>
        /// <param name="password">Returned password.</param>
        /// <param name="failReason">Returned failure reason.</param>
        /// <param name="nullIsNone">If true, interpret an empty string as 'none'.</param>
        /// <returns>True if parsed successfully.</returns>
        /// <remarks>
        /// This is a hand-crafted parser rather than a regex, because the regex would be impenetrable.
        /// </remarks>
        public bool TryParse(string text, out string proxyType, out string address, out string port, out string username, out string password, out string failReason, bool nullIsNone = false)
        {
            proxyType = null;
            address = null;
            port = null;
            username = null;
            password = null;
            failReason = null;

            if (string.IsNullOrWhiteSpace(text))
            {
                if (nullIsNone)
                {
                    return true;
                }

                failReason = "Empty string";
                return false;
            }

            if (text.Any(c => char.IsWhiteSpace(c)))
            {
                failReason = "Contains white space";
                return false;
            }

            // Pick off the proxy type.
            var colon = text.IndexOf(':');
            if (colon <= 0)
            {
                failReason = "No ':'";
                return false;
            }

            proxyType = text.Substring(0, colon);
            if (!this.names.Contains(proxyType, StringComparer.InvariantCultureIgnoreCase))
            {
                failReason = "Invalid type name";
                return false;
            }

            // Pick off the username and password.
            text = text.Substring(colon + 1);
            var at = text.IndexOf('@');
            if (at >= 0)
            {
                var u = text.Substring(0, at);
                colon = u.IndexOf(':');
                if (colon < 0)
                {
                    username = u;
                }
                else
                {
                    username = u.Substring(0, colon);
                    password = u.Substring(colon + 1);
                }

                text = text.Substring(at + 1);
            }

            if (text.StartsWith("["))
            {
                // Handle address in brackets.
                var rbracket = text.IndexOf(']');
                if (rbracket < 0)
                {
                    failReason = "Missing ']'";
                    return false;
                }

                address = text.Substring(1, rbracket - 1);
                text = text.Substring(rbracket + 1);
                if (text != string.Empty)
                {
                    // Pick off the port.
                    if (!text.StartsWith(":"))
                    {
                        failReason = "Garbage after ']'";
                        return false;
                    }

                    port = text.Substring(1);
                }
            }
            else
            {
                // Handle address outside of brackets and pick off the port.
                colon = text.IndexOf(':');
                if (colon < 0)
                {
                    address = text;
                }
                else
                {
                    address = text.Substring(0, colon);
                    port = text.Substring(colon + 1);
                }
            }

            // Check for invalid fields.
            if (address == string.Empty)
            {
                failReason = "Empty address";
            }
            else if (username == string.Empty)
            {
                failReason = "Empty username";
            }
            else if (password == string.Empty)
            {
                failReason = "Empty password";
            }
            else if (port != null && !ushort.TryParse(port, out _))
            {
                failReason = "Invalid port";
            }

            return failReason == null;
        }
    }
}
