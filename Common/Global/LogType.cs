using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 日志类型
/// </summary>
public class LogType
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public const int System = 0;
    /// <summary>
    /// 授权日志
    /// </summary>
    public const int Authorization = 1;
    /// <summary>
    /// 正常操作记录
    /// </summary>
    public const int Info = 2;
    /// <summary>
    /// 插件更新
    /// </summary>
    public const int PluginsUpdate = 3;
    /// <summary>
    /// 系统错误
    /// </summary>
    public int SystemError = 4;
}
