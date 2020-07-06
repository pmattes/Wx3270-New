// <copyright file="Ime.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

#pragma warning disable SA1307 // Field name must begin with uppercase letter
#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable SA1401 // Field must be private

namespace Wx3270
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class for setting the IME window location.
    /// </summary>
    public class Ime
    {
        /// <summary>
        /// IME control message.
        /// </summary>
        private const int WM_IME_CONTROL = 0x0283;

        /// <summary>
        /// Set composition window sub-message.
        /// </summary>
        private const int IMC_SETCOMPOSITIONWINDOW = 0x000c;

        /// <summary>
        /// Parameter type for the set composition window sub-message.
        /// </summary>
        private const int CFS_POINT = 0x0002;

        /// <summary>
        /// The IME window handle.
        /// </summary>
        private readonly IntPtr hIMEWnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ime"/> class.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        public Ime(IntPtr hWnd)
        {
            this.hIMEWnd = ImmGetDefaultIMEWnd(hWnd);
        }

        /// <summary>
        /// Sets the composition window location.
        /// </summary>
        /// <param name="x">X coordinate, relative to this window.</param>
        /// <param name="y">Y coordinate, relative to this window.</param>
        public void SetIMEWindowLocation(int x, int y)
        {
            COMPOSITIONFORM lParam = new COMPOSITIONFORM { dwStyle = CFS_POINT, ptCurrentPos = new POINT { x = x, y = y }, rcArea = new RECT() };
            SendMessage(this.hIMEWnd, WM_IME_CONTROL, IMC_SETCOMPOSITIONWINDOW, lParam);
        }

        /// <summary>
        /// Gets the default IME window handle.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <returns>IME window handle.</returns>
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <param name="msg">Message type.</param>
        /// <param name="wParam">Sub-message type.</param>
        /// <param name="lParam">Composition form.</param>
        /// <returns>Message processing result (depends on message type).</returns>
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, COMPOSITIONFORM lParam);

        /// <summary>
        /// A point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x = 0;
            public int y = 0;
        }

        /// <summary>
        /// A rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class RECT
        {
            public int left = 0;
            public int top = 0;
            public int right = 0;
            public int bottom = 0;
        }

        /// <summary>
        /// A composition form (parameter to the Set Composition Window sub-message).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class COMPOSITIONFORM
        {
            public int dwStyle = 0;
            public POINT ptCurrentPos = null;
            public RECT rcArea = null;
        }
    }
}
