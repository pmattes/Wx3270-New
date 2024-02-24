// <copyright file="FunctionLoader.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class to dynamically load functions from DLLs.
    /// </summary>
    public class FunctionLoader
    {
        /// <summary>
        /// Gets the dictionary of loaded libraries.
        /// </summary>
        private static ConcurrentDictionary<string, IntPtr> LoadedLibraries { get; } = new ConcurrentDictionary<string, IntPtr>();

        /// <summary>
        /// Loads a function from a DLL.
        /// </summary>
        /// <typeparam name="T">Delegate type.</typeparam>
        /// <param name="dllPath">DLL path.</param>
        /// <param name="functionName">Function name.</param>
        /// <returns>Delegate, or null.</returns>
        public static T LoadFunction<T>(string dllPath, string functionName)
        {
            // Normalize.
            if (!Path.IsPathRooted(dllPath))
            {
                dllPath = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "system32", dllPath);
            }

            // Get the library, cacheing the values.
            var hModule = LoadedLibraries.GetOrAdd(dllPath, (string dllPath) => LoadLibrary(dllPath));
            if (hModule == IntPtr.Zero)
            {
                return default;
            }

            // Load the function.
            var functionAddress = GetProcAddress(hModule, functionName);
            if (functionAddress == IntPtr.Zero)
            {
                return default;
            }

            // Return the delegate.
            return (T)(object)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("Kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }
}
