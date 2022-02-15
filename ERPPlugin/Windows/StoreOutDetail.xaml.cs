using CoreDBModels.Models;
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
    /// StoreOutDetail.xaml 的交互逻辑
    /// </summary>
    public partial class StoreOutDetail : Window
    {
        string id = "";//Id

        public class UIModel
        {
            public int StoreId { get; set; }
            public string StoreName { get; set; }
            public int MaxCount { get; set; }
            public int Count { get; set; }
        }

        public UIModel Model = new UIModel();
        public bool Succeed = false;
        int canUseCount = 0;

        public StoreOutDetail(string _id, string _name)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            id = _id;

            lblName.Content = _name;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStore();
        }

        private void cbStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _storeId = cbStore.SelectedValue.ToString().AsInt();//仓库
            if (_storeId > 0)
            {
                using (ERPDBContext context = new ERPDBContext())
                {
                    if (context.Stock.Any(c => c.StoreId == _storeId && c.GoodsId == id))
                    {
                        var _stock = context.Stock.First(c => c.StoreId == _storeId && c.GoodsId == id);
                        canUseCount = _stock.Count;
                        lblCanOutCount.Content = _stock.Count;
                    }
                    else
                    {
                        canUseCount = 0;
                        lblCanOutCount.Content = "0";
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            int storeId = cbStore.SelectedValue.ToString().AsInt();

            if (storeId == 0)
            {
                MessageBoxX.Show("请选择仓库", "空值提醒");
                return;
            }

            if (txtCount.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("数量不能为空", "空值提醒");
                txtCount.Focus();
                return;
            }

            if (!int.TryParse(txtCount.Text, out count))
            {
                MessageBoxX.Show("数量格式不正确", "格式错误");
                txtCount.Focus();
                txtCount.SelectAll();
                return;
            }

            if (count > canUseCount)
            {
                MessageBoxX.Show("库存不足", "库存不足");
                txtCount.Focus();
                txtCount.SelectAll();
                return;
            }

            Model.Count = count;
            Model.MaxCount = canUseCount;
            Model.StoreId = storeId;
            Model.StoreName = cbStore.Text;

            Succeed = true;
            Close();
        }

        private void LoadStore()
        {
            cbStore.Items.Clear();
            using (ERPDBContext context = new ERPDBContext())
            {
                var _store = context.SysDic.Where(c => c.ParentCode == DicData.Store).ToList();

                _store.Insert(0, new SysDic()
                {
                    Id = 0,
                    Name = "请选择"
                });

                cbStore.ItemsSource = _store;
                cbStore.DisplayMemberPath = "Name";
                cbStore.SelectedValuePath = "Id";
            }

            cbStore.SelectedIndex = 0;
        }
    }
}
