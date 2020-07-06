// <copyright file="WatchNode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Types of watched nodes.
    /// </summary>
    public enum WatchNodeType
    {
        /// <summary>
        /// A directory, recursively containing other directories and profiles.
        /// </summary>
        Folder,

        /// <summary>
        /// A profile, containing hosts.
        /// </summary>
        Profile,

        /// <summary>
        /// A host.
        /// </summary>
        Host,
    }

    /// <summary>
    /// A representation of a watched node.
    /// </summary>
    public abstract class WatchNode : IEquatable<WatchNode>, ICloneable
    {
        /// <summary>
        /// The children.
        /// </summary>
        private readonly List<WatchNode> children = new List<WatchNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchNode"/> class.
        /// </summary>
        public WatchNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchNode"/> class.
        /// </summary>
        /// <param name="children">Initial set of children.</param>
        public WatchNode(IEnumerable<WatchNode> children)
        {
            this.children.AddRange(children);
        }

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the set of children of this node.
        /// </summary>
        public IEnumerable<WatchNode> Children => this.children;

        /// <summary>
        /// Gets or sets the node type.
        /// </summary>
        public WatchNodeType Type { get; protected set; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public WatchNode Parent { get; private set; }

        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <param name="child">New child node.</param>
        public void Add(WatchNode child)
        {
            this.children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Depth-first walk of all child nodes.
        /// </summary>
        /// <typeparam name="T">Stack type.</typeparam>
        /// <param name="stack">Transient stack.</param>
        /// <param name="func">Function to call.</param>
        public void ForEach<T>(Stack<T> stack, Func<WatchNode, Stack<T>, T> func)
        {
            T element = func(this, stack);
            if (element != null)
            {
                stack.Push(element);
                foreach (var child in this.Children)
                {
                    child.ForEach(stack, func);
                }

                stack.Pop();
            }
        }

        /// <summary>
        /// Depth-first walk of all child nodes.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        public void ForEach(Action<WatchNode> action)
        {
            action(this);
            foreach (var child in this.Children)
            {
                child.ForEach(action);
            }
        }

        /// <summary>
        /// Depth-first walk of all child nodes, with a halting condition.
        /// </summary>
        /// <param name="f">Action to perform.</param>
        /// <returns>True if halted.</returns>
        public bool Any(Func<WatchNode, bool> f)
        {
            if (f(this))
            {
                return true;
            }

            foreach (var child in this.Children)
            {
                if (child.Any(f))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Compares two sub-trees.
        /// </summary>
        /// <param name="other">Other sub-tree.</param>
        /// <returns>True if equal.</returns>
        /// <remarks>This method is complete garbage.</remarks>
        public bool TreeEquals(WatchNode other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (!this.Equals(other))
            {
                return false;
            }

            if (this.children.Count != other.children.Count)
            {
                return false;
            }

            int index = 0;
            foreach (var child in this.Children)
            {
                if (!child.TreeEquals(other.children[index++]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clone a tree.
        /// </summary>
        /// <returns>Cloned tree.</returns>
        public WatchNode CloneTree()
        {
            var ret = (WatchNode)this.Clone();
            var stack = new Stack<WatchNode>();
            stack.Push(ret);
            foreach (var child in this.children)
            {
                child.ForEach(stack, (elt, s) =>
                {
                    var c = (WatchNode)elt.Clone();
                    s.Peek().Add(c);
                    return c;
                });
            }

            return ret;
        }

        /// <summary>
        /// Unlinks a node.
        /// </summary>
        /// <remarks>Dangerous.</remarks>
        public void Unlink()
        {
            // Remove from the parent.
            if (this.Parent != null)
            {
                this.Parent.children.Remove(this);
            }

            // Re-parent the children.
            foreach (var child in this.children)
            {
                this.Parent.Add(child);
            }
        }

        /// <summary>
        /// Convert to a string.
        /// </summary>
        /// <returns>String value.</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}[2]", this.Type, this.Name, this.children.Count);
        }

        /// <summary>
        /// Base equals method, compares common fields.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>False if not equal.</returns>
        public virtual bool Equals(WatchNode other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name;
        }

        /// <summary>
        /// Base hash code method, computes common fields.
        /// </summary>
        /// <returns>Partial hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^
                this.children.Aggregate(
                    this.Name.GetHashCode(),
                    (total, next) => total ^ next.GetHashCode());
        }

        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
