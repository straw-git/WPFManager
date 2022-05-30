using System.Collections.Generic;

/// <summary>
/// 数据库连接字符串集合
/// </summary>
public class DBConnections
{
    static Dictionary<string, string> Strs = new Dictionary<string, string>();

    static DBConnections()
    {

    }

    public static void Set(string _key, string _value)
    {
        if (Strs.ContainsKey(_key)) Strs[_key] = _value;
        else Strs.Add(_key, _value);
    }

    /// <summary>
    /// 获取连接字符串 默认返回本机
    /// </summary>
    /// <param name="_key"></param>
    /// <returns></returns>
    public static string Get(string _key)
    {
        if (Strs.ContainsKey(_key)) return Strs[_key];
        return @"Data Source=.;Initial Catalog=ZDB;User ID=sa;Password=123456;";
    }
}
