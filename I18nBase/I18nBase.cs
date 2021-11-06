// <copyright file="I18nBase.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace I18nBase
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    /// <summary>
    /// Internationalization and localization support.
    /// </summary>
    public static class I18nBase
    {
        /// <summary>
        /// The path separator.
        /// </summary>
        private const string Separator = ".";

        /// <summary>
        /// The directory containing message catalogs.
        /// </summary>
        private const string CatalogDir = "MessageCatalog";

        /// <summary>
        /// Regular expression for action names.
        /// </summary>
        private static readonly Regex ActionRegex = new Regex("[A-Za-z][A-Za-z0-9_]*\\(\\)");

        /// <summary>
        /// Regular expression for quoted action names.
        /// </summary>
        private static readonly Regex QuotedActionRegex = new Regex("'[A-Za-z][A-Za-z0-9_]*\\(\\)'");

        /// <summary>
        /// Some strings that don't get translated.
        /// </summary>
        private static readonly HashSet<string> NoTranslate = new HashSet<string>
        {
            "wx3270", "wx3270>", "pr3287", "b3270", "shift", "cmd.exe", "Shift", "Ctrl", "Alt", "Num", "Lock",
            "Python", "PowerShell", "VBScript", "JScript", "Alt+Shift",
        };

        /// <summary>
        /// Localizations of known strings.
        /// </summary>
        private static Dictionary<string, string> localized = new Dictionary<string, string>();

        /// <summary>
        /// The culture-specific DLL.
        /// </summary>
        private static Assembly i18Assembly;

        /// <summary>
        /// The method information for the LocalizeWord method.
        /// </summary>
        private static MethodInfo localizeWordMethodInfo;

        /// <summary>
        /// Gets a value indicating whether a message catalog is in use.
        /// </summary>
        public static bool UsingMessageCatalog { get; private set; }

        /// <summary>
        /// Gets or sets the requested culture.
        /// </summary>
        public static string RequestedCulture { get; set; } = "None";

        /// <summary>
        /// Gets or sets the effective cultire.
        /// </summary>
        public static string EffectiveCulture { get; set; } = "en-US";

        /// <summary>
        /// Gets or sets a value indicating whether dynamic resolution is allowed.
        /// </summary>
        public static bool AllowDynamic { get; set; } = true;

        /// <summary>
        /// Set up culture-specific information.
        /// </summary>
        /// <param name="nameSpaceName">Name space name.</param>
        /// <param name="cultureName">Culture name.</param>
        /// <param name="forceBootstrap">True to force bootstrapping instead of using a catalog or DLL.</param>
        public static void Setup(string nameSpaceName, string cultureName = null, bool forceBootstrap = false)
        {
            try
            {
                if (forceBootstrap)
                {
                    // Don't look for a message catalog or a DLL, but do static initialilzation in the finally block below.
                    return;
                }

                // Get the current culture if a specific one was not requested.
                if (cultureName == null)
                {
                    cultureName = CultureInfo.CurrentCulture.Name;
                }

                // Remember what was requested, for display.
                RequestedCulture = cultureName;

                // Try the specific culture name.
                var effectiveCultureName = cultureName;
                if (!TryCulture(cultureName, out string foundFile, out i18Assembly))
                {
                    if (cultureName.Contains("-"))
                    {
                        // Try again without the country suffix.
                        effectiveCultureName = cultureName.Substring(0, cultureName.IndexOf('-'));
                        if (!TryCulture(effectiveCultureName, out foundFile, out i18Assembly))
                        {
                            return;
                        }
                    }
                }

                if (foundFile != null)
                {
                    // Deserialize the message catalog.
                    var serializer = new JsonSerializer();
                    using (StreamReader t = new StreamReader(foundFile, new UTF8Encoding()))
                    {
                        using (JsonReader reader = new JsonTextReader(t))
                        {
                            try
                            {
                                localized = serializer.Deserialize<Dictionary<string, string>>(reader);
                                UsingMessageCatalog = true;
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Cannot deserialize message catalog '{foundFile}': {e.Message}");
                            }
                        }
                    }
                }
                else if (i18Assembly != null)
                {
                    // Find the localize method in the DLL.
                    var name = cultureName.Replace("-", "_");
                    var localizeType = i18Assembly.GetType(name + "." + name);
                    if (localizeType == null)
                    {
                        throw new Exception("Cannot find type " + name + "." + name + " in i18n DLL");
                    }

                    localizeWordMethodInfo = localizeType.GetMethod("LocalizeWord");
                    if (localizeWordMethodInfo == null)
                    {
                        throw new Exception("Cannot find Localize method in i18n DLL");
                    }
                }
                else
                {
                    return;
                }

                EffectiveCulture = effectiveCultureName;
            }
            finally
            {
                if (!UsingMessageCatalog)
                {
                    // Call static initialization.
                    foreach (var m in Assembly.GetCallingAssembly()
                        .GetTypes()
                        .Where(t => t.IsClass && t.Namespace == nameSpaceName)
                        .SelectMany(t => t.GetMethods().Where(m => m.IsStatic)))
                    {
                        if (m.CustomAttributes.Any(a => a.AttributeType == typeof(I18nInitAttribute)))
                        {
                            m.Invoke(null, new object[0]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dump the message catalog in JSON format.
        /// </summary>
        /// <param name="fileName">File to save into.</param>
        public static void DumpMessages(string fileName = null)
        {
            TextWriter t;
            if (fileName != null)
            {
                t = new StreamWriter(fileName, false, new UTF8Encoding());
            }
            else
            {
                t = Console.Out;
            }

            var sortedMessages = new Dictionary<string, string>();
            foreach (var key in localized.Keys.OrderBy(k => k))
            {
                sortedMessages[key] = localized[key];
            }

            var serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented,
            };
            using (JsonWriter writer = new JsonTextWriter(t))
            {
                serializer.Serialize(writer, sortedMessages);
            }

            if (fileName != null)
            {
                t.Dispose();
            }
        }

        /// <summary>
        /// Combine path components.
        /// </summary>
        /// <param name="components">Components to combine.</param>
        /// <returns>Combined components.</returns>
        public static string Combine(params string[] components)
        {
            return string.Join(Separator, components.Where(component => !string.IsNullOrEmpty(component)));
        }

        /// <summary>
        /// Combine path components.
        /// </summary>
        /// <param name="components">Components to combine.</param>
        /// <returns>Combined components.</returns>
        public static string Combine(IEnumerable<string> components)
        {
            return string.Join(Separator, components.Where(component => !string.IsNullOrEmpty(component)));
        }

        /// <summary>
        /// Get a localized string by path.
        /// </summary>
        /// <param name="path">Path name.</param>
        /// <param name="fallback">Fallback value.</param>
        /// <returns>Localized string.</returns>
        public static string Get(string path, string fallback = null)
        {
            if (fallback != null && !localized.ContainsKey(path))
            {
                return fallback;
            }

            return localized[path];
        }

        /// <summary>
        /// Localize a string by calling the DLL. Remember the object name and cache the value.
        /// </summary>
        /// <param name="s">String to localize.</param>
        /// <param name="path">Object path.</param>
        /// <param name="always">Always localize, even if uppercase.</param>
        /// <returns>Translated string.</returns>
        public static string LocalizeFlat(string s, string path, bool always = false)
        {
            // Normalize the path.
            path = path.Replace(" ", "_").Replace(Environment.NewLine, "_");

            if (localized.TryGetValue(path, out string trans))
            {
                // Already known.
                return trans;
            }

            if (!AllowDynamic)
            {
                throw new Exception($"Unitialized i18n for {path}");
            }

            if (localizeWordMethodInfo == null)
            {
                // No DLL, leave it in U.S. English.
                trans = s;
            }
            else
            {
                // Ask the DLL, word by word.
                var newString = new List<string>();
                foreach (var word in SplitWhite(s))
                {
                    var newWord = word;
                    do
                    {
                        if ((!always && newWord == newWord.ToUpperInvariant())
                            || newWord == " "
                            || newWord == Environment.NewLine
                            || newWord == string.Empty
                            || newWord.StartsWith("{")
                            || newWord.Length == 1
                            || NoTranslate.Contains(newWord)
                            || ActionRegex.IsMatch(newWord)
                            || QuotedActionRegex.IsMatch(newWord))
                        {
                            break;
                        }

                        // Ask the DLL.
                        newWord = (string)localizeWordMethodInfo.Invoke(null, new object[] { newWord });
                    }
                    while (false);

                    newString.Add(newWord);
                }

                trans = string.Join(string.Empty, newString);
            }

            // Remember.
            localized[path] = trans;
            return trans;
        }

        /// <summary>
        /// Look for a file or DLL matching a culture name.
        /// </summary>
        /// <param name="cultureName">Culture name.</param>
        /// <param name="foundFileName">Returned found file name.</param>
        /// <param name="foundAssembly">Returned found assembly.</param>
        /// <returns>True if match found.</returns>
        private static bool TryCulture(string cultureName, out string foundFileName, out Assembly foundAssembly)
        {
            foundFileName = null;
            foundAssembly = null;

            // Try for a match on a file.
            var tryFile = Path.Combine(CatalogDir, cultureName);
            if (File.Exists(tryFile))
            {
                foundFileName = tryFile;
                return true;
            }

            // Try for a match on a DLL.
            try
            {
                foundAssembly = Assembly.LoadFrom("i18n-" + cultureName + ".dll");
                return true;
            }
            catch (FileNotFoundException)
            {
            }

            return false;
        }

        /// <summary>
        /// Split a string, but leave the whitespace as words.
        /// </summary>
        /// <param name="s">String to split.</param>
        /// <returns>Split string.</returns>
        private static List<string> SplitWhite(string s)
        {
            var ret = new List<string>();

            var t = s;
            foreach (var word in s.Split(new[] { " ", Environment.NewLine }, StringSplitOptions.None))
            {
                ret.Add(word);
                t = t.Substring(word.Length);
                if (t.StartsWith(" "))
                {
                    ret.Add(" ");
                    t = t.Substring(1);
                }
                else if (t.StartsWith(Environment.NewLine))
                {
                    ret.Add(Environment.NewLine);
                    t = t.Substring(Environment.NewLine.Length);
                }
            }

            return ret;
        }
    }
}
