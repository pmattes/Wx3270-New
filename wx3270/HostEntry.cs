// <copyright file="HostEntry.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Wx3270.Contracts;

    /// <summary>
    /// Auto-connect type for a host entry.
    /// </summary>
    public enum AutoConnect
    {
        /// <summary>
        /// Do not connect.
        /// </summary>
        None,

        /// <summary>
        /// Connect when profile is loaded.
        /// </summary>
        Connect,

        /// <summary>
        /// Connect, and re-connect when the host disconnects.
        /// </summary>
        Reconnect,
    }

    /// <summary>
    /// Host type for file transfers.
    /// </summary>
    public enum HostType
    {
        /// <summary>
        /// Not specified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// TSO host.
        /// </summary>
        Tso,

        /// <summary>
        /// VM/CMS host.
        /// </summary>
        Vm,

        /// <summary>
        /// CICS host.
        /// </summary>
        Cics,
    }

    /// <summary>
    /// Printer session type.
    /// </summary>
    public enum PrinterSessionType
    {
        /// <summary>
        /// No printer session.
        /// </summary>
        None,

        /// <summary>
        /// Associate with interactive session.
        /// </summary>
        Associate,

        /// <summary>
        /// Use a specific LU.
        /// </summary>
        SpecificLu,
    }

    /// <summary>
    /// One host.
    /// </summary>
    public class HostEntry : IEquatable<HostEntry>, ISyncedEntry<HostEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostEntry"/> class.
        /// </summary>
        public HostEntry()
        {
            this.Name = string.Empty;
            this.Host = string.Empty;
            this.Port = string.Empty;
            this.LuNames = string.Empty;
            this.AcceptHostName = string.Empty;
            this.ClientCertificateName = string.Empty;
            this.AllowStartTls = true;
            this.LoginMacro = string.Empty;
            this.Description = string.Empty;
            this.WindowTitle = string.Empty;
            this.AutoConnect = AutoConnect.None;
            this.HostType = HostType.Unspecified;
            this.PrinterSessionType = PrinterSessionType.None;
            this.PrinterSessionLu = string.Empty;
            this.Prefixes = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostEntry"/> class.
        /// </summary>
        /// <param name="other">Existing host entry.</param>
        public HostEntry(HostEntry other)
        {
            this.Profile = other.Profile;
            this.Name = other.Name;
            this.Host = other.Host;
            this.Port = other.Port;
            this.LuNames = other.LuNames;
            this.AcceptHostName = other.AcceptHostName;
            this.ClientCertificateName = other.ClientCertificateName;
            this.AllowStartTls = other.AllowStartTls;
            this.LoginMacro = other.LoginMacro;
            this.Description = other.Description;
            this.WindowTitle = other.WindowTitle;
            this.AutoConnect = other.AutoConnect;
            this.HostType = other.HostType;
            this.PrinterSessionType = other.PrinterSessionType;
            this.PrinterSessionLu = other.PrinterSessionLu;
            this.Prefixes = other.Prefixes;
        }

        /// <summary>
        /// Gets or sets the profile this entry is a part of.
        /// </summary>
        [JsonIgnore]
        public Profile Profile { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the entry can be moved.
        /// </summary>
        [JsonIgnore]
        public bool CanMove => true;

        /// <summary>
        /// Gets a value indicating whether another entry can be moved before this one.
        /// </summary>
        [JsonIgnore]
        public bool CanMoveBefore => true;

        /// <summary>
        /// Gets or sets the host name.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the port name.
        /// </summary>
        public string Port { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the LU names.
        /// </summary>
        public string LuNames { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to accept the TELNET STARTTLS option.
        /// </summary>
        public bool AllowStartTls { get; set; }

        /// <summary>
        /// Gets or sets the host prefixes.
        /// </summary>
        public string Prefixes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TLS host name override.
        /// </summary>
        public string AcceptHostName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client certificate name.
        /// </summary>
        public string ClientCertificateName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the login macro.
        /// </summary>
        public string LoginMacro { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string WindowTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets auto-connect mode.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AutoConnect AutoConnect { get; set; }

        /// <summary>
        /// Gets or sets the file transfer host type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public HostType HostType { get; set; }

        /// <summary>
        /// Gets or sets the printer session type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PrinterSessionType PrinterSessionType { get; set; }

        /// <summary>
        /// Gets or sets the printer session LU.
        /// </summary>
        public string PrinterSessionLu { get; set; } = string.Empty;

        /// <summary>
        /// Compare one host entry to another.
        /// </summary>
        /// <param name="other">Other host.</param>
        /// <returns>True if they are equal.</returns>
        public bool Equals(HostEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase)
                && this.EqualsExeptName(other);
        }

        /// <summary>
        /// Compare one host entry to another, except for the name field.
        /// </summary>
        /// <param name="other">Other host.</param>
        /// <returns>True if they are equal.</returns>
        /// <comment>The profile field is not compared.</comment>
        public bool EqualsExeptName(HostEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Host.Equals(other.Host, StringComparison.InvariantCultureIgnoreCase)
                && this.Port.Equals(other.Port, StringComparison.InvariantCultureIgnoreCase)
                && this.LuNames.Equals(other.LuNames, StringComparison.InvariantCultureIgnoreCase)
                && this.AcceptHostName.Equals(other.AcceptHostName, StringComparison.InvariantCultureIgnoreCase)
                && this.ClientCertificateName.Equals(other.ClientCertificateName, StringComparison.InvariantCultureIgnoreCase)
                && this.AllowStartTls.Equals(other.AllowStartTls)
                && this.LoginMacro.Equals(other.LoginMacro)
                && this.Description.Equals(other.Description)
                && this.WindowTitle.Equals(other.WindowTitle)
                && this.AutoConnect.Equals(other.AutoConnect)
                && this.HostType.Equals(other.HostType)
                && this.PrinterSessionType.Equals(other.PrinterSessionType)
                && this.PrinterSessionLu.Equals(other.PrinterSessionLu, StringComparison.CurrentCultureIgnoreCase)
                && this.Prefixes.Equals(other.Prefixes);
        }

        /// <summary>
        /// Replace the contents of one <see cref="HostEntry"/> with another.
        /// </summary>
        /// <param name="other">Other entry.</param>
        public void Replace(HostEntry other)
        {
            if (other == null)
            {
                return;
            }

            this.Profile = other.Profile;
            this.Name = other.Name;
            this.Host = other.Host;
            this.Port = other.Port;
            this.LuNames = other.LuNames;
            this.AcceptHostName = other.AcceptHostName;
            this.ClientCertificateName = other.ClientCertificateName;
            this.AllowStartTls = other.AllowStartTls;
            this.LoginMacro = other.LoginMacro;
            this.Description = other.Description;
            this.WindowTitle = other.WindowTitle;
            this.AutoConnect = other.AutoConnect;
            this.HostType = other.HostType;
            this.PrinterSessionType = other.PrinterSessionType;
            this.PrinterSessionLu = other.PrinterSessionLu;
            this.Prefixes = other.Prefixes;
        }

        /// <summary>
        /// Return the string representation of a host entry.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.Name + "\t");

            switch (this.AutoConnect)
            {
                case AutoConnect.None:
                    break;
                case AutoConnect.Connect:
                    sb.Append("🗲 ");
                    break;
                case AutoConnect.Reconnect:
                    sb.Append("🗲+ ");
                    break;
            }

            if (!this.Name.Equals(this.Host))
            {
                sb.Append(this.Host);
            }

            if (!string.IsNullOrEmpty(this.Port))
            {
                sb.Append(" port=" + this.Port);
            }

            if (!string.IsNullOrEmpty(this.LuNames))
            {
                var lus = string.Join(",", this.LuNames.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                if (!string.IsNullOrEmpty(lus))
                {
                    sb.Append(" lu=" + lus);
                }
            }

            if (!string.IsNullOrEmpty(this.AcceptHostName))
            {
                sb.Append(" accept=" + this.AcceptHostName);
            }

            if (!string.IsNullOrEmpty(this.ClientCertificateName))
            {
                sb.Append(" cert=" + this.ClientCertificateName);
            }

            if (this.AllowStartTls)
            {
                sb.Append(" starttls");
            }

            if (this.HostType != HostType.Unspecified)
            {
                sb.Append(" " + this.HostType.ToString().ToUpperInvariant());
            }

            if (!string.IsNullOrEmpty(this.LoginMacro))
            {
                sb.Append(" +macro");
            }

            if (!string.IsNullOrWhiteSpace(this.Description))
            {
                sb.Append(" +description");
            }

            if (!string.IsNullOrWhiteSpace(this.WindowTitle))
            {
                sb.Append(" +title");
            }

            if (this.PrinterSessionType != PrinterSessionType.None)
            {
                sb.Append(" " + this.PrinterSessionType.ToString());
            }

            if (!string.IsNullOrEmpty(this.PrinterSessionLu))
            {
                sb.Append(" " + "printerSessionLu=" + this.PrinterSessionLu);
            }

            if (!string.IsNullOrEmpty(this.Prefixes))
            {
                sb.Append(" " + "prefixes=" + this.Prefixes);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Create a clone of a host entry.
        /// </summary>
        /// <returns>Cloned entry.</returns>
        public HostEntry Clone()
        {
            return new HostEntry
            {
                Name = this.Name,
                Host = this.Host,
                Port = this.Port,
                LuNames = this.LuNames,
                AcceptHostName = this.AcceptHostName,
                ClientCertificateName = this.ClientCertificateName,
                AllowStartTls = this.AllowStartTls,
                LoginMacro = this.LoginMacro,
                Description = this.Description,
                WindowTitle = this.WindowTitle,
                AutoConnect = this.AutoConnect,
                HostType = this.HostType,
                PrinterSessionType = this.PrinterSessionType,
                PrinterSessionLu = this.PrinterSessionLu,
                Prefixes = this.Prefixes,
            };
        }

        /// <summary>
        /// Check for a conflict between this entry and an edited entry.
        /// </summary>
        /// <param name="editedEntry">Edited entry.</param>
        /// <returns>Conflict status.</returns>
        public EntryConflict CheckConflict(HostEntry editedEntry)
        {
            // Check for a name conflict (replacing entry with the same name).
            if (!string.IsNullOrEmpty(this.Name)
                && !string.IsNullOrEmpty(editedEntry.Name)
                && this.Name.Equals(editedEntry.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return EntryConflict.Replace;
            }

            // Check for a name add (adding a name to a nameless entry).
            if (string.IsNullOrEmpty(this.Name)
                && !string.IsNullOrEmpty(editedEntry.Name)
                && this.EqualsExeptName(editedEntry))
            {
                return EntryConflict.Replace;
            }

            // Check for an auto-connect conflict (there can only be one auto-connect entry).
            if (this.AutoConnect != AutoConnect.None && editedEntry.AutoConnect != AutoConnect.None)
            {
                // Change this entry. Oh, this is sneaky.
                this.AutoConnect = AutoConnect.None;

                // Return the conflict indicator. Why?
                return EntryConflict.Modified;
            }

            return EntryConflict.None;
        }
    }
}
