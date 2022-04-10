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
        /// True if the macros dialog has ever been activated.
        /// </summary>
        private bool everActivated;

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

            // Subscribe to profile change events.
            this.ProfileManager.Change += (profile) => this.MacrosSet(profile);

            // Set up merge handler.
            this.ProfileManager.RegisterMerge(ImportType.MacrosMerge | ImportType.MacrosReplace, this.MergeHandler);

            // Set up the undo and redo buttons.
            this.ProfileManager.RegisterUndoRedo(this.undoButton, this.redoButton, this.toolTip1);

            // Register the uMacro() action.
            app.BackEnd.RegisterPassthru(Constants.Action.Macro, this.UiMacro);

            // Substitute.
            VersionSpecific.Substitute(this);
            VersionSpecific.Substitute(this.contextMenuStrip1);

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
        /// Merge in macros from another profile.
        /// </summary>
        /// <param name="toProfile">Current profile.</param>
        /// <param name="fromProfile">Merge profile.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if the list changed.</returns>
        private bool MergeHandler(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (importType.HasFlag(ImportType.MacrosReplace))
            {
                // Replace macros.
                if (!toProfile.Macros.SequenceEqual(fromProfile.Macros))
                {
                    toProfile.Macros = fromProfile.Macros;
                    return true;
                }

                return false;
            }
            else
            {
                // Merge macros.
                var changed = false;
                var newList = toProfile.Macros.ToList();
                foreach (var macro in fromProfile.Macros)
                {
                    var existing = newList.FirstOrDefault(m => m.Name.Equals(macro.Name));
                    if (existing == null)
                    {
                        // New name.
                        newList.Add(macro);
                        changed = true;
                    }
                    else
                    {
                        // Same name, maybe different value.
                        if (!existing.Equals(macro))
                        {
                            existing.Macro = macro.Macro;
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    toProfile.Macros = newList;
                }

                return changed;
            }
        }

        /// <summary>
        /// Apply a profile to the macros list.
        /// </summary>
        /// <param name="profile">New profile.</param>
        private void MacrosSet(Profile profile)
        {
            this.macroEntries.Entries = profile.Macros;
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
                    this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, "add/edit macro");
                }
                else if (result == DialogResult.Retry)
                {
                    // Start macro recorder.
                    this.StartRecording(editor.MacroName);
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
                this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, "add/edit macro");
            }
            else if (result == DialogResult.Retry)
            {
                // Start macro recorder.
                this.StartRecording(editor.MacroName);
            }
        }

        /// <summary>
        /// Macro recording is complete.
        /// </summary>
        /// <param name="text">Macro text.</param>
        /// <param name="name">Macro name.</param>
        private void RecordingComplete(string text, object name)
        {
            this.Show();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            using var editor = new MacroEditor(text, (string)name, true, this.app);
            var result = editor.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.macroEntries.Add(new MacroEntry { Name = editor.MacroName, Macro = editor.MacroText });
                this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, "add/edit macro");
            }
            else if (result == DialogResult.Retry)
            {
                // Retart macro recorder.
                this.StartRecording(editor.MacroName);
            }
        }

        /// <summary>
        /// Start recording a macro.
        /// </summary>
        private void StartRecording(string macroName)
        {
            this.app.MacroRecorder.Start(this.RecordingComplete, macroName);
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
            this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, "remove macro");
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
            if (this.Owner != null)
            {
                this.Owner.BringToFront();
            }
        }

        /// <summary>
        /// Drag drop method for the macros list box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacrosListBox_DragDrop(object sender, DragEventArgs e)
        {
            this.dragDrop.DragDrop(sender, e);
            this.ProfileManager.PushAndSave((current) => { current.Macros = this.macroEntries.Entries; }, "re-order macros");
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
            if (!this.everActivated)
            {
                this.everActivated = true;
                this.Location = MainScreen.CenteredOn(this.mainScreen, this);
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
            var item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            var tag = item.Tag as string;
            if (tag == null)
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
            Wx3270App.GetHelp("Macros");
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
    }
}
