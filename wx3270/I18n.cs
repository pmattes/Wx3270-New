﻿// <copyright file="I18n.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using I18nBase;

    /// <summary>
    /// Internationalization and localization support.
    /// </summary>
    public static class I18n
    {
        /// <summary>
        /// Symbolic text indicating that a control should not be localized.
        /// </summary>
        public const string NoLocal = "`";

        /// <summary>
        /// Symbolic Tag indicating that a control should not be recursively walked for localization.
        /// </summary>
        public const string NoWalk = "<nowalk>";

        /// <summary>
        /// The name of a form.
        /// </summary>
        public const string FormName = "form";

        /// <summary>
        /// The name of a form title.
        /// </summary>
        public const string Title = "title";

        /// <summary>
        /// Name of a tool tip.
        /// </summary>
        public const string ToolTipName = "toolTip";

        /// <summary>
        /// Walk a control hierarchy.
        /// </summary>
        /// <param name="control">Control to walk.</param>
        /// <param name="toolTip">Tool tip.</param>
        /// <returns>Set of nodes in the control.</returns>
        public static IEnumerable<I18nNode> Walk(Control control, ToolTip toolTip = null)
        {
            return Walk(string.Empty, control, toolTip);
        }

        /// <summary>
        /// Walk a context menu strip hierarchy.
        /// </summary>
        /// <param name="contextMenuStrip">Menu strip to walk.</param>
        /// <returns>Set of nodes in the control.</returns>
        public static IEnumerable<I18nNode> Walk(ContextMenuStrip contextMenuStrip)
        {
            return contextMenuStrip.Items.OfType<ToolStripMenuItem>().SelectMany(m => Walk(contextMenuStrip.Name, m));
        }

        /// <summary>
        /// Set up localization for forms. This happens late in initialization, just before starting the UI.
        /// </summary>
        public static void SetupForms()
        {
            foreach (var m in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "Wx3270").SelectMany(t => t.GetMethods().Where(m => m.IsStatic)))
            {
                if (m.CustomAttributes.Any(a => a.AttributeType == typeof(I18nFormInitAttribute)))
                {
                    m.Invoke(null, new object[0]);
                }
            }
        }

        /// <summary>
        /// Localize an entire form.
        /// </summary>
        /// <param name="form">Form to translate.</param>
        /// <param name="toolTip">Tool tip.</param>
        /// <param name="contextMenuStrips">Context menus.</param>
        public static void Localize(Form form, ToolTip toolTip = null, params ContextMenuStrip[] contextMenuStrips)
        {
            foreach (var node in Walk(form, toolTip).Where(n => n.Text != null))
            {
                if (!(node.Control is TextBox)
                    && !string.IsNullOrWhiteSpace(node.Text)
                    && !node.Text.StartsWith(NoLocal)
                    && !HasNoLocalTag(node.Control.Tag))
                {
                    var path = (node.Control == form) ? Combine(node.Path, Title) : node.Path;
                    node.Control.Text = LocalizeFlat(node.Text, path);
                }

                if (node.ToolTip != null && !string.IsNullOrWhiteSpace(toolTip.GetToolTip(node.Control)))
                {
                    var toolTipPath = Combine(node.Path, ToolTipName);
                    toolTip.SetToolTip(node.Control, LocalizeFlat(toolTip.GetToolTip(node.Control), toolTipPath));
                }

                if (node.Control.ContextMenuStrip != null)
                {
                    foreach (var m in Walk(node.Control.ContextMenuStrip))
                    {
                        var menuPath = Combine(node.Path, m.Path);
                        m.ToolStripMenuItem.Text = LocalizeFlat(m.ToolStripMenuItem.Text, menuPath);
                    }
                }
            }

            foreach (var contextMenuStrip in contextMenuStrips)
            {
                foreach (var m in Walk(contextMenuStrip))
                {
                    m.ToolStripMenuItem.Text = LocalizeFlat(m.ToolStripMenuItem.Text, Combine(form.Name, FormName, m.Path));
                }
            }
        }

        /// <summary>
        /// Localize the text of a static control.
        /// </summary>
        /// <param name="control">Control to compute.</param>
        /// <param name="text">English text to localize.</param>
        /// <returns>Localized text.</returns>
        public static string Localize(Control control, string text = null)
        {
            return LocalizeFlat(text ?? control.Text, ComputedPath(control));
        }

        /// <summary>
        /// Localize the text of a dynamic or non-parent/child control.
        /// </summary>
        /// <param name="control">Control to compute.</param>
        /// <param name="pathSuffix">Pathname suffix.</param>
        /// <param name="text">English text to localize.</param>
        /// <returns>Localized text.</returns>
        public static string Localize(Control control, string pathSuffix, string text = null)
        {
            pathSuffix = pathSuffix.Replace(" ", "_").Replace(Environment.NewLine, "_");
            return LocalizeFlat(text ?? control.Text, Combine(ComputedPath(control), pathSuffix));
        }

        /// <summary>
        /// Localize global text.
        /// </summary>
        /// <param name="pathName">Pathname.</param>
        /// <param name="text">English text to localize.</param>
        /// <returns>Localized text.</returns>
        public static string LocalizeGlobal(string pathName, string text)
        {
            return LocalizeFlat(text, pathName.Replace(" ", "_"));
        }

        /// <summary>
        /// Combine names.
        /// </summary>
        /// <param name="elements">Elements to combine.</param>
        /// <returns>Combined path elements.</returns>
        /// <remarks>
        /// Ideally this should be inherited from I18nBase, but you can't do that with static classes.
        /// </remarks>
        public static string Combine(params string[] elements)
        {
            return I18nBase.Combine(elements);
        }

        /// <summary>
        /// Combine names.
        /// </summary>
        /// <param name="elements">Elements to combine.</param>
        /// <returns>Combined path elements.</returns>
        /// <remarks>
        /// Ideally this should be inherited from I18nBase, but you can't do that with static classes.
        /// </remarks>
        public static string Combine(IEnumerable<string> elements)
        {
            return I18nBase.Combine(elements);
        }

        /// <summary>
        /// Get a localized string.
        /// </summary>
        /// <param name="path">Path name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Localized value.</returns>
        /// <remarks>
        /// Ideally this should be inherited from I18nBase, but you can't do that with static classes.
        /// </remarks>
        public static string Get(string path, string defaultValue = null)
        {
            return I18nBase.Get(path.Replace(" ", "_"), defaultValue);
        }

        /// <summary>
        /// Returns the title group name for a module.
        /// </summary>
        /// <param name="baseName">Base (module) name.</param>
        /// <returns>Group name.</returns>
        public static string TitleName(string baseName)
        {
            return Combine(baseName, "Title");
        }

        /// <summary>
        /// Returns the message group name for a module.
        /// </summary>
        /// <param name="baseName">Base (module) name.</param>
        /// <returns>Group name.</returns>
        public static string MessageName(string baseName)
        {
            return Combine(baseName, "Message");
        }

        /// <summary>
        /// Returns the string group name for a module.
        /// </summary>
        /// <param name="baseName">Base (module) name.</param>
        /// <returns>Group name.</returns>
        public static string StringName(string baseName)
        {
            return Combine(baseName, "String");
        }

        /// <summary>
        /// Ask the DLL to do low-level localization and cache the result.
        /// </summary>
        /// <param name="text">Text to localize.</param>
        /// <param name="path">Path name.</param>
        /// <returns>Localized string.</returns>
        /// <remarks>
        /// Ideally this should be inherited from I18nBase, but you can't do that with static classes.
        /// </remarks>
        private static string LocalizeFlat(string text, string path)
        {
            return I18nBase.LocalizeFlat(text, path);
        }

        /// <summary>
        /// Checks a tag for no localization.
        /// </summary>
        /// <param name="tag">Tag to check.</param>
        /// <returns>True if localization should be skipped.</returns>
        private static bool HasNoLocalTag(object tag)
        {
            return tag is string tagString && tagString.StartsWith(NoLocal);
        }

        /// <summary>
        /// Compute the path of a control.
        /// </summary>
        /// <param name="control">Control to scan.</param>
        /// <returns>Path name.</returns>
        private static string ComputedPath(Control control)
        {
            var names = new Stack<string>();
            for (var c = control; c != null; c = c.Parent)
            {
                names.Push(c.Name);
            }

            return Combine(names);
        }

        /// <summary>
        /// Recursively walk the children of a control.
        /// </summary>
        /// <param name="pathName">Path name.</param>
        /// <param name="control">Control to walk.</param>
        /// <param name="toolTip">Tool tip.</param>
        /// <returns>List of elements.</returns>
        private static IEnumerable<I18nNode> Walk(string pathName, Control control, ToolTip toolTip)
        {
            if ((control.Tag as string) == NoWalk)
            {
                return new List<I18nNode>();
            }

            var controlName = control is Form ? Combine(control.Name, FormName) : control.Name;
            var path = pathName == string.Empty ? controlName : Combine(pathName, controlName);
            var ret = new List<I18nNode>
            {
                new I18nNode
                {
                    Path = path,
                    Control = control,
                    Text = control.Text,
                    ToolTip = toolTip?.GetToolTip(control),
                },
            };

            foreach (var child in control.Controls.OfType<Control>())
            {
                ret.AddRange(Walk(path, child, toolTip));
            }

            return ret;
        }

        /// <summary>
        /// Recursively walk the children of a context menu.
        /// </summary>
        /// <param name="pathName">Path name.</param>
        /// <param name="menuItem">Menu item.</param>
        /// <returns>List of elements.</returns>
        private static IEnumerable<I18nNode> Walk(string pathName, ToolStripMenuItem menuItem)
        {
            var ret = new List<I18nNode>
            {
                new I18nNode
                {
                    Path = Combine(pathName, menuItem.Name),
                    Control = null,
                    Text = menuItem.Text,
                    ToolTip = null,
                    ToolStripMenuItem = menuItem,
                },
            };
            foreach (var child in menuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                ret.AddRange(Walk(Combine(pathName, menuItem.Name), child));
            }

            return ret;
        }
    }
}
