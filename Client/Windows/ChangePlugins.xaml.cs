using Common;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Windows
{
    /// <summary>
    /// ChangePlugins.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePlugins : Window
    {
        public ChangePlugins()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mySelectedPlugins.OnBackLoginClick += OnCloseClick;
            mySelectedPlugins.OnGoMainWindowClick += OnMainWindowClick;

            mySelectedPlugins.UpdatePluginsAsync();
        }

        /// <summary>
        /// 点击进入主页
        /// </summary>
        private void OnMainWindowClick()
        {
            if (MainWindowGlobal.MainWindow == null)
            {
                MainWindowGlobal.MainWindow = new MainWindow();
            }
            this.Visibility = Visibility.Collapsed;
            MainWindowGlobal.MainWindow.Show();
            MainWindowGlobal.MainWindow.ReLoadMenu();
        }

        /// <summary>
        /// 点击关闭
        /// </summary>
        private void OnCloseClick()
        {
            Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            mySelectedPlugins.OnBackLoginClick -= OnCloseClick;
        }

    }
}
