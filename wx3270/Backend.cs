// <copyright file="BackEnd.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Forms;
    using System.Xml;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Emulator back end, using b3270.
    /// </summary>
    public class BackEnd : IBackEnd, IElement
    {
        /// <summary>
        /// Minimum compatible version.
        /// </summary>
        private const string MinVersion = "4.0";

        /// <summary>
        /// Localization group for message box titles.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(BackEnd));

        /// <summary>
        /// Localization group for message box messages.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(BackEnd));

        /// <summary>
        /// Conrtol for method invocation.
        /// </summary>
        private readonly Control control;

        /// <summary>
        /// The set of XML element start methods.
        /// </summary>
        private readonly Dictionary<string, Delegate> startTable = new Dictionary<string, Delegate>();

        /// <summary>
        /// The set of XML element end methods.
        /// </summary>
        private readonly Dictionary<string, Delegate> endTable = new Dictionary<string, Delegate>();

        /// <summary>
        /// The pass-through actions defined before Run was called.
        /// </summary>
        private readonly List<Tuple<string, string, string>> pendingPassthruDefinitions = new List<Tuple<string, string, string>>();

        /// <summary>
        /// Synchronization object for <see cref="writer"/>.
        /// </summary>
        private readonly object writerSync = new object();

        /// <summary>
        /// The start-up configuration.
        /// </summary>
        private readonly StartupConfig startupConfig;

        /// <summary>
        /// Synchronization object for the <see cref="BackEnd.Debug"/> method.
        /// </summary>
        private readonly object debugSync = new object();

        /// <summary>
        /// Completions to call.
        /// </summary>
        private readonly Dictionary<string, Completion> completions = new Dictionary<string, Completion>();

        /// <summary>
        /// Pass-through actions.
        /// </summary>
        private readonly Dictionary<string, Passthru> passthruDict = new Dictionary<string, Passthru>();

        /// <summary>
        /// Timer for gathering trace output.
        /// </summary>
        private readonly System.Timers.Timer traceTimer = new System.Timers.Timer
        {
            Interval = 50,
            AutoReset = false,
            Enabled = false,
        };

        /// <summary>
        /// The trace output queue.
        /// </summary>
        private readonly ConcurrentQueue<string> traceQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// True if the back end is running.
        /// </summary>
        private bool running;

        /// <summary>
        /// The XML writer to talk to b3270.
        /// </summary>
        private XmlWriter writer;

        /// <summary>
        /// Sequence number for RunActions tags.
        /// </summary>
        private int runSeq;

        /// <summary>
        /// The back-end emulator process.
        /// </summary>
        private Process b3270;

        /// <summary>
        /// The task that reads XML from b3270.
        /// </summary>
        private Task xmlReaderTask;

        /// <summary>
        /// The XML reader.
        /// </summary>
        private XmlReader xmlReader;

        /// <summary>
        /// Start-up profile path.
        /// </summary>
        private string startupProfilePath;

        /// <summary>
        /// Thread for monitoring standard error output.
        /// </summary>
        private Thread stderrThread;

        /// <summary>
        /// True if tracing is still possible.
        /// </summary>
        private bool traceEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackEnd"/> class.
        /// </summary>
        /// <param name="control">Control for method invocation.</param>
        /// <param name="startupConfig">Start-up configuration.</param>
        public BackEnd(Control control, StartupConfig startupConfig)
        {
            this.control = control;
            this.startupConfig = startupConfig;
            this.traceTimer.Elapsed += this.FlushDebug;
        }

        /// <inheritdoc />
        public event Action OnExit = () => { };

        /// <inheritdoc />
        public event Action OnReady = () => { };

        /// <summary>
        /// Gets or sets a value indicating whether to display debug messages to the console.
        /// </summary>
        public static bool DebugFlag { get; set; }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.Fatal, "Fatal Error");
            I18n.LocalizeGlobal(Title.BackEndError, "Back-End Error");

            I18n.LocalizeGlobal(Message.CannotStart, "Cannot start b3270 back-end");
            I18n.LocalizeGlobal(Message.BackEndExit, "Back-end process exited with status");
            I18n.LocalizeGlobal(Message.XmlException, "Caught exception while processing back-end message");
        }

        /// <summary>
        /// Return a number of spaces.
        /// </summary>
        /// <param name="n">Number of spaces.</param>
        /// <returns>String of spaces.</returns>
        public static string Spaces(int n)
        {
            return string.Format("{0," + n + "}", string.Empty);
        }

        /// <summary>
        /// Ignore the result of a back-end action.
        /// </summary>
        /// <returns>Completion delegate.</returns>
        public static BackEndCompletion Ignore()
        {
            return (cookie, success, result, misc) => { };
        }

        /// <inheritdoc />
        public void Debug(string format, params object[] args)
        {
            if (!DebugFlag)
            {
                return;
            }

            lock (this.debugSync)
            {
                var fg = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine(format, args);
                Console.ForegroundColor = fg;
            }
        }

        /// <inheritdoc />
        public void Run()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;

            // Write start-up options into a b3270 profile.
            this.startupProfilePath = GetTempProfile();
            using (var stream = new FileStream(this.startupProfilePath, FileMode.CreateNew))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.Model, this.startupConfig.ModelParameter));
                if (!string.IsNullOrWhiteSpace(this.startupConfig.OversizeParameter))
                {
                    writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.Oversize, this.startupConfig.OversizeParameter));
                }

                writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.TraceDir, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory).Replace(@"\", @"\\")));
                if (this.startupConfig.Trace)
                {
                    writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.Trace, B3270.Value.True));
                }

                if (!string.IsNullOrEmpty(this.startupConfig.ScriptPort))
                {
                    writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.ScriptPort, this.startupConfig.ScriptPort));
                    if (this.startupConfig.ScriptPortOnce)
                    {
                        writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.ScriptPortOnce, B3270.Value.True));
                    }
                }

                if (!string.IsNullOrEmpty(this.startupConfig.Httpd))
                {
                    writer.WriteLine(B3270.ResourceFormat.Value(B3270.ResourceName.Httpd, this.startupConfig.Httpd));
                }
            }

            // Start up a copy of b3270.
            this.b3270 = new Process();
            this.b3270.StartInfo.UseShellExecute = false;
            this.b3270.StartInfo.RedirectStandardInput = true;
            this.b3270.StartInfo.RedirectStandardOutput = true;
            this.b3270.StartInfo.RedirectStandardError = true;
            this.b3270.StartInfo.FileName = "b3270";

            this.b3270.StartInfo.CreateNoWindow = true;
            this.b3270.Exited += this.Exited;
            this.b3270.EnableRaisingEvents = true;
            this.b3270.StartInfo.Arguments = string.Format(
                "{0} {1} {2} \"{3}\"",
                B3270.CommandLineOption.Utf8,
                B3270.CommandLineOption.MinVersion,
                MinVersion,
                this.startupProfilePath);

            // Create a thread to read b3270's standard error, but don't start it right away.
            this.stderrThread = new Thread(this.StandardErrorReader)
            {
                IsBackground = true,
            };

            // Start the backend process.
            try
            {
                this.b3270.Start();
            }
            catch (Exception e)
            {
                ErrorBox.ShowCopy(
                    this.control,
                    I18n.Get(Message.CannotStart) + ": " + Environment.NewLine + e.Message,
                    I18n.Get(Title.Fatal));
                this.Exit(1);
            }

            // Set up XML processing methods.
            this.RegisterStart(B3270.Indication.Initialize, (name, attrs) => { });
            this.RegisterEnd(B3270.Indication.Initialize, (name) =>
            {
                Wx3270.Trace.Line(Wx3270.Trace.Type.BackEnd, "Back-end initialization complete");
                File.Delete(this.startupProfilePath);
                this.startupProfilePath = null;
                this.OnReady();
            });
            this.RegisterStart(B3270.Indication.RunResult, this.StartRunResult);
            this.RegisterStart(B3270.Indication.Passthru, this.StartPassthru);

            // Set up an XmlWriter to talk to b3270.
            // Ask for UTF8 (which is probably redundant) and indentation with an empty indent character,
            // which means newlines will be sent with each element. This makes the line numbers in error
            // messages from libexpat make more sense, should something go haywire.
            var writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Replace,
                Indent = true,
                IndentChars = string.Empty,
            };

            // You can't specify the encoding for a process' standard input when you create the Process
            // object (like you can with standard output), but you can get at the base stream and
            // create a new stream writer with the right encoding. Without this, the XmlWriter will
            // ignore the encoding you request, and use the system encoding (e.g., Windows-1252)
            // instead.
            var utf8Writer = new StreamWriter(this.b3270.StandardInput.BaseStream, Encoding.UTF8);
            this.writer = XmlWriter.Create(utf8Writer, writerSettings);

            this.writer.WriteStartDocument();
            this.writer.WriteStartElement(B3270.StartElement.B3270In);
            this.writer.Flush();

            // Set up an XmlReader to listen to b3270.
            var readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            readerSettings.Async = true;
            this.xmlReader = XmlReader.Create(this.b3270.StandardOutput, readerSettings);
            this.xmlReaderTask = this.ReadEmulatorStream(this.xmlReader);

            // Register pending passthru actions.
            foreach (var p in this.pendingPassthruDefinitions)
            {
                this.RegisterPassthruToEmulator(p.Item1, p.Item1, p.Item3);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            // No more tracing. There is probably more to do here.
            this.traceEnabled = false;

            // Push a Quit through the emulator.
            this.RunAction(new BackEndAction(B3270.Action.Quit), ErrorBox.Ignore());
        }

        /// <inheritdoc />
        public void RegisterStart(string name, StartDelegate d)
        {
            if (this.startTable.TryGetValue(name, out Delegate existing))
            {
                this.startTable[name] = (StartDelegate)existing + d;
            }
            else
            {
                this.startTable[name] = d;
            }
        }

        /// <inheritdoc />
        public void RegisterEnd(string name, EndDelegate d)
        {
            if (this.endTable.TryGetValue(name, out Delegate existing))
            {
                this.endTable[name] = (EndDelegate)existing + d;
            }
            else
            {
                this.endTable[name] = d;
            }
        }

        /// <inheritdoc />
        public void Register(IBackEndEvent e)
        {
            foreach (var def in e.Def)
            {
                if (def.StartHandler != null)
                {
                    this.RegisterStart(def.Element, def.StartHandler);
                }

                if (def.EndHandler != null)
                {
                    this.RegisterEnd(def.Element, def.EndHandler);
                }
            }
        }

        /// <inheritdoc />
        public void RunActions(string actions, string type, BackEndCompletion completion, object cookie = null, NestedPassthru nestedPassthru = null)
        {
            lock (this.writerSync)
            {
                string tag = $"async-{this.runSeq++}";
                var comp = new Tuple<string, BackEndCompletion>(tag, completion);
                this.completions[tag] = new Completion(completion, cookie, nestedPassthru);

                this.writer.WriteStartElement(B3270.Operation.Run);
                this.writer.WriteAttributeString("actions", actions);
                if (type != null)
                {
                    this.writer.WriteAttributeString("type", type);
                }

                this.writer.WriteAttributeString(B3270.Attribute.RTag, tag);
                this.writer.WriteEndElement();
                this.writer.Flush();
            }
        }

        /// <inheritdoc />
        public void RunActions(string actions, BackEndCompletion completion, object cookie = null, NestedPassthru nestedPassthru = null)
        {
            this.RunActions(actions, null, completion, cookie, nestedPassthru);
        }

        /// <inheritdoc />
        public void RunAction(BackEndAction action, BackEndCompletion completion, object cookie = null)
        {
            this.RunActions(action.Encoding, completion, cookie);
        }

        /// <inheritdoc />
        public void RunAction(BackEndAction action, string type, BackEndCompletion completion, object cookie = null)
        {
            this.RunActions(action.Encoding, type, completion, cookie);
        }

        /// <inheritdoc />
        public void RunActions(IEnumerable<BackEndAction> actions, BackEndCompletion completion, object cookie = null)
        {
            this.RunActions(string.Join(" ", actions.Select(a => a.Encoding)), completion, cookie);
        }

        /// <inheritdoc />
        public void RegisterPassthru(string commandName, Passthru action, string helpText = null, string helpParams = null)
        {
            this.passthruDict[commandName] = action;
            if (this.running)
            {
                this.RegisterPassthruToEmulator(commandName, helpText, helpParams);
            }
            else
            {
                this.pendingPassthruDefinitions.Add(new Tuple<string, string, string>(commandName, helpText, helpParams));
            }
        }

        /// <inheritdoc />
        public void PassthruComplete(bool success, string result, string tag)
        {
            if (success)
            {
                lock (this.writerSync)
                {
                    this.writer.WriteStartElement(B3270.Operation.Succeed);
                    this.writer.WriteAttributeString(B3270.Attribute.PTag, tag);
                    if (!string.IsNullOrEmpty(result))
                    {
                        this.writer.WriteAttributeString(B3270.Attribute.Text, result);
                    }

                    this.writer.WriteEndElement();
                    this.writer.Flush();
                }
            }
            else
            {
                lock (this.writerSync)
                {
                    this.writer.WriteStartElement(B3270.Operation.Fail);
                    this.writer.WriteAttributeString(B3270.Attribute.PTag, tag);
                    this.writer.WriteAttributeString(B3270.Attribute.Text, string.IsNullOrEmpty(result) ? "Failed" : result);
                    this.writer.WriteEndElement();
                    this.writer.Flush();
                }
            }
        }

        /// <inheritdoc />
        public void Trace(string text)
        {
            this.traceQueue.Enqueue(text);
            this.traceTimer.Start();
        }

        /// <inheritdoc />
        public void Exit(int returnCode)
        {
            try
            {
                this.OnExit();
            }
            catch (Exception)
            {
            }

            Environment.Exit(returnCode);
        }

        /// <summary>
        /// Get the name of a temporary profile file.
        /// </summary>
        /// <returns>Path of temporary profile.</returns>
        private static string GetTempProfile()
        {
            var tempDir = Path.GetTempPath();
            var i = 0;
            while (true)
            {
                var path = Path.Combine(tempDir, $"wx3270tmp{i}.b3270");
                if (!File.Exists(path))
                {
                    return path;
                }

                i++;
            }
        }

        /// <summary>
        /// Back-end standard error reader.
        /// </summary>
        private void StandardErrorReader()
        {
            while (true)
            {
                string r = null;
                try
                {
                    r = this.b3270.StandardError.ReadLine();
                }
                catch (IOException)
                {
                }

                if (string.IsNullOrWhiteSpace(r))
                {
                    return;
                }

                ErrorBox.ShowCopy(this.control, r, I18n.Get(Title.BackEndError), MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Register a pass-through action with the emulator.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="helpText">Help text.</param>
        /// <param name="helpParams">Help for parameters.</param>
        private void RegisterPassthruToEmulator(string commandName, string helpText, string helpParams)
        {
            lock (this.writerSync)
            {
                this.writer.WriteStartElement(B3270.Operation.Register);
                this.writer.WriteAttributeString("name", commandName);
                if (helpText != null)
                {
                    this.writer.WriteAttributeString("help-text", helpText);
                }

                if (helpParams != null)
                {
                    this.writer.WriteAttributeString("help-params", helpParams);
                }

                this.writer.WriteEndElement();
                this.writer.Flush();
            }
        }

        /// <summary>
        /// The back end process exited.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private void Exited(object sender, EventArgs eventArgs)
        {
            var errorCode = 0;

            // If the backend never really got started, clean up the start-up profile.
            if (this.startupProfilePath != null)
            {
                File.Delete(this.startupProfilePath);
                this.startupProfilePath = null;
            }

            // If the backend failed, pop up an error message.
            if (this.b3270.ExitCode != 0)
            {
                var errorText = this.b3270.StandardError.ReadToEnd();
                if (errorText != string.Empty)
                {
                    var errorLines = errorText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if (errorLines.Count() > 5)
                    {
                        errorText = string.Join(Environment.NewLine, errorText.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Take(5)) + " ...";
                    }

                    errorText = ":" + Environment.NewLine + errorText;
                }

                var hex = (this.b3270.ExitCode & 0x80000000) != 0 ? $" (0x{this.b3270.ExitCode:X})" : string.Empty;
                ErrorBox.ShowCopy(
                    this.control,
                    I18n.Get(Message.BackEndExit) + " " + this.b3270.ExitCode + hex + errorText,
                    I18n.Get(Title.BackEndError));
                errorCode = 1;
            }

            // Do miscellaneous clean-up and quit.
            this.Exit(errorCode);
        }

        /// <summary>
        /// Read the emulator XML stream.
        /// </summary>
        /// <param name="reader">XML reader (from emulator).</param>
        /// <returns>Asynchronous task.</returns>
        private async Task ReadEmulatorStream(XmlReader reader)
        {
            var any = false;
            try
            {
                while (await reader.ReadAsync())
                {
                    // We got something. So now it's safe to monitor stderr asynchronously.
                    if (!any)
                    {
                        any = true;
                        this.stderrThread.Start();
                    }

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.XmlDeclaration:
                            this.Debug("Declaration {0}", reader.Value);
                            this.Debug("Encoding: {0}", reader);
                            break;
                        case XmlNodeType.Element:
                            this.Debug(
                                "{0}<{1}> AttributeCount {2} Depth {3}",
                                Spaces(reader.Depth),
                                reader.Name,
                                reader.AttributeCount,
                                reader.Depth);
                            var attributes = new AttributeDict();
                            if (reader.HasAttributes)
                            {
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    this.Debug(@"{0}{1}=""{2}""", Spaces(reader.Depth), reader.Name, reader.Value);
                                    attributes[reader.Name] = reader.Value;
                                }
                                while (reader.MoveToNextAttribute());

                                // Move the reader back to the element node.
                                reader.MoveToElement();
                            }

                            this.ProcessStartElement(reader.Name, attributes);

                            break;
                        case XmlNodeType.EndElement:
                            this.Debug("{0}</{1}> Depth {2}", Spaces(reader.Depth), reader.Name, reader.Depth);
                            if (reader.Depth == 0)
                            {
                                // Enclosing document element complete. We're done.
                                return;
                            }

                            this.ProcessEndElement(reader.Name);

                            break;
                        default:
                            this.Debug("{0}", reader.NodeType);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // Fail now, because this is run asynchronously without ever being waited for,
                // and otherwise the app would just mysteriously stall.
                Console.WriteLine("BackEnd caught exception: " + e.ToString());
                ErrorBox.ShowCopy(
                    this.control,
                    I18n.Get(Message.XmlException) + ":" + Environment.NewLine + e,
                    I18n.Get(Title.Fatal));

                // Do miscellaneous clean-up and exit.
                this.Exit(1);
            }
        }

        /// <summary>
        /// Process a run-result indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Element attributes.</param>
        private void StartRunResult(string name, AttributeDict attributes)
        {
            // Get the result text.
            if (!attributes.TryGetValue(B3270.Attribute.Text, out string text))
            {
                text = string.Empty;
            }

            // Get the success/failure state.
            bool success = false;
            if (attributes.TryGetValue(B3270.Attribute.Success, out string successString) && successString.Equals(B3270.Value.True))
            {
                success = true;
            }

            // Get the tag.
            if (!attributes.TryGetValue(B3270.Attribute.RTag, out string tag))
            {
                this.Debug("run-result has no tag", tag);
                return;
            }

            if (this.completions.TryGetValue(tag, out Completion completion))
            {
                completion.BackEndCompletion(completion.Cookie, success, text, attributes);
                this.completions.Remove(tag);
            }
        }

        /// <summary>
        /// Process a pass-through indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Element attributes.</param>
        private void StartPassthru(string name, AttributeDict attributes)
        {
            // Tell any interested caller about a nested action.
            if (attributes.TryGetValue(B3270.Attribute.ParentRTag, out string parentTag)
                && this.completions.TryGetValue(parentTag, out Completion completion)
                && completion.NestedPassthru != null)
            {
                completion.NestedPassthru(completion.Item2, attributes[B3270.Attribute.Action]);
            }

            // Run the action.
            if (this.passthruDict.TryGetValue(attributes[B3270.Attribute.Action], out Passthru passthru))
            {
                // Gather the argument list.
                var args = new List<string>();
                var i = 1;
                while (attributes.TryGetValue(B3270.Attribute.Arg + i, out string arg))
                {
                    args.Add(arg);
                    i++;
                }

                // Run the action.
                switch (passthru(name, args, out string result, attributes[B3270.Attribute.PTag]))
                {
                    case PassthruResult.Success:
                        this.PassthruComplete(true, result, attributes[B3270.Attribute.PTag]);
                        break;
                    case PassthruResult.Failure:
                        this.PassthruComplete(false, result, attributes[B3270.Attribute.PTag]);
                        break;
                    case PassthruResult.Pending:
                        break;
                }
            }
            else
            {
                // No such action.
                this.PassthruComplete(false, "Unknown passthru action", attributes[B3270.Attribute.PTag]);
            }
        }

        /// <summary>
        /// Process a received element.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Attributes dictionary.</param>
        private void ProcessStartElement(string name, AttributeDict attributes)
        {
            StartDelegate start;
            if (this.startTable.TryGetValue(name, out Delegate d))
            {
                start = (StartDelegate)d;
                start(name, attributes);
            }
        }

        /// <summary>
        /// Process the end of an XML element.
        /// </summary>
        /// <param name="name">Element name.</param>
        private void ProcessEndElement(string name)
        {
            EndDelegate end;
            if (this.endTable.TryGetValue(name, out Delegate d))
            {
                end = (EndDelegate)d;
                end(name);
            }
        }

        /// <summary>
        /// Flush the debug queue. Timeout for the trace timer.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        private void FlushDebug(object sender, ElapsedEventArgs args)
        {
            if (this.traceEnabled)
            {
                if (this.writer != null)
                {
                    lock (this.writerSync)
                    {
                        while (this.traceQueue.TryDequeue(out string text))
                        {
                            this.writer.WriteComment(text);
                        }

                        this.writer.Flush();
                    }
                }
                else
                {
                    // Write not created yet. Check again later.
                    this.traceTimer.Start();
                }
            }
        }

        /// <summary>
        /// Action completion state.
        /// </summary>
        private class Completion : Tuple<BackEndCompletion, object, NestedPassthru>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Completion"/> class.
            /// </summary>
            /// <param name="completion">Completion delegate.</param>
            /// <param name="cookie">Opaque cookie.</param>
            /// <param name="nestedPassthru">Nested pass-through delegate.</param>
            public Completion(BackEndCompletion completion, object cookie, NestedPassthru nestedPassthru)
                : base(completion, cookie, nestedPassthru)
            {
            }

            /// <summary>
            /// Gets the completion delegate.
            /// </summary>
            public BackEndCompletion BackEndCompletion
            {
                get { return this.Item1; }
            }

            /// <summary>
            /// Gets the opaque cookie.
            /// </summary>
            public object Cookie
            {
                get { return this.Item2; }
            }

            /// <summary>
            /// Gets the nested pass-through delegate.
            /// </summary>
            public NestedPassthru NestedPassthru
            {
                get { return this.Item3; }
            }
        }

        /// <summary>
        /// Localized message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Fatal error.
            /// </summary>
            public static readonly string Fatal = I18n.Combine(TitleName, "fatal");

            /// <summary>
            /// Back-end error (standard error output).
            /// </summary>
            public static readonly string BackEndError = I18n.Combine(TitleName, "backEndError");
        }

        /// <summary>
        /// Localized message box messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Cannot start back-end.
            /// </summary>
            public static readonly string CannotStart = I18n.Combine(MessageName, "cannotStart");

            /// <summary>
            /// Back end exit.
            /// </summary>
            public static readonly string BackEndExit = I18n.Combine(MessageName, "backEndExit");

            /// <summary>
            /// XML processing exception.
            /// </summary>
            public static readonly string XmlException = I18n.Combine(MessageName, "xmlException");
        }
    }
}
