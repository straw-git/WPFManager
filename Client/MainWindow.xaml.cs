
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
using CoreDBModels;
using Common.Utils;
using System.Threading;
using Client.Events;
using Common.Events;
using Client.Timers;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseMainWindow
    {
        /// <summary>
        /// 窗体是否加载完成
        /// </summary>
        bool windowLoaded = false;

        double windowWidth = 0;//窗体宽度 用于窗体大小发生改变
        double windowHeight = 0;//窗体高度  用于窗体大小发生改变

        public MainWindow()
        {
            InitializeComponent();
            Closing += WindowX_Closing;

            hideSecondMenusAnimation.Completed += HideSecondMenusAnimation_Completed;
            showSecondMenusAnimation.Completed += ShowSecondMenusAnimation_Completed;

            emails.OnClosing += OnEmailClosing;

            #region 初始化窗体宽高

            windowWidth = LocalSettings.settings.WindowWidth;
            windowHeight = LocalSettings.settings.WindowHeight;

            //设置窗体宽高
            Width = windowWidth;
            Height = windowHeight;

            #endregion 
        }

        #region override BaseMainWindow

        /// <summary>
        /// 在窗体底部显示文字
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="_color"></param>
        public override void WriteInfoOnBottom(string _info, string _color = "#000000")
        {
            lblInfo.Content = _info;
            lblInfo.Foreground = ColorHelper.ConvertToSolidColorBrush(_color);
        }

        /// <summary>
        /// 加载主导航
        /// </summary>
        public override void ReLoadMenu()
        {
            tabMenu.Items.Clear();

            int currIndex = 0;
            List<Plugins> plugins = MainWindowGlobal.CurrPlugins.OrderBy(c => c.Order).ToList();//插件排序

            foreach (var pluginsInfo in plugins)
            {
                List<PluginsModule> pluginsModules = MainWindowGlobal.CurrPluginsModules.Where(c => c.PluginsId == pluginsInfo.Id).OrderBy(c => c.Order).ToList(); //模块排序
                foreach (var moduleInfo in pluginsModules)
                {
                    moduleInfo.DLLName = pluginsInfo.DLLName;
                    TabItem _tabItem = new TabItem();
                    _tabItem.Tag = moduleInfo;
                    _tabItem.Header = moduleInfo.ModuleName;
                    _tabItem.GotFocus += _tabItem_GotFocus;
                    TabControlHelper.SetItemIcon(_tabItem, FontAwesomeCommon.GetUnicode(moduleInfo.Icon));

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

        /// <summary>
        /// 主导航切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            PluginsModule moduleInfo = (sender as TabItem).Tag as PluginsModule;
            List<ModulePage> _pages = MainWindowGlobal.CurrModulePages.Where(c => c.ModuleId == moduleInfo.Id).OrderBy(c => c.Order).ToList();//页面排序

            tvMenu.Items.Clear();
            tvMenu_Simple.Items.Clear();

            if (_pages == null || _pages.Count == 0) 
            {
                MessageBoxX.Show("当前模块下没有任何页面","空值提醒");
                return;
            }

            int currIndex = 0;

            #region 填充二级导航

            foreach (var page in _pages)
            {
                page.FullPath = $"pack://application:,,,/{moduleInfo.DLLName};component/{page.PagePath}";
                TreeViewItem _treeViewItem = new TreeViewItem();
                _treeViewItem.Header = page.PageName;
                _treeViewItem.Margin = new Thickness(0, 2, 0, 2);
                _treeViewItem.Padding = new Thickness(10, 0, 0, 0);
                _treeViewItem.Background = Brushes.Transparent;
                _treeViewItem.Tag = page;
                _treeViewItem.IsSelected = currIndex == 0;
                currIndex += 1;
                TreeViewHelper.SetItemIcon(_treeViewItem, FontAwesomeCommon.GetUnicode(page.Icon));

                TreeViewItem _treeViewItemSimple = new TreeViewItem();
                _treeViewItemSimple.Margin = new Thickness(0, 2, 0, 2);
                _treeViewItemSimple.Padding = new Thickness(10, 0, 0, 0);
                _treeViewItemSimple.Background = Brushes.Transparent;
                _treeViewItemSimple.Tag = page;
                _treeViewItemSimple.ToolTip = page.PageName;
                TreeViewHelper.SetItemIcon(_treeViewItemSimple, FontAwesomeCommon.GetUnicode(page.Icon));

                tvMenu.Items.Add(_treeViewItem);
                tvMenu_Simple.Items.Add(_treeViewItemSimple);
            }

            #endregion

            if (_pages.Count == 1)
            {
                //如果仅有一个导航
                bSecondMenu.Visibility = Visibility.Collapsed;
            }
            else if (_pages.Count > 1)
            {
                //如果存在多个导航
                bSecondMenu.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 设置页面
        /// </summary>
        /// <param name="_s"></param>
        public override void SetFrameSource(string _s)
        {
            mainFrame.Source = new Uri(_s, UriKind.RelativeOrAbsolute);
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTitle();

            EmailNotReadChangedEventObserver.Instance.AddEventListener(Codes.EmailNotReadChanged, OnEmailTimer);//监听邮件事件
            new EmailTimer().Start(5000);//开始定时读取邮件

            windowLoaded = true;
        }

        #region Timer

        private void OnEmailTimer(EmailChangedMessage p)
        {
            UIGlobal.RunUIAction(() =>
            {
                if (p.HasNotReadEmail)
                {
                    bdNotReadEmailCount.Visibility = Visibility.Visible;//是否显示原点
                    bdNotReadEmailCount.IsWaving = true;//原点是否闪烁
                }
                else
                {
                    bdNotReadEmailCount.Visibility = Visibility.Collapsed;//是否显示原点
                    bdNotReadEmailCount.IsWaving = false;//原点是否闪烁
                }
            });
        }

        #endregion

        #region Private 

        /// <summary>
        /// 更新Title
        /// </summary>
        public void UpdateTitle()
        {
            simpleLogo.ToolTip = lblTitle.Text = Title = LocalSettings.settings.MainWindowTitle;
            lblV.Content = $"{LocalSettings.settings.CompanyName}-{LocalSettings.settings.Versions}";
        }

        #endregion

        #region UI Method

        //选择插件
        private void btnSelectPlugins_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            ChangePlugins changePlugins = new ChangePlugins();
            changePlugins.ShowDialog();
            this.MaskVisible(false);
        }

        //更改密码
        private void btnChangePwd_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            UpdatePassword updatePassword = new UpdatePassword();
            updatePassword.ShowDialog();
            IsMaskVisible = false;
        }

        //皮肤
        private void btnSkin_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            SkinWindow skinWindow = new SkinWindow();
            skinWindow.ShowDialog();
            IsMaskVisible = false;
        }

        //设置
        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
            IsMaskVisible = false;
        }

        //切换二级导航
        private void TreeView_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tvMenu.SelectedItem != null)
            {
                TreeViewItem targetItem = tvMenu.SelectedItem as TreeViewItem;
                ModulePage page = targetItem.Tag as ModulePage;
                if (page == null) return;
                mainFrame.Source = new Uri(page.FullPath, UriKind.RelativeOrAbsolute);
            }
        }

        #endregion

        #region 完全关闭窗体

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var result = MessageBoxX.Show("是否退出？", "退出提醒", this, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                windowLoaded = false;//为防止触发ReSize方法

                Closing -= WindowX_Closing;
                CloseWindow();
            }
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

        #region 二级导航动画

        DoubleAnimation hideSecondMenusAnimation = new DoubleAnimation(200, 55, new Duration(TimeSpan.FromSeconds(0.5)));
        DoubleAnimation showSecondMenusAnimation = new DoubleAnimation(55, 200, new Duration(TimeSpan.FromSeconds(0.5)));

        /// <summary>
        /// 显示/隐藏导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowSecondMenus_Click(object sender, RoutedEventArgs e)
        {
            if (bSecondMenu.Width == 55 || bSecondMenu.Width == 200)
            {
                if (bSecondMenu.Width == 200)
                {
                    bSecondMenu.BeginAnimation(WidthProperty, hideSecondMenusAnimation);
                    gHeader.BeginAnimation(WidthProperty, hideSecondMenusAnimation);
                }
                else if (bSecondMenu.Width == 55)
                {
                    bSecondMenu.BeginAnimation(WidthProperty, showSecondMenusAnimation);
                    gHeader.BeginAnimation(WidthProperty, showSecondMenusAnimation);
                }
            }
        }

        private void ShowSecondMenusAnimation_Completed(object sender, EventArgs e)
        {
            bHeader.Visibility = Visibility.Collapsed;
            sHeader.Visibility = Visibility.Visible;
            tvMenu_Simple.Visibility = Visibility.Collapsed;
            tvMenu.Visibility = Visibility.Visible;
        }

        private void HideSecondMenusAnimation_Completed(object sender, EventArgs e)
        {
            bHeader.Visibility = Visibility.Visible;
            sHeader.Visibility = Visibility.Collapsed;
            tvMenu_Simple.Visibility = Visibility.Visible;
            tvMenu.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 导航关联

        /// <summary>
        /// 简约导航切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvMenu_Simple_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvMenu_Simple.SelectedItem == null) return;
            string pageName = (tvMenu_Simple.SelectedItem as TreeViewItem).ToolTip.ToString();//获取简约页面名
            foreach (TreeViewItem item in tvMenu.Items)
            {
                string currPageName = item.Header.ToString();//当前页面名
                if (currPageName == pageName)
                {
                    item.IsSelected = true;//更新大导航
                }
                else
                {
                    item.IsSelected = false;
                }
            }
            TreeView_PreviewMouseUp(null, null);//模拟大导航点击
        }

        /// <summary>
        /// 主导航切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvMenu.SelectedItem == null) return;
            string pageName = (tvMenu.SelectedItem as TreeViewItem).Header.ToString();//获取页面名
            foreach (TreeViewItem item in tvMenu_Simple.Items)
            {
                string currPageName = item.ToolTip.ToString();//当前页面名
                if (currPageName == pageName)
                {
                    item.IsSelected = true;//更新大导航
                }
                else
                {
                    item.IsSelected = false;
                }
            }
        }

        #endregion

        #region  邮件

        /// <summary>
        /// 通知和邮件关闭
        /// </summary>
        private void OnEmailClosing()
        {
            DoubleAnimation widthAni = new DoubleAnimation();
            widthAni.From = 300;
            widthAni.To = 0;
            widthAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            emails.BeginAnimation(WidthProperty, widthAni);
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            if (emails.Width > 100)
            {
                OnEmailClosing();
                return;
            }
            DoubleAnimation widthAni = new DoubleAnimation();
            widthAni.From = 0;
            widthAni.To = 300;
            widthAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            widthAni.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseIn };

            emails.BeginAnimation(WidthProperty, widthAni);
            emails.UpdateEmail();
        }

        #endregion

        #region 窗体改变事件

        double newWindowWidth = 0;
        double newWindowHeight = 0;

        /// <summary>
        /// 窗体大小更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseMainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (windowLoaded)
            {
                if (reSizeGrid.Visibility == Visibility.Collapsed) reSizeGrid.Visibility = Visibility.Visible;//如果隐藏 显示出来
                lblNewSizeInfo.Content = $"当前界面宽高：{e.NewSize.Width} * {e.NewSize.Height}";

                newWindowWidth = e.NewSize.Width;
                newWindowHeight = e.NewSize.Height;
            }
        }

        private void btnSaveSize_Click(object sender, RoutedEventArgs e)
        {
            reSizeGrid.Visibility = Visibility.Collapsed;//隐藏
            LocalSettings.UpdateSize(newWindowWidth, newWindowHeight);//更新本地数据
            windowWidth = newWindowWidth;
            windowHeight = newWindowHeight;
        }

        private void btnOmit_Click(object sender, RoutedEventArgs e)
        {
            reSizeGrid.Visibility = Visibility.Collapsed;//隐藏
        }

        /// <summary>
        /// 恢复设置的大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReSize_Click(object sender, RoutedEventArgs e)
        {
            Width = windowWidth;
            Height = windowHeight;
        }

        #endregion
    }
}