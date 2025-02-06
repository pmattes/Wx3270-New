// <copyright file="MainScreen.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using I18nBase;
    using Wx3270.Contracts;

    using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
    using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
    using MouseEventHandler = System.Windows.Forms.MouseEventHandler;
    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// The main screen.
    /// </summary>
    public partial class MainScreen : Form, IUpdate, IShift, IFlash
    {
        /// <summary>
        /// The name of the resize localization.
        /// </summary>
        private const string ResizeName = "MainScreen.Resize";

        /// <summary>
        /// The name of the snap localization.
        /// </summary>
        private const string SnapName = "MainScreen.Snap";

        /// <summary>
        /// The name of the localized tool tip for the macros button.
        /// </summary>
        private const string MacrosToolTipName = "MainScreen.ToolTip.Macros";

        /// <summary>
        /// The name of the localized tool tip for the macros button, while recording.
        /// </summary>
        private const string MacroRecordingToolTipName = "MainScreen.ToolTip.MacroRecording";

        /// <summary>
        /// The name of the menu item to start macro recording.
        /// </summary>
        private const string MacroRecordingItemName = "MainScreen.Item.Record";

        /// <summary>
        /// The name of the menu item to stop recording.
        /// </summary>
        private const string MacroStopRecordingItemName = "MainScreen.Item.StopRecording";

        /// <summary>
        /// The number of steps in the overlay menu bar animation.
        /// </summary>
        private const int OverlayMenuBarSteps = 6;

        /// <summary>
        /// The name of the localized start button.
        /// </summary>
        private const string StartButtonName = "MainScreen.StartButton";

        /// <summary>
        /// The name of the localized close error message.
        /// </summary>
        private const string CloseName = "MainScreen.Close";

        /// <summary>
        /// Name of localized message box titles.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(MainScreen));

        /// <summary>
        /// Name of localized message box messages.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(MainScreen));

        /// <summary>
        /// Set of macros.
        /// </summary>
        private readonly SyncedListBoxes<MacroEntry> macroEntries = new SyncedListBoxes<MacroEntry>();

        /// <summary>
        /// Chord timer.
        /// </summary>
        private readonly Timer chordTimer = new Timer { Interval = 3 * 1000, Enabled = true };

        /// <summary>
        /// Set if the pop-up keypad is minimized.
        /// </summary>
        private readonly HashSet<Form> keypadMinimized = new HashSet<Form>();

        /// <summary>
        /// Set if the pop-up keypad is active.
        /// </summary>
        private readonly HashSet<Form> keypadActive = new HashSet<Form>();

        /// <summary>
        /// Start button renderer.
        /// </summary>
        private readonly StartButton startButton = new StartButton();

        /// <summary>
        /// The macro record menu items.
        /// </summary>
        private readonly List<ToolStripMenuItem> macroRecordItems = new List<ToolStripMenuItem>();

        /// <summary>
        /// True if the window is activated.
        /// </summary>
        private bool isActivated;

        /// <summary>
        /// Flash state machine.
        /// </summary>
        private FlashFsm flashFsm;

        /// <summary>
        /// The current color mapping.
        /// </summary>
        private Colors colors;

        /// <summary>
        /// The pop-up keypad.
        /// </summary>
        private Keypad keypad;

        /// <summary>
        /// The pop-up APL keypad.
        /// </summary>
        private AplKeypad aplKeypad;

        /// <summary>
        /// The settings dialog.
        /// </summary>
        private Settings settings;

        /// <summary>
        /// The actions dialog.
        /// </summary>
        private Actions actionsDialog;

        /// <summary>
        /// The macros dialog.
        /// </summary>
        private Macros macros;

        /// <summary>
        /// The profile tree dialog.
        /// </summary>
        private ProfileTree profileTree;

        /// <summary>
        /// The trace window.
        /// </summary>
        private TraceWindow traceWindow;

        /// <summary>
        /// True if the OIA is drawn in the 3270 font.
        /// </summary>
        private bool oia3270Font;

        /// <summary>
        /// True if we are between ResizeBegin and ResizeEnd events.
        /// </summary>
        private bool resizeBeginPending;

        /// <summary>
        /// True if we are in color (3279) mode.
        /// </summary>
        private bool colorMode = true;

        /// <summary>
        /// The width of the fixed screen elements.
        /// </summary>
        private int fixedWidth;

        /// <summary>
        /// The height of the fixed screen elements.
        /// </summary>
        private int fixedHeight;

        /// <summary>
        /// True if the scroll bar is displayed.
        /// </summary>
        private bool scrollBarDisplayed = true;

        /// <summary>
        /// True if in F11 full screen mode.
        /// </summary>
        private bool fullScreen = false;

        /// <summary>
        /// True if the overlay menu bar is displayed.
        /// </summary>
        private bool overlayMenuBarDisplayed = false;

        /// <summary>
        /// The roll up / roll down step of the overlay menu bar.
        /// </summary>
        private int overlayMenuBarStep;

        /// <summary>
        /// The overlay menu bar animation direction (-1 for up, +1 for down, 0 for stable).
        /// </summary>
        private int overlayMenuBarDirection = 0;

        /// <summary>
        /// True if the menu bar is disabled (config option).
        /// </summary>
        private bool menuBarDisabled = false;

        /// <summary>
        /// The crossbar.
        /// </summary>
        private Crossbar crossbar;

        /// <summary>
        /// True if the tour is complete.
        /// </summary>
        private bool toured = false;

        /// <summary>
        /// True if there is a read-only pop-up pending.
        /// </summary>
        private bool readOnlyPopUpPending = false;

        /// <summary>
        /// The resize count.
        /// </summary>
        private int resizeCount;

        /// <summary>
        /// True if a resize is in progress.
        /// </summary>
        private bool resizeLocked;

        /// <summary>
        /// The last window location before a maximize or docking.
        /// </summary>
        private Point? lastLocation;

        /// <summary>
        /// The window handle.
        /// </summary>
        private IntPtr handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainScreen"/> class.
        /// </summary>
        public MainScreen()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Delegate for the Win32 IsWindowArranged function.
        /// </summary>
        /// <param name="handle">Window handle.</param>
        /// <returns>True if the window is arranged.</returns>
        public delegate bool IsWindowArrangedDelegate(IntPtr handle);

        /// <summary>
        /// Secondary initialization event.
        /// </summary>
        public event Action SecondaryInitEvent = () => { };

        /// <summary>
        /// Connection state change event.
        /// </summary>
        public event Action ConnectionStateEvent = () => { };

        /// <summary>
        /// SSL change event.
        /// </summary>
        public event Action SslEvent = () => { };

        /// <summary>
        /// Event signaled when the LU name changes.
        /// </summary>
        public event Action LuEvent = () => { };

        /// <summary>
        /// Event signaled when the font changes dynamically.
        /// </summary>
        public event Action<Font> DynamicFontEvent = (f) => { };

        /// <summary>
        /// The stages of flashing.
        /// </summary>
        private enum FlashState
        {
            /// <summary>
            /// First flash showing.
            /// </summary>
            Flash1,

            /// <summary>
            /// Pause before second flash.
            /// </summary>
            Pause,

            /// <summary>
            /// Second flash showing.
            /// </summary>
            Flash2,
        }

        /// <summary>
        /// Gets the live host color to drawing color map.
        /// </summary>
        public HostColors ColorMap => this.colors.HostColors;

        /// <summary>
        /// Gets the live screen font.
        /// </summary>
        public Font ScreenFont
        {
            get
            {
                if (this.screenBox == null || this.screenBox.ScreenFont == null)
                {
                    return this.DefaultScreenFont;
                }

                return this.screenBox.ScreenFont;
            }
        }

        /// <summary>
        /// Gets or sets the connected host type.
        /// </summary>
        public HostType ConnectHostType { get; set; }

        /// <summary>
        /// Gets the connect machine.
        /// </summary>
        public Connect Connect { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the window is maximized.
        /// </summary>
        public bool Maximized => this.WindowState == FormWindowState.Maximized;

        /// <summary>
        /// Gets the default screen font.
        /// </summary>
        private Font DefaultScreenFont => Profile.DefaultProfile.Font.Font();

        /// <summary>
        /// Gets or sets the application instance.
        /// </summary>
        private Wx3270App App { get; set; }

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private IBackEnd BackEnd => this.App.BackEnd;

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager => this.App.ProfileManager;

        /// <summary>
        /// Gets the set of keypads.
        /// </summary>
        private List<Form> Keypads { get; } = new List<Form>();

        /// <summary>
        /// Gets the set of flashable keypads.
        /// </summary>
        private List<IFlash> FlashableKeypads { get; } = new List<IFlash>();

        /// <summary>
        /// Gets the macro recorder.
        /// </summary>
        private MacroRecorder MacroRecorder => this.App.MacroRecorder;

        /// <summary>
        /// Gets the actions dialog.
        /// </summary>
        private Actions ActionsDialog
        {
            get
            {
                if (this.actionsDialog == null)
                {
                    this.actionsDialog = new Actions(this.App, this);
                }

                return this.actionsDialog;
            }
        }

        /// <summary>
        /// Gets the keypad.
        /// </summary>
        private Keypad Keypad
        {
            get
            {
                if (this.keypad == null)
                {
                    this.keypad = new Keypad(this.App, this, this.crossbar.OptionsCrossbar);
                    this.FlashableKeypads.Add(this.keypad);
                    this.Keypads.Add(this.keypad);
                }

                return this.keypad;
            }
        }

        /// <summary>
        /// Gets the APL keypad.
        /// </summary>
        private AplKeypad AplKeypad
        {
            get
            {
                if (this.aplKeypad == null)
                {
                    this.aplKeypad = new AplKeypad(this.App, this, this.crossbar.OptionsCrossbar);
                    this.FlashableKeypads.Add(this.aplKeypad);
                    this.Keypads.Add(this.aplKeypad);
                }

                return this.aplKeypad;
            }
        }

        /// <summary>
        /// Gets the settings dialog.
        /// </summary>
        private Settings SettingsDialog
        {
            get
            {
                this.settings ??= new Settings(this.App, this, this.Keypad, this.AplKeypad);
                return this.settings;
            }
        }

        /// <summary>
        /// Gets the macros dialog.
        /// </summary>
        private Macros Macros
        {
            get
            {
                this.macros ??= new Macros(this.App, this, this.macroEntries);
                return this.macros;
            }
        }

        /// <summary>
        /// Gets the profile tree dialog.
        /// </summary>
        private ProfileTree ProfileTree
        {
            get
            {
                this.profileTree ??= new ProfileTree(this.App, this, this.Connect);
                return this.profileTree;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the menu bar is visible.
        /// </summary>
        private bool MenuBarVisible => !(this.fullScreen || this.menuBarDisabled || (!this.ProfileManager.Current.MenuBar && !this.overlayMenuBarDisplayed));

        /// <summary>
        /// Create a new window title string.
        /// </summary>
        /// <param name="hostEntry">Current host entry, or null.</param>
        /// <param name="profile">Current profile.</param>
        /// <param name="host">Current connected host, or null.</param>
        /// <returns>New title string.</returns>
        public static string NewTitle(HostEntry hostEntry, Profile profile, string host)
        {
            var readOnlyPrefix = profile.ReadOnly ? "[" + Wx3270.ProfileManager.ReadOnlyName + "] " : string.Empty;
            if (hostEntry != null && !string.IsNullOrWhiteSpace(hostEntry.WindowTitle))
            {
                return readOnlyPrefix + hostEntry.WindowTitle;
            }

            if (!string.IsNullOrWhiteSpace(profile.WindowTitle))
            {
                return readOnlyPrefix + profile.WindowTitle;
            }

            var profileNameDisplay = readOnlyPrefix + profile.Name;
            if (host != null)
            {
                profileNameDisplay += " / " + host;
            }

            return profileNameDisplay + " - wx3270";
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(ResizeName, "resize");
            I18n.LocalizeGlobal(SnapName, "snap");
            I18n.LocalizeGlobal(MacrosToolTipName, "Macros");
            I18n.LocalizeGlobal(MacroRecordingToolTipName, "Recording macro - click to stop");
            I18n.LocalizeGlobal(MacroRecordingItemName, "Record new");
            I18n.LocalizeGlobal(MacroStopRecordingItemName, "Stop recording");
            I18n.LocalizeGlobal(StartButtonName, "START", true);
            I18n.LocalizeGlobal(CloseName, "Fatal close error");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global step 1.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), 1), "Tour: wx3270 main window");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), 1),
@"This is a quick tour of the wx3270 main window. It will show you the basic features and provide some helpful tips to get started.");

            // Connect button (Start Here)
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(connectPictureBox)), "Start Here: Connect button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(connectPictureBox)),
@"Click and select Quick Connect to make your first connection to a host.");

            // Start button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(actionsBox)), "Start button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(actionsBox)),
@"Click to open the Start window, which allows you to:
• Start IND$FILE file transfers 
• Save a screen snapshot or trace screen contents to a file or the printer
• See session statistics
• Turn on debug tracing
• Open the wx3270> prompt

Right-click for a menu to perform each of these actions individually.");

            // Keypad button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(keypadBox)), "Keypad button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(keypadBox)),
@"Click to open the Keypad window, which gives you easy access to 3270-specific keys and functions.

Press Alt and click to open the APL Keypad window, which lets you enter APL characters.

Right-click to get a menu for the 3270-specific keys.");

            // Connect button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(connectPictureBox), 1), "Connect button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(connectPictureBox), 1),
@"Click to create a new host connection, connect to a host you have already defined, or disconnect the current host session.");

            // Profiles and connections button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(profilePictureBox)), "Profiles and connections button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(profilePictureBox)),
@"This button lets you navigate between different profiles (common settings for a set of hosts) and connections (settings for an individual host).

Click to open a window that lets you create, edit, delete, copy, rename, and merge profiles and connections, and to switch between them.

Right-click to switch quickly between profiles.");

            // Macros button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(macrosPictureBox)), "Macros button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(macrosPictureBox)),
@"Click to open the Macros window, which lets you define, edit and run macros.

Right-click to pick a macro to run, or to record a new one from the keyboard.

If this button is flashing, there is a macro recording in progress. Click to finish the recording.");

            // Snap button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(snapBox)), "Snap button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(snapBox)),
@"Click to shrink the screen to the minimum size needed to contain the current font. This is usually used after resizing the screen with the mouse, which will increase or decrease the font size, but may also leave blank margins at the edges of the display.

It is usually simpler to adjust the font size with the Ctrl-+ and Ctrl-- keys.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(helpPictureBox)),
@"Click to display context-sensitive help from the x3270 Wiki in your browser, or to start this tour again.");

            // Settings button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(settingsBox)), "Settings button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(settingsBox)),
@"Click to open the Settings window, which allows various settings to be changed, such as the 3270 model number, cursor type, font, colors and sounds.

Right-click to undo or redo the last change.");

            // OiaLock indicator
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), nameof(oiaLock)), "OIA lock field");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), nameof(oiaLock)),
@"This field indicates the overall state of the emulator.

If it starts with an 'X', the keyboard is locked, with the rest of the field indicating why. For example, if it shows a jagged horizontal line, then there is no active connection. If it shows '[TCP]', then the emulator is waiting for the host to accept the TCP connection. If it shows a a stick figure surrounded by arrows, you have tried to enter input in a protected field, and should press Alt-R to reset.");

            // Global step 2.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), 2), "Mouse tips");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), 2),
@"Hover the mouse over a button to see what it does, and whether there are left- and right-click variants.

Hover over an input field to see what it is used for.

Hover over an indication in the OIA to get further details.

Right-click on the emulator display for a menu that lets you perform any action on the menu bar.");

            // Global step 3.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MainScreen), 3), "Common keys");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MainScreen), 3),
@"Press Alt-F11 to switch in and out of full-screen mode.

Press Shift-Esc to switch in and out of APL keyboard mode.

