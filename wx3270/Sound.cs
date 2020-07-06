// <copyright file="Sound.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Wx3270.Contracts;

    /// <summary>
    /// Sound playing class.
    /// </summary>
    public class Sound : ISound
    {
        /// <summary>
        /// The name of the console bell sound.
        /// </summary>
        public const string Beep = "Beep";

        /// <summary>
        /// The name of the keyboard click sound.
        /// </summary>
        public const string KeyClick = "keyclick";

        /// <summary>
        /// Mapping of sound names to temporary file names for the WAV files.
        /// </summary>
        private readonly Dictionary<string, string> soundLoaded = new Dictionary<string, string>();

        /// <summary>
        /// Send a command to MCI.
        /// </summary>
        /// <param name="command">Command text</param>
        /// <param name="buffer">Buffer (we do not use this)</param>
        /// <param name="bufferSize">Buffer size (we do not use this)></param>
        /// <param name="hwndCallback">Callback (we do not use this)</param>
        /// <returns>Zero for success</returns>
        [DllImport("winmm.dll")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Imposed by external DLL.")]
        public static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        /// <inheritdoc />
        public void Play(string name, int offset = 0)
        {
            this.Load(name);
            var result = mciSendString("play " + name + " from " + offset, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                throw new ArgumentException("Can't play file", name);
            }
        }

        /// <inheritdoc />
        public void Stop(string name)
        {
            var result = mciSendString("stop " + name, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                throw new ArgumentException("Can't stop file", name);
            }
        }

        /// <inheritdoc />
        public void CleanUp()
        {
            foreach (var pair in this.soundLoaded)
            {
                var result = mciSendString("close " + pair.Key, null, 0, IntPtr.Zero);
                if (result != 0)
                {
                    throw new InvalidOperationException("Can't close " + pair.Key);
                }

                File.Delete(pair.Value);
            }
        }

        /// <summary>
        /// Load a sound file.
        /// </summary>
        /// <param name="name">Name of the resource</param>
        private void Load(string name)
        {
            string tempFile;
            if (this.soundLoaded.TryGetValue(name, out tempFile))
            {
                return;
            }

            // Find the resource.
            var a = System.Reflection.Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("Wx3270.Resources." + name + ".wav");
            if (s == null)
            {
                return;
            }

            // Copy it to a temporary file.
            tempFile = Path.GetTempFileName();
            StreamWriter streamWriter = File.AppendText(tempFile);
            s.CopyTo(streamWriter.BaseStream);
            s.Close();
            streamWriter.Close();

            // Load it.
            var result = mciSendString("open \"" + tempFile + "\" type waveaudio alias " + name, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                throw new ArgumentException("Can't open file", name);
            }

            // Remember it.
            this.soundLoaded[name] = tempFile;

            // Note:
            // To get notifications, look here:
            //  https://stackoverflow.com/questions/3905732/how-do-i-repeat-a-midi-file-in-c
        }
    }
}
