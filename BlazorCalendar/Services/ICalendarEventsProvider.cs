using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorCalendar.Models;

namespace BlazorCalendar.Services
{
    public interface ICalendarEventsProvider
    {
         Task<IEnumerable<CalendarEvent>> GetEventsInMonthAsync(int year, int month);
         Task AddCalenderEventAsync(CalendarEvent calendarEvent);
    }
}
