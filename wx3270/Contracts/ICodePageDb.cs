// <copyright file="ICodePageDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Code page database interface.
    /// </summary>
    public interface ICodePageDb
    {
        /// <summary>
        /// Gets the set of all code pages canonical names and friendly names.
        /// </summary>
        public IEnumerable<string> All { get; }

        /// <summary>
        /// Adds (or immediately calls) a done action.
        /// </summary>
        /// <param name="action">Action to call.</param>
        public void AddDone(Action action);

        /// <summary>
        /// Gets the index of a particular code page.
        /// </summary>
        /// <param name="codePage">Code page name.</param>
        /// <returns>Index, or -1.</returns>
        public int Index(string codePage);

        /// <summary>
        /// Gets the canonincal name for a code page, given a (possible) alias.
        /// </summary>
        /// <param name="alias">Alias name.</param>
        /// <returns>Canonical name, or null.</returns>
        string CanonicalName(string alias);
    }
}
