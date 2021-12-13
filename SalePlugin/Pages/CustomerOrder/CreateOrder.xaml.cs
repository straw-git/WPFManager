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
    /// CreateOrder.xaml 的交互逻辑
    /// </summary>
    public partial class CreateOrder : BasePage
    {
        public CreateOrder()
        {
            InitializeComponent();
            Order = 0;
        }

        protected override void OnPageLoaded()
        {
           
        }
    }
}
