
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

namespace HRPlugin.Pages.HR
{
    /// <summary>
    /// Insurance.xaml 的交互逻辑
    /// </summary>
    public partial class Insurance : BasePage
    {
        public Insurance()
        {
            InitializeComponent();
            this.Order = 2;
            IsMenu = false;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public string Id { get; set; }
            public string JobpostName { get; set; }
            public string Name { get; set; }

            private string sB = "";
            public string SB 
            { 
                get=>sB; 
                set 
                {
                    sB = value;
                    NotifyPropertyChanged("SB");
                }
            }

            private string sBStartTime = "";
            public string SBStartTime 
            {
                get=>sBStartTime; 
                set 
                {
                    sBStartTime = value;
                    NotifyPropertyChanged("SBStartTime");
                }
            }

            private string sY = "";
            public string SY 
            {
                get=>sY; 
                set 
                {
                    sY = value;
                    NotifyPropertyChanged("SY");
                }
            }

            private int sYCount = 0;
            public int SYCount 
            {
                get=>sYCount;
                set 
                {
                    sYCount = value;
                    NotifyPropertyChanged("SYCount");
                } 
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        int dataCount = 0;
        int pagerCount = 0;
        int pageSize = 10;
        int currPage = 1;
        bool running = false;

        protected override void OnPageLoaded()
        {
            list.ItemsSource = Data;
            btnRef_Click(null, null);
        }

        #region Private Method

        private void LoadPager()
        {
            using (var context = new DBContext())
            {
                string name = txtName.Text;

                var staffs = context.Staff.AsEnumerable();

                if (!string.IsNullOrEmpty(name))
                {
                    staffs = staffs.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name));
                }
                dataCount = staffs.Count();
            }
            pagerCount = PagerGlobal.GetPagerCount(dataCount, pageSize);

            if (currPage > pagerCount) currPage = pagerCount;
            gPager.CurrentIndex = currPage;
            gPager.TotalIndex = pagerCount;
        }

        private async void UpdateGridAsync()
        {
            ShowLoadingPanel();
            if (running) return;
            running = true;
            Data.Clear();

            List<DBModels.Staffs.Staff> models = new List<DBModels.Staffs.Staff>();

            string name = txtName.Text;

            await Task.Run(() =>
            {
                using (var context = new DBContext())
                {
                    var staffs = context.Staff.AsEnumerable();

                    if (!string.IsNullOrEmpty(name))
                    {
                        staffs = staffs.Where(c => c.Name.Contains(name) || c.QuickCode.Contains(name));
                    }
                    models = staffs.OrderByDescending(c => c.CreateTime).Skip(pageSize * (currPage - 1)).Take(pageSize).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = models.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (DBContext context = new DBContext())
            {
                foreach (var item in models)
                {
                    UIModel _model = new UIModel()
                    {
                        Id = item.Id,
                        JobpostName = "",
                        Name = item.Name,
                        SB = "未参保",
                        SBStartTime = "",
                        SY = "无",
                        SYCount=0
                    };

                    _model.JobpostName = context.SysDic.First(c => c.Id == item.JobPostId).Name;

                    if (context.StaffInsurance.Any(c => c.StaffId == item.Id))
                    {
                        bool _sb = context.StaffInsurance.Any(c => c.StaffId == item.Id&&c.Type==0);
                        bool _sy = context.StaffInsurance.Any(c => c.StaffId == item.Id && c.Type == 1);
                        if (_sb) 
                        {
                            _model.SB = "已参保";
                            _model.SBStartTime = context.StaffInsurance.First(c => c.StaffId == item.Id && c.Type == 0).Start.ToString("yyyy年MM月dd日");
                        }

                        if (_sy) 
                        {
                            _model.SY = "有";
                            _model.SYCount = context.StaffInsurance.Count(c => c.StaffId == item.Id && c.Type == 1);
                        }
                    }

                    Data.Add(_model);
                }
            }
            HideLoadingPanel();
            running = false;
        }

        #endregion

        #region UI Method

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            btnRef_Click(null, null);
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            LoadPager();
            UpdateGridAsync();
        }

        private void gPager_CurrentIndexChanged(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            currPage = gPager.CurrentIndex;
            btnRef_Click(null, null);
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null) return;
            UIModel selectModel = list.SelectedItem as UIModel;

            MaskVisible(true);
            EditInsurance i = new EditInsurance(selectModel.Id);
            i.ShowDialog();
            MaskVisible(false);

            using (DBContext context = new DBContext()) 
            {
                var _model = Data.Single(c => c.Id == selectModel.Id);
                if (context.StaffInsurance.Any(c => c.StaffId == selectModel.Id))
                {
                    bool _sb = context.StaffInsurance.Any(c => c.StaffId == selectModel.Id && c.Type == 0);
                    bool _sy = context.StaffInsurance.Any(c => c.StaffId == selectModel.Id && c.Type == 1);
                    if (_sb)
                    {
                        _model.SB = "已参保";
                        _model.SBStartTime = context.StaffInsurance.First(c => c.StaffId == selectModel.Id && c.Type == 0).Start.ToString("yyyy年MM月dd日");
                    }

                    if (_sy)
                    {
                        _model.SY = "有";
                        _model.SYCount = context.StaffInsurance.Count(c => c.StaffId == selectModel.Id && c.Type == 1);
                    }
                }
            }
        }

        #endregion

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;
                gPager.IsEnabled = false;
                bNoData.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;
                gPager.IsEnabled = true;
                bNoData.IsEnabled = true;

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
