// <copyright file="FontSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using I18nBase;

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
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeFontSettings()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(fontTab)), "Tour: Font settings");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(fontTab)),
@"Modify the font used for the emulator display with this tab.");

            // Preview screen.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(fontPreviewScreenPictureBox)), "Preview window");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(fontPreviewScreenPictureBox)),
@"This window previews how the font will appear on the main window's emulator display.");

            // Preview screen.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(fontChangeButton)), "Change button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(fontChangeButton)),
@"Click to change the font.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

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
            this.screenFontDialog.Font = this.editedFont;
            if (this.screenFontDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            this.PropagateNewFont(this.screenFontDialog.Font);
        }

        /// <summary>
        /// Propagate the new font.
        /// </summary>
        /// <param name="newFont">New font.</param>
        public void PropagateNewFont(Font newFont)
        {
            // Propagate to the setting dialog.
            this.SetFont(newFont, this.editedColors, this.colorButton.Checked);
            this.fontLabel.Text = FriendlyName(newFont);
            this.fontScreenSample.ScreenBox.ScreenNewFont(newFont, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = newFont;

            // Change the profile.
            var newFontProfile = new FontProfile(this.editedFont);
            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    current.Font = newFontProfile;
                    current.Size = null;
                },
                ChangeName(ChangeKeyword.Font));
        }

        /// <summary>
        /// Change the colors of the fields on the Font tab.
        /// </summary>
        /// <param name="colors">Replacement colors.</param>
        /// <param name="colorMode">True if in 3279 mode.</param>
        public void RecolorFontTab(Colors colors, bool colorMode)
        {
            this.fontScreenSample.Invalidate();
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
            this.ProfileManager.AddChangeTo(this.FontProfileChanged);

            // Set up handler for edited color changes.
            this.EditedColorsChangedEvent += () => this.RecolorFontTab(this.editedColors, this.colorButton.Checked);

            // Set up handler for edited color mode changes.
            this.ColorModeChangedEvent += (color) => this.RecolorFontTab(this.editedColors, color);

            // Set up handler for dynamic font changes.
            this.mainScreen.DynamicFontEvent += this.DynamicFontChange;

            // Register our tour.
            var nodes = new[]
            {
                ((Control)this.fontTab, (int?)null, Orientation.Centered),
                (this.fontPreviewScreenPictureBox, null, Orientation.UpperLeftTight),
                (this.fontChangeButton, null, Orientation.UpperLeft),
            };
            this.RegisterTour(this.fontTab, nodes);
        }

        /// <summary>
        /// The profile changed. Apply font settings.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void FontProfileChanged(Profile oldProfile, Profile newProfile)
        {
            this.fontTab.Enabled = newProfile.ProfileType == ProfileType.Full;
            if (oldProfile == null || !oldProfile.Font.Equals(newProfile.Font))
            {
                var newFont = newProfile.Font.Font();
                this.SetFont(newFont, newProfile.Colors, newProfile.ColorMode);
            }
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
            this.SetFont(font, this.editedColors, this.colorButton.Checked);
            this.fontLabel.Text = FriendlyName(font);
            this.fontScreenSample.ScreenBox.ScreenNewFont(font, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = font;
        }
    }
}
