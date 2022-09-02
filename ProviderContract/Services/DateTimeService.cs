using System;

namespace ProviderContract.Services
{
    public static class DateTimeService
    {
        private static readonly DateTime unix = new DateTime(1970, 1, 1);

        public static DateTime FromSeconds(long timestamp)
        {
            return unix.AddSeconds(timestamp);
        }

        public static DateTime FromMilliSeconds(long timestamp)
        {
            return unix.AddMilliseconds(timestamp);
        }

        public static DateTime FromNanoSeconds(long timestamp)
        {
            return unix.AddMilliseconds(timestamp / 1000000);
        }
    }
}
