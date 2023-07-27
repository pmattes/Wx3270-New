// <copyright file="Program.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Splash
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The main program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            var pid = -1;
            string version = null;
            string copyright = null;
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-pid":
                            if (args.Length < i + 1 || !int.TryParse(args[i + 1], out pid))
                            {
                                pid = -1;
                            }

                            i++;
                            break;
                        case "-version":
                            if (args.Length >= i + 1)
                            {
                                version = args[++i];
                            }

                            break;
                        case "-copyright":
                            if (args.Length >= i + 1)
                            {
                                copyright = args[++i];
                            }

                            break;
                    }
                }

                if (args[0] == "-pid" && args.Length == 2)
                {
                    if (!int.TryParse(args[1], out pid))
                    {
                        pid = -1;
                    }
                }
            }
            var main = new SplashForm(pid, version, copyright);
            Application.Run(main);
        }
    }
}
