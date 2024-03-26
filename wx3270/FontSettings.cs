// <copyright file="FontSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Media.TextFormatting;
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
        /// The font family names.
        /// </summary>
        private IEnumerable<string> familyNames = new List<string>();

        /// <summary>
        /// The saved familyComboBox font.
        /// </summary>
        private Font familyComboBoxFont;

        /// <summary>
        /// The saved family combo box height.
        /// </summary>
        private int familyComboBoxItemHeight;

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

            // Font selector.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(fontFlowLayoutPanel)), "Font selection");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(fontFlowLayoutPanel)),
@"Use these controls to select the font family, size and style.");

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
            this.fontScreenSample.ScreenBox.ScreenNewFont(font, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = font;

            this.editedFont = font;

            this.RecolorFontTab(colors, colorMode);
        }

        /// <summary>
        /// Propagate the new font.
        /// </summary>
        /// <param name="newFont">New font.</param>
        public void PropagateNewFont(Font newFont)
        {
            // Propagate to the setting dialog.
            this.SetFont(newFont, this.editedColors, this.colorButton.Checked);
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

            // Initialize the font combo boxes.
            this.familyNames = new FixedWidthFontEnumerator().Names;
            this.familyComboBox.Items.AddRange(this.familyNames.ToArray());
            this.fontSizeComboBox.Items.Clear();
            this.fontSizeComboBox.Items.AddRange(new[] { 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 }.Select(i => i.ToString()).ToArray());

            // Register our tour.
            var nodes = new[]
            {
                ((Control)this.fontTab, (int?)null, Orientation.Centered),
                (this.fontPreviewScreenPictureBox, null, Orientation.UpperLeftTight),
                (this.fontFlowLayoutPanel, null, Orientation.UpperLeft),
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

                // Set up the font options.
                this.SafeControlModify(
                    new[] { this.familyComboBox, this.fontSizeComboBox },
                    () =>
                    {
                        this.familyComboBox.Text = newProfile.Font.Name;
                        this.fontSizeComboBox.Text = newProfile.Font.EmSize.ToString();
                        if (newProfile.Font.Style.HasFlag(FontStyle.Bold))
                        {
                            this.boldButton.ForeColor = SystemColors.Control;
                            this.boldButton.BackColor = SystemColors.Highlight;
                        }
                        else
                        {
                            this.boldButton.ForeColor = SystemColors.ControlText;
                            this.boldButton.BackColor = SystemColors.Control;
                        }

                        if (newProfile.Font.Style.HasFlag(FontStyle.Italic))
                        {
                            this.italicButton.ForeColor = SystemColors.Control;
                            this.italicButton.BackColor = SystemColors.Highlight;
                        }
                        else
                        {
                            this.italicButton.ForeColor = SystemColors.ControlText;
                            this.italicButton.BackColor = SystemColors.Control;
                        }
                    });
            }
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
            this.fontSizeComboBox.Text = font.SizeInPoints.ToString();
            this.fontScreenSample.ScreenBox.ScreenNewFont(font, this.CreateSampleImage(this.ColorMode));
            this.fontScreenSample.Invalidate();
            this.fontPreviewStatusLineLabel.Font = font;
        }

        /// <summary>
        /// Push out a font family change.
        /// </summary>
        /// <param name="text">Family name.</param>
        private void FontFamilyPush(string text)
        {
            this.ProfileManager.PushAndSave(
                current =>
                {
                    current.Font = new FontProfile(new Font(text, float.Parse(this.fontSizeComboBox.Text), current.Font.Font().Style, GraphicsUnit.Point));
                },
                ChangeName(ChangeKeyword.Font));
        }

        /// <summary>
        /// The font family selection changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontFamilyChanged(object sender, EventArgs e)
        {
            if (!this.lockedControls.Contains(this.familyComboBox) && this.familyComboBox.SelectedIndex >= 0)
            {
                this.FontFamilyPush(this.familyComboBox.Text);
            }
        }

        /// <summary>
        /// The font family is validating.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FamilyValidating(object sender, CancelEventArgs e)
        {
            if (this.lockedControls.Contains(this.familyComboBox))
            {
                return;
            }

            var text = this.familyComboBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                this.familyComboBox.Text = this.ProfileManager.Current.Font.Name;
                return;
            }

            if (!this.familyNames.Contains(text, StringComparer.OrdinalIgnoreCase))
            {
                ErrorBox.Show(I18n.Get(Message.InvalidFontName), I18n.Get(Title.Settings));
                e.Cancel = true;
                return;
            }

            this.FontFamilyPush(text);
        }

        /// <summary>
        /// Push a new font size to the profile.
        /// </summary>
        /// <param name="text">Font size text.</param>
        private void FontSizePush(string text)
        {
            this.ProfileManager.PushAndSave(
                current =>
                {
                    current.Font = new FontProfile(new Font(this.familyComboBox.Text, float.Parse(text), current.Font.Font().Style, System.Drawing.GraphicsUnit.Point));
                },
                ChangeName(ChangeKeyword.Font));
        }

        /// <summary>
        /// The font size index changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontSizeChanged(object sender, EventArgs e)
        {
            if (!this.lockedControls.Contains(this.fontSizeComboBox) && this.fontSizeComboBox.SelectedIndex >= 0)
            {
                this.FontSizePush(this.fontSizeComboBox.Text);
            }
        }

        /// <summary>
        /// The font size is validating.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FontSizeValidating(object sender, CancelEventArgs e)
        {
            if (this.lockedControls.Contains(this.fontSizeComboBox))
            {
                return;
            }

            var text = this.fontSizeComboBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                this.fontSizeComboBox.Text = this.ProfileManager.Current.Font.EmSize.ToString();
                return;
            }

            if (!float.TryParse(text, out float size) || size <= 0.0 || size > 100.0)
            {
                ErrorBox.Show(I18n.Get(Message.InvalidFontSize), I18n.Get(Title.Settings));
                e.Cancel = true;
                return;
            }

            this.FontSizePush(text);
        }

        /// <summary>
        /// The Bold button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BoldClick(object sender, EventArgs e)
        {
            // Flip the state of bold.
            this.ProfileManager.PushAndSave(
                current =>
                {
                    var currentFont = current.Font.Font();
                    current.Font = new FontProfile(new Font(currentFont, currentFont.Style ^ FontStyle.Bold));
                },
                ChangeName(ChangeKeyword.Font));
        }

        /// <summary>
        /// The Italic button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ItalicClick(object sender, EventArgs e)
        {
            // Flip the state of italic.
            this.ProfileManager.PushAndSave(
                current =>
                {
                    var currentFont = current.Font.Font();
                    current.Font = new FontProfile(new Font(currentFont, currentFont.Style ^ FontStyle.Italic));
                },
                ChangeName(ChangeKeyword.Font));
        }

        /// <summary>
        /// The familyComboBox needs to be drawn.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event agruments.</param>
        private void FamilyComboBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            var font = new Font((string)this.familyComboBox.Items[e.Index], this.familyComboBox.Font.SizeInPoints, FontStyle.Regular);
            e.DrawBackground();
            e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }

        /// <summary>
        /// The family combo box drop down was opened.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FamilyComboBoxDropDown(object sender, EventArgs e)
        {
            this.familyComboBoxFont = this.familyComboBox.Font;
            this.familyComboBoxItemHeight = this.familyComboBox.ItemHeight;
            this.familyComboBox.Font = new Font(this.familyComboBox.Font.Name, 16.0F, this.familyComboBox.Font.Style);
            this.familyComboBox.ItemHeight = 24;
        }

        /// <summary>
        /// The family combo box drop down was closed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FamilyComboBoxDropDownClosed(object sender, EventArgs e)
        {
            this.familyComboBox.Font = this.familyComboBoxFont;
            this.familyComboBox.ItemHeight = this.familyComboBoxItemHeight;
        }
    }
}
