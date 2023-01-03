// <copyright file="FontProfile.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Profile tab.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FontProfile : IEquatable<FontProfile>
    {
        /// <summary>
        /// The name of the bit-mapped 3270 font.
        /// </summary>
        public const string Name3270Font = "3270";

        /// <summary>
        /// The name of Ricardo Bánffy's 3270 font.
        /// </summary>
        public const string Name3270FontRb = "IBM 3270";

        /// <summary>
        /// The default font name.
        /// </summary>
        public const string DefaultName = Name3270FontRb;

        /// <summary>
        /// The default font size.
        /// </summary>
        public const int DefaultEmSize = 10;

        /// <summary>
        /// The fallback font name.
        /// </summary>
        public const string FallbackName = "Consolas";

        /// <summary>
        /// The fallback font size.
        /// </summary>
        public const int FallbackEmSize = 12;

        /// <summary>
        /// Backing field for <see cref="Default"/> .
        /// </summary>
        private static FontProfile defaultFontProfile = new FontProfile();

        /// <summary>
        /// Backing field for <see cref="name"/>.
        /// </summary>
        private string name = DefaultName;

        /// <summary>
        /// Backing field for <see cref="EmSize"/>.
        /// </summary>
        private float emSize = DefaultEmSize;

        /// <summary>
        /// Backing field for <see cref="Style"/>.
        /// </summary>
        private FontStyle style = FontStyle.Regular;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontProfile"/> class.
        /// </summary>
        public FontProfile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontProfile"/> class.
        /// </summary>
        /// <param name="font">Font to capture.</param>
        public FontProfile(Font font)
        {
            this.Name = font.Name;
            this.EmSize = font.SizeInPoints;
            this.Style = font.Style;
        }

        /// <summary>
        /// Gets the default font profile.
        /// </summary>
        public FontProfile Default => defaultFontProfile;

        /// <summary>
        /// Gets or sets the family name.
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets the em size.
        /// </summary>
        [JsonProperty]
        public float EmSize
        {
            get { return this.emSize; }
            set { this.emSize = value; }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public FontStyle Style
        {
            get { return this.style; }
            set { this.style = value; }
        }

        /// <summary>
        /// Constructs a real <see cref="System.Drawing.Font"/> from a profile.
        /// </summary>
        /// <returns>Real font.</returns>
        public Font Font()
        {
            var font = new Font(this.Name, this.EmSize, this.Style);
            if (font.Name != this.Name)
            {
                font = new Font(FallbackName, FallbackEmSize, FontStyle.Regular);
            }

            return font;
        }

        /// <summary>
        /// Equality test for a font profile.
        /// </summary>
        /// <param name="other">Other profile.</param>
        /// <returns>True if they are equal.</returns>
        public bool Equals(FontProfile other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Name == other.Name
                && this.EmSize == other.EmSize
                && this.Style == other.Style;
        }
    }
}
