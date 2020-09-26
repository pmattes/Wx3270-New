// <copyright file="SelectionManager.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Selection manager.
    /// </summary>
    public class SelectionManager : ISelectionManager
    {
        /// <summary>
        /// The threshold for interpreting mouse down as just activate.
        /// </summary>
        private const int ActivateClickMsec = 250;

        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.TitleName(nameof(SelectionManager));

        /// <summary>
        /// The application context.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// The starting corner of the selection.
        /// </summary>
        private Corner selectAnchor;

        /// <summary>
        /// The other corner of the selection.
        /// </summary>
        private Corner selectEnd;

        /// <summary>
        /// The time of the first click.
        /// </summary>
        private DateTime clickTime = DateTime.MinValue;

        /// <summary>
        /// The time of the last form activation.
        /// </summary>
        private DateTime activateTime = DateTime.MinValue;

        /// <summary>
        /// True if the mouse is down.
        /// </summary>
        private bool mouseIsDown = false;

        /// <summary>
        /// The mouse-up timer.
        /// </summary>
        private Timer mouseUpTimer = new Timer();

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionManager"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        public SelectionManager(Wx3270App app)
        {
            this.app = app;

            // Register selection-related actions.
            app.BackEnd.RegisterPassthru(Constants.Action.KeyboardSelect, this.UiKeyboardSelect);
            app.BackEnd.RegisterPassthru(Constants.Action.Copy, this.UiCopy);
            app.BackEnd.RegisterPassthru(Constants.Action.Cut, this.UiCut);
            app.BackEnd.RegisterPassthru(Constants.Action.Paste, this.UiPaste);

            // Register Ready actions.
            app.BackEnd.OnReady += () =>
            {
                app.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.Set, B3270.Setting.OverlayPaste, B3270.ToggleAction.Set),
                    ErrorBox.Completion(I18n.Get(Title.Initialization)));
            };

            // Get the mouse-up timer ready.
            this.mouseUpTimer.Interval = SystemInformation.DoubleClickTime;
            this.mouseUpTimer.Tick += this.MouseUpTick;
        }

        /// <summary>
        /// Gets the starting row of the selection, 0-origin.
        /// </summary>
        private int SelectStartRow0
        {
            get
            {
                return Math.Min(this.selectAnchor.Row0, this.selectEnd.Row0);
            }
        }

        /// <summary>
        /// Gets the ending row of the selection, 0-origin.
        /// </summary>
        private int SelectEndRow0
        {
            get
            {
                return Math.Max(this.selectAnchor.Row0, this.selectEnd.Row0);
            }
        }

        /// <summary>
        /// Gets the starting column of the selection, 0-origin.
        /// </summary>
        private int SelectStartColumn0
        {
            get
            {
                return Math.Min(this.selectAnchor.Column0, this.selectEnd.Column0);
            }
        }

        /// <summary>
        /// Gets the ending column of the selection, 0-origin.
        /// </summary>
        private int SelectEndColumn0
        {
            get
            {
                return Math.Max(this.selectAnchor.Column0, this.selectEnd.Column0);
            }
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.Copy, "Clipboard Copy");
            I18n.LocalizeGlobal(Title.Cut, "Clipboard Cut");
            I18n.LocalizeGlobal(Title.Paste, "Clipboard Paste");
            I18n.LocalizeGlobal(Title.CursorMove, "Cursor Move");
            I18n.LocalizeGlobal(Title.Initialization, "Selection Manager Initialization");
        }

        /// <inheritdoc />
        public void Activated()
        {
            this.activateTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public void MouseDown(int row1, int column1, bool shift)
        {
            if ((DateTime.UtcNow - this.activateTime).TotalMilliseconds < ActivateClickMsec)
            {
                return;
            }

            this.mouseIsDown = true;

            if (!shift)
            {
                this.Unselect();
            }

            var image = this.app.ScreenImage;
            var row0 = row1 - 1;
            var column0 = column1 - 1;
            if ((DateTime.UtcNow - this.clickTime).TotalMilliseconds <= SystemInformation.DoubleClickTime)
            {
                // Double click.
                // First, no triple clicks.
                this.clickTime = DateTime.MinValue;

                this.mouseUpTimer.Stop();

                if (shift)
                {
                    // Extend the selection first.
                    if (this.selectAnchor == null)
                    {
                        // No anchor yet, use the current cursor position.
                        this.selectAnchor = new Corner(image.CursorRow1 - 1, image.CursorColumn1 - 1);
                        this.selectEnd = new Corner(row0, column0);
                    }

                    if (image.Image[row0, column0].Text == ' ')
                    {
                        // No visible text in this spot.
                        this.Reselect();
                        return;
                    }
                }

                var leftColumn0 = column0;
                var rightColumn0 = column0;

                if (image.Image[row0, column0].IsDbcsLeft)
                {
                    // Left side of DBCS.
                    rightColumn0++;
                }
                else if (image.Image[row0, column0].IsDbcsRight)
                {
                    // Right side of DBCS.
                    leftColumn0--;
                }
                else
                {
                    // SBCS. Go left until we find the left margin or a space.
                    while (leftColumn0 >= 0 && image.Image[row0, leftColumn0].Text != ' ')
                    {
                        leftColumn0--;
                    }

                    leftColumn0++;

                    // Go right.
                    while (rightColumn0 < image.LogicalColumns && image.Image[row0, rightColumn0].Text != ' ')
                    {
                        rightColumn0++;
                    }

                    rightColumn0--;
                }

                if (this.selectAnchor == null)
                {
                    this.selectAnchor = new Corner(row0, leftColumn0);
                }

                if (column1 >= this.selectAnchor.Column0)
                {
                    // Extending to the right.
                    this.selectEnd = new Corner(row0, rightColumn0);
                }
                else
                {
                    // Extending to the left.
                    this.selectEnd = new Corner(row0, leftColumn0);
                }

                this.Reselect();
                return;
            }

            // Single click.
            if (shift)
            {
                if (this.selectAnchor == null)
                {
                    this.selectAnchor = new Corner(image.CursorRow1 - 1, image.CursorColumn1 - 1);
                }

                this.selectEnd = new Corner(row0, column0);
                this.Reselect();
            }
            else
            {
                this.selectAnchor = new Corner(row0, column0);
                this.mouseUpTimer.Start();
            }

            this.clickTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public void MouseMove(int row, int column)
        {
            if (this.mouseIsDown)
            {
                this.selectEnd = new Corner(row - 1, column - 1);
                this.Reselect();
                this.mouseUpTimer.Stop();
            }
        }

        /// <inheritdoc />
        public void MouseUp(int row, int column)
        {
            this.mouseIsDown = false;
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.Unselect();
            this.mouseIsDown = false;
        }

        /// <summary>
        /// Mouse-up timer tick.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MouseUpTick(object sender, EventArgs e)
        {
            // The mouse-up timer expired without the mouse moving or clicking again.
            // This is a cursor move.
            this.mouseUpTimer.Stop();
            this.MoveTo(this.selectAnchor.Row0 + 1, this.selectAnchor.Column0 + 1);
        }

        /// <summary>
        /// Unselect everything.
        /// </summary>
        /// <returns>True if anything was selected.</returns>
        private bool Unselect()
        {
            this.mouseUpTimer.Stop();
            var changed = this.app.UnselectAll();
            this.selectAnchor = null;
            this.selectEnd = null;
            return changed;
        }

        /// <summary>
        /// Move the cursor.
        /// </summary>
        /// <param name="row1">New row (1-origin).</param>
        /// <param name="column1">New column (1-origin).</param>
        private void MoveTo(int row1, int column1)
        {
            if (Oia.StateIs3270orSscp(this.app.ConnectionState))
            {
                this.app.BackEnd.RunAction(
                    new BackEndAction(B3270.Action.MoveCursor1, row1, column1),
                    ErrorBox.Completion(I18n.Get(Title.CursorMove)));
            }
        }

        /// <summary>
        /// Sync the screen with the anchor and end.
        /// </summary>
        private void Reselect()
        {
            this.app.SetSelect(
                this.SelectStartRow0,
                this.SelectStartColumn0,
                this.SelectEndRow0 - this.SelectStartRow0 + 1,
                this.SelectEndColumn0 - this.SelectStartColumn0 + 1);
        }

        /// <summary>
        /// UI pass-through keyboard selection action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Result string.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiKeyboardSelect(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = string.Empty;

            // Parse the argument.
            var argsArray = arguments.ToArray();
            if (argsArray.Length != 1)
            {
                result = Constants.Action.KeyboardSelect + " takes 1 argument";
                return PassthruResult.Failure;
            }

            var incRow = 0;
            var incColumn = 0;
            var image = this.app.ScreenImage;
            var cursorCell = image.Image[image.CursorRow0, image.CursorColumn0];

            // When flipped, switch left and right.
            if (image.Flipped)
            {
                if (argsArray[0].Equals(KeyDirection.Left, StringComparison.InvariantCultureIgnoreCase))
                {
                    argsArray[0] = KeyDirection.Right;
                }
                else if (argsArray[0].Equals(KeyDirection.Right, StringComparison.InvariantCultureIgnoreCase))
                {
                    argsArray[0] = KeyDirection.Left;
                }
            }

            switch (argsArray[0].ToLowerInvariant())
            {
                case KeyDirection.Up:
                    incRow = -1;
                    break;
                case KeyDirection.Down:
                    incRow = 1;
                    break;
                case KeyDirection.Left:
                    incColumn = cursorCell.IsDbcsRight ? -2 : -1;
                    break;
                case KeyDirection.Right:
                    incColumn = cursorCell.IsDbcsLeft ? 2 : 1;
                    break;
                default:
                    result = Constants.Action.KeyboardSelect + ": argument must be Up, Down, Left or Right";
                    return PassthruResult.Failure;
            }

            // Create a select anchor if there isn't one yet.
            if (this.selectAnchor == null)
            {
                this.selectAnchor = new Corner(image.CursorRow0, image.CursorColumn0);
            }

            // Compute the new selection area.
            var currentEnd = this.selectEnd ?? this.selectAnchor;
            Corner newEnd;
            if (currentEnd.Row0 + incRow < 0
                || currentEnd.Row0 + incRow >= image.LogicalColumns
                || currentEnd.Column0 + incColumn < 0
                || currentEnd.Column0 + incColumn >= image.LogicalColumns)
            {
                // Ran off the edge of the screen, stop there.
                newEnd = currentEnd;
            }
            else
            {
                // Move one cell over.
                newEnd = new Corner(currentEnd.Row0 + incRow, currentEnd.Column0 + incColumn);
            }

            // Change the selection area.
            this.selectEnd = newEnd;
            this.Reselect();

            // Move the cursor, too.

            // Success.
            return PassthruResult.Success;
        }

        /// <summary>
        /// Common 'copy' logic for the UI pass-through copy and cut actions.
        /// </summary>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="actionName">Name of the action, for error messages.</param>
        /// <param name="anySelected">Returned true if there is a selection.</param>
        /// <param name="result">Result string.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiCopyCut(IEnumerable<string> arguments, string actionName, out bool anySelected, out string result)
        {
            result = string.Empty;
            anySelected = false;

            // Validate the arguments.
            if (arguments.Any())
            {
                result = actionName + " takes 0 arguments";
                return PassthruResult.Failure;
            }

            // Get the selection.
            var selection = new List<string>();
            string selectLine = string.Empty;
            var image = this.app.ScreenImage;
            for (var row = 0; row < image.LogicalRows; row++)
            {
                for (var column = 0; column < image.LogicalColumns; column++)
                {
                    var i = image.Image[row, column];
                    if (!i.GraphicRendition.HasFlag(GraphicRendition.Selected) || i.GraphicRendition.HasFlag(GraphicRendition.NoCopy))
                    {
                        continue;
                    }

                    if (i.GraphicRendition.HasFlag(GraphicRendition.PrivateUse))
                    {
                        selectLine += ((char)((int)i.Text + B3270.PuaBase)).ToString();
                        continue;
                    }

                    if (i.GraphicRendition.HasFlag(GraphicRendition.Order))
                    {
                        selectLine += " ";
                        continue;
                    }

                    if (!i.IsDbcsRight)
                    {
                        // Orindary selected text. This includes the left side of DBCS characters.
                        selectLine += i.Text;
                    }
                    else if (!image.Image[row, column - 1].GraphicRendition.HasFlag(GraphicRendition.Selected))
                    {
                        // For the right side of a DBCS cell that is selected, and whose left side isn't, include
                        // the left side.
                        selectLine += image.Image[row, column - 1].Text;
                    }
                }

                if (!string.IsNullOrEmpty(selectLine))
                {
                    if (this.app.SelectState == SelectState.LastNvt)
                    {
                        // In NVT select mode, strip trailing blanks from the end of each line.
                        selectLine = selectLine.TrimEnd(new[] { ' ', '\u4040' });
                    }

                    selection.Add(selectLine);
                    selectLine = string.Empty;
                }
            }

            if (selection.Count() == 0)
            {
                return PassthruResult.Success;
            }

            anySelected = true;

            // Copy to the clipboard.
            var dataObject = new DataObject();
            dataObject.SetText(string.Join(Environment.NewLine, selection), TextDataFormat.UnicodeText);
            dataObject.SetText(string.Join(Environment.NewLine, selection), TextDataFormat.Text);
            try
            {
                this.app.Invoke(new MethodInvoker(() =>
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(dataObject, true);
                }));
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.Copy));
            }

            return PassthruResult.Success;
        }

        /// <summary>
        /// UI pass-through clipboard copy action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Result string.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiCopy(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            try
            {
                bool anySelected;
                return this.UiCopyCut(arguments, Constants.Action.Copy, out anySelected, out result);
            }
            finally
            {
                // Clear the selection. This would likely happen automatically because a keymap macro which called this action
                // did not call uKeyboardSelect(), but it is also possible to call uCopy()from some other context.
                this.app.UnselectAll();
            }
        }

        /// <summary>
        /// UI pass-through clipboard cut action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Result string.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiCut(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            // Do the 'copy' part, which might fail or produce nothing.
            PassthruResult copyResult;
            bool anySelected;
            if ((copyResult = this.UiCopyCut(arguments, Constants.Action.Cut, out anySelected, out result)) == PassthruResult.Failure || !anySelected)
            {
                return copyResult;
            }

            // Clear the selected region.
            var action = new BackEndAction(
                B3270.Action.ClearRegion,
                this.SelectStartRow0 + 1,
                this.SelectStartColumn0 + 1,
                this.SelectEndRow0 - this.SelectStartRow0 + 1,
                this.SelectEndColumn0 - this.SelectStartColumn0 + 1);
            this.app.BackEnd.RunAction(action, ErrorBox.Completion(I18n.Get(Title.Cut)));

            // Clear the selection. This would likely happen automatically because a keymap macro which called this action
            // did not call uKeyboardSelect(), but it is also possible to call uCut()from some other context.
            this.app.UnselectAll();
            return PassthruResult.Success;
        }

        /// <summary>
        /// UI pass-through clipboard paste action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Result string.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiPaste(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            result = string.Empty;

            // Validate the arguments.
            if (arguments.Any())
            {
                result = Constants.Action.Paste + " takes 0 arguments";
                return PassthruResult.Failure;
            }

            // Get the selection.
            IDataObject dataObject = null;
            this.app.Invoke(new MethodInvoker(() => { dataObject = Clipboard.GetDataObject(); }));
            if (dataObject == null)
            {
                Trace.Line(Trace.Type.Clipboard, "Nothing on clipboard");
                return PassthruResult.Success;
            }

            string data;
            if (dataObject.GetDataPresent(DataFormats.UnicodeText, true))
            {
                data = (string)dataObject.GetData(DataFormats.UnicodeText, true);
            }
            else if (dataObject.GetDataPresent(DataFormats.Text, true))
            {
                data = (string)dataObject.GetData(DataFormats.Text, true);
            }
            else
            {
                result = "No text on clipboard";
                return PassthruResult.Failure;
            }

            Trace.Line(Trace.Type.Clipboard, "Paste: got {0}", data);
            var action = new BackEndAction(B3270.Action.PasteString, "0x" + string.Join(string.Empty, Encoding.UTF8.GetBytes(data).Select(b => b.ToString("x2"))));
            this.app.BackEnd.RunAction(action, ErrorBox.Completion(I18n.Get(Title.Paste)));
            return PassthruResult.Success;
        }

        /// <summary>
        /// Convenience class to hold a corner of the selection area.
        /// </summary>
        private class Corner : Tuple<int, int>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Corner"/> class.
            /// </summary>
            /// <param name="row0">Row coordinate, 0-origin.</param>
            /// <param name="column0">Column coordinate, 0-origin.</param>
            public Corner(int row0, int column0)
                : base(row0, column0)
            {
            }

            /// <summary>
            /// Gets the row coordinate, 0-origin.
            /// </summary>
            public int Row0
            {
                get { return this.Item1; }
            }

            /// <summary>
            /// Gets the column coordinate, 0-origin.
            /// </summary>
            public int Column0
            {
                get { return this.Item2; }
            }
        }

        /// <summary>
        /// Directions for key-based selections.
        /// </summary>
        private class KeyDirection
        {
            /// <summary>
            /// Extend up.
            /// </summary>
            public const string Up = "up";

            /// <summary>
            /// Extend down.
            /// </summary>
            public const string Down = "down";

            /// <summary>
            /// Extend left.
            /// </summary>
            public const string Left = "left";

            /// <summary>
            /// Extend right.
            /// </summary>
            public const string Right = "right";
        }

        /// <summary>
        /// Localized titles of message boxes.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Copy operation.
            /// </summary>
            public static readonly string Copy = I18n.Combine(TitleName, "copy");

            /// <summary>
            /// Cut operation.
            /// </summary>
            public static readonly string Cut = I18n.Combine(TitleName, "cut");

            /// <summary>
            /// Paste operation.
            /// </summary>
            public static readonly string Paste = I18n.Combine(TitleName, "paste");

            /// <summary>
            /// Cursor move operation.
            /// </summary>
            public static readonly string CursorMove = I18n.Combine(TitleName, "cursorMove");

            /// <summary>
            /// Initialization.
            /// </summary>
            public static readonly string Initialization = I18n.Combine(TitleName, "initialization");
        }
    }
}
