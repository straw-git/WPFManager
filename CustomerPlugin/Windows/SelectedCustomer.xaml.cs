using Common;
using Common.Utils;
using CustomerDBModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace CustomerPlugin.Windows
{
    /// <summary>
    /// SelectedStaff.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedCustomer : Window
    {
        public bool Succeed = false;
        public CustomerDBModels.Customer Model = new CustomerDBModels.Customer();

        string mainPromotionCode = "";

        /// <summary>
        /// 选择客户
        /// </summary>
        /// <param name="_promotionCode">（有选择编码时，列表中他本人和他推荐的人将不会显示 以确保不会循环推荐）</param>
        public SelectedCustomer(string _promotionCode = "")
        {
            InitializeComponent();
            this.UseCloseAnimation();
            mainPromotionCode = _promotionCode;
        }

        #region Models

        public class UIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string IsMember { get; set; }
            public string MemberLevel { get; set; }
            public string PromotionCode { get; set; }
            public string CreateTime { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
        }

        #region Private Method

        private UIModel DBItem2UIModel(CustomerDBModels.Customer item)
        {
            UIModel _model = new UIModel();
            _model.CreateTime = item.CreateTime.ToString("yy年MM月dd日");
            _model.Id = item.Id;
            _model.Name = item.Name;
            _model.PromotionCode = item.PromotionCode;
            _model.IsMember = item.IsMember ? "是" : "否";

            return _model;
        }

        /// <summary>
        /// 查找表格的条件
        /// </summary>
        /// <param name="_customer"></param>
        /// <param name="_searchText"></param>
        /// <returns></returns>
        protected bool GetPagerWhere(CustomerDBModels.Customer _customer, string _searchText)
        {
            bool resultCondition = true;
            if (_searchText.NotEmpty())
            {
                //根据名称检索
                resultCondition &= _customer.Name.Contains(_searchText) || _customer.QuickCode.Contains(_searchText);
            }

            resultCondition &= !_customer.IsBlack;


            return resultCondition;
        }

        #endregion 

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                list.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                list.IsEnabled = true;

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

        #region UI Method

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //双击列表后模拟点击操作
            btnSubmit_Click(null, null);
        }

        private async void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string searchText = txtSearchText.Text.Trim();

                Data.Clear();//先清空再加入页面数据
                if (searchText.IsNullOrEmpty()) return;

                using (CustomerDBContext context = new CustomerDBContext())
                {
                    Expression<Func<CustomerDBModels.Customer, bool>> _where = n => GetPagerWhere(n, searchText);//按条件查询
                    Expression<Func<CustomerDBModels.Customer, DateTime>> _orderByDesc = n => n.CreateTime;//按时间倒序
                    //开始分页查询数据
                    var _zPager = await PagerCommon.BeginEFDataPagerAsync(context.Customer, _where, _orderByDesc, null, null, null, null, 0);
                    if (!_zPager.Result) return;
                    List<CustomerDBModels.Customer> _list = _zPager.EFDataList;

                    #region 页面数据填充

                    var mainBePromotionCodes = new List<int>();
                    if (mainPromotionCode.NotEmpty())
                    {
                        //他推荐的人
                        mainBePromotionCodes = (from c in context.Customer.Where(c => c.BePromotionCode == mainPromotionCode && !c.IsBlack) select c.Id).ToList(); 
                    }

                    foreach (var item in _list)
                    {
                        if (mainPromotionCode.NotEmpty())
                        {
                            if (item.PromotionCode == mainPromotionCode) continue;//他本人
                            if (mainBePromotionCodes != null && mainBePromotionCodes.IndexOf(item.Id) > -1) continue;//他推荐的人
                        }


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

                        Data.Add(_model);
                    }

                    #endregion
                }

                //结尾处必须结束分页查询
                PagerCommon.EndEFDataPager();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                UIModel selectedModel = list.SelectedItem as UIModel;

                if (selectedModel != null)
                {
                    using (CustomerDBContext context = new CustomerDBContext())
                    {
                        Succeed = true;
                        Model = context.Customer.First(c => c.Id == selectedModel.Id);
                    }
                }
            }

            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

    }
}
