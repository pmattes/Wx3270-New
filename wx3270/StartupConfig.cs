// <copyright file="StartupConfig.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// Start-up configuration.
    /// </summary>
    public class StartupConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupConfig"/> class.
        /// </summary>
        public StartupConfig()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to start tracing immediately.
        /// </summary>
        public bool Trace { get; set; }

        /// <summary>
        /// Gets or sets the script port for the script port option.
        /// </summary>
        public string ScriptPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exit after the controlling script exits.
        /// </summary>
        public bool ScriptPortOnce { get; set; }

        /// <summary>
        /// Gets or sets the HTTP listen spec.
        /// </summary>
        public string Httpd { get; set; }

        /// <summary>
        /// Gets or sets the model number.
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 3279 color mode is supported.
        /// </summary>
        public bool ColorMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether extended mode is supported.
        /// </summary>
        public bool ExtendedMode { get; set; }

        /// <summary>
        /// Gets or sets the oversize rows.
        /// </summary>
        public int OversizeRows { get; set; }

        /// <summary>
        /// Gets or sets the oversize columns.
        /// </summary>
        public int OversizeColumns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the UI is in portable mode.
        /// </summary>
        public bool Portable { get; set; }

        /// <summary>
        /// Gets the derived model parameter.
        /// </summary>
        public string ModelParameter => string.Format(
            "327{0}-{1}{2}",
            this.ColorMode ? "9" : "8",
            this.Model,
            this.ExtendedMode ? "-E" : string.Empty);

        /// <summary>
        /// Gets the derived oversize parameter.
        /// </summary>
        public string OversizeParameter => (this.OversizeRows != 0 || this.OversizeColumns != 0) ?
            string.Format("{0}x{1}", this.OversizeColumns, this.OversizeRows) :
            string.Empty;

        /// <summary>
        /// Merge fields from a profile into a startup config.
        /// </summary>
        /// <param name="profile">Profile to merge</param>
        public void MergeProfile(Profile profile)
        {
            this.Model = profile.Model;
            this.ColorMode = profile.ColorMode;
            this.ExtendedMode = profile.ExtendedMode;
            this.OversizeRows = profile.Oversize.Rows;
            this.OversizeColumns = profile.Oversize.Columns;
        }
    }
}
