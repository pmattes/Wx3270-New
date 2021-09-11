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
        /// The set of known strings.
        /// </summary>
        private static readonly Dictionary<string, string> KnownStrings = new Dictionary<string, string>();

        /// <summary>
        /// Regular expression for action names.
        /// </summary>
        private static readonly Regex actionRegex = new Regex("[A-Za-z][A-Za-z0-9_]*\\(\\)");

        /// <summary>
        /// Regular expression for quoted action names.
        /// </summary>
        private static readonly Regex quotedActionRegex = new Regex("'[A-Za-z][A-Za-z0-9_]*\\(\\)'");

        /// <summary>
        /// Localizations of known strings.
        /// </summary>
        private static Dictionary<string, string> Localized = new Dictionary<string, string>();

        /// <summary>
        /// The culture-specific DLL.
        /// </summary>
        private static Assembly i18Assembly;

        /// <summary>
        /// The method information for the LocalizeWord method.
        /// </summary>
        private static MethodInfo localizeWordMethodInfo;
        
        /// <summary>
        /// Some strings that don't get translated.
        /// </summary>
        private static readonly HashSet<string> noTranslate = new HashSet<string>
        {
            "wx3270", "wx3270>", "pr3287", "b3270", "shift", "cmd.exe", "Shift", "Ctrl", "Alt", "Num", "Lock",
            "Python", "PowerShell", "VBScript", "JScript", "Alt+Shift"
        };

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
        /// Split a string, but leave the whitespace as words.
        /// </summary>
        /// <param name="s">String to split</param>
        /// <returns>Split string</returns>
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

        /// <summary>
        /// Set up culture-specific information.
        /// </summary>
        /// <param name="nameSpaceName">Name space name</param>
        /// <param name="cultureName">Culture name</param>
        public static void Setup(string nameSpaceName, string cultureName = null)
        {
            string foundFile = null;
            try
            {
                if (cultureName == null)
                {
                    cultureName = CultureInfo.CurrentCulture.Name;
                }

                RequestedCulture = cultureName;
                if (RequestedCulture == "en-US")
                {
                    return;
                }

                try
                {
                    i18Assembly = Assembly.LoadFrom("i18n-" + cultureName + ".dll");
                }
                catch (FileNotFoundException)
                {
                    var tryFile = Path.Combine(CatalogDir, cultureName);
                    if (File.Exists(tryFile))
                    {
                        foundFile = tryFile;
                    }

                    if (foundFile == null && cultureName.Contains("-"))
                    {
                        // Try again without the country suffix.
                        cultureName = cultureName.Substring(0, cultureName.IndexOf('-'));
                        try
                        {
                            i18Assembly = Assembly.LoadFrom("i18n-" + cultureName + ".dll");
                        }
                        catch (FileNotFoundException)
                        {
                            tryFile = Path.Combine(CatalogDir, cultureName);
                            if (File.Exists(tryFile))
                            {
                                foundFile = tryFile;
                            }
                        }
                    }
                }

                if (i18Assembly == null && foundFile == null)
                {
                    return;
                }

                if (i18Assembly != null)
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
                else if (foundFile != null)
                {
                    // Deserialize the message catalog.
                    var serializer = new JsonSerializer();
                    using (StreamReader t = new StreamReader(foundFile, new UTF8Encoding()))
                    {
                        using (JsonReader reader = new JsonTextReader(t))
                        {
                            try
                            {
                                Localized = serializer.Deserialize<Dictionary<string,string>>(reader);
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Cannot deserialize message catalog '{foundFile}': {e.Message}");
                            }
                        }
                    }
                }
                else
                {
                    return;
                }

                EffectiveCulture = cultureName;
            }
            finally
            {
                // We always call the static localization code for each class.
                foreach (var m in Assembly.GetCallingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == nameSpaceName).SelectMany(t => t.GetMethods().Where(m => m.IsStatic)))
                {
                    if (m.CustomAttributes.Any(a => a.AttributeType == typeof(I18nInitAttribute)))
                    {
                        m.Invoke(null, new object[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Dump the localization tree.
        /// </summary>
        /// <param name="fileName">File to save into</param>
        public static void DumpTree(string fileName = null)
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

            var serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented
            };
            using (JsonWriter writer = new JsonTextWriter(t))
            {
                serializer.Serialize(writer, KnownStrings);
            }

            if (fileName != null)
            {
                t.Dispose();
            }
        }

        /// <summary>
        /// Combine path components.
        /// </summary>
        /// <param name="components">Components to combine</param>
        /// <returns>Combined components</returns>
        public static string Combine(params string[] components)
        {
            return string.Join(Separator, components);
        }

        /// <summary>
        /// Combine path components.
        /// </summary>
        /// <param name="components">Components to combine</param>
        /// <returns>Combined components</returns>
        public static string Combine(IEnumerable<string> components)
        {
            return string.Join(Separator, components);
        }

        /// <summary>
        /// Get a localized string by path.
        /// </summary>
        /// <param name="path">Path name</param>
        /// <param name="fallback">Fallback value</param>
        /// <returns>Localized string</returns>
        public static string Get(string path, string fallback = null)
        {
            if (fallback != null && !Localized.ContainsKey(path))
            {
                return fallback;
            }

            return Localized[path];
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

            if (Localized.TryGetValue(path, out string localized))
            {
                // Already known.
                return localized;
            }

            if (!AllowDynamic)
            {
                throw new Exception("Improper i18n");
            }

            if (localizeWordMethodInfo == null)
            {
                // No DLL, leave it in English.
                localized = s;
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
                            || noTranslate.Contains(newWord)
                            || actionRegex.IsMatch(newWord)
                            || quotedActionRegex.IsMatch(newWord))
                        {
                            break;
                        }

                        // Ask the DLL.
                        newWord = (string)localizeWordMethodInfo.Invoke(null, new object[] { newWord });

                    } while (false);

                    newString.Add(newWord);
                }

                localized = string.Join("", newString);
            }

            // Remember.
            KnownStrings[path] = s;
            Localized[path] = localized;
            return localized;
        }
    }
}
