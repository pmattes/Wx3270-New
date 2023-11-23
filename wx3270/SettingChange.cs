// <copyright file="SettingChange.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>using System;

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wx3270.Contracts;

    /// <summary>
    /// Setting change processor.
    /// </summary>
    public class SettingChange : ISettingChange
    {
        /// <summary>
        /// The back end.
        /// </summary>
        private readonly IBackEnd backEnd;

        /// <summary>
        /// The known settings.
        /// </summary>
        private readonly HashSet<string> knownSettings;

        /// <summary>
        /// The registered handlers.
        /// </summary>
        private readonly List<FilteredHandler> handlers = new List<FilteredHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingChange"/> class.
        /// </summary>
        /// <param name="backEnd">Back end.</param>
        public SettingChange(IBackEnd backEnd)
        {
            this.backEnd = backEnd;
            this.knownSettings = new KnownSettings().Settings;
            backEnd.RegisterStart(B3270.Indication.Setting, this.StartSetting);
        }

        /// <inheritdoc />
        public SettingsDictionary SettingsDictionary { get; private set; } = new SettingsDictionary();

        /// <inheritdoc />
        public SettingsDictionary DefaultsDictionary { get; private set; } = new SettingsDictionary();

        /// <inheritdoc />
        public HashSet<string> UnknownSettings { get; private set; } = new HashSet<string>();

        /// <inheritdoc />
        public void Register(SettingChangeHandler handler, ICollection<string> filter = null)
        {
            this.handlers.Add(new FilteredHandler { Handler = handler, Filter = filter });
        }

        /// <summary>
        /// Process a start indication for a setting.
        /// </summary>
        /// <param name="name">Indication name.</param>
        /// <param name="attributes">Attribute dictionary.</param>
        private void StartSetting(string name, AttributeDict attributes)
        {
            var settingName = attributes[B3270.Attribute.Name];
            if (!attributes.TryGetValue(B3270.Attribute.Value, out string settingValue))
            {
                settingValue = string.Empty;
            }

            if (!attributes.TryGetValue(B3270.Attribute.Cause, out string cause))
            {
                cause = string.Empty;
            }

            if (!this.backEnd.Ready)
            {
                this.DefaultsDictionary.Add(settingName, settingValue);
                if (!this.knownSettings.Contains(settingName))
                {
                    this.UnknownSettings.Add(settingName);
                }
            }

            if (!this.SettingsDictionary.TryGetValue(settingName, out string value) || value != settingValue)
            {
                this.SettingsDictionary.Add(settingName, settingValue);

                // Do not reflect UI-caused settings back to the UI.
                if (!cause.Equals(B3270.Cause.Ui, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var handler in this.handlers.Where(h => h.Filter == null || h.Filter.Contains(settingName, StringComparer.InvariantCultureIgnoreCase)))
                    {
                        handler.Handler(settingName, this.SettingsDictionary);
                    }
                }
            }
        }

        /// <summary>
        /// Filtered setting change registration.
        /// </summary>
        private class FilteredHandler
        {
            /// <summary>
            /// Gets or sets the handler.
            /// </summary>
            public SettingChangeHandler Handler { get; set; }

            /// <summary>
            /// Gets or sets the filter.
            /// </summary>
            public ICollection<string> Filter { get; set; }
        }
    }
}
