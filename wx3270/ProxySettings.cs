// <copyright file="ProxySettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Settings for proxy.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Proxies database.
        /// </summary>
        private readonly ProxiesDb proxiesDb = new ProxiesDb();

        /// <summary>
        /// Initialize the Proxy tab.
        /// </summary>
        public void ProxyTabInit()
        {
            // Set up the proxies database.
            this.BackEnd.Register(this.proxiesDb);

            // Register for profile changes and merges.
            this.ProfileManager.Change += this.ProxyProfileChanged;
            this.ProfileManager.RegisterMerge(ImportType.OtherSettingsReplace, this.MergeProxy);

            // Subscribe to setting changes.
            this.app.SettingChange.Register(
                (settingName, settingDictionary) => this.Invoke(new MethodInvoker(() => this.ProxySettingChanged(settingName, settingDictionary))),
                new[] { B3270.Setting.Proxy });

            // Create a dummy proxy editor, to get it localized.
            new ProxyEditor(Profile.DefaultProfile.Proxy, this.proxiesDb, null).Dispose();
        }

        /// <summary>
        /// Return a string or null.
        /// </summary>
        /// <param name="s">String to consider.</param>
        /// <returns>String or null.</returns>
        private static string MaybeNull(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            return s;
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
            if (string.IsNullOrEmpty(type) || type == Proxy.None || !this.proxiesDb.Proxies.ContainsKey(type))
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
        /// Set the enable state for the proxy fields.
        /// </summary>
        private void SetProxyEnable()
        {
            var type = this.proxyTypeTextBox.Text;
            var isNone = type == Proxy.None;
            var proxy = this.proxiesDb.Proxies[type];
            this.addressTextBox.Enabled = !isNone;
            this.portTextBox.Enabled = !isNone;
            this.usernameTextBox.Enabled = !isNone && proxy.TakesUsername;
            this.passwordTextBox.Enabled = !isNone && proxy.TakesUsername;
        }

        /// <summary>
        /// Process proxy options in a new profile.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void ProxyProfileChanged(Profile profile)
        {
            this.proxyTab.Enabled = profile.ProfileType == ProfileType.Full;

            var proxy = profile.Proxy;

            // Set the data fields in the UI.
            this.proxyTypeTextBox.Text = proxy.Type ?? Proxy.None;
            this.addressTextBox.Text = proxy.Address ?? string.Empty;
            this.portTextBox.Text = proxy.Port.HasValue ? proxy.Port.ToString() : string.Empty;
            this.usernameTextBox.Text = proxy.Username ?? string.Empty;
            this.passwordTextBox.Text = proxy.Password ?? string.Empty;
            this.SetProxyEnable();

            // Tell the emulator.
            this.BackEnd.RunAction(
                new BackEndAction(B3270.Action.Set, B3270.Setting.Proxy, this.ProxyValue(proxy.Type, proxy.Address, proxy.Port, proxy.Username, proxy.Password)),
                ErrorBox.Completion(I18n.Get(Title.Settings)));
        }

        /// <summary>
        /// Merge the proxy settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        private bool MergeProxy(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.Proxy.Equals(fromProfile.Proxy))
            {
                return false;
            }

            toProfile.Proxy = (Profile.ProxyClass)fromProfile.Proxy.Clone();
            return true;
        }

        /// <summary>
        /// The proxy setting changed.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Settings dictionary.</param>
        private void ProxySettingChanged(string settingName, SettingsDictionary settingDictionary)
        {
            if (!this.ready || settingName != B3270.Setting.Proxy)
            {
                return;
            }

            if (settingDictionary.TryGetValue(B3270.Setting.Proxy, out string proxy))
            {
                var parser = new ProxyParser(this.proxiesDb.Proxies.Keys);
                if (!parser.TryParse(
                    proxy,
                    out string proxyType,
                    out string address,
                    out string port,
                    out string username,
                    out string password,
                    out string failReason,
                    nullIsNone: true))
                {
                    ErrorBox.Show(I18n.Get(Message.InvalidProxySetting) + ": " + failReason, I18n.Get(Title.Settings));
                    return;
                }

                if (this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        current.Proxy.Type = proxyType;
                        current.Proxy.Address = address;
                        current.Proxy.Port = (port != null) ? ushort.Parse(port) : (int?)null;
                        current.Proxy.Username = username;
                        current.Proxy.Password = password;
                    },
                    this.ChangeName(B3270.Setting.Proxy)))
                {
                    this.proxyTypeTextBox.Text = proxyType ?? Proxy.None;
                    this.addressTextBox.Text = address ?? string.Empty;
                    this.portTextBox.Text = port ?? string.Empty;
                    this.usernameTextBox.Text = username ?? string.Empty;
                    this.passwordTextBox.Text = password ?? string.Empty;
                    this.SetProxyEnable();
                }
            }
        }

        /// <summary>
        /// The proxy type text box became active.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyTypeTextBoxEnter(object sender, EventArgs e)
        {
            this.proxyTab.Focus();
        }

        /// <summary>
        /// The proxy edit button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyEditButtonClick(object sender, EventArgs e)
        {
            var tag = (sender as Control).Tag as string;
            using (var editor = new ProxyEditor(this.ProfileManager.Current.Proxy, this.proxiesDb, tag))
            {
                // Pop up the dialog.
                if (editor.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                // Change the profile and the back end.
                var proxy = editor.ProxyValue;
                if (this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        current.Proxy = (Profile.ProxyClass)proxy.Clone();
                    },
                    this.ChangeName(B3270.Setting.Proxy)))
                {
                    this.BackEnd.RunAction(
                    new BackEndAction(
                        B3270.Action.Set,
                        B3270.Setting.Proxy,
                        this.ProxyValue(proxy.Type, proxy.Address, proxy.Port, proxy.Username, proxy.Password)),
                    ErrorBox.Completion(I18n.Get(Title.Settings)));
                }

                // Change the UI.
                this.proxyTypeTextBox.Text = proxy.Type ?? Proxy.None;
                this.addressTextBox.Text = proxy.Address ?? string.Empty;
                this.portTextBox.Text = proxy.Port.HasValue ? proxy.Port.ToString() : string.Empty;
                this.usernameTextBox.Text = proxy.Username ?? string.Empty;
                this.passwordTextBox.Text = proxy.Password ?? string.Empty;
                this.SetProxyEnable();
            }
        }
    }
}
