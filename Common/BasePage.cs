using Common.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using form = System.Windows.Forms;

namespace Common
{
    public abstract partial class BasePage : Page
    {
        public int Order = 0;
        public bool IsMenu = true;//是否包含在导航内
        public string Code = "";
        public DBModels.Sys.User CurrUser = null;//当前登录账户信息
        protected BaseMainWindow ParentWindow = null;//父窗体

        public BasePage()
        {
            this.Loaded += BasePage_Loaded;
        }

        #region  Base Page

        protected abstract void OnPageLoaded();

        private void BasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ParentWindow = System.Windows.Window.GetWindow(this) as BaseMainWindow;
            if (ParentWindow == null)
            {
                //使用页面 测试时会进入这里使用管理员账户
                using (DBContext context = new DBContext())
                    CurrUser = context.User.First(c => c.Name == "admin");
                TempBasePageData.message = new MainWindowTagInfo();
                TempBasePageData.message.CurrUser = CurrUser;
                return;
            }
            MainWindowTagInfo parentInfo = ParentWindow.Tag as MainWindowTagInfo;
            if (parentInfo == null)
            {
                //使用窗体 测试时会进入这里使用管理员账户
                MessageBoxX.Show("当前是测试环境", " 模拟管理员操作");
                using (DBContext context = new DBContext())
                {
                    parentInfo = new MainWindowTagInfo();
                    parentInfo.CurrUser = context.User.First(c => c.Name == "admin");
                }
            }
            TempBasePageData.message = parentInfo;
            CurrUser = parentInfo.CurrUser;

            CheckMenu(parentInfo);

            OnPageLoaded();
        }

        /// <summary>
        /// 背景层显示/隐藏
        /// </summary>
        /// <param name="_v"></param>
        protected void MaskVisible(bool _v)
        {
            if (ParentWindow == null) return;//避免测试环境
            ParentWindow.IsMaskVisible = _v;
        }

