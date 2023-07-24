// <copyright file="DefaultKeyboardMap.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Configurable options, serialized in a file.
    /// </summary>
    public partial class Profile
    {
        /// <summary>
        /// Set of keymaps to add for each new version.
        /// </summary>
        private static readonly Dictionary<VersionClass, KeyMap<KeyboardMap>> AddedKeyboardMaps = new Dictionary<VersionClass, KeyMap<KeyboardMap>>
        {
            {
                // 1.2 adds full screen, font stepping and the temporary menu bar.
                new VersionClass() { Major = 1, Minor = 2 },
                new KeyMap<KeyboardMap>
                {
                    { KeyMap<KeyboardMap>.Key(Keys.F11, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.FullScreen + "()" } },
                    { KeyMap<KeyboardMap>.Key(Keys.Oemplus, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.StepEfont + "(" + Constants.Misc.Bigger + ")" } },
                    { KeyMap<KeyboardMap>.Key(Keys.OemMinus, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.StepEfont + "(" + Constants.Misc.Smaller + ")" } },
                    { KeyMap<KeyboardMap>.Key(Keys.N, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.MenuBar + "()", Exact = true } },
                }
            },
        };

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
            { KeyMap<KeyboardMap>.Key(Keys.F4, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.Quit + "(-force)" } },
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
            { KeyMap<KeyboardMap>.Key(Keys.F11, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.FullScreen + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.D1, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(1)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.D2, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(2)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.D3, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.PA + "(3)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.A, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Attn + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.C, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.Copy + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.C, KeyboardModifier.Ctrl | KeyboardModifier.Shift | KeyboardModifier.ModeNvt), new KeyboardMap { Actions = B3270.Action.Key + "(U+0003)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.C, KeyboardModifier.Alt), new KeyboardMap { Actions = B3270.Action.Clear + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.D, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Dup + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.E, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.EraseEOF + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.M, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.FieldMark + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.N, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.MenuBar + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.P, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.PrintText + "(gdi,dialog)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.Q, KeyboardModifier.Alt), new KeyboardMap { Actions = Constants.Action.QuitIfNotConnected + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.R, KeyboardModifier.Alt | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Reset + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.U, KeyboardModifier.Ctrl | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.DeleteField + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.V, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.Paste + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.V, KeyboardModifier.Ctrl | KeyboardModifier.Shift | KeyboardModifier.ModeNvt), new KeyboardMap { Actions = B3270.Action.Key + "(U+0016)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.X, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.Cut + "()", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.X, KeyboardModifier.Ctrl | KeyboardModifier.Shift | KeyboardModifier.ModeNvt), new KeyboardMap { Actions = B3270.Action.Key + "(U+0018)", Exact = true } },
            { KeyMap<KeyboardMap>.Key(Keys.RControlKey, KeyboardModifier.None | KeyboardModifier.Mode3270), new KeyboardMap { Actions = B3270.Action.Enter + "()" } },
            { KeyMap<KeyboardMap>.Key(Keys.Escape, KeyboardModifier.Shift), new KeyboardMap { Actions = B3270.Action.Toggle + "(" + B3270.Setting.AplMode + ")" } },
            { KeyMap<KeyboardMap>.Key(Keys.Oemplus, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.StepEfont + "(" + Constants.Misc.Bigger + ")" } },
            { KeyMap<KeyboardMap>.Key(Keys.OemMinus, KeyboardModifier.Ctrl), new KeyboardMap { Actions = Constants.Action.StepEfont + "(" + Constants.Misc.Smaller + ")" } },
            { KeyMap<KeyboardMap>.Key("Scan29", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_diamond)", Exact = true } }, // `~
            { KeyMap<KeyboardMap>.Key("Scan29", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment, Exact = true } }, // `~
            { KeyMap<KeyboardMap>.Key("Scan02", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_diaeresis)", Exact = true } }, // 1!
            { KeyMap<KeyboardMap>.Key("Scan02", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downtackup)", Exact = true } }, // 1!
            { KeyMap<KeyboardMap>.Key("Scan03", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_overbar)", Exact = true } }, // 2@
            { KeyMap<KeyboardMap>.Key("Scan03", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_deltilde)", Exact = true } }, // 2@
            { KeyMap<KeyboardMap>.Key("Scan04", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(<)", Exact = true } }, // 3#
            { KeyMap<KeyboardMap>.Key("Scan04", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_delstile)", Exact = true } }, // 3#
            { KeyMap<KeyboardMap>.Key("Scan05", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_notgreater)", Exact = true } }, // 4$
            { KeyMap<KeyboardMap>.Key("Scan05", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_deltastile)", Exact = true } }, // 4$
            { KeyMap<KeyboardMap>.Key("Scan06", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(=)", Exact = true } }, // 5%
            { KeyMap<KeyboardMap>.Key("Scan06", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circlestile)", Exact = true } }, // 5%
            { KeyMap<KeyboardMap>.Key("Scan07", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_notless)", Exact = true } }, // 6^
            { KeyMap<KeyboardMap>.Key("Scan07", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circleslope)", Exact = true } }, // 6^
            { KeyMap<KeyboardMap>.Key("Scan08", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(>)", Exact = true } }, // 7&
            { KeyMap<KeyboardMap>.Key("Scan08", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circlebar)", Exact = true } }, // 7&
            { KeyMap<KeyboardMap>.Key("Scan09", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_notequal)", Exact = true } }, // 8*
            { KeyMap<KeyboardMap>.Key("Scan09", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circlestar)", Exact = true } }, // 8*
            { KeyMap<KeyboardMap>.Key("Scan0A", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downcaret)", Exact = true } }, // 9(
            { KeyMap<KeyboardMap>.Key("Scan0A", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downcarettilde)", Exact = true } }, // 9(
            { KeyMap<KeyboardMap>.Key("Scan0B", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upcaret)", Exact = true } }, // 0)
            { KeyMap<KeyboardMap>.Key("Scan0B", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upcarettilde)", Exact = true } }, // 0)
            { KeyMap<KeyboardMap>.Key("Scan0C", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_multiply)", Exact = true } }, // -_
            { KeyMap<KeyboardMap>.Key("Scan0C", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quotedot)", Exact = true } }, // -_
            { KeyMap<KeyboardMap>.Key("Scan0D", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_divide)", Exact = true } }, // =+
            { KeyMap<KeyboardMap>.Key("Scan0D", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quaddivide)", Exact = true } }, // =+
            { KeyMap<KeyboardMap>.Key("Scan10", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(?)", Exact = true } }, // Q
            { KeyMap<KeyboardMap>.Key("Scan10", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Qunderbar)", Exact = true } }, // Q
            { KeyMap<KeyboardMap>.Key("Scan11", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_omega)", Exact = true } }, // W
            { KeyMap<KeyboardMap>.Key("Scan11", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Wunderbar)", Exact = true } }, // W
            { KeyMap<KeyboardMap>.Key("Scan12", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_epsilon)", Exact = true } }, // E
            { KeyMap<KeyboardMap>.Key("Scan12", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Eunderbar)", Exact = true } }, // E
            { KeyMap<KeyboardMap>.Key("Scan13", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_rho)", Exact = true } }, // R
            { KeyMap<KeyboardMap>.Key("Scan13", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Runderbar)", Exact = true } }, // R
            { KeyMap<KeyboardMap>.Key("Scan14", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(~)", Exact = true } }, // T
            { KeyMap<KeyboardMap>.Key("Scan14", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Tunderbar)", Exact = true } }, // T
            { KeyMap<KeyboardMap>.Key("Scan15", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uparrow)", Exact = true } }, // Y
            { KeyMap<KeyboardMap>.Key("Scan15", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Yunderbar)", Exact = true } }, // Y
            { KeyMap<KeyboardMap>.Key("Scan16", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downarrow)", Exact = true } }, // U
            { KeyMap<KeyboardMap>.Key("Scan16", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Uunderbar)", Exact = true } }, // U
            { KeyMap<KeyboardMap>.Key("Scan17", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_iota)", Exact = true } }, // I
            { KeyMap<KeyboardMap>.Key("Scan17", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Iunderbar)", Exact = true } }, // I
            { KeyMap<KeyboardMap>.Key("Scan18", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_circle)", Exact = true } }, // O
            { KeyMap<KeyboardMap>.Key("Scan18", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Ounderbar)", Exact = true } }, // O
            { KeyMap<KeyboardMap>.Key("Scan19", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(*)", Exact = true } }, // P
            { KeyMap<KeyboardMap>.Key("Scan19", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Punderbar)", Exact = true } }, // P
            { KeyMap<KeyboardMap>.Key("Scan1A", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_leftarrow)", Exact = true } }, // [{
            { KeyMap<KeyboardMap>.Key("Scan1A", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quadquote)", Exact = true } }, // [{
            { KeyMap<KeyboardMap>.Key("Scan1B", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_rightarrow)", Exact = true } }, // ]}
            { KeyMap<KeyboardMap>.Key("Scan1B", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment, Exact = true } }, // ]}
            { KeyMap<KeyboardMap>.Key("Scan2B", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_lefttack)", Exact = true } }, // \|
            { KeyMap<KeyboardMap>.Key("Scan2B", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_righttack)", Exact = true } }, // \|
            { KeyMap<KeyboardMap>.Key("Scan1E", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_alpha)", Exact = true } }, // A
            { KeyMap<KeyboardMap>.Key("Scan1E", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Aunderbar)", Exact = true } }, // A
            { KeyMap<KeyboardMap>.Key("Scan1F", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upstile)", Exact = true } }, // S
            { KeyMap<KeyboardMap>.Key("Scan1F", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Sunderbar)", Exact = true } }, // S
            { KeyMap<KeyboardMap>.Key("Scan20", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downstile)", Exact = true } }, // D
            { KeyMap<KeyboardMap>.Key("Scan20", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Dunderbar)", Exact = true } }, // D
            { KeyMap<KeyboardMap>.Key("Scan21", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(_)", Exact = true } }, // F
            { KeyMap<KeyboardMap>.Key("Scan21", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Funderbar)", Exact = true } }, // F
            { KeyMap<KeyboardMap>.Key("Scan22", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_del)", Exact = true } }, // G
            { KeyMap<KeyboardMap>.Key("Scan22", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Gunderbar)", Exact = true } }, // G
            { KeyMap<KeyboardMap>.Key("Scan23", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_delta)", Exact = true } }, // H
            { KeyMap<KeyboardMap>.Key("Scan23", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Hunderbar)", Exact = true } }, // H
            { KeyMap<KeyboardMap>.Key("Scan24", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_jot)", Exact = true } }, // J
            { KeyMap<KeyboardMap>.Key("Scan24", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Junderbar)", Exact = true } }, // J
            { KeyMap<KeyboardMap>.Key("Scan25", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(')", Exact = true } }, // K
            { KeyMap<KeyboardMap>.Key("Scan25", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Kunderbar)" } }, // K
            { KeyMap<KeyboardMap>.Key("Scan26", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_quad)", Exact = true } }, // L
            { KeyMap<KeyboardMap>.Key("Scan26", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Lunderbar)", Exact = true } }, // L
            { KeyMap<KeyboardMap>.Key("Scan27", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downtackjot)", Exact = true } }, // ;:
            { KeyMap<KeyboardMap>.Key("Scan27", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_equiv)", Exact = true } }, // ;:
            { KeyMap<KeyboardMap>.Key("Scan28", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uptackjot)", Exact = true } }, // '"
            { KeyMap<KeyboardMap>.Key("Scan28", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment, Exact = true } }, // '"
            { KeyMap<KeyboardMap>.Key("Scan2C", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_leftshoe)", Exact = true } }, // Z
            { KeyMap<KeyboardMap>.Key("Scan2C", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Zunderbar)", Exact = true } }, // Z
            { KeyMap<KeyboardMap>.Key("Scan2D", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_rightshoe)", Exact = true } }, // X
            { KeyMap<KeyboardMap>.Key("Scan2D", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Xunderbar)", Exact = true } }, // X
            { KeyMap<KeyboardMap>.Key("Scan2E", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_upshoe)", Exact = true } }, // C
            { KeyMap<KeyboardMap>.Key("Scan2E", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Cunderbar)", Exact = true } }, // C
            { KeyMap<KeyboardMap>.Key("Scan2F", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downshoe)", Exact = true } }, // V
            { KeyMap<KeyboardMap>.Key("Scan2F", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Vunderbar)", Exact = true } }, // V
            { KeyMap<KeyboardMap>.Key("Scan30", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_downtack)", Exact = true } }, // B
            { KeyMap<KeyboardMap>.Key("Scan30", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Bunderbar)", Exact = true } }, // B
            { KeyMap<KeyboardMap>.Key("Scan31", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uptack)", Exact = true } }, // N
            { KeyMap<KeyboardMap>.Key("Scan31", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Nunderbar)", Exact = true } }, // N
            { KeyMap<KeyboardMap>.Key("Scan32", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_stile)", Exact = true } }, // M
            { KeyMap<KeyboardMap>.Key("Scan32", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_Dunderbar)", Exact = true } }, // M
            { KeyMap<KeyboardMap>.Key("Scan33", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_uptackjot)", Exact = true } }, // ,<
            { KeyMap<KeyboardMap>.Key("Scan33", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_commabar)", Exact = true } }, // ,<
            { KeyMap<KeyboardMap>.Key("Scan34", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_slopebar)", Exact = true } }, // .>
            { KeyMap<KeyboardMap>.Key("Scan34", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_deltaunderbar)", Exact = true } }, // .>
            { KeyMap<KeyboardMap>.Key("Scan35", KeyboardModifier.Alt | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Key + "(apl_slashbar)", Exact = true } }, // /?
            { KeyMap<KeyboardMap>.Key("Scan35", KeyboardModifier.Alt | KeyboardModifier.Shift | KeyboardModifier.Apl), new KeyboardMap { Actions = B3270.Action.Comment, Exact = true } }, // /?
        };

        /// <summary>
        /// Gets the per-version added keymaps.
        /// </summary>
        public static Dictionary<VersionClass, KeyMap<KeyboardMap>> PerVersionAddedKeyboardMaps => AddedKeyboardMaps;
    }
}
