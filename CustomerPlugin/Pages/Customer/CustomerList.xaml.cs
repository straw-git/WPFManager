﻿using Common;
using Common.Data.Local;
using Common.Utils;
using Common.Windows;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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

namespace CustomerPlugin.Pages.Customer
{
    /// <summary>
    /// CustomerList.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerList : BasePage
    {
        public CustomerList()
        {
            InitializeComponent();
            Order = 0;

            //测试
            OnPageLoaded();
        }

        #region Models

        class UIModel : BaseUIModel
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

            private string sex = "";
            public string Sex
            {
                get => sex;
                set
                {
                    sex = value;
                    NotifyPropertyChanged("Sex");
                }
            }

            private string phone = "";
            public string Phone
            {
                get => phone;
                set
                {
                    phone = value;
                    NotifyPropertyChanged("Phone");
                }
            }

            private string promotionCode = "";
            public string PromotionCode //推荐码
            {
                get => promotionCode;
                set
                {
                    promotionCode = value;
                    NotifyPropertyChanged("PromotionCode");
                }
            }
            private string promotioner = "";
            public string Promotioner //推荐人
            {
                get => promotioner;
                set
                {
                    promotioner = value;
                    NotifyPropertyChanged("PromotionCode");
                }
            }

            private string blackLinkContent = "加入黑名单";

            public string BlackLinkContent
            {
                get => blackLinkContent;
                set
                {
                    blackLinkContent = value;
                    NotifyPropertyChanged("BlackLinkContent");
                }
            }

            private Brush blackLinkForeground = new SolidColorBrush(Colors.Blue);

            public Brush BlackLinkForeground
            {
                get => blackLinkForeground;
                set
                {
                    blackLinkForeground = value;
                    NotifyPropertyChanged("BlackLinkForeground");
                }
            }

            private Brush nameForeground = new SolidColorBrush(Colors.Black);
            public Brush NameForeground//名字颜色
            {
                get => nameForeground;
                set
                {
                    nameForeground = value;
                    NotifyPropertyChanged("NameForeground");
                }
            }

            private string memberLinkContent = "";
            public string MemberLinkContent//办理会员按钮显示内容
            {
                get => memberLinkContent;
                set
                {
                    memberLinkContent = value;
                    NotifyPropertyChanged("MemberLinkContent");
                }
            }

            private Visibility memberButtonVisibility = Visibility.Visible;
            public Visibility MemberButtonVisibility//办理会员按钮显示状态
            {
                get => memberButtonVisibility;
                set
                {
                    memberButtonVisibility = value;
                    NotifyPropertyChanged("MemberButtonVisibility");
                }
            }

            private Visibility editButtonVisibility = Visibility.Visible;
            public Visibility EditButtonVisibility//编辑显示状态
            {
                get => editButtonVisibility;
                set
                {
                    editButtonVisibility = value;
                    NotifyPropertyChanged("EditButtonVisibility");
                }
            }

            public string MemberLevel { get; set; }
            public string CreateTime { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            SetColumn(list.Name, "Id", "编号");
            SetColumn(list.Name, "Name", "名称");
            SetColumn(list.Name, "Sex", "性别");
            SetColumn(list.Name, "Phone", "手机号");
            SetColumn(list.Name, "PromotionCode", "推荐码");
            SetColumn(list.Name, "Promotioner", "推荐人");
            SetColumn(list.Name, "MemberLevel", "等级");
            SetColumn(list.Name, "CreateTime", "创建时间");

            //绑定数据
            list.ItemsSource = Data;
            UpdateChart();
        }

        private async void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            if (pagerRunning) return;//如果正在运行 则退出 防止重复点击
            pagerRunning = true;

            if (e == null) gPager.CurrentIndex = 1;//如果是通过查询或者刷新点击的 直接显示第一页

            //查询条件
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            bool isMember = (bool)cbMember.IsChecked;
            bool isBlack = (bool)cbBlack.IsChecked;
            bool enableTime = (bool)cbEnableTime.IsChecked;
            DateTime startTime = dtStart.SelectedDateTime.MinDate();
            DateTime endTime = dtEnd.SelectedDateTime.MaxDate();

            Data.Clear();//先清空再加入页面数据

            using (DBContext context = new DBContext())
            {
                Expression<Func<DBModels.Member.Customer, bool>> _where = n => GetPagerWhere(n, name, phone, isMember, isBlack, enableTime, startTime, endTime);//按条件查询
                Expression<Func<DBModels.Member.Customer, DateTime>> _orderByDesc = n => n.CreateTime;//按时间倒序
                var _list = await GetDataPagerAsync(context.Customer, _where, _orderByDesc, gLoading, gPager, bNoData, new Control[1] { list });//获取数据

                foreach (var item in _list)
                {
                    var _model = DBItem2UIModel(item);

                    if (item.IsMember)
                    {
                        //会员
                        decimal rechargeSum = 0;
                        if (context.MemberRecharge.Any(c => c.CustomerId == item.Id))
                            rechargeSum = context.MemberRecharge.Where(c => c.CustomerId == item.Id).Sum(c => c.Price);//历史充值总金额
                        if (context.MemberLevel.OrderByDescending(c => c.LogPriceCount).Any(c => c.LogPriceCount <= rechargeSum))
                        {
                            _model.MemberLevel = context.MemberLevel.OrderByDescending(c => c.LogPriceCount).First(c => c.LogPriceCount <= rechargeSum).Name;
                        }
                        else
                        {
                            _model.MemberLevel = "无会员等级";
                        }
                    }
                    else
                    {
                        _model.MemberLevel = "非会员";
                    }

                    _model.Promotioner = "";
                    if (item.BePromotionCode.NotEmpty())
                    {
                        if (context.Customer.Any(c => c.PromotionCode == item.BePromotionCode))
                            _model.Promotioner = context.Customer.First(c => c.PromotionCode == item.BePromotionCode).Name;
                    }

                    Data.Add(_model);
                }
            }

            pagerRunning = false;
        }

