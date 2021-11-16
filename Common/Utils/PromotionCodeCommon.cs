
using System.Linq;

namespace Common.Utils
{
    /// <summary>
    /// 推广码
    /// </summary>
    public class PromotionCodeCommon
    {
        /// <summary>
        /// 生成新的编号 
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static string GetCode(int _id)
        {
            int minCodeLength = 4;

            if (_id.ToString().Length <= minCodeLength)
            {
                return _id.ToString().AutoLengthStr(minCodeLength);
            }
            else 
            {
                return _id.ToString();
            }
        }
    }
}
