using Common;
using DBModels.Finance;
using FixedAssetsPlugin.DB;
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

namespace FixedAssetsPlugin
{
    /// <summary>
    /// CheckFixedAssets.xaml 的交互逻辑
    /// </summary>
    public partial class CheckFixedAssets : Window
    {
        string code = "";
        int oldCount = 0;
        int oldState = 0;
        int oldLocation = 0;
        string oldName = "";
        string oldPhone = "";

        public CheckFixedAssets(string _code)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            code = _code;
        }

        public bool Succeed = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserGlobal.IsStaffLogin())
            {
                MessageBoxX.Show("当前操作者不是员工", "加载信息失败");
                Close();
            }

            LoadState();
            LoadLocation();
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var fixedAssets = context.FixedAssets.First(c => c.Id == code);

                oldCount = fixedAssets.Count;
                oldState = fixedAssets.State;
                oldLocation = fixedAssets.Location;
                oldName = fixedAssets.PrincipalName;
                oldPhone = fixedAssets.PrincipalPhone;

                txtCount.Text = oldCount.ToString();
                cbState.SelectedValue = oldState;
                cbLocation.SelectedValue = oldLocation;
                txtPrincipalName.Text = oldName;
                txtPrincipalPhone.Text = oldPhone;
            }
        }

        private void LoadLocation()
        {
            cbLocation.Items.Clear();
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var locations = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsLocation).ToList();

                cbLocation.ItemsSource = locations;
                cbLocation.DisplayMemberPath = "Name";
                cbLocation.SelectedValuePath = "Id";

                cbLocation.SelectedIndex = 0;
            }
        }

        private void LoadState()
        {
            cbState.Items.Clear();
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var locations = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsState).ToList();

                cbState.ItemsSource = locations;
                cbState.DisplayMemberPath = "Name";
                cbState.SelectedValuePath = "Id";

                cbState.SelectedIndex = 0;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;

            if (txtCount.Text.IsNullOrEmpty()) 
            {
                MessageBoxX.Show("请输入数量","空值提醒");
                txtCount.Focus();
                return;
            }

            if (!int.TryParse(txtCount.Text, out count)) 
            {
                MessageBoxX.Show("数量格式不正确", "格式错误");
                txtCount.Focus();
                txtCount.SelectAll();
                return;
            }

            using (FixedAssetsDBContext context = new FixedAssetsDBContext()) 
            {
                if (UserGlobal.CurrUser.StaffId.IsNullOrEmpty())
                {
                    MessageBoxX.Show("当前操作者不是员工", "加载信息失败");
                    return;
                }

                var staff = context.Staff.First(c => c.Id == UserGlobal.CurrUser.StaffId);

                var fixedAssets = context.FixedAssets.Single(c => c.Id == code);
                fixedAssets.Count = count;
                fixedAssets.LastCheck = DateTime.Now;
                fixedAssets.State = cbState.SelectedValue.ToString().AsInt();
                fixedAssets.Location = cbLocation.SelectedValue.ToString().AsInt();
                fixedAssets.PrincipalName = txtPrincipalName.Text;
                fixedAssets.PrincipalPhone = txtPrincipalPhone.Text;

                FixedAssetsCheck model = new FixedAssetsCheck();
                model.Check = fixedAssets.LastCheck;
                model.CreateTime = fixedAssets.LastCheck;
                model.Creator = UserGlobal.CurrUser.Id;
                model.FixedAssetsCode = fixedAssets.Id;
                model.NewCount = count;
                model.NewLocation = cbLocation.SelectedValue.ToString().AsInt();
                model.NewPrincipalName = txtPrincipalName.Text;
                model.NewPrincipalPhone = txtPrincipalPhone.Text;
                model.NewState = cbState.SelectedValue.ToString().AsInt();
                model.OldCount = oldCount;
                model.OldLocation = oldLocation;
                model.OldPrincipalName = oldName;
                model.OldPrincipalPhone = oldPhone;
                model.OldState = oldState;
                model.StaffId = staff.Id;
                model.StaffName = staff.Name;
                model.Remark = txtRemark.Text;

                context.FixedAssetsCheck.Add(model);

                context.SaveChanges();
            }

            Succeed = true;
            Close();
        }
    }
}
