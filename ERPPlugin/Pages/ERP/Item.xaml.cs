using DBModels.ERP;
using DBModels.Sys;
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
using System.Reflection;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// GoodsList.xaml 的交互逻辑
    /// </summary>
    public partial class Item : BasePage
    {
        public Item()
        {
            InitializeComponent();
            this.Order = 4;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public string Id { get; set; }

            private string name = "";
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                    NotifyPropertyChanged("SubName");
                }
            }
            public string SubName
            {
                get
                {
                    return Name.Length > 20 ? $"{Name.Substring(0, 20)}..." : Name;
                }
            }

            private string typeName = "";
            public string TypeName //物品类型
            {
                get => typeName;
                set
                {
                    typeName = value;
                    NotifyPropertyChanged("TypeName");
                }
            }

            private string unitName = "";
            public string UnitName
            {
                get => unitName;
                set
                {
                    unitName = value;
                    NotifyPropertyChanged("UnitName");
                }
            }

            private string packageName = "";
            public string PackageName
            {
                get => packageName;
                set
                {
                    packageName = value;
                    NotifyPropertyChanged("PackageName");
                }
            }

            private decimal salePrice = 0;
            public decimal SalePrice //零售价
            {
                get => salePrice;
                set
                {
                    salePrice = value;
                    NotifyPropertyChanged("SalePrice");
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
            list.ItemsSource = Data;
            LoadType();
            btnRef_Click(null, null);
        }


        #region Private Method

        private void LoadPager()
        {
            using (var context = new DBContext())
            {
                string name = txtName.Text;
                int typeId = cbType.SelectedValue.ToString().AsInt();

                var goods = context.Goods.Where(c => !c.IsDel);

                if (!string.IsNullOrEmpty(name))
                {
                    goods = goods.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name) || c.Remark.Contains(name));
                }
                if (typeId > 0)
                {
                    goods = goods.Where(c => c.TypeId == typeId);
                }

                dataCount = goods.Count();

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

            List<Goods> models = new List<Goods>();

            string name = txtName.Text;
            int typeId = cbType.SelectedValue.ToString().AsInt();

            await Task.Run(() =>
            {
                using (var context = new DBContext())
                {

                    var goods = context.Goods.Where(c => !c.IsDel);

                    if (!string.IsNullOrEmpty(name))
                    {
                        goods = goods.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name) || c.Remark.Contains(name));
                    }
                    if (typeId > 0)
                    {
                        goods = goods.Where(c => c.TypeId == typeId);
                    }

                    models = goods.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
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
                        Id = item.Id,
                        Name = item.Name,
                        SalePrice = item.SalePrice,
                        TypeName = context.SysDic.First(c => c.Id == item.TypeId).Name,
                        PackageName = item.Specification,
                        UnitName = context.SysDic.First(c => c.Id == item.UnitId).Name
                    };

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        /// <summary>
        /// 加载物品类型
        /// </summary>
        private void LoadType()
        {
            var _source = DataGlobal.GetDic(DicData.GoodsType);

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

        #endregion

        #region UI Method

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditGoods editGoods = new EditGoods();
            editGoods.ShowDialog();
            MaskVisible(false);
            UpdateGridAsync();
        }

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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            string id = (sender as Button).Tag.ToString();
            var selectModel = Data.Single(c => c.Id == id);
            EditGoods g = new EditGoods(id);
            g.ShowDialog();
            MaskVisible(false);
            if (g.Succeed)
            {
                using (DBContext context = new DBContext())
                {
                    selectModel.Name = g.Model.Name;
                    selectModel.PackageName = g.Model.Specification;
                    selectModel.TypeName = context.SysDic.First(c => c.Id == g.Model.TypeId).Name;
                    selectModel.UnitName = context.SysDic.First(c => c.Id == g.Model.UnitId).Name;
                }
            }
        }

        private void btnUpdateSalePrice_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            string id = (sender as Button).Tag.ToString();
            var selectModel = Data.First(c => c.Id == id);
            UpdateSalePrice u = new UpdateSalePrice(id, selectModel.SalePrice);
            u.ShowDialog();
            MaskVisible(false);
            if (u.Succeed)
            {
                selectModel.SalePrice = u.NewPrice;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();
            var selectModel = Data.Single(c => c.Id == id);
            var result = MessageBoxX.Show($"是否确认删除物品[{selectModel.Name}]？", "删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (DBContext context = new DBContext())
                {
                    var _model = context.Goods.Single(c => c.Id == id);
                    _model.IsDel = true;
                    _model.DelUser = TempBasePageData.message.CurrUser.Id;
                    _model.DelTime = DateTime.Now;

                    context.SaveChanges();
                }
                UpdateGridAsync();
            }
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
