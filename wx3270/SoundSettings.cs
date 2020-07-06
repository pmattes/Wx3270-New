// <copyright file="SoundSettings.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

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
        /// Initialize the Sounds tab.
        /// </summary>
        public void SoundsTabInit()
        {
            this.ProfileManager.Change += this.SoundsProfileChange;
            this.ProfileManager.RegisterMerge(ImportType.OtherSettingsReplace, this.MergeSound);
        }

        /// <summary>
        /// The profile changed. Set up the sound settings options.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void SoundsProfileChange(Profile profile)
        {
            this.soundsTab.Enabled = profile.ProfileType == ProfileType.Full;
            this.keyboardClickCheckBox.Checked = profile.KeyClick;
            this.audibleBellCheckBox.Checked = profile.AudibleBell;
        }

        /// <summary>
        /// Merge the sound settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        private bool MergeSound(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.KeyClick == fromProfile.KeyClick && toProfile.AudibleBell == fromProfile.AudibleBell)
            {
                return false;
            }

            toProfile.KeyClick = fromProfile.KeyClick;
            toProfile.AudibleBell = fromProfile.AudibleBell;
            return true;
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
            this.ProfileManager.PushAndSave((current) => current.KeyClick = this.keyboardClickCheckBox.Checked, this.ChangeName(ChangeKeyword.KeyboardClick));
        }

        /// <summary>
        /// The audible bell checkbox was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AudibleBellClick(object sender, EventArgs e)
        {
            this.ProfileManager.PushAndSave((current) => current.AudibleBell = this.audibleBellCheckBox.Checked, this.ChangeName(ChangeKeyword.AudibleBell));
        }
    }
}
