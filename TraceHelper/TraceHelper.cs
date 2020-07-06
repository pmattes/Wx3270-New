// <copyright file="TraceHelper.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace TraceHelper
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Trace file monitor.
    /// </summary>
    public class TraceHelper
    {
        /// <summary>
        /// Main procedure.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: TraceHelper <file-name>");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Environment.Exit(1);
            }

            var traceFile = args[0];

            // We speak UTF-8.
            Console.OutputEncoding = new UTF8Encoding();

            // Open the file.
            StreamReader stream = null;
            try
            {
                var f = File.Open(traceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stream = new StreamReader(f);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}: {1}", traceFile, e.Message);
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Environment.Exit(1);
            }

            // Set the console title.
            Console.Title = traceFile;

            // Do a simple read-retry loop.
            using (stream)
            {
                while (true)
                {
                    // Read until (current) EOF.
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }

                    // Pause and try some more.
                    Thread.Sleep(1 * 1000);
                }
            }
        }
    }
}
