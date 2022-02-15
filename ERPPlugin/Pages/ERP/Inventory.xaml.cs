using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Linq.Expressions;
using Common.Utils;
using Common.Windows;
using Common.MyAttributes;
using LiveCharts;
using LiveCharts.Wpf;
using ERPDBModels.Models;
using ERPPlugin.Windows;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// Inventory.xaml 的交互逻辑
    /// </summary>
    public partial class Inventory : BasePage
    {
        public Inventory()
        {
            InitializeComponent();
            this.Order = 0;

            //测试
            OnPageLoaded();
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string ItemId { get; set; }
            [DataSourceBinding("名称", -1, 1)]
            public string Name { get; set; }
            public string SubName
            {
                get
                {
                    return Name.Length > 20 ? $"{Name.Substring(0, 20)}..." : Name;
                }
            }
            [DataSourceBinding("仓库名称", 200, 2)]
            public string StoreName { get; set; }
            private int count = 0;
            [DataSourceBinding("库存余量", 150, 3)]
            public int Count //库存余量
            {
                get => count;
                set
                {
                    count = value;
                    NotifyPropertyChanged("Count");
                }
            }
        }

        #endregion

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            SetDataGridBinding(list, new UIModel(), Data);
            LoadChartByCount();
        }

        private void LoadChartByCount()
        {
            ccCount.Series = new SeriesCollection();
            ccCount.Series.Clear();
            using (ERPDBContext context = new ERPDBContext())
            {
                var store = context.Stock.GroupBy(c => c.StoreId).ToList();
                ccCount.Series.Add(new ColumnSeries()
                {
                    Title = "物品种类",
                    Values = new LiveCharts.ChartValues<int>(),
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0}", point.Y)
                });
                ccCount.AxisX = new AxesCollection();
                ccCount.AxisX.Add(new Axis() { Labels = new List<string>() });
                foreach (var item in store)
                {
                    ccCount.Series[0].Values.Add(item.Count());
                    ccCount.AxisX[0].Labels.Add(context.SysDic.First(c => c.Id == item.Key).Name);
                }
            }
        }

        #region UI Method

        private async void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            if (e == null) gPager.CurrentIndex = 1;//如果是通过查询或者刷新点击的 直接显示第一页

            Data.Clear();//先清空再加入页面数据

            using (ERPDBContext context = new ERPDBContext())
            {
                Expression<Func<Stock, int>> _orderByDesc = n => n.Id;//按Id倒序
                //开始分页查询数据
                var _zPager = await PagerCommon.BeginEFDataPagerAsync<Stock>(context.Stock, null, _orderByDesc, gLoading, gPager, bNoData, new Control[1] { list });
                if (!_zPager.Result) return;
                List<Stock> _list = _zPager.EFDataList;

                #region 页面数据填充

                foreach (var item in _list)
                {
                    string goodsName = context.Goods.Any(c => c.Id == item.GoodsId && !c.IsDel) ? context.Goods.First(c => c.Id == item.GoodsId).Name : "物品已删除";
                    string storeName = context.SysDic.Any(c => c.Id == item.StoreId) ? context.SysDic.First(c => c.Id == item.StoreId).Name : "仓库已删除"; ;
                    int count = context.StockLog.Any(c => c.GoodsId == item.GoodsId) ? context.StockLog.Where(c => c.GoodsId == item.GoodsId).OrderByDescending(c => c.CreateTime).First().Surplus : 0;
                    var _model = DBItem2UIModel(item, goodsName, storeName, count);
                    Data.Add(_model);
                }

                #endregion
            }

            //结尾处必须结束分页查询
            PagerCommon.EndEFDataPager();
        }
        private UIModel DBItem2UIModel(Stock item, string goodsName, string storeName, int count)
        {
            UIModel _model = new UIModel();
            _model.Count = count;
            _model.ItemId = item.GoodsId;
            _model.Id = item.Id;
            _model.Name = goodsName;
            _model.StoreName = storeName;
            return _model;
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            var _model = Data.First(x => x.Id == id);
            MaskVisible(true);
            CheckStore checkStore = new CheckStore(_model.Count);
            checkStore.ShowDialog();

            if (checkStore.Succeed)
            {
                //数量盘点成功
                using (ERPDBContext context = new ERPDBContext())
                {
                    var stock = context.Stock.Single(c => c.Id == id);
                    stock.Count = checkStore.Model.NewCount;

                    StockLog stockLog = new StockLog();
                    stockLog.StoreId = stock.StoreId;
                    stockLog.StockType = checkStore.Model.NewCount > checkStore.Model.OldCount ? Global.StockTypeIn : Global.StockTypeOut;
                    stockLog.GoodsId = stock.GoodsId;
                    stockLog.Count = Math.Abs(checkStore.Model.NewCount - checkStore.Model.OldCount);
                    stockLog.Surplus = checkStore.Model.NewCount;
                    stockLog.SalePrice = context.Goods.Any(c => c.Id == stock.GoodsId) ? context.Goods.First(c => c.Id == stock.GoodsId).SalePrice : 0;
                    stockLog.Price = context.StockLog.Any(c => c.StoreId == stock.Id) ? context.StockLog.First(c => c.StoreId == stock.Id).Price : 0;
                    stockLog.SupplierId = context.StockLog.Any(c => c.StoreId == stock.Id) ? context.StockLog.First(c => c.StoreId == stock.Id).SupplierId : 0;
                    stockLog.Manufacturer = "盘点";
                    stockLog.Remark = "盘点";

                    stockLog.Creator = UserGlobal.CurrUser.Id;
                    stockLog.CreateTime = DateTime.Now;

                    context.StockLog.Add(stockLog);
                    context.SaveChanges();
                }

                UpdatePager(null, null);
            }

            MaskVisible(false);
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
                var _list = new List<Stock>();
                using (ERPDBContext context = new ERPDBContext())
                {
                    _list = context.Stock.ToList();
                    foreach (var item in _list)
                    {
                        string goodsName = context.Goods.Any(c => c.Id == item.GoodsId && !c.IsDel) ? context.Goods.First(c => c.Id == item.GoodsId).Name : "物品已删除";
                        string storeName = context.SysDic.Any(c => c.Id == item.StoreId) ? context.SysDic.First(c => c.Id == item.StoreId).Name : "仓库已删除"; ;
                        int count = context.StockLog.Any(c => c.GoodsId == item.GoodsId) ? context.StockLog.Where(c => c.GoodsId == item.GoodsId).OrderByDescending(c => c.CreateTime).First().Surplus : 0;
                        var _model = DBItem2UIModel(item, goodsName, storeName, count);
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

    }
}
