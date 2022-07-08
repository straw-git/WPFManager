using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// ImageEditer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEditer : UserControl
    {
        /// <summary>
        /// 新文件类型
        /// </summary>
        public enum NewTypeEnum
        {
            /// <summary>
            /// 本地文件
            /// </summary>
            LocalFile,
            /// <summary>
            /// 摄像头
            /// </summary>
            Camera,
            /// <summary>
            /// 全是
            /// </summary>
            Both
        }

        #region Properties

        public static readonly DependencyProperty ShowNewProperty = DependencyProperty.Register("ShowNew", typeof(bool), typeof(ImageEditer), new PropertyMetadata(null));
        public bool ShowNew
        {
            get { return (bool)GetValue(ShowNewProperty); }
            set { SetValue(ShowNewProperty, value); }
        }
        public static readonly DependencyProperty ShowEditProperty = DependencyProperty.Register("ShowEdit", typeof(bool), typeof(ImageEditer), new PropertyMetadata(null));
        public bool ShowEdit
        {
            get { return (bool)GetValue(ShowEditProperty); }
            set { SetValue(ShowEditProperty, value); }
        }
        public static readonly DependencyProperty ShowDeleteProperty = DependencyProperty.Register("ShowDelete", typeof(bool), typeof(ImageEditer), new PropertyMetadata(null));
        public bool ShowDelete
        {
            get { return (bool)GetValue(ShowDeleteProperty); }
            set { SetValue(ShowDeleteProperty, value); }
        }
        public static readonly DependencyProperty NewTypeProperty = DependencyProperty.Register("NewType", typeof(NewTypeEnum), typeof(ImageEditer), new PropertyMetadata(null));
        public NewTypeEnum NewType
        {
            get { return (NewTypeEnum)GetValue(NewTypeProperty); }
            set { SetValue(NewTypeProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ImageEditer), new PropertyMetadata(null));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public delegate void OnNewHandler(string _url);
        public event OnNewHandler OnNew;
        public delegate void OnDeleteHandler(string _url);
        public event OnDeleteHandler OnDelete;
        public delegate void OnEditHandler(string _oldUrl, string _newUrl);
        public event OnEditHandler OnEdit;

        #endregion 

        public ImageEditer()
        {
            InitializeComponent();
        }

        string oldUrl = "";
        string newUrl = "";
        /// <summary>
        /// 选中的图片
        /// </summary>
        public string Text
        {
            get
            {
                string s = "";

                if (oldUrl.NotEmpty()) s = oldUrl;
                if (newUrl.NotEmpty()) s = newUrl;

                return s;
            }
        }

        public void UpdateImage(string _imgSource = "")
        {
            if (oldUrl.NotEmpty()) newUrl = _imgSource;
            else oldUrl = _imgSource;

            if (_imgSource.IsNullOrEmpty())
            {
                //没有图
                realImg.Visibility = Visibility.Collapsed;
                if (ShowNew)
                    gNew.Visibility = Visibility.Visible;
            }
            else
            {
                //有图
                realImg.Visibility = Visibility.Visible;
                img.Source = new BitmapImage(new Uri($"{UserGlobal.CoreSetting.APIUrl}imgs/{_imgSource}", UriKind.RelativeOrAbsolute));
                if (ShowNew)
                {
                    gNew.Visibility = Visibility.Collapsed;
                }
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
            if (UploadImage())
            {
                OnEdit?.Invoke(oldUrl, newUrl);
            }
        }

        private bool UploadImage()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*"
            };

            if ((bool)openfiledialog.ShowDialog())
            {
                var client = new WebClient();
                byte[] _b = client.UploadFile($"{UserGlobal.CoreSetting.APIUrl}Home/PostFile", "POST", openfiledialog.FileName);
                string fileName = Encoding.UTF8.GetString(_b, 0, _b.Length);

                UpdateImage(fileName);
                return true;
            }

            return false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(oldUrl);
            oldUrl = "";
            UpdateImage();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (UploadImage())
            {
                OnNew?.Invoke(oldUrl);
            }
        }

        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能未实现");
        }
    }
}
