using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PagerGlobal
{
    private static int pageSize = 10;

    /// <summary>
    /// 获取当前总页数
    /// </summary>
    /// <param name="_dataCount"></param>
    /// <param name="_pageSize"></param>
    /// <returns></returns>
    public static int GetPagerCount(int _dataCount, int _pageSize = 0)
    {
        if (_pageSize == 0) _pageSize = pageSize;
        int count = 1;
        if (_dataCount <= 0) return count;
        if (_dataCount % _pageSize == 0) count = _dataCount / _pageSize;
        else count = (_dataCount / _pageSize) + 1;
        return count;
    }
}
