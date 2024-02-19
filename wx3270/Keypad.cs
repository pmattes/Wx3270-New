// <copyright file="Keypad.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using I18nBase;

    using Wx3270.Contracts;
    using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

    /// <summary>
    /// The pop-up keypad.
    /// </summary>
    public partial class Keypad : Form, IShift, IFlash
    {
        /// <summary>
        /// Application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// Common keypad logic.
        /// </summary>
        private readonly KeypadCommon keypadCommon;

        /// <summary>
        /// Lock for <see cref="toured"/>.
        /// </summary>
        private readonly object tourLock = new object();

        /// <summary>
        /// True if the tour has been run.
        /// </summary>
        private bool toured;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keypad"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="flash">Flash interface.</param>
        /// <param name="opacity">Opacity interface.</param>
        public Keypad(Wx3270App app, IFlash flash, IOpacity opacity)
        {
            this.InitializeComponent();
            this.app = app;
            if (app != null)
            {
                this.keypadCommon = new KeypadCommon(app, this, flash, new[] { this.newLeftPanel, this.newMiddlePanel, this.newRightPanel }, this.keypadOuterPanel, this.PF1button);
                this.Opacity = app.ProfileManager.Current.OpacityPercent / 100.0;
            }

            if (opacity != null)
            {
                opacity.OpacityEvent += (percent) => this.Opacity = percent / 100.0;
            }

            // Localize.
            this.Text = I18n.Localize(this, "wx3270 Keypad");
            I18n.Localize(this, this.toolTip1);
        }

        /// <summary>
        /// Gets the current keyboard modifier state.
        /// </summary>
        public KeyboardModifier Mod => this.keypadCommon.Mod;

        /// <summary>
        /// Static form localization.
        /// </summary>
        [I18nFormInit]
        public static void FormLocalize()
        {
            new Keypad(null, null, null).Dispose();
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Click on a key.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Keypad), nameof(PF1button)), "Tour: Keypad keys");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Keypad), nameof(PF1button)),
@"Click on any of the keys to perform the listed function or enter the displayed character.

Right-click to perform the function and close the keypad window.

The Shift, Ctrl and Alt modifier keys may change the label and behavior of a key.

You may continue to type on the keyboard while the keypad window has focus.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Keypad), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Keypad), nameof(helpPictureBox)),
@"Click to display context-sensitive help from the x3270 Wiki in your browser, or to start this tour again.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// The keyboard modifiers changed.
        /// </summary>
        /// <param name="mod">New modifiers.</param>
        /// <param name="mask">Modifier mask.</param>
        public void ModChanged(KeyboardModifier mod, KeyboardModifier mask)
        {
            this.keypadCommon.ModChanged(mod, mask);
        }

        /// <summary>
        /// Apply a new set of keypad maps.
        /// </summary>
        /// <param name="maps">New maps.</param>
        public void ApplyMaps(KeyMap<KeypadMap> maps)
        {
            this.keypadCommon.ApplyMaps(maps);
        }

        /// <inheritdoc />
        public void Flash()
        {
            this.keypadCommon.Flash();
        }

        /// <inheritdoc />
        public void ActivationChange(Form form, bool activated)
        {
        }

        /// <summary>
        /// Override for key event processing.
        /// </summary>
        /// <param name="msg">Message received.</param>
        /// <param name="keyData">Key data.</param>
        /// <returns>True if message was processed.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.app.KeyHandler.CmdKey(msg, keyData))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Run the tour.
        /// </summary>
        private void RunTour()
        {
            var nodes = new[]
            {
                ((Control)this.PF1button, (int?)null, Orientation.UpperLeft),
                (this.helpPictureBox, null, Orientation.LowerRight),
            };
            Tour.Navigate(this, nodes);
        }

        /// <summary>
        /// The keypad form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.keypadCommon.FormClosing(sender, e);
        }

        /// <summary>
        /// Key down handler for the keypad.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_KeyDown(object sender, KeyEventArgs e)
        {
            this.keypadCommon.KeyDown(sender, e);
        }

        /// <summary>
        /// Key-press handler for the keypad.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.keypadCommon.KeyPress(sender, e);
        }

        /// <summary>
        /// Key-up event handler for the keypad.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_KeyUp(object sender, KeyEventArgs e)
        {
            this.keypadCommon.KeyUp(sender, e);
        }

        /// <summary>
        /// The mouse entered the keypad window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_Enter(object sender, EventArgs e)
        {
            this.keypadCommon.Enter(sender, e);
        }

        /// <summary>
        /// A mouse button was pressed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadMouseDown(object sender, MouseEventArgs e)
        {
            this.keypadCommon.MouseDown(sender, e);
        }

        /// <summary>
        /// The keypad was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_Activated(object sender, EventArgs e)
        {
            this.keypadCommon.Activated(sender, e);

            lock (this.tourLock)
            {
                if (!this.toured && !Tour.IsComplete(this))
                {
                    this.toured = true;
                    new TaskFactory().StartNew(() => this.Invoke(new MethodInvoker(() => this.RunTour())));
                }
            }
        }

        /// <summary>
        /// The keypad is being deactivated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Keypad_Deactivate(object sender, EventArgs e)
        {
            this.keypadCommon.Deactivated(sender, e);
        }

        /// <summary>
        /// The Help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// An item from the Help menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "Keypad", this.RunTour);
        }
    }
}
