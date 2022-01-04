using DBModels.ERP;
using DBModels.Sys;
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
using System.Reflection;
using System.Linq.Expressions;
using Common.Utils;
using Common.MyAttributes;
using Common.Windows;

namespace ERPPlugin.Pages.ERP
{
    /// <summary>
    /// GoodsList.xaml 的交互逻辑
    /// </summary>
    public partial class Item : BasePage
    {
        public Item()
        {
            InitializeComponent();
            this.Order = 4;

            //测试
            //OnPageLoaded();
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public string Id { get; set; }

            private string name = "";
            [DataSourceBinding("名称", -1, 2)]
            public string Name
            {
                get => name;
                set
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                    NotifyPropertyChanged("SubName");
                }
            }
            public string SubName
            {
                get
                {
                    return Name.Length > 20 ? $"{Name.Substring(0, 20)}..." : Name;
                }
            }

            private string typeName = "";
            [DataSourceBinding("类型", -1, 1)]
            public string TypeName //物品类型
            {
                get => typeName;
                set
                {
                    typeName = value;
                    NotifyPropertyChanged("TypeName");
                }
            }

            private string unitName = "";
            [DataSourceBinding("单位", -1, 3)]
            public string UnitName
            {
                get => unitName;
                set
                {
                    unitName = value;
                    NotifyPropertyChanged("UnitName");
                }
            }

            private string specification = "";
            [DataSourceBinding("规格", -1, 4)]
            public string Specification//规格
            {
                get => specification;
                set
                {
                    specification = value;
                    NotifyPropertyChanged("Specification");
                }
            }

            private decimal salePrice = 0;
            [DataSourceBinding("零售价", -1, 5)]
            public decimal SalePrice //零售价
            {
                get => salePrice;
                set
                {
                    salePrice = value;
                    NotifyPropertyChanged("SalePrice");
                }
            }

            private int count = 0;
            [DataSourceBinding("库存", 0, 6)]
            public int Count//数量
            {
                get => count;
                set
                {
                    count = value;
                    NotifyPropertyChanged("Count");
                }
            }
        }

        #endregion

