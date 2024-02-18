// <copyright file="ProfileWatchNode.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A representation of a profile and the hosts in it.
    /// </summary>
    public class ProfileWatchNode : WatchNode, IEquatable<WatchNode>, IEquatable<ProfileWatchNode>, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileWatchNode"/> class.
        /// </summary>
        public ProfileWatchNode()
        {
            this.Type = WatchNodeType.Profile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileWatchNode"/> class.
        /// </summary>
        /// <param name="hosts">List of hosts.</param>
        public ProfileWatchNode(IEnumerable<HostWatchNode> hosts)
            : base(hosts.Cast<WatchNode>())
        {
            this.Type = WatchNodeType.Profile;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is broken (cannot be parsed).
        /// </summary>
        public bool Broken { get; set; }

        /// <summary>
        /// Gets or sets the path name.
        /// </summary>
        public string PathName { get; set; }

        /// <summary>
        /// Gets or sets the profile value.
        /// </summary>
        public Profile Profile { get; set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IEnumerable<HostWatchNode> Hosts => this.Children.OfType<HostWatchNode>();

        /// <summary>
        /// Compare a <see cref="ProfileWatchNode"/> for equality.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True if equal.</returns>
        public bool Equals(ProfileWatchNode other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!base.Equals(other as WatchNode))
            {
                return false;
            }

            return this.PathName.Equals(other.PathName)
                && this.Broken == other.Broken
                && this.PathName.Equals(other.PathName)
                && this.Profile?.Equals(other.Profile) != false;
        }

        /// <summary>
        /// Compare a <see cref="WatchNode"/> for equality.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(WatchNode other)
        {
            return this.Equals(other as ProfileWatchNode);
        }

        /// <summary>
        /// Compare an object for equality.
        /// </summary>
        /// <param name="other">Other node.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as ProfileWatchNode);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.PathName.GetHashCode() ^ this.Broken.GetHashCode();
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public override object Clone()
        {
            return new ProfileWatchNode { Name = this.Name, Broken = this.Broken, PathName = this.PathName, Profile = this.Profile };
        }
    }
}
