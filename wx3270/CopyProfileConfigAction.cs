// <copyright file="CopyProfileConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a profile copy from one folder to another.
        /// </summary>
        private class CopyProfileConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CopyProfileConfigAction"/> class.
            /// </summary>
            /// <param name="what">Type of operation</param>
            /// <param name="profileName">Directory name</param>
            /// <param name="fromPath">Path to copy from</param>
            /// <param name="toPath">Path to copy to</param>
            /// <param name="profileManager">Profile manager</param>
            public CopyProfileConfigAction(string what, string profileName, string fromPath, string toPath, IProfileManager profileManager)
                : base(what)
            {
                this.ProfileName = profileName;
                this.FromPath = fromPath;
                this.ToPath = toPath;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
            }

            /// <summary>
            /// Gets the profile name.
            /// </summary>
            public string ProfileName { get; private set; }

            /// <summary>
            /// Gets the source path.
            /// </summary>
            public string FromPath { get; private set; }

            /// <summary>
            /// Gets the destination path.
            /// </summary>
            public string ToPath { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an undo (versus a redo).
            /// </summary>
            public bool IsUndo { get; private set; }

            /// <summary>
            /// Undo or redo a profile copy.
            /// </summary>
            /// <param name="from">Stack to move from</param>
            /// <param name="to">Stack to move to</param>
            /// <param name="skipInvert">True to skip inversion</param>
            public override void Apply(Stack<ConfigAction> from, Stack<ConfigAction> to, bool skipInvert)
            {
                if (!from.Any())
                {
                    return;
                }

                // Undo or redo the operation.
                var op = (CopyProfileConfigAction)from.Pop();
                var title = string.Empty;
                if (op.IsUndo)
                {
                    // Undo the operation: Remove the copy.
                    title = Title.UndoCopyProfile;
                    this.ProfileManager.Refocus(ProfileTree.DirNodeName(Path.GetDirectoryName(op.FromPath)), Path.GetFileNameWithoutExtension(op.FromPath));
                    try
                    {
                        File.Delete(op.ToPath);
                    }
                    catch (Exception e)
                    {
                        ErrorBox.Show(e.Message, I18n.Get(title));
                        return;
                    }
                }
                else
                {
                    // Copy the profile again.
                    title = Title.CopyProfile;
                    this.ProfileManager.Refocus(ProfileTree.DirNodeName(Path.GetDirectoryName(op.ToPath)), Path.GetFileNameWithoutExtension(op.ToPath));
                    try
                    {
                        File.Copy(op.FromPath, op.ToPath);
                    }
                    catch (Exception e)
                    {
                        ErrorBox.Show(e.Message, I18n.Get(title));
                        return;
                    }
                }

                if (!skipInvert)
                {
                    // Queue to the other stack, inverting the operation.
                    to.Push(new CopyProfileConfigAction(
                        this.What,
                        op.ProfileName,
                        this.FromPath,
                        this.ToPath,
                        this.ProfileManager)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
