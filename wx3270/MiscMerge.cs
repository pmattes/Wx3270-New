// <copyright file="MiscMerge.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Linq;

    /// <summary>
    /// Miscellaneous merge methods.
    /// </summary>
    /// <remarks>
    /// Propagates color profile changes to elements outside of the Settings dialog,
    /// and does color merges.
    /// </remarks>
    public class MiscMerge
    {
        /// <summary>
        /// Merge the color settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.ColorsReplace)]
        public static bool MergeColors(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (!toProfile.Colors.Equals(fromProfile.Colors))
            {
                toProfile.Colors = fromProfile.Colors;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Merge the hosts.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.HostsMerge | ImportType.HostsReplace)]
        public static bool MergeHosts(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.HostsReplace))
            {
                // Replace host definitions.
                if (!toProfile.Hosts.SequenceEqual(fromProfile.Hosts))
                {
                    toProfile.Hosts = fromProfile.Hosts.Select(host =>
                    {
                        var clone = host.Clone();
                        clone.Profile = toProfile;
                        return clone;
                    });
                    return true;
                }

                return false;
            }
            else
            {
                // Merge host definitions.
                var changed = false;
                var newHosts = toProfile.Hosts.ToDictionary(h => h.Name);
                foreach (var mergeHost in fromProfile.Hosts)
                {
                    if (!newHosts.TryGetValue(mergeHost.Name, out HostEntry found) || !mergeHost.Equals(found))
                    {
                        newHosts[mergeHost.Name] = mergeHost;
                        changed = true;
                    }
                }

                if (changed)
                {
                    toProfile.Hosts = newHosts.Values.Select(host =>
                    {
                        var clone = host.Clone();
                        clone.Profile = toProfile;
                        return clone;
                    });
                }

                return changed;
            }
        }

        /// <summary>
        /// Merge the keypad settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.KeypadMerge | ImportType.KeypadReplace)]
        public static bool MergeKeypad(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.KeypadReplace))
            {
                // Replace keypad definitions.
                if (!toProfile.KeypadMap.Equals(fromProfile.KeypadMap))
                {
                    toProfile.KeypadMap = fromProfile.KeypadMap;
                    return true;
                }

                return false;
            }
            else
            {
                // Merge keypad definitions.
                return toProfile.KeypadMap.Merge(fromProfile.KeypadMap);
            }
        }

        /// <summary>
        /// Merge the macros.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.MacrosMerge | ImportType.MacrosReplace)]
        public static bool MergeMacros(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.MacrosReplace))
            {
                // Replace macros.
                if (!toProfile.Macros.SequenceEqual(fromProfile.Macros))
                {
                    toProfile.Macros = fromProfile.Macros;
                    return true;
                }

                return false;
            }
            else
            {
                // Merge macros.
                var changed = false;
                var newList = toProfile.Macros.ToList();
                foreach (var macro in fromProfile.Macros)
                {
                    var existing = newList.FirstOrDefault(m => m.Name.Equals(macro.Name));
                    if (existing == null)
                    {
                        // New name.
                        newList.Add(macro);
                        changed = true;
                    }
                    else
                    {
                        // Same name, maybe different value.
                        if (!existing.Equals(macro))
                        {
                            existing.Macro = macro.Macro;
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    toProfile.Macros = newList;
                }

                return changed;
            }
        }

        /// <summary>
        /// Merge the sound settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.OtherSettingsReplace)]
        public static bool MergeSound(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.KeyClick == fromProfile.KeyClick && toProfile.AudibleBell == fromProfile.AudibleBell)
            {
                return false;
            }

            toProfile.KeyClick = fromProfile.KeyClick;
            toProfile.AudibleBell = fromProfile.AudibleBell;
            return true;
        }

        /// <summary>
        /// Merge the font settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.FontReplace)]
        public static bool MergeFont(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (!toProfile.Font.Equals(fromProfile.Font))
            {
                toProfile.Font = fromProfile.Font;
                toProfile.Size = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Merge the keyboard settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.KeyboardMerge | ImportType.KeyboardReplace)]
        public static bool MergeKeyboard(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.KeyboardReplace))
            {
                // Replace keyboard definitions.
                if (!toProfile.KeyboardMap.Equals(fromProfile.KeyboardMap))
                {
                    toProfile.KeyboardMap = fromProfile.KeyboardMap;
                    return true;
                }

                return false;
            }
            else
            {
                // Merge keyboard definitions.
                return toProfile.KeyboardMap.Merge(fromProfile.KeyboardMap);
            }
        }
    }
}