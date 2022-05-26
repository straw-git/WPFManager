using Client.Windows;
using Common;
using CoreDBModels;
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
            public bool IsRead { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
            public string Time { get; set; }
        }

        #endregion 

        public Action OnClosing;
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();//页面数据集合

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            OnClosing?.Invoke();
        }

        /// <summary>
        /// 更新邮件
        /// </summary>
        internal void UpdateEmail()
        {
            Data.Clear();

            List<Email> emails = new List<Email>();//所有邮件
            using (CoreDBContext context = new CoreDBContext())
            {

                List<Email> _list = context.Email
                    .Where(c => c.NoticeType == 0 && c.TargetId == UserGlobal.CurrUser.Id//按用户方式发给我的
                    || c.NoticeType == 2//发给所有人的
                    || c.NoticeType == 1 && c.TargetId == UserGlobal.CurrUser.RoleId)//按角色方式发给我的
                    .ToList();

                if (_list != null)
                    foreach (var item in _list)
                    {
                        UIModel model = new UIModel();
                        model.Id = item.Id;
                        model.IsRead = item.IsRead;
                        model.Time = item.SendTime.ToString("yy.MM.dd hh:mm");
                        model.Title = item.Content.Length > 9 ? $"{item.Content.Substring(0, 9)}..." : item.Content;

                        Data.Add(model);
                    }
            }
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
