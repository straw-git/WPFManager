
using Common;
using DBModels;
using DBModels.Sys;
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

namespace Client.Windows
{
    /// <summary>
    /// Add.xaml 的交互逻辑
    /// </summary>
    public partial class EditDic : Window
    {
        private SysDic parentInfo = new SysDic();
        private bool isEdit = false;
        private string editCode = "";

        public string ResultName = "";
        public string ResultContent = "";

        public bool Succeed = false;

        public EditDic(string parentCode, bool edit = false, SysDic editModel = null)
        {
            InitializeComponent();

            //父级
            if (parentCode.NotEmpty()) using (DBContext context = new DBContext()) parentInfo = context.SysDic.First(c => c.QuickCode == parentCode);

            isEdit = edit;
            if (edit)
            {
                editCode = editModel.QuickCode;
                Title = "编辑";
                btnSubmit.Content = "编 辑";

                txtName.Text = editModel.Name;
                txtContent.Text = editModel.Content;
            }
            else
            {
                lblParentName.Text = parentCode.NotEmpty() ? parentInfo.Name : "根目录";
                Title = "添加";
                btnSubmit.Content = "添 加";
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtName.IsEmpty("名称")) return;

            using (var context = new DBContext())
            {
                if (isEdit)
                {
                    //编辑
                    SysDic model = context.SysDic.Single(c => c.QuickCode == editCode);
                    model.Content = txtContent.Text;
                    model.Name = txtName.Text;

                    ResultName = txtName.Text;
                    ResultContent = txtContent.Text;

                    context.SaveChanges();
                }
                else
                {
                    //添加
                    SysDic model = new SysDic()
                    {
                        Content = txtContent.Text,
                        Creater = UserGlobal.CurrUser.Id,
                        CreateTime = DateTime.Now,
                        Name = txtName.Text,
                        ParentCode = parentInfo.QuickCode.IsNullOrEmpty()?"": parentInfo.QuickCode,
                        QuickCode = parentInfo.QuickCode.IsNullOrEmpty() ? txtName.Text.Convert2Pinyin() : $"{parentInfo.QuickCode}-{txtName.Text.Convert2Pinyin()}"
                    };

                    context.SysDic.Add(model);
                    context.SaveChanges();
                }
            }

            Succeed = true;
            DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }
    }
}
