// <copyright file="IKeyHandler.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Windows.Forms;

    /// <summary>
    /// Keyboard input processing.
    /// </summary>
    public interface IKeyHandler
    {
        /// <summary>
        /// Process a keyboard message.
        /// </summary>
        /// <param name="msg">Message value.</param>
        /// <param name="keyData">Key data.</param>
        /// <returns>True if message was processed.</returns>
        bool CmdKey(Message msg, Keys keyData);

        /// <summary>
        /// Process a KeyDown event.
        /// </summary>
        /// <param name="shift">Shift/Alt processing interface.</param>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// This is where keyboard maps catch events.
        /// </remarks>
        void ProcessKeyDown(IShift shift, KeyEventArgs e);

        /// <summary>
        /// Process a KeyUp event.
        /// </summary>
        /// <param name="shift">Shift processing interface.</param>
        /// <param name="e">Event arguments.</param>
        void ProcessKeyUp(IShift shift, KeyEventArgs e);

        /// <summary>
        /// Process a KeyPress event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// This is where data keys (keys that generate input characters) end up if they are not caught by a keyboard map.
        /// </remarks>
        void ProcessKeyPress(KeyPressEventArgs e);

        /// <summary>
        /// Enter the form.
        /// </summary>
        /// <param name="shift">Shift interface.</param>
        void Enter(IShift shift);

        /// <summary>
        /// Process a window activation.
        /// </summary>
        void Activate();
    }
}
