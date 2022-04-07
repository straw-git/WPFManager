
using Common;
using Common.Utils;
using CoreDBModels;
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRolesComobox();
            if (IsEdit)
            {
                InitUserInfo();
            }
        }

        /// <summary>
        /// 加载角色下拉
        /// </summary>
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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtAdminName.NotEmpty()) return;
            string name = txtAdminName.Text;
            if (txtAdminPwd.Password != txtReAdminPwd.Password)
            {
                MessageBoxX.Show("两次密码输入不一致", "密码验证错误");
                return;
            }
            string password = txtAdminPwd.Password;
            int roleId = cbRoles.SelectedValue.ToString().AsInt();//角色Id
            using (CoreDBContext context = new CoreDBContext())
            {
                if (IsEdit)
                {
                    #region  编辑状态
                    if (context.User.Any(c => c.Name == name && c.Id != editId))
                    {
                        //存在
                        MessageBoxX.Show($"存在相同账户名[{name}]", "数据存在");
                        return;
                    }

                    Model = context.User.Single(c => c.Id == editId);
                    Model.Name = name;
                    Model.Pwd = password;
                    Model.RoleId = roleId;

                    #endregion 
                    this.Log("账户编辑成功！");
                }
                else
                {
                    #region  添加状态
                    if (context.User.Any(c => c.Name == name))
                    {
                        //存在
                        MessageBoxX.Show($"存在相同账户名[{name}]", "数据存在");
                        return;
                    }

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

                    Model = context.User.Add(Model);
                    #endregion
                    this.Log("账户添加成功！");
                }
                context.SaveChanges();
            }
            btnClose_Click(null, null);//模拟关闭
        }
    }
}
