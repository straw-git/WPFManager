
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
                plugins.Items.Add($"{item.DLLPageName}:====[ {item.Order} ]");
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var handler = PendingBox.Show("正在更新插件...", "请等待", false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });

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
                gPluginDetails.Visibility = Visibility.Visible;
                lblDLLName.Content = dlls.SelectedItem.ToString();
                txtPageName.Focus();
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
                    pluginName = pluginName.Substring(0, pluginName.IndexOf(':'));
                    LocalPlugins.Models.Remove(LocalPlugins.Models.First(c => c.DLLPageName == pluginName));
                    LocalPlugins.Save();

                    UpdatePlugins();
                }
            }
        }

        private void btnCanel_Click(object sender, RoutedEventArgs e)
        {
            gPluginDetails.Visibility = Visibility.Collapsed;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string dllName = lblDLLName.Content.ToString();
            string fName = dllName.Substring(0, dllName.IndexOf('.'));
            string sName = txtPageName.Text.IsNullOrEmpty() ? "Pages" : txtPageName.Text;
            int order = 0;
            if (txtOrder.Text.IsNullOrEmpty())
            {
                order = 0;
            }
            else
            {
                if (!int.TryParse(txtOrder.Text, out order))
                {
                    MessageBox.Show("请输入正确数值");
                    txtOrder.Focus();
                    txtOrder.SelectAll();
                    return;
                }
            }

            string pName = $"{fName}.{sName}";

            if (LocalPlugins.Models.Any(c => c.DLLPageName == pName))
            {
                MessageBoxX.Show("请重新选择", "当前命名空间已存在");
                gPluginDetails.Visibility = Visibility.Collapsed;
                return;
            }

            LocalPlugins.Models.Add(new LocalPlugins.DBModel() { DLLPageName = pName, Order = order });
            LocalPlugins.Save();

            UpdatePlugins();

            gPluginDetails.Visibility = Visibility.Collapsed;
        }
    }
}
