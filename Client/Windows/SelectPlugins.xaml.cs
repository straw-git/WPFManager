using Client.CurrGlobal;
using Client.Pages;
using Common;
using Common.Data.Local;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using static Common.UserGlobal;

namespace Client.Windows
{
    /// <summary>
    /// SelectPlugins.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlugins : Window
    {
        public SelectPlugins()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        /// <summary>
        /// 当前选中的账套
        /// </summary>
        List<PluginsModel> CurrFocusModels = new List<PluginsModel>();
        /// <summary>
        /// 当前操作的窗体名称
        /// </summary>
        public string CurrWindowName = "";

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            bMenus.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="_currWindowName"></param>
        public async void ShowPluginsAsync(string _currWindowName = "")
        {
            CurrFocusModels.Clear();//清空临时账套
            CurrWindowName = _currWindowName;//更新调用窗体名称
            Visibility = Visibility.Visible;//显示
            gPlugins.Children.Clear();//初始化列表
            UpdatePluginsButtons();//检查一下按钮显示隐藏


            List<PluginsModel> pluginsModels = new List<PluginsModel>();

            await Task.Run(() =>
            {

                //添加本地允许的dll
                var _localPlugins = LocalPlugins.Models.OrderBy(c => c.Order).ToList();

                UIGlobal.RunUIAction(()=> 
                {
                    foreach (var m in _localPlugins)
                    {
                        //查看dll中的模块
                        var _currDLLPluginsModel = CheckPluginsDLL.GetPluginsModel(m.DLLPageName, m.Order);
                        if (_currDLLPluginsModel != null)
                        {
                            pluginsModels.Add(_currDLLPluginsModel);
                        }
                    }
                });

                pluginsModels = pluginsModels.OrderBy(c => c.Order).ToList();//排序

                #region 将当前页面中的账套排除

                if (_currWindowName.NotEmpty())
                {
                    var currWindowPlugins = MainWindowsGlobal.MainWindowsDic[_currWindowName].CurrWindowPlugins;
                    foreach (var p in currWindowPlugins)
                    {
                        //已经选择的 不在列表中显示
                        pluginsModels.Remove(pluginsModels.First(c => c.Code == p.Code));
                    }
                }

                #endregion

            });

            #region 显示列表

            foreach (var p in pluginsModels)
            {
                LogoBox pluginsLogo = new LogoBox();
                pluginsLogo.Margin = new Thickness(5);
                pluginsLogo.LogoContent = p.Name;
                pluginsLogo.ImageBack = p.LogoImageSource;
                pluginsLogo.PluginsData = p;
                pluginsLogo.CheckChanged += OnPluginCheckChanged;
                gPlugins.Children.Add(pluginsLogo);
            }

            #endregion
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void HidePlugins()
        {
            Visibility = Visibility.Collapsed;
            gPlugins.Children.Clear();
        }

        /// <summary>
        ///  选择更改触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="isChecked"></param>
        private void OnPluginCheckChanged(LogoBox sender, bool isChecked)
        {
            if (isChecked)
            {
                //选中 加入到其中
                CurrFocusModels.Add(sender.PluginsData);
            }
            else
            {
                //不选中 移除
                CurrFocusModels.Remove(sender.PluginsData);
            }
            //空值底部导航
            bMenus.Visibility = CurrFocusModels.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            UpdatePluginsButtons();
        }

        /// <summary>
        /// 更新操作按钮的显示
        /// </summary>
        private void UpdatePluginsButtons()
        {
            if (MainWindowsGlobal.MainWindowsDic.Keys.Count == 0)
            {
                btnAddCurrWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnAddCurrWindow.Visibility = Visibility.Visible;
            }
        }

        private void bTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnNewWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindowsGlobal.Data2MainWindow(Guid.NewGuid().ToString(), CurrFocusModels);
            Close();
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowsGlobal.MainWindowsDic.Keys.Count > 0)
            {
                Close();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void btnAddCurrWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindowsGlobal.Data2MainWindow(CurrWindowName, CurrFocusModels);
            Close();
        }
    }
}
