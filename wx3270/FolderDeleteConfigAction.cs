// <copyright file="FolderDeleteConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a folder delete.
        /// </summary>
        private class FolderDeleteConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FolderDeleteConfigAction"/> class.
            /// </summary>
            /// <param name="what">Description of operation.</param>
            /// <param name="path">Folder path.</param>
            /// <param name="undoFocusName">Name to focus on after undo.</param>
            /// <param name="redoFocusName">Name to focus on after redo.</param>
            /// <param name="profileManager">Profile manager.</param>
            public FolderDeleteConfigAction(string what, string path, string undoFocusName, string redoFocusName, IProfileManager profileManager)
                : base(what)
            {
                this.Path = path;
                this.UndoFocusName = undoFocusName;
                this.RedoFocusName = redoFocusName;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the profile.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Gets the undo focus name.
            /// </summary>
            public string UndoFocusName { get; private set; }

            /// <summary>
            /// Gets the redo focus name.
            /// </summary>
            public string RedoFocusName { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an undo.
            /// </summary>
            public bool IsUndo { get; private set; }

            /// <summary>
            /// Undo or redo a profile delete.
            /// </summary>
            /// <param name="from">Stack to move from.</param>
            /// <param name="to">Stack to move to.</param>
            /// <param name="skipInvert">True to skip inversion.</param>
            public override void Apply(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert)
            {
                if (!from.Any())
                {
                    return;
                }

                // Undo or redo the operation.
                var op = (FolderDeleteConfigAction)from.Peek();
                var title = op.IsUndo ? Title.UndoDeleteFolder : Title.DeleteFolder;

                from.Pop();
                if (op.IsUndo)
                {
                    // Reverse the operation: Restore the folder.
                    try
                    {
                        Directory.CreateDirectory(op.Path);
                    }
                    catch (Exception ex)
                    {
                        ErrorBox.Show(ex.Message, I18n.Get(title));
                        return;
                    }

                    this.ProfileManager.Refocus(op.UndoFocusName);
                }
                else
                {
                    // Re-do the operation: Delete the folder.
                    try
                    {
                        Directory.Delete(op.Path);
                    }
                    catch (Exception ex)
                    {
                        ErrorBox.Show(ex.Message, I18n.Get(title));
                        return;
                    }

                    this.ProfileManager.Refocus(op.RedoFocusName);
                }

                // Queue to the other stack, inverting the operation.
                if (!skipInvert)
                {
                    to.Push(new FolderDeleteConfigAction(this.What, op.Path, op.UndoFocusName, op.RedoFocusName, op.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