        private UIModel DBItem2UIModel(DBModels.Member.Customer item)
        {
            UIModel _model = new UIModel();
            _model.CreateTime = item.CreateTime.ToString("yy年MM月dd日");
            _model.Id = item.Id;
            _model.IsChecked = TableDataAny(list.Name, c => c.Id == item.Id);
            _model.Name = item.Name;
            _model.Phone = item.Phone;
            _model.Sex = item.Sex;
            _model.PromotionCode = item.PromotionCode;

            if (item.IsBlack)
            {
                //黑名单
                _model.BlackLinkContent = "移出黑名单";
                _model.BlackLinkForeground = new SolidColorBrush(Colors.Red);

                _model.MemberButtonVisibility = Visibility.Collapsed;
                _model.EditButtonVisibility = Visibility.Collapsed;

            }
            else
            {
                _model.BlackLinkContent = "加入黑名单";
                _model.BlackLinkForeground = new SolidColorBrush(Colors.Blue);

                _model.MemberButtonVisibility = Visibility.Visible;
                _model.EditButtonVisibility = Visibility.Visible;

                if (item.IsMember)
                {
                    _model.NameForeground = new SolidColorBrush(Colors.Orange);
                    _model.MemberLinkContent = "会员信息";
                }
                else
                {
                    _model.NameForeground = new SolidColorBrush(Colors.Black);
                    _model.MemberLinkContent = "加入会员";
                }

            }

            return _model;
        }

