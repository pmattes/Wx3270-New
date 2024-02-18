// <copyright file="SoundSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;
    using I18nBase;

    /// <summary>
    /// Settings for sound effects.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Backing field for <see cref="ClickValue"/>.
        /// </summary>
        private bool currentClick = Profile.DefaultProfile.KeyClick;

        /// <summary>
        /// Backing field for <see cref="BellValue"/>.
        /// </summary>
        private bool currentBell = Profile.DefaultProfile.AudibleBell;

        /// <summary>
        /// Gets a value indicating whether the console bell is enabled.
        /// This is the "live" value referenced by key event handlers.
        /// </summary>
        public bool BellValue => this.currentBell;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeSoundSettings()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(soundsTab)), "Tour: Sound settings");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(soundsTab)),
@"Use this tab to change the sounds that wx3270 makes.");

            // Enable/disable.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(keyboardClickCheckBox)), "Enable/disable");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(keyboardClickCheckBox)),
@"Click to enable or disable the sound.");

            // Preview.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(playKeyboardClick)), "Sound preview");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(playKeyboardClick)),
@"Click to listen to the sound.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Initialize the Sounds tab.
        /// </summary>
        public void SoundsTabInit()
        {
            this.ProfileManager.AddChangeTo(this.SoundsProfileChange);

            // Register our tour.
            var nodes = new[]
            {
                ((Control)this.soundsTab, (int?)null, Orientation.Centered),
                (this.keyboardClickCheckBox, null, Orientation.UpperLeft),
                (this.playKeyboardClick, null, Orientation.UpperLeft),
            };
            this.RegisterTour(this.soundsTab, nodes);
        }

        /// <summary>
        /// The profile changed. Set up the sound settings options.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void SoundsProfileChange(Profile oldProfile, Profile newProfile)
        {
            this.soundsTab.Enabled = newProfile.ProfileType == ProfileType.Full;
            this.keyboardClickCheckBox.Checked = newProfile.KeyClick;
            this.audibleBellCheckBox.Checked = newProfile.AudibleBell;
        }

        /// <summary>
        /// Handler for playing the keyboard click sound sample.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyClickPlayButtonClick(object sender, EventArgs e)
        {
            // Play the key click sound.
            this.Sound.Play(Wx3270.Sound.KeyClick);
        }

        /// <summary>
        /// Handler for playing the console bell sound sample.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConsoleBellPlayButtonClick(object sender, EventArgs e)
        {
            // Play the beep sound.
            this.Sound.Play(Wx3270.Sound.Beep);
        }

        /// <summary>
        /// The keyboard click checkbox was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void KeyboardClickClick(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.KeyClick = this.keyboardClickCheckBox.Checked, ChangeName(ChangeKeyword.KeyboardClick));
        }

        /// <summary>
        /// The audible bell checkbox was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AudibleBellClick(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.AudibleBell = this.audibleBellCheckBox.Checked, ChangeName(ChangeKeyword.AudibleBell));
        }
    }
}
