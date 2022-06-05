using Common;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Warehouse.Windows;

namespace Warehouse.Pages
{
    /// <summary>
    /// Supplier.xaml 的交互逻辑
    /// </summary>
    public partial class Supplier : Page
    {
        public Supplier()
        {
            InitializeComponent();
        }

        ObservableCollection<WarehouseDBModels.Supplier> Data = new ObservableCollection<WarehouseDBModels.Supplier>();//数据源
        bool isRunning = false;

        /// <summary>
        /// 加载供应商数据
        /// </summary>
        private async void LoadDataAsync()
        {
            if (isRunning) return;
            if (!isRunning) isRunning = true;
            string _name = txtSearchText.Text;
            string _type = "全部";
            switch (cbType.SelectedIndex)
            {
                case 1:
                    _type = "个人";
                    break;
                case 2:
                    _type = "单位";
                    break;
                default:
                    break;
            }
            Data.Clear();

            using (WarehouseDBContext context = new WarehouseDBContext())
            {
                var listW = context.Suppliers.Where(c => !c.IsDel);
                if (_name.NotEmpty()) listW = listW.Where(c => c.Name.Contains(_name));
                if (_type != "全部") listW = listW.Where(c => c.Type == _type);
                var listData = listW.ToList();

                await Task.Delay(200);
                if (listData != null)
                    foreach (var item in listData)
                    {
                        Data.Add(item);
                    }
                //无数据是否显示
                bNoData.Visibility = listData != null && listData.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            isRunning = false;
        }

        #region UI Method

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
            LoadDataAsync();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditSupplier addSupplier = new EditSupplier((sender as Button).Tag.ToString().AsInt());
            addSupplier.ShowDialog();
            LoadDataAsync();
            this.MaskVisible(false);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (WarehouseDBContext context = new WarehouseDBContext())
            {
                var selectModel = context.Suppliers.First(c => c.Id == (sender as Button).Tag.ToString().AsInt());
                var result = MessageBoxX.Show($"是否删除[{selectModel.Name}]", "删除数据提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    context.Suppliers.Remove(selectModel);
                    LoadDataAsync();
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditSupplier addSupplier = new EditSupplier();
            addSupplier.ShowDialog();
            LoadDataAsync();
            this.MaskVisible(false);
        }

        private void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoadDataAsync();
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDataAsync();
        }

        #endregion
    }
}
