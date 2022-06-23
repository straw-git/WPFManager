using Common;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


public static class UserControlExtensions
{
    static UserControlExtensions()
    {

    }

    public static void MaskVisible(this UserControl _page, bool _v)
    {
        var parentWindow = MainWindowGlobal.MainWindow;
        if (parentWindow != null)
        {
            parentWindow.MaskVisible(_v);
        }
    }

    public static void Log(this UserControl _page, string _logStr)
    {
        MainWindowGlobal.MainWindow.Log(_logStr);
    }

    /// <summary>
    /// 闪烁
    /// </summary>
    /// <param name="_page"></param>
    /// <param name="color"></param>
    /// <param name="_duration"></param>
    public static void FlickerColor(this UserControl _page,Color color, double _duration = 0.5)
    {
        SolidColorBrush ys = new SolidColorBrush();//颜色绘制

        ColorAnimation ks = new ColorAnimation();//颜色动画处理
        ks.From = color;//初始颜色
        ks.To = Colors.Transparent;//结束颜色
        ks.AutoReverse = true;//反向播放动画
        ks.RepeatBehavior = new RepeatBehavior(2);//无限循环播放
        ks.Completed += (object sender, EventArgs e) =>
        {
            _page.Background = new SolidColorBrush(Colors.Transparent);
        };
        ks.Duration = new Duration(TimeSpan.FromSeconds(_duration));//动画一次所用时间
        ys.BeginAnimation(SolidColorBrush.ColorProperty, ks);//颜色绘制使用动画绘制
        _page.Background = ys;//窗体背景颜色为绘制颜色
    }

}
