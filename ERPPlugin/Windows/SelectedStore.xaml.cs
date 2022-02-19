using CoreDBModels;
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

namespace ERPPlugin.Windows
{
    /// <summary>
    /// SelectedStore.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedStore : Window
    {
        public SelectedStore()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        public SysDic Model = null;
        public bool Succeed = false;

        List<SysDic> models = new List<SysDic>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                models = context.SysDic.Where(c => c.ParentCode == DicData.Store).ToList();
                if (models == null) models = new List<SysDic>();
                models.Add(new SysDic() 
                {
                    Id=0,
                    Name="不选择任何仓库"
                });
                list.ItemsSource = models;
            }
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null) return;
            Model = list.SelectedItem as SysDic;
            if (Model.Id < 0) return;
            Succeed = true;
            this.Close();
        }
    }
}
