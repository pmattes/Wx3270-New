// <copyright file="KeyMap.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// Match types.
    /// </summary>
    public enum MatchType
    {
        /// <summary>
        /// Match by key code.
        /// </summary>
        KeyCode,

        /// <summary>
        /// Match by scan code.
        /// </summary>
        ScanCode,
    }

    /// <summary>
    /// A set of key map.
    /// </summary>
    /// <typeparam name="T">Type of map.</typeparam>
    /// <remarks>
    /// The dictionary key is a string composed of the key name and the modifiers, rather than a tuple
    /// of those two, so that the dictionary can be serialized more easily. (The JSON library does not
    /// consider dictionary keys as types, so they can't easily be deserialized.)
    /// </remarks>
    public class KeyMap<T> : Dictionary<string, T>, IEquatable<KeyMap<T>>, IClear, INormalize
        where T : class, IEquatable<T>, ICloneable, IActions
    {
        /// <summary>
        /// Separator between modifiers and the chord name.
        /// </summary>
        private const string ChordSeparator = "|";

        /// <summary>
        /// The exact-mode modifier keys.
        /// </summary>
        private const int ExactModKeys = (int)(KeyboardModifier.Shift | KeyboardModifier.Ctrl | KeyboardModifier.Alt);

        /// <summary>
        /// Regular expression for matching uChord() actions.
        /// </summary>
        private static readonly Regex ChordRegex = new Regex(Constants.Action.Chord + @"\(" + "\"" + @"(?<actions>.*)" + "\"" + @"\)");

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMap{T}"/> class.
        /// </summary>
        public KeyMap()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMap{T}"/> class.
        /// </summary>
        /// <param name="other">Other keypad maps.</param>
        public KeyMap(KeyMap<T> other)
            : base()
        {
            // Clone the dictionary.
            foreach (var key in other.Keys)
            {
                this[key] = (T)other[key].Clone();
            }
        }

        /// <summary>
        /// Create a dictionary key from a key name and a set of modifiers.
        /// </summary>
        /// <param name="name">Key name.</param>
        /// <param name="mod">Set of modifiers.</param>
        /// <param name="chord">Chord name.</param>
        /// <returns>Dictionary key.</returns>
        public static string Key(string name, KeyboardModifier mod, string chord = null)
        {
            return string.Format(
                "{0} {1}{2}",
                name,
                mod,
                string.IsNullOrEmpty(chord) ? string.Empty : " " + ChordSeparator + " " + chord);
        }

        /// <summary>
        /// Create a dictionary key from a keysym and a set of modifiers.
        /// </summary>
        /// <param name="keysym">Keysym value.</param>
        /// <param name="mod">Set of modifiers.</param>
        /// <param name="chord">Chord name.</param>
        /// <returns>Dictionary key.</returns>
        public static string Key(Keys keysym, KeyboardModifier mod, string chord = null)
        {
            return string.Format(
                "{0} {1}{2}",
                KeyboardUtil.ToStringExtended(keysym),
                mod,
                string.IsNullOrEmpty(chord) ? string.Empty : " " + ChordSeparator + " " + chord);
        }

        /// <summary>
        /// Translated an encoded (dictionary key) key name to a friendly name.
        /// </summary>
        /// <param name="encodedKey">Encoded key name.</param>
        /// <returns>Decoded name.</returns>
        public static string DecodeKeyName(string encodedKey)
        {
            // Split out the chord.
            if (encodedKey.Contains("|"))
            {
                var chordParts = encodedKey.Split('|');
                return DecodeKeyName(chordParts[1].Trim()) + " + " + DecodeKeyName(chordParts[0].Trim());
            }

            // Separate the key name and modifiers.
            var parts = encodedKey.Split(new char[] { ' ' }, 2);
            var keyName = parts[0];
            if (keyName.Length == 2 && keyName[0] == 'D' && "0123456789".Contains(keyName[1]))
            {
                keyName = keyName.Substring(1);
            }

            if (parts.Length == 1)
            {
                return keyName;
            }

            // Reformat the modifiers.
            if (parts[1] == "None")
            {
                return keyName;
            }

            var modifiersList = parts[1].Replace(",", string.Empty).Split().ToList();
            var modes = new List<string>();
            foreach (var mod in new string[] { "Mode3270", "ModeNvt" })
            {
                if (modifiersList.Contains(mod))
                {
                    modifiersList.Remove(mod);
                    modes.Add(" / " + mod.Replace("Mode", string.Empty).ToUpper());
                }
            }

            modifiersList.Add(keyName);
            return string.Join("-", modifiersList) + modes.FirstOrDefault();
        }

        /// <summary>
        /// Checks a set of actions for a chord match.
        /// </summary>
        /// <param name="actions">Set of actions.</param>
        /// <returns>True if chord.</returns>
        public static bool IsChord(string actions)
        {
            return ChordRegex.IsMatch(actions);
        }

        /// <summary>
        /// Returns the chord name of a set of actions.
        /// </summary>
        /// <param name="actions">Set of actions.</param>
        /// <returns>True if chord.</returns>
        public static string ChordName(string actions)
        {
            var match = ChordRegex.Match(actions);
            if (match.Success)
            {
                return match.Groups["actions"].Value;
            }

            return null;
        }

        /// <summary>
        /// Translate a display chord name (modifiers-and-key) to a profile chord name (key and modifiers).
        /// </summary>
        /// <param name="displayChord">profile chord name.</param>
        /// <returns>Display chord.</returns>
        /// <remarks>Turns Ctrl-Alt-Z to Z Ctrl Alt.</remarks>
        public static string ProfileChord(string displayChord)
        {
            if (displayChord == null)
            {
                return null;
            }

            var words = displayChord.Split('-');
            return string.Join(" ", words.Skip(words.Length - 1).Concat(words.Take(words.Length - 1)));
        }

        /// <summary>
        /// Translate a profile chord name (key and optional modifiers) to a display chord name (modifiers-and-key).
        /// </summary>
        /// <param name="profileChord">profile chord name.</param>
        /// <returns>Display chord.</returns>
        /// <remarks>Turns Z Alt Ctrl to Ctrl-Alt-Z.</remarks>
        public static string DisplayChord(string profileChord)
        {
            if (profileChord == null)
            {
                return null;
            }

            var words = profileChord.Split(' ');
            return string.Join("-", words.Skip(1).Concat(words.Take(1)));
        }

        /// <summary>
        /// Equality test for keypad maps.
        /// </summary>
        /// <param name="other">Other maps.</param>
        /// <returns>True if they are equal.</returns>
        public bool Equals(KeyMap<T> other)
        {
            return this.SequenceEqual(other);
        }

        /// <summary>
        /// Try to get the closest match for a particular modifier state, using the base chord.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="scanCodeName">Scan code name (optional).</param>
        /// <param name="modifier">Modifier state.</param>
        /// <param name="map">Returned map.</param>
        /// <param name="exact">Returned true if modifier was matched exactly.</param>
        /// <param name="matchedModifier">Returned modifiers of matched entry.</param>
        /// <param name="matchType">Returned match type.</param>
        /// <returns>True if a match is found.</returns>
        public bool TryGetClosestMatch(
            string keyName,
            string scanCodeName,
            KeyboardModifier modifier,
            out T map,
            out bool exact,
            out KeyboardModifier matchedModifier,
            out MatchType matchType)
        {
            return this.TryGetClosestMatch(keyName, scanCodeName, modifier, null, out map, out exact, out matchedModifier, out matchType);
        }

        /// <summary>
        /// Try to get the closest match for a particular modifier state, using the base chord.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="scanCodeName">Scan code name (optional).</param>
        /// <param name="modifier">Modifier state.</param>
        /// <param name="chord">Chord name.</param>
        /// <param name="map">Returned map.</param>
        /// <returns>True if a match is found.</returns>
        public bool TryGetClosestMatch(
            string keyName,
            string scanCodeName,
            KeyboardModifier modifier,
            string chord,
            out T map)
        {
            return this.TryGetClosestMatch(keyName, scanCodeName, modifier, chord, out map, out _, out _, out _);
        }

        /// <summary>
        /// Try to get the closest match for a particular modifier state.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="scanCodeName">Scan code name (optional).</param>
        /// <param name="modifier">Modifier state.</param>
        /// <param name="chord">Chord name.</param>
        /// <param name="map">Returned map.</param>
        /// <param name="exact">Returned true if modifier was matched exactly.</param>
        /// <param name="matchedModifier">Returned modifiers of matched entry.</param>
        /// <param name="matchType">Returned match type.</param>
        /// <returns>True if a match is found.</returns>
        public bool TryGetClosestMatch(
            string keyName,
            string scanCodeName,
            KeyboardModifier modifier,
            string chord,
            out T map,
            out bool exact,
            out KeyboardModifier matchedModifier,
            out MatchType matchType)
        {
            exact = false;
            matchType = MatchType.KeyCode;

            T scanCodeMap = null;
            var scanCodeExact = false;
            var scanCodeModifier = KeyboardModifier.None;
            var matchesScanCode = scanCodeName != null && this.ClosestMatch(scanCodeName, modifier, chord, out scanCodeMap, out scanCodeExact, out scanCodeModifier);
            var matchesKey = this.ClosestMatch(keyName, modifier, chord, out T keyMap, out bool keyExact, out KeyboardModifier keyModifier);

            if (!matchesScanCode && !matchesKey)
            {
                // No match.
                map = null;
                matchedModifier = KeyboardModifier.None;
                return false;
            }

            if (matchesScanCode && (scanCodeExact || !matchesKey || (int)scanCodeModifier >= (int)keyModifier))
            {
                // Scan code matches exactly, or scan code matches and key doesn't, or scan code match is better.
                map = scanCodeMap;
                exact = scanCodeExact;
                matchedModifier = scanCodeModifier;
                matchType = MatchType.ScanCode;
                return true;
            }

            // Key match.
            map = keyMap;
            exact = keyExact;
            matchedModifier = keyModifier;
            return true;
        }

        /// <summary>
        /// Merge two key maps. This means adding any elements in <paramref name="other"/> that do not
        /// exist in this map, and replacing any elements in this map that have different values in
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Other key map to merge.</param>
        /// <returns>True if map changed.</returns>
        public bool Merge(KeyMap<T> other)
        {
            var changed = false;
            foreach (var pair in other)
            {
                if (!this.TryGetValue(pair.Key, out T found) || !found.Equals(pair.Value))
                {
                    this[pair.Key] = pair.Value;
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>
        /// Returns the names of all of the chords defined in the map.
        /// </summary>
        /// <returns>Set of chord names.</returns>
        public IEnumerable<string> Chords()
        {
            // Accumulate the parameters to the uChord() action.
            var chords = new List<string>();
            foreach (var actions in this.Values.Select(m => m.Actions))
            {
                var match = ChordRegex.Match(actions);
                if (match.Success)
                {
                    chords.Add(match.Groups["actions"].Value);
                }
            }

            // Sort, make unique, and remap to human-readable format.
            return new[] { Settings.NoChord }.Concat(chords.OrderBy(c => c).Distinct().Select(DisplayChord));
        }

        /// <summary>
        /// Normalize the dictionary.
        /// </summary>
        public void Normalize()
        {
            // Walk the dictionary, translating any keysyms with ambiguous names to their normalized names.
            // The definition of a normalized name is whatever Enum.ToString returns for that value, which is
            // platform- (and possibly compiler-) dependent.
            // The assumption here is that you will always get the same string back for the same value. You
            // just don't know which of the possible values that will be until you ask the first time.
            var copy = new Dictionary<string, T>(this);
            this.Clear();
            foreach (var key in copy.Keys)
            {
                // Normalize the action, if it a chord.
                var actions = copy[key];
                var chord = ChordName(actions.Actions);
                if (chord != null)
                {
                    actions.Actions = Constants.Action.Chord + "(\"" + NormalizeKey(chord) + "\")";
                }

                // Store the updated mapping using a normalized key.
                this[NormalizeKey(key)] = actions;
            }
        }

        /// <summary>
        /// Normalizes a key.
        /// </summary>
        /// <param name="key">Key and modifiers.</param>
        /// <returns>Normalized key.</returns>
        private static string NormalizeKey(string key)
        {
            var keySplit = key.Split(' ');
            var candidate = true;
            for (var i = 0; i < keySplit.Length; i++)
            {
                if (candidate && !keySplit[i].StartsWith("Scan"))
                {
                    keySplit[i] = KeyboardUtil.ToStringExtended(KeyboardUtil.ParseKeysExtended(keySplit[i]));
                }

                candidate = keySplit[i] == ChordSeparator;
            }

            return string.Join(" ", keySplit);
        }

        /// <summary>
        /// Get a closest match for a given map mode.
        /// </summary>
        /// <param name="keyName">Key name.</param>
        /// <param name="modifier">Keyboard modifier.</param>
        /// <param name="chord">Chord name.</param>
        /// <param name="map">Returned map.</param>
        /// <param name="exact">Returned true if there was an exact match.</param>
        /// <param name="matchedModifier">Returned keyboard modifier.</param>
        /// <returns>True if there was a match.</returns>
        private bool ClosestMatch(
            string keyName,
            KeyboardModifier modifier,
            string chord,
            out T map,
            out bool exact,
            out KeyboardModifier matchedModifier)
        {
            map = null;
            exact = false;
            matchedModifier = KeyboardModifier.None;

            for (int i = (int)modifier; i >= 0; i--)
            {
                if (((int)modifier & i) == i && this.TryGetValue(Key(keyName, (KeyboardModifier)i, chord), out T tryMap))
                {
                    if (tryMap.Exact && (i & ExactModKeys) != ((int)modifier & ExactModKeys))
                    {
                        // Exact mappings need to match the exact modifiers, exactly.
                        continue;
                    }

                    map = tryMap;
                    exact = i == (int)modifier;
                    matchedModifier = (KeyboardModifier)i;
                    return true;
                }
            }

            return false;
        }
    }
}
