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

namespace HRPlugin.Windows
{
    /// <summary>
    /// StaffDelete.xaml 的交互逻辑
    /// </summary>
    public partial class StaffDelete : Window
    {
        public StaffDelete()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        public class UIModel 
        {
            public DateTime StopTime { get; set; }
            public bool StopInsurance { get; set; }
            public bool StopContract { get; set; }
        }

        public bool Succeed = false;
        public UIModel Model = new UIModel();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Model.StopTime = dtStopTime.SelectedDateTime;
            Model.StopInsurance = (bool)cbInsurance.IsChecked;
            Model.StopContract = (bool)cbContract.IsChecked;

            Succeed = true;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }
    }
}
