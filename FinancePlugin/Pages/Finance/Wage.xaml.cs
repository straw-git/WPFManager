using DBModels.Staffs;
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

namespace FinancePlugin.Pages.Finance
{
    /// <summary>
    /// StaffFinance.xaml 的交互逻辑
    /// </summary>
    public partial class Wage : BasePage
    {
        public Wage()
        {
            InitializeComponent();
            this.Order = 1;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string JobPostName { get; set; }
            private decimal basePrice = 0;
            public decimal BasePrice
            {
                get => basePrice; set
                {
                    basePrice = value;
                    NotifyPropertyChanged("BasePrice");
                    NotifyPropertyChanged("Price");
                }
            }
            private decimal insurancePrice = 0;
            public decimal InsurancePrice
            {
                get => insurancePrice; set
                {
                    insurancePrice = value;
                    NotifyPropertyChanged("InsurancePrice");
                    NotifyPropertyChanged("Price");
                }
            }
            private decimal rewardPrice = 0;
            public decimal RewardPrice
            {
                get => rewardPrice; set
                {
                    rewardPrice = value;
                    NotifyPropertyChanged("RewardPrice");
                    NotifyPropertyChanged("Price");
                }
            }
            private decimal finePrice = 0;
            public decimal FinePrice
            {
                get => finePrice; set
                {
                    finePrice = value;
                    NotifyPropertyChanged("FinePrice");
                    NotifyPropertyChanged("Price");
                }
            }
            public decimal Price
            {
                get
                {
                    return basePrice + rewardPrice - insurancePrice - finePrice;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion 

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        ObservableCollection<StaffSalarySettlement> HisData = new ObservableCollection<StaffSalarySettlement>();

        protected override void OnPageLoaded()
        {
            LoadMonth();
            LoadHis();
            list.ItemsSource = Data;
            hisList.ItemsSource = HisData;
        }

        private void LoadHis()
        {
            HisData.Clear();
            using (DBContext context = new DBContext())
            {
                var his = context.StaffSalarySettlement.OrderByDescending(c => c.CreateTime).ToList();
                foreach (var item in his)
                {
                    HisData.Add(item);
                }
            }
        }

        private void LoadMonth()
        {
            cbMonth.Items.Clear();
            string currMonthCode = DateTime.Now.ToString("yyMM");

            using (DBContext context = new DBContext())
            {
                DateTime minTime = DateTime.Now;
                if (context.Staff.Any())
                {
                    minTime = context.Staff.Min(c => c.Register);
                }
                while (true)
                {
                    string code = minTime.ToString("yyMM");
                    if (!context.StaffSalarySettlement.Any(c => c.Id == code))
                        cbMonth.Items.Add(code);
                    if (currMonthCode == code)
                    {
                        break;
                    }
                    minTime = minTime.AddMonths(1);
                }
            }
            if (cbMonth.Items.Count > 0)
            {
                cbMonth.SelectedIndex = 0;
                btnCalculate.IsEnabled = true;
                btnSubmit.IsEnabled = true;
            }
            else
            {
                cbMonth.Items.Add("无待结算");
                cbMonth.SelectedIndex = 0;
                btnCalculate.IsEnabled = false;
                btnSubmit.IsEnabled = false;
                Data.Clear();
                lblCount.Content = 0;
                lblPriceCount.Content = 0;
            }
        }

        private async void LoadGridAsync()
        {
            if (!IsLoaded) return;

            Data.Clear();
            if (cbMonth.SelectedIndex < 0) return;
            string selectedMonth = cbMonth.SelectedItem.ToString();
            if (selectedMonth == "无待结算") return;
            loading.Visibility = Visibility.Visible;

            DateTime currTime = Convert.ToDateTime($"20{selectedMonth.Substring(0, 2)}-{selectedMonth.Substring(2, 2)}-01");

            List<Staff> models = new List<Staff>();
            await Task.Run(() =>
            {
                using (DBContext context = new DBContext())
                {
                    //当时还没有被删除的所有员工
                    models = context.Staff.ToList();
                }
            });
            await Task.Delay(300);
            using (DBContext context = new DBContext())
            {
                foreach (var item in models)
                {
                    //将停职的员工抛出去
                    if (item.IsDel && item.DelTime < currTime) { continue; }
                    Data.Add(new UIModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        JobPostName = context.SysDic.First(c => c.Id == item.JobPostId).Name
                    });
                }
            }
            loading.Visibility = Visibility.Collapsed;

            btnCalculate.IsEnabled = Data.Count > 0;
            lblCount.Content = Data.Count;
            lblPriceCount.Content = 0;
        }

        private async void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            list.IsEnabled = false;
            loading.Visibility = Visibility.Visible;

            string selectedMonth = cbMonth.SelectedItem.ToString();
            DateTime currTime = Convert.ToDateTime($"20{selectedMonth.Substring(0, 2)}-{selectedMonth.Substring(2, 2)}-01");
            int currTimeNumber = $"{selectedMonth}01".AsInt();

            DateTime minTime = currTime.MinDate();
            DateTime maxTime = currTime.AddMonths(1).AddDays(-1).MaxDate();
            int minTimeNumber = minTime.ToString("yyMMdd").AsInt();
            int maxTimeNumber = maxTime.ToString("yyMMdd").AsInt();

            await Task.Run(() =>
            {
                using (DBContext context = new DBContext())
                {
                    foreach (var item in Data)
                    {
                        //员工
                        var staff = context.Staff.First(c => c.Id == item.Id);
                        //工资
                        var wage = context.StaffSalary.Where(c => c.StaffId == staff.Id).OrderBy(c => c.CreateTime).ToList();

                        Dictionary<int, decimal> wageDic = new Dictionary<int, decimal>();
                        for (int i = minTimeNumber; i <= maxTimeNumber; i++)
                        {
                            wageDic.Add(i, 0);
                        }

                        foreach (var w in wage)
                        {
                            if (!w.IsEnd)
                            {
                                w.End = maxTime;
                            }
                            int xMinNumber = w.Start.ToString("yyMMdd").AsInt();
                            int xMaxNumber = w.End.ToString("yyMMdd").AsInt();
                            if (xMaxNumber < minTimeNumber) continue;
                            if (xMinNumber > maxTimeNumber) continue;
                            if (xMinNumber < minTimeNumber) xMinNumber = minTimeNumber;
                            if (xMaxNumber > maxTimeNumber) xMaxNumber = maxTimeNumber;
                            for (int i = xMinNumber; i <= xMaxNumber; i++)
                            {
                                wageDic[i] = w.Price;
                            }
                        }

                        item.BasePrice = Math.Round(wageDic.Values.Sum() / wageDic.Keys.Count, 0);

                        //奖罚
                        decimal fPrice = 0;
                        decimal rPrice = 0;
                        if (context.StaffSalaryOther.Any(c => c.StaffId == staff.Id && c.DoTime >= minTime && c.DoTime <= maxTime))
                        {
                            var other = context.StaffSalaryOther.Where(c => c.StaffId == staff.Id && c.DoTime >= minTime && c.DoTime <= maxTime);
                            foreach (var o in other)
                            {
                                if (o.Type == 0)
                                {
                                    fPrice += o.Price;
                                }
                                if (o.Type == 1)
                                {
                                    rPrice += o.Price;
                                }
                            }
                        }
                        item.RewardPrice = rPrice;
                        item.FinePrice = fPrice;
                        //保险
                        decimal iPrice = 0;
                        if (context.StaffInsurance.Any(c => c.StaffId == staff.Id && c.Start > minTime))
                        {
                            var insurances = context.StaffInsurance.Where(c => c.StaffId == staff.Id && c.Start > minTime);
                            foreach (var i in insurances)
                            {
                                if (i.Stop)
                                {
                                    //当月保险已停
                                    if (i.End < minTime) continue;
                                }
                                if (i.Monthly)
                                {
                                    iPrice += i.StaffPrice;
                                }
                                else
                                {
                                    if (i.Start.Year == minTime.Year && i.Start.Month == minTime.Month)
                                    {
                                        iPrice += i.StaffPrice;
                                    }
                                }
                            }
                        }
                        item.InsurancePrice = iPrice;
                    }
                }
            });
            await Task.Delay(300);

            list.IsEnabled = true;
            loading.Visibility = Visibility.Collapsed;
            lblCount.Content = Data.Count;
            lblPriceCount.Content = Data.Sum(c => c.Price);
        }

        private void cbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGridAsync();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            string staffId = (sender as Button).Tag.ToString();
            Data.Remove(Data.First(c => c.Id == staffId));
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show($"是否确认提交[{cbMonth.SelectedItem}]的结算？", "结算提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (DBContext contex = new DBContext())
                {
                    StaffSalarySettlement model = new StaffSalarySettlement();
                    model.CreateTime = DateTime.Now;
                    model.Creator = UserGlobal.CurrUser.Id;
                    model.Price = Data.Sum(c => c.Price);
                    model.StaffCount = Data.Count;
                    model.CreatorName = UserGlobal.CurrUser.Name;
                    model = contex.StaffSalarySettlement.Add(model);
                    model.Id = cbMonth.SelectedItem.ToString();
                    contex.SaveChanges();

                    HisData.Insert(0, model);

                    foreach (var item in Data)
                    {
                        var staff = contex.Staff.First(c => c.Id == item.Id);

                        StaffSalarySettlementLog itemModel = new StaffSalarySettlementLog();
                        itemModel.AwardPrice = item.RewardPrice;
                        itemModel.BasePrice = item.BasePrice;
                        itemModel.CreateTime = model.CreateTime;
                        itemModel.Creator = model.Creator;
                        itemModel.DeductionPrice = item.FinePrice;
                        itemModel.InsurancePrice = item.InsurancePrice;
                        itemModel.MonthCode = model.Id;
                        itemModel.Price = item.Price;
                        itemModel.SalePrice = 0;
                        itemModel.StaffId = staff.Id;
                        itemModel.StaffName = staff.Name;
                        itemModel.StaffQuickCode = staff.QuickCode;

                        contex.StaffSalarySettlementLog.Add(itemModel);
                    }
                    contex.SaveChanges();
                }

                MessageBoxX.Show("成功", "成功");
                LoadMonth();
            }
        }
    }
}
