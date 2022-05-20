using Common;
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
using System.Windows.Shapes;

namespace CorePlugin.Windows
{
    /// <summary>
    /// AddDepartment.xaml 的交互逻辑
    /// </summary>
    public partial class EditDepartment : Window
    {
        int editId = 0;
        bool IsEdit
        {
            get
            {
                return editId > 0;
            }
        }
        public EditDepartment(int _editId = 0)
        {
            InitializeComponent();
            editId = _editId;

            if (IsEdit) InitDepartment();
        }

        private void InitDepartment()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var department = context.Department.First(c => c.Id == editId);
                txtDepartmentName.Text = department.Name;
                txtRemark.Text = department.Remark;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtDepartmentName.NotEmpty()) return;
            string departmentName = txtDepartmentName.Text;
            string remark = txtRemark.Text;
            if (IsEdit)
            {
                //编辑状态
                using (CoreDBContext context = new CoreDBContext())
                {
                    if (context.Department.Any(c => c.Name == departmentName && c.ParentId == 0 && c.Id != editId))
                    {
                        MessageBoxX.Show("当前名称已存在", "数据重复");
                        txtDepartmentName.Focus();
                        return;
                    }
                    var model = context.Department.Single(c => c.Id == editId);
                    model.Name = departmentName;
                    model.Remark = remark;

                    context.SaveChanges();
                }
            }
            else
            {
                //添加状态
                using (CoreDBContext context = new CoreDBContext())
                {
                    if (context.Department.Any(c => c.Name == departmentName && c.ParentId == 0))
                    {
                        MessageBoxX.Show("当前名称已存在", "数据重复");
                        txtDepartmentName.Focus();
                        return;
                    }
                    context.Department.Add(new CoreDBModels.Department()
                    {
                        CreateTime = DateTime.Now,
                        Creator = UserGlobal.CurrUser.Id,
                        DelRemark = "",
                        DelUser = 0,
                        Index = 0,
                        IsDel = false,
                        Name = departmentName,
                        ParentId = 0,//当前就添加一层
                        Remark = remark
                    });
                    context.SaveChanges();
                }
            }

            DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
