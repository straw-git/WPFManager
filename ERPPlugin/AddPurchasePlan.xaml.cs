using Common;
using Common.Windows;
using DBModels.ERP;
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
using System.Windows.Shapes;

namespace ERPPlugin
{
    /// <summary>
    /// AddPurchasePlan.xaml 的交互逻辑
    /// </summary>
    public partial class AddPurchasePlan : Window
    {
        public AddPurchasePlan(int _id = 0)
        {
            InitializeComponent();
            this.UseCloseAnimation();
            id = _id;
        }

        #region Models

        class UIModel : INotifyPropertyChanged
        {
            public string Id { get; set; }
            public string GoodsId { get; set; }
            public string Name { get; set; }

            public int finished = 0;
            public int Finished
            {
                get => finished;
                set
                {
                    finished = value;
                    FinishedPre = Math.Round(((decimal)Finished / Count) * 100, 2);
                    NotifyPropertyChanged("FinishedPre");
                }
            }
            public decimal FinishedPre
            {
                get; set;
            }

            public int Count { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion 

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();
        Goods tempModel = new Goods();
        public PurchasePlan Model = new PurchasePlan();
        bool isEdit = false;
        string code = "";
        public bool Succeed = false;
        int id = 0;
        public decimal FinishedPre = 0;
        public decimal FinishedPrice = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        #region UI Method

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            (sender as ListBoxItem).Height = 70;
        }

        private void ListBoxItem_Unselected(object sender, RoutedEventArgs e)
        {
            (sender as ListBoxItem).Height = 40;
        }

        private void btnSelectGoods_Click(object sender, RoutedEventArgs e)
        {
            string selectedId = "";
            SelectedGoods g = new SelectedGoods(1);
            g.ShowDialog();
            if (g.Succeed)
            {
                if (g.Ids.Count == 1)
                {
                    selectedId = g.Ids[0];
                }
                if (string.IsNullOrEmpty(selectedId)) return;

                using (DBContext context = new DBContext())
                {
                    tempModel = context.Goods.First(c => c.Id == selectedId);
                    btnSelectGoods.Content = tempModel.Name;
                    btnSelectGoods.Tag = selectedId;
                }
            }
            else 
            {
                btnSelectGoods.Content = "选择物品";
                btnSelectGoods.Tag = "";
            }

            txtCount.Clear();
            txtCount.Focus();
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null) return;
            UIModel selectedModel = list.SelectedItem as UIModel;

            GoodsDetails g = new GoodsDetails(selectedModel.GoodsId, selectedModel.Name, selectedModel.Count, selectedModel.Finished);
            g.ShowDialog();

            using (DBContext context = new DBContext())
            {
                if (g.Succeed)
                {
                    var model = Data.Single(c => c.Id == selectedModel.Id);
                    model.Finished += g.Model.Count;

                    //更改计划项的完成量
                    var planItem = context.PurchasePlanItem.Single(c => c.PlanCode == code && c.GoodsId == model.GoodsId);
                    planItem.Finished = model.Finished;

                    //添加采购记录
                    context.PurchasePlanLog.Add(new PurchasePlanLog()
                    {
                        Count = g.Model.Count,
                        ItemId = model.Id,
                        Manufacturer = g.Model.Manufacturer,
                        PurchasePrice = g.Model.Price,
                        Remark = g.Model.Remark,
                        SupplierId = g.Model.SupplierId,
                        CreateTime = DateTime.Now,
                        Creator = TempBasePageData.message.CurrUser.Id
                    });

                    #region 检查状态

                    if (Data.Sum(c => c.Count) == Data.Sum(c => c.Finished))
                    {
                        if (!Model.Finished)
                        {
                            context.PurchasePlan.Single(c => c.Id == Model.Id).Finished = true;
                            Model.Finished = true;
                        }
                    }
                    else 
                    {
                        if (Model.Finished) 
                        {
                            context.PurchasePlan.Single(c => c.Id == Model.Id).Finished = false;
                            Model.Finished = false;
                        }
                    }

                    #endregion
                }

                context.SaveChanges();
            }
        }

        private void txtCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int count = 0;

                if (string.IsNullOrEmpty(tempModel.Id))
                {
                    MessageBoxX.Show("请选择物品", "空值提醒");
                    btnSelectGoods_Click(null, null);
                    return;
                }

                if (!int.TryParse(txtCount.Text, out count))
                {
                    txtCount.Foreground = Brushes.Red;
                    txtCount.Focus();
                    txtCount.SelectAll();
                    return;
                }

