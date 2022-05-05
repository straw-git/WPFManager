using Client.Animations._3DWave;
using Client.CurrGlobal;
using Client.Pages;
using Client.Windows;
using Common;
using Common.Data.Local;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml;

namespace Client
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        Storyboard stdStart;
        private readonly ParticleSystem _ps;
        private DispatcherTimer _frameTimer;

        public Login()
        {
            InitializeComponent();
            this.UseCloseAnimation();

            #region 动画

            stdStart = (Storyboard)this.Resources["start"];
            stdStart.Completed += (a, b) =>
            {
                this.root.Clip = null;
            };
            this.Loaded += Window_Loaded;
            this.Closed += Login_Closed;

            _frameTimer = new DispatcherTimer();
            _frameTimer.Tick += OnFrame;
            _frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            _frameTimer.Start();

            _ps = new ParticleSystem(50, 50, Colors.White, 30);

            WorldModels.Children.Add(_ps.ParticleModel);

            _ps.SpawnParticle(30);

            #endregion 
        }

        private void Login_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnFrame(object sender, EventArgs e)
        {
            _ps.Update();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stdStart.Begin();//启动动画

            login.Visibility = Visibility.Visible;
            selectPlugins.Visibility = Visibility.Collapsed;

            #region 事件监听

            login.OnLoginSucceed += OnLoginSucceed;
            login.OnLoginClosed += OnLoginClosed;

            selectPlugins.OnBackLoginClick += SelectPlugins2Login;
            selectPlugins.OnGoMainWindowClick += OnGoMainWindowClick;

            #endregion 

            CheckNullData();
        }

        private void OnLoginClosed()
        {
            Close();
        }

        private void OnGoMainWindowClick()
        {
            if (MainWindowGlobal.MainWindow == null) 
            {
                MainWindowGlobal.MainWindow = new MainWindow();
            }
            this.Visibility = Visibility.Collapsed;
            MainWindowGlobal.MainWindow.Show();
            MainWindowGlobal.MainWindow.ReLoadMenu();
        }

        private void SelectPlugins2Login()
        {
            selectPlugins.HidePlugins();
            login.ShowLogin();
        }

        /// <summary>
        /// 登录成功
        /// </summary>
        private void OnLoginSucceed()
        {
            //更改页面
            login.HideLogin();
            selectPlugins.ShowPlugins();

            //GlobalEvent.StartTimer(5);
        }

        private async void CheckNullData()
        {
            IsEnabled = false;
            var handler = PendingBox.Show("连接数据库...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });
            bool connectionSucceed = false;
            //检查数据
            await Task.Run(() =>
            {
                connectionSucceed = InitData.NullDataCheck();
            });
            await Task.Delay(200);
            if (connectionSucceed)
            {
                handler.UpdateMessage("连接成功,正在加载插件...");
            }
            await Task.Delay(200);
            handler.Close();

            IsEnabled = true;
            if (!connectionSucceed)
            {
                MessageBoxX.Show("数据库连接失败", "连接错误");
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void wb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
