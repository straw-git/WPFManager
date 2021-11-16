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
    public class TempBasePageData
    {
        public static MainWindowTagInfo message = new MainWindowTagInfo();

        /// <summary>
        /// 当前是否是员工登录
        /// </summary>
        /// <returns></returns>
        public static bool IsStaffLogin() 
        {
            return message.CurrUser.StaffId.NotEmpty();
        }
    }
}
