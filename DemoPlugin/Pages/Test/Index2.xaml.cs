using Common;
using Common.MyAttributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

namespace DemoPlugin.Pages.Test
{
    /// <summary>
    /// Welcome.xaml 的交互逻辑
    /// </summary>
    public partial class Index2 : BasePage
    {
        public Index2()
        {
            InitializeComponent();
        }

        #region Models

        class UIModel : BaseUIModel
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
            using (var context = new DBContext())
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
            if (running) return;
            running = true;
            Data.Clear();

            List<DBModels.Sys.User> models = new List<DBModels.Sys.User>();

            await Task.Run(() =>
            {
                using (var context = new DBContext())
                {

                    var users = context.User.Where(c => !c.IsDel);

                    models = users.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);

            using (DBContext context = new DBContext())
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
            running = false;
        }

        #endregion


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

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var s = list.SelectedItem;
        }
    }
}
