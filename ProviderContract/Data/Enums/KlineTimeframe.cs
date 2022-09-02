using System;

namespace ProviderContract.Data.Enums
{
    /// <summary>
    /// Период свечи
    /// </summary>
    public enum KlineTimeframe : int
    {
        _1MIN = 60,
        _5MIN = 300,
        _10MIN = 600,
        _15MIN = 900,
        _30MIN = 1800,
        _1HOUR = 3600,
        _4HOUR = 14400,
        _12HOUR = 43200,
        _1DAY = 86400,
        _3DAY = 259200,
        _1WEEK = 604800
    }
}
