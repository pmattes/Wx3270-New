// <copyright file="AttributeDict.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;

    /// <summary>
    /// An attribute dictionary for an element start.
    /// </summary>
    public class AttributeDict : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDict"/> class.
        /// </summary>
        public AttributeDict()
            : base()
        {
        }
    }
}