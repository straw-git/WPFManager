using Common.Entities;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Common
{
    public abstract partial class BasePage : Page
    {
        public int Order = 0;
        public bool IsMenu = true;//是否包含在导航内
        public string Code = "";
        public DBModels.Sys.User CurrUser = null;//当前登录账户信息
        protected BaseMainWindow ParentWindow = null;//父窗体

        public BasePage()
        {
            this.Loaded += BasePage_Loaded;
        }

        protected abstract void OnPageLoaded();

        private void BasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ParentWindow = System.Windows.Window.GetWindow(this) as BaseMainWindow;
            if (ParentWindow == null) return;
            MainWindowTagInfo parentInfo = ParentWindow.Tag as MainWindowTagInfo;
            if (parentInfo == null)
            {
                //测试时会进入这里使用管理员账户
                MessageBoxX.Show("当前是测试环境", " 模拟管理员操作");
                using (DBContext context = new DBContext())
                {
                    parentInfo = new MainWindowTagInfo();
                    parentInfo.CurrUser = context.User.First(c => c.Name == "admin");
                }
            }
            TempBasePageData.message = parentInfo;
            CurrUser = parentInfo.CurrUser;

            CheckMenu(parentInfo);

            OnPageLoaded();
        }

        protected void MaskVisible(bool _v)
        {
            ParentWindow.IsMaskVisible = _v;
        }

        private void CheckMenu(MainWindowTagInfo parentInfo)
        {
            if (parentInfo.CurrUser.Name == "admin") return;

            string _menuStr = parentInfo.CurrUser.Menus;
            List<string> Menus = _menuStr.Split('|').ToList();

            //Client.Pages.Manager.Dic
            string nps = this.ToString();
            string currName = nps.Substring(nps.LastIndexOf('.') + 1);
            nps = nps.Replace($".{currName}", "");
            string parentName = nps.Substring(nps.LastIndexOf('.') + 1);

            string menuName = $"{parentName}-{currName}-";

            List<string> CurrPageMenus = Menus.Where(c => c.StartsWith(menuName)).ToList();//当前页面中的按钮（已有的权限）
            var menuInfo = parentInfo.Dic.Keys.First(c => c.Code == parentName);
            var buttons = parentInfo.Dic[menuInfo].First(c => c.Code == currName).Buttons;
            foreach (var buttonInfo in buttons)
            {
                Button button = this.FindName(buttonInfo.Name) as Button;
                if (CurrPageMenus.Contains($"{parentName}-{currName}-{buttonInfo.Name}"))
                {
                    //存在权限
                    button.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    //不存在权限
                    button.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #region Data Pager

        object _running = new object();

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_source"></param>
        /// <param name="_where"></param>
        /// <param name="_pagination"></param>                                                                                                                                                                                                                                                                                                                                                                                                 
        /// <param name="_pageSize"></param>
        /// <returns></returns>
        protected List<T> GetDataPager<T>(dynamic _source, Expression<Func<T, bool>> _where, Pagination _pagination, int _pageSize = 10)
            where T : class
        {
            lock (_running) 
            {
                int dataCount = 0;
                int pagerCount = 0;
                int currPage = _pagination.CurrentIndex;

                var pd = _source as DbSet<T>;

                dataCount = _where == null ? pd.Count() : pd.Where(_where.Compile()).Count();
                pagerCount = PagerGlobal.GetPagerCount(dataCount);

                if (currPage > pagerCount) 
                {
                    currPage = pagerCount;
                    _pagination.CurrentIndex = currPage;
                }
                _pagination.TotalIndex = pagerCount;

                return _where == null
                    ? pd.Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList()
                    : pd.Where(_where.Compile()).Skip(_pageSize * (currPage - 1)).Take(_pageSize).ToList();
            }
        }


        #endregion 
    }
}
