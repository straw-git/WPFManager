
using CorePlugin.Windows;
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
using System.Reflection;
using Common.Utils;
using CoreDBModels;

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class User : Page
    {
        public User()
        {
            InitializeComponent();
            this.StartPageInAnimation();
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public int Id { get; set; }

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
            private string realName = "";
            public string RealName
            {
                get => realName;
                set
                {
                    realName = value;
                    NotifyPropertyChanged("RealName");
                }
            }
            public int RoleId { get; set; }
            private string roleName = "";
            public string RoleName //角色名称
            {
                get => roleName;
                set
                {
                    roleName = value;
                    NotifyPropertyChanged("RoleName");
                }
            }

            private string departmentName = "";
            public string DepartmentName
            {
                get => departmentName;
                set
                {
                    departmentName = value;
                    NotifyPropertyChanged("DepartmentName");
                }
            }

            private string positionName = "";
            public string PositionName
            {
                get => positionName;
                set
                {
                    positionName = value;
                    NotifyPropertyChanged("PositionName");
                }
            }

            private bool canLogin = false;
            public bool CanLogin
            {
                get => canLogin;
                set
                {
                    canLogin = value;
                    NotifyPropertyChanged("CanLogin");
                }
            }

            private int pageCount = 0;
            public int PageCount
            {
                get => pageCount;
                set
                {
                    pageCount = value;
                    NotifyPropertyChanged("PageCount");
                }
            }

            public int CreateYear { get; set; }
            public string CreateTime { get; set; }
        }

        #endregion

        //页面初始化
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRoles();//加载角色
            list.ItemsSource = Data;//绑定数据源
            cbDepartment.ItemsSource = DepartmentData;
            cbDepartment.DisplayMemberPath = "Name";
            cbDepartment.SelectedValuePath = "Id";
            cbPosition.ItemsSource = PositionData;
            cbPosition.DisplayMemberPath = "Name";
            cbPosition.SelectedValuePath = "Id";

            updateList = false;
            UpdateDepartment();
            updateList = true;
        }

        #region 左侧角色列表

        #region 角色属性

        bool allChecking = false;//是否在进行全选或反选操作

        #endregion 

        //获取选中的角色
        private List<int> GetSelectedRoleIds()
        {
            List<int> ids = new List<int>();
            foreach (var item in wpRoles.Children)
            {
                CheckBox target = (item as CheckBox);
                if ((bool)target.IsChecked)
                    ids.Add(target.Tag.ToString().AsInt());
            }
            return ids;
        }

        //初始化角色
        private void UpdateRoles()
        {
            List<Role> _roles = null;
            using (CoreDBContext context = new CoreDBContext())
            {
                _roles = context.Role.Where(c => !c.IsDel).ToList();
            }
            wpRoles.Children.Clear();
            foreach (var _r in _roles)
            {
                //添加角色
                CheckBox checkBox = new CheckBox();
                checkBox.Height = 30;
                checkBox.Content = _r.Name;
                checkBox.Background = new SolidColorBrush(Colors.Gray);
                checkBox.Foreground = new SolidColorBrush(Colors.White);
                checkBox.Margin = new Thickness(5);
                checkBox.Tag = _r.Id;
                checkBox.Checked += RoleItem_Checked;
                checkBox.Unchecked += RoleItem_Unchecked;
                CheckBoxHelper.SetCheckBoxStyle(checkBox, CheckBoxStyle.Button);
                CheckBoxHelper.SetCheckedBackground(checkBox, new SolidColorBrush(Colors.Black));
                CheckBoxHelper.SetCornerRadius(checkBox, new CornerRadius(5));

                wpRoles.Children.Add(checkBox);
            }
        }

        //角色不选中
        private void RoleItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (allChecking) return;//全选或反选操作中不做任何动作
            //CheckBox currRoleCheckBox = sender as CheckBox;
            if (btnSelectedAllRoles.Content.ToString() == "\xf058")
            {
                btnSelectedAllRoles.Content = "\xf05d";
                btnSelectedAllRoles.Foreground = ColorHelper.ConvertToSolidColorBrush("#EAEAEA");
            }
            //暂时简便直接刷新
            UpdateGridAsync();

        }

        //角色选中
        private void RoleItem_Checked(object sender, RoutedEventArgs e)
        {
            if (allChecking) return;//全选或反选操作中不做任何动作
                                    //CheckBox currRoleCheckBox = sender as CheckBox;
                                    //暂时简便直接刷新
            UpdateGridAsync();
        }

        //添加角色
        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditRole editRole = new EditRole();
            if (editRole.ShowDialog() == true)
            {
                UpdateRoles();
            }
            this.MaskVisible(false);
        }

        //全选、反选
        private void btnSelectedAllRoles_Click(object sender, RoutedEventArgs e)
        {
            allChecking = true;

            if (btnSelectedAllRoles.Content.ToString() == "\xf058")
            {
                #region 取消选择

                btnSelectedAllRoles.Content = "\xf05d";
                btnSelectedAllRoles.Foreground = ColorHelper.ConvertToSolidColorBrush("#EAEAEA");
                foreach (var item in wpRoles.Children)
                {
                    (item as CheckBox).IsChecked = false;
                }

                #endregion
            }
            else
            {
                #region 选择

                btnSelectedAllRoles.Content = "\xf058";
                btnSelectedAllRoles.Foreground = new SolidColorBrush(Colors.Black);
                foreach (var item in wpRoles.Children)
                {
                    (item as CheckBox).IsChecked = true;
                }

                #endregion
            }

            allChecking = false;
            UpdateGridAsync();
        }

        private void cbCanLogin_Click(object sender, RoutedEventArgs e)
        {
            int userId = (sender as CheckBox).Tag.ToString().AsInt();//获取用户Id
            var selectedUser = Data.First(c => c.Id == userId);//选中的User
            var result = MessageBoxX.Show($"是否确认更改[{selectedUser.Name}]登录权限？", "权限更新提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                UpdateCanLogin(userId, !selectedUser.CanLogin);
            else (sender as CheckBox).IsChecked = selectedUser.CanLogin;
        }

        //更新用户是否可登录
        private void UpdateCanLogin(int _userId, bool _canLogin)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                //更新数据
                context.User.Single(c => c.Id == _userId).CanLogin = _canLogin;
                context.SaveChanges();
                //更新UI
                Data.Single(c => c.Id == _userId).CanLogin = _canLogin;
            }
            this.Log("登录权限更新成功！");
        }

        #endregion

        #region 右侧用户列表

        #region 分页属性

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();//页面数据集合
        int dataCount = 0;//数据总条数
        int pagerCount = 0;//总页数
        int pageSize = 20;//页数据量
        int currPage = 1;//当前页码
        bool running = false;//是否正在执行查询

        #endregion

        /// <summary>
        /// 加载分页数据
        /// </summary>
        private async void UpdateGridAsync()
        {
            var selectedRoleIds = GetSelectedRoleIds();//选择的角色
            int departmentId = cbDepartment.SelectedItem == null ? 0 : (cbDepartment.SelectedItem as Department).Id;//选择的部门
            int positionId = cbPosition.SelectedItem == null ? 0 : (cbPosition.SelectedItem as DepartmentPosition).Id;//选择的职位
            string searchText = txtSearchText.Text;//按名称搜索

            ShowLoadingPanel();//显示Loading
            if (running) return;
            running = true;

            Data.Clear();
            List<CoreDBModels.User> models = new List<CoreDBModels.User>();

            await Task.Run(() =>
            {
                using (var context = new CoreDBContext())
                {
                    IQueryable<CoreDBModels.User> users = context.User.Where(c => !c.IsDel);
                    if (selectedRoleIds.Count > 0)
                        users = users.Where(c => selectedRoleIds.Contains(c.RoleId));
                    if (departmentId > 0)
                    {
                        //选择了部门
                        if (positionId > 0)
                        {
                            //选择了职位
                            users = users.Where(c => c.DepartmentPositionId == positionId);
                        }
                        else
                        {
                            //没有选择职位
                            users = users.Where(c => c.DepartmentId == departmentId);
                        }
                    }
                    if (searchText.NotEmpty())
                        users = users.Where(c => c.Name.Contains(searchText) || c.RealName.Contains(searchText));

                    dataCount = users.Count();
                    //
                    //页码
                    //
                    pagerCount = PagerUtils.GetPagerCount(dataCount, pageSize);
                    if (currPage > pagerCount) currPage = pagerCount;
                    //更新页码
                    UIGlobal.RunUIAction(() =>
                    {
                        gPager.CurrentIndex = currPage;
                        gPager.TotalIndex = pagerCount;
                    });

                    //生成分页数据
                    models = users.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);
            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;
            using (CoreDBContext context = new CoreDBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        CreateYear = item.CreateTime.Year,
                        CreateTime = item.CreateTime.ToString("MM-dd HH:mm"),
                        Id = item.Id,
                        Name = item.Name,
                        RoleId = item.RoleId,
                        RoleName = context.Role.First(c => c.Id == item.RoleId).Name,
                        CanLogin = item.CanLogin,
                        DepartmentName = context.Department.Any(c => c.Id == item.DepartmentId) ? context.Department.First(c => c.Id == item.DepartmentId).Name : "无部门",
                        PositionName = context.DepartmentPosition.Any(c => c.Id == item.DepartmentPositionId) ? context.DepartmentPosition.First(c => c.Id == item.DepartmentPositionId).Name : "无职位",
                        RealName = item.RealName
                    };

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        #region Grid

        //搜索
        private void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateGridAsync();
            }
        }

        //行加载事件 检查是否为超级管理员 
        //如果是超级管理员则不可修改
        private void list_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            UIModel userModel = e.Row.Item as UIModel;
            if (userModel.Name == "admin")//只有系统最初的这个admin用户 信息不可修改
            {
                e.Row.IsEnabled = false;
                e.Row.Background = new SolidColorBrush(Colors.LightBlue);//显示成灰色
            }
        }

        //重置密码为123456
        private void btnRePwd_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            var selectModel = Data.First(c => c.Id == id);
            var result = MessageBoxX.Show($"是否确认重置[{selectModel.Name}]密码？", "密码初始化提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    var user = context.User.Single(c => c.Id == id);
                    user.Pwd = "123456";
                    if (user.Id == UserGlobal.CurrUser.Id)
                    {
                        UserGlobal.CurrUser.Pwd = user.Pwd;
                    }
                    context.SaveChanges();
                }
                MessageBoxX.Show("初始密码\"123456\"", "密码初始化成功");
            }
        }

        //账号授权
        private void btnAuthorization_Click(object sender, RoutedEventArgs e)
        {
            int userId = (sender as Button).Tag.ToString().AsInt();
            this.MaskVisible(true);

            Authorization2User a = new Authorization2User(userId);
            a.ShowDialog();

            this.MaskVisible(false);

            if (a.Succeed)
            {
                MessageBoxX.Show("请当前用户重新登录", "授权成功");
            }
        }

        //删除账号
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            UIModel selectModel = Data.First(c => c.Id == id);

            var result = MessageBoxX.Show($"是否确认删除账户[{selectModel.Name}]？", "删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    var user = context.User.Single(c => c.Id == id);
                    user.IsDel = true;
                    user.DelTime = DateTime.Now;
                    user.DelUser = UserGlobal.CurrUser.Id;

                    context.SaveChanges();
                    Data.Remove(selectModel);
                }
            }
        }

        //添加账号
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            AddUser a = new AddUser();
            a.ShowDialog();
            this.MaskVisible(false);
            if (a.Succeed)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    Data.Insert(0, new UIModel()
                    {
                        CreateYear = a.Model.CreateTime.Year,
                        CreateTime = a.Model.CreateTime.ToString("MM-dd HH:mm"),
                        Id = a.Model.Id,
                        Name = a.Model.Name,
                        RoleId = a.Model.RoleId,
                        RoleName = context.Role.First(c => c.Id == a.Model.RoleId).Name,
                        CanLogin = a.Model.CanLogin
                    });
                }
            }
        }

        //页码更改事件
        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            UpdateGridAsync();
        }

        //刷新
        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridAsync();
        }

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;
                gPager.IsEnabled = false;
                bNoData.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;
                gPager.IsEnabled = true;
                bNoData.IsEnabled = true;

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

        #endregion

        #endregion

        #region 部门、职位

        ObservableCollection<Department> DepartmentData = new ObservableCollection<Department>();
        ObservableCollection<DepartmentPosition> PositionData = new ObservableCollection<DepartmentPosition>();
        bool updateList = false;//是否更新数据

        /// <summary>
        /// 更新部门
        /// </summary>
        private void UpdateDepartment()
        {
            DepartmentData.Clear();

            DepartmentData.Add(new Department()
            {
                Id = 0,
                Name = "全部部门"
            });
            using (CoreDBContext context = new CoreDBContext())
            {
                var list = context.Department.Where(c => !c.IsDel).ToList();
                if (list != null)
                    foreach (var item in list)
                    {
                        DepartmentData.Add(item);
                    }
            }
            cbDepartment.SelectedIndex = 0;
        }

        private void cbDepartment_SearchTextChanged(object sender, Panuon.UI.Silver.Core.SearchTextChangedEventArgs e)
        {

        }

        private void cbPosition_SearchTextChanged(object sender, Panuon.UI.Silver.Core.SearchTextChangedEventArgs e)
        {

        }

        private void cbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDepartment.SelectedItem != null)
            {
                var selectedModel = cbDepartment.SelectedItem as Department;

                PositionData.Clear();

                PositionData.Add(new DepartmentPosition()
                {
                    Id = 0,
                    Name = "全部职位"
                });

                if (selectedModel.Id > 0)
                {
                    using (CoreDBContext context = new CoreDBContext())
                    {
                        var list = context.DepartmentPosition.Where(c => !c.IsDel).ToList();
                        if (list != null)
                            foreach (var item in list)
                            {
                                PositionData.Add(item);
                            }
                    }
                }
                cbPosition.SelectedIndex = 0;

                if (updateList)
                    UpdateGridAsync();
            }
        }

        private void cbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPosition.SelectedItem != null)
            {
                var selectedModel = cbPosition.SelectedItem as DepartmentPosition;
                if (selectedModel.Id > 0)
                {
                    if (updateList)
                        UpdateGridAsync();
                }
            }
        }

        #endregion

    }
}
