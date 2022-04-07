using Common;
using Common.MyAttributes;
using Common.MyControls;
using NewPlugins.Windows;
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

namespace NewPlugins.Pages.Manager
{
    /// <summary>
    /// PluginsMsg.xaml 的交互逻辑
    /// </summary>
    public partial class PluginsMsg : Page
    {
        public class UIModel : BaseUIModel
        {
            public int Id { get; set; }
            [DataSourceBinding("名称", -1, 1)]
            public string Title { get; set; }
            [DataSourceBinding("模块数量", 100, 2)]
            public int ModuleCount { get; set; }
            public string ModuleTitles { get; set; }
            public int Order { get; set; }
            [DataSourceBinding("最后更新时间", 150, 3)]
            public DateTime UpdateTime { get; set; }
        }

        public PluginsMsg()
        {
            InitializeComponent();
            this.StartPageInAnimation();

        }

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            myPager.BindingDataGrid<UIModel>(Data, new MyButtonColumn()
            {
                Header = "操作",
                Width = -1,
                Buttons = new List<MyDataGridColumnButtonInfo>()
                {
                    new MyDataGridColumnButtonInfo()
                    {
                        Content="删除",
                        ClickHandler=new RoutedEventHandler(OnDelClick)
                    }
                }
            });
        }

        private void OnDelClick(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Data.Remove(Data.First());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            AddPlugins addPlugins = new AddPlugins();
            addPlugins.ShowDialog();
            this.MaskVisible(false);
        }
    }
}
