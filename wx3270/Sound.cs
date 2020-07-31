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
        /// Prefix for aliases.
        /// </summary>
        public const string Prefix = "wc3270-";

        /// <summary>
        /// Substitution token.
        /// </summary>
        public const string NameToken = "@";

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

        /// <inheritdoc />
        public void Play(string name, int offset = 0)
        {
            if (this.Load(name))
            {
                this.SoundOperation("play", name, $"{NameToken} from {offset}");
            }
        }

        /// <inheritdoc />
        public void Stop(string name)
        {
            if (this.Load(name))
            {
                this.SoundOperation("stop", name);
            }
        }

        /// <inheritdoc />
        public void CleanUp()
        {
            foreach (var pair in this.soundLoaded)
            {
                this.SoundOperation("close", pair.Key);
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
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Wx3270.Resources." + name + ".wav");
            if (stream == null)
            {
                Trace.Line(Trace.Type.Sound, $"Cannot find resource for sound {name}");
                this.soundFailed.Add(name);
                return false;
            }

            // Copy it to a temporary file.
            var tempFile = Path.GetTempFileName();
            StreamWriter streamWriter = File.AppendText(tempFile);
            stream.CopyTo(streamWriter.BaseStream);
            stream.Close();
            streamWriter.Close();

            // Load it.
            if (!this.SoundOperation("open", name, "\"" + tempFile + "\" type waveaudio alias " + NameToken))
            {
                this.soundFailed.Add(name);
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
        /// Perform a sound operation.
        /// </summary>
        /// <param name="verb">Command verb.</param>
        /// <param name="name">Sound name.</param>
        /// <param name="template">Command template.</param>
        /// <returns>True if command succeeded.</returns>
        private bool SoundOperation(string verb, string name, string template = NameToken)
        {
            var command = verb + " " + template.Replace(NameToken, Prefix + name);
            var result = mciSendString(command, null, 0, IntPtr.Zero);
            if (result != 0)
            {
                Trace.Line(Trace.Type.Sound, $"Cannot {command} sound {name}: {result}");
            }

            return result == 0;
        }
    }
}
