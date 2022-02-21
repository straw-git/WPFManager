using Common;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.MyControls
{
    /// <summary>
    /// LoginBox.xaml 的交互逻辑
    /// </summary>
    public partial class LoginBox : UserControl
    {
        Storyboard showSb;
        Storyboard hideSb;
        public LoginBox()
        {
            InitializeComponent();

            showSb = (Storyboard)this.Resources["showLogin"];
            hideSb= (Storyboard)this.Resources["hiddleLogin"];
            hideSb.Completed += (a, b) =>
            {
                Visibility = Visibility.Collapsed;
            };
        }

        public Action OnLoginFinished;
        public Action OnLocalPluginsClick;

        private void btnPlugins_Click(object sender, MouseButtonEventArgs e)
        {
            OnLocalPluginsClick?.Invoke();
        }

        public void HideLogin()
        {
            hideSb.Begin();
        }

        public void ShowLogin()
        {
            bLogin.Width = 5;
            Visibility = Visibility.Visible;
           
            showSb.Begin();
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

            if (UserGlobal.IsLogin && UserGlobal.CurrUser.Name == userName)
            {
                //如果已经登录并且名字一样

                OnLoginFinished?.Invoke();
            }
            else
            {

                var handler = PendingBox.Show("正在登录...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
                {
                    LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                    ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
                });

                bool loginSucceed = true;//是否登录成功
                string errorStr = "";//错误信息
                CoreDBModels.User userModel = null;

                #region 登录

                using (var context = new CoreDBContext())
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
                            userModel = context.User.First(c => c.Name == userName && c.Pwd == userPwd);
                            if (userModel == null || userModel.Id <= 0)
                            {
                                errorStr = "未知错误";
                            }
                            if (!userModel.CanLogin)
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
                loginSucceed = string.IsNullOrEmpty(errorStr) && userModel != null;

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

                    // 登录成功 
                    //填充用户数据
                    UserGlobal.SetCurrUser(userModel);
                    ////打开账套选择
                    //SelectPlugins selectPlugins = new SelectPlugins();
                    //selectPlugins.ShowPluginsAsync();

                    handler.UpdateMessage("数据装载成功！");
                    await Task.Delay(300);
                    handler.Close();

                    //selectPlugins.Show();

                    OnLoginFinished?.Invoke();
                    Notice.Show($"{UserGlobal.CurrUser.Name}登录", "欢迎", 3, MessageBoxIcon.Success);
                }
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
