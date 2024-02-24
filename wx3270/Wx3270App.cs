// <copyright file="Wx3270App.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Security.RightsManagement;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using I18nBase;
    using Microsoft.Win32;
    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for synthetic modifier changes.
    /// </summary>
    /// <param name="mod">New synthetic modifiers.</param>
    /// <param name="mask">Modifier mask.</param>
    public delegate void SyntheticModChange(KeyboardModifier mod, KeyboardModifier mask);

    /// <summary>
    /// The main control class.
    /// </summary>
    public class Wx3270App : IUpdate, IConnectionState
    {
        /// <summary>
        /// Command-line options.
        /// </summary>
        private static readonly (string option, string args, string explanation)[] CommandLineOptions = new[]
        {
            (Constants.Option.Allow, "operation[,operation...]", "Allow the specified restricted operations"),
            (Constants.Option.Connection, "connection-name", "Start a particular connection"),
            (Constants.Option.Console, string.Empty, "Attach a console at start-up (for debugging)"),
            (Constants.Option.Culture, "culture-name", "Override the system default culture for messages"),
            (Constants.Option.Detached, string.Empty, "Do not synchronize profile changes in read-only mode"),
            (Constants.Option.DumpLocalization, "file-name", "Dump the en-US localization database to a file"),
            (Constants.Option.Edit, string.Empty, "Open the profile in edit mode (do not auto-connect to a host)|Requires the " + Constants.Option.Profile + " option"),
            (Constants.Option.FullScreen, string.Empty, "Enter full-screen mode at start-up"),
            (Constants.Option.Help1, string.Empty, "Display command-line help"),
            (Constants.Option.Httpd, "[address:]port", "Start an HTTP server"),
            (Constants.Option.Location, "x,y", "Specify an initial location for the window"),
            (Constants.Option.Maximize, string.Empty, "Create the window maximized"),
            (Constants.Option.Model, "model-number", "Override the default 3270 model number"),
            (Constants.Option.NoButtons, string.Empty, "Do not display the menu bar"),
            (Constants.Option.NoBorder, string.Empty, "Create the widow without a border"),
            (Constants.Option.NoProfile, string.Empty, "Operate without a profile (use system default settings)"),
            (Constants.Option.NoScrollBar, string.Empty, "Do not display the scroll bar"),
            (Constants.Option.NoSplash, string.Empty, "Do not display the splash screen"),
            (Constants.Option.Oversize, "columnsxrows", "Override the default screen dimensions"),
            (Constants.Option.Profile, "profile-name", "Use the specified profile instead of Base"),
            (Constants.Option.ReadOnly, string.Empty, "Open the profile in read-only mode (do not save changed settings)"),
            (Constants.Option.ReadWrite, string.Empty, "Open the profile in read/write mode and warn if it can't be opened"),
            (Constants.Option.Restrict, "operation[,operation...]", "Disable the specified restricted operations"),
            (Constants.Option.ScriptPort, "[address:]port", "Start an s3270 scripting server"),
            (Constants.Option.ScriptPortOnce, string.Empty, "Exit wx3270 as soon as the first s3270 scripting session ends"),
            (Constants.Option.Topmost, string.Empty, "Make wx3270 the topmost window"),
            (Constants.Option.Trace, string.Empty, "Turn on back-end tracing and all types of user interface tracing"),
            (Constants.Option.UiTrace, "type[,type]", "Turn on back-end tracing and the specified types of user interface tracing"),
            (Constants.Option.V, "[file-name]", "Display a copyright message and exit|Optionally write message to file"),
        };

        /// <summary>
        /// Title group name for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Wx3270App));

        /// <summary>
        /// The main form (which runs the UI thread).
        /// </summary>
        private readonly Control control;

        /// <summary>
        /// The screen update interface.
        /// </summary>
        private readonly IUpdate update;

        /// <summary>
        /// The terminal bell.
        /// </summary>
        private Bell bell;

        /// <summary>
        /// Screen state.
        /// </summary>
        private Screen screen;

        /// <summary>
        /// OIA state.
        /// </summary>
        private Oia oia;

        /// <summary>
        /// Backing field for <see cref="AplMode"/>.
        /// </summary>
        private bool aplMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wx3270App"/> class.
        /// </summary>
        /// <param name="control">Control to invoke UI activity on.</param>
        /// <param name="update">Screen update interface.</param>
        /// <param name="splash">Splash screen.</param>
        public Wx3270App(Control control, IUpdate update, Splash splash)
        {
            this.control = control;
            this.update = update;
            this.Splash = splash;
            this.IsWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        /// <summary>
        /// Synthetic modifier change event.
        /// </summary>
        public event SyntheticModChange SyntheticModChange = (mod, mask) => { };

        /// <summary>
        /// Chord reset event.
        /// </summary>
        public event Action ChordResetEvent = () => { };

        /// <summary>
        /// Tracker for restrict and allow options.
        /// </summary>
        [Flags]
        private enum RestrictAllow
        {
            /// <summary>
            /// No option processed.
            /// </summary>
            Neither = 0,

            /// <summary>
            /// Restrict option processed.
            /// </summary>
            Restrict = 0x01,

            /// <summary>
            /// Allow option processed.
            /// </summary>
            Allow = 0x02,

            /// <summary>
            /// Both options processed.
            /// </summary>
            Both = Restrict | Allow,
        }

        /// <summary>
        /// Gets a value indicating whether we are running on Windows.
        /// </summary>
        public bool IsWindows { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a console has been attached.
        /// </summary>
        public bool ConsoleAttached { get; private set; }

        /// <summary>
        /// Gets the wx3270 prompt.
        /// </summary>
        public Prompt Prompt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether editing mode is set.
        /// </summary>
        public bool EditMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to pop up a warning if in read-only mode.
        /// </summary>
        public bool ReadWriteMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to open the profile in read-only mode unconditionally.
        /// </summary>
        public bool ReadOnlyMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to read in any profile at all.
        /// </summary>
        public bool NoProfileMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to watch the folder where the profile is.
        /// </summary>
        public bool NoWatchMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to maximize the window at start-up.
        /// </summary>
        public bool Maximize { get; private set; }

        /// <summary>
        /// Gets the 3270 model.
        /// </summary>
        public ModelName Model { get; private set; }

        /// <summary>
        /// Gets the oversize dimensions.
        /// </summary>
        public Oversize Oversize { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to go to full screen at start-up.
        /// </summary>
        public bool FullScreen { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to detach from profile updates in read-only mode.
        /// </summary>
        public bool Detached { get; set; }

        /// <summary>
        /// Gets the command-line host connection.
        /// </summary>
        public string Connection { get; private set; }

        /// <summary>
        /// Gets the security restrictions.
        /// </summary>
        public Restrictions Restrictions { get; private set; }

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        public IProfileManager ProfileManager { get; private set; }

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        public IBackEnd BackEnd { get; private set; }

        /// <summary>
        /// Gets the sound manager.
        /// </summary>
        public ISound Sound { get; private set; }

        /// <summary>
        /// Gets the key event handler.
        /// </summary>
        public IKeyHandler KeyHandler { get; private set; }

        /// <summary>
        /// Gets the selection manager.
        /// </summary>
        public ISelectionManager SelectionManager { get; private set; }

        /// <summary>
        /// Gets the hello handler.
        /// </summary>
        public IHello Hello { get; private set; }

        /// <summary>
        /// Gets the pop-up handler.
        /// </summary>
        public IPopup Popup { get; private set; }

        /// <summary>
        /// Gets the TLS hello handler.
        /// </summary>
        public ITlsHello TlsHello { get; private set; }

        /// <summary>
        /// Gets the statistics handler.
        /// </summary>
        public IStats Stats { get; private set; }

        /// <summary>
        /// Gets the current profile.
        /// </summary>
        public Profile Current => this.ProfileManager.Current;

        /// <summary>
        /// Gets the connect attempt.
        /// </summary>
        public IConnectAttempt ConnectAttempt { get; private set; }

        /// <summary>
        /// Gets the window title.
        /// </summary>
        public IWindowTitle WindowTitle { get; private set; }

        /// <summary>
        /// Gets the initial window location.
        /// </summary>
        public Point? Location { get; private set; }

        /// <summary>
        /// Gets a snapshot of the screen image.
        /// </summary>
        public ScreenImage ScreenImage => this.screen.ScreenImage;

        /// <summary>
        /// Gets the current OIA state.
        /// </summary>
        public IOiaState OiaState => this.oia.OiaState;

        /// <summary>
        /// Gets the current connection state.
        /// </summary>
        public ConnectionState ConnectionState => this.oia.ConnectionState;

        /// <summary>
        /// Gets a value indicating whether the current connection was initiated by the UI.
        /// </summary>
        public bool ConnectionIsUi => this.oia.ConnectionIsUi;

        /// <summary>
        /// Gets the IP address of the current connected-to host.
        /// </summary>
        public string CurrentHostIp => this.oia.CurrentHostIp;

        /// <summary>
        /// Gets the profile tracker.
        /// </summary>
        public IProfileTracker ProfileTracker { get; private set; }

        /// <summary>
        /// Gets the command-line host name.
        /// </summary>
        public B3270HostSpec CommandLineB3270HostSpec { get; private set; }

        /// <summary>
        ///  Gets the command-line port.
        /// </summary>
        public string CommandLinePort { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the listener configuration is locked.
        /// </summary>
        public Dictionary<string, bool> ListenLock { get; private set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets the setting change handler.
        /// </summary>
        public ISettingChange SettingChange { get; private set; }

        /// <summary>
        /// Gets the terminal name handler.
        /// </summary>
        public ITerminalName TerminalName { get; private set; }

        /// <summary>
        /// Gets the JSON localization dump file.
        /// </summary>
        public string DumpLocalization { get; private set; }

        /// <summary>
        /// Gets or sets the chord name.
        /// </summary>
        public string ChordName { get; set; }

        /// <summary>
        /// Gets the code page database.
        /// </summary>
        public CodePageDb CodePageDb { get; private set; }

        /// <summary>
        /// Gets the models database.
        /// </summary>
        public ModelsDb ModelsDb { get; private set; }

        /// <summary>
        /// Gets the proxies database.
        /// </summary>
        public ProxiesDb ProxiesDb { get; private set; }

        /// <summary>
        /// Gets the host prefixes.
        /// </summary>
        public IHostPrefix HostPrefix { get; private set; }

        /// <summary>
        /// Gets the known B3270 settings.
        /// </summary>
        public HashSet<string> KnownSettings { get; private set; } = new KnownSettings().Settings;

        /// <summary>
        /// Gets or sets a value indicating whether APL mode is set.
        /// </summary>
        public bool AplMode
        {
            get
            {
                return this.aplMode;
            }

            set
            {
                this.aplMode = value;
                this.SyntheticModChange(value ? KeyboardModifier.Apl : KeyboardModifier.None, KeyboardModifier.Apl);
            }
        }

        /// <summary>
        /// Gets the synthetic modifiers (APL mode, 3270 mode, NVT mode).
        /// </summary>
        public KeyboardModifier SyntheticModifiers => (this.AplMode ? KeyboardModifier.Apl : 0) |
            ModifierModeFromConnectionState(this.ConnectionState);

        /// <summary>
        /// Gets a value indicating whether this window should be topmost.
        /// </summary>
        public bool Topmost { get; private set; }

        /// <summary>
        /// Gets the screen select state.
        /// </summary>
        public SelectState SelectState => this.screen.ScreenImage.SelectState;

        /// <summary>
        /// Gets the local process interface.
        /// </summary>
        public Cmd Cmd { get; private set; }

        /// <summary>
        /// Gets the macro recorder.
        /// </summary>
        public MacroRecorder MacroRecorder { get; } = new MacroRecorder();

        /// <summary>
        /// Gets the splash screen.
        /// </summary>
        public Splash Splash { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the window should omit the border.
        /// </summary>
        public bool NoBorder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scroll bar should be removed.
        /// </summary>
        public bool NoScrollBar { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the button bar should be removed.
        /// </summary>
        public bool NoButtons { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the splash screen should be suppressed.
        /// </summary>
        public bool NoSplash { get; private set; }

        /// <summary>
        /// Gets or sets the pr3287 trace options.
        /// </summary>
        public string Pr3287TraceOptions { get; set; }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.SystemError, RawText.SystemError);

            I18n.LocalizeGlobal(Message.Failed, RawText.Failed);
            I18n.LocalizeGlobal(Message.ConsoleHeader, RawText.ConsoleHeader);
            I18n.LocalizeGlobal(Message.ConsoleCautionPrefix, RawText.ConsoleCautionPrefix);
            I18n.LocalizeGlobal(Message.ConsoleCaution, RawText.ConsoleCaution);
        }

        /// <summary>
        /// Gets the command-line help string.
        /// </summary>
        /// <returns>Command-line help text.</returns>
        public static string GetCommandLineOptions()
        {
            var ret = @"Usage:
    wx3270 [options] [hostname[:port]]
    wx3270 [options] [profilename.wx3270]
Hostname can use full wc3270 syntax
Options:
";
            const string Indent = "     ";
            foreach (var option in CommandLineOptions)
            {
                ret += " "
                    + option.option
                    + (string.IsNullOrEmpty(option.args) ? string.Empty : " ") + option.args
                    + Environment.NewLine
                    + Indent + option.explanation.Replace("|", Environment.NewLine + Indent) + Environment.NewLine;
            }

            return ret;
        }

        /// <summary>
        /// Map a connection state onto a modifier mode.
        /// </summary>
        /// <param name="state">Connection state.</param>
        /// <returns>Map mode.</returns>
        public static KeyboardModifier ModifierModeFromConnectionState(ConnectionState state)
        {
            switch (state)
            {
                case ConnectionState.ConnectedNvt:
                case ConnectionState.ConnectedNvtCharmode:
                case ConnectionState.ConnectedEnvt:
                    return KeyboardModifier.ModeNvt;
                case ConnectionState.Connected3270:
                case ConnectionState.ConnectedEsscp:
                case ConnectionState.ConnectedTn3270e:
                    return KeyboardModifier.Mode3270;
                default:
                    return KeyboardModifier.None;
            }
        }

        /// <summary>
        /// Format a help tag.
        /// </summary>
        /// <param name="tag">Tag to format.</param>
        /// <returns>Formatted tag.</returns>
        public static string FormatHelpTag(string tag)
        {
            return tag.Substring(0, 1).ToUpper() + string.Join(string.Empty, tag.Skip(1).Select((d) =>
            {
                var s = d.ToString();
                return char.IsUpper(d) ? " " + s : s;
            })).Replace(" Tab", string.Empty);
        }

        /// <summary>
        /// Pop up a help window.
        /// </summary>
        /// <param name="path">Relative path of help file.</param>
        public static void GetHelp(string path)
        {
            var version = typeof(Wx3270App).Assembly.GetName().Version;
            var fullPath = new[]
            {
                "http://x3270.bgp.nu/wx3270-help",
                version.Major + "." + version.Minor,
                I18nBase.EffectiveCulture,
                path,
            };
            Process.Start(string.Join("/", fullPath));
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public void Init(string[] args)
        {
            var attachConsole = false;
            string profileName = null;
            var startupConfig = new StartupConfig();
            this.ListenLock[B3270.Setting.ScriptPort] = false;
            this.ListenLock[B3270.Setting.Httpd] = false;
            string culture = null;
            var restrict = Restrictions.None;
            var allow = Restrictions.None;
            var restrictAllow = RestrictAllow.Neither;

            var s = new Stopwatch();
            s.Start();
            Trace.Line(Trace.Type.Window, "Wx3270App Init start");

            // Parse command line arguments.
            int? lastOpt = null;
            var i = 0;
            try
            {
                for (i = 0; i < args.Length; i++)
                {
                    if (lastOpt.HasValue)
                    {
                        break;
                    }

                    switch (args[i].ToLowerInvariant())
                    {
                        case Constants.Option.Allow:
                            this.ParseAllowRestrict(Constants.Option.Allow, args[++i], ref allow);
                            restrictAllow |= RestrictAllow.Allow;
                            break;
                        case Constants.Option.Connection:
                        case Constants.Option.Host:
                            this.Connection = args[++i];
                            break;
                        case Constants.Option.Console:
                            attachConsole = true;
                            break;
                        case Constants.Option.Culture:
                            culture = args[++i];
                            break;
                        case Constants.Option.Detached:
                            this.Detached = true;
                            break;
                        case Constants.Option.DumpLocalization:
                            this.DumpLocalization = args[++i];
                            break;
                        case Constants.Option.Edit:
                            this.EditMode = true;
                            this.ReadWriteMode = true;
                            break;
                        case Constants.Option.FullScreen:
                            this.FullScreen = true;
                            break;
                        case Constants.Option.Help1:
                        case Constants.Option.Help2:
                        case Constants.Option.Help3:
                            this.Splash.Stop();
                            ErrorBox.Show(
                                GetCommandLineOptions(),
                                "wx3270 " + Profile.VersionClass.FullVersion,
                                MessageBoxIcon.Information);
                            Environment.Exit(0);
                            break;
                        case Constants.Option.Httpd:
                            startupConfig.Httpd = args[++i];
                            this.ListenLock[B3270.Setting.Httpd] = true;
                            break;
                        case Constants.Option.Location:
                            var r = new Regex(@"^(?<x>-?\d+),(?<y>-?\d+)$");
                            var m = r.Match(args[++i]);
                            if (!m.Success)
                            {
                                this.Usage($"Invalid location '{args[i]}' -- must be X,Y");
                            }

                            try
                            {
                                this.Location = new Point(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value));
                            }
                            catch (Exception e)
                            {
                                this.Usage($"Invalid location '{args[i]}' -- {e.Message}");
                            }

                            break;
                        case Constants.Option.Maximize:
                            this.Maximize = true;
                            break;
                        case Constants.Option.Model:
                            if (!ModelName.TryParse(args[++i], out ModelName model))
                            {
                                this.Usage($"Invalid model '{args[i]}'");
                            }

                            this.Model = model;
                            break;
                        case Constants.Option.NoBorder:
                            this.NoBorder = true;
                            break;
                        case Constants.Option.NoButtons:
                            this.NoButtons = true;
                            break;
                        case Constants.Option.NoProfile:
                            this.NoProfileMode = true;
                            break;
                        case Constants.Option.NoScrollBar:
                            this.NoScrollBar = true;
                            break;
                        case Constants.Option.NoSplash:
                            this.NoSplash = true;
                            break;
                        case Constants.Option.NoWatch:
                            this.NoWatchMode = true;
                            break;
                        case Constants.Option.Oversize:
                            var oversizeString = args[++i];
                            if (!string.IsNullOrEmpty(oversizeString))
                            {
                                if (!Oversize.TryParse(oversizeString, out Oversize oversize))
                                {
                                    this.Usage($"Invalid oversize '{oversizeString}'");
                                }

                                this.Oversize = oversize;
                            }
                            else
                            {
                                this.Oversize = new Oversize { Columns = 0, Rows = 0 };
                            }

                            break;
                        case Constants.Option.Profile:
                            profileName = args[++i];
                            break;
                        case Constants.Option.ReadOnly:
                        case Constants.Option.Ro:
                            this.ReadOnlyMode = true;
                            break;
                        case Constants.Option.ReadWrite:
                            this.ReadWriteMode = true;
                            break;
                        case Constants.Option.ScriptPort:
                            startupConfig.ScriptPort = args[++i];
                            this.ListenLock[B3270.Setting.ScriptPort] = true;
                            break;
                        case Constants.Option.ScriptPortOnce:
                            startupConfig.ScriptPortOnce = true;
                            break;
                        case Constants.Option.Restrict:
                            this.ParseAllowRestrict(Constants.Option.Restrict, args[++i], ref restrict);
                            restrictAllow |= RestrictAllow.Restrict;
                            break;
                        case Constants.Option.Topmost:
                            this.Topmost = true;
                            break;
                        case Constants.Option.Trace:
                            startupConfig.Trace = true;
                            Trace.Flags = Trace.Type.All;
                            break;
                        case Constants.Option.UiTrace:
                            if (!Enum.TryParse(args[++i], true, out Trace.Type traceFlags))
                            {
                                this.Usage(
                                    $"Unknown trace type '{args[i]}'" + Environment.NewLine +
                                    "Types are: " + string.Join(", ", Enum.GetValues(typeof(Trace.Type)).OfType<Trace.Type>().Select(m => m.ToString())));
                            }

                            Trace.Flags = traceFlags;
                            if (Trace.Flags != Trace.Type.None)
                            {
                                startupConfig.Trace = true;
                            }

                            break;
                        case Constants.Option.Utf8:
                            break;
                        case Constants.Option.V:
                            this.Splash.Stop();
                            ErrorBox.Show(
                                "wx3270 " + Profile.VersionClass.FullVersion + Environment.NewLine + Constants.Copyright,
                                "wx3270 " + Profile.VersionClass.FullVersion,
                                MessageBoxIcon.Information);
                            Environment.Exit(0);
                            break;
                        case Constants.Option.Vfile:
                            this.Splash.Stop();
                            File.WriteAllText(args[++i], "wx3270 " + Profile.VersionClass.FullVersion, Encoding.UTF8);
                            Environment.Exit(0);
                            break;
                        default:
                            if (args[i].StartsWith("-"))
                            {
                                this.Usage($"Unknown argument '{args[i]}'");
                            }

                            lastOpt = i;
                            break;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                this.Usage($"Missing parameter value for {args[i - 1]}");
            }

            if (lastOpt.HasValue)
            {
                var positionalArgs = args.Skip(lastOpt.Value).ToList();
                var dashed = positionalArgs.Where(a => a.StartsWith("-"));
                if (dashed.Any())
                {
                    this.Usage($"Unknown or misformatted argument(s): {string.Join(" ", dashed)}");
                }

                if (positionalArgs.Count > 2)
                {
                    this.Usage("Extra arguments");
                }

                if (this.Connection != null)
                {
                    this.Usage("Cannot specify " + Constants.Option.Connection + " and a positional host name");
                }

                if (positionalArgs[0].EndsWith(Wx3270.ProfileManager.Suffix, StringComparison.OrdinalIgnoreCase))
                {
                    // They specified a profile name as a host, like wc3270.
                    if (positionalArgs.Count > 1)
                    {
                        this.Usage("Cannot specify a profile name and a positional port");
                    }

                    if (profileName != null)
                    {
                        this.Usage("Cannot specify " + Constants.Option.Profile + " and a positional profile name");
                    }

                    profileName = positionalArgs[0];
                }
                else
                {
                    // They specifed a b3270-style hostname and possibly a port.
                    if (!B3270HostSpec.TryParse(positionalArgs[0], out B3270HostSpec hostSpec))
                    {
                        this.Usage("Invalid host name");
                    }

                    if (positionalArgs.Count > 1)
                    {
                        if (hostSpec.Port != null)
                        {
                            this.Usage("Port specified twice");
                        }

                        hostSpec.Port = positionalArgs[1];
                    }

                    this.CommandLineB3270HostSpec = hostSpec;
                }
            }

            if (this.EditMode && profileName == null)
            {
                this.Usage("Must specify " + Constants.Option.Profile + " with " + Constants.Option.Edit);
            }

            if (!string.IsNullOrWhiteSpace(culture) && !string.IsNullOrWhiteSpace(this.DumpLocalization))
            {
                this.Usage("Cannot specify " + Constants.Option.Culture + " with " + Constants.Option.DumpLocalization + " (en-US is implied)");
            }

            switch (restrictAllow)
            {
                case RestrictAllow.Allow:
                    this.Restrictions = ~allow & Restrictions.All;
                    break;
                case RestrictAllow.Restrict:
                    this.Restrictions = restrict;
                    break;
                case RestrictAllow.Both:
                    this.Usage($"Cannot specify both {Constants.Option.Allow} and {Constants.Option.Restrict}");
                    break;
            }

            // Get additional restrictions from the Registry.
            var key = Registry.LocalMachine.OpenSubKey(Constants.Misc.RegistryKey, false);
            if (key != null)
            {
                var value = (string)key.GetValue(Constants.Misc.RestrictionsValue);
                if (value != null)
                {
                    if (Enum.TryParse(value, true, out Restrictions r))
                    {
                        this.Restrictions |= r;
                    }
                    else
                    {
                        ErrorBox.Show($"Invalid restrictions in the registry, ignoring: '{value}'", "Registry Error");
                    }
                }

                key.Close();
            }

            if (this.Restrictions.HasFlag(Restrictions.ModifyProfiles))
            {
                this.ReadOnlyMode = true;
            }

            if (this.Restrictions.HasFlag(Restrictions.GetHelp))
            {
                Tour.SuppressAutoTours = true;
            }

            // Attach a console, if they asked for one.
            if (attachConsole)
            {
                this.AttachConsole();
            }

            // Set up PATH to include the install folder, so scripts can find things like x3270if.exe.
            var path = Environment.GetEnvironmentVariable("PATH");
            if (path == null)
            {
                Environment.SetEnvironmentVariable("PATH", Application.StartupPath);
            }
            else if (!path.Split(';').Contains(Application.StartupPath, StringComparer.InvariantCultureIgnoreCase))
            {
                Environment.SetEnvironmentVariable("PATH", path + ";" + Application.StartupPath);
            }

            // Set up internationalization.
            try
            {
                I18nBase.Setup("Wx3270", culture, forceBootstrap: !string.IsNullOrEmpty(this.DumpLocalization));
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, "wx3270 Localization", MessageBoxIcon.Information);
            }

            // No profile means read-only and (I think) no-watch.
            if (this.NoProfileMode)
            {
                this.ReadOnlyMode = true;
                this.NoWatchMode = true;
            }

            // Load the profile for the first time, so we can use its settings to create basic objects.
            this.ProfileManager = new ProfileManager(this);
            if (!this.NoProfileMode)
            {
                if (!this.ProfileManager.LoadCreate(profileName, readOnly: this.ReadOnlyMode, out string fullProfilePath))
                {
                    Environment.Exit(1);
                }

                profileName = fullProfilePath;
            }

            // Start the profile tree.
            this.ProfileTracker = new ProfileTracker(this, Wx3270.ProfileManager.ProfileDirectory);
            this.ProfileTracker.Watch(Wx3270.ProfileManager.ProfileDirectory);
            if (profileName != null && !this.NoWatchMode)
            {
                // Watch whatever directory the command-line profile is in, too.
                this.ProfileTracker.Watch(Path.GetDirectoryName(Wx3270.ProfileManager.SafeGetFullPath(profileName)));
            }

            // Watch other directories listed in the registry.
            this.ProfileTracker.WatchOthers();

            // Tell the profile manager about the profile tracker.
            // This is a little awkward because the profile tracker needs to be started after the profile manager,
            // but the profile manager needs the profile tracker.
            this.ProfileManager.SetProfileList(this.ProfileTracker);

            // Start a back end instance.
            Wx3270.BackEnd.DebugFlag = false;
            startupConfig.MergeProfile(this.Current);
            this.BackEnd = new BackEnd(this.control, startupConfig);
            Trace.BackEnd = this.BackEnd;

            // Register basic indication handlers.
            this.BackEnd.Register(this.screen = new Screen(this.update));
            this.BackEnd.Register(this.oia = new Oia(this.update));
            this.BackEnd.Register(this.TlsHello = new TlsHello());
            this.BackEnd.Register(this.Hello = new Hello());
            this.BackEnd.Register(this.Popup = new Popup());
            this.BackEnd.Register(this.Stats = new Stats());
            this.BackEnd.Register(this.ConnectAttempt = new ConnectAttempt());
            this.BackEnd.Register(this.WindowTitle = new WindowTitle());
            this.BackEnd.Register(this.CodePageDb = new CodePageDb());
            this.BackEnd.Register(this.ModelsDb = new ModelsDb());
            this.BackEnd.Register(this.ProxiesDb = new ProxiesDb());
            this.BackEnd.Register(this.HostPrefix = new HostPrefix());

            // Register specialized/cacheing indication handlers.
            this.SettingChange = new SettingChange(this.BackEnd);
            this.TerminalName = new TerminalName(this.BackEnd);

            // Register UI actions.
            this.BackEnd.RegisterPassthru(Constants.Action.QuitIfNotConnected, this.QuitIfNotConnected);

            // Propagate connection state changes.
            this.oia.ConnectionStateChange += (state) =>
            {
                this.SyntheticModChange(ModifierModeFromConnectionState(state), KeyboardModifier.ModeModifiers);
            };

            // Set up other parts.
            this.Sound = new Sound();
            this.BackEnd.OnExit += () => this.control.Invoke(new MethodInvoker(() => this.Sound.CleanUp()));

            this.KeyHandler = new KeyHandler(this);
            this.SelectionManager = new SelectionManager(this);

            this.Prompt = new Prompt(this.BackEnd);
            this.Cmd = new Cmd(this.BackEnd);

            this.bell = new Bell(this);

            s.Stop();
            Trace.Line(Trace.Type.Window, $"Wx3270App Init done in {s.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Process a screen update.
        /// </summary>
        /// <param name="updateType">Update type.</param>
        /// <param name="updateState">Update state.</param>
        public void ScreenUpdate(ScreenUpdateType updateType, UpdateState updateState)
        {
            this.update.ScreenUpdate(updateType, updateState);
        }

        /// <summary>
        /// Run a delegate on the UI thread.
        /// </summary>
        /// <param name="d">Delegate to run.</param>
        /// <returns>Whatever Control.Invoke returns.</returns>
        public object Invoke(Delegate d)
        {
            return this.control.Invoke(d);
        }

        /// <summary>
        /// Attach a Windows console to the process.
        /// </summary>
        public void AttachConsole()
        {
            if (this.ConsoleAttached)
            {
                return;
            }

            this.ConsoleAttached = true;

            if (!NativeMethods.AllocConsole())
            {
                // This message may be displayed before localization is complete.
                ErrorBox.Show(
                    "AllocConsole " + I18n.Get(Message.Failed, RawText.Failed),
                    I18n.Get(Title.SystemError, RawText.SystemError));
                return;
            }

            // Set the console output encoding to UTF-8 to match b3270.
            Console.OutputEncoding = new UTF8Encoding();

            // Display the warning message.
            Console.WriteLine("wx3270 " + I18n.Get(Message.ConsoleHeader, RawText.ConsoleHeader));
            Console.WriteLine();
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(I18n.Get(Message.ConsoleCautionPrefix, RawText.ConsoleCautionPrefix) + ":");
            Console.ForegroundColor = foreground;
            Console.WriteLine(" " + I18n.Get(Message.ConsoleCaution, RawText.ConsoleCaution) + ".");
            Console.WriteLine();
        }

        /// <summary>
        /// Set a region to be selected, and clear everything else. NVT mode.
        /// </summary>
        /// <param name="startBaddr">Start buffer address.</param>
        /// <param name="endBaddr">End buffer address, inclusive.</param>
        public void SetSelectNvt(int startBaddr, int endBaddr)
        {
            this.screen.SetSelectNvt(startBaddr, endBaddr);
        }

        /// <summary>
        /// Set a region to be selected, and clear everything else. 3270 mode.
        /// </summary>
        /// <param name="startRow0">Starting row, 0-origin.</param>
        /// <param name="startColumn0">Starting column, 0-origin.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        public void SetSelect3270(int startRow0, int startColumn0, int rows, int columns)
        {
            this.screen.SetSelect3270(startRow0, startColumn0, rows, columns);
        }

        /// <summary>
        /// Unselect the whole screen.
        /// </summary>
        /// <returns>True if anything was selected.</returns>
        public bool UnselectAll()
        {
            return this.screen.UnselectAll();
        }

        /// <summary>
        /// A screen draw operation completed.
        /// </summary>
        public void DrawComplete()
        {
            this.screen.DrawComplete();
        }

        /// <summary>
        /// Reset a pending chord.
        /// </summary>
        public void ChordReset()
        {
            this.ChordName = null;
            this.ChordResetEvent();
        }

        /// <summary>
        /// Checks for a permitted operation.
        /// </summary>
        /// <param name="restriction">Restriction to test.</param>
        /// <returns>True if permitted.</returns>
        public bool Allowed(Restrictions restriction)
        {
            return !this.Restricted(restriction);
        }

        /// <summary>
        /// Checks for a permitted operation.
        /// </summary>
        /// <param name="restriction">Restriction to test.</param>
        /// <returns>True if permitted.</returns>
        public bool Restricted(Restrictions restriction)
        {
            return this.Restrictions.HasFlag(restriction);
        }

        /// <summary>
        /// Display a usage pop-up and exit.
        /// </summary>
        /// <param name="reason">Reason for error.</param>
        private void Usage(string reason)
        {
            this.Splash.Stop();
            ErrorBox.Show(
                "Invalid command line option(s):" + Environment.NewLine + reason + Environment.NewLine + Environment.NewLine + "Use the -help option to get command-line help",
                "wx3270 Command Line Error");
            Environment.Exit(1);
        }

        /// <summary>
        /// Parse an allow or restrict option.
        /// </summary>
        /// <param name="option">Option name.</param>
        /// <param name="value">Parameter value.</param>
        /// <param name="res">Modified restrictions.</param>
        private void ParseAllowRestrict(string option, string value, ref Restrictions res)
        {
            if (!Enum.TryParse(value, true, out Restrictions r))
            {
                var modes = string.Join(", ", Enum.GetValues(typeof(Restrictions)).OfType<Restrictions>().Select(m => m.ToString()));
                this.Usage($"Invalid {option} value '{value}'" + Environment.NewLine + $"Options are {modes}");
            }

            if (r == Restrictions.None)
            {
                res = r;
            }
            else
            {
                res |= r;
            }
        }

        /// <summary>
        /// Pass-through action to quit if not connected.
        /// </summary>
        /// <param name="name">Command name.</param>
        /// <param name="args">Argument list.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult QuitIfNotConnected(string name, IEnumerable<string> args, out string result, string tag)
        {
            // Validate arguments.
            if (args.Any())
            {
                result = Constants.Action.QuitIfNotConnected + " takes 0 arguments";
                return PassthruResult.Failure;
            }

            // If not connected, quit.
            if (this.ConnectionState <= ConnectionState.TcpPending)
            {
                this.BackEnd.Stop();
            }

            result = null;
            return PassthruResult.Success;
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// System error.
            /// </summary>
            public static readonly string SystemError = I18n.Combine(TitleName, "systemError");
        }

        /// <summary>
        /// Localized messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// System call failed.
            /// </summary>
            public static readonly string Failed = I18n.Combine(TitleName, "failed");

            /// <summary>
            /// Console header.
            /// </summary>
            public static readonly string ConsoleHeader = I18n.Combine(TitleName, "consoleHeader");

            /// <summary>
            /// Console caution prefix.
            /// </summary>
            public static readonly string ConsoleCautionPrefix = I18n.Combine(TitleName, "consoleCautionPrefix");

            /// <summary>
            /// Console caution.
            /// </summary>
            public static readonly string ConsoleCaution = I18n.Combine(TitleName, "consoleCaution");
        }

        /// <summary>
        /// Un-localized text, used for localization hints and for display of messages before localization is complete.
        /// </summary>
        private class RawText
        {
            public const string SystemError = "System Error";
            public const string Failed = "failed";
            public const string ConsoleHeader = "Debug Console";
            public const string ConsoleCautionPrefix = "CAUTION";
            public const string ConsoleCaution = "Closing this window will terminate wx3270";
        }
    }
}
