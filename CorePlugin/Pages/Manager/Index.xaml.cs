using CorePlugin.Windows;
using Common;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// AdminIndex.xaml 的交互逻辑
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
            this.Order = 0;
        }

        private void LoadLog()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                log.ItemsSource = null;
                log.ItemsSource = context.Log.OrderByDescending(c => c.CreateTime).Take(15).ToList();
            }
        }

        private void LoadChart()
        {
            SeriesCollection series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title="收入",
                            Fill =ColorHelper.ConvertToSolidColorBrush("#199700FF"),
                            Stroke =ColorHelper.ConvertToSolidColorBrush("#7F9700FF"),
                            Values = new ChartValues<ObservableValue>
                            {
                                new ObservableValue(4),
                                new ObservableValue(2),
                                new ObservableValue(8),
                                new ObservableValue(2),
                                new ObservableValue(3),
                                new ObservableValue(0),
                                new ObservableValue(1),
                            },
                            DataLabels = true,
                            LabelPoint = point => point.Y.ToString()
                        },
                        new LineSeries
                        {
                            Title="支出",
                            Fill = ColorHelper.ConvertToSolidColorBrush("#19FF0000"),
                            Stroke =ColorHelper.ConvertToSolidColorBrush("#FFFF0000"),
                            Values = new ChartValues<ObservableValue>
                            {
                                new ObservableValue(1),
                                new ObservableValue(3),
                                new ObservableValue(6),
                                new ObservableValue(8),
                                new ObservableValue(12),
                                new ObservableValue(0),
                                new ObservableValue(3),
                            },
                            DataLabels = true,
                            LabelPoint = point =>  point.Y.ToString()
                        }
                    };
            SeriesCollection = series;

            List<string> _s = new List<string>();
            using (CoreDBContext context = new CoreDBContext())
            {
                //lblGoodsCount.Content = context.Goods.Count(c => !c.IsDel);
                //lblStaffCount.Content = context.Staff.Count(c => !c.IsDel);
                //lblUserCount.Content = context.User.Count(c => !c.IsDel);

                for (int i = 0; i < 7; i++)
                {
                    DateTime _time = DateTime.Now.AddDays((6 - i) * -1);

                    //var ftIn = context.FinanceType.Where(c => c.TypeId == 0).ToList();//收入
                    //var ftOut = context.FinanceType.Where(c => c.TypeId == 1).ToList();//支出

                    //decimal inCount = 0;
                    //decimal outCount = 0;

                    //DateTime minTime = _time.MinDate();
                    //DateTime maxTime = _time.MaxDate();

                    //foreach (var item in ftIn)
                    //{
                    //    if (context.FinanceBill.Any(c => c.AddType == item.Id && c.BillTime >= minTime && c.BillTime <= maxTime))
                    //        inCount += context.FinanceBill.Where(c => c.AddType == item.Id && c.BillTime >= minTime && c.BillTime <= maxTime).Sum(c => c.Price);
                    //}
                    //foreach (var item in ftOut)
                    //{
                    //    if (context.FinanceBill.Any(c => c.AddType == item.Id && c.BillTime >= minTime && c.BillTime <= maxTime))
                    //        outCount += context.FinanceBill.Where(c => c.AddType == item.Id && c.BillTime >= minTime && c.BillTime <= maxTime).Sum(c => c.Price);
                    //}

                    //series[0].Values.Add(new ObservableValue((double)inCount));
                    //series[1].Values.Add(new ObservableValue((double)outCount));

                    _s.Add(_time.ToString("MM/dd"));
                }
            }
            Labels = _s.ToArray();

            Formatter = value => $"value {value}";

            PointLabel = chartPoint =>
    string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            DataContext = null;
            DataContext = this;
        }


        protected override void OnPageLoaded()
        {
            LoadChart();
            LoadLog();
        }
    }
}
