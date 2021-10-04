// <copyright file="ProfileTracker.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Tracks a tree of profiles and hosts.
    /// </summary>
    public class ProfileTracker : IProfileTracker
    {
        /// <summary>
        /// The name of titles, for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(ProfileTracker));

        /// <summary>
        /// The application context.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// The first directory to return in the tree.
        /// </summary>
        private readonly string firstDirectory;

        /// <summary>
        /// The file system watchers.
        /// </summary>
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        /// <summary>
        /// The set of explicitly watched folders.
        /// </summary>
        private readonly List<FolderWatchNode> roots = new List<FolderWatchNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileTracker"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="firstDirectory">First directory to return in the tree.</param>
        public ProfileTracker(Wx3270App app, string firstDirectory)
        {
            this.app = app;
            this.firstDirectory = firstDirectory;
        }

        /// <inheritdoc />
        public event ProfileTreeChanged ProfileTreeChanged = (newTree) => { };

        /// <inheritdoc />
        public List<FolderWatchNode> Tree
        {
            get => this.roots.OrderBy(n => n.PathName, new PrejudicedComparer(this.firstDirectory)).ToList();
        }

        /// <summary>
        /// Static initialization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.DirectoryWakError, "Directory Walk Error");
        }

        /// <inheritdoc />
        public void Watch(string directory)
        {
            // Check for a duplicate.
            if (this.IsDup(directory))
            {
                return;
            }

            // Do the initial directory scan.
            // We do this first, so that if we crash for some reason here, we do not leave a bomb sitting in
            // the registry for the next time we come up.
            this.Scan(directory);

            // Create a file system watcher.
            if (this.CreateWatcher(directory))
            {
                WatcherKey.Add(directory);
            }
        }

        /// <inheritdoc />
        public void WatchOthers()
        {
            foreach (var directory in WatcherKey.Directories)
            {
                if (this.CreateWatcher(directory))
                {
                    this.Scan(directory);
                }
            }
        }

        /// <inheritdoc />
        public bool Unwatch(string directory)
        {
            var watcher = this.watchers.FirstOrDefault(w => w.Path.Equals(directory, StringComparison.InvariantCultureIgnoreCase));
            if (watcher == null)
            {
                return false;
            }

            // Get rid of the watcher.
            this.watchers.Remove(watcher);
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            // Propagate the change up.
            var root = this.roots.OfType<FolderWatchNode>().Where(n => n.PathName.Equals(directory, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            this.roots.Remove(root);
            this.ProfileTreeChanged(this.Tree);

            // Remove it from the registry.
            return WatcherKey.Delete(directory);
        }

        /// <inheritdoc />
        public bool IsWatched(string dirPath)
        {
            foreach (var watchedDir in this.roots.Select(n => n.PathName))
            {
                if (watchedDir.Equals(dirPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                foreach (var subdir in Directory.EnumerateDirectories(watchedDir, "*", SearchOption.AllDirectories))
                {
                    if (subdir.Equals(dirPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Create a watcher for a directory.
        /// </summary>
        /// <param name="directory">Directory to watch.</param>
        /// <returns>True if watcher was created.</returns>
        private bool CreateWatcher(string directory)
        {
            if (Directory.Exists(directory) && !this.watchers.Select(w => w.Path).Contains(directory, StringComparer.InvariantCultureIgnoreCase))
            {
                // Create a file system watcher.
                var watcher = new FileSystemWatcher(directory, "*")
                {
                    IncludeSubdirectories = true,
                };
                watcher.Created += (sender, e) => this.Scan(directory);
                watcher.Changed += (sender, e) => this.Scan(directory);
                watcher.Renamed += (sender, e) => this.Scan(directory);
                watcher.Deleted += (sender, e) => this.Scan(directory);
                watcher.EnableRaisingEvents = true;
                this.watchers.Add(watcher);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check for a duplicate path.
        /// </summary>
        /// <param name="directory">Directory name.</param>
        /// <returns>True if duplicate.</returns>
        private bool IsDup(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            var path = Path.GetFullPath(directory);
            foreach (var root in this.roots)
            {
                if (root.Any((node) => node != root && node is FolderWatchNode folderNode && folderNode.PathName.Equals(path, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Scan the directory.
        /// </summary>
        /// <param name="directory">Directory name.</param>
        /// <param name="reRoot">Accumulating root.</param>
        private void Scan(string directory, FolderWatchNode reRoot = null)
        {
            if (reRoot == null)
            {
                // Seed a recursive walk.
                reRoot = new FolderWatchNode();
                this.Scan(directory, reRoot);
                reRoot = reRoot.Children.OfType<FolderWatchNode>().FirstOrDefault();

                // Compare the accumulated result against what we have right now.
                var current = this.roots.Where(n => n.PathName.Equals(directory, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (reRoot == null && current == null)
                {
                    return;
                }

                if (((reRoot == null) ^ (current == null)) || !current.TreeEquals(reRoot))
                {
                    if (current != null)
                    {
                        this.roots.Remove(current);
                    }

                    this.roots.Add(reRoot);
                    this.ProfileTreeChanged(this.Tree);
                }

                return;
            }

            var newFolder = new FolderWatchNode { Name = Path.GetFileName(directory), PathName = directory };
            reRoot.Add(newFolder);

            // Recursively walk the subdirectories in this directory.
            try
            {
                foreach (var subdir in Directory.EnumerateDirectories(directory, "*"))
                {
                    this.Scan(subdir, newFolder);
                }
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.DirectoryWakError));
                return;
            }

            // Walk the profiles in this directory.
            foreach (var profilePath in Directory.EnumerateFiles(directory, "*" + ProfileManager.Suffix))
            {
                if (profilePath.Equals(this.app.ProfileManager.Current.PathName))
                {
                    // Get the local copy instead of reading it.
                    newFolder.Add(
                        new ProfileWatchNode(this.app.ProfileManager.Current.Hosts.Select(h => new HostWatchNode(h.Name != string.Empty ? h.Name : h.Host, h.AutoConnect != AutoConnect.None, h)).ToList())
                        {
                            Name = Path.GetFileNameWithoutExtension(profilePath),
                            PathName = profilePath,
                            Profile = this.app.ProfileManager.Current,
                        });
                    continue;
                }

                var profile = ProfileManager.Read(profilePath, out _, out _, out _, out bool notFound);
                if (profile != null)
                {
                    // Add the new profile, with the hosts it contains.
                    newFolder.Add(
                        new ProfileWatchNode(profile.Hosts.Select(h => new HostWatchNode(h.Name != string.Empty ? h.Name : h.Host, h.AutoConnect != AutoConnect.None, h)).ToList())
                        {
                            Name = Path.GetFileNameWithoutExtension(profilePath),
                            PathName = profilePath,
                            Profile = profile,
                        });
                }
                else if (!notFound)
                {
                    // Add a broken profile.
                    newFolder.Add(new ProfileWatchNode
                    {
                        Name = Path.GetFileNameWithoutExtension(profilePath),
                        PathName = profilePath,
                        Broken = true,
                    });
                }
            }
        }

        /// <summary>
        /// Prejudiced string comparer.
        /// </summary>
        private class PrejudicedComparer : IComparer<string>
        {
            /// <summary>
            /// The string that is always first.
            /// </summary>
            private readonly string first;

            /// <summary>
            /// Initializes a new instance of the <see cref="PrejudicedComparer"/> class.
            /// </summary>
            /// <param name="first">String that is always first.</param>
            public PrejudicedComparer(string first)
            {
                this.first = first;
            }

            /// <summary>
            /// Prejudicially compares two strings.
            /// </summary>
            /// <param name="x">First string.</param>
            /// <param name="y">Second string.</param>
            /// <returns>Comparison value.</returns>
            public int Compare(string x, string y)
            {
                if (x.Equals(y, StringComparison.InvariantCultureIgnoreCase))
                {
                    return 0;
                }

                if (this.first != null)
                {
                    if (x.Equals(this.first, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return -1;
                    }

                    if (y.Equals(this.first, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return 1;
                    }
                }

                return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Default profile change.
            /// </summary>
            public static readonly string DirectoryWakError = I18n.Combine(TitleName, "directoryWalkError");
        }
    }
}
