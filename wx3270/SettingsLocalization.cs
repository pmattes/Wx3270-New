// <copyright file="SettingsLocalization.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using I18nBase;

    /// <summary>
    /// Localization data for the settings dialog.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Settings));

        /// <summary>
        /// Message group for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Settings));

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.Settings, "Settings");

            I18n.LocalizeGlobal(Message.CantParseListen, "Can't parse {0} listen port value '{1}'");
            I18n.LocalizeGlobal(Message.InvalidListenValue, "Invalid {0} value");
            I18n.LocalizeGlobal(Message.InvalidProxyAddress, "Invalid proxy address");
            I18n.LocalizeGlobal(Message.InvalidProxyPort, "Invalid proxy port");
            I18n.LocalizeGlobal(Message.InvalidProxyUsername, "Invalid proxy username");
            I18n.LocalizeGlobal(Message.InvalidProxyPassword, "Invalid proxy password");
            I18n.LocalizeGlobal(Message.InvalidProxySetting, "Got invalid proxy setting from the back end");

            I18n.LocalizeGlobal(I18n.Combine("settings", "opacity"), "Window opacity");
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private partial class Title
        {
            /// <summary>
            /// Settings errors.
            /// </summary>
            public static readonly string Settings = I18n.Combine(TitleName, "settings");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        private partial class Message
        {
            /// <summary>
            /// Bad listen value.
            /// </summary>
            public static readonly string CantParseListen = I18n.Combine(MessageName, "cantParseListen");

            /// <summary>
            /// Bad listen value.
            /// </summary>
            public static readonly string InvalidListenValue = I18n.Combine(MessageName, "invalidListenValue");

            /// <summary>
            /// Bad proxy address.
            /// </summary>
            public static readonly string InvalidProxyAddress = I18n.Combine(MessageName, "invalidProxyAddress");

            /// <summary>
            /// Bad proxy port.
            /// </summary>
            public static readonly string InvalidProxyPort = I18n.Combine(MessageName, "invalidProxyPort");

            /// <summary>
            /// Bad proxy username.
            /// </summary>
            public static readonly string InvalidProxyUsername = I18n.Combine(MessageName, "invalidProxyUsername");

            /// <summary>
            /// Bad proxy password.
            /// </summary>
            public static readonly string InvalidProxyPassword = I18n.Combine(MessageName, "invalidProxyPassword");

            /// <summary>
            /// Bad proxy setting.
            /// </summary>
            public static readonly string InvalidProxySetting = I18n.Combine(MessageName, "invalidProxySetting");
        }
    }
}
