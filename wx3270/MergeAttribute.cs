// <copyright file="MergeAttribute.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// Merge attribute.
    /// </summary>
    public class MergeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeAttribute"/> class.
        /// </summary>
        /// <param name="importType">Import type.</param>
        public MergeAttribute(ImportType importType)
        {
            this.ImportType = importType;
        }

        /// <summary>
        /// Gets or sets the import type.
        /// </summary>
        public ImportType ImportType { get; set; }
    }
}