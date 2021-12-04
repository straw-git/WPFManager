using DBModels.ERP;
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
using Common.MyAttributes;
using Common.Windows;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// PurchaseList.xaml 的交互逻辑
    /// </summary>
    public partial class PurchaseList : BasePage
    {
        public PurchaseList()
        {
            InitializeComponent();
            this.Order = 2;

            //测试
            //OnPageLoaded();
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public int Id { get; set; }
            [DataSourceBinding("采购单号", 200, 1)]
            public string PlanCode { get; set; }
            private decimal priceCount = 0;
            [DataSourceBinding("已消耗采购金额", 150, 3)]
            public decimal PriceCount
            {
                get => priceCount;
                set
                {
                    priceCount = value;
                    NotifyPropertyChanged("PriceCount");
                }
            }
            private string finish = "0%";
            [DataSourceBinding("完成量", 100, 4)]
            public string Finish
            {
                get => finish;
                set
                {
                    finish = value;
                    NotifyPropertyChanged("Finish");
                }
            }
            [DataSourceBinding("发起人", 150, 5)]
            public string Creator { get; set; }
            [DataSourceBinding("创建时间", 200, 2)]
            public string CreateTime { get; set; }
        }

        #endregion

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            SetDataGridBinding(list, new UIModel(), Data);
        }

        #region UI Mehod

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            AddPurchasePlan a = new AddPurchasePlan();
            a.ShowDialog();
            if (a.Succeed)
            {
                UpdatePager(null, null);
            }
            MaskVisible(false);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void btnCopyCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((sender as Button).Tag.ToString());
            Notice.Show("单号已成功复制到剪切板", "成功提醒", 2, MessageBoxIcon.Success);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void txtPlanCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdatePager(null, null);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            UIModel selectModel = Data.Single(c => c.Id == id);
            var result = MessageBoxX.Show($"是否确认删除编号为[{selectModel.PlanCode}]的采购计划？", "删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (DBContext context = new DBContext())
                {
                    //删除计划
                    context.PurchasePlan.Remove(context.PurchasePlan.First(c => c.Id == id));
                    Data.Remove(selectModel);

                    //删除计划项
                    context.PurchasePlanItem.RemoveRange(context.PurchasePlanItem.Where(c => c.PlanCode == selectModel.PlanCode));

                    context.SaveChanges();
                }
                UpdatePager(null, null);
            }
        }

        #endregion

        private async void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            if (e == null) gPager.CurrentIndex = 1;//如果是通过查询或者刷新点击的 直接显示第一页

            //查询条件
            string code = txtPlanCode.Text;
            bool finished = (bool)cbFinished.IsChecked;
            bool enableTime = (bool)cbEnableTime.IsChecked;
            DateTime start = dtStart.SelectedDateTime;
            DateTime end = dtEnd.SelectedDateTime;

            string listName = list.Name;

            Data.Clear();//先清空再加入页面数据

            using (DBContext context = new DBContext())
            {
                Expression<Func<PurchasePlan, bool>> _where = n => GetPagerWhere(n, code, finished, enableTime, start, end);//按条件查询
                Expression<Func<PurchasePlan, DateTime>> _orderByDesc = n => n.CreateTime;//按时间倒序
                //开始分页查询数据
                var _zPager = await PagerCommon.BeginEFDataPagerAsync(context.PurchasePlan, _where, _orderByDesc, gLoading, gPager, bNoData, new Control[1] { list });
                if (!_zPager.Result) return;
                List<PurchasePlan> _list = _zPager.EFDataList;

                #region 页面数据填充

                foreach (var item in _list)
                {
                    string creator = context.User.Any(c => c.Id == item.Creator) ? context.User.First(c => c.Id == item.Creator).Name : "已删除";
                    var items = context.PurchasePlanItem.Where(c => c.PlanCode == item.PlanCode).ToList();

                    decimal priceCount = 0;
                    int count = 0;
                    int finish = 0;
                    decimal finishedCount = 0;

                    for (int i = 0; i < items.Count; i++)
                    {
                        var d = items[i];
                        count += d.Count;
                        finish += d.Finished;
                        finishedCount += (decimal)d.Finished / d.Count;

                        //完成记录
                        var plog = context.PurchasePlanLog.Where(c => c.ItemId == d.Id).ToList();
                        for (int j = 0; j < plog.Count; j++)
                        {
                            var l = plog[j];
                            priceCount += l.Count * l.PurchasePrice;
                        }
                    }
                    var _model = DBItem2UIModel(item, listName, creator, finishedCount, items.Count, priceCount);
                    Data.Add(_model);
                }

                #endregion
            }

            //结尾处必须结束分页查询
            PagerCommon.EndEFDataPager();
        }

        private UIModel DBItem2UIModel(PurchasePlan item, string _listName, string _creator, decimal _finishedCount, int _itemCount, decimal _priceCount)
        {
            UIModel _model = new UIModel()
            {
                CreateTime = item.CreateTime.ToString("yy年MM月dd日 HH时mm分"),
                Creator = _creator,
                Finish = "0%",
                IsChecked = SelectedTableDataAny(_listName, c => c.Id == item.Id),
                Id = item.Id,
                PlanCode = item.PlanCode,
                PriceCount = 0
            };
            _model.Finish = $"{Math.Round(_finishedCount / _itemCount * 100, 2)}%";
            _model.PriceCount = _priceCount;

            return _model;
        }

        protected bool GetPagerWhere(PurchasePlan _purchasePlan, string _code, bool _finished, bool _enableTime, DateTime _start, DateTime _end)
        {
            bool resultCondition = true;
            if (_code.NotEmpty())
            {
                //根据名称检索
                resultCondition &= _purchasePlan.PlanCode.Contains(_code);
            }
            if (_finished)
            {
                resultCondition &= !_purchasePlan.Finished;
            }

            if (_enableTime)
            {
                resultCondition &= _purchasePlan.CreateTime >= _start && _purchasePlan.CreateTime <= _end;
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
                var _list = new List<PurchasePlan>();
                using (DBContext context = new DBContext())
                {
                    _list = context.PurchasePlan.OrderByDescending(c => c.CreateTime).ToList();
                    foreach (var item in _list)
                    {
                        string creator = context.User.Any(c => c.Id == item.Creator) ? context.User.First(c => c.Id == item.Creator).Name : "已删除";
                        var items = context.PurchasePlanItem.Where(c => c.PlanCode == item.PlanCode).ToList();

                        decimal priceCount = 0;
                        int count = 0;
                        int finish = 0;
                        decimal finishedCount = 0;

                        for (int i = 0; i < items.Count; i++)
                        {
                            var d = items[i];
                            count += d.Count;
                            finish += d.Finished;
                            finishedCount += (decimal)d.Finished / d.Count;

                            //完成记录
                            var plog = context.PurchasePlanLog.Where(c => c.ItemId == d.Id).ToList();
                            for (int j = 0; j < plog.Count; j++)
                            {
                                var l = plog[j];
                                priceCount += l.Count * l.PurchasePrice;
                            }
                        }
                        var _model = DBItem2UIModel(item, listName, creator, finishedCount, items.Count, priceCount);

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

        private void btnEditFinish_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null) return;
            UIModel selectedModel = list.SelectedItem as UIModel;

            MaskVisible(true);
            AddPurchasePlan a = new AddPurchasePlan(selectedModel.Id);
            a.ShowDialog();
            if (a.Succeed)
            {
                var _model = Data.Single(c => c.Id == selectedModel.Id);
                _model.Finish = $"{a.FinishedPre}%";//完成量
                _model.PriceCount = a.FinishedPrice;
            }
            MaskVisible(false);
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
