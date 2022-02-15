using Common;
using Common.Data.Local;
using Common.Utils;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Common.UserGlobal;

namespace SalePlugin
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //初始化插件
            LocalPlugins.Init();
            //加载所有皮肤
            LocalSkin.Init();
            //加载用户设置
            LocalSettings.Init();
            //初始化数据库连接数据
            LocalDB.Init();
            //服务端设置
            LocalServer.Init();
            //初始化样式
            StyleHelper.Init();

            using (CoreDBContext context = new CoreDBContext())
            {
                UserGlobal.CurrUser = context.User.First(c => c.Name == "admin");
            }

            AddPluginModels(new List<PluginsModel>() { GetPluginsModel(0) });
        }


        #region override BaseMainWindow

        public override void ShowLeftMenu(bool _show)
        {
            if (_show)
            {
                bSecondMenu.Width = 200;
            }
            else
            {
                bSecondMenu.Width = 0;
            }
        }

        public override void ShowTopMenu(bool _show)
        {
            if (_show)
            {
                gMainMenu.Height = 55;
            }
            else
            {
                gMainMenu.Height = 0;
            }
        }

        public override void ReLoadCurrTopMenu()
        {
            _tabItem_GotFocus(tabMenu.SelectedItem, null);
        }

        public override void SetFrameSource(string _s)
        {
            mainFrame.Source = new Uri(_s, UriKind.RelativeOrAbsolute);
        }

        public override void UpdateMenus()
        {
            tabMenu.Items.Clear();

            int currIndex = 0;
            foreach (var plugin in CurrWindowPlugins)
            {
                foreach (var modules in plugin.Modules)
                {
                    TabItem _tabItem = new TabItem();
                    _tabItem.Tag = modules;
                    _tabItem.Header = modules.Name;
                    _tabItem.GotFocus += _tabItem_GotFocus;

                    tabMenu.Items.Add(_tabItem);
                    if (currIndex == 0)
                    {
                        currIndex = 1;
                        _tabItem_GotFocus(_tabItem, null);
                    }
                }
            }

            if (CurrWindowPlugins.Count == 1 && CurrWindowPlugins[0].Modules.Count == 1)
            {
                //如果只有一个模块 隐藏上部导航
                ShowTopMenu(false);
            }
            else
            {
                ShowTopMenu(true);
            }

            tabMenu.SelectedIndex = 0;
        }

        private void _tabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            TabItem currTab = sender as TabItem;

            ModuleModel selectedMenu = currTab.Tag as ModuleModel;
            tvMenu.Items.Clear();
            var _pages = selectedMenu.Pages.OrderBy(c => c.Order).ToList();//页面排序

            int currIndex = 0;
            foreach (var page in _pages)
            {
                TreeViewItem _treeViewItem = new TreeViewItem();
                _treeViewItem.Header = page.Code;
                _treeViewItem.Margin = new Thickness(0, 2, 0, 2);
                _treeViewItem.Padding = new Thickness(10, 0, 0, 0);
                _treeViewItem.Background = Brushes.Transparent;
                _treeViewItem.Tag = page;
                _treeViewItem.IsSelected = currIndex == 0;

                tvMenu.Items.Add(_treeViewItem);

                if (currIndex == 0)
                {
                    currIndex = 1;
                    mainFrame.Source = new Uri(page.Url, UriKind.RelativeOrAbsolute);
                }
            }
        }

        #endregion 


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTitle();

            lblCurrUser.Text = UserGlobal.CurrUser.Name;
        }

        #region UI Method

        private void btnChangePwd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReLogin_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show("是否注销本次登录", "注销提醒", null, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(Assembly.GetExecutingAssembly().GetName().CodeBase);
                CloseWindow();
            }
        }

        private void btnSkin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSignout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show("是否退出应用", "退出提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(Assembly.GetExecutingAssembly().GetName().CodeBase);
                CloseWindow();
            }
        }

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var result = MessageBoxX.Show("是否退出应用", "退出提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CloseWindow();
            }
        }

        private void TreeView_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tvMenu.SelectedItem != null)
            {
                TreeViewItem targetItem = tvMenu.SelectedItem as TreeViewItem;
                PageModel page = targetItem.Tag as PageModel;
                mainFrame.Source = new Uri(page.Url, UriKind.RelativeOrAbsolute);
            }
        }

        #endregion

        #region Private Method

        public void UpdateTitle()
        {
            Title = lblTitle.Text = $"{LocalSettings.settings.MainWindowTitle}(V{LocalSettings.settings.Versions})";
            lblV.Content = $"by 1020    V{LocalSettings.settings.Versions}";
        }

        private void CloseWindow()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation dh = new DoubleAnimation();
            DoubleAnimation dw = new DoubleAnimation();
            dh.Duration = dw.Duration = sb.Duration = new Duration(new TimeSpan(0, 0, 1));
            dh.To = dw.To = 0;
            Storyboard.SetTarget(dh, this);
            Storyboard.SetTarget(dw, this);
            Storyboard.SetTargetProperty(dh, new PropertyPath("Height", new object[] { }));
            Storyboard.SetTargetProperty(dw, new PropertyPath("Width", new object[] { }));
            sb.Children.Add(dh);
            sb.Children.Add(dw);
            sb.Completed += AnimationCompleted;
            sb.Begin();
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 查找dll中的Page所在的空间
        /// </summary>
        /// <param name="_dllOrder">dll排序</param>
        public static PluginsModel GetPluginsModel(int _dllOrder)
        {
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            var pluginsInfo = new PluginsInfo();
            PluginsModel pluginsModel = new PluginsModel();
            pluginsModel.Code = pluginsInfo.dllName;
            pluginsModel.Name = pluginsInfo.pluginsCode;
            pluginsModel.Order = _dllOrder;
            pluginsModel.LogoImageSource = new BitmapImage(new Uri($"pack://application:,,,/{pluginsInfo.DLLName};component/{pluginsInfo.LogoImageName}"));
            pluginsModel.Modules = new List<ModuleModel>();

            //根据插件说明中的命名空间查找页面
            string[] pfs = pluginsInfo.PageFolderNames.Split(',');
            foreach (string pf in pfs)
            {
                if (string.IsNullOrEmpty(pf)) continue;

                //查找到的页面
                string _pfNames = $"{pluginsInfo.dllName}.{pf}";

                //此处挑选出带有MenuClassName的命名空间（文件夹）
                var _moduleFolders = (from t in currAssembly.GetTypes()
                                      where t.IsClass
                                      && t.Namespace != null
                                      && t.Namespace.StartsWith(_pfNames)
                                      && t.FullName.Contains(pluginsInfo.MenuClassName)
                                      && !t.FullName.Contains("<")
                                      && !t.FullName.Contains(">")
                                      && !t.FullName.Contains("+")
                                      select t.FullName).ToList();

                foreach (var _moduleFolder in _moduleFolders)
                {
                    BaseMenuInfo menuInfo = null;
                    //获取MenuIfo
                    menuInfo = (BaseMenuInfo)Activator.CreateInstance(Type.GetType(_moduleFolder));

                    if (menuInfo == null) continue;

                    ModuleModel moduleModel = new ModuleModel();
                    moduleModel.Code = menuInfo.Code;
                    moduleModel.Name = menuInfo.Name;
                    moduleModel.Order = menuInfo.SelfOrder;
                    moduleModel.Pages = new List<PageModel>();

                    //获取命名空间
                    string _moduleFolderNsp = _moduleFolder.Substring(0, _moduleFolder.LastIndexOf('.'));
                    //查找模块下的所有页面
                    var _currPages = (from t in currAssembly.GetTypes()
                                      where t.IsClass
                                      && t.Namespace != null
                                      && t.Namespace.StartsWith(_moduleFolderNsp)
                                      && !t.FullName.Contains(pluginsInfo.MenuClassName)
                                      && !t.FullName.Contains("<")
                                      && !t.FullName.Contains(">")
                                      && !t.FullName.Contains("+")
                                      select t.FullName).ToList();

                    try
                    {
                        foreach (var _currPage in _currPages)
                        {
                            #region 获取页面说明

                            Type itemObj = Type.GetType(_currPage);
                            BasePage itemPage = (BasePage)Activator.CreateInstance(itemObj);
                            if (!itemPage.IsMenu) continue;//不是导航 排除

                            itemPage.Code = $"{_currPage.Substring(_currPage.LastIndexOf('.') + 1)}";

                            #endregion

                            PageModel pageModel = new PageModel();
                            pageModel.Code = itemPage.Title;
                            pageModel.Order = itemPage.Order;
                            pageModel.Url = $"/{pf}/{moduleModel.Code}/{itemPage.Code}.xaml";

                            moduleModel.Pages.Add(pageModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    pluginsModel.Modules.Add(moduleModel);
                }
            }

            return pluginsModel;
        }


        #endregion

    }
}
