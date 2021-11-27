using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    /// <summary>
    /// 手机号验证
    /// </summary>
    public class PhoneNumberCommon
    {
        /// <summary>
        /// 11位电话号是否合格（当前只验证位数）
        /// </summary>
        /// <param name="_phoneNum"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber11(string _phoneNum) 
        {
            if (_phoneNum.Length == 11) 
            {
                return true;
            }
            return false;
        }
    }
}
