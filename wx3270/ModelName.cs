// <copyright file="ModelName.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Model name class.
    /// </summary>
    public class ModelName
    {
        /// <summary>
        /// The regular expression for parsing model names.
        /// </summary>
        private const string ModelRegex = @"(?i)^(IBM-)?(327(?<color>[89])-)?(?<model>[2345])(-(?<extended>E))?$";

        /// <summary>
        /// Gets or sets the full model name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the model number.
        /// </summary>
        public int ModelNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether color is supported.
        /// </summary>
        public bool Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 3270 extended mode is supported.
        /// </summary>
        public bool Extended { get; set; }

        /// <summary>
        /// Parses a string into a <see cref="ModelName"/>.
        /// </summary>
        /// <param name="name">Full or partial model name.</param>
        /// <param name="modelName">Returned <see cref="ModelName"/>.</param>
        /// <returns>True if name parsed successfully.</returns>
        public static bool TryParse(string name, out ModelName modelName)
        {
            var regex = new Regex(ModelRegex);
            var m = regex.Match(name);
            if (!m.Success)
            {
                modelName = null;
                return false;
            }

            modelName = new ModelName
            {
                Name = name,
                ModelNumber = m.Groups["model"].Success ? int.Parse(m.Groups["model"].Value) : Profile.DefaultProfile.Model,
                Color = !m.Groups["color"].Success || m.Groups["color"].Value == "9",
                Extended = true,
            };
            return true;
        }

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "327" + (this.Color ? "9" : "8") + "-" + this.ModelNumber + (this.Extended ? "-E" : string.Empty);
        }
    }
}
