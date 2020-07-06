// <copyright file="KeyCaptureForm.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    /// <summary>
    /// Key capture overlay window.
    /// </summary>
    public partial class KeyCaptureForm : Form
    {
        /// <summary>
        /// Key capture state machine.
        /// </summary>
        private readonly KeyCapture keyCapture = new KeyCapture();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyCaptureForm"/> class.
        /// </summary>
        public KeyCaptureForm()
        {
            this.InitializeComponent();

            this.keyCapture.KeyEvent += this.OnKeyEvent;
            this.keyCapture.AbortEvent += this.OnAbortEvent;

            I18n.Localize(this);
        }

        /// <summary>
        /// Gets the key that was pressed.
        /// </summary>
        public Keys KeyCode { get; private set; }

        /// <summary>
        /// Gets the keyboard modifiers.
        /// </summary>
        public Keys Modifiers { get; private set; }

        /// <summary>
        /// Gets the data character.
        /// </summary>
        public char? KeyChar { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the key is dead.
        /// </summary>
        public bool Dead { get; private set; }

        /// <summary>
        /// Gets the reason for an abort.
        /// </summary>
        public string AbortReason { get; private set; }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void LocalizeForm()
        {
            var capture = new KeyCaptureForm();
        }

        /// <summary>
        /// Override for key event processing.
        /// </summary>
        /// <param name="msg">Message received</param>
        /// <param name="keyData">Key data</param>
        /// <returns>True if message was processed</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.keyCapture.CmdKey(msg, keyData))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Key event handler.
        /// </summary>
        /// <param name="keyCode">Key code</param>
        /// <param name="modifiers">Set of modifiers</param>
        private void OnKeyEvent(Keys keyCode, Keys modifiers)
        {
            // Clear out the key state.
            KeyboardUtil.ClearKeyboardState();

            this.KeyCode = keyCode;
            this.KeyChar = KeyboardUtil.FromVkey(keyCode, modifiers, out bool isDead);
            this.Dead = isDead;
            this.Modifiers = modifiers;
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        /// <summary>
        /// Abort event handler.
        /// </summary>
        /// <param name="reason">Reason for abort</param>
        private void OnAbortEvent(string reason)
        {
            this.AbortReason = reason;
            this.DialogResult = DialogResult.Abort;
            this.Hide();
        }

        /// <summary>
        /// A key was pressed in the <see cref="KeyCaptureForm"/> window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyCapture_KeyDown(object sender, KeyEventArgs e)
        {
            this.keyCapture.KeyDown(sender, e);
        }

        /// <summary>
        /// A key was released in the <see cref="KeyCaptureForm"/> window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyCapture_KeyUp(object sender, KeyEventArgs e)
        {
            this.keyCapture.KeyUp(sender, e);
        }

        /// <summary>
        /// Mouse click method for the key capture screen.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyCapture_MouseClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }
    }
}
