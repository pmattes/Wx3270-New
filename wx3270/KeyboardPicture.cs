// <copyright file="KeyboardPicture.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Microsoft.Win32;

    /// <summary>
    /// Keyboard picture mode.
    /// </summary>
    public enum PictureMode
    {
        /// <summary>
        /// Defining a keymap entry.
        /// </summary>
        Select,

        /// <summary>
        /// Displaying the keymap.
        /// </summary>
        Display,
    }

    /// <summary>
    /// The graphical keyboard map display.
    /// </summary>
    ////[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public partial class KeyboardPicture : Form
    {
        /// <summary>
        /// Small font size.
        /// </summary>
        private const float SmallFontSize = 8.25F;

        /// <summary>
        /// Large font size.
        /// </summary>
        private const float LargeFontSize = 11.25F;

        /// <summary>
        /// Message name.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(KeyboardPicture));

        /// <summary>
        /// Dictionary of key labels. Maps a virtual key name (button tag) to the label text in the en-US culture.
        /// </summary>
        private readonly Dictionary<string, string> labelDictionary = new Dictionary<string, string>
        {
            { "RWin", "Win" },
            { "Space", string.Empty },
            { "LControlKey", "Ctrl" },
            { "RControlKey", "Ctrl" },
            { "Apps", "Apps" },
            { "RMenu", "Alt" },
            { "LMenu", "Alt" },
            { "LWin", "Win" },
            { "RShiftKey", "Shift" },
            { "LShiftKey", "Shift" },
            { "OemQuestion", "?" + Environment.NewLine + Environment.NewLine + "/" },
            { "OemPeriod", ">" + Environment.NewLine + Environment.NewLine + "." },
            { "Oemcomma", "<" + Environment.NewLine + Environment.NewLine + "," },
            { "M", "M" },
            { "N", "N" },
            { "B", "B" },
            { "V", "V" },
            { "C", "C" },
            { "X", "X" },
            { "Z", "Z" },
            { "Return", "Enter" },
            { "Capital", "Caps Lock" },
            { "Oem7", "\"" + Environment.NewLine + Environment.NewLine + "'" },
            { "Oem1", ":" + Environment.NewLine + Environment.NewLine + ";" },
            { "L", "L" },
            { "K", "K" },
            { "J", "J" },
            { "H", "H" },
            { "G", "G" },
            { "F", "F" },
            { "D", "D" },
            { "S", "S" },
            { "A", "A" },
            { "D0", ")" + Environment.NewLine + Environment.NewLine + "0" },
            { "Oem5", "|" + Environment.NewLine + Environment.NewLine + "\\" },
            { "Oem6", "}" + Environment.NewLine + Environment.NewLine + "]" },
            { "OemOpenBrackets", "{" + Environment.NewLine + Environment.NewLine + "[" },
            { "P", "P" },
            { "O", "O" },
            { "I", "I" },
            { "U", "U" },
            { "Y", "Y" },
            { "T", "T" },
            { "R", "R" },
            { "E", "E" },
            { "W", "W" },
            { "Q", "Q" },
            { "Tab", "Tab" },
            { "Back", "Backspace" },
            { "Oemplus", "+" + Environment.NewLine + Environment.NewLine + "=" },
            { "OemMinus", "_" + Environment.NewLine + Environment.NewLine + "-" },
            { "D9", "(" + Environment.NewLine + Environment.NewLine + "9" },
            { "D8", "*" + Environment.NewLine + Environment.NewLine + "8" },
            { "D7", "&" + Environment.NewLine + Environment.NewLine + "7" },
            { "D6", "^" + Environment.NewLine + Environment.NewLine + "6" },
            { "D5", "%" + Environment.NewLine + Environment.NewLine + "5" },
            { "D4", "$" + Environment.NewLine + Environment.NewLine + "4" },
            { "D3", "#" + Environment.NewLine + Environment.NewLine + "3" },
            { "D2", "@" + Environment.NewLine + Environment.NewLine + "2" },
            { "D1", "!" + Environment.NewLine + Environment.NewLine + "1" },
            { "Oemtilde", "~" + Environment.NewLine + Environment.NewLine + "`" },
            { "Add", "+" },
            { "NumPad0 Insert", "0" + Environment.NewLine + Environment.NewLine + "Ins" },
            { "Decimal Delete", "." + Environment.NewLine + Environment.NewLine + "Del" },
            { "NumPad3 Next", "3" + Environment.NewLine + Environment.NewLine + "PgDn" },
            { "NumPad2 Down", "2" + Environment.NewLine + Environment.NewLine + "↓" },
            { "NumPad1 End", "1" + Environment.NewLine + Environment.NewLine + "End" },
            { "NumPad6 Right", "6" + Environment.NewLine + Environment.NewLine + "→" },
            { "NumPad5 Clear", "5" + Environment.NewLine + Environment.NewLine },
            { "NumPad4 Left", "4" + Environment.NewLine + Environment.NewLine + "←" },
            { "NumPad9 PageUp", "9" + Environment.NewLine + Environment.NewLine + "PgUp" },
            { "NumPad8 Up", "8" + Environment.NewLine + Environment.NewLine + "↑" },
            { "NumPad7 Home", "7" + Environment.NewLine + Environment.NewLine + "Home" },
            { "Subtract", "-" },
            { "Multiply", "*" },
            { "Divide", "/" },
            { "NumLock", "Num" + Environment.NewLine + "Lock" },
            { "NumPadReturn", "Enter" },
            { "Right", "→" },
            { "Down", "↓" },
            { "Left", "←" },
            { "Up", "↑" },
            { "Next", "Page" + Environment.NewLine + "Down" },
            { "End", "End" },
            { "Delete", "Delete" },
            { "PageUp", "Page" + Environment.NewLine + "Up" },
            { "Home", "Home" },
            { "Insert", "Insert" },
            { "Pause", "Pause" + Environment.NewLine + Environment.NewLine + "Break" },
            { "Scroll", "Scroll" + Environment.NewLine + "Lock" },
            { "PrintScreen", "PrtScn" + Environment.NewLine + Environment.NewLine + "SysRq" },
            { "F12", "F12" },
            { "F11", "F11" },
            { "F10", "F10" },
            { "F9", "F9" },
            { "F8", "F8" },
            { "F7", "F7" },
            { "F6", "F6" },
            { "F5", "F5" },
            { "F4", "F4" },
            { "F3", "F3" },
            { "F2", "F2" },
            { "F1", "F1" },
            { "Escape", "Esc" },
        };

        /// <summary>
        /// Localized label dictionary.
        /// </summary>
        private readonly Dictionary<string, string> localizedLabelDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Key capture state machine.
        /// </summary>
        private readonly KeyCapture keyCapture = new KeyCapture();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardPicture"/> class.
        /// </summary>
        /// <param name="map">Keyboard map to display.</param>
        /// <param name="mode">Picture mode.</param>
        /// <param name="app">Application context.</param>
        public KeyboardPicture(KeyMap<KeyboardMap> map, PictureMode mode = PictureMode.Select, Wx3270App app = null)
        {
            this.InitializeComponent();

            this.Map = map;
            this.PictureMode = mode;

            this.keyCapture.KeyEvent += this.OnKeyEvent;
            this.keyCapture.AbortEvent += this.OnAbortEvent;

            // Set up the chord selector.
            this.chordComboBox.SelectedIndex = 0;

            // Localize the entire form.
            I18n.Localize(this, this.toolTip1);

            // Remember the localized labels.
            foreach (var label in this.keysPanel.Controls.OfType<Label>())
            {
                string tag;
                if (label.Tag == null || string.IsNullOrEmpty(tag = (string)label.Tag))
                {
                    continue;
                }

                if (!tag.StartsWith("="))
                {
                    this.localizedLabelDictionary[tag] =
                        label.Name == "spaceBar" ? string.Empty : I18n.Get(I18n.Path(label));
                }
            }

            // Handle restrictions.
            if (app != null && app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Finish up painting the window.
            if (mode == PictureMode.Display)
            {
                this.definitionsRadioButton.Checked = true;
                this.DisplayDefinitions();
                this.ChordIndex = 0;
                if (app != null)
                {
                    app.ProfileManager.Change += (newProfile) =>
                    {
                        this.Map = newProfile.KeyboardMap;
                        this.ChordIndex = 0;
                    };
                }
            }
            else
            {
                this.DisplayLabels();
            }
        }

        /// <summary>
        /// Gets the selected modifiers.
        /// </summary>
        public Keys SelectedModifiers { get; private set; }

        /// <summary>
        /// Gets a value indicating whether APL mode is set.
        /// </summary>
        public bool Apl { get; private set; }

        /// <summary>
        /// Gets the key that was pressed.
        /// </summary>
        public Keys KeyCode { get; private set; }

        /// <summary>
        /// Gets the scan code of the key that was pressed.
        /// </summary>
        public uint ScanCode { get; private set; }

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
        /// Gets or sets the keyboard map.
        /// </summary>
        public KeyMap<KeyboardMap> Map { get; set; }

        /// <summary>
        /// Gets or sets the chord index.
        /// </summary>
        public int ChordIndex
        {
            get
            {
                return this.chordComboBox.SelectedIndex;
            }

            set
            {
                this.chordComboBox.Items.Clear();
                this.chordComboBox.Items.AddRange(this.Map.Chords().ToArray());
                this.chordComboBox.SelectedIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the current keyboard modifier state.
        /// </summary>
        public KeyboardModifier KeyboardModifier
        {
            get
            {
                var ret = KeyboardModifier.None;
                foreach (var checkBox in this.modifiersTable.Controls.OfType<CheckBox>())
                {
                    string tag;
                    if (checkBox.Checked && checkBox.Tag != null && !string.IsNullOrEmpty(tag = (string)checkBox.Tag))
                    {
                        ret |= (KeyboardModifier)Enum.Parse(typeof(KeyboardModifier), tag);
                    }
                }

                if (this.mode3270CheckBox.Checked)
                {
                    ret |= KeyboardModifier.Mode3270;
                }

                if (this.nvtModeCheckBox.Checked)
                {
                    ret |= KeyboardModifier.ModeNvt;
                }

                return ret;
            }

            set
            {
                foreach (var checkBox in this.modifiersTable.Controls.OfType<CheckBox>())
                {
                    string tag;
                    if (checkBox.Tag != null && !string.IsNullOrEmpty(tag = (string)checkBox.Tag))
                    {
                        var mod = (KeyboardModifier)Enum.Parse(typeof(KeyboardModifier), tag);
                        checkBox.Checked = (value & mod) != 0;
                    }
                }

                this.mode3270CheckBox.Checked = (value & KeyboardModifier.Mode3270) != 0;
                this.nvtModeCheckBox.Checked = (value & KeyboardModifier.ModeNvt) != 0;
            }
        }

        /// <summary>
        /// Gets or sets the picture mode.
        /// </summary>
        public PictureMode PictureMode { get; set; } = PictureMode.Select;

        /// <summary>
        /// Gets the handle for the en-US culture, or -1 if that culture is not present.
        /// </summary>
        private static IntPtr EnUsHandle
        {
            get
            {
                var culture = InputLanguage.FromCulture(new CultureInfo("en-US"));
                return (culture != null) ? culture.Handle : (IntPtr)(-1);
            }
        }

        /// <summary>
        /// Gets the current keyboard modifier state as a <see cref="Keys"/> enumeration.
        /// </summary>
        private Keys KeyboardModifierKeys
        {
            get => KeyHelper.ModifierToKeys(this.KeyboardModifier);
        }

        // Gets the current chord.
        private string ChordName
        {
            get
            {
                var selected = (string)this.chordComboBox.SelectedItem;
                return selected == Settings.NoChord ? null : selected;
            }
        }

        /// <summary>
        /// Form localizations.
        /// </summary>
        [I18nFormInit]
        public static void LocalizeForm()
        {
            I18n.LocalizeGlobal(MessageNames.Input, "Input");
            I18n.LocalizeGlobal(MessageNames.Layout, "Layout");
            new KeyboardPicture(Profile.DefaultProfile.KeyboardMap).Dispose();
        }

        /// <summary>
        /// Redisplay when the window is re-opened.
        /// </summary>
        public void Redisplay()
        {
            if (this.definitionsRadioButton.Checked)
            {
                this.DisplayDefinitions();
            }

            this.keyCapture.Reset();
        }

        /// <summary>
        /// Override for key event processing.
        /// </summary>
        /// <param name="msg">Message received.</param>
        /// <param name="keyData">Key data.</param>
        /// <returns>True if message was processed.</returns>
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
        /// <param name="keyCode">Key code.</param>
        /// <param name="modifiers">Set of modifiers.</param>
        private void OnKeyEvent(Keys keyCode, Keys modifiers)
        {
            switch (this.PictureMode)
            {
                case PictureMode.Select:
                    // Clear out the key state.
                    KeyboardUtil.ClearKeyboardState();

                    this.KeyCode = keyCode;
                    this.KeyChar = KeyboardUtil.FromVkey(keyCode, modifiers, out bool isDead);
                    this.ScanCode = KeyboardUtil.VkeyToScanCode(keyCode, InputLanguage.CurrentInputLanguage);
                    this.Dead = isDead;
                    this.SelectedModifiers = modifiers;
                    this.Apl = this.KeyboardModifier.HasFlag(KeyboardModifier.Apl);
                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                    return;

                case PictureMode.Display:
                    switch (keyCode)
                    {
                        // If they pressed a modifier, set it. This is better done as a different event, but this will do for now.
                        case Keys.ControlKey:
                        case Keys.LControlKey:
                        case Keys.RControlKey:
                            this.KeyboardModifier ^= KeyboardModifier.Ctrl;
                            break;
                        case Keys.Menu:
                        case Keys.LMenu:
                        case Keys.RMenu:
                            this.KeyboardModifier ^= KeyboardModifier.Alt;
                            break;
                        case Keys.ShiftKey:
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            this.KeyboardModifier ^= KeyboardModifier.Shift;
                            break;

                        // If they pressed a data key, see if there is a chord by that name.
                        default:
                            if (this.ChordName != null)
                            {
                                break;
                            }

                            var scanCode = KeyboardUtil.VkeyToScanCode(keyCode, InputLanguage.CurrentInputLanguage);
                            var mods = KeyHelper.KeysToModifier(modifiers) | (this.KeyboardModifier & KeyboardModifier.SyntheticModifiers);
                            if (this.Map.TryGetClosestMatch(keyCode.ToStringExtended(), KeyHelper.ScanName(scanCode), mods, null, out KeyboardMap map)
                                && KeyMap<KeyboardMap>.IsChord(map.Actions))
                            {
                                var matchIndex = this.chordComboBox.Items.OfType<string>().ToList().IndexOf(KeyMap<KeyboardMap>.DisplayChord(KeyMap<KeyboardMap>.ChordName(map.Actions)));
                                if (matchIndex >= 0)
                                {
                                    this.chordComboBox.SelectedIndex = matchIndex;
                                }
                            }

                            break;
                    }

                    this.keyCapture.Reset();
                    break;
            }
        }

        /// <summary>
        /// Abort event handler.
        /// </summary>
        /// <param name="reason">Reason for abort.</param>
        private void OnAbortEvent(string reason)
        {
            this.AbortReason = reason;
            this.DialogResult = DialogResult.Abort;
            this.Hide();
        }

        /// <summary>
        /// Clear the modifier check boxes.
        /// </summary>
        private void ClearModifiers()
        {
            // Now a no-op. Used to disable the mode group boxes.
        }

        /// <summary>
        /// Display the key labels.
        /// </summary>
        private void DisplayLabels()
        {
            this.ClearModifiers();
            this.AdjustKey56();
            this.aplLegendPanel.Visible = this.KeyboardModifier.HasFlag(KeyboardModifier.Apl);
            this.aplLegendLabel.Visible = this.KeyboardModifier.HasFlag(KeyboardModifier.Apl);

            foreach (var label in this.keysPanel.Controls.OfType<Label>())
            {
                string tag;
                if (label.Tag == null || string.IsNullOrEmpty(tag = (string)label.Tag))
                {
                    continue;
                }

                // In label mode, there are no tool tips.
                this.toolTip1.SetToolTip(label, string.Empty);

                // If the tag starts with '=', it is a virtual scan code in hex.
                // Otherwise it is a virtual key name.
                if (!tag.StartsWith("="))
                {
                    // We need to find the translation of the en-US label found in labelDictionary.
                    // label.Text = this.labelDictionary[tag];
                    label.Text = this.localizedLabelDictionary[tag];
                    label.Font = new Font(label.Font.Name, SmallFontSize);
                    continue;
                }

                // Letters get the large font.
                label.Font = new Font(label.Font.Name, LargeFontSize);

                var scanCode = uint.Parse(tag.Substring(1), NumberStyles.AllowHexSpecifier);
                var currentVkey = KeyboardUtil.ScanCodeToVkey(scanCode, InputLanguage.CurrentInputLanguage);
                if (currentVkey == 0)
                {
                    label.Text = string.Empty;
                    continue;
                }

                var currentUnshiftedChar = KeyboardUtil.FromVkey(currentVkey, Keys.None, out _);
                if (currentUnshiftedChar == null || (uint)currentUnshiftedChar.Value <= ' ')
                {
                    label.Text = string.Empty;
                    continue;
                }

                string apl = null;
                string aplShift = null;
                if (this.KeyboardModifier.HasFlag(KeyboardModifier.Apl))
                {
                    AplKeys.TryLookupDisplay((int)scanCode, false, out apl);
                    AplKeys.TryLookupDisplay((int)scanCode, true, out aplShift);
                }

                var unshifted = new string(new[] { currentUnshiftedChar.Value });
                var altGr = KeyboardUtil.FromVkey(currentVkey, Keys.RMenu, out _);
                var altGrString = altGr.HasValue && altGr.Value >= ' ' ? new string(new[] { altGr.Value }) : null;

                var currentShiftedChar = KeyboardUtil.FromVkey(currentVkey, Keys.Shift, out _);
                if (currentShiftedChar == null || (uint)currentShiftedChar.Value <= ' ')
                {
                    // No shifted value.
                    if (apl != null || aplShift != null)
                    {
                        label.Text = ((aplShift != null) ? "   " + aplShift : string.Empty)
                            + Environment.NewLine
                            + unshifted + ((apl != null) ? "   " + apl : string.Empty);
                    }
                    else
                    {
                        if (altGrString != null)
                        {
                            label.Text = unshifted + Environment.NewLine + "   " + altGrString;
                        }
                        else
                        {
                            label.Text = unshifted + Environment.NewLine;
                        }
                    }

                    continue;
                }

                var shifted = new string(new[] { currentShiftedChar.Value });
                if (unshifted == shifted
                    || (char.IsLower(currentUnshiftedChar.Value) && currentShiftedChar.Value == char.ToUpper(currentUnshiftedChar.Value)))
                {
                    // Shifted is uppercase version of unshifted.
                    if (apl != null || aplShift != null)
                    {
                        label.Text = shifted + ((aplShift != null) ? "   " + aplShift : string.Empty)
                            + Environment.NewLine
                            + ((apl != null) ? "     " + apl : string.Empty);
                    }
                    else
                    {
                        if (altGrString != null)
                        {
                            label.Text = shifted + Environment.NewLine + "   " + altGrString;
                        }
                        else
                        {
                            label.Text = shifted + Environment.NewLine;
                        }
                    }

                    continue;
                }

                // Shifted and unshifted are different.
                if (apl != null || aplShift != null)
                {
                    label.Text = shifted + ((aplShift != null) ? "   " + aplShift : string.Empty)
                        + Environment.NewLine
                        + unshifted + ((apl != null) ? "   " + apl : string.Empty);
                }
                else
                {
                    if (altGrString != null)
                    {
                        label.Text = shifted + Environment.NewLine + unshifted + "   " + altGrString;
                        continue;
                    }

                    label.Text = shifted + Environment.NewLine + unshifted;
                }
            }

            this.rightAltKey.Text = InputLanguage.CurrentInputLanguage.Handle != EnUsHandle ? "AltGr" : "Alt";
        }

        /// <summary>
        /// Display the key names.
        /// </summary>
        private void DisplayNames()
        {
            this.ClearModifiers();
            this.AdjustKey56();
            this.aplLegendPanel.Visible = false;
            this.aplLegendLabel.Visible = false;
            foreach (var label in this.keysPanel.Controls.OfType<Label>())
            {
                string tag;
                label.Font = new Font(label.Font.Name, SmallFontSize);
                if (label.Tag != null && !string.IsNullOrEmpty(tag = (string)label.Tag))
                {
                    var text = string.Empty;
                    try
                    {
                        if (tag.StartsWith("="))
                        {
                            var scanCode = uint.Parse(tag.Substring(1), NumberStyles.AllowHexSpecifier);
                            var vkey = KeyboardUtil.ScanCodeToVkey(scanCode, InputLanguage.CurrentInputLanguage);
                            if (vkey == 0)
                            {
                                continue;
                            }

                            text = vkey.ToStringExtended();
                        }
                        else
                        {
                            text = tag.Replace(" ", Environment.NewLine + Environment.NewLine);
                        }
                    }
                    finally
                    {
                        label.Text = text;
                        this.toolTip1.SetToolTip(label, text);
                    }
                }
            }
        }

        /// <summary>
        /// Display the key definitions.
        /// </summary>
        private void DisplayDefinitions()
        {
            this.AdjustKey56();
            this.aplLegendPanel.Visible = false;
            this.aplLegendLabel.Visible = false;
            this.modifiersGroupBox.Enabled = true;
            this.modeGroupBox.Enabled = true;
            foreach (var label in this.keysPanel.Controls.OfType<Label>())
            {
                var fontSize = SmallFontSize;
                var fontStyle = FontStyle.Regular;
                string tag;
                if (label.Tag == null || string.IsNullOrEmpty(tag = (string)label.Tag))
                {
                    continue;
                }

                var text = string.Empty;
                var toolTipText = string.Empty;
                try
                {
                    var key = tag;
                    string scanCodeString;
                    Keys vkey = 0;
                    if (tag.StartsWith("="))
                    {
                        scanCodeString = "Scan" + tag.Substring(1);
                        var scanCode = uint.Parse(tag.Substring(1), NumberStyles.AllowHexSpecifier);
                        vkey = KeyboardUtil.ScanCodeToVkey(scanCode, InputLanguage.CurrentInputLanguage);
                        if (vkey == 0)
                        {
                            continue;
                        }

                        key = vkey.ToStringExtended();
                    }
                    else
                    {
                        var parts = tag.Split(' ');
                        if (parts.Length > 1)
                        {
                            if (this.numLockCheckBox.Checked)
                            {
                                key = parts[0];
                            }
                            else
                            {
                                key = parts[1];
                            }
                        }

                        vkey = KeyboardUtil.ParseKeysExtended(tag.Split(' ')[0]);
                        var scanCode = KeyboardUtil.VkeyToScanCode(vkey, InputLanguage.CurrentInputLanguage);
                        scanCodeString = KeyHelper.ScanName(scanCode);
                    }

                    if (this.Map.TryGetClosestMatch(
                        key,
                        scanCodeString,
                        this.KeyboardModifier,
                        KeyMap<KeyboardMap>.ProfileChord(this.ChordName),
                        out KeyboardMap map))
                    {
                        toolTipText = map.Actions;
                        text = this.DecodeKeyAction(map.Actions, out bool single, out bool underline);
                        if (single)
                        {
                            fontSize = LargeFontSize;
                        }

                        if (underline)
                        {
                            fontStyle = FontStyle.Underline;
                        }
                    }
                    else if (vkey != 0 && this.ChordName == null)
                    {
                        var keyChar = KeyboardUtil.FromVkey(vkey, this.KeyboardModifierKeys, out bool isDead);
                        if (keyChar.HasValue)
                        {
                            if ((int)keyChar.Value <= 0x20 || ((int)keyChar.Value >= 0x7f && (int)keyChar.Value < 0xa1))
                            {
                                var friendlyNames = new Dictionary<int, string>
                                {
                                    { 0x1b, "Escape" },
                                    { 0x0a, "Line Feed" },
                                    { 0x0d, "Carriage Return" },
                                    { 0x20, "Space" },
                                };
                                if (friendlyNames.ContainsKey((int)keyChar.Value))
                                {
                                    text = friendlyNames[(int)keyChar.Value];
                                    toolTipText = string.Format("Key(U+{0:X4})", (int)keyChar.Value);
                                }
                                else
                                {
                                    text = string.Format("Key(U+{0:X4})", (int)keyChar.Value);
                                    toolTipText = text;
                                }
                            }
                            else
                            {
                                text = keyChar.Value.ToString();
                                toolTipText = string.Format("Key(U+{0:X4})", (int)keyChar.Value);
                                fontSize = LargeFontSize;
                            }
                        }
                    }
                }
                finally
                {
                    label.Text = text;
                    label.Font = new Font(label.Font.Name, fontSize, fontStyle);
                    this.toolTip1.SetToolTip(label, toolTipText);
                }
            }
        }

        /// <summary>
        /// Decode a 'Key()' action for display.
        /// </summary>
        /// <param name="action">Action to decode.</param>
        /// <param name="single">Returned true if the action is a single key.</param>
        /// <param name="underline">Returned true to display underlined.</param>
        /// <returns>Display form of action.</returns>
        private string DecodeKeyAction(string action, out bool single, out bool underline)
        {
            single = false;
            underline = false;
            if (action == B3270.Action.Comment)
            {
                return string.Empty;
            }

            if (!action.StartsWith("Key("))
            {
                return action;
            }

            // Single character.
            var regex = new Regex(@"^Key\((?<key>.)\)$");
            var matches = regex.Match(action);
            if (matches.Success)
            {
                single = true;
                return matches.Groups["key"].Value;
            }

            // APL symbol.
            regex = new Regex(@"^Key\(apl_(?<apl>[a-zA-Z]+)\)$");
            matches = regex.Match(action);
            if (matches.Success)
            {
                foreach (var entry in DefaultKeypadMap.Map)
                {
                    if (entry.Value.Actions == action)
                    {
                        single = true;
                        return entry.Value.Text;
                    }
                }

                var aplSym = matches.Groups["apl"].Value;
                if (aplSym.EndsWith("underbar"))
                {
                    underline = true;
                    return aplSym.Substring(0, 1);
                }
            }

            return action;
        }

        /// <summary>
        /// Display the scan codes.
        /// </summary>
        private void DisplayScanCodes()
        {
            this.ClearModifiers();
            this.AdjustKey56();
            this.aplLegendPanel.Visible = false;
            this.aplLegendLabel.Visible = false;
            foreach (var label in this.keysPanel.Controls.OfType<Label>())
            {
                label.Font = new Font(label.Font.Name, SmallFontSize);
                string tag;
                if (label.Tag != null && !string.IsNullOrEmpty(tag = (string)label.Tag))
                {
                    var text = string.Empty;
                    try
                    {
                        if (tag.StartsWith("="))
                        {
                            text = tag.Substring(1);
                        }
                        else
                        {
                            var vkey = KeyboardUtil.ParseKeysExtended(tag.Split(' ')[0]);
                            var scanCode = KeyboardUtil.VkeyToScanCode(vkey, InputLanguage.CurrentInputLanguage);
                            text = scanCode.ToString("X2");
                        }
                    }
                    finally
                    {
                        label.Text = text;
                        this.toolTip1.SetToolTip(label, text);
                    }
                }
            }
        }

        /// <summary>
        /// One of the display radio buttons changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DisplayCheckedChanged(object sender, EventArgs e)
        {
            var button = (RadioButton)sender;
            if (!button.Checked)
            {
                return;
            }

            switch ((string)button.Tag)
            {
                case "Labels":
                    this.DisplayLabels();
                    break;
                case "Names":
                    this.DisplayNames();
                    break;
                case "Definitions":
                    this.DisplayDefinitions();
                    break;
                case "ScanCodes":
                    this.DisplayScanCodes();
                    break;
            }
        }

        /// <summary>
        /// One of the modifier checkboxes was checked or unchecked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ModifierCheckedChanged(object sender, EventArgs e)
        {
            if (this.definitionsRadioButton.Checked)
            {
                this.DisplayDefinitions();
            }
            else if (this.labelsRadioButton.Checked)
            {
                this.DisplayLabels();
            }
        }

        /// <summary>
        /// One of the 3270/NVT mode checkboxes was toggled.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ModeModifierCheckedChanged(object sender, EventArgs e)
        {
            // Make sure both are not set. If the other one is currently set, turn it off.
            var checkbox = (CheckBox)sender;
            if (checkbox.Checked)
            {
                foreach (var other in checkbox.Parent.Controls.OfType<CheckBox>().Where(c => c != checkbox && c.Checked))
                {
                    other.Checked = false;
                }
            }

            // Update everything else.
            this.ModifierCheckedChanged(sender, e);
        }

        /// <summary>
        /// The NumLock check box changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NumLockChanged(object sender, EventArgs e)
        {
            // This only matters when we are displaying definitions.
            if (!this.definitionsRadioButton.Checked)
            {
                return;
            }

            this.DisplayDefinitions();
        }

        /// <summary>
        /// The NVT/3270 mode changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ModeChanged(object sender, EventArgs e)
        {
            // This only matters when we are displaying definitions.
            if (!this.definitionsRadioButton.Checked)
            {
                return;
            }

            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
            {
                return;
            }

            this.DisplayDefinitions();
        }

        /// <summary>
        /// The user clicked on a key.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardPicture_Click(object sender, EventArgs e)
        {
            if (this.PictureMode == PictureMode.Display)
            {
                return;
            }

            var label = (Label)sender;
            var tag = (string)label.Tag;
            var splitTag = tag.Split(' ');
            if (splitTag.Length > 1)
            {
                // Keypad; need to pick NumLock or not.
                if (this.numLockCheckBox.Checked)
                {
                    tag = splitTag[0];
                }
                else
                {
                    tag = splitTag[1];
                }
            }

            if (tag.StartsWith("="))
            {
                var scanCode = uint.Parse(tag.Substring(1), NumberStyles.AllowHexSpecifier);
                var vkey = KeyboardUtil.ScanCodeToVkey(scanCode, InputLanguage.CurrentInputLanguage);
                if (vkey == 0)
                {
                    return;
                }

                this.KeyCode = vkey;
                this.ScanCode = scanCode;
            }
            else
            {
                this.KeyCode = KeyboardUtil.ParseKeysExtended(tag);
                this.ScanCode = KeyboardUtil.VkeyToScanCode(this.KeyCode, InputLanguage.CurrentInputLanguage);
            }

            this.SelectedModifiers = Control.ModifierKeys | this.KeyboardModifierKeys;
            this.Apl = this.KeyboardModifier.HasFlag(KeyboardModifier.Apl);
            this.KeyChar = KeyboardUtil.FromVkey(this.KeyCode, this.SelectedModifiers, out bool isDead);
            this.Dead = isDead;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// A key was pressed in the window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyCapture_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            this.keyCapture.KeyDown(sender, e);
        }

        /// <summary>
        /// A key was released in the window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyCapture_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // For Tab, we might get KeyUp without KeyDown, because there are controls on the form
            // which could be tabbed to (even though none of them have the TabStop property set).
            // So Forms grabs the Tab down events, but not the Tab up events. I probably need
            // KeyCapture to allow this, at least for Tab.
            this.keyCapture.KeyUp(sender, e);
        }

        /// <summary>
        /// The input language changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardPicture_InputLanguageChanged(object sender, System.Windows.Forms.InputLanguageChangedEventArgs e)
        {
            if (this.definitionsRadioButton.Checked)
            {
                this.DisplayDefinitions();
            }
            else if (this.labelsRadioButton.Checked)
            {
                this.DisplayLabels();
            }
            else if (this.keyNamesRadioButton.Checked)
            {
                this.DisplayNames();
            }
            else
            {
                this.DisplayScanCodes();
            }
        }

        /// <summary>
        /// Adjust the appearance of key 56 and the left Shift key.
        /// </summary>
        private void AdjustKey56()
        {
            // Set up key 56.
            var nonUs = InputLanguage.CurrentInputLanguage.Handle != EnUsHandle;
            this.key56.Visible = nonUs;
            this.leftShiftKey.Width = nonUs ? 86 : 139;

            // Set up the input language display.
            this.nativeNameLabel.Text = I18n.Get(MessageNames.Input) + ": " +
                InputLanguage.CurrentInputLanguage.Culture.KeyboardLayoutId + " " +
                InputLanguage.CurrentInputLanguage.Culture.NativeName;

            // Set up the layout.
            StringBuilder b = new StringBuilder(256);
            NativeMethods.GetKeyboardLayoutName(b);
            using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts\" + b.ToString());
            var layout = string.Empty;
            if (key != null)
            {
                layout = (string)key.GetValue("Layout Text");
            }

            this.layoutLabel.Text = I18n.Get(MessageNames.Layout) + ": " + layout ?? string.Empty;
        }

        /// <summary>
        /// The window was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardPicture_Activated(object sender, EventArgs e)
        {
            // If no control is active, the arrow keys are not intercepted.
            this.ActiveControl = null;

            if (this.labelsRadioButton.Checked)
            {
                // The input language may have changed while we were invisible.
                this.DisplayLabels();
            }

            // Turn off all of the tab stops. The designer seems to get confused about this.
            foreach (var button in this.displayTable.Controls.OfType<RadioButton>())
            {
                button.TabStop = false;
            }

            foreach (var checkBox in this.modifiersTable.Controls.OfType<CheckBox>())
            {
                checkBox.TabStop = false;
            }

            foreach (var button in this.modeTable.Controls.OfType<CheckBox>())
            {
                button.TabStop = false;
            }
        }

        /// <summary>
        /// A control became active.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardMap_Enter(object sender, EventArgs e)
        {
            // Refuse focus.
            this.ActiveControl = null;
        }

        /// <summary>
        /// The chord index changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ChordIndexChanged(object sender, EventArgs e)
        {
            if (this.definitionsRadioButton.Checked)
            {
                this.DisplayDefinitions();
            }
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardPicture_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.PictureMode == PictureMode.Select)
            {
                return;
            }

            e.Cancel = true;
            this.Hide();
            this.Owner.BringToFront();
        }

        /// <summary>
        /// The Help button was pressed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, EventArgs e)
        {
            Wx3270App.GetHelp("Keymap" + this.PictureMode.ToString());
        }

        /// <summary>
        /// Message types.
        /// </summary>
        private static class MessageNames
        {
            /// <summary>
            /// Input label.
            /// </summary>
            public static readonly string Input = I18n.Combine(MessageName, "label");

            /// <summary>
            /// Layout label.
            /// </summary>
            public static readonly string Layout = I18n.Combine(MessageName, "layout");
        }
    }
}
