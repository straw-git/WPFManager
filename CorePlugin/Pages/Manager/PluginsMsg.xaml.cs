using Common;
using Common.MyAttributes;
using Common.MyControls;
using CoreDBModels;
using CorePlugin.Windows;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// PluginsMsg.xaml 的交互逻辑
    /// </summary>
    public partial class PluginsMsg : Page
    {
        public class UIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DLLName { get; set; }
            public int ModuleCount { get; set; }
            public string ModuleNames { get; set; }
            public int Order { get; set; }
            public string UpdateTimeYear { get; set; }
            public string UpdateTime { get; set; }
            public string ImgSource { get; set; }
        }

        public PluginsMsg()
        {
            InitializeComponent();
            this.StartPageInAnimation();
        }

        //数据
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //绑定数据
            list.ItemsSource = Data;
            UpdateDataAsync();
        }

        //添加插件
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditPlugins editPlugins = new EditPlugins();
            editPlugins.ShowDialog();
            this.MaskVisible(false);
        }

        //删除插件
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            UIModel selectedModel = list.SelectedItem as UIModel;
            var result = MessageBoxX.Show($"是否确认删除模块[{selectedModel.Name}]？", "删除提醒", MainWindowGlobal.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext()) 
                {
                    context.Plugins.Remove(context.Plugins.First(c=>c.Id==selectedModel.Id));
                    context.PluginsModule.RemoveRange(context.PluginsModule.Where(c=>c.PluginsId==selectedModel.Id));
                    context.ModulePage.RemoveRange(context.ModulePage.Where(c=>c.PluginsId==selectedModel.Id)) ;
                    if (context.SaveChanges() > 0) 
                    {
                        Data.Remove(selectedModel);
                        this.Log("模块删除成功！");
                    }
                }
            }
        }

        //编辑插件
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            UIModel selectedModel = list.SelectedItem as UIModel;
            this.MaskVisible(true);
            EditPlugins editPlugins = new EditPlugins(selectedModel.Id);
            editPlugins.ShowDialog();
            this.MaskVisible(false);
        }

        #region Grid Common

        #region 全选

        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)(sender as CheckBox).IsChecked;
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].IsChecked != isCheck)
                {
                    if (isCheck)
                        //没选中变成选中
                        selectedMenus.IncrementNumber(Data[i]);
                    else
                        //选中变成没选中
                        selectedMenus.DecrementNumber(Data[i]);
                }
                Data[i].IsChecked = isCheck;
            }
        }

        #endregion

        #region 按钮选中

        private void cbFocusItem_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectItem = list.SelectedItem as BaseUIModel;
                selectItem.IsChecked = !selectItem.IsChecked;
                if (selectItem.IsChecked) selectedMenus.IncrementNumber(selectItem);
                else selectedMenus.DecrementNumber(selectItem);
            }
        }

        #endregion

        #endregion

        #region Pager

        /// <summary>
        /// 绑定数据
        /// </summary>
        private async void UpdateDataAsync()
        {
            btnRef.IsEnabled = false;
            Data.Clear();
            await Task.Delay(100);
            List<Plugins> plugins = new List<Plugins>();
            using (CoreDBContext context = new CoreDBContext())
            {
                //按条件搜索
                string searchText = txtSearchText.Text.Trim();
                plugins = searchText.IsNullOrEmpty()
                    ? context.Plugins.ToList()
                    :context.Plugins.Where(c=>c.Name.Contains(searchText)||c.DLLName.Contains(searchText)).ToList();

                bNoData.Visibility = plugins.Count > 0 ? Visibility.Collapsed : Visibility.Visible;//显示
                foreach (var pluginInfo in plugins)
                {
                    var modules = context.PluginsModule.Where(c => c.PluginsId == pluginInfo.Id).ToList();
                    string titles = "";
                    foreach (var m in modules)
                    {
                        titles += $"{m.ModuleName} ";
                    }
                    UIModel model = new UIModel();
                    model.Id = pluginInfo.Id;
                    model.ModuleCount = modules.Count;
                    model.ModuleNames = titles;
                    model.Order = pluginInfo.Order;
                    model.Name = pluginInfo.Name;
                    model.DLLName = pluginInfo.DLLName;
                    model.UpdateTimeYear = pluginInfo.UpdateTime.Year.ToString();
                    model.UpdateTime = pluginInfo.UpdateTime.ToString("MM-dd HH:mm:ss");
                    model.ImgSource = "";

                    Data.Add(model);
                }
            }
            btnRef.IsEnabled = true;
        }

        //刷新
        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataAsync();
        }

        #endregion

        //搜索
        private void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                UpdateDataAsync();
            }
        }
    }
}
