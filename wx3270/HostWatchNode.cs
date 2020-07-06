// <copyright file="HostWatchNode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// A summary representation of a host.
    /// </summary>
    public class HostWatchNode : WatchNode, IEquatable<WatchNode>, IEquatable<HostWatchNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostWatchNode"/> class.
        /// </summary>
        /// <param name="name">Host name.</param>
        /// <param name="autoConnect">True of host is auto-connect.</param>
        /// <param name="hostEntry">Host entry.</param>
        public HostWatchNode(string name, bool autoConnect, HostEntry hostEntry)
        {
            this.Type = WatchNodeType.Host;
            this.Name = name;
            this.AutoConnect = autoConnect;
            this.HostEntry = hostEntry;
        }

        /// <summary>
        /// Gets a value indicating whether the host is auto-connect.
        /// </summary>
        public bool AutoConnect { get; private set; }

        /// <summary>
        /// Gets the host entry.
        /// </summary>
        public HostEntry HostEntry { get; private set; }

        /// <summary>
        /// Compare a <see cref="HostWatchNode"/> for equality.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True if equal.</returns>
        public bool Equals(HostWatchNode other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!base.Equals(other))
            {
                return false;
            }

            return this.Name.Equals(other.Name) && this.AutoConnect == other.AutoConnect && this.HostEntry.Equals(other.HostEntry);
        }

        /// <summary>
        /// Compare a <see cref="WatchNode"/> for equality.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(WatchNode other)
        {
            return this.Equals(other as HostWatchNode);
        }

        /// <summary>
        /// Compare an object for equality.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as HostWatchNode);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.AutoConnect.GetHashCode() ^ this.HostEntry.GetHashCode();
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public override object Clone()
        {
            return new HostWatchNode(this.Name, this.AutoConnect, this.HostEntry);
        }
    }
}
