using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Utils
{
    public static class DateTimeExtensions
    {
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
