using Common;
using CoreDBModels;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Common.UserGlobal;

namespace Client.MyControls
{
    /// <summary>
    /// LogoBox.xaml 的交互逻辑
    /// </summary>
    public partial class PluginsBox : UserControl
    {
        public PluginsBox()
        {
            InitializeComponent();

            Code = Guid.NewGuid().ToString();
        }

        #region Properties

        public static readonly DependencyProperty ImageBackProperty = DependencyProperty.Register("ImageBack", typeof(BitmapSource), typeof(PluginsBox), new PropertyMetadata(null));
        public BitmapSource ImageBack
        {
            get { return (BitmapSource)GetValue(ImageBackProperty); }
            set { SetValue(ImageBackProperty, value); }
        }

        public static readonly DependencyProperty LogoContentProperty = DependencyProperty.Register("LogoContent", typeof(string), typeof(PluginsBox), new PropertyMetadata(null));
        public string LogoContent
        {
            get { return (string)GetValue(LogoContentProperty); }
            set { SetValue(LogoContentProperty, value); }
        }
        public static readonly DependencyProperty PluginsDataProperty = DependencyProperty.Register("PluginsData", typeof(Plugins), typeof(PluginsBox), new PropertyMetadata(null));
        public Plugins PluginsData
        {
            get { return (Plugins)GetValue(PluginsDataProperty); }
            set { SetValue(PluginsDataProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(PluginsBox), new PropertyMetadata(null));
        public bool IsChecked
        {
            get { return (bool)cbOpen.IsChecked; }
        }

        #endregion 

        /// <summary>
        /// 更改事件
        /// </summary>
        public Action<PluginsBox, bool> CheckChanged;
        /// <summary>
        /// 唯一标志
        /// </summary>
        public string Code;

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var moveX = (e.GetPosition(this.img).X / this.img.ActualWidth - 0.5) * (-25);
            var moveY = -(e.GetPosition(this.img).Y / this.img.ActualHeight - 0.5) * (-20);

            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.To = 10d;
            Vector3D axis = new Vector3D(moveX, moveY, 0);
            AxisAngleRotation3D aar = this.FindName("MyAxisAngleRotation3D") as AxisAngleRotation3D;
            if (aar != null)
            {
                aar.Axis = axis;
                aar.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            da.To = 0d;
            AxisAngleRotation3D aar = this.FindName("MyAxisAngleRotation3D") as AxisAngleRotation3D;
            if (aar != null)
            {
                aar.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
            }
        }

        private void vpLogo_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((bool)cbOpen.IsChecked) return;
            var s = vpLogo.Camera as PerspectiveCamera;
            s.Position = new Point3D(0, 0, 1000);
        }

        private void vpLogo_MouseLeave(object sender, MouseEventArgs e)
        {
            var s = vpLogo.Camera as PerspectiveCamera;
            s.Position = new Point3D(0, 0, 1500);
        }

        private void MyLogo_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void bLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(bool)cbOpen.IsChecked)
            {
                cbOpen.IsChecked = true;
                bLogo.BorderThickness = new Thickness(0, 2, 0, 0);
            }
            else
            {
                bLogo.BorderThickness = new Thickness(0);
                cbOpen.IsChecked = false;
            }
            //触发事件
            CheckChanged?.Invoke(this, (bool)cbOpen.IsChecked);
        }

        private void MyLogo_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckChanged = null;
        }

    }
}
