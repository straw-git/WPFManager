using Client.Events;
using Common;
using Common.Data.Local;
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
        public override void OnTimer(object state)
        {
            bool hasNotReadEmail = false;
            int timerCount = 0;
            using (CoreDBContext context = new CoreDBContext())
            {
                int userId = UserGlobal.CurrUser.Id;
                List<string> readRoleEmailIds = LocalReadEmail.GetRoleEmail();
                List<string> readAllUserEmailIds = LocalReadEmail.GetAllUserEmail();
                //阅读短信
                var _hasNotReadEmail = context.EmailSendTo
                    .Any(c => c.UserId == 0 && c.RoleId == 0 && !readRoleEmailIds.Contains(c.Id.ToString())  //所有用户都可以看的
                    || c.UserId == userId && !c.IsRead  //查找当前用户的
                    || c.RoleId == UserGlobal.CurrUser.RoleId && !readAllUserEmailIds.Contains(c.Id.ToString()));//查找当前角色的

                if (timerCount == 0)
                {
                    timerCount = 1;
                    hasNotReadEmail = !_hasNotReadEmail;//首次必须返回
                }
                if (_hasNotReadEmail != hasNotReadEmail)
                {
                    hasNotReadEmail = _hasNotReadEmail;
                }
                EmailNotReadChangedEventObserver.Instance.Dispatch(Codes.EmailNotReadChanged, new EmailChangedMessage() { HasNotReadEmail = _hasNotReadEmail });
            }
        }
    }
}
