using Common;
using DBModels.Staffs;
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

namespace HRPlugin
{
    /// <summary>
    /// EditContract.xaml 的交互逻辑
    /// </summary>
    public partial class EditContract : Window
    {
        public EditContract(string _staffId)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            staffId = _staffId;
        }

        public bool Succeed = false;
        string staffId = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (DBContext context = new DBContext())
            {
                if (context.StaffContract.Any(c => c.StaffId == staffId))
                {
                    //存在 编辑
                    var _contract = context.StaffContract.First(c => c.StaffId == staffId);
                    dtContractStart.SelectedDateTime = _contract.Start;
                    dtContractEnd.SelectedDateTime = _contract.End;
                    int year = _contract.End.Year - _contract.Start.Year;
                    switch (year)
                    {
                        case 1:
                            cbContractLong.SelectedIndex = 0;
                            break;
                        case 2:
                            cbContractLong.SelectedIndex = 1;
                            break;
                        case 3:
                            cbContractLong.SelectedIndex = 2;
                            break;
                        case 5:
                            cbContractLong.SelectedIndex = 3;
                            break;
                        default:
                            break;
                    }
                    dtContractWrite.SelectedDateTime = _contract.Write;
                    txtContractPrice.Text = _contract.Price.ToString();
                    txtContractRemark.Text = _contract.Remark;
                }
            }
            cbContractLong_SelectionChanged(null,null);
        }

        #region 劳动合同

        private void btnContractSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            if (string.IsNullOrEmpty(txtContractPrice.Text))
            {
                MessageBoxX.Show("合同金额不能为空", "空值提醒");
                txtContractPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtContractPrice.Text, out price))
            {
                MessageBoxX.Show("合同金额格式不正确", "格式错误");
                txtContractPrice.Focus();
                txtContractPrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                if (context.StaffContract.Any(c => c.StaffId == staffId))
                {
                    //存在 编辑
                    var _contract = context.StaffContract.Single(c => c.StaffId == staffId);
                    _contract.Start = dtContractStart.SelectedDateTime;
                    _contract.End = dtContractEnd.SelectedDateTime;
                    _contract.Write = dtContractWrite.SelectedDateTime;
                    _contract.Price = price;
                    _contract.Remark = txtContractRemark.Text;
                }
                else
                {
                    //不存在 添加
                    StaffContract _contract = new StaffContract();
                    _contract.CreateTime = DateTime.Now;
                    _contract.Creator = UserGlobal.CurrUser.Id;
                    _contract.End = dtContractEnd.SelectedDateTime;
                    _contract.Price = price;
                    _contract.Remark = txtContractRemark.Text;
                    _contract.StaffId = staffId;
                    _contract.Start = dtContractStart.SelectedDateTime;
                    _contract.Write = dtContractWrite.SelectedDateTime;
                    _contract.Stop = false;
                    _contract.StopTime = DateTime.Now;
                    _contract.StopUser = 0;

                    context.StaffContract.Add(_contract);
                }

                context.SaveChanges();
            }

            Succeed = true;
            Close();
        }

        private void cbContractLong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            switch (cbContractLong.SelectedIndex)
            {
                case 0:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(1);
                    break;
                case 1:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(2);
                    break;
                case 2:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(3);
                    break;
                case 3:
                    dtContractEnd.SelectedDateTime = dtContractStart.SelectedDateTime.AddYears(5);
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
