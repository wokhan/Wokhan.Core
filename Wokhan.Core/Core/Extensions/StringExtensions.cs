using System;
using System.Collections.Generic;
using System.Linq;

namespace Wokhan.Core.Extensions
{
    /// <summary>
    /// String class extensions
    /// </summary>
    public static class StringExtensions
    {


        /// <summary>
        /// Truncates a string to the specified max length (if needed)
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="maxlength">Maximum length to truncate the string at</param>
        /// <returns></returns>
        public static string Truncate(this string str, int maxLen)
        {
            return str.Length > maxLen ? str.Substring(0, maxLen) : str;
        }


        private static readonly Dictionary<char, char> PseudoChars = new Dictionary<char, char>
        {
            ['a'] = 'á',
            ['b'] = 'β',
            ['c'] = 'ç',
            ['d'] = 'δ',
            ['e'] = 'è',
            ['f'] = 'ƒ',
            ['g'] = 'ϱ',
            ['h'] = 'λ',
            ['i'] = 'ï',
            ['j'] = 'J',
            ['k'] = 'ƙ',
            ['l'] = 'ℓ',
            ['m'] = '₥',
            ['n'] = 'ñ',
            ['o'] = 'ô',
            ['p'] = 'ƥ',
            ['q'] = '9',
            ['r'] = 'ř',
            ['s'] = 'ƨ',
            ['t'] = 'ƭ',
            ['u'] = 'ú',
            ['v'] = 'Ʋ',
            ['w'] = 'ω',
            ['x'] = 'ж',
            ['y'] = '¥',
            ['z'] = 'ƺ',
            ['A'] = 'Â',
            ['B'] = 'ß',
            ['C'] = 'Ç',
            ['D'] = 'Ð',
            ['E'] = 'É',
            ['F'] = 'F',
            ['G'] = 'G',
            ['H'] = 'H',
            ['I'] = 'Ì',
            ['J'] = 'J',
            ['K'] = 'K',
            ['L'] = '£',
            ['M'] = 'M',
            ['N'] = 'N',
            ['O'] = 'Ó',
            ['P'] = 'Þ',
            ['Q'] = 'Q',
            ['R'] = 'R',
            ['S'] = '§',
            ['T'] = 'T',
            ['U'] = 'Û',
            ['V'] = 'V',
            ['W'] = 'W',
            ['X'] = 'X',
            ['Y'] = 'Ý',
            ['Z'] = 'Z'
        };

        /// <summary>
        /// Turns a string into pseudo string (latin languages only).
        /// Helpful when translating UI to quickly see untranslated part.
        /// <code>
        /// "window".ToPseudo() returns "ωïñδôω"
        /// </code>
        /// </summary>
        /// <param name="src"></param>
        /// <returns>The transformed string</returns>
        public static string ToPseudo(this string src)
        {
            if (src == null)
            {
                return null;
            }

            return new String(src.Select(x => PseudoChars.TryGetValue(x, out var r) ? r : x).ToArray());
        }
    }

}
