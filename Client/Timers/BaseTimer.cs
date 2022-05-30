using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Timers
{
    public abstract class BaseTimer
    {
        /// <summary>
        /// 是否已开始
        /// </summary>
        public bool IsStarted = false;

        public System.Threading.Timer timer;

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="_milliseconds">毫秒</param>
        public virtual void Start(int _milliseconds = 1000)
        {
            if (IsStarted) return;
            IsStarted = true;//已成功运行
            timer = new System.Threading.Timer(OnTimer,null,0, _milliseconds);
        }

        public abstract void OnTimer(object state);
    }
}
