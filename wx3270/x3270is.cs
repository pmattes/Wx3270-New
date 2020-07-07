// <copyright file="x3270is.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Microsoft.Win32;

    /// <summary>
    /// Interface logic for the x3270is DLL.
    /// </summary>
    public static class X3270is
    {
        /// <summary>
        /// The registry key.
        /// </summary>
        public const string RegistryKey = @"Software\x3270\x3270is";

        /// <summary>
        /// The value for the install directory.
        /// </summary>
        public const string InstallDirValue = "InstallDir";

        /// <summary>
        /// The name of the DLL.
        /// </summary>
        public const string DllName = "x3270is.dll";

        /// <summary>
        /// The program name.
        /// </summary>
        public const string ProgramName = "x3270is";

        /// <summary>
        /// Gets the path name of the DLL.
        /// </summary>
        public static string PathName
        {
            get
            {
                // Check for a copy installed with wx3270 itself.
                var name = Path.Combine(Application.StartupPath, DllName);
                if (File.Exists(name))
                {
                    return name;
                }

                // Check for an installed copy.
                var registryKey = Registry.LocalMachine.OpenSubKey(RegistryKey);
                if (registryKey != null)
                {
                    var installDir = registryKey.GetValue(InstallDirValue);
                    registryKey.Close();
                    if (installDir != null)
                    {
                        return Path.Combine((string)installDir, DllName);
                    }
                }

                // Guess.
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), ProgramName, DllName);
            }
        }
    }
}
