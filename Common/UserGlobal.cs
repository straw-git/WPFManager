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
        /// <summary>
        /// 数据表
        /// </summary>
        public static User CurrUser { get; set; }
        /// <summary>
        /// 所有页面信息
        /// </summary>
        public static List<PluginsPage> CurrPluginsPage { get; set; }
        /// <summary>
        /// 用户可用的属性
        /// </summary>
        public static List<Plugins> CanUsePlugins { get; set; }

        /// <summary>
        /// 设置当前用户的信息
        /// </summary>
        public static void SetCurrUser(User _user, List<Plugins> _canUsePlugins, List<PluginsPage> _pluginsPages)
        {
            IsLogin = true;
            CurrUser = _user;
            CurrPluginsPage = _pluginsPages;
            CanUsePlugins = _canUsePlugins;
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
