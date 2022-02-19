using Common;
using Common.Utils;
using Panuon.UI.Silver;
using SalePlugin.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalePlugin.Pages.CustomerOrder
{
    /// <summary>
    /// CreateOrder.xaml 的交互逻辑
    /// </summary>
    public partial class CreateOrder : BasePage
    {
        public CreateOrder()
        {
            InitializeComponent();
            Order = 1;
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public string GoodsId { get; set; }
            public string GoodsName { get; set; }
            public string Specification { get; set; }
            public string UnitName { get; set; }
            public decimal Price { get; set; }
            public string StockCount { get; set; }
            private int count = 1;
            public int Count
            {
                get => count;
                set
                {
                    count = value;
                    if (count <= 0) count = 1;
                    NotifyPropertyChanged("Count");
                    NotifyPropertyChanged("TotalPrice");
                }
            }

            public decimal TotalPrice
            {
                get
                {
                    return Price * Count;
                }
            }
        }

        class ResultUIModels
        {
            public string GoodsId { get; set; }
            public string GoodsName { get; set; }
            public string SubName
            {
                get
                {
                    return GoodsName.Length > 15 ? $"{GoodsName.Substring(0, 15)}..." : GoodsName;
                }
            }
            public int Count { get; set; }
            public decimal TotalPrice { get; set; }
        }

        class CustomerUIModel : BaseUIModel
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerSex { get; set; }
            public string CustomerAge { get; set; }
            public string CustomerPhone { get; set; }
            public string CustomerDate { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        ObservableCollection<ResultUIModels> ResultData = new ObservableCollection<ResultUIModels>();
        ObservableCollection<CustomerUIModel> CustomerData = new ObservableCollection<CustomerUIModel>();

        private bool openKey = false;//是否已开启按键操作（搜索时敲定回车后）
        private bool openKeyNumber = false;//是否开启数字输入（列表中选中项以后）
        private bool isTab = false;//是否触动了Tab键
        public bool IsTab
        {
            get => isTab;
            set
            {
                isTab = value;
                if (isTab)
                {
                    gp2.IsEnabled = true;
                    gp1.IsEnabled = false;
                }
                else
                {
                    gp2.IsEnabled = false;
                    gp1.IsEnabled = true;
                }
            }
        }
        private int deleteIndex = 0;//删除的账单Index

        protected override void OnPageLoaded()
        {
            list.ItemsSource = Data;
            result.ItemsSource = ResultData;
            customerList.ItemsSource = CustomerData;
            ReLoadTodayCustomerTemp();
            txtName.Focus();
            IsTab = false;
        }

        /// <summary>
        /// 刷新临时客户列表
        /// </summary>
        private void ReLoadTodayCustomerTemp()
        {
            CustomerData.Clear();
            using (CustomerDBContext context = new CustomerDBContext())
            {
                //获取当天所有访客
                DateTime minDate = DateTime.Now.MinDate();
                var customerTemps = context.CustomerTemp.Where(c => c.CreateTime >= minDate && c.CreateTime <= DateTime.Now).ToList();

                foreach (var cus in customerTemps)
                {
                    var customer = context.Customer.First(c => c.Id == cus.CustomerId);
                    CustomerUIModel model = new CustomerUIModel();
                    model.CustomerDate = cus.CreateTime.ToString("yy-MM-dd");
                    model.CustomerId = cus.Id;
                    model.CustomerName = cus.Name;
                    model.CustomerPhone = cus.Phone;
                    model.CustomerSex = customer.Sex;
                    model.CustomerAge = (DateTime.Now.Year - IdCardCommon.GetBirthday(customer.IdCard).Year).ToString();

                    CustomerData.Add(model);
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.ShowLeftMenu(true);
            ParentWindow.ShowTopMenu(true);
            ParentWindow.WindowState = WindowState.Normal;

            ParentWindow.ReLoadCurrTopMenu();
        }

        private void txtName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string name = txtName.Text.Trim();
            if (name.IsNullOrEmpty())
            {
                Data.Clear();
                openKey = false;
                return;
            }

            if (!openKey)
            {
                //筛选物品
                if (e.Key == Key.Enter)
                {
                    #region 筛选物品

                    Data.Clear();


                    //using (ERPDBContext context = new ERPDBContext())
                    //{
                    //    var goods = context.Goods.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name)).ToList();
                    //    foreach (var g in goods)
                    //    {
                    //        UIModel model = new UIModel();
                    //        model.Count = 1;
                    //        model.GoodsId = g.Id;
                    //        model.GoodsName = g.Name;
                    //        model.Price = g.SalePrice;
                    //        model.Specification = g.Specification;
                    //        model.StockCount = g.IsStock && context.Stock.Any(c => c.GoodsId == g.Id) ? context.Stock.First(c => c.GoodsId == g.Id).Count.ToString() : "无";
                    //        model.UnitName = context.SysDic.Any(c => c.Id == g.UnitId) ? context.SysDic.First(c => c.Id == g.UnitId).Name : "已删除";

                    //        Data.Add(model);
                    //    }
                    //}

                    if (Data.Count > 0)
                    {
                        openKey = true;//开启热键
                        if (list.Visibility == Visibility.Hidden) list.Visibility = Visibility.Visible;
                        list.SelectedIndex = 0;
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// 鼠标双击列表（就是回车效果）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //鼠标双击后 将选中条目加入到结算列表中
            string goodsId = (list.SelectedItem as UIModel).GoodsId;
            var selectedModel = Data.Single(c => c.GoodsId == goodsId);

            ResultData.Add(new ResultUIModels()
            {
                Count = selectedModel.Count,
                GoodsId = selectedModel.GoodsId,
                GoodsName = $"{selectedModel.GoodsName}({selectedModel.Specification})",
                TotalPrice = selectedModel.TotalPrice
            });

            lblTotalPrice.Content = ResultData.Sum(c => c.TotalPrice);

            selectedModel.Count = 1;

            Data.Clear();
            txtName.Clear();
            txtName.Focus();
            openKey = false;
            openKeyNumber = false;
        }

        private void BasePage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (ResultData.Count == 0)
                {
                    MessageBoxX.Show("当前没有任何收费项", "空值提醒");
                    return;
                }
                EscCommend();
                var result = MessageBoxX.Show("是否确认提交当前收银", "收银提醒", null, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("成功");
                    ResultData.Clear();
                    lblTotalPrice.Content = 0;
                }
            }
            else if (e.Key == Key.Escape)
            {
                EscCommend();
            }
            else if (openKey && !IsTab)
            {
                #region 上下键

                if (e.Key == Key.Down)
                {
                    openKeyNumber = false;
                    string goodsId = (list.SelectedItem as UIModel).GoodsId;
                    var selectedModel = Data.Single(c => c.GoodsId == goodsId);
                    selectedModel.Count = 1;

                    list.SelectedIndex += 1;

                }
                if (e.Key == Key.Up)
                {
                    openKeyNumber = false;
                    string goodsId = (list.SelectedItem as UIModel).GoodsId;
                    var selectedModel = Data.Single(c => c.GoodsId == goodsId);
                    selectedModel.Count = 1;

                    if (list.SelectedIndex == 0) return;
                    list.SelectedIndex -= 1;
                }

                #endregion

                #region 左右键

                if (e.Key == Key.Left)
                {
                    string goodsId = (list.SelectedItem as UIModel).GoodsId;
                    var selectedModel = Data.Single(c => c.GoodsId == goodsId);

                    if (selectedModel.Count > 1)
                        selectedModel.Count -= 1;
                }

                if (e.Key == Key.Right)
                {
                    string goodsId = (list.SelectedItem as UIModel).GoodsId;
                    var selectedModel = Data.Single(c => c.GoodsId == goodsId);

                    int storeCount = 0;
                    if (!int.TryParse(selectedModel.StockCount, out storeCount))
                    {
                        storeCount = int.MaxValue;
                    }

                    if (selectedModel.Count + 1 <= storeCount)
                        selectedModel.Count += 1;
                }

                #endregion

                #region 数字键

                if (e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9
                    || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 || e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9)
                {
                    string goodsId = (list.SelectedItem as UIModel).GoodsId;
                    var selectedModel = Data.Single(c => c.GoodsId == goodsId);

                    string count = selectedModel.Count.ToString();

                    if (!openKeyNumber && count == "1")
                    {
                        count = "";
                        openKeyNumber = true;
                    }

                    string keyStr = e.Key.ToString();
                    count += keyStr.Substring(keyStr.Length - 1);
                    int newCount = 0;
                    if (int.TryParse(count, out newCount))
                    {
                        selectedModel.Count = newCount;
                    }
                }

                #endregion

                #region 返回键

                if (e.Key == Key.Back)
                {
                    string goodsId = (list.SelectedItem as UIModel).GoodsId;
                    var selectedModel = Data.Single(c => c.GoodsId == goodsId);

                    string count = selectedModel.Count.ToString();

                    if (count.Length == 1)
                    {
                        count = "1";
                        openKeyNumber = false;
                    }
                    else
                    {
                        count = count.Substring(0, count.Length - 1);
                    }

                    int newCount = 0;
                    if (int.TryParse(count, out newCount))
                    {
                        selectedModel.Count = newCount;
                    }
                }

                #endregion

                #region 回车键

                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    //与双击列表效果一样
                    list_PreviewMouseDoubleClick(null, null);
                }

                #endregion

                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                if (IsTab == false && ResultData.Count == 0) return;//如果左侧列表中啥也没有 不理会

                IsTab = !IsTab;
                if (IsTab)
                {
                    if (ResultData.Count > 0) result.SelectedIndex = 0;
                }
            }
            else if (IsTab)
            {
                #region 上下键

                if (e.Key == Key.Down)
                {
                    result.SelectedIndex += 1;
                }
                if (e.Key == Key.Up)
                {
                    result.SelectedIndex -= 1;
                }
                #endregion

                #region 删除键

                if (e.Key == Key.Delete)
                {
                    deleteIndex = result.SelectedIndex;
                    ResultData.Remove(result.SelectedItem as ResultUIModels);

                    if (ResultData.Count == 0)
                    {
                        IsTab = false;
                    }
                    else
                    {
                        if (deleteIndex == 0) deleteIndex = 1;
                        result.SelectedIndex = deleteIndex - 1;
                    }
                }

                #endregion

                #region 作废

                if (e.Key == Key.F6)
                {
                    if (ResultData.Count == 0) return;

                    var result = MessageBoxX.Show("是否确认作废当前账单？", "作废提醒", null, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        EscCommend();
                        IsTab = false;
                        ResultData.Clear();
                        lblTotalPrice.Content = 0;
                    }
                    txtName.Focus();
                }

                #endregion

                #region 挂存

                if (e.Key == Key.F12)
                {
                    MaskVisible(true);
                    WinTempSingle winTempSingle = new WinTempSingle();
                    winTempSingle.ShowDialog();
                    MaskVisible(false);
                }

                #endregion 
            }
            else if (e.Key == Key.F6)
            {
                if (ResultData.Count == 0) return;

                var result = MessageBoxX.Show("是否确认作废当前账单？", "作废提醒", null, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    EscCommend();
                    IsTab = false;
                    ResultData.Clear();
                    lblTotalPrice.Content = 0;
                }
                txtName.Focus();
            }
            else if (e.Key == Key.F12)
            {
                MaskVisible(true);
                WinTempSingle winTempSingle = new WinTempSingle();
                winTempSingle.ShowDialog();
                MaskVisible(false);
            }
            else
            {
                txtName.Focus();
                txtName_PreviewKeyDown(txtName, e);
            }
        }

        /// <summary>
        /// ESC键触发
        /// </summary>
        private void EscCommend()
        {
            Data.Clear();
            txtName.Clear();
            txtName.Focus();
            openKey = false;
            openKeyNumber = false;
        }

        /// <summary>
        /// Ctrl+S 快捷键 (将当前列表保存为常用项)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CtrlSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ResultData.Count > 0)
            {
                MaskVisible(true);
                WinAddUserCommonObj w = new WinAddUserCommonObj();
                w.ShowDialog();
                MaskVisible(false);
            }
        }

        private void btnUserCommonObj_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            WinUserCommonObj w = new WinUserCommonObj();
            w.ShowDialog();
            MaskVisible(false);
        }

        private void customerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (customerList.SelectedItem != null)
            {
                var selectItem = customerList.SelectedItem as CustomerUIModel;
                bool targetIsChecked = !selectItem.IsChecked;

                foreach (var item in CustomerData)
                {
                    item.IsChecked = false;
                }

                CustomerData.Single(c => c.CustomerId == selectItem.CustomerId).IsChecked = targetIsChecked;

                if (targetIsChecked)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    SelectedTableData(customerList.Name, selectItem);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(customerList.Name, c => c.CustomerId==selectItem.CustomerId);
                }
            }
        }
    }
}
