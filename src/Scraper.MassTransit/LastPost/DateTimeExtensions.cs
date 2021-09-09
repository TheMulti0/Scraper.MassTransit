using System;

namespace Scraper.MassTransit
{
    internal static class DateTimeExtensions
    {
        public static DateTime Floor(this DateTime dateTime, TimeSpan interval)
        {
            return interval == TimeSpan.Zero 
                ? dateTime 
                : dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }
    }
}