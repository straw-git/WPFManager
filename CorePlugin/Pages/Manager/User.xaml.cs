﻿
using CorePlugin.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Common.Windows;
using System.Reflection;

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class User : BasePage
    {
        public User()
        {
            InitializeComponent();
            this.Order = 1;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }

            private string name = "";
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
            private string roleName = "";
            public string RoleName //角色名称
            {
                get => roleName;
                set
                {
                    roleName = value;
                    NotifyPropertyChanged("RoleName");
                }
            }

            private string staffName = "";
            public string StaffName
            {
                get => staffName;
                set
                {
                    staffName = value;
                    NotifyPropertyChanged("StaffName");
                }
            }
            private string canLogin = "";
            public string CanLogin
            {
                get => canLogin;
                set
                {
                    canLogin = value;
                    NotifyPropertyChanged("CanLogin");
                }
            }

            public string CreateTime { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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
            list.ItemsSource = Data;
            btnRef_Click(null, null);
        }
        #region Private Method

        private void LoadPager()
        {
            using (var context = new CoreDBContext())
            {
                var users = context.User.Where(c => !c.IsDel);
                dataCount = users.Count();
                Print(users);
            }
            pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);

            if (currPage > pagerCount) currPage = pagerCount;
            gPager.CurrentIndex = currPage;
            gPager.TotalIndex = pagerCount;
        }


        private void Print<T>(IQueryable<T> ts) where T : class
        {
            var list = ts.ToList();
            foreach (var item in list)
            {
                PropertyInfo[] props = null;
                try
                {
                    Type type = item.GetType();
                    object obj = Activator.CreateInstance(type);
                    props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var ss in props)
                    {
                        Console.WriteLine(ss);
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();
            if (running) return;
            running = true;
            Data.Clear();

            List<CoreDBModels.Models.User> models = new List<CoreDBModels.Models.User>();

            await Task.Run(() =>
            {
                using (var context = new CoreDBContext())
                {

                    var users = context.User.Where(c => !c.IsDel);

                    models = users.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (CoreDBContext context = new CoreDBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        CreateTime = item.CreateTime.ToString("yyyy年MM月dd日 HH时mm分"),
                        Id = item.Id,
                        Name = item.Name,
                        RoleName = context.SysDic.Any(c => c.Id == item.RoleId) ? context.SysDic.First(c => c.Id == item.RoleId).Name : "超级管理员",
                        StaffName = string.IsNullOrEmpty(item.StaffId) ? "未绑定" : context.Staff.First(c => c.Id == item.StaffId).Name,
                        CanLogin = item.CanLogin ? "允许" : "禁止"
                    };

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        #endregion

        #region UI Method

        private void btnRePwd_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            var selectModel = Data.First(c => c.Id == id);
            var result = MessageBoxX.Show($"是否确认重置[{selectModel.Name}]密码？", "密码初始化提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    var user = context.User.Single(c => c.Id == id);
                    user.Pwd = "123456";
                    if (user.Id == UserGlobal.CurrUser.Id)
                    {
                        UserGlobal.CurrUser.Pwd = user.Pwd;
                    }
                    context.SaveChanges();
                }
                MessageBoxX.Show("初始密码\"123456\"", "密码初始化成功");
            }
        }

        private void btnAuthorization_Click(object sender, RoutedEventArgs e)
        {
            int userId = (sender as Button).Tag.ToString().AsInt();
            ParentWindow.MaskVisible(true);

            Authorization2User a = new Authorization2User(userId);
            a.ShowDialog();

            ParentWindow.MaskVisible(false);

            if (a.Succeed)
            {
                MessageBoxX.Show("请当前用户重新登录", "授权成功");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            UIModel selectModel = Data.First(c => c.Id == id);

            var result = MessageBoxX.Show($"是否确认删除账户[{selectModel.Name}]？", "删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    var user = context.User.Single(c => c.Id == id);
                    if (user.Creator == 0) 
                    {
                        MessageBoxX.Show("系统超级管理员账户不允许被删除","操作失败");
                        return;
                    }
                    user.IsDel = true;
                    user.DelTime = DateTime.Now;
                    user.DelUser = UserGlobal.CurrUser.Id;

                    context.SaveChanges();
                    Data.Remove(selectModel);
                }
            }
        }

        private void btnSelectStaff_Click(object sender, RoutedEventArgs e)
        {
            int id = (sender as Button).Tag.ToString().AsInt();
            UIModel selectModel = Data.First(c => c.Id == id);

            ParentWindow.MaskVisible(true);
            SelectedStaff s = new SelectedStaff(1);
            s.ShowDialog();
            ParentWindow.MaskVisible(false);
            if (s.Succeed)
            {
                string staffId = s.Ids[0];
                using (CoreDBContext context = new CoreDBContext())
                {
                    context.User.Single(c => c.Id == id).StaffId = staffId;
                    context.SaveChanges();

                    var model = Data.Single(c => c.Id == id);
                    model.StaffName = context.Staff.First(c => c.Id == staffId).Name;
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.MaskVisible(true);
            AddUser a = new AddUser();
            a.ShowDialog();
            ParentWindow.MaskVisible(false);
            if (a.Succeed)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    Data.Insert(0, new UIModel()
                    {
                        CreateTime = a.Model.CreateTime.ToString("yyyy年MM月dd日 HH时mm分"),
                        Id = a.Model.Id,
                        Name = a.Model.Name,
                        RoleName = context.SysDic.First(c => c.Id == a.Model.RoleId).Name,
                        StaffName = string.IsNullOrEmpty(a.Model.StaffId) ? "未绑定" : context.Staff.First(c => c.Id == a.Model.StaffId).Name,
                        CanLogin = a.Model.CanLogin ? "允许" : "禁止"
                    });
                }
            }
        }

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
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