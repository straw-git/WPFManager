using Common;
using CorePlugin.Events;
using CorePlugin.Windows;
using Microsoft.Win32;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
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

namespace CorePlugin.MyControls
{
    /// <summary>
    /// PluginsItem.xaml 的交互逻辑
    /// </summary>
    public partial class PluginsItem : UserControl
    {
        public CoreDBModels.Plugins model;//当前页面数据模型
        public PluginsItem(CoreDBModels.Plugins _model)
        {
            InitializeComponent();
            model = _model;

            InitInfo();
        }

        ObservableCollection<string> DllsData = new ObservableCollection<string>();//关联文件数据源

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDependent();
            lbDLLs.ItemsSource = DllsData;
            UpdateImage(model.LogoImage);
        }

        //初始化信息
        private void InitInfo()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                lblName.Content = model.Name;
                txtName.Text = model.Name;
                lblDLLName.Content = model.DLLName;
                txtDLLName.Text = model.DLLName;
                lblModuleCount.Content = context.PluginsModule.Any(c => c.PluginsId == model.Id) ? context.PluginsModule.Count(c => c.PluginsId == model.Id) : 0;
                lblPageCount.Content = context.ModulePage.Any(c => c.PluginsId == model.Id) ? context.ModulePage.Count(c => c.PluginsId == model.Id) : 0;
                lblWebDownload.Content = model.WebDownload ? "是" : "否";
                cbWebDownload.IsChecked = model.WebDownload;
                lblEnablePlugins.Content = model.IsEnable ? "是" : "否";
                cbEnablePlugins.IsChecked = model.IsEnable;
                lblConnectionName.Content = model.ConnectionName;
                lblConnectionName.ToolTip = model.ConnectionName;
                txtConnectionName.Text = model.ConnectionName;
                lblConnectionString.Content = model.ConnectionString;
                lblConnectionString.ToolTip = model.ConnectionString;
                txtConnectionString.Text = model.ConnectionString;

                spDependentPlugins.Children.Clear();

                string[] ids = model.DependentIds.Split('|');
                if (ids.Length > 0)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        string idStr = ids[i];
                        if (idStr.IsNullOrEmpty()) continue;
                        int id = idStr.AsInt();

