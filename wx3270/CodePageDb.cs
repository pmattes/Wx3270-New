// <copyright file="CodePageDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Handler for code page indications from the back end.
    /// </summary>
    public class CodePageDb : BackEndEvent, ICodePageDb
    {
        /// <summary>
        /// The default code page.
        /// </summary>
        public const string Default = "bracket";

        /// <summary>
        /// The dictionary of code pages.
        /// </summary>
        private readonly Dictionary<string, CodePageDescription> codePages = new Dictionary<string, CodePageDescription>();

        /// <summary>
        /// True if we are inside a CodePages block.
        /// </summary>
        private bool running;

        /// <summary>
        /// True if the database is complete.
        /// </summary>
        private bool done;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodePageDb"/> class.
        /// </summary>
        public CodePageDb()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.CodePage, this.StartCodePage),
                new BackEndEventDef(B3270.Indication.CodePages, this.StartCodePages, this.EndCodePages),
            };
        }

        /// <summary>
        /// Event called when the database is complete.
        /// </summary>
        private event Action DoneEvent = () => { };

        /// <inheritdoc/>
        public IEnumerable<string> All => this.codePages.Keys.Concat(this.codePages.Values.SelectMany(v => v.FriendlyNames)).OrderBy(k => k, new CodePageComparer());

        /// <inheritdoc/>
        public void AddDone(Action action)
        {
            if (this.done)
            {
                action();
            }
            else
            {
                this.DoneEvent += action;
            }
        }

        /// <inheritdoc/>
        public int Index(string codePage)
        {
            return Array.IndexOf(this.All.ToArray(), codePage);
        }

        /// <inheritdoc/>
        public string CanonicalName(string alias)
        {
            if (this.codePages.ContainsKey(alias))
            {
                // Already canonical.
                return alias;
            }

            foreach (var kv in this.codePages)
            {
                if (kv.Value.Aliases.Contains(alias) || kv.Value.FriendlyNames.Contains(alias))
                {
                    // A known alias.
                    return kv.Key;
                }
            }

            // This should never happen.
            return null;
        }

        /// <summary>
        /// Get the aliases from an attribute dictionary.
        /// </summary>
        /// <param name="attributes">Attribute dictionary.</param>
        /// <returns>List of aliases.</returns>
        private static IEnumerable<string> GetAliases(AttributeDict attributes)
        {
            var index = 1;

            while (attributes.TryGetValue("alias" + index, out string alias))
            {
                yield return alias;
                index++;
            }
        }

        /// <summary>
        /// Processes a code page indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Indication attributes.</param>
        private void StartCodePage(string name, AttributeDict attributes)
        {
            if (this.running)
            {
                var codePageName = attributes[B3270.Attribute.Name];
                this.codePages[codePageName] = new CodePageDescription(codePageName, GetAliases(attributes));
            }
        }

        /// <summary>
        /// Processes a code pages indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Indication attributes.</param>
        private void StartCodePages(string name, AttributeDict attributes)
        {
            this.running = true;
        }

        /// <summary>
        /// Processes a code pages end indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        private void EndCodePages(string name)
        {
            this.running = false;
            this.done = true;
            this.DoneEvent();
        }

        /// <summary>
        /// Translation of canonical code page names to friendly names.
        /// </summary>
        private static class FriendlyName
        {
            /// <summary>
            /// Code page friendly names.
            /// </summary>
            private static readonly Dictionary<string, string[]> FriendlyNameTable = new Dictionary<string, string[]>()
            {
                { "cp037", new[] { "English (US)" } },
                { "cp273", new[] { "German" } },
                { "cp275", new[] { "Brazilian" } },
                { "cp277", new[] { "Norwegian" } },
                { "cp278", new[] { "Finnish", "Swedish" } },
                { "cp280", new[] { "Italian" } },
                { "cp284", new[] { "Spanish" } },
                { "cp285", new[] { "English (UK)" } },
                { "cp297", new[] { "French" } },
                { "cp424", new[] { "Hebrew" } },
                { "cp500", new[] { "Belgium" } },
                { "cp803", new[] { "Hebrew (old)" } },
                { "cp870", new[] { "Polish", "Slovenian" } },
                { "cp871", new[] { "Icelandic" } },
                { "cp875", new[] { "Greek" } },
                { "cp880", new[] { "Russian" } },
                { "cp930", new[] { "Japanese (Kana)" } },
                { "cp935", new[] { "Chinese simplified" } },
                { "cp937", new[] { "Chinese traditional" } },
                { "cp939", new[] { "Japanese (Latin)" } },
                { "cp1026", new[] { "Turkish" } },
                { "cp1123", new[] { "Ukrainian" } },
                { "cp1140", new[] { "English (US) with Euro" } },
                { "cp1141", new[] { "German with Euro" } },
                { "cp1142", new[] { "Norwegian with Euro" } },
                { "cp1143", new[] { "Finnish with Euro", "Swedish with Euro" } },
                { "cp1144", new[] { "Italian with Euro" } },
                { "cp1145", new[] { "Spanish with Euro" } },
                { "cp1146", new[] { "English (UK) with Euro" } },
                { "cp1147", new[] { "French with Euro" } },
                { "cp1148", new[] { "Belgium with Euro" } },
                { "cp1149", new[] { "Icelandic with Euro" } },
                { "cp1158", new[] { "Ukrainian with Euro" } },
                { "cp1160", new[] { "Thai" } },
                { "cp1388", new[] { "Chinese GB18030" } },
            };

            /// <summary>
            /// Localize the friendly names.
            /// </summary>
            [I18nInit]
            public static void Localize()
            {
                foreach (var entry in FriendlyNameTable)
                {
                    var index = 0;
                    foreach (var name in entry.Value)
                    {
                        FriendlyNameTable[entry.Key][index] = I18n.LocalizeGlobal(I18n.Combine("CodePage", entry.Key, index.ToString()), name);
                        index++;
                    }
                }
            }

            /// <summary>
            /// Returns the friendly version of a name.
            /// </summary>
            /// <param name="alias">Code page alias.</param>
            /// <returns>Friendly name, or null.</returns>
            public static IEnumerable<string> FriendlyNames(string alias)
            {
                if (FriendlyNameTable.TryGetValue(alias, out string[] friendlyNames))
                {
                    return friendlyNames;
                }

                return null;
            }
        }

        /// <summary>
        /// Code page comparer, ensures that "cp" names appear first, in numerical order.
        /// </summary>
        private class CodePageComparer : IComparer<string>
        {
            /// <summary>
            /// Comparison method.
            /// </summary>
            /// <param name="x">First string.</param>
            /// <param name="y">Second string.</param>
            /// <returns>-1 , 0, or +1.</y></returns>
            public int Compare(string x, string y)
            {
                if (x == y)
                {
                    return 0;
                }

                if (x.StartsWith("cp"))
                {
                    if (!y.StartsWith("cp"))
                    {
                        // cp is less than non-cp.
                        return -1;
                    }

                    // Both are cp, do a numeric comparison.
                    return int.Parse(x.Substring(2)) - int.Parse(y.Substring(2));
                }

                if (y.StartsWith("cp"))
                {
                    // x is non-cp; non-cp is greater than cp.
                    return 1;
                }

                // Neither is cp.
                return string.Compare(x, y);
            }
        }

        /// <summary>
        /// The description of a code page.
        /// </summary>
        private class CodePageDescription
        {
            /// <summary>
            /// The aliases for the code page.
            /// </summary>
            private readonly List<string> aliases = new List<string>();

            /// <summary>
            /// The friendly names for the code page.
            /// </summary>
            private readonly List<string> friendlyNames = new List<string>();

            /// <summary>
            /// Initializes a new instance of the <see cref="CodePageDescription"/> class.
            /// </summary>
            /// <param name="canonicalName">Canonical name.</param>
            /// <param name="aliases">List of aliases.</param>
            public CodePageDescription(string canonicalName, IEnumerable<string> aliases)
            {
                this.CanonicalName = canonicalName;
                this.aliases = aliases.ToList();
                var friendlyNames = FriendlyName.FriendlyNames(canonicalName);
                if (friendlyNames != null)
                {
                    this.friendlyNames = new List<string>(friendlyNames);
                }
            }

            /// <summary>
            /// Gets the canonical name.
            /// </summary>
            public string CanonicalName { get; }

            /// <summary>
            /// Gets additional aliases.
            /// </summary>
            public IReadOnlyList<string> Aliases => this.aliases;

            /// <summary>
            /// Gets the friendly names.
            /// </summary>
            public IReadOnlyList<string> FriendlyNames => this.friendlyNames;
        }
    }
}
