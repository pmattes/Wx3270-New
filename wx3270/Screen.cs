// <copyright file="Screen.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Wx3270.Contracts;

    /// <summary>
    /// Screen image processing.
    /// </summary>
    public class Screen : BackEndEvent
    {
        /// <summary>
        /// The minimum number of pending scrolls to allow.
        /// </summary>
        private const int MinScroll = 1;

        /// <summary>
        /// The maximum number of pending scrolls to allow.
        /// </summary>
        private const int MaxScroll = 500;

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
        /// The number of scroll operations pending.
        /// </summary>
        private int scrollsPending;

        /// <summary>
        /// The lock for <see cref="scrollsPending"/>.
        /// </summary>
        private object scrollLock = new object();

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
                new BackEndEventDef(B3270.Indication.Connection, this.StartConnection),
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
        /// Set a region to be selected, and clear everything else. NVT mode.
        /// </summary>
        /// <param name="startBaddr">Start buffer address.</param>
        /// <param name="endBaddr">End buffer address, inclusive.</param>
        public void SetSelectNvt(int startBaddr, int endBaddr)
        {
            if (this.screenImage.SetSelectNvt(startBaddr, endBaddr))
            {
                this.invoke.ScreenUpdate(ScreenUpdateType.Screen, new UpdateState(new ScreenImage(this.screenImage)));
            }
        }

        /// <summary>
        /// Set a region to be selected, and clear everything else. 3270 mode.
        /// </summary>
        /// <param name="startRow0">Starting row, 0-origin.</param>
        /// <param name="startColumn0">Starting column, 0-origin.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        public void SetSelect3270(int startRow0, int startColumn0, int rows, int columns)
        {
            if (this.screenImage.SetSelect3270(startRow0, startColumn0, rows, columns))
            {
                this.invoke.ScreenUpdate(ScreenUpdateType.Screen, new UpdateState(new ScreenImage(this.screenImage)));
            }
        }

        /// <summary>
        /// Unselect the entire screen.
        /// </summary>
        /// <returns>True if anything was selected.</returns>
        public bool UnselectAll()
        {
            var changed = this.screenImage.UnselectAll();
            if (changed)
            {
                this.invoke.ScreenUpdate(ScreenUpdateType.Screen, new UpdateState(new ScreenImage(this.screenImage)));
            }

            return changed;
        }

        /// <summary>
        /// A draw operation is complete. If there are pending scrolls, let them go now.
        /// </summary>
        public void DrawComplete()
        {
            var needInvoke = false;
            var backedUp = 0;
            lock (this.scrollLock)
            {
                if (this.scrollsPending != 0)
                {
                    backedUp = this.scrollsPending;
                    this.scrollsPending = 0;
                    needInvoke = true;
                }
            }

            if (needInvoke)
            {
                Trace.Line(Trace.Type.Draw, $"Invoking {backedUp} screen draw operations");

                // Run the screen update asynchronously.
                Task.Run(() => this.invoke.ScreenUpdate(ScreenUpdateType.Screen, new UpdateState(new ScreenImage(this.screenImage))));
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

            this.invoke.ScreenUpdate(ScreenUpdateType.Repaint, new UpdateState(new ScreenImage(this.screenImage)));
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
            this.invoke.ScreenUpdate(ScreenUpdateType.ScreenMode, new UpdateState(new ScreenImage(this.screenImage)));
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

            // If there is already a scroll pending, just remember it.
            lock (this.scrollLock)
            {
                if (++this.scrollsPending > MinScroll)
                {
                    if (this.scrollsPending >= MaxScroll)
                    {
                        this.scrollsPending = 1;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // Tell interested parties that the screen needs scrolling.
            this.invoke.ScreenUpdate(ScreenUpdateType.Scroll, new UpdateState(new ScreenImage(this.screenImage)));
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
                this.invoke.ScreenUpdate(ScreenUpdateType.Repaint, new UpdateState(new ScreenImage(this.screenImage)));
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
                this.invoke.ScreenUpdate(ScreenUpdateType.TraceFile, new UpdateState(new ScreenImage(this.screenImage)));
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
                this.screenImage.Thumb.Top = float.Parse(attributes[B3270.Attribute.Top], CultureInfo.InvariantCulture.NumberFormat);
                this.screenImage.Thumb.Shown = float.Parse(attributes[B3270.Attribute.Shown], CultureInfo.InvariantCulture.NumberFormat);
                this.screenImage.Thumb.Saved = int.Parse(attributes[B3270.Attribute.Saved]);
                this.screenImage.Thumb.Screen = int.Parse(attributes[B3270.Attribute.Screen]);
                this.screenImage.Thumb.Back = int.Parse(attributes[B3270.Attribute.Back]);
                this.invoke.ScreenUpdate(ScreenUpdateType.Thumb, new UpdateState(new ScreenImage(this.screenImage)));
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

            this.invoke.ScreenUpdate(ScreenUpdateType.Repaint, new UpdateState(new ScreenImage(this.screenImage)));
        }

        /// <summary>
        /// Process a connection state indication from the emulator.
        /// </summary>
        /// <param name="name">Element name.</param>
        /// <param name="attrs">Element attributes.</param>
        private void StartConnection(string name, AttributeDict attrs)
        {
            // Set the screen selection state based on the new connection state.
            var state = OiaState.ParseConnectionState(attrs[B3270.Attribute.State]);
            switch (state)
            {
                case ConnectionState.Connected3270:
                case ConnectionState.ConnectedTn3270e:
                case ConnectionState.ConnectedEsscp:
                    this.screenImage.SelectState = SelectState.Last3270;
                    break;
                case ConnectionState.ConnectedNvt:
                case ConnectionState.ConnectedNvtCharmode:
                case ConnectionState.ConnectedEnvt:
                    this.screenImage.SelectState = SelectState.LastNvt;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Process a screen-related element end.
        /// </summary>
        /// <param name="name">Element name.</param>
        private void EndScreen(string name)
        {
            this.inScreen = false;
            this.curRow = -1;

            lock (this.scrollLock)
            {
                // If there is a scroll pending, do nothing until it completes.
                if (this.scrollsPending != 0)
                {
                    return;
                }
            }

            this.invoke.ScreenUpdate(ScreenUpdateType.Screen, new UpdateState(new ScreenImage(this.screenImage)));
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
        /// Process a char or attr element.
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
                            var t = text[0];
                            if (t >= 0xd800 && t <= 0xdb7f)
                            {
                                // High surrogate. We don't know what to do with this at the moment, so we substitute a blob.
                                t = '▧';
                            }
                            else if (t >= 0xdc00 && t <= 0xdfff)
                            {
                                // Low surrogate. Second half of something we don't know what to do with.
                                text = text.Skip(1).ToArray();
                                continue;
                            }

                            this.screenImage.Image[this.curRow, column].Text = t;
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
