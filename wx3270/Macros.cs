// <copyright file="Macros.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using I18nBase;
    using Wx3270.Contracts;

    /// <summary>
    /// Macro manipulation.
    /// </summary>
    public partial class Macros : Form
    {
        /// <summary>
        /// Title group for localization.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(Macros));

        /// <summary>
        /// Message group for localization.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(Macros));

        /// <summary>
        /// Application instance.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// The drag and drop object.
        /// </summary>
        private readonly DragAndDrop<MacroEntry> dragDrop;

        /// <summary>
        /// The main screen.
        /// </summary>
        private readonly MainScreen mainScreen;

        /// <summary>
        /// The macros.
        /// </summary>
        private readonly SyncedListBoxes<MacroEntry> macroEntries;

        /// <summary>
        /// The set of macros currently running.
        /// </summary>
        private readonly ConcurrentDictionary<string, bool> macrosInProgress = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Macros"/> class.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="mainScreen">Main screen.</param>
        /// <param name="macroEntries">Macro entries.</param>
        public Macros(Wx3270App app, MainScreen mainScreen, SyncedListBoxes<MacroEntry> macroEntries)
        {
            this.InitializeComponent();

            this.app = app;
            this.mainScreen = mainScreen;
            this.macroEntries = macroEntries;

            // Set up drag and drop.
            this.dragDrop = new DragAndDrop<MacroEntry>(this, this.macroEntries, this.macrosListBox, "macro");

            // Link the listbox to the macro entries.
            this.macroEntries.AddListBox(this.macrosListBox);

            // Re-process the macros from the profile and subscribe to profile change events.
            this.macroEntries.Entries = this.ProfileManager.Current.Macros;
            this.ProfileManager.AddChangeTo((oldProfile, newProfile) => this.MacrosSet(newProfile));

            // Set up the undo and redo buttons.
            this.ProfileManager.RegisterUndoRedo(this.undoButton, this.redoButton, this.toolTip1);

            // Register the uMacro() action.
            app.BackEnd.RegisterPassthru(Constants.Action.Macro, this.UiMacro);

            // Enforce restrictions.
            if (app.Restricted(Restrictions.ChangeSettings))
            {
                this.macroAddButton.RemoveFromParent();
                this.macroRemoveButton.RemoveFromParent();
                this.macroEditButton.RemoveFromParent();
                this.recordButton.RemoveFromParent();

                this.editToolStripMenuItem.RemoveFromOwner();
                this.deleteToolStripMenuItem.RemoveFromOwner();

                this.undoButton.RemoveFromParent();
                this.redoButton.RemoveFromParent();
            }

            if (app.Restricted(Restrictions.GetHelp))
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Localize.
            I18n.Localize(this, this.toolTip1);

            // Substitute.
            VersionSpecific.Substitute(this);
            VersionSpecific.Substitute(this.contextMenuStrip1);
        }

        /// <summary>
        /// Gets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager => this.app.ProfileManager;

        /// <summary>
        /// Gets the emulator back end.
        /// </summary>
        private IBackEnd BackEnd => this.app.BackEnd;

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.MacroOutput, "Macro Output");
            I18n.LocalizeGlobal(Title.MacroError, "Macro");

            I18n.LocalizeGlobal(Message.AddOrEditMacro, "add/edit macro");
            I18n.LocalizeGlobal(Message.RemoveMacro, "remove macro");
            I18n.LocalizeGlobal(Message.ReorderMacros, "re-order macros");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Macros list.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(macrosListBox)), "Tour: Macros window");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(macrosListBox)),
@"A list of macros is displayed here.

Double-click on an entry to run it.

Right-click on an entry to get a menu of options.

Drag and drop to re-order the list, which will also change the order the macros appear on the main window.");

            // Add button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(macroAddButton)), "Add button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(macroAddButton)), @"Click to create a new macro with the macro editor.");

            // Record button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(recordButton)), "Record button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(recordButton)), @"Click to create a new macro from the keyboard with the macro recorder.");

            // Delete button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(macroRemoveButton)), "Delete button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(macroRemoveButton)), @"Click to delete the selected macro.");

            // Edit button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(macroEditButton)), "Edit button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(macroEditButton)), @"Click to edit the selected macro with the macro editor.");

            // Run button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(macroTestButton)), "Run button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(macroTestButton)), @"Click to execute the selected macro.");

            // Undo/redo buttons.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(undoButton)), "Undo and Redo buttons");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(undoButton)),
@"Click the '↶' (Undo) button to undo the last operation.

Click the '↷' (Redo) button to redo the last operation that was rolled back with the Undo button.

