using ObjectsComparer;
using System.Collections.Generic;

namespace Common.Utils
{
    /// <summary>
    /// 对象比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectComparerCommon<T> where T:class
    {
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="_t1"></param>
        /// <param name="_t2"></param>
        /// <param name="differences"></param>
        /// <returns></returns>
        public bool Compare(T _t1,T _t2,out IEnumerable<Difference> differences) 
        {
            return new ObjectsComparer.Comparer<T>().Compare(_t1, _t2, out differences);
        }
        public bool Compare(T _t1, T _t2)
        {
            return new ObjectsComparer.Comparer<T>().Compare(_t1, _t2);
        }
    }
}
