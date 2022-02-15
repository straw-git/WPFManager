using Common;
using Common.Utils;
using Common.Windows;
using CoreDBModels.Models;
using CustomerDBModels.Models;
using CustomerPlugin.Windows;
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

namespace CustomerPlugin.Windows
{
    /// <summary>
    /// EditMember.xaml 的交互逻辑
    /// </summary>
    public partial class EditMember : Window
    {
        int customerId = 0;
        Customer CustomerModel = new Customer();//客户信息
        Member MemberModel = new Member();//会员信息

        List<UIModel> memberRecharges = new List<UIModel>();//充值记录

        #region Models

        class UIModel : BaseUIModel
        {
            public string CreateTime { get; set; }
            public string Price { get; set; }
            public string MemberPrice { get; set; }
            public string Creator { get; set; }
        }

        #endregion

        public EditMember(int _customerId)
        {
            InitializeComponent();

            customerId = _customerId;

            using (CustomerDBContext context = new CustomerDBContext())
            {
                if (context.Member.Any(c => c.CustomerId == _customerId && !c.IsDelete))
                {
                    CustomerModel = context.Customer.First(c => c.Id == _customerId);
                    MemberModel = context.Member.First(c => c.CustomerId == _customerId && !c.IsDelete);

                    #region 更新会员 与客户信息对照

                    var _member = context.Member.Single(c => c.Id == MemberModel.Id);
                    bool _update = false;//是否有更新的必要
                    if (CustomerModel.Name != MemberModel.Name)
                    {
                        _member.Name = CustomerModel.Name;
                        _member.QuickCode = CustomerModel.QuickCode;
                        MemberModel.Name = CustomerModel.Name;
                        MemberModel.QuickCode = CustomerModel.QuickCode;
                        _update = true;
                    }
                    if (CustomerModel.IdCard != MemberModel.IdCard)
                    {
                        _member.IdCard = CustomerModel.IdCard;
                        MemberModel.IdCard = CustomerModel.IdCard;
                        _update = true;
                    }
                    if (CustomerModel.Phone != MemberModel.Phone)
                    {
                        _member.Phone = CustomerModel.Phone;
                        MemberModel.Phone = CustomerModel.Phone;
                        _update = true;
                    }

                    if (_update)
                        context.SaveChanges();

                    #endregion
                }
                else
                {
                    MessageBoxX.Show("当前会员信息不存在", "会员信息异常");
                    Close();
                }
            }

            LoadData();
        }

        /// <summary>
        /// 页面信息赋值
        /// </summary>
        private void LoadData()
        {
            txtName.Text = CustomerModel.Name;
            txtAddress.Text = CustomerModel.Address;
            txtIdCard.Text = CustomerModel.IdCard;
            txtNowAddress.Text = CustomerModel.AddressNow;
            txtPhone.Text = CustomerModel.Phone;
            cbSex.SelectedIndex = CustomerModel.Sex == "男" ? 0 : 1;
            txtBePromotionCode.Text = CustomerModel.BePromotionCode;

            txtMemberCard.Text = MemberModel.CardNumber;
            txtRechargePrice.Text = "0";
            txtPrice.Text = MemberModel.Price.ToString();

            list.ItemsSource = null;
            using (CustomerDBContext context = new CustomerDBContext())
            {
                var _list = context.MemberRecharge.OrderByDescending(c => c.CreateTime).Where(c => c.MemberId == MemberModel.Id).ToList();

                foreach (var item in _list)
                {
                    UIModel model = new UIModel();
                    model.CreateTime = item.CreateTime.ToString("yy年MM月dd日");
                    model.Creator = context.User.Any(c => c.Id == item.Creator) ? context.User.First(c => c.Id == item.Creator).Name : "";
                    model.MemberPrice = item.NewPrice.ToString();
                    model.Price = item.Price.ToString();

                    memberRecharges.Add(model);
                }
            }
            if (memberRecharges == null || memberRecharges.Count == 0) memberRecharges = new List<UIModel>();//防止错误
            list.ItemsSource = memberRecharges;
        }

