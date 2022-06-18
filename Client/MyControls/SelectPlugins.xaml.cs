using Client.CurrGlobal;
using Common;
using Common.Data.Local;
using Common.Utils;
using CoreDBModels;
using Panuon.UI.Silver;
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

            #region 动画

            hideSb = (Storyboard)this.Resources["hiddlePlugins"];
            hideSb.Completed += (a, b) =>
            {
                Visibility = Visibility.Collapsed;
                gPlugins.Children.Clear();
            };
            showSb = (Storyboard)this.Resources["showPlugins"];

            #endregion

            //动画结束
            showSb.Completed += (a, b) =>
            {
                UpdatePluginsAsync();
            };
        }

        /// <summary>
        /// 进入主窗体点击事件
        /// </summary>
        public Action OnGoMainWindowClick;
        /// <summary>
        /// 返回登录页点击事件
        /// </summary>
        public Action OnBackLoginClick;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

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
        /// 更新插件显示
        /// </summary>
        /// <param name="_currWindowName"></param>
        public async void UpdatePluginsAsync()
        {
            gPlugins.Children.Clear();//初始化列表

            //查找所有插件
            List<Plugins> plugins = UserGlobal.Plugins.OrderBy(c => c.Order).ToList();
            string baseUrl = UserGlobal.CoreSetting.APIUrl;//插件的下载更新地址
            if (baseUrl.EndsWith("/")) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            for (int i = 0; i < plugins.Count; i++)
            {
                Plugins p = plugins[i];
                string downLoadUrl = $"{baseUrl}/dlls/{p.Id}/";
                if (p.WebDownload)
                {
                    //从网上下载
                    PluginsDownload.ToPluginsFolder(downLoadUrl, $"{p.DLLName}.dll"); //强制更新插件 无论dll是否存在 都下载过来

                    if (!string.IsNullOrEmpty(p.DLLs))
                    {
                        //将依赖的dll加载过来
                        var dllsArr = p.DLLs.Split('|');
                        foreach (var d in dllsArr)
                        {
                            if (d.IsNullOrEmpty()) continue;
                            string dllName = d;
                            PluginsDownload.ToPluginsFolder(downLoadUrl, dllName);
                        }
                    }
                }
                else
                {
                    //不从网上下载 并且目录中不存在目标dll 
                    if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{p.DLLName}.dll"))
                    {
                        continue;//不存在文件 不加载
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(p.DLLs))
                        {
                            //将依赖的dll加载过来
                            var dllsArr = p.DLLs.Split('|');
                            foreach (var d in dllsArr)
                            {
                                if (d.IsNullOrEmpty()) continue;
                                string dllName = d;
                                //默认的格式是dll
                                if (d.IndexOf(".dll") == -1) dllName = $"{d}.dll";
                                if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{dllName}"))
                                {
                                    Notice.Show($"{p.Name}插件加载失败,未找到[{dllName}]该模块功能可能会引起功能出错！", "插件加载失败", 5, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }

                PluginsBox pluginsLogo = new PluginsBox();
                pluginsLogo.Margin = new Thickness(5);
                pluginsLogo.LogoContent = p.Name;
                Uri uri = null;

                #region 插件信息加载异常

                try
                {
                    uri = new Uri($"pack://application:,,,/{p.DLLName};component/{p.LogoImage}");
                }
                catch 
                {
                    Notice.Show($"{p.Name}插件加载失败,未找到插件必须程序,已略过！", "插件加载失败", 5, MessageBoxIcon.Error);
                    continue;
                }

                #endregion

                pluginsLogo.ImageBack = new BitmapImage(uri);
                pluginsLogo.PluginsData = p;

                if (MainWindowGlobal.CurrPlugins.Any(c => c.Id == p.Id))
                {
                    //这个插件当前已经存在并且被使用的话
                    pluginsLogo.Check();
                    //有选择的启用底部导航
                    btnAdd2MainWindow.Visibility = Visibility.Visible;
                }

                pluginsLogo.CheckChanged += OnPluginCheckChanged;
                gPlugins.Children.Add(pluginsLogo);
            }

            await Task.Delay(200);
            gLoading.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///  选择更改触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="isChecked"></param>
        private void OnPluginCheckChanged(PluginsBox sender, bool isChecked)
        {
            if (isChecked)
            {
                if (!MainWindowGlobal.CurrPlugins.Contains(sender.PluginsData))
                {
                    //选中 加入到其中
                    MainWindowGlobal.CurrPlugins.Add(sender.PluginsData);
                    MainWindowGlobal.CurrPluginsModules.AddRange(PluginsModules.Where(c => c.PluginsId == sender.PluginsData.Id).ToList());
                    MainWindowGlobal.CurrModulePages.AddRange(ModulePages.Where(c => c.PluginsId == sender.PluginsData.Id).ToList());
                }
            }
            else
            {
                if (MainWindowGlobal.CurrPlugins.Contains(sender.PluginsData))
                {
                    MainWindowGlobal.CurrPlugins.Remove(sender.PluginsData);
                    MainWindowGlobal.CurrPluginsModules.RemoveAll(c => c.PluginsId == sender.PluginsData.Id);
                    MainWindowGlobal.CurrModulePages.RemoveAll(c => c.PluginsId == sender.PluginsData.Id);
                }
            }
            //空值底部导航
            btnAdd2MainWindow.Visibility = MainWindowGlobal.CurrPlugins.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void btnAdd2MainWindow_Click(object sender, RoutedEventArgs e)
        {
            OnGoMainWindowClick?.Invoke();
        }

        private void BtnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            if (gPlugins.Children.Count > 0)
            {
                foreach (var item in gPlugins.Children)
                {
                    PluginsBox pluginsBox = item as PluginsBox;
                    pluginsBox.Check();//模拟选中
                }
            }
        }
    }
}
