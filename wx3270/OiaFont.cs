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
        /// </summary>
        public class Symbol
        {
            /// <summary>
            /// The letter A over a box.
            /// </summary>
            public const string CommA = "\u1e00";

            /// <summary>
            /// The insert indicator, like a '^'.
            /// </summary>
            public const string Insert = "\u1e01";

            /// <summary>
            /// The letter B over a box.
            /// </summary>
            public const string CommB = "\u1e02";

            /// <summary>
            /// The number 6 in a box.
            /// </summary>
            public const string Box6 = "\u1e03";

            /// <summary>
            /// An arrow pointing to the right.
            /// </summary>
            public const string RightArrow = "\u1e04";

            /// <summary>
            /// A hollow arrow pointing upward, used to indicate the Shift key.
            /// </summary>
            public const string UpShift = "\u1e05";

            /// <summary>
            /// An underlined letter B.
            /// </summary>
            public const string UnderB = "\u1e06";

            /// <summary>
            /// A hollow arrow pointing downward.
            /// </summary>
            public const string DownShift = "\u1e07";

            /// <summary>
            /// A question mark in a box.
            /// </summary>
            public const string BoxQuestion = "\u1e08";

            /// <summary>
            /// A solid box.
            /// </summary>
            public const string BoxSolid = "\u1e09";

            /// <summary>
            /// A broken communication line.
            /// </summary>
            public const string CommBad = "\u1e0a";

            /// <summary>
            /// The high side of a communication line.
            /// </summary>
            public const string CommHigh = "\u1e0b";

            /// <summary>
            /// The zig-zag part of a communication line.
            /// </summary>
            public const string CommJag = "\u1e0c";

            /// <summary>
            /// The low side of a communication line.
            /// </summary>
            public const string CommLow = "\u1e0d";

            /// <summary>
            /// The left side of a clock face.
            /// </summary>
            public const string ClockLeft = "\u1e0e";

            /// <summary>
            /// The right side of a clock face.
            /// </summary>
            public const string ClockRight = "\u1e0f";

            /// <summary>
            /// The keyboard lock sybols, like an X.
            /// </summary>
            public const string X = "\u1e10";

            /// <summary>
            /// An arrow pointing left.
            /// </summary>
            public const string LeftArrow = "\u1e11";

            /// <summary>
            /// The left side of a key (lock) symbol.
            /// </summary>
            public const string KeyLeft = "\u1e12";

            /// <summary>
            /// The right side of a key (lock) symbol.
            /// </summary>
            public const string KeyRight = "\u1e13";

            /// <summary>
            /// The number 4 in a box.
            /// </summary>
            public const string Box4 = "\u1e14";

            /// <summary>
            /// An underlined letter A.
            /// </summary>
            public const string UnderA = "\u1e15";

            /// <summary>
            /// A magnetic card.
            /// </summary>
            public const string MagCard = "\u1e16";

            /// <summary>
            /// A human symbol in a box.
            /// </summary>
            public const string BoxHuman = "\u1e17";

            /// <summary>
            /// A human symbol.
            /// </summary>
            public const string Human = "\u1e18";

            /// <summary>
            /// The letter N in a box.
            /// </summary>
            public const string BoxN = "\u1e19";

            /// <summary>
            /// A printer symbol.
            /// </summary>
            public const string Printer = "\u1e1a";

            /// <summary>
            /// A lock symbol.
            /// </summary>
            public const string Lock = "\u1e1b";

            /// <summary>
            /// A field mark symbol.
            /// </summary>
            public const string FieldMark = "\u1e1c";

            /// <summary>
            /// A dup symbol.
            /// </summary>
            public const string Dup = "\u1e1d";
        }
    }
}
