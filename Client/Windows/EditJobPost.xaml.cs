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
    /// EditJobPost.xaml 的交互逻辑
    /// </summary>
    public partial class EditJobPost : Window
    {
        public bool Succeed = false;
        private bool isEdit = false;
        private string parentCode = "";
        public EditJobPost(string _parentName, string _parentCode, bool _edit = false, SysDic _dic = null)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            lblParentName.Text = _parentName;

            isEdit = _edit;
            if (_edit)
            {
                txtName.Text = _dic.Name;
                txtContent.Text = _dic.Content;

                btnSubmit.Content = "编 辑";
            }
            else 
            {
                parentCode = _parentCode;
                btnSubmit.Content = "添 加";
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtName.IsEmpty("名称")) return;

            string newCode = $"{parentCode}-{txtName.Text.Convert2Pinyin()}";

            using (var context = new DBContext())
            {
                if (isEdit)
                {
                    //编辑
                    SysDic model = context.SysDic.Single(c => c.QuickCode == newCode);
                    model.Content = txtContent.Text;
                    model.Name = txtName.Text;

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
                        ParentCode = parentCode,
                        QuickCode = newCode
                    };

                    context.SysDic.Add(model);
                    context.SaveChanges();
                }
            }

            Succeed = true;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }
    }
}
