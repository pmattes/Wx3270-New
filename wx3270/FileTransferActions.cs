// <copyright file="FileTransferActions.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// The file transfer tab of the actions dialog.
    /// </summary>
    public partial class Actions : Form
    {
        /// <summary>
        /// The file transfer direction.
        /// </summary>
        private RadioEnum<Direction> fileTransferDirection;

        /// <summary>
        /// The file transfer host type radio buttons.
        /// </summary>
        private RadioEnum<HostType> fileTransferHostType;

        /// <summary>
        /// The file transfer destination exits action.
        /// </summary>
        private RadioEnum<ExistsAction> fileTransferExistsAction;

        /// <summary>
        /// The file transfer mode (binary or text).
        /// </summary>
        private RadioEnum<TransferMode> fileTransferMode;

        /// <summary>
        /// The file transfer record format.
        /// </summary>
        private RadioEnum<RecordFormat> fileTransferRecordFormat;

        /// <summary>
        /// The file allocation TSO allocation type.
        /// </summary>
        private RadioEnum<TsoAllocationType> fileTransferTsoAllocationType;

        /// <summary>
        /// File transfer progress dialog.
        /// </summary>
        private FileTransferProgress transferProgress;

        /// <summary>
        /// File transfer direction.
        /// </summary>
        private enum Direction
        {
            /// <summary>
            /// Send file to host.
            /// </summary>
            Send,

            /// <summary>
            /// Receive file from host.
            /// </summary>
            Receive,
        }

        /// <summary>
        /// File transfer destination file exists actions.
        /// </summary>
        private enum ExistsAction
        {
            /// <summary>
            /// Keep the existing file.
            /// </summary>
            Keep,

            /// <summary>
            /// Replace the existing file.
            /// </summary>
            Replace,

            /// <summary>
            /// Append to the existing file.
            /// </summary>
            Append,
        }

        /// <summary>
        /// File transfer mode.
        /// </summary>
        private enum TransferMode
        {
            /// <summary>
            /// Binary file.
            /// </summary>
            Binary,

            /// <summary>
            /// Text file.
            /// </summary>
            Ascii,
        }

        /// <summary>
        /// File transfer record format.
        /// </summary>
        private enum RecordFormat
        {
            /// <summary>
            /// Default format.
            /// </summary>
            Default,

            /// <summary>
            /// Fixed-length records.
            /// </summary>
            Fixed,

            /// <summary>
            /// Variable-length records.
            /// </summary>
            Variable,

            /// <summary>
            /// Undefined records.
            /// </summary>
            Undefined,
        }

        /// <summary>
        /// TSO file allocation type.
        /// </summary>
        private enum TsoAllocationType
        {
            /// <summary>
            /// Default units.
            /// </summary>
            Default,

            /// <summary>
            /// Allocate by tracks.
            /// </summary>
            Tracks,

            /// <summary>
            /// Allocate by cylinders.
            /// </summary>
            Cylinders,

            /// <summary>
            /// Allocate by <code>avblock</code>.
            /// </summary>
            Avblock,
        }

        /// <summary>
        /// Pop up the file transfer dialog.
        /// </summary>
        public void FileTransfer()
        {
            this.actionsTabs.SelectedTab = this.fileTransferTab;
            this.Show();
        }

        /// <summary>
        /// Localize the file transfer tab.
        /// </summary>
        private static void FileTransferLocalize()
        {
            I18n.LocalizeGlobal(FileTransferTitle.CopyFailure, "Copy Failure");
            I18n.LocalizeGlobal(FileTransferTitle.FileTransfer, "File Transfer");
            I18n.LocalizeGlobal(FileTransferTitle.LocalFile, "Local File");
            I18n.LocalizeGlobal(FileTransferTitle.HostFile, "Host File");
            I18n.LocalizeGlobal(FileTransferTitle.FileToSend, "File to send");
            I18n.LocalizeGlobal(FileTransferTitle.FileToSave, "File to save");

            I18n.LocalizeGlobal(FileTransferMessage.InvalidCharacters, "Name contains invalid character(s)");
            I18n.LocalizeGlobal(FileTransferMessage.InvalidNumeric, "Invalid numeric value");
        }

        /// <summary>
        /// The default code page.
        /// </summary>
        /// <returns>Default code page.</returns>
        private static string DefaultCodePage()
        {
            return Encoding.Default.CodePage.ToString();
        }

        /// <summary>
        /// Initialize the file transfer tab.
        /// </summary>
        private void FileTransferTabInit()
        {
            // Subscribe to connection events.
            this.mainScreen.ConnectionStateEvent += () => this.EnableDisable(connectionChange: true);

            // Subscribe to file transfer events.
            this.transferProgress = new FileTransferProgress(this.app, this.app.BackEnd, this.mainScreen);
            this.app.BackEnd.RegisterStart(B3270.Indication.Ft, (name, attrs) => this.mainScreen.Invoke(new MethodInvoker(() => this.StartFt(name, attrs))));

            // Set up radio button enums.
            this.fileTransferDirection = new RadioEnum<Direction>(this.directionBox);
            this.fileTransferDirection.Changed += (sender, args) => this.EnableDisable();
            this.fileTransferHostType = new RadioEnum<HostType>(this.hostTypeBox);
            this.fileTransferHostType.Value = HostType.Tso;
            this.fileTransferHostType.Changed += (sender, args) => this.EnableDisable();
            this.fileTransferExistsAction = new RadioEnum<ExistsAction>(this.existsBox);
            this.fileTransferExistsAction.Changed += (sender, args) => this.EnableDisable();
            this.fileTransferMode = new RadioEnum<TransferMode>(this.modeBox);
            this.fileTransferMode.Changed += (sender, args) => this.EnableDisable();
            this.fileTransferRecordFormat = new RadioEnum<RecordFormat>(this.recfmBox);
            this.fileTransferRecordFormat.Changed += (sender, args) => this.EnableDisable();
            this.fileTransferTsoAllocationType = new RadioEnum<TsoAllocationType>(this.tsoAllocationBox);
            this.fileTransferTsoAllocationType.Changed += (sender, args) => this.EnableDisable();
            this.avblockTextBox.Validated += (sender, args) => this.EnableDisable();

            // Subscribe to setting changes.
            this.app.SettingChange.Register(
                (settingName, settingDictionary) => this.Invoke(new MethodInvoker(() => this.FtSettingChanged(settingName, settingDictionary))),
                new[] { B3270.Setting.FtBufferSize });

            // Reset the form.
            this.FileTransferClear();
        }

        /// <summary>
        /// A file transfer setting changed.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        /// <param name="settingDictionary">Settings dictionary.</param>
        private void FtSettingChanged(string settingName, SettingsDictionary settingDictionary)
        {
            if (settingName == B3270.Setting.FtBufferSize && settingDictionary.TryGetValue(settingName, out int value))
            {
                this.bufferSizeTextBox.Text = value.ToString();
            }
        }

        /// <summary>
        /// Clear the file transfer tab.
        /// </summary>
        private void FileTransferClear()
        {
            this.localFileTextBox.Text = string.Empty;
            this.hostFileTextBox.Text = string.Empty;
            this.fileTransferDirection.Value = Direction.Receive;
            this.fileTransferHostType.Value = this.mainScreen.ConnectHostType == HostType.Unspecified ? HostType.Tso : this.mainScreen.ConnectHostType;
            this.fileTransferExistsAction.Value = ExistsAction.Keep;
            this.fileTransferMode.Value = TransferMode.Ascii;
            this.crCheckBox.Checked = true;
            this.remapCheckBox.Checked = true;
            this.windowsCodePageTextBox.Text = DefaultCodePage();
            this.fileTransferRecordFormat.Value = RecordFormat.Default;
            this.lreclTextBox.Text = string.Empty;
            this.primarySpaceTextBox.Text = string.Empty;
            this.secondarySpaceTextBox.Text = string.Empty;
            this.avblockTextBox.Text = string.Empty;
            this.fileTransferTsoAllocationType.Value = TsoAllocationType.Default;
            this.blockSizeTextBox.Text = string.Empty;
            this.bufferSizeTextBox.Text = string.Empty;
        }

        /// <summary>
        /// The file transfer local file name was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferLocalFileTextBoxClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.localFileTextBox.Text))
            {
                this.transferLocalFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                this.transferLocalFileDialog.InitialDirectory = Path.GetDirectoryName(this.localFileTextBox.Text);
            }

            this.transferLocalFileDialog.FileName = string.Empty;
            this.transferLocalFileDialog.Title = I18n.Get(this.directionSendButton.Checked ? FileTransferTitle.FileToSend : FileTransferTitle.FileToSave);
            this.transferLocalFileDialog.CheckFileExists = this.directionSendButton.Checked;
            var result = this.transferLocalFileDialog.ShowDialog();
            switch (result)
            {
                default:
                case DialogResult.Cancel:
                    return;
                case DialogResult.OK:
                    break;
            }

            this.localFileTextBox.Text = this.transferLocalFileDialog.FileName;
        }

        /// <summary>
        /// The file transfer clear form button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferClearFormButtonClick(object sender, EventArgs e)
        {
            this.FileTransferClear();
        }

        /// <summary>
        /// Enable and disable parts of the form as values change.
        /// </summary>
        /// <param name="connectionChange">True if this is a connection state change.</param>
        private void EnableDisable(bool connectionChange = false)
        {
            this.asciiBox.Enabled = this.modeAsciiButton.Checked;

            this.windowsCodePageLabel.Enabled = this.windowsCodePageTextBox.Enabled = this.remapCheckBox.Checked;

            this.recfmBox.Enabled = this.directionSendButton.Checked
                && !this.hostCicsButton.Checked
                && !this.existsAppendButton.Checked;
            this.tsoAllocationBox.Enabled = this.directionSendButton.Checked
                && this.hostTsoButton.Checked
                && !this.existsAppendButton.Checked;
            this.blockSizeLabel.Enabled = this.blockSizeTextBox.Enabled = this.hostTsoButton.Checked;

            this.lreclLabel.Enabled = this.lreclTextBox.Enabled = !this.recfmDefaultButton.Checked;

            this.avblockLabel.Enabled = this.avblockTextBox.Enabled = this.allocAvblockButton.Checked;

            this.existsKeepButton.Enabled = this.fileTransferDirection.Value == Direction.Receive;
            if (this.fileTransferDirection.Value == Direction.Send && this.fileTransferExistsAction.Value == ExistsAction.Keep)
            {
                this.fileTransferExistsAction.Value = ExistsAction.Replace;
            }

            // The transfer button is enabled only if:
            // - The two files have been specified.
            // - Connected in 3270 mode.
            // - Avblock is specified if needed.
            // That last bit may be a bit opaque.
            var copyValid =
                !string.IsNullOrEmpty(this.localFileTextBox.Text)
                && !string.IsNullOrEmpty(this.hostFileTextBox.Text)
                && !(this.tsoAllocationBox.Enabled
                     && this.allocAvblockButton.Checked
                     && string.IsNullOrEmpty(this.avblockTextBox.Text));
            var transferValid = copyValid
                && (this.app.ConnectionState == ConnectionState.Connected3270
                    || this.app.ConnectionState == ConnectionState.ConnectedTn3270e);
            this.copyActionButton.Enabled = copyValid;
            this.transferButton.Enabled = transferValid;

            if (connectionChange && this.app.ConnectionState != ConnectionState.NotConnected && this.mainScreen.ConnectHostType != HostType.Unspecified)
            {
                this.fileTransferHostType.Value = this.mainScreen.ConnectHostType;
            }
        }

        /// <summary>
        /// Validation for file transfer numeric fields.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferNumericValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text.Trim(new[] { ' ' });
            textBox.Text = text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!uint.TryParse(text, out uint u) || u == 0)
            {
                ErrorBox.Show(I18n.Get(FileTransferMessage.InvalidNumeric), I18n.Get(FileTransferTitle.FileTransfer));
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Validation for the local file text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void LocalFileTextBoxValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var text = this.localFileTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                this.localFileTextBox.Text = text;
                return;
            }

            if (Path.GetInvalidPathChars().Any(c => text.Contains(c)))
            {
                ErrorBox.Show(I18n.Get(FileTransferMessage.InvalidCharacters), I18n.Get(FileTransferTitle.LocalFile));
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Check a host file name for validity.
        /// </summary>
        /// <param name="text">Host file name.</param>
        /// <returns>True if the name is invalid.</returns>
        private bool InvalidHostFileName(string text)
        {
            return text.Any(c => c < ' ' || c >= 0x7f || c == '"') ||
                ((this.fileTransferHostType.Value == HostType.Tso || this.fileTransferHostType.Value == HostType.Cics) &&
                 text.Contains(" "));
        }

        /// <summary>
        /// Validation for the local file text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HostFileTextBoxValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.hostFileTextBox.Text = this.hostFileTextBox.Text.Trim();
        }

        /// <summary>
        /// Construct the back-end action to perform the file transfer.
        /// </summary>
        /// <returns>File transfer action.</returns>
        private BackEndAction TransferAction()
        {
            // Construct the command.
            var parms = new ParmsList();
            parms.Add("Direction={0}", this.fileTransferDirection);
            parms.Add("HostFile");
            parms.Add(this.hostFileTextBox.Text);
            parms.Add("LocalFile");
            parms.Add(this.localFileTextBox.Text);
            parms.Add("Host={0}", this.fileTransferHostType);
            parms.Add("Mode={0}", this.fileTransferMode);
            if (this.fileTransferMode.Value == TransferMode.Ascii)
            {
                string cr;
                if (this.crCheckBox.Checked)
                {
                    cr = this.fileTransferDirection.Value == Direction.Send ? "remove" : "add";
                }
                else
                {
                    cr = "keep";
                }

                parms.Add("Cr={0}", cr);
                parms.Add("Remap={0}", this.remapCheckBox.Checked ? "yes" : "no");
                parms.Add(
                    "WindowsCodePage={0}",
                    string.IsNullOrEmpty(this.windowsCodePageTextBox.Text) ? DefaultCodePage() : this.windowsCodePageTextBox.Text);
            }

            parms.Add("Exist={0}", this.fileTransferExistsAction);
            if (this.recfmBox.Enabled)
            {
                parms.Add("Recfm={0}", this.fileTransferRecordFormat);
                if (this.lreclTextBox.Enabled && !string.IsNullOrEmpty(this.lreclTextBox.Text))
                {
                    parms.Add("Lrecl={0}", this.lreclTextBox.Text);
                }
            }

            if (this.blockSizeTextBox.Enabled && !string.IsNullOrEmpty(this.blockSizeTextBox.Text))
            {
                parms.Add("Blksize={0}", this.blockSizeTextBox.Text);
            }

            if (this.tsoAllocationBox.Enabled && !string.IsNullOrEmpty(this.primarySpaceTextBox.Text))
            {
                parms.Add("Allocation={0}", this.fileTransferTsoAllocationType);
                parms.Add("PrimarySpace={0}", this.primarySpaceTextBox.Text);
                if (!string.IsNullOrEmpty(this.secondarySpaceTextBox.Text))
                {
                    parms.Add("SecondarySpace={0}", this.secondarySpaceTextBox.Text);
                }

                if (!string.IsNullOrEmpty(this.avblockTextBox.Text))
                {
                    parms.Add("Avblock={0}", this.avblockTextBox.Text);
                }
            }

            if (!string.IsNullOrEmpty(this.bufferSizeTextBox.Text))
            {
                parms.Add("BufferSize={0}", this.bufferSizeTextBox.Text);
            }

            return new BackEndAction("Transfer", parms.Parms);
        }

        /// <summary>
        /// The Copy Action button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CopyActionButtonClick(object sender, EventArgs e)
        {
            // Construct the action.
            var action = this.TransferAction();

            // Copy to the clipboard.
            var dataObject = new DataObject();
            dataObject.SetText(action.Encoding, TextDataFormat.UnicodeText);
            dataObject.SetText(action.Encoding, TextDataFormat.Text);
            try
            {
                this.app.Invoke(new MethodInvoker(() =>
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(dataObject, true);
                }));
            }
            catch (Exception ex)
            {
                ErrorBox.Show(ex.Message, I18n.Get(FileTransferTitle.CopyFailure));
            }
        }

        /// <summary>
        /// The transfer button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TransferButtonClick(object sender, EventArgs e)
        {
            // Validate the host file name.
            if (this.InvalidHostFileName(this.hostFileTextBox.Text))
            {
                ErrorBox.Show(I18n.Get(FileTransferMessage.InvalidCharacters), I18n.Get(FileTransferTitle.HostFile));
                return;
            }

            // Run the command.
            this.BackEnd.RunAction(
                this.TransferAction(),
                (cookie, success, result, misc) =>
                {
                    if (!success)
                    {
                        this.transferProgress.Hide();
                        ErrorBox.Show(result, I18n.Get(FileTransferTitle.FileTransfer));
                    }
                });
            this.SafeHide();
        }

        /// <summary>
        /// Handle a file transfer status indication.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Attribute dictionary.</param>
        private void StartFt(string name, AttributeDict attributes)
        {
            if (!attributes.TryGetValue(B3270.Attribute.Bytes, out string bytes))
            {
                bytes = null;
            }

            if (!attributes.TryGetValue(B3270.Attribute.Success, out string success))
            {
                success = null;
            }

            if (!attributes.TryGetValue(B3270.Attribute.Text, out string text))
            {
                text = null;
            }

            // Cancel any previous popdown.
            this.ftPopdownTimer.Stop();

            if (!this.transferProgress.Visible)
            {
                // Pop up the progress dialog.
                this.transferProgress.Show(this.mainScreen);
                this.transferProgress.Location = MainScreen.CenteredOn(this.mainScreen, this.transferProgress);
            }

            var state = attributes[B3270.Attribute.State];
            var cause = attributes[B3270.Attribute.Cause];
            this.transferProgress.NewState(state, bytes, success, text);
            if (state == B3270.FtState.Complete && cause != B3270.Cause.Ui)
            {
                // Hide the transfer progress dialog in 5 seconds.
                // Ideally, this should only happen if the transfer was not initiated by the UI.
                this.ftPopdownTimer.Tick += (tickSender, tickArgs) =>
                {
                    this.ftPopdownTimer.Stop();
                    this.transferProgress.Hide();
                };
                this.ftPopdownTimer.Start();
            }
        }

        /// <summary>
        /// Parameter list class.
        /// </summary>
        private class ParmsList
        {
            /// <summary>
            /// The parameter list.
            /// </summary>
            private readonly List<string> parms = new List<string>();

            /// <summary>
            /// Gets the completed text.
            /// </summary>
            public IEnumerable<string> Parms
            {
                get
                {
                    return this.parms;
                }
            }

            /// <summary>
            /// Add a formatted string.
            /// </summary>
            /// <param name="format">Format string.</param>
            /// <param name="args">Format arguments.</param>
            public void Add(string format, params object[] args)
            {
                this.parms.Add(string.Format(format, args));
            }
        }

        /// <summary>
        /// Localized message box titles.
        /// </summary>
        private class FileTransferTitle
        {
            /// <summary>
            /// Clipboard copy failure.
            /// </summary>
            public static readonly string CopyFailure = I18n.Combine(TitleName, "copyFailure");

            /// <summary>
            /// File transfer failure.
            /// </summary>
            public static readonly string FileTransfer = I18n.Combine(TitleName, "fileTransfer");

            /// <summary>
            /// Local file name error.
            /// </summary>
            public static readonly string LocalFile = I18n.Combine(TitleName, "localFile");

            /// <summary>
            /// Host file name error.
            /// </summary>
            public static readonly string HostFile = I18n.Combine(TitleName, "hostFile");

            /// <summary>
            /// File to send.
            /// </summary>
            public static readonly string FileToSend = I18n.Combine(TitleName, "fileToSend");

            /// <summary>
            /// File to save.
            /// </summary>
            public static readonly string FileToSave = I18n.Combine(TitleName, "fileToSave");
        }

        /// <summary>
        /// Localized message box text.
        /// </summary>
        private class FileTransferMessage
        {
            /// <summary>
            /// Invalid characters.
            /// </summary>
            public static readonly string InvalidCharacters = I18n.Combine(MessageName, "invalidCharacters");

            /// <summary>
            /// Invalid numeric value.
            /// </summary>
            public static readonly string InvalidNumeric = I18n.Combine(MessageName, "invalidNumeric");
        }
    }
}
