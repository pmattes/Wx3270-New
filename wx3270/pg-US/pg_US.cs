// <copyright file="pg_US.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace pg_US
{
    /// <summary>
    /// Translations for Pig Latin.
    /// </summary>
    /// <remarks>Makes words longer. Used to exercise adjustable forms layouts.</remarks>
    public static class pg_US
    {
        /// <summary>
        /// Localize a word.
        /// </summary>
        /// <param name="s">Value in US English</param>
        /// <returns>Translated string</returns>
        public static string LocalizeWord(string s)
        {
            var newWord = s;
            var leftPunct = string.Empty;
            var rightPunct = string.Empty;

            // Strip off punctuation.
            while (newWord.Length > 0 && char.IsPunctuation(newWord[0]))
            {
                leftPunct += newWord.Substring(0, 1);
                newWord = newWord.Substring(1);
            }

            while (newWord.Length > 0 && char.IsPunctuation(newWord[newWord.Length - 1]))
            {
                rightPunct = newWord.Substring(newWord.Length - 1) + rightPunct;
                newWord = newWord.Substring(0, newWord.Length - 1);
            }

            if (string.IsNullOrEmpty(newWord))
            {
                // All punctuation.
                return newWord;
            }

            // If it already starts with a vowel, it's easy.
            if ("AEIOUaeiou".Contains(newWord.Substring(0, 1)))
            {
                return newWord += "ay";
            }

            // Check for starting with an uppercase letter.
            var upper = char.IsUpper(newWord[0]);
            if (upper)
            {
                newWord = newWord.Substring(0, 1).ToLowerInvariant() + newWord.Substring(1);
            }

            // Isolate initial consonants.
            var consonants = string.Empty;
            while (newWord.Length > 0 && !"AEIOUaeiou".Contains(newWord.Substring(0, 1)))
            {
                consonants += newWord.Substring(0, 1);
                newWord = newWord.Substring(1);
            }

            // Recombine.
            newWord += consonants + "ay";

            // Restore case.
            if (upper)
            {
                newWord = newWord.Substring(0, 1).ToUpperInvariant() + newWord.Substring(1);
            }

            return leftPunct + newWord + rightPunct;
        }
    }
}
