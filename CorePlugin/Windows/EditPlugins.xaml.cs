using Common;
using Common.Data.Local;
using Common.Utils;
using Common.Windows;
using CoreDBModels;
using Panuon.UI.Silver;
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

namespace CorePlugin.Windows
{
    /// <summary>
    /// AddPlugins.xaml 的交互逻辑
    /// </summary>
    public partial class EditPlugins : Window
    {
        #region Models

        public class ModuleUIModel : BaseUIModel
        {
            public string TempId { get; set; }//临时Id 有临时Id的项为添加的
            public int ModuleId { get; set; }//模块Id
            public int PluginId { get; set; }
            private string _moduleName = "";//模块名称
            public string ModuleName
            {
                get => _moduleName;
                set
                {
                    _moduleName = value;
                    NotifyPropertyChanged("ModuleName");
                }
            }
            private string iconStr = "";//图标
            public string IconStr
            {
                get => iconStr;
                set
                {
                    iconStr = value;
                    NotifyPropertyChanged("IconStr");
                }
            }
            private string iconText = "";//图标Key
            public string IconText
            {
                get => iconText;
                set
                {
                    iconText = value;
                    NotifyPropertyChanged("IconText");
                }
            }
            public int Order { get; set; }
        }

        public class PageUIModel : BaseUIModel
        {
            public string ModuleTempId { get; set; }
            public string TempId { get; set; }
            public int PageId { get; set; }
            public int ModuleId { get; set; }
            public string PageName { get; set; }
            public string PagePath { get; set; }
            private string icon = "";//图标
            public string Icon
            {
                get => icon;
                set
                {
                    icon = value;
                    NotifyPropertyChanged("Icon");
                }
            }
            public string IconText { get; set; }
            public int Order { get; set; }
        }

        #endregion

        #region List Item Sources

        /// <summary>
        /// 模块列表
        /// </summary>
        ObservableCollection<ModuleUIModel> ModuleData = new ObservableCollection<ModuleUIModel>();
        /// <summary>
        /// 页面列表
        /// </summary>
        ObservableCollection<PageUIModel> PageData = new ObservableCollection<PageUIModel>();

        #endregion

        #region Base Data

        /// <summary>
        /// 编辑状态下的Id
        /// </summary>
        int editId = 0;
        /// <summary>
        /// 是否是编辑状态
        /// </summary>
        bool isEdit
        {
            get { return editId > 0; }
        }

        //
        // 数据库中的数据
        //
        Plugins pluginsDB = null;
        List<PluginsModule> pluginsModulesDB = new List<PluginsModule>();
        List<ModulePage> modulePagesDB = new List<ModulePage>();

        #endregion

        public EditPlugins(int _editId = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            editId = _editId;
            //绑定数据源
            listModules.ItemsSource = ModuleData;
            listPages.ItemsSource = PageData;

            if (isEdit)
            {
                InitPlugins();
            }
        }

        #region 插件操作

        private void InitPlugins()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                pluginsDB = context.Plugins.First(c => c.Id == editId);
                var _modules = context.PluginsModule.Where(c => c.PluginsId == editId).ToList();//获取所有的模块
                pluginsModulesDB.AddRange(_modules);//添加数据进列表
                foreach (var moduleItem in _modules)
                {
                    var _pages = context.ModulePage.Where(c => c.ModuleId == moduleItem.Id).ToList();//当前模块下的所有页面
                    AddModuleUIItem(moduleItem); //添加模块UI
                    if (_pages == null || _pages.Count == 0) continue;
                    modulePagesDB.AddRange(_pages);//添加数据进列表
                }
            }

            #region 填充插件数据

            txtPluginsName.Text = pluginsDB.Name;
            txtPluginsDLLName.Text = pluginsDB.DLLName;
            txtPluginsOrder.Text = pluginsDB.Order.ToString();
            txtPluginsLogoImgName.Text = pluginsDB.LogoImage;
            cbWebDownload.IsChecked = pluginsDB.WebDownload;
            txtConnectionName.Text = pluginsDB.ConnectionName;
            txtConnectionStr.Text = pluginsDB.ConnectionString;

