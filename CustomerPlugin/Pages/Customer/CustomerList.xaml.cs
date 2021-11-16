using Common;
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
            Order = 1;
        }

        #region Models

        class UIModel : BaseUIModel
        {
            public string Id { get; set; }

            private string name = "";
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
            public string UnitName
            {
                get => unitName;
                set
                {
                    unitName = value;
                    NotifyPropertyChanged("UnitName");
                }
            }

            private string packageName = "";
            public string PackageName
            {
                get => packageName;
                set
                {
                    packageName = value;
                    NotifyPropertyChanged("PackageName");
                }
            }

            private decimal salePrice = 0;
            public decimal SalePrice //零售价
            {
                get => salePrice;
                set
                {
                    salePrice = value;
                    NotifyPropertyChanged("SalePrice");
                }
            }
        }

        #endregion

        ObservableCollection<UIModel> Data = new ObservableCollection<UIModel>();

        protected override void OnPageLoaded()
        {
            using (DBContext context = new DBContext())
            {
                Expression<Func<DBModels.Sys.User, bool>> _where = n => GetPagerWhere(n);
                list.ItemsSource = GetDataPager(context.User, _where, gPager);
            }
        }

        private void UpdatePager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {
            int currPage = e == null ? 1 : e.CurrentIndex;
            using (DBContext context = new DBContext())
            {
                Expression<Func<DBModels.Sys.User, bool>> _where = n => GetPagerWhere(n);
                list.ItemsSource = GetDataPager(context.User, _where, gPager);
            }
        }

        protected bool GetPagerWhere(DBModels.Sys.User user)
        {
            bool boolResult = true;
            boolResult &= user.Name == "admin";

            return boolResult;
        }

        private void btnRef_Click(object sender, RoutedEventArgs e)
        {
            UpdatePager(null, null);
        }
    }
}
