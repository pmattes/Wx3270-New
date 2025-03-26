// <copyright file="ProxyEditor.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using I18nBase;
    using static Wx3270.B3270;

    /// <summary>
    /// The proxy editor.
    /// </summary>
    public partial class ProxyEditor : Form
    {
        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(ProxyEditor));

        /// <summary>
        /// Message group for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(ProxyEditor));

        /// <summary>
        /// The proxies database (proxy type definitions).
        /// </summary>
        private readonly IProxiesDb proxiesDb;

        /// <summary>
        /// The control that receives the initial focus.
        /// </summary>
        private readonly Control initialFocus;

        /// <summary>
        /// True if the form has ever been activated.
        /// </summary>
        private bool everActivated = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyEditor"/> class.
        /// </summary>
        /// <param name="proxy">Initial proxy value.</param>
        /// <param name="proxiesDb">Proxies database.</param>
        /// <param name="tag">Tag to match.</param>
        public ProxyEditor(Profile.ProxyClass proxy, IProxiesDb proxiesDb, string tag)
        {
            this.InitializeComponent();
            this.ProxyValue = (Profile.ProxyClass)proxy.Clone();
            this.proxiesDb = proxiesDb;

            // Localize.
            I18n.Localize(this, this.toolTip1);

            // Set up the combo box.
            this.proxyComboBox.Items.AddRange(this.proxiesDb.Proxies.Values.ToArray());

            // Set the data fields in the UI.
            if (string.IsNullOrEmpty(proxy.Type) || !this.proxiesDb.Proxies.ContainsKey(proxy.Type))
            {
                this.proxyComboBox.SelectedItem = this.proxiesDb.Proxies[Proxy.None];
            }
            else
            {
                this.proxyComboBox.SelectedItem = this.proxiesDb.Proxies[proxy.Type];
            }

            this.addressTextBox.Text = proxy.Address ?? string.Empty;
            this.portTextBox.Text = proxy.Port.HasValue ? proxy.Port.ToString() : string.Empty;
            this.usernameTextBox.Text = proxy.Username ?? string.Empty;
            this.passwordTextBox.Text = proxy.Password ?? string.Empty;

            this.initialFocus = this.tableLayoutPanel1.Controls.OfType<Control>().Where((c) => c.Tag == (object)tag).FirstOrDefault();
        }

        /// <summary>
        /// Gets the proxy value.
        /// </summary>
        public Profile.ProxyClass ProxyValue { get; }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.ProxyEditor, "Proxy Editor");

            I18n.LocalizeGlobal(Message.MissingProxyAddress, "Missing proxy address");
            I18n.LocalizeGlobal(Message.InvalidProxyAddress, "Invalid proxy address");
            I18n.LocalizeGlobal(Message.MissingProxyPort, "Missing proxy port");
            I18n.LocalizeGlobal(Message.InvalidProxyPort, "Invalid proxy port");
            I18n.LocalizeGlobal(Message.InvalidProxyUsername, "Invalid proxy username");
            I18n.LocalizeGlobal(Message.PasswordWithoutUsername, "Password specified without username");
            I18n.LocalizeGlobal(Message.InvalidProxyPassword, "Invalid proxy password");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor)), "Tour: Proxy editor");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor)),
@"Use this window to define or edit proxy settings.");

            // Proxy type.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(proxyComboBox)), "Proxy type");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(proxyComboBox)),
@"Select the type of proxy here.");

            // Address.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(addressTextBox)), "Proxy server address");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(addressTextBox)),
@"Enter the name or numeric address of the proxy server here.");

            // Port.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(portTextBox)), "Proxy server port");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(portTextBox)),
@"Enter the TCP port of the proxy server here.");

            // Username.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(usernameTextBox)), "User name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(usernameTextBox)),
@"Enter your user name for the proxy server here.

Not all proxy types allow user names, and not all servers that allow them require that you use one.");

            // Password.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(passwordTextBox)), "Password");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(passwordTextBox)),
@"Enter your password for the proxy server here.

Not all proxy types allow passwords, and not all servers that allow them require that you use one.");

            // Cancel button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(cancelButton)), "Cancel button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(cancelButton)),
@"Click to abandon any changes you have made.");

            // Save button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(okButton)), "Save button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(okButton)),
