using Common;
using Common.Utils;
using Common.Windows;
using CoreDBModels.Models;
using HRDBModels.Models;
using Microsoft.Win32;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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

namespace HRPlugin.Windows
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

        #region UI Models

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

        class WageUIModel
        {
            public string Header { get; set; }
            public string Content { get; set; }
        }

        class AttachmentUIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CreateTime { get; set; }
            public Bitmap Img { get; set; }
        }

        #endregion

        /// <summary>
        /// 商业保险状态
        /// </summary>
        enum InsuranceSYType { Add, Edit }

        ObservableCollection<InsuranceUIModel> InsuranceData = new ObservableCollection<InsuranceUIModel>();//商业保险数据
        InsuranceSYType syType = InsuranceSYType.Add;//商业保险状态
        int insuranceEditId = 0;
        List<WageUIModel> list = new List<WageUIModel>();//工资信息
        ObservableCollection<AttachmentUIModel> attachmentData = new ObservableCollection<AttachmentUIModel>();//附件列表

        public EditStaff(string _staffId = "")
        {
            InitializeComponent();
            this.UseCloseAnimation();

            staffId = _staffId;
            InitStaff();

            ClearWageUI();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            //绑定数据源
            dtWageStart.MinDate = StaffModel.Register;
            dtWageStart_SelectedDateTimeChanged(null, null);
            syInsuranceList.ItemsSource = InsuranceData;
            listAttachment.ItemsSource = attachmentData;

            //
            //初始化
            UpdateContractHistory();
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
                using (CoreDBContext context = new CoreDBContext())
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

            using (CoreDBContext context = new CoreDBContext())
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

            using (HRDBContext context = new HRDBContext())
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

            using (HRDBContext context = new HRDBContext())
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

            using (CoreDBContext context = new CoreDBContext())
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

        /// <summary>
        /// 最小添加时间
        /// </summary>
        private DateTime AddContract_Min_Date = Convert.ToDateTime($"{DateTime.Now.Year}-{DateTime.Now.Month}-01");
        /// <summary>
        /// 最大添加时间
        /// </summary>
        private DateTime AddContract_Max_Date = Convert.ToDateTime($"{DateTime.Now.Year}-{DateTime.Now.Month}-01").AddMonths(1).AddDays(-1);

        /// <summary>
        /// 更新历史合同签订记录
        /// </summary>
        private void UpdateContractHistory()
        {
            tvContractHistory.Items.Clear();//清空列表
            using (HRDBContext context = new HRDBContext())
            {
                bool hasContract = false;//是否有合同
                if (context.StaffContract.Any(c => c.StaffId == staffId))
                {
                    //存在历史
                    var history = context.StaffContract.Where(c => c.StaffId == staffId).OrderByDescending(c => c.CreateTime).ToList();
                    for (int i = 0; i < history.Count; i++)
                    {
                        var contract = history[i];//获取合同

                        var item = new TreeViewItem() { Header = $"{contract.Start.ToString("yy年MM月")}至{contract.End.ToString("yy年MM月")}", IsSelected = false, Tag = contract };
                        tvContractHistory.Items.Add(item);

                        if (contract.Start <= DateTime.Now && contract.End >= DateTime.Now)
                        {
                            //当前时间在范围内
                            item.IsSelected = true;
                            hasContract = true;//有合同
                        }

                        if (i == history.Count - 1)
                        {
                            //最后一条
                            AddContract_Min_Date = contract.End;
                            AddContract_Max_Date = DateTime.MaxValue;
                        }
                    }
                }

                //不存在历史
                tvContractHistory.Items.Insert(0, new TreeViewItem() { Header = "新的合同", IsSelected = !hasContract, Tag = null });
            }
        }

        /// <summary>
        /// 合同导航切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvContractHistory_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvContractHistory.SelectedItem == null) return;
            TreeViewItem selectedItem = tvContractHistory.SelectedItem as TreeViewItem;//获取选中项
            int selectedId = 0;
            if (selectedItem.Tag == null)
            {
                EnableContractGrid(true);
                //新合同 显示保存 不显示上传附件
                btnContractSubmit.Visibility = Visibility.Visible;
                btnAddContractAttachment.Visibility = Visibility.Collapsed;

                //设置时间上限下限
                dtContractStart.MinDate = AddContract_Min_Date;
                dtContractStart.MaxDate = AddContract_Max_Date;

                #region 填充页面

                dtContractStart.SelectedDateTime = AddContract_Min_Date.AddDays(1);
                cbContractLong_SelectionChanged(null, null);
                dtContractWrite.SelectedDateTime = DateTime.Now;
                txtContractPrice.Clear();
                txtContractRemark.Clear();
                lblContractState.Content = "未开始";

                #endregion
            }
            else
            {
                EnableContractGrid(false);
                //已有合同 显示上传附件 不显示保存
                btnContractSubmit.Visibility = Visibility.Collapsed;
                btnAddContractAttachment.Visibility = Visibility.Visible;

                StaffContract contract = selectedItem.Tag as StaffContract;//现有合同实体

                #region 填充页面

                dtContractStart.SelectedDateTime = contract.Start;
                dtContractEnd.SelectedDateTime = contract.End;
                int year = contract.End.Year - contract.Start.Year;
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
                dtContractWrite.SelectedDateTime = contract.Write;
                txtContractPrice.Text = contract.Price.ToString();
                txtContractRemark.Text = contract.Remark;

                //合同状态
                if (contract.Start < DateTime.Now && contract.End > DateTime.Now)
                {
                    lblContractState.Content = "执行中";
                }
                else if (contract.Start > DateTime.Now)
                {
                    lblContractState.Content = "未开始";
                }
                else if (contract.End < DateTime.Now)
                {
                    lblContractState.Content = "已过期";
                }

                #endregion



                selectedId = contract.Id;
            }

            //更新附件列表
            UpdateContractAttachment(selectedId);
        }

        /// <summary>
        /// 控制合同页面可操作性
        /// </summary>
        /// <param name="_enable"></param>
        private void EnableContractGrid(bool _enable)
        {
            dtContractStart.IsEnabled = _enable;
            cbContractLong.IsEnabled = _enable;
            dtContractEnd.IsEnabled = _enable;
            dtContractWrite.IsEnabled = _enable;
            txtContractPrice.IsEnabled = _enable;
            txtContractRemark.IsEnabled = _enable;
        }

        /// <summary>
        /// 更新附件列表
        /// </summary>
        private void UpdateContractAttachment(int _id)
        {
            bAttachment.Visibility = Visibility.Visible;//数据的显示
            attachmentData.Clear();
            if (_id == 0)
            {
                return;
            }
            MessageBox.Show("更新附件未实现");
            //using (HRDBContext context = new HRDBContext())
            //{
            //    var attachments = context.Attachments.Where(c => !c.IsDel && c.FromTable == AttachmentGlobal.StaffContractId && c.DataId == _id).ToList();
            //    if(attachments.Count > 0) bAttachment.Visibility = Visibility.Collapsed ;//数据的显示
            //    foreach (var item in attachments)
            //    {
            //        //第一个参数为：获取缩略图文件路径
            //        //第二个参数为：返回图片的宽
            //        //第三个参数为：返回图片的高
            //        //Bitmap bm = WindowsThumbnailProvider.GetThumbnail(path, pic_size, pic_size, ThumbnailOptions.None);

            //        var model = new AttachmentUIModel();
            //        model.Id = item.Id;
            //        model.CreateTime = item.CreateTime.ToString("yy年MM月dd日");
            //        model.Name = item.SavedName;
            //        model.Img = WindowsThumbnailProvider.GetThumbnail(item.SavedPath, 20, 20, ThumbnailOptions.None);
            //        attachmentData.Add(model);
            //    }
            //}
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

            using (HRDBContext context = new HRDBContext())
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
                context.SaveChanges();
            }

            Succeed = true;
            UpdateContractHistory();//更新列表
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

        /// <summary>
        /// 上传合同附件
        /// </summary>
        private void btnAddContractAttachment_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("C:");//打开本地文件测试
            MessageBox.Show("上传合同应至服务器端，待服务端完善后添加合同上传");
            //if (tvContractHistory.SelectedItem == null) return;
            //TreeViewItem selectedItem = tvContractHistory.SelectedItem as TreeViewItem;//获取选中项
            //StaffContract contract = selectedItem.Tag as StaffContract;//现有合同实体
        }

        /// <summary>
        /// 终止合同
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopContract_Click(object sender, RoutedEventArgs e)
        {
            if (tvContractHistory.SelectedItem == null) return;
            TreeViewItem selectedItem = tvContractHistory.SelectedItem as TreeViewItem;//获取选中项
            StaffContract contract = selectedItem.Tag as StaffContract;//现有合同实体

            SelectedDateTime selectedDateTime = new SelectedDateTime(contract.Start, DateTime.MaxValue, DateTime.Now);
            selectedDateTime.ShowDialog();
            if (selectedDateTime.Succeed)
            {
                DateTime endTime = selectedDateTime.SelectedDate;
                using (HRDBContext context = new HRDBContext())
                {
                    context.StaffContract.Single(c => c.Id == contract.Id).End = endTime;
                    context.SaveChanges();
                }
                UpdateContractAttachment(contract.Id);

                Notice.Show($"合同终止成功,终止时间{endTime.ToString("yy年MM月dd日")}", "终止成功");
            }
        }

        private void btnOpenContractAttachment_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("E:");
        }

        private void btnDeleteContractAttachment_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();//获取绑定Id
            //using (HRDBContext context = new HRDBContext())
            //{
            //    context.Attachments.Remove(context.Attachments.First(c => c.Id == id));//删除数据库
            //    attachmentData.Remove(attachmentData.First(c => c.Id == id));//删除UI

            //    context.SaveChanges();
            //}
            MessageBox.Show("移除附件未实现");
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

            using (HRDBContext context = new HRDBContext())
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

            using (HRDBContext context = new HRDBContext())
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

                using (HRDBContext context = new HRDBContext())
                {
                    model = context.StaffInsurance.Add(model);
                    context.SaveChanges();
                }
            }
            else if (syType == InsuranceSYType.Edit)
            {
                using (HRDBContext context = new HRDBContext())
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

            using (HRDBContext context = new HRDBContext())
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
            using (HRDBContext context = new HRDBContext())
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
            using (HRDBContext context = new HRDBContext())
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
            using (HRDBContext context = new HRDBContext())
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
