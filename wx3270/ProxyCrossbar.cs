// <copyright file="ProxyCrossbar.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using Wx3270.Contracts;
    using static Wx3270.Settings;

    /// <summary>
    /// Crossbar between the back end and the profile for proxy settings.
    /// </summary>
    public class ProxyCrossbar
    {
        /// <summary>
        /// Back-end initialization is complete.
        /// </summary>
        private bool ready;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyCrossbar"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="profileManager">Profile manager.</param>
        public ProxyCrossbar(Wx3270App app, IProfileManager profileManager)
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
            var settings = new[]
            {
                B3270.Setting.Proxy,
            };
            foreach (var setting in settings)
            {
                if (app.SettingChange.SettingsDictionary.TryGetValue(setting, out string _))
                {
                    this.BackEndToProfile(setting, app.SettingChange.SettingsDictionary);
                }
            }

            app.SettingChange.Register(this.BackEndToProfile, settings);
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
        /// Merge the proxy settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.OtherSettingsReplace)]
        public static bool Merge(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.Proxy.Equals(fromProfile.Proxy))
            {
                return false;
            }

            toProfile.Proxy = (Profile.ProxyClass)fromProfile.Proxy.Clone();
            return true;
        }

        /// <summary>
        /// Construct a proxy resource value.
        /// </summary>
        /// <param name="type">Proxy type.</param>
        /// <param name="address">Proxy address.</param>
        /// <param name="port">Proxy port.</param>
        /// <param name="username">Proxy username.</param>
        /// <param name="password">Proxy password.</param>
        /// <returns>Resource value.</returns>
        private string ProxyValue(string type, string address, int? port, string username, string password)
        {
            if (string.IsNullOrEmpty(type) || type == Proxy.None || !this.App.ProxiesDb.Proxies.ContainsKey(type))
            {
                return string.Empty;
            }

            var lbrack = string.Empty;
            var rbrack = string.Empty;
            if (address.Contains(":"))
            {
                lbrack = "[";
                rbrack = "]";
            }

            var ret = type + ":";
            if (!string.IsNullOrEmpty(username))
            {
                ret += username;
                if (!string.IsNullOrEmpty(password))
                {
                    ret += ":" + password;
                }

                ret += "@";
            }

            ret += lbrack + address + rbrack;
            if (port.HasValue)
            {
                ret += ":" + port.ToString();
            }

            return ret;
        }

        /// <summary>
        /// The profile has changed. Update the back end.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileChanged(Profile oldProfile, Profile newProfile)
        {
            var newSettings = new List<string>();

            if (oldProfile == null || !oldProfile.Proxy.Equals(newProfile.Proxy))
            {
                var proxy = newProfile.Proxy;
                newSettings.AddRange(new[] { B3270.Setting.Proxy, this.ProxyValue(proxy.Type, proxy.Address, proxy.Port, proxy.Username, proxy.Password) });
            }

            // Tell the back end about everything that should succeed (was created programmatically), which in general should not fail.
            if (newSettings.Count > 0)
            {
                this.App.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, newSettings), ErrorBox.Completion(I18n.Get(Settings.Title.Settings)));
            }
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

            switch (name)
            {
                case B3270.Setting.Proxy:
                    var parser = new ProxyParser(this.App.ProxiesDb.Proxies.Keys);
                    if (!parser.TryParse(
                        stringValue,
                        out string proxyType,
                        out string address,
                        out string port,
                        out string username,
                        out string password,
                        out string failReason,
                        nullIsNone: true))
                    {
                        ErrorBox.Show(I18n.Get(Settings.Message.InvalidProxySetting) + ": " + failReason, I18n.Get(Title.Settings));
                        return;
                    }

                    this.ProfileManager.PushAndSave(
                        (current) =>
                        {
                            current.Proxy.Type = proxyType;
                            current.Proxy.Address = address;
                            current.Proxy.Port = (port != null) ? ushort.Parse(port) : (int?)null;
                            current.Proxy.Username = username;
                            current.Proxy.Password = password;
                        },
                        ChangeName(B3270.Setting.Proxy));
                    break;
            }
        }
    }
}