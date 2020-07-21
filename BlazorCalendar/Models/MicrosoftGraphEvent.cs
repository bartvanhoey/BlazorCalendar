using System;

namespace BlazorCalendar.Models
{
    public class MicrosoftGraphEvent
    {
        public string Subject { get; set; }
        public DateTimeTimeZone Start { get; set; }
        public DateTimeTimeZone End { get; set; }
    }
}