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

    /// <summary>
    /// Localization utility.
    /// </summary>
    public class L10ntool
    {
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
        }

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            var verb = Verb.None;
            var parameters = new Dictionary<Parameter, string>();

            // Parse command-line arguments.
            int i;
            for (i = 0; i < args.Length; i++)
            {
                try
                {
                    switch (args[i])
                    {
                        // Verbs. One is required.
                        case "-create-csv":
                        case "-update-csv":
                        case "-csv-to-msgcat":
                            if (verb != Verb.None)
                            {
                                throw new ArgumentException("Only one verb can be specified");
                            }

                            verb = (Verb)Enum.Parse(typeof(Verb), args[i].Replace("-", string.Empty), ignoreCase: true);
                            break;

                        // Parameters.
                        case "-in-new-msgcat":
                        case "-in-old-msgcat":
                        case "-in-old-translated-msgcat":
                        case "-in-csv":
                        case "-out-csv":
                        case "-out-msgcat":
                            var p = (Parameter)Enum.Parse(typeof(Parameter), args[i].Replace("-", string.Empty), ignoreCase: true);
                            if (parameters.ContainsKey(p))
                            {
                                throw new ArgumentException($"Duplicate option {args[i]}");
                            }

                            parameters[p] = args[++i];
                            break;
                        default:
                            throw new ArgumentException($"Unknown option {args[i]}");
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException($"Missing value for {args[--i]}");
                }
            }

            var localize = new L10n();

            switch (verb)
            {
                case Verb.None:
                    throw new ArgumentException("Missing verb");
                case Verb.CreateCsv:
                    if (!parameters.ContainsKey(Parameter.InNewMsgcat) ||
                        !parameters.ContainsKey(Parameter.OutCsv))
                    {
                        throw new ArgumentException("Missing argument(s) for -create-csv");
                    }

                    if (parameters.Count != 2)
                    {
                        throw new ArgumentException("Extra argument(s) for -create-csv");
                    }

                    localize.CreateCsv(parameters[Parameter.InNewMsgcat], parameters[Parameter.OutCsv]);
                    break;
                case Verb.UpdateCsv:
                    if (!parameters.ContainsKey(Parameter.InOldMsgcat) ||
                        !parameters.ContainsKey(Parameter.InNewMsgcat) ||
                        !parameters.ContainsKey(Parameter.InOldTranslatedMsgcat) ||
                        !parameters.ContainsKey(Parameter.OutCsv))
                    {
                        throw new ArgumentException("Missing argument(s) for -update-csv");
                    }

                    if (parameters.Count != 4)
                    {
                        throw new ArgumentException("Extra argument(s) for -update-csv");
                    }

                    localize.UpdateCsv(
                        parameters[Parameter.InOldMsgcat],
                        parameters[Parameter.InNewMsgcat],
                        parameters[Parameter.InOldTranslatedMsgcat],
                        parameters[Parameter.OutCsv]);
                    break;
                case Verb.CsvToMsgcat:
                    if (!parameters.ContainsKey(Parameter.InCsv) ||
                        !parameters.ContainsKey(Parameter.OutMsgcat))
                    {
                        throw new ArgumentException("Missing argument(s) for -update-csv");
                    }

                    if (parameters.Count != 2)
                    {
                        throw new ArgumentException("Extra argument(s) for -csv-to-msgcat");
                    }

                    localize.CsvToMsgcat(parameters[Parameter.InCsv], parameters[Parameter.OutMsgcat]);
                    break;
            }
        }
    }
}
