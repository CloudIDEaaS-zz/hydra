using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Utils
{
    public static class DateTimeExtensions
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static unsafe extern bool QueryPerformanceCounter(long* lpPerformanceCount);

        public static long HighResolutionPerformanceCount
        {
            get
            {
                long performanceCount = 0;

                unsafe
                {
                    QueryPerformanceCounter(&performanceCount);
                }

                return performanceCount;
            }
        }

        public static int NextClosetDivisible(this int n, int shift, int multiplier = 10)
        {
            shift.Countdown((n2) =>
            {
                n *= multiplier;
            });

            return n;
        }

        public static DateTime ToUniversalNoConvert(this DateTime dateTime)
        {
            return dateTime.ToKind(DateTimeKind.Utc);
        }

        public static DateTime ToKind(this DateTime dateTime, DateTimeKind kind)
        {
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, kind);

            return dateTime;
        }

        public static float GetDecimalTimeComponent(this TimeSpan timeSpan, Func<TimeSpan, int> getTimeComponent, int decimals)
        {
            var component = getTimeComponent(timeSpan);
            var divisor = (float) 10.NextClosetDivisible(decimals);
            var fComponent = component.As<float>() / divisor;
            var time = Math.Round(fComponent, decimals);

            return (float)time;
        }

        public static string ToXmlDateTimeString(this DateTime dateTime, XmlDateTimeSerializationMode mode = XmlDateTimeSerializationMode.Utc)
        {
            return XmlConvert.ToString(dateTime, mode);
        }

        public static uint ToEpocTime(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;

            return (uint) diff.TotalSeconds;
        }

        public static string ToSortableDateTimeText(this DateTime time)
        {
            return time.ToString("yyyyMMdd_HHmmss_fffffff");
        }

        public static bool IsSortableDateTimeText(string text, out DateTime dateTime)
        {
            var regex = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})_");

            if (regex.IsMatch(text))
            {
                var match = regex.Match(text);
                var year = int.Parse(match.GetGroupValue("year"));
                var month = int.Parse(match.GetGroupValue("month"));
                var day = int.Parse(match.GetGroupValue("day"));
                var hour = int.Parse(match.GetGroupValue("hour"));
                var minute = int.Parse(match.GetGroupValue("minute"));
                var second = int.Parse(match.GetGroupValue("second"));

                dateTime = new DateTime(year, month, day, hour, minute, second);

                return true;
            }
            else
            {
                dateTime = DateTime.MinValue;

                return false;
            }
        }

        public static string ToPreciseDateTimeText(this DateTime time)
        {
            return time.ToShortDateString() + " " + time.ToString("HH:mm:ss.fffffff tt");
        }

        public static string ToDateTimeText(this DateTime time)
        {
            return time.ToShortDateString() + " " + time.ToLongTimeString();
        }

        public static DateTime ToTimeValueOnly(this DateTime dateTime)
        {
            return dateTime.AddYears(-dateTime.Year + 1).AddMonths(-dateTime.Month + 1).AddDays(-dateTime.Day + 1);
        }
    }
}
