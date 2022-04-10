// <copyright file="Prompt.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// The wx3270 prompt.
    /// </summary>
    public class Prompt
    {
        private const string BannerText = "wx3270 Prompt\n\n" +
                        "To execute one action and close this window, end the command line with '/'.\n" +
                        "To close this window, enter just '/' as the command line.\n" +
                        "To get help, use the '" + Constants.Action.Help + "()' action.\n";

        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Prompt));

        /// <summary>
        /// Message group for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Prompt));

        /// <summary>
        /// The emulator back end.
        /// </summary>
        private readonly IBackEnd backEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="Prompt"/> class.
        /// </summary>
        /// <param name="backEnd">Emulator back end.</param>
        public Prompt(IBackEnd backEnd)
        {
            this.backEnd = backEnd;
            this.backEnd.RegisterPassthru(Constants.Action.Help, this.UiHelp);
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.PromptError, "wx3270 Prompt");

            I18n.LocalizeGlobal(Message.Banner, BannerText);
            I18n.LocalizeGlobal(Message.QuitWarning, "Note: The 'Quit()' action will cause wx3270 to exit");
        }

        /// <summary>
        /// Start an instance of the wx3270 prompt.
        /// </summary>
        public void Start()
        {
            // Start a new console.
            var tempName = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(tempName, false, new UTF8Encoding(false)))
            {
                writer.WriteLine("x3270if.banner: {0}", Sanitize(I18n.Get(Message.Banner)));
                writer.WriteLine("x3270if.quit: {0}", Sanitize(I18n.Get(Message.QuitWarning)));
            }

            this.backEnd.RunAction(
                new BackEndAction(B3270.Action.Prompt, "wx3270", Constants.Action.Help, tempName),
                (cookie, success, result, misc) =>
                {
                    if (!success)
                    {
                        ErrorBox.Show(result, I18n.Get(Title.PromptError));
                    }
                });

            // Erase the temporary file in 5 seconds.
            var timer = new Timer
            {
                Interval = 5 * 1000,
                Enabled = true,
            };
            timer.Tick += (sender, args) =>
            {
                File.Delete(tempName);
                timer.Dispose();
            };
            timer.Start();
        }

        /// <summary>
        /// Sanitize text for display.
        /// </summary>
        /// <param name="text">Text to sanitize.</param>
        /// <returns>Sanitized text.</returns>
        private static string Sanitize(string text)
        {
            return text.Replace("\r", "\\r").Replace("\n", "\\n");
        }

        /// <summary>
        /// Provide help for the wx3270 prompt.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Command tag, for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiHelp(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            Wx3270App.GetHelp("wx3270 Prompt");
            result = string.Empty;
            return PassthruResult.Success;
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Error starting the helper app.
            /// </summary>
            public static readonly string PromptError = I18n.Combine(TitleName, "promptError");
        }

        /// <summary>
        /// Banner text.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Prompt banner message.
            /// </summary>
            public static readonly string Banner = I18n.Combine(MessageName, "banner");

            /// <summary>
            /// Prompt Quit warning.
            /// </summary>
            public static readonly string QuitWarning = I18n.Combine(MessageName, "quitWarning");
        }
    }
}
