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

namespace CorePlugin.Windows
{
    /// <summary>
    /// UpdateCount.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateCount : Window
    {
        public bool Succeed = false;
        public int Count = 0;

        public UpdateCount()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCount.Focus();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Count = 0;
            if (string.IsNullOrEmpty(txtCount.Text))
            {
                MessageBoxX.Show("请输入数量", "空值提醒");
                txtCount.Focus();
                return;
            }
            if (!int.TryParse(txtCount.Text, out Count))
            {
                MessageBoxX.Show("数量格式不正确", "格式错误");
                txtCount.Focus();
                txtCount.SelectAll();
                return;
            }

            Succeed = true;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSubmit_Click(null, null);
            }
        }
    }
}
