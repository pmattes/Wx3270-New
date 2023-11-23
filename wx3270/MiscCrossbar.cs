// <copyright file="MiscCrossbar.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Wx3270.Contracts;
    using static Wx3270.Settings;

    /// <summary>
    /// Crossbar between the back end and the profile for miscellaneous settings.
    /// </summary>
    public class MiscCrossbar
    {
        /// <summary>
        /// Back-end initialization is complete.
        /// </summary>
        private bool ready;

        /// <summary>
        /// True if a profile change is being processed.
        /// </summary>
        private bool processingRollback = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiscCrossbar"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="profileManager">Profile manager.</param>
        public MiscCrossbar(Wx3270App app, IProfileManager profileManager)
        {
            this.App = app;
            this.ProfileManager = profileManager;

            // Subscribe to the ready event.
            if (!(this.ready = this.App.BackEnd.Ready))
            {
                this.App.BackEnd.OnReady += () => { this.ready = true; };
            }

            // Subscribe to profile change events.
            profileManager.AddChangeTo(this.ProfileChanged);

            // Subscribe to back-end changes; push to the profile.
            foreach (var setting in app.SettingChange.SettingsDictionary.Keys)
            {
                if (app.SettingChange.SettingsDictionary.TryGetValue(setting, out string _))
                {
                    this.BackEndToProfile(setting, app.SettingChange.SettingsDictionary);
                }
            }

            app.SettingChange.Register(this.BackEndToProfile);
        }

        /// <summary>
        /// Gets or sets the application context.
        /// </summary>
        private Wx3270App App { get; set; }

        /// <summary>
        /// Gets or sets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager { get; set; }

        /// <summary>
        /// Merge the miscellaneous settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.OtherSettingsReplace)]
        public static bool Merge(Profile toProfile, Profile fromProfile, ImportType importType)
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

        /// <summary>
        /// The profile has changed. Update the back end.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileChanged(Profile oldProfile, Profile newProfile)
        {
            if (this.processingRollback)
            {
                // This code generated this profile change, to roll back the previous change. Don't process it again.
                return;
            }

            var newSettings = new List<string>();

            if (oldProfile == null || !oldProfile.MiscSettings.SequenceEqual(newProfile.MiscSettings))
            {
                foreach (var item in newProfile.MiscSettings)
                {
                    if (oldProfile == null || !oldProfile.MiscSettings.TryGetValue(item.Key, out var value) || value != item.Value)
                    {
                        // Item added or changed.
                        newSettings.AddRange(new[] { item.Key, item.Value });
                    }
                }

                if (oldProfile != null)
                {
                    foreach (var item in oldProfile.MiscSettings)
                    {
                        if (!newProfile.MiscSettings.ContainsKey(item.Key))
                        {
                            // Item removed.
                            this.App.SettingChange.DefaultsDictionary.TryGetValue(item.Key, out string defaultValue);
                            newSettings.AddRange(new[] { item.Key, defaultValue });
                        }
                    }
                }
            }

            // Tell the back end about settings that the user generated.
            while (newSettings.Count > 0)
            {
                var settingName = newSettings.First();
                var settingValue = newSettings.Skip(1).First();
                newSettings = newSettings.Skip(2).ToList();
                this.App.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, settingName, settingValue),
                    (cookie, success, result, misc) =>
                    {
                        if (!success)
                        {
                            ErrorBox.Show(result, I18n.Get(Settings.Title.Settings));
                            try
                            {
                                this.processingRollback = true;
                                this.ProfileManager.PushAndSave(
                                    (current) =>
                                    {
                                        if (oldProfile != null && oldProfile.MiscSettings.TryGetValue(settingName, out string oldValue))
                                        {
                                            current.MiscSettings[settingName] = oldValue;
                                        }
                                        else
                                        {
                                            current.MiscSettings.Remove(settingName);
                                        }
                                    },
                                    Wx3270.ProfileManager.ChangeName(I18n.Get(Settings.SettingPath(ChangeKeyword.Misc))));
                            }
                            finally
                            {
                                this.processingRollback = false;
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Get the change name for a setting.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <returns>Change name.</returns>
        private string GetChangeName(string name)
        {
            return Wx3270.ProfileManager.ChangeName(string.Format("{0} ({1})", I18n.Get(Settings.SettingPath(name)), this.ProfileManager.ExternalText));
        }

        /// <summary>
        /// A back-end setting changed. Send it to the profile.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="settingDictionary">Settings dictionary (values).</param>
        private void BackEndToProfile(string name, SettingsDictionary settingDictionary)
        {
            if (!this.ready || !settingDictionary.TryGetValue(name, out string stringValue))
            {
                return;
            }

            if (!this.App.KnownSettings.Contains(name))
            {
                this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        if (current.MiscSettings.TryGetValue(name, out var value)
                            && this.App.SettingChange.DefaultsDictionary.TryGetValue(name, out string defaultValue)
                            && stringValue == defaultValue)
                        {
                            current.MiscSettings.Remove(name);
                        }
                        else
                        {
                            current.MiscSettings[name] = stringValue;
                        }
                    },
                    this.GetChangeName(ChangeKeyword.Misc));
            }
        }
    }
}