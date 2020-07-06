// <copyright file="KeyboardUtil.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The graphical keyboard map display.
    /// </summary>
    public static class KeyboardUtil
    {
        /// <summary>
        /// Fake enumeration value for the numeric keypad return key.
        /// </summary>
        public const Keys NumPadReturn = (Keys)512;

        /// <summary>
        /// Fake scan code for the numeric keypad return key.
        /// </summary>
        public const uint NumPadReturnScanCode = 0xe01c;

        /// <summary>
        /// Translate a virtual key code to the string it encodes, if any.
        /// </summary>
        /// <param name="vkey">Virtual key code.</param>
        /// <param name="modifiers">Current modifiers.</param>
        /// <param name="isDead">Returned true if this is a dead key.</param>
        /// <returns>Key value, or null.</returns>
        public static char? FromVkey(Keys vkey, Keys modifiers, out bool isDead)
        {
            var keyStates = new byte[256];
            if (modifiers.HasFlag(Keys.Shift))
            {
                keyStates[(int)Keys.ShiftKey] = 0x80;
            }
#if false
            if (capsLock)
            {
                keyStates[(int)Keys.CapsLock] = 0x80;   
            }
#endif
            if (modifiers.HasFlag(Keys.Control))
            {
                keyStates[(int)Keys.ControlKey] = 0x80;
            }

            if (modifiers.HasFlag(Keys.Alt))
            {
                keyStates[(int)Keys.Menu] = 0x80;
            }

            if (modifiers.HasFlag(Keys.RMenu))
            {
                keyStates[(int)Keys.ControlKey] = 0x80;
                keyStates[(int)Keys.Menu] = 0x80;
            }

            isDead = false;
            var sb = new StringBuilder(10);
            int ret = NativeMethods.ToUnicodeEx((uint)vkey, 0, keyStates, sb, sb.Capacity, 0, InputLanguage.CurrentInputLanguage.Handle);
            ClearKeyboardState();
            switch (ret)
            {
                case -1:
                    // Dead key.
                    isDead = true;
                    return (sb.Length > 0) ? (char?)sb[0] : null;
                case 0:
                    // Failed.
                    return null;
                case 1:
                    // Success.
                    return (char?)(sb.Length > 0 ? sb[0] : 0);
                default:
                    if (ret < 0)
                    {
                        return null;
                    }

                    // This was found empirically. It might not be reliable.
                    isDead = true;
                    return sb[1];
            }
        }

        /// <summary>
        /// Clear keyboard state (partial dead key state).
        /// </summary>
        public static void ClearKeyboardState()
        {
            var sb = new StringBuilder(10);
            NativeMethods.ToUnicodeEx((uint)Keys.A, 0, new byte[256], sb, sb.Capacity, 0, InputLanguage.CurrentInputLanguage.Handle);
        }

        /// <summary>
        /// Extension method for virtual key to string conversion, including my fake value.
        /// </summary>
        /// <param name="keys">Key to decode.</param>
        /// <returns>Decoded key.</returns>
        public static string ToStringExtended(this Keys keys)
        {
            if (keys == NumPadReturn)
            {
                return "NumPadReturn";
            }

            return keys.ToString();
        }

        /// <summary>
        /// Convert a virtual key to a virtual scan code.
        /// </summary>
        /// <param name="vkey">Virtual key.</param>
        /// <param name="inputLanguage">Input language.</param>
        /// <returns>Virtual scan code.</returns>
        public static uint VkeyToScanCode(Keys vkey, InputLanguage inputLanguage)
        {
            // Map the fake keypad return key to a fake scan code.
            if (vkey == NumPadReturn)
            {
                return NumPadReturnScanCode;
            }

            return NativeMethods.MapVirtualKeyEx((uint)vkey, NativeMethods.MAPVK_VK_TO_VSC_EX, inputLanguage.Handle);
        }

        /// <summary>
        /// Convert a virtual scan code to a virtual key.
        /// </summary>
        /// <param name="scanCode">Scan code.</param>
        /// <param name="inputLanguage">Input language.</param>
        /// <returns>Virtual key.</returns>
        public static Keys ScanCodeToVkey(uint scanCode, InputLanguage inputLanguage)
        {
            // Map the fake keypad return key to a fake scan code.
            if (scanCode == NumPadReturnScanCode)
            {
                return NumPadReturn;
            }

            return (Keys)NativeMethods.MapVirtualKeyEx(scanCode, NativeMethods.MAPVK_VSC_TO_VK_EX, inputLanguage.Handle);
        }

        /// <summary>
        /// Parse an extended virtual key.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <returns>Parsed key value.</returns>
        public static Keys ParseKeysExtended(string text)
        {
            if (text == "NumPadReturn")
            {
                return NumPadReturn;
            }

            return (Keys)Enum.Parse(typeof(Keys), text);
        }
    }
}
