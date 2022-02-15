
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using Common.Utils;
using Common;
using ERPDBModels.Models;

namespace ERPPlugin.Windows
{
    /// <summary>
    /// EditGoods.xaml 的交互逻辑
    /// </summary>
    public partial class EditGoods : Window
    {
        bool isEdit = false;
        string goodsId = "";
        public Goods Model = new Goods();
        public bool Succeed = false;

        public EditGoods(string _goodsId = "")
        {
            InitializeComponent();
            this.UseCloseAnimation();

            if (string.IsNullOrEmpty(_goodsId))
            {
                isEdit = false;
                goodsId = $"G{DateTime.Now.ToString("yyMMddHHmmss")}";
                Title = "添加物品";
                lblTitle.Content = Title;
                btnAdd.Content = "添 加";
            }
            else
            {
                isEdit = true;
                goodsId = _goodsId;
                using (ERPDBContext context = new ERPDBContext())
                {
                    Model = context.Goods.First(c => c.Id == _goodsId);
                }

                Title = $"编辑[{Model.Name}]";
                lblTitle.Content = Title;
                btnAdd.Content = "编 辑";
                //编辑时不允许修改零售价
                txtSalePrice.IsEnabled = false;

                Model2UI();
            }

            Bitmap bmp = new Bitmap(QRCodeCommon.CreateBarCode(goodsId)); // get bitmap
            BitmapImage bmpResource = ImageCommon.BitmapToBitmapImage(bmp);
            imgQR.Source = bmpResource;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadType();
            LoadUnit();
        }

        #region Private Method

        #region Convert

        private bool UI2Model()
        {
            decimal salePrice = 0;

            #region Empty or Error

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBoxX.Show("名称不能为空", "空值提醒");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtSpecification.Text))
            {
                MessageBoxX.Show("规格不能为空", "空值提醒");
                txtSpecification.Focus();
                return false;
            }

            if (!decimal.TryParse(txtSalePrice.Text, out salePrice))
            {
                MessageBoxX.Show("零售价式错误", "格式错误");
                txtSalePrice.Focus();
                return false;
            }

            #endregion 

            Model = new Goods();
            Model.CreateTime = DateTime.Now;
            Model.Creator = UserGlobal.CurrUser.Id;
            Model.Name = txtName.Text;
            Model.QuickCode = txtQuickCode.Text;
            Model.UnitId = cbUnit.SelectedValue.ToString().AsInt();
            Model.TypeId = cbType.SelectedValue.ToString().AsInt();
            Model.Specification = txtSpecification.Text;
            Model.SalePrice = salePrice;
            Model.Remark = txtRemark.Text;
            Model.Id = goodsId;

            return true;
        }

        private void Model2UI()
        {
            txtName.Text = Model.Name;
            txtQuickCode.Text = Model.QuickCode;
            cbUnit.SelectedValue = Model.UnitId;
            cbType.SelectedValue = Model.TypeId;
            txtSpecification.Text = Model.Specification;
            txtSalePrice.Text = Model.SalePrice.ToString();
            txtRemark.Text = Model.Remark;
        }

        #endregion 

        /// <summary>
        /// 加载单位
        /// </summary>
        private void LoadUnit()
        {
            var _source = DataGlobal.GetDic(DicData.GoodsUnit);
            cbUnit.ItemsSource = _source;
            cbUnit.DisplayMemberPath = "Name";
            cbUnit.SelectedValuePath = "Id";

            if (_source.Count > 0)
                cbUnit.SelectedIndex = 0;
        }

        /// <summary>
        /// 加载物品类型
        /// </summary>
        private void LoadType()
        {
            var _source = DataGlobal.GetDic(DicData.GoodsType);
            cbType.ItemsSource = _source;
            cbType.DisplayMemberPath = "Name";
            cbType.SelectedValuePath = "Id";

            if (_source.Count > 0)
                cbType.SelectedIndex = 0;
        }

        #endregion

        #region UI Method

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!UI2Model()) return;
            using (ERPDBContext context = new ERPDBContext())
            {
                if (isEdit)
                {
                    //编辑状态
                    var goods = context.Goods.Single(c => c.Id == goodsId);
                    Model.IsDel = goods.IsDel;
                    Model.DelUser = goods.DelUser;
                    Model.DelTime = goods.DelTime;
                    Model.IsStock = (bool)cbStock.IsChecked;
                    ModelCommon.CopyPropertyToModel(Model, ref goods);
                }
                else
                {
                    if (context.Goods.Any(c => c.Name == Model.Name && c.Specification == Model.Specification && c.TypeId == Model.TypeId && c.UnitId == Model.UnitId))
                    {
                        MessageBoxX.Show("当前物品已存在", "数据重复");
                        return;
                    }
                    Model.IsDel = false;
                    Model.DelUser = 0;
                    Model.DelTime = Model.CreateTime;
                    Model.IsStock = (bool)cbStock.IsChecked;

                    //添加状态
                    context.Goods.Add(Model);
                }
                context.SaveChanges();
            }

            Succeed = true;

            MessageBoxX.Show(isEdit ? "编辑成功" : "添加成功", "成功提醒");

            if (!isEdit) this.Close();//添加状态直接关闭
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                txtQuickCode.Text = $"{txtName.Text.Convert2Pinyin().ToLower()}|{txtName.Text.Convert2Py().ToLower()}";
            }
        }

        #endregion
    }
}