        private void btnCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            txtNowAddress.Text = txtAddress.Text;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;
            if (!decimal.TryParse(txtRechargePrice.Text, out price))
            {
                MessageBoxX.Show("本次充值金额格式不正确", "格式错误");
                return;
            }

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

            if (!IdCardCommon.CheckIDCard(idCard))
            {
                MessageBoxX.Show("身份证号格式不正确", "格式错误");
                txtIdCard.Focus();
                txtIdCard.SelectAll();
                return;
            }

            if (!PhoneNumberCommon.IsPhoneNumber11(phone))
            {
                MessageBoxX.Show("电话号格式不正确", "格式错误");
                txtPhone.Focus();
                txtPhone.SelectAll();
                return;
            }

            #endregion 

            //更新信息
            using (CustomerDBContext context = new CustomerDBContext())
            {

                var _customer = context.Customer.Single(c => c.Id == CustomerModel.Id);
                var _member = context.Member.Single(c => c.Id == MemberModel.Id);

                #region 基础信息更新

                if (CustomerModel.Name != name)
                {
                    _customer.Name = name;
                    _member.Name = name;
                    _customer.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";
                    _member.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";

                    CustomerModel.Name = name;
                    MemberModel.Name = name;
                    CustomerModel.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";
                    MemberModel.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";
                }

                if (CustomerModel.Address != address)
                {
                    _customer.Address = address;

                    CustomerModel.Address = address;
                }

                if (CustomerModel.AddressNow != nowAddress)
                {
                    _customer.AddressNow = nowAddress;

                    CustomerModel.AddressNow = nowAddress;
                }

                if (CustomerModel.IdCard != idCard)
                {
                    _customer.IdCard = idCard;
                    _member.IdCard = idCard;

                    CustomerModel.IdCard = idCard;
                    MemberModel.IdCard = idCard;
                }

                if (CustomerModel.Phone != phone)
                {
                    _customer.Phone = phone;
                    _member.Phone = phone;

                    CustomerModel.Phone = phone;
                    MemberModel.Phone = phone;
                }

                if (CustomerModel.Sex != sex)
                {
                    _customer.Sex = sex;

                    CustomerModel.Sex = sex;
                }

                if (CustomerModel.BePromotionCode != txtBePromotionCode.Text)
                {
                    _customer.BePromotionCode = txtBePromotionCode.Text;

                    CustomerModel.BePromotionCode = txtBePromotionCode.Text;
                }

                if (MemberModel.CardNumber != txtMemberCard.Text)
                {
                    _member.CardNumber = txtMemberCard.Text;

                    MemberModel.CardNumber = txtMemberCard.Text;
                }


                #endregion

                #region 更新充值记录

                if (price > 0)
                {
                    CustomerDBModels.Models.MemberRecharge _recharge = new CustomerDBModels.Models.MemberRecharge();
                    _recharge.CreateTime = DateTime.Now;
                    _recharge.Creator = UserGlobal.CurrUser.Id;
                    _recharge.CustomerId = CustomerModel.Id;
                    _recharge.IdCard = idCard;
                    _recharge.MemberId = MemberModel.Id;
                    _recharge.Name = name;
                    _recharge.NewPrice = MemberModel.Price + price;
                    _recharge.OldPrice = MemberModel.Price;
                    _recharge.PayOrderId = 0;
                    _recharge.Price = price;
                    _recharge.QuickCode = $"{name.Convert2Pinyin()}|{name.Convert2Py()}";

                    _member.Price = MemberModel.Price + price;//充值时更新会员余额
                    MemberModel.Price = MemberModel.Price + price;

                    context.MemberRecharge.Add(_recharge);
                }

                #endregion

                context.SaveChanges();
            }
            Notice.Show($"{name}会员信息更新成功", "更新成功",MessageBoxIcon.Success);

            LoadData();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDeleteMember_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show($"是否确认注销会员 [{MemberModel.Name}]？ ", $"会员注销警告", null, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                using (CustomerDBContext context = new CustomerDBContext()) 
                {
                    var _member= context.Member.Single(c=>c.Id==MemberModel.Id);
                    _member.IsDelete = true;

                    var _customer = context.Customer.Single(c=>c.Id==CustomerModel.Id);
                    _customer.IsMember = false;

                    context.SaveChanges();
                }
            }
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
