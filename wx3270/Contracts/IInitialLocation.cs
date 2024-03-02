// <copyright file="IInitialLocation.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Drawing;

    /// <summary>
    /// Initial location interface.
    /// </summary>
    public interface IInitialLocation
    {
        /// <summary>
        /// Sets the initial location.
        /// </summary>
        Point? InitialLocation { set; }
    }
}
