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
    /// Contract.xaml 的交互逻辑
    /// </summary>
    public partial class Contract : BasePage
    {
        public Contract()
        {
            InitializeComponent();
            this.Order = 1;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public string Id { get; set; }
            public string JobpostName { get; set; }
            public string Name { get; set; }

            private string startTime = "";
            public string StartTime
            {
                get => startTime;
                set
                {
                    startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }

            private string endTime = "";
            public string EndTime
            {
                get => endTime;
                set
                {
                    endTime = value;
                    NotifyPropertyChanged("EndTime");
                }
            }

            private string writeTime = "";
            public string WriteTime
            {
                get => writeTime;
                set
                {
                    writeTime = value;
                    NotifyPropertyChanged("WriteTime");
                }
            }

            private int stopDays = 0;
            public int StopDays
            {
                get => stopDays;
                set
                {
                    stopDays = value;
                    NotifyPropertyChanged("StopDays");
                }
            }

            private Visibility stopButtonVisibility = Visibility.Visible;
            public Visibility StopButtonVisibility
            {
                get => stopButtonVisibility;
                set
                {
                    stopButtonVisibility = value;
                    NotifyPropertyChanged("StopButtonVisibility");
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

                var staffs = context.Staff.Where(c => !c.IsDel);

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
                    var staffs = context.Staff.Where(c => !c.IsDel);

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
                        EndTime = "",
                        StartTime = "未签订",
                        StopButtonVisibility = Visibility.Collapsed,
                        StopDays = 0,
                        WriteTime = ""
                    };

                    _model.JobpostName = context.SysDic.First(c => c.Id == item.JobPostId).Name;

                    if (context.StaffContract.Any(c => c.StaffId == item.Id))
                    {
                        var _contract = context.StaffContract.First(c => c.StaffId == item.Id);
                        if (_contract.Stop)
                        {
                            _model.Name += "(已终止)";
                            _model.StartTime = _contract.Start.ToString("yyyy年MM月dd日");
                            _model.EndTime = _contract.End.ToString("yyyy年MM月dd日");
                            _model.WriteTime = _contract.Write.ToString("yyyy年MM月dd日");
                            _model.StopDays = 0;
                            _model.StopButtonVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            _model.StartTime = _contract.Start.ToString("yyyy年MM月dd日");
                            _model.EndTime = _contract.End.ToString("yyyy年MM月dd日");
                            _model.WriteTime = _contract.Write.ToString("yyyy年MM月dd日");
                            _model.StopDays = (int)(_contract.End - DateTime.Now).TotalDays;
                            if (_model.StopDays < 0) _model.StopDays = 0;
                            _model.StopButtonVisibility = Visibility.Visible;
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
            EditContract i = new EditContract(selectModel.Id);
            i.ShowDialog();
            MaskVisible(false);
            if (i.Succeed)
            {
                using (DBContext context = new DBContext())
                {
                    var _model = Data.Single(c => c.Id == selectModel.Id);
                    var _contract = context.StaffContract.First(c => c.StaffId == selectModel.Id);
                    _model.StartTime = _contract.Start.ToString("yyyy年MM月dd日");
                    _model.EndTime = _contract.End.ToString("yyyy年MM月dd日");
                    _model.WriteTime = _contract.Write.ToString("yyyy年MM月dd日");
                    _model.StopDays = (int)(_contract.End - DateTime.Now).TotalDays;
                    if (_model.StopDays < 0) _model.StopDays = 0;
                    _model.StopButtonVisibility = Visibility.Visible;
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string staffId = (sender as Button).Tag.ToString();
            using (DBContext context = new DBContext())
            {
                var _contract = context.StaffContract.First(c => c.StaffId == staffId && !c.Stop);
                _contract.Stop = true;
                _contract.StopUser = UserGlobal.CurrUser.Id;
                _contract.StopTime = DateTime.Now;

                context.SaveChanges();

                UIModel model = Data.Single(c => c.Id == _contract.StaffId);
                model.Name += "(已终止)";
                model.StartTime = _contract.Start.ToString("yyyy年MM月dd日");
                model.EndTime = _contract.End.ToString("yyyy年MM月dd日");
                model.WriteTime = _contract.Write.ToString("yyyy年MM月dd日");
                model.StopDays = 0;
                model.StopButtonVisibility = Visibility.Collapsed;
            }
            MessageBoxX.Show("成功", "成功");
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
