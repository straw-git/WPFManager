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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Common.MyControls
{
    /// <summary>
    /// SelectedListMenuBar.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedListMenuBar : UserControl
    {
        #region Properties

        public static readonly DependencyProperty PrintVisibilityProperty = DependencyProperty.Register("PrintVisibility", typeof(Visibility), typeof(SelectedListMenuBar), new PropertyMetadata(null));
        public Visibility PrintVisibility
        {
            get { return (Visibility)GetValue(PrintVisibilityProperty); }
            set { SetValue(PrintVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ExportVisibilityProperty = DependencyProperty.Register("ExportVisibility", typeof(Visibility), typeof(SelectedListMenuBar), new PropertyMetadata(null));
        public Visibility ExportVisibility
        {
            get { return (Visibility)GetValue(ExportVisibilityProperty); }
            set { SetValue(ExportVisibilityProperty, value); }
        }
        public static readonly DependencyProperty DeleteVisibilityProperty = DependencyProperty.Register("DeleteVisibility", typeof(Visibility), typeof(SelectedListMenuBar), new PropertyMetadata(null));
        public Visibility DeleteVisibility
        {
            get { return (Visibility)GetValue(DeleteVisibilityProperty); }
            set { SetValue(DeleteVisibilityProperty, value); }
        }

        #endregion



        private int number = 0;
        public int Number
        {
            get => number;
            set
            {
                number = value;
            }
        }


        private bool menusShowed = false;
        private List<dynamic> Source = new List<dynamic>();

        public SelectedListMenuBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 增加
        /// </summary>
        public void IncrementNumber(dynamic _item)
        {
            Number += 1;
            bNum.Text = Number.ToString();
            ShowMenus();
            Source.Add(_item);
        }

        private void ShowMenus()
        {
            if (menusShowed) return;
            menusShowed = true;
            bNum.IsWaving = true;

            DoubleAnimation opactiyAni = new DoubleAnimation();
            opactiyAni.From = 0;
            opactiyAni.To = 1;
            opactiyAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            menus.BeginAnimation(OpacityProperty, opactiyAni);
        }

        /// <summary>
        /// 减少
        /// </summary>
        public void DecrementNumber(dynamic _item)
        {
            Number -= 1;
            bNum.Text = Number.ToString();
            HideMenu();
            Source.Remove(_item);
        }

        private void HideMenu()
        {
            if (Number > 0) return;

            menusShowed = false;
            bNum.IsWaving = false;
            DoubleAnimation opactiyAni = new DoubleAnimation();
            opactiyAni.From = 1;
            opactiyAni.To = 0;
            opactiyAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            menus.BeginAnimation(OpacityProperty, opactiyAni);
        }
    }
}
