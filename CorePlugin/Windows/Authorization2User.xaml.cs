using CoreDBModels;
using CorePlugin.Pages;
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
using System.Windows.Shapes;

namespace CorePlugin.Windows
{
    /// <summary>
    /// Authorization2User.xaml 的交互逻辑
    /// </summary>
    public partial class Authorization2User : Window
    {
        public bool Succeed = false;
        int userId = 0;

        public Authorization2User(int _userId)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            userId = _userId;
        }

        List<string> UserMenus = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            atPages.LoadPagesInfoByUserIdAsync(userId);
        }

        private void edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> result = atPages.GetResult();//获取选中的页面Id集合

            using (CoreDBContext context = new CoreDBContext())
            {
                #region 在角色权限中查找 如果角色中有但结果中没有 加入减量 如果角色中没有 但结果中有 加入增量

                var user = context.User.First(c => c.Id == userId);//获取当前 用户
                RolePlugins rolePlugins = context.RolePlugins.First(c => c.RoleId == user.RoleId);//查找当前用户的角色权限 这里因为添加角色的时候已经加入 所以肯定有值的 
                List<string> rolePages = rolePlugins.Pages.Split(',').ToList();//所有角色权限
                List<string> _res1 = rolePages.Where(a => !result.Exists(t => t == a)).ToList();//查找角色中存在 但结果中不存在的数据 此为减少的数据
                List<string> _res2 = result.Where(a => !rolePages.Exists(t => t == a)).ToList();//查找结果中存在 但角色中不存在的数据 此为添加的数据

                #endregion

                string increasePages = string.Join(",", _res2);//增量
                string decrementPages = string.Join(",", _res1);//减量

                if (increasePages.NotEmpty() || decrementPages.NotEmpty())
                {
                    if (context.UserPlugins.Any(c => c.UserId == userId))
                    {
                        //之前存在数据 修改
                        var userPluginsDB = context.UserPlugins.Single(c => c.UserId == userId);
                        userPluginsDB.IncreasePages = increasePages;
                        userPluginsDB.DecrementPages = decrementPages;
                        userPluginsDB.UpdateTime = DateTime.Now;
                    }
                    else
                    {
                        //之前不存在数据 新增
                        UserPlugins userPluginsDB = new UserPlugins();
                        userPluginsDB.DecrementPages = decrementPages;
                        userPluginsDB.IncreasePages = increasePages;
                        userPluginsDB.UpdateTime = DateTime.Now;
                        userPluginsDB.UserId = userId;
                        context.UserPlugins.Add(userPluginsDB);
                    }
                    context.SaveChanges();//执行

                    this.Log("用户授权成功！");
                }
            }
            btnClose_Click(null, null);//模拟点击关闭
        }
    }
}
