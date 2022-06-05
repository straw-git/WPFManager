using Panuon.UI.Silver;
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

namespace Warehouse.MyControls
{
    /// <summary>
    /// StoreItemBox.xaml 的交互逻辑
    /// </summary>
    public partial class StoreItem : UserControl
    {
        public StoreItem()
        {
            InitializeComponent();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Grid).Height = this.ActualHeight-10;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Grid).Height = 60;
        }

        Random ran = new Random();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBarHelper.SetAnimateTo(pbCount, ran.Next(0, 100));
        }
    }
}
