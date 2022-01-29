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

namespace DemoPlugin.Pages.Test
{
    /// <summary>
    /// Index1.xaml 的交互逻辑
    /// </summary>
    public partial class Index1 : BasePage
    {
        public Index1()
        {
            InitializeComponent();
            this.Order = 1;
        }

        protected override void OnPageLoaded()
        {
            
        }
    }
}
