// <copyright file="ErrorBox.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Windows.Forms;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Pop-up error boxes.
    /// </summary>
    public static class ErrorBox
    {
        /// <summary>
        /// Message to click yes or no to save text to clipboard.
        /// </summary>
        private static readonly string ClickYesOrNo = I18n.Combine(nameof(ErrorBox), "ClickYesOrNo");

        /// <summary>
        /// The English test of the yes or no message.
        /// </summary>
        private static readonly string YesOrNoText = "Click Yes to copy text to clipboard, No to close window without saving.";

        /// <summary>
        /// Pop up an error box.
        /// </summary>
        /// <param name="text">Body text.</param>
        /// <param name="title">Title text.</param>
        /// <param name="icon">Icon to display.</param>
        public static void Show(string text, string title, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Pop up an error box with a Copy to Clipboard option.
        /// </summary>
        /// <param name="control">Control to run copy thread on.</param>
        /// <param name="text">Body text.</param>
        /// <param name="title">Title text.</param>
        /// <param name="icon">Icon to display.</param>
        public static void ShowCopy(Control control, string text, string title, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            if (control == null)
            {
                Show(text, title, icon);
                return;
            }

            var result = MessageBox.Show(
                text + Environment.NewLine + Environment.NewLine + I18n.Get(ClickYesOrNo, YesOrNoText),
                title,
                MessageBoxButtons.YesNo,
                icon,
                MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes && control != null)
            {
                var dataObject = new DataObject();
                dataObject.SetText(title + ": " + text, TextDataFormat.UnicodeText);
                control.Invoke(new MethodInvoker(() =>
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(dataObject, true);
                }));
            }
        }

        /// <summary>
        /// Back-end completion method with error box pop-up on failure.
        /// </summary>
        /// <param name="title">Pop-up title.</param>
        /// <returns>Completion method.</returns>
        public static BackEndCompletion Completion(string title)
        {
            return (cookie, success, result, misc) =>
            {
                if (!success)
                {
                    Show(result, title);
                }
            };
        }

        /// <summary>
        /// Back-end completion method that does nothing.
        /// </summary>
        /// <returns>Completion method.</returns>
        public static BackEndCompletion Ignore()
        {
            return (cookie, success, result, misc) => { };
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(ClickYesOrNo, YesOrNoText);
        }
    }
}
