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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalePlugin.Pages.CashRegister
{
    /// <summary>
    /// CRSingle.xaml 的交互逻辑
    /// </summary>
    public partial class CRSingle : BasePage
    {
        public CRSingle()
        {
            InitializeComponent();
            IsMenu = false;
        }

        protected override void OnPageLoaded()
        {
            //主窗体操作
            ParentWindow.ShowLeftMenu(false);//隐藏左侧导航
            ParentWindow.ShowTopMenu(false);//隐藏头部导航
            ParentWindow.WindowState = WindowState.Maximized;//主窗体最大化
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.ShowLeftMenu(true);
            ParentWindow.ShowTopMenu(true);
            ParentWindow.WindowState = WindowState.Normal;

            ParentWindow.ReLoadCurrTopMenu();
        }
    }
}
