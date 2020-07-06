// <copyright file="FolderTreeNode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    /// <summary>
    /// Folder tree node.
    /// </summary>
    public class FolderTreeNode : TreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderTreeNode"/> class.
        /// </summary>
        /// <param name="name">Node name</param>
        public FolderTreeNode(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default profile folder.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the folder name (real path, not display name).
        /// </summary>
        public string FolderName { get; set; }
    }
}
