using Common.Utils;
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

namespace Common.MyControls
{
    /// <summary>
    /// IconComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class IconComboBox : UserControl
    {
        public class IconSelectorModel
        {
            public string Icon { get; set; }
            public string Key { get; set; }
        }

        public IconSelectorModel SelectorModel;
        public List<IconSelectorModel> list = new List<IconSelectorModel>();

        public IconComboBox()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var key in FontAwesomeCommon.TypeDict.Keys)
            {
                var _val = FontAwesomeCommon.TypeDict[key];
                IconSelectorModel model = new IconSelectorModel()
                {
                    Icon = FontAwesomeCommon.GetUnicode(_val),
                    Key = key
                };
                list.Add(model);
            }
            cbIcons.ItemsSource = list;
            cbIcons.DisplayMemberPath = "Icon";
            cbIcons.SelectedValuePath = "Key";

            cbIcons.SelectedIndex = 0;
        }

        public delegate void SelectionChangedEventHandler(object sender, IconSelectorModel selectorModel);
        public event SelectionChangedEventHandler SelectionChanged;

        private void cbIcons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectorModel = cbIcons.SelectedItem as IconSelectorModel;
            SelectionChanged?.Invoke(this, SelectorModel);
        }
    }
}
