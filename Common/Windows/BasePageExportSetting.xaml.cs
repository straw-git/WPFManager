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
    public partial class BasePageExportSetting : Window
    {
        public BasePageExportSetting(Dictionary<string, string> _cols)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            foreach (var key in _cols.Keys)
            {
                Data.Add(new UIModel()
                {
                    Code = key,
                    Title = _cols[key]
                });
            }
        }

        class UIModel
        {
            public string Code { get; set; }
            public string Title { get; set; }
        }

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string key = txtCode.Text;
            string value = txtTitle.Text;

            if (key.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入标题代码", "空值提醒");
                txtCode.Focus();
                txtCode.SelectAll();
                return;
            }
            if (value.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入对应标题", "空值提醒");
                txtTitle.Focus();
                txtTitle.SelectAll();
                return;
            }

            if (Data.Any(c => c.Code == key || c.Title == value))
            {
                MessageBoxX.Show("当前代码或标题重复", "添加失败");
                txtCode.Focus();
                txtCode.SelectAll();
                return;
            }

            Data.Add(new UIModel()
            {
                Code = key,
                Title = value
            });

            //清空文本
            txtCode.Clear();
            txtTitle.Clear();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedCode = (sender as Button).Tag.ToString();
            Data.Remove(Data.First(c => c.Code == selectedCode));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetResult()
        {
            Dictionary<string, string> cols = new Dictionary<string, string>();

            foreach (var item in Data)
            {
                cols.Add(item.Code, item.Title);
            }

            return cols;
        }
    }
}
