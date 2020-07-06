// <copyright file="ISynced.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// A synchronized collection of objects.
    /// </summary>
    /// <typeparam name="T">Type of entries stored</typeparam>
    public interface ISynced<T>
        where T : class, ISyncedEntry<T>
    {
        /// <summary>
        /// List change event.
        /// </summary>
        event Action ChangeEvent;

        /// <summary>
        /// Entry selected event.
        /// </summary>
        event EntrySelected<T> EntrySelectedEvent;

        /// <summary>
        /// Gets or sets the set of entries.
        /// </summary>
        IEnumerable<T> Entries { get; set; }

        /// <summary>
        /// Gets or sets an entry.
        /// </summary>
        /// <param name="index">Index of entry</param>
        /// <returns>Contents of entry</returns>
        T this[int index] { get; set; }

        /// <summary>
        /// Add an entry to the list.
        /// </summary>
        /// <param name="entry">New entry</param>
        /// <param name="beforeHead">True to add at head instead of tail</param>
        void Add(T entry, bool beforeHead = false);

        /// <summary>
        /// Move an entry.
        /// </summary>
        /// <param name="source">Source object index</param>
        /// <param name="destination">Destination object index</param>
        void Move(int source, int destination);

        /// <summary>
        /// Delete the selected entry.
        /// </summary>
        /// <param name="listBox">List box</param>
        void Delete(ListBox listBox);
    }
}
