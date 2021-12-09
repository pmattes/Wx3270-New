// <copyright file="Program.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    using I18nBase;

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
            Control mainControl = null;
            Wx3270App app = null;
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var main = new MainScreen();
                mainControl = main;
                app = new Wx3270App(main, main);
                app.Init(args);
                main.Init(app);

                // Set up forms localization. This needs to happen after main screen initialization.
                // It is only necessary if a message catalog file is not being used.
                if (!I18nBase.UsingMessageCatalog)
                {
                    I18n.SetupForms();
                }

                if (!string.IsNullOrEmpty(app.DumpLocalization))
                {
                    I18nBase.DumpMessages(app.DumpLocalization);
                }

                I18nBase.AllowDynamic = false;

                // Start the back end running.
                app.BackEnd.Run();

                // Trace how long it took to come up.
                stopwatch.Stop();
                Trace.Line(
                    Trace.Type.Window,
                    string.Format("Initialization took {0:F3} seconds", stopwatch.Elapsed.TotalMilliseconds / 1000.0));

                // Start the event loop.
                Application.Run();
            }
            catch (Exception e)
            {
                ErrorBox.ShowCopy(mainControl, e.ToString(), "Fatal User Interface Error");
                app?.BackEnd?.Exit(1);
                Environment.Exit(1);
            }
        }
    }
}
