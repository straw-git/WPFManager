using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class Codes
    {
        /// <summary>
        /// 没有阅读的邮件
        /// </summary>
        public const ushort EmailNotReadChanged = 1001;
        /// <summary>
        /// 通知数量改变
        /// </summary>
        public const ushort NoticeNotReadCountChanged = 1002;
    }
}
