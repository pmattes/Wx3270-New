// <copyright file="ModelChange.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Crossbar between the back end and the profile for options settings.
    /// </summary>
    public partial class MainScreen : Form
    {
        /// <summary>
        /// Describes what changed in the model.
        /// </summary>
        /// <param name="current">Current profile.</param>
        /// <param name="model">New model number.</param>
        /// <param name="isOversize">New oversize mode.</param>
        /// <param name="rows">New oversize rows.</param>
        /// <param name="columns">New oversize columns.</param>
        /// <param name="colorMode">New color mode.</param>
        /// <param name="extendedMode">New extended mode.</param>
        /// <returns>List of items that changed.</returns>
        private string WhatModelChanged(Profile current, int model, bool isOversize, int rows, int columns, bool colorMode, bool extendedMode)
        {
            var whatChanged = new List<string>();
            if (current.Model != model)
            {
                whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.Model)));
            }

            var oversizeChanged = current.Oversize.HasValue() != isOversize;
            if (oversizeChanged)
            {
                whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.OversizeMode)));
            }

            if (oversizeChanged && isOversize)
            {
                // Oversize turned on. Non-default values count.
                if (rows != this.App.ModelsDb.DefaultRows(model))
                {
                    whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.OversizeRows)));
                }

                if (columns != this.App.ModelsDb.DefaultColumns(model))
                {
                    whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.OversizeColumns)));
                }
            }
            else if (isOversize)
            {
                // Oversize stayed on. Compare to current.
                if (rows != current.Oversize.Rows)
                {
                    whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.OversizeRows)));
                }

                if (columns != current.Oversize.Columns)
                {
                    whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.OversizeColumns)));
                }
            }

            if (current.ColorMode != colorMode)
            {
                whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.ColorMode)));
            }

            if (current.ExtendedMode != extendedMode)
            {
                whatChanged.Add(I18n.Get(Settings.SettingPath(Settings.ChangeKeyword.ExtendedMode)));
            }

            return string.Join(", ", whatChanged);
        }

        /// <summary>
        /// Push a new model (model number, oversize, color mode) from the back end to the profile, based on an external change to
        /// one of the resources.
        /// </summary>
        private void ModelBackEndToProfile()
        {
            if (!this.App.BackEnd.Ready)
            {
                // This is just initial back end state.
                return;
            }

            // Get the screen image.
            var image = this.App.ScreenImage;

            var whatChanged = this.WhatModelChanged(
                this.ProfileManager.Current,
                image.Model,
                image.Oversize,
                image.MaxRows,
                image.MaxColumns,
                image.ColorMode,
                image.Extended);
            if (string.IsNullOrEmpty(whatChanged))
            {
                // Nothing has changed.
                return;
            }

            this.ProfileManager.PushAndSave(
                (current) =>
                {
                    // Apply the model.
                    current.Model = image.Model;

                    // Apply oversize.
                    current.Oversize.Rows = image.Oversize ? image.MaxRows : 0;
                    current.Oversize.Columns = image.Oversize ? image.MaxColumns : 0;

                    // Apply monochrome/color mode.
                    current.ColorMode = image.ColorMode;

                    // Apply extended mode.
                    current.ExtendedMode = image.Extended;

                    // Set the window size.
                    // The back end settings arrive here via a screen mode update.
                    // The model change has likely caused the window size to change, so we store it here.
                    // We don't want to do this when maximized or docked.
                    if (!this.Maximized && !this.IsWindowArranged())
                    {
                        current.Size = this.Size;
                    }
                },
                Wx3270.ProfileManager.ChangeName(string.Format("{0} ({1})", whatChanged, this.ProfileManager.ExternalText)));
        }
    }
}