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
    /// AddRole.xaml 的交互逻辑
    /// </summary>
    public partial class EditRole : Window
    {
        int editId = 0;
        bool IsEdit
        {
            get { return editId != 0; }
        }
        public EditRole(int _editId = 0)
        {
            InitializeComponent();
            editId = _editId;

            if (IsEdit) InitRole();
        }

        private void InitRole()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var _role = context.Role.First(c => c.Id == editId);
                txtRoleName.Text = _role.Name;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtRoleName.NotEmpty()) return;
            string _roleName = txtRoleName.Text;
            if (IsEdit)
            {
                //编辑模式
                using (CoreDBContext context = new CoreDBContext())
                {
                    var _role = context.Role.Single(c => c.Id == editId);
                    _role.Name = _roleName;

                    context.SaveChanges();
                }
                this.Log("编辑角色成功");
            }
            else
            {
                //添加模式
                using (CoreDBContext context = new CoreDBContext())
                {
                    var role = context.Role.Add(new CoreDBModels.Role()
                    {
                        DelTime = DateTime.Now,
                        DelUser = 0,
                        DelUserName = "",
                        IsDel = false,
                        Name = _roleName
                    });

                    if (context.SaveChanges() > 0)
                    {
                        #region  添加成功后 将角色权限添加

                        context.RolePlugins.Add(new CoreDBModels.RolePlugins()
                        {
                            Pages = "",
                            RoleId = role.Id,
                            UpdateTime = DateTime.Now
                        });
                        context.SaveChanges();

                        #endregion 
                    }
                }
                this.Log("添加角色成功");
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
    }
}
