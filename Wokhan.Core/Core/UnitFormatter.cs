using System;

namespace Wokhan.Core.Core;

public static class UnitFormatter
{
    static string[] units = new[] { "B", "KiB", "MiB", "GiB", "TiB" };
    public static string FormatBytes(double size, string? suffix = null)
    {
        int i;
        for (i = 0; size > 1024.0; size = size / 1024.0)
            i++;

        return String.Format("{0:#0.##}{1}{2}", size, units[i], suffix);
    }
}
