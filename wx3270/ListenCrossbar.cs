// <copyright file="ListenCrossbar.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using Wx3270.Contracts;
    using static Wx3270.Settings;

    /// <summary>
    /// Crossbar between the back end and the profile for listen settings.
    /// </summary>
    public class ListenCrossbar
    {
        /// <summary>
        /// The settings supported by this module.
        /// </summary>
        private readonly List<string> settings = new List<string>
        {
            B3270.Setting.Httpd,
            B3270.Setting.ScriptPort,
        };

        /// <summary>
        /// Back-end initialization is complete.
        /// </summary>
        private bool ready;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenCrossbar"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="profileManager">Profile manager.</param>
        /// <param name="mainScreen">Main screen.</param>
        public ListenCrossbar(Wx3270App app, IProfileManager profileManager)
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
            foreach (var setting in this.settings)
            {
                if (app.SettingChange.SettingsDictionary.TryGetValue(setting, out string _))
                {
                    this.BackEndToProfile(setting, app.SettingChange.SettingsDictionary);
                }
            }

            app.SettingChange.Register(this.BackEndToProfile, this.settings);
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
        /// Merge the listen settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.OtherSettingsReplace)]
        public static bool Merge(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.ListenPort.SequenceEqual(fromProfile.ListenPort))
            {
                return false;
            }

            foreach (var kv in fromProfile.ListenPort)
            {
                toProfile.ListenPort[kv.Key] = (ListenPort)kv.Value.Clone();
            }

            return true;
        }

        /// <summary>
        /// Format a listen parameter for the back end.
        /// </summary>
        /// <param name="address">Listen address.</param>
        /// <param name="port">Listen port.</param>
        /// <returns>Formatter parameter.</returns>
        private static string ListenParam(IPAddress address, ushort port)
        {
            return "[" + address + "]:" + port;
        }

        /// <summary>
        /// The profile has changed. Update the back end.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileChanged(Profile oldProfile, Profile newProfile)
        {
            var newSettings = new List<string>();

            foreach (var portName in this.settings)
            {
                if (!this.App.ListenLock[portName])
                {
                    ListenPort oldListenPort = null;
                    if (oldProfile != null && !oldProfile.ListenPort.TryGetValue(portName, out oldListenPort))
                    {
                        oldListenPort = null;
                    }

                    if (!newProfile.ListenPort.TryGetValue(portName, out ListenPort listenPort))
                    {
                        listenPort = null;
                    }

                    if ((oldListenPort == null && listenPort == null) || (oldListenPort != null && oldListenPort.Equals(listenPort)))
                    {
                        continue;
                    }

                    var toggleValue = listenPort != null ? ListenParam(listenPort.Address, listenPort.Port) : string.Empty;
                    newSettings.AddRange(new[] { portName, toggleValue });
                }
            }

            if (newSettings.Count > 0)
            {
                this.App.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, newSettings),
                    ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// A back-end setting changed. Send it to the profile.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="settingDictionary">Settings dictionary (values).</param>
        private void BackEndToProfile(string name, SettingsDictionary settingDictionary)
        {
            if (!this.ready || !settingDictionary.TryGetValue(name, out string value))
            {
                return;
            }

            ListenPort newListenPort = null;
            if (!string.IsNullOrEmpty(value))
            {
                var regex = new Regex(@"\[(?<address>.*)\]:(?<port>.*)");
                var match = regex.Match(value);
                if (!match.Success)
                {
                    ErrorBox.Show(string.Format(I18n.Get(Message.CantParseListen), name, value), I18n.Get(Title.Settings));
                    return;
                }

                newListenPort = new ListenPort { Address = IPAddress.Parse(match.Groups["address"].Value), Port = ushort.Parse(match.Groups["port"].Value) };
            }

            if (this.App.ListenLock[name])
            {
                // Listener is locked.
                return;
            }

            if (!this.ProfileManager.Current.ListenPort.TryGetValue(name, out ListenPort oldListenPort))
            {
                oldListenPort = null;
            }

            if (newListenPort == null)
            {
                if (oldListenPort != null)
                {
                    this.ProfileManager.PushAndSave(
                        (profile) => profile.ListenPort.Remove(name),
                        Wx3270.ProfileManager.DisableName(name + SettingPath(ChangeKeyword.ListeningPort) + " (" + this.ProfileManager.ExternalText + ")"));
                }
            }
            else
            {
                this.ProfileManager.PushAndSave(
                    (profile) => profile.ListenPort[name] = newListenPort,
                    Wx3270.ProfileManager.ChangeName(name + SettingPath(ChangeKeyword.ListeningPort) + " (" + this.ProfileManager.ExternalText + ")"));
            }
        }
    }
}