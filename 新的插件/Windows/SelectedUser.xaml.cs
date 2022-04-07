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
using System.Windows.Shapes;

namespace NewPlugins.Windows
{
    /// <summary>
    /// SelectedUser.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedUser : Window
    {
        public bool Succeed = false;
        public List<int> Ids = new List<int>();
        int MaxCount = 0;

        /// <summary>
        /// 选择人员
        /// </summary>
        public SelectedUser(int _maxCount)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            MaxCount = _maxCount;
        }

        #region Models

        public class UIModel
        {
            public int Id { get; set; }
            public bool IsChecked { get; set; }
            public bool IsEnable { get; set; }
            public string Name { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
            LoadListAsync();
        }

        #region Private Method

        private async void LoadListAsync()
        {
            ShowLoadingPanel();
            Data.Clear();
            IEnumerable<User> users = null;
            await Task.Run(() =>
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    users = context.User.Where(c=>c.CanLogin);
                    List<User> staffList = users.ToList();
                    for (int i = 0; i < staffList.Count; i++)
                    {
                        var s = staffList[i];
                        Data.Add(new UIModel()
                        {
                            Id = s.Id,
                            IsChecked = false,
                            Name = s.Name
                        });
                    }
                }
            });
            await Task.Delay(300);
            UIGlobal.RunUIAction(() =>
            {
                HideLoadingPanel();
            });
        }

        private void UpdateCount()
        {
            lblCount.Content = Ids.Count;
        }

        #endregion 

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

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (Ids.Count == 0)
            {
                Succeed = false;
                Ids.Clear();
            }
            else
            {
                Succeed = true;
            }
            Close();
        }

        private void cbFocus_Checked(object sender, RoutedEventArgs e)
        {
            int id = (sender as CheckBox).Tag.ToString().AsInt();
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

        private void cbFocus_Unchecked(object sender, RoutedEventArgs e)
        {
            int id = (sender as CheckBox).Tag.ToString().AsInt();
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

    }
}
