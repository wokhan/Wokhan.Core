using System;
using System.Linq;

namespace Wokhan.Core.Extensions;

/// <summary>
/// String class extensions
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Truncates a string to the specified max length (if needed)
    /// </summary>
    /// <param name="str">Source string</param>
    /// <param name="maxLen">Maximum length to truncate the string at</param>
    /// <returns></returns>
    public static string Truncate(this string str, int maxLen)
    {
        if (str is null)
        {
            return null;
        }
        return str.Length > maxLen ? str.Substring(0, maxLen) : str;
    }


    private static readonly char[] PseudoChars = new[]
    {
        'Â',
        'ß',
        'Ç',
        'Ð',
        'É',
        'F',
        'G',
        'H',
        'Ì',
        'J',
        'K',
        '£',
        'M',
        'N',
        'Ó',
        'Þ',
        'Q',
        'R',
        '§',
        'T',
        'Û',
        'V',
        'W',
        'X',
        'Ý',
        'Z',
        '?', '?', '?', '?', '?', '?',
        'á',
        'β',
        'ç',
        'δ',
        'è',
        'ƒ',
        'ϱ',
        'λ',
        'ï',
        'J',
        'ƙ',
        'ℓ',
        '₥',
        'ñ',
        'ô',
        'ƥ',
        '9',
        'ř',
        'ƨ',
        'ƭ',
        'ú',
        'Ʋ',
        'ω',
        'ж',
        '¥',
        'ƺ',
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
        if (src is null)
        {
            return null;
        }

        return new String(src.Select(x => x - 'A' > 0 && PseudoChars.Length > x - 'A' ? PseudoChars[x - 'A'] : '?').ToArray());
    }
}
