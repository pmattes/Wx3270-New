// <copyright file="MacroEntry.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using Newtonsoft.Json;
    using Wx3270.Contracts;

    /// <summary>
    /// One macro.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class MacroEntry : IEquatable<MacroEntry>, ISyncedEntry<MacroEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MacroEntry"/> class.
        /// </summary>
        public MacroEntry()
        {
            this.Name = string.Empty;
            this.Macro = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroEntry"/> class;
        /// </summary>
        /// <param name="other">Other entry</param>
        public MacroEntry(MacroEntry other)
        {
            this.Name = other.Name;
            this.Macro = other.Macro;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the entry can be moved.
        /// </summary>
        public bool CanMove => true;

        /// <summary>
        /// Gets a value indicating whether another entry can be moved before this one.
        /// </summary>
        [JsonIgnore]
        public bool CanMoveBefore => true;

        /// <summary>
        /// Gets or sets the macro text.
        /// </summary>
        [JsonProperty]
        public string Macro { get; set; }

        /// <summary>
        /// Make a copy of an entry.
        /// </summary>
        /// <returns>Cloned entry</returns>
        public MacroEntry Clone()
        {
            return new MacroEntry(this);
        }

        /// <summary>
        /// Compare one macro entry to another.
        /// </summary>
        /// <param name="other">Other host</param>
        /// <returns>True if they are equal</returns>
        public bool Equals(MacroEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase)
                && this.EqualsExeptName(other);
        }

        /// <summary>
        /// Compare one macro entry to another, except for the name field.
        /// </summary>
        /// <param name="other">Other macro</param>
        /// <returns>True if they are equal</returns>
        public bool EqualsExeptName(MacroEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Macro.Equals(other.Macro, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Replace the contents of one <see cref="MacroEntry"/> with another.
        /// </summary>
        /// <param name="other">Other entry</param>
        public void Replace(MacroEntry other)
        {
            if (other == null)
            {
                return;
            }

            this.Name = other.Name;
            this.Macro = other.Macro;
        }

        /// <summary>
        /// Return the string representation of a macro entry.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return this.Name + "\t " + this.Macro.Replace(Environment.NewLine, " ");
        }

        /// <summary>
        /// Check for a conflict between this entry and an edited entry.
        /// </summary>
        /// <param name="editedEntry">Edited entry</param>
        /// <returns>Conflict status</returns>
        public EntryConflict CheckConflict(MacroEntry editedEntry)
        {
            if (this.Name.Equals(editedEntry.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return EntryConflict.Replace;
            }

            return EntryConflict.None;
        }
    }
}
