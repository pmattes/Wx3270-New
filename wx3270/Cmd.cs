// <copyright file="Cmd.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Class to connect to a local process.
    /// </summary>
    public class Cmd
    {
        /// <summary>
        /// Localization group for message box titles.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Cmd));

        /// <summary>
        /// Instance sequence number.
        /// </summary>
        private static int sequence = 1;

        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Back end.
        /// </summary>
        private readonly IBackEnd backend;

        /// <summary>
        /// Standard output buffer.
        /// </summary>
        private readonly byte[] stdoutBuffer = new byte[4096];

        /// <summary>
        /// Standard error output buffer.
        /// </summary>
        private readonly byte[] stderrBuffer = new byte[4096];

        /// <summary>
        /// Socket output buffer.
        /// </summary>
        private readonly byte[] socketBuffer = new byte[4096];

        /// <summary>
        /// Instance sequence number.
        /// </summary>
        private readonly int instanceSequence;

        /// <summary>
        /// The cmd.exe process.
        /// </summary>
        private Process cmdProcess;

        /// <summary>
        /// The TCP listener.
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// Socket to/from the emulator.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Network stream created from the socket.
        /// </summary>
        private NetworkStream socketStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cmd"/> class.
        /// </summary>
        /// <param name="backend">Back end.</param>
        public Cmd(IBackEnd backend)
        {
            this.backend = backend;
            this.instanceSequence = sequence++;
        }

        /// <summary>
        /// Data source type.
        /// </summary>
        private enum DataSource
        {
            /// <summary>
            /// Standard output.
            /// </summary>
            StandardOutput,

            /// <summary>
            /// Standard error output.
            /// </summary>
            StandardError,

            /// <summary>
            /// Socket to emulator.
            /// </summary>
            Socket,
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.SystemError, "System Error");
            I18n.LocalizeGlobal(Title.ConnectError, "Local Process Connect Error");
        }

        /// <summary>
        /// Connect to a local process.
        /// </summary>
        /// <param name="hostEntry">Host entry.</param>
        public void Connect(HostEntry hostEntry)
        {
            lock (this.sync)
            {
                if (this.cmdProcess != null)
                {
                    // Double connect.
                    return;
                }

                // Create a cmd.exe process.
                this.cmdProcess = new Process();
                this.cmdProcess.StartInfo.FileName = hostEntry.Command;
                this.cmdProcess.StartInfo.Arguments = hostEntry.CommandLineOptions;
                this.cmdProcess.StartInfo.UseShellExecute = false;
                this.cmdProcess.StartInfo.RedirectStandardInput = true;
                this.cmdProcess.StartInfo.RedirectStandardOutput = true;
                this.cmdProcess.StartInfo.RedirectStandardError = true;
                this.cmdProcess.StartInfo.CreateNoWindow = true;
                this.cmdProcess.Exited += this.CmdExited;
                try
                {
                    this.cmdProcess.Start();
                }
                catch (Exception e)
                {
                    ErrorBox.Show(e.Message, I18n.Get(Title.SystemError));
                    this.Cleanup();
                    return;
                }

                // Create a listening socket.
                this.listener = new TcpListener(IPAddress.Loopback, 0);
                this.listener.Start();
                var port = ((IPEndPoint)this.listener.LocalEndpoint).Port;

                // Tell the back end to connect to the port.
                this.backend.RunAction(new BackEndAction(B3270.Action.Connect, B3270.Prefix.NoTelnetHost + ":" + string.Format("{0}:{1}", IPAddress.Loopback.ToString(), port)), this.ConnectDone);

                // Wait for the emulator connect.
                this.listener.BeginAcceptSocket(new AsyncCallback(this.AcceptDone), null);
            }
        }

        /// <summary>
        /// Connect command is complete.
        /// </summary>
        /// <param name="cookie">Call context.</param>
        /// <param name="success">True if command succeeded.</param>
        /// <param name="result">Error text.</param>
        /// <param name="misc">Miscellaneous attributes.</param>
        private void ConnectDone(object cookie, bool success, string result, AttributeDict misc)
        {
            if (!success)
            {
                ErrorBox.Show(result, I18n.Get(Title.ConnectError));
                lock (this.sync)
                {
                    this.Cleanup();
                }
            }
        }

        /// <summary>
        /// Socket accept is complete.
        /// </summary>
        /// <param name="ar">Asynchronous result.</param>
        private void AcceptDone(IAsyncResult ar)
        {
            lock (this.sync)
            {
                // Get the connected socket.
                this.socket = this.listener.EndAcceptSocket(ar);
                this.socketStream = new NetworkStream(this.socket);

                // Stop listening.
                this.listener.Stop();
                this.listener = null;

                // Start doing I/O.
                this.cmdProcess.StandardOutput.BaseStream.BeginRead(
                    this.stdoutBuffer,
                    0,
                    this.stdoutBuffer.Length,
                    (rar) => this.ReadDone(rar, DataSource.StandardOutput),
                    null);
                this.cmdProcess.StandardError.BaseStream.BeginRead(
                    this.stderrBuffer,
                    0,
                    this.stderrBuffer.Length,
                    (rar) => this.ReadDone(rar, DataSource.StandardError),
                    null);
                this.socketStream.BeginRead(
                    this.socketBuffer,
                    0,
                    this.socketBuffer.Length,
                    (rar) => this.ReadDone(rar, DataSource.Socket),
                    null);
            }
        }

        /// <summary>
        /// Asynchronous read completion.
        /// </summary>
        /// <param name="ar">Asynchronous completion result.</param>
        /// <param name="dataSource">Data source.</param>
        private void ReadDone(IAsyncResult ar, DataSource dataSource)
        {
            if (ar.CompletedSynchronously)
            {
                this.LockedReadDone(ar, dataSource);
            }
            else
            {
                lock (this.sync)
                {
                    this.LockedReadDone(ar, dataSource);
                }
            }
        }

        /// <summary>
        /// Asynchronous read completion, with the lock held.
        /// </summary>
        /// <param name="ar">Asynchronous completion result.</param>
        /// <param name="dataSource">Data source.</param>
        private void LockedReadDone(IAsyncResult ar, DataSource dataSource)
        {
            Stream stream = null;
            byte[] buffer = null;
            switch (dataSource)
            {
                case DataSource.StandardOutput:
                    if (this.cmdProcess == null)
                    {
                        return;
                    }

                    stream = this.cmdProcess.StandardOutput.BaseStream;
                    buffer = this.stdoutBuffer;
                    break;
                case DataSource.StandardError:
                    if (this.cmdProcess == null)
                    {
                        return;
                    }

                    stream = this.cmdProcess.StandardError.BaseStream;
                    buffer = this.stderrBuffer;
                    break;
                case DataSource.Socket:
                    if (this.socketStream == null)
                    {
                        return;
                    }

                    stream = this.socketStream;
                    buffer = this.socketBuffer;
                    break;
            }

            // End the read and check for EOF.
            var nread = stream.EndRead(ar);
            if (nread == 0)
            {
                this.Cleanup();
                return;
            }

            // Write to the other side.
            switch (dataSource)
            {
                case DataSource.StandardOutput:
                case DataSource.StandardError:
                    this.socketStream.Write(buffer, 0, nread);
                    break;
                case DataSource.Socket:
                    this.cmdProcess.StandardInput.BaseStream.Write(buffer, 0, nread);
                    this.cmdProcess.StandardInput.BaseStream.Flush();
                    break;
            }

            // Start reading again.
            stream.BeginRead(buffer, 0, buffer.Length, (rar) => this.ReadDone(rar, dataSource), null);
        }

        /// <summary>
        /// The cmd.exe process has exited.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CmdExited(object sender, EventArgs e)
        {
            lock (this.sync)
            {
                this.Cleanup();
            }
        }

        /// <summary>
        /// Clean up the state.
        /// </summary>
        private void Cleanup()
        {
            if (this.cmdProcess != null)
            {
                this.cmdProcess.Dispose();
                this.cmdProcess = null;
            }

            if (this.socketStream != null)
            {
                this.socketStream.Dispose();
                this.socketStream = null;
            }

            if (this.socket != null)
            {
                this.socket.Dispose();
                this.socket = null;
            }
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Error starting cmd.exe.
            /// </summary>
            public static readonly string SystemError = I18n.Combine(TitleName, "systemError");

            /// <summary>
            /// Error connecting back.
            /// </summary>
            public static readonly string ConnectError = I18n.Combine(TitleName, "connectError");
        }
    }
}
