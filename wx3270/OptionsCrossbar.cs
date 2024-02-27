// <copyright file="OptionsCrossbar.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Wx3270.Contracts;
    using static Wx3270.Settings;

    /// <summary>
    /// Crossbar between the back end and the profile for options settings.
    /// </summary>
    public class OptionsCrossbar : IOpacity
    {
        /// <summary>
        /// Mark between 'Set(-defer)' and 'Set(-defer,model,...)'.
        /// </summary>
        private const string Mark = "<mark>";

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsCrossbar"/> class.
        /// </summary>
        /// <param name="app">Application context.</param>
        /// <param name="profileManager">Profile manager.</param>
        public OptionsCrossbar(Wx3270App app, IProfileManager profileManager)
        {
            this.App = app;
            this.ProfileManager = profileManager;

            // Subscribe to profile change events.
            profileManager.AddChangeTo(this.ProfileChanged);

            // Subscribe to back-end terminal name changes; push to the profile.
            // This needs to be separate from the other settings because it arrives as a separate indication
            // from the back end.
            app.BackEnd.RegisterStart(
                B3270.Indication.TerminalName,
                (name, attrs) =>
                {
                    this.ProfileManager.PushAndSave(
                        (current) =>
                        {
                            current.TerminalNameOverride =
                                attrs[B3270.Attribute.Override].Equals(B3270.Value.True) ? attrs[B3270.Attribute.Text] : string.Empty;
                        },
                        Settings.ChangeName(B3270.Setting.TermName));
                });

            // Subscribe to back-end changes; push to the profile.
            var settings = new[]
            {
                B3270.Setting.AltCursor,
                B3270.Setting.CodePage,
                B3270.Setting.Crosshair,
                B3270.Setting.CursorBlink,
                B3270.Setting.MonoCase,
                B3270.Setting.NopSeconds,
                B3270.Setting.PreferIpv4,
                B3270.Setting.PreferIpv6,
                B3270.Setting.PrinterCodePage,
                B3270.Setting.PrinterName,
                B3270.Setting.PrinterOptions,
                B3270.Setting.Retry,
                B3270.Setting.ShowTiming,
                B3270.Setting.TermName,
                B3270.Setting.Typeahead,
            };
            foreach (var setting in settings)
            {
                if (app.SettingChange.SettingsDictionary.TryGetValue(setting, out string _))
                {
                    this.BackEndToProfile(setting, app.SettingChange.SettingsDictionary);
                }
            }

            app.SettingChange.Register(this.BackEndToProfile, settings);
        }

        /// <summary>
        /// Event signaled when the opacity changes.
        /// </summary>
        public event Action<int> OpacityEvent = (percent) => { };

        /// <summary>
        /// Gets or sets the main window handle.
        /// </summary>
        public IntPtr MainWindowHandle { get; set; }

        /// <summary>
        /// Gets or sets the application context.
        /// </summary>
        private Wx3270App App { get; set; }

        /// <summary>
        /// Gets or sets the profile manager.
        /// </summary>
        private IProfileManager ProfileManager { get; set; }

        /// <summary>
        /// Merge the miscellaneous settings.
        /// </summary>
        /// <param name="toProfile">Profile to merge into.</param>
        /// <param name="fromProfile">Profile to merge from.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if a merge was needed.</returns>
        [Merge(ImportType.OtherSettingsReplace)]
        public static bool Merge(Profile toProfile, Profile fromProfile, ImportType importType)
        {
            if (toProfile.ColorMode == fromProfile.ColorMode &&
                toProfile.CrosshairCursor == fromProfile.CrosshairCursor &&
                toProfile.CursorBlink == fromProfile.CursorBlink &&
                toProfile.CursorType == fromProfile.CursorType &&
                toProfile.HostCodePage == fromProfile.HostCodePage &&
                toProfile.MenuBar == fromProfile.MenuBar &&
                toProfile.Model == fromProfile.Model &&
                toProfile.Monocase == fromProfile.Monocase &&
                toProfile.NopInterval == fromProfile.NopInterval &&
                toProfile.OpacityPercent == fromProfile.OpacityPercent &&
                toProfile.Oversize.Equals(fromProfile.Oversize) &&
                toProfile.PreferIpv4 == fromProfile.PreferIpv4 &&
                toProfile.PreferIpv6 == fromProfile.PreferIpv6 &&
                toProfile.Printer == fromProfile.Printer &&
                toProfile.PrinterType == fromProfile.PrinterType &&
                toProfile.PrinterCodePage == fromProfile.PrinterCodePage &&
                toProfile.PrinterOptions == fromProfile.PrinterOptions &&
                toProfile.Retry == fromProfile.Retry &&
                toProfile.ScrollBar == fromProfile.ScrollBar &&
                toProfile.ShowTiming == fromProfile.ShowTiming &&
                toProfile.TerminalNameOverride == fromProfile.TerminalNameOverride &&
                toProfile.Typeahead == fromProfile.Typeahead &&
                toProfile.WindowTitle == fromProfile.WindowTitle)
            {
                return false;
            }

            toProfile.ColorMode = fromProfile.ColorMode;
            toProfile.CrosshairCursor = fromProfile.CrosshairCursor;
            toProfile.CursorBlink = fromProfile.CursorBlink;
            toProfile.CursorType = fromProfile.CursorType;
            toProfile.HostCodePage = fromProfile.HostCodePage;
            toProfile.MenuBar = fromProfile.MenuBar;
            toProfile.Model = fromProfile.Model;
            toProfile.Monocase = fromProfile.Monocase;
            toProfile.NopInterval = fromProfile.NopInterval;
            toProfile.OpacityPercent = fromProfile.OpacityPercent;
            toProfile.Oversize = fromProfile.Oversize.Clone();
            toProfile.PreferIpv4 = fromProfile.PreferIpv4;
            toProfile.PreferIpv6 = fromProfile.PreferIpv6;
            toProfile.Printer = fromProfile.Printer;
            toProfile.PrinterType = fromProfile.PrinterType;
            toProfile.PrinterCodePage = fromProfile.PrinterCodePage;
            toProfile.PrinterOptions = fromProfile.PrinterOptions;
            toProfile.Retry = fromProfile.Retry;
            toProfile.ScrollBar = fromProfile.ScrollBar;
            toProfile.ShowTiming = fromProfile.ShowTiming;
            toProfile.TerminalNameOverride = fromProfile.TerminalNameOverride;
            toProfile.Typeahead = fromProfile.Typeahead;
            toProfile.WindowTitle = fromProfile.WindowTitle;
            return true;
        }

        /// <summary>
        /// Construct a value for toggling the model.
        /// </summary>
        /// <param name="colorMode">Color mode.</param>
        /// <param name="model">Model number.</param>
        /// <param name="extendedMode">Extended mode.</param>
        /// <param name="rows">Number of rows.</param>
        /// <param name="columns">Number of columns.</param>
        /// <returns>List of arguments.</returns>
        private static IEnumerable<string> ModelSettings(bool colorMode, int model, bool extendedMode, int rows, int columns)
        {
            var startupConfig = new StartupConfig
            {
                Model = model,
                ColorMode = colorMode,
                ExtendedMode = extendedMode,
                OversizeRows = rows,
                OversizeColumns = columns,
            };
            return new[]
            {
                B3270.Value.Defer,
                B3270.Setting.Model, startupConfig.ModelParameter,
                B3270.Setting.Oversize, startupConfig.OversizeParameter,
            };
        }

        /// <summary>
        /// Completion method for model setting.
        /// </summary>
        /// <param name="cookie">Cookie (unused).</param>
        /// <param name="success">True if Set() was successful.</param>
        /// <param name="result">Result text.</param>
        /// <param name="misc">Miscellaneous attributes (ingnored).</param>
        private void ModelSetComplete(object cookie, bool success, string result, AttributeDict misc)
        {
            // Handle failures.
            if (!success)
            {
                ErrorBox.Show(result, I18n.Get(Settings.Title.Settings));
                return;
            }

            // Separate out the output before and after the mark.
            var before = new List<string>();
            var after = new List<string>();
            var marked = false;
            foreach (var line in result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line == Mark)
                {
                    marked = true;
                }
                else if (!marked)
                {
                    before.Add(line);
                }
                else
                {
                    after.Add(line);
                }
            }

            if (after.Except(before).Any())
            {
                // Something new was deferred.
                ErrorBox.ShowWithStop(
                    this.MainWindowHandle,
                    I18n.Get(Message.DeferredUntilDisconnectedPopUp),
                    I18n.Get(Title.Settings),
                    Constants.StopKey.Deferred);
            }
        }

        /// <summary>
        /// The profile has changed. Update the back end.
        /// </summary>
        /// <param name="oldProfile">Old profile.</param>
        /// <param name="newProfile">New profile.</param>
        private void ProfileChanged(Profile oldProfile, Profile newProfile)
        {
            var newSettings = new List<string>();

            // Check for model changes.
            if (oldProfile == null
                || oldProfile.ColorMode != newProfile.ColorMode
                || oldProfile.ExtendedMode != newProfile.ExtendedMode
                || oldProfile.Model != newProfile.Model
                || !oldProfile.Oversize.Equals(newProfile.Oversize))
            {
                var modelSettings = ModelSettings(
                    newProfile.ColorMode,
                    newProfile.Model,
                    newProfile.ExtendedMode,
                    newProfile.Oversize.Rows,
                    newProfile.Oversize.Columns);
                this.App.BackEnd.RunActions(
                    new[]
                    {
                        new BackEndAction(B3270.Action.Set, B3270.Value.Defer),
                        new BackEndAction(B3270.Action.Echo, Mark),
                        new BackEndAction(B3270.Action.Set, modelSettings),
                        new BackEndAction(B3270.Action.Set, B3270.Value.Defer),
                    },
                    this.ModelSetComplete);
            }

            // Check for other changes.
            if (oldProfile == null || oldProfile.AlwaysInsert != newProfile.AlwaysInsert)
            {
                newSettings.AddRange(new[] { B3270.Setting.AlwaysInsert, newProfile.AlwaysInsert ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.CrosshairCursor != newProfile.CrosshairCursor)
            {
                newSettings.AddRange(new[] { B3270.Setting.Crosshair, newProfile.CrosshairCursor ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.CursorBlink != newProfile.CursorBlink)
            {
                newSettings.AddRange(new[] { B3270.Setting.CursorBlink, newProfile.CursorBlink ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.CursorType != newProfile.CursorType)
            {
                newSettings.AddRange(new[] { B3270.Setting.AltCursor, newProfile.CursorType == CursorType.Underscore ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.HostCodePage != newProfile.HostCodePage)
            {
                newSettings.AddRange(new[] { B3270.Setting.CodePage, newProfile.HostCodePage });
            }

            if (oldProfile == null || oldProfile.Monocase != newProfile.Monocase)
            {
                newSettings.AddRange(new[] { B3270.Setting.MonoCase, newProfile.Monocase ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.NopInterval != newProfile.NopInterval)
            {
                newSettings.AddRange(new[] { B3270.Setting.NopSeconds, newProfile.NopInterval.ToString() });
            }

            if (oldProfile == null || oldProfile.OpacityPercent != newProfile.OpacityPercent)
            {
                this.OpacityEvent(newProfile.OpacityPercent);
            }

            if (oldProfile == null || oldProfile.PreferIpv4 != newProfile.PreferIpv4)
            {
                newSettings.AddRange(new[] { B3270.Setting.PreferIpv4, newProfile.PreferIpv4 ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.PreferIpv6 != newProfile.PreferIpv6)
            {
                newSettings.AddRange(new[] { B3270.Setting.PreferIpv6, newProfile.PreferIpv6 ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.PrinterCodePage != newProfile.PrinterCodePage)
            {
                newSettings.AddRange(new[] { B3270.Setting.PrinterCodePage, newProfile.PrinterCodePage });
            }

            if (oldProfile == null || oldProfile.Printer != newProfile.Printer)
            {
                newSettings.AddRange(new[] { B3270.Setting.PrinterName, newProfile.Printer });
            }

            if (oldProfile == null || oldProfile.PrinterOptions != newProfile.PrinterOptions)
            {
                var sentOptions = this.App.Pr3287TraceOptions + newProfile.PrinterOptions;
                if (this.App.Restricted(Restrictions.ExternalFiles))
                {
                    // Remove the pr3287 trace option.
                    sentOptions = Regex.Replace(sentOptions, Pr3287.CommandLineOption.TraceRegex, string.Empty);
                }

                newSettings.AddRange(new[] { B3270.Setting.PrinterOptions, sentOptions });
            }

            if (oldProfile == null || oldProfile.Retry != newProfile.Retry)
            {
                newSettings.AddRange(new[] { B3270.Setting.Retry, newProfile.Retry ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.ShowTiming != newProfile.ShowTiming)
            {
                newSettings.AddRange(new[] { B3270.Setting.ShowTiming, newProfile.ShowTiming ? B3270.Value.True : B3270.Value.False });
            }

            if (oldProfile == null || oldProfile.TerminalNameOverride != newProfile.TerminalNameOverride)
            {
                newSettings.AddRange(new[] { B3270.Setting.TermName, newProfile.TerminalNameOverride });
            }

            if (oldProfile == null || oldProfile.Typeahead != newProfile.Typeahead)
            {
                newSettings.AddRange(new[] { B3270.Setting.Typeahead, newProfile.Typeahead ? B3270.Value.True : B3270.Value.False });
            }

            // Tell the back end about everything that should succeed (was created programmatically), which in general should not fail.
            if (newSettings.Count > 0)
            {
                this.App.BackEnd.RunAction(new BackEndAction(B3270.Action.Set, newSettings), ErrorBox.Completion(I18n.Get(Settings.Title.Settings)));
            }
        }

        /// <summary>
        /// Get the change name for a setting.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <returns>Change name.</returns>
        private string GetChangeName(string name)
        {
            return Wx3270.ProfileManager.ChangeName(string.Format("{0} ({1})", I18n.Get(Settings.SettingPath(name)), this.ProfileManager.ExternalText));
        }

        /// <summary>
        /// A back-end setting changed. Send it to the profile.
        /// </summary>
        /// <param name="name">Setting name.</param>
        /// <param name="settingDictionary">Settings dictionary (values).</param>
        private void BackEndToProfile(string name, SettingsDictionary settingDictionary)
        {
            if (!this.App.BackEnd.Ready || !settingDictionary.TryGetValue(name, out string stringValue))
            {
                return;
            }

            var booleanSettings = new[]
            {
                B3270.Setting.AltCursor,
                B3270.Setting.Crosshair,
                B3270.Setting.CursorBlink,
                B3270.Setting.MonoCase,
                B3270.Setting.PreferIpv4,
                B3270.Setting.PreferIpv6,
                B3270.Setting.Retry,
                B3270.Setting.ShowTiming,
                B3270.Setting.Typeahead,
            };
            var booleanValue = false;
            if (booleanSettings.Contains(name) && !settingDictionary.TryGetValue(name, out booleanValue))
            {
                return;
            }

            var intSettings = new[] { B3270.Setting.NopSeconds };
            var intValue = 0;
            if (intSettings.Contains(name) && !settingDictionary.TryGetValue(name, out intValue))
            {
                return;
            }

            switch (name)
            {
                case B3270.Setting.AltCursor:
                    this.ProfileManager.PushAndSave((current) => current.CursorType = booleanValue ? CursorType.Underscore : CursorType.Block, this.GetChangeName(name));
                    break;
                case B3270.Setting.CodePage:
                    if (this.App.CodePageDb.Index(stringValue) >= 0)
                    {
                        this.ProfileManager.PushAndSave((current) => current.HostCodePage = stringValue, this.GetChangeName(name));
                    }

                    break;
                case B3270.Setting.Crosshair:
                    this.ProfileManager.PushAndSave((current) => current.CrosshairCursor = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.CursorBlink:
                    this.ProfileManager.PushAndSave((current) => current.CursorBlink = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.MonoCase:
                    this.ProfileManager.PushAndSave((current) => current.Monocase = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.NopSeconds:
                    this.ProfileManager.PushAndSave((current) => current.NopInterval = intValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.PreferIpv4:
                    this.ProfileManager.PushAndSave((current) => current.PreferIpv4 = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.PreferIpv6:
                    this.ProfileManager.PushAndSave((current) => current.PreferIpv6 = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.PrinterCodePage:
                    this.ProfileManager.PushAndSave((current) => current.PrinterCodePage = stringValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.PrinterName:
                    this.ProfileManager.PushAndSave((current) => current.Printer = stringValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.PrinterOptions:
                    // Strip trace options.
                    var trace = this.App.Pr3287TraceOptions;
                    if (!string.IsNullOrEmpty(trace) && stringValue.StartsWith(trace))
                    {
                        stringValue = stringValue.Remove(0, trace.Length);
                    }

                    this.ProfileManager.PushAndSave((current) => current.PrinterOptions = stringValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.Retry:
                    this.ProfileManager.PushAndSave((current) => current.Retry = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.ShowTiming:
                    this.ProfileManager.PushAndSave((current) => current.ShowTiming = booleanValue, this.GetChangeName(name));
                    break;
                case B3270.Setting.Typeahead:
                    this.ProfileManager.PushAndSave((current) => current.Typeahead = booleanValue, this.GetChangeName(name));
                    break;
            }
        }
    }
}