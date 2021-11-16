using DBModels.Sys;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// Supplier.xaml 的交互逻辑
    /// </summary>
    public partial class Supplier : BasePage
    {
        public Supplier()
        {
            InitializeComponent();
            this.Order = 1;
        }

        class UIModel
        {
            public int Id { get; set; }
            public string TypeName { get; set; }
            public string Address { get; set; }
            public string SubAddress
            {
                get
                {
                    return Address.Length > 20 ? $"{Address.Substring(0, 20)}..." : Address;
                }
            }
            public string Name { get; set; }//公司
            public string Qualification { get; set; }//供货资质

            public string Phone { get; set; }//联系电话
            public string ContactName { get; set; }//联系人
        }

        List<UIModel> models = new List<UIModel>();
        bool running = false;

        protected override void OnPageLoaded()
        {
            LoadSupplierType();
            LoadGridAsync();
        }

        #region Private Method

        private void LoadSupplierType()
        {
            var _source = DataGlobal.GetDic(DicData.SupplierType);
            _source.Insert(0, new SysDic()
            {
                Id = -1,
                Name = "全部"
            });
            cbType.ItemsSource = _source;
            cbType.DisplayMemberPath = "Name";
            cbType.SelectedValuePath = "Id";


            cbType.SelectedIndex = 0;
        }

        private async void LoadGridAsync()
        {
            ShowLoadingPanel();
            if (running) return;
            running = true;
            List<DBModels.ERP.Supplier> suppliers = new List<DBModels.ERP.Supplier>(); 

            await Task.Run(() =>
            {
                models.Clear();
                using (DBContext context = new DBContext())
                {
                    UIGlobal.RunUIAction(() =>
                    {
                        var _s = context.Supplier.Where(c => !c.IsDel);
                        int selectTypeId = cbType.SelectedValue.ToString().AsInt();
                        if (selectTypeId > 0)
                        {
                            _s = _s.Where(c => c.Type == selectTypeId);
                        }
                        if (!string.IsNullOrEmpty(txtName.Text))
                        {
                            _s = _s.Where(c => c.Name.Contains(txtName.Text) || c.Address.Contains(txtName.Text) || c.ContactName.Contains(txtName.Text));
                        }

                        suppliers = _s.ToList();
                    });

                    foreach (var item in suppliers)
                    {
                        models.Add(new UIModel()
                        {
                            Address = item.Address,
                            ContactName = item.ContactName,
                            Id = item.Id,
                            Name = item.Name,
                            Phone = item.Phone,
                            Qualification = item.Qualification ? "是" : "否",
                            TypeName = context.SysDic.First(c => c.Id == item.Type).Name
                        });
                    }
                }
            });

            await Task.Delay(300);

            HideLoadingPanel();

            bNoData.Visibility = models.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            list.ItemsSource = null;
            list.ItemsSource = models;

            running = false;
        }

        #endregion 

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;
                bNoData.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;
                bNoData.IsEnabled = true;

                OnLoadingHideComplate();
            }
        }

        private void OnLoadingHideComplate()
        {

        }

        private void OnLoadingShowComplate()
        {

        }

        #endregion

        #region UI Method

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditSupplier addSupplier = new EditSupplier((sender as Button).Tag.ToString().AsInt());
            addSupplier.ShowDialog();
            LoadGridAsync();
            MaskVisible(false);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (DBContext context = new DBContext())
            {
                var selectModel = context.Supplier.First(c => c.Id == (sender as Button).Tag.ToString().AsInt());
                var result = MessageBoxX.Show($"是否删除[{selectModel.Name}]", "删除数据提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    context.Supplier.Remove(selectModel);
                    LoadGridAsync();
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditSupplier addSupplier = new EditSupplier();
            addSupplier.ShowDialog();
            LoadGridAsync();
            MaskVisible(false);
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadGridAsync();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            LoadGridAsync();
        }

        #endregion
    }
}
