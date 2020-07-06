// <copyright file="KeyHelper.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Helper class for various key-related operations.
    /// </summary>
    public static class KeyHelper
    {
        /// <summary>
        /// Mapping between <see cref="Keys"/> and <see cref="KeyboardModifier"/>.
        /// </summary>
        private static readonly Dictionary<Keys, KeyboardModifier> KeysDict = new Dictionary<Keys, KeyboardModifier>
        {
            { Keys.Shift, KeyboardModifier.Shift },
            { Keys.Control, KeyboardModifier.Ctrl },
            { Keys.Alt, KeyboardModifier.Alt },
        };

        /// <summary>
        /// Map a <see cref="KeyboardModifier"/> to a set of <see cref="Keys"/> modifiers.
        /// </summary>
        /// <param name="modifier">Modifier bitmap.</param>
        /// <returns>Keys modifiers.</returns>
        public static Keys ModifierToKeys(KeyboardModifier modifier)
        {
            return KeysDict.Aggregate(Keys.None, (total, next) => total | (modifier.HasFlag(next.Value) ? next.Key : Keys.None));
        }

        /// <summary>
        /// Map a set of <see cref="Keys"/> modifiers to a <see cref="KeyboardModifier"/>.
        /// </summary>
        /// <param name="keys">Keys modifiers.</param>
        /// <returns>Modifier bitmap.</returns>
        public static KeyboardModifier KeysToModifier(Keys keys)
        {
            return KeysDict.Aggregate(KeyboardModifier.None, (total, next) => total | (keys.HasFlag(next.Key) ? next.Value : KeyboardModifier.None));
        }

        /// <summary>
        /// Convert a scan code to its lookup name.
        /// </summary>
        /// <param name="scanCode">Scan code.</param>
        /// <returns>Scan lookup name.</returns>
        public static string ScanName(uint scanCode)
        {
            return "Scan" + scanCode.ToString("X2");
        }
    }
}
