using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public string Code { get; set; }
            public List<ModuleModel> Modules { get; set; }
        }

        public class ModuleModel
        {
            public string Code { get; set; }
            public List<PageModel> Pages { get; set; }
        }

        public class PageModel
        {
            public string Code { get; set; }
            public List<ElementModel> Elements { get; set; }
        }

        public class ElementModel
        {
            public string Code { get; set; }
        }

        #endregion 

        /// <summary>
        /// 数据表
        /// </summary>
        public static DBModels.Sys.User CurrUser { get; set; }
        /// <summary>
        /// 所有的插件信息
        /// </summary>
        public static List<PluginsModel> Plugins = new List<PluginsModel>();

        /// <summary>
        /// 设置当前用户的信息
        /// </summary>
        public static void SetCurrUser(DBModels.Sys.User _user) 
        {
            CurrUser = _user;

        }


        public static Dictionary<string, Dictionary<BaseMenuInfo, List<MenuItemModel>>> Dic;

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
