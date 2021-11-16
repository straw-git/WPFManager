using DBModels.ERP;
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

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// Inventory.xaml 的交互逻辑
    /// </summary>
    public partial class Inventory : BasePage
    {
        public Inventory()
        {
            InitializeComponent();
            this.Order = 0;
            list.ItemsSource = Data;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string ItemId { get; set; }
            public string ItemName { get; set; }
            public string SubName
            {
                get
                {
                    return ItemName.Length > 20 ? $"{ItemName.Substring(0, 20)}..." : ItemName;
                }
            }

            public int StoreId { get; set; }
            public string StoreName { get; set; }
            private int count = 0;
            public int Count //库存余量
            {
                get => count;
                set
                {
                    count = value;
                    NotifyPropertyChanged("Count");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        int dataCount = 0;
        int pagerCount = 0;
        int pageSize = 10;
        int currPage = 1;
        bool running = false;

        protected override void OnPageLoaded()
        {
            btnRef_Click(null, null);
        }


        #region Private Method

        private void LoadPager()
        {
            using (var context = new DBContext())
            {
                string name = txtName.Text;

                var stock = context.Stock.AsEnumerable();

                if (!string.IsNullOrEmpty(name))
                {
                    var goods = context.Goods.Where(c => !c.IsDel);
                    goods = goods.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name) || c.Remark.Contains(name));
                    List<string> goodsIds = (from c in goods select c.Id).ToList();
                   
                    stock = stock.Where(c => goodsIds.IndexOf(c.GoodsId) > -1 );
                }

                dataCount = stock.Count();
            }
            pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);

            if (currPage > pagerCount) currPage = pagerCount;
            gPager.CurrentIndex = currPage;
            gPager.TotalIndex = pagerCount;
        }

        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();
            if (running) return;
            running = true;
            Data.Clear();

            List<Stock> models = new List<Stock>();

            string name = txtName.Text;
            await Task.Run(() =>
            {
                using (var context = new DBContext())
                {
                    var stock = context.Stock.AsEnumerable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        var goods = context.Goods.Where(c => !c.IsDel);
                        goods = goods.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name) || c.Remark.Contains(name));
                        List<string> goodsIds = (from c in goods select c.Id).ToList();
                        stock = stock.Where(c => goodsIds.IndexOf(c.GoodsId) > -1);
                    }

                    models = stock.OrderByDescending(c => c.Id).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (DBContext context = new DBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        Count = item.Count,
                        Id = item.Id,
                        ItemId = item.GoodsId,
                        ItemName = "",
                        StoreId = item.StoreId,
                        StoreName = context.SysDic.First(c => c.Id == item.StoreId).Name,
                    };

                    //库存信息
                    if (context.Goods.Any(c => c.Id == _model.ItemId))
                    {
                        _model.ItemName = context.Goods.First(c => c.Id == _model.ItemId).Name;
                    }

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        #endregion

        #region UI Method

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            btnRef_Click(null, null);
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            var _model = Data.First(x => x.Id == id);
            MaskVisible(true);
            CheckStore c = new CheckStore(_model.Count);
            c.ShowDialog();
            MaskVisible(false);
        }

        #endregion

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;
                gPager.IsEnabled = false;
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
                gPager.IsEnabled = true;
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
