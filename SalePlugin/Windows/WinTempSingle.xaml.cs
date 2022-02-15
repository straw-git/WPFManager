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

namespace SalePlugin.Windows
{
    /// <summary>
    /// WinTempSingle.xaml 的交互逻辑
    /// </summary>
    public partial class WinTempSingle : Window
    {
        public WinTempSingle()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tvTemp_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
