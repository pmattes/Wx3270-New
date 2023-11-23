// <copyright file="ISettingChange.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>using System;

namespace Wx3270.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Setting change handler.
    /// </summary>
    /// <param name="name">Setting name.</param>
    /// <param name="settingDictionary">Setting dictionary (values).</param>
    public delegate void SettingChangeHandler(string name, SettingsDictionary settingDictionary);

    /// <summary>
    /// Setting change processor.
    /// </summary>
    public interface ISettingChange
    {
        /// <summary>
        /// Gets the settings dictionary.
        /// </summary>
        SettingsDictionary SettingsDictionary { get; }

        /// <summary>
        /// Gets the default settings dictionary.
        /// </summary>
        SettingsDictionary DefaultsDictionary { get; }

        /// <summary>
        /// Gets the set of unknown settings.
        /// </summary>
        HashSet<string> UnknownSettings { get; }

        /// <summary>
        /// Register a filtered setting handler.
        /// </summary>
        /// <param name="handler">Handler delegate.</param>
        /// <param name="filter">Filter strings.</param>
        void Register(SettingChangeHandler handler, ICollection<string> filter = null);
    }
}
