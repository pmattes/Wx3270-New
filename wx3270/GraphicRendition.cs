// <copyright file="GraphicRendition.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// Graphic rendition flags.
    /// </summary>
    [Flags]
    public enum GraphicRendition
    {
        /// <summary>
        /// No GR attributes.
        /// </summary>
        None = 0,

        /// <summary>
        /// Underlined text.
        /// </summary>
        Underline = 0x1,

        /// <summary>
        /// Blinking text.
        /// </summary>
        Blink = 0x2,

        /// <summary>
        /// Highlighted text.
        /// </summary>
        Highlight = 0x4,

        /// <summary>
        /// Selectable by the light pen.
        /// </summary>
        Selectable = 0x8,

        /// <summary>
        /// Reverse-video (monochrome displays only).
        /// </summary>
        Reverse = 0x10,

        /// <summary>
        /// DBCS character (each value takes two cells).
        /// </summary>
        Wide = 0x20,

        /// <summary>
        /// Visible 3270 order.
        /// </summary>
        Order = 0x40,

        /// <summary>
        /// Copy/paste into the BMP Private Use Area.
        /// </summary>
        PrivateUse = 0x80,

        /// <summary>
        /// Do not copy into a paste buffer.
        /// </summary>
        NoCopy = 0x100,

        /// <summary>
        /// NVT-mode text wrapped.
        /// </summary>
        Wrap = 0x200,

        /// <summary>
        /// Selected text (added by <code>wx3270</code>, not by the emulator)
        /// </summary>
        Selected = 0x1000,

        /// <summary>
        /// Is the cursor location.
        /// </summary>
        IsCursor = 0x2000,

        /// <summary>
        /// Is part of the crosshair cursor.
        /// </summary>
        IsCrosshair = 0x4000,

        /// <summary>
        /// Is blinking on.
        /// </summary>
        IsBlinkingOn = 0x8000,
    }
}
