// <copyright file="KeyCapture.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Input;

    /// <summary>
    /// Event handler for a captured key.
    /// </summary>
    /// <param name="keyCode">Key code.</param>
    /// <param name="modifiers">Set of modifiers.</param>
    public delegate void KeyEventHandler(Keys keyCode, Keys modifiers);

    /// <summary>
    /// Event handler for aborts.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    public delegate void AbortEventHandler(string errorMessage);

    /// <summary>
    /// Key capture overlay window.
    /// </summary>
    public partial class KeyCapture
    {
        /// <summary>
        /// The modifiers.
        /// </summary>
        private const Keys AllModifiers = Keys.Shift | Keys.Control | Keys.Alt;

        /// <summary>
        /// The set of modifier keys that were pressed.
        /// </summary>
        private readonly HashSet<Keys> modsPressed = new HashSet<Keys>();

        /// <summary>
        /// The set of data keys that were pressed.
        /// </summary>
        private readonly HashSet<Keys> dataPressed = new HashSet<Keys>();

        /// <summary>
        /// The number of pending keypad enter key up events.
        /// </summary>
        private int keypadEnterUp;

        /// <summary>
        /// The number of pending keypad enter key down events.
        /// </summary>
        private int keypadEnterDown;

        /// <summary>
        /// Key event.
        /// </summary>
        public event KeyEventHandler KeyEvent = (keyCode, modifiers) => { };

        /// <summary>
        /// Abort event.
        /// </summary>
        public event AbortEventHandler AbortEvent = (errorMessage) => { };

        /// <summary>
        /// Process a keyboard message.
        /// </summary>
        /// <param name="msg">Message value.</param>
        /// <param name="keyData">Key data.</param>
        /// <returns>True if message was processed.</returns>
        public bool CmdKey(Message msg, Keys keyData)
        {
            if ((int)msg.WParam != (int)Keys.Return || ((int)msg.LParam & 0x01000000) == 0)
            {
                // Not an extended return key.
                return false;
            }

            switch (msg.Msg)
            {
                case NativeMethods.WM_KEYDOWN:
                    this.keypadEnterDown++;
                    break;
                case NativeMethods.WM_KEYUP:
                    this.keypadEnterUp++;
                    break;
            }

            return false;
        }

        /// <summary>
        /// A key was pressed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var keyCode = e.KeyCode;
            Trace.Line(Trace.Type.Key, "KeyCapture_KeyDown({0})", keyCode);

            if (e.Alt)
            {
                // Prevents annoying beep when Alt pressed with a data key.
                e.SuppressKeyPress = true;
            }

            switch (keyCode)
            {
                case Keys.ShiftKey:
                    if (!this.modsPressed.Contains(Keys.LShiftKey) && Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        this.modsPressed.Add(Keys.LShiftKey);
                    }
                    else if (!this.modsPressed.Contains(Keys.RShiftKey) && Keyboard.IsKeyDown(Key.RightShift))
                    {
                        this.modsPressed.Add(Keys.RShiftKey);
                    }

                    return;

                case Keys.ControlKey:
                    if (!this.modsPressed.Contains(Keys.LControlKey) && Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        this.modsPressed.Add(Keys.LControlKey);
                    }
                    else if (!this.modsPressed.Contains(Keys.RControlKey) && Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        this.modsPressed.Add(Keys.RControlKey);
                    }

                    return;

                case Keys.Menu:
                    if (!this.modsPressed.Contains(Keys.LMenu) && Keyboard.IsKeyDown(Key.LeftAlt))
                    {
                        this.modsPressed.Add(Keys.LMenu);
                    }
                    else if (!this.modsPressed.Contains(Keys.RMenu) && Keyboard.IsKeyDown(Key.RightAlt))
                    {
                        this.modsPressed.Add(Keys.RMenu);
                    }

                    return;

                default:
                    // Some other sort of key. Save it and its modifiers.
                    if (this.dataPressed.Count != 0)
                    {
                        this.Abort("Double data key");
                        return;
                    }

                    // Clever hack to distinguish Return from Keypad Return.
                    if (keyCode == Keys.Return && this.keypadEnterDown > 0)
                    {
                        keyCode = KeyboardUtil.NumPadReturn;
                    }

                    this.dataPressed.Add(keyCode | e.Modifiers);
                    break;
            }
        }

        /// <summary>
        /// A key was released in the <see cref="KeyCaptureForm"/> window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            var keyCode = e.KeyCode;
            Trace.Line(Trace.Type.Key, "KeyCapture_KeyUp({0})", keyCode);

            if (keyCode == Keys.ShiftKey || keyCode == Keys.ControlKey || keyCode == Keys.Menu)
            {
                if (this.modsPressed.Count > 1)
                {
                    // Figure out which key was released.
                    switch (keyCode)
                    {
                        case Keys.ShiftKey:
                            if (!Keyboard.IsKeyDown(Key.LeftShift))
                            {
                                this.modsPressed.Remove(Keys.LShiftKey);
                            }

                            if (!Keyboard.IsKeyDown(Key.RightShift))
                            {
                                this.modsPressed.Remove(Keys.RShiftKey);
                            }

                            break;

                        case Keys.ControlKey:
                            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                this.modsPressed.Remove(Keys.LControlKey);
                            }

                            if (!Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                this.modsPressed.Remove(Keys.RControlKey);
                            }

                            break;

                        case Keys.Menu:
                            if (!Keyboard.IsKeyDown(Key.LeftAlt))
                            {
                                this.modsPressed.Remove(Keys.LMenu);
                            }

                            if (!Keyboard.IsKeyDown(Key.RightAlt))
                            {
                                this.modsPressed.Remove(Keys.RMenu);
                            }

                            break;
                    }

                    return;
                }

                if (this.modsPressed.Count == 1 && !this.dataPressed.Any())
                {
                    // They released the remaining modifier.
                    Trace.Line(Trace.Type.Key, "KeyCapture_KeyUp success");
                    Trace.Line(Trace.Type.Key);
                    this.KeyEvent(this.modsPressed.FirstOrDefault(), Keys.None);
                }

                return;
            }

            // Data key up.
            if (this.dataPressed.Count == 0
                && (keyCode == Keys.Tab
                    || keyCode == Keys.Left
                    || keyCode == Keys.Right
                    || keyCode == Keys.Up
                    || keyCode == Keys.Down))
            {
                // Funky case where the Form eats KeyDown events for navigation keys,
                // but feeds us the KeyUp events.
                // The happens with Tab when some control has a TabStop set to true, and
                // with arrow keys when a control gets focus.
                this.KeyEvent(keyCode, e.KeyData & AllModifiers);
            }
            else
            {
                this.KeyEvent(
                    this.dataPressed.FirstOrDefault() & ~AllModifiers,
                    this.dataPressed.FirstOrDefault() & AllModifiers);
            }
        }

        /// <summary>
        /// Reset the pending state.
        /// </summary>
        public void Reset()
        {
            this.modsPressed.Clear();
            this.dataPressed.Clear();
        }

        /// <summary>
        /// Abort the capture.
        /// </summary>
        /// <param name="reason">Reason for abort.</param>
        private void Abort(string reason)
        {
            Trace.Line(Trace.Type.Key, "KeyCapture abort: {0}", reason);
            Trace.Line(Trace.Type.Key);
            this.AbortEvent(reason);
            this.Reset();
        }
    }
}
