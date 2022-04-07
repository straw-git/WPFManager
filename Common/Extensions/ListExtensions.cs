
using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    /// <summary>
    /// List string 转List int
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<int> String2Int(this List<string> list)
    {
        return list.Select<string, int>(x => Convert.ToInt32(x)) .ToList();
    }
    /// <summary>
    /// List int 转List string
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<string> Int2String(this List<int> list) 
    {
        return list.Select<int, string>(x => Convert.ToString(x)).ToList();
    }
}