The button labels include a count of how many Undo and Redo operations are saved.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(Macros), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(Macros), nameof(helpPictureBox)),
@"Click to display context-sensitive help from the wx3270 Wiki in your browser, or to start this tour again.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// An external (menu-based) macro has been added.
        /// </summary>
        /// <param name="text">Macro text.</param>
        public void Record(string text)
        {
            this.Show();
            this.RecordingComplete(text, string.Empty);
        }

        /// <summary>
        /// Apply a profile to the macros list.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void MacrosSet(Profile profile)
        {
            this.MacrosListBox_SelectedIndexChanged(null, new EventArgs()); // Why manually?
        }

        /// <summary>
        /// The Macro Edit button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacroEditButton_Click(object sender, EventArgs e)
        {
            var index = this.macrosListBox.SelectedIndex;
            if (index >= 0)
            {
                var macro = this.macroEntries[index];
                using var editor = new MacroEditor(macro.Macro, macro.Name, true, this.app);
                var result = editor.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    this.macroEntries[index] = new MacroEntry { Name = editor.MacroName, Macro = editor.MacroText };
                    this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, I18n.Get(Message.AddOrEditMacro));
                }
                else if (result == DialogResult.Retry)
                {
                    // Start macro recorder.
                    this.StartRecording(editor.MacroName, editor.State);
                }
            }
        }

        /// <summary>
        /// The Macro Test button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacroTestButton_Click(object sender, EventArgs e)
        {
            var index = this.macrosListBox.SelectedIndex;
            if (index >= 0)
            {
                var macro = this.macroEntries[index];
                var fullMacroName = Path.Combine(this.ProfileManager.Current.PathName, macro.Name);
                this.macrosInProgress[fullMacroName] = true;
                this.BackEnd.RunActions(
                    ActionSyntax.FormatForRun(macro.Macro),
                    "macro",
                    (cookie, success, result, misc) =>
                    {
                        this.macrosInProgress.TryRemove(fullMacroName, out bool ignored);
                        if (success && !string.IsNullOrWhiteSpace(result))
                        {
                            ErrorBox.Show(result, I18n.Get(Title.MacroOutput), MessageBoxIcon.Information);
                        }
                        else if (!success)
                        {
                            ErrorBox.Show(result, I18n.Get(Title.MacroError));
                        }
                    });
            }
        }

        /// <summary>
        /// The macro add button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacroAddButton_Click(object sender, EventArgs e)
        {
            using var editor = new MacroEditor(string.Empty, string.Empty, true, this.app);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.macroEntries.Add(new MacroEntry { Name = editor.MacroName, Macro = editor.MacroText });
                this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, I18n.Get(Message.AddOrEditMacro));
            }
            else if (result == DialogResult.Retry)
            {
                // Start macro recorder.
                this.StartRecording(editor.MacroName, editor.State);
            }
        }

        /// <summary>
        /// Macro recording is complete.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="context">Macro recording context.</param>
        private void RecordingComplete(string text, object context)
        {
            this.Show();
            if (string.IsNullOrEmpty(text) || context == null)
            {
                return;
            }

            (string name, MacroEditor.EditorState state) = (((string, MacroEditor.EditorState)?)context).Value;
            using var editor = new MacroEditor(text, name, canChangeName: true, this.app, state);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.macroEntries.Add(new MacroEntry { Name = editor.MacroName, Macro = editor.MacroText });
                this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, I18n.Get(Message.AddOrEditMacro));
            }
            else if (result == DialogResult.Retry)
            {
                // Retart macro recorder.
                this.StartRecording(editor.MacroName, editor.State);
            }
        }

        /// <summary>
        /// Start recording a macro.
        /// </summary>
        /// <param name="macroName">Macro name.</param>
        /// <param name="state">Macro editor state.</param>
        private void StartRecording(string macroName, MacroEditor.EditorState state = null)
        {
            this.app.MacroRecorder.Start(this.RecordingComplete, (macroName, state));
            this.Hide();
            this.mainScreen.Focus();
        }

        /// <summary>
        /// The macro remove button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacroRemoveButton_Click(object sender, EventArgs e)
        {
            this.macroEntries.Delete(this.macrosListBox);
            this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, I18n.Get(Message.RemoveMacro));
        }

        /// <summary>
        /// The macros list box was double clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_DoubleClick(object sender, EventArgs e)
        {
            this.MacroTestButton_Click(sender, e);
            this.Hide();
            this.Owner.BringToFront();
        }

        /// <summary>
        /// The macros list box selection index changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var anySelected = this.macrosListBox.SelectedIndex >= 0;
            this.macroEditButton.Enabled = anySelected;
            this.macroTestButton.Enabled = anySelected;
            this.macroRemoveButton.Enabled = anySelected;
        }

        /// <summary>
        /// The form is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Macros_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.Owner?.BringToFront();
        }

        /// <summary>
        /// Drag drop method for the macros list box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_DragDrop(object sender, DragEventArgs e)
        {
            this.dragDrop.DragDrop(sender, e);
            this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, I18n.Get(Message.ReorderMacros));
        }

        /// <summary>
        /// Drag over method for the macros list box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_DragOver(object sender, DragEventArgs e)
        {
            this.dragDrop.DragOver(sender, e);
        }

        /// <summary>
        /// Mouse down method for the macros list box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.dragDrop.MouseDown(sender, e);
            }

            if (e.Button == MouseButtons.Right)
            {
                var index = this.macrosListBox.IndexFromPoint(e.Location);
                if (index == ListBox.NoMatches)
                {
                    return;
                }

                // Select this item.
                this.macrosListBox.SelectedIndex = index;
            }
        }

        /// <summary>
        /// Mouse move method for the macros list box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.dragDrop.MouseMove(sender, e);
        }

        /// <summary>
        /// The macros dialog was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Macros_Activated(object sender, EventArgs e)
        {
            if (!Tour.IsComplete(this))
            {
                this.RunTour();
            }
        }

        /// <summary>
        /// The Undo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            this.ProfileManager.Undo();
        }

        /// <summary>
        /// The Redo button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RedoButton_Click(object sender, EventArgs e)
        {
            this.ProfileManager.Redo();
        }

        /// <summary>
        /// One of the context menu items was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ContextMenu_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item))
            {
                return;
            }

            if (!(item.Tag is string tag))
            {
                return;
            }

            if (this.macrosListBox.SelectedIndex == ListBox.NoMatches)
            {
                return;
            }

            switch (tag)
            {
                case "Run":
                    this.MacrosListBox_DoubleClick(sender, e);
                    break;
                case "Edit":
                    this.MacroEditButton_Click(sender, e);
                    break;
                case "Delete":
                    this.MacroRemoveButton_Click(sender, e);
                    break;
            }
        }

        /// <summary>
        /// The record button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RecordButton_Click(object sender, EventArgs e)
        {
            this.StartRecording(string.Empty);
        }

        /// <summary>
        /// The Help picture box was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Help_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.helpContextMenuStrip.Show(this.helpPictureBox, mouseEvent.Location);
            }
        }

        /// <summary>
        /// One of the help menu items was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "Macros", () => this.RunTour(isExplicit: true));
        }

        /// <summary>
        /// Runs the tour.
        /// </summary>
        /// <param name="isExplicit">True if invoked explicitly.</param>
        private void RunTour(bool isExplicit = false)
        {
            var nodes = new[]
            {
                ((Control)this.macrosListBox, (int?)null, Orientation.UpperLeftTight),
                (this.macroAddButton, null, Orientation.LowerLeft),
                (this.recordButton, null, Orientation.LowerLeft),
                (this.macroRemoveButton, null, Orientation.LowerLeft),
                (this.macroEditButton, null, Orientation.LowerRight),
                (this.macroTestButton, null, Orientation.LowerRight),
                (this.undoButton, null, Orientation.LowerRight),
                (this.helpPictureBox, null, Orientation.LowerRight),
            };
            Tour.Navigate(this, nodes, isExplicit: isExplicit);
        }

        /// <summary>
        /// The UI Macro action.
        /// </summary>
        /// <param name="commandName">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        /// <param name="result">Returned result.</param>
        /// <param name="tag">Tag for asynchronous completion.</param>
        /// <returns>Pass-through result.</returns>
        private PassthruResult UiMacro(string commandName, IEnumerable<string> arguments, out string result, string tag)
        {
            var args = arguments.ToList();
            if (args.Count != 1)
            {
                result = Constants.Action.Macro + "() takes 1 argument";
                return PassthruResult.Failure;
            }

            var macro = this.ProfileManager.Current.Macros.FirstOrDefault(h => h.Name.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
            if (macro == null)
            {
                result = Constants.Action.Macro + "(): no such macro defined in the current profile";
                return PassthruResult.Failure;
            }

            var fullMacroName = Path.Combine(this.ProfileManager.Current.PathName, macro.Name);
            if (this.macrosInProgress.ContainsKey(fullMacroName))
            {
                result = Constants.Action.Macro + "(): recursion not allowed";
                return PassthruResult.Failure;
            }

            this.macrosInProgress[fullMacroName] = true;
            this.BackEnd.RunActions(
                    ActionSyntax.FormatForRun(macro.Macro),
                    "macro",
                    (cookie, success, res, misc) =>
                    {
                        this.macrosInProgress.TryRemove(fullMacroName, out bool ignored);
                        this.app.BackEnd.PassthruComplete(success, res, tag);
                    });

            result = string.Empty;
            return PassthruResult.Pending;
        }

        /// <summary>
        /// The macros window was loaded.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosLoad(object sender, EventArgs e)
        {
            // For some reason, this form will not center on its parent without doing this explicitly.
            this.CenterToParent();
        }

        /// <summary>
        /// Message box titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Macro output.
            /// </summary>
            public static readonly string MacroOutput = I18n.Combine(TitleName, "macroOutput");

            /// <summary>
            /// Macro error.
            /// </summary>
            public static readonly string MacroError = I18n.Combine(TitleName, "macroError");
        }

        /// <summary>
        /// Messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Add or edit a macro.
            /// </summary>
            public static readonly string AddOrEditMacro = I18n.Combine(MessageName, "addOrEditMacro");

            /// <summary>
            /// Remove a macro.
            /// </summary>
            public static readonly string RemoveMacro = I18n.Combine(MessageName, "removeMacro");

            /// <summary>
            /// Re-order macros.
            /// </summary>
            public static readonly string ReorderMacros = I18n.Combine(MessageName, "re-order macros");
        }
    }
}
