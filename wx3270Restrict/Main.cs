// <copyright file="Main.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270Restrict
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using Wx3270;

    /// <summary>
    /// The Wx3270Restrict main window.
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
            this.InitializeComponent();
            this.errorLabel.Text = string.Empty;

            // Set up the current display.
            var key = Registry.LocalMachine.OpenSubKey(Constants.Misc.RegistryKey);
            var value = (key != null) ? (string)key.GetValue(Constants.Misc.RestrictionsValue) : null;
            if (value != null)
            {
                if (!Enum.TryParse(value, true, out this.currentRestrictions))
                {
                    this.SetErrorLabel("Registry key parse error");
                }
            }

            this.changing = true;
            var i = 0;
            foreach (var restriction in Enum.GetValues(typeof(Restrictions)).OfType<Restrictions>())
            {
                this.restrictionsCheckedListBox.Items.Add(restriction, false);
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

            this.DisplayRestrictions();

            this.changing = false;

            key.Close();

            // Ready to go.
            this.Show();
        }

        /// <summary>
        /// Display the current restrictions.
        /// </summary>
        private void DisplayRestrictions()
        {
            var i = 0;
            foreach (var restriction in Enum.GetValues(typeof(Restrictions)).OfType<Restrictions>())
            {
                if (restriction == Restrictions.All || restriction == Restrictions.None)
                {
                    this.restrictionsCheckedListBox.SetItemChecked(i, this.currentRestrictions == restriction);
                }
                else
                {
                    this.restrictionsCheckedListBox.SetItemChecked(i, this.currentRestrictions.HasFlag(restriction));
                }

                i++;
            }
        }

        /// <summary>
        /// Write the current restrictions to the registry.
        /// </summary>
        /// <param name="restrictions">New restrictions.</param>
        /// <returns>True if write succeeded.</returns>
        private bool WriteRestrictions(Restrictions restrictions)
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
                    return false;
                }

                try
                {
                    key.SetValue(Constants.Misc.RestrictionsValue, restrictions.ToString());
                    this.currentRestrictions = restrictions;
                    this.SetErrorLabel(string.Empty);
                    return true;
                }
                catch (Exception e)
                {
                    this.SetErrorLabel(e.Message);
                    return false;
                }
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
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.changing)
            {
                return;
            }

            try
            {
                this.changing = true;

                var newRestrictions = this.currentRestrictions;

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
                        // Set it back.
                        this.BeginInvoke(new Action(() => this.DisplayRestrictions()));
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
                        // Set it back.
                        this.BeginInvoke(new Action(() => this.DisplayRestrictions()));
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
                    if (!this.WriteRestrictions(newRestrictions))
                    {
                        // Redisplay.
                        this.BeginInvoke(new Action(() => this.DisplayRestrictions()));
                    }
                }
            }
            finally
            {
                this.changing = false;
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
