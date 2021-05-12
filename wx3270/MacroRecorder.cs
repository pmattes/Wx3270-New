// <copyright file="MacroRecorder.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Macro recorder.
    /// </summary>
    public class MacroRecorder
    {
        /// <summary>
        /// The flash interval.
        /// </summary>
        private const int FlashMs = 350;

        /// <summary>
        /// The flash timer.
        /// </summary>
        private readonly Timer flashTimer = new Timer();

        /// <summary>
        /// The accumulated actions.
        /// </summary>
        private readonly StringBuilder actions = new StringBuilder();

        /// <summary>
        /// True if the recorder is active.
        /// </summary>
        private bool running;

        /// <summary>
        /// True if the image is flashing.
        /// </summary>
        private bool flashing;

        /// <summary>
        /// Completion delegate.
        /// </summary>
        private Action<string, object> completion;

        /// <summary>
        /// The context supplied to the completion delegate.
        /// </summary>
        private object context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroRecorder"/> class.
        /// </summary>
        public MacroRecorder()
        {
            this.flashTimer.Interval = FlashMs;
            this.flashTimer.Tick += this.DoFlash;
        }

        /// <summary>
        /// Running / stop running event.
        /// </summary>
        public event Action<bool> RunningEvent = (running) => { };

        /// <summary>
        /// Flash event.
        /// </summary>
        public event Action<bool> FlashEvent = (on) => { };

        /// <summary>
        /// Gets a value indicating whether the recorder is running.
        /// </summary>
        public bool Running => this.running;

        /// <summary>
        /// Starts recording.
        /// </summary>
        /// <param name="completion">Completion delegate.</param>
        /// <param name="context">Context to pass to delegate.</param>
        public void Start(Action<string, object> completion, object context = null)
        {
            if (!this.running)
            {
                this.completion = completion;
                this.context = context;
                this.running = true;
                this.actions.Clear();
                this.FlashEvent(true);
                this.flashing = true;
                this.flashTimer.Start();

                this.RunningEvent(true);
            }
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        public void Stop()
        {
            this.ProcessStop();
        }

        /// <summary>
        /// Records a set of actions.
        /// </summary>
        /// <param name="actions">Actions to record.</param>
        public void Record(string actions)
        {
            if (this.running)
            {
                this.actions.Append(actions + Environment.NewLine);
            }
        }

        /// <summary>
        /// Aborts any recording in progress.
        /// </summary>
        public void Abort()
        {
            this.ProcessStop(isAbort: true);
        }

        /// <summary>
        /// Extracts the Unicode value from a Key() action.
        /// </summary>
        /// <param name="action">Action to parse.</param>
        /// <returns>Unicode value.</returns>
        private static int KeyCode(string action)
        {
            return int.Parse(action.Substring(B3270.Action.Key.Length + 3, 4), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        /// <param name="isAbort">True if this is an abort rather than a graceful completion.</param>
        private void ProcessStop(bool isAbort = false)
        {
            if (this.running)
            {
                this.flashTimer.Stop();
                this.running = false;
                this.FlashEvent(false);
                this.flashing = false;

                if (!isAbort)
                {
                    this.completion?.Invoke(this.CookedActions(), this.context);
                }

                this.completion = null;
                this.context = null;

                this.RunningEvent(false);
            }
        }

        /// <summary>
        /// Returns a cooked version of the actions.
        /// </summary>
        /// <returns>Cooked actions.</returns>
        private string CookedActions()
        {
            var rawActions = this.actions.ToString();
            if (string.IsNullOrEmpty(rawActions))
            {
                return rawActions;
            }

            var splitActions = rawActions.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var keys = new List<string>();
            var cooked = new List<string>();

            // Dump any pending Key() actions.
            void DumpKeys()
            {
                if (keys.Count > 0)
                {
                    if (keys.Count == 1)
                    {
                        cooked.Add(keys[0]);
                    }
                    else
                    {
                        var s = new string(keys.Select(k => (char)KeyCode(k)).ToArray());
                        cooked.Add(B3270.Action.String + "(\"" + s + "\")");
                    }

                    keys.Clear();
                }
            }

            foreach (var action in splitActions)
            {
                if (action.StartsWith(B3270.Action.Key + "(") &&
                    KeyCode(action) >= 0x20 && KeyCode(action) != '"')
                {
                    // Accumulate a Key() action.
                    keys.Add(action);
                }
                else
                {
                    DumpKeys();
                    cooked.Add(action);
                }
            }

            DumpKeys();
            return string.Join(Environment.NewLine, cooked);
        }

        /// <summary>
        /// Processes a flash.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        private void DoFlash(object sender, EventArgs args)
        {
            this.FlashEvent(!this.flashing);
            this.flashing = !this.flashing;
        }
    }
}
