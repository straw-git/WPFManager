using Client.Animation;
using Client.Animation._3DWave;
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

            stdStart = (Storyboard)this.Resources["start"];
            stdStart.Completed += (a, b) =>
            {
                this.root.Clip = null;
            };
            this.Loaded += Window_Loaded;

            _frameTimer = new DispatcherTimer();
            _frameTimer.Tick += OnFrame;
            _frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            _frameTimer.Start();

            _ps = new ParticleSystem(50, 50, Colors.White, 30);

            WorldModels.Children.Add(_ps.ParticleModel);

            _ps.SpawnParticle(30);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stdStart.Begin();

            login.Visibility = Visibility.Visible;
            localPlugins.Visibility = Visibility.Collapsed;
            selectPlugins.Visibility = Visibility.Collapsed;

            #region 事件监听

            login.OnLoginFinished += OnLoginFinished;
            login.OnLocalPluginsClick += OnLocalPluginsClick;

            localPlugins.OnBackLoginClick += LocalPluginsFinished;

            selectPlugins.AddNewWindow += AddPlugins2NewWindow;
            selectPlugins.JoinWindow += JoinPlugins2CurrWindow;
            selectPlugins.OnBackLoginClick += SelectPlugins2Login;

            #endregion 

            AutoUpdatePlugins.Update();
            CheckNullData();
        }

        private void SelectPlugins2Login()
        {
            selectPlugins.HidePlugins();
            login.ShowLogin();
        }

        private void OnLocalPluginsClick()
        {
            //插件管理
            login.HideLogin();
            localPlugins.ShowPlugins();
        }

        private void JoinPlugins2CurrWindow()
        {

        }

        private void AddPlugins2NewWindow()
        {

        }

        private void LocalPluginsFinished()
        {
            localPlugins.HidePlugins();
            login.ShowLogin();
        }

        private void OnLoginFinished()
        {
            UpdateLoginData();
            login.HideLogin();
            selectPlugins.ShowPlugins();
        }

        private void UpdateLoginData()
        {
            if (UserGlobal.IsLogin)
            {
                lblNotLogin.Visibility = Visibility.Collapsed;
                ddLogined.Visibility = Visibility.Visible;

                lblCurrUser.Text = UserGlobal.CurrUser.Name;
            }
            else
            {
                lblNotLogin.Visibility = Visibility.Visible;
                ddLogined.Visibility = Visibility.Collapsed;
            }
        }

        private void OnFrame(object sender, EventArgs e)
        {
            _ps.Update();
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
