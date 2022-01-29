using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPlugin.Pages.Test1
{
    public class MenuInfo : BaseMenuInfo
    {
        public MenuInfo() : base("测试导航 SelfOrder是内部导航排序") { SelfOrder = 1; }
    }
}
