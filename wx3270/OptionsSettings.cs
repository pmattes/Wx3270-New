// <copyright file="OptionsSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Printing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// The Options tab in the settings dialog.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// The sample image.
        /// </summary>
        private ScreenSample optionsScreenSample;

        /// <summary>
        /// The cursor type radio buttons.
        /// </summary>
        private RadioEnum<CursorType> cursorType;

        /// <summary>
        /// Initialize the Options tab.
        /// </summary>
        private void OptionsTabInit()
        {
            // Set up the screen sample.
            this.optionsScreenSample = new ScreenSample(
                this,
                this.optionsPreviewScreenPictureBox,
                this.optionsPreviewLayoutPanel,
                this.optionsPreviewStatusLineLabel,
                this.optionsPreviewSeparatorPictureBox,
                this.ColorMode);

            // Set up the radio buttons.
            this.cursorType = new RadioEnum<CursorType>(this.CursorGroupBox);
            this.cursorType.Changed += this.CursorCheckedChanged;

            // Set up the printer list.
            foreach (var printer in PrinterSettings.InstalledPrinters)
            {
                this.printerComboBox.Items.Add(printer);
            }

            // Set up the scroll bar checkbox.
            this.scrollbarCheckBox.Enabled = !this.app.NoScrollBar;

            // Subscribe to profile change events and merges.
            this.ProfileManager.ChangeTo += this.ProfileOptionsChanged;
            this.ProfileManager.RegisterMerge(ImportType.OtherSettingsReplace, this.MergeOptions);

            // Subscribe to color mode change events.
            this.ColorModeChangedEvent += (color) => this.optionsScreenSample.Invalidate();

            // Subscribe to settings changes.
            this.app.SettingChange.Register(
                (settingName, settingDictionary) => this.Invoke(new MethodInvoker(() => this.OptionsSettingChanged(settingName, settingDictionary))),
                new[] { B3270.Setting.PrinterName, B3270.Setting.PrinterOptions, B3270.Setting.PrinterCodePage, B3270.Setting.NopSeconds });
        }

        /// <summary>
        /// The profile has changed. Update the Options defaults.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileOptionsChanged(Profile oldProfile, Profile newProfile)
        {
            // Set up the GUI.
            this.OptionsSet(newProfile);

            // Tell the host about the screen settings.
            this.ApplyOptionsFromProfile(oldProfile, newProfile);
        }

        /// <summary>
        /// Merge the option settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        private bool MergeOptions(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            // Note: We do not merge the description.
            if (toProfile.Printer == fromProfile.Printer &&
                toProfile.PrinterCodePage == fromProfile.PrinterCodePage &&
                toProfile.PrinterOptions == fromProfile.PrinterOptions &&
                toProfile.NopInterval == fromProfile.NopInterval)
            {
                return false;
            }

            toProfile.Printer = fromProfile.Printer;
            toProfile.PrinterCodePage = fromProfile.PrinterCodePage;
            toProfile.PrinterOptions = fromProfile.PrinterOptions;
            toProfile.NopInterval = fromProfile.NopInterval;
            return true;
        }

        /// <summary>
        /// Apply new Options settings from the profile.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ApplyOptionsFromProfile(Profile oldProfile, Profile newProfile)
        {
            var settings = new List<string>();

            if (oldProfile == null || oldProfile.HostCodePage != newProfile.HostCodePage)
            {
                settings.AddRange(new[] { B3270.Setting.CodePage, newProfile.HostCodePage });
            }

            if (oldProfile == null || oldProfile.CursorType != newProfile.CursorType)
            {
                settings.AddRange(new[] { B3270.Setting.AltCursor, B3270.ToggleArgument.Action(newProfile.CursorType == CursorType.Underscore) });
            }

            if (oldProfile == null || oldProfile.CrosshairCursor != newProfile.CrosshairCursor)
            {
                settings.AddRange(new[] { B3270.Setting.Crosshair, B3270.ToggleArgument.Action(newProfile.CrosshairCursor) });
            }

            if (oldProfile == null || oldProfile.CursorBlink != newProfile.CursorBlink)
            {
                settings.AddRange(new[] { B3270.Setting.CursorBlink, B3270.ToggleArgument.Action(newProfile.CursorBlink) });
            }

            if (oldProfile == null || oldProfile.Monocase != newProfile.Monocase)
            {
                settings.AddRange(new[] { B3270.Setting.MonoCase, B3270.ToggleArgument.Action(newProfile.Monocase) });
            }

            if (oldProfile == null || oldProfile.Typeahead != newProfile.Typeahead)
            {
                settings.AddRange(new[] { B3270.Setting.Typeahead, B3270.ToggleArgument.Action(newProfile.Typeahead) });
            }

            if (oldProfile == null || oldProfile.ShowTiming != newProfile.ShowTiming)
            {
                settings.AddRange(new[] { B3270.Setting.ShowTiming, B3270.ToggleArgument.Action(newProfile.ShowTiming) });
            }

            if (oldProfile == null || oldProfile.AlwaysInsert != newProfile.AlwaysInsert)
            {
                settings.AddRange(new[] { B3270.Setting.AlwaysInsert, B3270.ToggleArgument.Action(newProfile.AlwaysInsert) });
            }

            if (oldProfile == null || oldProfile.Printer != newProfile.Printer)
            {
                settings.AddRange(new[] { B3270.Setting.PrinterName, newProfile.Printer });
            }

            if (oldProfile == null || oldProfile.PrinterOptions != newProfile.PrinterOptions)
            {
                var sentOptions = this.mainScreen.ActionsDialog.Pr3287TraceOptions + newProfile.PrinterOptions;
                if (this.app.Restricted(Restrictions.ExternalFiles))
                {
                    // Remove the pr3287 trace option.
                    sentOptions = Regex.Replace(sentOptions, Pr3287.CommandLineOption.TraceRegex, string.Empty);
                }

                settings.AddRange(new[] { B3270.Setting.PrinterOptions, sentOptions });
            }

            if (oldProfile == null || oldProfile.PrinterCodePage != newProfile.PrinterCodePage)
            {
                settings.AddRange(new[] { B3270.Setting.PrinterCodePage, newProfile.PrinterCodePage });
            }

            if (oldProfile == null || oldProfile.NopInterval != newProfile.NopInterval)
            {
                settings.AddRange(new[] { B3270.Setting.NopSeconds, newProfile.NopInterval.ToString() });
            }

            if (oldProfile == null || oldProfile.Retry != newProfile.Retry)
            {
                settings.AddRange(new[] { B3270.Setting.Retry, B3270.ToggleArgument.Action(newProfile.Retry) });
            }

            if (settings.Count != 0)
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, settings), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }

            if (!newProfile.Maximize && oldProfile != null)
            {
                // Force normal window if the profile says to, and this is not the initial load.
                this.mainScreen.Restore();
            }
        }

        /// <summary>
        /// Set the printer name in the UI.
        /// </summary>
        /// <param name="printerName">Printer name.</param>
        private void SetPrinterName(string printerName)
        {
            if (!string.IsNullOrWhiteSpace(printerName))
            {
                int index;
                if ((index = this.printerComboBox.FindStringExact(printerName)) >= 0)
                {
                    this.printerComboBox.SelectedIndex = index;
                }
                else
                {
                    this.printerComboBox.SelectedIndex = this.printerComboBox.Items.Add(printerName);
                }
            }
            else
            {
                this.printerComboBox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Set the Options settings to a given profile.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void OptionsSet(Profile profile)
        {
            // Handle the profile type.
            var isFull = profile.ProfileType == ProfileType.Full;
            this.optionsSampleGroupBox.Enabled = isFull;
            this.CursorGroupBox.Enabled = isFull;
            this.printerSessionGroupBox.Enabled = isFull;
            this.windowTitleLabel.Enabled = isFull;
            this.titleTextBox.Enabled = isFull;

            // Set the cursor type.
            this.cursorType.Value = profile.CursorType;

            // Set the crosshair cursor.
            this.crosshairCursorCheckBox.Checked = profile.CrosshairCursor;

            // Set cursor blink.
            this.cursorBlinkCheckBox.Checked = profile.CursorBlink;

            // Set monocase.
            this.MonoCaseCheckBox.Checked = profile.Monocase;

            // Redraw the sample.
            this.optionsScreenSample.Invalidate();

            // Set typeahead.
            this.typeaheadCheckBox.Checked = profile.Typeahead;

            // Set show timing.
            this.showTimingCheckBox.Checked = profile.ShowTiming;

            // Set always insert.
            this.alwaysInsertCheckBox.Checked = profile.AlwaysInsert;

            // Set printer options.
            this.printerOptionsTextBox.Text = profile.PrinterOptions;

            // Set printer code page.
            this.printerCodePageTextBox.Text = profile.PrinterCodePage;

            // Set printer name.
            this.SetPrinterName(profile.Printer);

            // Set NOP interval.
            this.nopCheckBox.Checked = profile.NopInterval != 0;

            // Set retry.
            this.retryCheckBox.Checked = profile.Retry;

            // Set IPv4/Ipv6 preference.
            this.preferIpv4CheckBox.Checked = profile.PreferIpv4;
            this.preferIpv6CheckBox.Checked = profile.PreferIpv6;

            // Set description and window title.
            this.descriptionTextBox.Text = profile.Description;
            this.titleTextBox.Text = profile.WindowTitle;

            // Set scroll bar.
            if (!this.app.NoScrollBar)
            {
                this.scrollbarCheckBox.Checked = profile.ScrollBar;
            }
        }

        /// <summary>
        /// One of the cursor type buttons changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CursorCheckedChanged(object sender, EventArgs e)
        {
            // Redraw the sample fields.
            this.optionsScreenSample.Invalidate();

            // Change the profile.
            if (this.ProfileManager.PushAndSave((current) => current.CursorType = this.cursorType.Value, this.ChangeName(B3270.Setting.AltCursor)))
            {
                // Tell the emulator.
                this.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, B3270.Setting.AltCursor, B3270.ToggleArgument.Action(this.CursorType == CursorType.Underscore)),
                    ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// A miscellaneous checkbox state was changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MiscCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox checkBox))
            {
                return;
            }

            // Redraw the sample fields.
            var settingName = (string)checkBox.Tag;
            switch (settingName)
            {
                case B3270.Setting.Crosshair:
                case B3270.Setting.CursorBlink:
                case B3270.Setting.MonoCase:
                    this.optionsScreenSample.Invalidate();
                    break;
                default:
                    break;
            }

            // Change the profile.
            if (this.ProfileManager.PushAndSave(
                (current) =>
                {
                    switch (settingName)
                    {
                        case B3270.Setting.Crosshair:
                            current.CrosshairCursor = checkBox.Checked;
                            break;
                        case B3270.Setting.CursorBlink:
                            current.CursorBlink = checkBox.Checked;
                            break;
                        case B3270.Setting.MonoCase:
                            current.Monocase = checkBox.Checked;
                            break;
                        case B3270.Setting.Typeahead:
                            current.Typeahead = checkBox.Checked;
                            break;
                        case B3270.Setting.ShowTiming:
                            current.ShowTiming = checkBox.Checked;
                            break;
                        case B3270.Setting.AlwaysInsert:
                            current.AlwaysInsert = checkBox.Checked;
                            break;
                        case B3270.Setting.Retry:
                            current.Retry = checkBox.Checked;
                            break;
                        case B3270.Setting.PreferIpv4:
                            current.PreferIpv4 = checkBox.Checked;
                            break;
                        case B3270.Setting.PreferIpv6:
                            current.PreferIpv6 = checkBox.Checked;
                            break;
                    }
                },
                this.ChangeName(settingName)))
            {
                // Tell the emulator.
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, settingName, B3270.ToggleArgument.Action(checkBox.Checked)), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// A miscellaneous local checkbox changed state.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MiscLocalCheckBoxChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox checkBox))
            {
                return;
            }

            var settingName = (string)checkBox.Tag;
            switch (settingName)
            {
                case ChangeKeyword.ScrollBar:
                    var size = this.mainScreen.ToggleScrollBar(checkBox.Checked);
                    this.ProfileManager.PushAndSave(
                        (current) =>
                        {
                            current.ScrollBar = checkBox.Checked;
                            if (size != null)
                            {
                                current.Size = size.Value;
                            }
                        },
                        this.ChangeName(settingName));
                    break;
            }
        }

        /// <summary>
        /// Paint method for the options screen box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScreenPictureBoxPaint(object sender, PaintEventArgs e)
        {
            // Use the standard method.
            this.SamplePaint(sender, e, this.optionsScreenSample);
        }

        /// <summary>
        /// The printer selection changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterComboBoxSelectionChangeCommitted(object sender, EventArgs e)
        {
            var printer = this.printerComboBox.SelectedItem.ToString();
            if (this.ProfileManager.PushAndSave((current) => current.Printer = printer, this.ChangeName(B3270.Setting.PrinterName)))
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.PrinterName, printer), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// The printer code page changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterCodePageValidated(object sender, EventArgs e)
        {
            var codePage = this.printerCodePageTextBox.Text.Trim();
            if (this.ProfileManager.PushAndSave((current) => current.PrinterCodePage = codePage, this.ChangeName(B3270.Setting.PrinterCodePage)))
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.PrinterCodePage, codePage), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// The printer options changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PrinterOptionsValidated(object sender, EventArgs e)
        {
            var options = this.mainScreen.ActionsDialog.Pr3287TraceOptions + this.printerOptionsTextBox.Text.Trim();
            var sentOptions = options;
            if (this.app.Restricted(Restrictions.ExternalFiles))
            {
                // Remove the pr3287 trace option.
                sentOptions = Regex.Replace(options, Pr3287.CommandLineOption.TraceRegex, string.Empty);
            }

            if (this.ProfileManager.PushAndSave((current) => current.PrinterOptions = options, this.ChangeName(B3270.Setting.PrinterOptions)))
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.PrinterOptions, sentOptions), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }

        /// <summary>
        /// The description text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DescriptionTextBoxValidated(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.Description = this.descriptionTextBox.Text, this.ChangeName(ChangeKeyword.Description));
        }

        /// <summary>
        /// The window title text box was validated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TitleTextBoxValidated(object sender, EventArgs e)
        {
            if (this.ProfileManager.PushAndSave((current) => current.WindowTitle = this.titleTextBox.Text, this.ChangeName(ChangeKeyword.WindowTitle)))
            {
                this.mainScreen.Retitle();
            }
        }

        /// <summary>
        /// A setting changed.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Setting dictionary (values).</param>
        private void OptionsSettingChanged(string settingName, SettingsDictionary settingDictionary)
        {
            if (!settingDictionary.TryGetValue(settingName, out string stringValue))
            {
                return;
            }

            var changeName = this.ProfileManager.ChangeName(string.Format("{0} ({1})", I18n.Get(SettingPath(settingName)), this.ProfileManager.ExternalText));
            switch (settingName)
            {
                case B3270.Setting.PrinterName:
                    this.SetPrinterName(stringValue);
                    this.ProfileManager.PushAndSave((current) => current.Printer = stringValue, changeName);
                    break;
                case B3270.Setting.PrinterOptions:
                    // Strip trace options.
                    var trace = this.mainScreen.ActionsDialog.Pr3287TraceOptions;
                    if (!string.IsNullOrEmpty(trace) && stringValue.StartsWith(trace))
                    {
                        stringValue = stringValue.Remove(0, trace.Length);
                    }

                    this.printerOptionsTextBox.Text = stringValue;
                    this.ProfileManager.PushAndSave((current) => current.PrinterOptions = stringValue, changeName);
                    break;
                case B3270.Setting.PrinterCodePage:
                    this.printerCodePageTextBox.Text = stringValue;
                    this.ProfileManager.PushAndSave((current) => current.PrinterCodePage = stringValue, changeName);
                    break;
                case B3270.Setting.NopSeconds:
                    if (!settingDictionary.TryGetValue(settingName, out int intValue))
                    {
                        return;
                    }

                    this.nopCheckBox.Checked = intValue != 0;
                    this.ProfileManager.PushAndSave((current) => current.NopInterval = intValue, changeName);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// The NOP check box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NopClick(object sender, EventArgs e)
        {
            var interval = this.nopCheckBox.Checked ? 30 : 0;
            if (this.ProfileManager.PushAndSave((current) => current.NopInterval = interval, this.ChangeName(B3270.Setting.NopSeconds)))
            {
                this.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, B3270.Setting.NopSeconds, interval), ErrorBox.Completion(I18n.Get(Title.Settings)));
            }
        }
    }
}
