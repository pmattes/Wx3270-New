// <copyright file="ISyncedEntry.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// How to resolve a conflict between two entries.
    /// </summary>
    public enum EntryConflict
    {
        /// <summary>
        /// No conflict.
        /// </summary>
        None,

        /// <summary>
        /// The existing entry was modified, no further action needed.
        /// </summary>
        Modified,

        /// <summary>
        /// The edited entry replaces the existing entry.
        /// Overwrite it and delete the edited entry.
        /// </summary>
        Replace,
    }

    /// <summary>
    /// An object suitable for use in a <see cref="SyncedListBoxes{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of entries</typeparam>
    /// <remarks>Keeps the list boxes synchronized</remarks>
    public interface ISyncedEntry<T>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the entry can be moved.
        /// </summary>
        bool CanMove { get; }

        /// <summary>
        /// Gets a value indicating whether an entry can be moved before this one.
        /// </summary>
        bool CanMoveBefore { get; }

        /// <summary>
        /// Creates a clone of the object.
        /// </summary>
        /// <returns>Cloned object</returns>
        T Clone();

        /// <summary>
        /// Replaces the contents of an object.
        /// </summary>
        /// <param name="entry">Entry to copy from</param>
        void Replace(T entry);

        /// <summary>
        /// Check for a conflict between this entry and an edited entry.
        /// </summary>
        /// <param name="editedEntry">Edited entry</param>
        /// <returns>Conflict status</returns>
        EntryConflict CheckConflict(T editedEntry);
    }
}
