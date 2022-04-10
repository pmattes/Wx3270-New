// <copyright file="KeypadCommon.cs" company="Paul Mattes">
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

    /// <summary>
    /// The pop-up keypad.
    /// </summary>
    public class KeypadCommon : IShift, IFlash
    {
        /// <summary>
        /// Key animation interval.
        /// </summary>
        private const int KeyReleaseMilliseconds = 250;

        /// <summary>
        /// Number of pixels to shift a key when pressed.
        /// </summary>
        private const int KeyShift = 4;

        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Keypad));

        /// <summary>
        /// Application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// Main window flash interface.
        /// </summary>
        private readonly IFlash mainWindowFlash;

        /// <summary>
        /// Queue of key release events.
        /// </summary>
        private readonly Queue<KeyRelease> keyReleases = new Queue<KeyRelease>();

        /// <summary>
        /// Flash state machine.
        /// </summary>
        private readonly FlashFsm flashFsm;

        /// <summary>
        /// Parent form.
        /// </summary>
        private readonly Form form;

        /// <summary>
        /// Consituent panels.
        /// </summary>
        private readonly List<Panel> panels;

        /// <summary>
        /// Flash button.
        /// </summary>
        private readonly Button flashButton;

        /// <summary>
        /// Outer panel.
        /// </summary>
        private readonly Panel outerPanel;

        /// <summary>
        /// The activation timer.
        /// </summary>
        private readonly Timer activateTimer;

        /// <summary>
        /// The button release timer.
        /// </summary>
        private readonly Timer buttonReleaseTimer;

        /// <summary>
        /// The window hide timer.
        /// </summary>
        private readonly Timer hideTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypadCommon"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="form">Parent form.</param>
        /// <param name="flash">Flash interface.</param>
        /// <param name="panels">Constituent panels.</param>
        /// <param name="outerPanel">Outer panel.</param>
        /// <param name="flashButton">Button to flash.</param>
        public KeypadCommon(Wx3270App app, Form form, IFlash flash, IEnumerable<Panel> panels, Panel outerPanel, Button flashButton)
        {
            this.app = app;
            this.mainWindowFlash = flash;
            this.form = form;
            this.panels = panels.ToList();
            this.outerPanel = outerPanel;
            this.flashButton = flashButton;

            // Register for profile changes.
            this.app.ProfileManager.Change += this.KeypadProfileChanged;

            // Register for synthetic modifier changes.
            this.app.SyntheticModChange += (mod, mask) => this.ModChanged(mod, mask);

            // Set up the flash FSM.
            this.flashFsm = new FlashFsm(
                (action) =>
                {
                    switch (action)
                    {
                        case FlashFsm.Action.Flash:
                            Trace.Line(Trace.Type.Window, "Keypad Flash 2");
                            this.flashButton.ForeColor = Color.LawnGreen;
                            break;
                        case FlashFsm.Action.Restore:
                            Trace.Line(Trace.Type.Window, "Keypad Restore");
                            this.flashButton.ForeColor = Color.DarkGray;
                            break;
                        default:
                            break;
                    }
                });

            // Set up the timers.
            this.activateTimer = new Timer { Interval = 500 };
            this.activateTimer.Tick += new EventHandler(this.ActivateTimer_Tick);
            this.buttonReleaseTimer = new Timer { Interval = 250 };
            this.buttonReleaseTimer.Tick += new EventHandler(this.ButtonReleaseTick);
            this.hideTimer = new Timer { Interval = 250 };
            this.hideTimer.Tick += new EventHandler(this.HideTick);
        }

        /// <summary>
        /// Gets the current keyboard modifier state.
        /// </summary>
        public KeyboardModifier Mod { get; private set; }

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private IBackEnd BackEnd => this.app.BackEnd;

        /// <summary>
        /// Gets the keyboard input handler.
        /// </summary>
        private IKeyHandler KeyHandler => this.app.KeyHandler;

        /// <summary>
        /// Gets the sound handler.
        /// </summary>
        private ISound Sound => this.app.Sound;

        /// <summary>
        /// Gets the current keypad maps.
        /// </summary>
        private KeyMap<KeypadMap> KeypadMaps => this.app.ProfileManager.Current.KeypadMap;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.KeypadOutput, "Keypad Action");
            I18n.LocalizeGlobal(Title.KeypadError, "Keypad");
        }

        /// <summary>
        /// The keyboard modifiers changed.
        /// </summary>
        /// <param name="mod">New modifiers.</param>
        /// <param name="mask">Modifier mask.</param>
        public void ModChanged(KeyboardModifier mod, KeyboardModifier mask)
        {
            if ((mod & mask) != (this.Mod & mask))
            {
                this.Mod = (this.Mod & ~mask) | (mod & mask);
                this.KeypadProfileChanged(this.app.ProfileManager.Current);
            }
        }

        /// <summary>
        /// Apply a new set of keypad maps.
        /// </summary>
        /// <param name="maps">New maps.</param>
        public void ApplyMaps(KeyMap<KeypadMap> maps)
        {
            foreach (var panel in this.panels)
            {
                foreach (var buttonPanel in panel.Controls.OfType<Panel>())
                {
                    var button = buttonPanel.Controls[0] as Button;
                    if (maps.TryGetClosestMatch(
                        Settings.ButtonBaseName(button.Name),
                        null,
                        this.Mod,
                        out KeypadMap map,
                        out _,
                        out _,
                        out _))
                    {
                        button.Text = map.Text;
                        if (!string.IsNullOrEmpty(map.Text))
                        {
                            button.Font = new Font(button.Font.Name, map.TextSize, button.Font.Style);
                        }

                        button.BackgroundImage = Settings.ImageForButton(button, map.BackgroundImage);
                    }
                    else
                    {
                        button.Text = string.Empty;
                        button.BackgroundImage = Settings.ImageForButton(button, KeypadBackgroundImage.Blank);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Flash()
        {
            if (this.flashFsm.Start() == FlashFsm.Action.Flash)
            {
                Trace.Line(Trace.Type.Window, "Keypad Flash 1");
                this.flashButton.ForeColor = Color.LawnGreen;
            }
        }

        /// <inheritdoc />
        public void ActivationChange(Form form, bool activated)
        {
            // This is never called.
        }

        /// <summary>
        /// The keypad form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.Line(Trace.Type.Window, "Keypad FormClosing");
            this.activateTimer.Stop();

            // Do not allow the form to be closed.
            e.Cancel = true;

            // Hide it instead.
            this.form.Hide();
        }

        /// <summary>
        /// Key down handler for the keypad.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void KeyDown(object sender, KeyEventArgs e)
        {
            this.KeyHandler.ProcessKeyDown(this, e);
        }

        /// <summary>
        /// Key-press handler for the keypad.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            this.KeyHandler.ProcessKeyPress(e);
        }

        /// <summary>
        /// Key-up event handler for the keypad.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void KeyUp(object sender, KeyEventArgs e)
        {
            this.KeyHandler.ProcessKeyUp(this, e);
        }

        /// <summary>
        /// The mouse entered the keypad window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void Enter(object sender, EventArgs e)
        {
            this.KeyHandler.Enter(this);
        }

        /// <summary>
        /// A mouse button was pressed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void MouseDown(object sender, MouseEventArgs e)
        {
            Trace.Line(Trace.Type.Window, $"Keypad MouseDown {(sender as Control).Name}");
            this.activateTimer.Stop();

            // Get the control that got the click.
            if (!(sender is Wx3270.NoSelectButton button))
            {
                return;
            }

            // Annoy people no end.
            if (this.app.ProfileManager.Current.KeyClick)
            {
                this.Sound.Play(Wx3270.Sound.KeyClick);
            }

            // The tag tells us if the button has been pressed and the button release animation has not run yet.
            var tag = button.Tag as string;
            if (tag != null && tag.StartsWith("*"))
            {
                return;
            }

            // Run the actions.
            if (this.KeypadMaps.TryGetClosestMatch(
                Settings.ButtonBaseName(button.Name),
                null,
                this.Mod,
                out KeypadMap map,
                out _,
                out _,
                out _))
            {
                this.app.MacroRecorder.Record(map.Actions);
                this.BackEnd.RunActions(ActionSyntax.FormatForRun(map.Actions), B3270.RunType.Keypad, this.KeypadCompletion);
            }

            // Make sure the containing panel is now fixed size.
            if (button.Parent is Panel parentPanel)
            {
                parentPanel.AutoSize = false;
            }

            // Shift the image down and to the right a smidge.
            button.Location = new Point(button.Location.X + KeyShift, button.Location.Y + KeyShift);

            // Mark the tag.
            button.Tag = "*";

            // Schedule a timeout to move it back.
            lock (this.keyReleases)
            {
                this.keyReleases.Enqueue(new KeyRelease(DateTime.UtcNow.AddMilliseconds(KeyReleaseMilliseconds), button, e.Button == MouseButtons.Right));
                this.buttonReleaseTimer.Start();
            }
        }

        /// <summary>
        /// The keypad was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void Activated(object sender, EventArgs e)
        {
            Trace.Line(Trace.Type.Window, "Keypad Activated");

            // Give the outer panel focus, which has the odd (but intended) effect of allowing the IME
            // to apply to the keypad window.
            this.outerPanel.Focus();

            // Tell the main screen we have focus.
            this.mainWindowFlash.ActivationChange((Form)sender, true);

            // Flash the main screen, for identification.
            this.activateTimer.Start();
        }

        /// <summary>
        /// The keypad was deactivated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void Deactivated(object sender, EventArgs e)
        {
            // Tell the main screen we no longer have focus.
            this.mainWindowFlash.ActivationChange((Form)sender, false);
        }

        /// <summary>
        /// Handle a profile change.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void KeypadProfileChanged(Profile profile)
        {
            this.ApplyMaps(profile.KeypadMap);
        }

        /// <summary>
        /// Completion delegate for keypad actions.
        /// </summary>
        /// <param name="context">Call context.</param>
        /// <param name="success">Success indication.</param>
        /// <param name="text">Result or error text.</param>
        /// <param name="misc">Miscellaneous attributes.</param>
        private void KeypadCompletion(object context, bool success, string text, AttributeDict misc)
        {
            if (success && !string.IsNullOrWhiteSpace(text))
            {
                ErrorBox.Show(text, I18n.Get(Title.KeypadOutput), MessageBoxIcon.Information);
            }
            else if (!success)
            {
                ErrorBox.Show(text, I18n.Get(Title.KeypadError));
            }
        }

        /// <summary>
        /// Timer tick for the button release timer.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ButtonReleaseTick(object sender, EventArgs e)
        {
            var hide = false;

            lock (this.keyReleases)
            {
                while (this.keyReleases.Count > 0)
                {
                    var keyRelease = this.keyReleases.Peek();
                    hide |= keyRelease.HideAfter;
                    var now = DateTime.UtcNow;
                    var overdue = now - keyRelease.When;
                    if (overdue.Milliseconds >= 0)
                    {
                        // Dequeue it.
                        this.keyReleases.Dequeue();

                        // Shift the image back.
                        keyRelease.Button.Location = new Point(keyRelease.Button.Location.X - KeyShift, keyRelease.Button.Location.Y - KeyShift);

                        // Put the tag back.
                        var actions = (string)keyRelease.Button.Tag;
                        keyRelease.Button.Tag = actions.Substring(1);
                    }
                    else
                    {
                        // Set the timer for the next item.
                        this.buttonReleaseTimer.Interval = -overdue.Milliseconds;
                        this.buttonReleaseTimer.Start();
                        return;
                    }
                }

                // Stop the timer.
                this.buttonReleaseTimer.Interval = KeyReleaseMilliseconds;
                this.buttonReleaseTimer.Stop();

                if (hide)
                {
                    this.hideTimer.Start();
                }
            }
        }

        /// <summary>
        /// The activate timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ActivateTimer_Tick(object sender, EventArgs e)
        {
            this.activateTimer.Stop();
            this.mainWindowFlash.Flash();
        }

        /// <summary>
        /// The hide timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HideTick(object sender, EventArgs e)
        {
            this.hideTimer.Stop();
            this.form.Hide();
        }

        /// <summary>
        /// Localized message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Output from a keypad action.
            /// </summary>
            public static readonly string KeypadOutput = I18n.Combine(TitleName, "keypadOutput");

            /// <summary>
            /// Error from a keypad action.
            /// </summary>
            public static readonly string KeypadError = I18n.Combine(TitleName, "keypadError");
        }
    }
}
