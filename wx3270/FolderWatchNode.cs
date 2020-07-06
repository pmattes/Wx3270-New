// <copyright file="FolderWatchNode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A representation of a folder and the profiles in it.
    /// </summary>
    public class FolderWatchNode : WatchNode, IEquatable<WatchNode>, IEquatable<FolderWatchNode>
    {
        /// <summary>
        /// The child nodes.
        /// </summary>
        private readonly List<WatchNode> children = new List<WatchNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderWatchNode"/> class.
        /// </summary>
        /// <param name="children">Child nodes</param>
        public FolderWatchNode(IEnumerable<WatchNode> children)
            : base(children)
        {
            this.Type = WatchNodeType.Folder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderWatchNode"/> class.
        /// </summary>
        public FolderWatchNode()
            : base()
        {
            this.Type = WatchNodeType.Folder;
        }

        /// <summary>
        /// Gets or sets the path name.
        /// </summary>
        public string PathName { get; set; }

        /// <summary>
        /// Compare a <see cref="FolderWatchNode"/> for equality.
        /// </summary>
        /// <param name="other">Other node</param>
        /// <returns>True if equal</returns>
        public bool Equals(FolderWatchNode other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!base.Equals(other as WatchNode))
            {
                return false;
            }

            return this.PathName.Equals(other.PathName);
        }

        /// <summary>
        /// Compare a <see cref="WatchNode"/> for equality.
        /// </summary>
        /// <param name="other">Other node</param>
        /// <returns>True if equal</returns>
        public override bool Equals(WatchNode other)
        {
            return this.Equals(other as FolderWatchNode);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.PathName.GetHashCode();
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>Cloned object</returns>
        public override object Clone()
        {
            return new FolderWatchNode { Name = this.Name, PathName = this.PathName };
        }
    }
}
