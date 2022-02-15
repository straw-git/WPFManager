using CorePlugin.Pages;
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

namespace CorePlugin.Windows
{
    /// <summary>
    /// Authorization2User.xaml 的交互逻辑
    /// </summary>
    public partial class Authorization2User : Window
    {
        public bool Succeed = false;
        int userId = 0;

        public Authorization2User(int _userId)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            userId = _userId;
        }

        List<string> UserMenus = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var user = context.User.Single(c => c.Id == userId);
                if (user.Menus.NotEmpty())
                    UserMenus = user.Menus.Split('|').ToList();
            }
            LoadMenu();
        }

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            UserMenus.Clear();
            LoadMenu();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Succeed = true;
            UpdateMenus();
            Close();
        }

        #endregion

        #region Private Method

        private void LoadMenu()
        {
            //tabMenus.Items.Clear();

            //foreach (var plugin in MenuManager.PluginList)
            //{
            //    var mainMenus = plugin.Value.Keys.OrderBy(c => c.SelfOrder).ToList();
            //    for (int i = 0; i < mainMenus.Count; i++)
            //    {
            //        var _menu = mainMenus[i];

            //        TabItem tabItem = new TabItem();
            //        tabItem.Header = _menu.Name;

            //        Grid grid = new Grid();

            //        ListBox listBox = new ListBox();
            //        listBox.MouseDoubleClick += MenuListBox_MouseDoubleClick;

            //        var childrens = plugin.Value[_menu];
            //        for (int j = 0; j < childrens.Count; j++)
            //        {
            //            var _childrenItem = childrens[j];

            //            StackPanel stackPanel = new StackPanel();
            //            stackPanel.Orientation = Orientation.Horizontal;

            //            CheckBox checkBox = new CheckBox();
            //            checkBox.Tag = _childrenItem;
            //            string code = $"{plugin.Key}-{_childrenItem.ParentCode}-{_childrenItem.Code}";
            //            checkBox.IsChecked = UserMenus.Any(c => c == code);
            //            checkBox.Loaded += SecondMenuCheckBox_Loaded;
            //            checkBox.Unloaded += SecondMenuCheckBox_Unloaded;

            //            Label label = new Label();
            //            label.Content = _childrenItem.Name;

            //            stackPanel.Children.Add(checkBox);
            //            stackPanel.Children.Add(label);
            //            listBox.Items.Add(stackPanel);
            //        }

            //        grid.Children.Add(listBox);
            //        tabItem.Content = grid;
            //        tabMenus.Items.Add(tabItem);
            //    }
            //}
            
        }

        private void MenuListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedItem == null) return;

            StackPanel panel = listBox.SelectedItem as StackPanel;
            CheckBox checkBox = panel.Children[0] as CheckBox;

            checkBox.IsChecked = !checkBox.IsChecked;
        }


        private void SecondMenuCheckBox_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Checked -= SecondMenuCheckBox_Checked;
            checkBox.Unchecked -= SecondMenuCheckBox_Unchecked;
        }

        private void SecondMenuCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Checked += SecondMenuCheckBox_Checked;
            checkBox.Unchecked += SecondMenuCheckBox_Unchecked;
        }

        private void SecondMenuCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //CheckBox checkBox = sender as CheckBox;
            //Grid parentGrid = ((checkBox.Parent as StackPanel).Parent as ListBox).Parent as Grid;
            //MenuItemModel itemModel = checkBox.Tag as MenuItemModel;

            //if (parentGrid.Children.Count == 2)
            //{
            //    WrapPanel wrapPanel = parentGrid.Children[0] as WrapPanel;
            //    foreach (var item in wrapPanel.Children)
            //    {
            //        CheckBox button = item as CheckBox;
            //        button.IsChecked = false;
            //    }
            //}

            //string code = $"{itemModel.PluginCode}-{itemModel.ParentCode}-{itemModel.Code}";
            //if (UserMenus.Contains(code))
            //{
            //    UserMenus.Remove(code);
            //}
        }

        private void SecondMenuCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //CheckBox checkBox = sender as CheckBox;
            //MenuItemModel itemModel = checkBox.Tag as MenuItemModel;
            //string code = $"{itemModel.PluginCode}-{itemModel.ParentCode}-{itemModel.Code}";
            //if (!UserMenus.Contains(code))
            //{
            //    UserMenus.Add(code);
            //}
        }

        private void MenuButtonCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string code = checkBox.Tag.ToString();

            if (UserMenus.Contains(code))
            {
                UserMenus.Remove(code);
            }
        }

        private void MenuButtonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string code = checkBox.Tag.ToString();

            if (!UserMenus.Contains(code))
            {
                UserMenus.Add(code);
            }
        }

        private void UpdateMenus()
        {
            string menus = "";
            if (UserMenus.Count > 0)
            {
                foreach (var item in UserMenus)
                {
                    menus += $"{item}|";
                }
                menus = menus.Substring(0, menus.Length - 1);
            }

            using (CoreDBContext context = new CoreDBContext())
            {
                var user = context.User.Single(c => c.Id == userId);
                user.Menus = menus;

                context.SaveChanges();
            }
        }

        #endregion
    }
}
