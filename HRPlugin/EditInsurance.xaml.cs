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

        class InsuranceUIModel : INotifyPropertyChanged
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

        enum InsuranceSYType { Add, Edit }

        ObservableCollection<InsuranceUIModel> InsuranceData = new ObservableCollection<InsuranceUIModel>();
        InsuranceSYType syType = InsuranceSYType.Add;
        int insuranceEditId = 0;

        public EditInsurance(string _staffId)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            staffId = _staffId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            syInsuranceList.ItemsSource = InsuranceData;
            LoadSB();
            LoadSY();
        }

        #region UI Method

        private void btnInsuranceSBSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal companyPrice = 0;
            decimal staffPrice = 0;

            #region Empty or Error

            if (string.IsNullOrEmpty(txtInsuranceSBCompanyPrice.Text))
            {
                MessageBoxX.Show("单位扣费不能为空", "空值提醒");
                txtInsuranceSBCompanyPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtInsuranceSBCompanyPrice.Text, out companyPrice))
            {
                MessageBoxX.Show("单位扣费格式不正确", "格式错误");
                txtInsuranceSBCompanyPrice.Focus();
                txtInsuranceSBCompanyPrice.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtInsuranceSBStaffPrice.Text))
            {
                MessageBoxX.Show("个人扣费不能为空", "空值提醒");
                txtInsuranceSBStaffPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtInsuranceSBStaffPrice.Text, out staffPrice))
            {
                MessageBoxX.Show("个人扣费格式不正确", "格式错误");
                txtInsuranceSBStaffPrice.Focus();
                txtInsuranceSBStaffPrice.SelectAll();
                return;
            }

            #endregion 

            using (DBContext context = new DBContext())
            {
                if (context.StaffInsurance.Any(c => c.StaffId == staffId && c.Type == 0))
                {
                    //存在社保 编辑
                    var _insurance = context.StaffInsurance.Single(c => c.StaffId == staffId && c.Type == 0);
                    _insurance.CompanyName = txtInsuranceSBCompanyName.Text;
                    _insurance.CompanyPrice = companyPrice;
                    _insurance.Remark = txtInsuranceSBRemark.Text;
                    _insurance.StaffPrice = staffPrice;
                    _insurance.Start = dtInsuranceSBStart.SelectedDateTime;
                    _insurance.Write = dtInsuranceSBWrite.SelectedDateTime;
                }
                else
                {
                    DateTime currTime = DateTime.Now;
                    //不存在社保  添加
                    StaffInsurance _insurance = new StaffInsurance();
                    _insurance.CompanyName = txtInsuranceSBCompanyName.Text;
                    _insurance.CompanyPrice = companyPrice;
                    _insurance.CreateTime = currTime;
                    _insurance.Creator = UserGlobal.CurrUser.Id;
                    _insurance.End = currTime;
                    _insurance.Monthly = true;
                    _insurance.Remark = txtInsuranceSBRemark.Text;
                    _insurance.StaffId = staffId;
                    _insurance.StaffPrice = staffPrice;
                    _insurance.Start = dtInsuranceSBStart.SelectedDateTime;
                    _insurance.Stop = false;
                    _insurance.StopUser = 0;
                    _insurance.Type = 0;
                    _insurance.Write = dtInsuranceSBWrite.SelectedDateTime;

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

        private void syInsuranceList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (InsuranceData.Count == 0) return;
            if (syInsuranceList.SelectedItem == null) return;

            InsuranceUIModel selectModel = syInsuranceList.SelectedItem as InsuranceUIModel;

            using (DBContext context = new DBContext())
            {
                var _insurance = context.StaffInsurance.First(c => c.Id == selectModel.Id);

                dtInsuranceSYStart.SelectedDateTime = _insurance.Start;
                dtInsuranceSYWrite.SelectedDateTime = _insurance.Write;
                cbInsuranceEnableMonthly.IsChecked = _insurance.Monthly;
                txtInsuranceSYCompanyPrice.Text = _insurance.CompanyPrice.ToString();
                txtInsuranceSYStaffPrice.Text = _insurance.StaffPrice.ToString();
                txtInsuranceSYCompanyName.Text = _insurance.CompanyName;
                txtInsuranceSYRemark.Text = _insurance.Remark;
            }

            syType = InsuranceSYType.Edit;
            insuranceEditId = selectModel.Id;

            //切换TabIndex
            Application.Current.Dispatcher.BeginInvoke((Action)delegate { tabInsuranceSY.SelectedIndex = 1; }, DispatcherPriority.Render, null);
        }

        private void cbInsuranceSYUseEnd_Checked(object sender, RoutedEventArgs e)
        {
            dtInsuranceSYEnd.IsEnabled = true;
        }

        private void cbSYUseEnd_Unchecked(object sender, RoutedEventArgs e)
        {
            dtInsuranceSYEnd.IsEnabled = false;
        }

        private void btnInsuranceSYSubmit_Click(object sender, RoutedEventArgs e)
        {
            //添加SY保险
            decimal companyPrice = 0;
            decimal staffPrice = 0;

            #region Empty or Error

            if (string.IsNullOrEmpty(txtInsuranceSYCompanyPrice.Text))
            {
                MessageBoxX.Show("单位扣费不能为空", "空值提醒");
                txtInsuranceSYCompanyPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtInsuranceSYCompanyPrice.Text, out companyPrice))
            {
                MessageBoxX.Show("单位扣费格式不正确", "格式错误");
                txtInsuranceSYCompanyPrice.Focus();
                txtInsuranceSYCompanyPrice.SelectAll();
                return;
            }
            if (string.IsNullOrEmpty(txtInsuranceSYStaffPrice.Text))
            {
                MessageBoxX.Show("个人扣费不能为空", "空值提醒");
                txtInsuranceSYStaffPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtInsuranceSYStaffPrice.Text, out staffPrice))
            {
                MessageBoxX.Show("个人扣费格式不正确", "格式错误");
                txtInsuranceSYStaffPrice.Focus();
                txtInsuranceSYStaffPrice.SelectAll();
                return;
            }

            #endregion 

            if (syType == InsuranceSYType.Add)
            {
                StaffInsurance model = new StaffInsurance();
                model.CompanyName = txtInsuranceSYCompanyName.Text;
                model.CompanyPrice = companyPrice;
                model.CreateTime = DateTime.Now;
                model.Creator = UserGlobal.CurrUser.Id;
                model.End = dtInsuranceSYEnd.SelectedDateTime;
                model.Monthly = (bool)cbInsuranceEnableMonthly.IsChecked;
                model.Remark = txtInsuranceSYRemark.Text;
                model.StaffPrice = staffPrice;
                model.Start = dtInsuranceSYStart.SelectedDateTime;
                model.Stop = false;
                model.StopUser = 0;
                model.StaffId = staffId;
                model.Type = 1;
                model.Write = dtInsuranceSYWrite.SelectedDateTime;

                using (DBContext context = new DBContext())
                {
                    model = context.StaffInsurance.Add(model);
                    context.SaveChanges();
                }
            }
            else if (syType == InsuranceSYType.Edit)
            {
                using (DBContext context = new DBContext())
                {
                    StaffInsurance model = context.StaffInsurance.Single(c => c.Id == insuranceEditId);
                    model.CompanyName = txtInsuranceSYCompanyName.Text;
                    model.CompanyPrice = companyPrice;
                    model.End = dtInsuranceSYEnd.SelectedDateTime;
                    model.Monthly = (bool)cbInsuranceEnableMonthly.IsChecked;
                    model.Remark = txtInsuranceSYRemark.Text;
                    model.StaffPrice = staffPrice;
                    model.Start = dtInsuranceSYStart.SelectedDateTime;
                    model.Type = 1;
                    model.Write = dtInsuranceSYWrite.SelectedDateTime;

                    context.SaveChanges();
                }
            }

            syType = InsuranceSYType.Add;
            insuranceEditId = 0;

            ClearSY();
            tabInsuranceSY.SelectedIndex = 0;
            LoadSY();
        }

        private void btnInsuranceStop_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();

            using (DBContext context = new DBContext())
            {
                var _insurance = context.StaffInsurance.Single(c => c.Id == id);
                _insurance.Stop = true;
                _insurance.StopUser = UserGlobal.CurrUser.Id;
                _insurance.End = DateTime.Now;

                var _model = InsuranceData.Single(c => c.Id == id);
                _model.EndTime = _insurance.End.ToString("yyyy年MM月dd日");

                context.SaveChanges();
            }
        }

        private void dtInsuranceSYStart_SelectedDateTimeChanged(object sender, Panuon.UI.Silver.Core.SelectedDateTimeChangedEventArgs e)
        {
            dtInsuranceSYEnd.MinDate = dtInsuranceSYStart.SelectedDateTime;
        }


        private void btnInsuranceSBStop_Click(object sender, RoutedEventArgs e)
        {
            using (DBContext context = new DBContext())
            {
                var insurance = context.StaffInsurance.Single(c => c.StaffId == staffId && c.Type == 0 && !c.Stop);

                insurance.Stop = true;
                insurance.StopUser = UserGlobal.CurrUser.Id;
                insurance.End = DateTime.Now;
                context.SaveChanges();
            }

            txtInsuranceSBCompanyName.Clear();
            txtInsuranceSBCompanyPrice.Clear();
            txtInsuranceSBRemark.Clear();
            txtInsuranceSBStaffPrice.Clear();

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
                    dtInsuranceSBStart.SelectedDateTime = _insurance.Start;
                    dtInsuranceSBWrite.SelectedDateTime = _insurance.Write;
                    txtInsuranceSBCompanyPrice.Text = _insurance.CompanyPrice.ToString();
                    txtInsuranceSBStaffPrice.Text = _insurance.StaffPrice.ToString();
                    txtInsuranceSBCompanyName.Text = _insurance.CompanyName;
                    txtInsuranceSBRemark.Text = _insurance.Remark;

                    btnInsuranceSBStop.Visibility = Visibility.Visible;
                }
                else
                {
                    btnInsuranceSBStop.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadSY()
        {
            InsuranceData.Clear();
            using (DBContext context = new DBContext())
            {
                if (context.StaffInsurance.Any(c => c.StaffId == staffId && c.Type == 1))
                {
                    var _insurances = context.StaffInsurance.Where(c => c.StaffId == staffId && c.Type == 1).ToList();

                    foreach (var item in _insurances)
                    {
                        InsuranceUIModel model = new InsuranceUIModel();
                        model.CompanyName = item.CompanyName;
                        model.Id = item.Id;
                        model.Remark = item.Remark;
                        model.SendType = item.Monthly ? "按月" : "无限期";
                        model.StartTime = item.Start.ToString("yyyy年MM月dd日");
                        InsuranceData.Add(model);
                    }
                }
            }
        }

        private void ClearSY()
        {
            dtInsuranceSYStart.SelectedDateTime = DateTime.Now;
            dtInsuranceSYWrite.SelectedDateTime = DateTime.Now;
            cbInsuranceEnableMonthly.IsChecked = false;
            txtInsuranceSYCompanyPrice.Clear();
            txtInsuranceSYStaffPrice.Clear();
            txtInsuranceSYCompanyName.Clear();
            txtInsuranceSYRemark.Clear();
        }

        #endregion

    }
}
