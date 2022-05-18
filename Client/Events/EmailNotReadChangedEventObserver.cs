using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Events
{
    public class EmailChangedMessage
    {
        /// <summary>
        /// 未读条目
        /// </summary>
        public bool HasNotReadEmail { get; set; }
    }

    public class EmailNotReadChangedEventObserver : ObserverBase<EmailNotReadChangedEventObserver, EmailChangedMessage, ushort>
    {
    }
}
