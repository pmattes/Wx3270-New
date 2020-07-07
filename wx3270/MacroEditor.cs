// <copyright file="MacroEditor.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

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
        private static readonly string TitleName = I18n.TitleName(nameof(MacroEditor));

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
        /// Initializes a new instance of the <see cref="MacroEditor"/> class.
        /// </summary>
        /// <param name="macroText">Macro text.</param>
        /// <param name="name">Macro name.</param>
        /// <param name="canChangeName">True if it is legal to change the macro name.</param>
        /// <param name="app">Application context.</param>
        public MacroEditor(string macroText, string name, bool canChangeName, Wx3270App app)
        {
            this.InitializeComponent();

            this.canChangeName = canChangeName;
            this.MacroBox.Text = macroText;
            if (!string.IsNullOrEmpty(this.MacroBox.Text) && !this.MacroBox.Text.EndsWith(Environment.NewLine))
            {
                this.MacroBox.Text += Environment.NewLine;
            }

            this.MacroBox.Select(this.MacroBox.Text.Length, 0);

            this.nameTextBox.Enabled = canChangeName;
            this.requiredLabel.Visible = canChangeName;

            this.app = app;

            if (!string.IsNullOrEmpty(name))
            {
                this.nameTextBox.Text = name;
            }

            // Localize.
            this.Text = I18n.NoLocal;

            I18n.Localize(this, this.toolTip1, this.connectContextMenuStrip, this.switchProfileContextMenuStrip, this.scriptContextMenuStrip);

            this.sourceDialog.Title = I18n.Localize(this, I18n.Combine("sourceDialog", "title"), this.sourceDialog.Title);
            this.newScriptDialog.Title = I18n.Localize(this, I18n.Combine("scriptDialog", "title"), this.newScriptDialog.Title);

            this.Text = (string.IsNullOrEmpty(name) ? string.Empty : name + " - ") + I18n.Get(Title.MacroEditor);

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
            I18n.LocalizeGlobal(Message.NoEdit, "No Script() or Source() action under the cursor");
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
            var text = button.Tag as string;
            if (text != null)
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
                if (this.ActiveControl.Equals(this.EditorCancelButton))
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
            int selectionStart = this.MacroBox.SelectionStart;
            var newText = this.MacroBox.Text.Insert(selectionStart, text + Environment.NewLine);
            this.MacroBox.Text = newText;
            this.MacroBox.SelectionStart = selectionStart + text.Length + Environment.NewLine.Length;

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
                this.InsertText(string.Format("Source({0})", BackEndAction.Quote(this.sourceDialog.FileName)));
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

            this.InsertText(string.Format("Script(pyw,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
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

            this.InsertText(string.Format("Script(PowerShell.exe,-File,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
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

            this.InsertText(string.Format("Script(wscript.exe,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
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

            this.InsertText(string.Format("Script(wscript.exe,{0})", BackEndAction.Quote(this.newScriptDialog.FileName)));
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
                    this.InsertText("Script(\"xxx\")");
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
            Wx3270App.GetHelp("MacroEditor");
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
            var item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            this.InsertText((string)item.Tag);
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
            if (currentLine.TrimStart(' ').StartsWith("Script("))
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
            else if (currentLine.TrimStart(' ').StartsWith("Source("))
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
