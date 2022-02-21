using CoreDBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Common
{
    /// <summary>
    /// 全局用户数据
    /// </summary>
    public partial class UserGlobal
    {
        #region Models

        public class PluginsModel
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public BitmapImage LogoImageSource { get; set; }
            public List<ModuleModel> Modules { get; set; }
            public int Order { get; set; }
        }

        public class ModuleModel
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public List<PageModel> Pages { get; set; }
            public int Order { get; set; }
            public string Icon { get; set; }
        }

        public class PageModel
        {
            public string Code { get; set; }
            public string Url { get; set; }
            public int Order { get; set; }
            public string Icon { get; set; }
        }


        #endregion 

        /// <summary>
        /// 数据表
        /// </summary>
        public static User CurrUser { get; set; }
        /// <summary>
        /// 所有的插件信息
        /// </summary>
        public static List<PluginsModel> SelectedPlugins = new List<PluginsModel>();

        /// <summary>
        /// 设置当前用户的信息
        /// </summary>
        public static void SetCurrUser(User _user)
        {
            IsLogin = true;
            CurrUser = _user;
        }

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="_plugins"></param>
        public static void AddPluginModel(List<PluginsModel> _plugins)
        {
            bool _update = false;
            foreach (var p in _plugins)
            {
                if (SelectedPlugins.Contains(p))
                {
                    //加入选中列表
                    SelectedPlugins.Add(p);
                    _update = true;
                }
            }

            //排序
            if (_update) SelectedPlugins = SelectedPlugins.OrderBy(c => c.Order).ToList();
        }

        /// <summary>
        /// 当前是否是员工登录
        /// </summary>
        /// <returns></returns>
        public static bool IsStaffLogin()
        {
            if (!IsLogin) return false;
            return CurrUser.StaffId.NotEmpty();
        }

        //是否已登录
        public static bool IsLogin = false;
    }
}
