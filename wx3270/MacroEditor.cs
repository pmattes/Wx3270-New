// <copyright file="MacroEditor.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// The macro editor.
    /// </summary>
    public partial class MacroEditor : Form
    {
        /// <summary>
        /// Localization group for message box titles.
        /// </summary>
        private static readonly string TitleName = I18n.PopUpTitleName(nameof(MacroEditor));

        /// <summary>
        /// Localization group for message box messages.
        /// </summary>
        private static readonly string MessageName = I18n.MessageName(nameof(MacroEditor));

        /// <summary>
        /// True if the macro name can be edited.
        /// </summary>
        private readonly bool canChangeName;

        /// <summary>
        /// The application context.
        /// </summary>
        private readonly Wx3270App app;

        /// <summary>
        /// True if the form has ever been activated.
        /// </summary>
        private bool everActivated;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroEditor"/> class.
        /// </summary>
        /// <param name="macroText">Macro text or insert text.</param>
        /// <param name="name">Macro name.</param>
        /// <param name="canChangeName">True if it is legal to change the macro name.</param>
        /// <param name="app">Application context.</param>
        /// <param name="previousState">Previous state.</param>
        public MacroEditor(string macroText, string name, bool canChangeName, Wx3270App app, EditorState previousState = null)
        {
            this.InitializeComponent();

            this.canChangeName = canChangeName;
            this.MacroBox.Text = InsertNicely(macroText, previousState, out int newCursor);
            if (!string.IsNullOrEmpty(this.MacroBox.Text) && !this.MacroBox.Text.EndsWith(Environment.NewLine))
            {
                this.MacroBox.Text += Environment.NewLine;
            }

            if (this.MacroBox.Text == Environment.NewLine)
            {
                this.MacroBox.Text = string.Empty;
            }

            this.MacroBox.Select(this.MacroBox.Text.Length, 0);
            if (newCursor >= 0)
            {
                this.MacroBox.SelectionStart = newCursor;
            }

            this.nameTextBox.Enabled = canChangeName;

            this.app = app;

            if (!string.IsNullOrEmpty(name))
            {
                this.nameTextBox.Text = name;
            }

            if (this.app?.Restricted(Restrictions.GetHelp) == true)
            {
                this.helpPictureBox.RemoveFromParent();
            }

            // Localize.
            this.Text = I18n.NoLocal;

            I18n.Localize(this, this.toolTip1, this.connectContextMenuStrip, this.switchProfileContextMenuStrip, this.scriptContextMenuStrip);

            this.sourceDialog.Title = I18n.Localize(this, I18n.Combine("sourceDialog", "title"), this.sourceDialog.Title);
            this.newScriptDialog.Title = I18n.Localize(this, I18n.Combine("scriptDialog", "title"), this.newScriptDialog.Title);

            this.Text = (string.IsNullOrEmpty(name) ? string.Empty : name + " - ") + I18n.Get(Title.MacroEditor);

            this.requiredLabel.Text = canChangeName ? "*" : string.Empty;

            // Substitute.
            VersionSpecific.Substitute(this);

            // Fix the PF drop-down, which the designer creates with space on the left for graphics or check marks.
            this.pfContextMenuStrip.Items.Clear();
            this.pfContextMenuStrip.Items.Add(this.PfStrip(1, 12));
            this.pfContextMenuStrip.Items.Add(this.PfStrip(13, 12));
        }

        /// <summary>
        /// Gets the macro name.
        /// </summary>
        public string MacroName => this.nameTextBox.Text;

        /// <summary>
        /// Gets the macro text.
        /// </summary>
        public string MacroText => this.MacroBox.Text.TrimEnd(Environment.NewLine.ToCharArray());

        /// <summary>
        /// Gets the location of the cursor in the macro editor.
        /// </summary>
        public int MacroCursor => this.MacroBox.SelectionStart;

        /// <summary>
        /// Gets the current state, used to resume after a recording completes.
        /// </summary>
        public EditorState State => new EditorState
        {
            Text = this.MacroBox.Text,
            SelectionStart = this.MacroBox.SelectionStart,
            SelectionLength = this.MacroBox.SelectionLength,
        };

        /// <summary>
        /// Static localization.
        /// </summary>
        [I18nInit]
        public static void Localize()
        {
            I18n.LocalizeGlobal(Title.MacroEditor, "Macro Editor");
            I18n.LocalizeGlobal(Title.ScriptCreation, "Script Creation");
            I18n.LocalizeGlobal(Title.EditScript, "Edit Script");
            I18n.LocalizeGlobal(Title.EditTarget, "Edit Target");

            I18n.LocalizeGlobal(Message.MustSpecifyName, "Must specify name");
            I18n.LocalizeGlobal(Message.Other, "Other");
            I18n.LocalizeGlobal(Message.InvalidMacroSyntax, "Invalid macro syntax");
            I18n.LocalizeGlobal(Message.UnrecognizedScriptType, "Unrecognized script type");
            I18n.LocalizeGlobal(Message.MissingFileName, "Missing file name");
            I18n.LocalizeGlobal(Message.NoEdit, "No " + B3270.Action.Script + "() or " + B3270.Action.Source + "() action under the cursor");

            // Set up the tour.
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1137 // Elements should have the same indentation

            // Global step 1.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), 1), "Tour: Macro Editor");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), 1),
@"A macro is a sequence of wx3270 actions to perform.

Use the Macro Editor to define or change a macro.");

            // Macro name.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(nameTextBox)), "Macro name");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(nameTextBox)),
@"Enter a unique name for the macro here.");

            // Macros.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(MacroBox)), "Macro text");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(MacroBox)),
@"Enter the actions here, one per line.

wx3270 will perform the actions in sequence until the end is reached, or one of them fails.

You can enter text directly from the keyboard, or you can click on one of the snippet buttons below to insert a snippet at the current cursor position.

You can put a comment in the macro by starting a line with '#'.");

            // Snippets.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(BackTabButton)), "Snippets");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(BackTabButton)),
@"Click on one of these buttons to insert a snippet into the macro at the current cursor position.

The snippets are not an exhaustive list of wx3270 actions, just some common ones.");

            // Script button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(scriptButton)), "Script button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(scriptButton)),
@"The Script button is special. It lets you create scripts using Python, PowerShell, VBScript, JScript, or the scripting language of your choice.

For the first four, wx3270 will generate a template for you, which illustrates how to initialize communication between your script and wx3270, send wx3270 a command, and get information from wx3270.");

            // Edit@ button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(editScriptButton)), "Edit@ button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(editScriptButton)),
@"To edit an existing script, place the cursor in the 'Macro text' box on a line containing a Script() action and click this button.");

            // Record button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(recordButton)), "Record button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(recordButton)),
