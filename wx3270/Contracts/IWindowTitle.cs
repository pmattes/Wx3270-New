// <copyright file="IWindowTitle.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Window title processor.
    /// </summary>
    public interface IWindowTitle : IBackEndEvent
    {
        /// <summary>
        /// Gets the programmatically-defined window title.
        /// </summary>
        string Title { get; }
    }
}
