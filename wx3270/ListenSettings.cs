// <copyright file="ListenSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows.Forms;
    using I18nBase;

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

        /// <summary>
        /// The display names for each listening port.
        /// </summary>
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
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeListenSettings()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(serversTab)), "Tour: Server settings");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(serversTab)),
@"Use this tab to change the protocols that wx3270 listens for.

These protocols allow wx3270 to be controlled by an outside program.");

            // s3270 server.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(s3270Box)), "s3270 server");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(s3270Box)),
@"The s3270 server listens for connections using the s3270 protocol.

Details of this protocol are on the x3270 Wiki.");

            // HTTP server.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(httpdBox)), "HTTP server");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(httpdBox)),
@"The HTTP server listens for connections using the HTTP protocol.

Details of this protocol are on the x3270 Wiki.");

            // Enable.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(s3270CheckBox)), "Enable the server");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(s3270CheckBox)),
@"Click to turn the server on and off.");

            // Address.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(s3270AddressBox)), "Address");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(s3270AddressBox)),
@"Specify the address to listen on here.

127.0.0.1 allows IPv4 connections from programs on your workstation.
0.0.0.0 allows IPv4 connections from anywhere.
::1 allows IPv6 connections from programs on your workstation.
:: allows IPv6 connections from anywhere.");

            // Port.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(s3270PortBox)), "TCP port");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(s3270PortBox)),
@"Specify the TCP port to listen on here.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

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

            this.ProfileManager.AddChangeTo(this.ProfileChange);

            // Set up a dummy listener editor for localization.
            new ServerEditor(string.Empty, string.Empty, string.Empty, null).Dispose();

            // Register our tour.
            var nodes = new[]
            {
                ((Control)this.serversTab, (int?)null, Orientation.Centered),
                (this.s3270Box, null, Orientation.UpperLeftTight),
                (this.httpdBox, null, Orientation.UpperRight),
                (this.s3270CheckBox, null, Orientation.UpperLeft),
                (this.s3270AddressBox, null, Orientation.UpperLeft),
                (this.s3270PortBox, null, Orientation.UpperLeft),
            };
            this.RegisterTour(this.serversTab, nodes);
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
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileChange(Profile oldProfile, Profile newProfile)
        {
            this.serversTab.Enabled = newProfile.ProfileType == ProfileType.Full;
            foreach (var portName in this.PortNames)
            {
                if (!this.app.ListenLock[portName])
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

                    this.ListenPortProfileToUI(portName, listenPort);
                }
            }
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

            this.ProfileManager.PushAndSave(
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
                Wx3270.ProfileManager.ChangeName(this.displayType[portName]));
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
                    this.ProfileManager.PushAndSave(
                        (profile) =>
                        {
                            profile.ListenPort[type] = new ListenPort { Address = address, Port = port };
                        },
                        Wx3270.ProfileManager.ChangeName(this.displayType[type]));
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
