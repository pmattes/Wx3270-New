// <copyright file="NewProfileConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a profile create.
        /// </summary>
        private class NewProfileConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NewProfileConfigAction"/> class.
            /// </summary>
            /// <param name="what">Operation description.</param>
            /// <param name="newName">New profile name.</param>
            /// <param name="newPath">New profile path.</param>
            /// <param name="profileManager">Profile manager.</param>
            public NewProfileConfigAction(string what, string newName, string newPath, IProfileManager profileManager)
                : base(what)
            {
                this.NewName = newName;
                this.NewPath = newPath;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the new profile name.
            /// </summary>
            public string NewName { get; private set; }

            /// <summary>
            /// Gets the new profile's path name.
            /// </summary>
            public string NewPath { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an undo.
            /// </summary>
            public bool IsUndo { get; private set; }

            /// <summary>
            /// Undo or redo a profile duplicate.
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
                var op = (NewProfileConfigAction)from.Peek();
                string title = string.Empty;
                try
                {
                    if (op.IsUndo)
                    {
                        // Reverse the operation: Delete the new profile.
                        title = Title.UndoCreateProfile;
                        this.ProfileManager.Refocus(ProfileTree.DirNodeName(Path.GetDirectoryName(op.NewPath))); // XXX: Not optimal.
                        File.Delete(op.NewPath);
                    }
                    else
                    {
                        // Re-do the operation: Copy the profile.
                        title = Title.CreateProfile;
                        this.ProfileManager.Refocus(ProfileTree.DirNodeName(Path.GetDirectoryName(op.NewPath)), op.NewName);
                        if (!this.ProfileManager.Save(op.NewPath, Profile.DefaultProfile))
                        {
                            ErrorBox.Show(I18n.Get(Message.ProfileSaveFailed), I18n.Get(Title.CreateProfile));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(title));
                }

                // Consume the operation.
                from.Pop();

                // Queue to the other stack, inverting the operation.
                if (!skipInvert)
                {
                    to.Push(new NewProfileConfigAction(op.What, op.NewName, op.NewPath, op.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
