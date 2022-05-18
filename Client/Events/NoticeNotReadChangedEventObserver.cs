using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Events
{
    public class NoticeChangedMessage
    {
        /// <summary>
        /// 未读条目
        /// </summary>
        public int NotReadCount { get; set; }
    }

    public class NoticeNotReadChangedEventObserver : ObserverBase<NoticeNotReadChangedEventObserver, NoticeChangedMessage, ushort>
    {
    }

}
