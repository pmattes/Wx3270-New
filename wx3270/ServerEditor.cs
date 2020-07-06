// <copyright file="ServerEditor.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Linq;
    using System.Net;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Server (listener) editor.
    /// </summary>
    public partial class ServerEditor : Form
    {
        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.TitleName(nameof(ServerEditor));

        /// <summary>
        /// Message group for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(ServerEditor));

        /// <summary>
        /// The tag of the field that gets the initial focus.
        /// </summary>
        private readonly Control initialFocus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEditor"/> class.
        /// </summary>
        /// <param name="service">Listening service name.</param>
        /// <param name="address">Listening address.</param>
        /// <param name="port">Listening port.</param>
        /// <param name="tag">Tag of field to get initial focus.</param>
        public ServerEditor(string service, string address, string port, string tag)
        {
            this.InitializeComponent();

            this.serviceValueLabel.Text = service;
            this.Address = address ?? string.Empty;
            this.Port = port ?? string.Empty;

            this.addressTextBox.Text = this.Address;
            this.portTextBox.Text = this.Port;

            // Set up the initial focus.
            this.initialFocus = this.tableLayoutPanel1.Controls.OfType<Control>().Where((c) => c.Tag == (object)tag).FirstOrDefault();
            if (this.initialFocus == null)
            {
                this.initialFocus = this.addressTextBox;
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);
        }

        /// <summary>
        /// Gets or sets the listening address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the listening port.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.ListenerEditor, "Listener Editor");

            I18n.LocalizeGlobal(Message.MissingListenAddress, "Missing listen address");
            I18n.LocalizeGlobal(Message.InvalidListenAddress, "Invalid listen address");
            I18n.LocalizeGlobal(Message.MissingListenPort, "Missing listen port");
            I18n.LocalizeGlobal(Message.InvalidListenPort, "Invalid listen port");
        }

        /// <summary>
        /// The listener editor window was shown.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ListenerEditorShown(object sender, System.EventArgs e)
        {
            if (this.initialFocus != null)
            {
                this.initialFocus.Focus();
            }
        }

        /// <summary>
        /// The cancel button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelButtonClick(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Validate the address field.
        /// </summary>
        /// <param name="text">Text of address field.</param>
        /// <returns>True if field is valid.</returns>
        private bool ValidateAddress(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ErrorBox.Show(I18n.Get(Message.MissingListenAddress), I18n.Get(Title.ListenerEditor));
                return false;
            }

            if (!IPAddress.TryParse(text, out _))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidListenAddress), I18n.Get(Title.ListenerEditor));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate the port field.
        /// </summary>
        /// <param name="text">Text of port field.</param>
        /// <returns>True if field is valid.</returns>
        private bool ValidatePort(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ErrorBox.Show(I18n.Get(Message.MissingListenPort), I18n.Get(Title.ListenerEditor));
                return false;
            }

            if (!ushort.TryParse(text, out ushort port) || port == 0)
            {
                ErrorBox.Show(I18n.Get(Message.InvalidListenPort), I18n.Get(Title.ListenerEditor));
                return false;
            }

            return true;
        }

        /// <summary>
        /// The OK button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OkButtonClick(object sender, System.EventArgs e)
        {
            // Validate the fields.
            if (!this.ValidateAddress(this.addressTextBox.Text))
            {
                this.addressTextBox.Focus();
                return;
            }

            if (!this.ValidatePort(this.portTextBox.Text))
            {
                this.portTextBox.Focus();
                return;
            }

            // Transfer from the UI to the return value.
            this.Address = this.addressTextBox.Text;
            this.Port = this.portTextBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// The help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, System.EventArgs e)
        {
            Wx3270App.GetHelp(Wx3270App.FormatHelpTag(nameof(ServerEditor)));
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Proxy editor errors.
            /// </summary>
            public static readonly string ListenerEditor = I18n.Combine(TitleName, "listenerEditor");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Missing listener address.
            /// </summary>
            public static readonly string MissingListenAddress = I18n.Combine(MessageName, "missingListenAddress");

            /// <summary>
            /// Bad proxy address.
            /// </summary>
            public static readonly string InvalidListenAddress = I18n.Combine(MessageName, "invalidListenAddress");

            /// <summary>
            /// Missing proxy address.
            /// </summary>
            public static readonly string MissingListenPort = I18n.Combine(MessageName, "missingListenPort");

            /// <summary>
            /// Bad proxy port.
            /// </summary>
            public static readonly string InvalidListenPort = I18n.Combine(MessageName, "invalidListenPort");
        }
    }
}
