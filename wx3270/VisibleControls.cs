// <copyright file="VisibleControls.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    /// <summary>
    /// A window documenting visible control characters.
    /// </summary>
    public partial class VisibleControls : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleControls"/> class.
        /// </summary>
        public VisibleControls()
        {
            this.InitializeComponent();
            I18n.Localize(this);
        }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void LocalizeForm()
        {
            var controls = new VisibleControls();
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void VisibleControls_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.Owner.BringToFront();
        }
    }
}