        //页面数据集合
        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            SetDataGridBinding(list, new UIModel(), Data);
            LoadType();
        }

        /// <summary>
        /// 加载物品类型
        /// </summary>
        private void LoadType()
        {
            var _source = DataGlobal.GetDic(DicData.GoodsType);

            _source.Insert(0, new SysDic()
            {
                Id = -1,
                Name = "全部"
            });

            cbType.ItemsSource = _source;
            cbType.DisplayMemberPath = "Name";
            cbType.SelectedValuePath = "Id";

            cbType.SelectedIndex = 0;
        }

        #region UI Method

        private async void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            if (e == null) gPager.CurrentIndex = 1;//如果是通过查询或者刷新点击的 直接显示第一页

            //查询条件
            string name = txtName.Text.Trim();
            int typeId = cbType.SelectedValue.ToString().AsInt();

            Data.Clear();//先清空再加入页面数据

            using (DBContext context = new DBContext())
            {
                Expression<Func<DBModels.ERP.Goods, bool>> _where = n => GetPagerWhere(n, name, typeId);//按条件查询
                Expression<Func<DBModels.ERP.Goods, DateTime>> _orderByDesc = n => n.CreateTime;//按时间倒序
                //开始分页查询数据
                var _zPager = await PagerCommon.BeginEFDataPagerAsync(context.Goods, _where, _orderByDesc, gLoading, gPager, bNoData, new Control[1] { list });
                if (!_zPager.Result) return;
                List<DBModels.ERP.Goods> _list = _zPager.EFDataList;

                #region 页面数据填充

                foreach (var item in _list)
                {
                    string typeName = context.SysDic.First(c => c.Id == item.TypeId).Name;
                    string unitName = context.SysDic.First(c => c.Id == item.UnitId).Name;
                    int count = context.Stock.Any(c => c.GoodsId == item.Id) ? context.Stock.Where(c => c.GoodsId == item.Id).Sum(c => c.Count) : 0;
                    var _model = DBItem2UIModel(item, typeName, unitName, count);
                    Data.Add(_model);
                }

                list.UpdateLayout();
                #endregion
            }

            //结尾处必须结束分页查询
            PagerCommon.EndEFDataPager();
        }

        private UIModel DBItem2UIModel(DBModels.ERP.Goods item, string typeName, string unitName, int count)
        {
            return new UIModel()
            {
                Id = item.Id,
                Name = item.Name,
                SalePrice = item.SalePrice,
                TypeName = typeName,
                Specification = item.Specification,
                UnitName = unitName,
                Count = count
            };
        }

        /// <summary>
        /// 查找表格的条件
        /// </summary>
        /// <param name="_goods"></param>
        /// <param name="_name"></param>
        /// <param name="_phone"></param>
        /// <param name="_isMember"></param>
        /// <param name="isBlcak"></param>
        /// <param name="_enableTime"></param>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <returns></returns>
        protected bool GetPagerWhere(DBModels.ERP.Goods _goods, string _name, int _typeId)
        {
            bool resultCondition = true;
            if (_name.NotEmpty())
            {
                //根据名称检索
                resultCondition &= _goods.Name.Contains(_name) || _goods.QuickCode.Contains(_name);
            }
            if (_typeId > 0)
            {
                resultCondition &= _goods.TypeId == _typeId;
            }
            resultCondition &= !_goods.IsDel;

            return resultCondition;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            EditGoods editGoods = new EditGoods();
            editGoods.ShowDialog();
            MaskVisible(false);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            string id = (sender as Button).Tag.ToString();
            var selectModel = Data.Single(c => c.Id == id);
            EditGoods g = new EditGoods(id);
            g.ShowDialog();
            MaskVisible(false);
            if (g.Succeed)
            {
                using (DBContext context = new DBContext())
                {
                    selectModel.Name = g.Model.Name;
                    selectModel.Specification = g.Model.Specification;
                    selectModel.TypeName = context.SysDic.First(c => c.Id == g.Model.TypeId).Name;
                    selectModel.UnitName = context.SysDic.First(c => c.Id == g.Model.UnitId).Name;
                }
            }
        }

        private void btnUpdateSalePrice_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);
            string id = (sender as Button).Tag.ToString();
            var selectModel = Data.First(c => c.Id == id);
            UpdateSalePrice u = new UpdateSalePrice(id, selectModel.SalePrice);
            u.ShowDialog();
            MaskVisible(false);
            if (u.Succeed)
            {
                selectModel.SalePrice = u.NewPrice;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();
            var selectModel = Data.Single(c => c.Id == id);
            var result = MessageBoxX.Show($"是否确认删除物品[{selectModel.Name}]？", "删除提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (DBContext context = new DBContext())
                {
                    var _model = context.Goods.Single(c => c.Id == id);
                    _model.IsDel = true;
                    _model.DelUser = UserGlobal.CurrUser.Id;
                    _model.DelTime = DateTime.Now;

                    context.SaveChanges();
                }
                UpdatePager(null, null);
            }
        }

        #endregion

        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)((sender as CheckBox).IsChecked);
            foreach (var item in Data)
            {
                item.IsChecked = isCheck;
                if (isCheck)
                {
                    //如果已经选中 说明原来没有选中 将它加入到列表
                    SelectedTableData(list.Name, item);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(list.Name, c => c.Id == item.Id);
                }
            }
        }

        #region 导出Excel

        private void btnExportCurrPage_Click(object sender, RoutedEventArgs e)
        {
            var listData = Data.ToList();//获取选中数据
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题
            if (list == null || listData.Count == 0)
            {
                MessageBoxX.Show("没有选中数据", "空值提醒");
                return;
            }
            new ExcelHelper().List2ExcelAsync(listData, $"页码{gPager.CurrentIndex}", columns, hiddleColumns.Keys.ToList());
        }

        private async void btnExportAllPage_Click(object sender, RoutedEventArgs e)
        {
            //导出所有数据
            List<UIModel> allData = new List<UIModel>();
            string listName = list.Name;

            await Task.Run(() =>
            {
                var _list = new List<Goods>();
                using (DBContext context = new DBContext())
                {
                    _list = context.Goods.ToList();
                    foreach (var item in _list)
                    {
                        string typeName = context.SysDic.First(c => c.Id == item.TypeId).Name;
                        string unitName = context.SysDic.First(c => c.Id == item.UnitId).Name;
                        int count = context.Stock.Any(c => c.GoodsId == item.Id) ? context.Stock.Where(c => c.GoodsId == item.Id).Sum(c => c.Count) : 0;
                        var _model = DBItem2UIModel(item, typeName, unitName, count);
                        allData.Add(_model);
                    }
                }
            });
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题

            new ExcelHelper().List2ExcelAsync(allData, "所有数据", columns, hiddleColumns.Keys.ToList());
        }

        private void btnExportFocusDatas_Click(object sender, RoutedEventArgs e)
        {
            var listData = GetSelectedTableData<UIModel>(list.Name);//获取选中数据
            var columns = GetDataGridColumnVisibleHeaders(list.Name, true);//获取所有显示列对应的标题
            var hiddleColumns = GetDataGridColumnVisibleHeaders(list.Name, false);//获取所有隐藏列的标题
            if (listData == null || listData.Count == 0)
            {
                MessageBoxX.Show("没有选中数据", "空值提醒");
                return;
            }
            new ExcelHelper().List2ExcelAsync(listData, "选中数据", columns, hiddleColumns.Keys.ToList());
        }

        #endregion

        private void btnTableColumnVisible_Click(object sender, RoutedEventArgs e)
        {
            MaskVisible(true);

            BasePageVisibilititySetting basePageVisibilititySetting = new BasePageVisibilititySetting(GetDataGridHeaders(list.Name));
            basePageVisibilititySetting.ShowDialog();

            var result = basePageVisibilititySetting.Result;
            foreach (var ri in result)
            {
                Visibility _visibility = ri.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                SetDataGridColumnVisibilitity(list.Name, ri.Title, _visibility);
            }

            UpdateDataGridColumnVisibility(list);

            MaskVisible(false);
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
                    SelectedTableData(list.Name, selectItem);
                }
                else
                {
                    //未选中说明原来是选中的 将它移出列表
                    UnSelectedTableData(list.Name, c => c.Id == selectItem.Id);
                }
            }
        }
    }
}
