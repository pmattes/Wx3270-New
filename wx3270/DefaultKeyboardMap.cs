// <copyright file="DefaultKeyboardMap.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    /// <summary>
    /// Configurable options, serialized in a file.
    /// </summary>
    public partial class Profile
    {
        /// <summary>
        /// Default keyboard map.
        /// </summary>
        private static readonly KeyMap<KeyboardMap> DefaultKeyboardMap = new KeyMap<KeyboardMap>
        {
            { KeyMap<KeyboardMap>.Key(Keys.Return, KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Enter + "()" } },
            { KeyMap<KeyboardMap>.Key(KeyboardUtil.NumPadReturn, KeyboardModifier.None | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Enter + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Return, KeyboardModifier.Shift | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Newline + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Back, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Erase + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Delete, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Delete + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Insert, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Toggle + "(" + B3270.Setting.InsertMode + ")" } },
            { KeyMap<KeyboardMap>.Key(Keys.Home, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Home + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.End, KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.FieldEnd + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Up, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Up + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Up, KeyboardModifier.Shift), new KeyboardMap { Actions = Constants.Action.KeyboardSelect + "(up)" } },
            { KeyMap<KeyboardMap>.Key(Keys.Down, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Down + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Down, KeyboardModifier.Shift), new KeyboardMap { Actions = Constants.Action.KeyboardSelect + "(down)" } },
            { KeyMap<KeyboardMap>.Key(Keys.Left, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Left + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Left, KeyboardModifier.Shift), new KeyboardMap { Actions = Constants.Action.KeyboardSelect + "(left)" } },
            { KeyMap<KeyboardMap>.Key(Keys.Left, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.PreviousWord + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Right, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Right + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Right, KeyboardModifier.Shift), new KeyboardMap { Actions = Constants.Action.KeyboardSelect + "(right)" } },
            { KeyMap<KeyboardMap>.Key(Keys.Right, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.NextWord + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Tab, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Tab + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Tab, KeyboardModifier.Shift | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.BackTab + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.PageUp, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Scroll + "(backward)" } },
            { KeyMap<KeyboardMap>.Key(Keys.PageDown, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.Scroll + "(forward)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F1, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(1)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F2, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(2)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F3, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(3)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F4, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(4)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F5, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(5)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F6, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(6)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F7, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(7)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F8, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(8)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F9, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(9)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F10, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(10)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F11, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(11)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F12, KeyboardModifier.None), new KeyboardMap { Actions = B3270.Action.PF + "(12)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F1, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(13)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F2, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(14)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F3, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(15)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F4, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(16)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F5, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(17)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F6, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(18)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F7, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(19)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F8, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(20)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F9, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(21)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F10, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(22)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F11, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(23)" } },
            { KeyMap<KeyboardMap>.Key(Keys.F12, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.PF + "(24)" } },
            { KeyMap<KeyboardMap>.Key(Keys.D1, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(1)" } },
            { KeyMap<KeyboardMap>.Key(Keys.D2, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(2)" } },
            { KeyMap<KeyboardMap>.Key(Keys.D3, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(3)" } },
            { KeyMap<KeyboardMap>.Key(Keys.D4, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(4)" } },
            { KeyMap<KeyboardMap>.Key(Keys.A, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Attn + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.C, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.Copy + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.C, KeyboardModifier.Ctrl | KeyboardModifier.Shift | KeyboardModifier.ModeNvt), new KeyboardMap { Actions = B3270.Action.Key + "(U+0003)" } },
            { KeyMap<KeyboardMap>.Key(Keys.C, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.Clear + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.D, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Dup + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.E, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.EraseEOF + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.F, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.Toggle + "(" + B3270.Setting.RightToLeftMode + ")" } },
            { KeyMap<KeyboardMap>.Key(Keys.M, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.FieldMark + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.P, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PrintText + "(gdi,dialog)" } },
            { KeyMap<KeyboardMap>.Key(Keys.Q, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.QuitIfNotConnected + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.R, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Reset + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.U, KeyboardModifier.Ctrl | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.DeleteField + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.V, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.Paste + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.V, KeyboardModifier.Ctrl | KeyboardModifier.Shift | KeyboardModifier.ModeNvt), new KeyboardMap { Actions = B3270.Action.Key + "(U+0016)" } },
            { KeyMap<KeyboardMap>.Key(Keys.V, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Toggle + "(" + B3270.Setting.ReverseInputMode + ")" } },
            { KeyMap<KeyboardMap>.Key(Keys.X, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.Cut + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.X, KeyboardModifier.Ctrl | KeyboardModifier.Shift | KeyboardModifier.ModeNvt), new KeyboardMap { Actions = B3270.Action.Key + "(U+0018)" } },
            { KeyMap<KeyboardMap>.Key(Keys.RControlKey, KeyboardModifier.None | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Enter + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Escape, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.Toggle + "(" + B3270.Setting.AplMode + ")" } },
            { KeyMap<KeyboardMap>.Key("Scan29", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_diamond)" } }, // `~
            { KeyMap<KeyboardMap>.Key("Scan29", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment } }, // `~
            { KeyMap<KeyboardMap>.Key("Scan02", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_diaeresis)" } }, // 1!
            { KeyMap<KeyboardMap>.Key("Scan02", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downtackup)" } }, // 1!
            { KeyMap<KeyboardMap>.Key("Scan03", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_overbar)" } }, // 2@
            { KeyMap<KeyboardMap>.Key("Scan03", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_deltilde)" } }, // 2@
            { KeyMap<KeyboardMap>.Key("Scan04", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(<)" } }, // 3#
            { KeyMap<KeyboardMap>.Key("Scan04", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_delstile)" } }, // 3#
            { KeyMap<KeyboardMap>.Key("Scan05", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_notgreater)" } }, // 4$
            { KeyMap<KeyboardMap>.Key("Scan05", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_deltastile)" } }, // 4$
            { KeyMap<KeyboardMap>.Key("Scan06", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(=)" } }, // 5%
            { KeyMap<KeyboardMap>.Key("Scan06", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circlestile)" } }, // 5%
            { KeyMap<KeyboardMap>.Key("Scan07", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_notless)" } }, // 6^
            { KeyMap<KeyboardMap>.Key("Scan07", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circleslope)" } }, // 6^
            { KeyMap<KeyboardMap>.Key("Scan08", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(>)" } }, // 7&
            { KeyMap<KeyboardMap>.Key("Scan08", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circlebar)" } }, // 7&
            { KeyMap<KeyboardMap>.Key("Scan09", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_notequal)" } }, // 8*
            { KeyMap<KeyboardMap>.Key("Scan09", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circlestar)" } }, // 8*
            { KeyMap<KeyboardMap>.Key("Scan0A", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downcaret)" } }, // 9(
            { KeyMap<KeyboardMap>.Key("Scan0A", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downcarettilde)" } }, // 9(
            { KeyMap<KeyboardMap>.Key("Scan0B", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upcaret)" } }, // 0)
            { KeyMap<KeyboardMap>.Key("Scan0B", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upcarettilde)" } }, // 0)
            { KeyMap<KeyboardMap>.Key("Scan0C", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_multiply)" } }, // -_
            { KeyMap<KeyboardMap>.Key("Scan0C", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quotedot)" } }, // -_
            { KeyMap<KeyboardMap>.Key("Scan0D", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_divide)" } }, // =+
            { KeyMap<KeyboardMap>.Key("Scan0D", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quaddivide)" } }, // =+
            { KeyMap<KeyboardMap>.Key("Scan10", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(?)" } }, // Q
            { KeyMap<KeyboardMap>.Key("Scan10", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Qunderbar)" } }, // Q
            { KeyMap<KeyboardMap>.Key("Scan11", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_omega)" } }, // W
            { KeyMap<KeyboardMap>.Key("Scan11", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Wunderbar)" } }, // W
            { KeyMap<KeyboardMap>.Key("Scan12", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_epsilon)" } }, // E
            { KeyMap<KeyboardMap>.Key("Scan12", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Eunderbar)" } }, // E
            { KeyMap<KeyboardMap>.Key("Scan13", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_rho)" } }, // R
            { KeyMap<KeyboardMap>.Key("Scan13", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Runderbar)" } }, // R
            { KeyMap<KeyboardMap>.Key("Scan14", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(~)" } }, // T
            { KeyMap<KeyboardMap>.Key("Scan14", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Tunderbar)" } }, // T
            { KeyMap<KeyboardMap>.Key("Scan15", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uparrow)" } }, // Y
            { KeyMap<KeyboardMap>.Key("Scan15", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Yunderbar)" } }, // Y
            { KeyMap<KeyboardMap>.Key("Scan16", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downarrow)" } }, // U
            { KeyMap<KeyboardMap>.Key("Scan16", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Uunderbar)" } }, // U
            { KeyMap<KeyboardMap>.Key("Scan17", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_iota)" } }, // I
            { KeyMap<KeyboardMap>.Key("Scan17", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Iunderbar)" } }, // I
            { KeyMap<KeyboardMap>.Key("Scan18", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circle)" } }, // O
            { KeyMap<KeyboardMap>.Key("Scan18", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Ounderbar)" } }, // O
            { KeyMap<KeyboardMap>.Key("Scan19", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(*)" } }, // P
            { KeyMap<KeyboardMap>.Key("Scan19", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Punderbar)" } }, // P
            { KeyMap<KeyboardMap>.Key("Scan1A", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_leftarrow)" } }, // [{
            { KeyMap<KeyboardMap>.Key("Scan1A", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quadquote)" } }, // [{
            { KeyMap<KeyboardMap>.Key("Scan1B", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_rightarrow)" } }, // ]}
            { KeyMap<KeyboardMap>.Key("Scan1B", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment } }, // ]}
            { KeyMap<KeyboardMap>.Key("Scan2B", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_lefttack)" } }, // \|
            { KeyMap<KeyboardMap>.Key("Scan2B", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_righttack)" } }, // \|
            { KeyMap<KeyboardMap>.Key("Scan1E", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_alpha)" } }, // A
            { KeyMap<KeyboardMap>.Key("Scan1E", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Aunderbar)" } }, // A
            { KeyMap<KeyboardMap>.Key("Scan1F", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upstile)" } }, // S
            { KeyMap<KeyboardMap>.Key("Scan1F", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Sunderbar)" } }, // S
            { KeyMap<KeyboardMap>.Key("Scan20", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downstile)" } }, // D
            { KeyMap<KeyboardMap>.Key("Scan20", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Dunderbar)" } }, // D
            { KeyMap<KeyboardMap>.Key("Scan21", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(_)" } }, // F
            { KeyMap<KeyboardMap>.Key("Scan21", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Funderbar)" } }, // F
            { KeyMap<KeyboardMap>.Key("Scan22", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_del)" } }, // G
            { KeyMap<KeyboardMap>.Key("Scan22", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Gunderbar)" } }, // G
            { KeyMap<KeyboardMap>.Key("Scan23", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_delta)" } }, // H
            { KeyMap<KeyboardMap>.Key("Scan23", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Hunderbar)" } }, // H
            { KeyMap<KeyboardMap>.Key("Scan24", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_jot)" } }, // J
            { KeyMap<KeyboardMap>.Key("Scan24", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Junderbar)" } }, // J
            { KeyMap<KeyboardMap>.Key("Scan25", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(')" } }, // K
            { KeyMap<KeyboardMap>.Key("Scan25", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Kunderbar)" } }, // K
            { KeyMap<KeyboardMap>.Key("Scan26", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quad)" } }, // L
            { KeyMap<KeyboardMap>.Key("Scan26", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Lunderbar)" } }, // L
            { KeyMap<KeyboardMap>.Key("Scan27", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downtackjot)" } }, // ;:
            { KeyMap<KeyboardMap>.Key("Scan27", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_equiv)" } }, // ;:
            { KeyMap<KeyboardMap>.Key("Scan28", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uptackjot)" } }, // '"
            { KeyMap<KeyboardMap>.Key("Scan28", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment } }, // '"
            { KeyMap<KeyboardMap>.Key("Scan2C", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_leftshoe)" } }, // Z
            { KeyMap<KeyboardMap>.Key("Scan2C", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Zunderbar)" } }, // Z
            { KeyMap<KeyboardMap>.Key("Scan2D", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_rightshoe)" } }, // X
            { KeyMap<KeyboardMap>.Key("Scan2D", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Xunderbar)" } }, // X
            { KeyMap<KeyboardMap>.Key("Scan2E", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upshoe)" } }, // C
            { KeyMap<KeyboardMap>.Key("Scan2E", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Cunderbar)" } }, // C
            { KeyMap<KeyboardMap>.Key("Scan2F", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downshoe)" } }, // V
            { KeyMap<KeyboardMap>.Key("Scan2F", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Vunderbar)" } }, // V
            { KeyMap<KeyboardMap>.Key("Scan30", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downtack)" } }, // B
            { KeyMap<KeyboardMap>.Key("Scan30", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Bunderbar)" } }, // B
            { KeyMap<KeyboardMap>.Key("Scan31", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uptack)" } }, // N
            { KeyMap<KeyboardMap>.Key("Scan31", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Nunderbar)" } }, // N
            { KeyMap<KeyboardMap>.Key("Scan32", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_stile)" } }, // M
            { KeyMap<KeyboardMap>.Key("Scan32", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Dunderbar)" } }, // M
            { KeyMap<KeyboardMap>.Key("Scan33", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uptackjot)" } }, // ,<
            { KeyMap<KeyboardMap>.Key("Scan33", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_commabar)" } }, // ,<
            { KeyMap<KeyboardMap>.Key("Scan34", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_slopebar)" } }, // .>
            { KeyMap<KeyboardMap>.Key("Scan34", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_deltaunderbar)" } }, // .>
            { KeyMap<KeyboardMap>.Key("Scan35", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_slashbar)" } }, // /?
            { KeyMap<KeyboardMap>.Key("Scan35", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment } }, // /?
        };
    }
}
