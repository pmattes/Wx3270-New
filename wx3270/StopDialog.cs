// <copyright file="StopDialog.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Drawing;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Dialog with an option to stop showing it.
    /// Roughly equivalent to a SHMessageBoxCheck dialog, but I can use my own method to decide
    /// that the dialog should be suppressed (e.g., a fake Registry).
    /// </summary>
    public partial class StopDialog : Form
    {
        /// <summary>
        /// Message to click yes or no to save text to clipboard.
        /// </summary>
        private static readonly string NotAgainName = I18n.Combine(nameof(StopDialog), "NotAgain");

        /// <summary>
        /// The English text of the not-again message.
        /// </summary>
        private static readonly string NotAgainText = "In the future, do not show me this dialog box";

        /// <summary>
        /// Initializes a new instance of the <see cref="StopDialog"/> class.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="icon">Icon to display.</param>
        public StopDialog(string title, string text, Icon icon)
        {
            this.InitializeComponent();
            this.Text = title;
            this.messageLabel.Text = text;
            if (icon != null)
            {
                this.iconPictureBox.Image = Bitmap.FromHicon(icon.Handle);
            }
            else
            {
                this.iconPictureBox.Visible = false;
            }

            // Localize.
            this.notAgainCheckBox.Text = I18n.Get(NotAgainName);
        }

        /// <summary>
        /// Gets a value indicating whether the dialog should be displayed again.
        /// </summary>
        public bool NotAgain => this.notAgainCheckBox.Checked;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(NotAgainName, NotAgainText);
        }
    }
}
