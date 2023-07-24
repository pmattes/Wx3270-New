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
        /// Source copy BitBlt operation.
        /// </summary>
        public const int SRCCOPY = 0xCC0020;

        /// <summary>
        /// Flags for the uType parameter of <see cref="SHMessageBoxCheck"/>.
        /// </summary>
        public enum MessageBoxCheckFlags : uint
        {
            /// <summary>
            /// Display the OK button.
            /// </summary>
            MB_OK = 0x00000000,

            /// <summary>
            /// Display the OK and Cancel buttons.
            /// </summary>
            MB_OKCANCEL = 0x00000001,

            /// <summary>
            /// Display the YES and NO buttons.
            /// </summary>
            MB_YESNO = 0x00000004,

            /// <summary>
            /// Display the stop icon (raised hand).
            /// </summary>
            MB_ICONHAND = 0x00000010,

            /// <summary>
            /// Display the question icon.
            /// </summary>
            MB_ICONQUESTION = 0x00000020,

            /// <summary>
            /// Display the error icon (exclamation point).
            /// </summary>
            MB_ICONEXCLAMATION = 0x00000030,

            /// <summary>
            /// Display the information icon (lower-case i).
            /// </summary>
            MB_ICONINFORMATION = 0x00000040,
        }

        /// <summary>
        /// Return values from displaying a message box.
        /// </summary>
        public enum MessageBoxReturnValue : int
        {
            /// <summary>
            /// Message box failed.
            /// </summary>
            IDERROR = -1,

            /// <summary>
            /// OK button pressed.
            /// </summary>
            IDOK = 1,

            /// <summary>
            /// Cancel button pressed or window closed.
            /// </summary>
            IDCANCEL = 2,

            /// <summary>
            /// Abort button pressed.
            /// </summary>
            IDABORT = 3,

            /// <summary>
            /// Retry button pressed.
            /// </summary>
            IDRETRY = 4,

            /// <summary>
            /// Ignore button pressed.
            /// </summary>
            IDIGNORE = 5,

            /// <summary>
            /// Yes button pressed.
            /// </summary>
            IDYES = 6,

            /// <summary>
            /// No button pressed.
            /// </summary>
            IDNO = 7,

            /// <summary>
            /// Close button pressed.
            /// </summary>
            IDCLOSE = 8,

            /// <summary>
            /// Help button pressed.
            /// </summary>
            IDHELP = 9,

            /// <summary>
            /// Try again button pressed.
            /// </summary>
            IDTRYAGAIN = 10,

            /// <summary>
            /// Continue button pressed.
            /// </summary>
            IDCONTINUE = 11,
        }

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
        /// Gets the system ANSI code page.
        /// </summary>
        /// <returns>ANSI code page.</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetACP();

        /// <summary>
        /// Gets the keyboard layout name.
        /// </summary>
        /// <param name="pwszLKID">Returned name.</param>
        /// <returns>False for success.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardLayoutName([Out] StringBuilder pwszLKID);

        /// <summary>
        /// Bit block transfer operation.
        /// </summary>
        /// <param name="hdc">Destination device context handle.</param>
        /// <param name="x">X coordinate of destination rectangle.</param>
        /// <param name="y">Y coordinate of destination rectangle.</param>
        /// <param name="cx">Width of source and destination rectangles.</param>
        /// <param name="cy">Height of source and destination rectangles.</param>
        /// <param name="hdcSrc">Handle to source device context.</param>
        /// <param name="x1">X coordinate of source rectangle.</param>
        /// <param name="y1">Y coordinate of source rectangle.</param>
        /// <param name="rop">Raster operation code.</param>
        /// <returns>Nonzero for success.</returns>
        [DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, int rop);

        /// <summary>
        /// Show a message box with a 'do not remind me' checkbox.
        /// </summary>
        /// <param name="hwnd">Owning window.</param>
        /// <param name="pszText">Text to display.</param>
        /// <param name="pszTitle">Title for the message box.</param>
        /// <param name="uType">Message box type.</param>
        /// <param name="iDefault">Default value.</param>
        /// <param name="pszRegVal">Registry key to save 'do not remind me' in.</param>
        /// <returns>Message box result.</returns>
        [DllImport("shlwapi.dll", EntryPoint = "#185", ExactSpelling = true, PreserveSig = true)]
        public static extern MessageBoxReturnValue SHMessageBoxCheckW(
            [In] IntPtr hwnd,
            [In] string pszText,
            [In] string pszTitle,
            [In] MessageBoxCheckFlags uType,
            [In] MessageBoxReturnValue iDefault,
            [In] string pszRegVal);
    }
}
