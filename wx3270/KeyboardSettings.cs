// <copyright file="KeyboardSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Keyboard tab in the settings dialog.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Format for undefined value string.
        /// </summary>
        private const string UndefinedFormat = "[{0}]";

        /// <summary>
        /// The base name for localized strings in this tab.
        /// </summary>
        private static readonly string KeyboardStringBase = I18n.StringName(I18n.Combine(nameof(Settings), nameof(keyboardTab)));

        /// <summary>
        /// The base name for localized tool tip strings.
        /// </summary>
        private static readonly string KeyboardToolTipBase = I18n.StringName(I18n.Combine(nameof(Settings), nameof(keyboardTab), I18n.ToolTipName));

        /// <summary>
        /// The current keyboard modifier.
        /// </summary>
        private KeyboardModifier currentKeyboardMod = KeyboardModifier.None;

        /// <summary>
        /// The key being edited.
        /// </summary>
        private Keys editedKey = Keys.None;

        /// <summary>
        /// The character produced by default for the key being edited.
        /// </summary>
        private char? editedKeyChar;

        /// <summary>
        /// The scan code of the key being edited.
        /// </summary>
        private uint editedScanCode;

        /// <summary>
        /// True if the key being edited is a dead key.
        /// </summary>
        private bool editedKeyDead;

        /// <summary>
        /// The name of the edited key.
        /// </summary>
        private string editedKeyName;

        /// <summary>
        /// Edited key match type.
        /// </summary>
        private MatchType editedKeyMatchType;

        /// <summary>
        /// Forced match type.
        /// </summary>
        private MatchType? forceMatchType;

        /// <summary>
        /// True if the edited key is a chord.
        /// </summary>
        private bool editedKeyIsChord;

        /// <summary>
        /// True if we are changing a keyboard modified check box programmatically.
        /// </summary>
        private bool changingKeyboardModifier;

        /// <summary>
        /// The keyboard picture.
        /// </summary>
        private KeyboardPicture keyboardPicture;

        /// <summary>
        /// The keyboard map being edited.
        /// </summary>
        private KeyMap<KeyboardMap> editedKeyboardMap = new KeyMap<KeyboardMap>();

        /// <summary>
        /// The base name for strings.
        /// </summary>
        private string stringName;

        /// <summary>
        /// Gets the localized name of 'no chord'.
        /// </summary>
        public static string NoChord => I18n.Get(KeyboardString.NoChord);

        /// <summary>
        /// Gets the current modifier keys as a Keys enumeration.
        /// </summary>
        private Keys ModifierKeysEnum
        {
            get
            {
                var ret = Keys.None;
                if (this.currentKeyboardMod.HasFlag(KeyboardModifier.Shift))
                {
                    ret |= Keys.Shift;
                }

                if (this.currentKeyboardMod.HasFlag(KeyboardModifier.Ctrl))
                {
                    ret |= Keys.Control;
                }

                if (this.currentKeyboardMod.HasFlag(KeyboardModifier.Alt))
                {
                    ret |= Keys.Alt;
                }

                return ret;
            }
        }

        /// <summary>
        /// Gets the current map mode.
        /// </summary>
        private KeyboardModifier CurrentMapModifier =>
            (this.modeNvtCheckBox.Checked ? KeyboardModifier.ModeNvt : KeyboardModifier.None) |
            (this.mode3270checkBox.Checked ? KeyboardModifier.Mode3270 : KeyboardModifier.None);

        /// <summary>
        /// Gets the edited key string.
        /// </summary>
        private string EditedKeyString
        {
            get
            {
                if (this.editedKeyMatchType == MatchType.ScanCode)
                {
                    return KeyHelper.ScanName(this.editedScanCode);
                }
                else
                {
                    return this.editedKey.ToStringExtended();
                }
            }
        }

        // Gets the current chord.
        private string ChordName
        {
            get
            {
                var selected = (string)this.chordComboBox.Items[this.chordComboBox.SelectedIndex];
                return selected == I18n.Get(KeyboardString.NoChord) ? null : selected;
            }
        }

        /// <summary>
        /// Gets the full set of modifiers (real and synthetic).
        /// </summary>
        private KeyboardModifier AllModifiers => this.currentKeyboardMod | this.CurrentMapModifier;

        /// <summary>
        /// Gets the full name of the edited key, including modifiers and chord.
        /// </summary>
        private string FullKeyName => ((this.ChordName != null) ? this.ChordName + " + " : string.Empty) +
            ((this.AllModifiers != KeyboardModifier.None) ? ModifierName(this.AllModifiers) + "-" : string.Empty) +
            this.EditedKeyString;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeKeyboardSettings()
        {
            I18n.LocalizeGlobal(KeyboardString.NoChord, "None");
            I18n.LocalizeGlobal(KeyboardString.KeyCode, "Key code");
            I18n.LocalizeGlobal(KeyboardString.ScanCode, "Scan code");
            I18n.LocalizeGlobal(KeyboardString.DeadKey, "dead");
            I18n.LocalizeGlobal(KeyboardString.DefaultInputChar, "Default input");
            I18n.LocalizeGlobal(KeyboardString.UndoKey, "key change");
            I18n.LocalizeGlobal(KeyboardString.UndoKeypadPosition, "keypad position");
            I18n.LocalizeGlobal(KeyboardString.UpdateKeymapVersion, "Update keyboard map for newer version");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(keyboardTab)), "Tour: Keyboard settings");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(keyboardTab)),
