// <copyright file="SyncedListBoxes.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for index selection changes.
    /// </summary>
    /// <param name="entry">Entry that is selected</param>
    /// <typeparam name="T">Type of entry</typeparam>
    /// <remarks><paramref name="entry"/> will be null if no entry is selected</remarks>
    public delegate void EntrySelected<T>(T entry);

    /// <summary>
    /// A set of entries, integrated with a set of <see cref="ListBox"/> controls.
    /// </summary>
    /// <typeparam name="T">Type of entries stored</typeparam>
    /// <remarks>Keeps the list boxes synchronized</remarks>
    public class SyncedListBoxes<T> : ISynced<T>
        where T : class, ISyncedEntry<T>
    {
        /// <summary>
        /// The list boxes.
        /// </summary>
        private readonly List<ListBox> listBoxes = new List<ListBox>();

        /// <summary>
        /// The list of entries.
        /// </summary>
        private readonly List<T> entries = new List<T>();

        /// <summary>
        /// Recursion avoidance counter.
        /// </summary>
        private int updating;

        /// <summary>
        /// List change event.
        /// </summary>
        public event Action ChangeEvent = () => { };

        /// <summary>
        /// Entry selected event.
        /// </summary>
        public event EntrySelected<T> EntrySelectedEvent = (entry) => { };

        /// <summary>
        /// Gets or sets the set of entries.
        /// </summary>
        public IEnumerable<T> Entries
        {
            get
            {
                // Return a copy.
                return this.entries.Select(e => e.Clone()).ToList();
            }

            set
            {
                this.Updating(() =>
                {
                    var array = value.ToArray();
                    this.entries.Clear();
                    this.entries.AddRange(array);
                    foreach (var box in this.listBoxes)
                    {
                        box.Items.Clear();
                        box.Items.AddRange(array);
                    }

                    this.ChangeEvent();
                });
            }
        }

        /// <summary>
        /// Gets or sets an entry.
        /// </summary>
        /// <param name="index">Index of entry</param>
        /// <returns>Contents of entry</returns>
        public T this[int index]
        {
            get
            {
                return this.entries[index];
            }

            set
            {
                this.entries[index] = value;
                foreach (var box in this.listBoxes)
                {
                    box.Items[index] = value;
                }

                // Check for modify conflicts.
                var i = 0;
                foreach (var entry in this.entries)
                {
                    if (i != index && entry.CheckConflict(value) == EntryConflict.Modified)
                    {
                        foreach (var box in this.listBoxes)
                        {
                            // Force an update of the list box entry.
                            box.Items[i] = entry;
                        }
                    }

                    i++;
                }

                // Check for a replace conflict.
                i = 0;
                foreach (var entry in this.entries)
                {
                    if (i == index)
                    {
                        break;
                    }

                    if (entry.CheckConflict(value) == EntryConflict.Replace)
                    {
                        // The edited entry replaces a different entry.
                        // Delete the other entry.
                        this.entries.RemoveAt(i);
                        foreach (var box in this.listBoxes)
                        {
                            box.Items.RemoveAt(i);
                            box.SelectedItem = value;
                        }

                        break;
                    }

                    i++;
                }

                this.ChangeEvent();
            }
        }

        /// <summary>
        /// Add a <see cref="ListBox"/> to the list.
        /// </summary>
        /// <param name="listBox">List box</param>
        public void AddListBox(ListBox listBox)
        {
            // Remember the ListBox.
            this.listBoxes.Add(listBox);

            // Subscribe to index change events.
            listBox.SelectedIndexChanged += new EventHandler(this.SelectIndexChanged);
        }

        /// <summary>
        /// Add an entry to the list.
        /// </summary>
        /// <param name="entry">New entry</param>
        /// <param name="beforeHead">True to add at head instead of tail</param>
        public void Add(T entry, bool beforeHead = false)
        {
            this.Updating(() =>
            {
                var i = 0;

                // Check for modify conflicts.
                foreach (var compareEntry in this.entries)
                {
                    if (this.entries[i].CheckConflict(entry) == EntryConflict.Modified)
                    {
                        foreach (var box in this.listBoxes)
                        {
                            box.Items[i] = compareEntry;
                        }
                    }

                    i++;
                }

                // Check for a replace conflict.
                i = 0;
                foreach (var compareEntry in this.entries)
                {
                    if (this.entries[i].CheckConflict(entry) == EntryConflict.Replace)
                    {
                        this.entries.RemoveAt(i);
                        foreach (var box in this.listBoxes)
                        {
                            box.Items.RemoveAt(i);
                        }

                        break;
                    }

                    i++;
                }

                // Add it.
                if (beforeHead)
                {
                    this.entries.Insert(0, entry);
                }
                else
                {
                    this.entries.Add(entry);
                }

                foreach (var box in this.listBoxes)
                {
                    if (beforeHead)
                    {
                        box.Items.Insert(0, entry);
                    }
                    else
                    {
                        box.Items.Add(entry);
                    }

                    box.SelectedItem = entry;
                }

                this.ChangeEvent();
            });
        }

        /// <summary>
        /// Move an entry.
        /// </summary>
        /// <param name="source">Source object index</param>
        /// <param name="destination">Destination object index</param>
        public void Move(int source, int destination)
        {
            if (source < 0 || source >= this.entries.Count || destination < 0 || destination >= this.entries.Count)
            {
                // Can't do that.
                return;
            }

            var entry = this.entries[source];
            this.entries.RemoveAt(source);
            this.entries.Insert(destination, entry);
            foreach (var box in this.listBoxes)
            {
                box.Items.RemoveAt(source);
                box.Items.Insert(destination, entry);
                box.SelectedIndex = destination;
            }

            this.ChangeEvent();
        }

        /// <summary>
        /// Delete the selected entry.
        /// </summary>
        /// <param name="listBox">List box</param>
        public void Delete(ListBox listBox)
        {
            this.Updating(() =>
            {
                var index = listBox.SelectedIndex;
                if (index < 0)
                {
                    return;
                }

                this.entries.RemoveAt(index);
                foreach (var box in this.listBoxes)
                {
                    box.Items.RemoveAt(index);
                    if (this.entries.Count > 0)
                    {
                        box.SelectedIndex = (index == this.entries.Count) ? index - 1 : index;
                    }
                }

                this.ChangeEvent();
            });
        }

        /// <summary>
        /// Perform an action while avoiding recursion.
        /// </summary>
        /// <param name="a">Action to perform</param>
        private void Updating(Action a)
        {
            this.updating++;
            try
            {
                a();
            }
            finally
            {
                this.updating--;
            }
        }

        /// <summary>
        /// The selection index changed. Tell the other list boxes and turn that into an entry event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SelectIndexChanged(object sender, EventArgs e)
        {
            // Avoid recursion.
            if (this.updating > 0)
            {
                return;
            }

            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            var index = listBox.SelectedIndex;
            foreach (var box in this.listBoxes)
            {
                if (box != listBox)
                {
                    box.SelectedIndex = index;
                }
            }

            this.EntrySelectedEvent(index >= 0 ? this.entries[index] : null);
        }
    }
}
