// <copyright file="Release.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    /// <summary>
    /// Information about the release, beyond the Assembly Version.
    /// </summary>
    public static class Release
    {
        /// <summary>
        /// The pre-release iteration, generally a capital letter. NULL for tagged releases.
        /// </summary>
        /// <remarks>
        /// CHANGE THIS WHENEVER PRE-RELEASE CODE IS CUT, AND MAKE SURE IT IS NULL WHEN A TAGGED RELEASE IS CUT.
        /// </remarks>
        private const string PreReleaseIteration = null;

        /// <summary>
        /// Release phases.
        /// </summary>
        public enum Phases
        {
            /// <summary>
            /// Alpha release.
            /// </summary>
            Alpha,

            /// <summary>
            /// Beta release.
            /// </summary>
            Beta,

            /// <summary>
            /// General availability release.
            /// </summary>
            GA,
        }

        /// <summary>
        /// Gets the release phase, or the pending release phase.
        /// </summary>
        /// <remarks>
        /// CHANGE THIS WHEN THE RELEASE PHASE CHANGES.
        /// </remarks>
        public static Phases Phase => Phases.Alpha;

        /// <summary>
        /// Gets the name of the release phase.
        /// </summary>
        public static string PhaseName
        {
            get
            {
                if (!string.IsNullOrEmpty(PreReleaseIteration))
                {
                    return PreReleaseIteration + "pre";
                }

                return Phase.ToString().ToLower();
            }
        }
    }
}
