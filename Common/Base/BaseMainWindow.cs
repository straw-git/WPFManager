using CoreDBModels;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class BaseMainWindow : WindowX
    {
        public void EnableMainWindow(bool _enable)
        {
            IsEnabled = _enable;
        }

        public void MaskVisible(bool _visible)
        {
            IsMaskVisible = _visible;
        }

        public abstract void SetFrameSource(string _s);//设置Frame内页
        public abstract void ReLoadMenu();//刷新导航
        public abstract void Log(string _logStr);//输出
    }
}
