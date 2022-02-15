using CoreDBModels;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using FinancePlugin.Windows;

namespace FinancePlugin.Pages.Finance
{
    /// <summary>
    /// Payment.xaml 的交互逻辑
    /// </summary>
    public partial class Payment : BasePage
    {
        public Payment()
        {
            InitializeComponent();
            this.Order = 2;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string CardId { get; set; }
            public string Remark { get; set; }
            private decimal price = 0;
            public decimal Price
            {
                get => price; set
                {
                    price = value;
                    NotifyPropertyChanged("Price");
                }
            }
            public string Holder { get; set; }//持有人

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion 

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();


        protected override void OnPageLoaded()
        {
            LoadTVMain();
            list.ItemsSource = Data;
        }


        #region UI Method

        private void btnChangePrice_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            var selectModel = Data.First(c => c.Id == id);
            MaskVisible(true);
            UpdatePrice a = new UpdatePrice("调整余额", selectModel.Price);
            a.ShowDialog();
            MaskVisible(false);
            if (a.Succeed)
            {
                using (FinanceDBContext context = new FinanceDBContext())
                {
                    context.Payment.Single(c => c.Id == id).Price = a.NewPrice;
                    context.SaveChanges();
                }
                Data.Single(c => c.Id == id).Price = a.NewPrice;
                MessageBoxX.Show("账户余额更新成功","成功");
            }
        }

        private void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateGridAsync();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int modelId = (tvMain.SelectedItem as TreeViewItem).Tag.ToString().AsInt();
            MaskVisible(true);
            AddPayment a = new AddPayment(modelId);
            a.ShowDialog();
            MaskVisible(false);

            if (a.Succeed)
            {
                btnRef_Click(null, null);
            }
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridAsync();
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 加载导航
        /// </summary>
        private void LoadTVMain()
        {
            tvMain.Items.Clear();

            using (CoreDBContext context = new CoreDBContext())
            {
                var payModels = context.SysDic.Where(c => c.ParentCode == DicData.PayModel).ToList();
                foreach (var item in payModels)
                {
                    tvMain.Items.Add(new TreeViewItem()
                    {
                        Header = item.Name,
                        Margin = new Thickness(2, 0, 0, 2),
                        Padding = new Thickness(10, 0, 0, 0),
                        Background = Brushes.Transparent,
                        Tag = item.Id,
                        IsSelected = false
                    });
                }

                if (payModels.Count > 0)
                    (tvMain.Items[0] as TreeViewItem).IsSelected = true;
            }
        }

        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();
            Data.Clear();

            List<FinanceDBModels.Models.Payment> models = new List<FinanceDBModels.Models.Payment>();

            int typeId = 0;

            if (tvMain.SelectedItem != null)
                typeId = (tvMain.SelectedItem as TreeViewItem).Tag.ToString().AsInt();

            await Task.Run(() =>
            {
                using (var context = new FinanceDBContext())
                {
                    var payments = context.Payment.Where(c => !c.IsDel);

                    if (typeId > 0)
                    {
                        payments = payments.Where(c => c.PayModelId == typeId);
                    }

                    models = payments.OrderByDescending(c => c.CreateTime).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (CoreDBContext context = new CoreDBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        CardId = item.CardId,
                        Holder = item.Holder,
                        Id = item.Id,
                        Remark = item.Remark,
                        Price = item.Price
                    };

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
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

    }
}
