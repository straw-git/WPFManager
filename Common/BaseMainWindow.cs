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
        public abstract void ShowLeftMenu(bool _show);//显示、隐藏左侧导航
        public abstract void ShowTopMenu(bool _show);//显示、隐藏上面导航
        public abstract void ReLoadCurrTopMenu();//刷新当前导航
        public abstract void SetFrameSource(string _s);//设置Frame内页
    }
}
