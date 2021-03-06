﻿// <copyright file="IActions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Key map actions interface.
    /// </summary>
    public interface IActions
    {
        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        string Actions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mapping is exact.
        /// </summary>
        bool Exact { get; set; }
    }
}