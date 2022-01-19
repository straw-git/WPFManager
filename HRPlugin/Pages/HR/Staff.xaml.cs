using DBModels.Staffs;
using DBModels.Sys;
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
using Common.Utils;
using System.Linq.Expressions;
using Common.Windows;
using Common.MyAttributes;

namespace HRPlugin.Pages.HR
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Staff : BasePage
    {
        public Staff()
        {
            InitializeComponent();
            this.Order = 1;
        }

        #region Models 

        class UIModel : BaseUIModel
        {
            public string Id { get; set; }
            private string name = "";
            [DataSourceBinding("姓名", -1, 1)]
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
            private int age = 0;
            [DataSourceBinding("年龄", 80, 2)]
            public int Age
            {
                get => age;
                set
                {
                    age = value;
                    NotifyPropertyChanged("Age");
                }
            }
            private string jobPostName = "";
            //职务
            [DataSourceBinding("职务", 100, 3)]
            public string JobPostName
            {
                get => jobPostName;
                set
                {
                    jobPostName = value;
                    NotifyPropertyChanged("JobPostName");
                }
            }

            [DataSourceBinding("基础工资", 100, 4)]
            public decimal Wage { get; set; }
            [DataSourceBinding("入职时间", 100, 5)]
            public string CreateTime { get; set; }

            public int SBInsuranceCount { get; set; }//社保数量
            public Brush SBInsuranceCountBrush
            {
                get
                {
                    return SBInsuranceCount == 0 ? Brushes.Red : Brushes.Green;
                }
            }
            public int SYInsuranceCount { get; set; }//商业保险数量
            [DataSourceBinding("合同剩余/天", 100, 6)]
            public string ContractSurplusDays { get; set; }//合同剩余天数
            public Brush ContractSurplusDaysBrush
            {
                get
                {
                    if (ContractSurplusDays == "无合同")
                    {
                        return Brushes.Red;
                    }
                    else if (ContractSurplusDays == "已过期")
                    {
                        return Brushes.Yellow;
                    }
                    else return Brushes.Black;
                }
            }
        }

        #endregion 

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        //当前选中的ID
        private int currSelectedId = 0;

        protected override void OnPageLoaded()
        {
            SetDataGridBinding(list, new UIModel(), Data);

            new JobPostTreeViewCommon(tvJobPost).Init();

            UpdateGauge();
        }

        #region UI Method

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null) return;
            UIModel selectedModel = list.SelectedItem as UIModel;
            if (selectedModel != null)
            {
                MaskVisible(true);
                EditStaff a = new EditStaff(selectedModel.Id);
                a.ShowDialog();
                if (a.Succeed)
                {
                    MessageBoxX.Show("编辑成功", "成功提醒");

                    #region 刷新数据

                    var _model = Data.Single(c => c.Id == selectedModel.Id);
                    using (DBContext context = new DBContext())
                    {
                        _model.Age = 0;
                        _model.CreateTime = a.StaffModel.CreateTime.ToString("yy-MM-dd");
                        _model.JobPostName = context.SysDic.Any(c => c.Id == a.StaffModel.JobPostId) ? context.SysDic.First(c => c.Id == a.StaffModel.JobPostId).Name : "无";
                        _model.Name = a.StaffModel.Name;
                    }
                    DateTime brthday = IdCardCommon.GetBirthday(a.StaffModel.IdCard);
                    _model.Age = DateTime.Now.Year - brthday.Year;

                    #endregion 
                }

                MaskVisible(false);
            }
        }

        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)((sender as CheckBox).IsChecked);
            foreach (var item in Data)
            {
                item.IsChecked = isCheck;
                if (isCheck)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    SelectedTableData(list.Name, item);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(list.Name, c => c.Id == item.Id);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditStaff a = new EditStaff();
            a.ShowDialog();
            if (a.Succeed)
            {
                UpdatePager(null, null);
            }

            MaskVisible(false);
        }

        private void tvJobPost_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = tvJobPost.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                int selectedJobPostId = 0;
                if (int.TryParse(selectedItem.Tag.ToString(), out selectedJobPostId))
                {
                    if (selectedItem.Items.Count > 0) return;
                    currSelectedId = selectedJobPostId;
                    UpdatePager(null, null);
                }
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdatePager(null, null);
            }
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectItem = list.SelectedItem as UIModel;
                bool targetIsChecked = !selectItem.IsChecked;
                Data.Single(c => c.Id == selectItem.Id).IsChecked = targetIsChecked;

                if (targetIsChecked)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    SelectedTableData(list.Name, selectItem);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(list.Name, c => c.Id == selectItem.Id);
                }
            }
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();
            var _model = Data.First(c => c.Id == id);
            var result = MessageBoxX.Show($"员工信息停职后,将涉及到该员工的所有工资、保险等信息影响统计结果", $"是否确认停职员工[{_model.Name}]", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MaskVisible(true);
                StaffDelete s = new StaffDelete();
                s.ShowDialog();
                MaskVisible(false);
                if (s.Succeed)
                {
                    using (DBContext context = new DBContext())
                    {
                        var staff = context.Staff.Single(c => c.Id == id);
                        staff.IsDel = true;
                        staff.DelUser = UserGlobal.CurrUser.Id;
                        staff.DelTime = s.Model.StopTime;

                        context.StaffSalary.Add(new StaffSalary()
                        {
                            CreateTime = staff.DelTime,
                            Creator = staff.DelUser,
                            End = staff.DelTime,
                            IsEnd = false,
                            Price = 0,
                            Register = staff.Register,
                            Remark = "停职",
                            SatffName = staff.Name,
                            StaffId = staff.Id,
                            StaffQuickCode = staff.QuickCode,
                            Start = s.Model.StopTime
                        });

                        if (s.Model.StopContract)
                        {
                            if (context.StaffContract.Any(c => c.StaffId == id && !c.Stop))
                            {
                                //终止合同
                                var contract = context.StaffContract.Single(c => c.StaffId == id);
                                contract.Stop = true;
                                contract.StopTime = staff.DelTime;
                                contract.StopUser = staff.DelUser;
                            }
                        }
                        if (s.Model.StopInsurance)
                        {
                            if (context.StaffInsurance.Any(c => c.StaffId == id && !c.Stop))
                            {
                                //停保
                                var insurances = context.StaffInsurance.Where(c => c.StaffId == id);
                                foreach (var item in insurances)
                                {
                                    var insurance = context.StaffInsurance.Single(c => c.Id == item.Id);
                                    insurance.Stop = true;
                                    insurance.End = staff.DelTime;
                                    insurance.StopUser = staff.DelUser;
                                }
                            }
                        }

                        Data.Remove(_model);

                        context.SaveChanges();
                    }
                    UpdateGauge();
                }
            }
        }

        private void btnTableColumnVisible_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            BasePageVisibilititySetting basePageVisibilititySetting = new BasePageVisibilititySetting(GetDataGridHeaders(list.Name));
            basePageVisibilititySetting.ShowDialog();

            var result = basePageVisibilititySetting.Result;
            foreach (var ri in result)
            {
                Visibility _visibility = ri.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                SetDataGridColumnVisibilitity(list.Name, ri.Title, _visibility);
            }

            UpdateDataGridColumnVisibility(list);

            MaskVisible(false);
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 更新进度条
        /// </summary>
        private void UpdateGauge()
        {
            using (DBContext context = new DBContext())
            {
                int staffCount = context.Staff.Count(c => !c.IsDel);
                pInsurance.To = staffCount;
                pInsurance.Value = context.StaffInsurance.Where(c => !c.Stop).GroupBy(c=>c.StaffId).Count();

                pContract.To = staffCount;
                pContract.Value = context.StaffContract.Where(c=>!c.Stop).GroupBy(c => c.StaffId).Count();
            }
        }

        private async void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            if (e == null) gPager.CurrentIndex = 1;//如果是通过查询或者刷新点击的 直接显示第一页

            //查询条件
            string name = txtName.Text.Trim();
            bool enableTime = (bool)cbEnableTime.IsChecked;
            DateTime startTime = dtStart.SelectedDateTime.MinDate();
            DateTime endTime = dtEnd.SelectedDateTime.MaxDate();
            string listName = list.Name;
            bool isNoInsurance = (bool)cbNoInsurance.IsChecked;
            bool isNoContract = (bool)cbNoContract.IsChecked;

            Data.Clear();//先清空再加入页面数据

            using (DBContext context = new DBContext())
            {
                Expression<Func<DBModels.Staffs.Staff, bool>> _where = n => GetPagerWhere(n, name, isNoInsurance, isNoContract, enableTime, startTime, endTime);//按条件查询
                Expression<Func<DBModels.Staffs.Staff, DateTime>> _orderByDesc = n => n.CreateTime;//按时间倒序
                //开始分页查询数据
                var _zPager = await PagerCommon.BeginEFDataPagerAsync(context.Staff, _where, _orderByDesc, gLoading, gPager, bNoData, new Control[1] { list });
                if (!_zPager.Result) return;
                List<DBModels.Staffs.Staff> _list = _zPager.EFDataList;

                #region 页面数据填充

                foreach (var item in _list)
                {
                    string jobpostName = context.SysDic.Any(c => c.Id == item.JobPostId) ? context.SysDic.First(c => c.Id == item.JobPostId).Name : "无";
                    var _model = DBItem2UIModel(item, jobpostName, listName);
                    //类型为社保 并且没有被停止的数量
                    _model.SBInsuranceCount = context.StaffInsurance.Count(c => c.Type == 0 && c.StaffId == item.Id && !c.Stop);
                    //类型为商业保险 并且没有被停止的数量
                    _model.SYInsuranceCount = context.StaffInsurance.Count(c => c.Type == 1 && c.StaffId == item.Id && !c.Stop);
                    //合同剩余天数
                    _model.ContractSurplusDays = "无合同";
                    var contractList = context.StaffContract.Where(c => c.StaffId == item.Id && !c.Stop).ToList();//所有合同

                    var nowTime = DateTime.Now.MinDate();
                    if (contractList != null && contractList.Count > 0)
                    {
                        if (contractList.Any(c => c.End >= nowTime))
                        {
                            _model.ContractSurplusDays = Math.Round((contractList.First(c => c.End >= nowTime).End - DateTime.Now).TotalDays).ToString();
                        }
                        else
                        {
                            _model.ContractSurplusDays = "已过期";
                        }
                    }
                    //基础工资
                    _model.Wage = 0;
                    if (context.StaffSalary.Any(c => c.StaffId == item.Id && c.Start <= nowTime && c.End >= nowTime))
                    {
                        var wage = context.StaffSalary.Where(c => c.StaffId == item.Id && c.Start <= nowTime && c.End >= nowTime).OrderByDescending(c => c.CreateTime).First();
                        _model.Wage = wage.Price;
                    }
                    Data.Add(_model);
                }

                #endregion
            }

            //结尾处必须结束分页查询
            PagerCommon.EndEFDataPager();
        }

        private UIModel DBItem2UIModel(DBModels.Staffs.Staff item, string _jobpostName, string _listName)
        {
            UIModel _model = new UIModel()
            {
                Age = 0,
                CreateTime = item.CreateTime.ToString("yy-MM-dd"),
                Id = item.Id,
                JobPostName = _jobpostName,
                Name = item.Name
            };

            DateTime brthday = IdCardCommon.GetBirthday(item.IdCard);
            _model.Age = DateTime.Now.Year - brthday.Year;
            return _model;
        }

        /// <summary>
        /// 查找表格的条件
        /// </summary>
        protected bool GetPagerWhere(DBModels.Staffs.Staff _staff, string _name, bool _isNoInsurance, bool _isNoContract, bool _enableTime, DateTime _start, DateTime _end)
        {
            bool resultCondition = true;
            if (_name.NotEmpty())
            {
                //根据名称检索
                resultCondition &= _staff.Name.Contains(_name) || _staff.QuickCode.Contains(_name);
            }
            if (currSelectedId > 0)
            {
                resultCondition &= _staff.JobPostId == currSelectedId;
            }
            if (_isNoInsurance)
            {
                //未参保
                using (DBContext context = new DBContext())
                {
                    resultCondition &= !context.StaffInsurance.Any(c => c.StaffId == _staff.Id && !c.Stop);
                }
            }
            if (_isNoContract)
            {
                //未签合同
                using (DBContext context = new DBContext())
                {
                    resultCondition &= !context.StaffContract.Any(c => c.StaffId == _staff.Id && !c.Stop);
                }
            }
            if (_enableTime)
            {
                resultCondition &= _staff.CreateTime >= _start && _staff.CreateTime <= _end;
            }

            resultCondition &= !_staff.IsDel;

            return resultCondition;
        }

        #endregion

        #region 导出Excel

        private void btnExportCurrPage_Click(object sender, RoutedEventArgs e)
        {
            var listData = Data.ToList();//获取选中数据
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题
            if (list == null || listData.Count == 0)
            {
                MessageBoxX.Show("没有选中数据", "空值提醒");
                return;
            }
            new ExcelHelper().List2ExcelAsync(listData, $"页码{gPager.CurrentIndex}", columns, hiddleColumns.Keys.ToList());
        }

        private async void btnExportAllPage_Click(object sender, RoutedEventArgs e)
        {
            //导出所有数据
            List<UIModel> allData = new List<UIModel>();
            string listName = list.Name;

            await Task.Run(() =>
            {
                var _list = new List<DBModels.Staffs.Staff>();
                using (DBContext context = new DBContext())
                {
                    _list = context.Staff.OrderByDescending(c => c.CreateTime).ToList();
                    foreach (var item in _list)
                    {
                        string jobpostName = context.SysDic.Any(c => c.Id == item.JobPostId) ? context.SysDic.First(c => c.Id == item.JobPostId).Name : "无";
                        var _model = DBItem2UIModel(item, jobpostName, listName);
                        //类型为社保 并且没有被停止的数量
                        _model.SBInsuranceCount = context.StaffInsurance.Count(c => c.Type == 0 && c.StaffId == item.Id && !c.Stop);
                        //类型为商业保险 并且没有被停止的数量
                        _model.SYInsuranceCount = context.StaffInsurance.Count(c => c.Type == 1 && c.StaffId == item.Id && !c.Stop);
                        //合同剩余天数
                        _model.ContractSurplusDays = "无合同";
                        var contractList = context.StaffContract.Where(c => c.StaffId == item.Id && !c.Stop).ToList();//所有合同

                        var nowTime = DateTime.Now.MinDate();
                        if (contractList != null && contractList.Count > 0)
                        {
                            if (contractList.Any(c => c.End >= nowTime))
                            {
                                _model.ContractSurplusDays = Math.Round((contractList.First(c => c.End >= nowTime).End - DateTime.Now).TotalDays).ToString();
                            }
                            else
                            {
                                _model.ContractSurplusDays = "已过期";
                            }
                        }
                        //基础工资
                        _model.Wage = 0;
                        if (context.StaffSalary.Any(c => c.StaffId == item.Id && c.Start >= nowTime && c.End <= nowTime))
                        {
                            var wage = context.StaffSalary.Where(c => c.StaffId == item.Id && c.Start >= nowTime && c.End <= nowTime).OrderByDescending(c => c.CreateTime).First();
                            _model.Wage = wage.Price;
                        }
                        allData.Add(_model);
                    }
                }
            });
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题

            new ExcelHelper().List2ExcelAsync(allData, "所有数据", columns, hiddleColumns.Keys.ToList());
        }

        private void btnExportFocusDatas_Click(object sender, RoutedEventArgs e)
        {
            var listData = GetSelectedTableData<UIModel>(list.Name);//获取选中数据
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题
            if (listData == null || listData.Count == 0)
            {
                MessageBoxX.Show("没有选中数据", "空值提醒");
                return;
            }
            new ExcelHelper().List2ExcelAsync(listData, "选中数据", columns, hiddleColumns.Keys.ToList());
        }

        #endregion

    }
}