        /// <summary>
        /// 查找表格的条件
        /// </summary>
        /// <param name="_customer"></param>
        /// <param name="_name"></param>
        /// <param name="_phone"></param>
        /// <param name="_isMember"></param>
        /// <param name="isBlcak"></param>
        /// <param name="_enableTime"></param>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <returns></returns>
        protected bool GetPagerWhere(DBModels.Member.Customer _customer, string _name, string _phone, bool _isMember, bool _isBlcak, bool _enableTime, DateTime _start, DateTime _end)
        {
            bool resultCondition = true;
            if (_name.NotEmpty())
            {
                //根据名称检索
                resultCondition &= _customer.Name.Contains(_name) || _customer.QuickCode.Contains(_name);
            }
            if (_phone.NotEmpty())
            {
                resultCondition &= _customer.Phone.Contains(_phone);
            }
            if (_enableTime)
            {
                resultCondition &= _customer.CreateTime >= _start && _customer.CreateTime <= _end;
            }
            if (_isMember)
            {
                resultCondition &= _customer.IsMember;
            }
            if (_isBlcak)
            {
                resultCondition &= _customer.IsBlack;
            }

            return resultCondition;
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void cbEnableTime_Checked(object sender, RoutedEventArgs e)
        {
            dtStart.IsEnabled = true;
            dtEnd.IsEnabled = true;
        }

        private void cbEnableTime_Unchecked(object sender, RoutedEventArgs e)
        {
            dtStart.IsEnabled = false;
            dtEnd.IsEnabled = false;
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectItem = list.SelectedItem as UIModel;
                bool targetIsChecked = !selectItem.IsChecked;
                Data.Single(c => c.Id == selectItem.Id).IsChecked = targetIsChecked;

                if (targetIsChecked)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    AddTableData(list.Name, selectItem);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    RemoveTableData(list.Name, c => c.Id == selectItem.Id);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            int id = 0;
            var tag = (sender as Button).Tag.ToString();
            if (!int.TryParse(tag, out id))
            {
                MessageBoxX.Show("请正确选择编辑项", "数据提取失败");
                return;
            }

            MaskVisible(true);

            EditCustomer editCustomer = new EditCustomer(id);
            editCustomer.ShowDialog();

            if (editCustomer.Succeed)
            {
                UIModel newModel = DBItem2UIModel(editCustomer.Result);
                newModel.Promotioner = "";
                if (editCustomer.Result.BePromotionCode.NotEmpty())
                {
                    using (DBContext context = new DBContext())
                        if (context.Customer.Any(c => c.PromotionCode == editCustomer.Result.BePromotionCode))
                            newModel.Promotioner = context.Customer.First(c => c.PromotionCode == editCustomer.Result.BePromotionCode).Name;
                }

                var _model = Data.Single(c => c.Id == id);
                ModelCommon.CopyPropertyToModel(newModel, ref _model);
            }

            MaskVisible(false);
        }

        private void btnEditBlack_Click(object sender, RoutedEventArgs e)
        {
            Button target = sender as Button;
            int id = target.Tag.ToString().AsInt();

            var _model = Data.Single(c => c.Id == id);

            var result = MessageBoxX.Show($"是否将 [{_model.Name}] {target.Content}？ ", $"{target.Content}提醒", null, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            if (target.Content.ToString() == "加入黑名单")
            {
                using (DBContext context = new DBContext())
                {
                    var _customer = context.Customer.Single(c => c.Id == id);
                    _customer.IsBlack = true;

                    _model.BlackLinkContent = "移出黑名单";
                    _model.BlackLinkForeground = new SolidColorBrush(Colors.Red);

                    _model.MemberButtonVisibility = Visibility.Collapsed;
                    _model.EditButtonVisibility = Visibility.Collapsed;

                    context.SaveChanges();
                }
            }
            else
            {
                using (DBContext context = new DBContext())
                {
                    var _customer = context.Customer.Single(c => c.Id == id);
                    _customer.IsBlack = false;

                    _model.BlackLinkContent = "加入黑名单";
                    _model.BlackLinkForeground = new SolidColorBrush(Colors.Blue);

                    _model.MemberButtonVisibility = Visibility.Visible;
                    _model.EditButtonVisibility = Visibility.Visible;

                    context.SaveChanges();
                }
            }
        }

        private void btn2Member_Click(object sender, RoutedEventArgs e)
        {
            Button target = sender as Button;
            int id = target.Tag.ToString().AsInt();

            var _model = Data.Single(c => c.Id == id);

            if (target.Content.ToString() == "加入会员")
            {
                var result = MessageBoxX.Show($"是否将 [{_model.Name}] 办理为会员？ ", "会员办理提醒", null, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (DBContext context = new DBContext())
                    {
                        var _customer = context.Customer.Single(c => c.Id == id);
                        _customer.IsMember = true;

                        _model.NameForeground = new SolidColorBrush(Colors.Orange);
                        _model.MemberLinkContent = "会员信息";

                        //添加会员信息
                        DBModels.Member.Member _member = new DBModels.Member.Member();
                        _member.Birthday = _customer.IdCard.NotEmpty() ? IdCardCommon.GetBirthday(_customer.IdCard).ToString("yyyy-MM-dd") : "";
                        _member.CardNumber = "";
                        _member.CreateTime = DateTime.Now;
                        _member.Creator = CurrUser.Id;
                        _member.CustomerId = id;
                        _member.IdCard = _customer.IdCard;
                        _member.Name = _customer.Name;
                        _member.Phone = _customer.Phone;
                        _member.Price = 0;
                        _member.QuickCode = _customer.QuickCode;
                        _member.RechargeCount = 0;
                        _member.Sex = _customer.Sex;
                        _member.IsDelete = false;

                        context.Member.Add(_member);
                        context.SaveChanges();

                        Notice.Show($"{_model.Name}已成为会员", "会员办理成功", MessageBoxIcon.Success);
                        //成功更新图表
                        UpdateChart();
                    }
                }
            }
            else
            {
                MaskVisible(true);

                EditMember editMember = new EditMember(id);
                editMember.ShowDialog();

                UpdatePager(null, null);
                UpdateChart();

                MaskVisible(false);
            }
        }

        /// <summary>
        /// 更新会员办理周报表格
        /// </summary>
        private void UpdateChart()
        {
            #region 新增客户图表

            string skinColor = LocalSettings.settings == null ? "#FFFFFF00" : LocalSkin.GetModelById(LocalSettings.settings.SkinId).SkinColor;
            SeriesCollection series = new SeriesCollection
                    {
                        new LineSeries
                        {
                            Title="7日新增",
                            Stroke =ColorHelper.ConvertToSolidColorBrush(skinColor),
                            Values = new ChartValues<ObservableValue>{ },
                            DataLabels = true,
                            LabelPoint = point => point.Y.ToString()
                        },
                        new LineSeries
                        {
                            Title="同期对比",
                            Stroke =ColorHelper.ConvertToSolidColorBrush("#FFFF0000"),
                            Values = new ChartValues<ObservableValue>{ },
                            DataLabels = true,
                            LabelPoint = point =>  point.Y.ToString()
                        }
                    };

            List<string> lableData = new List<string>();

            using (DBContext context = new DBContext())
            {
                for (int i = 7; i >= 0; i--)
                {
                    DateTime day = DateTime.Now.AddDays(i * -1);
                    DateTime topDay = DateTime.Now.AddDays((i + 7) * -1);

                    lableData.Add($"{day.Day}号");

                    DateTime nowDayTime = DateTime.Now;
                    DateTime topDayTime = DateTime.Now;

                    DateTime min = day.MinDate();
                    DateTime max = day.MaxDate();
                    //本周
                    series[0].Values.Add(new ObservableValue(context.Member.Count(c => c.CreateTime >= min && c.CreateTime <= max)));


                    DateTime minTop = topDay.MinDate();
                    DateTime maxTop = topDay.MaxDate();
                    //上周
                    series[1].Values.Add(new ObservableValue(context.Member.Count(c => c.CreateTime >= minTop && c.CreateTime <= maxTop)));

                }
            }

            xChart.AxisX[0].Labels = lableData.ToArray();
            xChart.Series = series;

            #endregion

            #region 会员比例图表

            pcState.Series.Clear();
            using (DBContext context = new DBContext())
            {
                int count = context.Customer.Count(c => !c.IsMember);
                int memberCount = context.Customer.Count(c => c.IsMember);

                pcState.Series.Add(new PieSeries()
                {
                    Title = "会员",
                    Values = new LiveCharts.ChartValues<int> { memberCount },
                    DataLabels = true,
                    Foreground = new SolidColorBrush(Colors.Black),
                });
                pcState.Series.Add(new PieSeries()
                {
                    Title = "非会员",
                    Values = new LiveCharts.ChartValues<int> { count },
                    DataLabels = true,
                    Foreground = new SolidColorBrush(Colors.Black),
                });
            }

            #endregion 
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            EditCustomer editCustomer = new EditCustomer();
            editCustomer.ShowDialog();

            if (editCustomer.Succeed)
            {
                UpdatePager(null, null);
            }

            MaskVisible(false);
        }

        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)((sender as CheckBox).IsChecked);
            foreach (var item in Data)
            {
                item.IsChecked = isCheck;
                if (isCheck)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    AddTableData(list.Name, item);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    RemoveTableData(list.Name, c => c.Id == item.Id);
                }
            }
        }

        #region 导出Excel

        private void btnExportCurrPage_Click(object sender, RoutedEventArgs e)
        {
            //导出本页数据
            ExportExcelAsync(Data.ToList(), list.Name,$"页码{gPager.CurrentIndex}");
        }

        private async void btnExportAllPage_Click(object sender, RoutedEventArgs e)
        {
            //导出所有数据
            List<DBModels.Member.Customer> allData = new List<DBModels.Member.Customer>();

            await Task.Delay(50);

            await Task.Run(() =>
            {
                using (DBContext context = new DBContext()) allData = context.Customer.ToList();
            });

            ExportExcelAsync(allData, list.Name,"所有数据");
        }

        private void btnExportFocusDatas_Click(object sender, RoutedEventArgs e)
        {
            //导出选中数据
            var listData = GetTableData<UIModel>(list.Name);
            if (list==null||listData.Count == 0) 
            {
                MessageBoxX.Show("没有选中数据","空值提醒");
                return;
            }
            ExportExcelAsync(listData, list.Name,"选中数据");
        }

        private void btnExportSetting_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            BasePageExportSetting basePageExportSetting = new BasePageExportSetting(GetColumns(list.Name));
            basePageExportSetting.ShowDialog();

            SetColumn(list.Name, basePageExportSetting.GetResult());

            MaskVisible(false);
        }

        #endregion

    }
}
