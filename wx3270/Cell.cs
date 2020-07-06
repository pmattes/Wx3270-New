// <copyright file="Cell.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// Screen cell.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        public Cell()
        {
            this.Text = ' ';
            this.HostForeground = HostColor.NeutralWhite;
            this.HostBackground = HostColor.NeutralBlack;
            this.GraphicRendition = GraphicRendition.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="other">Other cell to clone</param>
        public Cell(Cell other)
        {
            this.Text = other.Text;
            this.HostForeground = other.HostForeground;
            this.HostBackground = other.HostBackground;
            this.GraphicRendition = other.GraphicRendition;
        }

        /// <summary>
        /// Gets or sets the character at this location.
        /// </summary>
        public char Text { get; set; }

        /// <summary>
        /// Gets or sets the original host foreground color.
        /// </summary>
        public HostColor HostForeground { get; set; }

        /// <summary>
        /// Gets or sets the translated console background color.
        /// </summary>
        public HostColor HostBackground { get; set; }

        /// <summary>
        /// Gets or sets the graphic rendition flags.
        /// </summary>
        public GraphicRendition GraphicRendition { get; set; }

        /// <summary>
        /// Gets a value indicating whether the cell is the left side of a DBCS character.
        /// </summary>
        public bool IsDbcsLeft => this.GraphicRendition.HasFlag(GraphicRendition.Wide) && this.Text != 0;

        /// <summary>
        /// Gets a value indicating whether the cell is the right side of a DBCS character.
        /// </summary>
        public bool IsDbcsRight => this.GraphicRendition.HasFlag(GraphicRendition.Wide) && this.Text == 0;
    }
}
