
using System;

namespace Common.Utils
{
    public class IdCardCommon
    {
        /// <summary>
        /// 获取身份证号中的生日
        /// </summary>
        /// <param name="_idCardNumber"></param>
        /// <returns></returns>
        public static DateTime GetBirthday(string _idCardNumber)
        {
            string d = "";
            if (!string.IsNullOrEmpty(_idCardNumber) && _idCardNumber.Length == 18)
            {
                d= $"{_idCardNumber.Substring(6, 4)}-{_idCardNumber.Substring(10, 2)}-{_idCardNumber.Substring(12, 2)}";
            }
            if (!string.IsNullOrEmpty(d)) return Convert.ToDateTime(d);

            return DateTime.Now;
        }
    }
}
