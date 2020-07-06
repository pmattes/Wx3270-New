// <copyright file="ProfileRenameConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a profile rename.
        /// </summary>
        private class ProfileRenameConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileRenameConfigAction"/> class.
            /// </summary>
            /// <param name="what">Type of operation.</param>
            /// <param name="newName">New name.</param>
            /// <param name="profile">Existing profile.</param>
            /// <param name="profileManager">Profile manager.</param>
            public ProfileRenameConfigAction(string what, string newName, Profile profile, IProfileManager profileManager)
                : base(what)
            {
                this.Profile = profile;
                this.NewName = newName;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the new name.
            /// </summary>
            public string NewName { get; private set; }

            /// <summary>
            /// Gets the profile.
            /// </summary>
            public Profile Profile { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an undo (versus a redo).
            /// </summary>
            public bool IsUndo { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Undo or redo a rename.
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
                var op = (ProfileRenameConfigAction)from.Pop();
                var title = op.IsUndo ? Title.UndoRenameProfile : Title.RenameProfile;
                try
                {
                    // Make sure it is safe.
                    if (this.ProfileManager.IsCurrentPathName(op.Profile.PathName)
                        || this.ProfileManager.IsCurrentPathName(op.Profile.MappedPath(op.NewName)))
                    {
                        ErrorBox.Show(I18n.Get(Message.CannotRenameCurrentProfile), I18n.Get(title));
                        return;
                    }

                    if (op.IsUndo)
                    {
                        // Reverse the operation: Rename NewName to OldName.
                        this.ProfileManager.Refocus(op.Profile.DisplayFolder, op.Profile.Name);
                        File.Move(op.Profile.MappedPath(op.NewName), op.Profile.PathName);
                    }
                    else
                    {
                        // Re-do the operation: Rename OldName to NewName.
                        this.ProfileManager.Refocus(op.Profile.DisplayFolder, op.NewName);
                        File.Move(op.Profile.PathName, op.Profile.MappedPath(op.NewName));
                    }
                }
                catch (Exception ex)
                {
                    ErrorBox.Show(ex.Message, I18n.Get(title));
                    return;
                }

                // Queue to the other stack, inverting the operation.
                if (!skipInvert)
                {
                    to.Push(new ProfileRenameConfigAction(
                        op.What,
                        op.NewName,
                        op.Profile,
                        this.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
