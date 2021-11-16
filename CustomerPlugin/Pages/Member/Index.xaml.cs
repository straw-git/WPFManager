using Common;
using Common.Data.Local;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace CustomerPlugin.Pages.Member
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Index : BasePage
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public Func<ChartPoint, string> PointLabel
        {
            get; set;
        }
        public Index()
        {
            InitializeComponent();
            Order = 0;

            string skinColor = LocalSkin.GetModelById(LocalSettings.settings.SkinId).SkinColor;
            SeriesCollection series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title="本月新增",
                            Stroke =ColorHelper.ConvertToSolidColorBrush(skinColor),
                            Values = new ChartValues<ObservableValue>
                            {
                                //new ObservableValue(4),
                                //new ObservableValue(2),
                                //new ObservableValue(8),
                                //new ObservableValue(2),
                                //new ObservableValue(3),
                                //new ObservableValue(0),
                                //new ObservableValue(1),
                                //new ObservableValue(8),
                                //new ObservableValue(0),
                                //new ObservableValue(5),
                                //new ObservableValue(3),
                                //new ObservableValue(1),
                            },
                            DataLabels = true,
                            LabelPoint = point => point.Y.ToString()
                        },
                        new LineSeries
                        {
                            Title="上月同时",
                            Stroke =ColorHelper.ConvertToSolidColorBrush("#FFFF0000"),
                            Values = new ChartValues<ObservableValue>
                            {
                                //new ObservableValue(1),
                                //new ObservableValue(3),
                                //new ObservableValue(6),
                                //new ObservableValue(8),
                                //new ObservableValue(12),
                                //new ObservableValue(0),
                                //new ObservableValue(3),
                                //new ObservableValue(7),
                                //new ObservableValue(2),
                                //new ObservableValue(7),
                                //new ObservableValue(0),
                                //new ObservableValue(3),
                            },
                            DataLabels = true,
                            LabelPoint = point =>  point.Y.ToString()
                        }
                    };
            SeriesCollection = series;

            List<string> lableData = new List<string>();

            //本月多少天
            DateTime dtNow = DateTime.Now;
            int nowDays = DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            //上月多少天
            DateTime dtTop = DateTime.Now.AddMonths(-1);
            int topDays = DateTime.DaysInMonth(dtTop.Year, dtTop.Month);

            using (DBContext context = new DBContext())
            {
                int days = nowDays > topDays ? nowDays : topDays;
                for (int i = 1; i <= days; i++)
                {
                    lableData.Add($"{i}号");

                    DateTime nowDayTime = DateTime.Now;
                    DateTime topDayTime = DateTime.Now;

                    if (DateTime.TryParse($"{dtNow.Year}-{dtNow.Month}-{i}", out nowDayTime))
                    {
                        DateTime min = nowDayTime.MinDate();
                        DateTime max = nowDayTime.MaxDate();
                        //本月
                        series[0].Values.Add(new ObservableValue(context.Member.Count(c => c.CreateTime >= min && c.CreateTime <= max)));
                    }

                    if (DateTime.TryParse($"{dtTop.Year}-{dtTop.Month}-{i}", out topDayTime))
                    {
                        DateTime min = topDayTime.MinDate();
                        DateTime max = topDayTime.MaxDate();
                        //上月
                        series[1].Values.Add(new ObservableValue(context.Member.Count(c => c.CreateTime >= min && c.CreateTime <= max)));
                    }
                }
            }

            Labels = lableData.ToArray();

            Formatter = value => $"value {value}";

            PointLabel = chartPoint =>
    string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            DataContext = null;
            DataContext = this;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string CreateTime { get; set; }
            public string Name { get; set; }
            public string Sex { get; set; }
            public int Age { get; set; }
            public string MemberLevel { get; set; }
            public string PayPrice { get; set; }
            private string memberPrice = "0";
            public string MemberPrice
            {
                get => memberPrice;
                set
                {
                    memberPrice = value;
                    NotifyPropertyChanged("MemberPrice");
                }
            }

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
        }

        #region Private Method

        private void LoadPager()
        {
            using (var context = new DBContext())
            {
                string name = txtName.Text;
                bool useTime = (bool)cbEnableTime.IsChecked;
                DateTime startTime = dtStart.SelectedDateTime.MinDate();
                DateTime endTime = dtEnd.SelectedDateTime.MaxDate();

                var members = context.Member.AsEnumerable();

                if (!string.IsNullOrEmpty(name))
                {
                    members = members.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name));
                }
                if (useTime)
                {
                    members = members.Where(c => c.CreateTime >= startTime && c.CreateTime <= endTime);
                }
                dataCount = members.Count();
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

            List<DBModels.Member. Member> models = new List<DBModels.Member.Member>();

            string name = txtName.Text;
            bool useTime = (bool)cbEnableTime.IsChecked;
            DateTime startTime = dtStart.SelectedDateTime.MinDate();
            DateTime endTime = dtEnd.SelectedDateTime.MaxDate();

            await Task.Run(() =>
            {
                using (var context = new DBContext())
                {
                    var members = context.Member.AsEnumerable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        members = members.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name));
                    }
                    if (useTime)
                    {
                        members = members.Where(c => c.CreateTime >= startTime && c.CreateTime <= endTime);
                    }
                    models = members.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
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
                        Age = 0,
                        CreateTime = item.CreateTime.ToString("yyyy年MM月dd日 HH时mm分"),
                        Id = item.Id,
                        MemberLevel = "",
                        MemberPrice = item.Price.ToString(),
                        Name = item.Name,
                        PayPrice = "0",
                        Sex = ""
                    };

                    ////年龄
                    //var _patient = context.Patients.First(c => c.Id == item.PatientId);
                    //if (_patient.IdCard.NotEmpty())
                    //{
                    //    DateTime brthday = IdCardCommon.GetBirthday(_patient.IdCard);
                    //    _model.Age = DateTime.Now.Year - brthday.Year;
                    //}

                    ////花销
                    //decimal priceLogCount = 0;
                    //if (context.MemberPriceLogs.Count(c => c.MemberId == item.Id) > 0)
                    //    priceLogCount = context.MemberPriceLogs.Where(c => c.MemberId == item.Id).Sum(c => c.Price);
                    ////会员等级
                    //var memberLevel = new DB.Patients.MemberLevel();
                    //if (context.MemberLevels.Any(c => c.LogPriceCount >= priceLogCount))
                    //{
                    //    memberLevel = context.MemberLevels.First(c => c.LogPriceCount >= priceLogCount);
                    //}
                    //else
                    //{
                    //    memberLevel = context.MemberLevels.OrderByDescending(c => c.LogPriceCount).First();
                    //}

                    //_model.MemberLevel = memberLevel.Name;
                    //_model.Sex = _patient.Sex;

                    ////销售
                    //decimal salePrice = 0;
                    //if (context.Sales.Count(c => c.PatientId == _patient.Id) > 0)
                    //    salePrice = context.Sales.Where(c => c.PatientId == _patient.Id).Sum(c => c.PriceCount);
                    //_model.PayPrice = salePrice.ToString();

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        #endregion

        #region UI Method

        private void btnBindCard_Click(object sender, RoutedEventArgs e)
        {
            int memberId = (sender as Button).Tag.ToString().AsInt();

        }

        private void cbEnableTime_Checked(object sender, RoutedEventArgs e)
        {
            dtStart.IsEnabled = true;
            dtEnd.IsEnabled = true;
        }

        private void cbEnableTime_Unchecked(object sender, RoutedEventArgs e)
        {
            dtStart.IsEnabled = false;
            dtEnd.IsEnabled = false;
        }

        private void btnReceive_Click(object sender, RoutedEventArgs e)
        {
            //充值
            int memberId = (sender as Button).Tag.ToString().AsInt();

            //MaskVisible(true);
            //MemberRecharge a = new MemberRecharge(memberId);
            //a.ShowDialog();
            //MaskVisible(false);

            //if (a.Succeed)
            //{
            //    Data.Single(c => c.Id == memberId).MemberPrice = a.Price.ToString();
            //}
        }

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            btnRef_Click(null, null);
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
