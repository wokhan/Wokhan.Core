using System;
using System.Globalization;
#if __UAP__
using Windows.Globalization;
using Windows.Globalization.DateTimeFormatting;
#endif

namespace Wokhan.UWP.Extensions
{
    public static class DateTimeExtensions
    {

#if __UAP__
        private static readonly DateTimeFormatter DfMonthAbbr;
        private static readonly DateTimeFormatter DfTime;
        private static readonly DateTimeFormatter DfTime24;
        private static readonly DateTimeFormatter DfDate;
        private static readonly DateTimeFormatter DfDay;
        private static readonly DateTimeFormatter DfFull;
#else
        public static string DfMonthAbbr = "mmm";
        public static string DfTime = "H:mm";
        public static string DfTime24 = "HH:mm";
        public static string DfDate = "dd mmm";
        public static string DfDay = "dd";
        public static string DfFull = "d";
#endif

        public static string GetWeekNumber(this DateTime date, CalendarWeekRule weekRule)
        {
            return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(date, weekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek).ToString();
        }

        public static string GetWeekNumber(CalendarWeekRule weekRule)
        {
            return DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(DateTime.Now, weekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek).ToString();
        }


#if __UAP__
        static DateTimeExtensions()
        {
            var cultures = new[] { CultureInfo.CurrentCulture.ToString() };
            DfMonthAbbr = new DateTimeFormatter("month.abbreviated", cultures);
            DfDate = new DateTimeFormatter("day month.abbreviated", cultures);
            DfDay = new DateTimeFormatter("dayofweek.abbreviated", cultures);
            DfFull = new DateTimeFormatter("dayofweek.abbreviated day month.abbreviated", cultures);

            var dtf = new DateTimeFormatter(HourFormat.Default, MinuteFormat.Default, SecondFormat.Default);

            DfTime = new DateTimeFormatter("shorttime", new[] { "en-US" }, dtf.GeographicRegion, dtf.Calendar, ClockIdentifiers.TwelveHour);
            DfTime24 = new DateTimeFormatter("shorttime", new[] { "fr-FR" }, dtf.GeographicRegion, dtf.Calendar, ClockIdentifiers.TwentyFourHour);
        }
#endif

        public static string FormatDateFull(this DateTimeOffset time) => time.ToString(DfFull);

        public static string FormatDate(this DateTimeOffset startTime, bool useFriendlyOutput = false)
        {
            if (useFriendlyOutput)
            {
                switch ((int)(startTime - DateTimeOffset.Now.Date).TotalDays)
                {
                    case 0:
                        return "PeriodToday".Translate();

                    case 1:
                        return "PeriodTomorrow".Translate();

                    case 2:
                        return "PeriodIn2days".Translate();

                    case -1:
                        return "PeriodYesterday".Translate();

                    case -2:
                        return "Period2DaysAgo".Translate();

                    default:
                        return startTime.ToString(DfDate);
                }
            }

            return (startTime == DateTime.Today ? "" : startTime.ToString(DfDate));
        }

        public static string FormatTime(double x)
        {
            return new DateTimeOffset(1, 1, 1, (int)x, (int)(x - ((int)x)) / 100 * 60, 0, TimeSpan.Zero).FormatTime();
        }

        public static string FormatTime(this DateTimeOffset startTime, bool useFriendlyOutput = false, bool enable24 = true)
        {
            if (useFriendlyOutput && DateTime.Today == startTime)
            {
                var difmin = (startTime - DateTimeOffset.Now).TotalMinutes;
                if (difmin < 0)
                {
                    return "PeriodTimeNow".Translate();
                }
                else if (difmin < 60)
                {
                    return String.Format("PeriodTimeLess60".Translate(), difmin);
                }
                else
                {
                    return String.Format("PeriodTime".Translate(), (int)difmin / 60, (int)difmin % 60);
                }
            }

            return startTime.ToString(enable24 ? DfTime24 : DfTime);
        }

        public static string FormatMonthAbbr(this DateTimeOffset now) => now.ToString(DfMonthAbbr);

        public static string FormatDay(this DateTimeOffset now) => now.ToString(DfDay);

#if __UAP__
        public static string ToString(this DateTimeOffset dt, DateTimeFormatter formatter)
        {
            return formatter.Format(dt);
        }
#endif
    }
}
