// <copyright file="KeyboardModifier.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// A keyboard modifier.
    /// </summary>
    /// <remarks>
    /// The values are defined to enforce precedence when there is ambiguity: if there is a rule for Shift-X and a rule
    /// for Ctrl-X and the event is Shift-Ctrl-X, the Shift-X rule wins because Shift has higher precedence.
    /// 3270-mode and NVT-mode rules have the highest precedence.
    /// With this precedence definition, there is no need to order the rules.
    /// Of course, an exact match always wins.
    /// </remarks>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KeyboardModifier
    {
        /// <summary>
        /// No modifiers set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The alt key.
        /// </summary>
        Alt = 0x1,

        /// <summary>
        /// The control key.
        /// </summary>
        Ctrl = 0x2,

        /// <summary>
        /// The shift key.
        /// </summary>
        Shift = 0x4,

        /// <summary>
        /// 3270-only mode. Mutually exclusive with NVT-only mode.
        /// </summary>
        Mode3270 = 0x8,

        /// <summary>
        /// NVT-only mode. Mutually exclusive with 3270-only mode.
        /// </summary>
        ModeNvt = 0x10,

        /// <summary>
        /// APL mode, used to define a secondary key map.
        /// </summary>
        Apl = 0x20,

        /// <summary>
        /// The set of modifiers that correspond to actual keys.
        /// </summary>
        NaturalModifiers = Alt | Ctrl | Shift,

        /// <summary>
        /// The set of modifiers for 3270 versus NVT modes.
        /// </summary>
        ModeModifiers = Mode3270 | ModeNvt,

        /// <summary>
        /// The set of synthetic (non-keyboard) modifiers.
        /// </summary>
        SyntheticModifiers = ModeModifiers | Apl,
    }
}
