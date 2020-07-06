// <copyright file="BackEndEvent.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// Back-end events.
    /// </summary>
    public abstract class BackEndEvent : IBackEndEvent
    {
        /// <summary>
        /// The event to be signaled when the indication is received.
        /// </summary>
        private event Action ChangeEvent = () => { };

        /// <inheritdoc />
        public IEnumerable<BackEndEventDef> Def { get; protected set; }

        /// <inheritdoc />
        public void Add(Control control, Action handler)
        {
            // Run the handler in the context of the control.
            this.ChangeEvent += () => control.Invoke(new MethodInvoker(() => handler()));
        }

        /// <inheritdoc />
        public void Remove(Control control, Action handler)
        {
            // Run the handler in the context of the control.
            this.ChangeEvent -= () => control.Invoke(new MethodInvoker(() => handler()));
        }

        /// <summary>
        /// Signal the change event.
        /// </summary>
        protected void Change()
        {
            this.ChangeEvent();
        }
    }
}
