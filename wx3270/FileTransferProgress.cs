// <copyright file="FileTransferProgress.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// File transfer progress pop-up.
    /// </summary>
    public partial class FileTransferProgress : Form
    {
        /// <summary>
        /// Normal timer interval (50 milliseconds).
        /// </summary>
        private const int NormalInterval = 50;

        /// <summary>
        /// The number of normal ticks per second.
        /// </summary>
        private const int NormalTicksPerSecond = 1000 / NormalInterval;

        /// <summary>
        /// Rewind timer interval (25 milliseconds).
        /// </summary>
        private const int RewindInterval = 25;

        /// <summary>
        /// Maximum randomized forward ticks (7 seconds).
        /// </summary>
        private const int ForwardMaxTicks = 7 * NormalTicksPerSecond;

        /// <summary>
        /// Stop ticks before back-hitch and rewind (0.25 seconds).
        /// </summary>
        private const int StopTicks = NormalTicksPerSecond / 4;

        /// <summary>
        /// Maximum randomized backward ticks (1 second).
        /// </summary>
        private const int BackwardMaxTicks = 1 * NormalTicksPerSecond;

        /// <summary>
        /// Messages for this module.
        /// </summary>
        private static readonly string ProgressName = I18n.Combine(I18n.MessageName(nameof(FileTransferProgress)), "progress");

        /// <summary>
        /// Messages for this module.
        /// </summary>
        private static readonly string StatusName = I18n.Combine(I18n.MessageName(nameof(FileTransferProgress)), "status");

        /// <summary>
        /// Messages for this module.
        /// </summary>
        private static readonly string BytesName = I18n.Combine(I18n.MessageName(nameof(FileTransferProgress)), "bytes");

        /// <summary>
        /// Connection state object.
        /// </summary>
        private IConnectionState connectionState;

        /// <summary>
        /// Emulator back end.
        /// </summary>
        private IBackEnd backEnd;

        /// <summary>
        /// The main screen.
        /// </summary>
        private MainScreen mainScreen;

        /// <summary>
        /// Transfer state.
        /// </summary>
        private TransferState state;

        /// <summary>
        /// Random number generator for tape operations.
        /// </summary>
        private Random rand = new Random();

        /// <summary>
        /// The state of the animated tape.
        /// </summary>
        private TapeState tapeState;

        /// <summary>
        /// Ticks left in the current state.
        /// </summary>
        private int ticksLeft;

        /// <summary>
        /// The number of normal ticks moved forward.
        /// </summary>
        private int normalTicksForward;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTransferProgress"/> class.
        /// </summary>
        /// <param name="connectionState">Connection state object.</param>
        /// <param name="backEnd">Back end.</param>
        /// <param name="mainScreen">Main screen.</param>
        public FileTransferProgress(IConnectionState connectionState, IBackEnd backEnd, MainScreen mainScreen)
        {
            this.InitializeComponent();

            this.connectionState = connectionState;
            this.backEnd = backEnd;
            this.mainScreen = mainScreen;
            mainScreen.ConnectionStateEvent += this.OnConnectionEvent; // XXX App?

            // Localize before setting the initial state.
            I18n.Localize(this);
            LocalizeForm();

            this.NewState(B3270.FtState.Awaiting, null, null, null);
        }

        /// <summary>
        /// State of the transfer.
        /// </summary>
        private enum TransferState
        {
            /// <summary>
            /// Awaiting start of transfer.
            /// </summary>
            Awaiting,

            /// <summary>
            /// Transfer in progress.
            /// </summary>
            Running,

            /// <summary>
            /// Transfer aborting.
            /// </summary>
            Aborting,

            /// <summary>
            /// Transfer complete.
            /// </summary>
            Complete,
        }

        /// <summary>
        /// Animated tape drive states.
        /// </summary>
        private enum TapeState
        {
            /// <summary>
            /// Base state.
            /// </summary>
            Idle,

            /// <summary>
            /// Moving forward.
            /// </summary>
            Forward,

            /// <summary>
            /// Stopped before a back-hitch.
            /// </summary>
            HitchStop,

            /// <summary>
            /// Moving backwards.
            /// </summary>
            Backward,

            /// <summary>
            /// Stopped before a rewind.
            /// </summary>
            RewindStop,

            /// <summary>
            /// Rewinding at high speed.
            /// </summary>
            Rewind,
        }

        /// <summary>
        /// Form localization.
        /// </summary>
        public static void LocalizeForm()
        {
            I18n.LocalizeGlobal(Progress.Awaiting, "File transfer in progress");
            I18n.LocalizeGlobal(Progress.Complete, "File transfer complete");
            I18n.LocalizeGlobal(Progress.Failed, "File transfer failed");

            I18n.LocalizeGlobal(Status.Awaiting, "Status: Awaiting start of IND$FILE command");
            I18n.LocalizeGlobal(Status.Running, "Status: Running");
            I18n.LocalizeGlobal(Status.Aborting, "Status: Aborting");
            I18n.LocalizeGlobal(Status.Message, "Status: {0}");

            I18n.LocalizeGlobal(Bytes.Running, "{0} bytes transferred");
        }

        /// <summary>
        /// Update with new state.
        /// </summary>
        /// <param name="state">Transfer state.</param>
        /// <param name="bytes">Bytes transferred.</param>
        /// <param name="success">Success or failure indication.</param>
        /// <param name="text">Completion text.</param>
        public void NewState(string state, string bytes, string success, string text)
        {
            switch (state)
            {
                case B3270.FtState.Awaiting:
                    // New transfer, awaiting ack of IND$FILE command.
                    this.state = TransferState.Awaiting;
                    this.progressLabel.Text = I18n.Get(Progress.Awaiting);
                    this.progressLabel.ForeColor = SystemColors.ControlText;
                    this.statusLabel.Text = I18n.Get(Status.Awaiting);
                    this.bytesLabel.Visible = false;
                    this.cancelButton.Enabled = true;
                    this.okButton.Enabled = false;

                    this.animationTimer.Interval = NormalInterval;
                    this.tapeState = TapeState.Forward;
                    this.ticksLeft = this.rand.Next(ForwardMaxTicks);
                    this.normalTicksForward = 0;
                    this.animationTimer.Enabled = true;
                    break;

                case B3270.FtState.Running:
                    // In progress.
                    this.state = TransferState.Running;
                    this.statusLabel.Text = I18n.Get(Status.Running);
                    this.progressLabel.ForeColor = SystemColors.ControlText;
                    this.bytesLabel.Text = string.Format(I18n.Get(Bytes.Running), bytes);
                    this.bytesLabel.Visible = true;
                    this.cancelButton.Enabled = true;
                    this.okButton.Enabled = false;
                    this.animationTimer.Enabled = true;
                    break;

                case B3270.FtState.Aborting:
                    // Abort sent; awaiting host ack.
                    this.state = TransferState.Aborting;
                    this.statusLabel.Text = I18n.Get(Status.Aborting);
                    this.progressLabel.ForeColor = SystemColors.ControlText;
                    this.bytesLabel.Visible = false;
                    this.cancelButton.Enabled = false;
                    this.okButton.Enabled = false;
                    this.animationTimer.Enabled = true;
                    break;

                case B3270.FtState.Complete:
                    // Done.
                    this.state = TransferState.Complete;
                    var successBool = success == B3270.Value.True;
                    this.progressLabel.Text = I18n.Get(successBool ? Progress.Complete : Progress.Failed);
                    this.progressLabel.ForeColor = successBool ? SystemColors.ControlText : Color.Maroon;
                    this.statusLabel.Text = string.Format(I18n.Get(Status.Message), text);
                    this.bytesLabel.Visible = false;
                    this.cancelButton.Enabled = false;
                    this.okButton.Enabled = true;

                    this.tapeState = TapeState.RewindStop;
                    this.ticksLeft = StopTicks;
                    this.animationTimer.Enabled = true;
                    break;
            }
        }

        /// <summary>
        /// The connection changed state.
        /// </summary>
        private void OnConnectionEvent()
        {
            if (this.connectionState.ConnectionState != ConnectionState.Connected3270
                && this.connectionState.ConnectionState == ConnectionState.ConnectedTn3270e)
            {
                this.DialogResult = DialogResult.Cancel;
                this.animationTimer.Enabled = false;
                this.Hide();
            }
        }

        /// <summary>
        /// The cancel button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.backEnd.RunAction(new BackEndAction(B3270.Action.Transfer, B3270.Value.Cancel), BackEnd.Ignore());
            this.state = TransferState.Aborting;
            this.okButton.Enabled = true;
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (this.state >= TransferState.Aborting)
            {
                this.DialogResult = DialogResult.Cancel;
                this.animationTimer.Enabled = false;
                this.Hide();
            }
        }

        /// <summary>
        /// The OK button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.animationTimer.Enabled = false;
            this.Hide();
        }

        /// <summary>
        /// The animation timer expired.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var images = new Image[]
            {
                global::Wx3270.Properties.Resources.TapePair1,
                global::Wx3270.Properties.Resources.TapePair21,
                global::Wx3270.Properties.Resources.TapePair31,
                global::Wx3270.Properties.Resources.TapePair4,
            };
            var enable = true;

            // Get the current tag.
            var tag = int.Parse((string)this.tapePictureBox.Tag);

            switch (this.tapeState)
            {
                case TapeState.Idle:
                    enable = false;
                    break;
                case TapeState.Forward:
                    tag = (tag + 1) % images.Length;
                    this.normalTicksForward++;
                    if (--this.ticksLeft <= 0)
                    {
                        this.tapeState = TapeState.HitchStop;
                        this.ticksLeft = StopTicks;
                    }

                    break;
                case TapeState.HitchStop:
                    if (--this.ticksLeft <= 0)
                    {
                        this.tapeState = TapeState.Backward;
                        this.ticksLeft = this.rand.Next(BackwardMaxTicks);
                    }

                    break;
                case TapeState.Backward:
                    this.normalTicksForward--;
                    tag = (tag + images.Length - 1) % images.Length;
                    if (--this.ticksLeft <= 0)
                    {
                        this.tapeState = TapeState.Forward;
                        this.ticksLeft = this.rand.Next(ForwardMaxTicks);
                    }

                    break;
                case TapeState.RewindStop:
                    if (--this.ticksLeft <= 0)
                    {
                        this.animationTimer.Interval = RewindInterval;
                        this.tapeState = TapeState.Rewind;
                        this.ticksLeft = this.normalTicksForward;
                    }

                    break;
                case TapeState.Rewind:
                    tag = (tag + images.Length - 1) % images.Length;
                    if (--this.ticksLeft <= 0)
                    {
                        this.tapeState = TapeState.Idle;
                    }

                    break;
            }

            // Select the next image and set the next tag.
            this.tapePictureBox.Image = images[tag];
            this.tapePictureBox.Tag = tag.ToString();

            // Keep ticking.
            this.animationTimer.Enabled = enable;
        }

        /// <summary>
        /// The file transfer progress window is being loaded.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FileTransferProgressLoad(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        /// <summary>
        /// Progress message keywords.
        /// </summary>
        private class Progress
        {
            /// <summary>
            /// Awaiting start of transfer.
            /// </summary>
            public static readonly string Awaiting = I18n.Combine(ProgressName, "awaiting");

            /// <summary>
            /// Transfer complete.
            /// </summary>
            public static readonly string Complete = I18n.Combine(ProgressName, "complete");

            /// <summary>
            /// Transfer failed.
            /// </summary>
            public static readonly string Failed = I18n.Combine(ProgressName, "failed");
        }

        /// <summary>
        /// Status message keywords.
        /// </summary>
        private class Status
        {
            /// <summary>
            /// Awaiting start of transfer.
            /// </summary>
            public static readonly string Awaiting = I18n.Combine(StatusName, "awaiting");

            /// <summary>
            /// Transfer running.
            /// </summary>
            public static readonly string Running = I18n.Combine(StatusName, "running");

            /// <summary>
            /// Transfer aborting.
            /// </summary>
            public static readonly string Aborting = I18n.Combine(StatusName, "aborting");

            /// <summary>
            /// Final status message.
            /// </summary>
            public static readonly string Message = I18n.Combine(StatusName, "message");
        }

        /// <summary>
        /// Bytes message keywords.
        /// </summary>
        private class Bytes
        {
            /// <summary>
            /// Transfer running.
            /// </summary>
            public static readonly string Running = I18n.Combine(BytesName, "running");
        }
    }
}
