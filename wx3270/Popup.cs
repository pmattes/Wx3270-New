// <copyright file="Popup.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Emulator pop-up message handler.
    /// </summary>
    public class Popup : BackEndEvent, IPopup
    {
        /// <summary>
        /// The pop-up title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Popup));

        /// <summary>
        /// Initializes a new instance of the <see cref="Popup"/> class.
        /// </summary>
        public Popup()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.Popup, this.StartPopup),
            };
        }

        /// <summary>
        /// Connection error event.
        /// </summary>
        public event Action<string, bool> ConnectErrorEvent;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.Error, "Error");
            I18n.LocalizeGlobal(Title.Info, "Information");
            I18n.LocalizeGlobal(Title.Printer, "Printer Session Output");
            I18n.LocalizeGlobal(Title.Child, "Script Output");
            I18n.LocalizeGlobal(Title.Result, "Back-End Message");
        }

        /// <summary>
        /// Process a pop-up indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Element attributes.</param>
        private void StartPopup(string name, AttributeDict attributes)
        {
            var text = attributes[B3270.Attribute.Text].Replace("\n", Environment.NewLine);
            var errorOverride = attributes.TryGetValue(B3270.Attribute.Error, out string error) && error.Equals(B3270.Value.True);
            switch (attributes[B3270.Attribute.Type])
            {
                case B3270.PopupType.ConnectionError:
                    if (this.ConnectErrorEvent != null)
                    {
                        this.ConnectErrorEvent(text, attributes.ContainsKey(B3270.Attribute.Retrying) && bool.Parse(attributes[B3270.Attribute.Retrying]));
                    }
                    else
                    {
                        ErrorBox.Show(text, I18n.Get(Title.Error));
                    }

                    break;
                case B3270.PopupType.Error:
                    ErrorBox.Show(text, I18n.Get(Title.Error));
                    break;
                case B3270.PopupType.Info:
                    ErrorBox.Show(text, I18n.Get(Title.Info), MessageBoxIcon.Information);
                    break;
                case B3270.PopupType.Printer:
                    ErrorBox.Show(text, I18n.Get(Title.Printer), errorOverride ? MessageBoxIcon.Error : MessageBoxIcon.Information);
                    break;
                case B3270.PopupType.Child:
                    ErrorBox.Show(text, I18n.Get(Title.Child), errorOverride ? MessageBoxIcon.Error : MessageBoxIcon.Information);
                    break;
                default:
                case B3270.PopupType.Result:
                    ErrorBox.Show(text, I18n.Get(Title.Result), MessageBoxIcon.Information);
                    break;
            }
        }

        /// <summary>
        /// Localized pop-up titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Error pop-up.
            /// </summary>
            public static readonly string Error = I18n.Combine(TitleName, "error");

            /// <summary>
            /// Info pop-up.
            /// </summary>
            public static readonly string Info = I18n.Combine(TitleName, "info");

            /// <summary>
            /// Printer session output.
            /// </summary>
            public static readonly string Printer = I18n.Combine(TitleName, "printer");

            /// <summary>
            ///  Child process output.
            /// </summary>
            public static readonly string Child = I18n.Combine(TitleName, "child");

            /// <summary>
            /// Generic result.
            /// </summary>
            public static readonly string Result = I18n.Combine(TitleName, "result");
        }
    }
}
