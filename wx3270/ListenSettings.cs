// <copyright file="ListenSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// Settings for listening ports.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// The default s3270 listening port.
        /// </summary>
        private const ushort S3270DefaultPort = 4000;

        /// <summary>
        /// The default HTTP listening port.
        /// </summary>
        private const ushort HttpdDefaultPort = 8080;

        /// <summary>
        /// The tag for the address text boxes.
        /// </summary>
        private const string AddressTag = "address";

        /// <summary>
        /// The tag gfor the port text boxes.
        /// </summary>
        private const string PortTag = "port";

        /// <summary>
        /// The default listening port values.
        /// </summary>
        private readonly Dictionary<string, ushort> defaultPort = new Dictionary<string, ushort>
        {
            { B3270.Setting.ScriptPort, S3270DefaultPort },
            { B3270.Setting.Httpd, HttpdDefaultPort },
        };

        private readonly Dictionary<string, string> displayType = new Dictionary<string, string>
        {
            { B3270.Setting.ScriptPort, "s3270" },
            { B3270.Setting.Httpd, "HTTP" },
        };

        /// <summary>
        /// Gets the listening port names.
        /// </summary>
        private IEnumerable<string> PortNames => this.defaultPort.Keys;

        /// <summary>
        /// Initialize the Listen tab.
        /// </summary>
        public void ListenTabInit()
        {
            foreach (var portName in this.PortNames)
            {
                if (this.app.ListenLock[portName])
                {
                    var groupBox = this.serversTab.Controls.OfType<GroupBox>().Where(g => (string)g.Tag == portName).First();
                    groupBox.Enabled = false;
                    groupBox.Controls.OfType<Label>().Where(l => (string)l.Tag == "caveat").First().Visible = true;
                }
            }

            this.ProfileManager.ChangeTo += this.ListenProfileChange;
            this.ProfileManager.RegisterMerge(ImportType.OtherSettingsReplace, this.MergeListen);
            this.app.SettingChange.Register(
                (settingName, settingDictonary) => this.Invoke(new MethodInvoker(() => this.ListenSettingChanged(settingName, settingDictonary))),
                this.PortNames.ToArray());

            // Set up a dummy listener editor for localization.
            new ServerEditor(string.Empty, string.Empty, string.Empty, null).Dispose();
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
        /// Propagate listen port profile values to the UI.
        /// </summary>
        /// <param name="portName">Port name.</param>
        /// <param name="listenPort">Listen port.</param>
        private void ListenPortProfileToUI(string portName, ListenPort listenPort)
        {
            var groupBox = this.serversTab.Controls.OfType<GroupBox>().Where(g => (string)g.Tag == portName).First();
            var checkBox = groupBox.Controls.OfType<CheckBox>().First();
            checkBox.Checked = listenPort != null;
            var layoutPanel = groupBox.Controls.OfType<TableLayoutPanel>().First();
            layoutPanel.Enabled = listenPort != null;
            var addressBox = layoutPanel.Controls.OfType<TextBox>().Where(t => (string)t.Tag == AddressTag).First();
            addressBox.Text = listenPort != null ? listenPort.Address.ToString() : string.Empty;
            var portBox = layoutPanel.Controls.OfType<TextBox>().Where(t => (string)t.Tag == PortTag).First();
            portBox.Text = listenPort != null ? listenPort.Port.ToString() : string.Empty;
        }

        /// <summary>
        /// The profile changed.
        /// </summary>
        /// <param name="fromProfile">Old profile.</param>
        /// <param name="profile">New profile.</param>
        private void ListenProfileChange(Profile fromProfile, Profile profile)
        {
            this.serversTab.Enabled = profile.ProfileType == ProfileType.Full;
            foreach (var portName in this.PortNames)
            {
                if (!this.app.ListenLock[portName])
                {
                    ListenPort oldListenPort = null;
                    if (fromProfile != null && !fromProfile.ListenPort.TryGetValue(portName, out oldListenPort))
                    {
                        oldListenPort = null;
                    }

                    if (!profile.ListenPort.TryGetValue(portName, out ListenPort listenPort))
                    {
                        listenPort = null;
                    }

                    if ((oldListenPort == null && listenPort == null) || (oldListenPort != null && oldListenPort.Equals(listenPort)))
                    {
                        continue;
                    }

                    this.ListenPortProfileToUI(portName, listenPort);
                    var toggleValue = listenPort != null ? ListenParam(listenPort.Address, listenPort.Port) : string.Empty;
                    this.BackEnd.RunAction(
                        new BackEndAction(B3270.Action.Set, portName, toggleValue),
                        ErrorBox.Completion(I18n.Get(Title.Settings)));
                }
            }
        }

        /// <summary>
        /// Merge the listen settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        private bool MergeListen(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.ListenPort.SequenceEqual(fromProfile.ListenPort))
            {
                return false;
            }

            toProfile.ListenPort.Clear();
            foreach (var kv in fromProfile.ListenPort)
            {
                toProfile.ListenPort[kv.Key] = (ListenPort)kv.Value.Clone();
            }

            return true;
        }

        /// <summary>
        /// A setting changed, perhaps it is a listen value.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Setting values.</param>
        private void ListenSettingChanged(string settingName, SettingsDictionary settingDictionary)
        {
            if (!this.PortNames.Contains(settingName) || !settingDictionary.TryGetValue(settingName, out string value))
            {
                // Not one of our settings, or for some reason, not there.
                return;
            }

            ListenPort newListenPort = null;
            if (!string.IsNullOrEmpty(value))
            {
                var regex = new Regex(@"\[(?<address>.*)\]:(?<port>.*)");
                var match = regex.Match(value);
                if (!match.Success)
                {
                    ErrorBox.Show(string.Format(I18n.Get(Message.CantParseListen), settingName, value), I18n.Get(Title.Settings));
                    return;
                }

                newListenPort = new ListenPort { Address = IPAddress.Parse(match.Groups["address"].Value), Port = ushort.Parse(match.Groups["port"].Value) };
            }

            if (this.app.ListenLock[settingName])
            {
                // Profile is locked for this port. Just update the UI.
                this.ListenPortProfileToUI(settingName, newListenPort);
                return;
            }

            if (!this.ProfileManager.Current.ListenPort.TryGetValue(settingName, out ListenPort oldListenPort))
            {
                oldListenPort = null;
            }

            if (newListenPort == null)
            {
                if (oldListenPort != null)
                {
                    if (this.ready)
                    {
                        this.ProfileManager.PushAndSave(
                            (profile) => profile.ListenPort.Remove(settingName),
                            this.ProfileManager.DisableName(settingName + SettingPath(ChangeKeyword.ListeningPort) + " (" + this.ProfileManager.ExternalText + ")"));
                    }
                }
            }
            else
            {
                if (this.ready)
                {
                    this.ProfileManager.PushAndSave(
                        (profile) => profile.ListenPort[settingName] = newListenPort,
                        this.ProfileManager.ChangeName(settingName + SettingPath(ChangeKeyword.ListeningPort) + " (" + this.ProfileManager.ExternalText + ")"));
                }
            }

            this.ListenPortProfileToUI(settingName, newListenPort);
        }

        /// <summary>
        /// One of the listener check boxes was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ListenerCheckBoxClick(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var portName = (string)checkBox.Tag;
            var container = checkBox.Parent;
            var address = IPAddress.Any;
            var port = this.defaultPort[portName];

            var c = container.Controls.OfType<TableLayoutPanel>().First();
            c.Enabled = checkBox.Checked;
            c.Controls.OfType<TextBox>().Where(b => (string)b.Tag == AddressTag).First().Text = checkBox.Checked ? address.ToString() : string.Empty;
            c.Controls.OfType<TextBox>().Where(b => (string)b.Tag == PortTag).First().Text = checkBox.Checked ? port.ToString() : string.Empty;

            if (this.ProfileManager.PushAndSave(
                (profile) =>
                {
                    if (checkBox.Checked)
                    {
                        profile.ListenPort[portName] = new ListenPort { Address = address, Port = port };
                    }
                    else
                    {
                        profile.ListenPort.Remove(portName);
                    }
                },
                this.ProfileManager.ChangeName(portName + " " + I18n.Get(SettingPath(ChangeKeyword.ListenMode)))))
            {
                var toggleValue = checkBox.Checked ? ListenParam(address, port) : string.Empty;
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, portName, toggleValue),
                    ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// One of the edit buttons on the Servers tab was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ServerEditClick(object sender, EventArgs e)
        {
            if (!(sender is Control control))
            {
                return;
            }

            // Find the type, initial focus, and layout panel of interest.
            var focus = (string)null;
            string type;
            Control parent;
            if (control is TextBox)
            {
                if (!(control.Tag is string tag))
                {
                    return;
                }

                focus = tag;
                parent = control.Parent;
                if (!(parent.Tag is string parentTag))
                {
                    return;
                }

                type = parentTag;
            }
            else if (control is Button)
            {
                parent = control.Parent.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
                if (parent == null)
                {
                    return;
                }

                if (!(parent.Tag is string tag))
                {
                    return;
                }

                type = tag;
            }
            else
            {
                return;
            }

            // Find the current values.
            var addressBox = parent.Controls.OfType<TextBox>().Where(t => (t.Tag as string) == AddressTag).FirstOrDefault();
            var portBox = parent.Controls.OfType<TextBox>().Where(t => (t.Tag as string) == PortTag).FirstOrDefault();
            if (addressBox == null || portBox == null)
            {
                return;
            }

            // Edit them.
            using (var editor = new ServerEditor(this.displayType[type], addressBox.Text, portBox.Text, focus))
            {
                var result = editor.ShowDialog();
                if (result == DialogResult.OK)
                {
                    addressBox.Text = editor.Address;
                    portBox.Text = editor.Port;

                    var address = IPAddress.Parse(editor.Address);
                    var port = ushort.Parse(editor.Port);

                    // Push and save.
                    if (this.ProfileManager.PushAndSave(
                        (profile) =>
                        {
                            profile.ListenPort[type] = new ListenPort { Address = address, Port = port };
                        },
                        this.ProfileManager.ChangeName(this.displayType[type])))
                    {
                        this.BackEnd.RunAction(
                            new BackEndAction(B3270.Action.Set, type, ListenParam(address, port)),
                            ErrorBox.Completion(I18n.Get(Title.Settings)));
                    }
                }
            }
        }

        /// <summary>
        /// One of the server text boxes got focus. Don't let it.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ServerTextBoxEnter(object sender, EventArgs e)
        {
            this.serversTab.Focus();
        }
    }
}
