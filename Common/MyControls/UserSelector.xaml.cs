using Common.Data.Local;
using CoreDBModels;
using Panuon.UI.Silver;
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

namespace Common.MyControls
{
    /// <summary>
    /// UserSelector.xaml 的交互逻辑
    /// </summary>
    public partial class UserSelector : UserControl
    {
        public UserSelector()
        {
            InitializeComponent();
        }

        #region UI Models

        public class DepartmentUIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<DepartmentUIModel> Children { get; set; }
        }

        public class CheckedUserUIModel
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
        }

        #endregion 

        /// <summary>
        /// 当选中项更改
        /// </summary>
        public Action<List<CheckedUserUIModel>> OnCheckedChanged;

        ObservableCollection<DepartmentUIModel> DepartmentData = new ObservableCollection<DepartmentUIModel>();//部门集合
        ObservableCollection<DepartmentPosition> PositionData = new ObservableCollection<DepartmentPosition>();//职位集合

        List<CheckedUserUIModel> CheckedUsers = new List<CheckedUserUIModel>();//选中的用户

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tvDepartment.ItemsSource = DepartmentData;//绑定数据源
            tvPosition.ItemsSource = PositionData;
            tvPosition.DisplayMemberPath = "Name";
            tvPosition.SelectedValuePath = "Id";
        }

        public void ReLoad()
        {
            DepartmentData.Clear();

            using (CoreDBContext context = new CoreDBContext())
            {
                UpdateDepartment(0, context);
            }
        }

        public void RemoveSelctedUser(int _userId)
        {
            if (CheckedUsers.Any(c => c.UserId == _userId)) 
            {
                CheckedUsers.Remove(CheckedUsers.First(c => c.UserId == _userId));
                OnCheckedChanged?.Invoke(CheckedUsers);
            }

            tvPosition_SelectedItemChanged(null, null);//刷新部门
        }

        /// <summary>
        /// 递归查找部门
        /// </summary>
        /// <param name="_parentId"></param>
        /// <param name="_context"></param>
        /// <returns></returns>
        private List<DepartmentUIModel> UpdateDepartment(int _parentId, CoreDBContext _context)
        {
            List<DepartmentUIModel> models = new List<DepartmentUIModel>();
            var list = _context.Department.Where(c => !c.IsDel && c.ParentId == _parentId).OrderBy(c => c.Index).ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    DepartmentUIModel model = new DepartmentUIModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    model.Children = UpdateDepartment(item.Id, _context);

                    if (_parentId == 0)
                        DepartmentData.Add(model);

                    models.Add(model);
                }
            }
            return models;
        }

        /// <summary>
        /// 部门切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvDepartment_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvDepartment.SelectedItem == null) return;

            PositionData.Clear();

            DepartmentUIModel selectedModel = tvDepartment.SelectedItem as DepartmentUIModel;

            using (CoreDBContext context = new CoreDBContext())
            {
                var list = context.DepartmentPosition.Where(c => !c.IsDel && c.DepartmentId == selectedModel.Id).ToList();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        PositionData.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 职位切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvPosition_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvPosition.SelectedItem == null) return;

            wpUsers.Children.Clear();

            DepartmentPosition selectedModel = tvPosition.SelectedItem as DepartmentPosition;

            using (CoreDBContext context = new CoreDBContext())
            {
                var list = context.User.Where(c => !c.IsDel && c.DepartmentPositionId == selectedModel.Id).ToList();

                var skinInfo = LocalSkin.skins.First(c => c.SkinId == LocalSettings.settings.SkinId);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.Height = 30;
                        checkBox.Content = item.RealName;
                        checkBox.Background = ColorHelper.ConvertToSolidColorBrush(skinInfo.SkinColor);
                        checkBox.Foreground = ColorHelper.ConvertToSolidColorBrush(skinInfo.SkinOppositeColor);
                        checkBox.Margin = new Thickness(5);
                        checkBox.Tag = item.Id;
                        if (CheckedUsers.Any(c => c.UserId == item.Id))
                        {
                            checkBox.IsChecked = true;
                        }
                        checkBox.Checked += OnUser_Checked;
                        checkBox.Unchecked += OnUser_Unchecked;
                        CheckBoxHelper.SetCheckBoxStyle(checkBox, CheckBoxStyle.Button);
                        CheckBoxHelper.SetCheckedBackground(checkBox, new SolidColorBrush(Colors.Black));
                        CheckBoxHelper.SetCornerRadius(checkBox, new CornerRadius(5));

                        wpUsers.Children.Add(checkBox);
                    }
                }
            }
        }

        private void OnUser_Unchecked(object sender, RoutedEventArgs e)
        {
            int userId = (int)(sender as CheckBox).Tag;

            if (CheckedUsers.Any(c => c.UserId == userId))
            {
                CheckedUsers.Remove(CheckedUsers.First(c => c.UserId == userId));
                OnCheckedChanged?.Invoke(CheckedUsers);
            }
        }

        private void OnUser_Checked(object sender, RoutedEventArgs e)
        {
            int userId = (int)(sender as CheckBox).Tag;
            string userName = (sender as CheckBox).Content.ToString();

            if (!CheckedUsers.Any(c => c.UserId == userId))
            {
                CheckedUsers.Add(new CheckedUserUIModel()
                {
                    UserId = userId,
                    UserName = userName
                });
                OnCheckedChanged?.Invoke(CheckedUsers);
            }
        }
    }
}
