using DBModels.Staffs;
using Panuon.UI.Silver;
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
using Common.Windows;

namespace HRPlugin.Pages.HR
{
    /// <summary>
    /// WageRP.xaml 的交互逻辑
    /// </summary>
    public partial class WageRP : BasePage
    {
        public WageRP()
        {
            InitializeComponent();
            this.Order = 2;
        }

        #region Models

        class UIModel
        {
            public int Id { get; set; }
            public string JobpostName { get; set; }
            public string Name { get; set; }
            public decimal RewardPrice { get; set; }
            public decimal FinePrice { get; set; }
            public string Days { get; set; }
            public string Remark { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();


        protected override void OnPageLoaded()
        {
            dtDoTime.MinDate = DateTime.Now;
            list.ItemsSource = Data;
        }

        #region UI Method

        private void btnSelectStaff_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            SelectedStaff a = new SelectedStaff(1);
            a.ShowDialog();
            MaskVisible(false);
            if (a.Succeed)
            {
                using (DBContext context = new DBContext())
                {
                    string id = a.Ids[0];
                    var staff = context.Staff.First(c => c.Id == id);
                    btnSelectStaff.Content = staff.Name;
                    btnSelectStaff.Tag = staff.Id;

                    string monthCode = $"{DateTime.Now.ToString("yyMM")}";
                    Data.Clear();
                    var rp = context.StaffSalaryOther.Where(c => c.StaffId == staff.Id && c.MonthCode == monthCode).ToList();
                    bNoData.Visibility = rp.Count() > 0 ? Visibility.Collapsed : Visibility.Visible;
                    foreach (var item in rp)
                    {
                        Data.Add(new UIModel()
                        {
                            FinePrice = item.Type == 0 ? item.Price : 0,
                            Id = item.Id,
                            JobpostName = context.SysDic.First(c => c.Id == staff.JobPostId).Name,
                            Name = staff.Name,
                            Remark = item.Remark,
                            Days=item.DoTime.ToString("dd号"),
                            RewardPrice = item.Type == 0 ? 0 : item.Price
                        });
                    }
                }
                txtPrice.IsEnabled = true;
                txtRemark.IsEnabled = true;
                btnAddP.IsEnabled = true;
                btnAddR.IsEnabled = true;
                dtDoTime.IsEnabled = true;
            }
            else
            {
                btnSelectStaff.Content = "选择员工";
                btnSelectStaff.Tag = "";
                btnClear_Click(null, null);
            }
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            btnClear_Click(null, null);
            using (DBContext context = new DBContext())
            {
                string monthCode = $"{DateTime.Now.ToString("yyMM")}";
                var rp = context.StaffSalaryOther.Where(c => c.MonthCode == monthCode).ToList() ;
                bNoData.Visibility = rp.Count() > 0 ? Visibility.Collapsed : Visibility.Visible;
                foreach (var item in rp)
                {
                    var staff = context.Staff.First(c => c.Id == item.StaffId);
                    Data.Add(new UIModel()
                    {
                        FinePrice = item.Type == 0 ? item.Price : 0,
                        Id = item.Id,
                        JobpostName = context.SysDic.First(c => c.Id == staff.JobPostId).Name,
                        Name = staff.Name,
                        Remark = item.Remark,
                        RewardPrice = item.Type == 0 ? 0 : item.Price
                    });
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtPrice.Clear();
            txtRemark.Clear();
            txtPrice.IsEnabled = false;
            txtRemark.IsEnabled = false;
            btnAddP.IsEnabled = false;
            btnAddR.IsEnabled = false;
            dtDoTime.IsEnabled = false;
            Data.Clear();

            btnSelectStaff.Content = "选择员工";
            btnSelectStaff.Tag = "";
            bNoData.Visibility =Visibility.Visible;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            using (DBContext context = new DBContext())
            {
                context.StaffSalaryOther.Remove(context.StaffSalaryOther.First(c => c.Id == id));
                Data.Remove(Data.First(c => c.Id == id));
            }
        }

        private void btnAddR_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            if (txtPrice.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入奖罚金额", "空值提醒");
                txtPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("奖罚金额格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                StaffSalaryOther model = new StaffSalaryOther();
                model.CreateTime = DateTime.Now;
                model.Creator = UserGlobal.CurrUser.Id;
                model.DoTime = dtDoTime.SelectedDateTime;
                model.MonthCode = $"{dtDoTime.SelectedDateTime.ToString("yyMM")}";
                model.Price = price;
                model.Remark = txtRemark.Text;
                model.StaffId = btnSelectStaff.Tag.ToString();
                model.Type = 1;

                model = context.StaffSalaryOther.Add(model);

                context.SaveChanges();

                MessageBoxX.Show("成功", "成功");
                string monthCode = $"{DateTime.Now.ToString("yyMM")}";
                if (model.MonthCode == monthCode)
                {
                    var staff = context.Staff.First(c => c.Id == btnSelectStaff.Tag.ToString());
                    Data.Insert(0, new UIModel()
                    {
                        FinePrice = 0,
                        Id = model.Id,
                        JobpostName = context.SysDic.First(c => c.Id == staff.JobPostId).Name,
                        Name = staff.Name,
                        Remark = txtRemark.Text,
                        RewardPrice = price
                    });
                }
            }
            txtPrice.Clear();
            txtRemark.Clear();
        }

        private void btnAddP_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            if (txtPrice.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入奖罚金额", "空值提醒");
                txtPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("奖罚金额格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                StaffSalaryOther model = new StaffSalaryOther();
                model.CreateTime = DateTime.Now;
                model.Creator = UserGlobal.CurrUser.Id;
                model.DoTime = dtDoTime.SelectedDateTime;
                model.MonthCode = $"{dtDoTime.SelectedDateTime.ToString("yyMM")}";
                model.Price = price;
                model.Remark = txtRemark.Text;
                model.StaffId = btnSelectStaff.Tag.ToString();
                model.Type = 0;

                model = context.StaffSalaryOther.Add(model);

                context.SaveChanges();

                MessageBoxX.Show("成功", "成功");
                string monthCode = $"{DateTime.Now.ToString("yyMM")}";
                if (model.MonthCode == monthCode)
                {
                    var staff = context.Staff.First(c => c.Id == btnSelectStaff.Tag.ToString());
                    Data.Insert(0, new UIModel()
                    {
                        FinePrice = price,
                        Id = model.Id,
                        JobpostName = context.SysDic.First(c => c.Id == staff.JobPostId).Name,
                        Name = staff.Name,
                        Remark = txtRemark.Text,
                        RewardPrice = 0
                    });
                }
            }
            txtPrice.Clear();
            txtRemark.Clear();
        }
        #endregion
    }
}
