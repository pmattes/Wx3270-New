// <copyright file="KeyRelease.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// Keypad release animation.
    /// </summary>
    public class KeyRelease
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyRelease"/> class.
        /// </summary>
        /// <param name="when">When to shift back.</param>
        /// <param name="button">Button to shift.</param>
        /// <param name="hideAfter">True to hide the keypad afterward.</param>
        public KeyRelease(DateTime when, Wx3270.NoSelectButton button, bool hideAfter)
        {
            this.When = when;
            this.Button = button;
            this.HideAfter = hideAfter;
        }

        /// <summary>
        /// Gets when to shift the image back.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Gets which button to shift.
        /// </summary>
        public Wx3270.NoSelectButton Button { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to hide the keypad after releasing the key.
        /// </summary>
        public bool HideAfter { get; private set; }
    }
}
