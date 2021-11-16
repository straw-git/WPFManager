
using DBModels.Sys;
using Client.Pages;
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
using Common.Entities;

namespace Client.Windows
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
            using (DBContext context = new DBContext())
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
            tabMenus.Items.Clear();

            var mainMenus = MenuManager.Dic.Keys.OrderBy(c => c.Order).ToList();
            for (int i = 0; i < mainMenus.Count; i++)
            {
                var _menu = mainMenus[i];

                TabItem tabItem = new TabItem();
                tabItem.Header = _menu.Name;

                Grid grid = new Grid();

                ListBox listBox = new ListBox();
                listBox.Width = 150;
                listBox.HorizontalAlignment = HorizontalAlignment.Left;
                listBox.SelectionChanged += MenuListBox_SelectionChanged;
                listBox.MouseDoubleClick += MenuListBox_MouseDoubleClick;

                var childrens = MenuManager.Dic[_menu];
                for (int j = 0; j < childrens.Count; j++)
                {
                    var _childrenItem = childrens[j];

                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Horizontal;

                    CheckBox checkBox = new CheckBox();
                    checkBox.Tag = _childrenItem;
                    string code = $"{_childrenItem.ParentCode}-{_childrenItem.Code}";
                    checkBox.IsChecked = UserMenus.Any(c => c == code);
                    checkBox.Loaded += SecondMenuCheckBox_Loaded;
                    checkBox.Unloaded += SecondMenuCheckBox_Unloaded;

                    Label label = new Label();
                    label.Content = _childrenItem.Name;

                    stackPanel.Children.Add(checkBox);
                    stackPanel.Children.Add(label);
                    listBox.Items.Add(stackPanel);
                }

                grid.Children.Add(listBox);
                tabItem.Content = grid;
                tabMenus.Items.Add(tabItem);
            }
        }

        private void MenuListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedItem == null) return;

            StackPanel panel = listBox.SelectedItem as StackPanel;
            CheckBox checkBox = panel.Children[0] as CheckBox;

            checkBox.IsChecked = !checkBox.IsChecked;
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedItem == null) return;
            StackPanel panel = listBox.SelectedItem as StackPanel;
            CheckBox checkBox = panel.Children[0] as CheckBox;

            Grid parentGrid = listBox.Parent as Grid;

            WrapPanel wrapPanel = new WrapPanel();
            wrapPanel.Orientation = Orientation.Horizontal;
            wrapPanel.Margin = new Thickness(160, 0, 0, 0);

            MenuItemModel itemModel = checkBox.Tag as MenuItemModel;

            foreach (var button in itemModel.Buttons)
            {
                string code = $"{itemModel.ParentCode}-{itemModel.Code}-{button.Name}";
                CheckBox cb = new CheckBox();
                cb.Content = button.Content;
                cb.Margin = new Thickness(5);
                cb.Tag = code;
                cb.IsChecked = UserMenus.Any(c => c == code);
                cb.Loaded += MenuButton_Loaded;
                cb.Unloaded += MenuButton_Unloaded;

                wrapPanel.Children.Add(cb);
            }

            if (parentGrid.Children.Count == 2)
            {
                parentGrid.Children.RemoveAt(0);
            }

            parentGrid.Children.Insert(0, wrapPanel);
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
            CheckBox checkBox = sender as CheckBox;
            Grid parentGrid = ((checkBox.Parent as StackPanel).Parent as ListBox).Parent as Grid;
            MenuItemModel itemModel = checkBox.Tag as MenuItemModel;

            if (parentGrid.Children.Count == 2)
            {
                WrapPanel wrapPanel = parentGrid.Children[0] as WrapPanel;
                foreach (var item in wrapPanel.Children)
                {
                    CheckBox button = item as CheckBox;
                    button.IsChecked = false;
                }
            }

            string code = $"{itemModel.ParentCode}-{itemModel.Code}";
            if (UserMenus.Contains(code))
            {
                UserMenus.Remove(code);
            }
        }

        private void SecondMenuCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            MenuItemModel itemModel = checkBox.Tag as MenuItemModel;
            string code = $"{itemModel.ParentCode}-{itemModel.Code}";
            if (!UserMenus.Contains(code))
            {
                UserMenus.Add(code);
            }
        }

        private void MenuButton_Unloaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Checked -= MenuButtonCheckBox_Checked;
            checkBox.Unchecked -= MenuButtonCheckBox_Unchecked;
        }

        private void MenuButton_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            checkBox.Checked += MenuButtonCheckBox_Checked;
            checkBox.Unchecked += MenuButtonCheckBox_Unchecked;
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

            using (DBContext context = new DBContext())
            {
                var user = context.User.Single(c => c.Id == userId);
                user.Menus = menus;

                context.SaveChanges();
            }
        }

        #endregion
    }
}
