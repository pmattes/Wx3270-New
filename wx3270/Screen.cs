// <copyright file="Screen.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Linq;
    using Wx3270.Contracts;

    /// <summary>
    /// Screen image processing.
    /// </summary>
    public class Screen : BackEndEvent
    {
        /// <summary>
        /// Settings that trigger a screen update.
        /// </summary>
        private readonly string[] screenSettings = new[]
        {
            B3270.Setting.AltCursor,
            B3270.Setting.Crosshair,
            B3270.Setting.CursorBlink,
            B3270.Setting.MonoCase,
        };

        /// <summary>
        /// The current screen image.
        /// </summary>
        private readonly ScreenImage screenImage;

        /// <summary>
        /// The method to inform about screen changes.
        /// </summary>
        private readonly IUpdate invoke;

        /// <summary>
        /// True if we are inside a screen XML element.
        /// </summary>
        private bool inScreen;

        /// <summary>
        /// Current row number being processed within a screen element, if >= 0.
        /// </summary>
        private int curRow = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Screen"/> class.
        /// </summary>
        /// <param name="invoke">Invoke interface.</param>
        public Screen(IUpdate invoke)
        {
            this.Def = new[]
            {
                new BackEndEventDef(B3270.Indication.Attr, this.StartAttr),
                new BackEndEventDef(B3270.Indication.Char, this.StartChar),
                new BackEndEventDef(B3270.Indication.Cursor, this.StartCursor),
                new BackEndEventDef(B3270.Indication.Erase, this.StartErase),
                new BackEndEventDef(B3270.Indication.Flipped, this.StartFlipped),
                new BackEndEventDef(B3270.Indication.Row, this.StartRow, this.EndRow),
                new BackEndEventDef(B3270.Indication.Screen, this.StartScreen, this.EndScreen),
                new BackEndEventDef(B3270.Indication.ScreenMode, this.StartScreenMode),
                new BackEndEventDef(B3270.Indication.Scroll, this.StartScroll),
                new BackEndEventDef(B3270.Indication.Setting, this.StartSetting),
                new BackEndEventDef(B3270.Indication.Thumb, this.StartThumb),
                new BackEndEventDef(B3270.Indication.TraceFile, this.StartTraceFile),
            };

            this.screenImage = new ScreenImage();
            this.invoke = invoke;
        }

        /// <summary>
        /// Gets a snapshot of the current screen image.
        /// </summary>
        public ScreenImage ScreenImage
        {
            get
            {
                lock (this.screenImage)
                {
                    return new ScreenImage(this.screenImage);
                }
            }
        }

        /// <summary>
        /// Set the selection status of an individual cell.
        /// </summary>
        /// <param name="row">Row (0-origin).</param>
        /// <param name="column">Column (0-origin).</param>
        /// <param name="selected">Desired state.</param>
        public void SetSelect(int row, int column, bool selected)
        {
            if (this.screenImage.SetSelect(row, column, selected))
            {
                this.invoke.ScreenUpdate(ScreenUpdateType.Screen);
            }
        }

        /// <summary>
        /// Unselect the entire screen.
        /// </summary>
        public void UnselectAll()
        {
            if (this.screenImage.UnselectAll())
            {
                this.invoke.ScreenUpdate(ScreenUpdateType.Screen);
            }
        }

        /// <summary>
        /// Parse a set of comma-separated graphic rendition tokens into a <see cref="GraphicRendition"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>Parsed rendition.</returns>
        private static GraphicRendition ParseGr(string s)
        {
            var ret = GraphicRendition.None;

            if (!string.IsNullOrEmpty(s) && !s.Equals("default"))
            {
                foreach (var token in s.Split(','))
                {
                    ret |= (GraphicRendition)Enum.Parse(typeof(GraphicRendition), token.Replace("-", string.Empty), ignoreCase: true);
                }
            }

            return ret;
        }

        /// <summary>
        /// Process an erase start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartErase(string name, AttributeDict attributes)
        {
            lock (this.screenImage)
            {
                int? logicalRows = null;
                if (attributes.TryGetValue(B3270.Attribute.LogicalRows, out string newRows))
                {
                    logicalRows = int.Parse(newRows);
                }

                int? logicalColumns = null;
                if (attributes.TryGetValue(B3270.Attribute.LogicalColumns, out string newColumns))
                {
                    logicalColumns = int.Parse(newColumns);
                }

                HostColor? hostFg = null;
                if (attributes.TryGetValue(B3270.Attribute.Fg, out string color))
                {
                    hostFg = (HostColor)Enum.Parse(typeof(HostColor), color, ignoreCase: true);
                }

                HostColor? hostBg = null;
                if (attributes.TryGetValue(B3270.Attribute.Bg, out color))
                {
                    hostBg = (HostColor)Enum.Parse(typeof(HostColor), color, ignoreCase: true);
                }

                // Clear the screen.
                this.screenImage.Clear(
                    logicalRows,
                    logicalColumns,
                    hostFg,
                    hostBg);
            }

            this.invoke.ScreenUpdate(ScreenUpdateType.Screen);
        }

        /// <summary>
        /// Process a screen start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartScreen(string name, AttributeDict attributes)
        {
            this.inScreen = true;
            this.curRow = -1;
        }

        /// <summary>
        /// Process a row start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartRow(string name, AttributeDict attributes)
        {
            if (this.inScreen)
            {
                this.curRow = int.Parse(attributes[B3270.Attribute.Row]) - 1;
            }
        }

        /// <summary>
        /// Process a char start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartChar(string name, AttributeDict attributes)
        {
            if (this.curRow >= 0)
            {
                var text = attributes[B3270.Attribute.Text];
                this.ProcessCharOrAttr(attributes, text.ToArray(), text.Length);
            }
        }

        /// <summary>
        /// Process an attr start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartAttr(string name, AttributeDict attributes)
        {
            if (this.curRow >= 0)
            {
                this.ProcessCharOrAttr(attributes, null, int.Parse(attributes[B3270.Attribute.Count]));
            }
        }

        /// <summary>
        /// Process a cursor start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartCursor(string name, AttributeDict attributes)
        {
            lock (this.screenImage)
            {
                this.screenImage.CursorEnabled = attributes[B3270.Attribute.Enabled].Equals(B3270.Value.True);
                if (this.screenImage.CursorEnabled)
                {
                    this.screenImage.CursorRow1 = int.Parse(attributes[B3270.Attribute.Row]);
                    this.screenImage.CursorColumn1 = int.Parse(attributes[B3270.Attribute.Column]);
                }
            }

            this.invoke.ScreenUpdate(ScreenUpdateType.Cursor);
        }

        /// <summary>
        /// Process a screen mode start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartScreenMode(string name, AttributeDict attributes)
        {
            lock (this.screenImage)
            {
                var maxRows = int.Parse(attributes[B3270.Attribute.Rows]);
                var maxColumns = int.Parse(attributes[B3270.Attribute.Columns]);

                // Propagate certain fields to the screen image immediately.
                this.screenImage.ColorMode = attributes[B3270.Attribute.Color].Equals(B3270.Value.True);
                this.screenImage.Model = int.Parse(attributes[B3270.Attribute.Model]);
                this.screenImage.Extended = attributes[B3270.Attribute.Extended].Equals(B3270.Value.True);
                this.screenImage.Resize(maxRows, maxColumns, attributes[B3270.Attribute.Oversize].Equals(B3270.Value.True));
            }

            // Tell interested parties that the screen mode changed.
            this.invoke.ScreenUpdate(ScreenUpdateType.ScreenMode);
        }

        /// <summary>
        /// Process a scroll start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartScroll(string name, AttributeDict attributes)
        {
            // Get the colors.
            HostColor? hostFg = null;
            HostColor? hostBg = null;
            if (attributes.TryGetValue(B3270.Attribute.Fg, out string attrValue))
            {
                hostFg = (HostColor)Enum.Parse(typeof(HostColor), attrValue, ignoreCase: true);
            }

            if (attributes.TryGetValue(B3270.Attribute.Bg, out attrValue))
            {
                hostBg = (HostColor)Enum.Parse(typeof(HostColor), attrValue, ignoreCase: true);
            }

            lock (this.screenImage)
            {
                for (var row = 0; row < this.screenImage.MaxRows - 1; row++)
                {
                    for (var column = 0; column < this.screenImage.MaxColumns; column++)
                    {
                        this.screenImage.Image[row, column] = this.screenImage.Image[row + 1, column];
                    }
                }

                for (var column = 0; column < this.screenImage.MaxColumns; column++)
                {
                    var cell = new Cell
                    {
                        Text = ' ',
                    };

                    // The following two statements slow scrolling to a crawl.
                    if (hostFg.HasValue)
                    {
                        cell.HostForeground = hostFg.Value;
                    }

                    if (hostBg.HasValue)
                    {
                        cell.HostBackground = hostBg.Value;
                    }

                    this.screenImage.Image[this.screenImage.MaxRows - 1, column] = cell;
                }
            }

            // Tell interested parties that the screen needs scrolling.
            this.invoke.ScreenUpdate(ScreenUpdateType.Scroll);
        }

        /// <summary>
        /// Process a setting start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartSetting(string name, AttributeDict attributes)
        {
            var settingName = attributes[B3270.Attribute.Name];
            lock (this.screenImage)
            {
                if (!attributes.TryGetValue(B3270.Attribute.Value, out string value))
                {
                    value = string.Empty;
                }

                this.screenImage.Settings.Add(settingName, value);
            }

            if (this.screenSettings.Contains(settingName))
            {
                this.invoke.ScreenUpdate(ScreenUpdateType.Repaint);
            }
        }

        /// <summary>
        /// Process a trace file start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartTraceFile(string name, AttributeDict attributes)
        {
            lock (this.screenImage)
            {
                if (!attributes.TryGetValue(B3270.Attribute.Name, out string traceFileName))
                {
                    traceFileName = null;
                }

                this.screenImage.TraceFile = traceFileName;
                this.invoke.ScreenUpdate(ScreenUpdateType.TraceFile);
            }
        }

        /// <summary>
        /// Process a thumb start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartThumb(string name, AttributeDict attributes)
        {
            lock (this.screenImage)
            {
                this.screenImage.Thumb.Top = float.Parse(attributes[B3270.Attribute.Top]);
                this.screenImage.Thumb.Shown = float.Parse(attributes[B3270.Attribute.Shown]);
                this.screenImage.Thumb.Saved = int.Parse(attributes[B3270.Attribute.Saved]);
                this.screenImage.Thumb.Screen = int.Parse(attributes[B3270.Attribute.Screen]);
                this.screenImage.Thumb.Back = int.Parse(attributes[B3270.Attribute.Back]);
                this.invoke.ScreenUpdate(ScreenUpdateType.Thumb);
            }
        }

        /// <summary>
        /// Process a flipped start.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attributes">Dictionary of attributes.</param>
        private void StartFlipped(string name, AttributeDict attributes)
        {
            lock (this.screenImage)
            {
                this.screenImage.Flipped = attributes[B3270.Attribute.Value].Equals(B3270.Value.True);
            }

            this.invoke.ScreenUpdate(ScreenUpdateType.Repaint);
        }

        /// <summary>
        /// Process a screen-related element end.
        /// </summary>
        /// <param name="name">Element name.</param>
        private void EndScreen(string name)
        {
            this.inScreen = false;
            this.curRow = -1;
            this.invoke.ScreenUpdate(ScreenUpdateType.Screen);
        }

        /// <summary>
        /// Process a screen-related element end.
        /// </summary>
        /// <param name="name">Element name.</param>
        private void EndRow(string name)
        {
            this.curRow = -1;
        }

        /// <summary>
        /// Process a chae or attr element.
        /// </summary>
        /// <param name="attributes">Element attributes.</param>
        /// <param name="text">Text to store, or null.</param>
        /// <param name="count">Number of cells to modify.</param>
        private void ProcessCharOrAttr(AttributeDict attributes, char[] text, int count)
        {
            var column = int.Parse(attributes[B3270.Attribute.Column]) - 1;
            HostColor? hostFg = null;
            HostColor? hostBg = null;
            GraphicRendition? gr = null;
            if (attributes.TryGetValue(B3270.Attribute.Fg, out string attrValue))
            {
                hostFg = (HostColor)Enum.Parse(typeof(HostColor), attrValue, ignoreCase: true);
            }

            if (attributes.TryGetValue(B3270.Attribute.Bg, out attrValue))
            {
                hostBg = (HostColor)Enum.Parse(typeof(HostColor), attrValue, ignoreCase: true);
            }

            if (attributes.TryGetValue(B3270.Attribute.Gr, out attrValue))
            {
                gr = ParseGr(attrValue);
            }

            var dbcs = gr.HasValue && gr.Value.HasFlag(GraphicRendition.Wide);
            if (dbcs)
            {
                // Wide characters. Double each entry.
                // We assume here that the emulator will never send us an 'attr' indication that changes a cell
                // from SBCS to DBCS. That is, if we get a 'wide' GR, it will either be redundant (the cell is already
                // marked that way), or it will be part of a 'char' indication that sets the text to DBCS characters.
                count *= 2;
            }

            lock (this.screenImage)
            {
                for (int i = 0; i < count; i++)
                {
                    if (text != null)
                    {
                        // DBCS characters are stored with the real character in the first location, and a 0 in the second.
                        // The right-hand side of a DBCS cell is the *only* time a 0 will appear in Text.
                        if (!dbcs || (i & 1) == 0)
                        {
                            this.screenImage.Image[this.curRow, column].Text = text[0];
                            text = text.Skip(1).ToArray();
                        }
                        else
                        {
                            this.screenImage.Image[this.curRow, column].Text = '\0';
                        }
                    }

                    if (hostFg.HasValue)
                    {
                        this.screenImage.Image[this.curRow, column].HostForeground = hostFg.Value;
                    }

                    if (hostBg.HasValue)
                    {
                        this.screenImage.Image[this.curRow, column].HostBackground = hostBg.Value;
                    }

                    if (gr.HasValue)
                    {
                        this.screenImage.Image[this.curRow, column].GraphicRendition = gr.Value;
                    }

                    column++;
                }
            }
        }
    }
}
