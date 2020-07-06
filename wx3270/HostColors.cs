// <copyright file="HostColors.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Host color dictionary.
    /// </summary>
    public class HostColors : Dictionary<HostColor, Color>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostColors"/> class.
        /// </summary>
        public HostColors()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostColors"/> class.
        /// </summary>
        /// <param name="colors">Existing dictionary.</param>
        public HostColors(IDictionary<HostColor, Color> colors)
            : base(colors)
        {
        }

        /// <summary>
        /// Equality comparer.
        /// </summary>
        /// <param name="other">Other dictionary.</param>
        /// <returns>True if the dictionaries are equal.</returns>
        public bool Equals(HostColors other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Count == other.Count &&
                this.All(pair => other.ContainsKey(pair.Key) && pair.Value.Equals(other[pair.Key]));
        }
    }
}
