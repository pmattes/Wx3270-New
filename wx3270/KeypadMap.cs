// <copyright file="KeypadMap.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Wx3270.Contracts;

    /// <summary>
    /// A background image for a keypad button.
    /// </summary>
    public enum KeypadBackgroundImage
    {
        /// <summary>
        /// A blank key.
        /// </summary>
        Blank,

        /// <summary>
        /// The insert key.
        /// </summary>
        Insert,

        /// <summary>
        /// The delete key.
        /// </summary>
        Delete,
    }

    /// <summary>
    /// A keypad map.
    /// </summary>
    public class KeypadMap : IEquatable<KeypadMap>, ICloneable, IActions
    {
        /// <summary>
        /// Backing field for <see cref="BackgroundImage"/>.
        /// </summary>
        private KeypadBackgroundImage backgroundImage = KeypadBackgroundImage.Blank;

        /// <summary>
        /// Backing field for <see cref="Text"/>.
        /// </summary>
        private string text = string.Empty;

        /// <summary>
        /// Backing field for <see cref="TextSize"/>.
        /// </summary>
        private float textSize = 6.75F;

        /// <summary>
        /// Backing field for <see cref="Actions"/>.
        /// </summary>
        private string actions = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypadMap"/> class.
        /// </summary>
        public KeypadMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypadMap"/> class.
        /// </summary>
        /// <param name="other">Existing keypad map.</param>
        public KeypadMap(KeypadMap other)
        {
            if (other != null)
            {
                this.Text = other.Text;
                this.TextSize = other.TextSize;
                this.Actions = other.Actions;
                this.Exact = other.Exact;
            }
        }

        /// <summary>
        /// Gets or sets the background image name.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeypadBackgroundImage BackgroundImage
        {
            get { return this.backgroundImage; }
            set { this.backgroundImage = value; }
        }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        /// <summary>
        /// Gets or sets the label text size.
        /// </summary>
        public float TextSize
        {
            get { return this.textSize; }
            set { this.textSize = value; }
        }

        /// <inheritdoc />
        public string Actions
        {
            get { return this.actions; }
            set { this.actions = value; }
        }

        /// <inheritdoc />
        public bool Exact { get; set; }

        /// <inheritdoc />
        public bool Equals(KeypadMap other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Text == other.Text
                && this.TextSize == other.TextSize
                && this.Actions == other.Actions
                && this.Exact == other.Exact
                && this.BackgroundImage == other.BackgroundImage;
        }

        /// <summary>
        /// Object-based equality comparer.
        /// </summary>
        /// <param name="obj">Other (possible) keypad map.</param>
        /// <returns>True if they are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is KeypadMap map && this.Equals(map);
        }

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return this.Text.GetHashCode() ^ this.TextSize.GetHashCode() ^ this.Actions.GetHashCode() ^ this.BackgroundImage.GetHashCode();
        }

        /// <summary>
        /// Clone the keypad map entry.
        /// </summary>
        /// <returns>Cloned keypad map entry.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
