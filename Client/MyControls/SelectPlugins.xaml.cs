using Client.CurrGlobal;
using Common;
using Common.Data.Local;
using CoreDBModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using static Common.UserGlobal;

namespace Client.MyControls
{
    /// <summary>
    /// SelectPlugins.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlugins : UserControl
    {

        Storyboard hideSb;
        Storyboard showSb;
        public SelectPlugins()
        {
            InitializeComponent();

            hideSb = (Storyboard)this.Resources["hiddlePlugins"];
            hideSb.Completed += (a, b) =>
            {
                Visibility = Visibility.Collapsed;
                gPlugins.Children.Clear();
            };
            showSb = (Storyboard)this.Resources["showPlugins"];
            showSb.Completed += (a, b) =>
            {
                UpdatePluginsAsync();
            };
        }
        /// <summary>
        /// 当前操作的窗体名称
        /// </summary>
        public string CurrWindowName = "";
        List<BasePlugins> CurrFocusModels = new List<BasePlugins>();

        public Action AddNewWindow;
        public Action JoinWindow;
        public Action OnBackLoginClick;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            bMenus.Visibility = Visibility.Collapsed;
        }

        private void btnBackLogin_Click(object sender, RoutedEventArgs e)
        {
            OnBackLoginClick?.Invoke();
        }
        public void HidePlugins()
        {
            hideSb.Begin();
        }

        public void ShowPlugins()
        {
            gLoading.Visibility = Visibility.Visible;
            bPlugins.Width = 5;
            Visibility = Visibility.Visible;

            showSb.Begin();
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="_currWindowName"></param>
        public async void UpdatePluginsAsync(string _currWindowName = "")
        {
            await Task.Delay(200);
            CurrWindowName = _currWindowName;//更新调用窗体名称
            gPlugins.Children.Clear();//初始化列表
            UpdatePluginsButtons();//检查一下按钮显示隐藏

            List<BasePlugins> pluginsModels = new List<BasePlugins>();//获取所有插件

            if (IsLogin && CurrUser.Name == "admin")
            {
                #region 如果是初始的超级管理员账户 遍历插件目录下的所有dll文件

                //创建一个DirectoryInfo的类
                DirectoryInfo directoryInfo = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}{CheckPluginsDLL.PluginFolderName}\\");
                //获取当前的目录的文件
                FileInfo[] fileInfos = directoryInfo.GetFiles();

                foreach (FileInfo info in fileInfos)
                {
                    //获取文件的名称(包括扩展名)
                    //string fullName = info.FullName;
                    //获取文件的扩展名
                    string extension = info.Extension.ToLower();
                    if (extension == ".dll")
                    {
                        string pluginsName = info.Name.Substring(0, info.Name.LastIndexOf('.'));
                        //查看dll中的模块
                        var _currDLLPluginsModel = CheckPluginsDLL.GetPluginsModel(pluginsName,0);
                        if (_currDLLPluginsModel != null)
                        {
                            pluginsModels.Add(_currDLLPluginsModel);
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region 其它用户 根据权限加载遍历

                foreach (var plugins in CanUsePlugins)
                {
                    //查看dll中的模块
                    var _currDLLPluginsModel = CheckPluginsDLL.GetPluginsModel(plugins.DLLName,plugins.Id);
                    if (_currDLLPluginsModel != null)
                    {
                        pluginsModels.Add(_currDLLPluginsModel);
                    }
                }

                #endregion
            }

            pluginsModels = pluginsModels.OrderBy(c => c.Order).ToList();//排序

            #region 显示列表

            foreach (var p in pluginsModels)
            {
                LogoBox pluginsLogo = new LogoBox();
                pluginsLogo.Margin = new Thickness(5);
                pluginsLogo.LogoContent = p.PluginsTitle;
                pluginsLogo.ImageBack = new BitmapImage(new Uri($"pack://application:,,,/{p.PluginsDLLName};component/{p.logo}")); ;
                pluginsLogo.PluginsData = p;
                pluginsLogo.CheckChanged += OnPluginCheckChanged;
                gPlugins.Children.Add(pluginsLogo);
            }

            #endregion

            await Task.Delay(200);

            gLoading.Visibility = Visibility.Collapsed;
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

        private void btnNewWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindowsGlobal.Data2MainWindow(Guid.NewGuid().ToString(), CurrFocusModels);
            AddNewWindow?.Invoke();
        }

        private void btnAddCurrWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindowsGlobal.Data2MainWindow(CurrWindowName, CurrFocusModels);
            JoinWindow?.Invoke();
        }
    }
}
