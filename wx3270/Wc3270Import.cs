// <copyright file="Wc3270Import.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Media.Animation;
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
        private const string LineRegex = @"\s*(wc3270\.|wx3270\.|b3270\.|\*)(?<resource>[a-zA-Z0-9.]+)\s*:\s*(?<value>.*)\s*";

        /// <summary>
        /// Regular expression for a macro definition.
        /// </summary>
        private const string MacrosRegex = @"^\s*(?<name>[^:\s]+)\s*:\s*(?<commands>.*)";

        /// <summary>
        /// The line regular expression parser.
        /// </summary>
        private readonly Regex lineRegex = new Regex(LineRegex);

        /// <summary>
        /// The macros regular expression parser.
        /// </summary>
        private readonly Regex macrosRegex = new Regex(MacrosRegex);

        /// <summary>
        /// The back end database.
        /// </summary>
        private readonly IBackEndDb backEndDb;

        /// <summary>
        /// The file name.
        /// </summary>
        private string fileName = "(none)";

        /// <summary>
        /// Initializes a new instance of the <see cref="Wc3270Import"/> class.
        /// </summary>
        /// <param name="backEndDb">Back end database.</param>
        public Wc3270Import(IBackEndDb backEndDb)
        {
            this.backEndDb = backEndDb;
        }

        /// <summary>
        /// Gets or sets the attributes read from the file.
        /// </summary>
        private Dictionary<string, Wc3270Resource> Attributes { get; set; } = new Dictionary<string, Wc3270Resource>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Read a wc3270 profile and pick out the parts we can support.
        /// </summary>
        /// <param name="profileName">Pathname of profile.</param>
        public void Read(string profileName)
        {
            this.Read(profileName, File.ReadLines(profileName), fromFile: true);
        }

        /// <summary>
        /// Read a wc3270 profile from a set of strings.
        /// </summary>
        /// <param name="profileName">Path name.</param>
        /// <param name="lines">Set of strings.</param>
        /// <param name="fromFile">True if the resources came from a file.</param>
        public void Read(string profileName, IEnumerable<string> lines, bool fromFile = true)
        {
            this.fileName = profileName;
            var wholeLine = string.Empty;
            var lineNumber = 0;
            foreach (var line in lines)
            {
                lineNumber++;
                var ln = line.TrimStart(' ', '\t');
                if (string.IsNullOrWhiteSpace(ln) || ln.StartsWith("!") || ln.StartsWith("#"))
                {
                    continue;
                }

                if (ln.EndsWith("\\"))
                {
                    wholeLine += ln.TrimEnd('\\');
                    continue;
                }

                this.Parse(wholeLine + ln, lineNumber, fromFile: fromFile);
                wholeLine = string.Empty;
            }
        }

        /// <summary>
        /// Parse one complete line of input.
        /// </summary>
        /// <param name="line">Line to parse.</param>
        /// <param name="lineNumber">Line number.</param>
        /// <param name="fromFile">True if resources came from a file.</param>
        public void Parse(string line, int lineNumber = 0, bool fromFile = true)
        {
            // Do the basic syntax check.
            var match = this.lineRegex.Match(line);
            if (!match.Success)
            {
                throw this.FatalException("resource syntax error", lineNumber, fromFile);
            }

            // Remember just the latest value for each resource.
            this.Attributes[match.Groups["resource"].Value] = new Wc3270Resource(match.Groups["value"].Value, lineNumber);
        }

        /// <summary>
        /// Digest the parsed values.
        /// </summary>
        /// <returns>Profile with overridden values.</returns>
        public Profile Digest()
        {
            return this.Digest(out _, out _);
        }

        /// <summary>
        /// Digest the parsed values.
        /// </summary>
        /// <param name="host">Returned true if the profile has a hostname in it.</param>
        /// <param name="unmatched">Returned set of unmatched resources.</param>
        /// <param name="profile">Optional profile to modify.</param>
        /// <param name="addHostEntry">If true, add any hostname resource to the profile.</param>
        /// <param name="fromFile">If true, the values came from a file.</param>
        /// <param name="setAutoConnect">If true, added host entries should include auto-connect.</param>
        /// <returns>Profile with overridden values.</returns>
        public Profile Digest(out HostEntry host, out IEnumerable<string> unmatched, Profile profile = null, bool addHostEntry = true, bool fromFile = true, bool setAutoConnect = true)
        {
            host = null;
            unmatched = null;
            var unmatchedList = new List<string>();

            // Start with a default profile if none is specified.
            profile ??= new Profile();
            if (fromFile)
            {
                // Assume they want the same name as the file they are importing.
                profile.Imported = true;
                profile.Name = Path.GetFileNameWithoutExtension(this.fileName);
            }

            HostEntry hostEntry = null;

            // Set defaults for values that can't be checked until all of the attributes have been processed.
            var neutralBlack = 0;
            var neutralWhite = 15;
            Profile.OversizeClass oversize = null;

            // Validate the last-appearing values of each attribute.
            // Earlier values may be wrong, but they are ignored when read by wc3270.
            foreach (var pair in this.Attributes.OrderBy(kv => kv.Key, new HostnameFirstComparer()))
            {
                var resourceName = pair.Key;
                var value = pair.Value.Value;
                var lineNumber = pair.Value.LineNumber;
                var canonDict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var s in new[]
                {
                    // Ugly: This list needs to be maintained when support for new resources is added.
                    // But it allows me to use a simple switch statement below, and also lets me generate error messages
                    // with the canonical name.
                    // Ideally Wc3270.Resource should be an enum, but there are no string enums in C#.
                    B3270.Setting.AltCursor, B3270.Setting.AlwaysInsert, Wc3270.Resource.BellMode, Wc3270.Resource.Charset, B3270.Setting.CodePage,
                    Wc3270.Resource.ConsoleColorForHostColorNeutralBlack, Wc3270.Resource.ConsoleColorForHostColorNeutralWhite,
                    B3270.Setting.Crosshair, Wc3270.Resource.Hostname, Wc3270.Resource.Macros, B3270.Setting.Model,
                    B3270.Setting.Oversize, Wc3270.Resource.PrinterCodepage, B3270.Setting.PrinterLu, B3270.Setting.PrinterName,
                    B3270.Setting.Proxy, Wc3270.Resource.Title, B3270.Setting.VerifyHostCert,
                })
                {
                    canonDict[s] = s;
                }

                if (!canonDict.TryGetValue(resourceName, out string canonName))
                {
                    unmatchedList.Add(resourceName);
                    continue;
                }

                switch (canonName)
                {
                    case B3270.Setting.AltCursor:
                        if (!bool.TryParse(value, out bool altCursor))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        profile.CursorType = altCursor ? CursorType.Underscore : CursorType.Block;
                        break;
                    case B3270.Setting.AlwaysInsert:
                        if (!bool.TryParse(value, out bool alwaysInsert))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        profile.AlwaysInsert = alwaysInsert;
                        break;
                    case Wc3270.Resource.BellMode:
                        profile.AudibleBell = !value.Equals("none", StringComparison.InvariantCultureIgnoreCase) &&
                            !value.Equals("flash", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case Wc3270.Resource.Charset: // name
                    case B3270.Setting.CodePage: // name
                        profile.HostCodePage = this.backEndDb.CodePageDb.CanonicalName(value);
                        if (profile.HostCodePage == null)
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        break;
                    case Wc3270.Resource.ConsoleColorForHostColorNeutralBlack: // 15 for reverse video
                        if (!int.TryParse(value, out neutralBlack) || neutralBlack < 0 || neutralBlack > 15)
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        break;
                    case Wc3270.Resource.ConsoleColorForHostColorNeutralWhite: // 0 for reverse video
                        if (!int.TryParse(value, out neutralWhite) || neutralWhite < 0 || neutralWhite > 15)
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        break;
                    case B3270.Setting.Crosshair: // true/false
                        if (!bool.TryParse(value, out bool crosshair))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        profile.CrosshairCursor = crosshair;
                        break;
                    case Wc3270.Resource.Hostname: // L:lu@name:port
                        if (!B3270HostSpec.TryParse(value, out B3270HostSpec hostSpec))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        hostEntry = new HostEntry(hostSpec, this.backEndDb.HostPrefixDb.Prefixes) { AutoConnect = setAutoConnect ? AutoConnect.Connect : AutoConnect.None, };
                        if (addHostEntry &&
                            !profile.Hosts.Any(h => h.Name.Equals(hostEntry.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            // Append this host.
                            profile.Hosts = profile.Hosts.Append(hostEntry);
                        }

                        host = hostEntry;
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
                                    throw this.FatalException($"invalid {canonName} syntax, entry {entry}", lineNumber, fromFile);
                                }

                                var name = macroMatch.Groups["name"].Value;
                                var commands = macroMatch.Groups["commands"].Value;
                                if (newMacros.Any(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    throw this.FatalException($"invalid {canonName}: duplicate name '{name}'", lineNumber, fromFile);
                                }

                                var index = 0;
                                if (!ActionSyntax.CheckLine(commands, out _, ref index, out string errorText))
                                {
                                    throw this.FatalException($"invalid {canonName} '{name}': {errorText}", lineNumber, fromFile);
                                }

                                newMacros.Add(new MacroEntry() { Name = name, Macro = commands });
                                entry++;
                            }

                            profile.Macros = profile.Macros.Concat(newMacros).ToList();
                        }

                        break;
                    case B3270.Setting.Model: // 2 3 4 5
                        if (!ModelName.TryParse(value, out ModelName model))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        profile.Model = model.ModelNumber;
                        profile.ColorMode = model.Color;
                        profile.ExtendedMode = model.Extended; // XXX -- likely wrong now
                        break;
                    case B3270.Setting.Oversize: // cols x rows
                        if (!Oversize.TryParse(value, out Oversize os))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        oversize = new Profile.OversizeClass() { Columns = os.Columns, Rows = os.Rows };
                        break;
                    case Wc3270.Resource.PrinterCodepage:
                        profile.PrinterCodePage = value;
                        break;
                    case B3270.Setting.PrinterLu: // name or "."
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
                    case B3270.Setting.PrinterName:
                        profile.Printer = value;
                        break;
                    case B3270.Setting.Proxy: // type[name]:port
                        var parser = new ProxyParser(this.backEndDb.ProxiesDb);
                        if (!parser.TryParse(value, out Profile.ProxyClass proxy))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        profile.Proxy = proxy;
                        break;
                    case Wc3270.Resource.Title: // string
                        profile.WindowTitle = value;
                        break;
                    case B3270.Setting.VerifyHostCert: // true/false
                        if (!bool.TryParse(value, out bool verifyHostCert))
                        {
                            throw this.FatalException($"invalid {canonName} '{value}'", lineNumber, fromFile);
                        }

                        if (hostEntry != null)
                        {
                            if (verifyHostCert)
                            {
                                hostEntry.Prefixes = hostEntry.Prefixes.Replace(B3270.Prefix.NoVerifyCert, string.Empty);
                            }
                            else
                            {
                                if (!hostEntry.Prefixes.Contains(B3270.Prefix.NoVerifyCert))
                                {
                                    hostEntry.Prefixes += B3270.Prefix.NoVerifyCert;
                                }
                            }
                        }

                        break;
                }
            }

            if (neutralBlack == 15 && neutralWhite == 0)
            {
                // Reverse video.
                profile.Colors.HostColors = Settings.BlackOnWhiteScheme;
            }

            if (oversize != null)
            {
                // Oversize might conflict with model.
                if (oversize.Rows >= this.backEndDb.ModelsDb.DefaultRows(profile.Model)
                    && oversize.Columns >= this.backEndDb.ModelsDb.DefaultColumns(profile.Model))
                {
                    profile.Oversize = oversize;
                }
                else
                {
                    profile.Oversize = new Profile.OversizeClass();
                }
            }
            else if (profile.Oversize.Rows < this.backEndDb.ModelsDb.DefaultRows(profile.Model).Value
                || profile.Oversize.Columns < this.backEndDb.ModelsDb.DefaultColumns(profile.Model).Value)
            {
                profile.Oversize = new Profile.OversizeClass();
            }

            unmatched = unmatchedList;
            return profile;
        }

        /// <summary>
        /// Qualify an error message with the file name and line number.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="lineNumber">Line number.</param>
        /// <param name="fromFile">True if resources came from a file.</param>
        /// <returns>Error message with location.</returns>
        private Exception FatalException(string message, int lineNumber, bool fromFile)
        {
            return new InvalidDataException(fromFile ? $"{this.fileName}, line {lineNumber}: {message}" : $"Command line: {message}");
        }

        /// <summary>
        /// Class to return 'hostname' as the first element of an ordered list.
        /// </summary>
        private class HostnameFirstComparer : IComparer<string>
        {
            /// <summary>
            /// Compares two strings for sorting.
            /// </summary>
            /// <param name="x">First string.</param>
            /// <param name="y">Second string.</param>
            /// <returns>Usual return values for Compare.</returns>
            public int Compare(string x, string y)
            {
                var xh = x.Equals(Wc3270.Resource.Hostname, StringComparison.InvariantCultureIgnoreCase);
                var yh = x.Equals(Wc3270.Resource.Hostname, StringComparison.InvariantCultureIgnoreCase);
                if (xh && yh)
                {
                    return 0;
                }

                if (xh)
                {
                    return -1;
                }

                if (yh)
                {
                    return 1;
                }

                return string.Compare(x, y, ignoreCase: true);
            }
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
