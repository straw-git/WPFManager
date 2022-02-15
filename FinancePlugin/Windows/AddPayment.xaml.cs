
using FinanceDBModels.Models;
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

namespace FinancePlugin.Windows
{
    /// <summary>
    /// AddPayment.xaml 的交互逻辑
    /// </summary>
    public partial class AddPayment : Window
    {

        bool isEdit = false;
        public Payment Model = new Payment();
        public bool Succeed = false;
        int modelId = 0;

        public AddPayment(int payModelId, int _id = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();
            modelId = payModelId;
            if (_id > 0)
            {
                isEdit = true;
                btnSubmit.Content = "编辑账户";
                using (FinanceDBContext context = new FinanceDBContext())
                {
                    Model = context.Payment.First(c => c.Id == _id);
                    modelId = Model.Id;
                }
                Model2UI();
            }
            else
            {
                isEdit = false;
                btnSubmit.Content = "添加账户";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!UI2Model()) return;
            using (FinanceDBContext context = new FinanceDBContext())
            {
                if (isEdit) 
                {
                    MessageBox.Show("编辑未完成");
                    return;
                }
                else
                {
                    Model.CreateTime = DateTime.Now;
                    Model.Creator = Common.UserGlobal.CurrUser.Id;
                    Model.IsDel = false;
                    Model.DelTime = DateTime.Now;
                    Model.DelUser = 0;

                    Model = context.Payment.Add(Model);
                }

                context.SaveChanges();
            }

            Succeed = true;
            Close();
        }

        private void Model2UI()
        {
            txtCardId.Text = Model.CardId;
            txtHolder.Text = Model.Holder;
            txtPrice.Text = Model.Price.ToString();
            txtRemark.Text = Model.Remark;
        }

        private bool UI2Model()
        {
            if (string.IsNullOrEmpty(txtCardId.Text))
            {
                MessageBoxX.Show("请输入账号", "空值提醒");
                txtCardId.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtHolder.Text))
            {
                MessageBoxX.Show("请输入持有人", "空值提醒");
                txtHolder.Focus();
                return false;
            }
            decimal price = 0;
            if (!string.IsNullOrEmpty(txtPrice.Text))
            {
                if (!decimal.TryParse(txtPrice.Text, out price))
                {
                    MessageBoxX.Show("余额格式不正确", "格式错误");
                    txtPrice.Focus();
                    txtPrice.SelectAll();
                    return false;
                }
            }

            Model.Price = price;
            Model.CardId = txtCardId.Text;
            Model.Holder = txtHolder.Text;
            Model.PayModelId = modelId;
            Model.Remark = txtRemark.Text;

            return true;
        }
    }
}
