using Client.Animation;
using Client.Pages;
using Client.Windows;
using Common.Data.Local;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Xml;

namespace Client
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        Storyboard stdStart;
        public Login()
        {
            InitializeComponent();
            this.UseCloseAnimation();

            stdStart = (Storyboard)this.Resources["start"];
            stdStart.Completed += (a, b) =>
            {
                this.root.Clip = null;
            };
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stdStart.Begin();
            CheckNullData();
        }

        private async void CheckNullData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("..//..//App.config");
            XmlNode root = doc.SelectSingleNode("configuration");
            XmlNode node = root.SelectSingleNode("connectionStrings/add[@name='ZDBConnectionString']");
            XmlElement el = node as XmlElement;
            el.SetAttribute("connectionString", $"Data Source={LocalDB.Model.DataSource};Initial Catalog={LocalDB.Model.InitialCatalog};User ID={LocalDB.Model.UserId};Password={LocalDB.Model.Password};");
            doc.Save("..//..//App.config");

            var handler = PendingBox.Show("正在连接数据库...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });
            IsEnabled = false;
            bool connectionSucceed = false;
            await Task.Run(() =>
            {
                //空数据检查
                connectionSucceed = InitData.NullDataCheck();

                UIGlobal.RunUIAction(()=> 
                {
                    //获取所有系统导航
                    MenuManager.InitMenus();
                });
            });

            if (connectionSucceed)
                handler.UpdateMessage("连接成功");
            await Task.Delay(300);
            handler.Close();

            IsEnabled = true;
            if (!connectionSucceed)
            {
                MessageBoxX.Show("数据库连接失败", "连接错误");
                btnDBSetting_Click(null, null);
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string userPwd = txtPassword.Password.Trim();

            #region 验证

            if (string.IsNullOrEmpty(userName))
            {
                MessageBoxX.Show("账户不能为空", "空值判断");
                txtUserName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(userPwd))
            {
                MessageBoxX.Show("密码不能为空", "空值判断");
                txtPassword.Focus();
                return;
            }

            #endregion 

            var handler = PendingBox.Show("正在登录...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });

            bool loginSucceed = true;//是否登录成功
            string errorStr = "";//错误信息

            #region 登录

            using (var context = new DBContext())
            {
                if (context.User.Any(c => c.Name == userName))
                {
                    bool hasAny = context.User.Any(c => c.Name == userName && c.Pwd == userPwd);
                    if (!hasAny)
                    {
                        errorStr = "密码错误";
                    }
                    else
                    {
                        UserGlobal.CurrUser = context.User.First(c => c.Name == userName && c.Pwd == userPwd);
                        if (UserGlobal.CurrUser == null || UserGlobal.CurrUser.Id <= 0)
                        {
                            errorStr = "未知错误";
                        }
                        if (!UserGlobal.CurrUser.CanLogin)
                        {
                            errorStr = "无权登录";
                        }
                    }
                }
                else
                {
                    errorStr = "账户名不存在";
                }
            }

            #endregion 

            //有错误证明未成功
            loginSucceed = string.IsNullOrEmpty(errorStr);

            if (!loginSucceed)
            {
                Notice.Show($"{errorStr}！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                handler.Close();
                return;
            }
            else
            {
                handler.UpdateMessage("登录成功,正在装载用户数据...");

                await Task.Delay(500);

                handler.Close();

                #region 打开新页面

                this.Visibility = Visibility.Hidden;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();

                #endregion 

                Notice.Show($"{UserGlobal.CurrUser.Name}登录", "欢迎", 3, MessageBoxIcon.Success);
            }
        }

        public void btnDBSetting_Click(object sender, MouseEventArgs e)
        {
            Windows.DBSettingWindow a = new Windows.DBSettingWindow();
            a.ShowDialog();
            if (a.Succeed) 
            {
                CheckNullData();
            }
        }

        public void btnPlugins_Click(object sender, MouseEventArgs e)
        {
            Windows.Plugins a = new Windows.Plugins();
            a.ShowDialog();
        }

        private void btnExit_Click(object sender, MouseEventArgs e)
        {
            this.Close();
        }

    }
}
