// <copyright file="ConfigAction.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;

    /// <summary>
    /// Undo/Redo stack objects.
    /// </summary>
    public abstract class ConfigAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigAction"/> class.
        /// </summary>
        /// <param name="what">Description of change</param>
        /// <param name="profileName">Affected profile name</param>
        public ConfigAction(string what, string profileName = null)
        {
            this.What = profileName != null ? $"{what} ({profileName})" : what;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string What { get; private set; }

        /// <summary>
        /// Perform an undo or redo operation.
        /// </summary>
        /// <param name="from">Stack to move from</param>
        /// <param name="to">Stack to move to</param>
        /// <param name="skipInvert">True to skip inversion</param>
        public abstract void Apply(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert);
    }
}
