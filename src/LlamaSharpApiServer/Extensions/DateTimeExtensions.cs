// Source: https://briancaos.wordpress.com/2022/02/24/c-datetime-to-unix-timestamps/

namespace LlamaSharpApiServer.Extensions;

public static class DateTimeExtensions
{
    // Convert datetime to UNIX time
    public static long ToUnixTime(this DateTime dateTime)
    {
        DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
        return dto.ToUnixTimeSeconds();
    }

    // Convert datetime to UNIX time including miliseconds
    public static long ToUnixTimeMilliSeconds(this DateTime dateTime)
    {
        DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
        return dto.ToUnixTimeMilliseconds();
    }
}

