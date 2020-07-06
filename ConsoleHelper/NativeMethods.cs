// <copyright file="NativeMethods.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270Prompt
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Native methods for the wx3270 prompt.
    /// </summary>
    public class NativeMethods
    {
        /// <summary>
        /// Get a standard handle.
        /// </summary>
        /// <param name="stdHandle">Standard handle number</param>
        /// <returns>Standard handle</returns>
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetStdHandle(
            [param: MarshalAs(UnmanagedType.U4)]
            int stdHandle);

        /// <summary>
        /// Wrapper for <code>ReadConsole</code>.
        /// </summary>
        /// <param name="handle">Input handle</param>
        /// <param name="buffer">Returned buffer</param>
        /// <param name="numberOfCharsToRead">Maximum number of characters to read</param>
        /// <param name="numberOfCharsRead">Returned number of characters read</param>
        /// <param name="reserved">Always 0</param>
        /// <returns>True if it succeeded</returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadConsole(
            IntPtr handle,
            [param: MarshalAs(UnmanagedType.LPTStr), Out] StringBuilder buffer,
            [param: MarshalAs(UnmanagedType.U4)] uint numberOfCharsToRead,
            [param: MarshalAs(UnmanagedType.U4), Out] out uint numberOfCharsRead,
            [param: MarshalAs(UnmanagedType.U4)]
            uint reserved);
    }
}
