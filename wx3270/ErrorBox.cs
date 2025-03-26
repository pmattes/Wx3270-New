// <copyright file="ErrorBox.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
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
        /// The form map.
        /// </summary>
        private static readonly Dictionary<Form, Form> FormMap = new Dictionary<Form, Form>();

        /// <summary>
        /// Adds a form to the mapping list.
        /// </summary>
        /// <param name="fromForm">Form to map.</param>
        /// <param name="toForm">Form to map it to.</param>
        public static void SetFormMapping(Form fromForm, Form toForm)
        {
            FormMap[fromForm] = toForm;
        }

        /// <summary>
        /// Removes a form from the list.
        /// </summary>
        /// <param name="fromForm">Form to map.</param>
        /// <param name="toForm">Form to map it to.</param>
        public static void RemoveFormMapping(Form form)
        {
            FormMap.Remove(form);
        }

        /// <summary>
        /// Pop up an error box.
        /// </summary>
        /// <param name="text">Body text.</param>
        /// <param name="title">Title text.</param>
        /// <param name="icon">Icon to display.</param>
        /// <param name="onActiveForm">If true, parent to the active Form.</param>
        public static void Show(string text, string title, MessageBoxIcon icon = MessageBoxIcon.Error, bool onActiveForm = true)
        {
            if (onActiveForm)
            {
                Form activeForm = Form.ActiveForm;
                if (activeForm != null && FormMap.TryGetValue(activeForm, out Form mappedForm))
                {
                    activeForm = mappedForm;
                }

                if (activeForm != null)
                {
                    using (new CenterDialog(activeForm))
                    {
                        MessageBox.Show(activeForm, text, title, MessageBoxButtons.OK, icon);
                    }
                }
                else
                {
                    // No active form.
                    MessageBox.Show(text, AttributedTitle(title), MessageBoxButtons.OK, icon);
                }
            }
            else
            {
                MessageBox.Show(text, AttributedTitle(title), MessageBoxButtons.OK, icon);
            }
        }

        /// <summary>
        /// Pop up an error box, given a parent Form.
        /// </summary>
        /// <param name="control">Parent control.</param>
        /// <param name="text">Body text.</param>
        /// <param name="title">Title text.</param>
        /// <param name="icon">Icon to display.</param>
        public static void Show(Control control, string text, string title, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            using (new CenterDialog((Form)control))
            {
                MessageBox.Show(control, text, title, MessageBoxButtons.OK, icon);
            }
        }

        /// <summary>
        /// Pop up an error box with a Copy to Clipboard option.
        /// </summary>
        /// <param name="text">Body text.</param>
        /// <param name="title">Title text.</param>
        /// <param name="icon">Icon to display.</param>
        /// <param name="control">Optional control to own the pop-up.</param>
        /// <returns>Dialog result.</returns>
        public static DialogResult ShowYesNo(string text, string title, MessageBoxIcon icon = MessageBoxIcon.Information, Control control = null)
        {
            return MessageBox.Show(
                control,
                text,
                AttributedTitle(title),
                MessageBoxButtons.YesNo,
                icon,
                MessageBoxDefaultButton.Button2);
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

            var result = ShowYesNo(text + Environment.NewLine + Environment.NewLine + I18n.Get(ClickYesOrNo, YesOrNoText), title, icon);
            if (result == DialogResult.Yes)
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
        /// Pop up an info/error box with a checkbox to cancel further pop-ups.
        /// </summary>
        /// <param name="handle">Parent window hande.</param>
        /// <param name="text">Message box text.</param>
        /// <param name="title">Message box title.</param>
        /// <param name="stopKey">Registry key for stop state.</param>
        /// <param name="icon">Icon to display.</param>
        public static void ShowWithStop(IntPtr handle, string text, string title, string stopKey, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            Form activeForm = Form.ActiveForm;
            if (activeForm != null && FormMap.TryGetValue(activeForm, out Form mappedForm))
            {
                activeForm = mappedForm;
            }

            if (!Wx3270.Wx3270App.StaticPortable)
            {
                NativeMethods.MessageBoxCheckFlags iconFlags = icon switch
                {
                    MessageBoxIcon.Information => NativeMethods.MessageBoxCheckFlags.MB_ICONINFORMATION,
                    MessageBoxIcon.Error => NativeMethods.MessageBoxCheckFlags.MB_ICONEXCLAMATION,
                    MessageBoxIcon.Warning => NativeMethods.MessageBoxCheckFlags.MB_ICONEXCLAMATION,
                    MessageBoxIcon.Question => NativeMethods.MessageBoxCheckFlags.MB_ICONQUESTION,
                    MessageBoxIcon.None => 0U,
                    _ => 0U,
                };

                if (activeForm != null)
                {
                    using (new CenterDialog(activeForm))
                    {
                        NativeMethods.SHMessageBoxCheckW(
                            activeForm.Handle,
                            text,
                            title,
                            NativeMethods.MessageBoxCheckFlags.MB_OK | iconFlags,
                            NativeMethods.MessageBoxReturnValue.IDOK,
                            "wx3270." + stopKey);
                    }
                }
                else
                {
                    // No active form.
                    NativeMethods.SHMessageBoxCheckW(
                        handle,
                        text,
                        AttributedTitle(title),
                        NativeMethods.MessageBoxCheckFlags.MB_OK | iconFlags,
                        NativeMethods.MessageBoxReturnValue.IDOK,
                        "wx3270." + stopKey);
                }
            }
            else
            {
                // Use the fake registry and our home-grown dialog.
                Icon iconObject = icon switch
                {
                    MessageBoxIcon.Information => SystemIcons.Information,
                    MessageBoxIcon.Error => SystemIcons.Error,
                    MessageBoxIcon.Warning => SystemIcons.Warning,
                    MessageBoxIcon.Question => SystemIcons.Question,
                    MessageBoxIcon.None => null,
                    _ => null,
                };

                var r = SimplifiedRegistryFactory.Get(fake: true);
                using (var key = r.CurrentUserCreateSubKey("stop"))
                {
                    if (key.GetValue(stopKey) == null)
                    {
                        using (var stopDialog = new StopDialog(title, text, iconObject))
                        {
                            stopDialog.ShowDialog(activeForm);
                            if (stopDialog.NotAgain)
                            {
                                key.SetValue(stopKey, "true");
                            }
                        }
                    }
                }
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

        /// <summary>
        /// Make sure a title contains 'wx3270'.
        /// </summary>
        /// <param name="title">Proposed title.</param>
        /// <returns>Attributed title.</returns>
        private static string AttributedTitle(string title) => title.Contains("wx3270") ? title : title + " - wx3270";

        /// <summary>
        /// Magic to center the dialog.
        /// </summary>
        /// <remarks>
        /// From https://stackoverflow.com/questions/2576156/winforms-how-can-i-make-messagebox-appear-centered-on-mainform.
        /// </remarks>
        public class CenterDialog : IDisposable
        {
            /// <summary>
            /// The maximum number of times to try the search.
            /// </summary>
            private const int MaxTries = 10;

            /// <summary>
            /// Class name for Windows system dialogs.
            /// </summary>
            private const string DialogClass = "#32770";

            /// <summary>
            /// Owner form.
            /// </summary>
            private readonly Form owner = null;

            /// <summary>
            /// Number of times searched.
            /// </summary>
            private int numTries = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="CenterDialog"/> class.
            /// </summary>
            /// <param name="owner">Owner form.</param>
            public CenterDialog(Form owner)
            {
                if (owner.WindowState == FormWindowState.Minimized)
                {
                    return;
                }

                this.owner = owner;
                owner.BeginInvoke(new MethodInvoker(this.FindDialog));
            }

            /// <summary>
            /// Disposes the object.
            /// </summary>
            public void Dispose()
            {
                this.numTries = -10;
            }

            /// <summary>
            /// Find the dialog.
            /// </summary>
            private void FindDialog()
            {
                // Enumerate windows to find the dialog.
                if (this.numTries >= 0)
                {
                    if (NativeMethods.EnumThreadWindows(
                        NativeMethods.GetCurrentThreadId(),
                        new NativeMethods.EnumThreadWndProc(this.MoveWindow),
                        IntPtr.Zero))
                    {
                        // Failed, try again.
                        if (++this.numTries < MaxTries)
                        {
                            this.owner.BeginInvoke(new MethodInvoker(this.FindDialog));
                        }
                    }
                }
            }

            /// <summary>
            /// Checks if a window is a dialog. If it is, move it.
            /// </summary>
            /// <param name="windowHandle">Window handle.</param>
            /// <param name="lp">Ignored parameter.</param>
            /// <returns>False for succes.</returns>
            private bool MoveWindow(IntPtr windowHandle, IntPtr lp)
            {
                StringBuilder sb = new StringBuilder(260);
                NativeMethods.GetClassName(windowHandle, sb, sb.Capacity);
                if (sb.ToString() != DialogClass)
                {
                    // Not a dialog.
                    return true;
                }

                // Got it. Move it.
                Rectangle parentRectangle = new Rectangle(this.owner.Location, this.owner.Size);
                NativeMethods.GetWindowRect(windowHandle, out NativeMethods.RECT dialogRectangle);
                NativeMethods.MoveWindow(
                    windowHandle,
                    parentRectangle.Left + ((parentRectangle.Width - dialogRectangle.Right + dialogRectangle.Left) / 2),
                    parentRectangle.Top + ((parentRectangle.Height - dialogRectangle.Bottom + dialogRectangle.Top) / 2),
                    dialogRectangle.Right - dialogRectangle.Left,
                    dialogRectangle.Bottom - dialogRectangle.Top,
                    true);
                return false;
            }
        }
    }
}
