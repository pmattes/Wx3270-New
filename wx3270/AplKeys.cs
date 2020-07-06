// <copyright file="AplKeys.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// APL key definitions.
    /// </summary>
    public static class AplKeys
    {
        /// <summary>
        /// Dictionary of key mappings.
        /// </summary>
        private static Dictionary<AplKey, string> inputMap = new Dictionary<AplKey, string>
        {
            { new AplKey(41, false), "⋄" }, // `~
            { new AplKey(2, false), "¨" }, // 1!
            { new AplKey(2, true), "⌶" }, // 1!
            { new AplKey(3, false), "¯" }, // 2@
            { new AplKey(3, true), "⍫" }, // 2@
            { new AplKey(4, false), "<" }, // 3#
            { new AplKey(4, true), "⍒" }, // 3#
            { new AplKey(5, false), "≤" }, // 4$
            { new AplKey(5, true), "⍋" }, // 4$
            { new AplKey(6, false), "=" }, // 5%
            { new AplKey(6, true), "⌽" }, // 5%
            { new AplKey(7, false), "≥" }, // 6^
            { new AplKey(7, true), "⍉" }, // 6^
            { new AplKey(8, false), ">" }, // 7&
            { new AplKey(8, true), "⊖" }, // 7&
            { new AplKey(9, false), "≠" }, // 8*
            { new AplKey(9, true), "⍟" }, // 8*
            { new AplKey(10, false), "∨" }, // 9(
            { new AplKey(10, true), "⍱" }, // 9(
            { new AplKey(11, false), "∧" }, // 0)
            { new AplKey(11, true), "⍲" }, // 0)
            { new AplKey(12, false), "×" }, // -^
            { new AplKey(12, true), "!" }, // -^
            { new AplKey(13, false), "÷" }, // =+
            { new AplKey(13, true), "⌹" }, // =+
            { new AplKey(16, false), "?" }, // Q
            { new AplKey(17, false), "⍵" }, // W
            { new AplKey(18, false), "∊" }, // E
            { new AplKey(19, false), "⍴" }, // R
            { new AplKey(20, false), "~" }, // T
            { new AplKey(21, false), "↑" }, // Y
            { new AplKey(22, false), "↓" }, // U
            { new AplKey(23, false), "⍳" }, // I
            { new AplKey(24, false), "○" }, // O
            { new AplKey(25, false), "*" }, // P
            { new AplKey(26, false), "←" }, // [{
            { new AplKey(26, true), "⍞" }, // [{
            { new AplKey(27, false), "→" }, // ]}
            { new AplKey(43, false), "⊢" }, // \|
            { new AplKey(43, true), "⊣" }, // \|
            { new AplKey(30, false), "⍺" }, // A
            { new AplKey(31, false), "⌈" }, // S
            { new AplKey(32, false), "⌊" }, // D
            { new AplKey(33, false), "_" }, // F
            { new AplKey(34, false), "∇" }, // G
            { new AplKey(35, false), "∆" }, // H
            { new AplKey(36, false), "∘" }, // J
            { new AplKey(37, false), "'" }, // K
            { new AplKey(38, false), "⎕" }, // L
            { new AplKey(39, false), "⍎" }, // ;:
            { new AplKey(39, true), "≡" }, // ;:
            { new AplKey(40, false), "⍕" }, // '"
            { new AplKey(44, false), "⊂" }, // Z
            { new AplKey(45, false), "⊃" }, // X
            { new AplKey(46, false), "∩" }, // C
            { new AplKey(47, false), "∪" }, // V
            { new AplKey(48, false), "⊥" }, // B
            { new AplKey(49, false), "⊤" }, // N
            { new AplKey(50, false), "|" }, // M
            { new AplKey(51, false), "⍝" }, // ,<
            { new AplKey(51, true), "⍪" }, // ,<
            { new AplKey(52, false), "⍀" }, // .>
            { new AplKey(52, true), "⍙" }, // .>
            { new AplKey(53, false), "⌿" }, // /?
        };

        /// <summary>
        /// Look up an APL character by key code and shift state.
        /// </summary>
        /// <param name="scanCode">Key code.</param>
        /// <param name="shift">True if shifted.</param>
        /// <param name="result">Returned key name.</param>
        /// <returns>True if lookup succeeds.</returns>
        public static bool TryLookupDisplay(int scanCode, bool shift, out string result)
        {
            if (inputMap.TryGetValue(new AplKey(scanCode, shift), out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// A single APL key definition.
        /// </summary>
        private class AplKey : Tuple<int, bool>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AplKey"/> class.
            /// </summary>
            /// <param name="keyCode">Key code.</param>
            /// <param name="shift">True if shifted.</param>
            public AplKey(int keyCode, bool shift)
                : base(keyCode, shift)
            {
            }
        }
    }
}
