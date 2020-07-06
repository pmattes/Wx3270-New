// <copyright file="ActionSyntax.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using I18nBase;

    /// <summary>
    /// The macro editor.
    /// </summary>
    public static class ActionSyntax
    {
        /// <summary>
        /// Localization group for messages.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(ActionSyntax));

        /// <summary>
        /// Finite-state machine states for syntax checking.
        /// </summary>
        private enum ParserState
        {
            /// <summary>
            /// Before or between actions.
            /// </summary>
            Base,

            /// <summary>
            /// Inside an action name.
            /// </summary>
            ActionName,

            /// <summary>
            /// Inside the argument list.
            /// </summary>
            ReadyForArg,

            /// <summary>
            /// Inside an unquoted argument.
            /// </summary>
            UnquotedArg,

            /// <summary>
            /// Inside a quoted argument.
            /// </summary>
            QuotedArg,

            /// <summary>
            /// Backslash seen inside a quote.
            /// </summary>
            BackslashInsideQuote,

            /// <summary>
            /// Double backslash seen inside a quote.
            /// </summary>
            DoubleBackslashInsideQuote,

            /// <summary>
            /// After an argument.
            /// </summary>
            ArgEnd,

            /// <summary>
            /// After a comma (almost the same as <see cref="ReadyForArg"/>.
            /// </summary>
            ArgComing,
        }

        /// <summary>
        /// Localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Message.InvalidActionNameCharacter, "Invalid action name character");
            I18n.LocalizeGlobal(Message.ExpectedCommaRParen, "Expected ',' or ')'");
            I18n.LocalizeGlobal(Message.ExpectedLParen, "Expected '('");
            I18n.LocalizeGlobal(Message.ExpectedRParen, "Expected ')'");
            I18n.LocalizeGlobal(Message.ExpectedDQuote, "Expected '\"'");
        }

        /// <summary>
        /// Macro syntax checker, for one line.
        /// </summary>
        /// <param name="macro">Text to examine.</param>
        /// <param name="column">Returned error column.</param>
        /// <param name="index">Returned error index.</param>
        /// <param name="errorText">Returned error text.</param>
        /// <param name="args">Returned arguments.</param>
        /// <returns>True if the syntax is correct.</returns>
        public static bool CheckLine(string macro, out int column, ref int index, out string errorText, out string[] args)
        {
            // Make the compiler happy.
            column = 1;
            errorText = string.Empty;
            args = null;

            var argsList = new List<string>();
            var arg = string.Empty;

            // The syntax is:
            // name([arg][,arg...])...
            // Arguments can be quoted with double-quotes (which can hide parentheses, spaces and commas).
            // Inside a double-quoted argument, backslash escapes a double-quote character, except at the end,
            // where two backslashes (and the trailing double quote) become a single backslash.
            var state = ParserState.Base;
            foreach (var c in macro)
            {
                switch (state)
                {
                    case ParserState.Base:
                        // Start of an action name.
                        if (c == ' ')
                        {
                            // Spaces before and between actions are fine.
                            continue;
                        }
                        else if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                        {
                            // Identifiers start with letters or an underscore.
                            state = ParserState.ActionName;
                        }
                        else
                        {
                            // Anything else is bad.
                            errorText = I18n.Get(Message.InvalidActionNameCharacter) + $": '{c}'";
                            return false;
                        }

                        break;

                    case ParserState.ActionName:
                        // We've seen at least one character in an action name.
                        if (c == '(')
                        {
                            // After the name, a left paren. Ready for arguments.
                            state = ParserState.ReadyForArg;
                        }
                        else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' || c == '-'))
                        {
                            // Names continue with letters, digits, underscores and dashes.
                            errorText = I18n.Get(Message.InvalidActionNameCharacter) + $": '{c}'";
                            return false;
                        }

                        break;

                    case ParserState.ReadyForArg:
                    case ParserState.ArgComing:
                        // Ready for an argument.
                        if (c == '"')
                        {
                            // Quoted argument.
                            state = ParserState.QuotedArg;
                        }
                        else if (c == ')')
                        {
                            // End of arguments.
                            if (state == ParserState.ArgComing)
                            {
                                argsList.Add(string.Empty);
                            }

                            state = ParserState.Base;
                        }
                        else if (c != ',' && c != ' ')
                        {
                            // Unquoted argument.
                            state = ParserState.UnquotedArg;
                            arg += c;
                        }
                        else if (c == ',')
                        {
                            argsList.Add(string.Empty);
                            state = ParserState.ArgComing;
                        }

                        break;

                    case ParserState.QuotedArg:
                        // Inside a quoted argument.
                        if (c == '\\')
                        {
                            // One backslash inside a quoted argument.
                            state = ParserState.BackslashInsideQuote;
                        }
                        else if (c == '"')
                        {
                            // End of quoted parameter.
                            state = ParserState.ArgEnd;
                            argsList.Add(arg);
                            arg = string.Empty;
                        }
                        else
                        {
                            arg += c;
                        }

                        break;

                    case ParserState.BackslashInsideQuote:
                        // We've seen one backslash inside a quoted argument.
                        if (c == '\\')
                        {
                            // Now we've seen (at least) two.
                            state = ParserState.DoubleBackslashInsideQuote;
                        }
                        else
                        {
                            // No special meaning.
                            state = ParserState.QuotedArg;
                            if (c != '"')
                            {
                                arg += '\\';
                            }

                            arg += c;
                        }

                        break;

                    case ParserState.DoubleBackslashInsideQuote:
                        // We've seen (at least) two backslashes inside a quoted argument.
                        if (c == '\\')
                        {
                            // More than two.
                            arg += '\\';
                            continue;
                        }
                        else if (c == '"')
                        {
                            // Two backslashes before a double quote means end the argument with a single backslash.
                            state = ParserState.ArgEnd;
                            arg += '\\';
                            argsList.Add(arg);
                            arg = string.Empty;
                        }
                        else
                        {
                            // Two (or more) backslashes *not* followed by a double quote has no special meaning.
                            state = ParserState.QuotedArg;
                            arg += "\\\\";
                            arg += c;
                        }

                        break;

                    case ParserState.UnquotedArg:
                        // Inside an unquoted argument.
                        if (c == ')')
                        {
                            // End of action.
                            state = ParserState.Base;
                            argsList.Add(arg);
                            arg = string.Empty;
                        }
                        else if (c == ',')
                        {
                            // Ready for next argument.
                            state = ParserState.ArgComing;
                            argsList.Add(arg);
                            arg = string.Empty;
                        }
                        else if (c == ' ')
                        {
                            // End of parameter.
                            state = ParserState.ArgEnd;
                            argsList.Add(arg);
                            arg = string.Empty;
                        }
                        else
                        {
                            arg += c;
                        }

                        break;

                    case ParserState.ArgEnd:
                        // After an argument.
                        if (c == ')')
                        {
                            // Done with this action.
                            state = ParserState.Base;
                        }
                        else if (c == ',')
                        {
                            // Ready for the next argument.
                            state = ParserState.ArgComing;
                        }
                        else if (c != ' ')
                        {
                            // You can have more white space, but you can't start another argument without a comma.
                            errorText = I18n.Get(Message.ExpectedCommaRParen);
                            return false;
                        }

                        break;
                }

                column++;
                index++;
            }

            // At the end of the string.
            switch (state)
            {
                case ParserState.Base:
                    args = argsList.ToArray();
                    return true;
                case ParserState.ActionName:
                    errorText = I18n.Get(Message.ExpectedLParen);
                    break;
                default:
                case ParserState.ReadyForArg:
                case ParserState.UnquotedArg:
                case ParserState.ArgEnd:
                    errorText = I18n.Get(Message.ExpectedRParen);
                    break;
                case ParserState.QuotedArg:
                case ParserState.BackslashInsideQuote:
                case ParserState.DoubleBackslashInsideQuote:
                    errorText = I18n.Get(Message.ExpectedDQuote);
                    break;
            }

            return false;
        }

        /// <summary>
        /// Macro syntax checker, for one line.
        /// </summary>
        /// <param name="macro">Text to examine.</param>
        /// <param name="column">Returned error column.</param>
        /// <param name="index">Returned error index.</param>
        /// <param name="errorText">Returned error text.</param>
        /// <returns>True if the syntax is correct.</returns>
        public static bool CheckLine(string macro, out int column, ref int index, out string errorText)
        {
            return CheckLine(macro, out column, ref index, out errorText, out _);
        }

        /// <summary>
        /// Macro syntax checker.
        /// </summary>
        /// <param name="macro">Text to examine.</param>
        /// <param name="line">Returned error line.</param>
        /// <param name="column">Returned error column.</param>
        /// <param name="index">Returned error inde.</param>
        /// <param name="errorText">Returned error text.</param>
        /// <returns>True if the syntax is correct.</returns>
        public static bool Check(string macro, out int line, out int column, out int index, out string errorText)
        {
            // Make the compiler happy.
            line = 1;
            column = 1;
            index = 0;
            errorText = string.Empty;

            foreach (var textLine in macro.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                // Skip comments.
                if (textLine.TrimStart(' ').StartsWith("#"))
                {
                    index += textLine.Length;
                }
                else if (!CheckLine(textLine, out column, ref index, out errorText))
                {
                    return false;
                }

                line++;
                index += Environment.NewLine.Length;
            }

            // Good to go.
            return true;
        }

        /// <summary>
        /// Format a macro for running.
        /// </summary>
        /// <param name="macro">Macro to format.</param>
        /// <returns>Formatted macro.</returns>
        public static string FormatForRun(string macro)
        {
            // Remove comment lines, then join each non-empty line with a space.
            return string.Join(
                " ",
                macro.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim(' '))
                    .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")));
        }

        /// <summary>
        /// Message types.
        /// </summary>
        private static class Message
        {
            /// <summary>
            /// Invalid action name character.
            /// </summary>
            public static readonly string InvalidActionNameCharacter = I18n.Combine(MessageName, "invalidActionNameCharacter");

            /// <summary>
            /// Expected a comma or a right parenthesis.
            /// </summary>
            public static readonly string ExpectedCommaRParen = I18n.Combine(MessageName, "expectedCommaRParen");

            /// <summary>
            /// Expected a left parenthesis.
            /// </summary>
            public static readonly string ExpectedLParen = I18n.Combine(MessageName, "expectedLParen");

            /// <summary>
            /// Expected a right parenthesis.
            /// </summary>
            public static readonly string ExpectedRParen = I18n.Combine(MessageName, "expectedRParen");

            /// <summary>
            /// Expected a double quote.
            /// </summary>
            public static readonly string ExpectedDQuote = I18n.Combine(MessageName, "expectedDQuote");
        }
    }
}
