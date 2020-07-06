// <copyright file="WatcherKey.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Win32;

    /// <summary>
    /// Profile watcher registry key manipulations.
    /// </summary>
    public static class WatcherKey
    {
        /// <summary>
        /// Gets the list of watched directories.
        /// </summary>
        public static IEnumerable<string> Directories
        {
            get
            {
                var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.WatchKey);
                var dirNames = key.GetValueNames().Select(name => key.GetValue(name)).Cast<string>().ToArray();
                key.Close();
                return dirNames;
            }
        }

        /// <summary>
        /// Watch a directory that isn't being watched yet.
        /// </summary>
        /// <param name="directory">Directory to watch.</param>
        public static void Add(string directory)
        {
            // Create a registry key.
            var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.WatchKey);
            if (!key.GetValueNames().Any(valueName => ((string)key.GetValue(valueName)).Equals(directory, StringComparison.OrdinalIgnoreCase)))
            {
                // No key yet. Add one.
                var n = 1;
                while (true)
                {
                    var subkeyName = Constants.Misc.WatchValue + n++;
                    if (key.GetValue(subkeyName) == null)
                    {
                        key.SetValue(subkeyName, directory);
                        break;
                    }
                }
            }

            key.Close();
        }

        /// <summary>
        /// Removes a directory from the list.
        /// </summary>
        /// <param name="directory">Directory to delete.</param>
        /// <returns>True if directory was deleted.</returns>
        public static bool Delete(string directory)
        {
            bool deleted = false;
            var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.WatchKey);
            foreach (var valueName in key.GetValueNames())
            {
                var regDir = (string)key.GetValue(valueName);
                if (regDir.Equals(directory, StringComparison.OrdinalIgnoreCase))
                {
                    key.DeleteValue(valueName);
                    deleted = true;
                    break;
                }
            }

            key.Close();
            return deleted;
        }

        /// <summary>
        /// Tests for the existence of a watch.
        /// </summary>
        /// <param name="directory">Directory to check.</param>
        /// <returns>True if watched.</returns>
        public static bool Exists(string directory)
        {
            return Directories.Any(dir => dir.Equals(directory, StringComparison.OrdinalIgnoreCase));
        }
    }
}
