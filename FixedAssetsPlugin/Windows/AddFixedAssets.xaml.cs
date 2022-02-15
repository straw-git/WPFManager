
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
using FixedAssetsDBModels.Models;
using CoreDBModels.Models;

namespace FixedAssetsPlugin.Windows
{
    /// <summary>
    /// AddFixedAssets.xaml 的交互逻辑
    /// </summary>
    public partial class AddFixedAssets : Window
    {
        public AddFixedAssets()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        public bool Succeed = false;
        public FixedAssets Model = new FixedAssets();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!UserGlobal.IsStaffLogin())
            {
                MessageBoxX.Show("当前操作者不是员工", "加载信息失败");
                Close();
            }

            LoadState();
            LoadLocation();
        }

        private void LoadLocation()
        {
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var locations = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsLocation).ToList();
                if (locations == null || locations.Count == 0)
                {
                    locations = new List<SysDic>();
                    locations.Insert(0, new SysDic()
                    {
                        Id = 0,
                        Name = "无数据"
                    });
                }

                cbLocation.ItemsSource = locations;
                cbLocation.DisplayMemberPath = "Name";
                cbLocation.SelectedValuePath = "Id";

                cbLocation.SelectedIndex = 0;
            }
        }

        private void LoadState()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var locations = context.SysDic.Where(c => c.ParentCode == DicData.FixedAssetsState).ToList();
                if (locations == null || locations.Count == 0)
                {
                    locations = new List<SysDic>();
                    locations.Insert(0, new SysDic()
                    {
                        Id = 0,
                        Name = "无数据"
                    });
                }

                cbState.ItemsSource = locations;
                cbState.DisplayMemberPath = "Name";
                cbState.SelectedValuePath = "Id";

                cbState.SelectedIndex = 0;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            int state = cbState.SelectedValue.ToString().AsInt();
            int location = cbLocation.SelectedValue.ToString().AsInt();

            #region Null or Empty

            if (txtName.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入资产名称", "空值提醒");
                txtName.Focus();
                return;
            }

            if (state == 0)
            {
                MessageBoxX.Show("无状态数据", "空值提醒");
                return;
            }

            if (location == 0)
            {
                MessageBoxX.Show("无位置数据", "空值提醒");
                return;
            }

            if (txtPrincipalName.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入负责人", "空值提醒");
                txtPrincipalName.Focus();
                return;
            }

            if (txtCount.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入数量", "空值提醒");
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

            #endregion

            FixedAssets model = new FixedAssets();
            using (FixedAssetsDBContext context = new FixedAssetsDBContext())
            {
                var staff = context.Staff.First(c => c.Id == UserGlobal.CurrUser.StaffId);

                model.Count = count;
                model.CreateTime = DateTime.Now;
                model.Creator = UserGlobal.CurrUser.Id;
                model.DelTime = DateTime.Now;
                model.DelUser = 0;
                model.From = txtFrom.Text;
                model.Id = model.CreateTime.ToString("yyMMddHHmmss");
                model.IsDel = false;
                model.LastCheck = model.CreateTime;
                model.Location = location;
                model.ModelName = txtModelName.Text;
                model.Name = txtName.Text;
                model.PrincipalName = txtPrincipalName.Text;
                model.PrincipalPhone = txtPrincipalPhone.Text;
                model.Register = dtRegister.SelectedDateTime;
                model.State = state;
                model.UnitName = txtUnitName.Text;

                model = context.FixedAssets.Add(model);

                FixedAssetsCheck checkModel = new FixedAssetsCheck();
                checkModel.Check = model.Register;
                checkModel.CreateTime = model.CreateTime;
                checkModel.Creator = model.Creator;
                checkModel.FixedAssetsCode = model.Id;
                checkModel.NewCount = model.Count;
                checkModel.NewLocation = model.Location;
                checkModel.NewPrincipalName = model.PrincipalName;
                checkModel.NewPrincipalPhone = model.PrincipalPhone;
                checkModel.NewState = model.State;
                checkModel.OldCount = 0;
                checkModel.OldLocation = model.Location;
                checkModel.OldPrincipalName = "";
                checkModel.OldPrincipalPhone = "";
                checkModel.OldState = 0;
                checkModel.StaffId = UserGlobal.CurrUser.StaffId;
                checkModel.StaffName = staff.Name;

                context.FixedAssetsCheck.Add(checkModel);
                context.SaveChanges();
            }

           
            ModelCommon.CopyPropertyToModel(model, ref Model);
            Succeed = true;
            Close();
        }
    }
}
