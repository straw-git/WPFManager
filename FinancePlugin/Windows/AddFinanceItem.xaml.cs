
using Common.Windows;
using FinanceDBModels;
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
    /// AddFinanceItem.xaml 的交互逻辑
    /// </summary>
    public partial class AddFinanceItem : Window
    {
        public AddFinanceItem()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        public bool Succeed = false;

        private void btnSelectedStaff_Click(object sender, RoutedEventArgs e)
        {
            SelectedStaff s = new SelectedStaff(1);
            s.ShowDialog();
            if (s.Succeed)
            {
                string id = s.Ids[0];
                using (CoreDBContext context = new CoreDBContext())
                {
                    var staff = context.Staff.First(c => c.Id == id);
                    btnSelectedStaff.Content = staff.Name;
                    btnSelectedStaff.Tag = id;
                }
            }
            else
            {
                btnSelectedStaff.Content = "选择关联员工";
                btnSelectedStaff.Tag = "";
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAddType();
        }

        private void LoadAddType()
        {
            cbType.Items.Clear();
            using (FinanceDBContext context = new FinanceDBContext())
            {
                var types = context.FinanceType.ToList();

                cbType.ItemsSource = types;
                cbType.DisplayMemberPath = "Name";
                cbType.SelectedValuePath = "Id";

                if (types.Count > 0)
                    cbType.SelectedIndex = 0;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            decimal price = 0;

            if (txtPrice.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入记账金额", "空值提醒");
                txtPrice.Focus();
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBoxX.Show("记账金额格式不正确", "格式错误");
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            DateTime currTime = DateTime.Now;

            CoreDBModels.Staff staff = new CoreDBModels.Staff();
            string staffId = btnSelectedStaff.Tag.ToString();

            using (CoreDBContext context = new CoreDBContext())
            {   
                staff = context.Staff.First(c => c.Id == staffId);
            }

            using (FinanceDBContext context = new FinanceDBContext())
            {
                FinanceBill model = new FinanceBill();
                model.AddType = cbType.SelectedValue.ToString().AsInt();
                model.BillTime = dtBill.SelectedDate;
                model.CreateTime = currTime;
                model.Creator = Common.UserGlobal.CurrUser.Id;
                model.Remark = txtRemark.Text;
                model.StaffId = staffId;
                model.StaffName = staff.Name;
                model.StaffQuickCode = staff.QuickCode;
                model.Things = txtThings.Text;
                model.Price = price;

                context.FinanceBill.Add(model);
                context.SaveChanges();
            }

            MessageBoxX.Show("成功", "成功");

            //dtBill.SelectedDate = DateTime.Now;
            //cbType.SelectedIndex = 0;
            //txtPrice.Clear();
            //txtRemark.Clear();
            //txtThings.Clear();
            //btnSelectedStaff.Content = "选择关联员工";
            //btnSelectedStaff.Tag = "";

            Succeed = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAddType();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }
    }
}
