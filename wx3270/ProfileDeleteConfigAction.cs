// <copyright file="ProfileDeleteConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a profile delete.
        /// </summary>
        private class ProfileDeleteConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileDeleteConfigAction"/> class.
            /// </summary>
            /// <param name="what">Description of operation.</param>
            /// <param name="profile">Profile to save.</param>
            /// <param name="nextNodeName">Name of next node, for refocus on redo.</param>
            /// <param name="profileManager">Profile manager.</param>
            public ProfileDeleteConfigAction(string what, Profile profile, string nextNodeName, IProfileManager profileManager)
                : base(what)
            {
                this.Profile = profile;
                this.NextNodeName = nextNodeName;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the profile.
            /// </summary>
            public Profile Profile { get; private set; }

            /// <summary>
            /// Gets the next node name.
            /// </summary>
            public string NextNodeName { get; private set; }

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
                var op = (ProfileDeleteConfigAction)from.Peek();
                var title = op.IsUndo ? Title.UndoDeleteProfile : Title.DeleteProfile;

                // Make sure it is safe.
                if (op.ProfileManager.IsCurrentPathName(op.Profile.PathName))
                {
                    ErrorBox.Show(I18n.Get(Message.CannotDeleteCurrentProfile), I18n.Get(title));
                    return;
                }

                from.Pop();
                if (op.IsUndo)
                {
                    // Reverse the operation: Restore the profile.
                    this.ProfileManager.Refocus(op.Profile.DisplayFolder, op.Profile.Name);
                    this.ProfileManager.Save(op.Profile.PathName, op.Profile);
                }
                else
                {
                    // Re-do the operation: Delete the profile.
                    this.ProfileManager.Refocus(op.Profile.DisplayFolder, op.NextNodeName);
                    try
                    {
                        File.Delete(op.Profile.PathName);
                    }
                    catch (Exception ex)
                    {
                        ErrorBox.Show(ex.Message, I18n.Get(title));
                        return;
                    }
                }

                // Queue to the other stack, inverting the operation.
                if (!skipInvert)
                {
                    to.Push(new ProfileDeleteConfigAction(this.What, op.Profile, op.NextNodeName, op.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