@"Use this tab to change keyboard mappings. This tour walks you through the steps.");

            // Key selection.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(keyboardPictureButton)), "Key selection");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(keyboardPictureButton)),
@"Click to open a window that allows you to select the key to map.");

            // Selected key display.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(selectedKeyGroupBox)), "Selected key");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(selectedKeyGroupBox)),
@"The name of the key you have selected will appear here, along with its scan code, and its default mapping if it is a data-entry key.");

            // Modifiers.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(modifiersTableLayoutPanel)), "Modifier selection");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(modifiersTableLayoutPanel)),
@"Select the modifiers to match.

The Shift, Ctrl and Alt modifiers refer to the corresponding keys on the keyboard. The mapping will only apply if those keys are also pressed.

If 'NVT only', '3270 only' or APL are selected, the mapping will only apply when wx3270 is in those modes.");

            // Actions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(keyboardActionsTextBox)), "Actions");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(keyboardActionsTextBox)),
@"This box will display the current actions performed when the selected key is pressed.

Click to change those actions, using the Macro Editor.");

            // Delete button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(keyboardActionsRemoveButton)), "Delete button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(keyboardActionsRemoveButton)),
@"Click to remove the mapping for this key, restoring its default behavior.");

            // Chord intro.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(keyboardTab), 1), "Two-key mappings (Chords)");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(keyboardTab), 1),
@"Wx3270 can also map 2-key sequences, called chords. This will walk you through defining a chord.

Chords begin with a 'cord-start' key. Once you have defined a cord-start key, you can define other keys that complete the chord (are pressed in sequence after the chord-start key).");

            // Define a chord, first key.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(flowLayoutPanel1)), "Chord: Define a chord-start key");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(flowLayoutPanel1)),
@"Select a key and modifiers above, as usual. Then, select 'Start chord' here instead of 'Perform actions'.

The key will now be defined as a chord-start key, and will not perform any actions until a second key is pressed.");

            // Define a cord, second key.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(chordComboBox)), "Chord: Define the second key in a chord");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(chordComboBox)),
@"Once you have defined one or more cord-start keys, this drop-down will list them.

Select the first key of the chord you want to define here (the chord-start key). Then select the key and modifiers as usual, and enter the actions to perform.