Press Ctrl-+ to make the font bigger. Press Ctrl-- (Ctrl and the '-' key) to make it smaller. (These do not work when the screen is maximized.)

Press Alt-F4 or Alt-Q to exit wx3270.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Further initialization.
        /// </summary>
        /// <param name="app">Application instance.</param>
        public void Init(Wx3270App app)
        {
            var s = new Stopwatch();
            s.Start();
            Trace.Line(Trace.Type.Window, "MainScreen Init start");

            this.App = app;
            this.BaseInit(); // should do earlier?
            this.SecondaryInit();

            s.Stop();
            Trace.Line(Trace.Type.Window, $"MainScreen Init done in {s.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Process a screen update.
        /// </summary>
        /// <param name="updateType">Update type.</param>
        /// <param name="updateState">Update state.</param>
        public void ScreenUpdate(ScreenUpdateType updateType, UpdateState updateState)
        {
            // Run the update in the UI thread.
            this.Invoke(new MethodInvoker(() => this.ProcessUpdate(updateType, updateState)));
        }

        /// <summary>
        /// The (non-APL) keyboard modifiers changed.
        /// </summary>
        /// <param name="mod">New modifiers.</param>
        /// <param name="mask">Modifier mask.</param>
        public void ModChanged(KeyboardModifier mod, KeyboardModifier mask)
        {
            this.ChangeOiaMod(mod, mask);
        }

        /// <summary>
        /// APL mode changed.
        /// </summary>
        /// <param name="apl">True if APL mode.</param>
        public void AplChanged(bool apl)
        {
            this.App.AplMode = apl;
            this.ChangeOiaMod(apl ? KeyboardModifier.Apl : KeyboardModifier.None, KeyboardModifier.Apl);
        }

        /// <summary>
        /// Force the main screen window to maximize.
        /// </summary>
        /// <param name="why">Why we are maximizing.</param>
        public void Maximize(string why)
        {
            if (!this.Maximized)
            {
                Trace.Line(Trace.Type.Window, $"Maximizing ({why})");
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// Force the main screen window to restore (not be maximized).
        /// </summary>
        /// <param name="why">Why we are restoring.</param>
        public void Restore(string why)
        {
            if (this.Maximized)
            {
                Trace.Line(Trace.Type.Window, $"Restoring ({why})");
                this.WindowState = FormWindowState.Normal;
            }
        }

        /// <summary>
        /// Unconditionally close the app.
        /// </summary>
        public void AppClose()
        {
            // Hide the window so there is immediate feedback.
            this.Hide();

            // Close the current profile.
            this.ProfileManager.Close();

            // Stop by forcing the back end to exit.
            this.BackEnd.Stop();
        }

        /// <summary>
        /// Change the main screen color scheme.
        /// </summary>
        /// <param name="newColors">New color scheme.</param>
        /// <param name="colorMode">True if in 3279 mode.</param>
        public void Recolor(Colors newColors, bool colorMode)
        {
            // Update the real color maps.
            this.colors = new Colors(newColors);

            // Redraw the screen.
            this.ScreenNeedsDrawing(this.App.ScreenImage, "recolor", true);

            // Change the foreground color in the OIA.
            var fg = colorMode ? this.colors.HostColors[HostColor.Blue] : this.colors.MonoColors.Normal;
            var oiaControls = new Control[this.oiaLayoutPanel.Controls.Count + this.oiaLockFlowLayoutPanel.Controls.Count];
            this.oiaLayoutPanel.Controls.CopyTo(oiaControls, 0);
            this.oiaLockFlowLayoutPanel.Controls.CopyTo(oiaControls, this.oiaLayoutPanel.Controls.Count);
            foreach (var oiaField in oiaControls)
            {
                if (colorMode && (oiaField == this.oiaLock || oiaField == this.oiaLockNative))
                {
                    oiaField.ForeColor = this.colors.HostColors[this.oiaLock.Tag != null ? HostColor.Red : HostColor.NeutralWhite];
                }
                else
                {
                    oiaField.ForeColor = fg;
                }
            }

            // Correct the lock icon.
            this.ChangeOiaTls(this.App.OiaState);

            // Change the dividing bar color.
            this.topBar.BackColor = fg;
            this.bottomBar.BackColor = fg;

            // Change the form background color.
            this.BackColor = colorMode ? this.colors.HostColors[HostColor.NeutralBlack] : this.colors.MonoColors.Background;

            // Remember the mode.
            this.colorMode = colorMode;
        }

        /// <summary>
        /// Change the main screen font.
        /// </summary>
        /// <param name="font">New font.</param>
        public void Refont(Font font)
        {
            // Change the display font.
            this.ScreenNewFont(font);
            var newFont = font;
            if (this.Maximized || this.IsWindowArranged())
            {
                // Recompute the font size when maximized (ignore the selected size).
                newFont = this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic);
            }

            // Change the OIA.
            this.RefontOia(newFont);

            if (!this.Maximized && !this.IsWindowArranged())
            {
                // Do an implicit snap.
                this.ClientSize = this.mainScreenPanel.Size;
            }
        }

        /// <summary>
        /// Make the screen fit the current font tightly.
        /// </summary>
        public void Snap()
        {
            if (this.Maximized || this.Size == this.MinimumSize || this.IsWindowArranged())
            {
                // No snapping when maximized, at the minimum, or docked.
                return;
            }

            this.screenBox.Snap();
            this.ClientSize = this.mainScreenPanel.Size;
            if (this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.Size = this.Size;
                }, I18n.Get(SnapName)))
            {
                Trace.Line(Trace.Type.Window, "  Snap pushed");
            }
        }

        /// <inheritdoc />
        public void Flash()
        {
            if (!this.noFlashTimer.Enabled && this.flashFsm.Start() == FlashFsm.Action.Flash)
            {
                this.startLeftPictureBox.Image = Properties.Resources.StartClearerLeft;
                this.actionsBox.Image = Properties.Resources.StartClearerMiddle;
                this.startRightPictureBox.Image = Properties.Resources.StartClearerRight;
            }
        }

        /// <inheritdoc />
        public void ActivationChange(Form form, bool activated)
        {
            // Update the list of active keypads.
            if (activated)
            {
                this.keypadActive.Add(form);
            }
            else
            {
                this.keypadActive.Remove(form);
            }

            // If the first keypad has become active, and this window was not, start blinking.
            // If the last keypad has become inactive, and this window was not, stop blinking.
            if (!this.isActivated && this.keypadActive.Any())
            {
                this.screenBox.Activated(true);
            }

            if (!this.isActivated && !this.keypadActive.Any())
            {
                this.screenBox.Activated(false);
            }
        }

        /// <summary>
        /// Process a title change from the settings window.
        /// </summary>
        public void Retitle()
        {
            this.Text = NewTitle(this.Connect.ConnectHostEntry, this.ProfileManager.Current, this.CurrentHost());
        }

        /// <summary>
        /// Add or remove the scroll bar. Called when the scroll bar option changes.
        /// </summary>
        /// <param name="displayed">True if scroll bar should be displayed.</param>
        /// <returns>New screen size, or null.</returns>
        public Size? ToggleScrollBar(bool displayed)
        {
            if (this.App.NoScrollBar || displayed == this.scrollBarDisplayed)
            {
                return null;
            }

            // If currently maximized, restore first. This is rather hacky, but it works.
            var wasMaximized = false;
            if (this.Maximized)
            {
                this.Restore("scrollbar toggle start");
                wasMaximized = true;
            }

            if (!displayed)
            {
                this.BackEnd.RunAction(
                    new BackEndAction(
                        B3270.Action.Scroll,
                        "Set",
                        "0"),
                    Wx3270.BackEnd.Ignore());
                this.vScrollBar1.RemoveFromParent();
            }
            else
            {
                this.ScrollBarLayoutPanel.SuspendLayout();
                this.ScrollBarLayoutPanel.Controls.Add(this.vScrollBar1);
                this.ScrollBarLayoutPanel.Controls.SetChildIndex(this.vScrollBar1, 0);
                this.ScrollBarLayoutPanel.ResumeLayout();
            }

            this.scrollBarDisplayed = displayed;

            // Re-evluate the fixed area of the screen and set the overall screen size.
            this.AdjustFixedForScrollBar(displayed);
            this.ClientSize = this.mainScreenPanel.Size;
            var size = this.Size;

            // Return to maximized if needed.
            if (wasMaximized)
            {
                this.Maximize("scrollbar toggle end");
            }

            return size;
        }

        /// <summary>
        /// Switch the state of the fixed menu bar.
        /// </summary>
        /// <param name="displayed">True to display the fixed menu bar.</param>
        public void FixedMenuBarSwitch(bool displayed)
        {
            var size = this.ToggleFixedMenuBar(displayed);
            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.MenuBar = displayed;
                    if (size != null)
                    {
                        current.Size = size.Value;
                    }
                },
                Settings.ChangeName(Settings.ChangeKeyword.MenuBar));
        }

        /// <summary>
        /// Switch the state of the scrollbar.
        /// </summary>
        /// <param name="displayed">True to display the scrollbar.</param>
        public void ScrollBarSwitch(bool displayed)
        {
            var size = this.ToggleScrollBar(displayed);
            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.ScrollBar = displayed;
                    if (size != null)
                    {
                        current.Size = size.Value;
                    }
                },
                Settings.ChangeName(Settings.ChangeKeyword.ScrollBar));
        }

        /// <summary>
        /// Change the state of the menu bar. Called when the option changes.
        /// </summary>
        /// <param name="displayed">True if the menu bar should be displayed.</param>
        /// <returns>New screen size, or null.</returns>
        public Size? ToggleFixedMenuBar(bool displayed)
        {
            if (this.App.NoButtons || displayed == !this.menuBarDisabled)
            {
                return null;
            }

            if (this.overlayMenuBarDisplayed)
            {
                // Get rid of the overlay menu bar.
                this.topBar.RemoveFromParent();
                this.TopLayoutPanel.RemoveFromParent();
                this.overlayMenuBarDisplayed = false;
            }

            var wasFullScreen = false;
            if (this.fullScreen)
            {
                this.DoFullScreen();
                wasFullScreen = true;
            }

            // If currently maximized, restore first. This is rather hacky, but it works.
            var wasMaximized = false;
            if (this.Maximized)
            {
                this.Restore("menubar toggle start");
                wasMaximized = true;
            }

            if (!displayed)
            {
                // Hide the menu bar.
                this.mainTable.SuspendLayout();
                this.topBar.RemoveFromParent();
                this.TopLayoutPanel.RemoveFromParent();
                this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 0F);
                this.mainTable.ResumeLayout();
                this.fixedHeight -= this.topBar.Height + this.TopLayoutPanel.Height;
            }
            else
            {
                // Put the menu bar back.
                this.mainTable.SuspendLayout();
                this.topBar.Location = new Point(0, 0);
                this.mainTable.Controls.Add(this.topBar, 0, 1);
                this.mainTable.Controls.Add(this.TopLayoutPanel, 0, 0);
                this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 2F);
                this.mainTable.ResumeLayout();
                this.fixedHeight += this.topBar.Height + this.TopLayoutPanel.Height;
            }

            this.menuBarDisabled = !displayed;
            this.temporaryToolStripMenuItem.Enabled = !displayed || wasFullScreen;
            this.permanentToolStripMenuItem.Enabled = !displayed;

            // Re-evaluate the fixed area of the screen and set the overall screen size.
            this.screenBox.SetFixed(this.fixedWidth, this.fixedHeight);
            this.ClientSize = this.mainScreenPanel.Size;
            var size = this.Size;

            // Return to maximized if needed.
            if (wasMaximized)
            {
                this.Maximize("menubar toggle end");
            }

            if (wasFullScreen)
            {
                this.DoFullScreen(withWarning: false);
            }

            if (!displayed && !wasFullScreen)
            {
                this.PopUpMenuBarWarning();
            }

            if (displayed && wasFullScreen)
            {
                ErrorBox.Show(
                    I18n.Get(ErrorMessage.MenuBarToggleNop),
                    I18n.Get(Title.MenuBarEnabled),
                    MessageBoxIcon.Information);
            }

            return size;
        }

        /// <summary>
        /// Duplicates the current profile.
        /// </summary>
        public void DuplicateProfile()
        {
            this.ProfileTree.DuplicateProfile(this.ProfileManager.Current);
        }

        /// <summary>
        /// Override for key event processing.
        /// </summary>
        /// <param name="msg">Message received.</param>
        /// <param name="keyData">Key data.</param>
        /// <returns>True if message was processed.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.App.KeyHandler.CmdKey(msg, keyData))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Rounds a floating point number down.
        /// </summary>
        /// <param name="f">Number to round.</param>
        /// <param name="granularity">Rounding granularity.</param>
        /// <returns>Rounded number.</returns>
        private static float RoundDown(float f, float granularity)
        {
            return (float)(Math.Truncate(f / granularity) * granularity);
        }

        /// <summary>
        /// Tests for a window being arranged (docked).
        /// </summary>
        /// <returns>True if window is arranged.</returns>
        private bool IsWindowArranged()
        {
            var isWindowArranged = FunctionLoader.LoadFunction<IsWindowArrangedDelegate>("User32.dll", "IsWindowArranged");
            if (isWindowArranged == default)
            {
                return false;
            }

            return isWindowArranged(this.handle);
        }

        /// <summary>
        /// Add or remove the scroll bar. Called when the profile changes.
        /// </summary>
        /// <param name="displayed">True if scroll bar should be displayed.</param>
        private void ToggleScrollBarInternal(bool displayed)
        {
            if (this.App.NoScrollBar || displayed == this.scrollBarDisplayed)
            {
                return;
            }

            this.ScrollBarSwitch(displayed);
        }

        /// <summary>
        /// Add or remove the menu bar. Called when the profile changes.
        /// </summary>
        /// <param name="displayed">True if the menubar should be displayed.</param>
        private void ToggleMenuBarInternal(bool displayed)
        {
            if (this.App.NoButtons || displayed == !this.menuBarDisabled)
            {
                return;
            }

            if (this.overlayMenuBarDisplayed)
            {
                // Get rid of the overlay menu bar.
                this.overlayMenuBarStep = OverlayMenuBarSteps;
                this.overlayMenuBarDirection = -1;
                this.overlayMenuBarTimer.Start();
            }

            if (!displayed)
            {
                // Hide the menu bar.
                this.topBar.RemoveFromParent();
                this.TopLayoutPanel.RemoveFromParent();
                this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 0F);
                this.fixedHeight -= this.topBar.Height + this.TopLayoutPanel.Height;
            }
            else
            {
                // Put the menu bar back.
                this.topBar.Location = new Point(0, 0);
                this.mainTable.Controls.Add(this.topBar, 0, 1);
                this.mainTable.Controls.Add(this.TopLayoutPanel, 0, 0);
                this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 2F);
                this.fixedHeight += this.topBar.Height + this.TopLayoutPanel.Height;
            }

            this.menuBarDisabled = !displayed;

            // Re-evluate the fixed area of the screen and set the overall screen size.
            this.screenBox.SetFixed(this.fixedWidth, this.fixedHeight);
        }

        /// <summary>
        /// Returns the current host name, or null.
        /// </summary>
        /// <returns>Current host name, or null.</returns>
        private string CurrentHost()
        {
            if (this.App.ConnectionState == ConnectionState.NotConnected)
            {
                return null;
            }

            return (this.Connect.ConnectHostEntry != null) ? this.Connect.ConnectHostEntry.Name : this.App.CurrentHostIp;
        }

        /// <summary>
        /// Check if a location is visible.
        /// </summary>
        /// <param name="location">Location to evaluate.</param>
        /// <returns>True if location is visible.</returns>
        private bool IsVisible(Point location)
        {
            var screen = System.Windows.Forms.Screen.FromPoint(location);
            return screen.WorkingArea.Contains(new Rectangle(location, new Size(50, 50)));
        }

        /// <summary>
        /// Initialize non-designer main screen state.
        /// </summary>
        private void BaseInit()
        {
            // Force the window handle to be created, so early callbacks don't fail.
            this.handle = this.Handle;

            // Register for profile-related events.
            this.App.ProfileTracker.ProfileTreeChanged += (tree) => this.Invoke(new MethodInvoker(() => this.ProfileTreeChanged(tree)));
            this.ProfileManager.NewProfileOpened += (profile) =>
            {
                // Set the window location, if reasonable to do so.
                if (!this.App.Location.HasValue && profile.Location.HasValue && !profile.ReadOnly && this.IsVisible(profile.Location.Value))
                {
                    this.Location = profile.Location.Value;
                }
            };
            this.ProfileManager.AddChangeTo(this.ProfileChanged);
            this.ProfileManager.ChangeFinal += (oldProfile, newProfile, isNew, isInternal) =>
            {
                // Process these last:
                //  Scroll bar.
                //  Menu bar.
                //  Window size.
                Size? size = newProfile.Size.HasValue ? (Size?)new Size(newProfile.Size.Value.Width, newProfile.Size.Value.Height) : null;
                if ((size.HasValue && !size.Value.Equals(this.Size))
                    || oldProfile?.ScrollBar != newProfile.ScrollBar
                    || oldProfile?.MenuBar != newProfile.MenuBar)
                {
                    if (!newProfile.ScrollBar && this.scrollBarDisplayed)
                    {
                        Trace.Line(Trace.Type.Window, "MainScreen ChangeFinal Turning off scrollbar");
                    }

                    this.ToggleScrollBarInternal(newProfile.ScrollBar);

                    if (!newProfile.MenuBar && !this.menuBarDisabled)
                    {
                        Trace.Line(Trace.Type.Window, "MainScreen ChangeFinal Turning off menu bar");
                    }

                    this.ToggleMenuBarInternal(newProfile.MenuBar);

                    this.temporaryToolStripMenuItem.Enabled = this.menuBarDisabled || this.fullScreen;
                    this.permanentToolStripMenuItem.Enabled = this.menuBarDisabled && !this.fullScreen;

                    if (size.HasValue && !size.Value.Equals(this.Size) && !this.Maximized && !this.IsWindowArranged())
                    {
                        Trace.Line(Trace.Type.Window, $"MainScreen ChangeFinal setting size to {size.Value}");
                        this.Size = size.Value;
                    }
                }

                // Update key mappings.
                this.UpdateMenuKeyMappings();

                // Handle read-only profiles.
                this.readOnlyPopUpPending |= !this.App.ReadOnlyMode && newProfile.ReadOnlyForced;
            };
            this.ProfileManager.ProfileClosing += (profile) =>
            {
                if (!profile.ReadOnly)
                {
                    // If not maximized or docked, save the current location in the profile.
                    // Otherwise if we have a location prior to maximizing or docking, save that.
                    if (!this.Maximized && !this.IsWindowArranged())
                    {
                        profile.Location = this.Location;
                    }
                    else if (this.lastLocation.HasValue)
                    {
                        profile.Location = this.lastLocation.Value;
                    }
                }
            };
            this.ProfileManager.MainWindowHandle = this.Handle;

            // Set up other parts.
            this.Connect = new Connect(this.App, this);
            this.crossbar = new Crossbar(this.App, this.ProfileManager);
            this.crossbar.OptionsCrossbar.OpacityEvent += (percent) => this.Opacity = percent / 100.0;
            this.crossbar.OptionsCrossbar.MainWindowHandle = this.Handle;

            if (!string.IsNullOrEmpty(this.App.DumpLocalization) || !I18nBase.UsingMessageCatalog)
            {
                // Create all of the dialogs now instead of on-demand, to capture their localizations.
                var settings = this.SettingsDialog;
                var actionsDialog = this.ActionsDialog;
                var macros = this.Macros;
                var profileTree = this.ProfileTree;
            }

            // Register for macro change events.
            // This is done after creating the dialogs, so the macros update doesn't propagate too early.
            this.macroEntries.ChangeEvent += this.MacrosChanged;

            // VS designer doesn't seem to know about this.
            this.MouseWheel += new MouseEventHandler(this.MouseWheel_Event);
        }

        /// <summary>
        /// Mouse wheel event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MouseWheel_Event(object sender, MouseEventArgs e)
        {
            var numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            if (numberOfTextLinesToMove == 0)
            {
                return;
            }

            var delta = -(numberOfTextLinesToMove * this.vScrollBar1.SmallChange);
            if (delta > 0)
            {
                var interactiveMax = 1 + this.vScrollBar1.Maximum - this.vScrollBar1.LargeChange;
                if (this.vScrollBar1.Value + delta > interactiveMax)
                {
                    this.vScrollBar1.Value = interactiveMax;
                }
                else
                {
                    this.vScrollBar1.Value += delta;
                }
            }
            else
            {
                if (this.vScrollBar1.Value + delta < this.vScrollBar1.Minimum)
                {
                    this.vScrollBar1.Value = this.vScrollBar1.Minimum;
                }
                else
                {
                    this.vScrollBar1.Value += delta;
                }
            }

            this.VScrollBar1_Scroll(this.vScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, this.vScrollBar1.Value));
        }

        /// <summary>
        /// Adjust the size of the fixed parts of the screen based on the scroll bar appearing or disappearing.
        /// </summary>
        /// <param name="scrollBar">True if the scroll bar is displayed.</param>
        private void AdjustFixedForScrollBar(bool scrollBar)
        {
            if (scrollBar)
            {
                this.fixedWidth += this.vScrollBar1.Width;
            }
            else
            {
                this.fixedWidth -= this.vScrollBar1.Width;
            }

            this.screenBox.SetFixed(this.fixedWidth, this.fixedHeight);
        }

        /// <summary>
        /// Secondary initialization, called after the main screen objects have been set up.
        /// </summary>
        private void SecondaryInit()
        {
            // Set up the flasher.
            this.flashFsm = new FlashFsm(
                (action) =>
                {
                    switch (action)
                    {
                        case FlashFsm.Action.Flash:
                            this.startLeftPictureBox.Image = Properties.Resources.StartClearerLeft;
                            this.actionsBox.Image = Properties.Resources.StartClearerMiddle;
                            this.startRightPictureBox.Image = Properties.Resources.StartClearerRight;
                            break;
                        case FlashFsm.Action.Restore:
                            this.startLeftPictureBox.Image = Properties.Resources.StartBlankLeft;
                            this.actionsBox.Image = Properties.Resources.StartBlankMiddleWide;
                            this.startRightPictureBox.Image = Properties.Resources.StartBlankRight;
                            break;
                        default:
                            break;
                    }
                });

            // Set up the screen box and its events.
            this.screenBox = new ScreenBox("MainScreen", this.screenPictureBox, this.crosshairPictureBox);
            this.screenBox.SizeChanged += (cellSizeHeight) =>
            {
                // Reconfigure the components of the screen.
                this.TopLayoutPanel.Width = this.innerScreenTableLayoutPanel.Width;
                this.topBar.Width = this.innerScreenTableLayoutPanel.Width;
                this.bottomBar.Width = this.innerScreenTableLayoutPanel.Width;
                this.oiaLayoutPanel.Width = this.innerScreenTableLayoutPanel.Width;
                this.oiaLayoutPanel.Height = cellSizeHeight + 4;
            };
            this.screenBox.FontChanged += (font, dynamic) =>
            {
                this.RefontOia(font);
                if (dynamic && this.WindowState == FormWindowState.Normal && !this.IsWindowArranged() && this.FormBorderStyle != FormBorderStyle.None)
                {
                    this.DynamicFontEvent(font);
                }
            };

            Trace.Line(
                Trace.Type.Window,
                "SecondaryInit: Size {0} ClientSize {1} MinimumSize {2}",
                this.Size,
                this.ClientSize,
                this.MinimumSize);
            this.screenBox.SetMinimumClient(
                this.MinimumSize.Width - (this.Size.Width - this.ClientSize.Width),
                this.MinimumSize.Height - (this.Size.Height - this.ClientSize.Height));

            // Remove the scroll bar.
            if (this.App.NoScrollBar)
            {
                this.vScrollBar1.RemoveFromParent();
                this.scrollBarDisplayed = false;
            }

            // Remove the menu bar.
            if (this.App.NoButtons)
            {
                this.topBar.RemoveFromParent();
                this.TopLayoutPanel.RemoveFromParent();
                this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 0F);
            }

            // Measure the overhead: the size of the fixed parts of the main screen.
            //
            // If we do it now, without ever setting a font in the screen box, we will get crazy results.
            // If we wait until the profile has been pushed out, we will get the right answer, but we may
            // have sized the screen incorrectly, because it was computed assuming an overhead of {0,0}.
            // So we set a dummy font on the screen box here, and measure.
            this.Refont(new FontProfile().Font());

            // One more quirk. We need the size of the text in the OIA. This is not OiaLock.Font.Height. We need to
            // measure it.
            int hh;
            using (Graphics g = this.screenPictureBox.CreateGraphics())
            {
                hh = ScreenBox.ComputeCellSize(g, this.oiaLock.Font).Height;
            }

            this.fixedWidth = this.mainScreenPanel.Width - this.screenPictureBox.Parent.Width;
            this.fixedHeight = this.mainScreenPanel.Height - this.screenPictureBox.Parent.Height - hh;
            this.screenBox.SetFixed(this.fixedWidth, this.fixedHeight);

            // Set up the macro recorder.
            this.MacroRecorder.FlashEvent += this.OnFlashEvent;
            this.MacroRecorder.RunningEvent += this.OnMacroRecorderState;

            // Set up the initial screen position.
            if (this.App.Location.HasValue)
            {
                // Make sure the window is actually visible. The check is that enough of the window is
                // actually visible so that it could be dragged usefully.
                if (this.IsVisible(this.App.Location.Value))
                {
                    this.Location = this.App.Location.Value;
                }
            }

            // Set up undo/redo.
            this.ProfileManager.RegisterUndoRedo(this.undoToolStripMenuItem, this.redoToolStripMenuItem, this.toolTip1);

            // Subscribe to connection changes.
            this.ConnectionStateEvent += () =>
            {
                this.Retitle();
                this.SetPairedEnabled(this.disconnectMenuItem, this.App.ConnectionState != ConnectionState.NotConnected);
                this.SetPairedEnabled(this.quickConnectMenuItem, this.App.ConnectionState == ConnectionState.NotConnected);
            };

            // Subscribe to toggle changes.
            this.App.SettingChange.Register(
                this.SettingChanged,
                new[] { B3270.Setting.Trace, B3270.Setting.ScreenTrace, B3270.Setting.VisibleControl, B3270.Setting.AplMode });

            // Cascade secondary init to others.
            this.SecondaryInitEvent();

            // Dump errors from the first profile load, on a separate thread.
            this.profileErrorTimer.Enabled = true;

            // When the emulator is ready, there is more iniitialization to do.
            this.App.BackEnd.OnReady += this.WhenReadyInit;

            // Register the Chord action.
            this.BackEnd.RegisterPassthru(Constants.Action.Chord, this.Chord);
            this.App.ChordResetEvent += this.ChordReset;
            this.chordTimer.Tick += (sender, args) => this.ChordReset();

            // Register the local PrintText() action.
            this.BackEnd.RegisterPassthru(Constants.Action.PrintText, this.PrintText);

            // Handle user-generated window title changes.
            this.App.WindowTitle.Add(this, () =>
            {
                if (string.IsNullOrWhiteSpace(this.ProfileManager.Current.WindowTitle))
                {
                    this.Text = this.App.WindowTitle.Title;
                }
            });

            // Handle restrictions.
            if (this.App.Restricted(Restrictions.ModifyProfiles))
            {
                this.profileContextMenuStrip.Items.Clear();
            }

            if (this.App.Restricted(Restrictions.ModifyProfiles) || this.App.Restricted(Restrictions.ModifyHost))
            {
                this.quickConnectMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.ChangeSettings))
            {
                this.settingsBox.Visible = false;
            }

            if (this.App.Restricted(Restrictions.Prompt))
            {
                this.x3270PromptToolStripMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.FileTransfer))
            {
                this.fileTransferMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.ExternalFiles))
            {
                this.tracingToolStripMenuItem.RemoveFromOwner();
                this.saveToFileToolStripMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.Printing))
            {
                this.sendToPrinterToolStripMenuItem.RemoveFromOwner();
                this.printScreenToolStripMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.ChangeSettings))
            {
                this.controlCharsMenuItem.RemoveFromOwner();
                this.permanentToolStripMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.Visible = false;
                this.helpToolStripMenuItem.RemoveFromOwner();
            }

            if (this.App.Restricted(Restrictions.ChangeSettings) && !this.ProfileManager.Current.Macros.Any())
            {
                this.macrosPictureBox.Visible = false;
            }

            if (this.App.Restricted(Restrictions.Disconnect))
            {
                this.disconnectMenuItem.RemoveFromOwner();
            }

            // The screen tracing menu item might be emtpy at this point.
            if (!this.screenTracingMenuItem.HasDropDownItems)
            {
                this.screenTracingMenuItem.RemoveFromOwner();
            }

            // The profile tree may be moot at this point.
            if (this.App.Restricted(Restrictions.SwitchProfile) &&
                this.App.Restricted(Restrictions.ModifyHost) &&
                this.App.Restricted(Restrictions.ModifyProfiles) &&
                this.App.Restricted(Restrictions.NewWindow) &&
                this.App.Restricted(Restrictions.Disconnect) &&
                this.ProfileManager.Current.Hosts.Any(host => host.AutoConnect == AutoConnect.Reconnect))
            {
                this.profilePictureBox.Visible = false;
            }

            // The connect menu may be moot at this point.
            if (this.App.Restricted(Restrictions.Disconnect) &&
                this.ProfileManager.Current.Hosts.Any(host => host.AutoConnect == AutoConnect.Reconnect))
            {
                this.connectPictureBox.Visible = false;
            }

            if (this.App.NoBorder)
            {
                this.snapBox.Visible = false;
            }

            // Set up the screen snap and step emulator font actions.
            if (this.App.Allowed(Restrictions.ExternalFiles))
            {
                this.App.BackEnd.RegisterPassthru(Constants.Action.SnapScreen, this.UiSnapScreen);
                this.App.BackEnd.RegisterPassthru(Constants.Action.StepEfont, this.UiStepEfont);
            }

            // Set up the full-screen action.
            if (!this.App.NoBorder)
            {
                this.App.BackEnd.RegisterPassthru(Constants.Action.FullScreen, this.UiFullScreen);
            }

            if (!this.App.NoButtons)
            {
                this.App.BackEnd.RegisterPassthru(Constants.Action.MenuBar, this.UiMenuBar);
            }

            if (this.App.NoButtons)
            {
                // De-populate the context menu.
                foreach (var item in this.screenBoxContextMenuStrip.Items.Cast<ToolStripMenuItem>().ToList())
                {
                    if (item.Name != "editToolStripMenuItem" && item.Name != "keypadToolStripMenuItem")
                    {
                        item.RemoveFromOwner();
                    }
                }
            }

            // Clone the menu bar menus to the context menu.
            this.CloneMenus();

            // Localize.
            I18n.Localize(this, this.toolTip1);
            this.InitOiaLocalization();
            I18n.LocalizeGlobal(Title.Connect, "Connect");
            I18n.LocalizeGlobal(Title.MacroError, "Macro Error");
            I18n.LocalizeGlobal(Title.KeypadMenuError, "Keypad Menu Error");
            I18n.LocalizeGlobal(Title.FullScreen, "Full Screen Mode");
            I18n.LocalizeGlobal(Title.MenuBarDisabled, "Menu Bar Disabled");
            I18n.LocalizeGlobal(Title.MenuBarEnabled, "Menu Bar Enabled");
            I18n.LocalizeGlobal(ErrorMessage.NoSuchConnection, "No such connection");
            I18n.LocalizeGlobal(ErrorMessage.InvalidPrefixes, "Invalid prefix(es) in command-line host");
            I18n.LocalizeGlobal(ErrorMessage.MenuBarToggle, "To display the menu bar again, right-click on the main screen and select 'Menu bar'.");
            I18n.LocalizeGlobal(ErrorMessage.MenuBarToggleNop, "The menu bar is not displayed in full screen mode. Right-click on the main screen for the context menu.");
            I18n.LocalizeGlobal(ErrorMessage.FullScreenToggle, "To exit full screen mode, right-click on the main screen and select 'Full screen'.");
            I18n.LocalizeGlobal(ErrorMessage.RecordingDiscarded, "Pending macro recording discarded");
            I18n.LocalizeGlobal(SaveType.CommandLineHost, "Command-line host connection");

            // Initialize the OIA fields.
            var defState = Oia.DefaultOiaState;
            this.ChangeOiaNetwork(defState);
            this.ChangeOiaLock(defState);
            this.oiaPrinter.Text = string.Empty;
            this.oiaScreentrace.Text = string.Empty;
            this.oiaScript.Text = string.Empty;
            this.oiaTypeahead.Text = string.Empty;
            this.oiaAltShift.Text = string.Empty;
            this.oiaCx.Text = string.Empty;
            this.oiaReverse.Text = string.Empty;
            this.ChangeOiaInsert(defState);
            this.ChangeOiaTls(defState);
            this.ChangeOiaLu(defState);
            this.ChangeOiaTiming(defState);
            this.ChangeOiaCursor(defState);

            // Update the menu key mappings.
            this.UpdateMenuKeyMappings();

            // Set up the connect menu.
            this.ProfileTreeChanged(this.App.ProfileTracker.Tree);

            // Handle the no-border option.
            if (this.App.NoBorder)
            {
                this.ControlBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.snapBox.Visible = false;
            }

            // Make this window topmost, if requested.
            if (this.App.Topmost)
            {
                // Toggling topmost on and off brings this window to the top, but does not force it to stay there.
                this.TopMost = true;
                this.TopMost = false;
            }
        }

        /// <summary>
        /// Initialization once the back end is ready.
        /// </summary>
        private void WhenReadyInit()
        {
            // Tell the back end our Window handle.
            this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.WindowId, this.Handle), Wx3270.BackEnd.Ignore());

            // If there is a command-line model change, update the profile directly and save it.
            if (this.App.Model != null || this.App.Oversize != null)
            {
                if (this.App.Model != null)
                {
                    // current.Oversize = this.App.Oversize
                    this.ProfileManager.Current.Model = this.App.Model.ModelNumber;
                    this.ProfileManager.Current.ExtendedMode = this.App.Model.Extended;
                    this.ProfileManager.Current.ColorMode = this.App.Model.Color;
                }

                if (this.App.Oversize != null)
                {
                    if (this.App.Oversize.Columns == 0 && this.App.Oversize.Rows == 0)
                    {
                        this.ProfileManager.Current.Oversize = new Profile.OversizeClass { Columns = 0, Rows = 0 };
                    }
                    else
                    {
                        var model = this.App.ModelsDb.Models[this.ProfileManager.Current.Model];
                        void OversizeError(string message)
                        {
                            ErrorBox.Show(
                                $"Ignoring invalid {Constants.Option.Oversize} '{this.App.Oversize}': {message}",
                                "wx3270 Command Line Error",
                                MessageBoxIcon.Warning);
                        }

                        if (this.App.Oversize.Columns < model.Columns || this.App.Oversize.Rows < model.Rows)
                        {
                            OversizeError($"smaller than model {this.ProfileManager.Current.Model} dimensions");
                        }
                        else if (!this.ProfileManager.Current.ExtendedMode)
                        {
                            OversizeError($"extended data stream mode disabled");
                        }
                        else
                        {
                            this.ProfileManager.Current.Oversize = new Profile.OversizeClass { Columns = this.App.Oversize.Columns, Rows = this.App.Oversize.Rows };
                        }
                    }
                }

                this.ProfileManager.Save();
            }

            // Push out the initial profile.
            this.ProfileManager.PushFirst();

            // Display the main screen window.
            this.Show();

            // Set up command-line host connection.
            var alreadyConnecting = false;
            if (this.App.CommandLineB3270HostSpec != null)
            {
                HostEntry hostEntry;
                if (this.App.Connection != null)
                {
                    // They specified a connection name as well. Use that to locate the entry.
                    hostEntry = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(this.App.Connection, StringComparison.InvariantCultureIgnoreCase));
                    if (hostEntry != null)
                    {
                        // Modify the entry with the host spec.
                        hostEntry = new HostEntry(hostEntry, this.App.CommandLineB3270HostSpec, this.App.HostPrefix.Prefixes) { Name = this.App.Connection };
                        this.ProfileManager.PushAndSave(
                            current =>
                            {
                                current.Hosts = current.Hosts.Select(host => host.Name.Equals(this.App.Connection) ? hostEntry : host).ToList();
                            },
                            I18n.Get(SaveType.CommandLineHost));
                    }
                    else
                    {
                        // Create a new entry by that name, using the host spec.
                        hostEntry = new HostEntry(this.App.CommandLineB3270HostSpec, this.App.HostPrefix.Prefixes) { Name = this.App.Connection, Profile = this.ProfileManager.Current };
                        this.ProfileManager.PushAndSave(
                            current =>
                            {
                                current.Hosts = current.Hosts.Concat(new[] { hostEntry });
                            },
                            I18n.Get(SaveType.CommandLineHost));
                    }
                }
                else
                {
                    // No connection specified. Look for a match.
                    hostEntry = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name == HostEntry.AutoName(this.App.CommandLineB3270HostSpec));
                    if (hostEntry != null)
                    {
                        // Modify the entry with the host spec.
                        hostEntry = new HostEntry(hostEntry, this.App.CommandLineB3270HostSpec, this.App.HostPrefix.Prefixes) { Name = hostEntry.Name };
                        this.ProfileManager.PushAndSave(
                            current =>
                            {
                                current.Hosts = current.Hosts.Select(host => host.Name.Equals(hostEntry.Name) ? hostEntry : host).ToList();
                            },
                            I18n.Get(SaveType.CommandLineHost));
                    }
                    else
                    {
                        // Create a new entry, using the host spec.
                        hostEntry = new HostEntry(this.App.CommandLineB3270HostSpec, this.App.HostPrefix.Prefixes)
                        {
                            Name = HostEntry.AutoName(this.App.CommandLineB3270HostSpec),
                            Profile = this.ProfileManager.Current,
                        };
                        this.ProfileManager.PushAndSave(
                            current =>
                            {
                                current.Hosts = current.Hosts.Concat(new[] { hostEntry });
                            },
                            I18n.Get(SaveType.CommandLineHost));
                    }
                }

                if (hostEntry.InvalidPrefixes != null)
                {
                    var invalidPrefixes = string.Join(", ", hostEntry.InvalidPrefixes.Select(c => new string(new[] { c, ':' })));
                    ErrorBox.Show(
                        string.Format("{0}: {1}", I18n.Get(ErrorMessage.InvalidPrefixes), invalidPrefixes),
                        I18n.Get(Title.Connect),
                        MessageBoxIcon.Warning);
                }

                this.Connect.ConnectToHost(hostEntry);
                alreadyConnecting = true;
            }

            // Set up command-line auto-connect.
            if (!this.App.EditMode && !alreadyConnecting)
            {
                HostEntry autoConnectHost = null;
                if (this.App.Connection != null)
                {
                    autoConnectHost = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(this.App.Connection, StringComparison.InvariantCultureIgnoreCase));
                    if (autoConnectHost == null)
                    {
                        ErrorBox.Show(string.Format("{0}: {1}", I18n.Get(ErrorMessage.NoSuchConnection), this.App.Connection), I18n.Get(Title.Connect), MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    autoConnectHost = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.AutoConnect == AutoConnect.Connect || h.AutoConnect == AutoConnect.Reconnect);
                }

                if (autoConnectHost != null)
                {
                    this.Connect.ConnectToHost(autoConnectHost);
                }
            }
        }

        /// <summary>
        /// Clone a context menu strip to a tool strip menu item.
        /// </summary>
        /// <param name="from">Context menu strip to clone.</param>
        /// <param name="to">Tool strip menu item to attach it to.</param>
        /// <param name="handler">Event handler for copied menu items.</param>
        private void CloneSubmenu(ToolStripMenuItem from, ToolStripMenuItem to, EventHandler handler)
        {
            foreach (var item in from.DropDownItems)
            {
                var fromItem = (ToolStripMenuItem)item;
                var toItem = new ToolStripMenuItem()
                {
                    Name = fromItem.Name,
                    Size = fromItem.Size,
                    Tag = fromItem.Tag,
                    Text = fromItem.Text,
                    ForeColor = fromItem.ForeColor,
                    Image = fromItem.Image,
                    Enabled = fromItem.Enabled,
                };
                toItem.Click += new EventHandler(handler);
                to.DropDownItems.Add(toItem);
                if (fromItem.DropDownItems.Count > 0)
                {
                    this.CloneSubmenu(fromItem, toItem, handler);
                }
            }
        }

        /// <summary>
        /// Clone a context menu strip to a tool strip menu item.
        /// </summary>
        /// <param name="from">Context menu strip to clone.</param>
        /// <param name="to">Tool strip menu item to attach it to.</param>
        /// <param name="handler">Event handler for copied menu items.</param>
        private void CloneMenu(ContextMenuStrip from, ToolStripMenuItem to, EventHandler handler)
        {
            foreach (var item in from.Items)
            {
                if (item is ToolStripMenuItem fromItem)
                {
                    var toItem = new ToolStripMenuItem()
                    {
                        Name = fromItem.Name,
                        Size = fromItem.Size,
                        Tag = fromItem.Tag,
                        Text = fromItem.Text,
                        ForeColor = fromItem.ForeColor,
                        Image = fromItem.Image,
                        Enabled = fromItem.Enabled,
                    };
                    toItem.Click += new EventHandler(handler);
                    to.DropDownItems.Add(toItem);
                    if (fromItem.DropDownItems.Count > 0)
                    {
                        this.CloneSubmenu(fromItem, toItem, handler);
                    }
                }
                else if (item is ToolStripSeparator)
                {
                    to.DropDownItems.Add(new ToolStripSeparator());
                }
            }
        }

        /// <summary>
        /// Clone the menu bar context menus to the main context menu.
        /// </summary>
        private void CloneMenus()
        {
            // Clone the menus.
            if (!this.App.NoButtons)
            {
                this.CloneMenu(this.actionsMenuStrip, this.actionsToolStripMenuItem, this.ActionsClick);
                this.CloneMenu(this.connectMenuStrip, this.connectToolStripMenuItem, this.ConnectToProfileHost);
            }

            this.CloneMenu(this.keypadContextMenuStrip, this.keypadToolStripMenuItem, this.KeypadMenuClick);

            if (!this.App.NoButtons)
            {
                this.CloneMenu(this.loadContextMenuStrip, this.profilesToolStripMenuItem, this.LoadProfileHandler);
                this.CloneMenu(this.macrosContextMenuStrip, this.macrosToolStripMenuItem, this.RunMacro);
            }
        }

        /// <summary>
        /// Find a menu by name.
        /// </summary>
        /// <param name="items">Items to search.</param>
        /// <param name="name">Menu name.</param>
        /// <returns>Menu item, or null.</returns>
        private ToolStripMenuItem FindMenu(ToolStripItemCollection items, string name)
        {
            ToolStripMenuItem menuItem = null;
            foreach (var suffix in new string[] { string.Empty, "1", "2" })
            {
                menuItem = items.OfType<ToolStripMenuItem>().Where(item => item.Name.Equals(name + suffix, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (menuItem != null)
                {
                    return menuItem;
                }
            }

            foreach (var submenu in items.OfType<ToolStripMenuItem>())
            {
                menuItem = this.FindMenu(submenu.DropDownItems, name);
                if (menuItem != null)
                {
                    return menuItem;
                }
            }

            return null;
        }

        /// <summary>
        /// Update the context menu entries with key mappings.
        /// </summary>
        /// <param name="items">Drop-down items to search.</param>
        /// <param name="name">Item name to find.</param>
        /// <param name="action">Action to search for in the keymap.</param>
        private void UpdateContextKeyMapping(ToolStripItemCollection items, string name, string action)
        {
            var menuItem = this.FindMenu(items, name);
            if (menuItem != null)
            {
                var keymaps = this.FindKeymaps(action);
                menuItem.ShortcutKeyDisplayString = (keymaps.Count() != 0) ? KeyMap<KeyboardMap>.DecodeKeyName(keymaps.First().Key) : string.Empty;
            }
        }

        /// <summary>
        /// Update the context menu entries with key mappings.
        /// </summary>
        private void UpdateMenuKeyMappings()
        {
            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "fullScreenToolStripMenuItem", Constants.Action.FullScreen + "()");
            this.UpdateContextKeyMapping(this.editToolStripMenuItem.DropDownItems, "copyToolStripMenuItem", Constants.Action.Copy + "()");
            this.UpdateContextKeyMapping(this.editToolStripMenuItem.DropDownItems, "pasteToolStripMenuItem", Constants.Action.Paste + "()");
            this.UpdateContextKeyMapping(this.editToolStripMenuItem.DropDownItems, "cutToolStripMenuItem", Constants.Action.Cut + "()");
            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "biggerToolStripMenuItem", Constants.Action.StepEfont + $"({Constants.Misc.Bigger})");
            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "smallerToolStripMenuItem", Constants.Action.StepEfont + $"({Constants.Misc.Smaller})");
            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "quitToolStripMenuItem", B3270.Action.Quit + "(-force)");
            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "exitWx3270ToolStripMenuItem", B3270.Action.Quit + "(-force)");
            this.UpdateContextKeyMapping(this.actionsMenuStrip.Items, "exitWx3270ToolStripMenuItem", B3270.Action.Quit + "(-force)");
            for (var i = 1; i <= 24; i++)
            {
                this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, $"pF{i}ToolStripMenuItem", B3270.Action.PF + $"({i})");
                this.UpdateContextKeyMapping(this.keypadContextMenuStrip.Items, $"pF{i}ToolStripMenuItem", B3270.Action.PF + $"({i})");
            }

            for (var i = 1; i <= 2; i++)
            {
                this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, $"pA{i}ToolStripMenuItem", B3270.Action.PA + $"({i})");
                this.UpdateContextKeyMapping(this.keypadContextMenuStrip.Items, $"pA{i}ToolStripMenuItem", B3270.Action.PA + $"({i})");
            }

            foreach (var action in new string[]
                {
                    B3270.Action.Up, B3270.Action.Down, B3270.Action.Left, B3270.Action.Right, B3270.Action.Home, B3270.Action.Tab, B3270.Action.BackTab, B3270.Action.Newline,
                    B3270.Action.Reset, B3270.Action.Enter, B3270.Action.EraseInput, B3270.Action.CursorSelect, B3270.Action.Clear, B3270.Action.EraseEOF, B3270.Action.Dup,
                    B3270.Action.FieldMark, B3270.Action.Attn, B3270.Action.SysReq, B3270.Action.Delete,
                })
            {
                this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, action + "ToolStripMenuItem", action + "()");
                this.UpdateContextKeyMapping(this.keypadContextMenuStrip.Items, action + "ToolStripMenuItem", action + "()");
            }

            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "insertToolStripMenuItem", B3270.Action.Toggle + "(" + B3270.Setting.InsertMode + ")");
            this.UpdateContextKeyMapping(this.keypadContextMenuStrip.Items, "insertToolStripMenuItem", B3270.Action.Toggle + "(" + B3270.Setting.InsertMode + ")");

            this.UpdateContextKeyMapping(this.screenBoxContextMenuStrip.Items, "temporaryToolStripMenuItem", Constants.Action.MenuBar + "()");
        }

        /// <summary>
        /// Someone canceled a chord.
        /// </summary>
        private void ChordReset()
        {
            this.chordTimer.Stop();
            this.oiaCx.Text = string.Empty;
            this.App.ChordName = null;
        }

        /// <summary>
        /// Chord pass-through action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Immediate result.</param>
        /// <param name="tag">Asynchronous result tag.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult Chord(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            var argList = arguments.ToList();
            if (argList.Count() != 1)
            {
                result = "Chord requires 1 argument";
                return PassthruResult.Failure;
            }

            this.App.ChordName = argList[0];
            this.oiaCx.Text = "C…";

            // Clear the chord after 6 seconds.
            this.chordTimer.Start();

            result = string.Empty;
            return PassthruResult.Success;
        }

        /// <summary>
        /// PrintText pass-through action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Immediate result.</param>
        /// <param name="tag">Asynchronous result tag.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult PrintText(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = string.Empty;
            if (this.App.Restricted(Restrictions.Printing))
            {
                return PassthruResult.Success;
            }

            BackEndAction action;
            if (arguments.Count() > 0)
            {
                action = new BackEndAction(B3270.Action.PrintText, arguments);
            }
            else
            {
                action = new BackEndAction(B3270.Action.PrintText, B3270.Value.Gdi, B3270.Value.Dialog);
            }

            this.BackEnd.RunAction(action, Wx3270.BackEnd.Ignore());
            return PassthruResult.Success;
        }

        /// <summary>
        /// Event handler for load profile menu clicks.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LoadProfileHandler(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var p = (ProfileWatchNode)item.Tag;
            if (this.App.ConnectionState != ConnectionState.NotConnected)
            {
                ProfileTree.NewWindow(this, this.components, p.PathName, this.App);
            }
            else if (ModifierKeys.HasFlag(Keys.Shift) || !this.ProfileManager.IsCurrentPathName(p.PathName))
            {
                this.ProfileTree.LoadWithAutoConnect(p.PathName, ModifierKeys.HasFlag(Keys.Shift));
            }
        }

        /// <summary>
        /// Add items to one of the Connect menus.
        /// </summary>
        /// <param name="tree">Tree of watch nodes.</param>
        /// <param name="menuItems">Menu item collection to add to.</param>
        private void AddToConnectMenu(WatchNode tree, ToolStripItemCollection menuItems)
        {
            // Add to the connect menu.
            var hasSeparator = menuItems.OfType<ToolStripItem>().Any(item => item is ToolStripSeparator);
            var connectStack = new Stack<ToolStripItemCollection>();
            connectStack.Push(menuItems);
            tree.ForEach(connectStack, (node, stack) =>
            {
                switch (node.Type)
                {
                    case WatchNodeType.Folder:
                    case WatchNodeType.Profile:
                        if (this.App.Restricted(Restrictions.SwitchProfile))
                        {
                            // We can only list the current profile in the current folder.
                            if (node.Type == WatchNodeType.Folder)
                            {
                                if (!node.Any((n) =>
                                {
                                    if (n.Type == WatchNodeType.Profile)
                                    {
                                        return this.ProfileManager.IsCurrentPathName(((ProfileWatchNode)n).PathName);
                                    }

                                    return false;
                                }))
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                if (!this.ProfileManager.IsCurrentPathName(((ProfileWatchNode)node).PathName))
                                {
                                    return null;
                                }
                            }
                        }

                        // If this is the seed profile directory, return its parent. (Why?)
                        var folder = node as FolderWatchNode;
                        if (folder != null && folder.PathName == Wx3270.ProfileManager.SeedProfileDirectory)
                        {
                            return stack.Peek();
                        }

                        // If this is the current profile, skip it.
                        var p = node as ProfileWatchNode;
                        if (p != null && this.ProfileManager.IsCurrentPathName(p.PathName))
                        {
                            return null;
                        }

                        var name = stack.Count == 1 ? ProfileTree.DirNodeName(folder.PathName) : node.Name;
                        var nonterminal = new ToolStripMenuItem() { Text = name };
                        nonterminal.DropDown = new ContextMenuStrip { ShowImageMargin = false, ShowCheckMargin = false };
                        nonterminal.ToolTipText = (p != null && !string.IsNullOrWhiteSpace(p.Profile.Description)) ? p.Profile.Description : string.Empty;
                        if (stack.Peek() == menuItems && !hasSeparator)
                        {
                            stack.Peek().Add(new ToolStripSeparator());
                            hasSeparator = true;
                        }

                        stack.Peek().Add(nonterminal);
                        return nonterminal.DropDownItems;
                    case WatchNodeType.Host:
                        var profile = (node.Parent as ProfileWatchNode).Profile;
                        var host = node as HostWatchNode;
                        var item = new ToolStripMenuItem(host.Name, null, this.ConnectToOtherHost);
                        if (!string.IsNullOrWhiteSpace(host.HostEntry.Description))
                        {
                            item.ToolTipText = host.HostEntry.Description;
                        }

                        item.Tag = new Tuple<string, string>(profile.PathName, node.Name);
                        stack.Peek().Add(item);
                        return null;
                    default:
                        return null;
                }
            });
        }

        /// <summary>
        /// Add unique menu items for a given folder root.
        /// </summary>
        /// <param name="root">Folder root.</param>
        /// <param name="stripPrefix">Optional prefix to strip from display.</param>
        private void AddUniqueItems(FolderWatchNode root, string stripPrefix = null)
        {
            // Clone the tree.
            var tree = root.CloneTree();

            // Prune broken profiles.
            while (tree.Any((node) =>
            {
                if (node.Type == WatchNodeType.Profile && (node as ProfileWatchNode).Broken)
                {
                    node.Unlink();
                    return true;
                }

                return false;
            }))
            {
            }

            // If there are no profiles left, do nothing.
            if (!tree.Any((node) => node.Type == WatchNodeType.Profile))
            {
                // None anywhere.
                return;
            }

            // Prune folders that have no profiles.
            while (tree.Any((node) =>
            {
                if (node.Type != WatchNodeType.Host
                    && !node.Any((child) => child.Type == WatchNodeType.Profile))
                {
                    node.Unlink();
                    return true;
                }

                return false;
            }))
            {
            }

            // Add to the profile load menu.
            if (this.App.Allowed(Restrictions.SwitchProfile))
            {
                var loadStack = new Stack<ToolStripMenuItem>();
                loadStack.Push(this.loadMenuItem);
                tree.ForEach(loadStack, (node, stack) =>
                {
                    switch (node.Type)
                    {
                        case WatchNodeType.Folder:
                            var folder = (FolderWatchNode)node;
                            if (folder.PathName == Wx3270.ProfileManager.SeedProfileDirectory)
                            {
                                return stack.Peek();
                            }

                            var name = stack.Count == 1 ? ProfileTree.DirNodeName(folder.PathName) : node.Name;
                            var nonterminal = new ToolStripMenuItem() { Text = name };

                            // Explicitly set the DropDown with a ContextMenuStip, otherwise it is impossible to contol ShowImageMargin. Strange.
                            nonterminal.DropDown = new ContextMenuStrip { ShowImageMargin = false, ShowCheckMargin = false };
                            stack.Peek().DropDownItems.Add(nonterminal);
                            return nonterminal;
                        case WatchNodeType.Profile:
                            var profile = node as ProfileWatchNode;
                            var item = new ToolStripMenuItem() { Text = profile.Name, Tag = profile };
                            item.Click += this.LoadProfileHandler;
                            stack.Peek().DropDownItems.Add(item);
                            return null;
                        case WatchNodeType.Host:
                        default:
                            return null;
                    }
                });
            }

            // If there are no hosts, do nothing.
            if (!tree.Any((node) => node.Type == WatchNodeType.Host))
            {
                // None anywhere.
                return;
            }

            // Prune profiles without hosts.
            while (tree.Any((node) =>
            {
                if (!node.Any((child) => child.Type == WatchNodeType.Host))
                {
                    node.Unlink();
                    return true;
                }

                return false;
            }))
            {
            }

            // Add to the connect menus.
            this.AddToConnectMenu(tree, this.connectMenuStrip.Items);
            this.AddToConnectMenu(tree, this.connectToolStripMenuItem.DropDownItems);
        }

        /// <summary>
        /// The list of profiles and hosts changed.
        /// </summary>
        /// <param name="newFolderTree">New host tree.</param>
        private void ProfileTreeChanged(List<FolderWatchNode> newFolderTree)
        {
            void PopulateConnectMenu(ToolStripItemCollection items)
            {
                // Remove all of the items except the fixed ones.
                // The connect menu has fixed items, followed optionally by a separator and the defined connections.
                var itemsList = items.OfType<ToolStripItem>().ToList();
                var sawSeparator = false;
                foreach (var item in itemsList)
                {
                    if (!sawSeparator)
                    {
                        sawSeparator = item is ToolStripSeparator;
                    }
                    else
                    {
                        items.Remove(item);
                    }
                }

                // Start with the current profile's hosts.
                foreach (var host in this.ProfileManager.Current.Hosts)
                {
                    if (!sawSeparator)
                    {
                        items.Add(new ToolStripSeparator());
                        sawSeparator = true;
                    }

                    var item = new ToolStripMenuItem(host.Name, null, this.ConnectToProfileHost) { Tag = host };
                    if (!string.IsNullOrWhiteSpace(host.Description))
                    {
                        item.ToolTipText = host.Description;
                    }

                    items.Add(item);
                }
            }

            this.loadMenuItem.DropDownItems.Clear();
            PopulateConnectMenu(this.connectMenuStrip.Items);
            if (!this.App.NoButtons)
            {
                PopulateConnectMenu(this.connectToolStripMenuItem.DropDownItems);
            }

            // Next comes any folder that is under MyDocuments\wx3270 (ProfileManager.SeedProfileDirectory), with that prefix removed.
            var seedRoot = newFolderTree.Where(p => p.PathName == Wx3270.ProfileManager.SeedProfileDirectory).FirstOrDefault();
            if (seedRoot != null)
            {
                this.AddUniqueItems(seedRoot, Wx3270.ProfileManager.SeedProfileDirectory);
            }

            // Finally any folders outside of these.
            foreach (var root in newFolderTree.Where(p => p.PathName != Wx3270.ProfileManager.SeedProfileDirectory))
            {
                this.AddUniqueItems(root);
            }

            // Re-clone the drop-downs.
            var firstItem = this.profilesToolStripMenuItem.DropDownItems[0];
            this.profilesToolStripMenuItem.DropDownItems.Clear();
            this.profilesToolStripMenuItem.DropDownItems.Add(firstItem);
            this.profilesToolStripMenuItem.DropDownItems[0].Click += this.ProfilePictureBox_Click;
            if (!this.App.NoButtons)
            {
                this.CloneMenu(this.loadContextMenuStrip, this.profilesToolStripMenuItem, this.LoadProfileHandler);
            }
        }

        /// <summary>
        /// Connect to a profile host from the context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectToProfileHost(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripMenuItem;
            if (clickedMenuItem.Tag is HostEntry hostEntry)
            {
                // Connect to a particular host.
                if (this.App.ConnectionState != ConnectionState.NotConnected)
                {
                    ProfileTree.NewWindow(this, this.components, hostEntry.Profile.PathName, this.App, hostEntry.Name);
                }
                else
                {
                    this.Connect.ConnectToHost(hostEntry);
                }
            }
            else if (clickedMenuItem.Tag is string tagString)
            {
                // Quick connect or disconnect.
                switch (tagString)
                {
                    case "QuickConnect":
                        this.ProfileTree.CreateHostDialog(this.ProfileManager.Current, fromInside: false);
                        break;
                    case "Disconnect":
                        this.Connect.Disconnect();
                        break;
                }
            }
        }

        /// <summary>
        /// Connect to a host in a different profile from the context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectToOtherHost(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripMenuItem;
            var hostNode = clickedMenuItem.Tag as Tuple<string, string>;

            if (this.App.ConnectionState != ConnectionState.NotConnected)
            {
                ProfileTree.NewWindow(this, this.components, hostNode.Item1, this.App, hostNode.Item2);
                return;
            }

            // Switch to that profile.
            if (this.ProfileManager.Load(hostNode.Item1))
            {
                // Connect to the host.
                var hostEntry = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(hostNode.Item2));
                if (hostEntry != null)
                {
                    this.Connect.ConnectToHost(hostEntry);
                }
            }
        }

        /// <summary>
        /// The profile changed.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileChanged(Profile oldProfile, Profile newProfile)
        {
            // Update the load menu.
            foreach (var item in this.loadMenuItem.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Enabled = !item.Text.Equals(newProfile.Name);
                item.Checked = item.Text.Equals(newProfile.Name);
            }

            // Update the title.
            this.Text = NewTitle(null, newProfile, null);

            // Update the connect menu, a side-effect of profile tree processing.
            this.ProfileTreeChanged(this.App.ProfileTracker.Tree);

            // Update the font.
            if (oldProfile == null || !oldProfile.Font.Equals(newProfile.Font))
            {
                this.Refont(newProfile.Font.Font());
            }

            // Update menus that include keyboard mappings.
            if (oldProfile == null || !oldProfile.KeyboardMap.Equals(newProfile.KeyboardMap))
            {
                this.UpdateMenuKeyMappings();
            }

            // For non-full profiles, no quick connect.
            this.quickConnectMenuItem.Enabled = newProfile.ProfileType == ProfileType.Full;

            // Redraw the main screen.
            if (oldProfile == null || !oldProfile.Colors.Equals(newProfile.Colors) || oldProfile.ColorMode != newProfile.ColorMode)
            {
                this.Recolor(newProfile.Colors, newProfile.ColorMode);
            }

            // Update macros. It's not worth setting up a separate crossbar for this.
            this.macroEntries.Entries = newProfile.Macros;
        }

        /// <summary>
        /// The macros list changed.
        /// </summary>
        private void MacrosChanged()
        {
            void RedoMenu(ToolStripItemCollection items, bool saveFirst = false)
            {
                var first = saveFirst ? items[0] : null;
                items.Clear();
                if (first != null)
                {
                    items.Add(first);
                }

                items.AddRange(
                    this.macroEntries.Entries.Select(e => new ToolStripMenuItem(e.Name, null, this.RunMacro) { Tag = e }).ToArray());
                var item = new ToolStripMenuItem(
                    I18n.Get(this.MacroRecorder.Running ? MacroStopRecordingItemName : MacroRecordingItemName),
                    this.MacroRecorder.Running ? Properties.Resources.stop_recording : Properties.Resources.record1,
                    this.RunMacro)
                { Tag = "ToggleRecording" };
                items.Add(item);
                this.macroRecordItems.Add(item);
            }

            this.macroRecordItems.Clear();
            RedoMenu(this.macrosContextMenuStrip.Items);
            RedoMenu(this.macrosToolStripMenuItem.DropDownItems, saveFirst: true);
        }

        /// <summary>
        /// Run a macro from the context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RunMacro(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripMenuItem;
            if (clickedMenuItem.Tag is MacroEntry entry)
            {
                this.BackEnd.RunActions(entry.Macro, ErrorBox.Completion(I18n.Get(Title.MacroError)));
            }
            else if (clickedMenuItem.Tag is string tagString && tagString == "ToggleRecording")
            {
                this.ToggleRecording(sender, e);
            }
        }

        /// <summary>
        /// Process a macro recorder state change.
        /// </summary>
        /// <param name="running">True if recorder is running.</param>
        /// <param name="abort">True if recording was aborted.</param>
        private void OnMacroRecorderState(bool running, bool abort)
        {
            foreach (var item in this.macroRecordItems)
            {
                item.Text = running ? I18n.Get(MacroStopRecordingItemName) : I18n.Get(MacroRecordingItemName);
                item.Image = running ? Properties.Resources.stop_recording : Properties.Resources.record1;
            }

            this.toolTip1.SetToolTip(this.macrosPictureBox, running ? I18n.Get(MacroRecordingToolTipName) : I18n.Get(MacrosToolTipName));

            if (!running && abort)
            {
                ErrorBox.Show(I18n.Get(ErrorMessage.RecordingDiscarded), I18n.Get(Title.MacroError), MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Process a macro recorder flash event.
        /// </summary>
        /// <param name="flashing">True if flashing on.</param>
        private void OnFlashEvent(bool flashing)
        {
            this.macrosPictureBox.Image = flashing ? Properties.Resources.Tape4_flash : Properties.Resources.Tape4;
        }

        /// <summary>
        /// Start or stop recording a macro.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ToggleRecording(object sender, EventArgs e)
        {
            if (this.MacroRecorder.Running)
            {
                this.MacroRecorder.Stop();
            }
            else
            {
                this.MacroRecorder.Start(this.RecordingComplete);
            }
        }

        /// <summary>
        /// Macro recording is complete.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="context">Context object.</param>
        private void RecordingComplete(string text, object context)
        {
            if (!string.IsNullOrEmpty(text))
            {
                this.Macros.Record(text);
            }
        }

        /// <summary>
        /// Process a screen update.
        /// </summary>
        /// <param name="updateType">Update type.</param>
        /// <param name="updateState">Update state.</param>
        private void ProcessUpdate(ScreenUpdateType updateType, UpdateState updateState)
        {
            switch (updateType)
            {
                case ScreenUpdateType.Screen:
                    this.ScreenNeedsDrawing(updateState.ScreenImage, "update", false);
                    break;
                case ScreenUpdateType.Repaint:
                    this.ScreenNeedsDrawing(updateState.ScreenImage, "repaint", true);
                    break;
                case ScreenUpdateType.Lock:
                    this.ChangeOiaLock(updateState.OiaState);
                    break;
                case ScreenUpdateType.Insert:
                    this.ChangeOiaInsert(updateState.OiaState);
                    break;
                case ScreenUpdateType.Ssl:
                    this.ChangeOiaTls(updateState.OiaState);
                    this.SslEvent();
                    break;
                case ScreenUpdateType.LuName:
                    this.ChangeOiaLu(updateState.OiaState);
                    this.LuEvent();
                    break;
                case ScreenUpdateType.Timing:
                    this.ChangeOiaTiming(updateState.OiaState);
                    break;
                case ScreenUpdateType.Cursor:
                    this.ScreenNeedsDrawing(updateState.ScreenImage, "cursor", false);
                    break;
                case ScreenUpdateType.OiaCursor:
                    this.ChangeOiaCursor(updateState.OiaState);
                    break;
                case ScreenUpdateType.Connection:
                    this.ChangeOiaTls(updateState.OiaState);
                    this.ChangeOiaNetwork(updateState.OiaState);
                    this.ChangeOiaLock(updateState.OiaState);
                    this.ConnectionStateEvent();
                    break;
                case ScreenUpdateType.Network:
                    this.ChangeOiaNetwork(updateState.OiaState);
                    break;
                case ScreenUpdateType.ScreenMode:
                    this.ChangeScreenMode(updateState.ScreenImage);
                    this.ScreenNeedsDrawing(updateState.ScreenImage, "mode", true);
                    this.ModelBackEndToProfile();
                    break;
                case ScreenUpdateType.Thumb:
                    this.ChangeThumb();
                    break;
                case ScreenUpdateType.ScreenTrace:
                    this.ChangeOiaScreenTrace(updateState.OiaState);
                    break;
                case ScreenUpdateType.PrinterSession:
                    this.ChangeOiaPrinterSession(updateState.OiaState);
                    break;
                case ScreenUpdateType.Typeahead:
                    this.ChangeOiaTypeahead(updateState.OiaState);
                    break;
                case ScreenUpdateType.TraceFile:
                    this.ChangeTraceFile();
                    break;
                case ScreenUpdateType.Script:
                    this.ChangeScript(updateState.OiaState);
                    break;
                case ScreenUpdateType.Scroll:
                    this.ScreenNeedsDrawing(updateState.ScreenImage, "scroll", false);
                    break;
                case ScreenUpdateType.ReverseInput:
                    this.ChangeReverseInput(updateState.OiaState);
                    break;
            }
        }

        /// <summary>
        /// Repaint the OIA fields with a new font.
        /// </summary>
        /// <param name="font">New font.</param>
        private void RefontOia(Font font)
        {
            var try3270Font = font;
            if (VersionSpecific.SupportsPua)
            {
                // Find a 3270 font with the same metrics.
                if (font.Name != FontProfile.Name3270Font && font.Name != FontProfile.Name3270FontRb)
                {
                    var tryFontName = FontProfile.Name3270FontRb;
                    try3270Font = new Font(tryFontName, font.Size);
                    if (try3270Font.Name != tryFontName)
                    {
                        tryFontName = FontProfile.Name3270Font;
                        try3270Font = new Font(tryFontName, font.Size);
                    }

                    if (try3270Font.Name == tryFontName)
                    {
                        this.oia3270Font = true;

                        // Set up the cell measurements.
                        using (Graphics g = this.CreateGraphics())
                        {
                            var mainCellSize = TextRenderer.MeasureText(g, "X", font, new Size(1000, 1000), TextFormatFlags.Left | TextFormatFlags.NoPadding);
                            var xCellSize = TextRenderer.MeasureText(g, "X", try3270Font, new Size(1000, 1000), TextFormatFlags.Left | TextFormatFlags.NoPadding);
                            if (xCellSize.Width > mainCellSize.Width)
                            {
                                var mainRatio = (float)mainCellSize.Width / (float)mainCellSize.Height;
                                var xRatio = (float)xCellSize.Width / (float)mainCellSize.Height;
                                var newSize = RoundDown(font.Size * mainRatio / xRatio, 0.25F);
                                try3270Font.Dispose();
                                Trace.Line(Trace.Type.Window, "Trying new OIA 3270 font {0} (vs. {1})", newSize, font.Size, mainRatio, xRatio);
                                try3270Font = new Font(tryFontName, newSize);

                                xCellSize = TextRenderer.MeasureText(g, "X", try3270Font, new Size(1000, 1000), TextFormatFlags.Left | TextFormatFlags.NoPadding);
                                if (xCellSize.Width > mainCellSize.Width)
                                {
                                    Trace.Line(Trace.Type.Window, " (Too big!)");
                                }
                            }
                        }
                    }
                    else
                    {
                        try3270Font = font;
                        this.oia3270Font = false;
                    }
                }
                else
                {
                    this.oia3270Font = true;
                }
            }
            else
            {
                this.oia3270Font = false;
            }

            // Apply it to the OIA.
            foreach (var oiaField in this.oiaLayoutPanel.Controls.OfType<Control>())
            {
                if (oiaField.Tag != null && (string)oiaField.Tag == "Main")
                {
                    oiaField.Font = font;
                }
                else
                {
                    oiaField.Font = try3270Font;
                }
            }

            // Do the lock fields explicitly.
            this.oiaLock.Font = try3270Font;
            this.oiaLockNative.Font = font;
            this.oiaTiming.Font = try3270Font;
            this.oiaTimingNative.Font = font;
        }

        /// <summary>
        /// Set the 'Checked' property for a pair of menu items.
        /// </summary>
        /// <param name="mainItem">Menu item from main menu bar.</param>
        /// <param name="isChecked">True to set Checked.</param>
        private void SetPairedChecked(ToolStripMenuItem mainItem, bool isChecked)
        {
            mainItem.Checked = isChecked;
            var contextItem = this.FindMenu(this.screenBoxContextMenuStrip.Items, mainItem.Name);
            if (contextItem != null)
            {
                contextItem.Checked = isChecked;
            }
        }

        /// <summary>
        /// Set the 'Enabled' property for a pair of menu items.
        /// </summary>
        /// <param name="mainItem">Menu item from main menu bar.</param>
        /// <param name="isEnabled">True to set Enabled.</param>
        private void SetPairedEnabled(ToolStripMenuItem mainItem, bool isEnabled)
        {
            mainItem.Enabled = isEnabled;
            var contextItem = this.FindMenu(this.screenBoxContextMenuStrip.Items, mainItem.Name);
            if (contextItem != null)
            {
                contextItem.Enabled = isEnabled;
            }
        }

        /// <summary>
        /// A setting changed.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Setting dictionary.</param>
        private void SettingChanged(string settingName, SettingsDictionary settingDictionary)
        {
            switch (settingName)
            {
                case B3270.Setting.Trace:
                    this.SetPairedChecked(this.tracingToolStripMenuItem, settingDictionary.TryGetValue(B3270.Setting.Trace, out bool trace) && trace);
                    break;
                case B3270.Setting.ScreenTrace:
                    var screenTracing = settingDictionary.TryGetValue(B3270.Setting.ScreenTrace, out bool screenTrace) && screenTrace;
                    this.SetPairedChecked(this.screenTracingMenuItem, screenTracing);
                    this.SetPairedEnabled(this.sendToPrinterToolStripMenuItem, !screenTracing);
                    this.SetPairedEnabled(this.saveToFileToolStripMenuItem, !screenTracing);
                    break;
                case B3270.Setting.VisibleControl:
                    this.SetPairedChecked(this.controlCharsMenuItem, settingDictionary.TryGetValue(B3270.Setting.VisibleControl, out bool visibleControl) && visibleControl);
                    this.ScreenNeedsDrawing(this.App.ScreenImage, "visibleControl", true);
                    break;
                case B3270.Setting.AplMode:
                    this.AplChanged(settingDictionary.TryGetValue(B3270.Setting.AplMode, out bool aplMode) && aplMode);
                    break;
            }
        }

        /// <summary>
        /// The trace file changed.
        /// </summary>
        private void ChangeTraceFile()
        {
            var traceFileName = this.App.ScreenImage.TraceFile;
            if (!string.IsNullOrWhiteSpace(traceFileName))
            {
                // Start watching the trace.
                if (this.traceWindow == null)
                {
                    this.traceWindow = new TraceWindow(traceFileName, this.BackEnd);
                    this.BackEnd.OnExit += this.traceWindow.Dispose;
                }
            }
            else
            {
                // Stop watching the trace.
                if (this.traceWindow != null)
                {
                    this.BackEnd.OnExit -= this.traceWindow.Dispose;
                    this.traceWindow.Dispose();
                    this.traceWindow = null;
                }
            }
        }

        /// <summary>
        /// Paint method for the screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_Paint(object sender, PaintEventArgs e)
        {
            // First call the base class OnPaint method so registered delegates receive the event.
            this.OnPaint(e);

            // Draw the screen text.
            this.ScreenDraw(sender, e);

            // Let other screen operations go.
            this.App.DrawComplete();
        }

        /// <summary>
        /// Size change method for the screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_ClientSizeChanged(object sender, EventArgs e)
        {
            // Force a repaint.
            if (this.App != null && this.App.ScreenImage != null)
            {
                this.ScreenNeedsDrawing(this.App.ScreenImage, "sizeChange", true);
            }
        }

        /// <summary>
        /// Size change method for the main screen.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainTable_SizeChanged(object sender, EventArgs e)
        {
            this.vScrollBar1.Height = this.mainTable.Height;
        }

        /// <summary>
        /// Form load method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void X3270_Load(object sender, EventArgs e)
        {
            Trace.Line(Trace.Type.Window, "MainWindow Load");

            if (this.App.FullScreen)
            {
                this.SetFullScreen();
            }
            else if (this.App.Maximize)
            {
                this.Maximize("command-line -maximize");
            }

            // We will not receive a message for initial maximized state, so it has to be checked here.
            if (this.Maximized)
            {
                this.screenBox.Maximize(true, this.ClientSize);
                this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic); // XXX?
            }

            // Okay, we're loaded. Clear the splash screen.
            this.App.Splash.Stop();
        }

        /// <summary>
        /// Mouse button down event handler for the screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.overlayMenuBarDisplayed)
            {
                this.ToggleOverlayMenuBar();
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var (row1, column1) = this.screenBox.CellCoordinates1(e.Location);
            this.App.SelectionManager.MouseDown(row1, column1, ModifierKeys.HasFlag(Keys.Shift), e.Location);
        }

        /// <summary>
        /// Mouse move handler for the screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var (row1, column1) = this.screenBox.CellCoordinates1(e.Location);
            this.App.SelectionManager.MouseMove(this.screenBox, row1, column1, e.Location);
        }

        /// <summary>
        /// Mouse button up event handler for the screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var (row1, column1) = this.screenBox.CellCoordinates1(e.Location);
            this.App.SelectionManager.MouseUp(row1, column1);
        }

        /// <summary>
        /// Key down event handler for the main screen.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_KeyDown(object sender, KeyEventArgs e)
        {
            this.App.KeyHandler.ProcessKeyDown(this, e);
        }

        /// <summary>
        /// Key press handler.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.App.KeyHandler.ProcessKeyPress(e);
        }

        /// <summary>
        /// Key up handler.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_KeyUp(object sender, KeyEventArgs e)
        {
            this.App.KeyHandler.ProcessKeyUp(this, e);
        }

        /// <summary>
        /// Mouse click handler for the actions button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsBox_Click(object sender, EventArgs e)
        {
            if (!this.ActionsDialog.Visible)
            {
                this.ActionsDialog.Show(this);
            }

            this.ActionsDialog.Activate();
        }

        /// <summary>
        /// Mouse enter handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_MouseEnter(object sender, EventArgs e)
        {
            this.App.KeyHandler.Enter(this);
        }

        /// <summary>
        /// Resize handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Resize(object sender, EventArgs e)
        {
            var resizeCount = this.resizeCount++;
            Trace.Line(Trace.Type.Window, $"MainScreen Resize #{resizeCount} state {this.WindowState} Size {this.Size}  ClientSize {this.ClientSize} Location {this.Location}");

            switch (this.WindowState)
            {
                case FormWindowState.Minimized:
                    // Minimized. Hide the keypad.
                    foreach (var pad in this.Keypads)
                    {
                        if (pad != null && pad.Visible)
                        {
                            pad.Hide();
                            this.keypadMinimized.Add(pad);
                        }
                    }

                    break;

                case FormWindowState.Normal:
                case FormWindowState.Maximized:
                    // Restored.
                    if (!this.Maximized && !this.IsWindowArranged())
                    {
                        this.lastLocation = this.Location;
                    }

                    if (this.resizeLocked)
                    {
                        Trace.Line(Trace.Type.Window, $"Resize #{resizeCount} locked, abandoning");
                        return;
                    }

                    this.resizeLocked = true;
                    foreach (var pad in this.Keypads.Where(p => p != null))
                    {
                        if (this.keypadMinimized.Contains(pad))
                        {
                            // Restore the keypad, too.
                            this.noFlashTimer.Start();
                            pad.Show(this);
                            this.keypadMinimized.Remove(pad);
                        }
                    }

                    if (!this.resizeBeginPending &&
                        this.ClientSize != this.mainScreenPanel.Size &&
                        this.screenBox != null &&
                        this.screenBox.ResizeReady)
                    {
                        // Got a Resize on its own (outside of ResizeStart/ResizeEnd), which is more of a "Size" event.
                        // We respect the size and adapt the font size to accommodate it.
                        var isWindowArranged = this.IsWindowArranged();
                        var arrText = isWindowArranged ? "arranged" : string.Empty;
                        Trace.Line(Trace.Type.Window, $" ==> resize {arrText}");
                        this.screenBox.Maximize(this.Maximized || this.IsWindowArranged(), this.ClientSize);
                        if (this.WindowState == FormWindowState.Normal && !this.IsWindowArranged())
                        {
                            // Not maximized, not docked. Restore the font stored in the profile, and snap.
                            Trace.Line(Trace.Type.Window, $"Resize #{resizeCount} un-maximized/undocked -> restore font and snap");
                            this.Refont(this.ProfileManager.Current.Font.Font());
                        }
                        else
                        {
                            var newFont = this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic);
                            var fontProfile = new FontProfile(newFont);
                            Trace.Line(Trace.Type.Window, $"Resize #{resizeCount} after RecomputeFont -> {fontProfile}");
                            if (!isWindowArranged && this.WindowState == FormWindowState.Normal && this.FormBorderStyle != FormBorderStyle.None)
                            {
                                if (this.ProfileManager.PushAndSave(
                                    (current) =>
                                    {
                                        current.Font = fontProfile;
                                    },
                                    I18n.Get(ResizeName)))
                                {
                                    Trace.Line(Trace.Type.Window, $"  Resize #{resizeCount} font change to '{fontProfile}' pushed");
                                }
                            }
                        }
                    }

                    this.resizeLocked = false;
                    break;
            }

            Trace.Line(Trace.Type.Window, $"Resize #{resizeCount} done");
        }

        /// <summary>
        /// Form closing handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            try
            {
                this.AppClose();
            }
            catch (Exception ex)
            {
                this.App.Splash.Stop();
                ErrorBox.ShowCopy(this, ex.ToString(), I18n.Get(CloseName));
                this.BackEnd.Exit(1);
            }
        }

        /// <summary>
        /// Mouse click handler for the settings button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SettingsBox_Click(object sender, EventArgs e)
        {
            if (!this.SettingsDialog.Visible)
            {
                this.SettingsDialog.Show(this);
            }

            this.SettingsDialog.Activate();
        }

        /// <summary>
        /// The main screen was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Activated(object sender, EventArgs e)
        {
            Trace.Line(Trace.Type.Window, "MainScreen Activated");

            this.isActivated = true;

            // Give the main screen panel focus, which has the odd (but desired) effect of allowing
            // the IME to apply to the main screen.
            this.mainScreenPanel.Focus();

            // Start blinking.
            this.screenBox.Activated(true);

            // Tell the selection manager.
            this.App.SelectionManager.Activated();

            // Flash the keypad, so it can be identified.
            foreach (var pad in this.Keypads)
            {
                if (pad.Visible && !this.noFlashTimer.Enabled)
                {
                    Trace.Line(Trace.Type.Window, " ==> flash keypad window");
                    this.activateTimer.Start();
                }
            }

            // Handle the activation.
            this.App.KeyHandler.Activate();

            // Run the tour if the menu bar is not displayed.
            if (!this.MenuBarVisible && !this.toured && !Tour.IsComplete(this))
            {
                this.toured = true;
                new TaskFactory().StartNew(() => this.Invoke(new MethodInvoker(() => this.RunTour())));
            }
        }

        /// <summary>
        /// The main screen was deactivated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Deactivate(object sender, EventArgs e)
        {
            this.isActivated = false;

            if (!this.keypadActive.Any())
            {
                // Stop blinking.
                this.screenBox.Activated(false);
            }
        }

        /// <summary>
        /// The screen mode changed.
        /// </summary>
        /// <param name="image">Screen image.</param>
        /// <returns>True if mode actually changed.</returns>
        private bool ChangeScreenMode(ScreenImage image)
        {
            var modeChanged = this.screenBox.ChangeScreenMode(image);
            if (modeChanged && !this.Maximized)
            {
                // Do an implicit snap.
                this.ClientSize = this.mainScreenPanel.Size;
            }

            return modeChanged;
        }

        /// <summary>
        /// Timer tick for profile errors.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProfileErrorTimer_Tick(object sender, EventArgs e)
        {
            this.profileErrorTimer.Enabled = false;
            this.ProfileManager.DumpErrors();
        }

        /// <summary>
        /// The connect button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectPictureBox_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.connectMenuStrip.Show(this.connectPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// The profile button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProfilePictureBox_Click(object sender, EventArgs e)
        {
            if (!this.ProfileTree.Visible)
            {
                this.ProfileTree.Show(this);
            }

            this.ProfileTree.Activate();
        }

        /// <summary>
        /// The macros button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosPictureBox_Click(object sender, EventArgs e)
        {
            if (this.MacroRecorder.Running)
            {
                this.ToggleRecording(sender, e);
                return;
            }

            if (!this.Macros.Visible)
            {
                this.Macros.Show(this);
            }

            this.Macros.Activate();
        }

        /// <summary>
        /// The main screen has been shown.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Shown(object sender, EventArgs e)
        {
            // Read-only pop-up.
            if (this.readOnlyPopUpPending)
            {
                this.readOnlyPopUpPending = false;
                ErrorBox.ShowWithStop(
                    this.Handle,
                    string.Format(I18n.Get(Settings.Message.ReadOnly), this.ProfileManager.Current.Name, Constants.Option.Detached),
                    I18n.Get(Settings.Title.Settings),
                    Constants.StopKey.ReadOnly);
            }
        }

        /// <summary>
        /// The reset context menu item was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackEnd.RunAction(new BackEndAction(B3270.Action.Reset), Wx3270.BackEnd.Ignore());
        }

        /// <summary>
        /// Change the state of the vertical scrollbar thumb.
        /// </summary>
        private void ChangeThumb()
        {
            var thumb = this.App.ScreenImage.Thumb;
            var maximum = thumb.Screen + thumb.Saved - 1;
            var value = thumb.Saved - thumb.Back;
            var largeChange = thumb.Screen;
            Trace.Line(
                Trace.Type.Window,
                "Thumb: Saved {0} Screen {1} Back {2} -> Maximum {3} Value {4} LargeChange {5}",
                thumb.Saved,
                thumb.Screen,
                thumb.Back,
                maximum,
                value,
                largeChange);
            this.vScrollBar1.Minimum = 0;
            this.vScrollBar1.Maximum = maximum;
            this.vScrollBar1.Value = value;
            this.vScrollBar1.LargeChange = largeChange;
            this.vScrollBar1.SmallChange = 1;
        }

        /// <summary>
        /// The scroll bar was scrolled.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.scrollBarDisplayed && e.Type == ScrollEventType.EndScroll)
            {
                var setValue = this.App.ScreenImage.Thumb.Saved - e.NewValue;
                if (setValue < 0)
                {
                    setValue = 0;
                }

                Trace.Line(Trace.Type.Window, "VScrollBar1_Scroll: new value {0} -> set {1}", e.NewValue, setValue);
                this.BackEnd.RunAction(
                    new BackEndAction(
                        B3270.Action.Scroll,
                        "Set",
                        setValue.ToString()),
                    Wx3270.BackEnd.Ignore());
            }
        }

        /// <summary>
        /// An actions context menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsClick(object sender, EventArgs e)
        {
            string tagString;
            if (sender is Control control)
            {
                tagString = control.Tag as string;
            }
            else if (sender is ToolStripMenuItem menuItem)
            {
                tagString = menuItem.Tag as string;
            }
            else
            {
                return;
            }

            switch (tagString)
            {
                case "Prompt":
                    this.App.Prompt.Start();
                    break;
                case "Tracing":
                    this.ActionsDialog.Tracing = !this.ActionsDialog.Tracing;
                    break;
                case "FileTransfer":
                    this.ActionsDialog.FileTransfer();
                    break;
                case "Printer":
                case "File":
                case "Toggle":
                    this.ActionsDialog.ToggleScreenTracing(tagString);
                    break;
                case "VisibleControl":
                    this.ActionsDialog.VisibleControl = !this.ActionsDialog.VisibleControl;
                    break;
                case "CancelScripts":
                    this.ActionsDialog.CancelScripts();
                    break;
                case "ReEnableKeyboard":
                    this.ActionsDialog.ReenableKeyboard();
                    break;
                case "PrintScreen":
                    this.ActionsDialog.PrintTextButton_Click(sender, e);
                    break;
                case "DisplayKeymap":
                    this.ActionsDialog.DisplayKeymap();
                    break;
                case "Exit":
                    this.BackEnd.Stop();
                    break;
            }
        }

        /// <summary>
        /// The help picture box was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// Paint event for the crosshair picture box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CrosshairPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // First call the base class OnPaint method so registered delegates receive the event.
            this.OnPaint(e);

            // Draw the screen text.
            this.CrosshairDraw(sender, e);
        }

        /// <summary>
        /// Resize begin event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_ResizeBegin(object sender, EventArgs e)
        {
            Trace.Line(Trace.Type.Window, "MainWindow ResizeBegin");
            if (!this.IsWindowArranged())
            {
                this.resizeBeginPending = true;
            }
        }

        /// <summary>
        /// The Undo option was selected from the main screen settings context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Undo(object sender, EventArgs e)
        {
            this.ProfileManager.Undo();
        }

        /// <summary>
        /// The Redo option was selected from the main screen settings context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Redo(object sender, EventArgs e)
        {
            this.ProfileManager.Redo();
        }

        /// <summary>
        /// The Snap button has been clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SnapBox_Click(object sender, EventArgs e)
        {
            if (this.snapTimer.Enabled)
            {
                return;
            }

            this.snapBox.Image = Properties.Resources.ToggleUp4;
            this.snapTimer.Tag = null;
            this.snapTimer.Start();
        }

        /// <summary>
        /// Show the keypad.
        /// </summary>
        /// <param name="apl">True to show the APL keypad.</param>
        private void ShowKeypad(bool apl = false)
        {
            var keypad = apl ? this.AplKeypad : (Form)this.Keypad;
            var keypadLocation = (IInitialLocation)keypad;
            if (!keypad.Visible)
            {
                this.noFlashTimer.Start();
                switch (this.ProfileManager.Current.KeypadPosition)
                {
                    case KeypadPosition.Left:
                        keypadLocation.InitialLocation = new Point(this.Location.X - this.keypad.Width, this.Location.Y);
                        break;
                    case KeypadPosition.Centered:
                        break;
                    case KeypadPosition.Right:
                        keypadLocation.InitialLocation = new Point(this.Location.X + this.Width, this.Location.Y);
                        break;
                }

                keypad.Show(this);
            }
            else
            {
                this.noFlashTimer.Start();
                keypad.Activate();
            }
        }

        /// <summary>
        /// The keypad button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadBox_Click(object sender, EventArgs e)
        {
            this.ShowKeypad(this.Mod.HasFlag(KeyboardModifier.Alt));
        }

        /// <summary>
        /// An item from the keypad menu has been clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadMenuClick(object sender, EventArgs e)
        {
            var action = (sender as ToolStripMenuItem).Tag as string;
            this.BackEnd.RunActions(action, ErrorBox.Completion(I18n.Get(Title.KeypadMenuError)));
        }

        /// <summary>
        /// The Snap button timer has completed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SnapTimer_Tick(object sender, EventArgs e)
        {
            var tag = (string)this.snapTimer.Tag;
            if (string.IsNullOrEmpty(tag))
            {
                this.snapBox.Image = Properties.Resources.ToggleDown4;
                this.snapTimer.Tag = "pending";
                this.snapTimer.Start();
                return;
            }

            this.Snap();
            this.snapTimer.Stop();
            this.snapTimer.Tag = null;
        }

        /// <summary>
        /// The activate timer expired.
        /// </summary
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActivateTimer_Tick(object sender, EventArgs e)
        {
            this.activateTimer.Stop();
            foreach (var pad in this.FlashableKeypads)
            {
                // Is it okay to flash a keypad that isn't visible?
                pad.Flash();
            }
        }

        /// <summary>
        /// The no-flash timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NoFlashTimer_Tick(object sender, EventArgs e)
        {
            this.noFlashTimer.Stop();
        }

        /// <summary>
        /// The screen has been resized.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_ResizeEnd(object sender, EventArgs e)
        {
            var resizeCount = this.resizeCount++;
            Trace.Line(Trace.Type.Window, $"MainWindow ResizeEnd #{resizeCount} WindowState {this.WindowState} Size {this.Size} ClientSize {this.ClientSize} Location {this.Location}");
            if (!this.Maximized && !this.IsWindowArranged())
            {
                this.lastLocation = this.Location;
            }

            if (!this.resizeBeginPending)
            {
                Trace.Line(Trace.Type.Window, $"MainWindow ResizeEnd #{resizeCount} aborting, no ResizeBegin");
                return;
            }

            this.resizeBeginPending = false;

            // Recompute the font, only if the size has changed.
            if (this.ClientSize != this.mainScreenPanel.Size)
            {
                var isWindowArranged = this.IsWindowArranged();
                var arrText = isWindowArranged ? "arranged" : string.Empty;
                Trace.Line(Trace.Type.Window, $" ==> resize {arrText}");
                this.screenBox.Maximize(this.Maximized || this.IsWindowArranged(), this.ClientSize);
                var newFont = this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic);
                if (!this.fullScreen && !isWindowArranged && !this.Maximized)
                {
                    var fontProfile = new FontProfile(newFont);
                    if (this.ProfileManager.PushAndSave(
                        (current) =>
                        {
                            current.Font = fontProfile;
                            if (!this.Maximized)
                            {
                                current.Size = this.Size;
                            }
                        },
                        I18n.Get(ResizeName)))
                    {
                        Trace.Line(Trace.Type.Window, $" ==> ResizeEnd #{resizeCount} resize to '{fontProfile}' pushed");
                    }
                }
            }

            Trace.Line(Trace.Type.Window, $"ResizeEnd #{resizeCount} done");
        }

        /// <summary>
        /// Paint event handler for the start button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsBox_paint(object sender, PaintEventArgs e)
        {
            this.startButton.Render((PictureBox)sender, e, I18n.Get(StartButtonName));

            if (!this.toured && !Tour.IsComplete(this))
            {
                // The menu bar is now the correct size. Start the tour.
                this.toured = true;
                new TaskFactory().StartNew(() => this.Invoke(new MethodInvoker(() => this.RunTour())));
            }
        }

        /// <summary>
        /// Run the tour.
        /// </summary>
        private void RunTour()
        {
            var nodes = new List<(Control, int?, Orientation)>
            {
                (this, 1, Orientation.Centered),
            };
            if (this.MenuBarVisible)
            {
                var stops = new[]
                {
                    ((Control)this.connectPictureBox, (int?)null, Orientation.UpperLeft),
                    (this.actionsBox, null, Orientation.UpperLeft),
                    (this.keypadBox, null, Orientation.UpperLeft),
                    (this.connectPictureBox, 1, Orientation.UpperLeft),
                    (this.profilePictureBox, null, Orientation.UpperLeft),
                    (this.macrosPictureBox, null, Orientation.UpperLeft),
                    (this.snapBox, null, Orientation.UpperLeft),
                    (this.helpPictureBox, null, Orientation.UpperLeft),
                    (this.settingsBox, null, Orientation.UpperRight),
                };
                nodes.AddRange(stops.Where(stop => stop.Item1.Visible));
            }

            nodes.AddRange(new[]
            {
                ((Control)this.oiaLock, (int?)null, Orientation.LowerLeftTight),
                (this, 2, Orientation.Centered),
                (this, 3, Orientation.Centered),
            });

            Tour.Navigate(this, nodes);
        }

        /// <summary>
        /// Create an image of the main screen.
        /// </summary>
        /// <returns>Bitmap.</returns>
        private Image PrintClientRectangleToImage()
        {
            var bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            using (var bmpGraphics = Graphics.FromImage(bmp))
            {
                var bmpDC = bmpGraphics.GetHdc();
                using (Graphics formGraphics = Graphics.FromHwnd(this.Handle))
                {
                    var formDC = formGraphics.GetHdc();
                    NativeMethods.BitBlt(bmpDC, 0, 0, this.ClientSize.Width, this.ClientSize.Height, formDC, 0, 0, NativeMethods.SRCCOPY);
                    formGraphics.ReleaseHdc(formDC);
                }

                bmpGraphics.ReleaseHdc(bmpDC);
            }

            return bmp;
        }

        /// <summary>
        /// Take a screen snapshot.
        /// </summary>
        /// <param name="fileName">File to save the image in.</param>
        /// <param name="errmsg">Error message.</param>
        /// <returns>True for success.</returns>
        private bool SnapScreen(string fileName, out string errmsg)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                errmsg = "Window is minimized";
                return false;
            }

            Application.DoEvents();
            errmsg = null;
            using var bmp = this.PrintClientRectangleToImage();
            try
            {
                bmp.Save(fileName);
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }

            return string.IsNullOrEmpty(errmsg);
        }

        /// <summary>
        /// The UI snap screen action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiSnapScreen(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = null;
            var args = arguments.ToList();
            if (args.Count != 1)
            {
                result = Constants.Action.SnapScreen + "() takes 1 argument";
                return PassthruResult.Failure;
            }

            // Take the snapshot in the UI thread.
            string errmsg = null;
            this.Invoke(new MethodInvoker(() => this.SnapScreen(args[0], out errmsg)));
            if (string.IsNullOrEmpty(errmsg))
            {
                return PassthruResult.Success;
            }
            else
            {
                result = errmsg;
                return PassthruResult.Failure;
            }
        }

        /// <summary>
        /// Increase or decrease the emulator font size.
        /// </summary>
        /// <param name="keyword">Bigger or Smaller.</param>
        /// <param name="errmsg">Error message.</param>
        /// <returns>True for success.</returns>
        private bool StepEfont(string keyword, out string errmsg)
        {
            errmsg = null;
            if (this.WindowState == FormWindowState.Minimized || this.WindowState == FormWindowState.Maximized || this.IsWindowArranged())
            {
                return false;
            }

            bool bigger = false;
            if (keyword.Equals(Constants.Misc.Bigger, StringComparison.InvariantCultureIgnoreCase))
            {
                bigger = true;
            }
            else if (!keyword.Equals(Constants.Misc.Smaller, StringComparison.InvariantCultureIgnoreCase))
            {
                errmsg = $"Keyword must be '{Constants.Misc.Bigger}' or '{Constants.Misc.Smaller}'";
                return false;
            }

            var newSize = this.ProfileManager.Current.Font.EmSize + (bigger ? 1.0F : -1.0F);
            if (newSize > 0.0)
            {
                this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.Font = new FontProfile(new Font(current.Font.Name, newSize));
                },
                Settings.ChangeName(Settings.ChangeKeyword.Font));
            }

            return true;
        }

        /// <summary>
        /// The UI step emulator font action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiStepEfont(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = null;
            var args = arguments.ToList();
            if (args.Count != 1)
            {
                result = Constants.Action.StepEfont + "() takes 1 argument";
                return PassthruResult.Failure;
            }

            // Take the snapshot in the UI thread.
            string errmsg = null;
            this.Invoke(new MethodInvoker(() => this.StepEfont(args[0], out errmsg)));
            if (string.IsNullOrEmpty(errmsg))
            {
                return PassthruResult.Success;
            }
            else
            {
                result = errmsg;
                return PassthruResult.Failure;
            }
        }

        /// <summary>
        /// Sets full screen mode.
        /// </summary>
        /// <param name="withWarning">True to pop up the warning message.</param>
        private void SetFullScreen(bool withWarning = true)
        {
            this.resizeLocked = true;
            this.Maximize("full screen");

            if (!this.menuBarDisabled)
            {
                // Turn off the menu bar for the duration of F11 full screen mode.
                Trace.Line(Trace.Type.Window, "SetFullScreen turning off menu bar");
                this.mainTable.SuspendLayout();
                this.topBar.RemoveFromParent();
                this.TopLayoutPanel.RemoveFromParent();
                this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 0F);
                this.mainTable.ResumeLayout();
                this.overlayMenuBarDisplayed = false;
                this.fixedHeight -= this.topBar.Height + this.TopLayoutPanel.Height;
                this.screenBox.SetFixed(this.fixedWidth, this.fixedHeight);
            }

            Trace.Line(Trace.Type.Window, "SetFullScreen turning off window borders");
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.snapBox.Enabled = false;
            this.fullScreen = true;
            this.fullScreenToolStripMenuItem.Checked = this.fullScreen;
            Trace.Line(Trace.Type.Window, "SetFullScreen unlocking");
            this.resizeLocked = false;

            if (withWarning)
            {
                this.PopUpFullScreenWarning();
            }
        }

        /// <summary>
        /// Find keymaps containing a specific action.
        /// </summary>
        /// <param name="action">Action to search for.</param>
        /// <returns>List of actions.</returns>
        private IEnumerable<KeyValuePair<string, KeyboardMap>> FindKeymaps(string action)
        {
            return this.ProfileManager.Current.KeyboardMap
                .Where(km => km.Value.Actions.Equals(action, StringComparison.OrdinalIgnoreCase) && !km.Key.Contains("Apl"))
                .ToList();
        }

        /// <summary>
        /// Pop up an information box about how to get out of full screen mode.
        /// </summary>
        private void PopUpFullScreenWarning()
        {
            ErrorBox.ShowWithStop(
                this.handle,
                string.Format("{0}" + Environment.NewLine + Environment.NewLine + "{1}", I18n.Get(ErrorMessage.FullScreenToggle), I18n.Get(ErrorMessage.MenuBarToggle)),
                I18n.Get(Title.FullScreen),
                Constants.StopKey.FullScreen);
        }

        /// <summary>
        /// Pop up an information box about how to get the menu bar back.
        /// </summary>
        private void PopUpMenuBarWarning()
        {
            ErrorBox.ShowWithStop(
                this.handle,
                I18n.Get(ErrorMessage.MenuBarToggle),
                I18n.Get(Title.MenuBarDisabled),
                Constants.StopKey.MenuBar);
        }

        /// <summary>
        /// Toggle full screen mode.
        /// </summary>
        /// <param name="withWarning">True to pop up the warning.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult DoFullScreen(bool withWarning = true)
        {
            if (this.App.NoBorder)
            {
                // No-op if borders are permanently disabled.
                return PassthruResult.Success;
            }

            if (this.fullScreen)
            {
                // Turn off full screen.
                this.resizeLocked = true;
                if (!this.menuBarDisabled)
                {
                    if (this.overlayMenuBarDisplayed)
                    {
                        // Turn off the overlay menu bar.
                        this.topBar.RemoveFromParent();
                        this.TopLayoutPanel.RemoveFromParent();
                        this.overlayMenuBarDisplayed = false;
                    }

                    // Turn the integral menu bar back on.
                    this.topBar.Location = new Point(0, 0);
                    this.mainTable.SuspendLayout();
                    this.mainTable.Controls.Add(this.topBar, 0, 1);
                    this.mainTable.Controls.Add(this.TopLayoutPanel, 0, 0);
                    this.mainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 2F);
                    this.mainTable.ResumeLayout();

                    this.fixedHeight += this.topBar.Height + this.TopLayoutPanel.Height;
                    this.screenBox.SetFixed(this.fixedWidth, this.fixedHeight);
                }

                this.ControlBox = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.snapBox.Enabled = true;
                this.fullScreen = false;
                this.resizeLocked = false;
                this.Restore("fullscreen");
            }
            else
            {
                // Turn on full screen.
                this.SetFullScreen(withWarning);
            }

            this.fullScreenToolStripMenuItem.Checked = this.fullScreen;
            this.temporaryToolStripMenuItem.Enabled = this.fullScreen | this.menuBarDisabled;
            return PassthruResult.Success;
        }

        /// <summary>
        /// The UI full screen (F11) action. Toggles Alt-F11 full screen mode.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiFullScreen(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = null;
            if (arguments.Count() != 0)
            {
                result = Constants.Action.FullScreen + "() takes 0 arguments";
                return PassthruResult.Failure;
            }

            return this.DoFullScreen();
        }

        /// <summary>
        /// Hide the overlay menu bar.
        /// </summary>
        private void HideOverlayMenuBar()
        {
            this.overlayMenuBarStep = OverlayMenuBarSteps;
            this.overlayMenuBarDirection = -1;
            this.overlayMenuBarTimer.Start();
        }

        /// <summary>
        /// Pop the overlay menu bar up or down.
        /// </summary>
        /// <returns>Pass-through result.</returns>
        private PassthruResult ToggleOverlayMenuBar()
        {
            if (this.App.NoButtons || (!this.menuBarDisabled && !this.fullScreen))
            {
                // With the menu bar permanently disabled, or without the menu option or full-screen mode, this is a no-op.
                return PassthruResult.Success;
            }

            if (!this.overlayMenuBarDisplayed)
            {
                // Put the menubar back.
                this.TopLayoutPanel.Location = new Point(0, -this.TopLayoutPanel.Height);
                this.screenBoxPanel.Controls.Add(this.TopLayoutPanel);
                this.TopLayoutPanel.BringToFront();
                this.topBar.Location = new Point(0, -this.TopLayoutPanel.Height);
                this.screenBoxPanel.Controls.Add(this.topBar);
                this.topBar.BringToFront();
                this.overlayMenuBarStep = -OverlayMenuBarSteps;
                this.overlayMenuBarDirection = 1;
                this.overlayMenuBarTimer.Start();
            }
            else
            {
                this.HideOverlayMenuBar();
            }

            return PassthruResult.Success;
        }

        /// <summary>
        /// The UI menu bar action. Toggles the overlay menu bar on and off.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiMenuBar(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = null;
            if (arguments.Count() != 0)
            {
                result = Constants.Action.MenuBar + "() takes 0 arguments";
                return PassthruResult.Failure;
            }

            return this.ToggleOverlayMenuBar();
        }

        /// <summary>
        /// The menu bar hide timer has expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MenuBarHideTimer_Tick(object sender, EventArgs e)
        {
            // Hide the menu bar.
            this.menuBarHideTimer.Stop();
            if ((this.menuBarDisabled || this.fullScreen) && this.overlayMenuBarDisplayed)
            {
                this.HideOverlayMenuBar();
            }
        }

        /// <summary>
        /// Someone has clicked on the menu bar.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TopLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.overlayMenuBarDisplayed)
            {
                return;
            }

            var control = (Control)sender;
            if (control.Tag != null && control.Tag is string && (string)control.Tag == "Now")
            {
                // Remove the overlay menu bar immediately.
                this.ToggleOverlayMenuBar();
                return;
            }

            // Start a timer to make the menu bar disappear.
            this.menuBarHideTimer.Start();
        }

        /// <summary>
        /// Timer tick for the overlay menu bar (coming or going).
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OverlayMenuBarTimer_Tick(object sender, EventArgs e)
        {
            if (this.overlayMenuBarDirection < 0)
            {
                // Move the overlay menu bar up.
                if (--this.overlayMenuBarStep <= 0)
                {
                    // Done.
                    this.overlayMenuBarTimer.Stop();
                    this.topBar.RemoveFromParent();
                    this.TopLayoutPanel.RemoveFromParent();
                    this.overlayMenuBarDisplayed = false;
                }
                else
                {
                    this.TopLayoutPanel.Location = new Point(0, (this.TopLayoutPanel.Height * this.overlayMenuBarStep / OverlayMenuBarSteps) - this.TopLayoutPanel.Height);
                    this.topBar.Location = new Point(0, this.TopLayoutPanel.Location.Y + this.TopLayoutPanel.Height);
                }
            }
            else if (this.overlayMenuBarDirection > 0)
            {
                // Move the overlay menu bar down.
                this.TopLayoutPanel.Location = new Point(0, (this.TopLayoutPanel.Height * ++this.overlayMenuBarStep / OverlayMenuBarSteps) - this.TopLayoutPanel.Height);
                this.topBar.Location = new Point(0, this.TopLayoutPanel.Location.Y + this.TopLayoutPanel.Height);

                if (this.overlayMenuBarStep >= OverlayMenuBarSteps)
                {
                    // Done.
                    this.overlayMenuBarTimer.Stop();
                    this.overlayMenuBarDisplayed = true;
                }
            }
        }

        /// <summary>
        /// An item specific to the screen box context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBoxContextClick(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            switch ((string)menuItem.Tag)
            {
                case "FullScreen":
                    this.DoFullScreen();
                    break;
                case "MenuBar":
                    this.ToggleOverlayMenuBar();
                    break;
                case "Copy":
                    this.App.SelectionManager.Copy(out _);
                    break;
                case "Paste":
                    this.App.SelectionManager.Paste(out _);
                    break;
                case "Cut":
                    this.App.SelectionManager.Cut(out _);
                    break;
                case "MenuBarOneTime":
                    this.ToggleOverlayMenuBar();
                    break;
                case "MenuBarPermanent":
                    if (!this.fullScreen && this.menuBarDisabled)
                    {
                        this.FixedMenuBarSwitch(true);
                    }

                    break;
                case "Keypad":
                    this.ShowKeypad();
                    break;
                case "AplKeypad":
                    this.ShowKeypad(apl: true);
                    break;
                case "Bigger":
                    this.StepEfont(Constants.Misc.Bigger, out _);
                    break;
                case "Smaller":
                    this.StepEfont(Constants.Misc.Smaller, out _);
                    break;
                case "Snap":
                    this.SnapBox_Click(sender, e);
                    break;
                case "Exit":
                    this.BackEnd.Stop();
                    break;
            }
        }

        /// <summary>
        /// An item from the help button context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, EventArgs e)
        {
            if (this.overlayMenuBarDisplayed)
            {
                this.HideOverlayMenuBar();
            }

            Tour.HelpMenuClick(sender, e, "Main", this.RunTour);
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private static class Title
        {
            /// <summary>
            /// Host connect.
            /// </summary>
            public static readonly string Connect = I18n.Combine(TitleName, "connect");

            /// <summary>
            /// Macro error.
            /// </summary>
            public static readonly string MacroError = I18n.Combine(TitleName, "macroError");

            /// <summary>
            /// Keypad menu error.
            /// </summary>
            public static readonly string KeypadMenuError = I18n.Combine(TitleName, "keypadMenuError");

            /// <summary>
            /// Full screen one-time info.
            /// </summary>
            public static readonly string FullScreen = I18n.Combine(TitleName, "fullScreen");

            /// <summary>
            /// Menu bar disable one-time info.
            /// </summary>
            public static readonly string MenuBarDisabled = I18n.Combine(TitleName, "menuBarDisabled");

            /// <summary>
            /// Menu bar enabled.
            /// </summary>
            public static readonly string MenuBarEnabled = I18n.Combine(TitleName, "menuBarEnabled");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        private static class ErrorMessage
        {
            /// <summary>
            /// No such host.
            /// </summary>
            public static readonly string NoSuchConnection = I18n.Combine(MessageName, "noSuchConnection");

            /// <summary>
            /// Invalid prefixes in command-line host.
            /// </summary>
            public static readonly string InvalidPrefixes = I18n.Combine(MessageName, "invalidPrefixes");

            /// <summary>
            /// Informational pop-up about restoring the menu bar.
            /// </summary>
            public static readonly string MenuBarToggle = I18n.Combine(MessageName, "menuBarToggle");

            /// <summary>
            /// Informational pop-up about enabling the menu bar.
            /// </summary>
            public static readonly string MenuBarToggleNop = I18n.Combine(MessageName, "menuBarToggleNop");

            /// <summary>
            /// Informational pop-up about exiting full-screen mode.
            /// </summary>
            public static readonly string FullScreenToggle = I18n.Combine(MessageName, "fullScreenToggle");

            /// <summary>
            /// Informational pop-up about discarding a pending macro recording.
            /// </summary>
            public static readonly string RecordingDiscarded = I18n.Combine(MessageName, "recordingDiscarded");
        }

        /// <summary>
        /// Save types.
        /// </summary>
        private static class SaveType
        {
            /// <summary>
            /// Saving the command-line host connection.
            /// </summary>
            public static readonly string CommandLineHost = I18n.Combine(MessageName, "commandLineHost");
        }
    }
}
