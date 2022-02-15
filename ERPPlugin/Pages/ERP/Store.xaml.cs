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
using Common.Windows;
using ERPPlugin.Windows;
using ERPDBModels.Models;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// Store.xaml 的交互逻辑
    /// </summary>
    public partial class Store : BasePage
    {
        public Store()
        {
            InitializeComponent();
            this.Order = 3;

            //测试
            //OnPageLoaded();
        }

        #region Models

        class StoreUIModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SubName
            {
                get { return Name.Length > 10 ? $"{Name.Substring(0, 10)}..." : Name; }
            }
            public string StoreTypeName { get; set; }
            public int Count { get; set; }//操作数量
            public int Surplus { get; set; }//余量
            public string CreateTime { get; set; }
        }

        class GoodsItemUIModel : INotifyPropertyChanged
        {
            public string ItemId { get; set; }
            public string TargetId { get; set; }//物品或项目的Id
            public string Name { get; set; }
            public string SubName
            {
                get
                {
                    return Name.Length >= 20 ? $"{Name.Substring(0, 20)}..." : Name;
                }
            }
            public string TypeName { get; set; }
            public int StoreId { get; set; }
            private string storeName = "";
            public string StoreName
            {
                get => storeName;
                set
                {
                    storeName = value;
                    NotifyPropertyChanged("StoreName");
                }
            }
            public int Count { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        //页面数据集合
        ObservableCollection<StoreUIModel> Data = new ObservableCollection<StoreUIModel>();
        ObservableCollection<GoodsItemUIModel> planItemUIModels = new ObservableCollection<GoodsItemUIModel>();
        ObservableCollection<GoodsItemUIModel> singleItemUIModels = new ObservableCollection<GoodsItemUIModel>();
        ObservableCollection<GoodsItemUIModel> projectItemUIModels = new ObservableCollection<GoodsItemUIModel>();
        ObservableCollection<GoodsItemUIModel> outsItemUIModels = new ObservableCollection<GoodsItemUIModel>();

        /// <summary>
        /// 单品入库临时选择物品的数据
        /// </summary>
        GoodsDetails.UIModel singleTempDetailModel = new GoodsDetails.UIModel();
        /// <summary>
        /// 出库临时详细数据
        /// </summary>
        StoreOutDetail.UIModel outTempDetailModel = new StoreOutDetail.UIModel();
        /// <summary>
        /// 单品入库列表附属数据
        /// <para>Key：itemid   Value：详细信息</para>
        /// </summary>
        Dictionary<string, GoodsDetails.UIModel> singleListDataDic = new Dictionary<string, GoodsDetails.UIModel>();

        protected override void OnPageLoaded()
        {
            storeLogList.ItemsSource = Data;
            planGoodsList.ItemsSource = planItemUIModels;
            singleGoodsList.ItemsSource = singleItemUIModels;
            storeOutList.ItemsSource = outsItemUIModels;

            UpdateStoreLogGridAsync();
            btnUpdatePlan_Click(null, null);
        }

        #region Private Method

        private async void UpdateStoreLogGridAsync()
        {
            ShowLoadingPanel();
            Data.Clear();

            List<StockLog> stockLogs = new List<StockLog>();
            await Task.Run(() =>
            {
                using (ERPDBContext context = new ERPDBContext())
                {
                    stockLogs = context.StockLog.OrderByDescending(c => c.CreateTime).Take(20).ToList();
                }
            });

            await Task.Delay(300);

            bNoData.Visibility = stockLogs.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            using (ERPDBContext context = new ERPDBContext())
            {
                for (int i = 0; i < stockLogs.Count; i++)
                {
                    var item = stockLogs[i];

                    var _model = new StoreUIModel()
                    {
                        Count = item.Count,
                        CreateTime = item.CreateTime.ToString("yy年MM月dd日"),
                        Id = item.Id,
                        Name = "",
                        StoreTypeName = item.StockType == Global.StockTypeIn ? "入库" : "出库",
                        Surplus = item.Surplus
                    };

                    if (context.Goods.Any(c => c.Id == item.GoodsId))
                    {
                        _model.Name = context.Goods.First(c => c.Id == item.GoodsId).Name;
                    }

                    Data.Add(_model);
                }
            }

            HideLoadingPanel();
        }

        #endregion


        #region UI Method

        #region 采购单入库

        private void btnPlanStore_Click(object sender, RoutedEventArgs e)
        {
            //采购单入库
            if (planItemUIModels.Count == 0)
            {
                MessageBoxX.Show("没有任何入库数据", "空数据提醒");
                return;
            }

            var result = MessageBoxX.Show("请确认核对信息后入库,当前采购单将无法再次入库", "入库警告", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (ERPDBContext context = new ERPDBContext())
                {
                    DateTime createTime = DateTime.Now;
                    for (int i = 0; i < planItemUIModels.Count; i++)
                    {
                        var _uiModel = planItemUIModels[i];

                        if (_uiModel.StoreId == 0)
                        {
                            //不存在仓库直接退出
                            continue;
                        }

                        #region 添加库存记录

                        if (context.Stock.Any(c => c.GoodsId == _uiModel.TargetId && c.StoreId == _uiModel.StoreId))
                        {
                            //如果仓库中存在该物品 直接更改物品数量

                            //更改库存总表数量
                            var _stock = context.Stock.Single(c => c.GoodsId == _uiModel.TargetId && c.StoreId == _uiModel.StoreId);

                            //入库时的详细信息
                            var _planLog = context.PurchasePlanLog.Where(c => c.ItemId == _uiModel.ItemId).ToList();
                            var _goods = context.Goods.First(c => c.Id == _uiModel.TargetId);
                            int _stockAdd = 0;
                            for (int j = 0; j < _planLog.Count; j++)
                            {
                                var _pLogModel = _planLog[j];
                                var _stockModel = new StockLog();
                                _stockAdd += _pLogModel.Count;
                                _stockModel.Count = _pLogModel.Count;
                                _stockModel.CreateTime = createTime;
                                _stockModel.Creator = Common.UserGlobal.CurrUser.Id;
                                _stockModel.Manufacturer = _pLogModel.Manufacturer;
                                _stockModel.Price = _pLogModel.PurchasePrice;
                                _stockModel.Remark = _pLogModel.Remark;
                                _stockModel.SalePrice = _goods.SalePrice;
                                _stockModel.StockType = Global.StockTypeIn;
                                _stockModel.StoreId = _uiModel.StoreId;
                                _stockModel.SupplierId = _pLogModel.SupplierId;
                                _stockModel.Surplus = _stock.Count + _stockAdd;
                                _stockModel.GoodsId = _uiModel.TargetId;

                                context.StockLog.Add(_stockModel);
                            }

                            _stock.Count += _uiModel.Count;
                            
                        }
                        else
                        {
                            //如果物品不存在 添加物品 
                            context.Stock.Add(new Stock()
                            {
                                Count = _uiModel.Count,
                                StoreId = _uiModel.StoreId,
                                GoodsId = _uiModel.TargetId
                            });

                            //入库时的详细信息
                            var _planLog = context.PurchasePlanLog.Where(c => c.ItemId == _uiModel.ItemId).ToList();
                            var _goods = context.Goods.First(c => c.Id == _uiModel.TargetId);
                            int _stockAdd = 0;
                            for (int j = 0; j < _planLog.Count; j++)
                            {
                                var _pLogModel = _planLog[j];
                                var _stockModel = new StockLog();
                                _stockAdd += _pLogModel.Count;
                                _stockModel.Count = _pLogModel.Count;
                                _stockModel.CreateTime = createTime;
                                _stockModel.Creator = UserGlobal.CurrUser.Id;
                                _stockModel.Manufacturer = _pLogModel.Manufacturer;
                                _stockModel.Price = _pLogModel.PurchasePrice;
                                _stockModel.Remark = _pLogModel.Remark;
                                _stockModel.SalePrice = _goods.SalePrice;
                                _stockModel.StockType = Global.StockTypeIn;
                                _stockModel.StoreId = _uiModel.StoreId;
                                _stockModel.SupplierId = _pLogModel.SupplierId;
                                _stockModel.Surplus = _stockAdd;
                                _stockModel.GoodsId = _uiModel.TargetId;
                                context.StockLog.Add(_stockModel);
                            }
                        }

                        #endregion
                    }

                    //更改当前订单状态为入库
                    context.PurchasePlan.Single(c => c.PlanCode == cbPlanCode.SelectedItem.ToString()).Stock = true;
                    context.SaveChanges();
                }

                MessageBoxX.Show("入库成功", "成功");
                planItemUIModels.Clear();
                btnUpdatePlan_Click(null, null);

                btnRef_Click(null, null);
            }
        }

        private void btnDeletePlanItem_Click(object sender, RoutedEventArgs e)
        {
            //删除采购单条目
            Button target = sender as Button;
            string itemId = target.Tag.ToString();
            planItemUIModels.Remove(planItemUIModels.First(c => c.ItemId == itemId));
        }

        private void btnChangeStore_Click(object sender, RoutedEventArgs e)
        {
            //更改仓库
            string itemId = (sender as Button).Tag.ToString();
            MaskVisible(true);
            SelectedStore s = new SelectedStore();
            s.ShowDialog();
            MaskVisible(false);
            if (s.Succeed)
            {
                var _store = planItemUIModels.Single(c => c.ItemId == itemId);
                _store.StoreId = s.Model.Id;
                _store.StoreName = s.Model.Name;
            }
        }

        #endregion

        #region 单品入库

        private void btnSelectSingleGoods_Click(object sender, RoutedEventArgs e)
        {
            //选择物品
            string goodsName = "";
            string goodsId = "";

            MaskVisible(true);
            SelectedGoods s = new SelectedGoods(1);
            s.ShowDialog();
            MaskVisible(false);

            if (s.Ids.Count == 1)
            {
                goodsId = s.Ids[0];
                using (ERPDBContext context = new ERPDBContext())
                {
                    var _goods = context.Goods.First(c => c.Id == goodsId);
                    goodsName = _goods.Name;
                }
            }
            else return;

            MaskVisible(true);
            GoodsDetails gd = new GoodsDetails(goodsId, goodsName);
            gd.ShowDialog();
            MaskVisible(false);

            if (gd.Succeed)
            {
                singleTempDetailModel = gd.Model;

                //选择仓库
                MaskVisible(true);
                SelectedStore selectedStore = new SelectedStore();
                selectedStore.ShowDialog();
                MaskVisible(false);

                if (selectedStore.Succeed)
                {
                    string storeName = selectedStore.Model.Name;
                    int storeId = selectedStore.Model.Id;

                    if (storeId == 0) 
                    {
                        //没选择仓库
                        btnSelectSingleGoods.Content = "未选择物品";
                        btnSelectSingleGoods.Tag = "";
                        return;
                    }
                    //选择了仓库

                    #region Empty or Error

                    if (string.IsNullOrEmpty(goodsId))
                    {
                        MessageBoxX.Show("请选择物品", "空值提醒");
                        return;
                    }
                    if (storeId <= 0)
                    {
                        MessageBoxX.Show("请选择仓库", "空值提醒");
                        return;
                    }

                    #endregion

                    string itemId = Guid.NewGuid().ToString();

                    singleItemUIModels.Add(new GoodsItemUIModel()
                    {
                        Count = singleTempDetailModel.Count,
                        TargetId = goodsId,
                        ItemId = itemId,
                        Name = btnSelectSingleGoods.Content.ToString(),
                        StoreId = storeId,
                        StoreName = storeName,
                        TypeName = "物品"
                    });

                    singleListDataDic.Add(itemId, singleTempDetailModel);

                    btnSelectSingleGoods.Content = "未选择物品";
                    btnSelectSingleGoods.Tag = "";
                }
                else
                {
                    //没选择仓库
                    btnSelectSingleGoods.Content = "未选择物品";
                    btnSelectSingleGoods.Tag = "";
                    return;
                }
            }
            else
            {
                btnSelectSingleGoods.Content = "未选择物品";
                btnSelectSingleGoods.Tag = "";
                return;
            }
        }

        private void btnSingleStore_Click(object sender, RoutedEventArgs e)
        {
            //单品入库
            if (singleItemUIModels.Count == 0)
            {
                MessageBoxX.Show("没有任何入库数据", "空数据提醒");
                return;
            }

            for (int i = 0; i < singleItemUIModels.Count; i++)
            {
                var _item = singleItemUIModels[i];

                #region 添加库存记录

                using (ERPDBContext context = new ERPDBContext())
                {
                    if (context.Stock.Any(c => c.StoreId == _item.StoreId && c.GoodsId == _item.TargetId))
                    {
                        //存在直接修改数量
                        var _socket = context.Stock.First(c => c.StoreId == _item.StoreId && c.GoodsId == _item.TargetId);
                        _socket.Count += _item.Count;

                        var _dataModel = singleListDataDic[_item.ItemId];
                        context.StockLog.Add(new StockLog()
                        {
                            Count = _item.Count,
                            CreateTime = DateTime.Now,
                            Creator = UserGlobal.CurrUser.Id,
                            Manufacturer = _dataModel.Manufacturer,
                            Price = _dataModel.Price,
                            Remark = _dataModel.Remark,
                            SalePrice = context.Goods.First(c => c.Id == _item.TargetId).SalePrice,
                            StockType = Global.StockTypeIn,
                            StoreId = _item.StoreId,
                            SupplierId = _dataModel.SupplierId,
                            Surplus = _socket.Count,
                            GoodsId = _item.TargetId
                        });
                    }
                    else
                    {
                        //不存在直接添加
                        context.Stock.Add(new Stock()
                        {
                            Count = _item.Count,
                            StoreId = _item.StoreId,
                            GoodsId = _item.TargetId
                        });

                        var _dataModel = singleListDataDic[_item.ItemId];
                        context.StockLog.Add(new StockLog()
                        {
                            Count = _item.Count,
                            CreateTime = DateTime.Now,
                            Creator = UserGlobal.CurrUser.Id,
                            Manufacturer = _dataModel.Manufacturer,
                            Price = _dataModel.Price,
                            Remark = _dataModel.Remark,
                            SalePrice = context.Goods.First(c => c.Id == _item.TargetId).SalePrice,
                            StockType = Global.StockTypeIn,
                            StoreId = _item.StoreId,
                            SupplierId = _dataModel.SupplierId,
                            Surplus = _item.Count,
                            GoodsId = _item.TargetId
                        });
                    }

                    context.SaveChanges();
                }

                #endregion 
            }

            MessageBoxX.Show("入库成功", "成功");

            singleItemUIModels.Clear();
            singleListDataDic.Clear();

            btnRef_Click(null, null);
        }

        private void btnDeleteSingleItem_Click(object sender, RoutedEventArgs e)
        {
            //删除列表项
            Button target = sender as Button;
            string itemId = target.Tag.ToString();
            singleItemUIModels.Remove(singleItemUIModels.First(c => c.ItemId == itemId));
            if (singleListDataDic.ContainsKey(itemId)) singleListDataDic.Remove(itemId);
        }

        private void btnChangeSingleStore_Click(object sender, RoutedEventArgs e)
        {
            //更换仓库
            string itemId = (sender as Button).Tag.ToString();
            MaskVisible(true);
            SelectedStore s = new SelectedStore();
            s.ShowDialog();
            MaskVisible(false);
            if (s.Succeed)
            {
                var _store = singleItemUIModels.Single(c => c.ItemId == itemId);
                _store.StoreId = s.Model.Id;
                _store.StoreName = s.Model.Name;
            }
        }

        #endregion

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdateStoreLogGridAsync();
        }

        #endregion

        #region Loading

        private void ShowLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Visible)
            {
                gLoading.Visibility = Visibility.Visible;
                storeLogList.IsEnabled = false;
                bNoData.IsEnabled = false;

                OnLoadingShowComplate();
            }
        }

        private void HideLoadingPanel()
        {
            if (gLoading.Visibility != Visibility.Collapsed)
            {
                gLoading.Visibility = Visibility.Collapsed;
                storeLogList.IsEnabled = true;
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

        private void btnSelectGoodsOut_Click(object sender, RoutedEventArgs e)
        {
            string goodsName = "";
            string goodsId = "";

            MaskVisible(true);
            SelectedGoods s = new SelectedGoods(1, true);
            s.ShowDialog();
            MaskVisible(false);
            if (s.Ids.Count == 1)
            {
                goodsId = s.Ids[0];
                using (ERPDBContext context = new ERPDBContext())
                {
                    var _goods = context.Goods.First(c => c.Id == goodsId);
                    goodsName = _goods.Name;
                }
            }
            else return;

            MaskVisible(true);
            StoreOutDetail gd = new StoreOutDetail(goodsId, goodsName);
            gd.ShowDialog();
            MaskVisible(false);

            if (gd.Succeed)
            {
                outTempDetailModel = gd.Model;

                if (goodsId.IsNullOrEmpty())
                {
                    MessageBoxX.Show("请选择物品或项目", "空值提醒");
                    return;
                }

                string itemId = Guid.NewGuid().ToString();

                var _model = new GoodsItemUIModel();
                _model.Count = outTempDetailModel.Count;
                _model.TargetId = goodsId;
                _model.ItemId = itemId;
                _model.Name = goodsName;
                _model.StoreId = outTempDetailModel.StoreId;
                _model.StoreName = outTempDetailModel.StoreName;

                outsItemUIModels.Add(_model);

                btnSelectGoodsOut.Content = "未选择物品";
                btnSelectGoodsOut.Tag = "";
            }
            else
            {
                btnSelectGoodsOut.Content = "未选择物品";
                btnSelectGoodsOut.Tag = "";
            }
        }

        private void btnStoreOut_Click(object sender, RoutedEventArgs e)
        {
            if (outsItemUIModels.Count == 0)
            {
                MessageBoxX.Show("没有任何出库数据", "空数据提醒");
                return;
            }

            for (int i = 0; i < outsItemUIModels.Count; i++)
            {
                var _item = outsItemUIModels[i];

                #region 编辑库存记录

                using (ERPDBContext context = new ERPDBContext())
                {
                    if (context.Stock.Any(c => c.StoreId == _item.StoreId && c.GoodsId == _item.TargetId))
                    {
                        //存在直接修改数量
                        var _socket = context.Stock.First(c => c.StoreId == _item.StoreId && c.GoodsId == _item.TargetId);
                        _socket.Count -= _item.Count;

                        context.StockLog.Add(new StockLog()
                        {
                            Count = _item.Count,
                            CreateTime = DateTime.Now,
                            Creator = UserGlobal.CurrUser.Id,
                            Manufacturer = "",
                            SalePrice = 0,
                            StockType = Global.StockTypeOut,
                            StoreId = _item.StoreId,
                            Surplus = _socket.Count,
                            GoodsId = _item.TargetId,
                            Remark = "",
                            Price = 0,
                            SupplierId = 0
                        });
                    }

                    context.SaveChanges();
                }

                #endregion 
            }

            MessageBoxX.Show("出库成功", "成功");
            outsItemUIModels.Clear();

            btnRef_Click(null, null);
        }

        private void btnDeleteOutItem_Click(object sender, RoutedEventArgs e)
        {
            string itemId = (sender as Button).Tag.ToString();
            outsItemUIModels.Remove(outsItemUIModels.First(c => c.ItemId == itemId));
        }

        private void btnUpdatePlan_Click(object sender, RoutedEventArgs e)
        {
            cbPlanCode.SelectedItem = null;
            cbPlanCode.Items.Clear();
            using (ERPDBContext context = new ERPDBContext())
            {
                var planList = context.PurchasePlan.Where(c => !c.Stock).OrderByDescending(c => c.CreateTime).ToList();

                foreach (var item in planList)
                {
                    cbPlanCode.Items.Add(item.PlanCode);
                }
            }
        }

        private void cbPlanCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPlanCode.SelectedItem == null) return;

            string planCode = cbPlanCode.SelectedItem.ToString();//获得选中的采购计划号

            using (ERPDBContext context = new ERPDBContext())
            {
                if (!context.PurchasePlan.Any(c => c.PlanCode == planCode && !c.IsDel && c.Finished && !c.Stock))
                {
                    MessageBoxX.Show("请检查单号是否未采购完成或已入库", "单号不存在");
                    btnUpdatePlan_Click(null, null);
                    return;
                }

                string _storeName = "";
                int _storeId = 0;

                MaskVisible(true);
                SelectedStore s = new SelectedStore();
                s.ShowDialog();
                MaskVisible(false);
                if (s.Succeed)
                {
                    _storeName = s.Model.Name;
                    _storeId = s.Model.Id;
                }
                else
                {
                    btnUpdatePlan_Click(null, null);
                    return;
                }

                List<PurchasePlanItem> items = context.PurchasePlanItem.Where(c => c.PlanCode == planCode).ToList();

                planItemUIModels.Clear();
                foreach (var item in items)
                {
                    var _goods = context.Goods.First(c => c.Id == item.GoodsId);
                    GoodsItemUIModel _model = new GoodsItemUIModel()
                    {
                        ItemId = item.Id,
                        Count = item.Count,
                        TargetId = item.GoodsId,
                        Name = _goods.Name,
                        TypeName = context.SysDic.First(c => c.Id == _goods.TypeId).Name,
                        StoreId = _storeId,
                        StoreName = _storeName
                    };

                    planItemUIModels.Add(_model);
                }
            }
        }
    }
}
