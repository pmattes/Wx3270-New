// <copyright file="IStats.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Emulator stats message handler.
    /// </summary>
    public interface IStats : IBackEndEvent
    {
        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        long BytesReceived { get; }

        /// <summary>
        /// Gets the number of records received.
        /// </summary>
        long RecordsReceived { get; }

        /// <summary>
        /// Gets the number of bytes sent.
        /// </summary>
        long BytesSent { get; }

        /// <summary>
        /// Gets the number of records sent.
        /// </summary>
        long RecordsSent { get; }
    }
}
