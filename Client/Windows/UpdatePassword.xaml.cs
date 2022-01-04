using Common;
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

namespace Client.Windows
{
    /// <summary>
    /// UpdatePassword.xaml 的交互逻辑
    /// </summary>
    public partial class UpdatePassword : Window
    {
        public UpdatePassword()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtOldPwd.Password = UserGlobal.CurrUser.Pwd;
            txtNewPwd.Clear();
            txtNewPwdRef.Clear();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (txtOldPwd.Password.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入旧密码", "空值提醒");
                txtOldPwd.Focus();
                return;
            }
            if (txtNewPwd.Password.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入新密码", "空值提醒");
                txtNewPwd.Focus();
                return;
            }
            if (txtNewPwdRef.Password.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入重复密码", "空值提醒");
                txtNewPwdRef.Focus();
                return;
            }
            if (txtNewPwd.Password != txtNewPwdRef.Password)
            {
                MessageBoxX.Show("密码不一致", "错误");
                txtNewPwdRef.Focus();
                txtNewPwdRef.SelectAll();
                return;
            }

            if (UserGlobal.CurrUser.Pwd != txtOldPwd.Password)
            {
                MessageBoxX.Show("旧密码错误", "错误");
                txtOldPwd.Focus();
                txtOldPwd.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                context.User.Single(c => c.Id == UserGlobal.CurrUser.Id).Pwd = txtNewPwd.Password;
                UserGlobal.CurrUser.Pwd = txtNewPwd.Password;
                context.SaveChanges();
            }

            MessageBoxX.Show("成功", "成功");
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