@"To record actions from the keyboard with the macro recorder, click this button.

The recorded actions will replace the entire contents of the 'Macro text' box.");

            // Save button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(saveButton)), "Save button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(saveButton)),
@"Click to save the macro.");

            // Cancel button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(editorCancelButton)), "Save button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(editorCancelButton)),
@"Click to abandon your edits.");

            // Help button.
            I18n.LocalizeGlobal(Tour.TitleKey(nameof(MacroEditor), nameof(helpPictureBox)), "Help button");
            I18n.LocalizeGlobal(
                Tour.BodyKey(nameof(MacroEditor), nameof(helpPictureBox)),
@"Click to display context-dependent help from the wx3270 Wiki in your browser, or to restart this tour.");

#pragma warning restore SA1137 // Elements should have the same indentation
#pragma warning restore SA1118 // Parameter should not span multiple lines
        }

        /// <summary>
        /// Form localization.
        /// </summary>
        [I18nFormInit]
        public static void LocalizeForm()
        {
            new MacroEditor(string.Empty, "dummy", false, null).Dispose();
        }

        /// <summary>
        /// Insert new macro text into existing text, nicely.
        /// </summary>
        /// <param name="insertText">Text to insert.</param>
        /// <param name="oldState">Existing macro state.</param>
        /// <param name="newCursor">Returned new cursor location.</param>
        /// <returns>Combined string.</returns>
        internal static string InsertNicely(string insertText, EditorState oldState, out int newCursor)
        {
            newCursor = -1;

            // Make sure the inserted text does not end with a newline.
            insertText = insertText.TrimEnd(Environment.NewLine.ToArray());

            if (oldState == null || oldState.Text.Length == 0)
            {
                // No existing text.
                return insertText + Environment.NewLine;
            }

            var text = oldState.Text;
            var cursor = oldState.SelectionStart;
            if (oldState.SelectionLength > 0)
            {
                // If there was text selected, remove it first -- this is effectively a paste action.
                text = text.Remove(cursor, oldState.SelectionLength);

                // We don't want to replace part of a line. The user might do that with an explicit paste, but we won't.
                // Remove text backwards until we hit the beginning of the string or a newline. Point past the newline.
                while (cursor > 0 && text[cursor - 1] != Environment.NewLine[Environment.NewLine.Length - 1])
                {
                    text = text.Remove(--cursor, 1);
                }

                // Remove text forward until we hit (and remove) a newline or hit the end of the string.
                while (text.Length > cursor)
                {
                    if (text.Substring(cursor).StartsWith(Environment.NewLine))
                    {
                        text = text.Remove(cursor, Environment.NewLine.Length);
                        break;
                    }

                    text = text.Remove(cursor, 1);
                }
            }

            // Local function to append a newline to a string if it is non-empty.
            static string WithSeparator(string t) => t + (t.Length > 0 ? Environment.NewLine : string.Empty);

            // Trim away any trailing newlines in the existing text. We will put one back at the end.
            text = text.TrimEnd(Environment.NewLine.ToArray());

            if (cursor >= text.Length)
            {
                // Append.
                return WithSeparator(text) + insertText + Environment.NewLine;
            }

            // We don't want to insert in the middle of a line. So if the insertion point is not at the beginning of a line, back up until it is.
            while (cursor > 0 && !text.Substring(cursor).StartsWith(Environment.NewLine))
            {
                cursor--;
            }

            if (cursor == 0)
            {
                // Backed up to (or already at) the beginning of the string. Prepend.
                newCursor = insertText.Length;
                return insertText + Environment.NewLine + WithSeparator(text);
            }

            if (text.Substring(cursor).StartsWith(Environment.NewLine))
            {
                // Backed up, but not to the beginning of the string. 'cursor' is now pointing at the newline separating the previous line from
                // the line where the cursor was. Jump back to the beginning of the original cursor line.
                cursor += Environment.NewLine.Length;
            }

            // 'cursor' is now at the insertion point, after a newline, and there is text before and after it.
            newCursor = cursor + insertText.Length;
            return text.Substring(0, cursor) + insertText + Environment.NewLine + text.Substring(cursor) + Environment.NewLine;
        }

        /// <summary>
        /// Generate a context menu strip for a range of PF keys.
        /// </summary>
        /// <param name="start">Starting PF key.</param>
        /// <param name="count">Number of keys.</param>
        /// <returns>Menu strip.</returns>
        private ToolStripMenuItem PfStrip(int start, int count)
        {
            var drop = new ContextMenuStrip
            {
                Text = string.Format("{0}-{1}", start, start + count - 2),
                ShowCheckMargin = false,
                ShowImageMargin = false,
            };
            drop.Items.AddRange(Enumerable.Range(start, count).Select(n => new ToolStripMenuItem(n.ToString(), null, this.FixedContextMenuClick) { Tag = $"PF({n})" }).ToArray());
            return new ToolStripMenuItem(string.Format("{0}-{1}", start, start + count - 1)) { DropDown = drop };
        }

        /// <summary>
        /// The save button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OkayButton_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                return;
            }

            if (this.canChangeName && string.IsNullOrWhiteSpace(this.nameTextBox.Text))
            {
                ErrorBox.Show(I18n.Get(Message.MustSpecifyName), I18n.Get(Title.MacroEditor));
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// The cancel button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NotOkayButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// One of the macro fragment buttons was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Macro_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button.Tag is string text)
            {
                this.InsertText(text);
            }

            this.MacroBox.Focus();
        }

        /// <summary>
        /// Validation method for the macro body text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacroBox_Validating(object sender, CancelEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;

            if (!ActionSyntax.Check(text, out _, out _, out int index, out string errorText))
            {
                if (this.ActiveControl.Equals(this.editorCancelButton))
                {
                    // Allow the form to be closed, but get rid of the bad text so it does not haunt us later.
                    textBox.Text = string.Empty;
                }
                else
                {
                    // Complain.
                    ErrorBox.Show(I18n.Get(Message.InvalidMacroSyntax) + ": " + errorText, I18n.Get(Title.MacroEditor));
                    e.Cancel = true;

                    // Highlight the error.
                    textBox.Select(index, 1);
                    textBox.Focus();
                }
            }
        }

        /// <summary>
        /// Validation method for the name text box.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void NameTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!this.canChangeName)
            {
                // If they can't change the name, then it must be good.
                return;
            }

            // Leading and trailing spaces are bad.
            this.nameTextBox.Text = this.nameTextBox.Text.Trim(new[] { ' ' });
        }

        /// <summary>
        /// Insert text into the macro.
        /// </summary>
        /// <param name="text">Text to insert.</param>
        private void InsertText(string text)
        {
            var newText = InsertNicely(text, this.State, out int newCursor);
            if (!newText.EndsWith(Environment.NewLine))
            {
                newText += Environment.NewLine;
            }

            // XXX: Ideally, we could pay attention to the user's selection, and replace it. For now, we ignore it, except for its start position.
            this.MacroBox.Text = newText;
            this.MacroBox.SelectionStart = (newCursor >= 0) ? newCursor : newText.Length;

            this.ValidateChildren();
        }

        /// <summary>
        /// The Source button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Source_clicked(object sender, EventArgs e)
        {
            this.sourceDialog.InitialDirectory = ProfileManager.ProfileDirectory;
            if (this.sourceDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.InsertText(string.Format(B3270.Action.Source + "({0})", BackEndAction.Quote(this.sourceDialog.FileName)));
            }

            this.MacroBox.Focus();
        }

        /// <summary>
        /// Create a new Python script.
        /// </summary>
        private void NewPython()
        {
            this.newScriptDialog.InitialDirectory = ProfileManager.ProfileDirectory;
            this.newScriptDialog.Filter = "Python scripts (*.py)|*.py";
            this.newScriptDialog.FileName = string.Empty;
            if (this.newScriptDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // If the named script does not exist, create it with a template.
                if (!File.Exists(this.newScriptDialog.FileName))
                {
                    var a = System.Reflection.Assembly.GetExecutingAssembly();
                    var s = a.GetManifestResourceStream("Wx3270.Resources.Template.py");
                    if (s != null)
                    {
                        // Copy the template to the script.
                        using (var reader = new StreamReader(s))
                        {
                            using (var writer = new StreamWriter(this.newScriptDialog.FileName))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    writer.WriteLine(line.Replace("%INSTALL%", Path.Combine(Application.StartupPath, "Python").Replace(@"\", @"\\")));
                                }
                            }
                        }

                        // Edit the script.
                        // Eventually we will use a Registry-defined suffix-specific value for the editor.
                        var p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.Verb = "edit";
                        p.StartInfo.FileName = this.newScriptDialog.FileName;
                        p.StartInfo.Arguments = this.newScriptDialog.FileName;
                        p.Start();
                        p.WaitForExit();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ScriptCreation));
            }

            this.InsertText(string.Format(B3270.Action.Script + "(pyw,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
        }

        /// <summary>
        /// Create a new PowerShell script.
        /// </summary>
        private void NewPowershell()
        {
            this.newScriptDialog.InitialDirectory = ProfileManager.ProfileDirectory;
            this.newScriptDialog.Filter = "Powershell scripts (*.ps1)|*.ps1";
            this.newScriptDialog.FileName = string.Empty;
            if (this.newScriptDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // If the named script does not exist, create it with a template.
                if (!File.Exists(this.newScriptDialog.FileName))
                {
                    var a = System.Reflection.Assembly.GetExecutingAssembly();
                    var s = a.GetManifestResourceStream("Wx3270.Resources.Template.ps1");
                    if (s != null)
                    {
                        // Copy the template to the script.
                        using (var reader = new StreamReader(s))
                        {
                            using (var writer = new StreamWriter(this.newScriptDialog.FileName))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    writer.WriteLine(line.Replace("%DLL%", X3270is.PathName));
                                }
                            }
                        }

                        // Edit the script with the PowerShell ISE.
                        var p = new Process();
                        p.StartInfo.UseShellExecute = false;
                        ////p.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell_ise.exe");
                        p.StartInfo.FileName = "powershell_ise.exe";
                        p.StartInfo.Arguments = "-File \"" + this.newScriptDialog.FileName + "\"";
                        p.Start();
                        p.WaitForExit();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ScriptCreation));
            }

            this.InsertText(string.Format(B3270.Action.Script + "(" + B3270.ScriptOption.ShareConsole + ",PowerShell.exe,-File,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
        }

        /// <summary>
        /// Create a new VBScript script.
        /// </summary>
        private void NewVBScript()
        {
            this.newScriptDialog.InitialDirectory = ProfileManager.ProfileDirectory;
            this.newScriptDialog.Filter = "VBScript scripts (*.vbs)|*.vbs";
            this.newScriptDialog.FileName = string.Empty;
            if (this.newScriptDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // If the named script does not exist, create it with a template.
                if (!File.Exists(this.newScriptDialog.FileName))
                {
                    var a = System.Reflection.Assembly.GetExecutingAssembly();
                    var s = a.GetManifestResourceStream("Wx3270.Resources.Template.vbs");
                    if (s != null)
                    {
                        // Copy the template to the script.
                        using (var reader = new StreamReader(s))
                        {
                            using (var writer = new StreamWriter(this.newScriptDialog.FileName))
                            {
                                writer.Write(reader.ReadToEnd());
                            }
                        }

                        // Edit the script.
                        var p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.Verb = "edit";
                        p.StartInfo.FileName = this.newScriptDialog.FileName;
                        p.StartInfo.Arguments = this.newScriptDialog.FileName;
                        p.Start();
                        p.WaitForExit();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ScriptCreation));
            }

            this.InsertText(string.Format(B3270.Action.Script + "(wscript.exe,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
        }

        /// <summary>
        /// Create a new JScript script.
        /// </summary>
        private void NewJScript()
        {
            this.newScriptDialog.InitialDirectory = ProfileManager.ProfileDirectory;
            this.newScriptDialog.Filter = "JScript scripts (*.js)|*.js";
            this.newScriptDialog.FileName = string.Empty;
            if (this.newScriptDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // If the named script does not exist, create it with a template.
                if (!File.Exists(this.newScriptDialog.FileName))
                {
                    var a = System.Reflection.Assembly.GetExecutingAssembly();
                    var s = a.GetManifestResourceStream("Wx3270.Resources.Template.js");
                    if (s != null)
                    {
                        // Copy the template to the script.
                        using (var reader = new StreamReader(s))
                        {
                            using (var writer = new StreamWriter(this.newScriptDialog.FileName))
                            {
                                writer.Write(reader.ReadToEnd());
                            }
                        }

                        // Edit the script.
                        var p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.Verb = "edit";
                        p.StartInfo.FileName = this.newScriptDialog.FileName;
                        p.StartInfo.Arguments = this.newScriptDialog.FileName;
                        p.Start();
                        p.WaitForExit();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorBox.Show(e.Message, I18n.Get(Title.ScriptCreation));
            }

            this.InsertText(string.Format(B3270.Action.Script + "(wscript.exe,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
        }

        /// <summary>
        /// A script context menu item was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScriptContextMenuStrip_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var tag = (string)menuItem.Tag;
            switch (tag)
            {
                case "Python3":
                    this.NewPython();
                    break;
                case "PowerShell":
                    this.NewPowershell();
                    break;
                case "VBScript":
                    this.NewVBScript();
                    break;
                case "JScript":
                    this.NewJScript();
                    break;
                case "Other":
                    this.InsertText(B3270.Action.Script + "(\"xxx\")");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// The Script button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ScriptButton_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.scriptContextMenuStrip.Show((Button)sender, mouseEvent.Location);
            }

            this.MacroBox.Focus();
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
        /// The PA button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PA_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.paContextMenuStrip.Show((Button)sender, mouseEvent.Location);
            }
        }

        /// <summary>
        /// A context menu with fixed content was selected.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void FixedContextMenuClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item))
            {
                return;
            }

            this.InsertText((string)item.Tag);
            this.MacroBox.Focus();
        }

        /// <summary>
        /// The PF button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void PF_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                this.pfContextMenuStrip.Show((Button)sender, mouseEvent.Location);
            }
        }

        /// <summary>
        /// The Connect button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Connect_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                // Populate the context menu on the fly. Just use the hosts in the current profile.
                this.connectContextMenuStrip.Items.Clear();
                this.connectContextMenuStrip.Items.AddRange(
                    this.app.ProfileManager.Current.Hosts
                        .Select(host => new ToolStripMenuItem(host.Name, null, this.FixedContextMenuClick)
                        {
                            Tag = string.Format("uConnect({0})", BackEndAction.Quote(host.Name)),
                        })
                        .ToArray());
                this.connectContextMenuStrip.Items.Add(
                    new ToolStripMenuItem(I18n.Get(Message.Other), null, this.FixedContextMenuClick) { Tag = "uConnect(SomeHost)" });

                // Pop up the context menu.
                this.connectContextMenuStrip.Show((Button)sender, mouseEvent.Location);
            }
        }

        /// <summary>
        /// The Switch Profile button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void SwitchProfile_Click(object sender, EventArgs e)
        {
            var mouseEvent = (MouseEventArgs)e;
            if (mouseEvent.Button == MouseButtons.Left)
            {
                // Populate the conext menu on the fly.
                this.switchProfileContextMenuStrip.Items.Clear();

                foreach (var root in this.app.ProfileTracker.Tree)
                {
                    var tree = root.CloneTree();

                    // Prune broken profiles.
                    while (tree.Any((node) =>
                    {
                        if (node.Type == WatchNodeType.Profile && (node as ProfileWatchNode).Broken)
                        {
                            node.Unlink();
                            return true;
                        }

                        return false;
                    }))
                    {
                    }

                    // If there are no profiles left, do nothing.
                    if (!tree.Any((node) => node.Type == WatchNodeType.Profile))
                    {
                        // None anywhere.
                        break;
                    }

                    // Prune folders that have no profiles.
                    while (tree.Any((node) =>
                    {
                        if (node.Type != WatchNodeType.Host
                            && !node.Any((child) => child.Type == WatchNodeType.Profile))
                        {
                            node.Unlink();
                            return true;
                        }

                        return false;
                    }))
                    {
                    }

                    // Add to the profile the  menu.
                    var menuStack = new Stack<ToolStripItemCollection>();
                    menuStack.Push(this.switchProfileContextMenuStrip.Items);
                    tree.ForEach(menuStack, (node, stack) =>
                    {
                        switch (node.Type)
                        {
                            case WatchNodeType.Folder:
                                var folder = (FolderWatchNode)node;
                                if (folder.PathName == ProfileManager.SeedProfileDirectory)
                                {
                                    return stack.Peek();
                                }

                                var nonterminal = new ToolStripDropDownButton() { Text = node.Name, ShowDropDownArrow = true };
                                stack.Peek().Add(nonterminal);
                                return nonterminal.DropDownItems;
                            case WatchNodeType.Profile:
                                var profile = node as ProfileWatchNode;
                                stack.Peek().Add(new ToolStripMenuItem(profile.Name, null, this.FixedContextMenuClick)
                                {
                                    Tag = string.Format("uSwitchProfile({0})", BackEndAction.Quote(profile.PathName)),
                                });
                                return null;
                            case WatchNodeType.Host:
                            default:
                                return null;
                        }
                    });
                }

                this.switchProfileContextMenuStrip.Items.Add(
                    new ToolStripMenuItem(I18n.Get(Message.Other), null, this.FixedContextMenuClick) { Tag = "uSwitchProfile(\"SomeProfile\")" });

                // Pop up the context menu.
                this.switchProfileContextMenuStrip.Show((Button)sender, mouseEvent.Location);
            }
        }

        /// <summary>
        /// The Edit Script button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void EditScriptButtonClick(object sender, EventArgs e)
        {
            // Grab the text from the macro definition.
            var text = this.MacroBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            // Figure out what line the selection starts on.
            var start = this.MacroBox.SelectionStart;
            var split = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var index = 0;
            string currentLine = null;
            foreach (var line in split)
            {
                var next = index + line.Length + Environment.NewLine.Length;
                if (start >= index && start < next)
                {
                    currentLine = line;
                    break;
                }

                index = next;
            }

            if (currentLine == null)
            {
                return;
            }

            string editArg = null;

            // Make sure the line looks like a Script() action.
            if (currentLine.TrimStart(' ').StartsWith(B3270.Action.Script + "("))
            {
                // Figure out what kind of script it is, by looking for arguments that end with known suffixes like .py, .ps1, .vbs and .js.
                int ix = 0;
                if (!ActionSyntax.CheckLine(currentLine, out int column, ref ix, out string errorText, out string[] args))
                {
                    return;
                }

                var suffixes = new[] { "py", "ps1", "vbs", "js" };
                string suffix = null;
                foreach (var arg in args)
                {
                    suffix = suffixes.FirstOrDefault(s => arg.EndsWith("." + s, StringComparison.InvariantCultureIgnoreCase));
                    if (suffix != null)
                    {
                        editArg = arg;
                        break;
                    }
                }

                if (editArg == null)
                {
                    ErrorBox.Show(I18n.Get(Message.UnrecognizedScriptType), I18n.Get(Title.EditScript));
                    return;
                }
            }
            else if (currentLine.TrimStart(' ').StartsWith(B3270.Action.Source + "("))
            {
                int ix = 0;
                if (!ActionSyntax.CheckLine(currentLine, out int column, ref ix, out string errorText, out string[] args))
                {
                    return;
                }

                if (args.Length < 1)
                {
                    ErrorBox.Show(I18n.Get(Message.MissingFileName), I18n.Get(Title.EditSource));
                    return;
                }

                editArg = args[0];
            }
            else
            {
                ErrorBox.Show(I18n.Get(Message.NoEdit), I18n.Get(Title.EditTarget));
                return;
            }

            // Edit the script.
            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Verb = "edit";
            p.StartInfo.FileName = editArg;
            p.StartInfo.Arguments = editArg;
            p.Start();

            // Give the focus back to the text box.
            this.MacroBox.Focus();
        }

        /// <summary>
        /// The form was activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MacroEditorActivate(object sender, EventArgs e)
        {
            if (this.canChangeName && string.IsNullOrEmpty(this.nameTextBox.Text))
            {
                this.nameTextBox.Focus();
            }

            if (!this.everActivated)
            {
                this.everActivated = true;
                if (!Tour.IsComplete(this))
                {
                    this.RunTour();
                }
            }
        }

        /// <summary>
        /// The Record button was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void RecordButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        /// <summary>
        /// One of the help menu items was clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpMenuClick(object sender, EventArgs e)
        {
            Tour.HelpMenuClick(sender, e, "MacroEditor", this.RunTour);
        }

        /// <summary>
        /// Run the tour.
        /// </summary>
        private void RunTour()
        {
            var nodes = new List<(Control control, int? index, Orientation orientation)>
            {
                (this, 1, Orientation.Centered),
                (this.nameTextBox, null, Orientation.UpperLeft),
                (this.MacroBox, null, Orientation.UpperLeftTight),
                (this.BackTabButton, null, Orientation.LowerLeft),
                (this.scriptButton, null, Orientation.LowerRight),
                (this.editScriptButton, null, Orientation.LowerLeft),
                (this.recordButton, null, Orientation.LowerLeft),
                (this.saveButton, null, Orientation.LowerRight),
                (this.editorCancelButton, null, Orientation.LowerRight),
                (this.helpPictureBox, null, Orientation.LowerRight),
            }.Where(n => this.canChangeName || n.control != this.nameTextBox).ToList();
            Tour.Navigate(this, nodes);
        }

        /// <summary>
        /// The state of the macro text box.
        /// </summary>
        public class EditorState
        {
            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the index of the start of the selection.
            /// </summary>
            public int SelectionStart { get; set; }

            /// <summary>
            /// Gets or sets the length of the selection.
            /// </summary>
            public int SelectionLength { get; set; }
        }

        /// <summary>
        /// Localized titles.
        /// </summary>
        private class Title
        {
            /// <summary>
            /// Generic macro editor pop-up.
            /// </summary>
            public static readonly string MacroEditor = I18n.Combine(TitleName, "macroEditor");

            /// <summary>
            /// Script creation error.
            /// </summary>
            public static readonly string ScriptCreation = I18n.Combine(TitleName, "scriptCreation");

            /// <summary>
            /// Script editing error.
            /// </summary>
            public static readonly string EditScript = I18n.Combine(TitleName, "editScript");

            /// <summary>
            /// Source editing error.
            /// </summary>
            public static readonly string EditSource = I18n.Combine(TitleName, "editSource");

            /// <summary>
            /// Edit target error.
            /// </summary>
            public static readonly string EditTarget = I18n.Combine(TitleName, "editTarget");
        }

        /// <summary>
        /// Localized messages.
        /// </summary>
        private class Message
        {
            /// <summary>
            /// Must specify macro name.
            /// </summary>
            public static readonly string MustSpecifyName = I18n.Combine(MessageName, "mustSpecifyName");

            /// <summary>
            /// "Other" entry on menus.
            /// </summary>
            public static readonly string Other = I18n.Combine(MessageName, "other");

            /// <summary>
            /// Invalid macro syntax.
            /// </summary>
            public static readonly string InvalidMacroSyntax = I18n.Combine(MessageName, "invalidMacroSyntax");

            /// <summary>
            /// Unrecognized script type.
            /// </summary>
            public static readonly string UnrecognizedScriptType = I18n.Combine(MessageName, "unrecognizedScriptType");

            /// <summary>
            /// Missing file name.
            /// </summary>
            public static readonly string MissingFileName = I18n.Combine(MessageName, "missingFileName");

            /// <summary>
            /// No editable action under the cursor.
            /// </summary>
            public static readonly string NoEdit = I18n.Combine(MessageName, "noEdit");
        }
    }
}