                        var pluginsData = context.Plugins.First(c => c.Id == id);
                        DependentPluginsItem dependentPluginsItem = new DependentPluginsItem(model.Id, pluginsData);
                        spDependentPlugins.Children.Add(dependentPluginsItem);
                    }
                }

                DllsData.Clear();
                var dlls = model.DLLs.Split('|').ToList();
                foreach (var d in dlls)
                {
                    if (d.IsNullOrEmpty()) continue;
                    DllsData.Add(d);
                }

                lblName.Visibility = Visibility.Visible;
                lblDLLName.Visibility = Visibility.Visible;
                lblWebDownload.Visibility = Visibility.Visible;
                lblEnablePlugins.Visibility = Visibility.Visible;
                lblConnectionName.Visibility = Visibility.Visible;
                lblConnectionString.Visibility = Visibility.Visible;

                txtName.Visibility = Visibility.Collapsed;
                txtDLLName.Visibility = Visibility.Collapsed;
                cbWebDownload.Visibility = Visibility.Collapsed;
                cbEnablePlugins.Visibility = Visibility.Collapsed;
                txtConnectionName.Visibility = Visibility.Collapsed;
                txtConnectionString.Visibility = Visibility.Collapsed;
            }
        }

        //编辑模块信息
        private void btnEditModules_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            EditPlugins editPlugins = new EditPlugins(model.Id);
            editPlugins.ShowDialog();
            this.MaskVisible(false);

            InitInfo();
        }

        //删除插件信息
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxX.Show($"是否确认删除插件[{model.Name}:{model.DLLName}]？", "删除提醒", null, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                //派发删除事件
                PluginsDeleteEventObserver.Instance.Dispatch(Codes.PluginsDelete, new PluginsDeleteMessage() { Id = model.Id });
            }
        }

        //编辑插件信息
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            lblName.Visibility = Visibility.Collapsed;
            lblDLLName.Visibility = Visibility.Collapsed;
            lblWebDownload.Visibility = Visibility.Collapsed;
            lblEnablePlugins.Visibility = Visibility.Collapsed;
            lblConnectionName.Visibility = Visibility.Collapsed;
            lblConnectionString.Visibility = Visibility.Collapsed;

            txtName.Visibility = Visibility.Visible;
            txtDLLName.Visibility = Visibility.Visible;
            cbWebDownload.Visibility = Visibility.Visible;
            cbEnablePlugins.Visibility = Visibility.Visible;
            txtConnectionName.Visibility = Visibility.Visible;
            txtConnectionString.Visibility = Visibility.Visible;

            btnEdit.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;
        }

        //保存插件信息
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var plugins = context.Plugins.Single(c => c.Id == model.Id);
                plugins.ConnectionName = txtConnectionName.Text;
                plugins.ConnectionString = txtConnectionString.Text;
                plugins.DLLName = txtDLLName.Text;
                plugins.IsEnable = (bool)cbEnablePlugins.IsChecked;
                plugins.Name = txtName.Text;
                plugins.WebDownload = (bool)cbWebDownload.IsChecked;

                context.SaveChanges();
                model = plugins;
            }

            //model=更新Model
            InitInfo();

            btnEdit.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Collapsed;
        }

        #region 依赖插件

        //加载依赖插件
        private void LoadDependent()
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                var dPlugins = context.Plugins.Where(c => c.Id != model.Id).ToList();
                cbDependent.ItemsSource = dPlugins;
                cbDependent.DisplayMemberPath = "Name";
                cbDependent.SelectedValuePath = "Id";

                if (dPlugins != null && dPlugins.Count > 0)
                {
                    cbDependent.SelectedIndex = 0;
                }
            }
        }

        //添加依赖插件
        private void btnAddDependent_Click(object sender, RoutedEventArgs e)
        {
            if (cbDependent.SelectedItem == null) return;
            var pluginsData = cbDependent.SelectedItem as CoreDBModels.Plugins;

            List<string> ids = model.DependentIds.Split('|').ToList();
            if (ids.Contains(pluginsData.Id.ToString()))
            {
                MessageBoxX.Show($"[{pluginsData.Name}]已添加", "数据重复");
                return;
            }

            ids.Add(pluginsData.Id.ToString());
            using (CoreDBContext context = new CoreDBContext())
            {
                string dids = "";
                foreach (var id in ids)
                {
                    if (id.IsNullOrEmpty()) continue;
                    dids += $"|{id}";
                }
                if (dids.NotEmpty()) dids = dids.Substring(1);
                context.Plugins.Single(c => c.Id == model.Id).DependentIds = dids;
                model.DependentIds = dids;
                if (context.SaveChanges() > 0)
                {
                    DependentPluginsItem dependentPluginsItem = new DependentPluginsItem(model.Id, pluginsData);
                    spDependentPlugins.Children.Add(dependentPluginsItem);
                }
            }
        }

        #endregion

        #region 背景图处理

        public void UpdateImage(string _imgSource = "")
        {
            if (_imgSource.IsNullOrEmpty())
            {
                //没有图
                realImg.Visibility = Visibility.Collapsed;
                btnNewImage.Visibility = Visibility.Visible;
            }
            else
            {
                //有图
                realImg.Visibility = Visibility.Visible;
                img.Source = new BitmapImage(new Uri($"{UserGlobal.CoreSetting.APIUrl}imgs/{_imgSource}", UriKind.RelativeOrAbsolute));
                btnNewImage.Visibility = Visibility.Collapsed;
            }
        }

        private void realImg_MouseEnter(object sender, MouseEventArgs e)
        {
            btnUpdateImage.Visibility = Visibility.Visible;
            btnDeleteImage.Visibility = Visibility.Visible;
            op.Visibility = Visibility.Visible;
        }

        private void realImg_MouseLeave(object sender, MouseEventArgs e)
        {
            btnUpdateImage.Visibility = Visibility.Collapsed;
            btnDeleteImage.Visibility = Visibility.Collapsed;
            op.Visibility = Visibility.Collapsed;
        }

        private void btnUpdateImage_Click(object sender, RoutedEventArgs e)
        {
            UploadLogoImage();
        }

        /// <summary>
        /// 上传插件Logo
        /// </summary>
        /// <returns></returns>
        private bool UploadLogoImage()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*"
            };

            if ((bool)openfiledialog.ShowDialog())
            {
                var client = new WebClient();
                byte[] _b = client.UploadFile($"{UserGlobal.CoreSetting.APIUrl}Home/PostFile", "POST", openfiledialog.FileName);
                string fileName = Encoding.UTF8.GetString(_b, 0, _b.Length);

                using (CoreDBContext context = new CoreDBContext())
                {
                    context.Plugins.Single(c => c.Id == model.Id).LogoImage = fileName;
                    if (context.SaveChanges() > 0)
                    {
                        UpdateImage(fileName);
                    }
                }
                return true;
            }
            return false;
        }

        //删除图片
        private void btnDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                context.Plugins.Single(c => c.Id == model.Id).LogoImage = "";
                if (context.SaveChanges() > 0)
                {
                    UpdateImage();
                }
            }
        }

        //上传新图
        private void btnNewImage_Click(object sender, RoutedEventArgs e)
        {
            UploadLogoImage();
        }

        #endregion

        #region 关联文件

        //删除关联文件
        private void btnDeleteDLL_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();//获取ID
            var result = MessageBoxX.Show($"是否确认删除关联文件[{id}]？", "关联文件删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) 
            {
                using (CoreDBContext context = new CoreDBContext())
                {
                    var plugins = context.Plugins.Single(c => c.Id == model.Id);
                    string dlls = plugins.DLLs;

                    #region 从字符串中移除

                    List<string> dllList = dlls.Split('|').ToList();
                    dllList.Remove(id);

                    dlls = "";
                    foreach (var item in dllList)
                    {
                        if (item.IsNullOrEmpty()) continue;
                        dlls += $"|{item}";
                    }
                    if (dlls.NotEmpty()) dlls = dlls.Substring(1);

                    #endregion 

                    plugins.DLLs = dlls;
                    if (context.SaveChanges() > 0)//执行删除
                    {
                        DllsData.Remove(id);//更新UI
                    }
                }
            }
        }

        //添加关联文件
        private void btnAddDlls_Click(object sender, RoutedEventArgs e)
        {
            UploadFiles();
        }

        /// <summary>
        /// 上传关联文件
        /// </summary>
        /// <returns></returns>
        private bool UploadFiles()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                Filter = "所有文件|*.*"
            };

            if ((bool)openfiledialog.ShowDialog())
            {
                var client = new WebClient();
                byte[] _b = client.UploadFile($"{UserGlobal.CoreSetting.APIUrl}Home/PostFile", "POST", openfiledialog.FileName);
                string fileName = Encoding.UTF8.GetString(_b, 0, _b.Length);

                using (CoreDBContext context = new CoreDBContext())
                {
                    var plugins = context.Plugins.Single(c => c.Id == model.Id);
                    bool edit = true;
                    string dlls = plugins.DLLs;

                    if (dlls.IsNullOrEmpty())
                        dlls = fileName;
                    else if (dlls.Contains(fileName))
                        edit = false;
                    else
                        dlls += $"|{fileName}";

                    if (edit)
                    {
                        plugins.DLLs = dlls;
                        if (context.SaveChanges() > 0)
                        {
                            DllsData.Add(fileName);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

    }
}
