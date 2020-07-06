// <copyright file="ISound.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    /// <summary>
    /// Sound playing class.
    /// </summary>
    public interface ISound
    {
        /// <summary>
        /// Play a sound.
        /// </summary>
        /// <param name="name">Name of sound</param>
        /// <param name="offset">Offset into sound, in milliseconds</param>
        void Play(string name, int offset = 0);

        /// <summary>
        /// Stop playing a sound.
        /// </summary>
        /// <param name="name">Name of sound</param>
        void Stop(string name);

        /// <summary>
        /// Clean up the temporary sound files. Called at exit.
        /// </summary>
        void CleanUp();
    }
}