@"Click to save your changes.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(ProxyEditor), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(ProxyEditor), nameof(helpPictureBox)),
@"Click to display context-sensitive help from the x3270 Wiki in your browser, or to start this tour again.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
        /// Validate the address field.
        /// </summary>
        /// <param name="text">Text of address field.</param>
        /// <returns>True if field is valid.</returns>
        private bool ValidateAddress(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ErrorBox.Show(I18n.Get(Message.MissingProxyAddress), I18n.Get(Title.ProxyEditor));
                return false;
            }

            if (text.Any(c => " []@".Contains(c)))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidProxyAddress), I18n.Get(Title.ProxyEditor));
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
                ErrorBox.Show(I18n.Get(Message.MissingProxyPort), I18n.Get(Title.ProxyEditor));
                return false;
            }

            if (!ushort.TryParse(text, out _))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidProxyPort), I18n.Get(Title.ProxyEditor));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate the username field.
        /// </summary>
        /// <param name="username">Text of username field.</param>
        /// <param name="password">Text of password field.</param>
        /// <returns>True if field is valid.</returns>
        private bool ValidateUsername(string username, string password)
        {
            if (username.Any(c => " []:@".Contains(c)))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidProxyUsername), I18n.Get(Title.ProxyEditor));
                return false;
            }

            if (string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                ErrorBox.Show(I18n.Get(Message.PasswordWithoutUsername), I18n.Get(Title.ProxyEditor));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate the password field.
        /// </summary>
        /// <param name="text">Text of password field.</param>
        /// <returns>True if field is valid.</returns>
        private bool ValidatePassword(string text)
        {
            if (text.Any(c => " []:@".Contains(c)))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidProxyPassword), I18n.Get(Title.ProxyEditor));
                return false;
            }

            return true;
        }

        /// <summary>
        /// The Help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }

            // Wx3270App.GetHelp(Wx3270App.FormatHelpTag(nameof(ProxyEditor)));
        }

        /// <summary>
        /// The OK button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OkButtonClick(object sender, EventArgs e)
        {
            // Validate the fields.
            var proxy = (Proxy)this.proxyComboBox.SelectedItem;
            if (proxy.Name != Proxy.None)
            {
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

                if (proxy.TakesUsername && !this.ValidateUsername(this.usernameTextBox.Text, this.passwordTextBox.Text))
                {
                    this.usernameTextBox.Focus();
                    return;
                }

                if (proxy.TakesUsername && !this.ValidatePassword(this.usernameTextBox.Text))
                {
                    this.passwordTextBox.Focus();
                    return;
                }
            }

            // Transfer from the UI to the return value.
            this.ProxyValue.Type = (proxy.Name == Proxy.None) ? null : proxy.Name;
            this.ProxyValue.Address = MaybeNull(this.addressTextBox.Text);
            this.ProxyValue.Port = string.IsNullOrEmpty(this.portTextBox.Text) ? null : (int?)ushort.Parse(this.portTextBox.Text);
            this.ProxyValue.Username = MaybeNull(this.usernameTextBox.Text);
            this.ProxyValue.Password = MaybeNull(this.passwordTextBox.Text);

            // Done.
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// The Cancel button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// The proxy type selected index changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyTypeIndexChanged(object sender, EventArgs e)
        {
            if (!(this.proxyComboBox.SelectedItem is Proxy proxy))
            {
                return;
            }

            // Set data-entry fields to empty or defaults.
            var isNone = proxy.Name == Proxy.None;
            this.addressTextBox.Text = string.Empty;
            if (proxy.DefaultPort.HasValue)
            {
                this.portTextBox.Text = proxy.DefaultPort.ToString();
            }
            else
            {
                this.portTextBox.Text = string.Empty;
            }

            this.usernameTextBox.Text = string.Empty;
            this.passwordTextBox.Text = string.Empty;

            this.addressRequiredLabel.Visible = !isNone;
            this.portRequiredLabel.Visible = !isNone;

            this.addressTextBox.Enabled = !isNone;
            this.portTextBox.Enabled = !isNone;
            this.usernameTextBox.Enabled = proxy.TakesUsername;
            this.passwordTextBox.Enabled = proxy.TakesUsername;
        }

        /// <summary>
        /// The form was shown.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyEditor_Shown(object sender, EventArgs e)
        {
            this.initialFocus?.Focus();
        }

        /// <summary>
        /// An item on the help menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, Wx3270App.FormatHelpTag(nameof(ProxyEditor)), () => this.RunTour(isExplicit: true));
        }

        /// <summary>
        /// Run the tour.
        /// </summary>
        /// <param name="isExplicit">True if invoked explicitly.</param>
        private void RunTour(bool isExplicit = false)
        {
            var nodes = new[]
            {
                ((Control)this, (int?)null, Orientation.Centered),
                (this.proxyComboBox, null, Orientation.UpperLeftTight),
                (this.addressTextBox, null, Orientation.UpperLeftTight),
                (this.portTextBox, null, Orientation.UpperLeftTight),
                (this.usernameTextBox, null, Orientation.LowerLeftTight),
                (this.passwordTextBox, null, Orientation.LowerLeftTight),
                (this.cancelButton, null, Orientation.LowerRight),
                (this.okButton, null, Orientation.LowerRight),
                (this.helpPictureBox, null, Orientation.LowerRight),
            };
            Tour.Navigate(this, nodes, isExplicit: isExplicit);
        }

        /// <summary>
        /// The window was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ProxyEditorActivated(object sender, EventArgs e)
        {
            if (!this.everActivated)
            {
                this.everActivated = true;
                if (!Tour.IsComplete(this))
                {
                    this.RunTour();
                }
            }
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Proxy editor errors.
            /// </summary>
            public static readonly string ProxyEditor = I18n.Combine(TitleName, "proxyEditor");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Missing proxy address.
            /// </summary>
            public static readonly string MissingProxyAddress = I18n.Combine(MessageName, "missingProxyAddress");

            /// <summary>
            /// Bad proxy address.
            /// </summary>
            public static readonly string InvalidProxyAddress = I18n.Combine(MessageName, "invalidProxyAddress");

            /// <summary>
            /// Missing proxy address.
            /// </summary>
            public static readonly string MissingProxyPort = I18n.Combine(MessageName, "missingProxyPort");

            /// <summary>
            /// Bad proxy port.
            /// </summary>
            public static readonly string InvalidProxyPort = I18n.Combine(MessageName, "invalidProxyPort");

            /// <summary>
            /// Bad proxy username.
            /// </summary>
            public static readonly string InvalidProxyUsername = I18n.Combine(MessageName, "invalidProxyUsername");

            /// <summary>
            /// Password without username.
            /// </summary>
            public static readonly string PasswordWithoutUsername = I18n.Combine(MessageName, "passwordWithoutUsername");

            /// <summary>
            /// Bad proxy password.
            /// </summary>
            public static readonly string InvalidProxyPassword = I18n.Combine(MessageName, "invalidProxyPassword");
        }
    }
}
