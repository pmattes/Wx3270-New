// <copyright file="Main.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace wx3270Restrict
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using Wx3270;

    /// <summary>
    /// The wx3270Restrict main window.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The index of the All item.
        /// </summary>
        private readonly int allIndex;

        /// <summary>
        /// The index of the None item.
        /// </summary>
        private readonly int noneIndex;

        /// <summary>
        /// True if in the midst of processing a checkbox change.
        /// </summary>
        private bool changing;

        /// <summary>
        /// The current restrictions.
        /// </summary>
        private Restrictions currentRestrictions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            errorLabel.Text = string.Empty;

            // Set up the current display.
            var key = Registry.LocalMachine.OpenSubKey(Constants.Misc.RegistryKey);
            var value = (key != null) ? (string)key.GetValue(Constants.Misc.RestrictionsValue) : null;
            if (value != null)
            {
                if (!Enum.TryParse(value, true, out currentRestrictions))
                {
                    this.SetErrorLabel("Registry key parse error");
                }
            }

            changing = true;
            var i = 0;
            foreach (var restriction in Enum.GetValues(typeof(Restrictions)).OfType<Restrictions>())
            {
                this.restrictionsCheckedListBox.Items.Add(restriction, currentRestrictions.HasFlag(restriction));
                if (restriction == Restrictions.None)
                {
                    this.noneIndex = i;
                }

                if (restriction == Restrictions.All)
                {
                    this.allIndex = i;
                }

                i++;
            }

            changing = false;

            key.Close();

            // Ready to go.
            this.Show();
        }

        /// <summary>
        /// Write the current restrictions to the registry.
        /// </summary>
        private void WriteRestrictions()
        {
            RegistryKey key = null;
            try
            {
                try
                {
                    key = Registry.LocalMachine.CreateSubKey(Constants.Misc.RegistryKey);
                }
                catch (Exception e)
                {
                    this.SetErrorLabel(e.Message);
                    return;
                }

                try
                {
                    key.SetValue(Constants.Misc.RestrictionsValue, this.currentRestrictions.ToString());
                }
                catch (Exception e)
                {
                    this.SetErrorLabel(e.Message);
                    return;
                }

                this.SetErrorLabel(string.Empty);
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
        }

        /// <summary>
        /// The main window was closed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MainClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// An item in the checked list box was changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (changing)
            {
                return;
            }

            try
            {
                changing = true;

                var newRestrictions = currentRestrictions;

                var item = (Restrictions)this.restrictionsCheckedListBox.Items[e.Index];
                if (item == Restrictions.All)
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        newRestrictions = Restrictions.All;
                        for (var i = 0; i < this.restrictionsCheckedListBox.Items.Count; i++)
                        {
                            this.restrictionsCheckedListBox.SetItemChecked(i, (Restrictions)this.restrictionsCheckedListBox.Items[i] != Restrictions.None);
                        }
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() => this.restrictionsCheckedListBox.SetItemChecked(this.allIndex, true)));
                    }
                }
                else if (item == Restrictions.None)
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        newRestrictions = Restrictions.None;
                        for (var i = 0; i < this.restrictionsCheckedListBox.Items.Count; i++)
                        {
                            var r = (Restrictions)this.restrictionsCheckedListBox.Items[i];
                            if (r != Restrictions.None)
                            {
                                this.restrictionsCheckedListBox.SetItemChecked(i, false);
                            }
                        }
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() => this.restrictionsCheckedListBox.SetItemChecked(this.noneIndex, true)));
                    }
                }
                else
                {
                    // An actual value.
                    if (e.NewValue == CheckState.Checked)
                    {
                        newRestrictions |= item;
                    }
                    else
                    {
                        newRestrictions &= ~item;
                    }

                    this.restrictionsCheckedListBox.SetItemChecked(this.noneIndex, newRestrictions == Restrictions.None);
                    this.restrictionsCheckedListBox.SetItemChecked(this.allIndex, newRestrictions == Restrictions.All);
                }

                // Write out the new restrictions.
                if (newRestrictions != this.currentRestrictions)
                {
                    this.currentRestrictions = newRestrictions;
                    this.WriteRestrictions();
                }
            }
            finally
            {
                changing = false;
            }
        }

        /// <summary>
        /// Set the error label.
        /// </summary>
        /// <param name="text">Text to display.</param>
        private void SetErrorLabel(string text)
        {
            this.errorLabelTimer.Stop();
            this.errorLabel.Text = text;
            if (!string.IsNullOrEmpty(text))
            {
                this.errorLabelTimer.Start();
            }
        }

        /// <summary>
        /// The error label timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ErrorLabelTimerTick(object sender, EventArgs e)
        {
            this.SetErrorLabel(string.Empty);
        }

        /// <summary>
        /// The help button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpClick(object sender, EventArgs e)
        {
            var version = typeof(Wx3270Restrict).Assembly.GetName().Version;
            var fullPath = new[]
            {
                "http://x3270.bgp.nu/wx3270-help",
                version.Major + "." + version.Minor,
                "en-US",
                "wx3270Restrict",
            };
            Process.Start(string.Join("/", fullPath));
        }
    }
}
