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
            I18n.Localize(this);
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
            new StopDialog("`Nothing", "`Nothing", null).Dispose();
        }
    }
}
