﻿using Common;
using Common.Utils;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HRPlugin
{
    /// <summary>
    /// AddStaff.xaml 的交互逻辑
    /// </summary>
    public partial class EditStaff : Window
    {
        string staffId = "";
        bool isEdit = false;

        public bool Succeed = false;
        public Staff StaffModel = new Staff();//员工信息


        class InsuranceUIModel : BaseUIModel
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
        }


        enum InsuranceSYType { Add, Edit }

        ObservableCollection<InsuranceUIModel> InsuranceData = new ObservableCollection<InsuranceUIModel>();
        InsuranceSYType syType = InsuranceSYType.Add;
        int insuranceEditId = 0;


        class WageUIModel
        {
            public string Header { get; set; }
            public string Content { get; set; }
        }

        List<WageUIModel> list = new List<WageUIModel>();

        public EditStaff(string _staffId = "")
        {
            InitializeComponent();
            this.UseCloseAnimation();

            staffId = _staffId;
            InitStaff();

            ClearWageUI();

            InitContract();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtWageStart.MinDate = StaffModel.Register;
            dtWageStart_SelectedDateTimeChanged(null, null);

            syInsuranceList.ItemsSource = InsuranceData;
            LoadSB();
            LoadSY();
        }

        #region 基础信息

        #region InitStaff

        /// <summary>
        /// 初始化员工信息
        /// </summary>
        private void InitStaff()
        {
            if (string.IsNullOrEmpty(staffId))
            {
                isEdit = false;

                DateTime currTime = DateTime.Now;
                staffId = $"S{UserGlobal.CurrUser.Id}{currTime.ToString("yyyyMMddHHmmss")}";
                Title = "注册新员工";

                StaffModel.CreateTime = currTime;
                StaffModel.Creator = UserGlobal.CurrUser.Id;

                new JobPostTreeViewCommon(tvJobPost).Init(false);
            }
            else
            {
                isEdit = true;
                using (DBContext context = new DBContext())
                {
                    StaffModel = context.Staff.FirstOrDefault(c => c.Id == staffId);

                    var jobpost = context.SysDic.First(c => c.Id == StaffModel.JobPostId);

                    #region Model2UI

                    lblJobPost.Content = jobpost.Name;
                    lblJobPost.Tag = jobpost.Id;

                    txtBaseName.Text = StaffModel.Name;
                    cbBaseSex.SelectedIndex = StaffModel.Sex;
                    txtBasePhone.Text = StaffModel.Phone;
                    txtBaseWechat.Text = StaffModel.QQ;
                    txtBaseIDCard.Text = StaffModel.IdCard;
                    dtRegister.SelectedDateTime = StaffModel.Register;
                    txtBaseAddress.Text = StaffModel.Address;
                    txtBaseNowAddress.Text = StaffModel.NowAddress;

                    #endregion
                }
                Title = $"编辑[{StaffModel.Name}]信息";

                new JobPostTreeViewCommon(tvJobPost).Init(false, false, StaffModel.JobPostId);
            }

            StaffModel.Id = staffId;
        }

        #endregion 

        #region UI Method

        private void tvJobPost_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem selectedItem = tvJobPost.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                if (selectedItem.Items.Count > 0) return;
                int selectedJobPostId = 0;
                if (int.TryParse(selectedItem.Tag.ToString(), out selectedJobPostId))
                {
                    lblJobPost.Content = selectedItem.Header;
                    lblJobPost.Tag = selectedJobPostId;
                }
            }
        }

        private void btnCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            txtBaseNowAddress.Text = txtBaseAddress.Text;
        }

        private void btnBaseReadIDCard_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("读卡器连接失败");
        }

        private void btnBaseClearBaseInfo_Click(object sender, RoutedEventArgs e)
        {
            txtBaseName.Clear();
            txtBasePhone.Clear();
            txtBaseAddress.Clear();
            txtBaseIDCard.Clear();
            txtBaseNowAddress.Clear();
            txtBaseWechat.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnBaseSubmit_Click(object sender, RoutedEventArgs e)
        {
            int jobpostId = lblJobPost.Tag.ToString().AsInt();

            #region Empty or Error

            if (jobpostId <= 0)
            {
                MessageBoxX.Show("请选择职务", "空值提醒");
                return;
            }

            if (string.IsNullOrEmpty(txtBaseName.Text))
            {
                MessageBoxX.Show("请输入姓名", "空值提醒");
                txtBaseName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtBasePhone.Text))
            {
                MessageBoxX.Show("请输入手机号", "空值提醒");
                txtBasePhone.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtBaseIDCard.Text))
            {
                MessageBoxX.Show("请输入身份证号", "空值提醒");
                txtBaseIDCard.Focus();
                return;
            }

            #endregion

            #region UI2Model

            StaffModel.Name = txtBaseName.Text;
            StaffModel.Sex = cbBaseSex.SelectedIndex;
            StaffModel.Phone = txtBasePhone.Text;
            StaffModel.QQ = txtBaseWechat.Text;
            StaffModel.IdCard = txtBaseIDCard.Text;
            StaffModel.Register = dtRegister.SelectedDateTime;
            StaffModel.Address = txtBaseAddress.Text;
            StaffModel.NowAddress = txtBaseNowAddress.Text;
            StaffModel.JobPostId = jobpostId;
            StaffModel.QuickCode = $"{txtBaseName.Text.Convert2Py().ToLower()}|{txtBaseName.Text.Convert2Pinyin().ToLower() }";

            #endregion 

            using (DBContext context = new DBContext())
            {
                if (isEdit)
                {
                    if (context.Staff.Any(c => c.Id != StaffModel.Id && (c.IdCard == StaffModel.IdCard || c.Phone == StaffModel.Phone)))
                    {
                        MessageBoxX.Show("请检查手机号、身份证号是否重复", "员工已存在");
                        return;
                    }

                    #region 编辑状态

                    var _staff = context.Staff.Single(c => c.Id == StaffModel.Id);
                    ModelCommon.CopyPropertyToModel(StaffModel, ref _staff);

                    #endregion
                }
                else
                {
                    #region 添加状态

                    if (context.Staff.Any(c => c.IdCard == StaffModel.IdCard || c.Phone == StaffModel.Phone))
                    {
                        MessageBoxX.Show("请检查手机号、身份证号是否重复", "员工已存在");
                        return;
                    }
                    StaffModel.IsDel = false;
                    StaffModel.DelUser = 0;
                    StaffModel.DelTime = DateTime.Now;

                    StaffModel = context.Staff.Add(StaffModel);
                    DateTime currTime = DateTime.Now;

                    #endregion

                }

                context.SaveChanges();
            }
            Succeed = true;
        }

        #endregion

        #endregion

        #region 工资信息

        private void cbWageEnableEndTime_Checked(object sender, RoutedEventArgs e)
        {
            dtWageEnd.IsEnabled = true;
        }

        private void cbWageEnableEndTime_Unchecked(object sender, RoutedEventArgs e)
        {
            dtWageEnd.IsEnabled = false;
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

            SetWage(StaffModel.CreateTime, DateTime.MaxValue, 0);

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

        private void btnWageSubmit_Click(object sender, RoutedEventArgs e)
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
                model.Creator = UserGlobal.CurrUser.Id;
                model.IsEnd = (bool)cbWageEnableEndTime.IsChecked;
                model.End = dtWageEnd.SelectedDateTime.MaxDate();
                model.Price = wagePrice;
                model.Register = StaffModel.Register;
                model.Remark = txtWageRemark.Text;
                model.SatffName = StaffModel.Name;
                model.StaffId = staffId;
                model.StaffQuickCode = StaffModel.QuickCode;
                model.Start = dtWageStart.SelectedDateTime.MinDate();
                context.StaffSalary.Add(model);
                context.SaveChanges();
            }

            MessageBoxX.Show("成功", "成功");
            ClearWageUI();
        }

        private void ClearWageUI()
        {
            dtWageStart.SelectedDateTime = DateTime.Now;
            dtWageEnd.SelectedDateTime = DateTime.Now;
            cbWageEnableEndTime.IsChecked = false;
            txtWagePrice.Clear();
            txtWageRemark.Clear();

            using (DBContext context = new DBContext())
            {
                lblWageName.Content = StaffModel.Name;
                lblWageJobpostName.Content = context.SysDic.First(c => c.Id == StaffModel.JobPostId).Name;
            }

            UpdateTimeLine();
        }

        private void dtWageStart_SelectedDateTimeChanged(object sender, Panuon.UI.Silver.Core.SelectedDateTimeChangedEventArgs e)
        {
            dtWageEnd.SelectedDateTime = dtWageStart.SelectedDateTime;
            dtWageEnd.MinDate = dtWageStart.SelectedDateTime;
        }

        #endregion

        #region 劳动合同

        private void InitContract() 
        {
            using (DBContext context = new DBContext())
            {
                if (context.StaffContract.Any(c => c.StaffId == staffId))
                {
                    //存在 编辑
                    var _contract = context.StaffContract.First(c => c.StaffId == staffId);
                    dtContractStart.SelectedDateTime = _contract.Start;
                    dtContractEnd.SelectedDateTime = _contract.End;
                    int year = _contract.End.Year - _contract.Start.Year;
                    switch (year)
                    {
                        case 1:
                            cbContractLong.SelectedIndex = 0;
                            break;
                        case 2:
                            cbContractLong.SelectedIndex = 1;
                            break;
                        case 3:
                            cbContractLong.SelectedIndex = 2;
                            break;
                        case 5:
                            cbContractLong.SelectedIndex = 3;
                            break;
                        default:
                            break;
                    }
                    dtContractWrite.SelectedDateTime = _contract.Write;
                    txtContractPrice.Text = _contract.Price.ToString();
                    txtContractRemark.Text = _contract.Remark;
                }
            }
            cbContractLong_SelectionChanged(null, null);
        }

        private void btnContractSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            if (string.IsNullOrEmpty(txtContractPrice.Text))
            {
                MessageBoxX.Show("合同金额不能为空", "空值提醒");
                txtContractPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtContractPrice.Text, out price))
            {
                MessageBoxX.Show("合同金额格式不正确", "格式错误");
                txtContractPrice.Focus();
                txtContractPrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                if (context.StaffContract.Any(c => c.StaffId == staffId))
                {
                    //存在 编辑
                    var _contract = context.StaffContract.Single(c => c.StaffId == staffId);
                    _contract.Start = dtContractStart.SelectedDateTime;
                    _contract.End = dtContractEnd.SelectedDateTime;
                    _contract.Write = dtContractWrite.SelectedDateTime;
                    _contract.Price = price;
                    _contract.Remark = txtContractRemark.Text;
                }
                else
                {
                    //不存在 添加
                    StaffContract _contract = new StaffContract();
                    _contract.CreateTime = DateTime.Now;
                    _contract.Creator = UserGlobal.CurrUser.Id;
                    _contract.End = dtContractEnd.SelectedDateTime;
                    _contract.Price = price;
                    _contract.Remark = txtContractRemark.Text;
                    _contract.StaffId = staffId;
                    _contract.Start = dtContractStart.SelectedDateTime;
                    _contract.Write = dtContractWrite.SelectedDateTime;
                    _contract.Stop = false;
                    _contract.StopTime = DateTime.Now;
                    _contract.StopUser = 0;

                    context.StaffContract.Add(_contract);
                }

                context.SaveChanges();
            }

            Succeed = true;
        }

        private void cbContractLong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            switch (cbContractLong.SelectedIndex)
            {
                case 0:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(1);
                    break;
                case 1:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(2);
                    break;
                case 2:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(3);
                    break;
                case 3:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(5);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 保险信息

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

        #endregion

    }
}