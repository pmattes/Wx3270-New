// <copyright file="Tour.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;
    using I18nBase;
    using Microsoft.Win32;
    using static Wx3270.Settings;

    /// <summary>
    /// Tour orientation (where the object is relative to the text).
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// Object is above the text and to the left.
        /// </summary>
        UpperLeft,

        /// <summary>
        /// Same as <see cref="UpperLeft"/>, but pointing to the left side.
        /// </summary>
        UpperLeftTight,

        /// <summary>
        /// Object is above the text and to the right.
        /// </summary>
        UpperRight,

        /// <summary>
        /// Object is below the text and to the left.
        /// </summary>
        LowerLeft,

        /// <summary>
        /// Same as <see cref="LowerLeft"/>, but pointing to the left side.
        /// </summary>
        LowerLeftTight,

        /// <summary>
        /// Object is below the text and to the right.
        /// </summary>
        LowerRight,

        /// <summary>
        /// Object is centered behind the text and there is no arrow.
        /// </summary>
        Centered,
    }

    /// <summary>
    /// The direction that the user wants to go.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// No direction.
        /// </summary>
        None,

        /// <summary>
        /// To the left (forward).
        /// </summary>
        Left,

        /// <summary>
        /// To the right (backward).
        /// </summary>
        Right,
    }

    /// <summary>
    /// Helpful tour window.
    /// </summary>
    public partial class Tour : Form
    {
        /// <summary>
        /// The name of the Tour section.
        /// </summary>
        private const string I18nSection = "Tour";

        /// <summary>
        /// The name of the Tour titles.
        /// </summary>
        private const string I18nTitle = "Title";

        /// <summary>
        /// The name of the Tour body text.
        /// </summary>
        private const string I18nBody = "Body";

        /// <summary>
        /// The name of the title of the Tour pop-up.
        /// </summary>
        private const string TourReminderTitle = "Tour.Reminder.Title";

        /// <summary>
        /// The name of the body of the Tour pop-up.
        /// </summary>
        private const string TourReminderBody = "Tour.Reminder.Body";

        /// <summary>
        /// Parent form.
        /// </summary>
        private Control parent;

        /// <summary>
        /// Optional parent suffix.
        /// </summary>
        private string suffix;

        /// <summary>
        /// Navigation nodes.
        /// </summary>
        private (Control control, int? iteration, Orientation orientation)[] nodes;

        /// <summary>
        /// Current tour index.
        /// </summary>
        private int index;

        /// <summary>
        /// True if the target control was made visible by the tour.
        /// </summary>
        private bool controlMadeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tour"/> class.
        /// </summary>
        public Tour()
        {
            this.InitializeComponent();
            I18n.Localize(this, this.toolTip1);
        }

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            using (var t = new Tour())
            {
                I18n.Localize(t, t.toolTip1);
            }

            I18n.LocalizeGlobal(TourReminderTitle, "wx3270 Tour");
            I18n.LocalizeGlobal(TourReminderBody, "To run this or any other tour again, right-click on the '?' help button.");
        }

        /// <summary>
        /// Returns the localization key for the title of a tour.
        /// </summary>
        /// <param name="control">Control to be toured.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        public static string TitleKey(Control control, int? iteration) => I18n.Combine(BaseKey(control, iteration), I18nTitle);

        /// <summary>
        /// Returns the localization key for the title of a tour.
        /// </summary>
        /// <param name="form">Form name.</param>
        /// <param name="component">Component name.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        public static string TitleKey(string form, string component, int? iteration = null) => I18n.Combine(BaseKey(form, component, iteration), I18nTitle);

        /// <summary>
        /// Returns the localization key for the title of a tour.
        /// </summary>
        /// <param name="form">Form name.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        public static string TitleKey(string form, int? iteration = null) => I18n.Combine(BaseKey(form, iteration), I18nTitle);

        /// <summary>
        /// Returns the localization key for the body of a tour.
        /// </summary>
        /// <param name="control">Control to be toured.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        public static string BodyKey(Control control, int? iteration = null) => I18n.Combine(BaseKey(control, iteration), I18nBody);

        /// <summary>
        /// Returns the localization key for the body of a tour.
        /// </summary>
        /// <param name="form">Form name.</param>
        /// <param name="component">Component name.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        public static string BodyKey(string form, string component, int? iteration = null) => I18n.Combine(BaseKey(form, component, iteration), I18nBody);

        /// <summary>
        /// Returns the localization key for the body of a tour.
        /// </summary>
        /// <param name="form">Form name.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        public static string BodyKey(string form, int? iteration = null) => I18n.Combine(BaseKey(form, iteration), I18nBody);

        /// <summary>
        /// Tests whether the user has been shown this tour.
        /// </summary>
        /// <param name="control">Control in question.</param>
        /// <param name="suffix">Optional suffix.</param>
        /// <returns>True if the tour is complete.</returns>
        public static bool IsComplete(Control control, string suffix = null)
        {
            using var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.TourCompleteKey);
            return key.GetValue(TourCompleteValue(nameof(Tour))) != null || key.GetValue(TourCompleteValue(control, suffix)) != null;
        }

        /// <summary>
        /// An item on the Help menu was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        /// <param name="helpKeyword">Keyword for online help.</param>
        /// <param name="tourAction">Action to perform to start a tour.</param>
        public static void HelpMenuClick(object sender, EventArgs e, string helpKeyword, Action tourAction)
        {
            if (!(sender is ToolStripMenuItem menuItem))
            {
                return;
            }

            var tag = (string)menuItem.Tag;
            switch (tag)
            {
                case "Help":
                    Wx3270App.GetHelp(helpKeyword);
                    break;
                case "Tour":
                    tourAction();
                    break;
            }
        }

        /// <summary>
        /// Create a tour and navigate it.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="nodes">Tour nodes.</param>
        /// <param name="suffix">Optional suffix.</param>
        public static void Navigate(Control parent, IEnumerable<(Control control, int? iteration, Orientation orientation)> nodes, string suffix = null)
        {
            var tour = new Tour();
            tour.NavigateFrom(parent, nodes, suffix);
        }

        /// <summary>
        /// Navigate a tour.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="nodes">Tour nodes.</param>
        /// <param name="suffix">Optional suffix.</param>
        /// <returns>Dialog result.</returns>
        public DialogResult NavigateFrom(Control parent, IEnumerable<(Control control, int? iteration, Orientation orientation)> nodes, string suffix = null)
        {
            this.nodes = nodes.ToArray();
            if (this.nodes.Length == 0)
            {
                return DialogResult.Cancel;
            }

            // Remember the tour.
            this.parent = parent;
            this.suffix = suffix;

            // Set up for the first node.
            this.index = 0;
            this.Iterate(Direction.None);

            // Display the form and navigate from there.
            return this.ShowDialog(parent.FindForm());
        }

        /// <summary>
        /// Returns the value name for a completed tour.
        /// </summary>
        /// <param name="control">Control in question.</param>
        /// <param name="suffix">Optional suffix.</param>
        /// <returns>Value name.</returns>
        private static string TourCompleteValue(Control control, string suffix = null) => TourCompleteValue((control is TabPage ? (control.FindForm().Name + ".") : string.Empty) + control.Name + (suffix ?? string.Empty));

        /// <summary>
        /// Returns the value name for a completed tour.
        /// </summary>
        /// <param name="className">Name of class.</param>
        /// <returns>Value name.</returns>
        private static string TourCompleteValue(string className) => Profile.VersionClass.GetThis().ToString() + "-" + className;

        /// <summary>
        /// Stores tour completion in the registry.
        /// </summary>
        /// <param name="control">Control in question.</param>
        /// <param name="suffix">Optional suffix.</param>
        private static void SetComplete(Control control, string suffix = null)
        {
            using var key = Registry.CurrentUser.CreateSubKey(Constants.Misc.TourCompleteKey);
            key.SetValue(TourCompleteValue(control, suffix), 0);
        }

        /// <summary>
        /// Returns the base localization key for a tour.
        /// </summary>
        /// <param name="control">Control to be toured.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        private static string BaseKey(Control control, int? iteration) => I18n.Combine(I18nSection, (control is Form) ? string.Empty : control.FindForm().Name, control.Name, iteration.HasValue ? iteration.ToString() : string.Empty);

        /// <summary>
        /// Returns the base localization key for a tour.
        /// </summary>
        /// <param name="form">Form name.</param>
        /// <param name="component">Component name.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        private static string BaseKey(string form, string component, int? iteration) => I18n.Combine(I18nSection, form, component, iteration.HasValue ? iteration.ToString() : string.Empty);

        /// <summary>
        /// Returns the base localization key for a tour.
        /// </summary>
        /// <param name="form">Form name.</param>
        /// <param name="iteration">Optional iteration.</param>
        /// <returns>Localization key.</returns>
        private static string BaseKey(string form, int? iteration) => I18n.Combine(I18nSection, form, iteration.HasValue ? iteration.ToString() : string.Empty);

        /// <summary>
        /// Make sure a table layout panel item has the right coordinates.
        /// </summary>
        /// <param name="control">Control in question.</param>
        /// <param name="column">Desired column.</param>
        /// <param name="row">Desired row.</param>
        private void SetItemColumnRow(Control control, int column, int row)
        {
            if (control.Parent != this.outerTableLayoutPanel || this.outerTableLayoutPanel.GetColumn(control) != column || this.outerTableLayoutPanel.GetRow(control) != row)
            {
                this.outerTableLayoutPanel.Controls.Remove(control);
                this.outerTableLayoutPanel.Controls.Add(control, column, row);
            }
        }

        /// <summary>
        /// Sets up a new instance of the <see cref="Tour"/> class.
        /// </summary>
        /// <param name="control">Control that the tour refers to.</param>
        /// <param name="iteration">Optional iteration number.</param>
        /// <param name="orientation">Orientation of that control.</param>
        /// <param name="previous">True if the previous button should be active.</param>
        /// <param name="next">True if the next button should be active.</param>
        private void Setup(Control control, int? iteration, Orientation orientation, bool previous = true, bool next = true)
        {
            if (control != null)
            {
                switch (orientation)
                {
                    case Orientation.UpperLeft:
                    case Orientation.UpperLeftTight:
                        this.outerTableLayoutPanel.SuspendLayout();
                        this.arrowPictureBox.Image = Properties.Resources.arrow_left_up2;
                        this.SetItemColumnRow(this.arrowPictureBox, 0, 0);
                        this.SetItemColumnRow(this.borderPanel, 1, 1);
                        this.outerTableLayoutPanel.ResumeLayout();
                        this.Location = control.PointToScreen(
                            new Point(
                                orientation == Orientation.UpperLeft ? control.Width * 2 / 3 : control.Width * 1 / 3,
                                orientation == Orientation.UpperLeft ? control.Height * 2 / 3 : control.Height * 1 / 3));
                        this.titleLabel.Anchor = AnchorStyles.Left;
                        break;
                    case Orientation.UpperRight:
                        this.outerTableLayoutPanel.SuspendLayout();
                        this.arrowPictureBox.Image = Properties.Resources.arrow_right_up;
                        this.SetItemColumnRow(this.arrowPictureBox, 1, 0);
                        this.SetItemColumnRow(this.borderPanel, 0, 1);
                        this.outerTableLayoutPanel.ResumeLayout();
                        this.Location = control.PointToScreen(new Point((control.Width * 1 / 3) - this.Width, control.Height * 2 / 3));
                        this.titleLabel.Anchor = AnchorStyles.Right;
                        break;
                    case Orientation.LowerLeft:
                    case Orientation.LowerLeftTight:
                        this.outerTableLayoutPanel.SuspendLayout();
                        this.arrowPictureBox.Image = Properties.Resources.arrow_left_down;
                        this.SetItemColumnRow(this.arrowPictureBox, 0, 1);
                        this.SetItemColumnRow(this.borderPanel, 1, 0);
                        this.outerTableLayoutPanel.ResumeLayout();
                        this.Location = control.PointToScreen(
                            new Point(
                                orientation == Orientation.LowerLeft ? control.Width * 2 / 3 : control.Width * 1 / 3,
                                (control.Height * 1 / 3) - this.Height));
                        this.titleLabel.Anchor = AnchorStyles.Left;
                        break;
                    case Orientation.LowerRight:
                        this.outerTableLayoutPanel.SuspendLayout();
                        this.arrowPictureBox.Image = Properties.Resources.arrow_right_down;
                        this.SetItemColumnRow(this.arrowPictureBox, 1, 1);
                        this.SetItemColumnRow(this.borderPanel, 0, 0);
                        this.outerTableLayoutPanel.ResumeLayout();
                        this.Location = control.PointToScreen(new Point((control.Width * 1 / 3) - this.Width, (control.Height * 1 / 3) - this.Height));
                        this.titleLabel.Anchor = AnchorStyles.Right;
                        break;
                    case Orientation.Centered:
                        this.outerTableLayoutPanel.Controls.Remove(this.arrowPictureBox);
                        this.Location = control.PointToScreen(new Point((control.Width - this.Width) / 2, (control.Height - this.Height) / 2));
                        this.titleLabel.Anchor = AnchorStyles.None;
                        break;
                }
            }

            // Set up button defaults.
            this.previousButton.Enabled = previous;
            this.nextButton.Enabled = next;
            if (next)
            {
                this.AcceptButton = this.nextButton;
                this.ActiveControl = this.nextButton;
            }
            else
            {
                this.AcceptButton = this.closeButton;
                this.ActiveControl = this.closeButton;
            }

            // Make the backgrounds of the window and the arrow transparent.
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.DarkRed;
            this.TransparencyKey = Color.DarkRed;
            this.arrowPictureBox.BackColor = Color.DarkRed;

            // Get the text.
            if (control != null)
            {
                var titleKey = TitleKey(control, iteration);
                this.titleLabel.Text = I18n.Get(titleKey, $"Error: Title not found");
                var bodyKey = BodyKey(control, iteration);
                this.bodyTextBox.Text = I18n.Get(bodyKey, $"Error: Tour text '{bodyKey}' not found");
            }
        }

        /// <summary>
        /// Wrap up the tour.
        /// </summary>
        private void Wrap()
        {
            if (!IsComplete(this.parent, this.suffix))
            {
                SetComplete(this.parent, this.suffix);
            }

            this.Close();

            // Remind them how to take the tour again.
            NativeMethods.SHMessageBoxCheckW(
            this.parent.Handle,
            I18n.Get(TourReminderBody),
            I18n.Get(TourReminderTitle),
            NativeMethods.MessageBoxCheckFlags.MB_OK | NativeMethods.MessageBoxCheckFlags.MB_ICONINFORMATION,
            NativeMethods.MessageBoxReturnValue.IDOK,
            "wx3270.Tour");
        }

        /// <summary>
        /// Go to the next step in the tour.
        /// </summary>
        /// <param name="direction">Direction to step.</param>
        private void Iterate(Direction direction)
        {
            // Make the previous control disappear, if we made it appear in the first place.
            if (this.controlMadeVisible)
            {
                this.nodes[this.index].control.Visible = false;
            }

            // Advance.
            switch (direction)
            {
                case Direction.Left:
                    if (this.index > 0)
                    {
                        this.index--;
                    }

                    break;
                case Direction.Right:
                    if (this.index < this.nodes.Length - 1)
                    {
                        this.index++;
                    }

                    break;
                case Direction.None:
                default:
                    break;
            }

            var control = this.nodes[this.index].control;
            if (control.Visible)
            {
                this.controlMadeVisible = false;
            }
            else
            {
                // Make the current control visible, if it isn't now.
                control.Visible = true;
                this.controlMadeVisible = true;
            }

            // Rearrange the form.
            this.Setup(control, this.nodes[this.index].iteration, this.nodes[this.index].orientation, previous: this.index > 0, next: this.index < this.nodes.Length - 1);
        }

        /// <summary>
        /// The Quit Tour button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void QuitTourButtonClick(object sender, EventArgs e)
        {
            this.Wrap();
        }

        /// <summary>
        /// The Stop tours button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void StopToursButtonClick(object sender, EventArgs e)
        {
            SetComplete(this);
            this.Wrap();
        }

        /// <summary>
        /// The Previous button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PreviousClick(object sender, EventArgs e)
        {
            this.Iterate(Direction.Left);
        }

        /// <summary>
        /// The Quit Tour button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NextClick(object sender, EventArgs e)
        {
            this.Iterate(Direction.Right);
        }

        /// <summary>
        /// The text box got focus. We don't want it to.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void TextBoxEnter(object sender, EventArgs e)
        {
            this.ActiveControl = this.nextButton.Enabled ? this.nextButton : (this.previousButton.Enabled ? this.previousButton : this.closeButton);
        }

        /// <summary>
        /// A key was pressed on the form.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arrguments.</param>
        private void TourKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageUp:
                    if (this.previousButton.Enabled)
                    {
                        this.Iterate(Direction.Left);
                    }

                    break;
                case Keys.PageDown:
                    if (this.nextButton.Enabled)
                    {
                        this.Iterate(Direction.Right);
                    }

                    break;
            }
        }
    }
}
