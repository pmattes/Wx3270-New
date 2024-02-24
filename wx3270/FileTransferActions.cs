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
    using I18nBase;

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
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void LocalizeFileTransferActions()
        {
            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global instructions.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(fileTransferTab)), "Tour: File Transfer");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(fileTransferTab)),
@"Use this tab to transfer files to or from the host using the IND$FILE program.");

            // Direction.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(directionBox)), "Transfer direction");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(directionBox)),
@"Use these buttons to select the direction of data transfer.");

            // Local file.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(localFileTextBox)), "Local file");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(localFileTextBox)),
@"Enter the name of the file on your workstation here.

In send mode, this is the file that will be sent to the host.

In receive mode, this is the file that will receive a copy of the file from the host.

Click the Browse button to the right to select a file interactively.");

            // Host file.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(hostFileTextBox)), "Host file");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(hostFileTextBox)),
@"Enter the name of the file on the host here.

In send mode, this is the file that will receive a copy of the file from your workstation.

In receive mode, this is the file on the host that will be sent to your workstation.");

            // Host type.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(hostTypeBox)), "Host type");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(hostTypeBox)),
@"Use these buttons to select the host type. IND$FILE has specific syntax and options for each host type, so this is important to get right.

The default setting for these buttons comes from the connection definition.");

            // Exists options.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(existsBox)), "What if the destination file already exists?");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(existsBox)),
@"Use these buttons to define what happens if the destination file already exists.

'Keep it' means that the transfer will be aborted.

'Replace it' means that the file will be overwritten.

'Append to it' means that the transferred file will be appended to the end of the existing file.");

            // Transfer mode.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(modeBox)), "Transfer mode");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(modeBox)),
@"Use these buttons to define the transfer mode.

A binary transfer leaves the contents of the file unchanged.

An ASCII text transfer requires translation between EBCDIC and your workstation's code page.");

            // Add/remove CRs.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(crCheckBox)), "Add/remove CRs");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(crCheckBox)),
@"Check this box to enable automatically adding CR/LF characters at the ends of lines when receiving a text file from the host, and removing CR/LF characters when sending a text file to the host.");

            // Map character set.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(remapCheckBox)), "Map character set");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(remapCheckBox)),
@"The IND$FILE program has a built-in mapping between EBCDIC and ASCII that may not correspond to the code page on your workstation.

Check this box to enable re-mapping the ASCII text between the IND$FILE code page and a specific Windows code page.

Select the code page in the box below.");

            // Host file record format.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(recfmBox)), "Host file record format");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(recfmBox)),
@"When sending a file to the host, use these buttons to select the record format for the file.

If applicable, you can also define the file's logical record length.");

            // TSO file allocation.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(tsoAllocationBox)), "TSO file allocation");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(tsoAllocationBox)),
@"When sending a file to a TSO host, use these options to control how the file will be allocated.");

            // Block size.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(blockSizeTextBox)), "Block size");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(blockSizeTextBox)),
@"Enter the block size used for the transfer here.");

            // Buffer size.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(bufferSizeTextBox)), "Buffer size");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(bufferSizeTextBox)),
@"For DFT-mode file transfers, enter the buffer size (the maximum size of each chunk of data transferred) here.

Generally larger buffer sizes result in faster transfers, but some misconfigured hosts will crash the session if the buffer size is too large.");

            // Additional options.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(additionalOptionsTextBox)), "Additional options");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(additionalOptionsTextBox)),
@"Enter additional options for the IND$FILE program here.

Use this option with care.");

            // Reset Form button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(fileTransferClearFormButton)), "Reset Form button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(fileTransferClearFormButton)),
@"Click to restore the form to its default state.");

            // Copy Action button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(copyActionButton)), "Copy Action button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(copyActionButton)),
@"Click to capture a copy of the Transfer() action on the Windows clipboard, so you can perform this same file transfer in a script or a keyboard/keypad map.");

            // Transfer button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(transferButton)), "Transfer button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(transferButton)),
@"Click to start the transfer.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Actions), nameof(helpPictureBox4)), "Help");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Actions), nameof(helpPictureBox4)),
@"Click to display context-sensitive help from the x3270 Wiki in your browser, or to start this tour again.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
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
            this.app.SettingChange.Register(this.FtSettingChanged, new[] { B3270.Setting.FtBufferSize });

            // Reset the form.
            this.FileTransferClear();

            // Register the tour.
            var nodes = new[]
            {
                ((Control)this.fileTransferTab, (int?)null, Orientation.Centered),
                (this.directionBox, null, Orientation.UpperRight),
                (this.localFileTextBox, null, Orientation.UpperLeft),
                (this.hostFileTextBox, null, Orientation.UpperLeft),
                (this.hostTypeBox, null, Orientation.UpperLeft),
                (this.existsBox, null, Orientation.UpperLeft),
                (this.modeBox, null, Orientation.UpperRight),
                (this.crCheckBox, null, Orientation.UpperRight),
                (this.remapCheckBox, null, Orientation.UpperRight),
                (this.recfmBox, null, Orientation.UpperLeft),
                (this.tsoAllocationBox, null, Orientation.UpperRight),
                (this.blockSizeTextBox, null, Orientation.LowerLeft),
                (this.bufferSizeTextBox, null, Orientation.LowerLeft),
                (this.additionalOptionsTextBox, null, Orientation.LowerLeft),
                (this.fileTransferClearFormButton, null, Orientation.LowerRight),
                (this.copyActionButton, null, Orientation.LowerRight),
                (this.transferButton, null, Orientation.LowerRight),
                (this.helpPictureBox4, null, Orientation.LowerRight),
            };
            this.RegisterTour(this.fileTransferTab, nodes);
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
            this.additionalOptionsTextBox.Text = string.Empty;
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
        /// Validation for the additional options text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AdditionalOptionsTextBoxValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.additionalOptionsTextBox.Text = this.additionalOptionsTextBox.Text.Trim();
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

            if (!string.IsNullOrEmpty(this.additionalOptionsTextBox.Text))
            {
                parms.Add("{0}", this.additionalOptionsTextBox.Text);
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
