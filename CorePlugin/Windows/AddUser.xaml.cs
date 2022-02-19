
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
        public bool isEdit = false;
        int userId = 0;

        public AddUser(int _userId = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            userId = _userId;
            if (_userId == 0)
            {
                isEdit = false;
                lblRole.Content = "当前未选择角色";
                lblRole.Tag = 0;
                new RoleTreeViewCommon(tvRole).Init(false);
            }
            else
            {
                isEdit = true;

                using (CoreDBContext context = new CoreDBContext())
                {
                    Model = context.User.First(c => c.Id == _userId);
                    var role = context.SysDic.First(c => c.Id == Model.RoleId);

                    lblRole.Content = role.Name;
                    lblRole.Tag = role.Id;

                    txtAdminName.Text = Model.Name;
                    txtAdminPwd.Password = Model.Pwd;
                    txtReAdminPwd.Password = Model.Pwd;

                    cbCanLogin.IsChecked = Model.CanLogin;

                    new RoleTreeViewCommon(tvRole).Init(false, false, Model.RoleId);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int roleId = lblRole.Tag.ToString().AsInt();
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
                if (isEdit)
                {
                    if (context.User.Any(c => c.Name == txtAdminName.Text && c.Id != userId))
                    {
                        MessageBoxX.Show("账户名已存在", "数据异常");
                        txtAdminName.Focus();
                        txtAdminName.SelectAll();
                        return;
                    }
                    //编辑
                    Model = context.User.Single(c => c.Id == userId);
                    Model.CanLogin = (bool)cbCanLogin.IsChecked;
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
                    Model.StaffId = "";
                    Model.CanLogin = (bool)cbCanLogin.IsChecked;
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

        private void tvRole_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem selectedItem = tvRole.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                if (selectedItem.Items.Count > 0) return;

                int id = selectedItem.Tag.ToString().AsInt();
                if (id > 0)
                {
                    lblRole.Content = selectedItem.Header;
                    lblRole.Tag = id;
                }
            }
        }

        #endregion

    }
}
