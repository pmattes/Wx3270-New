// <copyright file="wx3270Prompt.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270Prompt
{
    using System;
    using System.IO.Pipes;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using I18nBase;

    /// <summary>
    /// wx3270 prompt application. Shuttles data between the emulator and a console window.
    /// </summary>
    public class Wx3270Prompt
    {
        /// <summary>
        /// Module name for localization.
        /// </summary>
        private const string MyName = "Wx3270Prompt";

        /// <summary>
        /// Maximum amount of data to read from the emulator.
        /// </summary>
        private const int MaxRead = 65536;

        /// <summary>
        /// Constant for the standard input handle.
        /// </summary>
        private const int StdInputHandle = -10;

        /// <summary>
        /// Localization group for titles.
        /// </summary>
        private static readonly string TitleName = I18nBase.Combine(MyName, "Title");

        /// <summary>
        /// Localization group for messages.
        /// </summary>
        private static readonly string MessageName = I18nBase.Combine(MyName, "Message");

        /// <summary>
        /// Synchronization object.
        /// </summary>
        private static readonly object Sync = new object();

        /// <summary>
        /// Main procedure.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            string culture = null;
            string dumpLocalization = null;
            string pipe = null;

            // Parse command line arguments.
            int? lastOpt = null;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (lastOpt.HasValue)
                    {
                        break;
                    }

                    switch (args[i].ToLowerInvariant())
                    {
                        case "-culture":
                            culture = args[++i];
                            break;
                        case "-dumplocalization":
                            dumpLocalization = args[++i];
                            break;
                        default:
                            if (args[i].StartsWith("-"))
                            {
                                Usage(string.Format("Unknown argument '{0}'", args[i]));
                            }

                            lastOpt = i;
                            break;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                Usage("Missing parameter value");
            }

            if (lastOpt.HasValue)
            {
                var positionalArgs = args.Skip(lastOpt.Value).ToList();
                var dashed = positionalArgs.Where(a => a.StartsWith("-"));
                if (dashed.Any())
                {
                    Usage(string.Format("Unknown or misformatted argument(s): {0}", string.Join(" ", dashed)));
                }

                if (positionalArgs.Count > 1)
                {
                    Usage("Extra arguments");
                }

                pipe = positionalArgs[0];
            }

            if (pipe == null && dumpLocalization == null)
            {
                Usage();
            }

            // Set up localization.
            I18nBase.Setup("Wx3270Prompt", culture);
            if (dumpLocalization != null)
            {
                I18nBase.DumpTree(dumpLocalization);
                if (pipe == null)
                {
                    Environment.Exit(0);
                }
            }

            // Set up the console to do UTF-8 I/O.
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Console.Title = I18nBase.Get(TitleName);

            //// Uncomment this to debug the timeout logic in wx3270.
            // Thread.Sleep(10 * 1000);

            // Connect to wx3270.
            var stream = new NamedPipeClientStream(".", pipe, PipeDirection.InOut);
            stream.Connect();
            stream.ReadMode = PipeTransmissionMode.Message;
            var streamTask = MonitorStream(stream);

            // Give verbose help.
            Console.WriteLine(I18nBase.Get(TitleName));
            Console.WriteLine();
            Console.WriteLine(I18nBase.Get(Message.Banner));
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(I18nBase.Get(Message.Note));
            Console.ForegroundColor = fg;
            Console.WriteLine();

            // Shuttle messages back and forth.
            var exitAfter = false;
            while (true)
            {
                if (exitAfter)
                {
                    break;
                }

                // Get a line of input.
                // Note: If wx3270 goes away while we are sitting here, we don't know.
                // I'm not sure how best to address this. Perhaps I could have a thread that
                // periodically checks stream.IsConnected.
                fg = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("wx3270> ");
                Console.ForegroundColor = fg;

                var input = RawReadConsole();
                if (input == null)
                {
                    // Probably won't happen any more. This was from when I used ^Z to close the window.
                    break;
                }

                input = input.Trim();
                if (input.EndsWith("/"))
                {
                    exitAfter = true;
                    input = input.Substring(0, input.Length - 1);
                }

                // Don't bother sending empty strings.
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                var output = RunCommand(stream, input);
                if (output == null)
                {
                    break;
                }

                if (output.Length > 1)
                {
                    Console.ForegroundColor = output[0] == '+' ? ConsoleColor.DarkGreen : ConsoleColor.Red;
                    Console.WriteLine(output.Substring(1));
                    Console.ForegroundColor = fg;
                }
            }
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18nBase.LocalizeFlat("wx3270 Prompt", TitleName);

            I18nBase.LocalizeFlat(
                "To close this window, type '/' and press Enter, or use the close button on the window border." + Environment.NewLine +
                "To run one command and close this window, end the command with '/', e.g., 'PF(1)/'." + Environment.NewLine +
                "To get help for this window, enter the command 'uHelp()'.",
                Message.Banner);
            I18nBase.LocalizeFlat("Note that the 'Quit()' and 'Exit()' actions will terminate wx3270, and not just close this window.", Message.Note);
            I18nBase.LocalizeFlat("Emulator connection broken.", Message.ConnectionBroken);
        }

        /// <summary>
        /// Write a usage message and exit.
        /// </summary>
        /// <param name="text">Optional text to display</param>
        private static void Usage(string text = null)
        {
            if (text != null)
            {
                Console.Error.WriteLine(text);
            }

            Console.Error.WriteLine("Usage: wx3270Prompt [-culture <culture>] [-dumplocalization <file>] <pipe-name>");
            Environment.Exit(1);
        }

        /// <summary>
        /// Send a command to the emulator.
        /// </summary>
        /// <param name="stream">Pipe stream</param>
        /// <param name="command">Command to send</param>
        /// <returns>Result text, or null if the connection is gone</returns>
        private static string RunCommand(PipeStream stream, string command)
        {
            lock (Sync)
            {
                var inputBuffer = new byte[MaxRead];
                int nread;

                // Write it to b3270, which might not be running any more.
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(command);
                    stream.Write(bytes, 0, bytes.Length);

                    // Read the response.
                    nread = stream.Read(inputBuffer, 0, MaxRead);
                    if (nread == 0)
                    {
                        // Graceful exit.
                        return null;
                    }
                }
                catch (System.IO.IOException)
                {
                    // Less-graceful exit.
                    return null;
                }

                // Translate the response.
                return Encoding.UTF8.GetString(inputBuffer, 0, nread);
            }
        }

        /// <summary>
        /// Monitor the named pipe for wx3270 exit.
        /// </summary>
        /// <param name="stream">Named pipe stream</param>
        /// <returns>Asynchronous task</returns>
        private static async Task MonitorStream(PipeStream stream)
        {
            while (true)
            {
                // Wait a bit.
                await Task.Delay(5 * 1000);

                // Send an empty command.
                var result = RunCommand(stream, "#");
                if (result == null)
                {
                    Console.WriteLine();
                    Console.WriteLine(I18nBase.Get(Message.ConnectionBroken));
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Read from the console using <code>ReadConsole</code> instead of <code>Console.Readline</code>.
        /// </summary>
        /// <returns>Next line of input</returns>
        private static string RawReadConsole()
        {
            IntPtr handle = NativeMethods.GetStdHandle(StdInputHandle);
            const int MaxCount = 1024;
            StringBuilder builder = new StringBuilder(MaxCount);

            if (NativeMethods.ReadConsole(handle, builder, MaxCount, out uint noCharacters, 0) == false)
            {
                return null;
            }
            else
            {
                builder.Length = (int)noCharacters;
                return builder.ToString();
            }
        }

        /// <summary>
        /// Message keys.
        /// </summary>
        private static class Message
        {
            /// <summary>
            /// The main banner.
            /// </summary>
            public static readonly string Banner = I18nBase.Combine(MessageName, "banner");

            /// <summary>
            /// The note at the bottom of the banner.
            /// </summary>
            public static readonly string Note = I18nBase.Combine(MessageName, "note");

            /// <summary>
            /// Connection broken.
            /// </summary>
            public static readonly string ConnectionBroken = I18nBase.Combine(MessageName, "connectionBroken");
        }
    }
}
