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
    /// EditDeaprtmentPosition.xaml 的交互逻辑
    /// </summary>
    public partial class EditDeaprtmentPosition : Window
    {
        int departmentId = 0;
        string departmentName = "";
        int editId = 0;
        bool IsEdit
        {
            get { return editId > 0; }
        }

        public EditDeaprtmentPosition(int _departmentId, string _departmentName, int _editId = 0)
        {
            InitializeComponent();
            departmentId = _departmentId;
            departmentName = _departmentName;
            editId = _editId;

            if (IsEdit)
            {
                InitPositionInfo();
                gbPosition.Header = $"[{departmentName}]部门编辑职位";
            }
            else
            {
                gbPosition.Header = $"[{departmentName}]部门添加职位";
            }
        }

        private void InitPositionInfo()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var model = context.DepartmentPosition.First(c => c.Id == editId);
                txtPositionName.Text = model.Name;
                txtRemark.Text = model.Remark;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtPositionName.NotEmpty()) return;

            string positionName = txtPositionName.Text;
            string remark = txtRemark.Text;
            int maxUserCount = 0;

            if (!int.TryParse(txtMaxUserCount.Text, out maxUserCount))
            {
                MessageBoxX.Show("请正确输入上限人数", "解析失败");
                txtMaxUserCount.Focus();
                return;
            }

            if (IsEdit)
            {
                //编辑状态
                using (CoreDBContext context = new CoreDBContext())
                {
                    if (context.DepartmentPosition.Any(c => c.DepartmentId == departmentId && c.Name == positionName && c.Id != editId))
                    {
                        MessageBoxX.Show("当前名称已存在", "重复数据");
                        txtPositionName.Focus();
                        return;
                    }

                    var model = context.DepartmentPosition.Single(c => c.Id == editId);
                    model.Name = positionName;
                    model.MaxUserCount = maxUserCount;
                    model.Remark = remark;

                    context.SaveChanges();
                }
            }
            else
            {
                //添加状态
                using (CoreDBContext context = new CoreDBContext())
                {
                    if (context.DepartmentPosition.Any(c => c.DepartmentId == departmentId && c.Name == positionName))
                    {
                        MessageBoxX.Show("当前名称已存在", "重复数据");
                        txtPositionName.Focus();
                        return;
                    }

                    context.DepartmentPosition.Add(new CoreDBModels.DepartmentPosition()
                    {
                        CreateTime = DateTime.Now,
                        Creator = UserGlobal.CurrUser.Id,
                        DelRemark = "",
                        DelUser = 0,
                        DepartmentId = departmentId,
                        Index = 0,
                        IsDel = false,
                        MaxUserCount = maxUserCount,
                        Name = positionName,
                        Remark = remark
                    });

                    context.SaveChanges();
                }
            }

            DialogResult = true;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
