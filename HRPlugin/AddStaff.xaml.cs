using Common;
using Common.Utils;
using DBModels.Staffs;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// AddStaff.xaml 的交互逻辑
    /// </summary>
    public partial class AddStaff : Window
    {
        string staffId = "";
        bool isEdit = false;

        public bool Succeed = false;
        public Staff StaffModel = new Staff();//员工信息

        public AddStaff(string _staffId = "")
        {
            InitializeComponent();
            this.UseCloseAnimation();

            if (string.IsNullOrEmpty(_staffId))
            {
                isEdit = false;

                DateTime currTime = DateTime.Now;
                staffId = $"S{TempBasePageData.message.CurrUser.Id}{currTime.ToString("yyyyMMddHHmmss")}";
                Title = "注册新员工";

                StaffModel.CreateTime = currTime;
                StaffModel.Creator = TempBasePageData.message.CurrUser.Id;

                new JobPostTreeViewCommon(tvJobPost).Init(false);
            }
            else
            {
                isEdit = true;
                staffId = _staffId;
                using (DBContext context = new DBContext())
                {
                    StaffModel = context.Staff.FirstOrDefault(c => c.Id == _staffId);

                    var jobpost = context.SysDic.First(c => c.Id == StaffModel.JobPostId);

                    #region Model2UI

                    lblJobPost.Content = jobpost.Name;
                    lblJobPost.Tag = jobpost.Id;

                    txtName.Text = StaffModel.Name;
                    cbSex.SelectedIndex = StaffModel.Sex;
                    txtPhone.Text = StaffModel.Phone;
                    txtWechat.Text = StaffModel.QQ;
                    txtIDCard.Text = StaffModel.IdCard;
                    dtRegister.SelectedDateTime = StaffModel.Register;
                    txtAddress.Text = StaffModel.Address;
                    txtNowAddress.Text = StaffModel.NowAddress;

                    #endregion
                }
                Title = $"编辑[{StaffModel.Name}]信息";

                new JobPostTreeViewCommon(tvJobPost).Init(false, false, StaffModel.JobPostId);
            }

            StaffModel.Id = staffId;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region UI Method

        private void tvJobPost_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem selectedItem = tvJobPost.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                if (selectedItem.Items.Count > 0) return;
                int selectedJobPostId = 0;
                if (int.TryParse(selectedItem.Tag.ToString(), out selectedJobPostId))
                {
                    lblJobPost.Content = selectedItem.Header;
                    lblJobPost.Tag = selectedJobPostId;
                }
            }
        }

        private void btnCopyAddress_Click(object sender, RoutedEventArgs e)
        {
            txtNowAddress.Text = txtAddress.Text;
        }

        private void btnReadIDCard_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("读卡器连接失败");
        }

        private void btnClearBaseInfo_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtIDCard.Clear();
            txtNowAddress.Clear();
            txtWechat.Clear();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Succeed = false;
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int jobpostId = lblJobPost.Tag.ToString().AsInt();

            #region Empty or Error

            if (jobpostId <= 0)
            {
                MessageBoxX.Show("请选择职务", "空值提醒");
                return;
            }

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBoxX.Show("请输入姓名", "空值提醒");
                txtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBoxX.Show("请输入手机号", "空值提醒");
                txtPhone.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtIDCard.Text))
            {
                MessageBoxX.Show("请输入身份证号", "空值提醒");
                txtIDCard.Focus();
                return;
            }

            #endregion

            #region UI2Model

            StaffModel.Name = txtName.Text;
            StaffModel.Sex = cbSex.SelectedIndex;
            StaffModel.Phone = txtPhone.Text;
            StaffModel.QQ = txtWechat.Text;
            StaffModel.IdCard = txtIDCard.Text;
            StaffModel.Register = dtRegister.SelectedDateTime;
            StaffModel.Address = txtAddress.Text;
            StaffModel.NowAddress = txtNowAddress.Text;
            StaffModel.JobPostId = jobpostId;
            StaffModel.QuickCode = $"{txtName.Text.Convert2Py().ToLower()}|{txtName.Text.Convert2Pinyin().ToLower() }";

            #endregion 

            using (DBContext context = new DBContext())
            {
                if (isEdit)
                {
                    if (context.Staff.Any(c => c.Id != StaffModel.Id && (c.IdCard == StaffModel.IdCard || c.Phone == StaffModel.Phone)))
                    {
                        MessageBoxX.Show("请检查手机号、身份证号是否重复", "员工已存在");
                        return;
                    }

                    #region 编辑状态

                    var _staff = context.Staff.Single(c => c.Id == StaffModel.Id);
                    ModelCommon.CopyPropertyToModel(StaffModel, ref _staff);

                    #endregion
                }
                else
                {
                    #region 添加状态

                    if (context.Staff.Any(c => c.IdCard == StaffModel.IdCard || c.Phone == StaffModel.Phone))
                    {
                        MessageBoxX.Show("请检查手机号、身份证号是否重复", "员工已存在");
                        return;
                    }
                    StaffModel.IsDel = false;
                    StaffModel.DelUser = 0;
                    StaffModel.DelTime = DateTime.Now;

                    StaffModel = context.Staff.Add(StaffModel);
                    DateTime currTime = DateTime.Now;

                    #endregion

                }

                context.SaveChanges();
            }
            Succeed = true;
            Close();
        }

        #endregion

    }
}
