using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPlugin.Pages.Test
{
    public class MenuInfo : BaseMenuInfo
    {
        public MenuInfo() : base("我是主导航 位置：页面(Pages)/功能文件夹(Test)/MenuInfo中") { SelfOrder = 0; }
    }
}
