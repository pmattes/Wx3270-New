// <copyright file="IBackEndEvent.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Back-end events.
    /// </summary>
    public interface IBackEndEvent
    {
        /// <summary>
        /// Gets the set of start handlers.
        /// </summary>
        IEnumerable<BackEndEventDef> Def { get; }

        /// <summary>
        /// Add a handler to the event.
        /// </summary>
        /// <param name="control">Invoking control</param>
        /// <param name="handler">Handler to invoke</param>
        void Add(Control control, Action handler);

        /// <summary>
        /// Remove a handler from the event.
        /// </summary>
        /// <param name="control">Invoking control</param>
        /// <param name="handler">Handler to invoke</param>
        void Remove(Control control, Action handler);
    }
}