            #endregion

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckPluginUpdate())
            {
                UpdatePlugins();//更新插件信息
                this.Log("插件信息保存成功！");
            }
        }

        #endregion

        #region 关闭

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion 

        #region 窗体拖动

        private void edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #endregion 

        #region 模块操作

        //删除模块
        private void btnDeleteModule_Click(object sender, RoutedEventArgs e)
        {
            int moduleId = (sender as Button).Tag.ToString().AsInt();
            var selectedModule = ModuleData.First(c => c.ModuleId == moduleId);
            var result = MessageBoxX.Show($"是否确认删除模块[{selectedModule.ModuleName}],删除后模块页面也会一起删除？", "模块删除提醒", this, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    context.PluginsModule.Remove(context.PluginsModule.First(c => c.Id == moduleId));
                    context.ModulePage.RemoveRange(context.ModulePage.Where(c => c.ModuleId == moduleId));
                    context.SaveChanges();
                }
                ModuleData.Remove(selectedModule);
                PageData.Clear();
                this.Log($"模块[{selectedModule.ModuleName}]及子项删除成功！");
            }
        }

        //更换页面icon
        private void btnFindIcon_Click(object sender, RoutedEventArgs e)
        {
            IconSelectorDialog iconWindow = new IconSelectorDialog();
            if (iconWindow.ShowDialog() == true)
            {
                PageUIModel selectedPage = listPages.SelectedItem as PageUIModel;//选中的页面
                var _page = PageData.Single(c => c.TempId == selectedPage.TempId);
                _page.Icon = iconWindow.SelectorModel.SelectedIcon;
                _page.IconText = iconWindow.SelectorModel.SelectedText;

                #region 更新数据库

                using (CoreDBContext context = new CoreDBContext())
                {
                    context.ModulePage.Single(c => c.Id == _page.PageId).Icon = _page.IconText;
                    context.SaveChanges();
                }

                #endregion 
            }
        }

        //添加模块
        private void btnAddModule_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckPluginUpdate())
            {
                UpdatePlugins();//更新插件信息
            }

            var first = FontAwesomeCommon.TypeDict.First();
            PluginsModule moduleDB = null;

            #region 向数据库中添加新的模块

            using (CoreDBContext context = new CoreDBContext())
            {
                int order = context.PluginsModule.Any(c => c.PluginsId == editId) ? context.PluginsModule.Where(c => c.PluginsId == editId).Max(c => c.Order) + 1 : 0;
                moduleDB = context.PluginsModule.Add(new PluginsModule()
                {
                    DLLName = txtPluginsDLLName.Text,
                    Icon = first.Key,
                    ModuleName = "新模块",
                    Order = order,
                    PluginsId = pluginsDB.Id
                });
                context.SaveChanges();
            }

            #endregion

            AddModuleUIItem(moduleDB);//添加模块UI
        }

        //更新插件信息
        private bool UpdatePlugins()
        {
            this.Log("正在保存插件信息...");

            #region 赋值

            string pluginName = txtPluginsName.Text;
            string pluginDLLName = txtPluginsDLLName.Text;
            string pluginLogoName = txtPluginsLogoImgName.Text;
            int pluginOrder = 0;
            bool pluginWebDownload = (bool)cbWebDownload.IsChecked;

            #endregion

            #region 验证

            if (!txtPluginsName.NotEmpty()) return false;
            if (!txtPluginsDLLName.NotEmpty()) return false;
            if (!txtPluginsLogoImgName.NotEmpty()) return false;
            if (!int.TryParse(txtPluginsOrder.Text, out pluginOrder)) txtPluginsOrder.Clear();
            if (!txtPluginsOrder.NotEmpty()) return false;

            #endregion

            #region 之前没有信息直接添加 有信息更新

            if (pluginsDB == null)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    pluginsDB = context.Plugins.Add(new Plugins()
                    {
                        DLLName = pluginDLLName,
                        LogoImage = pluginLogoName,
                        Name = pluginName,
                        UpdateTime = DateTime.Now,
                        WebDownload = pluginWebDownload,
                        Order = pluginOrder,
                        ConnectionName = txtConnectionName.Text.Trim(),
                        ConnectionString = txtConnectionStr.Text.Trim()
                    });
                    context.SaveChanges();
                }
            }
            else
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    //更新数据
                    var _plugins = context.Plugins.Single(c => c.Id == pluginsDB.Id);
                    _plugins.DLLName = pluginDLLName;
                    _plugins.LogoImage = pluginLogoName;
                    _plugins.Name = pluginName;
                    _plugins.UpdateTime = DateTime.Now;
                    _plugins.WebDownload = pluginWebDownload;
                    _plugins.Order = pluginOrder;
                    _plugins.ConnectionName = txtConnectionName.Text.Trim();
                    _plugins.ConnectionString = txtConnectionStr.Text.Trim();

                    //更新实体
                    pluginsDB.DLLName = _plugins.DLLName;
                    pluginsDB.LogoImage = _plugins.LogoImage;
                    pluginsDB.Name = _plugins.Name;
                    pluginsDB.UpdateTime = _plugins.UpdateTime;
                    pluginsDB.WebDownload = _plugins.WebDownload;
                    pluginsDB.Order = _plugins.Order;
                    pluginsDB.ConnectionName = _plugins.ConnectionName;
                    pluginsDB.ConnectionString = _plugins.ConnectionString;
                    context.SaveChanges();
                }
            }

            #endregion

            this.Log("插件保存完成...");
            return true;
        }

        //添加模块UI
        private string AddModuleUIItem(PluginsModule _moduleDB)
        {
            ModuleUIModel moduleUIModel = new ModuleUIModel();
            moduleUIModel.ModuleId = _moduleDB.Id;
            moduleUIModel.ModuleName = _moduleDB.ModuleName;
            moduleUIModel.TempId = Guid.NewGuid().ToString();
            moduleUIModel.PluginId = pluginsDB.Id;
            moduleUIModel.IconStr = FontAwesomeCommon.GetUnicode(_moduleDB.Icon);
            moduleUIModel.IconText = _moduleDB.Icon;
            moduleUIModel.Order = _moduleDB.Order;

            ModuleData.Add(moduleUIModel);
            ModuleData.OrderByInt(c => c.Order);//重新排序

            return moduleUIModel.TempId;
        }

        //刷新页面
        private void RefPages(List<ModulePage> _pageDBs, string _moduleTempId)
        {
            PageData.Clear();
            foreach (var pageDB in _pageDBs)
            {
                PageUIModel pageUIModel = new PageUIModel();
                pageUIModel.Icon = FontAwesomeCommon.GetUnicode(pageDB.Icon);
                pageUIModel.ModuleId = pageDB.ModuleId;
                pageUIModel.ModuleTempId = _moduleTempId;
                pageUIModel.PageId = pageDB.Id;
                pageUIModel.PageName = pageDB.PageName;
                pageUIModel.PagePath = pageDB.PagePath;
                pageUIModel.TempId = Guid.NewGuid().ToString();
                pageUIModel.Order = pageDB.Order;
                pageUIModel.IconText = pageDB.Icon;

                PageData.Add(pageUIModel);
            }
            if (PageData.Count > 0) PageData.OrderByInt(c => c.Order);
        }

        //检查插件信息是否需要修改
        private bool CheckPluginUpdate()
        {
            if (pluginsDB == null) return false;

            Plugins _plugins;
            using (CoreDBContext context = new CoreDBContext())
            {
                _plugins = context.Plugins.First(c => c.Id == pluginsDB.Id);//数据库中的值
            }
            //比对与数据库中的是否相同
            return new ObjectComparerCommon<Plugins>().Compare(pluginsDB, _plugins);
        }

        //编辑模块图标
        private void txtModuleIcon_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            string _key = txt.Text;
            string tempId = txt.Tag.ToString();
            string icon = FontAwesomeCommon.GetUnicode(_key);
            TextBoxHelper.SetIcon(txt, icon);
            //更新数据源图标
            ModuleData.First(c => c.TempId == tempId).IconStr = icon;
        }

        //模块选中更改
        private void listModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageData.Clear();//清空列表
            ModuleUIModel selectedModule = listModules.SelectedItem as ModuleUIModel;//选中的模块
            var _pages = modulePagesDB.Where(c => c.ModuleId == selectedModule.ModuleId).ToList();
            RefPages(_pages, selectedModule.TempId);
        }

        //添加页面
        private void btnAddPage_Click(object sender, RoutedEventArgs e)
        {
            string moduleTempId = (sender as Button).Tag.ToString();
            ModuleUIModel selectedModule = ModuleData.First(c => c.TempId == moduleTempId);//选中的模块

            ModulePage pageDB = null;
            using (CoreDBContext context = new CoreDBContext())
            {
                int order = context.ModulePage.Any(c => c.ModuleId == selectedModule.ModuleId) ? context.ModulePage.Where(c => c.ModuleId == selectedModule.ModuleId).Max(c => c.Order) + 1 : 0;
                pageDB = context.ModulePage.Add(new ModulePage()
                {
                    Icon = FontAwesomeCommon.TypeDict.Keys.First(),
                    ModuleId = selectedModule.ModuleId,
                    Order = order,
                    PageName = "新页面",
                    PagePath = "页面路径",
                    PluginsId = selectedModule.PluginId
                });
                context.SaveChanges();
            }

            //添加数据
            PageData.Add(new PageUIModel()
            {
                TempId = Guid.NewGuid().ToString(),
                ModuleTempId = selectedModule.TempId,
                ModuleId = selectedModule.ModuleId,
                Icon = FontAwesomeCommon.GetUnicode(pageDB.Icon),
                IconText = pageDB.Icon,
                Order = pageDB.Order,
                PageId = pageDB.Id,
                PageName = pageDB.PageName,
                PagePath = pageDB.PagePath
            });

            PageData.OrderByInt(c => c.Order);

            this.Log("页面添加成功！");
        }

        //删除模块中的页面
        private void btnDeleteModulePage_Click(object sender, RoutedEventArgs e)
        {
            string tempId = (sender as Button).Tag.ToString();//TempId
            var selectedPage = PageData.First(c => c.TempId == tempId);//选中的数据
            using (CoreDBContext context = new CoreDBContext())
            {
                context.ModulePage.Remove(context.ModulePage.First(c => c.Id == selectedPage.PageId));
                if (context.SaveChanges() > 0)
                    PageData.Remove(selectedPage);//移除数据
                this.Log("删除成功！");
            }
        }

        //页面被编辑
        private void listPages_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var selectedPage = e.Row.Item as PageUIModel;//被编辑的页面数据
            //编辑数据库
            using (CoreDBContext context = new CoreDBContext())
            {
                var _pageDB = context.ModulePage.Single(c => c.Id == selectedPage.PageId);
                _pageDB.Icon = selectedPage.IconText;
                _pageDB.PageName = selectedPage.PageName;
                _pageDB.PagePath = selectedPage.PagePath;
                this.Log("页面编辑成功！");
                if (context.SaveChanges() == 0)
                {
                    #region 编辑失败

                    var _uiPage = PageData.Single(c => c.PageId == selectedPage.PageId);//页面中的原型
                    var _pageRigDB = context.ModulePage.First(c => c.Id == selectedPage.PageId);
                    _uiPage.Icon = FontAwesomeCommon.GetUnicode(_pageRigDB.Icon);
                    _uiPage.IconText = _pageRigDB.Icon;
                    _uiPage.PageName = _pageRigDB.PageName;
                    _uiPage.PagePath = _pageRigDB.PagePath;

                    #endregion

                    this.Log("页面编辑失败！");
                }
            }
        }

        //编辑模块信息
        private void btnEditModule_Click(object sender, RoutedEventArgs e)
        {
            int moduleId = (sender as Button).Tag.ToString().AsInt();//选中的模块
            var selectedModule = ModuleData.First(c => c.ModuleId == moduleId);

            using (CoreDBContext context = new CoreDBContext())
            {
                var _moduleDB = context.PluginsModule.Single(c => c.Id == moduleId);//数据
                _moduleDB.ModuleName = selectedModule.ModuleName;
                _moduleDB.Icon = selectedModule.IconText;
                context.SaveChanges();
            }

            this.Log("模块信息更新完成！");
        }

        #endregion

    }
}
