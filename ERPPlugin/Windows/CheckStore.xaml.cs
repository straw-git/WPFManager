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

namespace ERPPlugin.Windows
{
    /// <summary>
    /// CheckCount.xaml 的交互逻辑
    /// </summary>
    public partial class CheckStore : Window
    {
        public CheckStore(int _oldCount)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            Model.OldCount = _oldCount;
            lblOldCount.Content = _oldCount;
            txtCount.Text = _oldCount.ToString();
        }

        public class UIModel
        {
            public int OldCount { get; set; }
            public int NewCount { get; set; }
            public string Remark { get; set; }
        }

        public UIModel Model = new UIModel();
        public bool Succeed = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCount.Focus();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int newCount = 0;
            if (string.IsNullOrEmpty(txtCount.Text))
            {
                MessageBoxX.Show("请输入数量", "空值提醒");
                txtCount.Focus();
                return;
            }
            if (!int.TryParse(txtCount.Text, out newCount))
            {
                MessageBoxX.Show("数量格式不正确", "格式错误");
                txtCount.Focus();
                txtCount.SelectAll();
                return;
            }

            Model.NewCount = newCount;
            Model.Remark = txtReamrk.Text;

            Succeed = true;
            Close();
        }

        private void txtCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int newCount = 0;
            if (!int.TryParse(txtCount.Text, out newCount))
            {
                return;
            }
            else 
            {
                lblMinusCount.Content = Math.Abs(Model.OldCount - newCount);
            }
        }
    }
}
