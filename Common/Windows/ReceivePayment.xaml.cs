
using DBModels.Finance;
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

namespace Common.Windows
{
    /// <summary>
    /// AddRecharge.xaml 的交互逻辑
    /// </summary>
    public partial class ReceivePayment : Window
    {
        public enum PaymentType
        {
            /// <summary>
            /// 收款
            /// </summary>
            Receive,
            /// <summary>
            /// 付款
            /// </summary>
            Pay
        }

        public bool Succeed = false;
        public PaymentLog Model = new PaymentLog();
        public decimal useMemberPrice = 0;
        string useWindowName = "";
        decimal maxMemberPrice = 0;
        /// <summary>
        /// 收款还是付款
        /// </summary>
        PaymentType type = PaymentType.Receive;

        public ReceivePayment() 
        {
            MessageBox.Show("测试");
            MessageBox.Show(TempBasePageData.message.CurrUser.Name);
        }

        /// <summary>
        /// 收付款
        /// </summary>
        /// <param name="_type">类型</param>
        /// <param name="_useWindowName">使用的窗体名称</param>
        /// <param name="_price">金额</param>
        /// <param name="_memberPrice">会员金额</param>
        public ReceivePayment(PaymentType _type, string _useWindowName, decimal _price = 0, decimal _memberPrice = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            useWindowName = _useWindowName;
            type = _type;
            maxMemberPrice = _memberPrice;

            if (_type == PaymentType.Receive)
            {
                if (_memberPrice > 0)
                {
                    lblMaxMemberPrice.Content = _memberPrice;
                    spMember.Visibility = Visibility.Hidden;
                }
            }
            else if (_type == PaymentType.Pay)
            {
                spMember.Visibility = Visibility.Hidden;
            }

            if (_price > 0)
            {
                txtPrice.IsEnabled = false;
                txtPrice.Text = _price.ToString();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPayment();
            txtPrice.Focus();
        }

        private void LoadPayment()
        {
            using (DBContext context = new DBContext())
            {
                var payModels = context.SysDic.Where(c => c.ParentCode == DicData.PayModel).ToList();

                payModels.Insert(0, new SysDic()
                {
                    Id = -1,
                    Name = "请选择"
                });

                cbPayment.ItemsSource = payModels;
                cbPayment.DisplayMemberPath = "Name";
                cbPayment.SelectedValuePath = "Id";

                cbPayment.SelectedIndex = 0;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBoxX.Show("请输入金额", "空值提醒");
                txtPrice.Focus();
                return;
            }

            decimal price = 0;
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("金额格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            if (cbPayment.SelectedIndex == 0)
            {
                MessageBoxX.Show("请选择支付方式", "空值提醒");
                return;
            }
            if (cbCard.SelectedIndex == 0)
            {
                MessageBoxX.Show("请选择账号", "空值提醒");
                return;
            }
            if (!decimal.TryParse(txtUseMemberPrice.Text, out useMemberPrice))
            {
                MessageBoxX.Show("请输入正确使用金额", "格式错误");
                txtUseMemberPrice.Focus();
                txtUseMemberPrice.SelectAll();
                return;
            }

            if (useMemberPrice > maxMemberPrice)
            {
                MessageBoxX.Show("账户余额不足,请核对后输入", "余额不足");
                txtUseMemberPrice.Focus();
                txtUseMemberPrice.SelectAll();
                return;
            }

            Model.CreateTime = DateTime.Now;
            Model.Creator = TempBasePageData.message.CurrUser.Id;
            Model.PaymentId = cbCard.SelectedValue.ToString().AsInt();
            Model.PayModelId = cbPayment.SelectedValue.ToString().AsInt();
            Model.Price = type == PaymentType.Receive ? price : price * -1;
            Model.UseWindowName = $"[{type}]{useWindowName}";
            Model.Code = Guid.NewGuid().ToString();


            //using (DBContext context = new DBContext())
            //{
            //    Model = context.PaymentLog.Add(Model);
            //    var payment = context.Payment.Single(c => c.Id == Model.PaymentId);
            //    payment.Price += price;//账户余额

            //    context.SaveChanges();
            //}

            Succeed = true;
            Close();
        }

        private void cbPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPayment.SelectedValue.ToString().AsInt() > 0)
            {
                SysDic focusModel = cbPayment.SelectedItem as SysDic;
                using (DBContext context = new DBContext())
                {
                    var cards = context.Payment.Where(c => c.PayModelId == focusModel.Id).ToList();

                    cards.Insert(0, new Payment()
                    {
                        Id = -1,
                        Holder = "请选择"
                    });

                    cbCard.ItemsSource = null;
                    cbCard.ItemsSource = cards;
                    cbCard.DisplayMemberPath = "Holder";
                    cbCard.SelectedValuePath = "Id";

                    cbCard.SelectedIndex = 0;
                }
            }
        }

        private void txtPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSubmit_Click(null, null);
            }
        }
    }
}
