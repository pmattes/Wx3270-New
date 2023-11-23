// <copyright file="IModelsDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handler for model indications from the back end.
    /// </summary>
    public interface IModelsDb
    {
        /// <summary>
        /// Gets the dictionary of models.
        /// </summary>
        public IReadOnlyDictionary<int, ModelDimensions> Models { get; }

        /// <summary>
        /// Adds (or immediately calls) a done action.
        /// </summary>
        /// <param name="action">Action to call.</param>
        public void AddDone(Action action);

        /// <summary>
        /// Returns the default number of rows for a given model.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <returns>Number of rows.</returns>
        public int? DefaultRows(int model);

        /// <summary>
        /// Returns the default number of columns for a given model.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <returns>Number of columns.</returns>
        public int? DefaultColumns(int model);
    }
}
