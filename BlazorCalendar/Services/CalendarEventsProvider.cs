using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;

namespace BlazorCalendar.Services
{
  public class CalendarEventsProvider : ICalendarEventsProvider
  {
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly HttpClient _httClient;
    private const string BASE_URL = "https://graph.microsoft.com/v1.0/me/events";

    public CalendarEventsProvider(IAccessTokenProvider accessTokenProvider, HttpClient httClient)
    {
      _accessTokenProvider = accessTokenProvider;
      _httClient = httClient;
    }

    public async Task<IEnumerable<CalendarEvent>> GetEventsInMonthAsync(int year, int month)
    {
      var accessToken = await GetAccessTokenAsync();
      if (accessToken == null) return null;

      _httClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
      var response = await _httClient.GetAsync(ConstructGraphUrl(year, month));

      if (!response.IsSuccessStatusCode) return null;

      var content = await response.Content.ReadAsStringAsync();
      var graphEventsResponse = JsonConvert.DeserializeObject<GraphEventsResponse>(content);

      var events = new List<CalendarEvent>();

      foreach (var item in graphEventsResponse.Value)
      {
        events.Add(new CalendarEvent
        {
          Subject = item.Subject,
          StartDate = item.Start.ConvertToLocalDateTime(),
          EndDate = item.End.ConvertToLocalDateTime()
        });
      }
      return events;
    }

    private async Task<string> GetAccessTokenAsync()
    {
      var tokenRequest = await _accessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions
      {
        Scopes = new[] { "https://graph.microsoft.com/Calendars.ReadWrite" }
      });

      if (tokenRequest.TryGetToken(out var token))
      {
        if (token != null) return token.Value;
      }
      return null;
    }

    private string ConstructGraphUrl(int year, int month)
    {
      var lastDayInMonth = DateTime.DaysInMonth(year, month);
      return $"{BASE_URL}?$filter=start/dateTime ge '{year}-{month}-01T08:00' and end/datetime le '{year}-{month}-{lastDayInMonth}T00:00'&select=subject,start,end";
    }

    public async Task AddCalenderEventAsync(CalendarEvent calendarEvent)
    {
      var accessToken = await GetAccessTokenAsync();
      if (accessToken == null) return;

      _httClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);

      var startDate = calendarEvent.StartDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
      var endDate = calendarEvent.EndDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");

      string eventAsJson = JsonConvert.SerializeObject(new MicrosoftGraphEvent
      {
        Subject = calendarEvent.Subject,
        Start = new DateTimeTimeZone { DateTime = startDate, TimeZone = "UTC" },
        End = new DateTimeTimeZone { DateTime = endDate, TimeZone = "UTC" },
      });

      var content = new StringContent(eventAsJson);
      content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

      var response = await _httClient.PostAsync(BASE_URL, content);

      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("Event has been successfully added");
      }
      else
      {
        Console.WriteLine(response.StatusCode);
      }

    }
  }
}