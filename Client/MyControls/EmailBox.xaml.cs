using Client.Events;
using Client.Windows;
using Common;
using Common.Data.Local;
using Common.Events;
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
            private bool isRead = false;
            public bool IsRead
            {
                get => isRead;
                set
                {
                    isRead = value;
                    Icon = isRead ? "\xf2b7" : "\xf0e0";
                    Background = isRead ? new SolidColorBrush(Colors.AliceBlue) : new SolidColorBrush(Colors.Transparent);
                    NotifyPropertyChanged("IsRead");
                    NotifyPropertyChanged("Icon");
                    NotifyPropertyChanged("Background");
                }
            }
            public string Icon { get; set; }
            public string Title { get; set; }
            public string Time { get; set; }
            public Brush Background { get; set; }
        }

        #endregion 

        public Action OnClosing;
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();//页面数据集合

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EmailNotReadChangedEventObserver.Instance.AddEventListener(Codes.EmailNotReadChanged, OnEmailTimer);
            list.ItemsSource = Data;
        }

        private void OnEmailTimer(EmailChangedMessage p)
        {
            if (p.HasNotReadEmail) 
            {
                UIGlobal.RunUIAction(()=> 
                {
                    UpdateEmail();
                });
            }
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

            List<EmailSendTo> emails = new List<EmailSendTo>();//所有邮件

            int userId = UserGlobal.CurrUser.Id;
            List<string> readRoleEmailIds = LocalReadEmail.GetRoleEmail();
            List<string> readAllUserEmailIds = LocalReadEmail.GetAllUserEmail();

            using (CoreDBContext context = new CoreDBContext())
            {
                List<EmailSendTo> _list = context.EmailSendTo
                    .Where(c => c.UserId == 0 && c.RoleId == 0   //所有用户都可以看的
                    || c.UserId == userId   //查找当前用户的
                    || c.RoleId == UserGlobal.CurrUser.RoleId)//查找当前角色的
                    .OrderByDescending(c => c.SendTime)
                    .ToList();

                if (_list != null)
                {
                    foreach (var item in _list)
                    {
                        UIModel model = new UIModel();
                        model.Id = item.Id;

                        model.IsRead = item.IsRead;
                        if (item.UserId == 0 && item.RoleId == 0)
                        {
                            //如果发给全员的
                            model.IsRead = readAllUserEmailIds.Contains(item.Id.ToString());
                        }
                        if (item.RoleId > 0)
                        {
                            //如果发给角色的
                            model.IsRead = readRoleEmailIds.Contains(item.Id.ToString());
                        }
                        model.Icon = model.IsRead ? "\xf2b7" : "\xf0e0";
                        model.Background = model.IsRead ? new SolidColorBrush(Colors.AliceBlue) :new SolidColorBrush(Colors.Transparent) ;
                        model.Time = item.SendTime.ToString("yy.MM.dd hh:mm");
                        model.Title = item.EmailTitle.Length > 9 ? $"{item.EmailTitle.Substring(0, 9)}..." : item.EmailTitle;

                        Data.Add(model);
                    }
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

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null) return;
            var selectedModel = list.SelectedItem as UIModel;
            this.MaskVisible(true);
            EditEmail editEmailOrNotice = new EditEmail(selectedModel.Id);
            editEmailOrNotice.ShowDialog();
            this.MaskVisible(false);

            var dataModel = Data.Single(c => c.Id == selectedModel.Id);
            dataModel.IsRead = true;
        }
    }
}
