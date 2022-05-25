// <copyright file="VersionSpecific.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Version-specific behaviors.
    /// </summary>
    public static class VersionSpecific
    {
        /// <summary>
        /// The Add symbol as it appears in constants, and as displayed on Windows 10.
        /// </summary>
        public const string Add = "➕";

        /// <summary>
        /// The Add symbol as displayed on older versions.
        /// </summary>
        public const string AddAlt = "✚";

        /// <summary>
        /// The Delete symbol as it appears in constants, and as displayed on Windows 10.
        /// </summary>
        public const string Delete = "❌";

        /// <summary>
        /// The Delete symbol as displayed on older versions.
        /// </summary>
        public const string DeleteAlt = "✖";

        /// <summary>
        /// The Return symbol as it appears in constants, and as displayed on Windows 10.
        /// </summary>
        public const string Return = "⤶";

        /// <summary>
        /// The Return symbol as displayed on older versions.
        /// </summary>
        public const string ReturnAlt = "↲";

        /// <summary>
        /// The Edit symbol as it appears in constants, and as displayed on Windows 10.
        /// </summary>
        public const string Edit = "🖉";

        /// <summary>
        /// The Edit symbol as displayed on older versions.
        /// </summary>
        public const string EditAlt = "";

        /// <summary>
        /// The Connect symbol as it appears in constants, and as displayed on Windows 10.
        /// </summary>
        public const string Connect = "🗲";

        /// <summary>
        /// The Connect symbol as displayed on older versions.
        /// </summary>
        public const string ConnectAlt = "~";

        /// <summary>
        /// The Lock symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Lock = "🔒";

        /// <summary>
        /// The Lock symbol as it is displayed on older versions.
        /// </summary>
        public const string LockAlt = "S";

        /// <summary>
        /// The APL symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Apl = "⍺";

        /// <summary>
        /// The APL symbol as it is displayed on older versions.
        /// </summary>
        public const string AplAlt = "a";

        /// <summary>
        /// The clock symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Clock = "🕓";

        /// <summary>
        /// The clock symbol as it is displayed on older versions.
        /// </summary>
        public const string ClockAlt = "";

        /// <summary>
        /// The printer symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Printer = "🖶";

        /// <summary>
        /// The printer symbol as it is displayed on older versions.
        /// </summary>
        public const string PrinterAlt = "P";

        /// <summary>
        /// The overflow symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Overflow = "🚶>";

        /// <summary>
        /// The overflow symbol as it is displayed on older versions.
        /// </summary>
        public const string OverflowAlt = "Overflow";

        /// <summary>
        /// The protected symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Protected = "←🚶→";

        /// <summary>
        /// The protected symbol as it is displayed on older versions.
        /// </summary>
        public const string ProtectedAlt = "Protected";

        /// <summary>
        /// The record symbol as it appears in constants, and is displayed on Windows 10.
        /// </summary>
        public const string Rec = "⏺";

        /// <summary>
        /// The record symbol as it is displayed on older versions.
        /// </summary>
        public const string RecAlt = "●";

        /// <summary>
        /// Gets the displayed Add symbol.
        /// </summary>
        public static string AddDisplay => Win10OrGreater ? Add : AddAlt;

        /// <summary>
        /// Gets the displayed Delete symbol.
        /// </summary>
        public static string DeleteDisplay => Win10OrGreater ? Delete : DeleteAlt;

        /// <summary>
        /// Gets the displayed Return symbol.
        /// </summary>
        public static string ReturnDisplay => Win10OrGreater ? Return : ReturnAlt;

        /// <summary>
        /// Gets the displayed Edit symbol.
        /// </summary>
        public static string EditDisplay => Win10OrGreater ? Edit : EditAlt;

        /// <summary>
        /// Gets the displayed Connect symbol.
        /// </summary>
        public static string ConnectDisplay => Win10OrGreater ? Connect : ConnectAlt;

        /// <summary>
        /// Gets the displayed Lock symbol.
        /// </summary>
        public static string LockDisplay => Win10OrGreater ? Lock : LockAlt;

        /// <summary>
        /// Gets the displayed APL symbol.
        /// </summary>
        public static string AplDisplay => Win10OrGreater ? Apl : AplAlt;

        /// <summary>
        /// Gets the displayed Clock symbol.
        /// </summary>
        public static string ClockDisplay => Win10OrGreater ? Clock : ClockAlt;

        /// <summary>
        /// Gets the displayed Printer symbol.
        /// </summary>
        public static string PrinterDisplay => Win10OrGreater ? Printer : PrinterAlt;

        /// <summary>
        /// Gets the displayed Overflow symbol.
        /// </summary>
        public static string OverflowDisplay => Win10OrGreater ? Overflow : OverflowAlt;

        /// <summary>
        /// Gets the displayed Protected symbol.
        /// </summary>
        public static string ProtectedDisplay => Win10OrGreater ? Protected : ProtectedAlt;

        /// <summary>
        /// Gets a value indicating whether the OS is Windows 10 or later.
        /// </summary>
        public static bool Win10OrGreater => Environment.OSVersion.Version.Major >= 10;

        /// <summary>
        /// Gets the Clock symbol, plus a space, if it is supported.
        /// </summary>
        public static string ClockPlusSpace => Win10OrGreater ? Clock + " " : string.Empty;

        /// <summary>
        /// Gets a value indicating whether the platform supports Private Use Area (PUA) characters.
        /// </summary>
        public static bool SupportsPua => Win10OrGreater;

        /// <summary>
        /// Do version-specific text substitutions.
        /// </summary>
        /// <param name="text">Text to operate on.</param>
        /// <returns>Modified text.</returns>
        public static string Substitute(string text)
        {
            return text
                .Replace(Add, AddAlt)
                .Replace(Delete, DeleteAlt)
                .Replace(Edit + " ", string.Empty)
                .Replace(Connect, ConnectAlt)
                .Replace(Clock, ClockAlt)
                .Replace(Rec, RecAlt);
        }

        /// <summary>
        /// Substitute certain version-specific text in control text.
        /// </summary>
        /// <param name="control">Control to modify.</param>
        public static void Substitute(Control control)
        {
            if (Win10OrGreater)
            {
                return;
            }

            foreach (var node in I18n.Walk(control, always: true).Select(n => n.Control))
            {
                node.Text = Substitute(node.Text);
            }
        }

        /// <summary>
        /// Substitute certain version-specific text in context menu text.
        /// </summary>
        /// <param name="contextMenuStrip">Context menu strip to modify.</param>
        public static void Substitute(ContextMenuStrip contextMenuStrip)
        {
            if (Win10OrGreater)
            {
                return;
            }

            foreach (var node in I18n.Walk(contextMenuStrip).Select(n => n.ToolStripMenuItem))
            {
                node.Text = Substitute(node.Text);
            }
        }
    }
}
