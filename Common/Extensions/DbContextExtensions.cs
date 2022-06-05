using System;
using System.Data.Entity;

public static class DbContextExtensions
{
    /// <summary>
    /// 判断数据表是否存在
    /// </summary>
    /// <param name="_tableName">表名称</param>
    /// <returns></returns>
    public static bool ExisTable(this DbContext _context, string _tableName)
    {
        var countResult = _context.Database.SqlQuery(typeof(int),
            $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='{_tableName}'")
            .ToListAsync().Result;
        if (countResult == null || countResult.Count == 0) return false;
        int count = 0;
        if (!int.TryParse(countResult[0].ToString(), out count)) return false;
        return count > 0;
    }
}
