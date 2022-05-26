using Client.Events;
using Common;
using Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Timers
{
    public class EmailTimer : BaseTimer
    {
        bool hasNotReadEmail = false;
        int timerCount = 0;

        public override void OnTimer(object state)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                int userId = UserGlobal.CurrUser.Id;
                //阅读短信
                var _hasNotReadEmail = context.Email
                    .Any(c => c.NoticeType == 0 && c.TargetId == userId&& !c.IsRead
                    || c.NoticeType == 2 && c.TargetId == userId && !c.IsRead
                    || c.NoticeType == 1 && c.TargetId == UserGlobal.CurrUser.RoleId&& !c.IsRead);

                if (timerCount == 0) 
                {
                    timerCount = 1; 
                    hasNotReadEmail = !_hasNotReadEmail;//首次必须返回
                }
                if (_hasNotReadEmail != hasNotReadEmail)
                {
                    hasNotReadEmail = _hasNotReadEmail;
                    EmailNotReadChangedEventObserver.Instance.Dispatch(Codes.EmailNotReadChanged, new EmailChangedMessage() { HasNotReadEmail = _hasNotReadEmail });
                }
            }
        }
    }
}
