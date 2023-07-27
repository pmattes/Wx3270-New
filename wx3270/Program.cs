// <copyright file="Program.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
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

                // Proceed with initialization. */
                var splash = args.Any(arg => arg.ToLowerInvariant() == Constants.Option.NoSplash) ? null : StartSplash();
                var main = new MainScreen() { Splash = splash };
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

        /// <summary>
        /// Start the splash screen.
        /// </summary>
        /// <returns>Splash screen process, or null.</returns>
        public static Process StartSplash()
        {
            var splash = new Process();
            splash.StartInfo.UseShellExecute = false;
            splash.StartInfo.FileName = "splash";

            // Get the version number and copyright from the assembly.
            var assemblyVersion = typeof(Program).Assembly.GetName().Version;
            var version = assemblyVersion.Major.ToString() + "." + assemblyVersion.Minor.ToString();
            var splashArguments = $"-pid {Process.GetCurrentProcess().Id} -version {version}";
            var assemblyAttributes = typeof(Program).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if (assemblyAttributes.Length > 0 && assemblyAttributes[0] is AssemblyCopyrightAttribute copyrightAttribute)
            {
                splashArguments += $" -copyright \"{copyrightAttribute.Copyright}\"";
            }

            splash.StartInfo.Arguments = splashArguments;
            try
            {
                splash.Start();
            }
            catch (Exception)
            {
                splash.Dispose();
                return null;
            }

            return splash;
        }
    }
}
