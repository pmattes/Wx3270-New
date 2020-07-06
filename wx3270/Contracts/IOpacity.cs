// <copyright file="IOpacity.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;

    /// <summary>
    /// Interface that implements Clear.
    /// </summary>
    public interface IOpacity
    {
        /// <summary>
        /// Opacity changed.
        /// </summary>
        event Action<int> OpacityEvent;
    }
}
