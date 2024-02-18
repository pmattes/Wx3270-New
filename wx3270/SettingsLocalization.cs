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
            I18n.LocalizeGlobal(Message.LockedWhileConnected, "locked while connected");

            I18n.LocalizeGlobal(SettingPath("opacity"), "window opacity");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Common buttons.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), 99), "Common buttons");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), 99),
@"Next are some buttons that are common to all Settings window tabs.");

            // Save As button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(readOnlyFlowLayoutPanel)), "Common buttons: Save a copy");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(readOnlyFlowLayoutPanel)),
@"If this message is visible, wx3270 is running in read-only mode. Changes to settings will not be saved.

To save the current settings in a new profile, click on 'Save a copy'.");

            // Defaults button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(setToDefaultsButton)), "Common buttons: Set to Defaults");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(setToDefaultsButton)),
@"Click to revert all settings to their factory defaults.");

            // Undo/redo buttons.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(undoButton)), "Common buttons: Undo and Redo");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(undoButton)),
@"Click the '↶' (Undo) button to undo the last operation.

Click the '↷' (Redo) button to redo the last operation that was rolled back with the Undo button.

The button labels include a count of how many Undo and Redo operations are saved.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Settings), nameof(helpPictureBox)), "Common buttons: Help");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Settings), nameof(helpPictureBox)),
@"Click to display context-sensitive help from the x3270 Wiki in your browser, or to start this tour again.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        public partial class Title
        {
            /// <summary>
            /// Settings errors.
            /// </summary>
            public static readonly string Settings = I18n.Combine(TitleName, "settings");
        }

        /// <summary>
        /// Message box messages.
        /// </summary>
        public partial class Message
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

            /// <summary>
            /// Setting is locked while connected.
            /// </summary>
            public static readonly string LockedWhileConnected = I18n.Combine(MessageName, "lockedWhileConnected");
        }
    }
}
