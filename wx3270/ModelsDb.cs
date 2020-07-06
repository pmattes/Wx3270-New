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
    public class ModelsDb : BackEndEvent
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
        /// Event that is signaled when the models list is ready.
        /// </summary>
        public event Action Done = () => { };

        /// <summary>
        /// Gets the dictionary of models.
        /// </summary>
        public IReadOnlyDictionary<int, ModelDimensions> Models => this.models;

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
            this.Done();
        }
    }
}
