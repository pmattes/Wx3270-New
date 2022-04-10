// <copyright file="TraceWindow.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// The trace monitor window.
    /// </summary>
    public class TraceWindow : IDisposable
    {
        /// <summary>
        /// Exit code for Control-C.
        /// </summary>
        private const uint CtrlCExit = 0xC000013A;

        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// The back end process.
        /// </summary>
        private readonly IBackEnd backEnd;

        /// <summary>
        /// Disposal complete flag.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The current instance of the helper process.
        /// </summary>
        private Process helperProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceWindow"/> class.
        /// </summary>
        /// <param name="fileName">File to monitor.</param>
        /// <param name="backEnd">Back end.</param>
        public TraceWindow(string fileName, IBackEnd backEnd)
        {
            this.backEnd = backEnd;
            try
            {
                this.helperProcess = Process.Start("TraceHelper.Exe", $"\"{fileName}\"");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Trace Helper Error");
                return;
            }

            this.helperProcess.Exited += this.TraceHelperExited;
            this.helperProcess.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Public Dispose method.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal Dispose method.
        /// </summary>
        /// <param name="disposing">True if called from the public <see cref="TraceWindow.Dispose"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.disposed = true;

                lock (this.sync)
                {
                    if (this.helperProcess != null)
                    {
                        this.helperProcess.Exited -= this.TraceHelperExited;
                        try
                        {
                            this.helperProcess.Kill();
                        }
                        catch (Exception e)
                        {
                            if (!(e is InvalidOperationException || e is Win32Exception))
                            {
                                throw;
                            }
                        }

                        this.helperProcess.Dispose();
                        this.helperProcess = null;
                    }
                }
            }
        }

        /// <summary>
        /// The trace helper process exited.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TraceHelperExited(object sender, EventArgs e)
        {
            var exitCode = this.helperProcess.ExitCode;
            lock (this.sync)
            {
                this.helperProcess.Dispose();
                this.helperProcess = null;
            }

            if ((uint)exitCode == CtrlCExit)
            {
                // The trace window was closed by ^C or the window close button. Stop tracing.
                this.backEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.Trace, B3270.Value.False), BackEnd.Ignore());
            }
        }
    }
}
