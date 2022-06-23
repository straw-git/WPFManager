using Common;
using Common.Dialogs;
using CorePlugin.Windows;
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

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// DepartmentPositionMsg.xaml 的交互逻辑
    /// </summary>
    public partial class DepartmentPositionMsg : Page
    {
        public DepartmentPositionMsg()
        {
            InitializeComponent();
            this.StartPageInAnimation();
        }

        #region UI Models

        public class DepartmentUIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            private int positionCount = 0;
            public int PositionCount
            {
                get => positionCount;
                set
                {
                    positionCount = value;
                    NotifyPropertyChanged("PositionCount");
                    NotifyPropertyChanged("PageColor");
                }
            }
            public Brush PageColor
            {
                get
                {
                    return PositionCount > 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                }
            }
            public List<DepartmentUIModel> Children { get; set; }
        }

        public class DepartmentPositionUIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int MaxUserCount { get; set; }
            public int UserCount { get; set; }
            public bool DeleteButtonIsEnabled
            {
                get { return UserCount == 0; }
            }

            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }

        #endregion 

        ObservableCollection<DepartmentUIModel> DepartmentData = new ObservableCollection<DepartmentUIModel>();//部门集合
        ObservableCollection<DepartmentPositionUIModel> PositionData = new ObservableCollection<DepartmentPositionUIModel>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tvDepartment.ItemsSource = DepartmentData;//绑定数据源
            list.ItemsSource = PositionData;
            ReLoadDepartment();
        }

        private void ReLoadDepartment()
        {
            DepartmentData.Clear();

            using (CoreDBContext context = new CoreDBContext())
            {
                UpdateDepartment(0, context);
            }
        }

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
                    if (model.Children.Count > 0)
                    {
                        model.PositionCount = model.Children.Sum(c => c.PositionCount);
                    }
                    else
                    {
                        model.PositionCount = _context.DepartmentPosition.Any(c => c.DepartmentId == item.Id)
                            ? _context.DepartmentPosition.Count(c => c.DepartmentId == item.Id)
                            : 0;
                    }

                    if (_parentId == 0)
                        DepartmentData.Add(model);

                    models.Add(model);
                }
            }

            return models;
        }

        private void btnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditDepartment editDepartment = new EditDepartment();
            if (editDepartment.ShowDialog() == true)
            {
                ReLoadDepartment();
            }
            this.MaskVisible(false);
        }

        private void btnEditDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (tvDepartment.SelectedItem == null) return;
            var selectedModel = tvDepartment.SelectedItem as DepartmentUIModel;//选中的部门
            this.MaskVisible(true);
            EditDepartment editDepartment = new EditDepartment(selectedModel.Id);
            if (editDepartment.ShowDialog() == true)
            {
                ReLoadDepartment();
            }
            this.MaskVisible(false);
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            ReLoadDepartment();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selectedModel = tvDepartment.SelectedItem as DepartmentUIModel;//选中的部门
            if (tvDepartment.SelectedItem == null || selectedModel == null)
            {
                MessageBoxX.Show("没有选中的部门,请先选择部门", "数据缺失");
                return;
            }

            this.MaskVisible(true);
            EditDeaprtmentPosition editDeaprtmentPosition = new EditDeaprtmentPosition(selectedModel.Id, selectedModel.Name);
            if (editDeaprtmentPosition.ShowDialog() == true)
            {
                //添加成功了
                tvDepartment_SelectedItemChanged(null, null);//刷新列表
            }

            this.MaskVisible(false);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null) return;
            var selectedModel = list.SelectedItem as DepartmentPositionUIModel;
            this.MaskVisible(true);

            DeleteVRemarkDialog deleteDepartmentDialog = new DeleteVRemarkDialog($"是否确认删除职位[{selectedModel.DepartmentName}-{selectedModel.Name}]？");
            if (deleteDepartmentDialog.ShowDialog() == true)
            {
                string delRemark = deleteDepartmentDialog.GetRemark();
                //确认删除
                using (CoreDBContext context = new CoreDBContext())
                {
                    context.DepartmentPosition.Remove(context.DepartmentPosition.First(c => c.Id == selectedModel.Id));
                    if (context.SaveChanges() == 1)
                    {
                        //成功删除
                        PositionData.Remove(selectedModel);//更新列表UI
                    }
                    else
                    {
                        //删除失败
                        MessageBoxX.Show("删除失败", "数据库错误");
                    }
                }
            }

            this.MaskVisible(false);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem == null) return;
            var selectedModel = list.SelectedItem as DepartmentPositionUIModel;
            this.MaskVisible(true);
            EditDeaprtmentPosition editDeaprtmentPosition = new EditDeaprtmentPosition(selectedModel.DepartmentId, selectedModel.DepartmentName, selectedModel.Id);
            if (editDeaprtmentPosition.ShowDialog() == true)
            {
                tvDepartment_SelectedItemChanged(null, null);
            }

            this.MaskVisible(false);
        }

        private async void tvDepartment_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvDepartment.SelectedItem == null) return;

            gLoading.Visibility = Visibility.Visible;

            PositionData.Clear();
            var selectedModel = tvDepartment.SelectedItem as DepartmentUIModel;//选中的部门

            using (CoreDBContext context = new CoreDBContext())
            {
                var list = context.DepartmentPosition.Where(c => !c.IsDel && c.DepartmentId == selectedModel.Id).ToList();

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        DepartmentPositionUIModel model = new DepartmentPositionUIModel();
                        model.Id = item.Id;
                        model.Name = item.Name;
                        model.MaxUserCount = item.MaxUserCount;
                        model.UserCount = context.User.Any(c => c.DepartmentPositionId == item.Id)
                        ? context.User.Count(c => c.DepartmentPositionId == item.Id)
                        : 0;
                        model.DepartmentId = selectedModel.Id;
                        model.DepartmentName = selectedModel.Name;

                        PositionData.Add(model);
                    }
                }
            }

            await Task.Delay(500);

            if (PositionData.Count == 0)
                bNoData.Visibility = Visibility.Visible;
            else bNoData.Visibility = Visibility.Collapsed;

            gLoading.Visibility = Visibility.Collapsed;
        }

        private void btnDeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能未实现");
        }

        private void btnDeleteDepartmentM_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能未实现");
        }
    }
}
