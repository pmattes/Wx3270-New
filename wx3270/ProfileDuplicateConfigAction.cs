// <copyright file="ProfileDuplicateConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a profile duplication.
        /// </summary>
        public class ProfileDuplicateConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileDuplicateConfigAction"/> class.
            /// </summary>
            /// <param name="what">Description of operation.</param>
            /// <param name="original">Original profile name.</param>
            /// <param name="duplicate">Duplicate profile name.</param>
            /// <param name="profileManager">Profile manager.</param>
            public ProfileDuplicateConfigAction(string what, Profile original, string duplicate, IProfileManager profileManager)
                : base(what)
            {
                this.Original = original;
                this.Duplicate = duplicate;
                this.ProfileManager = profileManager;
                this.OriginalIsDefaults = original.Name.Equals(Wx3270.ProfileManager.DefaultValuesName);
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the original profile.
            /// </summary>
            public Profile Original { get; private set; }

            /// <summary>
            /// Gets the duplicate profile name.
            /// </summary>
            public string Duplicate { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the original profile is default values.
            /// </summary>
            public bool OriginalIsDefaults { get; private set; }

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
                var op = (ProfileDuplicateConfigAction)from.Peek();
                var title = string.Empty;
                try
                {
                    if (op.IsUndo)
                    {
                        // Reverse the operation: Delete the duplicate.
                        title = Title.UndoDuplicateProfile;
                        if (this.ProfileManager.Equals(op.Duplicate))
                        {
                            ErrorBox.Show(I18n.Get(Message.CannotDeleteCurrentProfile), I18n.Get(title));
                            return;
                        }

                        if (!op.OriginalIsDefaults)
                        {
                            this.ProfileManager.Refocus(op.Original.DisplayFolder, op.Original.Name);
                        }

                        File.Delete(op.Original.MappedPath(op.Duplicate));
                    }
                    else
                    {
                        title = Title.DuplicateProfile;
                        if (this.ProfileManager.IsCurrentPathName(op.Original.MappedPath(op.Duplicate)))
                        {
                            ErrorBox.Show(I18n.Get(Message.CannotOverwriteCurrentProfile), I18n.Get(title));
                            return;
                        }

                        // Re-do the operation: Copy the profile.
                        if (op.OriginalIsDefaults)
                        {
                            this.ProfileManager.Refocus(DefaultDirNodeName, op.Duplicate);
                            if (!this.ProfileManager.Save(op.Original.MappedPath(op.Duplicate), Profile.DefaultProfile))
                            {
                                ErrorBox.Show(I18n.Get(Message.ProfileSaveFailed), I18n.Get(title));
                                return;
                            }
                        }
                        else
                        {
                            this.ProfileManager.Refocus(op.Original.DisplayFolder, op.Duplicate);
                            File.Copy(op.Original.PathName, op.Original.MappedPath(op.Duplicate));
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
                    to.Push(new ProfileDuplicateConfigAction(
                        op.What,
                        op.Original,
                        op.Duplicate,
                        op.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
