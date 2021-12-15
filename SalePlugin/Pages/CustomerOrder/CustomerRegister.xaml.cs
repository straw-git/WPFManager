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

namespace SalePlugin.Pages.CustomerOrder
{
    /// <summary>
    /// CustomerRegister.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerRegister : BasePage
    {
        public CustomerRegister()
        {
            InitializeComponent();
            Order = 0;
        }

        protected override void OnPageLoaded()
        {
            
        }

        private void btnCopyAddress_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReadIdCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
