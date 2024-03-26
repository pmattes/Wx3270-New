// <copyright file="FixedWidthFontEnumerator.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Fixed-width font enumerator.
    /// </summary>
    public class FixedWidthFontEnumerator
    {
        /// <summary>
        /// Internal list of font family names.
        /// </summary>
        private readonly List<string> tempNames = new List<string>();

        /// <summary>
        /// Gets the fonts.
        /// </summary>
        public IEnumerable<string> Names
        {
            get
            {
                this.tempNames.Clear();
                Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                IntPtr hdc = graphics.GetHdc();
                var logfont = new NativeMethods.LOGFONT() { lfCharSet = NativeMethods.DEFAULT_CHARSET };
                NativeMethods.EnumFontFamiliesEx(hdc, ref logfont, new NativeMethods.FONTENUMPROC(this.FontFamilyCallback), IntPtr.Zero, 0);
                graphics.ReleaseHdc();

                return this.tempNames.OrderBy(f => f).ToList();
            }
        }

        /// <summary>
        /// Font family callback.
        /// </summary>
        /// <param name="lpelf">Logical font description.</param>
        /// <param name="lpntm">Text metrics (ununsed).</param>
        /// <param name="fontType">Font type.</param>
        /// <param name="lParam">Opaque parameter (unused).</param>
        /// <returns>1 (success).</returns>
        private int FontFamilyCallback(ref NativeMethods.ENUMLOGFONTEX lpelf, ref NativeMethods.NEWTEXTMETRIC lpntm, uint fontType, IntPtr lParam)
        {
            if ((lpelf.elfLogFont.lfPitchAndFamily & 0x3) == NativeMethods.FIXED_PITCH
                && (fontType & NativeMethods.TRUETYPE_FONTTYPE) != 0
                && lpelf.elfScript == "Western"
                && !lpelf.elfFullName.StartsWith("@"))
            {
                this.tempNames.Add(lpelf.elfLogFont.lfFaceName);
            }

            return 1;
        }

        /// <summary>
        /// Native methods used by the <see cref="FixedWidthFontEnumerator"/> class.
        /// </summary>
        internal class NativeMethods
        {
#pragma warning disable SA1600 // Elements should be documented
            /// <summary>
            /// String length for font family.
            /// </summary>
            public const int LF_FACESIZE = 32;

            /// <summary>
            /// String length for full font name.
            /// </summary>
            public const int LF_FULLFACESIZE = 64;

            /// <summary>
            /// Default character set (Western).
            /// </summary>
            public const int DEFAULT_CHARSET = 1;

            /// <summary>
            /// Fixed-pitch font.
            /// </summary>
            public const int FIXED_PITCH = 1;

            /// <summary>
            /// TrueType font type.
            /// </summary>
            public const int TRUETYPE_FONTTYPE = 0x0004;

            /// <summary>
            /// Delegate called by <see cref="EnumFontFamiliesEx(IntPtr, ref LOGFONT, FONTENUMPROC, IntPtr, uint)"/>.
            /// </summary>
            /// <param name="lpelf">Logical font info.</param>
            /// <param name="lpntm">Text metrics.</param>
            /// <param name="FontType">Font type.</param>
            /// <param name="lParam">Opaque parameter.</param>
            /// <returns>What <see cref="EnumFontFamiliesEx(IntPtr, ref LOGFONT, FONTENUMPROC, IntPtr, uint)"/> will return.</returns>
            public delegate int FONTENUMPROC(ref ENUMLOGFONTEX lpelf, ref NEWTEXTMETRIC lpntm, uint FontType, IntPtr lParam);

            /// <summary>
            /// Win32 function to enumerate font families.
            /// </summary>
            /// <param name="hdc">Device contect handle.</param>
            /// <param name="lpLogfont">Logical font structure.</param>
            /// <param name="lpEnumFontFamExProc">Callback function.</param>
            /// <param name="lParam">Opaque parameter to pass to the callback method.</param>
            /// <param name="dwFlags">Option flags.</param>
            /// <returns>The last value returned by the callback function.</returns>
            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            public static extern int EnumFontFamiliesEx(IntPtr hdc, ref LOGFONT lpLogfont, FONTENUMPROC lpEnumFontFamExProc, IntPtr lParam, uint dwFlags);

            /// <summary>
            /// Logical font information.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct LOGFONT
            {
                public int lfHeight;
                public int lfWidth;
                public int lfEscapement;
                public int lfOrientation;
                public int lfWeight;
                public byte lfItalic;
                public byte lfUnderline;
                public byte lfStrikeOut;
                public byte lfCharSet;
                public byte lfOutPrecision;
                public byte lfClipPrecision;
                public byte lfQuality;
                public byte lfPitchAndFamily;

                /// <summary>
                /// Face name.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
                public string lfFaceName;
            }

            /// <summary>
            /// Enumerated font information.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct ENUMLOGFONTEX
            {
                /// <summary>
                /// Logical font information.
                /// </summary>
                public LOGFONT elfLogFont;

                /// <summary>
                /// Full name of font.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FULLFACESIZE)]
                public string elfFullName;

                /// <summary>
                /// Font style.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
                public string elfStyle;

                /// <summary>
                /// Font script.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
                public string elfScript;
            }

            /// <summary>
            /// Text metrics for a font.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct NEWTEXTMETRIC
            {
                public int tmHeight;
                public int tmAscent;
                public int tmDescent;
                public int tmInternalLeading;
                public int tmExternalLeading;
                public int tmAveCharWidth;
                public int tmMaxCharWidth;
                public int tmWeight;
                public int tmOverhang;
                public int tmDigitizedAspectX;
                public int tmDigitizedAspectY;
                public char tmFirstChar;
                public char tmLastChar;
                public char tmDefaultChar;
                public char tmBreakChar;
                public byte tmItalic;
                public byte tmUnderlined;
                public byte tmStruckOut;
                public byte tmPitchAndFamily;
                public byte tmCharSet;
                public uint ntmFlags;
                public uint ntmSizeEM;
                public uint ntmCellHeight;
                public uint ntmAvgWidth;
            }
#pragma warning restore SA1600 // Elements should be documented
        }
    }
}