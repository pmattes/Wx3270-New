// <copyright file="ModelsDb.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handler for model indications from the back end.
    /// </summary>
    public class ModelsDb : BackEndEvent, IModelsDb
    {
        /// <summary>
        /// The dictionary of models.
        /// </summary>
        private readonly Dictionary<int, ModelDimensions> models = new Dictionary<int, ModelDimensions>();

        /// <summary>
        /// True if we are inside a models block.
        /// </summary>
        private bool running;

        /// <summary>
        /// True if we are done procesing the block.
        /// </summary>
        private bool done;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelsDb"/> class.
        /// </summary>
        public ModelsDb()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.Model, this.StartModel),
                new BackEndEventDef(B3270.Indication.Models, this.StartModels, this.EndModels),
            };
        }

        /// <summary>
        /// Event called when the database is complete.
        /// </summary>
        private event Action DoneEvent = () => { };

        /// <inheritdoc/>
        public IReadOnlyDictionary<int, ModelDimensions> Models => this.models;

        /// <inheritdoc/>
        public void AddDone(Action action)
        {
            if (this.done)
            {
                action();
            }
            else
            {
                this.DoneEvent += action;
            }
        }

        /// <inheritdoc/>
        public int? DefaultRows(int model)
        {
            return this.DefaultDimensions(model)?.Rows;
        }

        /// <inheritdoc/>
        public int? DefaultColumns(int model)
        {
            return this.DefaultDimensions(model)?.Columns;
        }

        /// <summary>
        /// Processes a model indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Indication attributes.</param>
        private void StartModel(string name, AttributeDict attributes)
        {
            if (this.running)
            {
                var model = int.Parse(attributes[B3270.Attribute.Model]);
                this.models[model] = new ModelDimensions(
                    model,
                    int.Parse(attributes[B3270.Attribute.Rows]),
                    int.Parse(attributes[B3270.Attribute.Columns]));
            }
        }

        /// <summary>
        /// Processes a models start indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Indication attributes.</param>
        private void StartModels(string name, AttributeDict attributes)
        {
            this.running = true;
        }

        /// <summary>
        /// Processes a models end indication.
        /// </summary>
        /// <param name="name">Indication name.</param>
        private void EndModels(string name)
        {
            this.running = false;
            this.done = true;
            this.DoneEvent();
        }

        /// <summary>
        /// Get the default dimensions for a model.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <returns>Default dimensions.</returns>
        private ModelDimensions DefaultDimensions(int model)
        {
            return this.Models.ContainsKey(model) ? this.Models[model] : null;
        }
    }
}
