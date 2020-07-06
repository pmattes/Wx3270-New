// <copyright file="NativeMethods.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Wrapper class for some native Win32 methods that do not have .NET equivalents.
    /// </summary>
    public class NativeMethods
    {
        /// <summary>
        /// Standard input handle for <see cref="GetStdHandle"/>.
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;

        /// <summary>
        /// Standard output handle for <see cref="GetStdHandle"/>.
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;

        /// <summary>
        /// Standard error handle for <see cref="GetStdHandle"/>.
        /// </summary>
        public const int STD_ERROR_HANDLE = -12;

        /// <summary>
        /// Map a virtual key to a virtual scan code.
        /// </summary>
        public const uint MAPVK_VK_TO_VSC = 0;

        /// <summary>
        /// Map a virtual key to a virtual scan code, distinguishing left from right.
        /// </summary>
        public const uint MAPVK_VK_TO_VSC_EX = 4;

        /// <summary>
        /// Map a virtual scan code to a virtual key code.
        /// </summary>
        public const uint MAPVK_VSC_TO_VK = 1;

        /// <summary>
        /// Map a virtual scan code to a virtual key code, distinguishing left from right.
        /// </summary>
        public const int MAPVK_VSC_TO_VK_EX = 3;

        /// <summary>
        /// Vertical scroll message.
        /// </summary>
        public const int WM_VSCROLL = 0x115;

        /// <summary>
        /// Scroll one line up.
        /// </summary>
        public const int SB_LINEUP = 0;

        /// <summary>
        /// Scroll one line down.
        /// </summary>
        public const int SB_LINEDOWN = 1;

        /// <summary>
        /// Key down message.
        /// </summary>
        public const int WM_KEYDOWN = 0x0100;

        /// <summary>
        /// Key up message.
        /// </summary>
        public const int WM_KEYUP = 0x0101;

        /// <summary>
        /// The Win32 GetStdHandle call.
        /// </summary>
        /// <param name="nStdHandle">Handle, as defined above.</param>
        /// <returns>Integer pointer.</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle")]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// The Win32 AllocConsole call.
        /// </summary>
        /// <returns>True if console was allocated successfully.</returns>
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        /// <summary>
        /// The Win32 FreeConsole call.
        /// </summary>
        /// <returns>True if console was allocated successfully.</returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeConsole")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();

        /// <summary>
        /// The Win32 ToUnicodeEx call.
        /// </summary>
        /// <param name="wVirtKey">Virtual key to map.</param>
        /// <param name="wScanCode">Scan code to map.</param>
        /// <param name="lpKeyState">States of other keys (control keys).</param>
        /// <param name="pwszBuff">Buffer to return result.</param>
        /// <param name="cchBuff">Size of return buffer.</param>
        /// <param name="wFlags">Conversion flags.</param>
        /// <param name="dwhkl">Input language handle.</param>
        /// <returns>Length of result, or -1.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        /// <summary>
        /// The Win32 MapVirtualKeyEx call.
        /// </summary>
        /// <param name="uCode">Code to translate.</param>
        /// <param name="uMapType">Mapping type.</param>
        /// <param name="dwhkl">Input locale identifier.</param>
        /// <returns>Translated key.</returns>
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        /// <summary>
        /// Gets the keyboard state.
        /// </summary>
        /// <param name="lpKeyState">Returned keyboard state.</param>
        /// <returns>True for success.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        /// <summary>
        /// Gets the keyboard state for one key.
        /// </summary>
        /// <param name="nVirtKey">Key code.</param>
        /// <returns>Key state.</returns>
        [DllImport("user32.dll")]
        public static extern short GetKeyState(uint nVirtKey);

        /// <summary>
        /// Send a window message.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <param name="Msg">Message type.</param>
        /// <param name="wParam">Parameter 1.</param>
        /// <param name="lParam">Parameter 2.</param>
        /// <returns>Result of message send.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Gets the system ANSI code page.
        /// </summary>
        /// <returns>ANSI code page.</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetACP();
    }
}
