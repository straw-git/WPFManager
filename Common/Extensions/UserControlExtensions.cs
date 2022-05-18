using Common;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
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
}
