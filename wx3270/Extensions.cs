// <copyright file="Extensions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Various extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Join folder names in a tree node's path.
        /// </summary>
        /// <param name="treeNode">Tree node.</param>
        /// <returns>Joined path.</returns>
        /// <remarks>
        /// This is used as the name of each TreeView node. It compresses folder nodes into a single component,
        /// separated by the TreeView's separator character '*' from the optional profile and host names.
        /// </remarks>
        public static string JoinedFullPath(this TreeNode treeNode)
        {
            // Create a stack of the nodes, all the way from the root.
            var stack = new Stack<TreeNode>();
            var node = treeNode;
            do
            {
                stack.Push(node);
                node = node.Parent;
            }
            while (node != null);

            // Join the folders with the system directory separator, and everything else with the TreeView path separator.
            return string.Join(
                treeNode.TreeView.PathSeparator,
                new[]
                {
                    string.Join(Path.DirectorySeparatorChar.ToString(), stack.TakeWhile(n => n is FolderTreeNode).Select(n => n.Text)),
                }.Concat(stack.SkipWhile(n => n is FolderTreeNode).Select(n => n.Text)));
        }

        /// <summary>
        /// Remove a control from a parent.
        /// </summary>
        /// <param name="control">Control to remove.</param>
        public static void RemoveFromParent(this Control control)
        {
            control.Parent?.Controls.Remove(control);
        }

        /// <summary>
        /// Remove a tool strip menu item from its owner.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public static void RemoveFromOwner(this ToolStripMenuItem item)
        {
            item.Owner?.Items.Remove(item);
        }
    }
}
