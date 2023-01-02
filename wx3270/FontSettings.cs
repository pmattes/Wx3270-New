// <copyright file="FontSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The Font tab in the settings dialog.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// The currently selected font.
        /// </summary>
        private Font editedFont;

        /// <summary>
        /// The screen sample.
        /// </summary>
        private ScreenSample fontScreenSample;

        /// <summary>
        /// Gets the friendly (display) name for a font.
        /// </summary>
        /// <param name="font">Font to translate.</param>
        /// <returns>Friendly name.</returns>
        public static string FriendlyName(Font font)
        {
            string ret = font.Name;
            if (font.Bold)
            {
                ret += " Bold";
            }

            if (font.Italic)
            {
                ret += " Italic";
            }

            return $"{ret}, {font.SizeInPoints}pt";
        }

        /// <summary>
        /// Sets the displayed screen font.
        /// </summary>
        /// <param name="font">Font to display.</param>
        /// <param name="colors">Color table.</param>
        /// <param name="colorMode">True if in 3279 mode.</param>
        public void SetFont(Font font, Colors colors, bool colorMode)
        {
            this.fontLabel.Text = FriendlyName(font);
            this.fontScreenSample.ScreenBox.ScreenNewFont(font, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = font;

            this.editedFont = font;

            this.RecolorFontTab(colors, colorMode);
        }

        /// <summary>
        /// Screen font change button.
        /// </summary>
        public void ChangeFont()
        {
            // Pop up the font dialog.
            this.ScreenFontDialog.Font = this.editedFont;
            if (this.ScreenFontDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            this.PropagateNewFont(this.ScreenFontDialog.Font);
        }

        /// <summary>
        /// Step the font (one size bigger or smaller).
        /// </summary>
        /// <param name="newFont">New font.</param>
        public void PropagateNewFont(Font newFont)
        {
            // Propagate to the setting dialog.
            this.SetFont(newFont, this.editedColors, this.ColorButton.Checked);
            this.fontLabel.Text = FriendlyName(newFont);
            this.fontScreenSample.ScreenBox.ScreenNewFont(newFont, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = newFont;

            // Change the screen.
            this.mainScreen.Refont(newFont);

            // Change the profile.
            var newFontProfile = new FontProfile(this.editedFont);
            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.Font = newFontProfile;
                    current.Size = this.mainScreen.Size;
                },
                this.ChangeName(ChangeKeyword.Font));
        }

        /// <summary>
        /// Change the colors of the fields on the Font tab.
        /// </summary>
        /// <param name="colors">Replacement colors.</param>
        /// <param name="colorMode">True if in 3279 mode.</param>
        public void RecolorFontTab(Colors colors, bool colorMode)
        {
            // Nothing for now
        }

        /// <summary>
        /// Initialize the Font tab.
        /// </summary>
        private void FontTabInit()
        {
            // Set up the screen sample.
            this.fontPreviewStatusLineLabel.Font = Profile.DefaultProfile.Font.Font();
            this.fontScreenSample = new ScreenSample(
                this,
                this.fontPreviewScreenPictureBox,
                this.fontPreviewTableLayoutPanel,
                this.fontPreviewStatusLineLabel,
                this.fontPreviewSeparatorPictureBox,
                this.ColorMode);

            // Set up handler for profile changes.
            this.ProfileManager.Change += this.FontProfileChanged;

            // Register the merge handler.
            this.ProfileManager.RegisterMerge(ImportType.FontReplace, this.MergeFont);

            // Set up handler for edited color changes.
            this.EditedColorsChangedEvent += () => this.RecolorFontTab(this.editedColors, this.ColorButton.Checked);

            // Set up handler for edited color mode changes.
            this.ColorModeChangedEvent += (color) => this.RecolorFontTab(this.editedColors, color);

            // Set up handler for dynamic font changes.
            this.mainScreen.DynamicFontEvent += this.DynamicFontChange;
        }

        /// <summary>
        /// Merge the font from a different profile.
        /// </summary>
        /// <param name="toProfile">Current profile.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if the font changed.</returns>
        private bool MergeFont(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (!toProfile.Font.Equals(fromProfile.Font))
            {
                toProfile.Font = fromProfile.Font;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The profile changed. Apply font settings.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void FontProfileChanged(Profile profile)
        {
            this.fontTab.Enabled = profile.ProfileType == ProfileType.Full;
            var newFont = profile.Font.Font();
            this.SetFont(newFont, profile.Colors, profile.ColorMode);
            this.mainScreen.Refont(newFont);
        }

        /// <summary>
        /// Handler for the font change button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontChangeButtonClick(object sender, EventArgs e)
        {
            this.ChangeFont();
        }

        /// <summary>
        /// Paint method for the font preview screen.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontPreviewScreenPictureBoxPaint(object sender, PaintEventArgs e)
        {
            // Use the standard method.
            this.SamplePaint(sender, e, this.fontScreenSample);
        }

        /// <summary>
        /// The font changed because of a user-initiated window resize.
        /// </summary>
        /// <param name="font">New font value.</param>
        private void DynamicFontChange(Font font)
        {
            // Propagate the results to the setting dialog.
            this.SetFont(font, this.editedColors, this.ColorButton.Checked);
            this.fontLabel.Text = FriendlyName(font);
            this.fontScreenSample.ScreenBox.ScreenNewFont(font, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = font;
        }
    }
}
