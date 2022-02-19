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

namespace CustomerPlugin.Pages.Customer
{
    /// <summary>
    /// MemberLevel.xaml 的交互逻辑
    /// </summary>
    public partial class MemberLevel : BasePage
    {
        public MemberLevel()
        {
            InitializeComponent();
            Order = 1;
        }

        //页面数据集合
        ObservableCollection<CustomerDBModels.MemberLevel> Data = new ObservableCollection<CustomerDBModels.MemberLevel>();
        bool running = false;

        protected override void OnPageLoaded()
        {
            list.ItemsSource = Data;
            UpdateGridAsync();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            decimal price = 0;

            if (txtName.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入会员标识", "空值提醒");
                txtName.Focus();
                return;
            }
            if (txtPrice.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入金额", "空值提醒");
                txtPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("金额格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            CustomerDBModels.MemberLevel level = new CustomerDBModels.MemberLevel();
            level.Name = name;
            level.LogPriceCount = price;

            using (CustomerDBContext context = new CustomerDBContext())
            {
                level = context.MemberLevel.Add(level);
                context.SaveChanges();
            }

            MessageBoxX.Show("成功", "成功");

            UpdateGridAsync();
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridAsync();
        }


        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();
            if (running) return;
            running = true;
            Data.Clear();

            List<CustomerDBModels.MemberLevel> models = new List<CustomerDBModels.MemberLevel>();

            await Task.Run(() =>
            {
                using (var context = new CustomerDBContext())
                {
                    models = context.MemberLevel.OrderBy(c => c.LogPriceCount).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            foreach (var item in models)
            {
                Data.Add(item);
            }

            HideLoadingPanel();
            running = false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            var selectModel = Data.First(c => c.Id == id);
            var result = MessageBoxX.Show($"是否确认删除会员标识[{selectModel.Name}]？", "删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CustomerDBContext context = new CustomerDBContext())
                {
                    context.MemberLevel.Remove(selectModel);
                    Data.Remove(selectModel);
                }
            }
        }


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
