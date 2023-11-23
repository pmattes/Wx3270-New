// <copyright file="MiscSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Miscellaneous settings.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// The unknown settings.
        /// </summary>
        private readonly Dictionary<string, string> miscSettings = new Dictionary<string, string>();

        /// <summary>
        /// The text boxes for the unknown settings.
        /// </summary>
        private readonly Dictionary<string, TextBox> miscTextBox = new Dictionary<string, TextBox>();

        /// <summary>
        /// True if the initial table has been populated.
        /// </summary>
        private bool initialTableDone;

        /// <summary>
        /// Initialize the Miscellaneous tab.
        /// </summary>
        public void MiscTabInit()
        {
            if (this.BackEnd.Ready)
            {
                this.PopulateMiscInitial();
            }
            else
            {
                this.BackEnd.OnReady += this.PopulateMiscInitial;
            }

            this.ProfileManager.AddChangeTo(this.MiscProfileChanged);
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
            if (!inProfile && this.app.SettingChange.DefaultsDictionary.TryGetValue(name, out string defaultValue) && text == defaultValue)
            {
                // Nothing to do (kept default).
                return;
            }

            if (inProfile && text == profileValue)
            {
                // Nothing to do (kept last value).
                return;
            }

            this.ProfileManager.PushAndSave(
                profile =>
                {
                    if (inProfile && this.app.SettingChange.DefaultsDictionary.TryGetValue(name, out string defaultValue) && text == defaultValue)
                    {
                        profile.MiscSettings.Remove(name);
                    }
                    else
                    {
                        profile.MiscSettings[name] = text;
                    }
                },
                I18n.Get(SettingPath(ChangeKeyword.Misc)));
        }

        /// <summary>
        /// Add an entry to the miscellaneous settings table.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="value">Setting value.</param>
        private void AddEntry(string name, string value)
        {
            var label = new Label()
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Location = new System.Drawing.Point(3, 6),
                Name = name + "Label",
                Size = new System.Drawing.Size(102, 13),
                TabIndex = this.miscSettings.Count,
                Text = name,
            };
            this.miscellaneousTableLayoutPanel.Controls.Add(label, 0, this.miscSettings.Count);

            var textBox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(181, 3),
                Name = name + "TextBox",
                Size = new System.Drawing.Size(173, 20),
                TabIndex = this.miscSettings.Count,
                Text = value,
                Tag = name,
            };
            textBox.Validated += new EventHandler(this.MiscTextValidated);
            this.miscellaneousTableLayoutPanel.Controls.Add(textBox, 1, this.miscSettings.Count);
            this.miscTextBox[name] = textBox;
            this.miscSettings[name] = value;
            this.miscellaneousTableLayoutPanel.RowCount = this.miscSettings.Count;
        }

        /// <summary>
        /// Populate the initial table.
        /// </summary>
        private void PopulateMiscInitial()
        {
            if (this.initialTableDone)
            {
                return;
            }

            this.initialTableDone = true;

            foreach (var name in this.app.SettingChange.UnknownSettings)
            {
                var defaultValue = string.Empty;
                this.app.SettingChange.DefaultsDictionary.TryGetValue(name, out defaultValue);
                this.AddEntry(name, defaultValue);
            }
        }

        /// <summary>
        /// The profile changed.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void MiscProfileChanged(Profile oldProfile, Profile newProfile)
        {
            this.miscellaneousTab.Enabled = newProfile.ProfileType == ProfileType.Full;

            foreach (var setting in newProfile.MiscSettings)
            {
                if (this.miscSettings.ContainsKey(setting.Key))
                {
                    // Update the UI.
                    this.miscTextBox[setting.Key].Text = setting.Value;
                    this.miscSettings[setting.Key] = setting.Value;
                }
            }

            if (oldProfile != null)
            {
                // Set items that were deleted from the profile to their default values.
                foreach (var setting in oldProfile.MiscSettings)
                {
                    if (this.miscSettings.ContainsKey(setting.Key) && !newProfile.MiscSettings.ContainsKey(setting.Key))
                    {
                        this.app.SettingChange.DefaultsDictionary.TryGetValue(setting.Key, out string defaultValue);
                        this.miscTextBox[setting.Key].Text = defaultValue;
                        this.miscSettings[setting.Key] = defaultValue;
                    }
                }
            }
        }
    }
}
