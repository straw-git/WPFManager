using Common;
using CoreDBModels;
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
            hideSb = (Storyboard)this.Resources["hiddleLogin"];
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

                CoreDBModels.User userModel = null;

                #region 登录

                using (var context = new CoreDBContext())
                {
                    if (context.User.Any(c => c.Name == userName))
                    {
                        if (context.User.Any(c => c.Name == userName && c.Pwd == userPwd))
                        {
                            //登录成功
                            userModel = context.User.First(c => c.Name == userName && c.Pwd == userPwd && c.CanLogin);
                            if (userModel == null || userModel.Id <= 0)
                            {
                                Notice.Show($"未知错误！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                                handler.Close();
                                return;
                            }
                            else
                            {
                                //继续装载数据
                                handler.UpdateMessage("登录成功,正在装载用户数据...");
                                string userPluginsStr = context.UserPlugins.Any(c => c.UserId == userModel.Id) ? context.UserPlugins.First(c => c.UserId == userModel.Id).Pages : "";
                                string rolePluginsStr = context.RolePlugins.Any(c => c.RoleId == userModel.RoleId) ? context.RolePlugins.First(c => c.RoleId == userModel.RoleId).Pages : "";
                                List<PluginsPage> pages = new List<PluginsPage>();
                                List<Plugins> canUsePlugins = new List<Plugins>();//允许使用的插件信息

                                //查找用户页面
                                switch (userModel.LoadPluginsType)
                                {
                                    case 0:
                                        #region Role 按角色方式加载
                                        string[] _rps = rolePluginsStr.Split(',');
                                        foreach (var pId in _rps)
                                        {
                                            int pluginsId = 0;
                                            int.TryParse(pId, out pluginsId);
                                            if (pluginsId == 0) continue;

                                            if (context.PluginsPage.Any(c => c.Id == pluginsId))
                                            {
                                                var pp = context.PluginsPage.First(c => c.Id == pluginsId);
                                                if (!canUsePlugins.Any(c => c.Id == pp.PluginsId))
                                                {
                                                    //不存在当前插件 将当前插件加入到允许使用的插件列表
                                                    var p = context.Plugins.First(c => c.Id == pp.PluginsId);
                                                    canUsePlugins.Add(p);
                                                }
                                                pages.Add(pp);
                                            }
                                        }
                                        #endregion 
                                        break;
                                    case 1:
                                        #region User 按用户方式加载
                                        string[] _ups = rolePluginsStr.Split(',');
                                        foreach (var pId in _ups)
                                        {
                                            int pluginsId = 0;
                                            int.TryParse(pId, out pluginsId);
                                            if (pluginsId == 0) continue;

                                            if (context.PluginsPage.Any(c => c.Id == pluginsId))
                                            {
                                                var pp = context.PluginsPage.First(c => c.Id == pluginsId);
                                                if (!canUsePlugins.Any(c => c.Id == pp.PluginsId))
                                                {
                                                    //不存在当前插件 将当前插件加入到允许使用的插件列表
                                                    var p = context.Plugins.First(c => c.Id == pp.PluginsId);
                                                    canUsePlugins.Add(p);
                                                }
                                                pages.Add(pp);
                                            }
                                        }
                                        #endregion
                                        break;
                                    case 2:
                                        #region  All 覆盖用户与角色权限
                                        List<string> _rpsList = rolePluginsStr.Split(',').ToList();
                                        List<string> _upsList = rolePluginsStr.Split(',').ToList();

                                        foreach (var pId in _rpsList)
                                        {
                                            if (!string.IsNullOrEmpty(pId) && _upsList.Contains(pId))
                                            {
                                                _upsList.Remove(pId);
                                            }
                                            int pluginsId = 0;
                                            int.TryParse(pId, out pluginsId);
                                            if (pluginsId == 0) continue;

                                            if (context.PluginsPage.Any(c => c.Id == pluginsId))
                                            {
                                                var pp = context.PluginsPage.First(c => c.Id == pluginsId);
                                                if (!canUsePlugins.Any(c => c.Id == pp.PluginsId))
                                                {
                                                    //不存在当前插件 将当前插件加入到允许使用的插件列表
                                                    var p = context.Plugins.First(c => c.Id == pp.PluginsId);
                                                    canUsePlugins.Add(p);
                                                }
                                                pages.Add(pp);
                                            }
                                        }
                                        foreach (var pId in _upsList)
                                        {
                                            int pluginsId = 0;
                                            int.TryParse(pId, out pluginsId);
                                            if (pluginsId == 0) continue;

                                            if (context.PluginsPage.Any(c => c.Id == pluginsId))
                                            {
                                                var pp = context.PluginsPage.First(c => c.Id == pluginsId);
                                                if (!canUsePlugins.Any(c => c.Id == pp.PluginsId))
                                                {
                                                    //不存在当前插件 将当前插件加入到允许使用的插件列表
                                                    var p = context.Plugins.First(c => c.Id == pp.PluginsId);
                                                    canUsePlugins.Add(p);
                                                }
                                                pages.Add(pp);
                                            }
                                        }
                                        #endregion 
                                        break;
                                    default:
                                        throw new Exception();//没有处理的加载插件形式 抛出异常
                                }

                                await Task.Delay(500);
                                UserGlobal.SetCurrUser(userModel, canUsePlugins, pages);
                                handler.UpdateMessage("数据装载成功！");
                                await Task.Delay(300);
                                handler.Close();
                                OnLoginFinished?.Invoke();
                                Notice.Show($"{UserGlobal.CurrUser.Name}登录", "欢迎", 3, MessageBoxIcon.Success);
                            }
                        }
                        else
                        {
                            Notice.Show($"密码错误！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                            handler.Close();
                            return;
                        }
                    }
                    else
                    {
                        Notice.Show($"账户不存在！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                        handler.Close();
                        return;
                    }
                }
                #endregion
            }
        }
    }
}
