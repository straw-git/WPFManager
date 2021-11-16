
using System;


public static class DateTimeExtensions
{
    public static DateTime FormatYYYYMMDD2DateTime(this DateTime _time)
    {
        return Convert.ToDateTime(_time.ToString("yyyy-MM-dd"));
    }

    public static string FormatYYYYMMDD2String(this DateTime _time)
    {
        return _time.ToString("yyyy-MM-dd");
    }

    public static DateTime MinDate(this DateTime _time)
    {
        return Convert.ToDateTime(_time.ToString("yyyy-MM-dd 00:00:00"));
    }


    public static DateTime MaxDate(this DateTime _time)
    {
        return Convert.ToDateTime(_time.ToString("yyyy-MM-dd 23:59:59"));
    }

    public static DateTime CurrMonthMinDate(this DateTime _time)
    {
        return Convert.ToDateTime(_time.ToString("yyyy-MM-01 00:00:00"));
    }
}
