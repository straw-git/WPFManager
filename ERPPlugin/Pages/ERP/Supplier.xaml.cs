using DBModels.Sys;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Common.Utils;
using Common.MyAttributes;
using Common.Windows;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// Supplier.xaml 的交互逻辑
    /// </summary>
    public partial class Supplier : BasePage
    {
        public Supplier()
        {
            InitializeComponent();
            this.Order = 1;

            //测试
            //OnPageLoaded();
        }

        class UIModel : BaseUIModel
        {
            public int Id { get; set; }
            [DataSourceBinding("类型", 80, 1)]
            public string TypeName { get; set; }
            [DataSourceBinding("地址", -1, 3)]
            public string Address { get; set; }
            public string SubAddress
            {
                get
                {
                    return Address.Length > 20 ? $"{Address.Substring(0, 20)}..." : Address;
                }
            }
            [DataSourceBinding("名称", -1, 2)]
            public string Name { get; set; }//公司
            [DataSourceBinding("资质", 60, 4)]
            public string Qualification { get; set; }//供货资质
            [DataSourceBinding("联系电话", 120, 6)]
            public string Phone { get; set; }//联系电话
            [DataSourceBinding("联系人", 100, 5)]
            public string ContactName { get; set; }//联系人
        }

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            SetDataGridBinding(list, new UIModel(), Data);
        }

        private async void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            if (e == null) gPager.CurrentIndex = 1;//如果是通过查询或者刷新点击的 直接显示第一页

            //查询条件
            string name = txtName.Text.Trim();
            string listName = list.Name;

            Data.Clear();//先清空再加入页面数据

            using (DBContext context = new DBContext())
            {
                Expression<Func<DBModels.ERP.Supplier, bool>> _where = n => GetPagerWhere(n, name);//按条件查询
                Expression<Func<DBModels.ERP.Supplier, DateTime>> _orderByDesc = n => n.CreateTime;//按时间倒序
                //开始分页查询数据
                var _zPager = await PagerCommon.BeginEFDataPagerAsync(context.Supplier, _where, _orderByDesc, gLoading, gPager, bNoData, new Control[1] { list });
                if (!_zPager.Result) return;
                List<DBModels.ERP.Supplier> _list = _zPager.EFDataList;

                #region 页面数据填充

                foreach (var item in _list)
                {
                    string typeName = context.SysDic.First(c => c.Id == item.Type).Name;
                    var _model = DBItem2UIModel(item, listName, typeName);

                    Data.Add(_model);
                }

                #endregion
            }

            //结尾处必须结束分页查询
            PagerCommon.EndEFDataPager();
        }

        private UIModel DBItem2UIModel(DBModels.ERP.Supplier item, string _listName, string _typeName)
        {
            return new UIModel()
            {
                Address = item.Address,
                ContactName = item.ContactName,
                IsChecked = SelectedTableDataAny(_listName, c => c.Id == item.Id),
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Qualification = item.Qualification ? "是" : "否",
                TypeName = _typeName
            };
        }

        /// <summary>
        /// 查找表格的条件
        /// </summary>
        /// <param name="_supplier"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        protected bool GetPagerWhere(DBModels.ERP.Supplier _supplier, string _name)
        {
            bool resultCondition = true;
            if (_name.NotEmpty())
            {
                //根据名称检索
                resultCondition &= _supplier.Name.Contains(_name);
            }

            return resultCondition;
        }

        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)((sender as CheckBox).IsChecked);
            foreach (var item in Data)
            {
                item.IsChecked = isCheck;
                if (isCheck)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    SelectedTableData(list.Name, item);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(list.Name, c => c.Id == item.Id);
                }
            }
        }

        #region UI Method

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditSupplier addSupplier = new EditSupplier((sender as Button).Tag.ToString().AsInt());
            addSupplier.ShowDialog();
            UpdatePager(null,null);
            MaskVisible(false);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (DBContext context = new DBContext())
            {
                var selectModel = context.Supplier.First(c => c.Id == (sender as Button).Tag.ToString().AsInt());
                var result = MessageBoxX.Show($"是否删除[{selectModel.Name}]", "删除数据提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    context.Supplier.Remove(selectModel);
                    UpdatePager(null, null);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditSupplier addSupplier = new EditSupplier();
            addSupplier.ShowDialog();
            UpdatePager(null, null);
            MaskVisible(false);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null,null);
        }

        #endregion

        #region 导出Excel

        private void btnExportCurrPage_Click(object sender, RoutedEventArgs e)
        {
            var listData = Data.ToList();//获取选中数据
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题
            if (list == null || listData.Count == 0)
            {
                MessageBoxX.Show("没有选中数据", "空值提醒");
                return;
            }
            new ExcelHelper().List2ExcelAsync(listData, $"页码{gPager.CurrentIndex}", columns, hiddleColumns.Keys.ToList());
        }

        private async void btnExportAllPage_Click(object sender, RoutedEventArgs e)
        {
            //导出所有数据
            List<UIModel> allData = new List<UIModel>();
            string listName = list.Name;

            await Task.Run(() =>
            {
                var _list = new List<DBModels.ERP.Supplier>();
                using (DBContext context = new DBContext())
                {
                    _list = context.Supplier.OrderByDescending(c => c.CreateTime).ToList();
                    foreach (var item in _list)
                    {
                        string typeName = context.SysDic.First(c => c.Id == item.Type).Name;
                        var _model = DBItem2UIModel(item, listName, typeName);

                        allData.Add(_model);
                    }
                }
            });
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题

            new ExcelHelper().List2ExcelAsync(allData, "所有数据", columns, hiddleColumns.Keys.ToList());
        }

        private void btnExportFocusDatas_Click(object sender, RoutedEventArgs e)
        {
            var listData = GetSelectedTableData<UIModel>(list.Name);//获取选中数据
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题
            if (listData == null || listData.Count == 0)
            {
                MessageBoxX.Show("没有选中数据", "空值提醒");
                return;
            }
            new ExcelHelper().List2ExcelAsync(listData, "选中数据", columns, hiddleColumns.Keys.ToList());
        }

        #endregion

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectItem = list.SelectedItem as UIModel;
                bool targetIsChecked = !selectItem.IsChecked;
                Data.Single(c => c.Id == selectItem.Id).IsChecked = targetIsChecked;

                if (targetIsChecked)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    SelectedTableData(list.Name, selectItem);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(list.Name, c => c.Id == selectItem.Id);
                }
            }
        }

        private void btnTableColumnVisible_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            BasePageVisibilititySetting basePageVisibilititySetting = new BasePageVisibilititySetting(GetDataGridHeaders(list.Name));
            basePageVisibilititySetting.ShowDialog();

            var result = basePageVisibilititySetting.Result;
            foreach (var ri in result)
            {
                Visibility _visibility = ri.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                SetDataGridColumnVisibilitity(list.Name, ri.Title, _visibility);
            }

            UpdateDataGridColumnVisibility(list);

            MaskVisible(false);
        }
    }
}
