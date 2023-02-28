// <copyright file="SelectionManager.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
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
        /// Prefix for HTTP.
        /// </summary>
        private const string Http = "http://";

        /// <summary>
        /// Prefix for HTTPS.
        /// </summary>
        private const string Https = "https://";

        /// <summary>
        /// Fraction for half of a cell.
        /// </summary>
        private const double Half = 0.5;

        /// <summary>
        /// Fraction for the left third of a cell.
        /// </summary>
        private const double LeftThird = 1.0 / 3.0;

        /// <summary>
        /// Fraction for the right third of a cell.
        /// </summary>
        private const double RightThird = 2.0 / 3.0;

        /// <summary>
        /// Fraction for the left quarter of a cell.
        /// </summary>
        private const double LeftQuarter = 0.25;

        /// <summary>
        /// Fraction for the right quarter of a cell.
        /// </summary>
        private const double RightQuarter = 0.75;

        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(SelectionManager));

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
        /// Location in pixels where the first mouse click happened.
        /// </summary>
        private Point initialLocation;

        /// <summary>
        /// Row where the first mouse click happened.
        /// </summary>
        private int initialRow0;

        /// <summary>
        /// Column where the first mouse click happened.
        /// </summary>
        private int initialColumn0;

        /// <summary>
        /// Cursor row before we moved it.
        /// </summary>
        private int? cursorUnmoveRow1;

        /// <summary>
        /// Cursor column before we moved it.
        /// </summary>
        private int? cursorUnmoveColumn1;

        /// <summary>
        /// True if MouseUp should move the cursor.
        /// </summary>
        private bool canMove = false;

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
        }

        /// <summary>
        /// Gets the buffer address of the start of the selection.
        /// </summary>
        private int SelectStartBaddr0
        {
            get
            {
                var baddrAnchor = (this.selectAnchor.Row0 * this.app.ScreenImage.LogicalColumns) + this.selectAnchor.Column0;
                var baddrEnd = (this.selectEnd.Row0 * this.app.ScreenImage.LogicalColumns) + this.selectEnd.Column0;
                return (baddrAnchor < baddrEnd) ? baddrAnchor : baddrEnd;
            }
        }

        /// <summary>
        /// Gets the buffer address of the end of the selection.
        /// </summary>
        private int SelectEndBaddr0
        {
            get
            {
                var baddrAnchor = (this.selectAnchor.Row0 * this.app.ScreenImage.LogicalColumns) + this.selectAnchor.Column0;
                var baddrEnd = (this.selectEnd.Row0 * this.app.ScreenImage.LogicalColumns) + this.selectEnd.Column0;
                return (baddrAnchor > baddrEnd) ? baddrAnchor : baddrEnd;
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
            I18n.LocalizeGlobal(Title.UrlError, "URL Start Error");
        }

        /// <inheritdoc />
        public void Activated()
        {
            this.activateTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public void MouseDown(int row1, int column1, bool shift, Point location)
        {
            if ((DateTime.UtcNow - this.activateTime).TotalMilliseconds < ActivateClickMsec)
            {
                return;
            }

            this.mouseIsDown = true;

            if (!shift && this.Unselect())
            {
                this.canMove = false;
            }
            else
            {
                this.canMove = true;
            }

            var image = this.app.ScreenImage;
            var row0 = row1 - 1;
            var column0 = column1 - 1;
            if ((DateTime.UtcNow - this.clickTime).TotalMilliseconds <= SystemInformation.DoubleClickTime)
            {
                // Double click.
                // First, no triple clicks.
                this.clickTime = DateTime.MinValue;

                this.canMove = false;

                if (this.cursorUnmoveRow1.HasValue)
                {
                    // Undo a cursor move.
                    this.MoveTo(this.cursorUnmoveRow1.Value, this.cursorUnmoveColumn1.Value);
                    this.cursorUnmoveRow1 = null;
                    this.cursorUnmoveColumn1 = null;
                }

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
                    }

                    return;
                }

                var leftColumn0 = column0;
                var rightColumn0 = column0;
                var leftRow0 = row0;
                var rightRow0 = row0;

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
                    if (image.Image[leftRow0, leftColumn0].Text != ' ')
                    {
                        // SBCS. Scan left.
                        while (leftColumn0 >= 0 && image.Image[leftRow0, leftColumn0].Text != ' ')
                        {
                            leftColumn0--;
                        }

                        while (leftRow0 > 0 &&
                            leftColumn0 == -1 &&
                            image.Image[leftRow0, 0].Text != ' ' &&
                            image.Image[leftRow0 - 1, image.LogicalColumns - 1].GraphicRendition.HasFlag(GraphicRendition.Wrap))
                        {
                            leftRow0--;
                            leftColumn0 = image.LogicalColumns - 1;

                            while (leftColumn0 >= 0 && image.Image[leftRow0, leftColumn0].Text != ' ')
                            {
                                leftColumn0--;
                            }
                        }

                        leftColumn0++;

                        // Scan right.
                        while (rightColumn0 < image.LogicalColumns && image.Image[rightRow0, rightColumn0].Text != ' ')
                        {
                            rightColumn0++;
                        }

                        while (rightRow0 < image.LogicalRows &&
                            rightColumn0 == image.LogicalColumns &&
                            image.Image[rightRow0, rightColumn0 - 1].Text != ' ' &&
                            image.Image[rightRow0, rightColumn0 - 1].GraphicRendition.HasFlag(GraphicRendition.Wrap))
                        {
                            rightRow0++;
                            rightColumn0 = 0;
                            while (rightColumn0 < image.LogicalColumns && image.Image[rightRow0, rightColumn0].Text != ' ')
                            {
                                rightColumn0++;
                            }
                        }

                        rightColumn0--;
                    }
                }

                // Check for a URL.
                if (!shift)
                {
                    var selection = string.Empty;
                    for (var baddr = (leftRow0 * this.app.ScreenImage.LogicalColumns) + leftColumn0;
                        baddr <= (rightRow0 * this.app.ScreenImage.LogicalColumns) + rightColumn0;
                        baddr++)
                    {
                        selection += image.Image[baddr / this.app.ScreenImage.LogicalColumns, baddr % this.app.ScreenImage.LogicalColumns].Text;
                    }

                    int index;
                    if ((index = selection.IndexOf(Http)) >= 0 || ((index = selection.IndexOf(Https)) >= 0))
                    {
                        selection = selection.Substring(index);
                        while (selection.Length > 0 && !Uri.IsWellFormedUriString(selection, UriKind.Absolute))
                        {
                            selection = selection.Substring(0, selection.Length - 1);
                        }

                        if (selection.Length > 0)
                        {
                            try
                            {
                                Trace.Line(Trace.Type.Clipboard, "Starting URL {0}", selection);
                                Process.Start(selection);
                            }
                            catch (Exception e)
                            {
                                ErrorBox.Show(e.Message, I18n.Get(Title.UrlError));
                            }

                            return;
                        }
                    }
                }

                // Select the word.
                this.selectAnchor = new Corner(leftRow0, leftColumn0);
                this.selectEnd = new Corner(rightRow0, rightColumn0);
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
                this.canMove = false;
            }
            else
            {
                this.initialLocation = location;
                this.initialRow0 = row0;
                this.initialColumn0 = column0;
            }

            this.clickTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public void MouseMove(ICell cell, int row1, int column1, Point location)
        {
            var row0 = row1 - 1;
            var column0 = column1 - 1;

            if (this.clickTime == DateTime.MinValue)
            {
                // Double click: ignore mouse movement.
                return;
            }

            if (!this.mouseIsDown)
            {
                // Should not happen.
                return;
            }

            if (this.selectAnchor == null)
            {
                do
                {
                    // Initial movement.
                    int? anchorRow0 = null;
                    int? anchorColumn0 = null;
                    int? endColumn0 = null;

                    if (location.X < this.initialLocation.X)
                    {
                        // Movement to the left.
                        if (cell.HorizontalFraction(this.initialLocation) >= LeftThird)
                        {
                            if (column0 < this.initialColumn0 || cell.HorizontalFraction(location) <= LeftQuarter)
                            {
                                // Moved enough to set the anchor to the initial cell.
                                anchorColumn0 = this.initialColumn0;
                            }
                        }
                        else if (column0 < this.initialColumn0 && cell.HorizontalFraction(location) <= Half)
                        {
                            // Moved enough to set the anchor to the cell to the left of the initial cell.
                            anchorColumn0 = this.initialColumn0 - 1;
                        }

                        if (anchorColumn0 != null)
                        {
                            // If we're in the same column as we started, that's the end column.
                            // Otherwise, if we're in the left half of a different column, that's the end column.
                            // Otherwise, the end column is the one to the right of where we are now.
                            endColumn0 = (column0 == this.initialColumn0) ? column0 :
                                (cell.HorizontalFraction(location) <= 0.5 ? column0 : column0 + 1);
                        }
                    }
                    else if (location.X > this.initialLocation.X)
                    {
                        // Movement to the right.
                        if (cell.HorizontalFraction(this.initialLocation) <= RightThird)
                        {
                            if (column0 > this.initialColumn0 || cell.HorizontalFraction(location) >= RightQuarter)
                            {
                                // Moved enough to set the anchor to the initial cell.
                                anchorColumn0 = this.initialColumn0;
                            }
                        }
                        else if (column0 > this.initialColumn0 && cell.HorizontalFraction(location) > Half)
                        {
                            // Moved enough to set the anchor to the cell to the right of the initial cell.
                            anchorColumn0 = this.initialColumn0 + 1;
                        }

                        if (anchorColumn0 != null)
                        {
                            // If we're in the same column as we started, that's the end column.
                            // Otherwise, if we're in the right half of a different column, that's the end column.
                            // Otherwise, the end column is the one to the left of where we are now.
                            endColumn0 = (column0 == this.initialColumn0) ? column0 :
                                (cell.HorizontalFraction(location) >= Half ? column0 : column0 - 1);
                        }
                    }

                    if (((row0 > this.initialRow0) && cell.VerticalFraction(location) >= Half) ||
                        ((row0 < this.initialRow0) && cell.VerticalFraction(location) <= Half))
                    {
                        // Moved up or down by a row.
                        anchorRow0 = this.initialRow0;
                    }

                    if (anchorColumn0 == null && anchorRow0 == null)
                    {
                        // Not enough movement in either dimension.
                        // break;
                        return;
                    }

                    if (anchorRow0 == null)
                    {
                        // Insufficient vertical movement.
                        anchorRow0 = this.initialRow0;
                    }

                    if (anchorColumn0 == null)
                    {
                        // Insufficient horizontal movement, but sufficient vertical movement.
                        anchorColumn0 = endColumn0 = this.initialColumn0;
                    }

                    this.selectAnchor = new Corner(anchorRow0.Value, anchorColumn0.Value);
                    this.selectEnd = new Corner(row0, endColumn0.Value);
                    this.Reselect();
                }
                while (false);
            }
            else
            {
                // Extend the selection if we've crossed the middle of a new cell.
                // Reduce the selection if we've crossed into a new cell anywhere.
                if ((row0 > this.selectEnd.Row0 && cell.VerticalFraction(location) >= Half) ||
                    row0 < this.selectEnd.Row0 ||
                    (column0 > this.selectEnd.Column0 && cell.HorizontalFraction(location) >= Half) ||
                    column0 < this.selectEnd.Column0)
                {
                    this.selectEnd = new Corner(row0, column0);
                    this.Reselect();
                }
            }

            this.canMove = false;
        }

        /// <inheritdoc />
        public void MouseUp(int row, int column)
        {
            // Save the cursor location and move it.
            if (this.canMove)
            {
                this.canMove = false;
                this.cursorUnmoveRow1 = this.app.ScreenImage.CursorRow1;
                this.cursorUnmoveColumn1 = this.app.ScreenImage.CursorColumn1;
                this.MoveTo(this.initialRow0 + 1, this.initialColumn0 + 1);
            }

            this.mouseIsDown = false;
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.Unselect();
            this.mouseIsDown = false;
        }

        /// <summary>
        /// Unselect everything.
        /// </summary>
        /// <returns>True if anything was selected.</returns>
        private bool Unselect()
        {
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
            // Whenever we change the selection state, we can forget about bouncing the cursor back
            // from a double-click.
            this.cursorUnmoveRow1 = null;
            this.cursorUnmoveColumn1 = null;

            if (this.app.ScreenImage.SelectState == SelectState.LastNvt)
            {
                // Do an NVT-mode continuous select, passing the start and end buffer addresses.
                this.app.SetSelectNvt(this.SelectStartBaddr0, this.SelectEndBaddr0);
            }
            else
            {
                // Do a 3270-mode rectangular select, passing the upper-left corner, number of rows and number of columns.
                this.app.SetSelect3270(
                    Math.Min(this.selectAnchor.Row0, this.selectEnd.Row0),
                    Math.Min(this.selectAnchor.Column0, this.selectEnd.Column0),
                    Math.Abs(this.selectAnchor.Row0 - this.selectEnd.Row0) + 1,
                    Math.Abs(this.selectAnchor.Column0 - this.selectEnd.Column0) + 1);
            }
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

                if (this.app.SelectState == SelectState.LastNvt &&
                    row != image.LogicalRows - 1 &&
                    image.Image[row, image.LogicalColumns - 1].GraphicRendition.HasFlag(GraphicRendition.Wrap))
                {
                    // Continuous selections with a line wrap. Just continue.
                    continue;
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
                return this.UiCopyCut(arguments, Constants.Action.Copy, out _, out result);
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
            if ((copyResult = this.UiCopyCut(arguments, Constants.Action.Cut, out bool anySelected, out result)) == PassthruResult.Failure || !anySelected)
            {
                return copyResult;
            }

            if (this.app.ScreenImage.SelectState != SelectState.Last3270)
            {
                // Clearing the screen is possible (and useful) only in 3270 mode.
                this.app.UnselectAll();
                return PassthruResult.Success;
            }

            // Clear the selected region.
            var action = new BackEndAction(
                B3270.Action.ClearRegion,
                Math.Min(this.selectAnchor.Row0, this.selectEnd.Row0) + 1,
                Math.Min(this.selectAnchor.Column0, this.selectEnd.Column0) + 1,
                Math.Abs(this.selectAnchor.Row0 - this.selectEnd.Row0) + 1,
                Math.Abs(this.selectAnchor.Column0 - this.selectEnd.Column0) + 1);
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

            /// <summary>
            /// URL error.
            /// </summary>
            public static readonly string UrlError = I18n.Combine(TitleName, "urlError");
        }
    }
}
