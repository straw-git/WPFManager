using Common;
using Common.Utils;
using Common.Windows;
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

namespace CustomerPlugin
{
    /// <summary>
    /// EditCustomer.xaml 的交互逻辑
    /// </summary>
    public partial class EditCustomer : Window
    {
        public int id = 0;
        public DBModels.Member.Customer Result = null;
        public bool Succeed = false;

        public EditCustomer(int _id = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            id = _id;

            if (_id == 0) btnSubmit.Content = "登 记";
            if (_id > 0) LoadData();
        }

        private void LoadData()
        {
            using (DBContext context = new DBContext())
            {
                var model = context.Customer.First(c => c.Id == id);//获取实例

                txtName.Text = model.Name;
                txtAddress.Text = model.Address;
                txtIdCard.Text = model.IdCard;
                txtNowAddress.Text = model.AddressNow;
                txtPhone.Text = model.Phone;
                cbSex.SelectedIndex = model.Sex == "男" ? 0 : 1;
                txtBePromotionCode.Text = model.BePromotionCode;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string address = txtAddress.Text;
            string nowAddress = txtNowAddress.Text;
            string idCard = txtIdCard.Text;
            string phone = txtPhone.Text;
            string sex = cbSex.SelectedIndex == 0 ? "男" : "女";

            #region 验证

            if (name.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入名称", "空值提醒");
                txtName.Focus();
                return;
            }
            if (idCard.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入身份证号", "空值提醒");
                txtIdCard.Focus();
                return;
            }
            if (phone.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入电话号", "空值提醒");
                txtPhone.Focus();
                return;
            }
            if ((bool)cbCheckIdCard.IsChecked)
            {
                if (!IdCardCommon.CheckIDCard(idCard))
                {
                    MessageBoxX.Show("身份证号格式不正确", "格式错误");
                    txtIdCard.Focus();
                    txtIdCard.SelectAll();
                    return;
                }
            }

            if ((bool)cbCheckPhone.IsChecked)
            {
                if (!PhoneNumberCommon.IsPhoneNumber11(phone))
                {
                    MessageBoxX.Show("电话号格式不正确", "格式错误");
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                    return;
                }
            }

            #endregion 

            using (DBContext context = new DBContext())
            {
                if (id > 0)
                {
                    #region 重复登记验证

                    if (context.Customer.Where(c => c.Id != id).Any(c => c.IdCard == idCard || c.Phone == phone))
                    {
                        MessageBoxX.Show("手机号或身份证号重复", "数据重复");
                        return;
                    }

                    #endregion

                    DBModels.Member.Customer customer = context.Customer.Single(c => c.Id == id);
                    customer.Address = address;
                    customer.AddressNow = nowAddress;
                    customer.BePromotionCode = txtBePromotionCode.Text;
                    customer.Birthday = IdCardCommon.GetBirthday(idCard).ToString("yyyy-MM-dd");
                    customer.IdCard = idCard;
                    customer.Name = name;
                    customer.Phone = phone;
                    customer.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";
                    customer.Sex = sex;

                    Result = customer;
                    context.SaveChanges();

                    Notice.Show($"{name}编辑成功", "编辑成功", MessageBoxIcon.Success);
                }
                else 
                {
                    #region 重复登记验证

                    if (context.Customer.Any(c => c.IdCard == idCard || c.Phone == phone))
                    {
                        MessageBoxX.Show("手机号或身份证号重复", "数据重复");
                        return;
                    }

                    #endregion

                    DBModels.Member.Customer customer = new DBModels.Member.Customer();
                    customer.Address = address;
                    customer.AddressNow = nowAddress;
                    customer.BePromotionCode = txtBePromotionCode.Text;
                    customer.Birthday = IdCardCommon.GetBirthday(idCard).ToString("yyyy-MM-dd");
                    customer.Creater = TempBasePageData.message.CurrUser.Id;
                    customer.CreateTime = DateTime.Now;
                    customer.IdCard = idCard;
                    customer.IsMember = false;
                    customer.Name = name;
                    customer.IsBlack = false;
                    customer.Phone = phone;
                    customer.PromotionCode = "";
                    customer.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";
                    customer.Sex = sex;
                    customer = context.Customer.Add(customer);
                    context.SaveChanges();

                    //更新推荐码
                    context.Customer.Single(c => c.Id == customer.Id).PromotionCode = PromotionCodeCommon.GetCode(customer.Id);
                    context.SaveChanges();

                    Notice.Show($"{name}登记成功", "登记成功",MessageBoxIcon.Success);
                }
            }

            Succeed = true;
            Close();
        }

        private void btnReadIdCard_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("功能未实现");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtAddress.Clear();
            txtIdCard.Clear();
            txtNowAddress.Clear();
            txtPhone.Clear();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            txtNowAddress.Text = txtAddress.Text;
        }

        private void btnFindBePromotionCode_Click(object sender, RoutedEventArgs e)
        {
            SelectedCustomer selectedCustomer = new SelectedCustomer();
            selectedCustomer.ShowDialog();

            if (selectedCustomer.Succeed)
            {
                txtBePromotionCode.Text = selectedCustomer.Model.PromotionCode;
            }
            else 
            {
                txtBePromotionCode.Clear();
            }
        }
    }
}
