
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 附件全局
    /// </summary>
    public class AttachmentGlobal
    {
        /// <summary>
        /// 员工合同附件
        /// </summary>
        public static int StaffContractId = 1000;

        /// <summary>
        /// 附件从属表
        /// </summary>
        public static List<int> AttachmentTableIds=new List<int>()
        {
            StaffContractId
        };
    }
}
