// <copyright file="BackEndEventDef.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Back-end event handler definition.
    /// </summary>
    public class BackEndEventDef
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackEndEventDef"/> class.
        /// </summary>
        /// <param name="element">Element name.</param>
        /// <param name="startHandler">Handler for start indications.</param>
        /// <param name="endHandler">Handler for end indications.</param>
        public BackEndEventDef(string element, StartDelegate startHandler, EndDelegate endHandler = null)
        {
            this.Element = element;
            this.StartHandler = startHandler;
            this.EndHandler = endHandler;
        }

        /// <summary>
        /// Gets the element name.
        /// </summary>
        public string Element { get; private set; }

        /// <summary>
        /// Gets the start handler.
        /// </summary>
        public StartDelegate StartHandler { get; private set; }

        /// <summary>
        /// Gets the end handler.
        /// </summary>
        public EndDelegate EndHandler { get; private set; }
    }
}
