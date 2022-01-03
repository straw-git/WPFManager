using Common;
using DBModels.Staffs;
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
using System.Windows.Shapes;

namespace HRPlugin
{
    /// <summary>
    /// EditStaffWage.xaml 的交互逻辑
    /// </summary>
    public partial class EditStaffWage : Window
    {
        string staffId = "";
        Staff staff = new Staff();

        class UIModel
        {
            public string Header { get; set; }
            public string Content { get; set; }
        }

        List<UIModel> list = new List<UIModel>();

        public EditStaffWage(string _staffId)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            staffId = _staffId;

            ClearUI();
        }

        private void cbEnableEndTime_Checked(object sender, RoutedEventArgs e)
        {
            dtEnd.IsEnabled = true;
        }

        private void cbEnableEndTime_Unchecked(object sender, RoutedEventArgs e)
        {
            dtEnd.IsEnabled = false;
        }

        private void UpdateTimeLine()
        {
            tlWage.IsEnabled = false;
            tlWage.Items.Clear();
            list.Clear();
            List<StaffSalary> models = new List<StaffSalary>();

            using (DBContext context = new DBContext())
            {
                if (context.StaffSalary.Any(c => c.StaffId == staffId))
                    models = context.StaffSalary.Where(c => c.StaffId == staffId).OrderBy(c => c.CreateTime).ToList();
            }

            WAGELIST.Clear();

            SetWage(staff.CreateTime, DateTime.MaxValue, 0);

            foreach (var item in models)
            {
                DateTime end = item.End;
                if (!item.IsEnd) end = DateTime.MaxValue;
                SetWage(item.Start, end, item.Price);
            }

            DateTime maxDate = DateTime.MaxValue.MinDate();
            if (WAGELIST.Any(c => c.Start == maxDate)) 
                WAGELIST.Remove(WAGELIST.First(c => c.Start == maxDate));

            WAGELIST = WAGELIST.OrderBy(c => c.Start).ToList();

            foreach (var item in WAGELIST)
            {
                tlWage.Items.Add(new TimelineItem()
                {
                    Content = item.Price,
                    Header = item.Start.ToString("yy年MM月dd日"),
                    Height = 60,
                });
            }

            //当前工资额
            lblWageNow.Content = 0;
            if (WAGELIST.Any(c => c.Start < DateTime.Now)) 
            {
                lblWageNow.Content = WAGELIST.Last(c => c.Start < DateTime.Now).Price;
            }

            tlWage.IsEnabled = true;
        }

        class WageModel
        {
            public DateTime Start { get; set; }
            public decimal Price { get; set; }
        }

        List<WageModel> WAGELIST = new List<WageModel>();

        private void SetWage(DateTime _start, DateTime _end, decimal _price)
        {
            _start = _start.MinDate();
            _end = _end.MinDate();

            if (WAGELIST.Count == 0)
            {
                WAGELIST.Add(new WageModel() { Start = _start, Price = _price });
                WAGELIST.Add(new WageModel() { Start = _end, Price = _price });
                return;
            }

            WageModel start = new WageModel();
            WageModel end = new WageModel();

            start.Start = _start;
            start.Price = _price;

            if (WAGELIST.Any(c => c.Start == _start))
            {
                WAGELIST.Remove(WAGELIST.First(c => c.Start == _start));
            }

            end.Start = _end;
            end.Price = WAGELIST.Any(c => c.Start < _end) ? WAGELIST.First(c => c.Start < _end).Price : _price;

            if (WAGELIST.Any(c => c.Start == _end))
            {
                WAGELIST.Remove(WAGELIST.First(c => c.Start == _end));
            }

            WAGELIST.RemoveAll(c => c.Start >= _start && c.Start <= _end);

            WAGELIST.Add(start);
            WAGELIST.Add(end);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal wagePrice = 0;

            if (txtWagePrice.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入基础工资", "空值提醒");
                txtWagePrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtWagePrice.Text, out wagePrice))
            {
                MessageBoxX.Show("基础工资格式不正确", "格式错误");
                txtWagePrice.Focus();
                txtWagePrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                StaffSalary model = new StaffSalary();
                model.CreateTime = DateTime.Now;
                model.Creator = TempBasePageData.message.CurrUser.Id;
                model.IsEnd = (bool)cbEnableEndTime.IsChecked;
                model.End = dtEnd.SelectedDateTime.MaxDate();
                model.Price = wagePrice;
                model.Register = staff.Register;
                model.Remark = txtRemark.Text;
                model.SatffName = staff.Name;
                model.StaffId = staffId;
                model.StaffQuickCode = staff.QuickCode;
                model.Start = dtStart.SelectedDateTime.MinDate();
                context.StaffSalary.Add(model);
                context.SaveChanges();
            }

            MessageBoxX.Show("成功", "成功");
            ClearUI();
        }

        private void ClearUI()
        {
            dtStart.SelectedDateTime = DateTime.Now;
            dtEnd.SelectedDateTime = DateTime.Now;
            cbEnableEndTime.IsChecked = false;
            txtWagePrice.Clear();
            txtRemark.Clear();

            using (DBContext context = new DBContext())
            {
                staff = context.Staff.First(c => c.Id == staffId);
                lblName.Content = staff.Name;
                lblJobpostName.Content = context.SysDic.First(c => c.Id == staff.JobPostId).Name;
            }

            UpdateTimeLine();
        }

        private void dtStart_SelectedDateTimeChanged(object sender, Panuon.UI.Silver.Core.SelectedDateTimeChangedEventArgs e)
        {
            dtEnd.SelectedDateTime = dtStart.SelectedDateTime;
            dtEnd.MinDate = dtStart.SelectedDateTime;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtStart.MinDate = staff.Register;
            dtStart_SelectedDateTimeChanged(null, null);
        }
    }
}
