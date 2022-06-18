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
    /// UploadImageBox.xaml 的交互逻辑
    /// </summary>
    public partial class UploadImageBox : UserControl
    {
        #region Properties

        public static readonly DependencyProperty ShowNewProperty = DependencyProperty.Register("ShowNew", typeof(bool), typeof(UploadImageBox), new PropertyMetadata(null));
        public bool ShowNew
        {
            get { return (bool)GetValue(ShowNewProperty); }
            set { SetValue(ShowNewProperty, value); }
        }
        public static readonly DependencyProperty ShowEditProperty = DependencyProperty.Register("ShowEdit", typeof(bool), typeof(UploadImageBox), new PropertyMetadata(null));
        public bool ShowEdit
        {
            get { return (bool)GetValue(ShowEditProperty); }
            set { SetValue(ShowEditProperty, value); }
        }
        public static readonly DependencyProperty ShowDeleteProperty = DependencyProperty.Register("ShowDelete", typeof(bool), typeof(UploadImageBox), new PropertyMetadata(null));
        public bool ShowDelete
        {
            get { return (bool)GetValue(ShowDeleteProperty); }
            set { SetValue(ShowDeleteProperty, value); }
        }

        public delegate void OnNewHandler(string _url);
        public event OnNewHandler OnNew;
        public delegate void OnDeleteHandler();
        public event OnDeleteHandler OnDelete;
        public delegate void OnEditHandler(string _oldUrl, string _newUrl);
        public event OnEditHandler OnEdit;

        #endregion 

        public UploadImageBox()
        {
            InitializeComponent();
        }

        string oldUrl = "";
        string newUrl = "";

        public void UpdateImage(string _imgSource = "")
        {
            if (oldUrl.NotEmpty()) newUrl = _imgSource;
            else oldUrl = _imgSource;

            if (_imgSource.IsNullOrEmpty())
            {
                //没有图
                realImg.Visibility = Visibility.Collapsed;
                if (ShowNew)
                    btnNew.Visibility = Visibility.Visible;
            }
            else
            {
                //有图
                realImg.Visibility = Visibility.Visible;
                img.Source = new BitmapImage(new Uri($"{UserGlobal.CoreSetting.APIUrl}imgs/{_imgSource}", UriKind.RelativeOrAbsolute));
                if (ShowNew)
                    btnNew.Visibility = Visibility.Collapsed;
            }
        }

        private void realImg_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ShowEdit)
                btnUpdate.Visibility = Visibility.Visible;
            if (ShowDelete)
                btnDelete.Visibility = Visibility.Visible;
            op.Visibility = Visibility.Visible;
        }

        private void realImg_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ShowEdit)
                btnUpdate.Visibility = Visibility.Collapsed;
            if (ShowDelete)
                btnDelete.Visibility = Visibility.Collapsed;
            op.Visibility = Visibility.Collapsed;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

            OnEdit?.Invoke(oldUrl, newUrl);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

            OnNew?.Invoke(newUrl);
        }
    }
}
