// <copyright file="IBackEnd.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Delegate for completion of a back-end command.
    /// </summary>
    /// <param name="cookie">Context passed to <see cref="IBackEnd.RunActions"/> call.</param>
    /// <param name="success">True if command succeeded.</param>
    /// <param name="result">Result string, might be empty.</param>
    public delegate void BackEndCompletion(object cookie, bool success, string result);

    /// <summary>
    /// Delegate for a pass-through action called because of a UI action.
    /// </summary>
    /// <param name="cookie">Cookie passed.</param>
    /// <param name="passthruName">Name of pass-through action.</param>
    public delegate void NestedPassthru(object cookie, string passthruName);

    /// <summary>
    /// Delegate for a pass-through action.
    /// </summary>
    /// <param name="commandName">Command name.</param>
    /// <param name="arguments">List of arguments.</param>
    /// <param name="result">Returned result.</param>
    /// <param name="tag">Tag for asynchronous completion.</param>"
    /// <returns>True if action succeeded.</returns>
    public delegate PassthruResult Passthru(string commandName, IEnumerable<string> arguments, out string result, string tag);

    /// <summary>
    /// The result of a pass-through operation.
    /// </summary>
    public enum PassthruResult
    {
        /// <summary>
        /// The operation succeeded.
        /// </summary>
        Success,

        /// <summary>
        /// The operation failed.
        /// </summary>
        Failure,

        /// <summary>
        /// The operation will complete asynchronously.
        /// </summary>
        Pending,
    }

    /// <summary>
    /// Emulator back end, using b3270.
    /// </summary>
    public interface IBackEnd : IElement
    {
        /// <summary>
        /// Actions to perform prior to exiting.
        /// </summary>
        event Action OnExit;

        /// <summary>
        /// Actions to perform when the emulator is ready.
        /// </summary>
        event Action OnReady;

        /// <summary>
        /// Print a debug message.
        /// </summary>
        /// <param name="format">String format.</param>
        /// <param name="args">Optional arguments.<./param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// Run the emulator.
        /// </summary>
        void Run();

        /// <summary>
        /// Stop the back end.
        /// </summary>
        void Stop();

        /// <summary>
        /// Register a start handler for a back-end event.
        /// </summary>
        /// <param name="e">Back-end event.</param>
        void Register(IBackEndEvent e);

        /// <summary>
        /// Run some actions.
        /// </summary>
        /// <param name="actions">Actions to perform.</param>
        /// <param name="type">Type of actions.</param>
        /// <param name="completion">Completion delegate.</param>
        /// <param name="cookie">Completion cookie.</param>
        /// <param name="nestedPassthru">Nested pass-through delegate.</param>
        void RunActions(string actions, string type, BackEndCompletion completion, object cookie = null, NestedPassthru nestedPassthru = null);

        /// <summary>
        /// Run some actions.
        /// </summary>
        /// <param name="actions">Actions to perform.</param>
        /// <param name="completion">Completion delegate.</param>
        /// <param name="cookie">Completion cookie.</param>
        /// <param name="nestedPassthru">Nested pass-through delegate.</param>
        void RunActions(string actions, BackEndCompletion completion, object cookie = null, NestedPassthru nestedPassthru = null);

        /// <summary>
        /// Run an action.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        /// <param name="completion">Completion delegate.</param>
        /// <param name="cookie">Completion cookie.</param>
        void RunAction(BackEndAction action, BackEndCompletion completion, object cookie = null);

        /// <summary>
        /// Run an action.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        /// <param name="type">Action type.</param>
        /// <param name="completion">Completion delegate.</param>
        /// <param name="cookie">Completion cookie.</param>
        void RunAction(BackEndAction action, string type, BackEndCompletion completion, object cookie = null);

        /// <summary>
        /// Run some actions.
        /// </summary>
        /// <param name="actions">Actions to perform.</param>
        /// <param name="completion">Completion delegate.</param>
        /// <param name="cookie">Completion cookie.</param>
        void RunActions(IEnumerable<BackEndAction> actions, BackEndCompletion completion, object cookie = null);

        /// <summary>
        /// Register a pass-through action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="action">Action delegate.</param>
        /// <param name="helpText">Help text.</param>
        /// <param name="helpParams">Help parameter text.</param>
        void RegisterPassthru(string commandName, Passthru action, string helpText = null, string helpParams = null);

        /// <summary>
        /// Pass-through completion.
        /// </summary>
        /// <param name="success">True if action succeeded.</param>
        /// <param name="result">Result string.</param>
        /// <param name="tag">Tag from emulator.</param>
        void PassthruComplete(bool success, string result, string tag);

        /// <summary>
        /// Trace some text.
        /// </summary>
        /// <param name="text">Text to trace.</param>
        void Trace(string text);

        /// <summary>
        /// Exit the application.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        void Exit(int errorCode);
    }
}
