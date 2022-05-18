using Common.MyControls;
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
    /// EditEmail.xaml 的交互逻辑
    /// </summary>
    public partial class EditEmail : Window
    {
        public EditEmail()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSelectUser_Click(object sender, RoutedEventArgs e)
        {
            if (myUserSelector.Visibility == Visibility.Collapsed)
            {
                myUserSelector.Visibility = Visibility.Visible;
            }
            else 
            {
                myUserSelector.Visibility = Visibility.Collapsed;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
