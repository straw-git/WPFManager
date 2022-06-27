
using Common;
using Common.Utils;
using CoreDBModels;
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

namespace CorePlugin.Windows
{
    /// <summary>
    /// AddUser.xaml 的交互逻辑
    /// </summary>
    public partial class AddUser : Window
    {
        public bool Succeed = false;
        public User Model = new User();
        bool IsEdit
        {
            get { return editId > 0; }
        }
        int editId = 0;

        public AddUser(int _userId = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            editId = _userId;
        }

        /// <summary>
        /// 编辑时初始化用户信息
        /// </summary>
        private void InitUserInfo()
        {
            User userModel = null;
            using (CoreDBContext context = new CoreDBContext())
            {
                userModel = context.User.First(c => c.Id == editId);
            }
            cbRoles.SelectedValue = userModel.RoleId;
            txtAdminName.Text = userModel.Name;
            txtAdminPwd.Password = userModel.Pwd;
            txtReAdminPwd.Password = userModel.Pwd;

            cbDepartment.SelectedItem = DepartmentData.First(c => c.Id == userModel.DepartmentId);
            cbPosition.SelectedItem = PositionData.First(c => c.Id == userModel.DepartmentPositionId);
            txtIdCard.Text = userModel.IdCard;
            txtRealName.Text = userModel.RealName;
            cbNewPosition.SelectedItem = NewPositionData.First(c => c.Id == userModel.NewPositionId);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRolesComobox();

            //绑定数据源
            cbDepartment.ItemsSource = DepartmentData;
            cbDepartment.DisplayMemberPath = "Name";
            cbDepartment.SelectedValuePath = "Id";

            cbPosition.ItemsSource = PositionData;
            cbPosition.DisplayMemberPath = "Name";
            cbPosition.SelectedValuePath = "Id";

            cbNewPosition.ItemsSource = NewPositionData;
            cbNewPosition.DisplayMemberPath = "Name";
            cbNewPosition.SelectedValuePath = "Id";

            LoadDepartmentComobox();
            if (IsEdit)
            {
                InitUserInfo();
            }
        }

        #region 部门职位

        ObservableCollection<Department> DepartmentData = new ObservableCollection<Department>();
        ObservableCollection<DepartmentPosition> PositionData = new ObservableCollection<DepartmentPosition>();
        ObservableCollection<DepartmentPosition> NewPositionData = new ObservableCollection<DepartmentPosition>();

        /// <summary>
        /// 加载部门下拉
        /// </summary>
        private void LoadDepartmentComobox()
        {
            DepartmentData.Clear();

            using (CoreDBContext context = new CoreDBContext())
            {
                var departments = context.Department.Where(c => !c.IsDel).ToList();
                foreach (var item in departments)
                {
                    DepartmentData.Add(item);
                }
            }
            if (DepartmentData.Count == 0)
            {
                DepartmentData.Add(new Department()
                {
                    Id = 0,
                    Name = "请添加部门"
                });
            }

            cbDepartment.SelectedIndex = 0;
        }

        private void cbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDepartment.SelectedItem == null) return;

            PositionData.Clear();
            NewPositionData.Clear();

            Department selectedModel = cbDepartment.SelectedItem as Department;
            using (CoreDBContext context = new CoreDBContext())
            {
                var departmentPositions = context.DepartmentPosition.Where(c => !c.IsDel).ToList();
                foreach (var item in departmentPositions)
                {
                    PositionData.Add(item);
                    NewPositionData.Add(item);
                }
            }

            if (PositionData.Count == 0)
            {
                PositionData.Add(new DepartmentPosition()
                {
                    Id = 0,
                    Name = "请添加职位"
                });
                NewPositionData.Add(new DepartmentPosition()
                {
                    Id = 0,
                    Name = "无"
                });
            }

