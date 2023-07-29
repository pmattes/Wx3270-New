// <copyright file="Splash.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Splash screen class.
    /// </summary>
    public class Splash
    {
        /// <summary>
        /// The splash process.
        /// </summary>
        private Process process;

        /// <summary>
        /// Start the splash screen.
        /// </summary>
        public void Start()
        {
            this.process = new Process();
            this.process.StartInfo.UseShellExecute = false;
            this.process.StartInfo.FileName = "splash";

            // Get the version number and copyright from the assembly.
            var assemblyVersion = typeof(Program).Assembly.GetName().Version;
            var version = assemblyVersion.Major.ToString() + "." + assemblyVersion.Minor.ToString();
            var splashArguments = $"-pid {Process.GetCurrentProcess().Id} -version {version}";
            var assemblyAttributes = typeof(Program).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if (assemblyAttributes.Length > 0 && assemblyAttributes[0] is AssemblyCopyrightAttribute copyrightAttribute)
            {
                splashArguments += $" -copyright \"{copyrightAttribute.Copyright}\"";
            }

            this.process.StartInfo.Arguments = splashArguments;
            try
            {
                this.process.Start();
            }
            catch (Exception)
            {
                this.process.Dispose();
                this.process = null;
            }
        }

        /// <summary>
        /// Stop the splash screen.
        /// </summary>
        public void Stop()
        {
            try
            {
                this.process?.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            this.process?.Dispose();
            this.process = null;
        }
    }
}
