using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common.Utils
{
    /// <summary>
    /// 分页公用
    /// </summary>
    public class PagerUtils
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

        #region EF分页方式

        public class EFPagerBack
        {
            public bool Result { get; set; }
            public dynamic EFDataList { get; set; }
            public int DataCount { get; set; }
        }
        /// <summary>
        /// 当前是否正在执行GetEFDataPagerAsync分页操作
        ///  防止刷新过快 造成数据混乱
        /// </summary>
        private static bool _EFDataPagerRunning = false;

        /// <summary>
        /// 开始EF分页查询
        /// </summary>
        /// <typeparam name="T">数据表类</typeparam>
        /// <param name="_source">EF数据源 DBSET</param>
        /// <param name="_where">查询条件  查询全部null</param>
        ///<param name="_orderByDesc">按时间倒序排列条件 不能为null</param>
        /// <param name="_loading">加载动画 null</param>   
        /// <param name="_pagination">分页控件 null</param>   
        /// <param name="_noDataUIEle">无数据显示的页面元素</param>   
        /// <param name="_enableUIEles">加载期间不可操作的控件</param>   
        /// <param name="_pageSize">页面显示数据条数 当分页数量为0时 返回所有数据</param>
        /// <param name="_autoEndRunning">是否自动关闭刷新限制 默认false true时在高频度刷新下容易出现数据展示错误</param>
        /// <returns></returns>
        public static async Task<EFPagerBack> BeginEFDataPagerAsync<T>(dynamic _source, Expression<Func<T, bool>> _where, Expression<Func<T, DateTime>> _orderByDesc, Loading _loading, Pagination _pagination, FrameworkElement _noDataUIEle = null, FrameworkElement[] _enableUIEles = null, int _pageSize = 10, bool _autoEndRunning = false)
            where T : class
        {
            if (_orderByDesc == null)
            {
                MessageBoxX.Show("请输入排序条件", "空值提醒");
                return new EFPagerBack() { Result = false, EFDataList = new List<T>() };

            }
            if (!_EFDataPagerRunning)
                _EFDataPagerRunning = true;
            else return new EFPagerBack() { Result = false, EFDataList = new List<T>() };

            //显示动画
            if (_loading != null && _loading.Visibility != System.Windows.Visibility.Visible)
                _loading.Visibility = System.Windows.Visibility.Visible;
            if (_pagination != null)
                _pagination.IsEnabled = false;
            if (_noDataUIEle != null)
                _noDataUIEle.Visibility = System.Windows.Visibility.Collapsed;

            if (_enableUIEles != null)
            {
                foreach (var item in _enableUIEles)
                {
                    item.IsEnabled = false;
                }
            }

            await Task.Delay(300);

            int dataCount = 0;//总数据量
            int pagerCount = 0;//页数
            int currPage = _pagination == null ? 1 : _pagination.CurrentIndex;//当前页码
            var pd = _source as DbSet<T>;
            var _EFDataList = new List<T>();

            await Task.Run(() =>
            {
                if (_pageSize == 0)
                {
                    #region 返回所有数据

                    //有排序
                    _EFDataList = _where == null
                            ? pd.OrderByDescending(_orderByDesc).ToList()
                            : pd.OrderByDescending(_orderByDesc).Where(_where.Compile()).ToList();

                    #endregion
                }
                else
                {
                    dataCount = _where == null ? pd.Count() : pd.Where(_where.Compile()).Count();//数据总条数
                    pagerCount = GetPagerCount(dataCount);//总页数

                    #region 返回分页数据

                    //获取分页数据

                    //有排序
                    _EFDataList = _where == null
                            ? pd.OrderByDescending(_orderByDesc).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList()
                            : pd.OrderByDescending(_orderByDesc).Where(_where.Compile()).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList();
                    #endregion
                }
            });

            await Task.Delay(300);

            if (_pagination != null)
                _pagination.TotalIndex = pagerCount;//设置控件显示总页数


            //隐藏动画 
            if (_loading != null && _loading.Visibility != System.Windows.Visibility.Collapsed)
                _loading.Visibility = System.Windows.Visibility.Collapsed;
            if (_pagination != null)
                _pagination.IsEnabled = true;
            if (_noDataUIEle != null)
                _noDataUIEle.Visibility = dataCount > 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;//设置无数据控件（Border）显示

            if (_enableUIEles != null)
            {
                foreach (var item in _enableUIEles)
                {
                    item.IsEnabled = true;
                }
            }

            if (_autoEndRunning)
                EndEFDataPager();

            return new EFPagerBack() { Result = true, EFDataList = _EFDataList, DataCount = dataCount };
        }

        /// <summary>
        /// 开始EF分页查询
        /// </summary>
        /// <typeparam name="T">数据表类</typeparam>
        /// <param name="_source">EF数据源 DBSET</param>
        /// <param name="_where">查询条件  查询全部null</param>
        ///<param name="_orderByDesc">按时间倒序排列条件 不启用为 null</param>
        /// <param name="_loading">加载动画 null</param>   
        /// <param name="_pagination">分页控件 null</param>   
        /// <param name="_noDataUIEle">无数据显示的页面元素</param>   
        /// <param name="_enableUIEles">加载期间不可操作的控件</param>   
        /// <param name="_pageSize">页面显示数据条数 当分页数量为0时 返回所有数据</param>
        /// <param name="_autoEndRunning">是否自动关闭刷新限制 默认false true时在高频度刷新下容易出现数据展示错误</param>
        /// <returns></returns>
        public static async Task<EFPagerBack> BeginEFDataPagerAsync<T>(dynamic _source, Expression<Func<T, bool>> _where, Expression<Func<T, int>> _orderByDesc, Loading _loading, Pagination _pagination, FrameworkElement _noDataUIEle = null, FrameworkElement[] _enableUIEles = null, int _pageSize = 10, bool _autoEndRunning = false)
            where T : class
        {
            if (_orderByDesc == null)
            {
                MessageBoxX.Show("请输入排序条件", "空值提醒");
                return new EFPagerBack() { Result = false, EFDataList = new List<T>() };

            }
            if (!_EFDataPagerRunning)
                _EFDataPagerRunning = true;
            else return new EFPagerBack() { Result = false, EFDataList = new List<T>() };

            //显示动画
            if (_loading != null && _loading.Visibility != System.Windows.Visibility.Visible)
                _loading.Visibility = System.Windows.Visibility.Visible;
            if (_pagination != null)
                _pagination.IsEnabled = false;
            if (_noDataUIEle != null)
                _noDataUIEle.Visibility = System.Windows.Visibility.Collapsed;

            if (_enableUIEles != null)
            {
                foreach (var item in _enableUIEles)
                {
                    item.IsEnabled = false;
                }
            }

            await Task.Delay(300);

            int dataCount = 0;//总数据量
            int pagerCount = 0;//页数
            int currPage = _pagination == null ? 1 : _pagination.CurrentIndex;//当前页码
            var pd = _source as DbSet<T>;
            var _EFDataList = new List<T>();

            await Task.Run(() =>
            {
                if (_pageSize == 0)
                {
                    #region 返回所有数据

                    if (_orderByDesc == null)
                    {
                        //无排序
                        _EFDataList = _where == null
                                ? pd.ToList()
                                : pd.Where(_where.Compile()).ToList();
                    }
                    else
                    {
                        //有排序
                        _EFDataList = _where == null
                                ? pd.OrderByDescending(_orderByDesc).ToList()
                                : pd.OrderByDescending(_orderByDesc).Where(_where.Compile()).ToList();
                    }

                    #endregion
                }
                else
                {
                    dataCount = _where == null ? pd.Count() : pd.Where(_where.Compile()).Count();//数据总条数
                    pagerCount = GetPagerCount(dataCount);//总页数

                    #region 返回分页数据

                    //获取分页数据
                    if (_orderByDesc == null)
                    {
                        //无排序
                        _EFDataList = _where == null
                                ? pd.Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList()
                                : pd.Where(_where.Compile()).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList();
                    }
                    else
                    {
                        //有排序
                        _EFDataList = _where == null
                                ? pd.OrderByDescending(_orderByDesc).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList()
                                : pd.OrderByDescending(_orderByDesc).Where(_where.Compile()).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList();
                    }

                    #endregion
                }
            });

            await Task.Delay(300);

            if (_pagination != null)
                _pagination.TotalIndex = pagerCount;//设置控件显示总页数


            //隐藏动画 
            if (_loading != null && _loading.Visibility != System.Windows.Visibility.Collapsed)
                _loading.Visibility = System.Windows.Visibility.Collapsed;
            if (_pagination != null)
                _pagination.IsEnabled = true;
            if (_noDataUIEle != null)
                _noDataUIEle.Visibility = dataCount > 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;//设置无数据控件（Border）显示

            if (_enableUIEles != null)
            {
                foreach (var item in _enableUIEles)
                {
                    item.IsEnabled = true;
                }
            }

            if (_autoEndRunning)
                EndEFDataPager();

            return new EFPagerBack() { Result = true, EFDataList = _EFDataList };
        }


        /// <summary>
        /// 结束EF分页查询（关闭刷新限制）
        /// </summary>
        public static void EndEFDataPager()
        {
            _EFDataPagerRunning = false;
        }

        #endregion
    }
}
