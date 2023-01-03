// <copyright file="ScreenBox.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using Wx3270.Contracts;

    /// <summary>
    /// Delegate for size change events.
    /// </summary>
    /// <param name="cellSizeHeight">New cell size height.</param>
    public delegate void SizeChanged(int cellSizeHeight);

    /// <summary>
    /// Resize operation type.
    /// </summary>
    public enum ResizeType
    {
        /// <summary>
        /// Window has been maximized.
        /// </summary>
        Maximizing,

        /// <summary>
        /// Drawing while maximized.
        /// </summary>
        MaximizedDraw,

        /// <summary>
        /// User changed the window size.
        /// </summary>
        Dynamic,
    }

    /// <summary>
    /// A 3270 display.
    /// </summary>
    public class ScreenBox : IBrush, IMeasure
    {
        /// <summary>
        /// Cache of solid color brushes.
        /// </summary>
        private readonly Dictionary<Color, Brush> brushCache = new Dictionary<Color, Brush>();

        /// <summary>
        /// APL-special to Greek translation for the 3270 font.
        /// </summary>
        private readonly Dictionary<string, string> font3270Subst = new Dictionary<string, string>
        {
            { "⍺", "α" },
            { "∊", "ε" },
            { "⍳", "ι" },
            { "⍴", "ρ" },
            { "⍵", "ω" },
        };

        /// <summary>
        /// The IME window.
        /// </summary>
        private readonly Ime ime;

        /// <summary>
        /// The picture box containing the screen.
        /// </summary>
        private readonly Control pictureBox;

        /// <summary>
        /// The picture box behind this one where the crosshair is drawn.
        /// </summary>
        private readonly Control crosshairBox;

        /// <summary>
        /// The blink timer.
        /// </summary>
        private readonly Timer blinkTimer;

        /// <summary>
        /// The last-used maximum number of rows.
        /// </summary>
        private int lastRows = 0;

        /// <summary>
        /// The last-used maximum number of columns.
        /// </summary>
        private int lastColumns = 0;

        /// <summary>
        /// The logical rows on the screen.
        /// </summary>
        private int logicalRows = 0;

        /// <summary>
        /// The logical columns on the screen.
        /// </summary>
        private int logicalColumns = 0;

        /// <summary>
        /// True if blinking text should be displayed (we are on the 'display' cycle).
        /// </summary>
        private bool blinkOn = true;

        /// <summary>
        /// True if there are any blinking fields on the screen.
        /// </summary>
        private bool anyBlinkers;

        /// <summary>
        /// True if the containing form has been activated.
        /// </summary>
        private bool activated;

        /// <summary>
        /// The size of the fixed areas of the screen.
        /// </summary>
        private Size overhead;

        /// <summary>
        /// The minimum client width.
        /// </summary>
        private int minimumClientWidth;

        /// <summary>
        /// The minimum client height.
        /// </summary>
        private int minimumClientHeight;

        /// <summary>
        /// The main screen client size while maximized.
        /// </summary>
        private Size maximizedSize;

        /// <summary>
        /// True if the screen is in right-to-left drawing mode.
        /// </summary>
        private bool flipped;

        /// <summary>
        /// The last rendered screen image.
        /// </summary>
        private ScreenImage lastImage;

        /// <summary>
        /// The odd-width character measurer.
        /// </summary>
        private OddChars oddChars;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenBox"/> class.
        /// </summary>
        /// <param name="type">Screen type.</param>
        /// <param name="pictureBox">Control implementing the screen.</param>
        /// <param name="crosshairBox">Crosshair control to invalidate when this one is drawn.</param>
        public ScreenBox(string type, Control pictureBox, Control crosshairBox = null)
        {
            this.Type = type;
            this.pictureBox = pictureBox;
            this.crosshairBox = crosshairBox;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                this.ime = new Ime(pictureBox.Handle);
            }

            this.blinkTimer = new Timer();
            this.blinkTimer.Tick += new EventHandler(this.BlinkTimerTick);
            this.blinkTimer.Interval = 500;

            this.oddChars = new OddChars(this);
        }

        /// <summary>
        /// Size change event.
        /// </summary>
        public event SizeChanged SizeChanged = (cellSizeHeight) => { };

        /// <summary>
        /// Font change event.
        /// </summary>
        public event Action<Font, bool> FontChanged = (newFont, dynamic) => { };

        /// <summary>
        /// Gets the next screen image to draw.
        /// </summary>
        public ScreenImage NextImage { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the screen is maximized.
        /// </summary>
        public bool Maximized { get; private set; }

        /// <summary>
        /// Gets the current screen font.
        /// </summary>
        public Font ScreenFont { get; private set; }

        /// <summary>
        /// Gets the underlined screen drawing font.
        /// </summary>
        public Font UnderlineFont { get; private set; }

        /// <summary>
        /// Gets the crosshair width.
        /// </summary>
        public int CrosshairWidth => this.CellSize.Width < 3 ? this.CellSize.Width : 3;

        /// <summary>
        /// Gets the crosshair height.
        /// </summary>
        public int CrosshairHeight => this.CellSize.Height < 3 ? this.CellSize.Height : 3;

        /// <summary>
        /// Gets a value indicating whether we are ready for a resize.
        /// </summary>
        public bool ResizeReady => this.lastColumns != 0 && this.lastRows != 0;

        /// <summary>
        /// Gets the size of a character cell.
        /// </summary>
        public Size CellSize { get; private set; }

        /// <summary>
        /// Gets the minimum client size.
        /// </summary>
        private Size MinimumClientSize => new Size(this.minimumClientWidth, this.minimumClientHeight);

        /// <summary>
        /// Gets or sets the screen type.
        /// </summary>
        private string Type { get; set; }

        /// <summary>
        /// Compute the size of a character cell for a given font.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="font">Font to measure.</param>
        /// <returns>Cell size.</returns>
        public static Size ComputeCellSize(Graphics g, Font font)
        {
            return TextRenderer.MeasureText(g, "X", font, new Size(1000, 1000), TextFormatFlags.Left | TextFormatFlags.NoPadding);
        }

        /// <summary>
        /// Cause the screen to be drawn.
        /// </summary>
        /// <param name="why">Why it needs to be redrawn.</param>
        /// <param name="complete">True if the entire screen needs redrawing.</param>
        /// <param name="image">New screen image.</param>
        public void ScreenNeedsDrawing(string why, bool complete, ScreenImage image)
        {
            var mode = complete ? "all" : "partial";
            var sequence = (image != null) ? " #" + image.Sequence : string.Empty;
            Trace.Line(Trace.Type.Draw, $"{this.Type} ScreenNeedsDrawing {why} {mode}{sequence}");

            if (image != null)
            {
                this.NextImage = image;
            }

            if (this.pictureBox != null)
            {
                if (this.lastImage != null && image != null && !complete)
                {
                    // Partial update.
                    var r = this.DrawArea(this.lastImage, image);
                    var rStr = (r != null) ? r.ToString() : "(none)";
                    Trace.Line(Trace.Type.Draw, $"{this.Type} ScreenNeedsDrawing partial {rStr}");
                    if (r != null)
                    {
                        this.pictureBox.Invalidate(r.Value);
                    }
                    else
                    {
                        // The (blinking) cursor may have moved, or blinking text may have changed, while blinked off.
                        // The text to be drawn when blink comes back on may be different. Save it.
                        this.lastImage = image;
                    }
                }
                else
                {
                    Trace.Line(Trace.Type.Draw, $"{this.Type} ScreenNeedsDrawing all");
                    this.pictureBox.Invalidate();
                }
            }

            if (this.crosshairBox != null)
            {
                this.crosshairBox.Invalidate();
            }
        }

        /// <summary>
        /// Change the screen font explicitly (via a profile or the settings dialog).
        /// </summary>
        /// <param name="font">New font.</param>
        /// <param name="image">Screen image.</param>
        public void ScreenNewFont(Font font, ScreenImage image)
        {
            this.ProcessNewFont(font, image.MaxRows, image.MaxColumns, (this.Maximized ? this.maximizedSize : this.MinimumClientSize) - this.overhead);
        }

        /// <summary>
        /// Compute the rectangle that needs to be redrawn.
        /// </summary>
        /// <param name="oldImage">Previous screen image.</param>
        /// <param name="newImage">New screen image.</param>
        /// <returns>Rectangle that needs re-drawing.</returns>
        public Rectangle? DrawArea(ScreenImage oldImage, ScreenImage newImage)
        {
            // If the image size changed, redraw completely.
            if (oldImage.MaxColumns != newImage.MaxColumns || oldImage.MaxRows != newImage.MaxRows)
            {
                Trace.Line(Trace.Type.Draw, "DrawArea: size changed");
                return new Rectangle(0, 0, newImage.MaxColumns * this.CellSize.Width, newImage.MaxRows * this.CellSize.Height);
            }

            // Annotate a copy of the new image with blink, cursor and crosshair state.
            var newImageCopy = this.Annotate(newImage);

            // Compute.
            Rectangle? r = null;
            for (int row = 0; row < newImageCopy.MaxRows; row++)
            {
                for (var column = newImageCopy.MaxColumns - 1; column >= 0; column--)
                {
                    if (newImageCopy.Image[row, column].Equals(oldImage.Image[row, column]))
                    {
                        continue;
                    }

                    // The current cell is considered wide if it has the wide attribute and is nonzero.
                    var cell = newImageCopy.Image[row, column];
                    var gr = cell.GraphicRendition;
                    var wide = gr.HasFlag(GraphicRendition.Wide) && cell.Text != '\0';
                    var renderedColumn = newImageCopy.Flipped ? newImageCopy.MaxColumns - 1 - column : column;
                    if (newImageCopy.Flipped && wide)
                    {
                        renderedColumn--;
                    }

                    // Compute the rectangle.
                    var rectangle = new Rectangle(
                        renderedColumn * this.CellSize.Width,
                        row * this.CellSize.Height,
                        wide ? (this.CellSize.Width * 2) : this.CellSize.Width,
                        this.CellSize.Height);

                    if (r == null)
                    {
                        r = rectangle;
                    }
                    else
                    {
                        r = Rectangle.Union(r.Value, rectangle);
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// The screen needs to be redrawn.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        /// <param name="image">Current screen image.</param>
        /// <param name="colors">Current colors.</param>
        public void ScreenDraw(
            object sender,
            PaintEventArgs e,
            ScreenImage image,
            Colors colors)
        {
            var all = (e.ClipRectangle.Width == image.MaxColumns * this.CellSize.Width && e.ClipRectangle.Height == image.MaxRows * this.CellSize.Height) ?
                "all" : "partial";
            Trace.Line(Trace.Type.Draw, $"ScreenDraw({this.Type}): {e.ClipRectangle} {all} #{image.Sequence}");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var font3270 = this.ScreenFont.Name == FontProfile.Name3270Font || this.ScreenFont.Name.StartsWith(FontProfile.Name3270FontRb);

            if (font3270 && this.ScreenFont.SizeInPoints >= 10)
            {
                // XXX: Will this change anything?
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            }

            // Remember the cursor state and logical dimensions.
            this.logicalRows = image.LogicalRows;
            this.logicalColumns = image.LogicalColumns;

            // Remember flipped state.
            this.flipped = image.Flipped;

            // Clear it.
            var clearBg = ColorBackground(image, colors);
            e.Graphics.Clear(clearBg);

            // No blinkers at the moment.
            this.anyBlinkers = false;

            // Get toggles.
            image.Settings.TryGetValue(B3270.Setting.MonoCase, out bool monoCase);
            image.Settings.TryGetValue(B3270.Setting.Crosshair, out bool crosshair);
            image.Settings.TryGetValue(B3270.Setting.AltCursor, out bool altCursor);
            image.Settings.TryGetValue(B3270.Setting.CursorBlink, out bool cursorBlink);

            // Compute the height of an underscore cursor.
            int underscoreCursorHeight = this.CellSize.Height / 5;
            if (underscoreCursorHeight == 0)
            {
                underscoreCursorHeight = 1;
            }

            if (image.CursorEnabled && cursorBlink)
            {
                this.anyBlinkers = true;
            }

            // Figure out the cursor location.
            (var cursorRow0, var cursorColumn0, var flippedCursorColumn0) = ComputeCursor(image);

            if (image.CursorEnabled && this.ime != null)
            {
                // Set the IME composition window location.
                this.ime.SetIMEWindowLocation(
                    flippedCursorColumn0 * this.CellSize.Width,
                    (cursorRow0 + 1) * this.CellSize.Height);
            }

            var clipped = 0;
            var drawn = 0;

            var pending = new Pending(this, e.Graphics, clearBg, this.oddChars);

            // Write the image.
            for (int row = 0; row < image.MaxRows; row++)
            {
                for (var column = image.MaxColumns - 1; column >= 0; column--)
                {
                    var renderedColumn = image.Flipped ? image.MaxColumns - 1 - column : column;
                    var cell = image.Image[row, column];
                    var gr = cell.GraphicRendition;

                    if (gr.HasFlag(GraphicRendition.Blink))
                    {
                        this.anyBlinkers = true;
                    }

                    bool drawingCursorLocation =
                        image.CursorEnabled
                        && (!cursorBlink || this.blinkOn)
                        && row == cursorRow0
                        && column == cursorColumn0;
                    bool drawingUnderscoreCursor = altCursor && drawingCursorLocation;

                    bool crosshairRow = false;
                    bool crosshairColumn = false;
                    if (image.CursorEnabled && crosshair && (!drawingCursorLocation || altCursor))
                    {
                        if (row == cursorRow0)
                        {
                            crosshairRow = true;
                        }

                        if (column == cursorColumn0)
                        {
                            crosshairColumn = true;
                        }
                    }

                    // Compute drawing colors.
                    var backgroundColor = image.ColorMode ? colors.HostColors[cell.HostBackground] : colors.MonoColors.Background;
                    var foregroundColor = image.ColorMode ?
                        colors.HostColors[cell.HostForeground] :
                        (gr.HasFlag(GraphicRendition.Highlight) ?
                            colors.MonoColors.Intensified :
                            colors.MonoColors.Normal);

                    // Grab the character to display.
                    var ch = cell.Text;

                    // Handle visible 3270 order characters.
                    if (gr.HasFlag(GraphicRendition.Order))
                    {
                        if ((ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'Z'))
                        {
                            // Start Field is always black on yellow.
                            backgroundColor = colors.HostColors[HostColor.Yellow];
                            foregroundColor = colors.HostColors[HostColor.Black];
                        }

                        if (ch == '*' && font3270)
                        {
                            // Dup.
                            ch = OiaFont.Symbol.Dup;
                        }
                        else if (ch == ';' && font3270)
                        {
                            // Field Mark.
                            ch = OiaFont.Symbol.FieldMark;
                        }
                        else
                        {
                            // All orders are underlined.
                            gr |= GraphicRendition.Underline;
                        }
                    }

                    // The current cell is considered wide if it has the wide attribute and is nonzero.
                    var wide = gr.HasFlag(GraphicRendition.Wide) && cell.Text != '\0';
                    if (image.Flipped && wide)
                    {
                        renderedColumn--;
                    }

                    // Compute the rectangle.
                    var rectangle = new Rectangle(
                        renderedColumn * this.CellSize.Width,
                        row * this.CellSize.Height,
                        wide ? (this.CellSize.Width * 2) : this.CellSize.Width,
                        this.CellSize.Height);

                    if (!rectangle.IntersectsWith(e.ClipRectangle))
                    {
                        // No need to draw it.
                        clipped++;
                        continue;
                    }

                    // Invert foreground and background for the block cursor and for inverted text.
                    if ((drawingCursorLocation && !altCursor) || (!image.ColorMode && gr.HasFlag(GraphicRendition.Reverse)))
                    {
                        var x = backgroundColor;
                        backgroundColor = foregroundColor;
                        foregroundColor = x;
                    }

                    // Finally, if the text is selected, and we're not drawing a block cursor, use the selection color as the background.
                    var selected = gr.HasFlag(GraphicRendition.Selected);
                    if (!selected
                        && gr.HasFlag(GraphicRendition.Wide)
                        && cell.Text != '\0'
                        && image.Image[row, column + 1].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        // The current cell is not selected, but is the left side of a DBCS character whose right
                        // side *is* selected. Render it as selected.
                        selected = true;
                    }

                    if (selected && !(drawingCursorLocation && !altCursor))
                    {
                        backgroundColor = colors.SelectBackground;
                    }

                    // If drawing the cursor, it will happen inline.
                    var forceInline = drawingCursorLocation || crosshairRow || crosshairColumn;
                    if (forceInline)
                    {
                        drawn += pending.Flush();
                    }

                    // Draw the background color.
                    if (forceInline && backgroundColor != clearBg)
                    {
                        e.Graphics.FillRectangle(this.BrushFromColor(backgroundColor), rectangle);
                    }

                    // Draw the crosshair cursor.
                    if (crosshairRow)
                    {
                        int height = this.CrosshairHeight;
                        var crosshairHorizontalRectangle = new Rectangle(
                            renderedColumn * this.CellSize.Width,
                            (row * this.CellSize.Height) + (this.CellSize.Height - height),
                            wide ? (this.CellSize.Width * 2) : this.CellSize.Width,
                            height);

                        e.Graphics.FillRectangle(this.BrushFromColor(colors.CrosshairColor), crosshairHorizontalRectangle);
                    }

                    if (crosshairColumn)
                    {
                        int width = this.CrosshairWidth;
                        var crosshairVerticalRectangle = new Rectangle(
                            renderedColumn * this.CellSize.Width,
                            row * this.CellSize.Height,
                            width,
                            this.CellSize.Height);
                        e.Graphics.FillRectangle(this.BrushFromColor(colors.CrosshairColor), crosshairVerticalRectangle);
                    }

                    // Draw the underscore cursor, which is in the foreground color.
                    if (drawingUnderscoreCursor)
                    {
                        var underscoreRectangle = new Rectangle(
                            renderedColumn * this.CellSize.Width,
                            (row * this.CellSize.Height) + (this.CellSize.Height - underscoreCursorHeight),
                            wide ? (this.CellSize.Width * 2) : this.CellSize.Width,
                            underscoreCursorHeight);
                        e.Graphics.FillRectangle(this.BrushFromColor(foregroundColor), underscoreRectangle);
                    }

                    // Implement blinking text.
                    if (ch != '\0' && gr.HasFlag(GraphicRendition.Blink) && !this.blinkOn)
                    {
                        ch = ' ';
                    }

                    // If it's a null (right-hand side of DBCS), do nothing.
                    if (ch == '\0')
                    {
                        continue;
                    }

                    var displayString = new string(new[] { ch });

                    // Implement monocase.
                    if (monoCase)
                    {
                        displayString = displayString.ToUpperInvariant();
                    }

                    // If the font wants to display an SBCS character in a wide substitution font, display a default character instead.
                    if (ch > 0xff &&
                        !gr.HasFlag(GraphicRendition.Wide) &&
                        this.MeasureText(e.Graphics, displayString).Width > this.CellSize.Width)
                    {
                        displayString = "▨";
                    }

                    // Do APL special symbol translation where possible.
                    if (font3270 && this.font3270Subst.TryGetValue(displayString, out string aplDisplayString))
                    {
                        displayString = aplDisplayString;
                    }

                    // Draw the character. We use TextRenderer rather than DrawString, because there does not appear to be any
                    // way to stop DrawString from shifting the output an arbitrary number of pixels to the right. Could it be that I was
                    // not setting ScreenAlignment.Left? Worth experimenting, I suppose.
                    if (drawingUnderscoreCursor)
                    {
                        // Draw the text in the background color, which will overwrite part of the underscore cursor.
                        // We also avoid the underline font, which conflicts with the cursor.
                        TextRenderer.DrawText(
                            e.Graphics,
                            displayString,
                            this.ScreenFont,
                            new Point(rectangle.X, rectangle.Y),
                            backgroundColor,
                            TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

                        // Re-draw the text in the foreground color, clipped to a rectangle that does not include the underscore cursor.
                        var altRect = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width * 2, rectangle.Height - underscoreCursorHeight);
                        TextRenderer.DrawText(
                            e.Graphics,
                            displayString,
                            this.ScreenFont,
                            altRect,
                            foregroundColor,
                            TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                    }
                    else
                    {
                        // Accumulate additional text.
                        drawn += pending.AddText(rectangle, displayString, backgroundColor, foregroundColor, gr, image.Flipped);
                    }
                }

                // Flush whatever might be pending from this row.
                drawn += pending.Flush();
            }

            // Begin blinking.
            if (this.anyBlinkers)
            {
                this.StartBlinking();
            }
            else
            {
                this.StopBlinking();
            }

            // Remember the last image, for future incremental updates.
            this.lastImage = this.Annotate(image);

            stopwatch.Stop();
            var msec = stopwatch.ElapsedMilliseconds;
            var total = this.lastRows * this.lastColumns;
            var unclipped = total - clipped;
            var per = msec / (double)drawn;
            Trace.Line(
                Trace.Type.Draw,
                $"ScreenDraw {msec}ms {total}/{unclipped}/{drawn} total/unclipped/drawn {per:N3} ms/char");
        }

        /// <summary>
        /// The crosshair needs to be redrawn.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        /// <param name="image">Current screen image.</param>
        /// <param name="colors">Current colors.</param>
        public void CrosshairDraw(
            object sender,
            PaintEventArgs e,
            ScreenImage image,
            Colors colors)
        {
            // Clear the image.
            var clearBg = ColorBackground(image, colors);
            e.Graphics.Clear(clearBg);

            // If the cursor is off, we are not in crosshair mode, or the screen array is not floating, there is nothing to do.
            if (!image.CursorEnabled ||
                !image.Settings.TryGetValue(B3270.Setting.Crosshair, out bool crosshair) ||
                !crosshair ||
                this.pictureBox.Location == new Point(0, 0))
            {
                return;
            }

            // Figure out the cursor location.
            (var cursorRow0, _, var cursorColumn0) = ComputeCursor(image);

            // Draw the vertical.
            if (this.pictureBox.Height < this.crosshairBox.Height)
            {
                int width = this.CrosshairWidth;
                var crosshairVerticalRectangle = new Rectangle(
                    this.pictureBox.Location.X + (cursorColumn0 * this.CellSize.Width),
                    0,
                    width,
                    this.crosshairBox.Height);
                e.Graphics.FillRectangle(this.BrushFromColor(colors.CrosshairColor), crosshairVerticalRectangle);
            }

            // Draw the horizontal.
            if (this.pictureBox.Width < this.crosshairBox.Width)
            {
                int height = this.CrosshairHeight;
                var crosshairHorizontalRectangle = new Rectangle(
                    0,
                    this.pictureBox.Location.Y + (cursorRow0 * this.CellSize.Height) + (this.CellSize.Height - height),
                    this.crosshairBox.Width,
                    height);

                e.Graphics.FillRectangle(this.BrushFromColor(colors.CrosshairColor), crosshairHorizontalRectangle);
            }
        }

        /// <summary>
        /// The form has been activated or deactivated.
        /// </summary>
        /// <param name="activated">True if the form has been activated.</param>
        public void Activated(bool activated)
        {
            this.activated = activated;
            if (activated)
            {
                if (this.anyBlinkers)
                {
                    this.blinkOn = true;
                    this.blinkTimer.Enabled = true;
                }
            }
            else
            {
                if (this.anyBlinkers)
                {
                    this.blinkOn = true;
                    this.blinkTimer.Enabled = false;
                    this.ScreenNeedsDrawing("activated", false, null);
                }
            }
        }

        /// <summary>
        /// Set the fixed width and height.
        /// </summary>
        /// <param name="width">Fixed width.</param>
        /// <param name="height">Fixed height.</param>
        public void SetFixed(int width, int height)
        {
            this.overhead = new Size(width, height);
            Trace.Line(Trace.Type.Window, "ScreenBox.SetFixed: {0}", this.overhead);
        }

        /// <summary>
        /// Set the minimum client size.
        /// </summary>
        /// <param name="width">Minimum width.</param>
        /// <param name="height">Minimum height.</param>
        public void SetMinimumClient(int width, int height)
        {
            this.minimumClientWidth = width;
            this.minimumClientHeight = height;
            Trace.Line(Trace.Type.Window, "ScreenBox.SetMinimumClient: {0}", this.MinimumClientSize);
        }

        /// <summary>
        /// Maximize (or un-maximize) the screen.
        /// </summary>
        /// <param name="nowMaximized">True if now maximized.</param>
        /// <param name="clientSize">Main screen form client size.</param>
        public void Maximize(bool nowMaximized, Size clientSize)
        {
            Trace.Line(Trace.Type.Window, "ScreenBox.Maximize nowMaximized={0} clientSize={1}", nowMaximized, clientSize);

            if (this.Maximized == nowMaximized)
            {
                // Avoid redundant operations.
                Trace.Line(Trace.Type.Window, " Nop");
                return;
            }

            this.maximizedSize = clientSize;
            this.Maximized = nowMaximized;
        }

        /// <summary>
        /// Translate mouse coordinates to a row and column.
        /// </summary>
        /// <param name="mouseLocation">Location from mouse event.</param>
        /// <returns>1-origin row and column.</returns>
        public Tuple<int, int> CellCoordinates(Point mouseLocation)
        {
            // Figure out where the mouse is in terms of character cells.
            var y = mouseLocation.Y;
            y = Math.Max(y, 0);
            y = Math.Min(y, (this.CellSize.Height * this.logicalRows) - 1);
            var row = y / this.CellSize.Height;

            var x = mouseLocation.X;
            x = Math.Max(x, 0);
            x = Math.Min(x, (this.CellSize.Width * this.logicalColumns) - 1);
            var column = x / this.CellSize.Width;

            // Return those coordinates.
            return new Tuple<int, int>(row + 1, (this.flipped ? (this.logicalColumns - 1) - column : column) + 1);
        }

        /// <summary>
        /// Re-compute the font for a given target size.
        /// </summary>
        /// <param name="targetSize">Target size.</param>
        /// <param name="resizeType">Resize type.</param>
        /// <returns>New font.</returns>
        public Font RecomputeFont(Size targetSize, ResizeType resizeType)
        {
            Trace.Line(Trace.Type.Window);
            Trace.Line(Trace.Type.Window, "RecomputeFont: targetSize {0} resizeType {1}", targetSize, resizeType);
            Trace.Line(Trace.Type.Window, " current font height: {0} {1}s", this.ScreenFont.Size, this.ScreenFont.Unit);

            // Compute the current size.
            Size currentTextSize = new Size(
                this.CellSize.Width * this.lastColumns,
                this.CellSize.Height * (this.lastRows + 1));

            // Compute the area they are leaving for text.
            Trace.Line(Trace.Type.Window, " overhead {0}", this.overhead);
            var targetTextSize = targetSize - this.overhead;
            Trace.Line(Trace.Type.Window, " current text size {0}, current total size {1}", currentTextSize, currentTextSize + this.overhead);

            // Don't let them go below the minimum text area.
            targetTextSize = this.MinimumSize(targetTextSize);

            // Compute a new font size to fill the screen.
            var maxCell = new Size(
                Math.Max(1, targetTextSize.Width / this.lastColumns),
                Math.Max(1, targetTextSize.Height / (this.lastRows + 1)));

            Trace.Line(Trace.Type.Window, " targetTextSize {0} maxCell {1}", targetTextSize, maxCell);

            // Specifying the height of a font in pixels does not actually specify the size of a character cell. It is the
            // size of a character embedded in a larger cell. So I need to compute the right size.
            var pixelHeight = maxCell.Height;
            Font font;
            Size size;
            using (Graphics g = this.pictureBox.CreateGraphics())
            {
                font = new Font(this.ScreenFont.FontFamily, pixelHeight, this.ScreenFont.Style, GraphicsUnit.Pixel);
                size = ComputeCellSize(g, font);
                Trace.Line(
                    Trace.Type.Window,
                    " initial  pixelHeight {0} => size {1}, total size {2}",
                    pixelHeight,
                    size,
                    new Size(size.Width * this.lastColumns, size.Height * (this.lastRows + 1)) + this.overhead);
                while ((size.Height > maxCell.Height || size.Width > maxCell.Width) && pixelHeight > 1)
                {
                    pixelHeight--;
                    font = new Font(this.ScreenFont.FontFamily, pixelHeight, this.ScreenFont.Style, GraphicsUnit.Pixel);
                    size = ComputeCellSize(g, font);
                    Trace.Line(
                        Trace.Type.Window,
                        " iterated pixelHeight {0} => size {1}, total size {2}",
                        pixelHeight,
                        size,
                        new Size(size.Width * this.lastColumns, size.Height * (this.lastRows + 1)) + this.overhead);
                }
            }

            var newTextSize = new Size(size.Width * this.lastColumns, size.Height * (this.lastRows + 1));
            Trace.Line(Trace.Type.Window, " newTextSize: {0}, new total size {1}", newTextSize, newTextSize + this.overhead);

            // Use the new font.
            this.ProcessNewFont(font, this.lastRows, this.lastColumns, resizeType == ResizeType.Dynamic ? targetSize - this.overhead : (Size?)targetTextSize);
            this.FontChanged(font, resizeType == ResizeType.Dynamic);
            return font;
        }

        /// <summary>
        /// Snap the screen (make it fit the current font tightly).
        /// </summary>
        public void Snap()
        {
            Trace.Line(Trace.Type.Window, "ScreenBox.Snap");

            // Set the screen box size. This used to happen every time the font changed.
            this.SetScreenBoxSize(this.lastRows, this.lastColumns, false, null);
        }

        /// <summary>
        /// The screen mode changed. Possibly change the screen box size or (if maximized) recompute the font.
        /// </summary>
        /// <param name="image">New screen image.</param>
        /// <returns>True if screen box size changed.</returns>
        public bool ChangeScreenMode(ScreenImage image)
        {
            if (image.MaxRows != this.lastRows || image.MaxColumns != this.lastColumns)
            {
                this.SetScreenBoxSize(image.MaxRows, image.MaxColumns, true, this.Maximized ? (Size?)this.maximizedSize : null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get a brush for a given color.
        /// </summary>
        /// <param name="color">color to map.</param>
        /// <returns>Cached brush.</returns>
        public Brush BrushFromColor(Color color)
        {
            if (this.brushCache.TryGetValue(color, out Brush brush))
            {
                return brush;
            }

            brush = new SolidBrush(color);
            this.brushCache[color] = brush;
            return brush;
        }

        /// <summary>
        /// Measure a text string.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Size of rendered string.</returns>
        public Size MeasureText(Graphics graphics, string text)
        {
            return TextRenderer.MeasureText(
                graphics,
                text,
                this.UnderlineFont,
                new Size(1000, 1000),
                TextFormatFlags.Left | TextFormatFlags.NoPadding);
        }

        /// <summary>
        /// Compute the cursor coordinates.
        /// </summary>
        /// <param name="image">Image to inspect.</param>
        /// <returns>Cursor coordinates.</returns>
        private static (int, int, int) ComputeCursor(ScreenImage image)
        {
            // Figure out the cursor location. If the cursor is on the right side of a DBCS
            // character, act as if it is on the left side.
            var cursorRow0 = image.CursorRow0;
            var cursorColumn0 = image.CursorColumn0;
            var flippedCursorColumn0 = image.Flipped ? (image.LogicalColumns - image.CursorColumn1) : cursorColumn0;
            if (image.Image[cursorRow0, cursorColumn0].Text == '\0')
            {
                cursorColumn0--;
                flippedCursorColumn0--;
            }

            return (cursorRow0, cursorColumn0, flippedCursorColumn0);
        }

        /// <summary>
        /// Compute the background color.
        /// </summary>
        /// <param name="image">Image to inspect.</param>
        /// <param name="colors">Color map.</param>
        /// <returns>Background color.</returns>
        private static Color ColorBackground(ScreenImage image, Colors colors)
        {
            return image.ColorMode ? colors.HostColors[HostColor.NeutralBlack] : colors.MonoColors.Background;
        }

        /// <summary>
        /// Annotate how an image should be drawn.
        /// </summary>
        /// <param name="image">Screen image.</param>
        /// <returns>Annotated image.</returns>
        private ScreenImage Annotate(ScreenImage image)
        {
            // Get toggles.
            image.Settings.TryGetValue(B3270.Setting.Crosshair, out bool crosshair);
            image.Settings.TryGetValue(B3270.Setting.CursorBlink, out bool cursorBlink);

            // Annotate the new image with blink, cursor and crosshair state.
            var imageCopy = new ScreenImage(image);
            (var cursorRow0, var cursorColumn0, _) = ComputeCursor(imageCopy);
            for (var row = 0; row < imageCopy.MaxRows; row++)
            {
                for (var column = 0; column < imageCopy.MaxColumns; column++)
                {
                    imageCopy.Image[row, column].GraphicRendition &= ~(GraphicRendition.IsBlinkingOn | GraphicRendition.IsCrosshair | GraphicRendition.IsCursor);

                    if (this.blinkOn && imageCopy.Image[row, column].GraphicRendition.HasFlag(GraphicRendition.Blink))
                    {
                        imageCopy.Image[row, column].GraphicRendition |= GraphicRendition.IsBlinkingOn;
                    }

                    if (imageCopy.CursorEnabled)
                    {
                        if ((!cursorBlink || this.blinkOn) && row == cursorRow0 && column == cursorColumn0)
                        {
                            // XXX: Not sure about the right-hand side of a DBCS cursor here.
                            imageCopy.Image[row, column].GraphicRendition |= GraphicRendition.IsCursor;
                            if (cursorBlink && this.blinkOn)
                            {
                                imageCopy.Image[row, column].GraphicRendition |= GraphicRendition.IsBlinkingOn;
                            }
                        }

                        if (crosshair && (row == cursorRow0 || column == cursorColumn0))
                        {
                            imageCopy.Image[row, column].GraphicRendition |= GraphicRendition.IsCrosshair;
                        }
                    }
                }
            }

            return imageCopy;
        }

        /// <summary>
        /// Process a font change.
        /// </summary>
        /// <param name="font">New font.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        /// <param name="minimumParentSize">Minimum parent size.</param>
        private void ProcessNewFont(Font font, int rows, int columns, Size? minimumParentSize)
        {
            if (this.ScreenFont != font)
            {
                Trace.Line(
                    Trace.Type.Window,
                    "ProcessNewFont {0} {1}s, {2} rows {3} columns, minSize {4}",
                    font.Size,
                    font.Unit,
                    rows,
                    columns,
                    minimumParentSize);

                // Save the font.
                this.ScreenFont = font;

                // Create the underline font.
                if (this.UnderlineFont != null)
                {
                    this.UnderlineFont.Dispose();
                }

                this.UnderlineFont = new Font(this.ScreenFont, this.ScreenFont.Style | FontStyle.Underline);

                // Set up the cell measurements.
                Trace.Line(Trace.Type.Window, " Old cell size: {0}", this.CellSize);
                using (Graphics g = this.pictureBox.CreateGraphics())
                {
                    this.CellSize = this.MeasureText(g, "X");
                    Trace.Line(Trace.Type.Window, " New cell size: {0}", this.CellSize);
                    this.oddChars.Reset(this.CellSize);
                }
            }

            // Resize the screen.
            this.SetScreenBoxSize(rows, columns, false, minimumParentSize);
        }

        /// <summary>
        /// Make sure a size is within the minimum.
        /// </summary>
        /// <param name="size">Proposed size.</param>
        /// <param name="minimum">Minimum size.</param>
        /// <returns>Adjusted size.</returns>
        private Size MinimumSize(Size size, Size? minimum = null)
        {
            if (minimum != null)
            {
                return new Size(
                    size.Width < minimum.Value.Width ? minimum.Value.Width : size.Width,
                    size.Height < minimum.Value.Height ? minimum.Value.Height : size.Height);
            }
            else
            {
                return size;
            }
        }

        /// <summary>
        /// Set the size of the screen box.
        /// </summary>
        /// <param name="rows">New rows.</param>
        /// <param name="columns">New columns.</param>
        /// <param name="isExplicit">True if called explicitly.</param>
        /// <param name="minimumParentSize">Minimum parent window size.</param>
        private void SetScreenBoxSize(int rows, int columns, bool isExplicit, Size? minimumParentSize)
        {
            Trace.Line(
                Trace.Type.Window,
                "SetScreenBoxSize rows {0} columns {1} isExplcit {2} minimumParentSize {3}",
                rows,
                columns,
                isExplicit,
                minimumParentSize);

            // If explicit and maximized, just remember the rows/columns and recompute the font.
            if (isExplicit && this.Maximized)
            {
                Trace.Line(Trace.Type.Window, " explicit and Maximized");
                this.lastRows = rows;
                this.lastColumns = columns;
                this.RecomputeFont(this.maximizedSize, ResizeType.MaximizedDraw);
                return;
            }

            var newSize = new Size(columns * this.CellSize.Width, rows * this.CellSize.Height);
            Trace.Line(Trace.Type.Window, " pictureBox.Size {0} -> {1}", this.pictureBox.Size, newSize);
            this.pictureBox.Size = newSize; // might be a no-op

            // Make sure the parent is within the minimum size.
            Size? adjustedMinimumParentSize = null;
            if (minimumParentSize.HasValue)
            {
                adjustedMinimumParentSize = new Size(minimumParentSize.Value.Width, minimumParentSize.Value.Height - this.CellSize.Height);
            }

            var parentSize = this.MinimumSize(newSize, adjustedMinimumParentSize);
            Trace.Line(Trace.Type.Window, " pictureBox.Parent.Size {0} -> {1}", this.pictureBox.Parent.Size, parentSize);
            this.pictureBox.Parent.Size = parentSize;
            if (this.crosshairBox != null)
            {
                this.crosshairBox.Location = new Point(0, 0);
                this.crosshairBox.Size = parentSize;
                if (parentSize != newSize)
                {
                    // Float the picture box in its parent.
                    this.pictureBox.Location = new Point((parentSize.Width - newSize.Width) / 2, (parentSize.Height - newSize.Height) / 2);
                }
                else
                {
                    // Align the picture box with its parent.
                    this.pictureBox.Location = new Point(0, 0);
                }

                Trace.Line(
                    Trace.Type.Window,
                    " pictureBox Location {0} size {1} cellSize {2} parentSize {3} newSize {4}",
                    this.pictureBox.Location,
                    this.pictureBox.Size,
                    this.CellSize,
                    this.pictureBox.Parent.Size,
                    newSize);
            }

            Trace.Line(Trace.Type.Window, " newSize {0}", newSize);

            // Rearrange the main window.
            this.SizeChanged(this.CellSize.Height);

            this.lastRows = rows;
            this.lastColumns = columns;
        }

        /// <summary>
        /// The screen contains blinking text. If the blinking timer is not running, start it.
        /// </summary>
        private void StartBlinking()
        {
            if (this.activated && !this.blinkTimer.Enabled)
            {
                this.blinkOn = true;
                this.blinkTimer.Enabled = true;
            }
        }

        /// <summary>
        /// The screen does not contain blinking text. If the blinking timer is running, stop it.
        /// </summary>
        private void StopBlinking()
        {
            if (this.blinkTimer.Enabled)
            {
                this.blinkOn = true;
                this.blinkTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Timer tick for blinking text.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void BlinkTimerTick(object sender, EventArgs e)
        {
            // Toggle blinking text visibility.
            this.blinkOn = !this.blinkOn;

            // Repaint the screen.
            this.ScreenNeedsDrawing("blink timer", false, this.lastImage);

            // Blink again.
            this.blinkTimer.Enabled = true;
        }

        /// <summary>
        /// Class to hold pending updates.
        /// </summary>
        private class Pending
        {
            /// <summary>
            /// Brush allocator.
            /// </summary>
            private readonly IBrush brush;

            /// <summary>
            /// Graphics context.
            /// </summary>
            private readonly Graphics graphics;

            /// <summary>
            /// The clear background color.
            /// </summary>
            private readonly Color clearBg;

            /// <summary>
            /// The odd character measurer.
            /// </summary>
            private readonly OddChars oddChars;

            /// <summary>
            /// True if there is output pending.
            /// </summary>
            private bool pending = false;

            /// <summary>
            /// Pending rectangle.
            /// </summary>
            private Rectangle rectangle;

            /// <summary>
            /// Pending text.
            /// </summary>
            private string text;

            /// <summary>
            /// Pending background color.
            /// </summary>
            private Color backgroundColor;

            /// <summary>
            /// Pending foreground color.
            /// </summary>
            private Color foregroundColor;

            /// <summary>
            /// Graphic rendition.
            /// </summary>
            private GraphicRendition gr;

            /// <summary>
            /// Initializes a new instance of the <see cref="Pending"/> class.
            /// </summary>
            /// <param name="brush">Brush allocator.</param>
            /// <param name="graphics">Graphics context.</param>
            /// <param name="clearBg">Clear background color.</param>
            public Pending(IBrush brush, Graphics graphics, Color clearBg, OddChars oddChars)
            {
                this.brush = brush;
                this.graphics = graphics;
                this.clearBg = clearBg;
                this.oddChars = oddChars;
            }

            /// <summary>
            /// Add some text.
            /// </summary>
            /// <param name="rectangle">Bounding rectangle.</param>
            /// <param name="text">Text to prepend.</param>
            /// <param name="backgroundColor">Background color.</param>
            /// <param name="foregroundColor">Foreground color.</param>
            /// <param name="gr">Graphic rendition.</param>
            /// <param name="append">True if appending, false if prepending.</param>
            /// <returns>Number of characters drawn.</returns>
            public int AddText(Rectangle rectangle, string text, Color backgroundColor, Color foregroundColor, GraphicRendition gr, bool append)
            {
                var drawn = 0;
                var isOdd = this.oddChars.IsOdd(text[0], gr.HasFlag(GraphicRendition.Wide), this.graphics);

                if (!this.pending)
                {
                    // Greenfield.
                    this.Set(rectangle, text, backgroundColor, foregroundColor, gr);
                    if (isOdd)
                    {
                        drawn += this.Flush();
                    }

                    return drawn;
                }

                if (backgroundColor != this.backgroundColor
                    || foregroundColor != this.foregroundColor
                    || gr != this.gr
                    || isOdd)
                {
                    // Incompatible. Flush what's pending first.
                    drawn = this.Flush();

                    // Start accumulating again.
                    this.Set(rectangle, text, backgroundColor, foregroundColor, gr);
                    if (isOdd)
                    {
                        drawn += this.Flush();
                    }

                    return drawn;
                }

                // Prepend.
                if (append)
                {
                    this.text += text;
                }
                else
                {
                    this.text = text + this.text;
                }

                this.rectangle = Rectangle.Union(this.rectangle, rectangle);
                return drawn;
            }

            /// <summary>
            /// Flushes the pending data.
            /// </summary>
            /// <returns>Number of characters drawn.</returns>
            public int Flush()
            {
                var drawn = 0;

                if (!this.pending)
                {
                    return drawn;
                }

                // Paint the background.
                if (this.backgroundColor != this.clearBg)
                {
                    this.graphics.FillRectangle(this.brush.BrushFromColor(this.backgroundColor), this.rectangle);
                }

                // Do some trimming.
                if (!this.gr.HasFlag(GraphicRendition.Underline))
                {
                    while (this.text.StartsWith(" "))
                    {
                        this.rectangle.X += this.brush.CellSize.Width;
                        this.rectangle.Width -= this.brush.CellSize.Width;
                        this.text = this.text.Substring(1);
                    }

                    while (this.text.EndsWith(" "))
                    {
                        this.rectangle.Width -= this.brush.CellSize.Width;
                        this.text = this.text.Substring(0, this.text.Length - 1);
                    }
                }

                // Draw the text.
                if (this.text.Length != 0)
                {
                    // Trace.Line(Trace.Type.Draw, $"Flushing {this.text.Length} characters");
                    TextRenderer.DrawText(
                                this.graphics,
                                this.text,
                                this.gr.HasFlag(GraphicRendition.Underline) ? this.brush.UnderlineFont : this.brush.ScreenFont,
                                new Point(this.rectangle.X, this.rectangle.Y),
                                this.foregroundColor,
                                TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                    drawn = this.text.Length;
                }

                this.pending = false;
                return drawn;
            }

            /// <summary>
            /// Set the value.
            /// </summary>
            /// <param name="rectangle">Bounding rectangle.</param>
            /// <param name="text">Text to prepend.</param>
            /// <param name="backgroundColor">Background color.</param>
            /// <param name="foregroundColor">Foreground color.</param>
            /// <param name="gr">Graphic rendition.</param>
            private void Set(Rectangle rectangle, string text, Color backgroundColor, Color foregroundColor, GraphicRendition gr)
            {
                this.rectangle = rectangle;
                this.text = text;
                this.backgroundColor = backgroundColor;
                this.foregroundColor = foregroundColor;
                this.gr = gr;
                this.pending = true;
            }
        }

        /// <summary>
        /// Odd-width character tracking class.
        /// </summary>
        private class OddChars
        {
            /// <summary>
            /// The text measure interface.
            /// </summary>
            private readonly IMeasure measure;

            /// <summary>
            /// The dictionary.
            /// </summary>
            private readonly Dictionary<char, bool> isOdd = new Dictionary<char, bool>();

            /// <summary>
            /// The cell size.
            /// </summary>
            private Size cellSize;

            public OddChars(IMeasure measure)
            {
                this.measure = measure;
            }

            /// <summary>
            /// Reset, with a new cell size.
            /// </summary>
            /// <param name="cellSize">New cell size.</param>
            public void Reset(Size cellSize)
            {
                this.cellSize = cellSize;
                this.isOdd.Clear();
            }

            /// <summary>
            /// Tests a character for being an odd size.
            /// </summary>
            /// <param name="c">Character to measure.</param>
            /// <param name="isDbcs">True if double-width.</param>
            /// <param name="graphics">Graphics context.</param>
            /// <returns>True if odd sized.</returns>
            public bool IsOdd(char c, bool isDbcs, Graphics graphics)
            {
                if (this.isOdd.TryGetValue(c, out bool odd))
                {
                    return odd;
                }

                var size = this.measure.MeasureText(graphics, c.ToString());
                odd = size.Width != this.cellSize.Width * (isDbcs ? 2 : 1);
                this.isOdd[c] = odd;
                if (odd)
                {
                    Trace.Line(Trace.Type.Draw, $"IsOdd: U+{(int)c:X4} is odd");
                }

                return odd;
            }
        }
    }
}
