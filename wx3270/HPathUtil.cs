// <copyright file="HPathUtil.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.IO;

    /// <summary>
    /// Path utilities.
    /// </summary>
    public static class HPathUtil
    {
        /// <summary>
        /// Normalizes a path.
        /// </summary>
        /// <param name="path">Path to normalize.</param>
        /// <returns>Normalized path.</returns>
        public static string Normalize(string path) => Path.GetFullPath(path).ToLowerInvariant();

        /// <summary>
        /// Compares two paths for equality.
        /// </summary>
        /// <param name="path1">First path.</param>
        /// <param name="path2">Second path.</param>
        /// <returns>True if equal.</returns>
        public static bool ArePathsEqual(string path1, string path2) => Normalize(path1) == Normalize(path2);
    }
}
