// <copyright file="Crossbar.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Crossbar between the back end and the profile.
    /// </summary>
    public class Crossbar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crossbar"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="profileManager">Profile manager.</param>
        public Crossbar(Wx3270App app, IProfileManager profileManager)
        {
            this.ListenCrossbar = new ListenCrossbar(app, profileManager);
            this.MiscCrossbar = new MiscCrossbar(app, profileManager);
            this.OptionsCrossbar = new OptionsCrossbar(app, profileManager);
            this.ProxyCrossbar = new ProxyCrossbar(app, profileManager);
        }

        /// <summary>
        /// Gets the listeners crossbar.
        /// </summary>
        public ListenCrossbar ListenCrossbar { get; }

        /// <summary>
        /// Gets the miscellaneous crossbar.
        /// </summary>
        public MiscCrossbar MiscCrossbar { get; }

        /// <summary>
        /// Gets the options crossbar.
        /// </summary>
        public OptionsCrossbar OptionsCrossbar { get; }

        /// <summary>
        /// Gets the proxy crossbar.
        /// </summary>
        public ProxyCrossbar ProxyCrossbar { get; }
    }
}