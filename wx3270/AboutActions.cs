// <copyright file="AboutActions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;
    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// About tab in the actions dialog.
    /// </summary>
    public partial class Actions
    {
        /// <summary>
        /// The name of the copyright message.
        /// </summary>
        private const string CopyrightName = "Copyright";

        /// <summary>
        /// The back end hello handler.
        /// </summary>
        private IHello hello;

        /// <summary>
        /// The back end TLS hello handler.
        /// </summary>
        private ITlsHello tlsHello;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeAboutActions()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(aboutTab)), "Tour: About");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(aboutTab)),
@"Use this tab to display copyright and configuration information.");

            // About wx3270.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(guiCopyrightTextBox)), "wx3270 build and copyright");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(guiCopyrightTextBox)),
@"This is the build information and copyright notice for wx3270.");

            // Culture.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(cultureLabel)), "Culture");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(cultureLabel)),
@"The culture is displayed here, which controls localized text.

wx3270 does not have localizations for all cultures, so it might be using a less-specific or fallback culture.");

            // b3270 copyright.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(backendCopyrightTextBox)), "b3270 build and copyright");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(backendCopyrightTextBox)),
@"This is the build information and copyright notice for b3270, the emulator back-end process.");

            // TLS provider.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(tlsProviderValueLabel)), "TLS provider");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(tlsProviderValueLabel)),
@"The TLS provider (the library that implements the TLS protocol) is displayed here.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(helpPictureBox2)), "Help");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(helpPictureBox2)),
@"Click to display context-sensitive help from the x3270 Wiki in your browser, or to start this tour again.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Localize the About tab.
        /// </summary>
        private static void AboutLocalize()
        {
            I18n.LocalizeGlobal(CopyrightName, Constants.Copyright, true);
        }

        /// <summary>
        /// Initialize the About tab.
        /// </summary>
        private void AboutTabInit()
        {
            this.hello = this.app.Hello;
            this.tlsHello = this.app.TlsHello;

            // Fill in the version and copyright strings.
            this.guiVersionLabel.Text = "wx3270 " + Profile.VersionClass.FullVersion;
            this.guiCopyrightTextBox.Text = I18n.Get(CopyrightName);

            // Go get the hello fields.
            this.OnHello();

            // Fill in localization information.
            this.cultureLabel.Text =
                I18n.Localize(this.cultureLabel, "requested", "Requested culture") + ": " + I18nBase.RequestedCulture + "; " +
                I18n.Localize(this.cultureLabel, "effective", "effective culture") + ": " + I18nBase.EffectiveCulture;

            // Subscribe to changes, in case they haven't been set yet.
            this.hello.Add(this.mainScreen, this.OnHello);
            this.tlsHello.Add(this.mainScreen, this.OnHello);

            // Register the tour.
            var nodes = new[]
            {
                ((Control)this.aboutTab, (int?)null, Orientation.Centered),
                (this.guiCopyrightTextBox, null, Orientation.UpperLeftTight),
                (this.cultureLabel, null, Orientation.UpperLeft),
                (this.backendCopyrightTextBox, null, Orientation.LowerLeftTight),
                (this.tlsProviderValueLabel, null, Orientation.LowerLeft),
                (this.helpPictureBox2, null, Orientation.LowerRight),
            };
            this.RegisterTour(this.aboutTab, nodes);
        }

        /// <summary>
        /// Handle a hello message.
        /// </summary>
        private void OnHello()
        {
            this.backendBuildLabel.Text = this.hello.Build;
            this.backendCopyrightTextBox.Text = this.hello.Copyright;
            this.tlsProviderValueLabel.Text = this.tlsHello.Provider;
        }
    }
}
