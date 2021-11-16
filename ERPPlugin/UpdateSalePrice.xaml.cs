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

namespace ERPPlugin
{
    /// <summary>
    /// UpdateSalePrice.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateSalePrice : Window
    {
        string id = "";
        decimal oldPrice = 0;
        public decimal NewPrice = 0;
        public bool Succeed = false;

        public UpdateSalePrice(string _id, decimal _oldPrice)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            id = _id;
            oldPrice = _oldPrice;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtPrice.Text = oldPrice.ToString();
            txtPrice.Focus();
            txtPrice.SelectAll();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal _newPrice = 0;
            if (!decimal.TryParse(txtPrice.Text, out _newPrice))
            {
                MessageBoxX.Show("输入零售价格式错误", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                context.Goods.Single(c => c.Id == id).SalePrice = _newPrice;

                context.SaveChanges();
            }
            NewPrice = _newPrice;
            Succeed = true;
            this.Close();
        }
    }
}
