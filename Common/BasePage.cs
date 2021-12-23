using Common.Entities;
using Common.MyAttributes;
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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            string pluginName = nps.Substring(0, nps.IndexOf('.'));
            string currName = nps.Substring(nps.LastIndexOf('.') + 1);
            nps = nps.Replace($".{currName}", "");
            string parentName = nps.Substring(nps.LastIndexOf('.') + 1);

            string menuName = $"{pluginName}-{parentName}-{currName}-";

            List<string> CurrPageMenus = Menus.Where(c => c.StartsWith(menuName)).ToList();//当前页面中的按钮（已有的权限）

            foreach (var plugin in parentInfo.Dic)
            {
                if (plugin.Value.Keys.Any(c => c.Code == parentName))
                {
                    var menuInfo = plugin.Value.Keys.First(c => c.Code == parentName);
                    if (plugin.Value[menuInfo].Any(c => c.Code == currName))
                    {
                        var buttons = plugin.Value[menuInfo].First(c => c.Code == currName).Buttons;
                        foreach (var buttonInfo in buttons)
                        {
                            Button button = this.FindName(buttonInfo.Name) as Button;
                            if (CurrPageMenus.Contains($"{pluginName}-{parentName}-{currName}-{buttonInfo.Name}"))
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
                }
            }
        }

        #endregion

        #region Base Table

        #region DataGrid数据绑定

        /// <summary>
        /// DataGrid 所有绑定列属性信息 （包含列的显示隐藏 调整后 调用UpdateDataGridColumnVisibility 方法执行）
        /// </summary>
        protected Dictionary<string, List<DataSourceBindingAttribute>> DataGridBindingColumnAtts = new Dictionary<string, List<DataSourceBindingAttribute>>();

        /// <summary>
        /// 设置DataGrid数据源（此方法将会填充DataGrid列及列绑定）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dataGrid"></param>
        /// <param name="_t"></param>
        /// <param name="_source"></param>
        protected void SetDataGridBinding<T>(DataGrid _dataGrid, T _t, dynamic _source) where T : class
        {
            //表格的Name作为存储的数据键
            string _tableKey = _dataGrid.Name;
            //获取对象所有属性
            System.Reflection.PropertyInfo[] properties = _t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            //如果对象没有属性 直接退出
            if (properties.Length <= 0)
            {
                return;
            }

            //清空显示列（为避免特殊情况数据重复刷新）
            if (DataGridBindingColumnAtts.ContainsKey(_tableKey))
            {
                DataGridBindingColumnAtts[_tableKey].Clear();
            }
            else DataGridBindingColumnAtts.Add(_tableKey, new List<DataSourceBindingAttribute>());

            #region 将实体的绑定属性提取出来

            List<DataSourceBindingAttribute> _tempAtts = new List<DataSourceBindingAttribute>();
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                //属性名称
                string columnName = item.Name;
                // 获取指定属性的属性描述
                var bindingAtt = typeof(T).GetProperty(columnName).GetCustomAttribute<DataSourceBindingAttribute>();


                if (bindingAtt != null)//没有标记属性的值不显示
                {
                    bindingAtt.BindingName = columnName;
                    _tempAtts.Add(bindingAtt);
                }
            }

            #endregion

            DataGridBindingColumnAtts[_tableKey] = _tempAtts.OrderBy(c => c.Index).ToList();//对显示列进行排序

            #region 将绑定属性绑定至DataGrid 为其添加新列并按顺序完成绑定

            foreach (var bindingAtt in DataGridBindingColumnAtts[_tableKey])
            {
                //Edit DataGrid Columns
                if (_dataGrid.Columns.Any(c => c.Header != null && c.Header.ToString() == bindingAtt.ColumnHeader))
                {
                    //如果DataGrid中有表头和数据中一样的 不显示
                    continue;
                }
                else
                {
                    //显示并绑定数据
                    DataGridColumn column = new DataGridTextColumn() { Header = bindingAtt.ColumnHeader, Binding = new Binding(bindingAtt.BindingName) };
                    if (bindingAtt.Width == -1)
                    {
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    }
                    else if (bindingAtt.Width == 0)
                    {
                        column.Width = DataGridLength.Auto; //new DataGridLength(0, DataGridLengthUnitType.Auto);
                    }
                    else
                    {
                        column.Width = bindingAtt.Width;// new DataGridLength(bindingAtt.Width);
                    }
                    _dataGrid.Columns.Insert(bindingAtt.Index, column);
                }
            }

            #endregion

            //绑定数据源
            _dataGrid.ItemsSource = _source;
        }

        /// <summary>
        /// 设置列的显示或隐藏 (设置后 必须执行UpdateDataGridColumnVisibility 方法后才能看见效果)
        /// </summary>
        /// <param name="_header"></param>
        /// <param name="_visibility"></param>
        protected void SetDataGridColumnVisibilitity(string _tableKey, string _header, Visibility _visibility)
        {
            if (!DataGridBindingColumnAtts.ContainsKey(_tableKey))
            {
                MessageBoxX.Show("没有显示列", "未查询到的显示项");
                return;
            }
            if (DataGridBindingColumnAtts[_tableKey].Any(c => c.ColumnHeader == _header))
            {
                //数据表中存在空值列
                DataGridBindingColumnAtts[_tableKey].Single(c => c.ColumnHeader == _header).ColumnVisibilitity = _visibility;
            }
        }

        /// <summary>
        /// 更新 列的显示 隐藏 
        /// </summary>
        /// <param name="_dataGrid"></param>
        /// <param name="_header"></param>
        /// <param name="_visibility"></param>
        protected void UpdateDataGridColumnVisibility(DataGrid _dataGrid)
        {
            string _tableKey = _dataGrid.Name;
            if (!DataGridBindingColumnAtts.ContainsKey(_tableKey))
            {
                MessageBoxX.Show("没有显示列", "未查询到的显示项");
                return;
            }
            foreach (var column in _dataGrid.Columns)
            {
                if (column.Header != null && DataGridBindingColumnAtts[_tableKey].Any(c => c.ColumnHeader == column.Header.ToString()))
                {
                    //数据表中存在空值列
                    column.Visibility = DataGridBindingColumnAtts[_tableKey].First(c => c.ColumnHeader == column.Header.ToString()).ColumnVisibilitity;
                }
            }
        }

        /// <summary>
        /// 获取表格的所有显示列
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <returns></returns>
        protected Dictionary<string, string> GetDataGridColumnVisibleHeaders(string _tableKey, bool _visible)
        {
            var dic = new Dictionary<string, string>();
            if (DataGridBindingColumnAtts.ContainsKey(_tableKey))
            {
                for (int i = 0; i < DataGridBindingColumnAtts[_tableKey].Count; i++)
                {
                    var _column = DataGridBindingColumnAtts[_tableKey][i];
                    if (_visible)
                    {
                        if (_column.ColumnVisibilitity == Visibility.Visible)
                        {
                            //获取显示元素
                            dic.Add(_column.BindingName, _column.ColumnHeader);
                        }
                    }
                    else
                    {
                        if (_column.ColumnVisibilitity == Visibility.Collapsed)
                        {
                            //获取隐藏元素
                            dic.Add(_column.BindingName, _column.ColumnHeader);
                        }
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 获取表格所有列（返回的结果 绑定名称|标题|是否显示（0：是1：否））
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <returns></returns>
        protected List<string> GetDataGridHeaders(string _tableKey)
        {
            List<string> result = new List<string>();

            if (DataGridBindingColumnAtts.ContainsKey(_tableKey))
            {
                for (int i = 0; i < DataGridBindingColumnAtts[_tableKey].Count; i++)
                {
                    var _column = DataGridBindingColumnAtts[_tableKey][i];
                    string _isShow = _column.ColumnVisibilitity == Visibility.Visible ? "0" : "1";
                    result.Add($"{_column.BindingName}|{_column.ColumnHeader}|{_isShow}");
                }
            }

            return result;
        }

        #endregion

        #region 选中数据操作

        /// <summary>
        /// 数据集合（选中的数据集合）
        /// </summary>
        private Dictionary<string, List<dynamic>> SelectedTableDatas = new Dictionary<string, List<dynamic>>();

        /// <summary>
        /// 选中数据（用于记录存储 列表中选中数据）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_tableKey"></param>
        /// <param name="_data"></param>
        protected void SelectedTableData<T>(string _tableKey, T _data) where T : class
        {
            if (SelectedTableDatas.ContainsKey(_tableKey))
            {
                SelectedTableDatas[_tableKey].Add(_data);
            }
            else
            {
                SelectedTableDatas.Add(_tableKey, new List<dynamic>() { _data });
            }
        }

        /// <summary>
        /// 查找选中项中是否存在某条件
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <param name="_where"></param>
        /// <returns></returns>
        protected bool SelectedTableDataAny(string _tableKey, Func<dynamic, bool> _where)
        {
            if (SelectedTableDatas.ContainsKey(_tableKey))
            {
                return SelectedTableDatas[_tableKey].Any(_where);
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
        protected void SelectedTableData<T>(string _tableKey, List<T> _datas) where T : class
        {
            foreach (var item in _datas)
            {
                SelectedTableData(_tableKey, item);
            }
        }

        /// <summary>
        /// 获取某表格的所有选中数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_tableKey"></param>
        /// <returns></returns>
        protected List<dynamic> GetSelectedTableData<T>(string _tableKey)
        {
            if (SelectedTableDatas.ContainsKey(_tableKey)) return SelectedTableDatas[_tableKey];
            else return null;
        }

        /// <summary>
        /// 移除表格数据（用于记录存储 列表中选中数据）
        /// </summary>
        /// <param name="_tableKey"></param>
        /// <param name="_firstWhere"></param>
        protected void UnSelectedTableData(string _tableKey, Func<dynamic, bool> _firstWhere)
        {
            if (SelectedTableDatas[_tableKey].Any(_firstWhere))
                SelectedTableDatas[_tableKey].Remove(SelectedTableDatas[_tableKey].First(_firstWhere));
        }

        #endregion 

        #endregion
    }
}
