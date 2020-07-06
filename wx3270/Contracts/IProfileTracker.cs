// <copyright file="IProfileTracker.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Delegate for profile tree changes.
    /// </summary>
    /// <param name="newTree">New profile tree</param>
    public delegate void ProfileTreeChanged(List<FolderWatchNode> newTree);

    /// <summary>
    /// Tracks a tree of profiles and hosts.
    /// </summary>
    public interface IProfileTracker
    {
        /// <summary>
        /// The change event for the host tree.
        /// </summary>
        event ProfileTreeChanged ProfileTreeChanged;

        /// <summary>
        /// Gets the current tree.
        /// </summary>
        List<FolderWatchNode> Tree { get; }

        /// <summary>
        /// Watch a directory for profiles being moved around or edited.
        /// </summary>
        /// <param name="directory">Directory to watch</param>
        void Watch(string directory);

        /// <summary>
        /// Watch directories listed in the registry.
        /// </summary>
        void WatchOthers();

        /// <summary>
        /// Stop watching a directory.
        /// </summary>
        /// <param name="directory">Directory name</param>
        /// <returns>True if folder was watched</returns>
        bool Unwatch(string directory);

        /// <summary>
        /// Checks if a directory is a subdirectory of a watched folder.
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <returns>True if it is a watched subdirectory</returns>
        bool IsWatched(string dirPath);
    }
}
