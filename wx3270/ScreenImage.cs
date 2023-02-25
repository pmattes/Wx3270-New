// <copyright file="ScreenImage.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System.Threading;

    /// <summary>
    /// Selection state.
    /// </summary>
    public enum SelectState
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown,

        /// <summary>
        /// Last state was (or current state is) NVT.
        /// </summary>
        LastNvt,

        /// <summary>
        /// Last state was (or current state is) 3270.
        /// </summary>
        Last3270,
    }

    /// <summary>
    /// Screen image.
    /// </summary>
    public class ScreenImage
    {
        /// <summary>
        /// The rolling sequence number.
        /// </summary>
        private static int sequence;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenImage"/> class.
        /// </summary>
        public ScreenImage()
        {
            this.Sequence = Interlocked.Increment(ref sequence);
            this.ColorMode = true;
            this.MaxRows = 24;
            this.MaxColumns = 80;
            this.Model = 2;
            this.Extended = true;
            this.Thumb = new Thumb();
            this.Clear(this.MaxRows, this.MaxColumns, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenImage"/> class.
        /// </summary>
        /// <param name="other">Existing <see cref="ScreenImage"/> to clone.</param>
        public ScreenImage(ScreenImage other)
        {
            this.Sequence = Interlocked.Increment(ref sequence);
            this.ColorMode = other.ColorMode;
            this.MaxRows = other.MaxRows;
            this.MaxColumns = other.MaxColumns;
            this.LogicalRows = other.LogicalRows;
            this.LogicalColumns = other.LogicalColumns;
            this.HostForeground = other.HostForeground;
            this.HostBackground = other.HostBackground;
            this.CursorEnabled = other.CursorEnabled;
            this.CursorRow1 = other.CursorRow1;
            this.CursorColumn1 = other.CursorColumn1;
            this.Oversize = other.Oversize;
            this.Model = other.Model;
            this.Extended = other.Extended;
            this.Settings = new SettingsDictionary(other.Settings);
            this.TraceFile = other.TraceFile;
            this.Thumb = other.Thumb.Clone();
            this.Flipped = other.Flipped;
            this.SelectState = other.SelectState;
            this.Image = new Cell[this.MaxRows, this.MaxColumns];
            for (int row = 0; row < this.MaxRows; row++)
            {
                for (int column = 0; column < this.MaxColumns; column++)
                {
                    this.Image[row, column] = new Cell(other.Image[row, column]);
                }
            }
        }

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        public int Sequence { get; private set; }

        /// <summary>
        /// Gets or sets the individual cells.
        /// </summary>
        public Cell[,] Image { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the emulator supports color.
        /// </summary>
        public bool ColorMode { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of rows.
        /// </summary>
        public int MaxRows { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of columns.
        /// </summary>
        public int MaxColumns { get; set; }

        /// <summary>
        /// Gets or sets the current number of logical rows in the screen image.
        /// </summary>
        public int LogicalRows { get; set; }

        /// <summary>
        /// Gets or sets the current number of logical columns in the screen image.
        /// </summary>
        public int LogicalColumns { get; set; }

        /// <summary>
        /// Gets or sets the default host foreground color.
        /// </summary>
        public HostColor HostForeground { get; set; } = HostColor.NeutralWhite;

        /// <summary>
        /// Gets or sets the default host background color.
        /// </summary>
        public HostColor HostBackground { get; set; } = HostColor.NeutralBlack;

        /// <summary>
        /// Gets or sets a value indicating whether the cursor is enabled.
        /// </summary>
        public bool CursorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the current cursor row (1-origin).
        /// </summary>
        public int CursorRow1 { get; set; }

        /// <summary>
        /// Gets the current cursor row (0-origin).
        /// </summary>
        public int CursorRow0
        {
            get { return this.CursorRow1 - 1; }
        }

        /// <summary>
        /// Gets or sets the current cursor column (1-origin).
        /// </summary>
        public int CursorColumn1 { get; set; }

        /// <summary>
        /// Gets the current cursor column (0-origin).
        /// </summary>
        public int CursorColumn0
        {
            get { return this.CursorColumn1 - 1; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is oversize.
        /// </summary>
        public bool Oversize { get; set; }

        /// <summary>
        /// Gets or sets the model number.
        /// </summary>
        public int Model { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether extended mode is on.
        /// </summary>
        public bool Extended { get; set; }

        /// <summary>
        /// Gets or sets the scrollbar thumb state.
        /// </summary>
        public Thumb Thumb { get; set; }

        /// <summary>
        /// Gets or sets the settings dictionary.
        /// </summary>
        public SettingsDictionary Settings { get; set; } = new SettingsDictionary();

        /// <summary>
        /// Gets or sets the trace file name.
        /// </summary>
        public string TraceFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is in right-to-left mode.
        /// </summary>
        public bool Flipped { get; set; }

        /// <summary>
        /// Gets or sets the selection state.
        /// </summary>
        public SelectState SelectState { get; set; }

        /// <summary>
        /// Mark a single cell as selected or unselected.
        /// </summary>
        /// <param name="row">Row (0-origin).</param>
        /// <param name="column">Column (0-origin).</param>
        /// <param name="selected">True if the cell is to be selected.</param>
        /// <returns>True if screen changed.</returns>
        public bool SetSelect(int row, int column, bool selected)
        {
            var changed = false;
            if (selected)
            {
                // Set selected state.
                if (!this.Image[row, column].GraphicRendition.HasFlag(GraphicRendition.Selected))
                {
                    this.Image[row, column].GraphicRendition |= GraphicRendition.Selected;
                    changed = true;
                }

                // Include DBCS. Either side causes both sides to be selected.
                if (this.Image[row, column].IsDbcsLeft)
                {
                    if (!this.Image[row, column + 1].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        this.Image[row, column + 1].GraphicRendition |= GraphicRendition.Selected;
                        changed = true;
                    }
                }

                if (this.Image[row, column].IsDbcsRight)
                {
                    if (!this.Image[row, column - 1].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        this.Image[row, column - 1].GraphicRendition |= GraphicRendition.Selected;
                        changed = true;
                    }
                }
            }
            else
            {
                // Clear selected state.
                if (this.Image[row, column].GraphicRendition.HasFlag(GraphicRendition.Selected))
                {
                    this.Image[row, column].GraphicRendition &= ~GraphicRendition.Selected;
                    changed = true;
                }

                // Cover DBCS. Clearing either side causes both sides to be deselected.
                if (this.Image[row, column].IsDbcsLeft)
                {
                    if (this.Image[row, column + 1].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        this.Image[row, column + 1].GraphicRendition &= ~GraphicRendition.Selected;
                        changed = true;
                    }
                }

                if (this.Image[row, column].IsDbcsRight)
                {
                    if (this.Image[row, column - 1].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        this.Image[row, column - 1].GraphicRendition &= ~GraphicRendition.Selected;
                        changed = true;
                    }
                }
            }

            return changed;
        }

        /// <summary>
        /// Set a region to be selected, and clear everything else. NVT mode.
        /// </summary>
        /// <param name="startBaddr">Start buffer address.</param>
        /// <param name="endBaddr">End buffer address, inclusive.</param>
        /// <returns>True if anything changed.</returns>
        public bool SetSelectNvt(int startBaddr, int endBaddr)
        {
            // If endBaddr is the left half of a DBCS character, include the right half.
            // If we don't do this, then when we scanned the right half, we would unselect the left half.
            if (this.Image[endBaddr / this.MaxColumns, endBaddr % this.MaxColumns].IsDbcsLeft)
            {
                endBaddr++;
            }

            var changed = false;
            for (var r = 0; r < this.MaxRows; r++)
            {
                for (var c = 0; c < this.MaxColumns; c++)
                {
                    var baddr = (r * this.MaxColumns) + c;
                    changed |= this.SetSelect(
                        r,
                        c,
                        baddr >= startBaddr && baddr <= endBaddr);
                }
            }

            return changed;
        }

        /// <summary>
        /// Set a region to be selected, and clear everything else. 3270 mode.
        /// </summary>
        /// <param name="startRow0">Starting row, 0-origin.</param>
        /// <param name="startColumn0">Starting column, 0-origin.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        /// <returns>True if anything changed.</returns>
        public bool SetSelect3270(int startRow0, int startColumn0, int rows, int columns)
        {
            var changed = false;
            for (var r = 0; r < this.MaxRows; r++)
            {
                // If the last character in this row is the left half of a DBCS character, include the right half.
                // If we don't do this, then when we scanned the right half, we would unselect the left half.
                int mc = columns;
                if (r >= startRow0 && r < startRow0 + rows && this.Image[r, startColumn0 + columns - 1].IsDbcsLeft)
                {
                    mc++;
                }

                for (var c = 0; c < this.MaxColumns; c++)
                {
                    changed |= this.SetSelect(
                        r,
                        c,
                        r >= startRow0 && r < startRow0 + rows && c >= startColumn0 && c < startColumn0 + mc);
                }
            }

            return changed;
        }

        /// <summary>
        /// Unselect everything on the screen.
        /// </summary>
        /// <returns>True if screen changed.</returns>
        public bool UnselectAll()
        {
            var ret = false;

            for (var row = 0; row < this.MaxRows; row++)
            {
                for (var column = 0; column < this.MaxColumns; column++)
                {
                    if (this.Image[row, column].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        this.Image[row, column].GraphicRendition &= ~GraphicRendition.Selected;
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Resize the screen image.
        /// </summary>
        /// <param name="maxRows">Maximum rows.</param>
        /// <param name="maxColumns">Maximum columns.</param>
        /// <param name="oversize">True if screen is oversize.</param>
        public void Resize(int maxRows, int maxColumns, bool oversize)
        {
            this.MaxRows = maxRows;
            this.LogicalRows = maxRows;
            this.MaxColumns = maxColumns;
            this.LogicalColumns = maxColumns;
            this.Oversize = oversize;

            // Set up a temporary image. The next 'clear' will set up a real one.
            this.Image = new Cell[this.MaxRows, this.MaxColumns];
            for (int row = 0; row < this.MaxRows; row++)
            {
                for (int column = 0; column < this.MaxColumns; column++)
                {
                    this.Image[row, column] = new Cell { Text = ' ' };
                }
            }
        }

        /// <summary>
        /// Clear a screen image.
        /// </summary>
        /// <param name="logicalRows">Logical rows.</param>
        /// <param name="logicalColumns">Logical columns.</param>
        /// <param name="hostForeground">Host foreground color.</param>
        /// <param name="hostBackground">Host background color.</param>
        public void Clear(int? logicalRows, int? logicalColumns, HostColor? hostForeground, HostColor? hostBackground)
        {
            if (logicalRows.HasValue)
            {
                this.LogicalRows = logicalRows.Value;
            }

            if (logicalColumns.HasValue)
            {
                this.LogicalColumns = logicalColumns.Value;
            }

            if (hostForeground.HasValue)
            {
                this.HostForeground = hostForeground.Value;
            }

            if (hostBackground.HasValue)
            {
                this.HostBackground = hostBackground.Value;
            }

            // Set up a default image.
            this.Image = new Cell[this.MaxRows, this.MaxColumns];
            for (int row = 0; row < this.MaxRows; row++)
            {
                for (int column = 0; column < this.MaxColumns; column++)
                {
                    this.Image[row, column] = new Cell
                    {
                        Text = ' ',
                        HostForeground = this.HostForeground,
                        HostBackground = this.HostBackground,
                        GraphicRendition = GraphicRendition.None,
                    };
                }
            }

            this.CursorRow1 = 1;
            this.CursorColumn1 = 1;
        }
    }
}
