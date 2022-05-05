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

        /// <summary>
        /// 登录成功时触发
        /// </summary>
        public Action OnLoginSucceed;
        /// <summary>
        /// 关闭触发
        /// </summary>
        public Action OnLoginClosed;

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
                OnLoginSucceed?.Invoke();
            }
            else
            {
                var handler = PendingBox.Show("正在登录...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
                {
                    LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                    ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
                });

                User userModel = null;

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
                                Panuon.UI.Silver.Notice.Show($"未知错误！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                                handler.Close();
                                return;
                            }
                            else
                            {
                                //继续装载数据
                                handler.UpdateMessage("登录成功,正在装载用户数据...");

                                //清空数据
                                UserGlobal.Plugins.Clear();
                                UserGlobal.PluginsModules.Clear();
                                UserGlobal.ModulePages.Clear();

                                #region 加载权限

                                if (userModel.RoleId == context.Role.First(c => c.Name == "超级管理员").Id)
                                {
                                    #region 超级管理员

                                    UserGlobal.Plugins = context.Plugins.ToList();
                                    UserGlobal.PluginsModules = context.PluginsModule.ToList();
                                    UserGlobal.ModulePages = context.ModulePage.ToList();

                                    #endregion 
                                }
                                else
                                {
                                    //获取角色权限
                                    string rolePluginsStr = context.RolePlugins.Any(c => c.RoleId == userModel.RoleId) ? context.RolePlugins.First(c => c.RoleId == userModel.RoleId).Pages : "";
                                    //获取用户自定义权限
                                    UserPlugins userPlugins = context.UserPlugins.FirstOrDefault(c => c.Id == userModel.Id);
                                    if (userPlugins != null && userPlugins.Id > 0)
                                    {
                                        if (userPlugins.IncreasePages.NotEmpty())
                                        {
                                            //在角色权限基础上的增加页面
                                            string[] increasePages = userPlugins.IncreasePages.Split(',');
                                            foreach (var _iPage in increasePages)
                                            {
                                                int _pageId = 0;
                                                if (int.TryParse(_iPage, out _pageId))
                                                {
                                                    string _iStr = rolePluginsStr.NotEmpty() ? $",{_pageId}" : _pageId.ToString();
                                                    rolePluginsStr += _iStr;//将字符串追加到末尾
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                        if (userPlugins.DecrementPages.NotEmpty())
                                        {
                                            //在角色权限基础上的减少页面
                                            string[] decrementPages = userPlugins.DecrementPages.Split(',');
                                            List<string> _currRoles = rolePluginsStr.Split(',').ToList();//当前的所有角色
                                            bool _currRolesUpdate = false;//当前所有角色是否更新
                                            foreach (var _iPage in decrementPages)
                                            {
                                                if (_iPage.NotEmpty())
                                                {
                                                    if (_currRoles.Contains(_iPage))
                                                    {
                                                        _currRolesUpdate = true;
                                                        _currRoles.Remove(_iPage); //如果有这一项 移除
                                                    }
                                                }
                                                else { continue; }
                                            }
                                            if (_currRolesUpdate)
                                                rolePluginsStr = string.Join(",", _currRoles); //如果有更改，重新整理移除后的字符串
                                        }
                                    }

                                    GetRightByPageStr(context, rolePluginsStr);
                                }

                                #endregion

                                await Task.Delay(500);

                                UserGlobal.SetCurrUser(userModel, context.CoreSetting.First());
                                handler.UpdateMessage("数据装载成功！");
                                await Task.Delay(300);
                                handler.Close();
                                OnLoginSucceed?.Invoke();
                                Panuon.UI.Silver.Notice.Show($"{UserGlobal.CurrUser.Name}登录", "欢迎", 3, MessageBoxIcon.Success);
                            }
                        }
                        else
                        {
                            Panuon.UI.Silver.Notice.Show($"密码错误！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                            handler.Close();
                            return;
                        }
                    }
                    else
                    {
                        Panuon.UI.Silver.Notice.Show($"账户不存在！", "登录失败", 5, Panuon.UI.Silver.MessageBoxIcon.Error);
                        handler.Close();
                        return;
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 根据权限数据设置用户权限
        /// </summary>
        /// <param name="context"></param>
        /// <param name="_str"></param>
        private void GetRightByPageStr(CoreDBContext context, string _str)
        {
            string[] _rps = _str.Split(',');
            foreach (string _pageId in _rps)
            {
                int pageId = 0;
                int.TryParse(_pageId, out pageId);
                if (pageId == 0) continue;

                if (context.ModulePage.Any(c => c.Id == pageId))
                {
                    var pageInfo = context.ModulePage.First(c => c.Id == pageId);//获取页面信息
                    var moduleInfo = context.PluginsModule.First(c => c.Id == pageInfo.ModuleId);//获取模块信息
                    var pluginInfo = context.Plugins.First(c => c.Id == moduleInfo.PluginsId);//获取插件信息

                    if (pageInfo != null && moduleInfo != null && pluginInfo != null)
                    {
                        if (!UserGlobal.Plugins.Contains(pluginInfo))
                        {
                            //将已有插件的连接加入连接管理器
                            if (pluginInfo.ConnectionName.NotEmpty() && pluginInfo.ConnectionString.NotEmpty())
                                DBConnections.Set(pluginInfo.ConnectionName, pluginInfo.ConnectionString);
                            UserGlobal.Plugins.Add(pluginInfo);//添加插件
                        }
                        if (!UserGlobal.PluginsModules.Contains(moduleInfo)) UserGlobal.PluginsModules.Add(moduleInfo);//添加模块
                        if (!UserGlobal.ModulePages.Contains(pageInfo)) UserGlobal.ModulePages.Add(pageInfo);//添加页面
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            OnLoginClosed?.Invoke();
        }
    }
}
