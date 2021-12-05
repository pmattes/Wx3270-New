// <copyright file="Constants.cs" company="Paul Mattes">
// Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;

    /// <summary>
    /// Security restrictions.
    /// </summary>
    [Flags]
    public enum Restrictions
    {
        /// <summary>
        /// No restrictions.
        /// </summary>
        None = 0,

        /// <summary>
        /// Disallow file transfers.
        /// </summary>
        FileTransfer = 0x1,

        /// <summary>
        /// Profile modifications.
        /// </summary>
        ModifyProfiles = 0x2,

        /// <summary>
        /// Tracing.
        /// </summary>
        Tracing = 0x4,

        /// <summary>
        /// wx3270 prompt.
        /// </summary>
        Prompt = 0x10,

        /// <summary>
        /// Change external files.
        /// </summary>
        ExternalFiles = 0x20,

        /// <summary>
        /// Switch to a different profile.
        /// </summary>
        SwitchProfile = 0x40,

        /// <summary>
        /// Open a new window.
        /// </summary>
        NewWindow = 0x80,

        /// <summary>
        /// Modify host settings.
        /// </summary>
        ModifyHost = 0x100,

        /// <summary>
        /// Change any settings.
        /// </summary>
        ChangeSettings = 0x200,

        /// <summary>
        /// Get help.
        /// </summary>
        GetHelp = 0x400,

        /// <summary>
        /// Use the printer.
        /// </summary>
        Printing = 0x800,

        /// <summary>
        /// Disconnect from the host.
        /// </summary>
        Disconnect = 0x1000,

        /// <summary>
        /// Disallow everything.
        /// </summary>
        All = FileTransfer | ModifyProfiles | Tracing | Prompt | ExternalFiles | SwitchProfile | NewWindow | ModifyHost | ChangeSettings | GetHelp | Printing | Disconnect,
    }

    /// <summary>
    /// Wx3270 manifest constants.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// The copyright message.
        /// </summary>
        public const string Copyright = @"Copyright © 2016-2021 Paul Mattes.
All rights reserved.
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 * Neither the names of Paul Mattes nor the names of his contributors
    may be used to endorse or promote products derived from this software
    without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY PAUL MATTES ""AS IS"" AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO
EVENT SHALL PAUL MATTES BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.";

        /// <summary>
        /// Wx3270 command line options.
        /// </summary>
        public static class Option
        {
            /// <summary>
            /// Allow option (lock down all but certain operations).
            /// </summary>
            public const string Allow = "-allow";

            /// <summary>
            /// The console option.
            /// </summary>
            public const string Console = "-console";

            /// <summary>
            /// The culture option.
            /// </summary>
            public const string Culture = "-culture";

            /// <summary>
            /// Dump the localization dictionary in JSON format.
            /// </summary>
            public const string DumpLocalization = "-dumplocalization";

            /// <summary>
            /// The edit option. Suppresses auto-connect and implies read/write.
            /// </summary>
            public const string Edit = "-edit";

            /// <summary>
            /// The HTTP daemon option.
            /// </summary>
            public const string Httpd = "-httpd";

            /// <summary>
            /// The host option.
            /// </summary>
            public const string Host = "-host";

            /// <summary>
            /// The initial window location option.
            /// </summary>
            public const string Location = "-location";

            /// <summary>
            /// The no-border option.
            /// </summary>
            public const string NoBorder = "-noborder";

            /// <summary>
            /// The no-buttons option.
            /// </summary>
            public const string NoButtons = "-nobuttons";

            /// <summary>
            /// The no-profile option.
            /// </summary>
            public const string NoProfile = "-noprofile";

            /// <summary>
            /// The no scroll bar option.
            /// </summary>
            public const string NoScrollBar = "-noscrollbar";

            /// <summary>
            /// The profile option.
            /// </summary>
            public const string Profile = "-profile";

            /// <summary>
            /// The read-only option. Opens the profile in read-only mode.
            /// </summary>
            public const string ReadOnly = "-readonly";

            /// <summary>
            /// An abbreviation for "-readonly".
            /// </summary>
            public const string Ro = "-ro";

            /// <summary>
            /// The read/write option. Pops up a warning if the profile can't be opened
            /// in read/write mode.
            /// </summary>
            public const string ReadWrite = "-readwrite";

            /// <summary>
            /// Restrict option (lock down certain operations).
            /// </summary>
            public const string Restrict = "-restrict";

            /// <summary>
            /// Script port option.
            /// </summary>
            public const string ScriptPort = "-scriptport";

            /// <summary>
            /// Script port once option (exit once script disconnects).
            /// </summary>
            public const string ScriptPortOnce = "-scriptportonce";

            /// <summary>
            /// Make this window the topmost.
            /// </summary>
            public const string Topmost = "-topmost";

            /// <summary>
            /// The trace option.
            /// </summary>
            public const string Trace = "-trace";

            /// <summary>
            /// The UI trace option.
            /// </summary>
            public const string UiTrace = "-uitrace";

            /// <summary>
            /// The (no-op) UTF-8 option.
            /// </summary>
            public const string Utf8 = "-utf8";

            /// <summary>
            /// Display a copyright message and exit.
            /// </summary>
            public const string V = "-v";

            /// <summary>
            /// Write a version string to a file and exit.
            /// </summary>
            public const string Vfile = "-vfile";
        }

        /// <summary>
        /// Names of actions defined by the user interface (all start with 'u').
        /// </summary>
        public class Action
        {
            /// <summary>
            /// The name of the Chord action.
            /// </summary>
            public const string Chord = "uChord";

            /// <summary>
            /// The name of the UI connect action.
            /// </summary>
            public const string Connect = "uConnect";

            /// <summary>
            /// The name of the Copy action.
            /// </summary>
            public const string Copy = "uCopy";

            /// <summary>
            /// The name of the Cut action.
            /// </summary>
            public const string Cut = "uCut";

            /// <summary>
            /// The name of the wx3270 prompt help action.
            /// </summary>
            public const string Help = "uHelp";

            /// <summary>
            /// The name of the keyboard select action.
            /// </summary>
            public const string KeyboardSelect = "uKeyboardSelect";

            /// <summary>
            /// The name of the Macro action.
            /// </summary>
            public const string Macro = "uMacro";

            /// <summary>
            /// The name of the Paste action.
            /// </summary>
            public const string Paste = "uPaste";

            /// <summary>
            /// The name of the PrintText action.
            /// </summary>
            public const string PrintText = "uPrintText";

            /// <summary>
            /// The Quit if Not Connected action.
            /// </summary>
            public const string QuitIfNotConnected = "uQuitIfNotConnected";

            /// <summary>
            /// The name of the UI switch profile action.
            /// </summary>
            public const string SwitchProfile = "uSwitchProfile";
        }

        /// <summary>
        /// Miscellaneous constants.
        /// </summary>
        public class Misc
        {
            /// <summary>
            /// Registry key.
            /// </summary>
            public const string RegistryKey = @"Software\x3270\wx3270";

            /// <summary>
            /// Registry value for restrictions.
            /// </summary>
            public const string RestrictionsValue = "Restrictions";

            /// <summary>
            /// Registry key for watched folders.
            /// </summary>
            public const string WatchKey = RegistryKey + @"\Watch";

            /// <summary>
            /// Registry value for watched directories.
            /// </summary>
            public const string WatchValue = "dir";

            /// <summary>
            /// The keyboard map library.
            /// </summary>
            public const string Library = "Library";
        }
    }
}
