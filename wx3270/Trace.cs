// <copyright file="Trace.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using Wx3270.Contracts;

    /// <summary>
    /// Tracing utility.
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// The backlog of traces made before the back end was ready.
        /// </summary>
        private static readonly ConcurrentQueue<string> BackEndBacklog = new ConcurrentQueue<string>();

        /// <summary>
        /// Lock object for the backlog.
        /// </summary>
        private static readonly object BacklogSync = new object();

        /// <summary>
        /// Trace types.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// No events.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Window events.
            /// </summary>
            Window = 0x01,

            /// <summary>
            /// Key events.
            /// </summary>
            Key = 0x02,

            /// <summary>
            /// Profile operations.
            /// </summary>
            Profile = 0x04,

            /// <summary>
            /// Back end operations.
            /// </summary>
            BackEnd = 0x08,

            /// <summary>
            /// Clipboard operations.
            /// </summary>
            Clipboard = 0x10,

            /// <summary>
            /// Sound operations.
            /// </summary>
            Sound = 0x20,

            /// <summary>
            /// Screen draw operations.
            /// </summary>
            Draw = 0x40,

            /// <summary>
            /// All events.
            /// </summary>
            All = Window | Key | Profile | BackEnd | Clipboard | Sound | Draw,
        }

        /// <summary>
        /// Gets or sets the current trace flags.
        /// </summary>
        public static Type Flags { get; set; } = Type.None;

        /// <summary>
        /// Gets or sets the back end.
        /// </summary>
        public static IBackEnd BackEnd { get; set; }

        /// <summary>
        /// Generate a line of  trace output.
        /// </summary>
        /// <param name="type">Trace type.</param>
        /// <param name="format">Format string.</param>
        /// <param name="arg">Format arguments.</param>
        public static void Line(Type type, string format, params object[] arg)
        {
            if ((Flags & type) != 0)
            {
                var expanded = Prefix(type) + " " + string.Format(format, arg);

                TraceOrStore(expanded);
            }
        }

        /// <summary>
        /// Generate a line of trace output.
        /// </summary>
        /// <param name="type">Trace type.</param>
        /// <param name="text">String to display.</param>
        public static void Line(Type type, string text)
        {
            if ((Flags & type) != 0)
            {
                var expanded = Prefix(type) + " " + text;
                TraceOrStore(expanded);
            }
        }

        /// <summary>
        /// Generate a line of  trace output.
        /// </summary>
        /// <param name="type">Trace type.</param>
        public static void Line(Type type)
        {
            if ((Flags & type) != 0)
            {
                var expanded = Prefix(type);
                TraceOrStore(expanded);
            }
        }

        /// <summary>
        /// Generate the trace prefix.
        /// </summary>
        /// <param name="type">Trace type.</param>
        /// <returns>Prefix string.</returns>
        private static string Prefix(Type type)
        {
            var now = DateTime.UtcNow;
            return string.Format("[{0}] {1:D2}:{2:D2}.{3:D3}", type, now.Minute, now.Second, now.Millisecond);
        }

        /// <summary>
        /// Trace an item, or store it in the backlog.
        /// </summary>
        /// <param name="text">Text to trace.</param>
        private static void TraceOrStore(string text)
        {
            try
            {
                Console.WriteLine(text);
            }
            catch (Exception)
            {
            }

            lock (BacklogSync)
            {
                if (BackEnd?.Ready == true)
                {
                    while (BackEndBacklog.TryDequeue(out string item))
                    {
                        BackEnd.Trace(item);
                    }

                    BackEnd.Trace(text);
                }
                else
                {
                    BackEndBacklog.Enqueue(text);
                }
            }
        }
    }
}
