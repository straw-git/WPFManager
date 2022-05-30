using Common;
using Common.Data.Local;
using Common.MyControls;
using CoreDBModels;
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

namespace Client.Windows
{
    /// <summary>
    /// EditEmail.xaml 的交互逻辑
    /// </summary>
    public partial class EditEmail : Window
    {
        int editId = 0;
        bool IsEdit
        {
            get { return editId > 0; }
        }

        public EditEmail(int _editId = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            editId = _editId;
            if (IsEdit)
            {
                //加载显示
                gEmail.IsEnabled = false;
                btnSend.IsEnabled = false;

                using (CoreDBContext context = new CoreDBContext())
                {
                    var emailSendToMeModel = context.EmailSendTo.First(c => c.Id == editId);
                    var emailModel = context.Email.First(c => c.Id == emailSendToMeModel.EmailId);

                    //将当前编辑为已读
                    var sendModel = context.EmailSendTo.Single(c => c.Id == emailSendToMeModel.EmailId);
                    if (sendModel.UserId == 0 && sendModel.RoleId == 0)
                    {
                        var _readed = LocalReadEmail.GetAllUserEmail();//获取本地已阅读的邮件
                        if (!_readed.Contains(editId.ToString()))//如果不在已阅读的列表中 添加
                            LocalReadEmail.ReadAllUserEmail(editId);
                    }
                    if (sendModel.RoleId > 0)
                    {
                        var _readed = LocalReadEmail.GetRoleEmail();//获取本地已阅读的邮件
                        if (!_readed.Contains(editId.ToString()))//如果不在已阅读的列表中 添加
                        LocalReadEmail.ReadRoleEmail(editId);
                    }
                    //修改数据库
                    if (sendModel.UserId > 0)
                    {
                        sendModel.IsRead = true;
                        sendModel.UserReadTime = DateTime.Now.ToString();
                        context.SaveChanges();
                    }

                    txtTitle.Text = emailModel.Title;
                    rtbEditor.SetTime(emailModel.SendTime.ToString(rtbEditor.TimerFormatStr));
                    rtbEditor.SetText(emailModel.Content);
                }
            }
        }

        /// <summary>
        /// 选中的用户
        /// </summary>
        List<UserSelector.CheckedUserUIModel> selectedUsers = new List<UserSelector.CheckedUserUIModel>();
        /// <summary>
        /// 主题信息
        /// </summary>
        LocalSkin.SkinModel skinInfo = LocalSkin.skins.First(c => c.SkinId == LocalSettings.settings.SkinId);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoles();//加载所有角色
            myUserSelector.OnCheckedChanged += OnSelectedUserChanged;//添加事件监听
            rbSend2User.IsChecked = true;//选择发送人

            if (!IsEdit)
            {
                //如果是新建模式 
                rtbEditor.StartTime();//开始计时
            }
        }

        /// <summary>
        /// 加载角色
        /// </summary>
        private void LoadRoles()
        {
            List<Role> roles = new List<Role>();
            using (CoreDBContext context = new CoreDBContext())
            {
                roles = context.Role.Where(c => !c.IsDel).ToList();
            }
            cbRoles.ItemsSource = roles;
            cbRoles.DisplayMemberPath = "Name";
            cbRoles.SelectedValuePath = "Id";
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //移除监听
            RemoveSelectedUserListener();
        }

        #region 选择用户

