// <copyright file="WindowTitle.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using Wx3270.Contracts;

    /// <summary>
    /// Window title processor.
    /// </summary>
    public class WindowTitle : BackEndEvent, IWindowTitle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowTitle"/> class.
        /// </summary>
        /// <param name="invoke">Invoke interface for callbacks.</param>
        public WindowTitle()
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.WindowTitle, this.ProcessWindowTitle),
            };
        }

        /// <inheritdoc />
        public string Title { get; private set; }

        /// <summary>
        /// Process a window title indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void ProcessWindowTitle(string name, AttributeDict attrs)
        {
            this.Title = attrs[B3270.Attribute.Text];

            // Signal the event.
            this.Change();
        }
    }
}
