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
using FinanceDBModels;
using FinancePlugin.Windows;

namespace FinancePlugin.Pages.Finance
{
    /// <summary>
    /// Revenue.xaml 的交互逻辑
    /// </summary>
    public partial class Revenue : BasePage
    {
        public Revenue()
        {
            InitializeComponent();
            this.Order = 0;
        }

        #region Models

        class UIModel
        {
            public string BillTime { get; set; }
            public string StaffName { get; set; }
            public string TypeName { get; set; }
            public string CreateStaffName { get; set; }
            public decimal Price { get; set; }
            public string Things { get; set; }
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
            LoadAddType();
            list.ItemsSource = Data;
        }

        #region UI Method

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            AddFinanceItem a = new AddFinanceItem();
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

        private void btnTypeManager_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            AddFinanceType a = new AddFinanceType();
            a.ShowDialog();

            MaskVisible(false);
        }

        private void cbEnableTime_Checked(object sender, RoutedEventArgs e)
        {
            dtStart.IsEnabled = true;
            dtEnd.IsEnabled = true;
        }

        private void cbEnableTime_Unchecked(object sender, RoutedEventArgs e)
        {
            dtStart.IsEnabled = false;
            dtEnd.IsEnabled = false;
        }

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            btnRef_Click(null, null);
        }

        #endregion

        #region Private Method

        private void LoadAddType()
        {
            cbType.Items.Clear();
            using (FinanceDBContext context = new FinanceDBContext())
            {
                var types = context.FinanceType.ToList();

                types.Insert(0, new FinanceType()
                {
                    Id = 0,
                    Name = "全部"
                });

                cbType.ItemsSource = types;
                cbType.DisplayMemberPath = "Name";
                cbType.SelectedValuePath = "Id";

                if (types.Count > 0)
                    cbType.SelectedIndex = 0;
            }
        }

        private void LoadPager()
        {
            using (var context = new FinanceDBContext())
            {
                int typeId = cbType.SelectedValue.ToString().AsInt();

                var financeBill = context.FinanceBill.AsEnumerable();
                DateTime startTime = dtStart.SelectedDateTime.MinDate();
                DateTime endTime = dtEnd.SelectedDateTime.MaxDate();

                if (typeId > 0)
                {
                    financeBill = financeBill.Where(c => c.AddType == typeId);
                }
                if ((bool)cbEnableTime.IsChecked)
                {
                    financeBill = financeBill.Where(c => c.BillTime >= startTime && c.BillTime <= endTime);
                }

                dataCount = financeBill.Count();

                lblPriceCount.Content = financeBill.Sum(c => c.Price);
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

            List<FinanceBill> models = new List<FinanceBill>();

            int typeId = cbType.SelectedValue.ToString().AsInt();
            DateTime startTime = dtStart.SelectedDateTime.MinDate();
            DateTime endTime = dtEnd.SelectedDateTime.MaxDate();
            bool enableTime = (bool)cbEnableTime.IsChecked;

            await Task.Run(() =>
            {
                using (var context = new FinanceDBContext())
                {
                    var financeBill = context.FinanceBill.AsEnumerable();

                    if (typeId > 0)
                    {
                        financeBill = financeBill.Where(c => c.AddType == typeId);
                    }
                    if (enableTime)
                    {
                        financeBill = financeBill.Where(c => c.BillTime >= startTime && c.BillTime <= endTime);
                    }

                    models = financeBill.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (CoreDBContext context = new CoreDBContext())
            {
                foreach (var item in models)
                {
                    var user = context.User.First(c => c.Id == item.Creator);
                    Staff staff = new Staff() { Name="超级管理员" };
                    if (user.StaffId.NotEmpty())
                        staff = context.Staff.First(c => c.Id == user.StaffId);

                    UIModel _model = new UIModel()
                    {
                        BillTime = item.BillTime.ToString("yyyy年MM月dd日"),
                        CreateStaffName = staff.Name,
                        Price = item.Price,
                        StaffName = item.StaffName,
                        Things = item.Things,
                        TypeName ="待更新"// context.FinanceType.First(c => c.Id == item.AddType).Name
                    };

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
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