        private void OnSelectedUserChanged(List<UserSelector.CheckedUserUIModel> _users)
        {
            selectedUsers = _users;

            RemoveSelectedUserListener();
            spSelectedUsers.Children.Clear();
            if (_users != null)
            {
                foreach (var item in _users)
                {
                    Button button = new Button();
                    button.Background = ColorHelper.ConvertToSolidColorBrush(skinInfo.ButtonBgColor);
                    ButtonHelper.SetHoverBrush(button, ColorHelper.ConvertToSolidColorBrush(skinInfo.ButtonHoverColor));
                    button.Content = item.UserName;
                    button.Tag = item.UserId;
                    button.Margin = new Thickness(10, 0, 0, 0);
                    button.Click += OnSlectedUser_Click;

                    spSelectedUsers.Children.Add(button);
                }
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        private void RemoveSelectedUserListener()
        {
            foreach (var item in spSelectedUsers.Children)
            {
                Button button = item as Button;
                button.Click -= OnSlectedUser_Click;

            }
        }

        private void OnSlectedUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var result = MessageBoxX.Show($"是否取消对[{button.Content}]发送邮件？", "取消发送人提醒", this, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                int userId = (int)button.Tag;
                myUserSelector.RemoveSelctedUser(userId);
            }
        }

        private void btnSelectUser_Click(object sender, RoutedEventArgs e)
        {
            if (myUserSelector.Visibility == Visibility.Collapsed)
            {
                myUserSelector.Visibility = Visibility.Visible;
                myUserSelector.ReLoad();
            }
            else
            {
                myUserSelector.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string emailContent = rtbEditor.GetText();
            string title = txtTitle.Text;
            if (emailContent.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入内容", "空值提醒");
                return;
            }
            if (title.IsNullOrEmpty())
            {
                title = $"{UserGlobal.CurrUser.RealName}-{DateTime.Now.ToString("yyyy.MM.dd")}";
            }

            if ((bool)rbSend2User.IsChecked)
            {
                #region 发送给个人

                if (selectedUsers.Count == 0)
                {
                    MessageBoxX.Show("请选择接收人", "空值提醒");
                    btnSelectUser_Click(null, null);//模拟点击
                    return;
                }

                using (CoreDBContext context = new CoreDBContext())
                {
                    var _email = context.Email.Add(new Email()
                    {
                        Content = emailContent,
                        EmailType = 0,
                        FromId = UserGlobal.CurrUser.Id,
                        SendTime = DateTime.Now,
                        Title = title
                    });
                    if (context.SaveChanges() > 0)
                    {
                        //发送成功
                        foreach (var item in selectedUsers)
                        {
                            context.EmailSendTo.Add(new EmailSendTo()
                            {
                                EmailId = _email.Id,
                                EmailTitle = title,
                                IsRead = false,
                                RoleId = 0,
                                SendTime = _email.SendTime,
                                UserId = item.UserId,
                                UserReadTime = ""
                            });
                        }
                        if (context.SaveChanges() > 0)
                        {
                            this.Log("发送成功");
                            btnClose_Click(null, null);//模拟点击关闭
                        }
                    }
                }

                #endregion 
            }
            else if ((bool)rbSend2AllUser.IsChecked)
            {
                #region 发送给所有人

                using (CoreDBContext context = new CoreDBContext())
                {
                    var _email = context.Email.Add(new Email()
                    {
                        Content = emailContent,
                        EmailType = 1,
                        FromId = UserGlobal.CurrUser.Id,
                        SendTime = DateTime.Now,
                        Title = title
                    });
                    if (context.SaveChanges() > 0)
                    {
                        //发送成功

                        context.EmailSendTo.Add(new EmailSendTo()
                        {
                            EmailId = _email.Id,
                            EmailTitle = title,
                            IsRead = false,
                            RoleId = 0,
                            SendTime = _email.SendTime,
                            UserId = 0,
                            UserReadTime = ""
                        });
                        if (context.SaveChanges() > 0)
                        {
                            this.Log("发送成功");
                            btnClose_Click(null, null);//模拟点击关闭
                        }
                    }
                }

                #endregion
            }
            else if ((bool)rbSend2Role.IsChecked)
            {
                #region 发送给角色

                if (cbRoles.SelectedItem == null)
                {
                    MessageBoxX.Show("没有发现角色信息", "空值提醒");
                    return;
                }

                Role selectedRole = cbRoles.SelectedItem as Role;

                using (CoreDBContext context = new CoreDBContext())
                {
                    var _email = context.Email.Add(new Email()
                    {
                        Content = emailContent,
                        EmailType = 2,
                        FromId = UserGlobal.CurrUser.Id,
                        SendTime = DateTime.Now,
                        Title = title
                    });
                    if (context.SaveChanges() > 0)
                    {
                        //发送成功
                        context.EmailSendTo.Add(new EmailSendTo()
                        {
                            EmailId = _email.Id,
                            EmailTitle = title,
                            IsRead = false,
                            RoleId = selectedRole.Id,
                            SendTime = _email.SendTime,
                            UserId = 0,
                            UserReadTime = ""
                        });
                        if (context.SaveChanges() > 0)
                        {
                            this.Log("发送成功");
                            btnClose_Click(null, null);//模拟点击关闭
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #region 选择发送条件

        private void rbSend2Role_Checked(object sender, RoutedEventArgs e)
        {
            //发送给角色
            gSelectRoles.Visibility = Visibility.Visible;
            //清空底部
            spSelectedUsers.Children.Clear();
            selectedUsers.Clear();
            if (cbRoles.Items.Count > 0)
                cbRoles.SelectedIndex = 0;
            //禁用选择按钮
            btnSelectUser.IsEnabled = false;
        }

        private void rbSend2User_Checked(object sender, RoutedEventArgs e)
        {
            //发送给用户
            gSelectRoles.Visibility = Visibility.Collapsed;
            //清空底部
            spSelectedUsers.Children.Clear();
            selectedUsers.Clear();
            //启用选择按钮
            btnSelectUser.IsEnabled = true;
        }

        private void rbSend2AllUser_Checked(object sender, RoutedEventArgs e)
        {
            //发送给所有用户
            gSelectRoles.Visibility = Visibility.Collapsed;
            //底部
            spSelectedUsers.Children.Clear();
            selectedUsers.Clear();
            Button button = new Button();
            button.Background = ColorHelper.ConvertToSolidColorBrush(skinInfo.ButtonBgColor);
            ButtonHelper.SetHoverBrush(button, ColorHelper.ConvertToSolidColorBrush(skinInfo.ButtonHoverColor));
            button.Content = "发送给所有人";
            button.Tag = 0;
            spSelectedUsers.Children.Add(button);
            selectedUsers.Add(new UserSelector.CheckedUserUIModel() { UserId = 0, UserName = "" });
            //禁用选择按钮
            btnSelectUser.IsEnabled = false;
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRoles.SelectedItem == null) return;

            Role selectedRole = cbRoles.SelectedItem as Role;
            if (spSelectedUsers.Children.Count > 0)
            {
                spSelectedUsers.Children.Clear();
            }

            Button button = new Button();
            button.Background = ColorHelper.ConvertToSolidColorBrush(skinInfo.ButtonBgColor);
            ButtonHelper.SetHoverBrush(button, ColorHelper.ConvertToSolidColorBrush(skinInfo.ButtonHoverColor));
            button.Content = selectedRole.Name;
            button.Tag = selectedRole.Id;

            spSelectedUsers.Children.Add(button);
        }

        #endregion

    }
}
