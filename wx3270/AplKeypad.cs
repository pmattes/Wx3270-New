// <copyright file="AplKeypad.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;

    using Wx3270.Contracts;
    using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

    /// <summary>
    /// The APL pop-up keypad.
    /// </summary>
    public partial class AplKeypad : Form, IShift, IFlash
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
        /// Initializes a new instance of the <see cref="AplKeypad"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="flash">Flash interface.</param>
        public AplKeypad(Wx3270App app, IFlash flash)
        {
            this.InitializeComponent();
            this.app = app;
            this.keypadCommon = new KeypadCommon(app, this, flash, new[] { this.keyboardPanel }, this.keypadOuterPanel, this.graveButton);

            // Localize.
            this.Text = I18n.Localize(this, "wx3270 APL Keypad");
        }

        /// <summary>
        /// Gets the current keyboard modifier state.
        /// </summary>
        public KeyboardModifier Mod => this.keypadCommon.Mod;

        /// <summary>
        /// Register an opacity event.
        /// </summary>
        /// <param name="opacity">Opacity interface.</param>
        public void RegisterOpacity(IOpacity opacity)
        {
            opacity.OpacityEvent += (percent) => this.Opacity = percent / 100.0;
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

        /// <summary>
        /// Flash to indicate where the window is.
        /// </summary>
        public void Flash()
        {
            this.keypadCommon.Flash();
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
        }
    }
}
