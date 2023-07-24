// <copyright file="IProfileManager.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Change handler delegate.
    /// </summary>
    /// <param name="profile">New profile.</param>
    public delegate void ChangeHandler(Profile profile);

    /// <summary>
    /// Change-to handler delegate.
    /// </summary>
    /// <param name="oldProfile">Old profile.</param>
    /// <param name="newProfile">New profile.</param>
    public delegate void ChangeToHandler(Profile oldProfile, Profile newProfile);

    /// <summary>
    /// List change handler delegate.
    /// </summary>
    /// <param name="names">New list of file names.</param>
    public delegate void ListChangeHandler(IEnumerable<string> names);

    /// <summary>
    /// Pending changes handler delegate.
    /// </summary>
    /// <param name="pending">True if changes are now pending.</param>
    public delegate void ChangesPendingHandler(bool pending);

    /// <summary>
    /// Merge handler delegate.
    /// </summary>
    /// <param name="toProfile">Current profile, might be returned modified.</param>
    /// <param name="fromProfile">Profile to merge in.</param>
    /// <param name="importType">Import type.</param>
    /// <returns>True if the profile changed.</returns>
    public delegate bool MergeHandler(Profile toProfile, Profile fromProfile, ImportType importType);

    /// <summary>
    /// Change action.
    /// </summary>
    /// <param name="current">Profile to change.</param>
    public delegate void ChangeAction(Profile current);

    /// <summary>
    /// Safety check handler.
    /// </summary>
    /// <param name="oldProfile">Old profile.</param>
    /// <param name="newProfile">New profile.</param>
    /// <param name="safe">Returned false if it is not safe to make a change.</param>
    public delegate void SafetyCheckHandler(Profile oldProfile, Profile newProfile, ref bool safe);

    /// <summary>
    /// Refocus handler.
    /// </summary>
    /// <param name="profileDir">Pathname of directory where the profile is.</param>
    /// <param name="profileName">Profile that was re-added.</param>
    /// <param name="hostName">Host that was re-added, or null.</param>
    public delegate void RefocusHandler(string profileDir, string profileName, string hostName);

    /// <summary>
    /// Old version delegate.
    /// </summary>
    /// <param name="oldVersion">Old version.</param>
    /// <param name="saved">Set to true if the profile was modified and saved.</param>
    public delegate void OldVersionHandler(Profile.VersionClass oldVersion, ref bool saved);

    /// <summary>
    /// Profile manager.
    /// </summary>
    public interface IProfileManager
    {
        /// <summary>
        /// The change event.
        /// </summary>
        event ChangeHandler Change;

        /// <summary>
        /// The change-to event.
        /// </summary>
        event ChangeToHandler ChangeTo;

        /// <summary>
        /// The final change event. Called after <see cref="Change"/>.
        /// </summary>
        event Action<Profile, bool> ChangeFinal;

        /// <summary>
        /// The list change event.
        /// </summary>
        event ListChangeHandler ListChange;

        /// <summary>
        /// The pending changes event.
        /// </summary>
        event ChangesPendingHandler ChangesPending;

        /// <summary>
        /// The profile change safety check event.
        /// </summary>
        event SafetyCheckHandler SafetyCheck;

        /// <summary>
        /// The profile refocus event.
        /// </summary>
        event RefocusHandler RefocusEvent;

        /// <summary>
        /// The event for default profile changes.
        /// </summary>
        event Action DefaultProfileChanged;

        /// <summary>
        /// A profile was just opened.
        /// </summary>
        event ChangeHandler NewProfileOpened;

        /// <summary>
        /// A profile is about to close.
        /// </summary>
        event ChangeHandler ProfileClosing;

        /// <summary>
        /// A profile with an older version was loaded.
        /// </summary>
        event OldVersionHandler OldVersion;

        /// <summary>
        /// Gets the current profile.
        /// </summary>
        Profile Current { get; }

        /// <summary>
        /// Gets the localized text for an external change.
        /// </summary>
        string ExternalText { get; }

        /// <summary>
        /// Sets up the profile list.
        /// </summary>
        /// <param name="profileTracker">Profile tracker.</param>
        void SetProfileList(IProfileTracker profileTracker);

        /// <summary>
        /// Ensures that the profile directory and default profile exist.
        /// </summary>
        void CreateProfileDirectoryAndProfile();

        /// <summary>
        /// Load a profile, i.e., make some profile current.
        /// </summary>
        /// <param name="profilePath">Full profile pathname.</param>
        /// <param name="readOnly">If true, open read-only.</param>
        /// <param name="doErrorPopups">If true, do pop-ups for errors.</param>
        /// <returns>True if load was successful.</returns>
        bool Load(string profilePath, bool readOnly = false, bool doErrorPopups = true);

        /// <summary>
        /// Load a profile, i.e., make some profile current.
        /// </summary>
        /// <param name="profilePath">Full profile pathname.</param>
        /// <param name="outProfilePath">Returned full profile path.</param>
        /// <param name="readOnly">If true, open read-only.</param>
        /// <param name="doErrorPopups">If true, do pop-ups for errors.</param>
        /// <returns>True if load was successful.</returns>
        bool Load(string profilePath, out string outProfilePath, bool readOnly = false, bool doErrorPopups = true);

        /// <summary>
        /// Merge data from another profile.
        /// </summary>
        /// <param name="destProfile">Destination profile.</param>
        /// <param name="mergeProfilePath">Source profile path.</param>
        /// <param name="importType">Import type.</param>
        /// <returns>True if the file was successfully read.</returns>
        bool Merge(Profile destProfile, string mergeProfilePath, ImportType importType);

        /// <summary>
        /// Merge from a given profile.
        /// </summary>
        /// <param name="destProfile">Destination profile.</param>
        /// <param name="mergeProfile">Profile to get settings from.</param>
        /// <param name="importType">Type of import operation.</param>
        /// <returns>True if anything changed.</returns>
        bool Merge(Profile destProfile, Profile mergeProfile, ImportType importType);

        /// <summary>
        /// Undo the last change.
        /// </summary>
        /// <param name="skipInvert">True to skip inversion.</param>
        void Undo(bool skipInvert = false);

        /// <summary>
        /// Redo the last un-done change.
        /// </summary>
        void Redo();

        /// <summary>
        /// Act as if the profile were read for the first time.
        /// </summary>
        void PushFirst();

        /// <summary>
        /// Save a profile.
        /// </summary>
        /// <param name="profilePathName">Profile path name.</param>
        /// <param name="profile">Profile to save.</param>
        /// <returns>True if save succeeded.</returns>
        bool Save(string profilePathName = null, Profile profile = null);

        /// <summary>
        /// Save (and keep open) the default profile.
        /// </summary>
        /// <param name="stream">Returned open file stream.</param>
        /// <returns>True if save succeeded.</returns>
        bool SaveDefault(out FileStream stream);

        /// <summary>
        /// Handle a profile error.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <remarks>
        /// Needed because some errors popping up from handlers cause screen drawing to go haywire.
        /// </remarks>
        void ProfileError(string message);

        /// <summary>
        /// Dump out pending profile errors.
        /// </summary>
        void DumpErrors();

        /// <summary>
        /// Push a change and save it.
        /// </summary>
        /// <param name="action">Action to perform to change the profile.</param>
        /// <param name="what">Summary of the action.</param>
        /// <param name="profileToChange">Profile to modify, defaults to current.</param>
        /// <param name="refocus">Optional refocus handler.</param>
        /// <returns>True if changes actually occurred.</returns>
        bool PushAndSave(ChangeAction action, string what, Profile profileToChange = null, IRefocus refocus = null);

        /// <summary>
        /// Register a merge handler.
        /// </summary>
        /// <param name="importType">Import type.</param>
        /// <param name="handler">Merge handler.</param>
        void RegisterMerge(ImportType importType, MergeHandler handler);

        /// <summary>
        /// Register Undo and Redo buttons.
        /// </summary>
        /// <param name="undo">Undo button.</param>
        /// <param name="redo">Redo button.</param>
        /// <param name="toolTip">Tool tip.</param>
        void RegisterUndoRedo(Button undo, Button redo, ToolTip toolTip);

        /// <summary>
        /// Register Undo and Redo menu items.
        /// </summary>
        /// <param name="undo">Undo menu item.</param>
        /// <param name="redo">Redo menu item.</param>
        /// <param name="toolTip">Tool tip.</param>
        void RegisterUndoRedo(ToolStripMenuItem undo, ToolStripMenuItem redo, ToolTip toolTip);

        /// <summary>
        /// Set the profile to all default values, treating it like an external change.
        /// </summary>
        void SetToDefaults();

        /// <summary>
        /// Flush the undo and redo stacks.
        /// </summary>
        void FlushUndoRedo();

        /// <summary>
        /// Push a new undo object on the stack.
        /// </summary>
        /// <param name="action">Undo/redo object.</param>
        void PushConfigAction(ConfigAction action);

        /// <summary>
        /// Tests a profile pathname for being the current profile.
        /// </summary>
        /// <param name="profilePathName">Profile name.</param>
        /// <returns>True if it is the name of the current profile.</returns>
        bool IsCurrentPathName(string profilePathName);

        /// <summary>
        /// Tests a profile pathname for being the default profile.
        /// </summary>
        /// <param name="profilePathName">Profile name.</param>
        /// <returns>True if it is the name of the default profile.</returns>
        bool IsDefaultPathName(string profilePathName);

        /// <summary>
        /// Signal the <see cref="RefocusEvent"/> event.
        /// </summary>
        /// <param name="profileDir">Profile directory.</param>
        /// <param name="profileName">Profile name.</param>
        /// <param name="hostName">Host name, or null.</param>
        void Refocus(string profileDir, string profileName = null, string hostName = null);

        /// <summary>
        /// Set the default profile.
        /// </summary>
        /// <param name="profile">Profile to use as default.</param>
        void SetDefaultProfile(Profile profile);

        /// <summary>
        /// Closes the current profile before exiting.
        /// </summary>
        /// <param name="profile">Profile to close.</param>
        void Close(Profile profile = null);

        /// <summary>
        /// Returns the localized version of "change xyz".
        /// </summary>
        /// <param name="text">Attribute to format (already localized).</param>
        /// <returns>Attribute localized with "change".</returns>
        string ChangeName(string text);

        /// <summary>
        /// Returns the localized version of "disable xyz".
        /// </summary>
        /// <param name="text">Attribute to format (already localized).</param>
        /// <returns>Attribute localized with "disable".</returns>
        string DisableName(string text);
    }
}
