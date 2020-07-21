using System;

namespace BlazorCalendar.Models
{
    public class DateTimeTimeZone
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }

        public DateTime ConvertToLocalDateTime() {
            var dateTime = System.DateTime.Parse(DateTime);

            TimeZoneInfo timeZone = null;
            if (TimeZone == "UTC") {
                var utc = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                timeZone = TimeZoneInfo.Utc;
            }
            else
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            }
            return new DateTimeOffset(dateTime, timeZone.BaseUtcOffset).LocalDateTime;
        }
    }
}