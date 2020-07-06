// <copyright file="FolderWatchConfigAction.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Wx3270.Contracts;

    /// <summary>
    /// A tree of profiles.
    /// </summary>
    public partial class ProfileTree
    {
        /// <summary>
        /// Class for undoing and redoing a folder watch.
        /// </summary>
        private class FolderWatchConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FolderWatchConfigAction"/> class.
            /// </summary>
            /// <param name="what">Type of operation</param>
            /// <param name="dirName">Directory name</param>
            /// <param name="profileTracker">Profile tracker</param>
            /// <param name="profileManager">Profile manager</param>
            public FolderWatchConfigAction(string what, string dirName, IProfileTracker profileTracker, IProfileManager profileManager)
                : base(what)
            {
                this.DirName = dirName;
                this.ProfileTracker = profileTracker;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the directory name.
            /// </summary>
            public string DirName { get; private set; }

            /// <summary>
            /// Gets the profile tracker.
            /// </summary>
            public IProfileTracker ProfileTracker { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an undo (versus a redo).
            /// </summary>
            public bool IsUndo { get; private set; }

            /// <summary>
            /// Undo or redo a folder watch.
            /// </summary>
            /// <param name="from">Stack to move from.</param>
            /// <param name="to">Stack to move to.</param>
            /// <param name="skipInvert">True to skip the inversion.</param>
            public override void Apply(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert)
            {
                if (!from.Any())
                {
                    return;
                }

                // Undo or redo the operation.
                var op = (FolderWatchConfigAction)from.Pop();
                if (op.IsUndo)
                {
                    // Undo the operation: Stop watching the folder.
                    if (op.DirName.Equals(Path.GetDirectoryName(this.ProfileManager.Current.PathName), StringComparison.InvariantCultureIgnoreCase))
                    {
                        ErrorBox.Show(I18n.Get(Message.CannotStopWatching), I18n.Get(Title.UndoWatchFolder));
                        return;
                    }

                    this.ProfileTracker.Unwatch(op.DirName);
                }
                else
                {
                    // Start watching the folder again.
                    this.ProfileManager.Refocus(DirNodeName(this.DirName));
                    this.ProfileTracker.Watch(op.DirName);
                }

                // Queue to the other stack, inverting the operation.
                if (!skipInvert)
                {
                    to.Push(new FolderWatchConfigAction(
                        this.What,
                        op.DirName,
                        this.ProfileTracker,
                        this.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}