// <copyright file="SettingsDictionary.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Concurrent;

    /// <summary>
    /// A dictionary of settings.
    /// </summary>
    public class SettingsDictionary
    {
        /// <summary>
        /// The raw dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> rawDictionary = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDictionary"/> class.
        /// </summary>
        public SettingsDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDictionary"/> class.
        /// </summary>
        /// <param name="other">Existing setting dictionary.</param>
        public SettingsDictionary(SettingsDictionary other)
        {
            this.rawDictionary = new ConcurrentDictionary<string, string>(other.rawDictionary);
        }

        /// <summary>
        /// Add an entry.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="value">Setting value.</param>
        public void Add(string name, string value)
        {
            this.rawDictionary[name] = value;
        }

        /// <summary>
        /// Add an entry.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="value">Setting value.</param>
        public void Add(string name, object value)
        {
            this.Add(name, value.ToString());
        }

        /// <summary>
        /// Try to get a string-valued entry.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="value">Returned value.</param>
        /// <returns>True if entry exists.</returns>
        public bool TryGetValue(string name, out string value)
        {
            return this.rawDictionary.TryGetValue(name, out value);
        }

        /// <summary>
        /// Try to get a Boolean-valued entry.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="value">Returned value.</param>
        /// <returns>True if entry exists.</returns>
        public bool TryGetValue(string name, out bool value)
        {
            value = false;
            if (!this.rawDictionary.TryGetValue(name, out string stringValue))
            {
                return false;
            }

            switch (stringValue.ToLowerInvariant())
            {
                case "true":
                case "set":
                case "on":
                case "1":
                    value = true;
                    return true;
                case "false":
                case "clear":
                case "off":
                case "0":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Try to get an integer-valued entry.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="value">Returned value.</param>
        /// <returns>True if entry exists.</returns>
        public bool TryGetValue(string name, out int value)
        {
            value = 0;
            return this.rawDictionary.TryGetValue(name, out string stringValue) && int.TryParse(stringValue, out value);
        }
    }
}
