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

namespace SalePlugin.Pages.SaleOrder
{
    /// <summary>
    /// HisOrder.xaml 的交互逻辑
    /// </summary>
    public partial class HisOrder : BasePage
    {
        public HisOrder()
        {
            InitializeComponent();
            Order = 1;
        }

        protected override void OnPageLoaded()
        {

        }

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {

        }
    }
}
