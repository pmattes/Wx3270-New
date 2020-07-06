// <copyright file="DragAndDrop.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Wx3270.Contracts;

    /// <summary>
    /// Drag and drop utility class.
    /// </summary>
    /// <typeparam name="T">Entry type</typeparam>
    public class DragAndDrop<T>
        where T : class, ISyncedEntry<T>
    {
        /// <summary>
        /// The control.
        /// </summary>
        private readonly Control control;

        /// <summary>
        /// The entries for the list box.
        /// </summary>
        private readonly ISynced<T> entries;

        /// <summary>
        /// The entries for the other list box (if any).
        /// </summary>
        private readonly ISynced<T> otherEntries;

        /// <summary>
        /// The list box.
        /// </summary>
        private readonly ListBox listBox;

        /// <summary>
        /// Tag for entries this list generates.
        /// </summary>
        private readonly string myTag;

        /// <summary>
        /// Tag for entries the other list generates (if any).
        /// </summary>
        private readonly string otherTag;

        /// <summary>
        /// Location where the mouse button was pressed inside the list box.
        /// </summary>
        private Point? mouseDownPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragAndDrop{T}"/> class.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="entries">My entries</param>
        /// <param name="otherEntries">Other list box's entries</param>
        /// <param name="listBox">List box</param>
        /// <param name="myTag">Tag for my entries</param>
        /// <param name="otherTag">Tag for other list box's entries</param>
        public DragAndDrop(Control control, ISynced<T> entries, ISynced<T> otherEntries, ListBox listBox, string myTag, string otherTag)
        {
            this.control = control;
            this.entries = entries;
            this.otherEntries = otherEntries;
            this.listBox = listBox;
            this.myTag = myTag + " ";
            this.otherTag = otherTag + " ";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragAndDrop{T}"/> class.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="entries">My entries</param>
        /// <param name="listBox">List box</param>
        /// <param name="myTag">Tag for my entries</param>
        public DragAndDrop(Control control, ISynced<T> entries, ListBox listBox, string myTag)
        {
            this.control = control;
            this.entries = entries;
            this.listBox = listBox;
            this.myTag = myTag + " ";
        }

        /// <summary>
        /// Mouse down method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                this.mouseDownPoint = e.Location;
            }
        }

        /// <summary>
        /// Mouse move method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (this.listBox.Items.Count == 0)
            {
                return;
            }

            if (e.Button.HasFlag(MouseButtons.Left)
                && this.mouseDownPoint.HasValue
                && (Math.Abs(e.X - this.mouseDownPoint.Value.X) >= SystemInformation.DoubleClickSize.Width
                    || Math.Abs(e.Y - this.mouseDownPoint.Value.Y) >= SystemInformation.DoubleClickSize.Height))
            {
                var index = this.listBox.IndexFromPoint(this.mouseDownPoint.Value);
                if (index != ListBox.NoMatches && ((T)this.listBox.Items[index]).CanMove)
                {
                    this.control.DoDragDrop(this.myTag + index, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// Drag over method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (!e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                return;
            }

            var contents = (string)e.Data.GetData(DataFormats.Text);
            var tag = contents.Split(' ')[0] + " ";
            if (tag.Equals(this.myTag))
            {
                // Move within this list.
                e.Effect = DragDropEffects.Move;
            }
            else if (this.otherTag != null && tag.Equals(this.otherTag))
            {
                // Copy other list to this one.
                e.Effect = DragDropEffects.Copy;
                return;
            }
            else
            {
                return;
            }

            var point = this.listBox.PointToClient(new Point(e.X, e.Y));
            var index = this.listBox.IndexFromPoint(point);
            var selectedIndex = this.listBox.SelectedIndex;
            if (index < 0)
            {
                index = selectedIndex;
            }

            if (index != selectedIndex)
            {
                this.listBox.SelectedIndex = index;
            }

            if (point.Y <= (this.listBox.Font.Height * 2))
            {
                this.listBox.TopIndex -= 1;
            }
            else if (point.Y >= this.listBox.Height - (this.listBox.Font.Height * 2))
            {
                this.listBox.TopIndex += 1;
            }
        }

        /// <summary>
        /// Drag drop method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        public void DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                var contents = (string)e.Data.GetData(DataFormats.Text);
                if (contents.StartsWith(this.myTag))
                {
                    // Move within my list box.
                    var sourceIndex = int.Parse(contents.Substring(this.myTag.Length));
                    var point = this.listBox.PointToClient(new Point(e.X, e.Y));
                    var targetIndex = this.listBox.IndexFromPoint(point);
                    if (targetIndex < 0
                        || targetIndex == sourceIndex
                        || (targetIndex < sourceIndex && !((ISyncedEntry<T>)this.listBox.Items[targetIndex]).CanMoveBefore))
                    {
                        return;
                    }

                    // Move the original entry before or after the target entry.
                    this.entries.Move(sourceIndex, targetIndex);
                }

                if (this.otherTag != null && contents.StartsWith(this.otherTag))
                {
                    // Copy from the other list to this one.
                    var index = int.Parse(contents.Substring(this.otherTag.Length));
                    this.entries.Add(this.otherEntries[index]);
                }
            }
        }
    }
}
