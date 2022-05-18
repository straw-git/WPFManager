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
    public class NoticeTimer : BaseTimer
    {
        int count = 0;
        int timerCount = 0;

        public override void OnTimer(object state)
        {
            int userId = UserGlobal.CurrUser.Id;
            using (CoreDBContext context = new CoreDBContext())
            {
                //阅读通知
                var notices = context.Notice.Where(c => c.EndTime >= DateTime.Now).OrderByDescending(c => c.CreateTime).ToList();
                int notReadNoticeCount = 0;

                foreach (var item in notices)
                {
                    if (!context.UserNotice.Any(c => c.UserId == userId && c.NoticeId == item.Id))
                    {
                        //未读
                        notReadNoticeCount += 1;
                    }
                }
                if (timerCount == 0)
                {
                    timerCount = 1;
                    count = notReadNoticeCount + 1;//首次必须返回
                }
                if (count != notReadNoticeCount)
                {
                    count = notReadNoticeCount;
                    NoticeNotReadChangedEventObserver.Instance.Dispatch(Codes.NoticeNotReadCountChanged, new NoticeChangedMessage() { NotReadCount = notReadNoticeCount });
                }
            }

        }
    }
}
