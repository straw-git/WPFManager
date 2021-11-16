
using Common.Data.Local;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// DBSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DBSettingWindow : WindowX
    {
        public DBSettingWindow()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        public bool Succeed = false;

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string dataSource = txtDataSource.Text.Trim();
            string dbName = txtDatabaseName.Text.Trim();
            string userId = txtUserId.Text.Trim();
            string password = txtPassword.Text.Trim();

            string serverIp = txtServerIp.Text.Trim();
            string serverPort = txtServerPort.Text.Trim();


            if (string.IsNullOrEmpty(dataSource))
            {
                tab.SelectedIndex = 0;
                txtDataSource.Focus();
                MessageBoxX.Show("请输入数据源", "提示", Application.Current.MainWindow);
                return;
            }
            if (string.IsNullOrEmpty(dbName))
            {
                tab.SelectedIndex = 0;
                txtDatabaseName.Focus();
                MessageBoxX.Show("请输入数据库名称", "提示", Application.Current.MainWindow);
                return;
            }
            if (string.IsNullOrEmpty(userId))
            {
                tab.SelectedIndex = 0;
                txtUserId.Focus();
                MessageBoxX.Show("请输入账号", "提示", Application.Current.MainWindow);
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                tab.SelectedIndex = 0;
                txtPassword.Focus();
                MessageBoxX.Show("请输入密码", "提示", Application.Current.MainWindow);
                return;
            }

            //if (string.IsNullOrEmpty(serverIp))
            //{
            //    tab.SelectedIndex = 1;
            //    txtServerIp.Focus();
            //    MessageBoxX.Show("请输入服务端IP", "提示", Application.Current.MainWindow);
            //    return;
            //}
            //if (string.IsNullOrEmpty(serverPort))
            //{
            //    tab.SelectedIndex = 1;
            //    txtServerPort.Focus();
            //    MessageBoxX.Show("请输入服务端端口", "提示", Application.Current.MainWindow);
            //    return;
            //}

            LocalDB.Model.DataSource = txtDataSource.Text;
            LocalDB.Model.InitialCatalog = txtDatabaseName.Text;
            LocalDB.Model.UserId = txtUserId.Text;
            LocalDB.Model.Password = txtPassword.Text;
            LocalDB.Save();
            Succeed = true;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            txtDataSource.Text = LocalDB.Model.DataSource;
            txtDatabaseName.Text = LocalDB.Model.InitialCatalog;
            txtUserId.Text = LocalDB.Model.UserId;
            txtPassword.Text = LocalDB.Model.Password;
        }
    }
}
