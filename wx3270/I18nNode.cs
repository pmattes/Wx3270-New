// <copyright file="I18nNode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    /// <summary>
    /// One node in the control hierarchy.
    /// </summary>
    public class I18nNode
    {
        /// <summary>
        /// Gets or sets the path name.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the control.
        /// </summary>
        public Control Control { get; set; }

        /// <summary>
        /// Gets or sets the tool strip menu item.
        /// </summary>
        public ToolStripMenuItem ToolStripMenuItem { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the tool tip text.
        /// </summary>
        public string ToolTip { get; set; }
    }
}
