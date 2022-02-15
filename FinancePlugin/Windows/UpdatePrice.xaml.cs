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

namespace FinancePlugin.Windows
{
    /// <summary>
    /// UpdateWage.xaml 的交互逻辑
    /// </summary>
    public partial class UpdatePrice : Window
    {
        public decimal OldPrice = 0;
        public decimal NewPrice = 0;
        public bool Succeed = false;

        public UpdatePrice(string _title,decimal _oldPrice)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            Title = _title;
            OldPrice = _oldPrice;
            lblOldPrice.Content = _oldPrice;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtPrice.Focus();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBoxX.Show("请输入更新金额", "空值提醒");
                txtPrice.Focus();
                return;
            }
            decimal _price = 0;
            if (!decimal.TryParse(txtPrice.Text, out _price))
            {
                MessageBoxX.Show("价格格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            NewPrice = _price;

            Succeed = true;
            Close();
        }
    }
}