                if (Data.Any(c => c.GoodsId == tempModel.Id))
                {
                    MessageBoxX.Show("当前物品已选择,如数量错误,请删除后重新操作", "数据重复");
                    return;
                }

                txtCount.Foreground = Brushes.Black;
                string id = Guid.NewGuid().ToString();
                Data.Add(new UIModel()
                {
                    Id = id,
                    Count = count,
                    GoodsId = tempModel.Id,
                    Name = btnSelectGoods.Content.ToString(),
                    Finished = 0
                });

                if (isEdit)
                {
                    //编辑状态下直接加入数据库
                    using (DBContext context = new DBContext())
                    {
                        context.PurchasePlanItem.Add(new PurchasePlanItem()
                        {
                            Count = count,
                            GoodsId = tempModel.Id,
                            PlanCode = code,
                            Id = id,
                            Finished = 0
                        });
                        context.SaveChanges();
                    }
                }

                btnSelectGoods.Content = "选择物品";
                btnSelectGoods.Tag = "";
                txtCount.Clear();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (Data.Count == 0)
            {
                MessageBoxX.Show("采购列表不能为空", "空值提醒");
                return;
            }
            if (!isEdit)
            {
                //添加状态
                UI2Model();

                using (DBContext context = new DBContext())
                {
                    Model.Stock = false;
                    context.PurchasePlan.Add(Model);
                    foreach (var item in Data)
                    {
                        context.PurchasePlanItem.Add(new PurchasePlanItem()
                        {
                            Count = item.Count,
                            GoodsId = item.GoodsId,
                            PlanCode = code,
                            Id = item.Id,
                            Finished = 0
                        });
                    }

                    context.SaveChanges();
                }
            }

            UpdateFinished();

            using (DBContext context = new DBContext())
            {
                FinishedPrice = 0;
                for (int i = 0; i < Data.Count; i++)
                {
                    var _d = Data[i];
                    var items = context.PurchasePlanLog.Where(c => c.ItemId == _d.Id).ToList();

                    for (int j = 0; j < items.Count; j++)
                    {
                        var item = items[j];
                        FinishedPrice += item.PurchasePrice * item.Count;
                    }
                }
            }
            Succeed = true;
            Close();
        }

        private void btnCopyCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(lblCode.Content.ToString());
            Notice.Show("单号已成功复制到剪切板", "成功提醒", 2, MessageBoxIcon.Success);
        }

        #endregion

        #region Private Method

        private void Init()
        {
            if (id > 0)
            {
                isEdit = true;
                using (DBContext context = new DBContext())
                {
                    Model = context.PurchasePlan.First(c => c.Id == id);
                    code = Model.PlanCode;

                    var items = context.PurchasePlanItem.Where(c => c.PlanCode == code).ToList();
                    foreach (var item in items)
                    {
                        Data.Add(new UIModel()
                        {
                            Id = item.Id,
                            Count = item.Count,
                            GoodsId = item.GoodsId,
                            Name = context.Goods.First(c => c.Id == item.GoodsId).Name,
                            Finished = item.Finished
                        });
                    }
                }
                btnSubmit.Content = "完 成";

                list.MouseDoubleClick += List_MouseDoubleClick;
            }
            else
            {
                isEdit = false;
                code = $"P{DateTime.Now.ToString("yyMMddhhmmss")}";
            }

            lblCode.Content = code;

            list.ItemsSource = Data;
        }

        private void UpdateFinished()
        {
            FinishedPre = Math.Round(Data.Sum(c => c.FinishedPre) / Data.Count, 2);
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Tag.ToString();
            if (isEdit)
            {
                //编辑状态下 删除数据库中数据
                using (DBContext context = new DBContext())
                {
                    context.PurchasePlanItem.Remove(context.PurchasePlanItem.First(c => c.Id == id));
                    context.SaveChanges();
                }
            }
            Data.Remove(Data.First(c => c.Id == id));
            UpdateFinished();
        }

        private void UI2Model()
        {
            Model = new PurchasePlan();
            Model.CreateTime = DateTime.Now;
            Model.Creator = TempBasePageData.message.CurrUser.Id;
            Model.PlanCode = code;
            Model.Finished = false;

            Model.IsDel = false;
            Model.DelUser = 0;
            Model.DelTime = DateTime.Now;
        }

        #endregion
    }
}
