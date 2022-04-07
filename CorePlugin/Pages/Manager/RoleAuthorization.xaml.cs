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

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// RoleAuthorization.xaml 的交互逻辑
    /// </summary>
    public partial class RoleAuthorization : Page
    {
        public RoleAuthorization()
        {
            InitializeComponent();
            this.StartPageInAnimation();
        }

        #region Models

        public class RoleUIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            private int pageCount = 0;
            public int PageCount
            {
                get => pageCount;
                set
                {
                    pageCount = value;
                    NotifyPropertyChanged("PageCount");
                    NotifyPropertyChanged("PageColor");
                }
            }
            public Brush PageColor
            {
                get
                {
                    return pageCount > 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                }
            }
        }

        #endregion

        ObservableCollection<RoleUIModel> RoleData = new ObservableCollection<RoleUIModel>();//角色页面数据集合

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lbRoles.ItemsSource = RoleData;//绑定角色数据源
            UpdateRoles();
            HideLoading();
            ShowEmpty();
        }

        #region Roles

        //更新角色列表
        private void UpdateRoles()
        {
            btnRefRoles.IsEnabled = true;
            RoleData.Clear();
            List<Role> roles = null;
            using (CoreDBContext context = new CoreDBContext())
            {
                roles = context.Role.Where(c => !c.IsDel && c.Name != "超级管理员").ToList();//获取角色列表
                foreach (var r in roles)
                {
                    int pageCount = 0;

                    #region 查找当前角色的页面权限数量
                    if (context.RolePlugins.Any(c => c.RoleId == r.Id))
                    {
                        string pages = context.RolePlugins.First(c => c.RoleId == r.Id).Pages;
                        if (pages.NotEmpty())
                            pageCount = pages.Split(',').Length;
                    }
                    #endregion

                    RoleUIModel roleUIModel = new RoleUIModel();
                    roleUIModel.Id = r.Id;
                    roleUIModel.Name = r.Name;
                    roleUIModel.PageCount = pageCount;
                    RoleData.Add(roleUIModel);
                }
            }
            btnRefRoles.IsEnabled = false;
        }

        //角色切换事件
        private async void lbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RoleUIModel selectedModel = lbRoles.SelectedItem as RoleUIModel;//选中的项
            if (selectedModel == null) ShowEmpty();
            ShowLoading();
            atPages.LoadPageInfoByRoleIdAsync(selectedModel.Id);
            await Task.Delay(200);
            HideLoading();
            ShowEmpty(false);
        }

        #endregion

        #region Loading

        private void ShowLoading()
        {
            gLoading.Visibility = Visibility.Visible;
            atPages.IsEnabled = false;
        }

        private void HideLoading()
        {
            gLoading.Visibility = Visibility.Collapsed;
            atPages.IsEnabled = true;
        }

        #endregion

        #region Empty

        private void ShowEmpty(bool _show = true)
        {
            bNoData.Visibility = _show ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        //保存
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            RoleUIModel selectedModel = lbRoles.SelectedItem as RoleUIModel;//选中的角色
            if (selectedModel == null) return;
            List<string> result = atPages.GetResult();//获取选中的页面Id集合

            using (CoreDBContext context = new CoreDBContext())
            {
                string pages = result.Count == 0 ? "" : string.Join(",", result);

                var rolePlugins = context.RolePlugins.Single(c => c.RoleId == selectedModel.Id);//获取角色页面
                rolePlugins.Pages = pages;
                rolePlugins.UpdateTime = DateTime.Now;
                context.SaveChanges();
                //更新UI
                RoleData.Single(c => c.Id == selectedModel.Id).PageCount = result.Count;

                this.Log("角色授权成功！");
            }
        }
    }
}
