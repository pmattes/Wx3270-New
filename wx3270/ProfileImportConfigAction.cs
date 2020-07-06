// <copyright file="ProfileImportConfigAction.cs" company="Paul Mattes">
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
        /// Class for undoing and redoing a profile import.
        /// </summary>
        private class ProfileImportConfigAction : ConfigAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProfileImportConfigAction"/> class.
            /// </summary>
            /// <param name="what">Description of action.</param>
            /// <param name="externalPath">Original profile path.</param>
            /// <param name="local">Local profile name.</param>
            /// <param name="destFolder">Destination folder.</param>
            /// <param name="profileManager">Profile manager.</param>
            public ProfileImportConfigAction(string what, string externalPath, string local, string destFolder, IProfileManager profileManager, Wx3270App app)
                : base(what)
            {
                this.ExternalPath = externalPath;
                this.Local = local;
                this.DestFolder = destFolder;
                this.ProfileManager = profileManager;
                this.IsUndo = true;
                this.App = app;
            }

            /// <summary>
            /// Gets the original profile path name.
            /// </summary>
            public string ExternalPath { get; private set; }

            /// <summary>
            /// Gets the duplicate profile name.
            /// </summary>
            public string Local { get; private set; }

            /// <summary>
            /// Gets the destination folder.
            /// </summary>
            public string DestFolder { get; private set; }

            /// <summary>
            /// Gets the profile manager.
            /// </summary>
            public IProfileManager ProfileManager { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an undo.
            /// </summary>
            public bool IsUndo { get; private set; }

            /// <summary>
            /// Gets the application context.
            /// </summary>
            public Wx3270App App { get; private set; }

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
                var op = (ProfileImportConfigAction)from.Peek();
                var localPath = Path.Combine(op.DestFolder, op.Local + Wx3270.ProfileManager.Suffix);
                var title = string.Empty;
                try
                {
                    if (op.IsUndo)
                    {
                        // Reverse the operation: Delete the local copy.
                        title = Title.UndoImportProfile;
                        if (this.ProfileManager.IsCurrentPathName(localPath))
                        {
                            ErrorBox.Show(I18n.Get(Message.CannotDeleteCurrentProfile), I18n.Get(title));
                            return;
                        }

                        this.ProfileManager.Refocus(ProfileTree.DirNodeName(op.DestFolder));
                        File.Delete(localPath);
                    }
                    else
                    {
                        title = Title.ImportProfile;
                        if (this.ProfileManager.IsCurrentPathName(localPath))
                        {
                            ErrorBox.Show(I18n.Get(Message.CannotOverwriteCurrentProfile), I18n.Get(title));
                            return;
                        }

                        // Re-do the operation: import the profile.
                        this.ProfileManager.Refocus(ProfileTree.DirNodeName(op.DestFolder), op.Local);
                        if (Path.GetExtension(op.ExternalPath).Equals(Wx3270.ProfileManager.Suffix, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Straight copy.
                            File.Copy(op.ExternalPath, localPath);
                        }
                        else
                        {
                            // Import from wc3270.
                            var import = new Wc3270Import(this.App.CodePageDb);
                            import.Read(op.ExternalPath);
                            this.ProfileManager.Save(localPath, import.Digest());
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
                    to.Push(new ProfileImportConfigAction(op.What, op.ExternalPath, op.Local, op.DestFolder, op.ProfileManager, op.App)
                    {
                        IsUndo = !op.IsUndo,
                    });
                }
            }
        }
    }
}
