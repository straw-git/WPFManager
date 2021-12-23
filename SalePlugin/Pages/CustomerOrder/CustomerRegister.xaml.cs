using Common;
using Common.Utils;
using DBModels.Member;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalePlugin.Pages.CustomerOrder
{
    /// <summary>
    /// CustomerRegister.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerRegister : BasePage
    {
        public CustomerRegister()
        {
            InitializeComponent();
            Order = 0;

            //测试
            OnPageLoaded();
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhone { get; set; }
            public string CustomerIdCard { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            customerList.ItemsSource = Data;
        }

        private void btnCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            txtNowAddress.Text = txtAddress.Text;
        }

        private void btnReadIdCard_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未实现");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtAddress.Clear();
            txtIdCard.Clear();
            txtName.Clear();
            txtNowAddress.Clear();
            txtPhone.Clear();
            txtTW.Text = "36.6";
            txtBePromotionCode.Clear();
            cbSex.SelectedIndex = 0;
            cbXCM.IsChecked = false;

            EnableRegisterUI(true);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string address = txtAddress.Text;
            string nowAddress = txtNowAddress.Text;
            string idCard = txtIdCard.Text;
            string phone = txtPhone.Text;
            string sex = cbSex.SelectedIndex == 0 ? "男" : "女";
            decimal tw = 0;

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
            if (!decimal.TryParse(txtTW.Text, out tw))
            {
                MessageBoxX.Show("体温格式不正确", "格式错误");
                txtTW.Focus();
                txtTW.SelectAll();
                return;
            }

            if (tw <= 35 || tw >= 37)
            {
                MessageBoxX.Show("请注意 当前登记者体温异常", "体温异常提醒");
            }

            #endregion 

            Customer customer = new Customer();

            using (DBContext context = new DBContext())
            {
                if (context.Customer.Any(c => c.IdCard == idCard))
                {
                    customer = context.Customer.First(c => c.IdCard == idCard);
                }
                else
                {
                    #region 重复登记验证

                    if (context.Customer.Any(c => c.Phone == phone))
                    {
                        MessageBoxX.Show("手机号重复", "数据重复");
                        return;
                    }

                    #endregion

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
                    string pCode = PromotionCodeCommon.GetCode(customer.Id);
                    context.Customer.Single(c => c.Id == customer.Id).PromotionCode = pCode;
                    customer.PromotionCode = pCode;
                    context.SaveChanges();
                }

                CustomerTemp customerTemp = new CustomerTemp();
                customerTemp.Creater = CurrUser.Id;
                customerTemp.CreateTime = DateTime.Now;
                customerTemp.CustomerId = customer.Id;
                customerTemp.IdCard = customer.IdCard;
                customerTemp.IsMember = customer.IsMember;
                customerTemp.Name = customer.Name;
                customerTemp.Phone = customer.Phone;
                customerTemp.QuickCode = customer.QuickCode;
                customerTemp.TW = tw;
                customerTemp.XCM = (bool)cbXCM.IsChecked;

                context.CustomerTemp.Add(customerTemp);
                context.SaveChanges();

                Notice.Show($"{name}登记成功", "登记成功", MessageBoxIcon.Success);
                EnableRegisterUI(true);
            }
        }

        private void EnableRegisterUI(bool _enable)
        {
            txtAddress.IsEnabled = _enable;
            txtIdCard.IsEnabled = _enable;
            txtName.IsEnabled = _enable;
            txtNowAddress.IsEnabled = _enable;
            txtPhone.IsEnabled = _enable;
            cbSex.IsEnabled = _enable;
        }

        private void btnFindBePromotionCode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearchName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                UpdateGrid();

            }
        }

        private void UpdateGrid()
        {
            bNoData.Visibility = Visibility.Visible;
            Data.Clear();
            string name = txtSearchName.Text;
            if (name.IsNullOrEmpty())
            {
                return;
            }

            using (DBContext context = new DBContext())
            {
                if (context.Customer.Any(c => c.Name.Contains(name) || c.QuickCode.Contains(name)))
                {
                    var _list = context.Customer.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name) || c.IdCard.Contains(name) || c.Phone.Contains(name)).ToList();
                    foreach (var item in _list)
                    {
                        Data.Add(new UIModel()
                        {
                            CustomerId = item.Id,
                            CustomerIdCard = item.IdCard,
                            CustomerName = item.Name,
                            CustomerPhone = item.Phone
                        });
                    }
                    if (_list != null && _list.Count > 0)
                    {
                        customerList.Focus();
                        customerList.SelectedIndex = 0;
                        bNoData.Visibility = Visibility.Collapsed;
                    }
                    else bNoData.Visibility = Visibility.Visible;
                }
            }
        }

        private void customerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (customerList.SelectedItem == null) return;
            UIModel selectedModel = customerList.SelectedItem as UIModel;
            if (selectedModel == null) return;

            EnableRegisterUI(false);
            using (DBContext context = new DBContext())
            {
                var customer = context.Customer.First(c => c.Id == selectedModel.CustomerId);

                txtName.Text = customer.Name;
                cbSex.SelectedIndex = customer.Sex == "男" ? 0 : 1;
                txtIdCard.Text = customer.IdCard;
                txtPhone.Text = customer.Phone;
                txtAddress.Text = customer.Address;
                txtNowAddress.Text = customer.AddressNow;
                txtBePromotionCode.Text = customer.BePromotionCode;
            }

            if (txtBePromotionCode.Text.IsNullOrEmpty()) btnFindBePromotionCode.IsEnabled = true; else btnFindBePromotionCode.IsEnabled = false;

            txtTW.Focus();
            txtTW.SelectAll();
        }

        private void customerList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                customerList_MouseDoubleClick(null, null);
                e.Handled = false;
            }
            else if (e.Key == Key.Down)
            {
                if (Data.Count > 0)
                {
                    if (customerList.SelectedIndex >= 0)
                    {
                        customerList.SelectedIndex += 1;
                    }
                    else
                    {
                        customerList.SelectedIndex = 0;
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                if (Data.Count > 0)
                {
                    if (customerList.SelectedIndex >= 1)
                    {
                        customerList.SelectedIndex -= 1;
                    }
                    else
                    {
                        customerList.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                //输入其它 直接将焦点兑回txt
                txtSearchName.Focus();
                txtSearchName_PreviewKeyDown(txtSearchName, e);
            }
        }
    }
}
