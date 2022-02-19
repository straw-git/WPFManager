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
using Common;
using CoreDBModels;
using FixedAssetsDBModels;
using FixedAssetsPlugin.Windows;

namespace FixedAssetsPlugin.Pages.FixedAssetsPages
{
    /// <summary>
    /// Check.xaml 的交互逻辑
    /// </summary>
    public partial class Check : BasePage
    {
        public Check()
        {
            InitializeComponent();
            this.Order = 1;
        }

        #region Models

        class UIModel
        {
            public string Id { get; set; }
            public string StateName { get; set; }
            public string Name { get; set; }
            public string ModelName { get; set; }
            public string UnitName { get; set; }
            public string LocationName { get; set; }
            public string SubLocationName
            {
                get
                {
                    return LocationName.Length > 20 ? $"{LocationName.Substring(0, 20)}..." : LocationName;
                }
            }
            public string PrincipalName { get; set; }
            public int Count { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        int dataCount = 0;
        int pagerCount = 0;
        int pageSize = 10;
        int currPage = 1;
        bool running = false;


        protected override void OnPageLoaded()
        {
            LoadState();
            list.ItemsSource = Data;
        }


        #region Private Method

        private void LoadState()
        {
            cbState.Items.Clear();
            using (CoreDBContext context = new CoreDBContext())
            {
                var states = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsState).ToList();
                if (states == null) states = new List<SysDic>();
                states.Insert(0, new SysDic()
                {
                    Id = 0,
                    Name = "全部"
                });
                cbState.ItemsSource = states;
                cbState.DisplayMemberPath = "Name";
                cbState.SelectedValuePath = "Id";

                cbState.SelectedIndex = 0;
            }
        }

        private void LoadPager()
        {
            using (var context = new FixedAssetsDBContext())
            {
                string name = txtName.Text;
                int stateId = cbState.SelectedValue.ToString().AsInt();
                string code = txtCode.Text;

                var fixedAssets = context.FixedAssets.Where(c => !c.IsDel);

                if (name.NotEmpty())
                {
                    fixedAssets = fixedAssets.Where(c => c.Name.Contains(name));
                }
                if (stateId > 0)
                {
                    fixedAssets = fixedAssets.Where(c => c.State == stateId);
                }
                if (code.NotEmpty())
                {
                    fixedAssets = fixedAssets.Where(c => c.Id.Contains(code));
                }

                dataCount = fixedAssets.Count();
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

            List<FixedAssets> models = new List<FixedAssets>();
            string name = txtName.Text;
            int stateId = cbState.SelectedValue.ToString().AsInt();
            string code = txtCode.Text;

            await Task.Run(() =>
            {
                using (var context = new FixedAssetsDBContext())
                {
                    var fixedAssets = context.FixedAssets.Where(c => !c.IsDel);

                    if (name.NotEmpty())
                    {
                        fixedAssets = fixedAssets.Where(c => c.Name.Contains(name));
                    }
                    if (stateId > 0)
                    {
                        fixedAssets = fixedAssets.Where(c => c.State == stateId);
                    }
                    if (code.NotEmpty())
                    {
                        fixedAssets = fixedAssets.Where(c => c.Id.Contains(code));
                    }

                    models = fixedAssets.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();

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
                        Count = item.Count,
                        Id = item.Id,
                        LocationName = context.SysDic.First(c => c.Id == item.Location).Name,
                        ModelName = item.ModelName,
                        Name = item.Name,
                        PrincipalName = item.PrincipalName,
                        StateName = context.SysDic.First(c => c.Id == item.State).Name,
                        UnitName = item.UnitName
                    };

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

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();
            MaskVisible(true);

            CheckFixedAssets c = new CheckFixedAssets(id);
            c.ShowDialog();

            MaskVisible(false);
            if (c.Succeed)
            {
                btnRef_Click(null, null);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            AddFixedAssets a = new AddFixedAssets();
            a.ShowDialog();

            MaskVisible(false);
            if (a.Succeed)
            {
                btnRef_Click(null, null);
            }
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
