// <copyright file="IElement.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Delegate for processing an XML element start.
    /// </summary>
    /// <param name="name">Element name.</param>
    /// <param name="attributes">Attribute dictionary.</param>
    public delegate void StartDelegate(string name, AttributeDict attributes);

    /// <summary>
    /// Delegate for processing an XML element end.
    /// </summary>
    /// <param name="name">Element name.</param>
    public delegate void EndDelegate(string name);

    /// <summary>
    /// Element processing interface.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Register a start method.
        /// </summary>
        /// <param name="name">XML element name.</param>
        /// <param name="d">Method to invoke.</param>
        void RegisterStart(string name, StartDelegate d);

        /// <summary>
        /// Register an end method.
        /// </summary>
        /// <param name="name">XML element name.</param>
        /// <param name="d">Method to invoke.</param>
        void RegisterEnd(string name, EndDelegate d);
    }
}