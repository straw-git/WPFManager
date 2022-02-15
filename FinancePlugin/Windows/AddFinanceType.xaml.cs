
using FinanceDBModels.Models;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinancePlugin.Windows
{
    /// <summary>
    /// SelectedProject.xaml 的交互逻辑
    /// </summary>
    public partial class AddFinanceType : Window
    {
        public AddFinanceType()
        {
            InitializeComponent();
            this.UseCloseAnimation();
        }

        #region Models

        class UIModel
        {
            public string Name { get; set; }//类型名称
            public int TypeId { get; set; }//财务类型类型  0:进项 1：出项
            public string TypeName { get; set; }
            public string CreateTime { get; set; }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            list.ItemsSource = Data;
            LoadListAsync();
        }

        #region Private Method

        private async void LoadListAsync()
        {
            Data.Clear();
            List<FinanceType> models = new List<FinanceType>();
            await Task.Run(() =>
            {
                using (FinanceDBContext context = new FinanceDBContext())
                {
                    models = context.FinanceType.OrderByDescending(c=>c.CreateTime).ToList();
                }
            });
            await Task.Delay(300);
            foreach (var item in models)
            {
                Data.Add(new UIModel()
                {
                    CreateTime = item.CreateTime.ToString("yyyy年MM月dd日"),
                    Name = item.Name,
                    TypeId = item.TypeId,
                    TypeName = item.TypeId == 0 ? "进项" : "出项"
                });
            }
        }

        #endregion

        #region UI Method


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.IsNullOrEmpty())
            {
                MessageBoxX.Show("请输入类型名称", "空值提醒");
                return;
            }

            FinanceType model = new FinanceType();
            model.CreateTime = DateTime.Now;
            model.Creator = Common.UserGlobal.CurrUser.Id;
            model.Name = txtName.Text;
            model.TypeId = (bool)cbIsIn.IsChecked ? 0 : 1;

            using (FinanceDBContext context = new FinanceDBContext()) 
            {
                context.FinanceType.Add(model);
                context.SaveChanges();
            }
            MessageBoxX.Show("成功","成功");
            Data.Insert(0,new UIModel()
            {
                CreateTime = model.CreateTime.ToString("yyyy年MM月dd日"),
                Name = model.Name,
                TypeId = model.TypeId,
                TypeName = model.TypeId == 0 ? "进项" : "出项"
            });
            txtName.Clear();
            cbIsIn.IsChecked = false;
        }

        #endregion 
    }
}
