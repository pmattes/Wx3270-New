// <copyright file="ListenPort.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    /// Configuration for a listening port.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ListenPort : ICloneable, IEquatable<ListenPort>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListenPort"/> class.
        /// </summary>
        public ListenPort()
        {
            this.Address = IPAddress.Any;
            this.Port = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenPort"/> class.
        /// </summary>
        /// <param name="other">Other port.</param>
        public ListenPort(ListenPort other)
        {
            this.Address = new IPAddress(other.Address.GetAddressBytes());
            this.Port = other.Port;
        }

        /// <summary>
        /// Gets or sets the listening address.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress Address { get; set; }

        /// <summary>
        /// Gets or sets the listening port.
        /// </summary>
        [JsonProperty]
        public ushort Port { get; set; }

        /// <summary>
        /// Clone a <see cref="ListenPort"/>.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public object Clone()
        {
            return new ListenPort() { Address = this.Address, Port = this.Port };
        }

        /// <summary>
        /// Equality comparer.
        /// </summary>
        /// <param name="other">Other listen port.</param>
        /// <returns>True if equal.</returns>
        public bool Equals(ListenPort other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Address.Equals(other.Address) && this.Port == other.Port;
        }

        /// <summary>
        /// Object-level equality comparer.
        /// </summary>
        /// <param name="obj">Other object.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ListenPort);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return this.Address.GetHashCode() ^ this.Port.GetHashCode();
        }
    }
}
