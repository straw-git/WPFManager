
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
using Common.Entities;
using Common.Data.Local;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MainWindowTagInfo tag = new MainWindowTagInfo();
            tag.CurrUser = UserGlobal.CurrUser;
            tag.Dic = MenuManager.PluginDic;
            this.Tag = tag;

            WindowGlobal.MainWindow = this;
        }


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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTitle();
            LoadMenu();

            lblCurrUser.Text = UserGlobal.CurrUser.Name;
        }



        #region UI Method

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
                Process.Start(Assembly.GetExecutingAssembly().GetName().CodeBase);
                CloseWindow();
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
                if (targetItem.Tag != null && !string.IsNullOrEmpty(targetItem.Tag.ToString()))
                {
                    string url = targetItem.Tag.ToString();
                    mainFrame.Source = new Uri(url, UriKind.RelativeOrAbsolute);
                }
            }
        }

        #endregion

        #region Private Method

        private void LoadMenu()
        {
            tabMenu.Items.Clear();
            List<string> UserMenu = new List<string>();

            if (UserGlobal.CurrUser.Menus.NotEmpty())
                UserMenu = UserGlobal.CurrUser.Menus.Split('|').ToList();

            int currIndex = 0;

            foreach (var plugin in MenuManager.PluginDic)
            {
                var mainMenus = plugin.Value.Keys.OrderBy(c => c.SelfOrder).ToList();
                for (int i = 0; i < mainMenus.Count; i++)
                {
                    var _menu = mainMenus[i];

                    //这里筛选主导航
                    if (UserMenu.Any(c => c.StartsWith($"{plugin.Key}-{_menu.Code}-")) || UserGlobal.CurrUser.Name == "admin")
                    {
                        TabItem _tabItem = new TabItem();
                        _tabItem.Tag = _menu;
                        _tabItem.Name = $"{plugin.Key}0{_menu.Code}";
                        _tabItem.Header = _menu.Name;
                        _tabItem.GotFocus += _tabItem_GotFocus;

                        tabMenu.Items.Add(_tabItem);
                        if (currIndex == 0)
                        {
                            currIndex = 1;
                            _tabItem_GotFocus(_tabItem, null);
                        }
                    }
                }
            }

            tabMenu.SelectedIndex = 0;
        }

        private void _tabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            TabItem currTab = sender as TabItem;

            BaseMenuInfo selectedMenu = currTab.Tag as BaseMenuInfo;
            string name = currTab.Name;
            var arr = name.Split('0');

            tvMenu.Items.Clear();
            List<string> UserMenu = new List<string>();
            if (UserGlobal.CurrUser.Menus.NotEmpty())
                UserMenu = UserGlobal.CurrUser.Menus.Split('|').ToList();

            var childrens = MenuManager.PluginDic[arr[0]][selectedMenu].OrderBy(c => c.Order).ToList();

            int currIndex = 0;
            for (int i = 0; i < childrens.Count; i++)
            {
                var _menu = childrens[i];

                if (UserMenu.Any(c => c == $"{_menu.PluginCode}-{_menu.ParentCode}-{_menu.Code}") || UserGlobal.CurrUser.Name == "admin")
                {
                    TreeViewItem _treeViewItem = new TreeViewItem();
                    _treeViewItem.Header = _menu.Name;
                    _treeViewItem.Margin = new Thickness(0, 2, 0, 2);
                    _treeViewItem.Padding = new Thickness(10, 0, 0, 0);
                    _treeViewItem.Background = Brushes.Transparent;
                    _treeViewItem.Tag = _menu.Url;
                    _treeViewItem.IsSelected = currIndex == 0;

                    tvMenu.Items.Add(_treeViewItem);

                    if (currIndex == 0)
                    {
                        currIndex = 1;
                        mainFrame.Source = new Uri(_menu.Url, UriKind.RelativeOrAbsolute);
                    }
                }
            }
        }

        public void UpdateTitle()
        {
            Title = lblTitle.Text = $"{LocalSettings.settings.MainWindowTitle}(V{LocalSettings.settings.Versions})";
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

        #endregion

    }
}
