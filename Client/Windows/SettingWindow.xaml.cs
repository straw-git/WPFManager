using Panuon.UI.Silver;
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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : WindowX
    {
        public SettingWindow()
        {
            InitializeComponent();

            this.UseCloseAnimation();
        }

        private void TvMenu_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tvMenu.SelectedItem != null)
            {
                TreeViewItem selectedTnh = tvMenu.SelectedItem as TreeViewItem;
                string header = selectedTnh.Header.ToString();
                if (!string.IsNullOrEmpty(header))//如果选的是数据点，而不是文件夹
                {
                    if (selectedTnh.Tag == null) return;
                    string tag = selectedTnh.Tag.ToString();
                    if (!string.IsNullOrEmpty(tag))
                    {
                        settingFrame.Source = new Uri(tag, UriKind.Relative);
                    }
                }
            }
        }
    }
}
