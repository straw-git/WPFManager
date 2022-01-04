
using DBModels.ERP;
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
using Common.Utils;
using Common;

namespace ERPPlugin
{
    /// <summary>
    /// AddSupplier.xaml 的交互逻辑
    /// </summary>
    public partial class EditSupplier : Window
    {
        bool isEdit = false;
        int id = 0;
        public Supplier Model = new Supplier();
        public bool Succeed = false;

        public EditSupplier(int _id = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            id = _id;
            if (_id > 0)
            {
                isEdit = true;
                using (DBContext context = new DBContext())
                {
                    Model = context.Supplier.First(c => c.Id == _id);
                }
                Title = $"编辑[{Model.Name}]";
                btnSubmit.Content = "编 辑";

                Model2UI();
            }
            else
            {
                isEdit = false;
                Title = "添加供应商";
                btnSubmit.Content = "添 加";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSupplierType();
        }

        #region Private Method

        private void LoadSupplierType()
        {
            var _source = DataGlobal.GetDic(DicData.SupplierType);
            cbType.ItemsSource = _source;
            cbType.DisplayMemberPath = "Name";
            cbType.SelectedValuePath = "Id";

            if (_source.Count > 0)
                cbType.SelectedIndex = 0;
        }

        #region Convert

        private bool UI2Model()
        {
            #region Empty or Error

            if (string.IsNullOrEmpty(txtName.Text))
            {
                txtName.Focus();
                MessageBoxX.Show("名称不能为空", "空值提醒");
                return false;
            }

            if (string.IsNullOrEmpty(txtContactName.Text))
            {
                txtContactName.Focus();
                MessageBoxX.Show("联系人不能为空", "空值提醒");
                return false;
            }

            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                txtPhone.Focus();
                MessageBoxX.Show("联系电话不能为空", "空值提醒");
                return false;
            }

            #endregion

            if (!isEdit)
            {
                using (DBContext context = new DBContext())
                {
                    if (context.Supplier.Any(c => c.Name == txtName.Text))
                    {
                        MessageBoxX.Show("当前供应商已存在", "数据重复");
                        txtName.Focus();
                        txtName.SelectAll();
                        return false;
                    }
                }
            }

            Model = new Supplier();
            Model.Address = txtAddress.Text;
            Model.ContactName = txtContactName.Text;
            Model.Creater = UserGlobal.CurrUser.Id;
            Model.Id = id;
            Model.IsDel = false;
            Model.Name = txtName.Text;
            Model.Phone = txtPhone.Text;
            Model.Qualification = (bool)cbQualification.IsChecked;
            Model.Type = cbType.SelectedValue.ToString().AsInt();
            Model.CreateTime = DateTime.Now;

            return true;
        }

        private void Model2UI()
        {
            txtAddress.Text = Model.Address;
            txtContactName.Text = Model.ContactName;
            txtName.Text = Model.Name;
            txtPhone.Text = Model.Phone;
            cbQualification.IsChecked = Model.Qualification;
            cbType.SelectedValue = Model.Type;
        }

        #endregion

        #endregion

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!UI2Model()) return;
            using (DBContext context = new DBContext())
            {
                if (isEdit)
                {
                    var supplier = context.Supplier.Single(c => c.Id == id);
                    ModelCommon.CopyPropertyToModel(Model, ref supplier);
                }
                else
                {
                    context.Supplier.Add(Model);
                }

                context.SaveChanges();
            }

            MessageBoxX.Show(isEdit ? "编辑成功" : "添加成功", "成功提醒");

            isEdit = true;
            Close();
        }

        #endregion 
    }
}
