
using CoreDBModels;
using CorePlugin.Windows;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using CoreDBModels.Models;

namespace CorePlugin.Pages.Manager
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Dic : BasePage
    {
        public Dic()
        {
            InitializeComponent();
            this.Order = 2;
        }

        /// <summary>
        /// 页面数据模型
        /// </summary>
        private class UIModel : INotifyPropertyChanged
        {
            public int Id { get; set; }
            private string name = "";
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
            public string Code { get; set; }
            public string ParentName { get; set; }
            public string CreaterName { get; set; }
            private string content = "";
            public string Content
            {
                get => content;
                set
                {
                    content = value;
                    NotifyPropertyChanged("Content");
                    NotifyPropertyChanged("SubContent");
                }
            }
            public string SubContent
            {
                get
                {
                    string _c = content;
                    if (content.Length > 20)
                    {
                        _c = content.Substring(0, 20) + "...";
                    }
                    return _c;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        //选中的简码
        private string selectedCode = "";

        //当前页码
        private int currPage = 1;
        //数据总数
        private int dataCount = 0;
        //总页数
        private int pagerCount = 0;
        //页数据
        private int pageSize = 10;
        bool running = false;

        protected override void OnPageLoaded()
        {
            LoadTVMain();
            InitList();
        }

        #region Private Method

        /// <summary>
        /// 加载导航
        /// </summary>
        private void LoadTVMain()
        {
            tvMain.Items.Clear();

            List<SysDic> list = new List<SysDic>();
            using (CoreDBContext context = new CoreDBContext())
            {
                list = context.SysDic.Where(c => c.ParentCode == "").ToList();

                foreach (var item in list)
                {
                    int count = context.SysDic.Count(c => c.ParentCode == item.QuickCode);

                    tvMain.Items.Add(new TreeViewItem()
                    {
                        Header = $"{item.Name}-[{count}]",
                        ToolTip = $"{item.Name} [ 数据量：{count} ]",
                        Margin = new Thickness(2, 0, 0, 2),
                        Padding = new Thickness(10, 0, 0, 0),
                        Background = Brushes.Transparent,
                        Tag = item.QuickCode,
                        IsSelected = false
                    });
                }
            }

            var firstItem = (tvMain.Items[0] as TreeViewItem);
            firstItem.IsSelected = true;
        }

        /// <summary>
        /// 加载分页
        /// </summary>
        private void LoadPager()
        {
            if (selectedCode == DicData.JobPost)
            {
                #region jobpost pager

                //组织架构页码

                #endregion 
            }
            else
            {
                #region normal pager

                using (var context = new CoreDBContext())
                {
                    dataCount = context.SysDic.Where(c => c.ParentCode == selectedCode).Count();
                }
                pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);

                if (currPage > pagerCount) currPage = pagerCount;
                dPager.CurrentIndex = currPage;
                dPager.TotalIndex = pagerCount;

                #endregion 
            }
        }


        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitList()
        {
            dList.DataContext = Data;
            LoadPager();
        }

        /// <summary>
        /// 更新表格
        /// </summary>
        private async void UpdateGridAsync()
        {
            if (running) return;
            running = true;
            if (selectedCode == DicData.JobPost)
            {
                #region load jobpost code data 

                tvJobpost.Items.Clear();

                List<SysDic> dics = new List<SysDic>();

                using (var context = new CoreDBContext())
                {
                    var jobpostList = context.SysDic.Where(c => c.ParentCode == DicData.JobPost);
                    foreach (var dic in jobpostList)
                    {
                        TreeViewItem item = new TreeViewItem();
                        item.Header = dic.Name;
                        item.Margin = new Thickness(2, 0, 0, 2);
                        item.Padding = new Thickness(10, 0, 0, 0);
                        item.Background = Brushes.Transparent;
                        item.Tag = dic.QuickCode;
                        item.IsSelected = false;
                        tvJobpost.Items.Add(item);
                        GetChildItem(dic.QuickCode, item);
                    }
                }

                await Task.Delay(300);
                //组织架构刷新

                HideLoadingPanel();
                #endregion 
            }
            else
            {
                #region load normal code data

                Data.Clear();
                List<SysDic> dics = new List<SysDic>();
                await Task.Run(() =>
                {
                    using (CoreDBContext context = new CoreDBContext())
                    {
                        dics = context.SysDic.Where(c => c.ParentCode == selectedCode).OrderByDescending(c => c.Id).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                    }
                });
                await Task.Delay(300);

                string parentName = (tvMain.SelectedItem as TreeViewItem).Header.ToString();
                parentName = parentName.Substring(0, parentName.LastIndexOf('-'));

                using (CoreDBContext context = new CoreDBContext())
                {
                    foreach (var item in dics)
                    {
                        string creater = "系统";
                        if (item.Creater > 0)
                        {
                            creater = context.User.First(c => c.Id == item.Creater).Name;
                        }
                        Data.Add(new UIModel()
                        {
                            Code = item.QuickCode,
                            CreaterName = creater,
                            Id = item.Id,
                            Name = item.Name,
                            ParentName = parentName,
                            Content = item.Content
                        });
                    }
                }

                HideLoadingPanel();

                #endregion 
            }
            running = false;
        }

        private void GetChildItem(string _parentCode, TreeViewItem _parentItem)
        {
            using (var context = new CoreDBContext())
            {
                var dics = context.SysDic.Where(c => c.ParentCode == _parentCode);
                if (dics == null || dics.Count() == 0) return;
                foreach (var dic in dics)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = dic.Name;
                    item.Margin = new Thickness(2, 0, 0, 2);
                    item.Padding = new Thickness(10, 0, 0, 0);
                    item.Background = Brushes.Transparent;
                    item.Tag = dic.QuickCode;
                    item.IsSelected = false;
                    _parentItem.Items.Add(item);
                    GetChildItem(dic.QuickCode, item);
                }
            }
        }

        #endregion

        #region UI Method

        private void btnAddParentDic_Click(object sender, RoutedEventArgs e)
        {
            #region add dic item

            ParentWindow.MaskVisible(true);
            EditDic add = new EditDic("");
            add.ShowDialog();
            if (add.Succeed)
            {
                LoadTVMain();
                UpdateGridAsync();
                LoadPager();
            }
            ParentWindow.MaskVisible(false);

            #endregion 
        }

        private void tvMain_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = tvMain.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.IsSelected)
            {
                //更新选中
                selectedCode = selectedItem.Tag.ToString();
                ShowLoadingPanel();
                UpdateGridAsync();
                LoadPager();
                ShowChildPage();
            }
        }

        /// <summary>
        /// 显示子页面
        /// </summary>
        private void ShowChildPage()
        {
            if (selectedCode == DicData.JobPost)
            {
                normal.Visibility = Visibility.Collapsed;
                jobpost.Visibility = Visibility.Visible;
            }
            else
            {
                normal.Visibility = Visibility.Visible;
                jobpost.Visibility = Visibility.Collapsed;
            }
        }

        private void btnAddDic_Click(object sender, RoutedEventArgs e)
        {
            #region add dic item

            ParentWindow.MaskVisible(true);
            EditDic add = new EditDic(selectedCode);
            add.ShowDialog();
            if (add.Succeed)
            {
                UpdateGridAsync();
                LoadPager();
            }
            ParentWindow.MaskVisible(false);

            #endregion 
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            ShowLoadingPanel();
            UpdateGridAsync();
        }

        private void dPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = dPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void listEditButton_Click(object sender, RoutedEventArgs e)
        {
            Button target = sender as Button;
            string code = target.Tag.ToString();

            SysDic editModel = new SysDic();
            using (var context = new CoreDBContext())
            {
                editModel = context.SysDic.First(c => c.QuickCode == code);
            }
            ParentWindow.MaskVisible(true);
            EditDic edit = new EditDic(selectedCode, true, editModel);
            edit.ShowDialog();
            if (edit.Succeed)
            {
                //成功后 更新结果
                var targetData = Data.Single(c => c.Code == code);
                targetData.Name = edit.ResultName;
                targetData.Content = edit.ResultContent;
            }
            ParentWindow.MaskVisible(false);
        }

        private void listDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button target = sender as Button;
            string code = target.Tag.ToString();

            using (var context = new CoreDBContext())
            {
                SysDic dic = context.SysDic.First(c => c.QuickCode == code);
                if (dic.Creater == 0)
                {
                    MessageBoxX.Show("系统创建的对象不允许修改或删除", "操作失败");
                    return;
                }
                var result = MessageBoxX.Show($"是否确认删除[{dic.Name}]？", "删除提醒", null, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    context.SysDic.Remove(dic);
                    context.SaveChanges();
                    LoadPager();
                    btnRef_Click(null, null);
                }
            }
        }

        private void btnDeleteJobPost_Click(object sender, RoutedEventArgs e)
        {
            if (tvJobpost.SelectedItem == null) return;

            TreeViewItem item = tvJobpost.SelectedItem as TreeViewItem;
            if (item == null) return;

            string name = item.Header.ToString();
            string code = item.Tag.ToString();
            using (var context = new CoreDBContext())
            {
                var result = MessageBoxX.Show($"是否确认删除[{name}]？", "删除提醒", null, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    context.SysDic.Remove(context.SysDic.First(c => c.QuickCode == code));
                    context.SaveChanges();
                    LoadPager();
                    btnRef_Click(null, null);
                }
            }
        }

        private void dList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dList.SelectedItem == null) return;
            UIModel focusModel = dList.SelectedItem as UIModel;
            if (focusModel == null || focusModel.Id <= 0) return;

            string code = focusModel.Code;

            SysDic editModel = new SysDic();
            using (var context = new CoreDBContext())
            {
                editModel = context.SysDic.First(c => c.QuickCode == code);
            }

            if (editModel.Creater == 0)
            {
                MessageBoxX.Show("系统创建的对象不允许修改或删除", "操作失败");
                return;
            }

            ParentWindow.MaskVisible(true);
            EditDic edit = new EditDic(selectedCode, true, editModel);
            edit.ShowDialog();
            if (edit.Succeed)
            {
                //成功后 更新结果
                var targetData = Data.Single(c => c.Code == code);
                targetData.Name = edit.ResultName;
                targetData.Content = edit.ResultContent;
            }
            ParentWindow.MaskVisible(false);
        }

        private void btnAddJobPost_Click(object sender, RoutedEventArgs e)
        {
            if (tvJobpost.SelectedItem == null) return;

            TreeViewItem item = tvJobpost.SelectedItem as TreeViewItem;
            if (item == null) return;

            string name = item.Header.ToString();
            string code = item.Tag.ToString();

            ParentWindow.MaskVisible(true);
            EditJobPost add = new EditJobPost(name, code);
            add.ShowDialog();
            if (add.Succeed)
            {
                btnRef_Click(null, null);
            }
            ParentWindow.MaskVisible(false);
        }

        #endregion 

        #region Loading

        private void ShowLoadingPanel()
        {
            if (lData.Visibility != Visibility.Visible)
            {
                lData.Visibility = Visibility.Visible;
                dList.IsEnabled = false;
                dPager.IsEnabled = false;
                tvJobpost.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (lData.Visibility != Visibility.Collapsed)
            {
                lData.Visibility = Visibility.Collapsed;
                dList.IsEnabled = true;
                dPager.IsEnabled = true;
                tvJobpost.IsEnabled = true;
                OnLoadingHideComplate();
            }
        }

        private void OnLoadingHideComplate()
        {

        }

        private void OnLoadingShowComplate()
        {

        }

        #endregion

    }
}
