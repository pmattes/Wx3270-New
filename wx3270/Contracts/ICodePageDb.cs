// <copyright file="ICodePageDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Code page database interface.
    /// </summary>
    public interface ICodePageDb
    {
        /// <summary>
        /// Gets the canonincal name for a code page, given a (possible) alias.
        /// </summary>
        /// <param name="alias">Alias name.</param>
        /// <returns>Canonical name, or null.</returns>
        string CanonicalName(string alias);
    }
}
