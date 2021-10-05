// <copyright file="L10ntool.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

/// <summary>
/// Wx3270 localization utility.
/// </summary>
namespace L10ntool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Localization utility.
    /// </summary>
    public class L10ntool
    {
        /// <summary>
        /// Map of verbs to parameters.
        /// </summary>
        private static readonly Dictionary<Verb, IEnumerable<Parameter>> ParamMap = new Dictionary<Verb, IEnumerable<Parameter>>
        {
            { Verb.CreateCsv, new[] { Parameter.InNewMsgcat, Parameter.OutCsv } },
            { Verb.UpdateCsv, new[] { Parameter.InOldMsgcat, Parameter.InNewMsgcat, Parameter.InOldTranslatedMsgcat, Parameter.OutCsv } },
            { Verb.CsvToMsgcat, new[] { Parameter.InCsv, Parameter.OutMsgcat } },
            { Verb.RenameMsgcat, new[] { Parameter.InBeforeMsgcat, Parameter.InAfterMsgcat, Parameter.InOldTranslatedMsgcat, Parameter.OutMsgcat } },
            { Verb.AddMissing, new[] { Parameter.InNewMsgcat, Parameter.InOldTranslatedMsgcat, Parameter.OutMsgcat } },
        };

        /// <summary>
        /// Operation to perform.
        /// </summary>
        private enum Verb
        {
            /// <summary>
            /// None specified.
            /// </summary>
            None,

            /// <summary>
            /// Create a CSV file from a message catalog file.
            /// </summary>
            CreateCsv,

            /// <summary>
            /// Update a CSV file from old and new message catalog files and a translated old message catalog file.
            /// </summary>
            UpdateCsv,

            /// <summary>
            /// Create a message catalog file from a (presumably translated) CSV file.
            /// </summary>
            CsvToMsgcat,

            /// <summary>
            /// Rename entries in a message catalog.
            /// </summary>
            RenameMsgcat,

            /// <summary>
            /// Add missing entries to a message catalog.
            /// </summary>
            AddMissing,
        }

        /// <summary>
        /// Parameter types.
        /// </summary>
        private enum Parameter
        {
            /// <summary>
            /// New message catalog.
            /// </summary>
            InNewMsgcat,

            /// <summary>
            /// Old message catalog.
            /// </summary>
            InOldMsgcat,

            /// <summary>
            /// Old translated message catalog.
            /// </summary>
            InOldTranslatedMsgcat,

            /// <summary>
            /// Input CSV file.
            /// </summary>
            InCsv,

            /// <summary>
            /// Output CSV file.
            /// </summary>
            OutCsv,

            /// <summary>
            /// Output message catalog.
            /// </summary>
            OutMsgcat,

            /// <summary>
            /// Rename 'before' message catalog.
            /// </summary>
            InBeforeMsgcat,

            /// <summary>
            /// Rename 'after' message catalog.
            /// </summary>
            InAfterMsgcat,
        }

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            var verb = Verb.None;
            var parameters = new Dictionary<Parameter, string>();

            var verbOptions = Enum.GetNames(typeof(Verb)).Where(name => name != nameof(Verb.None)).Select(name => EnumToOption(name)).ToArray();
            var parameterOptions = Enum.GetNames(typeof(Parameter)).Select(name => EnumToOption(name)).ToArray();

            try
            {
                // Parse command-line arguments.
                int i;
                for (i = 0; i < args.Length; i++)
                {
                    try
                    {
                        if (verbOptions.Contains(args[i]))
                        {
                            if (verb != Verb.None)
                            {
                                throw new ArgumentException("Only one verb can be specified");
                            }

                            verb = (Verb)Enum.Parse(typeof(Verb), args[i].Replace("-", string.Empty), ignoreCase: true);
                        }
                        else if (parameterOptions.Contains(args[i]))
                        {
                            var p = (Parameter)Enum.Parse(typeof(Parameter), args[i].Replace("-", string.Empty), ignoreCase: true);
                            if (parameters.ContainsKey(p))
                            {
                                throw new ArgumentException($"Duplicate option {args[i]}");
                            }

                            parameters[p] = args[++i];
                        }
                        else
                        {
                            throw new ArgumentException($"Unknown option {args[i]}.");
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new ArgumentException($"Missing value for {args[--i]}.");
                    }
                }

                if (verb == Verb.None)
                {
                    throw new ArgumentException(
                            string.Format(
                                "Missing verb. Verbs are: {0}.",
                                string.Join(
                                    " ",
                                    Enum.GetNames(typeof(Verb)).Where(name => name != nameof(Verb.None)).Select(name => EnumToOption(name)))));
                }

                var map = ParamMap[verb];
                if (!map.All(p => parameters.ContainsKey(p)))
                {
                    throw new ArgumentException(
                        string.Format(
                            "Missing argument(s) for {0}. Requires {1}.",
                            EnumToOption(verb.ToString()),
                            string.Join(" ", map.Select(p => EnumToOption(p.ToString())))));
                }

                if (parameters.Count != map.Count())
                {
                    throw new ArgumentException(
                        string.Format(
                            "Extra parameter(s) for {0}: {1}.",
                            EnumToOption(verb.ToString()),
                            string.Join(" ", parameters.Keys.Except(map).Select(p => EnumToOption(p.ToString())))));
                }

                var localize = new L10n();
                switch (verb)
                {
                    case Verb.CreateCsv:
                        localize.CreateCsv(parameters[Parameter.InNewMsgcat], parameters[Parameter.OutCsv]);
                        break;
                    case Verb.UpdateCsv:
                        localize.UpdateCsv(
                            parameters[Parameter.InOldMsgcat],
                            parameters[Parameter.InNewMsgcat],
                            parameters[Parameter.InOldTranslatedMsgcat],
                            parameters[Parameter.OutCsv]);
                        break;
                    case Verb.CsvToMsgcat:
                        localize.CsvToMsgcat(parameters[Parameter.InCsv], parameters[Parameter.OutMsgcat]);
                        break;
                    case Verb.RenameMsgcat:
                        localize.Rename(
                            parameters[Parameter.InBeforeMsgcat],
                            parameters[Parameter.InAfterMsgcat],
                            parameters[Parameter.InOldTranslatedMsgcat],
                            parameters[Parameter.OutMsgcat]);
                        break;
                    case Verb.AddMissing:
                        localize.AddMissing(
                            parameters[Parameter.InNewMsgcat],
                            parameters[Parameter.InOldTranslatedMsgcat],
                            parameters[Parameter.OutMsgcat]);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }
        }

        /// <summary>
        /// Translates an enumeration name to an option name.
        /// </summary>
        /// <param name="enumName">Enumeration to translate.</param>
        /// <returns>Translated enumeration.</returns>
        private static string EnumToOption(string enumName)
        {
            var outList = new List<char>();
            foreach (var c in enumName)
            {
                if (c == char.ToUpperInvariant(c))
                {
                    outList.Add('-');
                    outList.Add(char.ToLowerInvariant(c));
                }
                else
                {
                    outList.Add(c);
                }
            }

            return new string(outList.ToArray());
        }
    }
}
