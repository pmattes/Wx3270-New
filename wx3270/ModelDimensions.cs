// <copyright file="ModelDimensions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    using I18nBase;

    /// <summary>
    /// Model screen dimensions class.
    /// </summary>
    public class ModelDimensions : Tuple<int, int, int>
    {
        /// <summary>
        /// Name of the model dimensions string.
        /// </summary>
        private static readonly string ModelDimensionsFormat = I18n.Combine(nameof(Settings), "modelDimensions");

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensions"/> class.
        /// </summary>
        /// <param name="model">Model number.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        public ModelDimensions(int model, int rows, int columns)
            : base(model, rows, columns)
        {
        }

        /// <summary>
        /// Gets the model number.
        /// </summary>
        public int Model
        {
            get
            {
                return this.Item1;
            }
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int Rows
        {
            get
            {
                return this.Item2;
            }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int Columns
        {
            get
            {
                return this.Item3;
            }
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(ModelDimensionsFormat, "Model {0} ({1} rows, {2} columns)");
        }

        /// <summary>
        /// Converts a <see cref="ModelDimensions"/> to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return string.Format(I18n.Get(ModelDimensionsFormat), this.Model, this.Rows, this.Columns);
        }
    }
}
