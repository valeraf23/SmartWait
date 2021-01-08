using System;

namespace SmartWait.WaitSteps
{
    public static class TimeExtensions
    {
        public static TimeSpan ToSpan(this Time time, double step) => time switch
        {
            Time.FromHours => TimeSpan.FromHours(step),
            Time.FromMilliseconds => TimeSpan.FromMilliseconds(step),
            Time.FromSeconds => TimeSpan.FromSeconds(step),
            Time.FromMinutes => TimeSpan.FromMinutes(step),
            _ => throw new ArgumentOutOfRangeException(time.ToString())
        };
    }
}