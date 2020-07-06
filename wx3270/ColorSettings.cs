// <copyright file="ColorSettings.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// The Colors tab in the settings dialog.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// The "black on white" color scheme.
        /// </summary>
        public static readonly HostColors BlackOnWhiteScheme = new HostColors
        {
            { HostColor.NeutralBlack, Color.FromArgb(255, 248, 248, 248) },
            { HostColor.Blue, Color.RoyalBlue },
            { HostColor.Red, Color.Red },
            { HostColor.Pink, Color.HotPink },
            { HostColor.Green, Color.FromArgb(255, 0, 164, 0) },
            { HostColor.Turquoise, Color.Green },
            { HostColor.Yellow, Color.FromArgb(255, 241, 193, 3) },
            { HostColor.NeutralWhite, Color.Black },
            { HostColor.Black, Color.Black },
            { HostColor.DeepBlue, Color.Blue },
            { HostColor.Orange, Color.FromArgb(255, 228, 157, 126) },
            { HostColor.Purple, Color.DarkViolet },
            { HostColor.PaleGreen, Color.FromArgb(255, 7, 243, 7) },
            { HostColor.PaleTurquoise, Color.FromArgb(255, 108, 227, 204) },
            { HostColor.Grey, Color.Gray },
            { HostColor.White, Color.White },
        };

        /// <summary>
        /// The set of 48 "basic" colors in the color dialog.
        /// Defining this manually is insanity, but it appears to be the only way.
        /// </summary>
        private readonly HashSet<Color> basicColors = new HashSet<Color>
        {
            Color.FromArgb(255, 255, 128, 128),
            Color.FromArgb(255, 255, 255, 128),
            Color.FromArgb(255, 128, 255, 128),
            Color.FromArgb(255, 0, 255, 128),
            Color.FromArgb(255, 128, 255, 255),
            Color.FromArgb(255, 0, 128, 255),
            Color.FromArgb(255, 255, 128, 192),
            Color.FromArgb(255, 255, 128, 255),
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 128, 255, 0),
            Color.FromArgb(255, 0, 255, 64),
            Color.FromArgb(255, 0, 255, 255),
            Color.FromArgb(255, 0, 128, 192),
            Color.FromArgb(255, 128, 128, 192),
            Color.FromArgb(255, 255, 0, 255),
            Color.FromArgb(255, 128, 64, 64),
            Color.FromArgb(255, 255, 128, 64),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 128, 128),
            Color.FromArgb(255, 0, 64, 128),
            Color.FromArgb(255, 128, 128, 255),
            Color.FromArgb(255, 128, 0, 64),
            Color.FromArgb(255, 255, 0, 128),
            Color.FromArgb(255, 128, 0, 0),
            Color.FromArgb(255, 255, 128, 0),
            Color.FromArgb(255, 0, 128, 0),
            Color.FromArgb(255, 0, 128, 64),
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 0, 0, 160),
            Color.FromArgb(255, 128, 0, 128),
            Color.FromArgb(255, 128, 0, 255),
            Color.FromArgb(255, 64, 0, 0),
            Color.FromArgb(255, 128, 64, 0),
            Color.FromArgb(255, 0, 64, 0),
            Color.FromArgb(255, 0, 64, 64),
            Color.FromArgb(255, 0, 0, 128),
            Color.FromArgb(255, 0, 0, 64),
            Color.FromArgb(255, 64, 0, 64),
            Color.FromArgb(255, 64, 0, 128),
            Color.FromArgb(255, 0, 0, 0),
            Color.FromArgb(255, 128, 128, 0),
            Color.FromArgb(255, 128, 128, 64),
            Color.FromArgb(255, 128, 128, 128),
            Color.FromArgb(255, 64, 128, 128),
            Color.FromArgb(255, 192, 192, 192),
            Color.FromArgb(255, 64, 0, 64),
            Color.FromArgb(255, 255, 255, 255),
        };

        /// <summary>
        /// The "green on white" monochrome color scheme.
        /// </summary>
        private readonly MonoColors greenOnWhiteScheme = new MonoColors
        {
            Normal = Color.FromArgb(255, 0, 157, 0),
            Intensified = Color.DarkGreen,
            Background = Color.FromArgb(255, 248, 248, 248),
        };

        /// <summary>
        /// Color sample screen.
        /// </summary>
        private ScreenSample colorScreenSample;

        /// <summary>
        /// Monochrome sample screen.
        /// </summary>
        private ScreenSample monoScreenSample;

        /// <summary>
        /// Application instance.
        /// </summary>
        private Wx3270App app;

        /// <summary>
        /// The main screen.
        /// </summary>
        private MainScreen mainScreen;

        /// <summary>
        /// The edited colors.
        /// </summary>
        private Colors editedColors = new Colors(Profile.DefaultProfile.Colors);

        /// <summary>
        /// Event called when any of the edited colors change.
        /// </summary>
        private event Action EditedColorsChangedEvent = () => { };

        /// <summary>
        /// Types of monochrome color buttons.
        /// </summary>
        private enum MonoColorType
        {
            /// <summary>
            /// No match.
            /// </summary>
            None,

            /// <summary>
            /// Background color.
            /// </summary>
            Background,

            /// <summary>
            /// Selection background color.
            /// </summary>
            SelectBackground,

            /// <summary>
            /// Normal color.
            /// </summary>
            Normal,

            /// <summary>
            /// Intensified color.
            /// </summary>
            Intensified,
        }

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager => this.app.ProfileManager;

        /// <summary>
        /// Map a color-related control name to a color.
        /// </summary>
        /// <param name="controlName">Name of the control.</param>
        /// <param name="suffix">Suffix to remove.</param>
        /// <param name="hostColor">Returned host color.</param>
        /// <returns>True if successful.</returns>
        private static bool TryHostColorFromControlName(string controlName, string suffix, out HostColor hostColor)
        {
            string hostColorName = controlName.Substring(0, controlName.Length - suffix.Length);
            if (hostColorName.Equals("Gray", StringComparison.InvariantCultureIgnoreCase))
            {
                hostColorName = "Grey";
            }

            return Enum.TryParse(hostColorName, true, out hostColor);
        }

        /// <summary>
        /// Set up the Color tab.
        /// </summary>
        private void ColorTabInit()
        {
            // Set up the screen boxes.
            this.colorScreenSample = new ScreenSample(
                this,
                this.colorPreviewScreenPictureBox,
                this.colorPreviewTableLayoutPanel,
                this.colorPreviewStatusLineLabel,
                this.colorPreviewSeparatorPictureBox,
                true);
            this.monoScreenSample = new ScreenSample(
                this,
                this.monoPreviewScreenPictureBox,
                this.monoPreviewTableLayoutPanel,
                this.monoPreviewStatusLineLabel,
                this.monoPreviewSeparatorPictureBox,
                false);

            // Set up event handler for profile changes.
            this.ProfileManager.Change += (profile) =>
            {
                this.ResetColorMap(profile.Colors);
                this.colors3279Tab.Enabled = profile.ProfileType == ProfileType.Full && profile.ColorMode;
                this.colors3278Tab.Enabled = profile.ProfileType == ProfileType.Full && !profile.ColorMode;
            };

            // Set up the initial enables for the color tabs, and set up a handler for when the
            // mode changes.
            this.colors3279Tab.Enabled = this.ProfileManager.Current.ColorMode;
            this.colors3278Tab.Enabled = !this.ProfileManager.Current.ColorMode;
            this.ColorModeChangedEvent += (color) =>
            {
                this.colors3279Tab.Enabled = color;
                this.colors3278Tab.Enabled = !color;
            };

            // Set up merge handlers.
            this.ProfileManager.RegisterMerge(ImportType.ColorsReplace, this.MergeColors);

            // Set up event handler for edited colors changes.
            this.EditedColorsChangedEvent += this.RepaintColorTab;
            this.EditedColorsChangedEvent += this.PushColors;

            // Set up the background list indices.
            this.BackgroundList.Items.AddRange(ColorBackground.Objects().Select(o => new ColorBackground
            {
                DisplayName = I18n.Localize(this.BackgroundList, o.InternalName, o.DisplayName),
                InternalName = o.InternalName,
            }).ToArray());
            this.BackgroundList.SelectedIndex = 0;
            this.MonoBackgroundList.Items.AddRange(MonoBackground.Objects().Select(o => new MonoBackground
            {
                DisplayName = I18n.Localize(this.MonoBackgroundList, o.InternalName, o.DisplayName),
                InternalName = o.InternalName,
            }).ToArray());
            this.MonoBackgroundList.SelectedIndex = 0;
        }

        /// <summary>
        /// Merge colors from a different profile.
        /// </summary>
        /// <param name="toProfile">Current profile.</param>
        /// <param name="fromProfile">Merge profile.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if profile changed.</returns>
        private bool MergeColors(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (!toProfile.Colors.Equals(fromProfile.Colors))
            {
                toProfile.Colors = fromProfile.Colors;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Apply changed colors or color mode to the screen and profile.
        /// </summary>
        private void PushColors()
        {
            // Apply to the screen.
            this.mainScreen.Recolor(this.editedColors, this.ColorMode);

            // Apply to the profile.
            this.ProfileManager.PushAndSave((current) => current.Colors = new Colors(this.editedColors), this.ChangeName(ChangeKeyword.Color));
        }

        /// <summary>
        /// Reset the host color map.
        /// </summary>
        /// <param name="colors">New colors.</param>
        private void ResetColorMap(Colors colors)
        {
            this.editedColors = new Colors(colors);
            this.EditedColorsChangedEvent();
        }

        /// <summary>
        /// Repaint the color tab, according to the current color map. Called when one or more of the edited color choices changes.
        /// </summary>
        private void RepaintColorTab()
        {
            var hostColors = this.editedColors.HostColors;

            // Set the color scheme radio buttons.
            if (hostColors.Equals(Profile.DefaultProfile.Colors.HostColors))
            {
                if (!this.whiteOnBlackButton.Checked)
                {
                    this.whiteOnBlackButton.Checked = true;
                }
            }
            else if (hostColors.Equals(BlackOnWhiteScheme))
            {
                if (!this.blackOnWhiteButton.Checked)
                {
                    this.blackOnWhiteButton.Checked = true;
                }
            }
            else if (!this.customButton.Checked)
            {
                this.customButton.Checked = true;
            }

            // Set the swatches and samples.
            foreach (var controlObject in this.hostColorsBox.Controls)
            {
                var control = controlObject as Control;
                if (control == null)
                {
                    continue;
                }

                HostColor hostColor;
                if (control.Name.EndsWith("Swatch"))
                {
                    if (TryHostColorFromControlName(control.Name, "Swatch", out hostColor))
                    {
                        control.BackColor = hostColors[hostColor];
                    }
                }
                else if (control.Name.EndsWith("Sample"))
                {
                    if (TryHostColorFromControlName(control.Name, "Sample", out hostColor))
                    {
                        control.ForeColor = hostColors[hostColor];
                    }
                }
            }

            // Set the selection color.
            this.selectionSwatch.BackColor = this.editedColors.SelectBackground;

            // Set the crosshair color.
            this.crosshairSwatch.BackColor = this.editedColors.CrosshairColor;

            // Set the background colors of the samples.
            this.BackgroundListSelectedIndexChanged(this.BackgroundList, null);

            // Redraw the preview.
            this.colorScreenSample.Invalidate();

            // Do the same for the mono colors tab.
            this.RepaintMonoColorTab();
        }

        /// <summary>
        /// Repaint the 3278 Colors tab.
        /// </summary>
        private void RepaintMonoColorTab()
        {
            // Set the monochrome color scheme radio buttons.
            if (this.editedColors.MonoColors.Equals(Profile.DefaultProfile.Colors.MonoColors))
            {
                this.greenOnBlackButton.Checked = true;
            }
            else if (this.editedColors.MonoColors.Equals(this.greenOnWhiteScheme))
            {
                this.greenOnWhiteButton.Checked = true;
            }
            else
            {
                this.monoCustomButton.Checked = true;
            }

            // Set the swatches and samples.
            this.monoBackgroundSwatch.BackColor = this.editedColors.MonoColors.Background;
            this.monoSelectBackgroundSwatch.BackColor = this.editedColors.SelectBackground;
            this.monoNormalSwatch.BackColor = this.editedColors.MonoColors.Normal;
            this.monoNormalSample.ForeColor = this.editedColors.MonoColors.Normal;
            this.monoIntensifiedSwatch.BackColor = this.editedColors.MonoColors.Intensified;
            this.monoIntensifiedSample.ForeColor = this.editedColors.MonoColors.Intensified;
            this.monoCrosshairSample.BackColor = this.editedColors.CrosshairColor;

            // Set the background colors of the samples.
            this.MonoBackgroundListSelectedIndexChanged(this.MonoBackgroundList, null);

            // Redraw the preview.
            this.monoScreenSample.Invalidate();
        }

        /// <summary>
        /// Redefine a host color.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void HostColorPicker(object sender, EventArgs e)
        {
            // Figure out the color being changed.
            var control = sender as Control;
            if (control == null)
            {
                return;
            }

            HostColor hostColor;
            if (!(control is Button && TryHostColorFromControlName(control.Name, "Button", out hostColor)) &&
                !(control.Name.EndsWith("Sample") && TryHostColorFromControlName(control.Name, "Sample", out hostColor)) &&
                !(control.Name.EndsWith("Swatch") && TryHostColorFromControlName(control.Name, "Swatch", out hostColor)))
            {
                return;
            }

            var color = this.editedColors.HostColors[hostColor];
            if (this.ChangeColor(ref color))
            {
                // Officially it's custom now.
                this.customButton.Checked = true;

                // Change the color map.
                this.editedColors.HostColors[hostColor] = color;
                this.EditedColorsChangedEvent();
            }
        }

        /// <summary>
        /// Redefine a monochrome host color.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoColorPicker(object sender, EventArgs e)
        {
            // Figure out the color being changed.
            var control = sender as Control;
            if (control == null)
            {
                return;
            }

            // Figure out the color.
            var colorType = MonoColorType.None;
            foreach (var t in Enum.GetValues(typeof(MonoColorType)).Cast<MonoColorType>())
            {
                if (t != MonoColorType.None && control.Name.StartsWith("mono" + t.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    colorType = t;
                    break;
                }
            }

            Color color;
            switch (colorType)
            {
                default:
                case MonoColorType.None:
                    return;
                case MonoColorType.Background:
                    color = this.editedColors.MonoColors.Background;
                    break;
                case MonoColorType.SelectBackground:
                    color = this.editedColors.SelectBackground;
                    break;
                case MonoColorType.Normal:
                    color = this.editedColors.MonoColors.Normal;
                    break;
                case MonoColorType.Intensified:
                    color = this.editedColors.MonoColors.Intensified;
                    break;
            }

            // Pop up the dialog.
            if (!this.ChangeColor(ref color))
            {
                return;
            }

            // Change thr color.
            switch (colorType)
            {
                case MonoColorType.Background:
                    this.editedColors.MonoColors.Background = color;
                    break;
                case MonoColorType.SelectBackground:
                    this.editedColors.SelectBackground = color;
                    break;
                case MonoColorType.Normal:
                    this.editedColors.MonoColors.Normal = color;
                    break;
                case MonoColorType.Intensified:
                    this.editedColors.MonoColors.Intensified = color;
                    break;
            }

            // Officially it's custom now.
            this.monoCustomButton.Checked = true;

            // Propagate the change.
            this.EditedColorsChangedEvent();
        }

        /// <summary>
        /// Change the selection background color.
        /// </summary>
        private void ChangeSelectionColor()
        {
            var selectBackground = this.editedColors.SelectBackground;
            if (this.ChangeColor(ref selectBackground))
            {
                this.editedColors.SelectBackground = selectBackground;
                this.EditedColorsChangedEvent();
            }
        }

        /// <summary>
        /// Change the crosshair cursor color.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CrosshairColorButtonClick(object sender, EventArgs e)
        {
            var crosshairColor = this.editedColors.CrosshairColor;
            if (this.ChangeColor(ref crosshairColor))
            {
                this.editedColors.CrosshairColor = crosshairColor;
                this.EditedColorsChangedEvent();
            }
        }

        /// <summary>
        /// Redefine a color. A wrapper around the color picker dialog that manages the custom colors.
        /// </summary>
        /// <param name="color">Color to change.</param>
        /// <returns>True if color changed.</returns>
        private bool ChangeColor(ref Color color)
        {
            // See if it is a basic color.
            var argb = color.ToArgb();
            if (!this.basicColors.Any(c => c.ToArgb() == argb))
            {
                // See if it is already defined as a custom color.
                var customColors = new List<Color>();
                foreach (var customColor in this.HostColorDialog.CustomColors)
                {
                    var drawingColor = ColorTranslator.FromOle(customColor);
                    if (drawingColor != Color.White)
                    {
                        customColors.Add(ColorTranslator.FromOle(customColor));
                    }
                }

                if (!customColors.Contains(color))
                {
                    // No, add it.
                    if (customColors.Count() < 16)
                    {
                        // Easy. Append it.
                        customColors.Add(color);
                    }
                    else
                    {
                        // Not so easy. Replace the last one.
                        customColors[15] = color;
                    }
                }

                // Reset the custom colors.
                this.HostColorDialog.CustomColors = customColors.Select(c => ColorTranslator.ToOle(c)).ToArray();
            }

            // Show the dialog.
            this.HostColorDialog.Color = color;
            this.HostColorDialog.ShowDialog();

            // When the dialog is finished, propagate the color.
            var newColor = this.HostColorDialog.Color;
            if (newColor == color)
            {
                return false;
            }

            color = newColor;
            return true;
        }

        /// <summary>
        /// The list index changed for the background color selection.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void BackgroundListSelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            // Figure out which one it is.
            var item = listBox.SelectedItem as ColorBackground;
            if (item == null)
            {
                return;
            }

            // Set the samples' background colors.
            var color = item.ColorValue(this.editedColors);
            foreach (var sample in this.hostColorsBox.Controls.OfType<Label>().Where(c => c.Name.EndsWith("Sample")))
            {
                sample.BackColor = color;
            }
        }

        /// <summary>
        /// The list index changed for the monochrome background color selection.
        /// </summary>
        /// <param name="sender">Sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoBackgroundListSelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            // Figure out which one it is.
            var item = listBox.SelectedItem as MonoBackground;
            if (item == null)
            {
                return;
            }

            // Set the samples' background colors.
            var color = item.ColorValue(this.editedColors);
            this.monoNormalSample.BackColor = color;
            this.monoIntensifiedSample.BackColor = color;
        }

        /// <summary>
        /// Handler for a white-on-black radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void WhiteOnBlackClick(object sender, EventArgs e)
        {
            // The color scheme was changed to or from White on Black.
            var button = sender as RadioButton;
            if (button == null || !button.Checked)
            {
                return;
            }

            // Switch to the default color map.
            this.editedColors.HostColors = new HostColors(Profile.DefaultProfile.Colors.HostColors);
            this.EditedColorsChangedEvent();
        }

        /// <summary>
        /// Handler for the black-on-white radio button ckick.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BlackOnWhiteCheckedClick(object sender, EventArgs e)
        {
            // The color scheme was changed to or from Black on White.
            var button = sender as RadioButton;
            if (button == null || !button.Checked)
            {
                return;
            }

            // Switch to the black-on-white color map.
            this.editedColors.HostColors = new HostColors(BlackOnWhiteScheme);
            this.EditedColorsChangedEvent();
        }

        /// <summary>
        /// Handler for the green-on-black radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void GreenOnBlackButtonClick(object sender, EventArgs e)
        {
            // The monochrome color scheme changed to or from Green on Black.
            var button = sender as RadioButton;
            if (button == null || !button.Checked)
            {
                return;
            }

            // Switch to the default monochrome color map.
            this.editedColors.MonoColors = new MonoColors(Profile.DefaultProfile.Colors.MonoColors);
            this.EditedColorsChangedEvent();
        }

        /// <summary>
        /// Handler for the green-on-white radio button click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void GreenOnWhiteButtonClick(object sender, EventArgs e)
        {
            // The monochrome color scheme changed to or from Green on White.
            var button = sender as RadioButton;
            if (button == null || !button.Checked)
            {
                return;
            }

            // Switch to the default monochrome color map.
            this.editedColors.MonoColors = new MonoColors(this.greenOnWhiteScheme);
            this.EditedColorsChangedEvent();
        }

        /// <summary>
        /// Handler for clicking on any of the color editing buttons.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColorClick(object sender, EventArgs e)
        {
            this.HostColorPicker(sender, e);
        }

        /// <summary>
        /// Handler for clicking on any of the monochrome color editing buttons.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoClick(object sender, EventArgs e)
        {
            this.MonoColorPicker(sender, e);
        }

        /// <summary>
        /// Paint method for the monochrome preview box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MonoPreviewScreenPictureBoxPaint(object sender, PaintEventArgs e)
        {
            // Use the standard method.
            this.SamplePaint(sender, e, this.monoScreenSample, false);
        }

        /// <summary>
        /// Paint method for the color preview box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ColorPreviewScreenPictureBoxPaint(object sender, PaintEventArgs e)
        {
            // Use the standard method.
            this.SamplePaint(sender, e, this.colorScreenSample, true);
        }
    }
}
