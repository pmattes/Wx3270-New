// <copyright file="FakeRegistry.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using Newtonsoft.Json.Linq;
    using Wx3270.Contracts;

    /// <summary>
    /// Interface to the fake registry.
    /// </summary>
    public class FakeRegistry : ISimplifiedRegistry
    {
        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <param name="baseName">Optional base name.</param>
        /// <returns>Database path.</returns>
        public static string GetPath(string baseName = null) => FakeRegistryKey.GetPath(baseName);

        /// <inheritdoc/>
        public ISimplifiedRegistryKey CurrentUserCreateSubKey(string name, string baseName = null)
        {
            return new FakeRegistryKey(name, baseName);
        }

        /// <summary>
        /// Registry key class that uses a fake registry.
        /// </summary>
        /// <remarks>
        /// The fake registry is a simple JSON file that contains one object.
        /// Its keys are the sub-key names.
        /// Its values are also objects. Their keys are the value names.
        /// It only supports strings.
        /// </remarks>
        private class FakeRegistryKey : ISimplifiedRegistryKey
        {
            /// <summary>
            /// The file name.
            /// </summary>
            private const string JsonFileName = "registry.json";

            /// <summary>
            /// The file path.
            /// </summary>
            private readonly string path;

            /// <summary>
            /// The key name.
            /// </summary>
            private string key;

            /// <summary>
            /// Initializes a new instance of the <see cref="FakeRegistryKey"/> class.
            /// </summary>
            /// <param name="name">Sub-key name.</param>
            /// <param name="baseName">Optional base name.</param>
            public FakeRegistryKey(string name, string baseName)
            {
                this.key = name;
                this.path = GetPath(baseName);
            }

            /// <summary>
            /// Gets the path of the database file.
            /// </summary>
            /// <param name="baseName">Optional base name.</param>
            /// <returns>Path of file.</returns>
            public static string GetPath(string baseName = null) => Path.Combine(baseName ?? Application.StartupPath, JsonFileName);

            /// <summary>
            /// Disposes the key.
            /// </summary>
            public void Dispose()
            {
                // Close the file.
                this.key = null;
            }

            /// <inheritdoc/>
            public object GetValue(string name)
            {
                // Return the sub-key if it exists.
                if (!File.Exists(this.path))
                {
                    return null;
                }

                var database = JObject.Parse(File.ReadAllText(this.path));
                if (!database.ContainsKey(this.key))
                {
                    return null;
                }

                var key = database.GetValue(this.key);
                if (key.Type != JTokenType.Object)
                {
                    return null;
                }

                var values = (JObject)key;
                if (!values.ContainsKey(name))
                {
                    return null;
                }

                var value = values.GetValue(name);
                if (value.Type != JTokenType.String)
                {
                    return null;
                }

                return ((JValue)value).Value;
            }

            /// <inheritdoc/>
            public void SetValue(string name, object value)
            {
                if (!(value is string stringValue))
                {
                    throw new ArgumentException("Fake registry only supports strings");
                }

                // Get the current database.
                var database = File.Exists(this.path) ? JObject.Parse(File.ReadAllText(this.path)) : new JObject();

                // Fetch or create the key.
                JObject keyObject;
                var keyExists = false;
                if (database.ContainsKey(this.key))
                {
                    keyObject = (JObject)database.GetValue(this.key);
                    keyExists = true;
                }
                else
                {
                    keyObject = new JObject();
                }

                // Add the value to the key.
                if (keyObject.ContainsKey(name))
                {
                    keyObject.Remove(name);
                }

                keyObject.Add(name, new JValue(stringValue));

                // Add the key to the database.
                if (keyExists)
                {
                    database.Remove(this.key);
                }

                database.Add(this.key, keyObject);

                // Rewrite the file.
                File.WriteAllText(this.path, database.ToString());
            }

            /// <inheritdoc/>
            public void DeleteValue(string name)
            {
                if (!File.Exists(this.path))
                {
                    return;
                }

                var database = JObject.Parse(File.ReadAllText(this.path));
                if (!database.ContainsKey(this.key))
                {
                    return;
                }

                var keyObject = (JObject)database.GetValue(this.key);
                if (keyObject.ContainsKey(name))
                {
                    keyObject.Remove(name);
                    database.Remove(this.key);
                    database.Add(this.key, keyObject);
                    File.WriteAllText(this.path, database.ToString());
                }
            }

            /// <inheritdoc/>
            public string[] GetValueNames()
            {
                if (!File.Exists(this.path))
                {
                    return new string[] { };
                }

                var database = JObject.Parse(File.ReadAllText(this.path));
                if (!database.ContainsKey(this.key))
                {
                    return new string[] { };
                }

                var ret = new List<string>();
                var keyValue = (JObject)database.GetValue(this.key);
                foreach (var value in keyValue)
                {
                    ret.Add(value.Key);
                }

                return ret.ToArray();
            }

            /// <summary>
            /// Closes the key.
            /// </summary>
            public void Close()
            {
            }
        }
    }
}