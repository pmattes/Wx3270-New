// <copyright file="Program.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
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
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var main = new MainScreen();
                mainControl = main;
                var app = new Wx3270App(main, main);
                app.Init(args);
                main.Init(app);

                // Set up forms localization. This needs to happen after main screen initialization.
                I18n.SetupForms();
                if (!string.IsNullOrEmpty(app.DumpLocalization))
                {
                    I18nBase.DumpMessages(app.DumpLocalization);
                }

                if (!string.IsNullOrEmpty(app.DumpMissingMessages))
                {
                    I18nBase.DumpMissingMessages(app.DumpMissingMessages);
                }

                I18nBase.AllowDynamic = false;

                // Start the back end running.
                app.BackEnd.Run();

                // Start the event loop.
                Application.Run();
            }
            catch (Exception e)
            {
                ErrorBox.ShowCopy(mainControl, e.ToString(), "Fatal User Interface Error");
            }
        }
    }
}
