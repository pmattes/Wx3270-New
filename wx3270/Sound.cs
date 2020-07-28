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
    using System.Windows.Forms;

    using I18nBase;
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
        /// Title name, for localization.
        /// </summary>
        private static readonly string TitleName = I18n.TitleName(nameof(Sound));

        /// <summary>
        /// Message name, for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Sound));

        /// <summary>
        /// Mapping of sound names to temporary file names for the WAV files.
        /// </summary>
        private readonly Dictionary<string, string> soundLoaded = new Dictionary<string, string>();

        /// <summary>
        /// Set of failed sound files.
        /// </summary>
        private readonly HashSet<string> soundFailed = new HashSet<string>();

        /// <summary>
        /// Send a command to MCI.
        /// </summary>
        /// <param name="command">Command text.</param>
        /// <param name="buffer">Buffer (we do not use this).</param>
        /// <param name="bufferSize">Buffer size (we do not use this)>.</param>
        /// <param name="hwndCallback">Callback (we do not use this).</param>
        /// <returns>Zero for success.</returns>
        [DllImport("winmm.dll")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Imposed by external DLL.")]
        public static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.SoundError, "Sound Error");

            I18n.LocalizeGlobal(Message.CannotLoad, "Cannot load sound '{0}'");
            I18n.LocalizeGlobal(Message.CannotPlay, "Cannot play sound '{0}'");
            I18n.LocalizeGlobal(Message.CannotStop, "Cannot stop sound '{0}'");
            I18n.LocalizeGlobal(Message.CannotClose, "Cannot close sound '{0}'");
        }

        /// <inheritdoc />
        public void Play(string name, int offset = 0)
        {
            if (!this.Load(name))
            {
                return;
            }

            var result = mciSendString("play " + name + " from " + offset, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                ErrorBox.Show(string.Format(I18n.Get(Message.CannotPlay), name), I18n.Get(Title.SoundError), MessageBoxIcon.Error);
            }
        }

        /// <inheritdoc />
        public void Stop(string name)
        {
            var result = mciSendString("stop " + name, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                ErrorBox.Show(string.Format(I18n.Get(Message.CannotStop), name), I18n.Get(Title.SoundError), MessageBoxIcon.Error);
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
                    throw new InvalidOperationException(string.Format(I18n.Get(Message.CannotClose), pair.Key));
                }

                File.Delete(pair.Value);
            }

            this.soundLoaded.Clear();
        }

        /// <summary>
        /// Load a sound file.
        /// </summary>
        /// <param name="name">Name of the resource.</param>
        /// <returns>True if sound loaded successfully.</returns>
        private bool Load(string name)
        {
            if (this.soundFailed.Contains(name))
            {
                return false;
            }

            if (this.soundLoaded.ContainsKey(name))
            {
                return true;
            }

            // Find the resource.
            var a = System.Reflection.Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("Wx3270.Resources." + name + ".wav");
            if (s == null)
            {
                this.soundFailed.Add(name);
                return false;
            }

            // Copy it to a temporary file.
            var tempFile = Path.GetTempFileName();
            StreamWriter streamWriter = File.AppendText(tempFile);
            s.CopyTo(streamWriter.BaseStream);
            s.Close();
            streamWriter.Close();

            // Load it.
            var result = mciSendString("open \"" + tempFile + "\" type waveaudio alias " + name, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                this.soundFailed.Add(name);
                ErrorBox.Show(string.Format(I18n.Get(Message.CannotLoad), name), I18n.Get(Title.SoundError), MessageBoxIcon.Error);
                return false;
            }

            // Remember it.
            this.soundLoaded[name] = tempFile;

            // Note:
            // To get notifications, look here:
            //  https://stackoverflow.com/questions/3905732/how-do-i-repeat-a-midi-file-in-c
            return true;
        }

        /// <summary>
        /// Miscellaneous messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Cannot load message.
            /// </summary>
            public static readonly string CannotLoad = I18n.Combine(MessageName, "cannotLoad");

            /// <summary>
            /// Cannot play message.
            /// </summary>
            public static readonly string CannotPlay = I18n.Combine(MessageName, "cannotPlay");

            /// <summary>
            /// Cannot stop message.
            /// </summary>
            public static readonly string CannotStop = I18n.Combine(MessageName, "cannotStop");

            /// <summary>
            /// Cannot close message.
            /// </summary>
            public static readonly string CannotClose = I18n.Combine(MessageName, "cannotClose");
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Profile edit.
            /// </summary>
            public static readonly string SoundError = I18n.Combine(TitleName, "soundError");
        }
    }
}
