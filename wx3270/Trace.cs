// <copyright file="Trace.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// Tracing utility.
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// Trace types.
        /// </summary>
        public enum Type
        {
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
        public static Type Flags { get; set; } = Type.All;

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
                try
                {
                    Console.WriteLine(Prefix(type) + " " + format, arg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Prefix(type) + $" cannot display, exception {e.Message}");
                }
            }
        }

        /// <summary>
        /// Generate a line of  trace output.
        /// </summary>
        /// <param name="type">Trace type.</param>
        /// <param name="text">String to display.</param>
        public static void Line(Type type, string text)
        {
            if ((Flags & type) != 0)
            {
                try
                {
                    Console.WriteLine(Prefix(type) + " " + text);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Prefix(type) + $" cannot display, exception {e.Message}");
                }
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
                try
                {
                    Console.WriteLine(Prefix(type));
                }
                catch (Exception e)
                {
                    Console.WriteLine(Prefix(type) + $" cannot display, exception {e.Message}");
                }
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
            return string.Format("[{0}] {1:D2}:{2:D2}.{3:D4}", type, now.Minute, now.Second, now.Millisecond);
        }
    }
}
