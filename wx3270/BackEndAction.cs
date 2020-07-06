// <copyright file="BackEndAction.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An emulator action and its parameters.
    /// </summary>
    public class BackEndAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackEndAction"/> class.
        /// </summary>
        /// <param name="actionName">Action name.</param>
        /// <param name="parameters">Action parameters.</param>
        public BackEndAction(string actionName, params object[] parameters)
        {
            this.ActionName = actionName;
            this.Parameters = new List<string>(parameters.Select(p => p.ToString()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackEndAction"/> class.
        /// </summary>
        /// <param name="actionName">Action name.</param>
        /// <param name="parameters">Action parameters.</param>
        public BackEndAction(string actionName, IEnumerable<object> parameters)
        {
            this.ActionName = actionName;
            this.Parameters = new List<string>(parameters.Select(p => p.ToString()));
        }

        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        public IEnumerable<string> Parameters { get; set; }

        /// <summary>
        /// Gets the b3270 encoding for the action.
        /// </summary>
        public string Encoding => string.Format(
            "{0}({1})",
            this.ActionName,
            string.Join(",", this.Parameters.Select(p => Quote(p))));

        /// <summary>
        /// Quote a parameter appropriately for the emulator.
        /// </summary>
        /// <param name="param">Parameter to quote.</param>
        /// <returns>Quoted parameter.</returns>
        /// <remarks>
        /// From the original Xt Intrinsics manual:
        /// A quoted string may contain an embedded quotation mark if the quotation mark is preceded by a single backslash (\).
        /// The three-character sequence ‘‘\\"’’ is interpreted as ‘‘single backslash followed by end-of-string’’.
        /// This is simpler (and more subtle) than it seems.
        /// </remarks>
        public static string Quote(string param)
        {
            // Characters that always trigger quoting. The left paren is not techinically necessary, but things are less
            // confusing if it is quoted. A double quote at the beginning of the string is also a trigger.
            // Note that neither a double quote elsewhere nor a backslash trigger quoting.
            const string QuotedChars = " ,()";

            // We translate empty strings to empty quoted strings, though this is also not technically required.
            if (param.Equals(string.Empty))
            {
                return "\"\"";
            }

            if (!param.Any(c => QuotedChars.Contains(c)) && !param.StartsWith("\""))
            {
                return param;
            }

            var content = param.Replace("\"", "\\\"");
            if (content.EndsWith(@"\"))
            {
                content += @"\";
            }

            return "\"" + content + "\"";
        }
    }
}
