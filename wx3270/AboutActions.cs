// <copyright file="AboutActions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
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
