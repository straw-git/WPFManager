
using Client.Pages;
using Common.Data.Local;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Client.Windows
{
    /// <summary>
    /// Plugins.xaml 的交互逻辑
    /// </summary>
    public partial class Plugins : WindowX
    {
        public Plugins()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdateDLLs_Click(null, null);
            UpdatePlugins();
        }

        private void UpdatePlugins()
        {
            plugins.Items.Clear();
            var pluginsData = LocalPlugins.Models.OrderBy(c => c.Order).ToList();
            foreach (var item in pluginsData)
            {
                plugins.Items.Add($"{item.DLLPageName}");
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var handler = PendingBox.Show("正在更新插件...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });

            #region 更新插件

            LocalPlugins.Models.Clear();

            for (int i = 0; i < plugins.Items.Count; i++)
            {
                var item = plugins.Items[i];
                LocalPlugins.Models.Add(new LocalPlugins.DBModel() { DLLPageName = item.ToString(), Order = i });
            }
            LocalPlugins.Save();

            #endregion 

            MenuManager.InitMenus();

            await Task.Delay(200);
            handler.UpdateMessage("更新完成！");
            await Task.Delay(200);
            handler.Close();
            Close();
        }

        private void btnUpdateDLLs_Click(object sender, RoutedEventArgs e)
        {
            dlls.Items.Clear();
            //创建一个DirectoryInfo的类
            DirectoryInfo directoryInfo = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}plugins\\");
            //获取当前的目录的文件
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (FileInfo info in fileInfos)
            {
                //获取文件的名称(包括扩展名)
                string fullName = info.FullName;
                //获取文件的扩展名
                string extension = info.Extension.ToLower();
                if (extension == ".dll")
                {
                    if (fullName.EndsWith("Plugin.dll"))
                    {
                        //插件要求命名必须以Plugin结尾的dll文件
                        dlls.Items.Add(info.Name);
                    }
                }
            }

        }

        private void dlls_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dlls.Items.Count > 0 && dlls.SelectedItem != null)
            {
                string dllName = dlls.SelectedItem.ToString();
                string fName = dllName.Substring(0, dllName.IndexOf('.'));
                string sName = "Pages";
                int order = 0;

                string pName = $"{fName}.{sName}";

                if (LocalPlugins.Models.Any(c => c.DLLPageName == pName))
                {
                    MessageBoxX.Show("请重新选择", "当前命名空间已存在");
                    return;
                }

                LocalPlugins.Models.Add(new LocalPlugins.DBModel() { DLLPageName = pName, Order = order });
                LocalPlugins.Save();

                UpdatePlugins();
            }
        }


        private void plugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (plugins.Items.Count > 0 && plugins.SelectedItem != null)
            {
                string pluginName = plugins.SelectedItem.ToString();

                if (pluginName == "Client.Pages")
                {
                    MessageBoxX.Show("此项不允许删除", "错误");
                    return;
                }

                var result = MessageBoxX.Show($"是否确认移除插件{pluginName}", "插件移除提醒", null, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    LocalPlugins.Models.Remove(LocalPlugins.Models.First(c => c.DLLPageName == pluginName));
                    LocalPlugins.Save();

                    UpdatePlugins();
                }
            }
        }

        private void LBoxSort_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(plugins);
                HitTestResult result = VisualTreeHelper.HitTest(plugins, pos);
                if (result == null)
                {
                    return;
                }
                var listBoxItem = FindVisualParent<ListBoxItem>(result.VisualHit);
                if (listBoxItem == null || listBoxItem.Content != plugins.SelectedItem)
                {
                    return;
                }
                DataObject dataObj = new DataObject(listBoxItem.Content.ToString());
                DragDrop.DoDragDrop(plugins, dataObj, DragDropEffects.Move);
            }
        }

        private void LBoxSort_OnDrop(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(plugins);
            var result = VisualTreeHelper.HitTest(plugins, pos);
            if (result == null)
            {
                return;
            }
            //查找元数据
            var sourcePerson = e.Data.GetData(typeof(string)).ToString();
            if (sourcePerson == null)
            {
                return;
            }
            //查找目标数据
            var listBoxItem = FindVisualParent<ListBoxItem>(result.VisualHit);
            if (listBoxItem == null)
            {
                return;
            }
            var targetPerson = listBoxItem.Content.ToString();
            if (ReferenceEquals(targetPerson, sourcePerson))
            {
                return;
            }
            plugins.Items.Remove(sourcePerson);
            plugins.Items.Insert(plugins.Items.IndexOf(targetPerson), sourcePerson);
        }

        //根据子元素查找父元素
        public T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}
