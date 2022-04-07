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
        /// 当前账户权限下的插件
        /// </summary>
        public static List<Plugins> Plugins = new List<Plugins>();
        /// <summary>
        /// 当前账户权限下的模块
        /// </summary>
        public static List<PluginsModule> PluginsModules = new List<PluginsModule>();
        /// <summary>
        /// 当前账户权限下的页面
        /// </summary>
        public static List<ModulePage> ModulePages = new List<ModulePage>();

        public static CoreSetting CoreSetting { get; set; }

        /// <summary>
        /// 设置当前用户的信息
        /// </summary>
        public static void SetCurrUser(User _user, CoreSetting _coreSetting)
        {
            IsLogin = true;
            CurrUser = _user;
            CoreSetting = _coreSetting;
        }


        //是否已登录
        public static bool IsLogin = false;
    }
}
