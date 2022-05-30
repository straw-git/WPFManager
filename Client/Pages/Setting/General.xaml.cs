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

namespace Client.Pages.Setting
{
    /// <summary>
    /// General.xaml 的交互逻辑
    /// </summary>
    public partial class General : Page
    {
        public General()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string pluginUpdateUrl = txtPluginUpdateUrl.Text;
            string loginTitle = txtLoginTitle.Text;

            if (pluginUpdateUrl.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入插件更新路径", "空值提醒");
                txtPluginUpdateUrl.Focus();
                return;
            }

            if (loginTitle.IsNullOrEmpty()) 
            {
                txtLoginTitle.Text = "LOGIN";
            }

            using (CoreDBContext context = new CoreDBContext())
            {
                var setting = context.CoreSetting.Single();
                setting.PluginsUpdateBaseUrl = pluginUpdateUrl;
                setting.LoginTitle = txtLoginTitle.Text;

                context.SaveChanges();
            }

            Notice.Show("设置更新完成！", "成功");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (CoreDBContext context = new CoreDBContext()) 
            {
                var info = context.CoreSetting.First();
                txtPluginUpdateUrl.Text = info.PluginsUpdateBaseUrl;
                txtLoginTitle.Text = info.LoginTitle;
            }
        }
    }
}
