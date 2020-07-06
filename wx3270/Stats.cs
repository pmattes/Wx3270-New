// <copyright file="Stats.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Emulator stats message handler.
    /// </summary>
    public class Stats : BackEndEvent, IStats
    {
        /// <summary>
        /// Synchronization object.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Backing field for <see cref="BytesReceived"/>.
        /// </summary>
        private long bytesReceived;

        /// <summary>
        /// Backing field for <see cref="RecordsReceived"/>.
        /// </summary>
        private long recordsReceived;

        /// <summary>
        /// Backing field for <see cref="BytesSent"/>.
        /// </summary>
        private long bytesSent;

        /// <summary>
        /// Backing field for <see cref="RecordsSent"/> .
        /// </summary>
        private long recordsSent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Stats"/> class.
        /// </summary>
        public Stats()
        {
            this.Def = new[] { new BackEndEventDef("stats", this.StartStats) };
        }

        /// <inheritdoc />
        public long BytesReceived
        {
            get
            {
                lock (this.sync)
                {
                    return this.bytesReceived;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.bytesReceived = value;
                }
            }
        }

        /// <inheritdoc />
        public long RecordsReceived
        {
            get
            {
                lock (this.sync)
                {
                    return this.recordsReceived;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.recordsReceived = value;
                }
            }
        }

        /// <inheritdoc />
        public long BytesSent
        {
            get
            {
                lock (this.sync)
                {
                    return this.bytesSent;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.bytesSent = value;
                }
            }
        }

        /// <inheritdoc />
        public long RecordsSent
        {
            get
            {
                lock (this.sync)
                {
                    return this.recordsSent;
                }
            }

            private set
            {
                lock (this.sync)
                {
                    this.recordsSent = value;
                }
            }
        }

        /// <summary>
        /// Process a stats indication.
        /// </summary>
        /// <param name="name">Element name</param>
        /// <param name="attributes">Element attributes</param>
        private void StartStats(string name, AttributeDict attributes)
        {
            this.BytesReceived = long.Parse(attributes[B3270.Attribute.BytesReceived]);
            this.RecordsReceived = long.Parse(attributes[B3270.Attribute.RecordsReceived]);
            this.BytesSent = long.Parse(attributes[B3270.Attribute.BytesSent]);
            this.RecordsSent = long.Parse(attributes[B3270.Attribute.RecordsSent]);

            this.Change();
        }
    }
}
