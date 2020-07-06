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
        /// <summary>
        /// Gets or sets the actions to perform.
        /// </summary>
        public string Actions { get; set; } = string.Empty;

        /// <summary>
        /// Clone a keyboard map entry.
        /// </summary>
        /// <returns>Cloned keyboard map entry.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Equality comparer for a keyboard map.
        /// </summary>
        /// <param name="other">Other map.</param>
        /// <returns>True if they are equal.</returns>
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

            return this.Actions == other.Actions;
        }

        /// <summary>
        /// Object-based equality comparer.
        /// </summary>
        /// <param name="obj">Other (possible) keyboard map.</param>
        /// <returns>True if they are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is KeyboardMap && this.Equals((KeyboardMap)obj);
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
