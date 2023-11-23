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
        /// Initialize the Proxy tab.
        /// </summary>
        public void ProxyTabInit()
        {
            // Register for profile changes.
            this.ProfileManager.AddChangeTo(this.ProxyProfileChanged);

            // Create a dummy proxy editor, to get it localized.
            new ProxyEditor(Profile.DefaultProfile.Proxy, this.app.ProxiesDb, null).Dispose();
        }

        /// <summary>
        /// Set the enable state for the proxy fields.
        /// </summary>
        private void SetProxyEnable()
        {
            var type = this.proxyTypeTextBox.Text;
            var isNone = type == Proxy.None;
            var proxy = this.app.ProxiesDb.Proxies[type];
            this.addressTextBox.Enabled = !isNone;
            this.portTextBox.Enabled = !isNone;
            this.usernameTextBox.Enabled = !isNone && proxy.TakesUsername;
            this.passwordTextBox.Enabled = !isNone && proxy.TakesUsername;
        }

        /// <summary>
        /// Process proxy options in a new profile.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProxyProfileChanged(Profile oldProfile, Profile newProfile)
        {
            this.proxyTab.Enabled = newProfile.ProfileType == ProfileType.Full;
            var proxy = newProfile.Proxy;

            // Set the data fields in the UI.
            this.proxyTypeTextBox.Text = proxy.Type ?? Proxy.None;
            this.addressTextBox.Text = proxy.Address ?? string.Empty;
            this.portTextBox.Text = proxy.Port.HasValue ? proxy.Port.ToString() : string.Empty;
            this.usernameTextBox.Text = proxy.Username ?? string.Empty;
            this.passwordTextBox.Text = proxy.Password ?? string.Empty;
            this.SetProxyEnable();
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
            using (var editor = new ProxyEditor(this.ProfileManager.Current.Proxy, this.app.ProxiesDb, tag))
            {
                // Pop up the dialog.
                if (editor.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                // Change the profile and the back end.
                var proxy = editor.ProxyValue;
                this.ProfileManager.PushAndSave(
                    (current) =>
                    {
                        current.Proxy = (Profile.ProxyClass)proxy.Clone();
                    },
                    ChangeName(B3270.Setting.Proxy));
            }
        }
    }
}
