// <copyright file="KeypadSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Keypad tab in the settings dialog.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Button border width.
        /// </summary>
        private const int KeyBorderWidth = 2;

        /// <summary>
        /// Margin around each key pane.
        /// </summary>
        private const int KeyPaneMargin = 3;

        /// <summary>
        /// Default text size.
        /// </summary>
        private const float DefaultTextSize = 6.75F;

        /// <summary>
        /// The base name for localized strings in this tab.
        /// </summary>
        private static readonly string KeypadStringBase = I18n.StringName(I18n.Combine(nameof(Settings), nameof(keypadTab)));

        /// <summary>
        /// The collection of keypad buttons.
        /// </summary>
        private readonly Dictionary<string, NoSelectButton> keypadButtons = new Dictionary<string, NoSelectButton>();

        /// <summary>
        /// The current keypad keyboard modifier.
        /// </summary>
        private KeyboardModifier currentKeypadMod = KeyboardModifier.None;

        /// <summary>
        /// The current keypad key.
        /// </summary>
        private Wx3270.NoSelectButton currentKeypadButton;

        /// <summary>
        /// The keypad maps being edited.
        /// </summary>
        private KeyMap<KeypadMap> editedKeypadMaps = new KeyMap<KeypadMap>();

        /// <summary>
        /// The keypad map being edited.
        /// </summary>
        private KeypadMap editedKeymap;

        /// <summary>
        /// The keypad map this entry is inherited from.
        /// </summary>
        private KeypadMap inheritedKeymap;

        /// <summary>
        /// True if keypad map selection is in progress.
        /// </summary>
        private bool keymapSelectionInProgress;

        /// <summary>
        /// The keypad position radio buttons.
        /// </summary>
        private RadioEnum<KeypadPosition> keypadPosition;

        /// <summary>
        /// The keypad type radio buttons.
        /// </summary>
        private RadioEnum<KeypadType> keypadType;

        /// <summary>
        /// Keypad type enumeration.
        /// </summary>
        private enum KeypadType
        {
            /// <summary>
            /// 3270 keypad.
            /// </summary>
            Type3270,

            /// <summary>
            /// APL keypad.
            /// </summary>
            TypeApl,
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeKeypadSettings()
        {
            I18n.LocalizeGlobal(KeypadString.ClickToEdit, "Click to edit actions");
            I18n.LocalizeGlobal(KeypadString.ActionsInherited, "Actions inherited from less-specific definition");
            I18n.LocalizeGlobal(KeypadString.ClickToAddMoreSpecific, "Click to add a more-specific definition");
            I18n.LocalizeGlobal(KeypadString.ClickToAdd, "Click to add actions");
            I18n.LocalizeGlobal(KeypadString.KeypadActions, "Keypad Actions");
            I18n.LocalizeGlobal(KeypadString.ChangeKeypadActions, "change keypad actions");
            I18n.LocalizeGlobal(KeypadString.ChangeKeyTextSize, "change key text size");
            I18n.LocalizeGlobal(KeypadString.RemoveKeyDefinition, "remove key definition");
            I18n.LocalizeGlobal(KeypadString.ChangeKeyBackground, "change key background");
            I18n.LocalizeGlobal(KeypadString.ChangeKeyText, "change key text");
    }

        /// <summary>
        /// Return the base name for a button name.
        /// </summary>
        /// <param name="name">Button name.</param>
        /// <returns>Base name.</returns>
        public static string ButtonBaseName(string name)
        {
            if (name.EndsWith("Button", StringComparison.InvariantCultureIgnoreCase))
            {
                return name.Substring(0, name.Length - "Button".Length);
            }

            return name;
        }

        /// <summary>
        /// Compute the background image for a button.
        /// </summary>
        /// <param name="button">Button to consider.</param>
        /// <param name="backgroundImage">Desired image type.</param>
        /// <param name="borderWidth">Border width.</param>
        /// <returns>Usable image.</returns>
        public static Image ImageForButton(Button button, KeypadBackgroundImage backgroundImage, int borderWidth = 0)
        {
            if (button.Height != button.Width)
            {
                if (button.Width == 72 + (2 * borderWidth) && button.Height == 48 + (2 * borderWidth))
                {
                    return Properties.Resources.BlankWide48;
                }

                if (button.Width == 72 + (2 * borderWidth) && button.Height == 96 + (2 * borderWidth))
                {
                    return Properties.Resources.BlankVert;
                }

                // A reasonable default.
                return Properties.Resources.Blank48;
            }

            switch (backgroundImage)
            {
                default:
                case KeypadBackgroundImage.Blank:
                    return Properties.Resources.Blank48;
                case KeypadBackgroundImage.Insert:
                    return Properties.Resources.Insert48_2;
                case KeypadBackgroundImage.Delete:
                    return Properties.Resources.Delete48;
            }
        }

        /// <summary>
        /// Friendly name for a set of modifiers.
        /// </summary>
        /// <param name="mod">Set of modifiers.</param>
        /// <returns>Formatted string.</returns>
        public static string ModifierName(KeyboardModifier mod)
        {
            if (mod == KeyboardModifier.None)
            {
                return "No Modifiers";
            }

            List<string> names = new List<string>();
            if (mod.HasFlag(KeyboardModifier.Shift))
            {
                names.Add(KeyboardModifier.Shift.ToString());
            }

            if (mod.HasFlag(KeyboardModifier.Ctrl))
            {
                names.Add(KeyboardModifier.Ctrl.ToString());
            }

            if (mod.HasFlag(KeyboardModifier.Alt))
            {
                names.Add(KeyboardModifier.Alt.ToString());
            }

            if (mod.HasFlag(KeyboardModifier.Apl))
            {
                names.Add("APL");
            }

            if (mod.HasFlag(KeyboardModifier.Mode3270))
            {
                names.Add("3270");
            }

            if (mod.HasFlag(KeyboardModifier.ModeNvt))
            {
                names.Add("NVT");
            }

            return string.Join("-", names);
        }

        /// <summary>
        /// Quoted (and spaced) name for a set of modifiers.
        /// </summary>
        /// <param name="mod">Set of modifiers.</param>
        /// <returns>Formatted string.</returns>
        private static string QuotedModifierName(KeyboardModifier mod)
        {
            if (mod == KeyboardModifier.None)
            {
                return string.Empty;
            }
            else
            {
                return $"'{ModifierName(mod)}' ";
            }
        }

        /// <summary>
        /// Initialize the keypad tab.
        /// </summary>
        /// <param name="keypad">Keypad dialog, for copying.</param>
        /// <param name="aplKeypad">APL keypad dialog, for copying.</param>
        private void KeypadTabInit(Keypad keypad, AplKeypad aplKeypad)
        {
            // Create a table layout panel with five columns.
            // The odd columns are auto-size, the even ones are fixed-size.
            var keypadPanel = new TableLayoutPanel
            {
                RowCount = 1,
                ColumnCount = 5,
                Parent = this.fakeKeypadPanel,
            };
            keypadPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            keypadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            keypadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            keypadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            keypadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            keypadPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            keypadPanel.AutoSize = true;
            keypadPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            var panelColumn = 0;

            // Copy the buttons from the keypad.
            // The keypad has three table layout panels, each containing a button embedded in a panel (for animation).
            foreach (var panel in new[] { keypad.newLeftPanel, keypad.newMiddlePanel, keypad.newRightPanel })
            {
                var outerPanel = new Panel
                {
                    BackColor = Color.Black,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Margin = new Padding(0),
                };
                keypadPanel.Controls.Add(outerPanel);
                keypadPanel.SetCellPosition(outerPanel, new TableLayoutPanelCellPosition(panelColumn, 0));
                panelColumn += 2;

                var newPanel = new TableLayoutPanel
                {
                    Parent = outerPanel,
                    Location = new Point(KeyPaneMargin, KeyPaneMargin),
                    Margin = new Padding(KeyPaneMargin),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    BackColor = panel.BackColor,
                    RowCount = panel.RowCount,
                    ColumnCount = panel.ColumnCount,
                };
                foreach (var buttonPanel in panel.Controls.OfType<Panel>())
                {
                    var button = buttonPanel.Controls[0] as Button;
                    var newButton = new Wx3270.NoSelectButton();
                    newPanel.Controls.Add(newButton);
                    newButton.Parent = newPanel;
                    newButton.Name = button.Name;
                    newButton.Tag = button.Tag;
                    newButton.Size = new Size(button.Size.Width + (2 * KeyBorderWidth), button.Size.Height + (2 * KeyBorderWidth));
                    newButton.BackColor = button.BackColor;
                    newButton.BackgroundImage = button.BackgroundImage;
                    newButton.Font = button.Font;
                    newButton.Text = button.Text;
                    newButton.ForeColor = button.ForeColor;
                    newButton.Margin = button.Margin;
                    newButton.Padding = button.Padding;
                    newButton.TabStop = false;
                    newButton.FlatStyle = button.FlatStyle;
                    newButton.FlatAppearance.BorderSize = KeyBorderWidth;
                    newButton.FlatAppearance.BorderColor = Color.Black;

                    newPanel.SetCellPosition(newButton, panel.GetPositionFromControl(buttonPanel));
                    newPanel.SetRowSpan(newButton, panel.GetRowSpan(buttonPanel));

                    this.keypadButtons.Add(ButtonBaseName(button.Name), newButton);

                    newButton.Click += this.KeypadSettingsButtonClick;
                }
            }

            // Create a simple panel for the APL keypad.
            var aplKeypadPanel = new Panel
            {
                BackColor = Color.Black,
                AutoScroll = true,
                Parent = this.fakeKeypadPanel,
                Size = keypadPanel.Size,
                Margin = new Padding(0),
                Visible = false,
            };
            var innerAplKeypadPanel = new Panel
            {
                Parent = aplKeypadPanel,
                BackColor = Color.Black,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0),
            };

            // Copy the buttons from the APL keypad.
            var colAccum = 0;
            var rowAccum = aplKeypad.keyboardPanel.Controls[0].Size.Height / 2;
            var previousY = 0;
            foreach (var buttonPanel in aplKeypad.keyboardPanel.Controls.OfType<Panel>().OrderBy(p => p.Location, new LocationComparer()))
            {
                var button = buttonPanel.Controls[0] as Button;
                var newButton = new Wx3270.NoSelectButton();
                innerAplKeypadPanel.Controls.Add(newButton);
                newButton.Parent = innerAplKeypadPanel;
                newButton.Name = button.Name;
                newButton.Tag = button.Tag;
                newButton.Size = new Size(button.Size.Width + (2 * KeyBorderWidth), button.Size.Height + (2 * KeyBorderWidth));
                newButton.BackColor = button.BackColor;
                newButton.BackgroundImage = button.BackgroundImage;
                newButton.Font = button.Font;
                newButton.Text = button.Text;
                newButton.ForeColor = button.ForeColor;
                newButton.Margin = button.Margin;
                newButton.Padding = button.Padding;
                newButton.TabStop = false;
                newButton.FlatStyle = button.FlatStyle;
                newButton.FlatAppearance.BorderSize = KeyBorderWidth;
                newButton.FlatAppearance.BorderColor = Color.Black;

                if (buttonPanel.Location.Y != previousY)
                {
                    rowAccum += 2 * KeyBorderWidth;
                    colAccum = 0;
                    previousY = buttonPanel.Location.Y;
                }

                newButton.Location = new Point(buttonPanel.Location.X + colAccum, buttonPanel.Location.Y + rowAccum);
                colAccum += 2 * KeyBorderWidth;

                this.keypadButtons.Add(ButtonBaseName(button.Name), newButton);

                newButton.Click += this.KeypadSettingsButtonClick;
            }

            // Set up the keypad type group box.
            this.keypadType = new RadioEnum<KeypadType>(this.KeypadTypeFlowLayoutPanel);
            this.keypadType.Changed += (sender, args) =>
            {
                keypadPanel.Visible = this.keypadType.Value == KeypadType.Type3270;
                aplKeypadPanel.Visible = this.keypadType.Value == KeypadType.TypeApl;
            };

            // Set up the keypad position group box.
            this.keypadPosition = new RadioEnum<KeypadPosition>(this.keypadPositionGroupBox);
            this.keypadPosition.Changed += (sender, args) => this.ProfileManager.PushAndSave((current) => current.KeypadPosition = this.keypadPosition.Value, I18n.Get(KeyboardString.UndoKeypadPosition));

            // Don't highlight the whole actions text box when it gets the focus.
            this.editedButtonActionsTextBox.GotFocus += (sender, args) => this.editedButtonActionsTextBox.Select(this.editedButtonActionsTextBox.Text.Length, 0);

            // Subscribe to profile changes.
            this.ProfileManager.Change += this.KeypadProfileChange;

            // Register for merges.
            this.ProfileManager.RegisterMerge(ImportType.KeypadMerge | ImportType.KeypadReplace, this.KeypadMergeHandler);
        }

        /// <summary>
        /// Merge in the keypad definitions from another profile.
        /// </summary>
        /// <param name="toProfile">Current profile.</param>
        /// <param name="fromProfile">Merge profile.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if the list changed.</returns>
        private bool KeypadMergeHandler(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.KeypadReplace))
            {
                // Replace keypad definitions.
                if (!toProfile.KeypadMap.Equals(fromProfile.KeypadMap))
                {
                    toProfile.KeypadMap = fromProfile.KeypadMap;
                    return true;
                }

                return false;
            }
            else
            {
                // Merge keypad definitions.
                return toProfile.KeypadMap.Merge(fromProfile.KeypadMap);
            }
        }

        /// <summary>
        /// Profile change event handler.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void KeypadProfileChange(Profile profile)
        {
            this.keypadTab.Enabled = profile.ProfileType == ProfileType.Full || profile.ProfileType == ProfileType.KeypadMapTemplate;

            // Copy in the new keypad maps.
            this.editedKeypadMaps = new KeyMap<KeypadMap>(profile.KeypadMap);

            // Repaint.
            this.SelectKeymap();

            // Propagate the keypad position.
            this.keypadPosition.Value = profile.KeypadPosition;
        }

        /// <summary>
        /// A new key or modifiers has been selected. Set labels and buttons to match.
        /// </summary>
        private void SelectKeymap()
        {
            try
            {
                // Avoid validation and text change events.
                this.keymapSelectionInProgress = true;

                // No edited keymap until we find one.
                this.editedKeymap = null;
                this.inheritedKeymap = null;

                var matched = false;
                foreach (var pair in this.keypadButtons)
                {
                    var button = pair.Value;
                    if (this.editedKeypadMaps.TryGetClosestMatch(
                        pair.Key,
                        null,
                        this.currentKeypadMod,
                        out KeypadMap map,
                        out bool exact,
                        out KeyboardModifier matchedModifier,
                        out _))
                    {
                        // Found an entry. Repaint the key.
                        button.Text = map.Text;
                        if (!string.IsNullOrEmpty(button.Text))
                        {
                            button.Font = new Font(button.Font.Name, map.TextSize, button.Font.Style);
                        }

                        button.BackgroundImage = ImageForButton(button, map.BackgroundImage, KeyBorderWidth);

                        if (button == this.currentKeypadButton)
                        {
                            matched = true;

                            // Repaint the editing boxes.
                            this.editedButtonTextTextBox.Text = map.Text;
                            this.keypadTextSize.Value = (decimal)map.TextSize;
                            this.editedButtonActionsTextBox.Text = map.Actions + Environment.NewLine;
                            this.editedButtonActionsTextBox.Select(this.editedButtonActionsTextBox.Text.Length, 0);
                            this.keypadActionsInheritedLabel.Visible = !exact;
                            if (!exact)
                            {
                                var inheritedName = new List<string>();
                                var modifier = matchedModifier == KeyboardModifier.None ? string.Empty : matchedModifier.ToString().Replace(",", string.Empty).Replace(" ", "-");
                                if (!string.IsNullOrEmpty(modifier))
                                {
                                    inheritedName.Add(modifier);
                                }

                                inheritedName.Add(pair.Key.ToString());
                                this.keypadActionsInheritedLabel.Text = string.Format(
                                    I18n.Get(KeyboardString.InheritedFrom),
                                    string.Join("-", inheritedName));
                            }

                            switch (map.BackgroundImage)
                            {
                                default:
                                case KeypadBackgroundImage.Blank:
                                    this.blankRadioButton.Checked = true;
                                    this.editedButtonTextTextBox.Enabled = true;
                                    break;
                                case KeypadBackgroundImage.Insert:
                                    this.insertRadioButton.Checked = true;
                                    this.editedButtonTextTextBox.Enabled = false;
                                    break;
                                case KeypadBackgroundImage.Delete:
                                    this.deleteRadioButton.Checked = true;
                                    this.editedButtonTextTextBox.Enabled = false;
                                    break;
                            }

                            this.insertRadioButton.Enabled = this.currentKeypadButton.Width == this.currentKeypadButton.Height;
                            this.deleteRadioButton.Enabled = this.currentKeypadButton.Width == this.currentKeypadButton.Height;

                            if (exact)
                            {
                                // We have an edited keymap.
                                this.editedKeymap = map;

                                this.toolTip1.SetToolTip(this.editedButtonActionsTextBox, I18n.Get(KeypadString.ClickToEdit));
                                this.keypadRemoveButton.Enabled = true;
                            }
                            else
                            {
                                // We have an inherited keymap.
                                this.inheritedKeymap = map;

                                this.toolTip1.SetToolTip(
                                    this.editedButtonActionsTextBox,
                                    I18n.Get(KeypadString.ActionsInherited) + Environment.NewLine + I18n.Get(KeypadString.ClickToAddMoreSpecific));
                                this.keypadRemoveButton.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        // No mapping.
                        button.Text = string.Empty;
                        button.BackgroundImage = ImageForButton(button, KeypadBackgroundImage.Blank, KeyBorderWidth);
                    }
                }

                if (!matched)
                {
                    // Nothing in the editing boxes.
                    this.editedButtonTextTextBox.Text = string.Empty;
                    this.editedButtonTextTextBox.Enabled = true;
                    this.keypadTextSize.Value = (decimal)DefaultTextSize;
                    this.editedButtonActionsTextBox.Text = string.Empty;
                    this.keypadActionsInheritedLabel.Visible = false;
                    this.blankRadioButton.Checked = true;
                    this.insertRadioButton.Checked = false;
                    this.deleteRadioButton.Checked = false;
                    this.toolTip1.SetToolTip(this.editedButtonActionsTextBox, I18n.Get(KeypadString.ClickToAdd));

                    if (this.currentKeypadButton != null)
                    {
                        this.keypadRemoveButton.Enabled = false;
                        this.blankRadioButton.Checked = true;

                        this.insertRadioButton.Enabled = this.currentKeypadButton.Width == this.currentKeypadButton.Height;
                        this.deleteRadioButton.Enabled = this.currentKeypadButton.Width == this.currentKeypadButton.Height;
                    }
                    else
                    {
                        this.keypadRemoveButton.Enabled = false;
                    }
                }
            }
            finally
            {
                this.keymapSelectionInProgress = false;
            }
        }

        /// <summary>
        /// One of the keypad buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadSettingsButtonClick(object sender, EventArgs e)
        {
            // Turn on this button.
            var button = (Wx3270.NoSelectButton)sender;
            this.currentKeypadButton = button;

            // Repaint the button borders.
            foreach (var b in this.keypadButtons.Values)
            {
                b.FlatAppearance.BorderColor = (b == button) ? Color.Red : Color.Black;
            }

            // Now it's safe to edit.
            this.keypadMappingGroupBox.Enabled = true;

            // Repaint, enable and disable.
            this.SelectKeymap();
        }

        /// <summary>
        /// One of the keypad modifier check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadModifierClick(object sender, EventArgs e)
        {
            var invert = new Dictionary<KeyboardModifier, KeyboardModifier>
            {
                { KeyboardModifier.Mode3270, KeyboardModifier.ModeNvt },
                { KeyboardModifier.ModeNvt, KeyboardModifier.Mode3270 },
            };

            // Figure out the checkbox and the modifier.
            var checkBox = (CheckBox)sender;
            var modifier = (KeyboardModifier)Enum.Parse(typeof(KeyboardModifier), (string)checkBox.Tag);

            if (checkBox.Checked)
            {
                this.currentKeypadMod |= modifier;
            }
            else
            {
                this.currentKeypadMod &= ~modifier;
            }

            // NVT and 3270 are mutually exclusive.
            if (invert.TryGetValue(modifier, out KeyboardModifier otherModifier))
            {
                this.currentKeypadMod &= ~otherModifier;
                checkBox.Parent.Controls.OfType<CheckBox>().Where(c => (KeyboardModifier)Enum.Parse(typeof(KeyboardModifier), (string)c.Tag) == otherModifier).First().Checked = false;
            }

            // Repaint, enable and disable.
            this.SelectKeymap();
        }

        /// <summary>
        /// Save changes to the current key mapping.
        /// </summary>
        private void StoreKeymap()
        {
            if (this.editedKeymap != null)
            {
                this.editedKeypadMaps[KeyMap<KeypadMap>.Key(ButtonBaseName(this.currentKeypadButton.Name), this.currentKeypadMod)] = this.editedKeymap;

                // Repaint the key.
                this.currentKeypadButton.Text = this.editedKeymap.Text;
                if (!string.IsNullOrEmpty(this.editedKeymap.Text))
                {
                    this.currentKeypadButton.Font = new Font(this.currentKeypadButton.Font.Name, this.editedKeymap.TextSize, this.currentKeypadButton.Font.Style);
                }

                this.currentKeypadButton.BackgroundImage = ImageForButton(this.currentKeypadButton, this.editedKeymap.BackgroundImage, KeyBorderWidth);
            }

            // Repaint the options.
            this.SelectKeymap();
        }

        /// <summary>
        /// The actions text box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EditedButtonActionsTextBoxClick(object sender, EventArgs e)
        {
            var text = string.Empty;
            if (this.editedKeymap != null)
            {
                text = this.editedKeymap.Actions;
            }
            else if (this.inheritedKeymap != null)
            {
                text = this.inheritedKeymap.Actions;
            }

            this.EditKeypadActions(text);
        }

        /// <summary>
        /// Start recording a keypad entry.
        /// </summary>
        private void StartRecordingKeypad()
        {
            this.app.MacroRecorder.Start(this.KeypadRecordingComplete);
            this.Hide();
            this.mainScreen.Focus();
        }

        /// <summary>
        /// Edit the actions for an existing keypad map.
        /// </summary>
        /// <param name="text">Macro text.</param>
        private void EditKeypadActions(string text)
        {
            using var editor = new MacroEditor(text, I18n.Get(KeypadString.KeypadActions), false, this.app);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var textBox = this.editedButtonActionsTextBox;
                textBox.Text = editor.MacroText + Environment.NewLine;
                if (this.editedKeymap != null && this.editedKeymap.Actions != editor.MacroText)
                {
                    this.editedKeymap.Actions = editor.MacroText;
                    this.StoreKeymap();
                }
                else if (this.editedKeymap == null)
                {
                    this.editedKeymap = new KeypadMap(this.inheritedKeymap) { Actions = editor.MacroText };
                    this.StoreKeymap();
                }

                this.PushAndSaveKeypad(I18n.Get(KeypadString.ChangeKeypadActions));
            }
            else if (result == DialogResult.Retry)
            {
                // Restart macro recorder.
                this.StartRecordingKeypad();
            }
        }

        /// <summary>
        /// Macro recording is complete.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="context">Completion context.</param>
        private void KeypadRecordingComplete(string text, object context)
        {
            this.Show();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            this.EditKeypadActions(text);
        }

        /// <summary>
        /// The label size text box was changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadTextSizeValidating(object sender, CancelEventArgs e)
        {
            float f = (float)this.keypadTextSize.Value;
            if (this.editedKeymap != null && this.editedKeymap.TextSize != f)
            {
                this.editedKeymap.TextSize = f;
                this.StoreKeymap();
            }

            if (this.editedKeymap == null && f != DefaultTextSize)
            {
                // Create a new entry.
                this.editedKeymap = new KeypadMap(this.inheritedKeymap) { TextSize = f };
                this.StoreKeymap();
            }

            this.PushAndSaveKeypad(I18n.Get(KeypadString.ChangeKeyTextSize));
        }

        /// <summary>
        /// The keypad remove button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadRemoveButtonClick(object sender, EventArgs e)
        {
            this.editedKeypadMaps.Remove(KeyMap<KeypadMap>.Key(ButtonBaseName(this.currentKeypadButton.Name), this.currentKeypadMod));
            this.SelectKeymap();

            this.PushAndSaveKeypad(I18n.Get(KeypadString.RemoveKeyDefinition));
        }

        /// <summary>
        /// The label text changed for a keypad map.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeypadTextChanged(object sender, EventArgs e)
        {
            if (this.keymapSelectionInProgress)
            {
                return;
            }

            if (this.editedKeymap == null)
            {
                if (!string.IsNullOrWhiteSpace(this.editedButtonTextTextBox.Text))
                {
                    this.editedKeymap = new KeypadMap(this.inheritedKeymap) { Text = this.editedButtonTextTextBox.Text };
                    this.StoreKeymap();
                }
            }
            else if (this.editedKeymap.Text != this.editedButtonTextTextBox.Text)
            {
                this.editedKeymap.Text = this.editedButtonTextTextBox.Text;
                this.StoreKeymap();
            }

            // Don't push the change into the profile until the field is validated (exited).
            ////this.PushAndSaveKeypad("change key text");
        }

        /// <summary>
        /// One of the background image radio buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BackgroundImageClick(object sender, EventArgs e)
        {
            if (this.keymapSelectionInProgress)
            {
                return;
            }

            var button = (RadioButton)sender;
            if (!button.Checked)
            {
                return;
            }

            var image = (KeypadBackgroundImage)Enum.Parse(typeof(KeypadBackgroundImage), (string)button.Tag);

            if (this.editedKeymap == null)
            {
                this.editedKeymap = new KeypadMap(this.inheritedKeymap) { BackgroundImage = image };
                this.editedKeymap.BackgroundImage = image;
                if (image == KeypadBackgroundImage.Blank)
                {
                    this.editedButtonTextTextBox.Enabled = true;
                }
                else
                {
                    this.editedKeymap.Text = string.Empty;
                    this.editedButtonTextTextBox.Enabled = false;
                }

                this.StoreKeymap();
            }
            else if (this.editedKeymap.BackgroundImage != image)
            {
                this.editedKeymap.BackgroundImage = image;
                if (image == KeypadBackgroundImage.Blank)
                {
                    this.editedButtonTextTextBox.Enabled = true;
                }
                else
                {
                    this.editedKeymap.Text = string.Empty;
                    this.editedButtonTextTextBox.Enabled = false;
                }

                this.StoreKeymap();
            }

            this.PushAndSaveKeypad(I18n.Get(KeypadString.ChangeKeyBackground));
        }

        /// <summary>
        /// The keypad label text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EditedButtonTextTextBoxValidated(object sender, EventArgs e)
        {
            this.PushAndSaveKeypad(I18n.Get(KeypadString.ChangeKeyText));
        }

        /// <summary>
        /// Push a keypad change out to the profile and update the displayed keymap.
        /// </summary>
        /// <param name="changeName">Name of change, for undo/redo.</param>
        private void PushAndSaveKeypad(string changeName)
        {
            if (this.ProfileManager.PushAndSave((current) => current.KeypadMap = new KeyMap<KeypadMap>(this.editedKeypadMaps), changeName))
            {
                this.keypad.ApplyMaps(this.editedKeypadMaps);
            }
        }

        /// <summary>
        /// Localized strings.
        /// </summary>
        private static class KeypadString
        {
            /// <summary>
            /// Tool tip: Click to edit actions.
            /// </summary>
            public static readonly string ClickToEdit = I18n.Combine(KeypadStringBase, "ClickToEdit");

            /// <summary>
            /// Tool tip: Actions are inherited.
            /// </summary>
            public static readonly string ActionsInherited = I18n.Combine(KeypadStringBase, "ActionsInherited");

            /// <summary>
            /// Tool tip: Click to add more specific actions.
            /// </summary>
            public static readonly string ClickToAddMoreSpecific = I18n.Combine(KeypadStringBase, "ClickToAddMoreSpecific");

            /// <summary>
            /// Tool tip: Click to add actions.
            /// </summary>
            public static readonly string ClickToAdd = I18n.Combine(KeypadStringBase, "ClickToAdd");

            /// <summary>
            /// Window title: Keymap Actions.
            /// </summary>
            public static readonly string KeypadActions = I18n.Combine(KeypadStringBase, "KeypadActions");

            /// <summary>
            /// Change description: Keypad actions.
            /// </summary>
            public static readonly string ChangeKeypadActions = I18n.Combine(KeypadStringBase, "ChangeKeypadActions");

            /// <summary>
            /// Change description: Key text size.
            /// </summary>
            public static readonly string ChangeKeyTextSize = I18n.Combine(KeypadStringBase, "ChangeKeyTextSize");

            /// <summary>
            /// Change description: Remove key definition.
            /// </summary>
            public static readonly string RemoveKeyDefinition = I18n.Combine(KeypadStringBase, "RemoveKeyDefinition");

            /// <summary>
            /// Change description: Key background.
            /// </summary>
            public static readonly string ChangeKeyBackground = I18n.Combine(KeypadStringBase, "ChangeKeyBackground");

            /// <summary>
            /// Change description: Key text.
            /// </summary>
            public static readonly string ChangeKeyText = I18n.Combine(KeypadStringBase, "ChangeKeyText");
        }

        /// <summary>
        /// Class for comparing two points.
        /// </summary>
        private class LocationComparer : IComparer<Point>
        {
            public int Compare(Point a, Point b)
            {
                if (a.Y < b.Y)
                {
                    return -1;
                }

                if (a.Y > b.Y)
                {
                    return 1;
                }

                if (a.X < b.X)
                {
                    return -1;
                }

                if (a.X > b.X)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