        private void CheckMenu(MainWindowTagInfo parentInfo)
        {
            if (parentInfo.CurrUser.Name == "admin") return;

            string _menuStr = parentInfo.CurrUser.Menus;
            List<string> Menus = _menuStr.Split('|').ToList();

            //Client.Pages.Manager.Dic
            string nps = this.ToString();
            string currName = nps.Substring(nps.LastIndexOf('.') + 1);
            nps = nps.Replace($".{currName}", "");
            string parentName = nps.Substring(nps.LastIndexOf('.') + 1);

            string menuName = $"{parentName}-{currName}-";

            List<string> CurrPageMenus = Menus.Where(c => c.StartsWith(menuName)).ToList();//当前页面中的按钮（已有的权限）
            var menuInfo = parentInfo.Dic.Keys.First(c => c.Code == parentName);
            var buttons = parentInfo.Dic[menuInfo].First(c => c.Code == currName).Buttons;
            foreach (var buttonInfo in buttons)
            {
                Button button = this.FindName(buttonInfo.Name) as Button;
                if (CurrPageMenus.Contains($"{parentName}-{currName}-{buttonInfo.Name}"))
                {
                    //存在权限
                    button.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    //不存在权限
                    button.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion 

        #region Data Pager

        ///// <summary>
        ///// 在子类中使用 防止重复点击
        ///// </summary>
        //protected bool pagerRunning = false;

        ///// <summary>
        ///// 获取分页数据
        ///// </summary>
        ///// <typeparam name="T">数据表类</typeparam>
        ///// <param name="_source">EF数据源 DBSET</param>
        ///// <param name="_where">查询条件  查询全部null</param>
        /////<param name="_orderByDesc">按时间倒序排列条件 不启用为 null</param>
        ///// <param name="_loading">加载动画 null</param>   
        ///// <param name="_pagination">分页控件 null</param>   
        ///// <param name="_noDataUIEle">无数据显示的页面元素</param>   
        ///// <param name="_enableUIEles">加载期间不可操作的控件</param>   
        ///// <param name="_pageSize">页面显示数据条数 当分页数量为0时 返回所有数据</param>
        ///// <returns></returns>
        //protected async Task<List<T>> GetDataPagerAsync<T>(dynamic _source, Expression<Func<T, bool>> _where, Expression<Func<T, DateTime>> _orderByDesc, Loading _loading, Pagination _pagination, FrameworkElement _noDataUIEle = null, FrameworkElement[] _enableUIEles = null, int _pageSize = 10)
        //    where T : class
        //{
        //    //显示动画
        //    if (_loading != null && _loading.Visibility != System.Windows.Visibility.Visible)
        //        _loading.Visibility = System.Windows.Visibility.Visible;
        //    if (_pagination != null)
        //        _pagination.IsEnabled = false;
        //    if (_noDataUIEle != null)
        //        _noDataUIEle.Visibility = System.Windows.Visibility.Collapsed;

        //    if (_enableUIEles != null)
        //    {
        //        foreach (var item in _enableUIEles)
        //        {
        //            item.IsEnabled = false;
        //        }
        //    }

        //    await Task.Delay(300);

        //    int dataCount = 0;//总数据量
        //    int pagerCount = 0;//页数
        //    int currPage = _pagination==null?1: _pagination.CurrentIndex;//当前页码
        //    var pd = _source as DbSet<T>;
        //    var _list = new List<T>();

        //    await Task.Run(() =>
        //    {
        //        if (_pageSize == 0)
        //        {
        //            #region 返回所有数据

        //            if (_orderByDesc == null)
        //            {
        //                //无排序
        //                _list = _where == null
        //                        ? pd.ToList()
        //                        : pd.Where(_where.Compile()).ToList();
        //            }
        //            else
        //            {
        //                //有排序
        //                _list = _where == null
        //                        ? pd.OrderByDescending(_orderByDesc).ToList()
        //                        : pd.OrderByDescending(_orderByDesc).Where(_where.Compile()).ToList();
        //            }

        //            #endregion
        //        }
        //        else 
        //        {
        //            dataCount = _where == null ? pd.Count() : pd.Where(_where.Compile()).Count();//数据总条数
        //            pagerCount = PagerGlobal.GetPagerCount(dataCount);//总页数

        //            #region 返回分页数据

        //            //获取分页数据
        //            if (_orderByDesc == null)
        //            {
        //                //无排序
        //                _list = _where == null
        //                        ? pd.Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList()
        //                        : pd.Where(_where.Compile()).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList();
        //            }
        //            else
        //            {
        //                //有排序
        //                _list = _where == null
        //                        ? pd.OrderByDescending(_orderByDesc).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList()
        //                        : pd.OrderByDescending(_orderByDesc).Where(_where.Compile()).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList();
        //            }

        //            #endregion
        //        }
        //    });

        //    await Task.Delay(300);

        //    if (_pagination != null)
        //        _pagination.TotalIndex = pagerCount;//设置控件显示总页数


        //    //隐藏动画 
        //    if (_loading.Visibility != System.Windows.Visibility.Collapsed)
        //        _loading.Visibility = System.Windows.Visibility.Collapsed;
        //    if (_pagination != null)
        //        _pagination.IsEnabled = true;
        //    if (_noDataUIEle != null)
        //        _noDataUIEle.Visibility = dataCount > 0 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;//设置无数据控件（Border）显示

        //    if (_enableUIEles != null)
        //    {
        //        foreach (var item in _enableUIEles)
        //        {
        //            item.IsEnabled = true;
        //        }
        //    }

        //    return _list;
        //}


        #endregion

        #region Base Table

        /// <summary>
        /// 数据显示列集合（打印或导出时使用的数据行对应头）
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> TableColumns = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// 数据集合（选中的数据集合）
        /// </summary>
        private Dictionary<string, List<dynamic>> TableDatas = new Dictionary<string, List<dynamic>>();

        /// <summary>
        /// 获取当前表格的显示列
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <returns></returns>
        protected Dictionary<string, string> GetColumns(string _tableKey)
        {
            if (TableColumns.ContainsKey(_tableKey)) return TableColumns[_tableKey];
            else return new Dictionary<string, string>();
        }

        /// <summary>
        /// 设置列显示（用于记录导出数据时列的显示值）
        /// </summary>
        /// <param name="_tableKey">表格标志</param>
        /// <param name="_pro">数据名</param>
        /// <param name="_hea">显示名</param>
        protected void SetColumn(string _tableKey, string _pro, string _hea)
        {
            if (!TableColumns.ContainsKey(_tableKey)) TableColumns.Add(_tableKey, new Dictionary<string, string>());

            if (TableColumns[_tableKey].ContainsKey(_pro)) TableColumns[_tableKey][_pro] = _hea;
            else TableColumns[_tableKey].Add(_pro, _hea);
        }

        protected void SetColumn(string _tableKey, Dictionary<string, string> _dic)
        {
            if (TableColumns.ContainsKey(_tableKey)) TableColumns[_tableKey] = _dic;
            else TableColumns.Add(_tableKey, _dic);
        }

        /// <summary>
        /// 加入表格数据（用于记录存储 列表中选中数据）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_tableKey"></param>
        /// <param name="_data"></param>
        protected void AddTableData<T>(string _tableKey, T _data) where T : class
        {
            if (TableDatas.ContainsKey(_tableKey))
            {
                TableDatas[_tableKey].Add(_data);
            }
            else
            {
                TableDatas.Add(_tableKey, new List<dynamic>() { _data });
            }
        }

        /// <summary>
        /// 查找TableDatas中是否存在某条件
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <param name="_where"></param>
        /// <returns></returns>
        protected bool TableDataAny(string _tableKey, Func<dynamic, bool> _where)
        {
            if (TableDatas.ContainsKey(_tableKey))
            {
                return TableDatas[_tableKey].Any(_where);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 加入表格数据（用于记录存储 列表中选中数据）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_tableKey"></param>
        /// <param name="_datas"></param>
        protected void AddTableData<T>(string _tableKey, List<T> _datas) where T : class
        {
            foreach (var item in _datas)
            {
                AddTableData(_tableKey, item);
            }
        }

        protected List<dynamic> GetTableData<T>(string _tableKey)
        {
            if (TableDatas.ContainsKey(_tableKey)) return TableDatas[_tableKey];
            else return null;
        }

        /// <summary>
        /// 移除表格数据（用于记录存储 列表中选中数据）
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <param name="_firstWhere"></param>
        protected void RemoveTableData(string _tableKey, Func<dynamic, bool> _firstWhere)
        {
            if (TableDatas[_tableKey].Any(_firstWhere))
                TableDatas[_tableKey].Remove(TableDatas[_tableKey].First(_firstWhere));
        }

        /// <summary>
        /// 为DataGrid设置右键菜单（用于右键导出或选择页面数据）
        /// </summary>
        /// <param name="_grid"></param>
        protected void SetGridContextMenu(DataGrid _grid)
        {
            ContextMenu _menu = new ContextMenu();
            _menu.Padding = new Thickness(0, 5, 0, 5);
            _menu.Width = 250;
            _menu.FontSize = 12;
            ContextMenuHelper.SetCornerRadius(_menu, new CornerRadius(5));

            MenuItem selectedCurrPage = new MenuItem() { Header = "选中当前页" };
            MenuItem exportAll = new MenuItem() { Header = "导出Excel" };
            MenuItem exportselected = new MenuItem() { Header = "选中项" };
            MenuItem exportCurrPage = new MenuItem() { Header = "当前页" };
            exportAll.Items.Add(exportselected);
            exportAll.Items.Add(exportCurrPage);

            if (_grid.ContextMenu == null) _grid.ContextMenu = _menu;

            _grid.ContextMenu.Items.Add(selectedCurrPage);
            _grid.ContextMenu.Items.Add(exportAll);
        }

        #endregion

    /// <summary>
    /// 导出Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list">导出数据</param>
    /// <param name="_tableKey">表名称</param>
    /// <param name="_sheetName">保存的ExcelSheet名</param>
        protected async void ExportExcelAsync<T>(List<T> _list, string _tableKey, string _sheetName = "导出文件_Zyue")
        {
            //Panuon.UI.Silver消息框
            var _handler = PendingBox.Show("正在导出Excel...", "请等待", false, ParentWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });

            string _savePath = "";//文件保存路径
            var folderBrowser = new form.FolderBrowserDialog();//Winform dll中调用选择文件夹
            if (folderBrowser.ShowDialog() == form.DialogResult.OK)
            {
                _savePath = folderBrowser.SelectedPath;//选中的文件夹路径
            }
            else
            {
                MessageBox.Show("已取消");
                _handler.Close();
            }

            string excelFullPath = $"{_savePath}\\[{_sheetName}]{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";//导出文件路径

            await Task.Run(() =>
            {
                IWorkbook wk = null;
                wk = new XSSFWorkbook();//创建一个工作簿

                var sheet = wk.CreateSheet(_sheetName);//创建Sheet

                if (_list != null && _list.Count > 0)
                {
                    if (!TableColumns.ContainsKey(_tableKey) || TableColumns[_tableKey].Count == 0)
                    {
                        UIGlobal.RunUIAction(()=> 
                        {
                            MessageBox.Show("没有设置显示列");
                            _handler.Close();
                        });
                        
                        return;
                    }

                    //获取对象所有属性
                    System.Reflection.PropertyInfo[] properties = _list[0].GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                    #region 添加第一行标题数据

                    int cNum = 0;
                    for (int i = 0; i < properties.Count(); i++)
                    {
                        var _item = properties[i];

                        //属性名称
                        string name = _item.Name;

                        var _row = i == 0 ? sheet.CreateRow(0) : sheet.GetRow(0);//创建或获取第一行

                        if (TableColumns[_tableKey].ContainsKey(name))//是否存在对应的中文介绍（只导出存在中文介绍的列） 
                        {
                            var _headerCell = _row.CreateCell(cNum);//创建列
                            _headerCell.SetCellValue(name);//设置列值
                            cNum++;
                        }
                    }

                    #endregion

                    var _headerRow = sheet.GetRow(0);//获取第一行

                    #region 添加数据

                    for (int i = 0; i < _list.Count; i++)
                    {
                        var _item = _list[i];

                        var _currRow = sheet.CreateRow(1 + i);//创建当前数据的行
                        for (int j = 0; j < cNum; j++)
                        {
                            string _propertityName = _headerRow.GetCell(j).StringCellValue;//获取属性名称
                            string _propertityValue = _item.GetType().GetProperty(_propertityName).GetValue(_item, null).ToString();//获取属性值

                            _currRow.CreateCell(j).SetCellValue(_propertityValue);//设置单元格数据
                        }
                    }

                    #endregion

                    #region 更新列标题

                    for (int j = 0; j < cNum; j++)
                    {
                        string _propertityName = _headerRow.GetCell(j).StringCellValue;//获取属性名称
                        _headerRow.CreateCell(j).SetCellValue(TableColumns[_tableKey][_propertityName]);//更新标题
                    }

                    #endregion 
                }
                else
                {
                    UIGlobal.RunUIAction(() =>  //其它县城内调用UI主线程方法
                    {
                        MessageBox.Show("没有导出数据");
                        _handler.Close();
                    });
                    return;
                }

                try
                {
                    using (FileStream fs = new FileStream(excelFullPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        wk.Write(fs);//保存文件
                    }
                }
                catch(Exception ex)
                {
                    UIGlobal.RunUIAction(() =>
                    {
                        MessageBoxX.Show($"导出失败, [{ex.Message}]", "文件占用提示");
                        _handler.Close();
                    });
                }
            });

            Notice.Show(excelFullPath, "Excel导出成功", 3, MessageBoxIcon.Success);
            _handler.Close();
        }
    }
}
