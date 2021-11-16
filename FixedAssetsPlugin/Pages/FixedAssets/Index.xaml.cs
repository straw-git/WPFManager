
using DBModels.Sys;
using LiveCharts;
using LiveCharts.Wpf;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;


namespace FixedAssetsPlugin.Pages.FixedAssets
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Index : BasePage
    {
        public Index()
        {
            InitializeComponent();
            this.Order = 0;
        }


        #region Chart

        private void LoadChartByCount()
        {
            ccCount.Series = new SeriesCollection();
            ccCount.Series.Clear();
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var fixedAssets = context.FixedAssets.ToList();
                ccCount.Series.Add(new ColumnSeries()
                {
                    Title = "按固定资产数量",
                    Values = new LiveCharts.ChartValues<int>(),
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0}", point.Y)
                });
                ccCount.AxisX = new AxesCollection();
                ccCount.AxisX.Add(new Axis() { Labels = new List<string>() });
                foreach (var item in fixedAssets)
                {
                    ccCount.Series[0].Values.Add(item.Count);
                    ccCount.AxisX[0].Labels.Add(item.Name);
                }
            }
        }

        private void LoadChartByLocation()
        {
            ccLocation.Series = new SeriesCollection();

            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var locations = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsLocation).ToList();
                ccLocation.Series.Add(new RowSeries()
                {
                    Title = "按存放位置",
                    Values = new LiveCharts.ChartValues<int>(),
                    DataLabels = true,
                    LabelPoint = point => string.Format("{0}", point.X)
                });
                ccLocation.AxisY = new AxesCollection();
                ccLocation.AxisY.Add(new Axis() { Labels = new List<string>() });
                foreach (var item in locations)
                {
                    int count = 0;
                    if (context.FixedAssets.Any(c => c.Location == item.Id))
                        count = context.FixedAssets.Where(c => c.Location == item.Id).Sum(c => c.Count);


                    ccLocation.Series[0].Values.Add(count);
                    ccLocation.AxisY[0].Labels.Add(item.Name);
                }
            }
        }

        private void LoadChartByState()
        {
            pcState.Series.Clear();
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var states = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsState).ToList();
                foreach (var item in states)
                {
                    int count = 0;
                    if (context.FixedAssets.Any(c => c.State == item.Id))
                        count = context.FixedAssets.Where(c => c.State == item.Id).Sum(c => c.Count);

                    pcState.Series.Add(new PieSeries()
                    {
                        Title = item.Name,
                        Values = new LiveCharts.ChartValues<int> { count },
                        DataLabels = true,
                        LabelPoint = point => string.Format("{0:P}", point.Participation),
                        Foreground = new SolidColorBrush(Colors.Black),
                    });
                }
            }
        }

        #endregion

        #region Models

        class UIModel
        {
            public string Id { get; set; }
            public string StateName { get; set; }
            public string Name { get; set; }
            public string ModelName { get; set; }
            public string UnitName { get; set; }
            public string LocationName { get; set; }
            public string SubLocationName
            {
                get
                {
                    return LocationName.Length > 20 ? $"{LocationName.Substring(0, 20)}..." : LocationName;
                }
            }
            public string PrincipalName { get; set; }
            public int Count { get; set; }
        }

        class LogUIModel
        {
            public string CheckTime { get; set; }
            public string Name { get; set; }
            public string ModelName { get; set; }
            public string UnitName { get; set; }
            public string UpdateStr { get; set; }
            public string CheckStaffName { get; set; }
        }

        #endregion


        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        int dataCount = 0;
        int pagerCount = 0;
        int pageSize = 10;
        int currPage = 1;

        ObservableCollection<LogUIModel> LogData = new ObservableCollection<LogUIModel>();
        int dataCount_log = 0;
        int pagerCount_log = 0;
        int pageSize_log = 10;
        int currPage_log = 1;

        bool running = false;


        protected override void OnPageLoaded()
        {
            LoadChartByState();
            LoadChartByLocation();
            LoadChartByCount();
            LoadState();
            list.ItemsSource = Data;
            logList.ItemsSource = LogData;

            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                b1.Visibility = b2.Visibility = b3.Visibility = context.FixedAssets.Any() ? Visibility.Collapsed : Visibility.Visible;
            }

            LoadLogPager();
            UpdateLogGridAsync();
        }

        #region Private Method

        private void LoadState()
        {
            cbState.Items.Clear();
            using (DBContext context = new DBContext())
            {
                var states = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsState).ToList();
                if (states == null) states = new List<SysDic>();
                states.Insert(0, new SysDic()
                {
                    Id = 0,
                    Name = "全部"
                });
                cbState.ItemsSource = states;
                cbState.DisplayMemberPath = "Name";
                cbState.SelectedValuePath = "Id";

                cbState.SelectedIndex = 0;
            }
        }

        private void LoadPager()
        {
            using (var context = new FixedAssetsDBContext())
            {
                string name = txtName.Text;
                int stateId = cbState.SelectedValue.ToString().AsInt();
                string code = txtCode.Text;

                var fixedAssets = context.FixedAssets.Where(c => !c.IsDel);

                if (name.NotEmpty())
                {
                    fixedAssets = fixedAssets.Where(c => c.Name.Contains(name));
                }
                if (stateId > 0)
                {
                    fixedAssets = fixedAssets.Where(c => c.State == stateId);
                }
                if (code.NotEmpty())
                {
                    fixedAssets = fixedAssets.Where(c => c.Id.Contains(code));
                }

                dataCount = fixedAssets.Count();
            }
            pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);

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

            List<DB.FixedAssets> models = new List<DB.FixedAssets>();
            string name = txtName.Text;
            int stateId = cbState.SelectedValue.ToString().AsInt();
            string code = txtCode.Text;

            await Task.Run(() =>
            {
                using (var context = new FixedAssetsDBContext())
                {
                    var fixedAssets = context.FixedAssets.Where(c => !c.IsDel);

                    if (name.NotEmpty())
                    {
                        fixedAssets = fixedAssets.Where(c => c.Name.Contains(name));
                    }
                    if (stateId > 0)
                    {
                        fixedAssets = fixedAssets.Where(c => c.State == stateId);
                    }
                    if (code.NotEmpty())
                    {
                        fixedAssets = fixedAssets.Where(c => c.Id.Contains(code));
                    }

                    models = fixedAssets.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();

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
                        Count = item.Count,
                        Id = item.Id,
                        LocationName = context.SysDic.First(c => c.Id == item.Location).Name,
                        ModelName = item.ModelName,
                        Name = item.Name,
                        PrincipalName = item.PrincipalName,
                        StateName = context.SysDic.First(c => c.Id == item.State).Name,
                        UnitName = item.UnitName
                    };

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        private void LoadLogPager()
        {
            using (var context = new FixedAssetsDBContext())
            {
                dataCount_log = context.FixedAssetsCheck.Count();
            }
            pagerCount_log = PagerGlobal.GetPagerCount(dataCount_log, pageSize_log);

            if (currPage_log > pagerCount_log) currPage_log = pagerCount_log;
            gLogPager.CurrentIndex = currPage_log;
            gLogPager.TotalIndex = pagerCount_log;
        }

        private async void UpdateLogGridAsync()
        {
            if (running) return;
            running = true;
            LogData.Clear();

            List<DB.FixedAssetsCheck> models = new List<DB.FixedAssetsCheck>();

            await Task.Run(() =>
            {
                using (var context = new FixedAssetsDBContext())
                {
                    models = context.FixedAssetsCheck.OrderByDescending(c => c.CreateTime).Skip(pageSize_log * (currPage_log - 1)).Take(pageSize_log).ToList();
                }
            });

            await Task.Delay(300);

            bLogNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                foreach (var item in models)
                {
                    LogUIModel _model = new LogUIModel()
                    {
                        CheckStaffName = item.StaffName,
                        CheckTime = item.Check.ToString("yyyy年MM月dd日"),
                        ModelName = "",
                        Name = "",
                        UnitName = "",
                        UpdateStr = ""
                    };

                    var fixedAssets = context.FixedAssets.First(c => c.Id == item.FixedAssetsCode);
                    _model.ModelName = fixedAssets.ModelName;
                    _model.Name = fixedAssets.Name;
                    _model.UnitName = fixedAssets.UnitName;

                    string updateStr = "";
                    if (item.OldCount != item.NewCount)
                    {
                        updateStr += $"数量[{item.OldCount}->{item.NewCount }];";
                    }
                    if (item.OldLocation != item.NewLocation)
                    {
                        string oldName = item.OldLocation == 0 ? "无" : context.SysDic.First(c => c.Id == item.OldLocation).Name;
                        updateStr += $"位置[{oldName}->{context.SysDic.First(c => c.Id == item.NewLocation).Name}];";
                    }
                    if (item.OldPrincipalName != item.NewPrincipalName)
                    {
                        string oldName = item.OldPrincipalName.IsNullOrEmpty() ? "无" : item.OldPrincipalName;
                        updateStr += $"负责人[{oldName}->{item.NewPrincipalName}];";
                    }
                    if (item.OldState != item.NewState)
                    {
                        string oldName = item.OldState == 0 ? "无" : context.SysDic.First(c => c.Id == item.OldState).Name;
                        updateStr += $"状态[{oldName}->{context.SysDic.First(c => c.Id == item.NewState).Name}];";
                    }
                    if (item.OldPrincipalPhone != item.NewPrincipalPhone)
                    {
                        updateStr += $"负责人电话号;";
                    }
                    _model.UpdateStr = updateStr;
                    LogData.Add(_model);
                }
            }
            running = false;
        }

        #endregion

        #region UI Method

        private void pcState_DataClick(object sender, ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
        }

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            UpdateGridAsync();
        }

        private void gLogPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage_log = gLogPager.CurrentIndex;
            UpdateLogGridAsync();
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
