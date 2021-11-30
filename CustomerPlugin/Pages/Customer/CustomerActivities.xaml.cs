using Common;
using Common.Data.Local;
using DBModels.Activities;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace CustomerPlugin.Pages.Customer
{
    /// <summary>
    /// CustomerActivities.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerActivities : BasePage
    {
        protected override void OnPageLoaded()
        {
            LoadMemberLevel();
            UpdateNormalChart();
            UpdateMemberChart();
        }

        string skinColor = LocalSettings.settings == null ? "#FFFFFF00" : LocalSkin.GetModelById(LocalSettings.settings.SkinId).SkinColor;

        public CustomerActivities()
        {
            InitializeComponent();
            this.Order = 2;

            DataContext = this;

            //测试
            //OnPageLoaded();

        }

        private void UpdateNormalChart()
        {
            normalChart.Series = null;

            #region 大众满减活动

            List<MJActivity> normalList = new List<MJActivity>();

            using (DBContext context = new DBContext())
            {
                if (context.MJActivity.Any(c => c.MemberTypeId == 0))
                {
                    normalNoData.Visibility = Visibility.Collapsed;

                    normalList = context.MJActivity.Where(c => c.MemberTypeId == 0 && c.Type == 0).OrderBy(c => c.M).ToList();
                    string[] nLabels = new string[normalList.Count + 2];
                    nLabels[0] = "0";
                    StepLineSeries lines = new StepLineSeries
                    {
                        Title = "大众消费活动",
                        Stroke = ColorHelper.ConvertToSolidColorBrush(skinColor),
                        Values = new ChartValues<ObservableValue>
                            {
                                new ObservableValue(0)
                            },
                        DataLabels = true,
                        LabelPoint = point => $"消费满（{nLabels[(int)point.X]}）减（{point.Y}）"
                    };

                    for (int i = 0; i < normalList.Count; i++)
                    {
                        var n = normalList[i];
                        nLabels[i + 1] = n.M.ToString();
                        lines.Values.Add(new ObservableValue((double)n.J));
                    }
                    SeriesCollection series = new SeriesCollection { lines };

                    normalChart.AxisX[0].Labels = nLabels;
                    normalChart.Series = series;
                }
                else
                {
                    normalNoData.Visibility = Visibility.Visible;
                }
            }

            #endregion
        }

        private void UpdateMemberChart()
        {
            memberChart.Series = null;

            int memberType = cbMemberLevel.SelectedValue.ToString().AsInt();

            #region 会员满减活动

            List<MJActivity> memberList = new List<MJActivity>();

            using (DBContext context = new DBContext())
            {
                //存在会员活动
                if (context.MJActivity.Any(c => c.MemberTypeId == memberType))
                {
                    memberNoData.Visibility = Visibility.Collapsed;
                    memberList = context.MJActivity.Where(c => c.MemberTypeId == memberType && c.Type == 0).OrderBy(c => c.MemberTypeId).ToList();

                    List<string> lbs = new List<string>() { "0" };

                    for (int i = 0; i < memberList.Count; i++)
                    {
                        string s = memberList[i].M.ToString();
                        if (lbs.IndexOf(s) == -1)
                        {
                            //不存在
                            lbs.Add(s);
                        }
                    }

                    var mLabels = lbs.ToArray();

                    var typeIds = (from c in memberList group c.MemberTypeId by c.MemberTypeId).ToList();
                    SeriesCollection series = new SeriesCollection();

                    foreach (var item in typeIds)
                    {
                        string memberLevel = context.MemberLevel.First(c => c.Id == item.Key).Name;
                        StepLineSeries lines = new StepLineSeries
                        {
                            Title = $"[{memberLevel}]消费活动",
                            Stroke = ColorHelper.ConvertToSolidColorBrush(skinColor),
                            Values = new ChartValues<ObservableValue>
                            {
                                new ObservableValue(0)
                            },
                            DataLabels = true,
                            LabelPoint = point => $"消费满（{mLabels[(int)point.X]}）减（{point.Y}）"
                        };

                        for (int i = 0; i < memberList.Count; i++)
                        {
                            var n = memberList[i];
                            lines.Values.Add(new ObservableValue((double)n.J));
                        }
                        series.Add(lines);
                    }

                    memberChart.AxisX[0].Labels = mLabels;
                    memberChart.Series = series;
                }
                else
                {
                    memberNoData.Visibility = Visibility.Visible;
                }
            }

            #endregion
        }

        private void LoadMemberLevel()
        {
            using (DBContext context = new DBContext())
            {
                var _levels = context.MemberLevel.OrderBy(c => c.LogPriceCount).ToList();

                cbMemberLevel.ItemsSource = _levels;
                cbMemberLevel.DisplayMemberPath = "Name";
                cbMemberLevel.SelectedValuePath = "Id";

                cbMemberLevel.SelectedIndex = 0;
            }
        }

        private void MemberChart_DataClick(object sender, ChartPoint chartPoint)
        {
            MessageBox.Show(chartPoint.SeriesView.Title);
        }

        private void NormalChart_DataClick(object sender, ChartPoint chartPoint)
        {
            MessageBox.Show(chartPoint.SeriesView.Title);
        }

        private void btnAddNormal_Click(object sender, RoutedEventArgs e)
        {
            decimal m = 0;
            decimal j = 0;

            if (txtNormalM.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入满值金额", "空值提醒");
                txtNormalM.Focus();
                return;
            }
            if (!decimal.TryParse(txtNormalM.Text, out m))
            {
                MessageBoxX.Show("满值金额格式不正确", "格式错误");
                txtNormalM.Focus();
                txtNormalM.SelectAll();
                return;
            }
            if (txtNormalJ.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入减值金额", "空值提醒");
                txtNormalJ.Focus();
                return;
            }
            if (!decimal.TryParse(txtNormalJ.Text, out j))
            {
                MessageBoxX.Show("减值金额格式不正确", "格式错误");
                txtNormalJ.Focus();
                txtNormalJ.SelectAll();
                return;
            }

            if (j > m)
            {
                MessageBoxX.Show("满减逻辑不正确", "逻辑错误");
                txtNormalJ.Focus();
                txtNormalJ.SelectAll();
                return;
            }

            MJActivity activity = new MJActivity();
            activity.CreateTime = DateTime.Now;
            activity.Creator = CurrUser.Id;
            activity.J = j;
            activity.M = m;
            activity.MemberTypeId = 0;
            activity.Type = 0;

            using (DBContext context = new DBContext())
            {
                activity = context.MJActivity.Add(activity);
                context.SaveChanges();
            }
            MessageBoxX.Show("成功", "成功");
            UpdateNormalChart();

            txtNormalM.Clear();
            txtNormalJ.Clear();
        }

        private void btnAddMember_Click(object sender, RoutedEventArgs e)
        {
            decimal m = 0;
            decimal j = 0;
            int memberLevel = cbMemberLevel.SelectedValue.ToString().AsInt();

            if (txtMemberM.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入满值金额", "空值提醒");
                txtMemberM.Focus();
                return;
            }
            if (!decimal.TryParse(txtMemberM.Text, out m))
            {
                MessageBoxX.Show("满值金额格式不正确", "格式错误");
                txtMemberM.Focus();
                txtMemberM.SelectAll();
                return;
            }
            if (txtMemberJ.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入减值金额", "空值提醒");
                txtMemberJ.Focus();
                return;
            }
            if (!decimal.TryParse(txtMemberJ.Text, out j))
            {
                MessageBoxX.Show("减值金额格式不正确", "格式错误");
                txtMemberJ.Focus();
                txtMemberJ.SelectAll();
                return;
            }

            if (j > m)
            {
                MessageBoxX.Show("满减逻辑不正确", "逻辑错误");
                txtMemberJ.Focus();
                txtMemberJ.SelectAll();
                return;
            }

            MJActivity activity = new MJActivity();
            activity.CreateTime = DateTime.Now;
            activity.Creator = CurrUser.Id;
            activity.J = j;
            activity.M = m;
            activity.MemberTypeId = memberLevel;
            activity.Type = 0;

            using (DBContext context = new DBContext())
            {
                activity = context.MJActivity.Add(activity);
                context.SaveChanges();
            }
            MessageBoxX.Show("成功", "成功");
            UpdateMemberChart();

            txtMemberM.Clear();
            txtMemberJ.Clear();
        }

        private void cbMemberLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMemberChart();
        }
    }
}
