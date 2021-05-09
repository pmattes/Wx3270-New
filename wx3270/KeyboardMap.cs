// <copyright file="KeyboardMap.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Wx3270.Contracts;

    /// <summary>
    /// A keyboard map entry.
    /// </summary>
    public class KeyboardMap : IEquatable<KeyboardMap>, ICloneable, IActions
    {
        /// <inheritdoc />
        public string Actions { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool Exact { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <inheritdoc />
        public bool Equals(KeyboardMap other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Actions == other.Actions && this.Exact == other.Exact;
        }

        /// <summary>
        /// Object-based equality comparer.
        /// </summary>
        /// <param name="obj">Other (possible) keyboard map.</param>
        /// <returns>True if they are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is KeyboardMap map && this.Equals(map);
        }

        /// <summary>
        /// Get the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return this.Actions.GetHashCode();
        }
    }
}
