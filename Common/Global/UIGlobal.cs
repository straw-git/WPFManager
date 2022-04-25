using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

public partial class UIGlobal
{
    /// <summary>
    /// 运行UI线程
    /// </summary>
    /// <param name="action"></param>
    public static void RunUIAction(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }
}
