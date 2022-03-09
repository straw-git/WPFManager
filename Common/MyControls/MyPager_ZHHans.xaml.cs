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

namespace Common.MyControls
{
    /// <summary>
    /// MyPager_ZHHans.xaml 的交互逻辑
    /// </summary>
    public partial class MyPager_ZHHans : UserControl
    {

        #region Properties

        public static readonly DependencyProperty ImageBackProperty = DependencyProperty.Register("ImageBack", typeof(BitmapSource), typeof(MyPager_ZHHans), new PropertyMetadata(null));
        public BitmapSource ImageBack
        {
            get { return (BitmapSource)GetValue(ImageBackProperty); }
            set { SetValue(ImageBackProperty, value); }
        }

        public static readonly DependencyProperty LogoContentProperty = DependencyProperty.Register("LogoContent", typeof(string), typeof(MyPager_ZHHans), new PropertyMetadata(null));
        public string LogoContent
        {
            get { return (string)GetValue(LogoContentProperty); }
            set { SetValue(LogoContentProperty, value); }
        }
        public static readonly DependencyProperty PluginsDataProperty = DependencyProperty.Register("PluginsData", typeof(BasePlugins), typeof(MyPager_ZHHans), new PropertyMetadata(null));
        public BasePlugins PluginsData
        {
            get { return (BasePlugins)GetValue(PluginsDataProperty); }
            set { SetValue(PluginsDataProperty, value); }
        }

        #endregion 

        public delegate void PageChangeEventHandler(object sender, int pageIndex);
        public event PageChangeEventHandler OnPageChange;

        public MyPager_ZHHans()
        {
            InitializeComponent();
        }
    }
}
