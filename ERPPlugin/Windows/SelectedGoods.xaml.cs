
using ERPDBModels.Models;
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
using System.Windows.Shapes;

namespace ERPPlugin.Windows
{
    /// <summary>
    /// SelectedGoods.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedGoods : Window
    {
        /// <summary>
        /// 选择物品
        /// </summary>
        /// <param name="_maxCount">数量</param>
        /// <param name="_store">是否验证库存</param>
        /// <param name="_showNoStoreInfo">是否显示无库存需求物品</param>
        public SelectedGoods(int _maxCount, bool _store = false,bool _showNoStoreInfo=false)
        {
            InitializeComponent();

            MaxCount = _maxCount;
            Store = _store;
        }

        public class UIModel : INotifyPropertyChanged
        {
            private bool isChecked = false;
            public bool IsChecked
            {
                get => isChecked;
                set
                {
                    isChecked = value;
                    NotifyPropertyChanged("IsChecked");
                }
            }
            private bool isEnable = false;
            public bool IsEnable
            {
                get => isEnable;
                set
                {
                    isEnable = value;
                    NotifyPropertyChanged("IsEnable");
                }
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public string SubName
            {
                get
                {
                    return Name.Length >= 20 ? $"{Name.Substring(0, 20)}..." : Name;
                }
            }
            public string Package { get; set; }
            public string SalePrice { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        int MaxCount = 0;
        bool Store = false;
        bool ShowNoStoreInfo = false;
        public List<string> Ids = new List<string>();
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        public bool Succeed = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
            LoadListAsync();
        }

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;

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

        private void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadListAsync(txtSearchText.Text);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Succeed = true;
            Close();
        }

        private void cbFocusGoods_Checked(object sender, RoutedEventArgs e)
        {
            string id = (sender as CheckBox).Tag.ToString();
            if (Ids.Count == MaxCount - 1)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    if (!Data[i].IsChecked && Data[i].Id != id)
                        Data[i].IsEnable = false;
                }
            }
            Ids.Add(id);
            UpdateCount();
        }

        private void cbFocusGoods_Unchecked(object sender, RoutedEventArgs e)
        {
            string id = (sender as CheckBox).Tag.ToString();
            for (int i = 0; i < Data.Count; i++)
            {
                if (!Data[i].IsEnable)
                    Data[i].IsEnable = true;
            }
            Ids.Remove(id);
            UpdateCount();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        #endregion 

        #region Private Method

        private async void LoadListAsync(string _searchText = "")
        {
            Ids.Clear();
            Data.Clear();

            ShowLoadingPanel();
            List<Goods> goods = new List<Goods>();

            await Task.Run(() =>
            {
                using (ERPDBContext context = new ERPDBContext())
                {
                    if (string.IsNullOrEmpty(_searchText))
                    {
                        goods = context.Goods.ToList();
                    }
                    else
                    {
                        goods = context.Goods.Where(c=>c.Name.Contains(_searchText)||c.QuickCode.Contains(_searchText)||c.Remark.Contains(_searchText)).ToList();
                    }

                    if (Store) 
                    {
                        //筛选有库存的
                        int count = goods.Count;

                        for (int i = count - 1; i >= 0; i--)
                        {
                            var item = goods[i];
                            if (!item.IsStock)
                            {
                                //不用验证库存的物品
                                continue;
                            }
                            if (!context.Stock.Any(c => c.GoodsId == item.Id))
                            {
                                //库存中不存在
                                goods.Remove(item);
                            }
                            else
                            {
                                if (context.Stock.Where(c => c.GoodsId == item.Id).Sum(c => c.Count) == 0)
                                {
                                    //库存为0
                                    goods.Remove(item);
                                }
                            }
                        }
                    }
                }
            });
            await Task.Delay(300);
            for (int i = 0; i < goods.Count; i++)
            {
                var s = goods[i];
                //不显示无库存需求物品并且  当前物品无库存需求
                if (!ShowNoStoreInfo && !s.IsStock) continue;

                Data.Add(new UIModel()
                {
                    Id = s.Id,
                    IsChecked = false,
                    IsEnable = true,
                    Name = s.Name,
                    Package = s.Specification,
                    SalePrice = s.SalePrice.ToString()
                });
            }
            HideLoadingPanel();
        }

        private void UpdateCount()
        {
            lblCount.Content = Ids.Count;
        }

        #endregion

    }
}
