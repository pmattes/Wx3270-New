// <copyright file="MainScreen.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using I18nBase;
    using Wx3270.Contracts;

    using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
    using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
    using MouseEventHandler = System.Windows.Forms.MouseEventHandler;

    /// <summary>
    /// The main screen.
    /// </summary>
    public partial class MainScreen : Form, IUpdate, IShift, IFlash
    {
        /// <summary>
        /// The name of the 3270 font.
        /// </summary>
        public const string Name3270Font = "3270";

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
        /// Set if the keypad has ever been displayed.
        /// </summary>
        private readonly HashSet<Form> keypadEverUp = new HashSet<Form>();

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
        /// The macro record menu item.
        /// </summary>
        private ToolStripMenuItem macroRecordItem;

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
        /// Secondary initialization event.
        /// </summary>
        public event Action SecondaryInitEvent = () => { };

        /// <summary>
        /// Connection state change event.
        /// </summary>
        public event Action ConnectionStateEvent = () => { };

        /// <summary>
        /// Screen mode change event.
        /// </summary>
        public event Action ScreenModeEvent = () => { };

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
        /// Gets the actions dialog.
        /// </summary>
        public Actions ActionsDialog { get; private set; }

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
        private Form[] Keypads => new Form[] { this.keypad, this.aplKeypad };

        /// <summary>
        /// Gets the set of flashable keypads.
        /// </summary>
        private IFlash[] FlashableKeypads => new IFlash[] { this.keypad, this.aplKeypad };

        /// <summary>
        /// Gets the macro recorder.
        /// </summary>
        private MacroRecorder MacroRecorder => this.App.MacroRecorder;

        /// <summary>
        /// Compute the location for a centered dialog window.
        /// </summary>
        /// <param name="parent">Parent form.</param>
        /// <param name="child">Child form.</param>
        /// <returns>Location to draw child.</returns>
        public static Point CenteredOn(Control parent, Control child)
        {
            var p = parent.Location;
            p.X += (parent.Width / 2) - (child.Width / 2);
            if (child.Height < parent.Height)
            {
                p.Y += (parent.Height / 2) - (child.Height / 2);
            }

            return p;
        }

        /// <summary>
        /// Create a new window title string.
        /// </summary>
        /// <param name="hostEntry">Current host entry, or null.</param>
        /// <param name="profile">Current profile.</param>
        /// <param name="host">Current connected host, or null.</param>
        /// <returns>New title string.</returns>
        public static string NewTitle(HostEntry hostEntry, Profile profile, string host)
        {
            if (hostEntry != null && !string.IsNullOrWhiteSpace(hostEntry.WindowTitle))
            {
                return hostEntry.WindowTitle;
            }

            if (!string.IsNullOrWhiteSpace(profile.WindowTitle))
            {
                return profile.WindowTitle;
            }

            var profileNameDisplay = profile.Name;
            if (profile.ReadOnly)
            {
                profileNameDisplay += "(" + Wx3270.ProfileManager.ReadOnlyName + ")";
            }

            if (host != null)
            {
                return $"{profileNameDisplay} / {host} - wx3270";
            }
            else
            {
                return $"{profileNameDisplay} - wx3270";
            }
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
        }

        /// <summary>
        /// Further initialization.
        /// </summary>
        /// <param name="app">Application instance.</param>
        public void Init(Wx3270App app)
        {
            this.App = app;
            this.BaseInit(); // should do earlier?
            this.SecondaryInit();
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
        public void Maximize()
        {
            if (!this.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// Force the main screen window to restore (not be maximized).
        /// </summary>
        public void Restore()
        {
            if (this.Maximized)
            {
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
            foreach (var oiaField in this.OiaLayoutPanel.Controls)
            {
                if (colorMode && oiaField == (object)this.OiaLock)
                {
                    this.OiaLock.ForeColor = this.colors.HostColors[this.OiaLock.Tag != null ? HostColor.Red : HostColor.NeutralWhite];
                }
                else
                {
                    ((Control)oiaField).ForeColor = fg;
                }
            }

            // Correct the lock icon.
            this.ChangeOiaTls(this.App.OiaState);

            // Change the dividing bar color.
            this.TopBar.BackColor = fg;
            this.BottomBar.BackColor = fg;

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
            if (this.Maximized)
            {
                // Recompute the font size when maximized (ignore the selected size).
                newFont = this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic);
            }

            // Change the OIA.
            this.RefontOia(newFont);

            if (!this.Maximized)
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
            if (this.Maximized || this.Size == this.MinimumSize)
            {
                // No snapping when maximized or at the minimum.
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
        /// Add or remove the scroll bar.
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
                this.Restore();
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

            // Reevluate the fixed area of the screen and set the overall screen size.
            this.ResetFixed(displayed);
            this.ClientSize = this.mainScreenPanel.Size;
            var size = this.Size;

            // Return to maximized if needed.
            if (wasMaximized)
            {
                this.Maximize();
            }

            return size;
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
        /// Add or remove the scroll bar.
        /// </summary>
        /// <param name="displayed">True if scroll bar should be displayed.</param>
        private void ToggleScrollBarInternal(bool displayed)
        {
            if (this.App.NoScrollBar || displayed == this.scrollBarDisplayed)
            {
                return;
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

            // Reevaluate the sizes of the fixed elements.
            this.ResetFixed(displayed);
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

            // Register for profile hosts change events.
            this.macroEntries.ChangeEvent += this.MacrosChanged;

            // Register for profile-related events.
            this.App.ProfileTracker.ProfileTreeChanged += (tree) => this.Invoke(new MethodInvoker(() => this.ProfileTreeChanged(tree)));
            this.ProfileManager.Change += (profile) => this.Invoke(new MethodInvoker(() => this.ProfileChanged(profile)));
            this.ProfileManager.NewProfileOpened += (profile) =>
            {
                // Set the window location, if reasonable to do so.
                if (!this.App.Location.HasValue && profile.Location.HasValue && this.IsVisible(profile.Location.Value))
                {
                    this.Location = profile.Location.Value;
                }
            };
            this.ProfileManager.ChangeFinal += (profile, isNew) =>
            {
                var maximize = profile.Maximize;
                Size? size = profile.Size.HasValue ? (Size?)new Size(profile.Size.Value.Width, profile.Size.Value.Height) : null;
                if (maximize || size.HasValue)
                {
                    // Run the following after any other back-end action, such as changing the model:
                    //  Scroll bar.
                    //  Maximize.
                    // Set the size, even if we are going to maximize, so when we un-maximize, we get the right size.
                    this.BackEnd.RunAction(new BackEndAction(B3270.Action.Query, B3270.Query.Model), (cookie, success, result, misc) =>
                    {
                        this.ToggleScrollBarInternal(profile.ScrollBar);

                        if (maximize)
                        {
                            this.Maximize();
                        }

                        if (size.HasValue)
                        {
                            Trace.Line(Trace.Type.Window, $"MainScreen ChangeFinal setting size to {size.Value}");
                            this.Size = size.Value;
                        }
                    });
                }
            };
            this.ProfileManager.ProfileClosing += (profile) =>
            {
                // When a profile is closed, save the window location.
                profile.Location = this.Location;
            };

            // Set up other parts.
            this.keypad = new Keypad(this.App, this);
            this.aplKeypad = new AplKeypad(this.App, this);
            this.settings = new Settings(this.App, this, this.keypad, this.aplKeypad);
            this.ActionsDialog = new Actions(this.App, this);
            this.Connect = new Connect(this.App, this);
            this.macros = new Macros(this.App, this, this.macroEntries);
            this.profileTree = new ProfileTree(this.App, this, this.Connect);

            // Glue keypad and opacity setting together.
            this.keypad.RegisterOpacity(this.settings);

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
        /// Re-measure the size of the fixed parts of the screen.
        /// </summary>
        /// <param name="scrollBar">True if the scroll bar is displayed.</param>
        private void ResetFixed(bool scrollBar)
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
                this.TopBar.Width = this.innerScreenTableLayoutPanel.Width;
                this.BottomBar.Width = this.innerScreenTableLayoutPanel.Width;
                this.OiaLayoutPanel.Width = this.innerScreenTableLayoutPanel.Width;
                this.OiaLayoutPanel.Height = cellSizeHeight + 4;
            };
            this.screenBox.FontChanged += (font, dynamic) =>
            {
                this.RefontOia(font);
                if (dynamic)
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
                this.TopBar.RemoveFromParent();
                this.TopLayoutPanel.RemoveFromParent();
                this.MainTable.RowStyles[1] = new RowStyle(SizeType.Absolute, 0F);
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
                hh = ScreenBox.ComputeCellSize(g, this.OiaLock.Font).Height;
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

            // Subscribe to profile changes.
            this.ProfileManager.Change += this.ProfileChange;

            // Subscribe to connection changes.
            this.ConnectionStateEvent += () =>
            {
                this.Retitle();
                this.disconnectMenuItem.Enabled = this.App.ConnectionState != ConnectionState.NotConnected;
                this.quickConnectMenuItem.Enabled = this.App.ConnectionState == ConnectionState.NotConnected;
            };

            // Subscribe to toggle changes.
            this.App.SettingChange.Register(
                (settingName, settingDictionary) => this.Invoke(new MethodInvoker(() => this.OnSettingEvent(settingName, settingDictionary))),
                new[] { B3270.Setting.Trace, B3270.Setting.ScreenTrace, B3270.Setting.VisibleControl, B3270.Setting.AplMode });

            // Cascade secondary init to others.
            this.SecondaryInitEvent();

            // Dump errors from the first profile load, on a separate thread.
            this.ProfileErrorTimer.Enabled = true;

            // When the emulator is ready, push out the initial profile and show the window.
            this.App.BackEnd.OnReady += () =>
            {
                var autoConnect = false;

                // Push out the initial profile.
                this.ProfileManager.PushFirst();

                // Display the main screen window.
                this.Show();

                // Set up command-line auto-connect.
                if (!this.App.EditMode)
                {
                    HostEntry autoConnectHost = null;
                    if (this.App.HostConnection != null)
                    {
                        autoConnectHost = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(this.App.HostConnection, StringComparison.InvariantCultureIgnoreCase));
                        if (autoConnectHost == null)
                        {
                            ErrorBox.Show(string.Format("{0}: {1}", I18n.Get(ErrorMessage.NoSuchHost), this.App.HostConnection), I18n.Get(Title.HostConnect), MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        autoConnectHost = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.AutoConnect == AutoConnect.Connect || h.AutoConnect == AutoConnect.Reconnect);
                    }

                    if (autoConnectHost != null)
                    {
                        autoConnect = true;
                        this.Connect.ConnectToHost(autoConnectHost);
                    }
                }

                // Set up command-line host connection.
                if (!autoConnect && this.App.CommandLineHost != null)
                {
                    var autoName = HostEntry.AutoName(this.App.CommandLineHost, this.App.CommandLinePort);
                    var hostEntry = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(autoName, StringComparison.InvariantCultureIgnoreCase));
                    if (hostEntry == null)
                    {
                        hostEntry = new HostEntry(this.App.CommandLineHost, this.App.CommandLinePort, this.App.HostPrefix.Prefixes)
                        {
                            Profile = this.ProfileManager.Current,
                        };

                        if (hostEntry.InvalidPrefixes != null)
                        {
                            var invalidPrefixes = string.Join(", ", hostEntry.InvalidPrefixes.Select(c => new string(new[] { c, ':' })));
                            ErrorBox.Show(
                                string.Format("{0}: {1}", I18n.Get(ErrorMessage.InvalidPrefixes), invalidPrefixes),
                                I18n.Get(Title.HostConnect),
                                MessageBoxIcon.Warning);
                        }

                        this.ProfileManager.PushAndSave(
                            current =>
                            {
                                current.Hosts = current.Hosts.Concat(new[] { hostEntry });
                            },
                            I18n.Get(SaveType.CommandLineHost));
                    }

                    this.Connect.ConnectToHost(hostEntry);
                }
            };

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
                this.settingsBox.RemoveFromParent();
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
            }

            if (this.App.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            if (this.App.Restricted(Restrictions.ChangeSettings) && !this.ProfileManager.Current.Macros.Any())
            {
                this.macrosPictureBox.RemoveFromParent();
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
                this.profilePictureBox.RemoveFromParent();
            }

            // The connect menu may be moot at this point.
            if (this.App.Restricted(Restrictions.Disconnect) &&
                this.ProfileManager.Current.Hosts.Any(host => host.AutoConnect == AutoConnect.Reconnect))
            {
                this.connectPictureBox.RemoveFromParent();
            }

            if (this.App.NoBorder)
            {
                this.snapBox.RemoveFromParent();
            }

            // Set up the screen snap action.
            if (this.App.Allowed(Restrictions.ExternalFiles))
            {
                this.App.BackEnd.RegisterPassthru(Constants.Action.SnapScreen, this.UiSnapScreen);
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);
            this.InitOiaLocalization();
            I18n.LocalizeGlobal(Title.HostConnect, "Host Connect");
            I18n.LocalizeGlobal(Title.MacroError, "Macro Error");
            I18n.LocalizeGlobal(Title.KeypadMenuError, "Keypad Menu Error");
            I18n.LocalizeGlobal(ErrorMessage.NoSuchHost, "No such host connection");
            I18n.LocalizeGlobal(ErrorMessage.InvalidPrefixes, "Invalid prefix(es) in command-line host");
            I18n.LocalizeGlobal(SaveType.CommandLineHost, "Command-line host connection");

            // Set up the connect menu.
            this.ProfileTreeChanged(this.App.ProfileTracker.Tree);

            // Handle the no-border option.
            if (this.App.NoBorder)
            {
                this.ControlBox = false;
                this.FormBorderStyle = FormBorderStyle.None;
                this.snapBox.RemoveFromParent();
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
        /// Someone canceled a chord.
        /// </summary>
        private void ChordReset()
        {
            this.chordTimer.Stop();
            this.OiaCx.Text = string.Empty;
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
            this.OiaCx.Text = "C…";

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
                            stack.Peek().DropDownItems.Add(
                                profile.Name,
                                null,
                                (sender, e) =>
                                {
                                    if (this.App.ConnectionState != ConnectionState.NotConnected)
                                    {
                                        ProfileTree.NewWindow(this, this.components, profile.PathName);
                                    }
                                    else if (ModifierKeys.HasFlag(Keys.Shift) || !this.ProfileManager.IsCurrentPathName(profile.PathName))
                                    {
                                        this.profileTree.LoadWithAutoConnect(profile.PathName, ModifierKeys.HasFlag(Keys.Shift));
                                    }
                                });
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

            // Add to the connect menu.
            var connectStack = new Stack<ToolStripMenuItem>();
            connectStack.Push(this.connectMenuItem);
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
                        stack.Peek().DropDownItems.Add(nonterminal);
                        return nonterminal;
                    case WatchNodeType.Host:
                        var profile = (node.Parent as ProfileWatchNode).Profile;
                        var host = node as HostWatchNode;
                        var item = new ToolStripMenuItem(host.Name, null, this.ConnectToOtherHost);
                        if (!string.IsNullOrWhiteSpace(host.HostEntry.Description))
                        {
                            item.ToolTipText = host.HostEntry.Description;
                        }

                        item.Tag = new Tuple<string, string>(profile.PathName, node.Name);
                        stack.Peek().DropDownItems.Add(item);
                        return null;
                    default:
                        return null;
                }
            });
        }

        /// <summary>
        /// The list of profiles and hosts changed.
        /// </summary>
        /// <param name="newFolderTree">New host tree.</param>
        private void ProfileTreeChanged(List<FolderWatchNode> newFolderTree)
        {
            this.connectMenuItem.DropDownItems.Clear();
            this.loadMenuItem.DropDownItems.Clear();

            // Start with the current profile's hosts.
            foreach (var host in this.ProfileManager.Current.Hosts)
            {
                var item = new ToolStripMenuItem(host.Name, null, this.ConnectToProfileHost);
                if (!string.IsNullOrWhiteSpace(host.Description))
                {
                    item.ToolTipText = host.Description;
                }

                this.connectMenuItem.DropDownItems.Add(item);
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
        }

        /// <summary>
        /// Connect to a profile host from the context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectToProfileHost(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripMenuItem;
            var hostEntry = this.ProfileManager.Current.Hosts.FirstOrDefault(h => h.Name.Equals(clickedMenuItem.Text));
            if (hostEntry != null)
            {
                if (this.App.ConnectionState != ConnectionState.NotConnected)
                {
                    ProfileTree.NewWindow(this, this.components, hostEntry.Profile.PathName, hostEntry.Name);
                }
                else
                {
                    this.Connect.ConnectToHost(hostEntry);
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
                ProfileTree.NewWindow(this, this.components, hostNode.Item1, hostNode.Item2);
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
        /// <param name="profile">New profile.</param>
        private void ProfileChanged(Profile profile)
        {
            // Update the load menu.
            foreach (var item in this.loadMenuItem.DropDownItems.Cast<ToolStripMenuItem>())
            {
                item.Enabled = !item.Text.Equals(profile.Name);
                item.Checked = item.Text.Equals(profile.Name);
            }

            // Update the title.
            this.Text = NewTitle(null, profile, null);

            // Update the connect menu, a side-effect of profile tree processing.
            this.ProfileTreeChanged(this.App.ProfileTracker.Tree);
        }

        /// <summary>
        /// The macros list changed.
        /// </summary>
        private void MacrosChanged()
        {
            this.macrosContextMenuStrip.Items.Clear();
            this.macrosContextMenuStrip.Items.AddRange(
                this.macroEntries.Entries.Select(e => new ToolStripMenuItem(e.Name, null, this.RunMacro) { Tag = e }).ToArray());
            this.macroRecordItem = new ToolStripMenuItem(
                I18n.Get(this.MacroRecorder.Running ? MacroStopRecordingItemName : MacroRecordingItemName),
                this.MacroRecorder.Running ? Properties.Resources.stop_recording : Properties.Resources.record1,
                this.ToggleRecording);
            this.macrosContextMenuStrip.Items.Add(this.macroRecordItem);
        }

        /// <summary>
        /// Run a macro from the context menu.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RunMacro(object sender, EventArgs e)
        {
            var clickedMenuItem = sender as ToolStripMenuItem;
            var entry = (MacroEntry)clickedMenuItem.Tag;
            this.BackEnd.RunActions(entry.Macro, ErrorBox.Completion(I18n.Get(Title.MacroError)));
        }

        /// <summary>
        /// Process a macro recorder state change.
        /// </summary>
        /// <param name="running">True if recorder is running.</param>
        private void OnMacroRecorderState(bool running)
        {
            if (running)
            {
                this.macroRecordItem.Text = I18n.Get(MacroStopRecordingItemName);
                this.macroRecordItem.Image = Properties.Resources.stop_recording;
                this.toolTip1.SetToolTip(this.macrosPictureBox, I18n.Get(MacroRecordingToolTipName));
            }
            else
            {
                this.macroRecordItem.Text = I18n.Get(MacroRecordingItemName);
                this.macroRecordItem.Image = Properties.Resources.record1;
                this.toolTip1.SetToolTip(this.macrosPictureBox, I18n.Get(MacrosToolTipName));
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
                this.macros.Record(text);
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
                    this.ScreenModeEvent();
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
                if (font.Name != Name3270Font)
                {
                    try3270Font = new Font(Name3270Font, font.Size);
                    if (try3270Font.Name == Name3270Font)
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
                                try3270Font = new Font(Name3270Font, newSize);

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
            foreach (var oiaField in this.OiaLayoutPanel.Controls.OfType<Control>())
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
        }

        /// <summary>
        /// The profile changed (was loaded).
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void ProfileChange(Profile profile)
        {
            // For non-full profiles, no quick connect.
            this.quickConnectMenuItem.Enabled = profile.ProfileType == ProfileType.Full;

            // Redraw the main screen.
            this.Recolor(profile.Colors, profile.ColorMode);
        }

        /// <summary>
        /// A setting changed.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Setting dictionary.</param>
        private void OnSettingEvent(string settingName, SettingsDictionary settingDictionary)
        {
            switch (settingName)
            {
                case B3270.Setting.Trace:
                    this.tracingToolStripMenuItem.Checked = settingDictionary.TryGetValue(B3270.Setting.Trace, out bool trace) && trace;
                    break;
                case B3270.Setting.ScreenTrace:
                    var screenTracing = settingDictionary.TryGetValue(B3270.Setting.ScreenTrace, out bool screenTrace) && screenTrace;
                    this.screenTracingMenuItem.Checked = screenTracing;
                    this.sendToPrinterToolStripMenuItem.Enabled = !screenTracing;
                    this.saveToFileToolStripMenuItem.Enabled = !screenTracing;
                    break;
                case B3270.Setting.VisibleControl:
                    this.controlCharsMenuItem.Checked = settingDictionary.TryGetValue(B3270.Setting.VisibleControl, out bool visibleControl) && visibleControl;
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
            this.vScrollBar1.Height = this.MainTable.Height;
        }

        /// <summary>
        /// Form load method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void X3270_Load(object sender, EventArgs e)
        {
            // Initialize the OIA fields.
            var defState = Oia.DefaultOiaState;
            this.ChangeOiaNetwork(defState);
            this.ChangeOiaLock(defState);
            this.OiaPrinter.Text = string.Empty;
            this.OiaScreentrace.Text = string.Empty;
            this.OiaScript.Text = string.Empty;
            this.OiaTypeahead.Text = string.Empty;
            this.OiaAltShift.Text = string.Empty;
            this.OiaCx.Text = string.Empty;
            this.OiaReverse.Text = string.Empty;
            this.ChangeOiaInsert(defState);
            this.ChangeOiaTls(defState);
            this.ChangeOiaLu(defState);
            this.ChangeOiaTiming(defState);
            this.ChangeOiaCursor(defState);

            Trace.Line(Trace.Type.Window, "MainWindow Load");

            // We will not receive a message for initial maximized state, so it has to be checked here.
            if (this.Maximized)
            {
                this.screenBox.Maximize(true, this.ClientSize);
                this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic); // XXX?
            }
        }

        /// <summary>
        /// Mouse button down event handler for the screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var cellLocation = this.screenBox.CellCoordinates(e.Location);
            this.App.SelectionManager.MouseDown(cellLocation.Item1, cellLocation.Item2, ModifierKeys.HasFlag(Keys.Shift));
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

            var cellLocation = this.screenBox.CellCoordinates(e.Location);
            this.App.SelectionManager.MouseMove(cellLocation.Item1, cellLocation.Item2);
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

            var cellLocation = this.screenBox.CellCoordinates(e.Location);
            this.App.SelectionManager.MouseUp(cellLocation.Item1, cellLocation.Item2);
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
            Trace.Line(Trace.Type.Window, $"MainScreen Resize state {this.WindowState} Size {this.Size}  ClientSize {this.ClientSize} Location {this.Location}");

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
                    foreach (var pad in this.Keypads)
                    {
                        if (this.keypadMinimized.Contains(pad))
                        {
                            // Restore the keypad, too.
                            this.noFlashTimer.Start();
                            pad.Show();
                            this.keypadMinimized.Remove(pad);
                        }
                    }

                    if (!this.resizeBeginPending &&
                        this.ClientSize != this.mainScreenPanel.Size &&
                        this.screenBox.ResizeReady)
                    {
                        // Got a Resize on its own (outside of ResizeStart/ResizeEnd), which is more of a "Size" event.
                        Trace.Line(Trace.Type.Window, " ==> resize");
                        this.screenBox.Maximize(this.Maximized, this.ClientSize);
                        var newFont = this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic);
                        if (this.ProfileManager.PushAndSave(
                            (current) =>
                            {
                                current.Font = new FontProfile(newFont);
                                if (this.Maximized)
                                {
                                    current.Maximize = true;
                                }
                                else
                                {
                                    current.Size = this.Size;
                                    current.Maximize = false;
                                }
                            }, I18n.Get(ResizeName)))
                        {
                            Trace.Line(Trace.Type.Window, "  Resize pushed");
                        }
                    }

                    break;
            }
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
            if (!this.settings.Visible)
            {
                this.settings.Show(this);
            }

            this.settings.Activate();
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
            this.ProfileErrorTimer.Enabled = false;
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
            // You can't restore the profile tree while a macro is being recorded.
            this.MacroRecorder.Abort();

            if (!this.profileTree.Visible)
            {
                this.profileTree.Show(this);
            }

            this.profileTree.Activate();
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

            if (!this.macros.Visible)
            {
                this.macros.Show(this);
            }

            this.macros.Activate();
        }

        /// <summary>
        /// The disconnect item in the connection context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DisconnectMenuItem_Click(object sender, EventArgs e)
        {
            this.Connect.Disconnect();
        }

        /// <summary>
        /// The quick connect item in the connection context menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void QuickConnectMenuItem_Click(object sender, EventArgs e)
        {
            this.profileTree.CreateHostDialog(this.ProfileManager.Current);
        }

        /// <summary>
        /// The main screen has been shown.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainScreen_Shown(object sender, EventArgs e)
        {
            // Nothing at the moment.
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
        /// The Cancel Scripts context menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsCancelScripts(object sender, EventArgs e)
        {
            this.ActionsDialog.CancelScripts();
        }

        /// <summary>
        /// The prompt context menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsPromptClick(object sender, EventArgs e)
        {
            this.App.Prompt.Start();
        }

        /// <summary>
        /// The tracing context menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsTracingClick(object sender, EventArgs e)
        {
            this.ActionsDialog.Tracing = !this.ActionsDialog.Tracing;
        }

        /// <summary>
        /// The visible control characters menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsVisibleControlClick(object sender, EventArgs e)
        {
            this.ActionsDialog.VisibleControl = !this.ActionsDialog.VisibleControl;
        }

        /// <summary>
        /// The Print Screen menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrintScreen_Click(object sender, EventArgs e)
        {
            this.ActionsDialog.PrintTextButton_Click(sender, e);
        }

        /// <summary>
        /// The screen tracing menu item was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenTracing_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item))
            {
                return;
            }

            this.ActionsDialog.ToggleScreenTracing((string)item.Tag);
        }

        /// <summary>
        /// The help picture box was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Click(object sender, EventArgs e)
        {
            Wx3270App.GetHelp("Main");
        }

        /// <summary>
        /// The re-enable keyboard menu item was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ReenableKeyboard_Click(object sender, EventArgs e)
        {
            this.ActionsDialog.ReenableKeyboard();
        }

        /// <summary>
        /// The display keymap menu item was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DisplayKeymap_Click(object sender, EventArgs e)
        {
            this.ActionsDialog.DisplayKeymap();
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
            this.resizeBeginPending = true;
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
        /// The keypad button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadBox_Click(object sender, EventArgs e)
        {
            var keypad = this.Mod.HasFlag(KeyboardModifier.Alt) ? this.aplKeypad : (Form)this.keypad;
            if (!keypad.Visible)
            {
                this.noFlashTimer.Start();
                keypad.Show();
                if (!this.keypadEverUp.Contains(keypad))
                {
                    this.keypadEverUp.Add(keypad);
                    switch (this.ProfileManager.Current.KeypadPosition)
                    {
                        case KeypadPosition.Left:
                            keypad.Location = new Point(this.Location.X - this.keypad.Width, this.Location.Y);
                            break;
                        case KeypadPosition.Centered:
                            keypad.Location = CenteredOn(this, this.keypad);
                            break;
                        case KeypadPosition.Right:
                            keypad.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                            break;
                    }
                }
            }
            else
            {
                this.noFlashTimer.Start();
                keypad.Activate();
            }
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
            Trace.Line(Trace.Type.Window, $"MainWindow ResizeEnd WindowState {this.WindowState} Size {this.Size} ClientSize {this.ClientSize} Location {this.Location}");

            this.resizeBeginPending = false;

            // Recompute the font, only if the size has changed.
            if (this.ClientSize != this.mainScreenPanel.Size)
            {
                Trace.Line(Trace.Type.Window, " ==> resize");
                this.screenBox.Maximize(this.Maximized, this.ClientSize);
                var newFont = this.screenBox.RecomputeFont(this.ClientSize, ResizeType.Dynamic);

                if (this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        current.Font = new FontProfile(newFont);
                        if (this.Maximized)
                        {
                            current.Maximize = true;
                        }
                        else
                        {
                            current.Size = this.Size;
                            current.Maximize = false;
                        }
                    }, I18n.Get(ResizeName)))
                {
                    Trace.Line(Trace.Type.Window, " ==> resize pushed");
                }
            }
        }

        /// <summary>
        /// The File Transfer button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransfer_Click(object sender, EventArgs e)
        {
            this.ActionsDialog.FileTransfer();
        }

        /// <summary>
        /// Paint event handler for the start button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActionsBox_paint(object sender, PaintEventArgs e)
        {
            this.startButton.Render((PictureBox)sender, e, I18n.Get(StartButtonName));
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
        /// Message box titles.
        /// </summary>
        private static class Title
        {
            /// <summary>
            /// Host connect.
            /// </summary>
            public static readonly string HostConnect = I18n.Combine(TitleName, "hostConnect");

            /// <summary>
            /// Macro error.
            /// </summary>
            public static readonly string MacroError = I18n.Combine(TitleName, "macroError");

            /// <summary>
            /// Keypad menu error.
            /// </summary>
            public static readonly string KeypadMenuError = I18n.Combine(TitleName, "keypadMenuError");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        private static class ErrorMessage
        {
            /// <summary>
            /// No such host.
            /// </summary>
            public static readonly string NoSuchHost = I18n.Combine(MessageName, "noSuchHost");

            /// <summary>
            /// Invalid prefixes in command-line host.
            /// </summary>
            public static readonly string InvalidPrefixes = I18n.Combine(MessageName, "invalidPrefixes");
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