            cbPosition.SelectedIndex = 0;
            cbNewPosition.SelectedIndex = 0;
        }

        #endregion

        //加载角色下拉
        private void LoadRolesComobox()
        {
            List<Role> roles = null;
            using (CoreDBContext context = new CoreDBContext())
            {
                roles = context.Role.Where(c => !c.IsDel).ToList();
            }
            cbRoles.ItemsSource = roles;
            cbRoles.DisplayMemberPath = "Name";
            cbRoles.SelectedValuePath = "Id";

            cbRoles.SelectedIndex = 0;
        }

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int roleId = 0;// lblRole.Tag.ToString().AsInt();
            if (roleId <= 0)
            {
                MessageBoxX.Show("请选择角色", "空值提醒");
                return;
            }
            if (string.IsNullOrEmpty(txtAdminName.Text))
            {
                MessageBoxX.Show("账号不能为空", "空值提醒");
                txtAdminName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtAdminPwd.Password))
            {
                MessageBoxX.Show("密码不能为空", "空值提醒");
                txtAdminName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtReAdminPwd.Password))
            {
                MessageBoxX.Show("确认密码不能为空", "空值提醒");
                txtReAdminPwd.Focus();
                return;
            }
            if (txtAdminPwd.Password != txtReAdminPwd.Password)
            {
                MessageBoxX.Show("两次密码不一致", "数据异常");
                txtReAdminPwd.Focus();
                txtReAdminPwd.SelectAll();
                return;
            }


            using (CoreDBContext context = new CoreDBContext())
            {
                if (IsEdit)
                {
                    if (context.User.Any(c => c.Name == txtAdminName.Text && c.Id != editId))
                    {
                        MessageBoxX.Show("账户名已存在", "数据异常");
                        txtAdminName.Focus();
                        txtAdminName.SelectAll();
                        return;
                    }
                    //编辑
                    Model = context.User.Single(c => c.Id == editId);
                    Model.CreateTime = DateTime.Now;
                    Model.Creator = UserGlobal.CurrUser.Id;
                    Model.Name = txtAdminName.Text;
                    Model.Pwd = txtAdminPwd.Password;
                    Model.RoleId = roleId;
                }
                else
                {
                    if (context.User.Any(c => c.Name == txtAdminName.Text))
                    {
                        MessageBoxX.Show("账户名已存在", "数据异常");
                        txtAdminName.Focus();
                        txtAdminName.SelectAll();
                        return;
                    }
                    //添加
                    Model.IsDel = false;
                    Model.DelUser = 0;
                    Model.DelTime = DateTime.Now;
                    //Model.StaffId = "";
                    Model.CanLogin = true;
                    Model.CreateTime = DateTime.Now;
                    Model.Creator = UserGlobal.CurrUser.Id;
                    Model.Name = txtAdminName.Text;
                    Model.Pwd = txtAdminPwd.Password;
                    Model.RoleId = roleId;

                    Model = context.User.Add(Model);

                }
                context.SaveChanges();
            }

            Succeed = true;
            Close();
        }

        #endregion

        #region 拖动窗体

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #endregion 

        //编辑
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            #region 验证

            if (!txtAdminName.NotEmpty())
            {
                tab.SelectedIndex = 0;
                return;
            }
            if (!txtRealName.NotEmpty())
            {
                tab.SelectedIndex = 1;
                return;
            }
            if (!txtIdCard.NotEmpty())
            {
                tab.SelectedIndex = 1;
                return;
            }

            string name = txtAdminName.Text;
            if (txtAdminPwd.Password != txtReAdminPwd.Password)
            {
                MessageBoxX.Show("两次密码输入不一致", "密码验证错误");
                return;
            }
            string password = txtAdminPwd.Password;
            int roleId = cbRoles.SelectedValue.ToString().AsInt();//角色Id

            string realName = txtRealName.Text;
            string idCard = txtIdCard.Text;

            int departmentId = (int)cbDepartment.SelectedValue;
            int positionId = (int)cbPosition.SelectedValue;

            if (departmentId == 0)
            {
                MessageBoxX.Show("请先添加部门后再继续操作", "部门不存在");
                return;
            }
            if (positionId == 0)
            {
                MessageBoxX.Show("请先添加职位后再继续操作", "职位不存在");
                return;
            }

            #endregion 

            using (CoreDBContext context = new CoreDBContext())
            {
                if (IsEdit)
                {
                    #region 验证

                    if (context.User.Any(c => c.Name == name && c.Id != editId))
                    {
                        //存在
                        MessageBoxX.Show($"存在相同账户名[{name}]", "数据存在");
                        return;
                    }

                    #endregion 

                    #region  编辑状态

                    Model = context.User.Single(c => c.Id == editId);
                    Model.Name = name;
                    Model.Pwd = password;
                    Model.RoleId = roleId;
                    Model.DepartmentId = departmentId;
                    Model.DepartmentPositionId = positionId;
                    Model.RealName = realName;
                    Model.IdCard = idCard;
                    Model.PositionEndTime = (bool)cbUsePositionEndTime.IsChecked
                        ? dtpUsePositionEndTime.SelectedDateTime
                        : DateTime.Now.AddYears(20);
                    Model.NewPositionId = (int)cbNewPosition.SelectedValue;
                    Model.NewPositionStartTime = Model.PositionEndTime;
                    Model.PositionType = cbPositionType.SelectedIndex;

                    #endregion 

                    this.Log("账户编辑成功！");
                }
                else
                {
                    #region 验证

                    if (context.User.Any(c => c.Name == name))
                    {
                        //存在
                        MessageBoxX.Show($"存在相同账户名[{name}]", "数据存在");
                        return;
                    }

                    //获取职位信息
                    var position = context.DepartmentPosition.First(c => c.Id == positionId);
                    int positionUserCount = context.User.Any(c => c.DepartmentPositionId == positionId)
                        ? context.User.Count(c => c.DepartmentPositionId == positionId)
                        : 0;
                    if (position.MaxUserCount <= positionUserCount)
                    {
                        MessageBoxX.Show($"当前职位的定员数已满，不能继续添加", "超过最大数");
                        return;
                    }

                    #endregion 

                    #region  添加状态

                    Model = new User();
                    Model.CanLogin = true;
                    Model.CreateTime = DateTime.Now;
                    Model.Creator = UserGlobal.CurrUser.Id;
                    Model.DelTime = DateTime.Now;
                    Model.DelUser = 0;
                    Model.IsDel = false;
                    Model.Name = name;
                    Model.Pwd = password;
                    Model.RoleId = roleId;
                    Model.DepartmentId = departmentId;
                    Model.DepartmentPositionId = positionId;
                    Model.RealName = realName;
                    Model.IdCard = idCard;
                    Model.PositionEndTime = (bool)cbUsePositionEndTime.IsChecked
                        ? dtpUsePositionEndTime.SelectedDateTime
                        : DateTime.Now.AddYears(20);
                    Model.NewPositionId = (int)cbNewPosition.SelectedValue;
                    Model.NewPositionStartTime = Model.PositionEndTime;
                    Model.PositionType = cbPositionType.SelectedIndex;

                    Model = context.User.Add(Model);

                    #endregion

                    this.Log("账户添加成功！");
                }
                context.SaveChanges();
            }

            btnClose_Click(null, null);//模拟关闭
        }

        #region 到期时间

        private void cbUsePositionEndTime_Checked(object sender, RoutedEventArgs e)
        {
            dtpUsePositionEndTime.IsEnabled = true;
        }

        private void cbUsePositionEndTime_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpUsePositionEndTime.IsEnabled = false;
        }

        #endregion
    }
}
