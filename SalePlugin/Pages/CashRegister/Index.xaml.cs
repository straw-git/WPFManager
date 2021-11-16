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
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Index : BasePage
    {
        public Index()
        {
            InitializeComponent();
            Order = 0;
        }
        protected override void OnPageLoaded() 
        {

        }

        private void btnSingle_Click(object sender, RoutedEventArgs e)
        {
            string _s = $"pack://application:,,,/SalePlugin;component/Pages/CashRegister/CRSingle.xaml";
            ParentWindow.SetFrameSource(_s);
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            string _s = $"pack://application:,,,/SalePlugin;component/Pages/CashRegister/CROrder.xaml";
            ParentWindow.SetFrameSource(_s);
        }
    }
}
