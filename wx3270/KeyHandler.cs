// <copyright file="KeyHandler.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Input;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Keyboard input processing.
    /// </summary>
    public class KeyHandler : IKeyHandler
    {
        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(KeyHandler));

        /// <summary>
        /// The application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// Individual (left and right) modifier keys known to be pressed.
        /// </summary>
        private readonly HashSet<Keys> modsPressed = new HashSet<Keys>();

        /// <summary>
        /// Other keys that are known to be pressed.
        /// </summary>
        private readonly HashSet<Keys> nonModsPressed = new HashSet<Keys>();

        /// <summary>
        /// UTF32 encoder for identifying keys outside of the Unicode BMP.
        /// </summary>
        private UTF32Encoding utf32 = new UTF32Encoding(!BitConverter.IsLittleEndian, false, true);

        /// <summary>
        /// True if a data key or second modifier was seen while a modifier was pressed.
        /// </summary>
        private bool modDisqualified;

        /// <summary>
        /// Keypad enter key down transitions.
        /// </summary>
        private int keypadEnterDown;

        /// <summary>
        /// Keypad enter key up transitions.
        /// </summary>
        private int keypadEnterUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyHandler"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        public KeyHandler(Wx3270App app)
        {
            this.app = app;
        }

        /// <summary>
        /// Gets the emulator back end instance.
        /// </summary>
        private IBackEnd BackEnd => this.app.BackEnd;

        /// <summary>
        /// Gets the sound handler.
        /// </summary>
        private ISound Sound => this.app.Sound;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.KeyOutput, "Key Action");
            I18n.LocalizeGlobal(Title.KeyError, "Key");
        }

        /// <summary>
        /// Returns all of the enumeration names for a Keys value.
        /// </summary>
        /// <param name="key">Key value.</param>
        /// <returns>Vector of names.</returns>
        public static IEnumerable<string> EnumNames(Keys key)
        {
            foreach (var e in Enum.GetNames(typeof(Keys)).Where(n => Enum.TryParse(n, out Keys k) && k == key))
            {
                yield return e;
            }
        }

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
        /// Process a KeyDown event.
        /// </summary>
        /// <param name="shift">Shift/Alt processing interface.</param>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// This is where keyboard maps catch events.
        /// </remarks>
        public void ProcessKeyDown(IShift shift, System.Windows.Forms.KeyEventArgs e)
        {
            Trace.Line(Trace.Type.Key, "ProcessKeyDown: Code {0} Modifiers {1} Chord {2}", e.KeyCode, e.Modifiers, this.app.ChordName ?? "(none)");

            if (this.app.ProfileManager.Current.KeyClick)
            {
                this.Sound.Play(Wx3270.Sound.KeyClick);
            }

            // Propagate modifier changes to the OIA.
            shift.ModChanged(
                ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) ? KeyboardModifier.Shift : 0) |
                ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) ? KeyboardModifier.Ctrl : 0) |
                ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) ? KeyboardModifier.Alt : 0),
                KeyboardModifier.NaturalModifiers);

            if (e.Alt)
            {
                // Prevents annoying beep when Alt pressed with a data key.
                // This does not appear to be needed any more.
                // e.SuppressKeyPress = true;
            }

            // Handle a modifier key separately, because there might have a mapping for it, but it is
            // processed at the Up transition.
            var keyCode = e.KeyCode;
            if (keyCode == Keys.ShiftKey || keyCode == Keys.ControlKey || keyCode == Keys.Menu)
            {
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

                        break;

                    case Keys.ControlKey:
                        if (!this.modsPressed.Contains(Keys.LControlKey) && Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            this.modsPressed.Add(Keys.LControlKey);
                        }
                        else if (!this.modsPressed.Contains(Keys.RControlKey) && Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            this.modsPressed.Add(Keys.RControlKey);
                        }

                        break;

                    case Keys.Menu:
                        if (!this.modsPressed.Contains(Keys.LMenu) && Keyboard.IsKeyDown(Key.LeftAlt))
                        {
                            this.modsPressed.Add(Keys.LMenu);
                        }
                        else if (!this.modsPressed.Contains(Keys.RMenu) && Keyboard.IsKeyDown(Key.RightAlt))
                        {
                            this.modsPressed.Add(Keys.RMenu);
                        }

                        break;
                }

                Trace.Line(Trace.Type.Key, "ProcessKeyDown: mods -> {0}", string.Join(", ", this.modsPressed));

                if (this.nonModsPressed.Any() || this.modsPressed.Count > 1)
                {
                    // Modifier pressed while non-modifier down, or more than one
                    // modifier pressed.
                    this.modDisqualified = true;
                    Trace.Line(Trace.Type.Key, "ProcessKeyDown: modDisqualified -> {0}", this.modDisqualified);
                }

                return;
            }

            // Synthesize the keypad return key.
            if (keyCode == Keys.Return && this.keypadEnterDown > 0)
            {
                --this.keypadEnterDown;
                keyCode = KeyboardUtil.NumPadReturn;
            }

            // Look up a keyboard mapping for this key.
            var scanCode = KeyboardUtil.VkeyToScanCode(keyCode, InputLanguage.CurrentInputLanguage);
            Trace.Line(Trace.Type.Key, "ProcessKeyDown: scanCode {0:X}", scanCode);
            foreach (var keyName in new[] { keyCode.ToStringExtended() }.Concat(EnumNames(keyCode)))
            {
                if (this.app.ProfileManager.Current.KeyboardMap.TryGetClosestMatch(
                    keyName,
                    KeyHelper.ScanName(scanCode),
                    this.ModifiersToKeyboardModifier(e.Modifiers),
                    this.app.ChordName,
                    out KeyboardMap map))
                {
                    this.app.MacroRecorder.Record(map.Actions);
                    var actions = ActionSyntax.FormatForRun(map.Actions);
                    Trace.Line(Trace.Type.Key, " ==> {0}", actions);
                    var keyAction = new KeyAction();
                    this.BackEnd.RunActions(actions, B3270.RunType.Keymap, this.KeyDone, keyAction, this.KeyNested);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                }
            }

            if (this.app.ChordName != null)
            {
                Trace.Line(Trace.Type.Key, "(KeyDown ignored -- chord)");
                this.app.ChordReset();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            // Remember this key.
            this.nonModsPressed.Add(keyCode);

            if (this.modsPressed.Any())
            {
                // Non-modifier pressed while modifier down.
                this.modDisqualified = true;
                Trace.Line(Trace.Type.Key, "ProcessKeyDown: modDisqualified -> {0}", this.modDisqualified);
            }
        }

        /// <summary>
        /// Process a KeyUp event.
        /// </summary>
        /// <param name="shift">Shift processing interface.</param>
        /// <param name="e">Event arguments.</param>
        public void ProcessKeyUp(IShift shift, System.Windows.Forms.KeyEventArgs e)
        {
            Trace.Line(Trace.Type.Key, "ProcessKeyUp: Code {0} Modifiers {1} Chord {2}", e.KeyCode, e.Modifiers, this.app.ChordName ?? "(none)");

            // Propagate modifier changes to the OIA.
            shift.ModChanged(
                ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) ? KeyboardModifier.Shift : 0) |
                ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) ? KeyboardModifier.Ctrl : 0) |
                ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) ? KeyboardModifier.Alt : 0),
                KeyboardModifier.NaturalModifiers);

            var keyCode = e.KeyCode;
            if (keyCode != Keys.ShiftKey && keyCode != Keys.ControlKey && keyCode != Keys.Menu)
            {
                // Forget this key.
                this.nonModsPressed.Remove(keyCode);
                return;
            }

            // Modifier key.
            // Save the original modifier key count and the first saved modifier.
            var modsPressedBefore = this.modsPressed.Count;
            var first = this.modsPressed.FirstOrDefault();

            // Figure out which key was released and remove it.
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

            if (!(modsPressedBefore == 1 && !this.modsPressed.Any()))
            {
                // We didn't remove the last modifier.
                if (!this.modsPressed.Any())
                {
                    // No modifiers left, start fresh.
                    this.modDisqualified = false;
                    Trace.Line(Trace.Type.Key, "ProcessKeyUp: modDisqualified -> {0}", this.modDisqualified);
                }

                e.Handled = true;
                return;
            }

            // Last modifier released.
            Trace.Line(Trace.Type.Key, "KeyCapture_KeyUp success {0}{1}", first, this.modDisqualified ? " (ignored)" : string.Empty);
            if (!this.modDisqualified)
            {
                var scanCode = KeyboardUtil.VkeyToScanCode(first, InputLanguage.CurrentInputLanguage);
                if (this.app.ProfileManager.Current.KeyboardMap.TryGetClosestMatch(
                    first.ToStringExtended(),
                    KeyHelper.ScanName(scanCode),
                    Wx3270App.ModifierModeFromConnectionState(this.app.ConnectionState) | this.app.SyntheticModifiers,
                    this.app.ChordName,
                    out KeyboardMap map))
                {
                    this.app.MacroRecorder.Record(map.Actions);
                    var actions = ActionSyntax.FormatForRun(map.Actions);
                    Trace.Line(Trace.Type.Key, " ==> {0}", actions);
                    var keyAction = new KeyAction();
                    this.BackEnd.RunActions(actions, B3270.RunType.Keymap, this.KeyDone, keyAction, this.KeyNested);
                }

                if (this.app.ChordName != null)
                {
                    Trace.Line(Trace.Type.Key, "(KeyUp ignored -- chord)");
                    this.app.ChordReset();
                }
            }

            Trace.Line(Trace.Type.Key);

            this.modDisqualified = false;
            Trace.Line(Trace.Type.Key, "ProcessKeyUp: modDisqualified -> {0}", this.modDisqualified);
        }

        /// <summary>
        /// Process a KeyPress event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// This is where data keys (keys that generate input characters) end up if they are not caught by a keyboard map.
        /// </remarks>
        public void ProcessKeyPress(KeyPressEventArgs e)
        {
            // New emoji are outside the BMP, and come in as two UTF16 characters. Until or unless the back end supports
            // these, explicitly drop them.
            try
            {
                var output = new byte[4];
                this.utf32.GetBytes(new[] { e.KeyChar }, 0, 1, output, 0);
            }
            catch (Exception ex)
            {
                Trace.Line(Trace.Type.Key, "ProcessKeyPress: key #{0:X4} is not valid UTF16: {1}", (int)e.KeyChar, ex.Message);
                return;
            }

            if (e.KeyChar < ' ')
            {
                Trace.Line(Trace.Type.Key, "ProcessKeyPress: Char U+{0:X4}", (int)e.KeyChar);
            }
            else
            {
                Trace.Line(Trace.Type.Key, "ProcessKeyPress: Char U+{0:X4} '{1}'", (int)e.KeyChar, e.KeyChar);
            }

            if (this.app.ChordName != null)
            {
                Trace.Line(Trace.Type.Key, "(KeyPress ignored -- chord)");
                this.app.ChordReset();
                return;
            }

            // Input the data.
            var code = string.Format("U+{0:X4}", (int)e.KeyChar);
            this.app.MacroRecorder.Record($"{B3270.Action.Key}({code})");
            this.BackEnd.RunAction(new BackEndAction(B3270.Action.Key, code), B3270.RunType.Keymap, this.KeyDone);
            if (this.app.ChordName != null)
            {
                this.app.ChordReset();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Enter the form.
        /// </summary>
        /// <param name="shift">Shift interface.</param>
        public void Enter(IShift shift)
        {
            // Update the OIA.
            shift.ModChanged(
                ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) ? KeyboardModifier.Shift : 0) |
                ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) ? KeyboardModifier.Ctrl : 0) |
                ((Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) ? KeyboardModifier.Alt : 0),
                KeyboardModifier.NaturalModifiers);

            // Flush the pending state.
            this.modsPressed.Clear();
            this.nonModsPressed.Clear();
            this.modDisqualified = Keyboard.IsKeyDown(Key.LeftShift)
                || Keyboard.IsKeyDown(Key.RightShift)
                || Keyboard.IsKeyDown(Key.LeftCtrl)
                || Keyboard.IsKeyDown(Key.RightCtrl)
                || Keyboard.IsKeyDown(Key.LeftAlt)
                || Keyboard.IsKeyDown(Key.RightAlt);
            Trace.Line(Trace.Type.Key, "Enter: modDisqualified -> {0}", this.modDisqualified);
        }

        /// <summary>
        /// Translate a set of modifier keys to a keyboard modifier.
        /// </summary>
        /// <param name="modifiers">Set of modifier keys.</param>
        /// <returns>Translated modifiers.</returns>
        private KeyboardModifier ModifiersToKeyboardModifier(Keys modifiers)
        {
            return (modifiers.HasFlag(Keys.Shift) ? KeyboardModifier.Shift : KeyboardModifier.None)
                | (modifiers.HasFlag(Keys.Control) ? KeyboardModifier.Ctrl : KeyboardModifier.None)
                | (modifiers.HasFlag(Keys.Alt) ? KeyboardModifier.Alt : KeyboardModifier.None)
                | this.app.SyntheticModifiers;
        }

        /// <summary>
        /// Completion method for a key action.
        /// </summary>
        /// <param name="cookie">Call context.</param>
        /// <param name="success">Success or failure.</param>
        /// <param name="text">Error message or result.</param>
        /// <param name="misc">Miscellaneous attributes.</param>
        private void KeyDone(object cookie, bool success, string text, AttributeDict misc)
        {
            if (success && !string.IsNullOrEmpty(text))
            {
                ErrorBox.Show(text, I18n.Get(Title.KeyOutput), MessageBoxIcon.Information);
            }
            else if (!success)
            {
                ErrorBox.Show(text, I18n.Get(Title.KeyError));
            }

            if (cookie != null)
            {
                // If what the key did did not include a keyboard select action, undo all selections.
                var keyAction = (KeyAction)cookie;
                Trace.Line(Trace.Type.Key, "Passthru actions performed by key mapping: {0}", string.Join(",", keyAction.NestedActions));
                if (!keyAction.NestedActions.Contains(Constants.Action.KeyboardSelect, StringComparer.InvariantCultureIgnoreCase))
                {
                    this.app.SelectionManager.Clear();
                }
            }
        }

        /// <summary>
        /// A nested pass-through action was called by a keyboard map.
        /// </summary>
        /// <param name="cookie">Call context.</param>
        /// <param name="actionName">Action name.</param>
        private void KeyNested(object cookie, string actionName)
        {
            if (cookie != null)
            {
                var keyAction = (KeyAction)cookie;
                keyAction.Add(actionName);
            }
        }

        /// <summary>
        /// Pending key action state.
        /// </summary>
        private class KeyAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyAction"/> class.
            /// </summary>
            public KeyAction()
            {
                this.NestedActions = new HashSet<string>();
            }

            /// <summary>
            /// Gets the nested actions that were called.
            /// </summary>
            public HashSet<string> NestedActions { get; private set; }

            /// <summary>
            /// Adds a nested pass-through action to the set of actions.
            /// </summary>
            /// <param name="action">Name of pass-through action.</param>
            public void Add(string action)
            {
                this.NestedActions.Add(action);
            }
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Key action output.
            /// </summary>
            public static readonly string KeyOutput = I18n.Combine(TitleName, "keyOutput");

            /// <summary>
            /// Key action error.
            /// </summary>
            public static readonly string KeyError = I18n.Combine(TitleName, "keyError");
        }
    }
}
