using CoreDBModels;
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

namespace CorePlugin.MyControls
{
    /// <summary>
    /// AuthorizationItems.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorizationItems : UserControl
    {
        public AuthorizationItems()
        {
            InitializeComponent();
        }

        #region 数据

        List<Plugins> plugins;//所有插件数据
        List<PluginsModule> modules;//所有模块数据
        List<ModulePage> pages;//所有页面数据

        List<int> currUserPages;//当前用户的页面权限 Id
        List<ListBox> listBoxes;//当前页面的所有ListBox

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 加载插件中的页面信息
        /// </summary>
        /// <param name="_roleId"></param>
        public async void LoadPageInfoByRoleIdAsync(int _roleId)
        {
            tabMenus.Items.Clear();
            currUserPages = new List<int>();//初始化当前页面数据
            listBoxes = new List<ListBox>();

            await Task.Run(() =>
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    string rolePluginsStr = context.RolePlugins.Any(c => c.RoleId == _roleId) ? context.RolePlugins.First(c => c.RoleId == _roleId).Pages : "";
                    if (rolePluginsStr.NotEmpty())
                        currUserPages = rolePluginsStr.Split(',').ToList().String2Int();//将List<string>转为List<int>

                    plugins = context.Plugins.OrderBy(c => c.Order).ToList();
                    modules = context.PluginsModule.ToList();
                    pages = context.ModulePage.ToList();
                }
            });

            for (int i = 0; i < plugins.Count; i++)
            {
                var p = plugins[i];//插件
                var pModules = modules.Where(c => c.PluginsId == p.Id).OrderBy(c => c.Order).ToList();//插件下的模块

                if (pModules != null && pModules.Count > 0)
                    for (int j = 0; j < pModules.Count; j++)
                    {
                        var m = pModules[j];//模块

                        TabItem mTabItem = new TabItem();
                        mTabItem.Header = m.ModuleName;

                        Grid mGrid = new Grid();
                        ListBox mListBox = new ListBox();
                        mGrid.Children.Add(mListBox);

                        listBoxes.Add(mListBox);
                        mTabItem.Content = mGrid;
                        tabMenus.Items.Add(mTabItem);
                        var mPages = pages.Where(c => c.ModuleId == m.Id).OrderBy(c => c.Order).ToList();//模块下的页面

                        if (mPages != null && mPages.Count > 0)
                            for (int z = 0; z < mPages.Count; z++)
                            {
                                var page = mPages[z];//页面

                                CheckBox pCheckBox = new CheckBox();
                                pCheckBox.Content = page.PageName;
                                pCheckBox.Margin = new Thickness(5);
                                pCheckBox.Tag = page.Id;
                                pCheckBox.IsChecked = currUserPages.Contains(page.Id);
                                mListBox.Items.Add(pCheckBox);
                            }
                    }
            }

            if (tabMenus.Items.Count > 0) tabMenus.SelectedIndex = 0;
        }

        /// <summary>
        /// 加载插件中的页面信息
        /// <paramref name="_userId">账户Id</paramref>
        /// </summary>
        public async void LoadPagesInfoByUserIdAsync(int _userId)
        {
            tabMenus.Items.Clear();
            currUserPages = new List<int>();//初始化当前页面数据
            listBoxes = new List<ListBox>();

            await Task.Run(() =>
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    #region 此处逻辑 与 客户端=》加载权限 部分相同

                    var userModel = context.User.First(c => c.Id == _userId);//用户数据
                    string rolePluginsStr = context.RolePlugins.Any(c => c.RoleId == userModel.RoleId) ? context.RolePlugins.First(c => c.RoleId == userModel.RoleId).Pages : "";
                    UserPlugins userPlugins = context.UserPlugins.FirstOrDefault(c => c.UserId == userModel.Id);//获取用户自定义权限
                    if (userPlugins != null && userPlugins.Id > 0)
                    {
                        if (userPlugins.IncreasePages.NotEmpty())
                        {
                            //在角色权限基础上的增加页面
                            string[] increasePages = userPlugins.IncreasePages.Split(',');
                            foreach (var _iPage in increasePages)
                            {
                                int _pageId = 0;
                                if (int.TryParse(_iPage, out _pageId))
                                {
                                    string _iStr = rolePluginsStr.NotEmpty() ? $",{_pageId}" : _pageId.ToString();
                                    rolePluginsStr += _iStr;//将字符串追加到末尾
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        if (userPlugins.DecrementPages.NotEmpty())
                        {
                            //在角色权限基础上的减少页面
                            string[] decrementPages = userPlugins.DecrementPages.Split(',');
                            List<string> _currRoles = rolePluginsStr.Split(',').ToList();//当前的所有角色
                            bool _currRolesUpdate = false;//当前所有角色是否更新
                            foreach (var _iPage in decrementPages)
                            {
                                if (_iPage.NotEmpty())
                                {
                                    if (_currRoles.Contains(_iPage))
                                    {
                                        _currRolesUpdate = true;
                                        _currRoles.Remove(_iPage); //如果有这一项 移除
                                    }
                                }
                                else { continue; }
                            }
                            if (_currRolesUpdate)
                                rolePluginsStr = string.Join(",", _currRoles); //如果有更改，重新整理移除后的字符串
                        }
                    }
                    if (rolePluginsStr.NotEmpty())
                        currUserPages = rolePluginsStr.Split(',').ToList().String2Int();//将List<string>转为List<int>

                    #endregion

                    plugins = context.Plugins.OrderBy(c => c.Order).ToList();
                    modules = context.PluginsModule.ToList();
                    pages = context.ModulePage.ToList();
                }
            });

            for (int i = 0; i < plugins.Count; i++)
            {
                var p = plugins[i];//插件
                var pModules = modules.Where(c => c.PluginsId == p.Id).OrderBy(c => c.Order).ToList();//插件下的模块

                if (pModules != null && pModules.Count > 0)
                    for (int j = 0; j < pModules.Count; j++)
                    {
                        var m = pModules[j];//模块

                        TabItem mTabItem = new TabItem();
                        mTabItem.Header = m.ModuleName;

                        Grid mGrid = new Grid();
                        ListBox mListBox = new ListBox();
                        mGrid.Children.Add(mListBox);

                        listBoxes.Add(mListBox);
                        mTabItem.Content = mGrid;
                        tabMenus.Items.Add(mTabItem);
                        var mPages = pages.Where(c => c.ModuleId == m.Id).OrderBy(c => c.Order).ToList();//模块下的页面

                        if (mPages != null && mPages.Count > 0)
                            for (int z = 0; z < mPages.Count; z++)
                            {
                                var page = mPages[z];//页面

                                CheckBox pCheckBox = new CheckBox();
                                pCheckBox.Content = page.PageName;
                                pCheckBox.Margin = new Thickness(5);
                                pCheckBox.Tag = page.Id;
                                pCheckBox.IsChecked = currUserPages.Contains(page.Id);
                                mListBox.Items.Add(pCheckBox);
                            }
                    }
            }

            if (tabMenus.Items.Count > 0) tabMenus.SelectedIndex = 0;
        }

        /// <summary>
        /// 获得选中结果集
        /// </summary>
        public List<string> GetResult()
        {
            List<string> rights = new List<string>();
            foreach (var lb in listBoxes)
            {
                foreach (var che in lb.Items)
                {
                    var _checkBox = che as CheckBox;
                    if ((bool)_checkBox.IsChecked)
                        rights.Add(_checkBox.Tag.ToString());
                }
            }
            return rights;
        }
    }
}
