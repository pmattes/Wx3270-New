// <copyright file="ErrorBox.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// Pop-up error boxes.
    /// </summary>
    public static class ErrorBox
    {
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
        /// Back-end completion method with error box pop-up on failure.
        /// </summary>
        /// <param name="title">Pop-up title.</param>
        /// <returns>Completion method.</returns>
        public static BackEndCompletion Completion(string title)
        {
            return (cookie, success, result) =>
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
            return (cookie, success, result) => { };
        }
    }
}
