using Common.Utils;
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

namespace Common.MyControls
{
    /// <summary>
    /// MyPager_ZHHans.xaml 的交互逻辑
    /// </summary>
    public partial class MyPager_ZHHans : UserControl
    {
        /// <summary>
        /// 当前的索引
        /// </summary>
        public int CurrIndex
        {
            get { return pager.CurrentIndex; }
            set
            {
                pager.CurrentIndex = value;
            }
        }
        /// <summary>
        /// 单页数据条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return Convert.ToInt32((cbPageSize.SelectedItem as ComboBoxItem).Tag);
            }
        }

        public int DataCount = 0;

        public delegate void PageChangeEventHandler(object sender, int pageIndex);
        public event PageChangeEventHandler OnPageChange;

        public MyPager_ZHHans()
        {
            InitializeComponent();
        }

        private void Pagination_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            txtZHHansCurrIndex.Text = e.CurrentIndex.ToString();
            OnPageChange?.Invoke(this, e.CurrentIndex);
        }

        private void btn2Page_Click(object sender, RoutedEventArgs e)
        {
            int goPage = 0;
            int.TryParse(txtZHHansCurrIndex.Text, out goPage);
            if (goPage < 1 || goPage > Convert.ToInt32(lblPageCount.Content)) 
            {
                MessageBoxX.Show("输入不正确的页码","页码错误");
                txtZHHansCurrIndex.Focus();
                txtZHHansCurrIndex.SelectAll();
                return;
            }
            OnPageChange?.Invoke(this, goPage);
        }

        private void txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblPageCount == null) return;
            int pageCount = PagerUtils.GetPagerCount(DataCount, PageSize);//总页数
            lblPageCount.Content = pageCount;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbPageSize_SelectionChanged(null,null);
        }
    }
}
