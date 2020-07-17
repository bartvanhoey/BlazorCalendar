using System;
using BlazorCalendar.Helpers;

namespace BlazorCalendar.Models
{
    public class CalendarEvent
    {
        public CalendarEvent()
        {
            Color = RandomColorHelper.GetRandomColorClass();
        }

        public string Subject { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; private set; }
    }
}