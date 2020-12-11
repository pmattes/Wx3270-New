// <copyright file="Wc3270Import.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Wx3270.Contracts;

    /// <summary>
    /// A class for importing wc3270 profiles.
    /// </summary>
    public class Wc3270Import
    {
        /// <summary>
        /// The wc3270 profile suffix.
        /// </summary>
        public const string Suffix = ".wc3270";

        /// <summary>
        /// Regular expression for line parsing.
        /// </summary>
        private const string LineRegex = @"\s*(wc3270\.|\*)(?<resource>[a-zA-Z0-9.]+)\s*:\s*(?<value>.*)\s*";

        /// <summary>
        /// Regular expression for hostname parsing.
        /// </summary>
        private const string HostRegex = @"((?<tls>L:))?((?<lu>[a-zA-Z0-9-_,]+)@)?(\[(?<host>[^\]]+)]|(?<host>[^:]+))(:(?<port>[0-9]+))?";

        /// <summary>
        /// Regular expression for the oversize option.
        /// </summary>
        private const string OversizeRegex = @"(?<cols>\d+)x(?<rows>\d+)";

        /// <summary>
        /// Regular expression for a macro definition.
        /// </summary>
        private const string MacrosRegex = @"^\s*(?<name>[^:\s]+)\s*:\s*(?<commands>.*)";

        /// <summary>
        /// The line regular expression parser.
        /// </summary>
        private readonly Regex lineRegex = new Regex(LineRegex);

        /// <summary>
        /// The host regular expression parser.
        /// </summary>
        private readonly Regex hostRegex = new Regex(HostRegex);

        /// <summary>
        /// The oversize regular expression parser.
        /// </summary>
        private readonly Regex oversizeRegex = new Regex(OversizeRegex);

        /// <summary>
        /// The macros regular expression parser.
        /// </summary>
        private readonly Regex macrosRegex = new Regex(MacrosRegex);

        /// <summary>
        /// The code page database.
        /// </summary>
        private readonly ICodePageDb codePageDb;

        /// <summary>
        /// The file name.
        /// </summary>
        private string fileName = "(none)";

        /// <summary>
        /// Initializes a new instance of the <see cref="Wc3270Import"/> class.
        /// </summary>
        /// <param name="codePageDb">Code page database.</param>
        public Wc3270Import(ICodePageDb codePageDb)
        {
            this.codePageDb = codePageDb;
        }

        /// <summary>
        /// Gets or sets the attributes read from the file.
        /// </summary>
        private Dictionary<string, Wc3270Resource> Attributes { get; set; } = new Dictionary<string, Wc3270Resource>();

        /// <summary>
        /// Read a wc3270 profile and pick out the parts we can support.
        /// </summary>
        /// <param name="profileName">Pathname of profile.</param>
        public void Read(string profileName)
        {
            this.fileName = profileName;
            using (var file = new StreamReader(profileName, Encoding.GetEncoding(NativeMethods.GetACP())))
            {
                var wholeLine = string.Empty;
                string line;
                var lineNumber = 0;
                while ((line = file.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.TrimStart(' ', '\t');
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("!") || line.StartsWith("#"))
                    {
                        continue;
                    }

                    if (line.EndsWith("\\"))
                    {
                        wholeLine += line.TrimEnd('\\');
                        continue;
                    }

                    this.Parse(wholeLine + line, lineNumber);
                    wholeLine = string.Empty;
                }
            }
        }

        /// <summary>
        /// Parse one complete line of input.
        /// </summary>
        /// <param name="line">Line to parse.</param>
        /// <param name="lineNumber">Line number.</param>
        public void Parse(string line, int lineNumber = 0)
        {
            // Do the basic syntax check.
            var match = this.lineRegex.Match(line);
            if (!match.Success)
            {
                throw this.FatalException("syntax error", lineNumber);
            }

            // Just remember the latest value for each resource.
            this.Attributes[match.Groups["resource"].Value] = new Wc3270Resource(match.Groups["value"].Value, lineNumber);
        }

        /// <summary>
        /// Digest the parsed values.
        /// </summary>
        /// <returns>Profile with overridden values.</returns>
        public Profile Digest()
        {
            // Start with a default profile.
            var profile = new Profile();
            HostEntry hostEntry = null;

            // Set defaults for values that can't be checked until all of the attributes have been processed.
            var verifyHostCert = true;
            var alwaysInsert = false;
            var neutralBlack = 0;
            var neutralWhite = 15;
            Profile.OversizeClass oversize = null;

            // Validate the last-appearing values of each attribute.
            // Earlier values may be wrong, but they are ignored when read by wc3270.
            foreach (var pair in this.Attributes)
            {
                var resourceName = pair.Key;
                var value = pair.Value.Value;
                var lineNumber = pair.Value.LineNumber;
                switch (resourceName)
                {
                    case Wc3270.Resource.Hostname: /* L:lu@name:port */
                        var hostMatch = this.hostRegex.Match(value);
                        if (!hostMatch.Success)
                        {
                            throw this.FatalException("host syntax error", lineNumber);
                        }

                        if (hostEntry == null)
                        {
                            hostEntry = new HostEntry
                            {
                                Name = hostMatch.Groups["host"].Value,
                                Host = hostMatch.Groups["host"].Value,
                                AutoConnect = AutoConnect.Connect,
                            };
                            profile.Hosts = new[] { hostEntry };
                        }

                        if (hostMatch.Groups["tls"].Success)
                        {
                            // L:
                            hostEntry.Prefixes = B3270.Prefix.TlsTunnel;
                        }

                        if (hostMatch.Groups["lu"].Success)
                        {
                            // LU@
                            var lu = hostMatch.Groups["lu"].Value;
                            if (lu.Split(new[] { ',' }, StringSplitOptions.None).Length != lu.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length)
                            {
                                throw this.FatalException("host LU syntax error", lineNumber);
                            }

                            hostEntry.LuNames = lu;
                        }

                        if (hostMatch.Groups["port"].Success)
                        {
                            // :port
                            hostEntry.Port = hostMatch.Groups["port"].Value;
                        }

                        break;
                    case Wc3270.Resource.Proxy: /* type[name]:port, no support yet */
                        break;
                    case Wc3270.Resource.Model: /* 2 3 4 5 */
                        if (!int.TryParse(value, out int model) || model < 2 || model > 5)
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        profile.Model = model;
                        break;
                    case Wc3270.Resource.Oversize: /* cols x rows */
                        var oversizeMatch = this.oversizeRegex.Match(value);
                        if (!oversizeMatch.Success
                            || !int.TryParse(oversizeMatch.Groups["cols"].Value, out int cols)
                            || !int.TryParse(oversizeMatch.Groups["rows"].Value, out int rows)
                            || cols * rows > Settings.OversizeMax)
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        oversize = new Profile.OversizeClass() { Columns = cols, Rows = rows };
                        break;
                    case Wc3270.Resource.CodePage: /* name */
                    case Wc3270.Resource.Charset: /* name */
                        profile.HostCodePage = this.codePageDb.CanonicalName(value);
                        if (profile.HostCodePage == null)
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        break;
                    case Wc3270.Resource.Crosshair: /* true/false */
                        bool crosshair;
                        if (!bool.TryParse(value, out crosshair))
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        profile.CrosshairCursor = true;
                        break;
                    case Wc3270.Resource.AltCursor: /* true/false */
                        bool altCursor;
                        if (!bool.TryParse(value, out altCursor))
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        if (profile.CursorType == CursorType.Block)
                        {
                            profile.CursorType = CursorType.Underscore;
                        }

                        break;
                    case Wc3270.Resource.PrinterLu: /* name or "." */
                        if (hostEntry != null)
                        {
                            if (value == ".")
                            {
                                hostEntry.PrinterSessionType = PrinterSessionType.Associate;
                            }
                            else
                            {
                                hostEntry.PrinterSessionType = PrinterSessionType.SpecificLu;
                                hostEntry.PrinterSessionLu = value;
                            }
                        }

                        break;
                    case Wc3270.Resource.PrinterName:
                        profile.Printer = value;
                        break;
                    case Wc3270.Resource.PrinterCodepage:
                        profile.PrinterCodePage = value;
                        break;
                    case Wc3270.Resource.ConsoleColorForHostColorNeutralBlack: /* 15 for reverse video */
                        if (!int.TryParse(value, out neutralBlack) || neutralBlack < 0 || neutralBlack > 15)
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        break;
                    case Wc3270.Resource.ConsoleColorForHostColorNeutralWhite: /* 0 for reverse video */
                        if (!int.TryParse(value, out neutralWhite) || neutralWhite < 0 || neutralWhite > 15)
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        break;
                    case Wc3270.Resource.VerifyHostCert: /* true/false */
                        if (!bool.TryParse(value, out verifyHostCert))
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        break;
                    case Wc3270.Resource.AlwaysInsert: /* true/false */
                        if (!bool.TryParse(value, out alwaysInsert))
                        {
                            throw this.FatalException($"invalid {resourceName} '{value}'", lineNumber);
                        }

                        break;
                    case Wc3270.Resource.Macros:
                        {
                            var newMacros = new List<MacroEntry>();
                            var macros = value.Split(new[] { "\\n" }, StringSplitOptions.None);
                            var entry = 1;
                            foreach (var macro in macros)
                            {
                                var macroMatch = this.macrosRegex.Match(macro);
                                if (!macroMatch.Success)
                                {
                                    throw this.FatalException($"invalid {resourceName} syntax, entry {entry}", lineNumber);
                                }

                                var name = macroMatch.Groups["name"].Value;
                                var commands = macroMatch.Groups["commands"].Value;
                                if (newMacros.Any(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    throw this.FatalException($"invalid {resourceName}: duplicate name '{name}'", lineNumber);
                                }

                                var index = 0;
                                if (!ActionSyntax.CheckLine(commands, out _, ref index, out string errorText))
                                {
                                    throw this.FatalException($"invalid {resourceName} '{name}': {errorText}", lineNumber);
                                }

                                newMacros.Add(new MacroEntry() { Name = name, Macro = commands });
                                entry++;
                            }

                            profile.Macros = newMacros;
                        }

                        break;
                    default:
                        break;
                }
            }

            if (hostEntry != null && verifyHostCert == false)
            {
                hostEntry.Prefixes += B3270.Prefix.NoVerifyCert;
            }

            if (neutralBlack == 15 && neutralWhite == 0)
            {
                // Reverse video.
                profile.Colors.HostColors = Settings.BlackOnWhiteScheme;
            }

            if (oversize != null)
            {
                // Oversize might conflict with model.
                if (profile.Oversize.Rows < Settings.DefaultRows(profile.Model)
                    || profile.Oversize.Columns < Settings.DefaultColumns(profile.Model))
                {
                    throw this.FatalException(
                        $"{Wc3270.Resource.Oversize} '{this.Attributes["oversize"].Value}' conflicts with {Wc3270.Resource.Model} '{profile.Model}'",
                        this.Attributes["oversize"].LineNumber);
                }

                profile.Oversize = oversize;
            }
            else
            {
                profile.Oversize.Rows = Settings.DefaultRows(profile.Model);
                profile.Oversize.Columns = Settings.DefaultColumns(profile.Model);
            }

            if (alwaysInsert)
            {
                profile.AlwaysInsert = true;
            }

            return profile;
        }

        /// <summary>
        /// Qualify an error message with the file name and line number.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="lineNumber">Line number.</param>
        /// <returns>Error message with location.</returns>
        private Exception FatalException(string message, int lineNumber)
        {
            return new InvalidDataException($"{this.fileName}, line {lineNumber}: {message}");
        }

        /// <summary>
        /// A resource value.
        /// </summary>
        private class Wc3270Resource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Wc3270Resource"/> class.
            /// </summary>
            /// <param name="value">Resource value.</param>
            /// <param name="lineNumber">Line number in session file.</param>
            public Wc3270Resource(string value, int lineNumber)
            {
                this.Value = value;
                this.LineNumber = lineNumber;
            }

            /// <summary>
            /// Gets the resource value.
            /// </summary>
            public string Value { get; private set; }

            /// <summary>
            /// Gets the line number.
            /// </summary>
            public int LineNumber { get; private set; }
        }
    }
}