The mapping for key and modifiers will only apply when that key is pressed in sequence after the chord-start key.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Initialize the keyboard tab.
        /// </summary>
        private void KeyboardTabInit()
        {
            // No key specified yet.
            this.selectedKeyLabel.Text = string.Empty;
            this.keymapSelectedLabel.Text = string.Empty;
            this.scanCodeSelectedLabel.Text = string.Empty;

            // Subscribe to profile changes.
            this.ProfileManager.AddChangeTo(this.KeyboardProfileChange);

            // Set up the chord.
            this.chordComboBox.SelectedIndex = 0; // None

            // Play with focus.
            this.keyboardActionsTextBox.GotFocus += (sender, args) => this.keyboardActionsEditButton.Focus();

            // Subscribe to old version events.
            this.ProfileManager.OldVersion += (Profile.VersionClass oldVersion, ref bool saved) =>
            {
                var addedMappings = Profile.PerVersionAddedKeyboardMaps.Where(kv => kv.Key > oldVersion).Select(kv => kv.Value).ToList();
                if (addedMappings.Any())
                {
                    var newKeys = new List<string>();
                    foreach (var mapping in addedMappings)
                    {
                        foreach (var entry in mapping)
                        {
                            newKeys.Add(KeyMap<KeyboardMap>.DecodeKeyName(entry.Key));
                        }
                    }

                    var joinedStrings = string.Join(", ", newKeys.ToArray(), 0, newKeys.Count - 1);
                    if (newKeys.Count > 1)
                    {
                        joinedStrings += " " + I18n.Get(KeyboardString.And) + " " + newKeys.Last();
                    }

                    var yesNo = MessageBox.Show(
                        string.Format(I18n.Get(Message.OldProfile), oldVersion, joinedStrings),
                        I18n.Get(Title.OldProfile),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                    if (yesNo == DialogResult.Yes)
                    {
                        saved |= this.ProfileManager.PushAndSave(
                            (current) =>
                            {
                                foreach (var mapping in addedMappings)
                                {
                                    foreach (var entry in mapping)
                                    {
                                        current.KeyboardMap[entry.Key] = entry.Value;
                                    }
                                }
                            },
                            I18n.Get(KeyboardString.UpdateKeymapVersion));
                    }
                }
            };

            // Set up message box titles.
            I18n.LocalizeGlobal(Title.OldProfile, "Old Profile Version Detected");

            // Set up message box messages.
            I18n.LocalizeGlobal(Message.OldProfile, "This profile is from wx3270 version {0}. New default keyboard mappings have been added since then." + Environment.NewLine + Environment.NewLine + "Update the keyboard mappings for {1}?");

            // Set up the localized strings.
            this.stringName = I18n.StringName(I18n.Combine(nameof(Settings), this.keyboardTab.Name));
            I18n.LocalizeGlobal(KeyboardString.Undefined, "not defined");
            I18n.LocalizeGlobal(KeyboardString.Actions, "actions");
            I18n.LocalizeGlobal(KeyboardString.In3270Mode, "in 3270 mode");
            I18n.LocalizeGlobal(KeyboardString.InNvtMode, "in NVT mode");
            I18n.LocalizeGlobal(KeyboardString.InheritedFrom, "inherited from {0}");
            I18n.LocalizeGlobal(KeyboardString.DefaultInput, "uses default input rule");
            I18n.LocalizeGlobal(KeyboardString.StartChord, "Wait for second key");
            I18n.LocalizeGlobal(KeyboardString.And, "and");

            // Localize the mouse-over text for the actions text box.
            I18n.LocalizeGlobal(KeyboardToolTip.ClickToEdit, "Click to edit definition");
            I18n.LocalizeGlobal(
                KeyboardToolTip.ClickToOverride,
                "Actions inherited from less-specific definition" + Environment.NewLine + "Click to create specific definition");
            I18n.LocalizeGlobal(KeyboardToolTip.ClickToAdd, "Click to add definition");

            // Register our tour.
            var nodes = new[]
            {
                ((Control)this.keyboardTab, (int?)null, Orientation.Centered),
                (this.keyboardPictureButton, null, Orientation.UpperLeft),
                (this.selectedKeyGroupBox, null, Orientation.UpperRight),
                (this.modifiersTableLayoutPanel, null, Orientation.UpperLeft),
                (this.keyboardActionsTextBox, null, Orientation.LowerLeftTight),
                (this.keyboardActionsRemoveButton, null, Orientation.LowerLeft),
                (this.keyboardTab, 1, Orientation.Centered),
                (this.flowLayoutPanel1, null, Orientation.LowerLeft),
                (this.chordComboBox, null, Orientation.UpperLeft),
            };
            this.RegisterTour(this.keyboardTab, nodes);
        }

        /// <summary>
        /// Profile change event handler.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void KeyboardProfileChange(Profile oldProfile, Profile newProfile)
        {
            this.keyboardTab.Enabled = newProfile.ProfileType == ProfileType.Full || newProfile.ProfileType == ProfileType.KeyboardMapTemplate;

            if (oldProfile == null || !oldProfile.KeyboardMap.Equals(newProfile.KeyboardMap))
            {
                // Copy in the new keypad maps.
                this.editedKeyboardMap = new KeyMap<KeyboardMap>(newProfile.KeyboardMap);

                // Re-do the chords.
                this.chordComboBox.Items.Clear();
                this.chordComboBox.Items.AddRange(newProfile.KeyboardMap.Chords().ToArray());
                this.chordComboBox.SelectedIndex = 0;

                // Repaint.
                this.SelectKeyboard();
            }
        }

        /// <summary>
        /// A new key or modifiers has been selected. Set labels and buttons to match.
        /// </summary>
        private void SelectKeyboard()
        {
            if (this.editedKey == Keys.None)
            {
                this.keyboardActionsTextBox.Text = string.Empty;
                this.keyboardActionsTextBox.Enabled = false;
                this.keyboardActionsTextBox.BackColor = System.Drawing.SystemColors.Control;
                this.keyboardActionsInheritedLabel.Visible = false;
                return;
            }

            // Re-evaluate the chord list.
            var newChords = this.editedKeyboardMap.Chords();
            var chordsNow = new List<string>();
            chordsNow.AddRange(this.chordComboBox.Items.Cast<string>());
            if (!chordsNow.SequenceEqual(newChords))
            {
                this.chordComboBox.Items.Clear();
                this.chordComboBox.Items.AddRange(this.editedKeyboardMap.Chords().ToArray());
                this.chordComboBox.SelectedIndex = 0;
            }

            var toolTip = string.Empty;
            this.keyboardActionsTextBox.Enabled = true;
            this.keyboardActionsTextBox.BackColor = System.Drawing.Color.White;
            this.editedKeyIsChord = false;
            if (this.editedKeyboardMap.TryGetClosestMatch(
                this.editedKey.ToStringExtended(),
                KeyHelper.ScanName(this.editedScanCode),
                this.currentKeyboardMod | this.CurrentMapModifier,
                KeyMap<KeyboardMap>.ProfileChord(this.ChordName),
                out KeyboardMap map,
                out bool exact,
                out KeyboardModifier matchedModifier,
                out MatchType matchType)
                && !(this.forceMatchType.HasValue && matchType != this.forceMatchType.Value))
            {
                this.editedKeyMatchType = matchType;
                if (exact)
                {
                    // Exact match.
                    this.keyboardActionsTextBox.Text = map.Actions;
                    this.keyboardActionsTextBox.Enabled = true;
                    this.keyboardActionsTextBox.BackColor = System.Drawing.Color.White;
                    this.keyboardActionsInheritedLabel.Visible = false;
                    toolTip = I18n.Get(KeyboardToolTip.ClickToEdit);
                    this.keyboardActionsAddKeyButton.Enabled = false;
                    this.keyboardActionsEditButton.Enabled = true;
                    this.keyboardActionsRemoveButton.Enabled = true;

                    // For an exact match of a scan code, you can't define a key code match to replace it.
                    this.matchKeyRadioButton.Enabled = this.editedKeyMatchType == MatchType.KeyCode;

                    this.exactMatchCheckBox.Enabled = true;
                }
                else
                {
                    // Inexact match.
                    this.keyboardActionsTextBox.Text = map.Actions;
                    this.keyboardActionsTextBox.Enabled = false;
                    this.keyboardActionsTextBox.BackColor = System.Drawing.SystemColors.Control;
                    var inheritedName = new List<string>();
                    var modifier = matchedModifier == KeyboardModifier.None ? string.Empty : ModifierName(matchedModifier);
                    if (!string.IsNullOrEmpty(modifier))
                    {
                        inheritedName.Add(modifier);
                    }

                    inheritedName.Add(this.EditedKeyString);
                    this.keyboardActionsInheritedLabel.Text = string.Format(
                        I18n.Get(KeyboardString.InheritedFrom),
                        string.Join("-", inheritedName));
                    this.keyboardActionsInheritedLabel.Visible = true;
                    toolTip = I18n.Get(KeyboardToolTip.ClickToOverride);
                    this.keyboardActionsAddKeyButton.Enabled = true;
                    this.keyboardActionsEditButton.Enabled = false;
                    this.keyboardActionsRemoveButton.Enabled = false;

                    // For an inexact scan code match, you can define a more-specific key code match to override it.
                    this.matchKeyRadioButton.Enabled = true;

                    this.exactMatchCheckBox.Enabled = false;
                }

                this.matchKeyRadioButton.Checked = this.editedKeyMatchType == MatchType.KeyCode;
                this.matchScanCodeRadioButton.Checked = this.editedKeyMatchType == MatchType.ScanCode;

                this.exactMatchCheckBox.Checked = map.Exact;
            }
            else
            {
                // Not defined.
                if (this.editedKeyChar.HasValue && this.ChordName == null)
                {
                    this.keyboardActionsTextBox.Text = string.Format("Key(U+{0:X4})", (int)this.editedKeyChar.Value);
                    this.keyboardActionsInheritedLabel.Text = I18n.Get(KeyboardString.DefaultInput);
                    this.keyboardActionsInheritedLabel.Visible = true;
                }
                else
                {
                    this.keyboardActionsTextBox.Text = I18n.Get(KeyboardString.Undefined);
                    this.keyboardActionsInheritedLabel.Visible = false;
                }

                toolTip = I18n.Get(KeyboardToolTip.ClickToAdd);
                this.keyboardActionsAddKeyButton.Enabled = true;
                this.keyboardActionsEditButton.Enabled = true;
                this.keyboardActionsTextBox.Enabled = true;
                this.keyboardActionsTextBox.BackColor = System.Drawing.Color.White;
                this.keyboardActionsRemoveButton.Enabled = false;
                this.matchKeyRadioButton.Enabled = true;
                if (!this.forceMatchType.HasValue)
                {
                    // By default, an undefined match is a key code match.
                    this.editedKeyMatchType = MatchType.KeyCode;
                    this.matchKeyRadioButton.Checked = true;
                    this.matchScanCodeRadioButton.Checked = false;
                }

                this.exactMatchCheckBox.Checked = false;
                this.exactMatchCheckBox.Enabled = false;
            }

            // Set up the actions/chord radio buttons.
            if (this.chordComboBox.SelectedIndex == 0)
            {
                // No chord selected.
                this.editedKeyIsChord = KeyMap<KeyboardMap>.IsChord(this.keyboardActionsTextBox.Text);
                this.actionsRadioButton.Enabled = true;
                this.actionsRadioButton.Checked = !this.editedKeyIsChord;
                this.chordRadioButton.Enabled = true;
                this.chordRadioButton.Checked = this.editedKeyIsChord;
            }
            else
            {
                // Chord selected.
                this.actionsRadioButton.Enabled = false;
                this.actionsRadioButton.Checked = true;
                this.chordRadioButton.Enabled = false;
                this.chordRadioButton.Checked = false;
            }

            if (this.editedKeyIsChord)
            {
                this.keyboardActionsTextBox.Enabled = false;
                this.keyboardActionsTextBox.BackColor = System.Drawing.SystemColors.Control;
                this.keyboardActionsTextBox.Text = I18n.Get(KeyboardString.StartChord);
                this.keyboardActionsAddKeyButton.Enabled = false;
                this.keyboardActionsEditButton.Enabled = false;
            }

            // You only get one try to force the other match.
            this.forceMatchType = null;

            this.ComputeKeyName();

            this.toolTip1.SetToolTip(this.keyboardActionsTextBox, toolTip);

            // Push to the profile.
            this.ProfileManager.PushAndSave((current) => current.KeyboardMap = new KeyMap<KeyboardMap>(this.editedKeyboardMap), I18n.Get(KeyboardString.UndoKey));
        }

        /// <summary>
        /// Re-compute the edited key name.
        /// </summary>
        private void ComputeKeyName()
        {
            if (this.editedKey == Keys.None)
            {
                this.editedKeyName = string.Empty;
                this.selectedKeyLabel.Text = string.Empty;
                this.keymapSelectedLabel.Text = string.Empty;
                this.scanCodeSelectedLabel.Text = string.Empty;
                this.editedKeyCharLabel.Text = string.Empty;
                this.keyCharValuePanel.Visible = false;
                return;
            }

            this.selectedKeyLabel.Text = this.FullKeyName;
            this.editedKeyName = this.editedKey.ToStringExtended();
            this.keymapSelectedLabel.Text = I18n.Get(KeyboardString.KeyCode) + string.Format(": {0}", this.editedKeyName);
            this.scanCodeSelectedLabel.Text = I18n.Get(KeyboardString.ScanCode) + string.Format(": {0:X2}", this.editedScanCode);
            this.editedKeyChar = KeyboardUtil.FromVkey(this.editedKey, this.ModifierKeysEnum, out _);

            if (this.ChordName == null && this.editedKeyChar != null)
            {
                this.editedKeyCharLabel.Text = I18n.Get(KeyboardString.DefaultInputChar) +
                    string.Format(
                        ": U+{0:X4}{1}",
                        (int)this.editedKeyChar.Value,
                        this.editedKeyDead ? string.Format(" ({0})", I18n.Get(KeyboardString.DeadKey)) : string.Empty);

                if (!char.IsControl(this.editedKeyChar.Value))
                {
                    this.editedKeyCharValueLabel.Text = this.editedKeyChar.ToString();
                    this.editedKeyCharValueLabel.Visible = true;
                }
                else
                {
                    this.editedKeyCharValueLabel.Visible = false;
                }

                this.keyCharValuePanel.Visible = true;
            }
            else
            {
                this.keyCharValuePanel.Visible = false;
            }
        }

        /// <summary>
        /// A keyboard action button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardActionClick(object sender, EventArgs e)
        {
            var button = sender as Button;
            var tag = button.Tag as string;
            var text = string.Empty;
            var matchType = this.editedKeyMatchType;
            switch (tag)
            {
                case "Add":
                    // Ignore the current text.
                    break;
                case "Edit":
                    text = this.keyboardActionsTextBox.Text;
                    if (text.Equals(I18n.Get(KeyboardString.Undefined)))
                    {
                        text = string.Empty;
                    }

                    break;
                case "Delete":
                    this.editedKeyboardMap.Remove(KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers, KeyMap<KeyboardMap>.ProfileChord(this.ChordName)));
                    this.SelectKeyboard();
                    return;
                default:
                    return;
            }

            var name = this.FullKeyName + " " + I18n.Get(KeyboardString.Actions);
            using var editor = new MacroEditor(text, name, false, this.app);
            var result = editor.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.keyboardActionsTextBox.Text = editor.MacroText;
                this.editedKeyboardMap[KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers, KeyMap<KeyboardMap>.ProfileChord(this.ChordName))] =
                    new KeyboardMap { Actions = editor.MacroText, Exact = this.exactMatchCheckBox.Checked };
                this.SelectKeyboard();
            }
            else if (result == DialogResult.Retry)
            {
                // Record it.
                this.StartRecordingKeymap(name, editor.State);
            }
            else
            {
                // Kludge.
                this.editedKeyMatchType = matchType;
            }
        }

        /// <summary>
        /// Start recording a keymap entry.
        /// </summary>
        /// <param name="macroName">Macro name.</param>
        /// <param name="state">Macro editor state.</param>
        private void StartRecordingKeymap(string macroName, MacroEditor.EditorState state)
        {
            this.app.MacroRecorder.Start(this.KeymapRecordingComplete, (macroName, state));
            this.Hide();
            this.mainScreen.Focus();
        }

        /// <summary>
        /// Macro recording is complete.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="context">Completion context.</param>
        private void KeymapRecordingComplete(string text, object context)
        {
            this.Show();
            if (string.IsNullOrEmpty(text) || context == null)
            {
                return;
            }

            (string macroName, MacroEditor.EditorState state) = (((string, MacroEditor.EditorState)?)context).Value;
            using var editor = new MacroEditor(text, macroName, false, this.app, state);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.keyboardActionsTextBox.Text = editor.MacroText;
                this.editedKeyboardMap[KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers, KeyMap<KeyboardMap>.ProfileChord(this.ChordName))] =
                    new KeyboardMap { Actions = editor.MacroText, Exact = this.exactMatchCheckBox.Checked };
                this.SelectKeyboard();
            }
            else if (result == DialogResult.Retry)
            {
                // Restart macro recorder.
                this.StartRecordingKeymap(editor.MacroName, editor.State);
            }
        }

        /// <summary>
        /// The keyboard actions text box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardActionsClick(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;
            if (text.Equals(I18n.Get(KeyboardString.Undefined)))
            {
                text = string.Empty;
            }

            var name = this.FullKeyName + " " + I18n.Get(KeyboardString.Actions);
            using var editor = new MacroEditor(text, name, false, this.app);
            var result = editor.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox.Text = editor.MacroText;
                this.editedKeyboardMap[KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers, KeyMap<KeyboardMap>.ProfileChord(this.ChordName))] =
                    new KeyboardMap { Actions = editor.MacroText, Exact = this.exactMatchCheckBox.Checked };
                this.SelectKeyboard();
            }
            else if (result == DialogResult.Retry)
            {
                this.StartRecordingKeymap(name, editor.State);
            }
        }

        /// <summary>
        /// One of the keyboard modifier check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardModifierClick(object sender, EventArgs e)
        {
            if (this.changingKeyboardModifier)
            {
                return;
            }

            var checkBox = (CheckBox)sender;
            var flag = (KeyboardModifier)Enum.Parse(typeof(KeyboardModifier), (string)checkBox.Tag);
            if (checkBox.Checked)
            {
                this.currentKeyboardMod |= flag;
            }
            else
            {
                this.currentKeyboardMod &= ~flag;
            }

            this.editedKeyChar = null;
            this.editedKeyDead = false;

            this.ComputeKeyName();
            this.SelectKeyboard();
        }

        /// <summary>
        /// One of the emulator mode check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EmulatorModeClick(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            switch ((string)checkBox.Tag)
            {
                case "3270":
                    if (checkBox.Checked)
                    {
                        this.modeNvtCheckBox.Checked = false;
                    }

                    break;
                case "NVT":
                    if (checkBox.Checked)
                    {
                        this.mode3270checkBox.Checked = false;
                    }

                    break;
                default:
                    break;
            }

            if (this.editedKey == Keys.None)
            {
                return;
            }

            this.SelectKeyboard();
        }

        /// <summary>
        /// The exact match check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ExactMatchCheckBoxClick(object sender, EventArgs e)
        {
            if (this.editedKey != Keys.None)
            {
                // Change the edited keymap entry to be exclusive.
                var key = KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers, KeyMap<KeyboardMap>.ProfileChord(this.ChordName));
                if (this.editedKeyboardMap.TryGetValue(key, out KeyboardMap map))
                {
                    this.editedKeyboardMap[key] = new KeyboardMap { Actions = map.Actions, Exact = this.exactMatchCheckBox.Checked };
                    this.SelectKeyboard();
                }
            }
        }

        /// <summary>
        /// Tests whether the matching keymap entry inherits from a 3270-mode-specific entry.
        /// </summary>
        /// <returns>True if it does.</returns>
        private bool Inherits3270()
        {
            return this.editedKeyboardMap.TryGetClosestMatch(
                this.editedKey.ToStringExtended(),
                KeyHelper.ScanName(this.editedScanCode),
                this.currentKeyboardMod | this.CurrentMapModifier | KeyboardModifier.Mode3270,
                KeyMap<KeyboardMap>.ProfileChord(this.ChordName),
                out _,
                out _,
                out KeyboardModifier matchedModifier,
                out MatchType matchType)
                && !(this.forceMatchType.HasValue && matchType != this.forceMatchType.Value)
                && matchedModifier.HasFlag(KeyboardModifier.Mode3270);
        }

        /// <summary>
        /// The display keyboard map layout button was checked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardPictureButtonClick(object sender, EventArgs e)
        {
            if (this.keyboardPicture == null)
            {
                this.keyboardPicture = new KeyboardPicture(this.editedKeyboardMap);
            }
            else
            {
                this.keyboardPicture.Map = this.editedKeyboardMap;
                this.keyboardPicture.Redisplay();
            }

            this.keyboardPicture.ChordIndex = this.chordComboBox.SelectedIndex;

            // Deliberately exlude Ctrl, Shift, Alt, but preserve the other modifiers.
            this.keyboardPicture.KeyboardModifier = (this.currentKeyboardMod & KeyboardModifier.Apl) | this.CurrentMapModifier;

            if (this.keyboardPicture.ShowDialog(this) == DialogResult.OK)
            {
                this.editedKey = this.keyboardPicture.KeyCode;
                this.editedKeyChar = this.keyboardPicture.KeyChar;
                this.editedKeyDead = this.keyboardPicture.Dead;
                this.editedScanCode = this.keyboardPicture.ScanCode;
                this.chordComboBox.SelectedIndex = this.keyboardPicture.ChordIndex;
                this.currentKeyboardMod =
                        ((this.keyboardPicture.SelectedModifiers.HasFlag(Keys.Shift) || this.keyboardPicture.KeyboardModifier.HasFlag(KeyboardModifier.Shift))
                            ? KeyboardModifier.Shift : KeyboardModifier.None)
                        | ((this.keyboardPicture.SelectedModifiers.HasFlag(Keys.Control) || this.keyboardPicture.KeyboardModifier.HasFlag(KeyboardModifier.Ctrl))
                            ? KeyboardModifier.Ctrl : KeyboardModifier.None)
                        | ((this.keyboardPicture.SelectedModifiers.HasFlag(Keys.Alt) || this.keyboardPicture.KeyboardModifier.HasFlag(KeyboardModifier.Alt))
                            ? KeyboardModifier.Alt : KeyboardModifier.None)
                        | (this.keyboardPicture.Apl ? KeyboardModifier.Apl : KeyboardModifier.None);
                try
                {
                    this.changingKeyboardModifier = true;
                    this.keyboardShiftCheckBox.Checked = this.currentKeyboardMod.HasFlag(KeyboardModifier.Shift);
                    this.keyboardCtrlCheckBox.Checked = this.currentKeyboardMod.HasFlag(KeyboardModifier.Ctrl);
                    this.keyboardAltCheckBox.Checked = this.currentKeyboardMod.HasFlag(KeyboardModifier.Alt);
                    this.keyboardAplModeCheckBox.Checked = this.currentKeyboardMod.HasFlag(KeyboardModifier.Apl);

                    // If there is a match for this key and modifiers in 3270 mode, turn on 3270 mode.
                    // This prevents people from thinking they have created a mapping that does not apply in 3270 mode, when
                    // they intended it to. E.g., Alt-e.
                    this.mode3270checkBox.Checked = this.Inherits3270();
                }
                finally
                {
                    this.changingKeyboardModifier = false;
                }

                this.ComputeKeyName();
                this.SelectKeyboard();
            }
        }

        /// <summary>
        /// One of the match type buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MatchTypeClick(object sender, EventArgs e)
        {
            if (this.editedKey == Keys.None)
            {
                // Nothing going on yet.
                return;
            }

            var button = (RadioButton)sender;
            var tag = (string)button.Tag;
            switch (tag)
            {
                case "KeyCode":
                    if (this.editedKeyMatchType == MatchType.KeyCode)
                    {
                        // No change.
                        return;
                    }

                    this.editedKeyMatchType = MatchType.KeyCode;
                    this.forceMatchType = MatchType.KeyCode;
                    break;
                case "ScanCode":
                    if (this.editedKeyMatchType == MatchType.ScanCode)
                    {
                        // No change.
                        return;
                    }

                    this.editedKeyMatchType = MatchType.ScanCode;
                    this.forceMatchType = MatchType.ScanCode;
                    break;
            }

            this.SelectKeyboard();
        }

        /// <summary>
        /// The input language changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardInputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            // Reset the current key, because the translation between scan code and key may have changed.
            this.editedKey = Keys.None;
            this.ComputeKeyName();

            this.keyboardShiftCheckBox.Checked = false;
            this.keyboardCtrlCheckBox.Checked = false;
            this.keyboardAltCheckBox.Checked = false;
            this.keyboardAplModeCheckBox.Checked = false;

            this.mode3270checkBox.Checked = false;
            this.modeNvtCheckBox.Checked = false;
        }

        /// <summary>
        /// The selection index on the chord combo box changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ChordComboBoxIndexChanged(object sender, EventArgs e)
        {
            var box = (ComboBox)sender;
            if (box.SelectedIndex == 0)
            {
                this.actionsRadioButton.Enabled = true;
                this.chordRadioButton.Enabled = true;
            }
            else
            {
                this.actionsRadioButton.Enabled = false;
                this.chordRadioButton.Enabled = false;
            }

            // Reselect.
            this.SelectKeyboard();
        }

        /// <summary>
        /// One of the chord-mode radio buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ChordClick(object sender, EventArgs e)
        {
            if (this.editedKey == Keys.None)
            {
                return;
            }

            var button = (RadioButton)sender;
            if (!button.Checked)
            {
                // Only respond to the button that gets turned on.
                return;
            }

            switch ((string)button.Tag)
            {
                case "Actions":
                    // Transform chord to actions.
                    if (!this.editedKeyIsChord)
                    {
                        return;
                    }

                    this.editedKeyboardMap.Remove(KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers));
                    break;

                case "Chord":
                    // Transform actions to chord.
                    if (this.editedKeyIsChord)
                    {
                        return;
                    }

                    var key = KeyMap<KeyboardMap>.Key(this.EditedKeyString, this.AllModifiers);
                    this.editedKeyboardMap[key] = new KeyboardMap { Actions = Constants.Action.Chord + "(\"" + key + "\")" };
                    break;
            }

            // Reselect.
            this.SelectKeyboard();
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        public static partial class Title
        {
            /// <summary>
            /// Old profile version detected.
            /// </summary>
            public static readonly string OldProfile = I18n.Combine(TitleName, "oldProfile");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        public static partial class Message
        {
            /// <summary>
            /// Old profile version detected.
            /// </summary>
            public static readonly string OldProfile = I18n.Combine(MessageName, "oldProfile");
        }

        /// <summary>
        /// Localized strings.
        /// </summary>
        private static class KeyboardString
        {
            /// <summary>
            /// An undefined action.
            /// </summary>
            public static readonly string Undefined = I18n.Combine(KeyboardStringBase, "Undefined");

            /// <summary>
            /// The word "actions" for the macro editor title.
            /// </summary>
            public static readonly string Actions = I18n.Combine(KeyboardStringBase, "Actions");

            /// <summary>
            /// 3270 mode for the macro editor title.
            /// </summary>
            public static readonly string In3270Mode = I18n.Combine(KeyboardStringBase, "In3270Mode");

            /// <summary>
            /// NVT mode for the macro editor title.
            /// </summary>
            public static readonly string InNvtMode = I18n.Combine(KeyboardStringBase, "InNvtMode");

            /// <summary>
            /// Inherited from.
            /// </summary>
            public static readonly string InheritedFrom = I18n.Combine(KeyboardStringBase, "InheritedFrom");

            /// <summary>
            /// Default input.
            /// </summary>
            public static readonly string DefaultInput = I18n.Combine(KeyboardStringBase, "DefaultInput");

            /// <summary>
            /// Start chord.
            /// </summary>
            public static readonly string StartChord = I18n.Combine(KeyboardStringBase, "StartChord");

            /// <summary>
            /// No chord.
            /// </summary>
            public static readonly string NoChord = I18n.Combine(KeyboardStringBase, "NoChord");

            /// <summary>
            /// Selected key code.
            /// </summary>
            public static readonly string KeyCode = I18n.Combine(KeyboardStringBase, "KeyCode");

            /// <summary>
            /// Selected scan code.
            /// </summary>
            public static readonly string ScanCode = I18n.Combine(KeyboardStringBase, "ScanCode");

            /// <summary>
            /// Default input character.
            /// </summary>
            public static readonly string DefaultInputChar = I18n.Combine(KeyboardStringBase, "DefaultInputChar");

            /// <summary>
            /// Dead key.
            /// </summary>
            public static readonly string DeadKey = I18n.Combine(KeyboardStringBase, "DeadKey");

            /// <summary>
            /// Undo key change.
            /// </summary>
            public static readonly string UndoKey = I18n.Combine(KeyboardStringBase, "UndoKey");

            /// <summary>
            /// Undo key change.
            /// </summary>
            public static readonly string UndoKeypadPosition = I18n.Combine(KeyboardStringBase, "UndoKeypad");

            /// <summary>
            /// The word 'and'.
            /// </summary>
            public static readonly string And = I18n.Combine(KeyboardStringBase, "and");

            /// <summary>
            /// Change name for updating the keyboard map.
            /// </summary>
            public static readonly string UpdateKeymapVersion = I18n.Combine(KeyboardStringBase, "updateKeymapVersion");
        }

        /// <summary>
        /// Names for localized tool tips for the keyboard actions text box.
        /// </summary>
        private static class KeyboardToolTip
        {
            /// <summary>
            /// Click to edit.
            /// </summary>
            public static readonly string ClickToEdit = I18n.Combine(KeyboardToolTipBase, "ClickToEdit");

            /// <summary>
            /// Click to create.
            /// </summary>
            public static readonly string ClickToOverride = I18n.Combine(KeyboardToolTipBase, "ClickToOverride");

            /// <summary>
            /// Click to add.
            /// </summary>
            public static readonly string ClickToAdd = I18n.Combine(KeyboardToolTipBase, "ClickToAdd");
        }
    }
}
