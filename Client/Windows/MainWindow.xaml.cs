
using DBModels.Sys;
using Client.Events;
using Client.Helper;
using Client.Pages;
using Client.Windows;
using Common;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Common.Data.Local;
using Client.CurrGlobal;
using static Common.UserGlobal;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseMainWindow
    {
        public MainWindow(string _currWindowName)
        {
            InitializeComponent();
            CurrWindowName = _currWindowName;

            Closing += WindowX_Closing;
        }

        /// <summary>
        /// 当前窗体名称（用于MainWindowsGlobal.cs 控制窗体）
        /// </summary>
        public string CurrWindowName = "";

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
            UpdateMenus();

            lblCurrUser.Text = UserGlobal.CurrUser.Name;
        }

        public void UpdateTitle()
        {
            Title = lblTitle.Text = $"{LocalSettings.settings.MainWindowTitle}(V{LocalSettings.settings.Versions})";
        }

        #region UI Method

        private void btnSelectedPlugins_Click(object sender, RoutedEventArgs e)
        {
            SelectPlugins selectPlugins = new SelectPlugins();
            selectPlugins.ShowPlugins(CurrWindowName);
            selectPlugins.Show();
        }

        private void btnChangePwd_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            Windows.UpdatePassword updatePassword = new Windows.UpdatePassword();
            updatePassword.ShowDialog();
            IsMaskVisible = false;
        }

        private void btnReLogin_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show("是否注销本次登录", "注销提醒", null, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MainWindowsGlobal.MainWindowsDic.Clear();
                Login login = new Login();
                login.Show();
                Close();
            }
        }

        private void btnSkin_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            Windows.SkinWindow skinWindow = new Windows.SkinWindow();
            skinWindow.ShowDialog();
            IsMaskVisible = false;
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            Windows.SettingWindow settingWindow = new Windows.SettingWindow();
            settingWindow.ShowDialog();
            IsMaskVisible = false;
        }

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var result = MessageBoxX.Show("是否退出当前账套", "账套关闭提醒", this, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Closing -= WindowX_Closing;
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

        #region 完全关闭窗体

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
            MainWindowsGlobal.MainWindowsDic.Remove(CurrWindowName);//将自己移除
            if (MainWindowsGlobal.MainWindowsDic.Keys.Count > 0)
                Close();
            else Application.Current.Shutdown();
        }

        #endregion

    }
}
