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
            this.Order = 0;
        }

        #region Models 

        class UIModel : INotifyPropertyChanged
        {
            public string Id { get; set; }
            private string name = "";
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
            public string JobPostName
            {
                get => jobPostName;
                set
                {
                    jobPostName = value;
                    NotifyPropertyChanged("JobPostName");
                }
            }
            public string CreateTime { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion 

        enum SearchType
        {
            Role,
            JobPost
        }

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        List<SysDic> roles = new List<SysDic>();

        //当前页码
        private int currPage = 1;
        //数据总数
        private int dataCount = 0;
        //总页数
        private int pagerCount = 0;
        //页数据
        private int pageSize = 10;

        //当前选中的类型
        private SearchType currSelectedType = SearchType.Role;
        //当前选中的ID
        private int currSelectedId = 0;
        bool running = false;


        protected override void OnPageLoaded()
        {
            new RoleTreeViewCommon(tvRole).Init();
            new JobPostTreeViewCommon(tvJobPost).Init();
            list.ItemsSource = Data;
            btnRef_Click(null, null);
        }

        #region UI Method

        private void btnWage_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();

            MaskVisible(true);
            EditStaffWage a = new EditStaffWage(id);
            a.ShowDialog();
            MaskVisible(false);
        }

        private void dPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = dPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            AddStaff a = new AddStaff();
            a.ShowDialog();
            if (a.Succeed)
            {
                UpdateGridAsync();
            }

            MaskVisible(false);
        }

        private void tvRole_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = tvRole.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                int selectedRoleId = 0;
                if (int.TryParse(selectedItem.Tag.ToString(), out selectedRoleId))
                {
                    currSelectedType = SearchType.Role;
                    currSelectedId = selectedRoleId;
                    ShowLoadingPanel();
                    UpdateGridAsync();
                    LoadPager();
                }
            }
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
                    currSelectedType = SearchType.JobPost;
                    currSelectedId = selectedJobPostId;
                    ShowLoadingPanel();
                    UpdateGridAsync();
                    LoadPager();
                }
            }
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            btnRef_Click(null, null);
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnRef_Click(null, null);
            }
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null) return;
            UIModel selectedModel = list.SelectedItem as UIModel;
            if (selectedModel != null)
            {
                MaskVisible(true);
                AddStaff a = new AddStaff(selectedModel.Id);
                a.ShowDialog();
                if (a.Succeed)
                {
                    MessageBoxX.Show("编辑成功", "成功提醒");

                    #region 刷新数据

                    var _model = Data.Single(c => c.Id == selectedModel.Id);
                    using (DBContext context = new DBContext())
                    {
                        _model.Age = 0;
                        _model.CreateTime = a.StaffModel.CreateTime.ToString("yyyy年MM月dd日");
                        _model.JobPostName = context.SysDic.First(c => c.Id == a.StaffModel.JobPostId).Name;
                        _model.Name = a.StaffModel.Name;
                    }
                    DateTime brthday = IdCardCommon.GetBirthday(a.StaffModel.IdCard);
                    _model.Age = DateTime.Now.Year - brthday.Year;

                    #endregion 
                }

                MaskVisible(false);
            }
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
                        staff.DelUser = TempBasePageData.message.CurrUser.Id;
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
                }
            }
        }

        #endregion

        #region Private Method

        private void LoadPager()
        {
            string name = txtName.Text;
            using (var context = new DBContext())
            {
                var staffs = context.Staff.Where(c => !c.IsDel);
                if (currSelectedId > 0)
                {
                    if (currSelectedType == SearchType.Role)
                    {
                        var roleStaffs = (from c in context.User where c.RoleId == currSelectedId && c.StaffId != "" select c.StaffId).ToList();
                        if (roleStaffs.Count == 0)
                        {
                            return;
                        }
                        int xx = 0;
                        foreach (var item in staffs)
                        {
                            if (roleStaffs.IndexOf(item.Id) > -1)
                            {
                                xx += 1;
                            }
                        }
                        dataCount = xx;
                    }
                    if (currSelectedType == SearchType.JobPost)
                    {
                        staffs = staffs.Where(c => c.JobPostId == currSelectedId);
                    }
                }
                if (!string.IsNullOrEmpty(name))
                {
                    staffs = staffs.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name));
                    dataCount = staffs.Count();
                }
                
            }
            pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);

            if (currPage > pagerCount) currPage = pagerCount;
            dPager.CurrentIndex = currPage;
            dPager.TotalIndex = pagerCount;
        }

        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();

            if (running) return;
            running = true;

            string name = txtName.Text;
            Data.Clear();
            List<DBModels.Staffs.Staff> models = new List<DBModels.Staffs.Staff>();

            await Task.Run(() =>
            {
                using (DBContext context = new DBContext())
                {
                    var staffs = context.Staff.Where(c => !c.IsDel);
                    if (!string.IsNullOrEmpty(name))
                    {
                        staffs = staffs.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name));
                    }
                    if (currSelectedId > 0)
                    {
                        if (currSelectedType == SearchType.Role)
                        {
                            var roleStaffs = (from c in context.User where c.RoleId == currSelectedId && c.StaffId != "" select c.StaffId).ToList();
                            if (roleStaffs.Count == 0)
                            {
                                UIGlobal.RunUIAction(() =>
                                {
                                    HideLoadingPanel();
                                });
                                running = false;
                                return;
                            }
                            foreach (var item in staffs)
                            {
                                if (roleStaffs.IndexOf(item.Id) > -1) 
                                {
                                    models.Add(item);
                                }
                            }
                            models = models.OrderBy(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                        }
                        if (currSelectedType == SearchType.JobPost)
                        {
                            staffs = staffs.Where(c => c.JobPostId == currSelectedId);
                            models = staffs.OrderBy(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                        }
                    }

                }
            });
            await Task.Delay(300);

            bNoData.Visibility = models.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            using (DBContext context = new DBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        Age = 0,
                        CreateTime = item.CreateTime.ToString("yyyy年MM月dd日"),
                        Id = item.Id,
                        JobPostName = context.SysDic.First(c => c.Id == item.JobPostId).Name,
                        Name = item.Name
                    };

                    DateTime brthday = IdCardCommon.GetBirthday(item.IdCard);
                    _model.Age = DateTime.Now.Year - brthday.Year;

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        #endregion

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;
                dPager.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;
                dPager.IsEnabled = true;
                OnLoadingHideComplate();
            }
        }

        private void OnLoadingHideComplate()
        {

        }

        private void OnLoadingShowComplate()
        {

        }

        #endregion

    }
}
