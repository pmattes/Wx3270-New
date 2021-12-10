// <copyright file="Profile.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Wx3270.Contracts;

    /// <summary>
    /// Possible values for the keypad position.
    /// </summary>
    public enum KeypadPosition
    {
        /// <summary>
        /// Centered over the window.
        /// </summary>
        Centered,

        /// <summary>
        /// To the left of the window.
        /// </summary>
        Left,

        /// <summary>
        /// To the right of the window.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Possible values for the cursor type.
    /// </summary>
    public enum CursorType
    {
        /// <summary>
        /// Block cursor (entire character cell).
        /// </summary>
        Block,

        /// <summary>
        /// Underscore cursor (bottom 20% of the character cell).
        /// </summary>
        Underscore,
    }

    /// <summary>
    /// Profile type.
    /// </summary>
    public enum ProfileType
    {
        /// <summary>
        /// Full profile (the default).
        /// </summary>
        Full,

        /// <summary>
        /// Keyboard map template.
        /// </summary>
        KeyboardMapTemplate,

        /// <summary>
        /// KeypadMapTemplate.
        /// </summary>
        KeypadMapTemplate,
    }

    /// <summary>
    /// Configurable options, serialized in a file.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Profile : IEquatable<Profile>
    {
        /// <summary>
        /// The name of a profile that wasn't read from a file.
        /// </summary>
        public const string NoName = "(none)";

        /// <summary>
        /// Default mapping of host colors to drawing colors.
        /// </summary>
        private static readonly HostColors DefaultScreenColors = new HostColors
        {
            { HostColor.NeutralBlack, Color.Black },
            { HostColor.Blue, Color.RoyalBlue },
            { HostColor.Red, Color.Red },
            { HostColor.Pink, Color.HotPink },
            { HostColor.Green, Color.FromArgb(255, 0, 164, 0) },
            { HostColor.Turquoise, Color.Turquoise },
            { HostColor.Yellow, Color.Yellow },
            { HostColor.NeutralWhite, Color.White },
            { HostColor.Black, Color.Black },
            { HostColor.DeepBlue, Color.Blue },
            { HostColor.Orange, Color.Orange },
            { HostColor.Purple, Color.DarkViolet },
            { HostColor.PaleGreen, Color.PaleGreen },
            { HostColor.PaleTurquoise, Color.PaleTurquoise },
            { HostColor.Grey, Color.Gray },
            { HostColor.White, Color.White },
        };

        /// <summary>
        /// Default monochrome colors.
        /// </summary>
        private static readonly MonoColors DefaultScreenMonoColors = new MonoColors
        {
            Normal = Color.FromArgb(255, 0, 178, 0),
            Intensified = Color.Lime,
            Background = Color.Black,
        };

        /// <summary>
        /// Backing field for <see cref="KeypadMap"/>.
        /// </summary>
        private KeyMap<KeypadMap> keypadMap = new KeyMap<KeypadMap>(DefaultKeypadMap.Map);

        /// <summary>
        /// Backing field for <see cref="KeyboardMap"/>.
        /// </summary>
        private KeyMap<KeyboardMap> keyboardMap = new KeyMap<KeyboardMap>(DefaultKeyboardMap);

        /// <summary>
        /// Backing field for <see cref="PathName"/>.
        /// </summary>
        private string pathName = string.Empty;

        /// <summary>
        /// Gets the default profile.
        /// </summary>
        public static Profile DefaultProfile
        {
            get
            {
                var p = new Profile { Name = ProfileManager.DefaultValuesName };
                foreach (var kv in p.KeypadMap)
                {
                    kv.Value.Text = DefaultKeypadMap.LocalizeText(kv.Key, kv.Value);
                    kv.Value.TextSize = DefaultKeypadMap.LocalizeTextSize(kv.Key, kv.Value);
                }

                return p;
            }
        }

        /// <summary>
        /// Gets or sets the profile name. (Not serialized.)
        /// </summary>
        public string Name { get; set; } = NoName;

        /// <summary>
        /// Gets or sets the full profile pathname. (Not serialized.)
        /// </summary>
        public string PathName
        {
            get
            {
                return this.pathName;
            }

            set
            {
                this.pathName = value;
                this.DisplayFolder = ProfileTree.DirNodeName(Path.GetDirectoryName(value));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is read-only. (Not serialized.)
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the display-friendly version of the folder name. (Not serialized.)
        /// </summary>
        public string DisplayFolder { get; set; }

        /// <summary>
        /// Gets or sets the profile version.
        /// </summary>
        [JsonProperty]
        public VersionClass Version { get; set; } = VersionClass.GetThis();

        /// <summary>
        /// Gets or sets the profile type.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProfileType ProfileType { get; set; } = ProfileType.Full;

        /// <summary>
        /// Gets or sets where to launch the keypad.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public KeypadPosition KeypadPosition { get; set; } = KeypadPosition.Centered;

        /// <summary>
        /// Gets or sets a value indicating whether keys make an audible click.
        /// </summary>
        [JsonProperty]
        public bool KeyClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the terminal bell is audible.
        /// </summary>
        [JsonProperty]
        public bool AudibleBell { get; set; } = true;

        /// <summary>
        /// Gets or sets the colors.
        /// </summary>
        [JsonProperty]
        public Colors Colors { get; set; } = new Colors
        {
            HostColors = new HostColors(DefaultScreenColors),
            MonoColors = new MonoColors(DefaultScreenMonoColors),
            SelectBackground = Color.DarkGray,
            CrosshairColor = Color.Purple,
        };

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        [JsonProperty]
        public FontProfile Font { get; set; } = new FontProfile();

        /// <summary>
        /// Gets or sets the host code page.
        /// </summary>
        [JsonProperty]
        public string HostCodePage { get; set; } = CodePageDb.Default;

        /// <summary>
        /// Gets or sets the model number.
        /// </summary>
        [JsonProperty]
        public int Model { get; set; } = 4;

        /// <summary>
        /// Gets or sets a value indicating whether to use extended mode -E suffix on the terminal name).
        /// </summary>
        [JsonProperty]
        public bool ExtendedMode { get; set; } = true;

        /// <summary>
        /// Gets or sets the terminal name override.
        /// </summary>
        [JsonProperty]
        public string TerminalNameOverride { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to emulate a 3279.
        /// </summary>
        [JsonProperty]
        public bool ColorMode { get; set; } = true;

        /// <summary>
        /// Gets or sets oversize screen dimensions.
        /// </summary>
        [JsonProperty]
        public OversizeClass Oversize { get; set; } = new OversizeClass();

        /// <summary>
        /// Gets or sets the cursor type.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public CursorType CursorType { get; set; } = CursorType.Block;

        /// <summary>
        /// Gets or sets a value indicating whether the cursor should blink.
        /// </summary>
        [JsonProperty]
        public bool CursorBlink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display a cursor crosshair.
        /// </summary>
        [JsonProperty]
        public bool CrosshairCursor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether text should be displayed in upper case.
        /// </summary>
        [JsonProperty]
        public bool Monocase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window should be maximized.
        /// </summary>
        [JsonProperty]
        public bool Maximize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether type-ahead should be permitted.
        /// </summary>
        [JsonProperty]
        public bool Typeahead { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to display command timing in the OIA.
        /// </summary>
        [JsonProperty]
        public bool ShowTiming { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to default to insert mode.
        /// </summary>
        [JsonProperty]
        public bool AlwaysInsert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the scrollbar should appear.
        /// </summary>
        [JsonProperty]
        public bool ScrollBar { get; set; } = true;

        /// <summary>
        /// Gets or sets the macros.
        /// </summary>
        [JsonProperty]
        public IEnumerable<MacroEntry> Macros { get; set; } = new List<MacroEntry>();

        /// <summary>
        /// Gets or sets the hosts.
        /// </summary>
        [JsonProperty]
        public IEnumerable<HostEntry> Hosts { get; set; } = new List<HostEntry>();

        /// <summary>
        /// Gets or sets the keypad map.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(ClearingConverter<KeyMap<KeypadMap>>))]
        public KeyMap<KeypadMap> KeypadMap
        {
            get => this.keypadMap;
            set { this.keypadMap = new KeyMap<KeypadMap>(value); }
        }

        /// <summary>
        /// Gets or sets the keyboard map.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(NormalizingConverter<KeyMap<KeyboardMap>>))]
        public KeyMap<KeyboardMap> KeyboardMap
        {
            get => this.keyboardMap;
            set { this.keyboardMap = new KeyMap<KeyboardMap>(value); }
        }

        /// <summary>
        /// Gets or sets the listening ports.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, ListenPort> ListenPort { get; set; } = new Dictionary<string, ListenPort>();

        /// <summary>
        /// Gets or sets the printer name.
        /// </summary>
        [JsonProperty]
        public string Printer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the printer code page.
        /// </summary>
        [JsonProperty]
        public string PrinterCodePage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the printer options.
        /// </summary>
        [JsonProperty]
        public string PrinterOptions { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the TELNET NOP transmit interval, in seconds.
        /// </summary>
        [JsonProperty]
        public int NopInterval { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Location property is valid.
        /// </summary>
        public bool HasLocation { get; set; }

        /// <summary>
        /// Gets or sets the main window location.
        /// </summary>
        [JsonProperty]
        public Point? Location { get; set; }

        /// <summary>
        /// Gets or sets the main window size.
        /// </summary>
        [JsonProperty]
        public Size? Size { get; set; }

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        [JsonProperty]
        public ProxyClass Proxy { get; set; } = new ProxyClass();

        /// <summary>
        /// Gets or sets the window opacity.
        /// </summary>
        [JsonProperty]
        public int OpacityPercent { get; set; } = 100;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        [JsonProperty]
        public string WindowTitle { get; set; }

        /// <summary>
        /// Gets or sets the miscellaneous settings.
        /// </summary>
        [JsonProperty]
        public Dictionary<string, string> MiscSettings { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Should-serialize method for the <see cref="Oversize"/> field.
        /// </summary>
        /// <returns>True if it should be serialized.</returns>
        public bool ShouldSerializeOversize()
        {
            return this.Oversize.HasValue();
        }

        /// <summary>
        /// Clone a profile (deep copy).
        /// </summary>
        /// <param name="overrideName">Override name.</param>
        /// <param name="overridePathName">Override path name.</param>
        /// <returns>Cloned copy.</returns>
        public Profile Clone(string overrideName = null, string overridePathName = null)
        {
            // Start with a shallow copy.
            var ret = (Profile)this.MemberwiseClone();

            // Override the name, if requested.
            if (overrideName != null)
            {
                ret.Name = overrideName;
            }

            if (overridePathName != null)
            {
                ret.PathName = overridePathName;
            }

            // Do deep copies of fields that need them. Unfortunately, this needs to be kept up manually.
            ret.Colors = new Colors(this.Colors);
            ret.Oversize = this.Oversize.Clone();
            ret.Macros = this.Macros.Select(m => new MacroEntry(m)).ToList();
            ret.Hosts = this.Hosts.Select(h => new HostEntry(h)).ToList();
            ret.KeypadMap = new KeyMap<KeypadMap>(this.KeypadMap);
            ret.KeyboardMap = new KeyMap<KeyboardMap>(this.KeyboardMap);
            ret.ListenPort = new Dictionary<string, ListenPort>();
            foreach (var port in this.ListenPort)
            {
                ret.ListenPort[port.Key] = new ListenPort(port.Value);
            }

            ret.Proxy = this.Proxy.Clone() as ProxyClass;
            ret.MiscSettings = new Dictionary<string, string>();
            foreach (var setting in this.MiscSettings)
            {
                ret.MiscSettings[setting.Key] = setting.Value;
            }

            return ret;
        }

        /// <summary>
        /// Apply this profile's path to a different name.
        /// </summary>
        /// <param name="name">New name.</param>
        /// <returns>New path name.</returns>
        public string MappedPath(string name)
        {
            if (string.IsNullOrEmpty(this.PathName))
            {
                // Defaults, no pathname associated, use the default directory.
                return ProfileManager.ProfilePath(name);
            }

            var ret = Path.Combine(Path.GetDirectoryName(this.PathName), name);
            if (!ret.EndsWith(ProfileManager.Suffix))
            {
                ret += ProfileManager.Suffix;
            }

            return ret;
        }

        /// <summary>
        /// Compare profiles.
        /// </summary>
        /// <param name="other">Other profile.</param>
        /// <returns>True if the profiles are equal.</returns>
        public bool Equals(Profile other)
        {
            // Easy cases.
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Hard case. We do it very inefficiently, but no maintenance is required when this class changes.
            return this.Serialized().SequenceEqual(other.Serialized());
        }

        /// <summary>
        /// Get the serialized bytes for a profile.
        /// </summary>
        /// <returns>Sequence of bytes.</returns>
        private IEnumerable<byte> Serialized()
        {
            byte[] b;
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    var ser = new JsonSerializer();
                    var jsonWriter = new JsonTextWriter(writer);
#if false
                    jsonWriter.Formatting = Formatting.Indented;
#endif
                    ser.Serialize(jsonWriter, this);
                    writer.Flush();
                    var length = (int)stream.Length;
                    b = new byte[length];
                    stream.Position = 0;
                    stream.Read(b, 0, length);
                }
            }

            return b;
        }

        /// <summary>
        /// Version number class.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class VersionClass
        {
            /// <summary>
            /// Gets the full version, for display rather than comparison.
            /// </summary>
            public static string FullVersion =>
                typeof(Profile).Assembly.GetName().Version.Major.ToString() + "." +
                typeof(Profile).Assembly.GetName().Version.Minor + Release.PhaseName +
                typeof(Profile).Assembly.GetName().Version.Build;

            /// <summary>
            /// Gets or sets the major version number.
            /// </summary>
            [JsonProperty]
            public int Major { get; set; }

            /// <summary>
            /// Gets or sets the minor version number.
            /// </summary>
            [JsonProperty]
            public int Minor { get; set; }

            /// <summary>
            /// Compare two versions.
            /// </summary>
            /// <param name="a">First version.</param>
            /// <param name="b">Second version.</param>
            /// <returns>True if <paramref name="a"/> is greater than <paramref name="b"/>.</returns>
            public static bool operator >(VersionClass a, VersionClass b)
            {
                if (a == null && b == null)
                {
                    return false;
                }

                if (a == null)
                {
                    return true;
                }

                if (b == null)
                {
                    return true;
                }

                return a.Major > b.Major || (a.Major == b.Major && a.Minor > b.Minor);
            }

            /// <summary>
            /// Compare two versions.
            /// </summary>
            /// <param name="a">First version.</param>
            /// <param name="b">Second version.</param>
            /// <returns>True if <paramref name="a"/> is less than <paramref name="b"/>.</returns>
            public static bool operator <(VersionClass a, VersionClass b)
            {
                if (a == null && b == null)
                {
                    return false;
                }

                if (b == null)
                {
                    return true;
                }

                if (a == null)
                {
                    return true;
                }

                return b.Major > a.Major || (b.Major == a.Major && b.Minor > a.Minor);
            }

            /// <summary>
            /// Get a copy of this assembly's version number.
            /// </summary>
            /// <returns>Version number.</returns>
            public static VersionClass GetThis()
            {
                return new VersionClass
                {
                    Major = typeof(Profile).Assembly.GetName().Version.Major,
                    Minor = typeof(Profile).Assembly.GetName().Version.Minor,
                };
            }

            /// <summary>
            /// Convert to a string.
            /// </summary>
            /// <returns>String representation.</returns>
            public override string ToString()
            {
                return string.Format("{0}.{1}", this.Major.ToString(), this.Minor.ToString());
            }
        }

        /// <summary>
        /// Oversize screen dimensions class.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class OversizeClass : IEquatable<OversizeClass>
        {
            /// <summary>
            /// Gets or sets the number of rows.
            /// </summary>
            [JsonProperty]
            public int Rows { get; set; }

            /// <summary>
            /// Gets or sets the number of columns.
            /// </summary>
            [JsonProperty]
            public int Columns { get; set; }

            /// <summary>
            /// Test if an <see cref="OversizeClass"/> has a meaningful value.
            /// </summary>
            /// <returns>True if the object has a value.</returns>
            public bool HasValue()
            {
                return this.Rows != 0 && this.Columns != 0;
            }

            /// <summary>
            /// Clones an <see cref="OversizeClass"/> object.
            /// </summary>
            /// <returns>Cloned copy.</returns>
            public OversizeClass Clone()
            {
                return (OversizeClass)this.MemberwiseClone();
            }

            /// <summary>
            /// Compares two <see cref="OversizeClass"/> objects for equality.
            /// </summary>
            /// <param name="other">Other object.</param>
            /// <returns>True if they are equal.</returns>
            public bool Equals(OversizeClass other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.Rows == other.Rows
                    && this.Columns == other.Columns;
            }
        }

        /// <summary>
        /// Proxy class.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class ProxyClass : ICloneable, IEquatable<ProxyClass>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProxyClass"/> class.
            /// </summary>
            public ProxyClass()
            {
            }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            [JsonProperty]
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the address.
            /// </summary>
            [JsonProperty]
            public string Address { get; set; }

            /// <summary>
            /// Gets or sets the port.
            /// </summary>
            [JsonProperty]
            public int? Port { get; set; }

            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            [JsonProperty]
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the password.
            /// </summary>
            [JsonProperty]
            public string Password { get; set; }

            /// <summary>
            /// Clones a <see cref="ProxyClass"/>.
            /// </summary>
            /// <returns>Cloned instance.</returns>
            public object Clone()
            {
                return this.MemberwiseClone();
            }

            /// <summary>
            /// Compare for equality.
            /// </summary>
            /// <param name="other">Other proxy.</param>
            /// <returns>True if equal.</returns>
            public bool Equals(ProxyClass other)
            {
                return this.Type == other.Type &&
                    this.Address == other.Address &&
                    this.Port == other.Port &&
                    this.Username == other.Username &&
                    this.Password == other.Password;
            }
        }

        /// <summary>
        /// Private JSON converter for the keyboard map.
        /// </summary>
        /// <typeparam name="T">Underlying type.</typeparam>
        private class ClearingConverter<T> : JsonConverter
            where T : IClear
        {
            /// <summary>
            /// Tests if a type can be converted.
            /// </summary>
            /// <param name="objectType">Object type.</param>
            /// <returns>True if it can be converted.</returns>
            public override bool CanConvert(Type objectType)
            {
                return typeof(T).IsAssignableFrom(objectType);
            }

            /// <summary>
            /// Deserialize a keyboard map.
            /// </summary>
            /// <param name="reader">JSON reader.</param>
            /// <param name="objectType">Object type.</param>
            /// <param name="existingValue">Existing value.</param>
            /// <param name="serializer">JSON serializer.</param>
            /// <returns>Deserialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // Clear the existing keymap first.
                var oldDict = (T)existingValue;
                oldDict.Clear();

                // Deserialize as usual.
                return serializer.Deserialize(reader, typeof(T));
            }

            /// <summary>
            /// Serialize a keyboard map.
            /// </summary>
            /// <param name="writer">JSON writer.</param>
            /// <param name="value">Value to serialize.</param>
            /// <param name="serializer">JSON serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Serialize as usual, using the underlying type.
                serializer.Serialize(writer, value, typeof(T));
            }
        }

        /// <summary>
        /// Private JSON converter for the keyboard map.
        /// </summary>
        /// <typeparam name="T">Underlying type.</typeparam>
        private class NormalizingConverter<T> : ClearingConverter<T>
            where T : IClear, INormalize
        {
            /// <summary>
            /// Tests if a type can be converted.
            /// </summary>
            /// <param name="objectType">Object type.</param>
            /// <returns>True if it can be converted.</returns>
            public override bool CanConvert(Type objectType)
            {
                return base.CanConvert(objectType);
            }

            /// <summary>
            /// Deserialize a keyboard map.
            /// </summary>
            /// <param name="reader">JSON reader.</param>
            /// <param name="objectType">Object type.</param>
            /// <param name="existingValue">Existing value.</param>
            /// <param name="serializer">JSON serializer.</param>
            /// <returns>Deserialized object.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // Deserialize first.
                var ret = base.ReadJson(reader, objectType, existingValue, serializer);
                var normal = (INormalize)ret;
                normal.Normalize();
                return ret;
            }

            /// <summary>
            /// Serialize a keyboard map.
            /// </summary>
            /// <param name="writer">JSON writer.</param>
            /// <param name="value">Value to serialize.</param>
            /// <param name="serializer">JSON serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                base.WriteJson(writer, value, serializer);
            }
        }
    }
}
