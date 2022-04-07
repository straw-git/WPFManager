
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

public static class ObservableCollectionExtensions
{
    /// <summary>
    /// 根据Int类型排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_collection"></param>
    /// <param name="_orderByDesc"></param>
    public static void OrderByInt<T>(this ObservableCollection<T> _collection, Expression<Func<T, int>> _orderByDesc) where T:class
    {
        if (_collection == null || _collection.Count <= 1) return;

        List<T> lst = _collection.AsQueryable().OrderBy(_orderByDesc).ToList();
        var count = _collection.Count;
        for (int m = 0; m < count; m++)
        {
            var dex = _collection.IndexOf(lst[m]);
            if (dex == m) continue;
            _collection.Move(dex, m);
        }
    }
}
