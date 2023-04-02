using System;

namespace Wokhan.Core;

/// <summary>
/// A static class offering a single method to help formatting numeric values.
/// </summary>
public static class UnitFormatter
{

    private static string[] defaultUnits = new[] { "", "K", "M", "G", "T" };

    /// <summary>
    /// Formats a given value (size, bandwitdth, ...) using the default units (["", "K", "M", "G", "T"]) or the specified one.
    /// For instance FormatValue(1_000_000, "bps") will return "1Mbps" and FormatValue(2_345_000, units: ["", "KiB", "MiB", "GiB", "TiB"]) will return "2.345MiB".
    /// </summary>
    /// <param name="value"></param>
    /// <param name="suffix">An optional suffix which will get added to the output string (for instance, 'bps').</param>
    /// <param name="mathBase">An optional mathematical base (1000, 1024) depending on the unit you'll use.</param>
    /// <param name="units">An array containing the units (related to the base you use).</param>
    /// <returns></returns>
    public static string FormatValue(double value, string? suffix = null, int mathBase = 1000, string[]? units = null)
    {
        units ??= defaultUnits;

        var num = Math.Max(0, Math.Min(units.Length - 1, (int)Math.Log(value, mathBase)));
        value = (int)(value / Math.Pow(mathBase, num) * 100) / 100;

        return $"{value:#0.##}{units[num]}{suffix}";
    }

    static string[] units = new[] { "B", "KiB", "MiB", "GiB", "TiB" };

    [Obsolete("This method has been deprecated. Please use FormatUnit instead.")]
    public static string FormatBytes(double size, string? suffix = null)
    {
        int i;
        for (i = 0; size > 1024.0; size = size / 1024.0)
            i++;

        return String.Format("{0:#0.##}{1}{2}", size, units[i], suffix);
    }
}
