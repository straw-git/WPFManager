using Client.Windows;
using Common;
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

namespace Client.MyControls
{
    /// <summary>
    /// MyEmailsOrNotices.xaml 的交互逻辑
    /// </summary>
    public partial class EmailBox : UserControl
    {
        public EmailBox()
        {
            InitializeComponent();
        }

        #region UI Models

        public class UIModel : BaseUIModel 
        {
            public int Id { get; set; }
        }

        #endregion 

        public Action OnClosing;
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();//页面数据集合

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            OnClosing?.Invoke();
        }

        /// <summary>
        /// 更新邮件
        /// </summary>
        internal void UpdateEmail()
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditEmail editEmailOrNotice = new EditEmail();
            editEmailOrNotice.ShowDialog();
            this.MaskVisible(false);
        }
    }
}
