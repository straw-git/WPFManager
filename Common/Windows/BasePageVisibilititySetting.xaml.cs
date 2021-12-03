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
using System.Windows.Shapes;

namespace Common.Windows
{
    /// <summary>
    /// SelectedGoods.xaml 的交互逻辑
    /// </summary>
    public partial class BasePageVisibilititySetting : Window
    {
        public List<UIModel> Result = new List<UIModel>();
        public BasePageVisibilititySetting(List<string> _cols)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            foreach (var c in _cols)
            {
                var _s = c.Split('|');
                Data.Add(new UIModel()
                {
                    Code = _s[0],
                    Title = _s[1],
                    IsChecked = _s[2] == "0" ? true : false
                });
            }
        }

        public class UIModel : BaseUIModel
        {
            public string Code { get; set; }
            public string Title { get; set; }
        }

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Result = Data.ToList();
            this.Close();
        }

        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)((sender as CheckBox).IsChecked);
            foreach (var item in Data)
            {
                item.IsChecked = isCheck;
            }
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectItem = list.SelectedItem as UIModel;
                bool targetIsChecked = !selectItem.IsChecked;
                Data.Single(c => c.Code == selectItem.Code).IsChecked = targetIsChecked;
            }
        }
    }
}
