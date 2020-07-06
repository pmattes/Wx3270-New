// <copyright file="FlashFsm.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// A generic flash interface.
    /// </summary>
    public class FlashFsm
    {
        /// <summary>
        /// The timer.
        /// </summary>
        private readonly Timer timer = new Timer();

        /// <summary>
        /// The tick method.
        /// </summary>
        private readonly Action<Action> tick;

        /// <summary>
        /// The state.
        /// </summary>
        private State state;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashFsm"/> class.
        /// </summary>
        /// <param name="tick">Tick method.</param>
        public FlashFsm(Action<Action> tick)
        {
            this.tick = tick;
            this.timer.Interval = 500;
            this.timer.Tick += this.Tick;
        }

        /// <summary>
        /// Actions to perform.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Do the flash.
            /// </summary>
            Flash,

            /// <summary>
            /// Restore the original state.
            /// </summary>
            Restore,

            /// <summary>
            /// Do nothing.
            /// </summary>
            Nop,
        }

        /// <summary>
        /// Internal state.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Idle state.
            /// </summary>
            None,

            /// <summary>
            /// First flash.
            /// </summary>
            Flash1,

            /// <summary>
            /// Pause between flashes.
            /// </summary>
            Pause,

            /// <summary>
            /// Second flash.
            /// </summary>
            Flash2,
        }

        /// <summary>
        /// Start the flash sequence.
        /// </summary>
        /// <returns>Action to take.</returns>
        public Action Start()
        {
            if (this.state != State.None)
            {
                return Action.Nop;
            }

            this.state = State.Flash1;
            this.timer.Start();
            return Action.Flash;
        }

        /// <summary>
        /// The timer has expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Tick(object sender, EventArgs e)
        {
            switch (this.state)
            {
                default:
                    break;
                case State.Flash1:
                    this.state = State.Pause;
                    this.tick(Action.Restore);
                    this.timer.Start();
                    break;
                case State.Pause:
                    this.state = State.Flash2;
                    this.tick(Action.Flash);
                    this.timer.Start();
                    break;
                case State.Flash2:
                    this.timer.Stop();
                    this.state = State.None;
                    this.tick(Action.Restore);
                    break;
            }
        }
    }
}
