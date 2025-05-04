using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Application.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset ToMoscowTime(this DateTime dateTime) => new DateTimeOffset(dateTime, TimeSpan.FromHours(3));
        public static DateTime FromMoscowTime(this DateTimeOffset dateTime) => dateTime.ToOffset(TimeSpan.FromHours(3)).DateTime;

        public static DateTime RoundHour(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

    }
}
