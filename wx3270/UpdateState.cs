// <copyright file="UpdateState.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Tracing utility.
    /// </summary>
    public class UpdateState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateState"/> class.
        /// </summary>
        /// <param name="screenImage">Screen image.</param>
        public UpdateState(ScreenImage screenImage)
        {
            this.ScreenImage = screenImage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateState"/> class.
        /// </summary>
        /// <param name="oiaState">OIA state.</param>
        public UpdateState(IOiaState oiaState)
        {
            this.OiaState = oiaState;
        }

        /// <summary>
        /// Gets the screen image.
        /// </summary>
        public ScreenImage ScreenImage { get; }

        /// <summary>
        /// Gets the OIA state.
        /// </summary>
        public IOiaState OiaState { get; }
    }
}
