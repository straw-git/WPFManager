using DBModels.ERP;
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
    /// UpdatePurchasePlanCount.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsDetails : Window
    {
        #region Models

        public class UIModel
        {
            public string Id { get; set; }//物品Id 
            public string Name { get; set; }//物品名称
            public string SupplierName { get; set; }//供应商名称
            public int SupplierId { get; set; }//供应商Id
            public string Manufacturer { get; set; }//生产厂家
            public int Count { get; set; }//数量
            public decimal Price { get; set; }//进货价
            public string Remark { get; set; }
        }

        #endregion

        public bool Succeed = false;
        public UIModel Model = new UIModel();
        int maxInputCount = 0;

        /// <summary>
        /// 更改采购计划量
        /// </summary>
        public GoodsDetails(string _goodsId, string _goodsName, int _maxCount = 0, int _finishedCount = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            Model.Id = _goodsId;
            Model.Name = _goodsName;
            lblName.Content = _goodsName;
            if (_maxCount > 0)
            {
                txtCount.Width = 100;
                lblFinishedCount.Content = $"{_finishedCount}/{_maxCount}";
                maxInputCount = _maxCount - _finishedCount;
                if (maxInputCount == 0)
                {
                    txtCount.IsEnabled = false;
                }
            }
            else
            {
                txtCount.Width = 200;
                lblFinishedCount.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSupplier();
        }

        #region Private Method

        /// <summary>
        /// 加载供应商
        /// </summary>
        private void LoadSupplier()
        {
            List<Supplier> suppliers = new List<Supplier>();
            using (DBContext context = new DBContext())
            {
                suppliers = context.Supplier.Where(c => !c.IsDel).ToList();
            }
            cbSupplier.ItemsSource = suppliers;
            cbSupplier.DisplayMemberPath = "Name";
            cbSupplier.SelectedValuePath = "Id";

            if (suppliers.Count > 0) cbSupplier.SelectedIndex = 0;
        }

        private bool UI2Model()
        {
            Supplier supplier = cbSupplier.SelectedItem as Supplier;
            decimal price = 0;
            int count = 0;

            #region Empty or Error

            if (string.IsNullOrEmpty(txtManufacturer.Text)) txtManufacturer.Text = "无";
            if (string.IsNullOrEmpty(txtPrice.Text)) 
            {
                MessageBoxX.Show("进货价不能为空", "空值提醒");
                txtPrice.Focus();
                return false;
            }
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("进货价格式错误", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return false;
            }
            if (string.IsNullOrEmpty(txtCount.Text))
            {
                MessageBoxX.Show("数量不能为空", "空值提醒");
                txtCount.Focus();
                return false;
            }
            if (!int.TryParse(txtCount.Text, out count))
            {
                MessageBoxX.Show("数量格式错误", "格式错误");
                txtCount.Focus();
                txtCount.SelectAll();
                return false;
            }
            if (maxInputCount > 0 && maxInputCount < count)
            {
                MessageBoxX.Show($"当前最大输入数量[{maxInputCount}]", "超出规定数量");
                txtCount.Focus();
                txtCount.SelectAll();
                return false;
            }

            if (count <= 0) 
            {
                MessageBoxX.Show("数量格式错误", "格式错误");
                txtCount.Focus();
                txtCount.SelectAll();
                return false;
            }

            #endregion 

            Model.Count = count;
            Model.Manufacturer = txtManufacturer.Text;
            Model.Price = price;
            Model.Remark = txtRemark.Text;
            Model.SupplierId = supplier.Id;
            Model.SupplierName = supplier.Name;

            return true;
        }

        #endregion

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            this.Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!UI2Model()) return;
            Succeed = true;
            this.Close();
        }

        #endregion 

    }
}
