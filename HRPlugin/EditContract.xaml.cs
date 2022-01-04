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
                    dtStart.SelectedDateTime = _contract.Start;
                    dtEnd.SelectedDateTime = _contract.End;
                    int year = _contract.End.Year - _contract.Start.Year;
                    switch (year)
                    {
                        case 1:
                            cbLong.SelectedIndex = 0;
                            break;
                        case 2:
                            cbLong.SelectedIndex = 1;
                            break;
                        case 3:
                            cbLong.SelectedIndex = 2;
                            break;
                        case 5:
                            cbLong.SelectedIndex = 3;
                            break;
                        default:
                            break;
                    }
                    dtWrite.SelectedDateTime = _contract.Write;
                    txtPrice.Text = _contract.Price.ToString();
                    txtRemark.Text = _contract.Remark;
                }
            }
            cbLong_SelectionChanged(null,null);
        }

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                MessageBoxX.Show("合同金额不能为空", "空值提醒");
                txtPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("合同金额格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            using (DBContext context = new DBContext())
            {
                if (context.StaffContract.Any(c => c.StaffId == staffId))
                {
                    //存在 编辑
                    var _contract = context.StaffContract.Single(c => c.StaffId == staffId);
                    _contract.Start = dtStart.SelectedDateTime;
                    _contract.End = dtEnd.SelectedDateTime;
                    _contract.Write = dtWrite.SelectedDateTime;
                    _contract.Price = price;
                    _contract.Remark = txtRemark.Text;
                }
                else
                {
                    //不存在 添加
                    StaffContract _contract = new StaffContract();
                    _contract.CreateTime = DateTime.Now;
                    _contract.Creator = UserGlobal.CurrUser.Id;
                    _contract.End = dtEnd.SelectedDateTime;
                    _contract.Price = price;
                    _contract.Remark = txtRemark.Text;
                    _contract.StaffId = staffId;
                    _contract.Start = dtStart.SelectedDateTime;
                    _contract.Write = dtWrite.SelectedDateTime;
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

        private void cbLong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            switch (cbLong.SelectedIndex)
            {
                case 0:
                    dtEnd.SelectedDateTime = dtStart.SelectedDateTime.AddYears(1);
                    break;
                case 1:
                    dtEnd.SelectedDateTime = dtStart.SelectedDateTime.AddYears(2);
                    break;
                case 2:
                    dtEnd.SelectedDateTime = dtStart.SelectedDateTime.AddYears(3);
                    break;
                case 3:
                    dtEnd.SelectedDateTime = dtStart.SelectedDateTime.AddYears(5);
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
