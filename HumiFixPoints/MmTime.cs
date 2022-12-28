using System;

namespace HumiFixPoints
{
    public static class MmTime
    {
        public static long GetUnixTime(DateTime time) => ((DateTimeOffset)time).ToUnixTimeSeconds();

        public static double GetMjdFromUnix(long unixSeconds) => (unixSeconds + 2209161600.0) / 86400.0 + 15018.0;

        public static double GetMjd(DateTime time) => GetMjdFromUnix(GetUnixTime(time));
    }
}
