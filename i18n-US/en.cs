// <copyright file = "en.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace en
{
    /// <summary>
    /// Translations for non-US English.
    /// </summary>
#pragma warning disable IDE1006 // Naming Styles
    public static class en
    {
        /// <summary>
        /// Dictionary of US-to-British spellings.
        /// </summary>
        private static readonly Dictionary<string, string> map = new Dictionary<string, string>
        {
            {  "color", "colour" },
            {  "colors", "colours" },
            {  "Color", "Colour" },
            {  "Colors", "Colours" },
            {  "gray", "grey" },
            {  "Gray", "Grey" },
        };

        /// <summary>
        /// Localize a word.
        /// </summary>
        /// <param name="s">Value in US English.</param>
        /// <returns>Localized string.</returns>
        public static string LocalizeWord(string s)
        {
            var ret = s;
            foreach (var pair in map)
            {
                ret = ret.Replace(pair.Key, pair.Value);
            }

            return ret;
        }
    }
}