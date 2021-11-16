using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Client.Helper
{
    public class DelayHelper : IDisposable
    {
        /// <summary>
        /// 计时器
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// {Action:Function delaySecond：与上次的间隔时间(秒数)
        /// </summary>
        List<dynamic> FinishActions = null;

        /// <summary>
        /// startAction方法执行完成
        /// </summary>
        private bool startActionFinished = false;

        /// <summary>
        /// _finishActions 参数{Action:Function delaySecond：与上次的间隔时间(秒数)}
        /// </summary>
        /// <param name="_startAction"></param>
        /// <param name="_finishActions"></param>
        public DelayHelper(Action _startAction, List<dynamic> _finishActions)
        {
            DoAction(_startAction, _finishActions);
        }

        public DelayHelper(Action _startAction, Action _finishAction, int _delaySecond)
        {
            List<dynamic> _list = new List<dynamic>();
            _list.Add(new { Action = _finishAction, delaySecond = _delaySecond });
            DoAction(_startAction, _list);
        }

        private void DoAction(Action _startAction, List<dynamic> _finishActions)
        {
            FinishActions = _finishActions;

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, FinishActions[0].delaySecond);
            timer.Start();

            _startAction?.Invoke();
            startActionFinished = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //如果开始方法结束了
            if (startActionFinished)
            {
                FinishActions[0].Action?.Invoke();
                FinishActions.RemoveAt(0);
                if (FinishActions != null && FinishActions.Count > 0)
                    timer.Interval = new TimeSpan(0, 0, 0, FinishActions[0].delaySecond);
                if (FinishActions != null && FinishActions.Count == 0)
                    Dispose();
            }
        }

        public void Dispose()
        {
            startActionFinished = false;
            timer.Tick -= Timer_Tick;
            timer.Stop();
            timer = null;
        }
    }
}
