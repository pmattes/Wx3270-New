// <copyright file="DefaultKeypadMap.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using I18nBase;

    /// <summary>
    /// Default keypad map.
    /// </summary>
    public class DefaultKeypadMap
    {
        /// <summary>
        /// Default definitions for the pop-up keypad.
        /// </summary>
        public static readonly KeyMap<KeypadMap> Map = new KeyMap<KeypadMap>
        {
            { KeyMap<KeypadMap>.Key("PF1", KeyboardModifier.None), new KeypadMap { Text = "PF1", TextSize = 8.25F, Actions = B3270.Action.PF + "(1)" } },
            { KeyMap<KeypadMap>.Key("PF1", KeyboardModifier.Shift), new KeypadMap { Text = "PF13", TextSize = 8.25F, Actions = B3270.Action.PF + "(13)" } },
            { KeyMap<KeypadMap>.Key("PF2", KeyboardModifier.None), new KeypadMap { Text = "PF2", TextSize = 8.25F, Actions = B3270.Action.PF + "(2)" } },
            { KeyMap<KeypadMap>.Key("PF2", KeyboardModifier.Shift), new KeypadMap { Text = "PF14", TextSize = 8.25F, Actions = B3270.Action.PF + "(14)" } },
            { KeyMap<KeypadMap>.Key("PF3", KeyboardModifier.None), new KeypadMap { Text = "PF3", TextSize = 8.25F, Actions = B3270.Action.PF + "(3)" } },
            { KeyMap<KeypadMap>.Key("PF3", KeyboardModifier.Shift), new KeypadMap { Text = "PF15", TextSize = 8.25F, Actions = B3270.Action.PF + "(15)" } },
            { KeyMap<KeypadMap>.Key("PF4", KeyboardModifier.None), new KeypadMap { Text = "PF4", TextSize = 8.25F, Actions = B3270.Action.PF + "(4)" } },
            { KeyMap<KeypadMap>.Key("PF4", KeyboardModifier.Shift), new KeypadMap { Text = "PF16", TextSize = 8.25F, Actions = B3270.Action.PF + "(16)" } },
            { KeyMap<KeypadMap>.Key("PF5", KeyboardModifier.None), new KeypadMap { Text = "PF5", TextSize = 8.25F, Actions = B3270.Action.PF + "(5)" } },
            { KeyMap<KeypadMap>.Key("PF5", KeyboardModifier.Shift), new KeypadMap { Text = "PF17", TextSize = 8.25F, Actions = B3270.Action.PF + "(17)" } },
            { KeyMap<KeypadMap>.Key("PF6", KeyboardModifier.None), new KeypadMap { Text = "PF6", TextSize = 8.25F, Actions = B3270.Action.PF + "(6)" } },
            { KeyMap<KeypadMap>.Key("PF6", KeyboardModifier.Shift), new KeypadMap { Text = "PF18", TextSize = 8.25F, Actions = B3270.Action.PF + "(18)" } },
            { KeyMap<KeypadMap>.Key("PF7", KeyboardModifier.None), new KeypadMap { Text = "PF7", TextSize = 8.25F, Actions = B3270.Action.PF + "(7)" } },
            { KeyMap<KeypadMap>.Key("PF7", KeyboardModifier.Shift), new KeypadMap { Text = "PF19", TextSize = 8.25F, Actions = B3270.Action.PF + "(19)" } },
            { KeyMap<KeypadMap>.Key("PF8", KeyboardModifier.None), new KeypadMap { Text = "PF8", TextSize = 8.25F, Actions = B3270.Action.PF + "(8)" } },
            { KeyMap<KeypadMap>.Key("PF8", KeyboardModifier.Shift), new KeypadMap { Text = "PF20", TextSize = 8.25F, Actions = B3270.Action.PF + "(20)" } },
            { KeyMap<KeypadMap>.Key("PF9", KeyboardModifier.None), new KeypadMap { Text = "PF9", TextSize = 8.25F, Actions = B3270.Action.PF + "(9)" } },
            { KeyMap<KeypadMap>.Key("PF9", KeyboardModifier.Shift), new KeypadMap { Text = "PF21", TextSize = 8.25F, Actions = B3270.Action.PF + "(21)" } },
            { KeyMap<KeypadMap>.Key("PF10", KeyboardModifier.None), new KeypadMap { Text = "PF10", TextSize = 8.25F, Actions = B3270.Action.PF + "(10)" } },
            { KeyMap<KeypadMap>.Key("PF10", KeyboardModifier.Shift), new KeypadMap { Text = "PF22", TextSize = 8.25F, Actions = B3270.Action.PF + "(22)" } },
            { KeyMap<KeypadMap>.Key("PF11", KeyboardModifier.None), new KeypadMap { Text = "PF11", TextSize = 8.25F, Actions = B3270.Action.PF + "(11)" } },
            { KeyMap<KeypadMap>.Key("PF11", KeyboardModifier.Shift), new KeypadMap { Text = "PF23", TextSize = 8.25F, Actions = B3270.Action.PF + "(23)" } },
            { KeyMap<KeypadMap>.Key("PF12", KeyboardModifier.None), new KeypadMap { Text = "PF12", TextSize = 8.25F, Actions = B3270.Action.PF + "(12)" } },
            { KeyMap<KeypadMap>.Key("PF12", KeyboardModifier.Shift), new KeypadMap { Text = "PF24", TextSize = 8.25F, Actions = B3270.Action.PF + "(24)" } },
            { KeyMap<KeypadMap>.Key("PA1", KeyboardModifier.None), new KeypadMap { Text = "PA1", TextSize = 8.25F, Actions = B3270.Action.PA + "(1)" } },
            { KeyMap<KeypadMap>.Key("PA2", KeyboardModifier.None), new KeypadMap { Text = "PA2", TextSize = 8.25F, Actions = B3270.Action.PA + "(2)" } },
            { KeyMap<KeypadMap>.Key("Not", KeyboardModifier.None), new KeypadMap { Text = "¬", TextSize = 14F, Actions = B3270.Action.Key + "(U+00AC)" } },
            { KeyMap<KeypadMap>.Key("Tab", KeyboardModifier.None), new KeypadMap { Text = "→|", TextSize = 14.25F, Actions = B3270.Action.Tab + "()" } },
            { KeyMap<KeypadMap>.Key("BackTab", KeyboardModifier.None), new KeypadMap { Text = "|←", TextSize = 14.25F, Actions = B3270.Action.BackTab + "()" } },
            { KeyMap<KeypadMap>.Key("Erase", KeyboardModifier.None), new KeypadMap { Text = "←", TextSize = 14.25F, Actions = B3270.Action.Erase + "()" } },
            { KeyMap<KeypadMap>.Key("Newline", KeyboardModifier.None), new KeypadMap { Text = "⤶", TextSize = 27.75F, Actions = B3270.Action.Newline + "()" } },
            { KeyMap<KeypadMap>.Key("Reset", KeyboardModifier.None), new KeypadMap { Text = "RESET", TextSize = 6.75F, Actions = B3270.Action.Reset + "()" } },
            { KeyMap<KeypadMap>.Key("Enter", KeyboardModifier.None), new KeypadMap { Text = "ENTER", TextSize = 6.75F, Actions = B3270.Action.Enter + "()" } },
            { KeyMap<KeypadMap>.Key("EraseInput", KeyboardModifier.None), new KeypadMap { Text = "ERASE" + Environment.NewLine + "INPUT", TextSize = 6F, Actions = B3270.Action.EraseInput + "()" } },
            { KeyMap<KeypadMap>.Key("CursorSelect", KeyboardModifier.None), new KeypadMap { Text = "CURSR" + Environment.NewLine + "SEL", TextSize = 6F, Actions = B3270.Action.CursorSelect + "()" } },
            { KeyMap<KeypadMap>.Key("Clear", KeyboardModifier.None), new KeypadMap { Text = "CLEAR", TextSize = 6F, Actions = B3270.Action.Clear + "()" } },
            { KeyMap<KeypadMap>.Key("EraseEOF", KeyboardModifier.None), new KeypadMap { Text = "ERASE" + Environment.NewLine + "EOF", TextSize = 6F, Actions = B3270.Action.EraseEOF + "()" } },
            { KeyMap<KeypadMap>.Key("Dup", KeyboardModifier.None), new KeypadMap { Text = "DUP", TextSize = 6F, Actions = B3270.Action.Dup + "()" } },
            { KeyMap<KeypadMap>.Key("FieldMark", KeyboardModifier.None), new KeypadMap { Text = "FIELD" + Environment.NewLine + "MARK", TextSize = 6F, Actions = B3270.Action.FieldMark + "()" } },
            { KeyMap<KeypadMap>.Key("Attn", KeyboardModifier.None), new KeypadMap { Text = "ATTN", TextSize = 6F, Actions = B3270.Action.Attn + "()" } },
            { KeyMap<KeypadMap>.Key("CursorBlink", KeyboardModifier.None), new KeypadMap { Text = "CURSR" + Environment.NewLine + "BLINK", TextSize = 6F, Actions = B3270.Action.Toggle + "(" + B3270.Setting.CursorBlink + ")" } },
            { KeyMap<KeypadMap>.Key("SysReq", KeyboardModifier.None), new KeypadMap { Text = "SYS" + Environment.NewLine + "REQ", TextSize = 6F, Actions = B3270.Action.SysReq + "()" } },
            { KeyMap<KeypadMap>.Key("Insert", KeyboardModifier.None), new KeypadMap { Actions = B3270.Action.Toggle + "(" + B3270.Setting.InsertMode + ")", BackgroundImage = KeypadBackgroundImage.Insert } },
            { KeyMap<KeypadMap>.Key("Up", KeyboardModifier.None), new KeypadMap { Text = "↑", TextSize = 14.25F, Actions = B3270.Action.Up + "()" } },
            { KeyMap<KeypadMap>.Key("Delete", KeyboardModifier.None), new KeypadMap { Actions = B3270.Action.Delete + "()", BackgroundImage = KeypadBackgroundImage.Delete } },
            { KeyMap<KeypadMap>.Key("Left", KeyboardModifier.None), new KeypadMap { Text = "←", TextSize = 14.25F, Actions = B3270.Action.Left + "()" } },
            { KeyMap<KeypadMap>.Key("Down", KeyboardModifier.None), new KeypadMap { Text = "↓", TextSize = 14.25F, Actions = B3270.Action.Down + "()" } },
            { KeyMap<KeypadMap>.Key("Right", KeyboardModifier.None), new KeypadMap { Text = "→", TextSize = 14.25F, Actions = B3270.Action.Right + "()" } },
            { KeyMap<KeypadMap>.Key("BlankButton8", KeyboardModifier.Mode3270), new KeypadMap { Text = "SAVE" + Environment.NewLine + "A", TextSize = 6.0F, Actions = B3270.Action.SaveInput + "(A)" } },
            { KeyMap<KeypadMap>.Key("BlankButton8", KeyboardModifier.Shift | KeyboardModifier.Mode3270), new KeypadMap { Text = "SAVE" + Environment.NewLine + "B", TextSize = 6.0F, Actions = B3270.Action.SaveInput + "(B)" } },
            { KeyMap<KeypadMap>.Key("BlankButton8", KeyboardModifier.Ctrl | KeyboardModifier.Mode3270), new KeypadMap { Text = "SAVE" + Environment.NewLine + "C", TextSize = 6.0F, Actions = B3270.Action.SaveInput + "(C)" } },
            { KeyMap<KeypadMap>.Key("BlankButton8", KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeypadMap { Text = "SAVE" + Environment.NewLine + "D", TextSize = 6.0F, Actions = B3270.Action.SaveInput + "(D)" } },
            { KeyMap<KeypadMap>.Key("BlankButton9", KeyboardModifier.Mode3270), new KeypadMap { Text = "RE-" + Environment.NewLine + "STORE" + Environment.NewLine + "A", TextSize = 6.0F, Actions = B3270.Action.RestoreInput + "(A)" } },
            { KeyMap<KeypadMap>.Key("BlankButton9", KeyboardModifier.Shift | KeyboardModifier.Mode3270), new KeypadMap { Text = "RE-" + Environment.NewLine + "STORE" + Environment.NewLine + "B", TextSize = 6.0F, Actions = B3270.Action.RestoreInput + "(B)" } },
            { KeyMap<KeypadMap>.Key("BlankButton9", KeyboardModifier.Ctrl | KeyboardModifier.Mode3270), new KeypadMap { Text = "RE-" + Environment.NewLine + "STORE" + Environment.NewLine + "C", TextSize = 6.0F, Actions = B3270.Action.RestoreInput + "(C)" } },
            { KeyMap<KeypadMap>.Key("BlankButton9", KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeypadMap { Text = "RE-" + Environment.NewLine + "STORE" + Environment.NewLine + "D", TextSize = 6.0F, Actions = B3270.Action.RestoreInput + "(D)" } },

            // APL keypad.
            { KeyMap<KeypadMap>.Key("grave", KeyboardModifier.None), new KeypadMap { Text = "⋄", TextSize = 16F, Actions = B3270.Action.Key + "(apl_diamond)" } },
            { KeyMap<KeypadMap>.Key("one", KeyboardModifier.None), new KeypadMap { Text = "¨", TextSize = 14F, Actions = B3270.Action.Key + "(apl_diaeresis)" } },
            { KeyMap<KeypadMap>.Key("one", KeyboardModifier.Shift), new KeypadMap { Text = "⌶", TextSize = 14F, Actions = B3270.Action.Key + "(apl_downtackup)" } },
            { KeyMap<KeypadMap>.Key("two", KeyboardModifier.None), new KeypadMap { Text = "¯", TextSize = 14F, Actions = B3270.Action.Key + "(apl_overbar)" } },
            { KeyMap<KeypadMap>.Key("two", KeyboardModifier.Shift), new KeypadMap { Text = "⍫", TextSize = 14F, Actions = B3270.Action.Key + "(apl_deltilde)" } },
            { KeyMap<KeypadMap>.Key("three", KeyboardModifier.None), new KeypadMap { Text = "<", TextSize = 14F, Actions = B3270.Action.Key + "(\"<\")" } },
            { KeyMap<KeypadMap>.Key("three", KeyboardModifier.Shift), new KeypadMap { Text = "⍒", TextSize = 14F, Actions = B3270.Action.Key + "(apl_delstile)" } },
            { KeyMap<KeypadMap>.Key("four", KeyboardModifier.None), new KeypadMap { Text = "≤", TextSize = 14F, Actions = B3270.Action.Key + "(apl_notgreater)" } },
            { KeyMap<KeypadMap>.Key("four", KeyboardModifier.Shift), new KeypadMap { Text = "⍋", TextSize = 14F, Actions = B3270.Action.Key + "(apl_deltastile)" } },
            { KeyMap<KeypadMap>.Key("five", KeyboardModifier.None), new KeypadMap { Text = "=", TextSize = 14F, Actions = B3270.Action.Key + "(=)" } },
            { KeyMap<KeypadMap>.Key("five", KeyboardModifier.Shift), new KeypadMap { Text = "⌽", TextSize = 14F, Actions = B3270.Action.Key + "(apl_circlestile)" } },
            { KeyMap<KeypadMap>.Key("six", KeyboardModifier.None), new KeypadMap { Text = "≥", TextSize = 14F, Actions = B3270.Action.Key + "(apl_notless)" } },
            { KeyMap<KeypadMap>.Key("six", KeyboardModifier.Shift), new KeypadMap { Text = "⍉", TextSize = 14F, Actions = B3270.Action.Key + "(apl_circleslope)" } },
            { KeyMap<KeypadMap>.Key("seven", KeyboardModifier.None), new KeypadMap { Text = ">", TextSize = 14F, Actions = B3270.Action.Key + "(\">\")" } },
            { KeyMap<KeypadMap>.Key("seven", KeyboardModifier.Shift), new KeypadMap { Text = "⊖", TextSize = 14F, Actions = B3270.Action.Key + "(apl_circlebar)" } },
            { KeyMap<KeypadMap>.Key("eight", KeyboardModifier.None), new KeypadMap { Text = "≠", TextSize = 14F, Actions = B3270.Action.Key + "(apl_notequal)" } },
            { KeyMap<KeypadMap>.Key("eight", KeyboardModifier.Shift), new KeypadMap { Text = "⍟", TextSize = 16F, Actions = B3270.Action.Key + "(apl_circlestar)" } },
            { KeyMap<KeypadMap>.Key("nine", KeyboardModifier.None), new KeypadMap { Text = "∨", TextSize = 14F, Actions = B3270.Action.Key + "(apl_downcaret)" } },
            { KeyMap<KeypadMap>.Key("nine", KeyboardModifier.Shift), new KeypadMap { Text = "⍱", TextSize = 16F, Actions = B3270.Action.Key + "(apl_downcarettilde)" } },
            { KeyMap<KeypadMap>.Key("zero", KeyboardModifier.None), new KeypadMap { Text = "∧", TextSize = 14F, Actions = B3270.Action.Key + "(apl_upcaret)" } },
            { KeyMap<KeypadMap>.Key("zero", KeyboardModifier.Shift), new KeypadMap { Text = "⍲", TextSize = 16F, Actions = B3270.Action.Key + "(apl_upcarettilde)" } },
            { KeyMap<KeypadMap>.Key("minus", KeyboardModifier.None), new KeypadMap { Text = "×", TextSize = 14F, Actions = B3270.Action.Key + "(apl_multiply)" } },
            { KeyMap<KeypadMap>.Key("minus", KeyboardModifier.Shift), new KeypadMap { Text = "!", TextSize = 14F, Actions = B3270.Action.Key + "(apl_quotedot)" } },
            { KeyMap<KeypadMap>.Key("equals", KeyboardModifier.None), new KeypadMap { Text = "÷", TextSize = 14F, Actions = B3270.Action.Key + "(apl_divide)" } },
            { KeyMap<KeypadMap>.Key("equals", KeyboardModifier.Shift), new KeypadMap { Text = "⌹", TextSize = 14F, Actions = B3270.Action.Key + "(apl_quaddivide)" } },
            { KeyMap<KeypadMap>.Key("q", KeyboardModifier.None), new KeypadMap { Text = "?", TextSize = 14F, Actions = B3270.Action.Key + "(?)" } },
            { KeyMap<KeypadMap>.Key("w", KeyboardModifier.None), new KeypadMap { Text = "⍵", TextSize = 14F, Actions = B3270.Action.Key + "(apl_omega)" } },
            { KeyMap<KeypadMap>.Key("e", KeyboardModifier.None), new KeypadMap { Text = "∊", TextSize = 20F, Actions = B3270.Action.Key + "(apl_epsilon)" } },
            { KeyMap<KeypadMap>.Key("e", KeyboardModifier.Shift), new KeypadMap { Text = "⍷", TextSize = 14F, Actions = B3270.Action.Key + "(apl_epsilonunderbar)" } },
            { KeyMap<KeypadMap>.Key("r", KeyboardModifier.None), new KeypadMap { Text = "⍴", TextSize = 14F, Actions = B3270.Action.Key + "(apl_rho)" } },
            { KeyMap<KeypadMap>.Key("t", KeyboardModifier.None), new KeypadMap { Text = "~", TextSize = 14F, Actions = B3270.Action.Key + "(apl_tilde)" } },
            { KeyMap<KeypadMap>.Key("y", KeyboardModifier.None), new KeypadMap { Text = "↑", TextSize = 12F, Actions = B3270.Action.Key + "(apl_uparrow)" } },
            { KeyMap<KeypadMap>.Key("u", KeyboardModifier.None), new KeypadMap { Text = "↓", TextSize = 12F, Actions = B3270.Action.Key + "(apl_downarrow)" } },
            { KeyMap<KeypadMap>.Key("i", KeyboardModifier.None), new KeypadMap { Text = "⍳", TextSize = 14F, Actions = B3270.Action.Key + "(apl_iota)" } },
            { KeyMap<KeypadMap>.Key("i", KeyboardModifier.Shift), new KeypadMap { Text = "⍸", TextSize = 14F, Actions = B3270.Action.Key + "(apl_iotaunderbar)" } },
            { KeyMap<KeypadMap>.Key("o", KeyboardModifier.None), new KeypadMap { Text = "○", TextSize = 14F, Actions = B3270.Action.Key + "(apl_circle)" } },
            { KeyMap<KeypadMap>.Key("o", KeyboardModifier.Shift), new KeypadMap { Text = "⍥", TextSize = 14F, Actions = B3270.Action.Key + "(apl_diaeresiscircle)" } },
            { KeyMap<KeypadMap>.Key("p", KeyboardModifier.None), new KeypadMap { Text = "*", TextSize = 14F, Actions = B3270.Action.Key + "(apl_star)" } },
            { KeyMap<KeypadMap>.Key("leftBracket", KeyboardModifier.None), new KeypadMap { Text = "←", TextSize = 12F, Actions = B3270.Action.Key + "(apl_leftarrow)" } },
            { KeyMap<KeypadMap>.Key("leftBracket", KeyboardModifier.Shift), new KeypadMap { Text = "⍞", TextSize = 14F, Actions = B3270.Action.Key + "(apl_quadquote)" } },
            { KeyMap<KeypadMap>.Key("rightBracket", KeyboardModifier.None), new KeypadMap { Text = "→", TextSize = 12F, Actions = B3270.Action.Key + "(apl_rightarrow)" } },
            { KeyMap<KeypadMap>.Key("backslash", KeyboardModifier.None), new KeypadMap { Text = "⊢", TextSize = 14F, Actions = B3270.Action.Key + "(apl_righttack)" } },
            { KeyMap<KeypadMap>.Key("backslash", KeyboardModifier.Shift), new KeypadMap { Text = "⊣", TextSize = 14F, Actions = B3270.Action.Key + "(apl_lefttack)" } },
            { KeyMap<KeypadMap>.Key("a", KeyboardModifier.None), new KeypadMap { Text = "⍺", TextSize = 14F, Actions = B3270.Action.Key + "(apl_alpha)" } },
            { KeyMap<KeypadMap>.Key("s", KeyboardModifier.None), new KeypadMap { Text = "⌈", TextSize = 14F, Actions = B3270.Action.Key + "(apl_upstile)" } },
            { KeyMap<KeypadMap>.Key("d", KeyboardModifier.None), new KeypadMap { Text = "⌊", TextSize = 14F, Actions = B3270.Action.Key + "(apl_downstile)" } },
            { KeyMap<KeypadMap>.Key("f", KeyboardModifier.None), new KeypadMap { Text = "_", TextSize = 14F, Actions = B3270.Action.Key + "(_)" } },
            { KeyMap<KeypadMap>.Key("g", KeyboardModifier.None), new KeypadMap { Text = "∇", TextSize = 14F, Actions = B3270.Action.Key + "(apl_del)" } },
            { KeyMap<KeypadMap>.Key("h", KeyboardModifier.None), new KeypadMap { Text = "∆", TextSize = 11F, Actions = B3270.Action.Key + "(apl_delta)" } },
            { KeyMap<KeypadMap>.Key("j", KeyboardModifier.None), new KeypadMap { Text = "∘", TextSize = 14F, Actions = B3270.Action.Key + "(apl_jot)" } },
            { KeyMap<KeypadMap>.Key("j", KeyboardModifier.Shift), new KeypadMap { Text = "⍤", TextSize = 14F, Actions = B3270.Action.Key + "(apl_diaeresisjot)" } },
            { KeyMap<KeypadMap>.Key("k", KeyboardModifier.None), new KeypadMap { Text = "'", TextSize = 14F, Actions = B3270.Action.Key + "(apl_quote)" } },
            { KeyMap<KeypadMap>.Key("l", KeyboardModifier.None), new KeypadMap { Text = "⎕", TextSize = 14F, Actions = B3270.Action.Key + "(apl_quad)" } },
            { KeyMap<KeypadMap>.Key("l", KeyboardModifier.Shift), new KeypadMap { Text = "⌷", TextSize = 14F, Actions = B3270.Action.Key + "(apl_squad)" } },
            { KeyMap<KeypadMap>.Key("semicolon", KeyboardModifier.None), new KeypadMap { Text = "⍎", TextSize = 14F, Actions = B3270.Action.Key + "(apl_downtackjot)" } },
            { KeyMap<KeypadMap>.Key("semicolon", KeyboardModifier.Shift), new KeypadMap { Text = "≡", TextSize = 14F, Actions = B3270.Action.Key + "(apl_equiv)" } },
            { KeyMap<KeypadMap>.Key("apostrophe", KeyboardModifier.None), new KeypadMap { Text = "⍕", TextSize = 14F, Actions = B3270.Action.Key + "(apl_uptackjot)" } },
            { KeyMap<KeypadMap>.Key("z", KeyboardModifier.None), new KeypadMap { Text = "⊂", TextSize = 14F, Actions = B3270.Action.Key + "(apl_leftshoe)" } },
            { KeyMap<KeypadMap>.Key("x", KeyboardModifier.None), new KeypadMap { Text = "⊃", TextSize = 14F, Actions = B3270.Action.Key + "(apl_rightshoe)" } },
            { KeyMap<KeypadMap>.Key("c", KeyboardModifier.None), new KeypadMap { Text = "∩", TextSize = 8F, Actions = B3270.Action.Key + "(apl_upshoe)" } },
            { KeyMap<KeypadMap>.Key("v", KeyboardModifier.None), new KeypadMap { Text = "∪", TextSize = 14F, Actions = B3270.Action.Key + "(apl_downshoe)" } },
            { KeyMap<KeypadMap>.Key("b", KeyboardModifier.None), new KeypadMap { Text = "⊥", TextSize = 14F, Actions = B3270.Action.Key + "(apl_uptack)" } },
            { KeyMap<KeypadMap>.Key("n", KeyboardModifier.None), new KeypadMap { Text = "⊤", TextSize = 14F, Actions = B3270.Action.Key + "(apl_downtack)" } },
            { KeyMap<KeypadMap>.Key("m", KeyboardModifier.None), new KeypadMap { Text = "|", TextSize = 14F, Actions = B3270.Action.Key + "(apl_stile)" } },
            { KeyMap<KeypadMap>.Key("comma", KeyboardModifier.None), new KeypadMap { Text = "⍝", TextSize = 14F, Actions = B3270.Action.Key + "(apl_upshoejot)" } },
            { KeyMap<KeypadMap>.Key("comma", KeyboardModifier.Shift), new KeypadMap { Text = "⍪", TextSize = 20F, Actions = B3270.Action.Key + "(apl_commabar)" } },
            { KeyMap<KeypadMap>.Key("period", KeyboardModifier.None), new KeypadMap { Text = "⍀", TextSize = 14F, Actions = B3270.Action.Key + "(apl_slopebar)" } },
            { KeyMap<KeypadMap>.Key("period", KeyboardModifier.Shift), new KeypadMap { Text = "⍙", TextSize = 14F, Actions = B3270.Action.Key + "(apl_deltaunderbar)" } },
            { KeyMap<KeypadMap>.Key("slash", KeyboardModifier.None), new KeypadMap { Text = "⌿", TextSize = 14F, Actions = B3270.Action.Key + "(apl_slashbar)" } },
        };

        /// <summary>
        /// Performs static localization.
        /// </summary>
        /// <remarks>
        /// This is called from localization, very early in initiailization, due to the <see cref="I18nInitAttribute"/> attribute.
        /// </remarks>
        [I18nInit]
        public static void Localize()
        {
            // Localize the default keypad map.
            foreach (var kv in Map)
            {
                if (IsLocalized(kv.Key))
                {
                    I18n.LocalizeGlobal(KeyLocalizeName(kv.Key), kv.Value.Text, always: true);
                    I18n.LocalizeGlobal(KeyLocalizeSizeName(kv.Key), kv.Value.TextSize.ToString(), always: true);
                }
            }
        }

        /// <summary>
        /// Returns the localization name for a keymap entry.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns>Localization name.</returns>
        public static string KeyLocalizeName(string key)
        {
            return I18n.Combine("Keypad", key);
        }

        /// <summary>
        /// Returns the size localization name for a keymap entry.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns>Localization name.</returns>
        public static string KeyLocalizeSizeName(string key)
        {
            return I18n.Combine("Keypad", key, "Size");
        }

        /// <summary>
        /// Tests a key for localization.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns>True if key is localized.</returns>
        public static bool IsLocalized(string key)
        {
            return char.IsUpper(key[0]) && !key.StartsWith("PF") && !key.StartsWith("PA");
        }

        /// <summary>
        /// Returns the localized text for a default keymap entry.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="map">Keymap entry.</param>
        /// <returns>Localized name.</returns>
        public static string LocalizeText(string key, KeypadMap map)
        {
            if (IsLocalized(key))
            {
                return I18n.Get(KeyLocalizeName(key));
            }

            return map.Text;
        }

        /// <summary>
        /// Returned the localized text size for a default keymap entry.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="map">Keymap entry.</param>
        /// <returns>Localized size.</returns>
        public static float LocalizeTextSize(string key, KeypadMap map)
        {
            if (IsLocalized(key))
            {
                if (float.TryParse(I18n.Get(KeyLocalizeSizeName(key)), out float size))
                {
                    return size;
                }

                return 6.00F;
            }

            return map.TextSize;
        }
    }
}