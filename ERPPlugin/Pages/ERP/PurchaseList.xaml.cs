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
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string PlanCode { get; set; }
            private decimal priceCount = 0;
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
            public string Finish
            {
                get => finish;
                set
                {
                    finish = value;
                    NotifyPropertyChanged("Finish");
                }
            }
            public string Creator { get; set; }
            public string CreateTime { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        int dataCount = 0;
        int pagerCount = 0;
        int pageSize = 10;
        int currPage = 1;
        bool running = false;

        protected override void OnPageLoaded()
        {
            list.ItemsSource = Data;
            LoadPager();
            UpdateGridAsync();
        }

        #region Private Method

        private void LoadPager()
        {
            string code = txtPlanCode.Text;
            bool finished = (bool)cbFinished.IsChecked;
            bool enableTime = (bool)cbEnableTime.IsChecked;
            DateTime start = dtStart.SelectedDateTime;
            DateTime end = dtEnd.SelectedDateTime;

            using (var context = new DBContext())
            {
                var plans = context.PurchasePlan.Where(c => !c.IsDel);

                if (!string.IsNullOrEmpty(code))
                {
                    plans = plans.Where(c => c.PlanCode.Contains(code));
                }
                if (finished)
                {
                    plans = plans.Where(c => !c.Finished);
                }

                if (enableTime)
                {
                    plans = plans.Where(c => c.CreateTime >= start && c.CreateTime <= end);
                }

                dataCount = plans.Count();
                pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);
            }

            if (currPage > pagerCount) currPage = pagerCount;
            gPager.CurrentIndex = currPage;
            gPager.TotalIndex = pagerCount;
        }

        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();
            if (running) return;

            running = true;
            Data.Clear();

            List<PurchasePlan> models = new List<PurchasePlan>();

            string code = txtPlanCode.Text;
            bool finished = (bool)cbFinished.IsChecked;
            bool enableTime = (bool)cbEnableTime.IsChecked;
            DateTime start = dtStart.SelectedDateTime;
            DateTime end = dtEnd.SelectedDateTime;

            await Task.Run(() =>
            {
                using (var context = new DBContext())
                {
                    var plans = context.PurchasePlan.Where(c => !c.IsDel);

                    if (!string.IsNullOrEmpty(code))
                    {
                        plans = plans.Where(c => c.PlanCode.Contains(code));
                    }
                    if (finished)
                    {
                        plans = plans.Where(c => !c.Finished);
                    }

                    if (enableTime)
                    {
                        plans = plans.Where(c => c.CreateTime >= start && c.CreateTime <= end);
                    }

                    models = plans.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (DBContext context = new DBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        CreateTime = item.CreateTime.ToString("yy年MM月dd日 HH时mm分"),
                        Creator = context.User.First(c => c.Id == item.Creator).Name,
                        Finish = "0%",
                        Id = item.Id,
                        PlanCode = item.PlanCode,
                        PriceCount = 0
                    };

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

                    _model.Finish = $"{Math.Round(finishedCount / items.Count * 100, 2)}%";
                    _model.PriceCount = priceCount;
                    Data.Add(_model);
                }
            }

            HideLoadingPanel();
            running = false;
        }

        #endregion

        #region UI Mehod

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            AddPurchasePlan a = new AddPurchasePlan();
            a.ShowDialog();
            if (a.Succeed)
            {
                LoadPager();
                UpdateGridAsync();
            }
            MaskVisible(false);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
        }

        private void btnCopyCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((sender as Button).Tag.ToString());
            Notice.Show("单号已成功复制到剪切板", "成功提醒", 2, MessageBoxIcon.Success);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridAsync();
        }

        private void txtPlanCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateGridAsync();
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
                LoadPager();
            }
        }

        #endregion

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;
                gPager.IsEnabled = false;
                bNoData.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;
                gPager.IsEnabled = true;
                bNoData.IsEnabled = true;

                OnLoadingHideComplate();
            }
        }

        private void OnLoadingHideComplate()
        {

        }

        private void OnLoadingShowComplate()
        {

        }


        #endregion

    }
}
