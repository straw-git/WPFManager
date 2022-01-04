using Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 页面临时数据
    /// </summary>
    public partial class UserGlobal
    {
        public static DBModels.Sys.User CurrUser { get; set; }
        public static Dictionary<string, Dictionary<BaseMenuInfo, List<MenuItemModel>>> Dic;

        /// <summary>
        /// 当前是否是员工登录
        /// </summary>
        /// <returns></returns>
        public static bool IsStaffLogin() 
        {
            return CurrUser.StaffId.NotEmpty();
        }

        //是否已登录
        public static bool IsLogin = false;
    }
}
