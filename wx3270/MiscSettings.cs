// <copyright file="MiscSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// Miscellaneous settings.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// The unknown settings.
        /// </summary>
        private readonly Dictionary<string, string> unknownSettings = new Dictionary<string, string>();

        /// <summary>
        /// The text boxes for the unknown settings.
        /// </summary>
        private readonly Dictionary<string, TextBox> miscTextBox = new Dictionary<string, TextBox>();

        /// <summary>
        /// The values last sent for the unknown settings.
        /// </summary>
        private readonly Dictionary<string, string> lastSentSettings = new Dictionary<string, string>();

        /// <summary>
        /// The known settings.
        /// </summary>
        private HashSet<string> knownSettings;

        /// <summary>
        /// Initialize the Miscellaneous tab.
        /// </summary>
        public void MiscTabInit()
        {
            var known = typeof(B3270.Setting)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(info => info.IsLiteral && !info.IsInitOnly)
                .Select(info => info.GetRawConstantValue())
                .OfType<string>();
            this.knownSettings = new HashSet<string>(known);
            this.app.SettingChange.Register(this.MiscSetting);
            this.ProfileManager.Change += this.MiscProfileChanged;
            this.ProfileManager.RegisterMerge(ImportType.OtherSettingsReplace, this.MergeMisc);
        }

        private static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        /// <summary>
        /// A miscellaneous setting changed.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="settingDictionary">Setting dictionary.</param>
        private void MiscSetting(string name, SettingsDictionary settingDictionary)
        {
            if (this.knownSettings.Contains(name) || this.unknownSettings.ContainsKey(name))
            {
                return;
            }

            if (!settingDictionary.TryGetValue(name, out string value))
            {
                value = string.Empty;
            }

            // Programmatically add a row to the miscellaneous settings table.
            var label = new Label()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Location = new System.Drawing.Point(3, 6),
                Name = name + "Label",
                Size = new System.Drawing.Size(102, 13),
                TabIndex = this.unknownSettings.Count,
                Text = name,
            };
            this.miscellaneousTableLayoutPanel.Controls.Add(label, 0, this.unknownSettings.Count);

            var textBox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(181, 3),
                Name = name + "TextBox",
                Size = new System.Drawing.Size(173, 20),
                TabIndex = this.unknownSettings.Count,
                Text = value,
                Tag = name,
            };
            textBox.Validated += new EventHandler(this.MiscTextValidated);
            this.miscellaneousTableLayoutPanel.Controls.Add(textBox, 1, this.unknownSettings.Count);
            this.miscTextBox[name] = textBox;

            // Save the value only once, when we first learn of the setting. That makes the values in the
            // dictionary the defaults.
            this.unknownSettings[name] = value;
            this.miscellaneousTableLayoutPanel.RowCount = this.unknownSettings.Count;

            // Remember what the last value was, so we don't set it again.
            this.lastSentSettings[name] = value;
        }

        /// <summary>
        /// A miscellaneous setting was validated (control was exited).
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Event arguments.</param>
        private void MiscTextValidated(object sender, EventArgs args)
        {
            if (!(sender is TextBox textBox))
            {
                return;
            }

            var name = textBox.Tag as string;
            var text = textBox.Text;
            var inProfile = this.ProfileManager.Current.MiscSettings.TryGetValue(name, out string profileValue);

            // If there is nothing in the profile for this setting and the UI value matches the default, there is nothing to do.
            if (!inProfile && text == this.unknownSettings[name])
            {
                return;
            }

            if (!inProfile || profileValue != text)
            {
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, name, textBox.Text),
                    (cookie, success, result, misc) =>
                    {
                        if (success)
                        {
                            // Successfully set, update the profile.
                            this.ProfileManager.PushAndSave(profile => profile.MiscSettings[name] = text, I18n.Get(SettingPath(ChangeKeyword.Misc)));
                        }
                        else
                        {
                            // Unsuccessful, revert.
                            textBox.Text = inProfile ? profileValue : this.unknownSettings[name];
                            ErrorBox.Show(result, I18n.Get(Title.Settings));
                        }
                    });
            }
        }

        /// <summary>
        /// The profile changed.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void MiscProfileChanged(Profile profile)
        {
            this.miscellaneousTab.Enabled = profile.ProfileType == ProfileType.Full;

            // Set each miscellaneous value to what's in the new profile, or to the initial defaults we were given.
            var settings = new List<string>();
            var textValues = new List<Tuple<TextBox, string>>();
            foreach (var kv in this.unknownSettings)
            {
                if (!profile.MiscSettings.TryGetValue(kv.Key, out string value))
                {
                    value = kv.Value;
                }

                // Accumulate parameters for a Set action.
                if (!this.lastSentSettings.TryGetValue(kv.Key, out string lastSent) || lastSent != value)
                {
                    settings.AddRange(new[] { kv.Key, value });

                    // Remember what we last sent.
                    this.lastSentSettings[kv.Key] = value;
                }

                // Change the text box in the UI.
                if (this.miscTextBox.TryGetValue(kv.Key, out TextBox textBox))
                {
                    textBox.Text = value;
                }
            }

            if (settings.Any())
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, settings), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// Merge the miscellaneous settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        private bool MergeMisc(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.MiscSettings.SequenceEqual(fromProfile.MiscSettings))
            {
                return false;
            }

            toProfile.MiscSettings.Clear();
            foreach (var kv in fromProfile.MiscSettings)
            {
                toProfile.MiscSettings[kv.Key] = kv.Value;
            }

            return true;
        }
    }
}
