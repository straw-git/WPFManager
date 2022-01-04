using Common;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HRPlugin
{
    /// <summary>
    /// EditInsurance.xaml 的交互逻辑
    /// </summary>
    public partial class EditInsurance : Window
    {
        public string staffId = "";

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string Remark { get; set; }
            public string StartTime { get; set; }

            private string endTime = "";
            public string EndTime
            {
                get => endTime;
                set
                {
                    endTime = value;
                    NotifyPropertyChanged("EndTime");
                }
            }
            public string SendType { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion 

        enum SYType { Add, Edit }

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        SYType syType = SYType.Add;
        int editId = 0;

        public EditInsurance(string _staffId)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            staffId = _staffId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            syList.ItemsSource = Data;
            LoadSB();
            LoadSY();
        }

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSBSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal companyPrice = 0;
            decimal staffPrice = 0;

            #region Empty or Error

            if (string.IsNullOrEmpty(txtSBCompanyPrice.Text))
            {
                MessageBoxX.Show("单位扣费不能为空", "空值提醒");
                txtSBCompanyPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtSBCompanyPrice.Text, out companyPrice))
            {
                MessageBoxX.Show("单位扣费格式不正确", "格式错误");
                txtSBCompanyPrice.Focus();
                txtSBCompanyPrice.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtSBStaffPrice.Text))
            {
                MessageBoxX.Show("个人扣费不能为空", "空值提醒");
                txtSBStaffPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtSBStaffPrice.Text, out staffPrice))
            {
                MessageBoxX.Show("个人扣费格式不正确", "格式错误");
                txtSBStaffPrice.Focus();
                txtSBStaffPrice.SelectAll();
                return;
            }

            #endregion 

            using (DBContext context = new DBContext())
            {
                if (context.StaffInsurance.Any(c => c.StaffId == staffId && c.Type == 0))
                {
                    //存在社保 编辑
                    var _insurance = context.StaffInsurance.Single(c => c.StaffId == staffId && c.Type == 0);
                    _insurance.CompanyName = txtSBCompanyName.Text;
                    _insurance.CompanyPrice = companyPrice;
                    _insurance.Remark = txtSBRemark.Text;
                    _insurance.StaffPrice = staffPrice;
                    _insurance.Start = dtSBStart.SelectedDateTime;
                    _insurance.Write = dtSBWrite.SelectedDateTime;
                }
                else
                {
                    DateTime currTime = DateTime.Now;
                    //不存在社保  添加
                    StaffInsurance _insurance = new StaffInsurance();
                    _insurance.CompanyName = txtSBCompanyName.Text;
                    _insurance.CompanyPrice = companyPrice;
                    _insurance.CreateTime = currTime;
                    _insurance.Creator = UserGlobal.CurrUser.Id;
                    _insurance.End = currTime;
                    _insurance.Monthly = true;
                    _insurance.Remark = txtSBRemark.Text;
                    _insurance.StaffId = staffId;
                    _insurance.StaffPrice = staffPrice;
                    _insurance.Start = dtSBStart.SelectedDateTime;
                    _insurance.Stop = false;
                    _insurance.StopUser = 0;
                    _insurance.Type = 0;
                    _insurance.Write = dtSBWrite.SelectedDateTime;

                    context.StaffInsurance.Add(_insurance);
                }

                int _count = context.SaveChanges();
                if (_count == 0)
                {
                    MessageBoxX.Show("失败", "失败");
                    return;
                }
            }

            MessageBoxX.Show("成功", "成功");
        }

        private void syList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Data.Count == 0) return;
            if (syList.SelectedItem == null) return;

            UIModel selectModel = syList.SelectedItem as UIModel;

            using (DBContext context = new DBContext())
            {
                var _insurance = context.StaffInsurance.First(c => c.Id == selectModel.Id);

                dtSYStart.SelectedDateTime = _insurance.Start;
                dtSYWrite.SelectedDateTime = _insurance.Write;
                cbEnableMonthly.IsChecked = _insurance.Monthly;
                txtSYCompanyPrice.Text = _insurance.CompanyPrice.ToString();
                txtSYStaffPrice.Text = _insurance.StaffPrice.ToString();
                txtSYCompanyName.Text = _insurance.CompanyName;
                txtSYRemark.Text = _insurance.Remark;
            }

            syType = SYType.Edit;
            editId = selectModel.Id;

            //切换TabIndex
            Application.Current.Dispatcher.BeginInvoke((Action)delegate { tabSY.SelectedIndex = 1; }, DispatcherPriority.Render, null);
        }

        private void cbSYUseEnd_Checked(object sender, RoutedEventArgs e)
        {
            dtSYEnd.IsEnabled = true;
        }

        private void cbSYUseEnd_Unchecked(object sender, RoutedEventArgs e)
        {
            dtSYEnd.IsEnabled = false;
        }

        private void btnSYSubmit_Click(object sender, RoutedEventArgs e)
        {
            //添加SY保险
            decimal companyPrice = 0;
            decimal staffPrice = 0;

            #region Empty or Error

            if (string.IsNullOrEmpty(txtSYCompanyPrice.Text))
            {
                MessageBoxX.Show("单位扣费不能为空", "空值提醒");
                txtSYCompanyPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtSYCompanyPrice.Text, out companyPrice))
            {
                MessageBoxX.Show("单位扣费格式不正确", "格式错误");
                txtSYCompanyPrice.Focus();
                txtSYCompanyPrice.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtSYStaffPrice.Text))
            {
                MessageBoxX.Show("个人扣费不能为空", "空值提醒");
                txtSYStaffPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtSYStaffPrice.Text, out staffPrice))
            {
                MessageBoxX.Show("个人扣费格式不正确", "格式错误");
                txtSYStaffPrice.Focus();
                txtSYStaffPrice.SelectAll();
                return;
            }

            #endregion 

            if (syType == SYType.Add)
            {
                StaffInsurance model = new StaffInsurance();
                model.CompanyName = txtSYCompanyName.Text;
                model.CompanyPrice = companyPrice;
                model.CreateTime = DateTime.Now;
                model.Creator = UserGlobal.CurrUser.Id;
                model.End = dtSYEnd.SelectedDateTime;
                model.Monthly = (bool)cbEnableMonthly.IsChecked;
                model.Remark = txtSYRemark.Text;
                model.StaffPrice = staffPrice;
                model.Start = dtSYStart.SelectedDateTime;
                model.Stop = false;
                model.StopUser = 0;
                model.StaffId = staffId;
                model.Type = 1;
                model.Write = dtSYWrite.SelectedDateTime;

                using (DBContext context = new DBContext())
                {
                    model = context.StaffInsurance.Add(model);
                    context.SaveChanges();
                }
            }
            else if (syType == SYType.Edit)
            {
                using (DBContext context = new DBContext())
                {
                    StaffInsurance model = context.StaffInsurance.Single(c => c.Id == editId);
                    model.CompanyName = txtSYCompanyName.Text;
                    model.CompanyPrice = companyPrice;
                    model.End = dtSYEnd.SelectedDateTime;
                    model.Monthly = (bool)cbEnableMonthly.IsChecked;
                    model.Remark = txtSYRemark.Text;
                    model.StaffPrice = staffPrice;
                    model.Start = dtSYStart.SelectedDateTime;
                    model.Type = 1;
                    model.Write = dtSYWrite.SelectedDateTime;

                    context.SaveChanges();
                }
            }

            syType = SYType.Add;
            editId = 0;

            ClearSY();
            tabSY.SelectedIndex = 0;
            LoadSY();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();

            using (DBContext context = new DBContext())
            {
                var _insurance = context.StaffInsurance.Single(c => c.Id == id);
                _insurance.Stop = true;
                _insurance.StopUser = UserGlobal.CurrUser.Id;
                _insurance.End = DateTime.Now;

                var _model = Data.Single(c => c.Id == id);
                _model.EndTime = _insurance.End.ToString("yyyy年MM月dd日");

                context.SaveChanges();
            }
        }

        private void dtSYStart_SelectedDateTimeChanged(object sender, Panuon.UI.Silver.Core.SelectedDateTimeChangedEventArgs e)
        {
            dtSYEnd.MinDate = dtSYStart.SelectedDateTime;
        }


        private void btnSBStop_Click(object sender, RoutedEventArgs e)
        {
            using (DBContext context = new DBContext())
            {
                var insurance = context.StaffInsurance.Single(c => c.StaffId == staffId && c.Type == 0 && !c.Stop);

                insurance.Stop = true;
                insurance.StopUser = UserGlobal.CurrUser.Id;
                insurance.End = DateTime.Now;
                context.SaveChanges();
            }

            txtSBCompanyName.Clear();
            txtSBCompanyPrice.Clear();
            txtSBRemark.Clear();
            txtSBStaffPrice.Clear();

            MessageBoxX.Show("成功", "成功");
        }

        #endregion

        #region Private Method

        private void LoadSB()
        {
            using (DBContext context = new DBContext())
            {
                if (context.StaffInsurance.Any(c => c.StaffId == staffId && c.Type == 0))
                {
                    var _insurance = context.StaffInsurance.First(c => c.StaffId == staffId && c.Type == 0);
                    dtSBStart.SelectedDateTime = _insurance.Start;
                    dtSBWrite.SelectedDateTime = _insurance.Write;
                    txtSBCompanyPrice.Text = _insurance.CompanyPrice.ToString();
                    txtSBStaffPrice.Text = _insurance.StaffPrice.ToString();
                    txtSBCompanyName.Text = _insurance.CompanyName;
                    txtSBRemark.Text = _insurance.Remark;

                    btnSBStop.Visibility = Visibility.Visible;
                }
                else
                {
                    btnSBStop.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadSY()
        {
            Data.Clear();
            using (DBContext context = new DBContext())
            {
                if (context.StaffInsurance.Any(c => c.StaffId == staffId && c.Type == 1))
                {
                    var _insurances = context.StaffInsurance.Where(c => c.StaffId == staffId && c.Type == 1).ToList();

                    foreach (var item in _insurances)
                    {
                        UIModel model = new UIModel();
                        model.CompanyName = item.CompanyName;
                        model.Id = item.Id;
                        model.Remark = item.Remark;
                        model.SendType = item.Monthly ? "按月" : "无限期";
                        model.StartTime = item.Start.ToString("yyyy年MM月dd日");
                        Data.Add(model);
                    }
                }
            }
        }

        private void ClearSY()
        {
            dtSYStart.SelectedDateTime = DateTime.Now;
            dtSYWrite.SelectedDateTime = DateTime.Now;
            cbEnableMonthly.IsChecked = false;
            txtSYCompanyPrice.Clear();
            txtSYStaffPrice.Clear();
            txtSYCompanyName.Clear();
            txtSYRemark.Clear();
        }

        #endregion

    }
}
