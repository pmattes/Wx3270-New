// <copyright file="ru-XX.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace ru_XX
{
    /// <summary>
    /// Translations to naively transliterated Cyrillic.
    /// </summary>
    /// <remarks>Used to catch missing internationalization.</remarks>
    public static class ru_XX
    {
        /// <summary>
        /// Latin to Cyrillic transliteration.
        /// </summary>
        private static readonly Dictionary<char, char> toCyrillic = new Dictionary<char, char>
        {
            ['a'] = 'a',
            ['A'] = 'А',
            ['b'] = 'б',
            ['B'] = 'Б',
            ['c'] = 'ц',
            ['C'] = 'Ц',
            ['d'] = 'д',
            ['D'] = 'Д',
            ['e'] = 'е',
            ['E'] = 'Е',
            ['f'] = 'ф',
            ['F'] = 'Ф',
            ['g'] = 'г',
            ['H'] = 'Г',
            ['i'] = 'и',
            ['I'] = 'И',
            ['j'] = 'й',
            ['J'] = 'Й',
            ['k'] = 'к',
            ['K'] = 'К',
            ['l'] = 'л',
            ['L'] = 'Л',
            ['m'] = 'м',
            ['M'] = 'М',
            ['n'] = 'н',
            ['N'] = 'Н',
            ['o'] = 'о',
            ['O'] = 'О',
            ['p'] = 'п',
            ['P'] = 'П',
            ['r'] = 'р',
            ['R'] = 'Р',
            ['s'] = 'с',
            ['S'] = 'С',
            ['t'] = 'т',
            ['T'] = 'Т',
            ['u'] = 'у',
            ['U'] = 'У',
            ['v'] = 'в',
            ['V'] = 'В',
            ['x'] = 'х',
            ['X'] = 'Х',
            ['y'] = 'ы',
            ['Y'] = 'Ы',
            ['z'] = 'з',
            ['Z'] = 'З'
        };

        /// <summary>
        /// Localize a word.
        /// </summary>
        /// <param name="s">Value in US English</param>
        /// <returns>Translated string</returns>
        public static string LocalizeWord(string s)
        {
            // Transliterate Latin to Cyrillic.
            var cyrillic = string.Empty;
            foreach (var c in s)
            {
                cyrillic += toCyrillic.ContainsKey(c) ? toCyrillic[c] : c;
            }

            return cyrillic;
        }
    }
}
