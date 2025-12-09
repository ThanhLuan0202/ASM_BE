using System;

namespace ASM_Repositories.Helper
{
    public static class TimeZoneHelper
    {
        private const string DefaultWindowsTzId = "SE Asia Standard Time"; // VN, Windows ID
        private const string DefaultIanaTzId = "Asia/Bangkok"; // VN, IANA ID
        private static readonly Lazy<TimeZoneInfo> _timeZone = new(() => ResolveTimeZone());

        public static TimeZoneInfo TimeZone => _timeZone.Value;

        private static TimeZoneInfo ResolveTimeZone()
        {
            var envTz = Environment.GetEnvironmentVariable("ASM_TIMEZONE_ID");

            static TimeZoneInfo? TryTz(string? id)
            {
                if (string.IsNullOrWhiteSpace(id)) return null;
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch { return null; }
            }

            return
                TryTz(envTz) ??
                TryTz(DefaultWindowsTzId) ??
                TryTz(DefaultIanaTzId) ??
                TimeZoneInfo.Utc;
        }
    }
}

