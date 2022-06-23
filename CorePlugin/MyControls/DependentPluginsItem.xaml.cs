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
    /// DependentPluginsItem.xaml 的交互逻辑
    /// </summary>
    public partial class DependentPluginsItem : UserControl
    {
        public CoreDBModels.Plugins Model = null;
        int currPluginsId = 0;

        public DependentPluginsItem( int _currPluginsId, CoreDBModels.Plugins _model)
        {
            InitializeComponent();
            Model = _model;
            currPluginsId = _currPluginsId;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lblName.Content = Model.Name;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            b.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            b.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show($"是否确认取消[{Model.Name}]的依赖？", "取消依赖提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    string dependentIds = context.Plugins.First(c => c.Id == currPluginsId).DependentIds;

                    List<string> dependentIdArr = dependentIds.Split('|').ToList();
                    dependentIdArr.Remove(Model.Id.ToString());
                    dependentIds = "";
                    for (int i = 0; i < dependentIdArr.Count; i++)
                    {
                        dependentIds += $"|{dependentIdArr[i]}";
                    }

                    if (dependentIds.NotEmpty()) dependentIds = dependentIds.Substring(1);

                    context.Plugins.Single(c => c.Id == currPluginsId).DependentIds = dependentIds;
                    if (context.SaveChanges() > 0)
                    {
                        (Parent as StackPanel).Children.Remove(this);
                    }
                }
            }
        }
    }
}
