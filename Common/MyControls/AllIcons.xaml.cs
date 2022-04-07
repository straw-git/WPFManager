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
    /// AllIcons.xaml 的交互逻辑
    /// </summary>
    public partial class AllIcons : UserControl
    {
        public class IconSelectorModel
        {
            public string SelectedIcon { get; set; }
            public string SelectedText { get; set; }
        }

        public IconSelectorModel SelectorModel;

        public AllIcons()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            uicons.Children.Clear();
            foreach (var key in FontAwesomeCommon.TypeDict.Keys)
            {
                var _val = FontAwesomeCommon.TypeDict[key];
                string iconStr = FontAwesomeCommon.GetUnicode(_val);

                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new SolidColorBrush(Colors.LightGray);
                border.Margin = new Thickness(5);
                border.MouseDown += Border_MouseDown;

                StackPanel stackPanel = new StackPanel();
                stackPanel.VerticalAlignment = VerticalAlignment.Center;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

                Label iconLabel = new Label();
                iconLabel.Content = iconStr;
                iconLabel.FontFamily = (FontFamily)FindResource("FontAwesome");
                iconLabel.FontSize = 25;
                iconLabel.HorizontalAlignment = HorizontalAlignment.Center;

                Label textLabel = new Label();
                textLabel.Content = key;

                stackPanel.Children.Add(iconLabel);
                stackPanel.Children.Add(textLabel);

                border.Tag = new IconSelectorModel()
                {
                    SelectedIcon = iconStr,
                    SelectedText = key
                };
                border.Child = stackPanel;
                uicons.Children.Add(border);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectorModel = (sender as Border).Tag as IconSelectorModel;
        }
    }
}
