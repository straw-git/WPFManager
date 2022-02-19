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

namespace FinancePlugin.Windows
{
    /// <summary>
    /// SelectedStaff.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedStaff : Window
    {
        int roleId = 0;
        public bool Succeed = false;
        public List<string> Ids = new List<string>();
        int MaxCount = 0;

        /// <summary>
        /// 选择人员
        /// </summary>
        public SelectedStaff(int _maxCount, int _roleId = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            roleId = _roleId;
            MaxCount = _maxCount;
        }

        #region Models

        public class UIModel
        {
            public string Id { get; set; }
            public bool IsChecked { get; set; }
            public bool IsEnable { get; set; }
            public string Name { get; set; }
            public string JobPostName { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
            LoadListAsync();
        }

        #region Private Method

        private async void LoadListAsync(string _searchText = "")
        {
            ShowLoadingPanel();
            Data.Clear();
            List<Staff> staffs = new List<Staff>();
            await Task.Run(() =>
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    if (roleId > 0)
                    {
                        var _users = context.User.Where(c => c.RoleId == roleId).ToList();
                        foreach (var user in _users)
                        {
                            if (string.IsNullOrEmpty(user.StaffId)) continue;
                            staffs.Add(context.Staff.First(c => c.Id == user.StaffId));
                        }
                    }
                    else 
                    {
                        staffs = context.Staff.ToList();
                    }

                    if (!string.IsNullOrEmpty(_searchText))
                    {
                        staffs = staffs.Where(c => c.Name.Contains(_searchText) || c.QuickCode.Contains(_searchText)).ToList();
                    }

                    UIGlobal.RunUIAction(() =>
                    {
                        for (int i = 0; i < staffs.Count; i++)
                        {
                            var s = staffs[i];
                            Data.Add(new UIModel()
                            {
                                Id = s.Id,
                                IsChecked = false,
                                IsEnable=true,
                                JobPostName = context.SysDic.First(c => c.Id == s.JobPostId).Name,
                                Name = s.Name
                            });
                        }
                    });
                }
            });
            await Task.Delay(300);
            HideLoadingPanel();
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

        private void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadListAsync(txtSearchText.Text);
            }
        }

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
            string id = (sender as CheckBox).Tag.ToString();
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
            string id = (sender as CheckBox).Tag.ToString();
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
