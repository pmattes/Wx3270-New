// <copyright file="OiaFont.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// Operator Information Area (OIA) font information.
    /// </summary>
    public class OiaFont
    {
        /// <summary>
        /// Special symbols in the 3270 font.
        /// These live in the Unicode Private Use area (U+E000 - U+F8FF).
        /// </summary>
        public class Symbol
        {
            /// <summary>
            /// The letter A over a box.
            /// </summary>
            public const string CommA = "\ue000";

            /// <summary>
            /// The insert indicator, like a '^'.
            /// </summary>
            public const string Insert = "\ue001";

            /// <summary>
            /// The letter B over a box.
            /// </summary>
            public const string CommB = "\ue002";

            /// <summary>
            /// The number 6 in a box.
            /// </summary>
            public const string Box6 = "\ue003";

            /// <summary>
            /// An arrow pointing to the right.
            /// </summary>
            public const string RightArrow = "\ue004";

            /// <summary>
            /// A hollow arrow pointing upward, used to indicate the Shift key.
            /// </summary>
            public const string UpShift = "\ue005";

            /// <summary>
            /// An underlined letter B.
            /// </summary>
            public const string UnderB = "\ue006";

            /// <summary>
            /// A hollow arrow pointing downward.
            /// </summary>
            public const string DownShift = "\ue007";

            /// <summary>
            /// A question mark in a box.
            /// </summary>
            public const string BoxQuestion = "\ue008";

            /// <summary>
            /// A solid box.
            /// </summary>
            public const string BoxSolid = "\ue009";

            /// <summary>
            /// A broken communication line.
            /// </summary>
            public const string CommBad = "\ue00a";

            /// <summary>
            /// The high side of a communication line.
            /// </summary>
            public const string CommHigh = "\ue00b";

            /// <summary>
            /// The zig-zag part of a communication line.
            /// </summary>
            public const string CommJag = "\ue00c";

            /// <summary>
            /// The low side of a communication line.
            /// </summary>
            public const string CommLow = "\ue00d";

            /// <summary>
            /// The left side of a clock face.
            /// </summary>
            public const string ClockLeft = "\ue00e";

            /// <summary>
            /// The right side of a clock face.
            /// </summary>
            public const string ClockRight = "\ue00f";

            /// <summary>
            /// The keyboard lock sybols, like an X.
            /// </summary>
            public const string X = "\ue010";

            /// <summary>
            /// An arrow pointing left.
            /// </summary>
            public const string LeftArrow = "\ue011";

            /// <summary>
            /// The left side of a key (lock) symbol.
            /// </summary>
            public const string KeyLeft = "\ue012";

            /// <summary>
            /// The right side of a key (lock) symbol.
            /// </summary>
            public const string KeyRight = "\ue013";

            /// <summary>
            /// The number 4 in a box.
            /// </summary>
            public const string Box4 = "\ue014";

            /// <summary>
            /// An underlined letter A.
            /// </summary>
            public const string UnderA = "\ue015";

            /// <summary>
            /// A magnetic card.
            /// </summary>
            public const string MagCard = "\ue016";

            /// <summary>
            /// A human symbol in a box.
            /// </summary>
            public const string BoxHuman = "\ue017";

            /// <summary>
            /// A human symbol.
            /// </summary>
            public const string Human = "\ue018";

            /// <summary>
            /// The letter N in a box.
            /// </summary>
            public const string BoxN = "\ue019";

            /// <summary>
            /// A printer symbol.
            /// </summary>
            public const string Printer = "\ue01a";

            /// <summary>
            /// A lock symbol.
            /// </summary>
            public const string Lock = "\ue01b";

            /// <summary>
            /// A field mark symbol.
            /// </summary>
            public const char FieldMark = '\ue02a';

            /// <summary>
            /// A dup symbol.
            /// </summary>
            public const char Dup = '\ue03b';
        }
    }
}
