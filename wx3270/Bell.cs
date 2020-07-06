// <copyright file="Bell.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// Terminal bell.
    /// </summary>
    public class Bell
    {
        /// <summary>
        /// The sound object.
        /// </summary>
        private readonly ISound sound;

        /// <summary>
        /// The application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bell"/> class.
        /// </summary>
        /// <param name="app">Application instance</param>
        public Bell(Wx3270App app)
        {
            this.app = app;
            this.app.BackEnd.RegisterStart(B3270.Indication.Bell, this.RingBell);
            this.sound = this.app.Sound;
        }

        /// <summary>
        /// Ring the terminal bell.
        /// </summary>
        /// <param name="name">Element name</param>
        /// <param name="attributes">Attribute dictionary</param>
        private void RingBell(string name, AttributeDict attributes)
        {
            if (this.app.ProfileManager.Current.AudibleBell)
            {
                // Play the sound in the UI thread.
                this.app.Invoke(new MethodInvoker(() => this.sound.Play(Sound.Beep)));
            }
        }
    }
}
