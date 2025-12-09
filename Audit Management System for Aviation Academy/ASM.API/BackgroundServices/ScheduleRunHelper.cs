using System;

namespace ASM.API.BackgroundServices
{
    public static class ScheduleRunHelper
    {
        public static TimeSpan GetDelayUntilNextRunUtc(DateTime nowUtc, TimeSpan dailyTargetUtc, TimeSpan hourlyInterval)
        {
            var todayTarget = new DateTime(
                nowUtc.Year,
                nowUtc.Month,
                nowUtc.Day,
                dailyTargetUtc.Hours,
                dailyTargetUtc.Minutes,
                dailyTargetUtc.Seconds,
                DateTimeKind.Utc);

            var nextDaily = nowUtc < todayTarget ? todayTarget : todayTarget.AddDays(1);
            var nextHourly = nowUtc.Add(hourlyInterval);
            var next = nextHourly < nextDaily ? nextHourly : nextDaily;

            return next - nowUtc;
        }
    }
}

