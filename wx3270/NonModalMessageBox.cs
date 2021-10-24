// <copyright file="NonModalMessageBox.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Non-modal message box.
    /// </summary>
    public partial class NonModalMessageBox : Form
    {
        /// <summary>
        /// Completion callback.
        /// </summary>
        private Action<DialogResult> completion;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonModalMessageBox"/> class.
        /// </summary>
        /// <param name="title">Window title.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="retryAbort">True to display retry and abort buttons.</param>
        /// <param name="completion">Completion callback.</param>
        public NonModalMessageBox(string title, string text, bool retryAbort, Action<DialogResult> completion)
        {
            this.InitializeComponent();

            // Localize.
            I18n.Localize(this);

            // Set up the basics.
            this.completion = completion;
            this.Text = title;
            this.textLabel.Text = text;

            // Set up the requested buttons.
            if (retryAbort)
            {
                this.okButton.RemoveFromParent();
                this.AcceptButton = this.retryButton;
                this.CancelButton = this.cancelButton;
            }
            else
            {
                this.retryButton.RemoveFromParent();
                this.cancelButton.RemoveFromParent();
                this.AcceptButton = this.okButton;
            }
        }

        /// <summary>
        /// Gets or sets the message text.
        /// </summary>
        public string MessageText
        {
            get => this.textLabel.Text;

            set
            {
                this.textLabel.Text = value;
            }
        }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void FormLocalize()
        {
            new NonModalMessageBox("`none", "`none", false, null).Dispose();
        }

        /// <summary>
        /// One of the buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ButtonClick(object sender, EventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            // Return the result associated with this button, then close the form.
            var tag = (string)button.Tag;
            this.completion((DialogResult)Enum.Parse(typeof(DialogResult), tag));
            this.completion = null;
            this.Close();
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BoxFormClosing(object sender, FormClosingEventArgs e)
        {
            // Closing the form is the same as hitting the accept button.
            this.completion?.Invoke((DialogResult)Enum.Parse(typeof(DialogResult), ((Button)this.AcceptButton).Tag as string));
        }
    }
}
