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
    }
}
