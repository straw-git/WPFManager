using Common;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


public static class PageExtensions
{
    static PageExtensions() 
    {

    }
    public static void StartPageInAnimation(this Page _page)
    {
        Storyboard sb = new Storyboard();
        ThicknessAnimation margin = new ThicknessAnimation();
        DoubleAnimation opacity = new DoubleAnimation();
        margin.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 450));
        opacity.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 350));
        margin.From = new Thickness(100, 0, -100, 0);
        opacity.From = 0;
        margin.To = new Thickness(0);
        margin.DecelerationRatio = 0.9;
        opacity.To = 1;

        Storyboard.SetTarget(margin, _page);
        Storyboard.SetTarget(opacity, _page);
        Storyboard.SetTargetProperty(margin, new PropertyPath("Margin", new object[] { }));
        Storyboard.SetTargetProperty(opacity, new PropertyPath("Opacity", new object[] { }));
        sb.Children.Add(margin);
        sb.Children.Add(opacity);
        sb.Begin();

    }

    public static void MaskVisible(this Page _page, bool _v)
    {
        var parentWindow = _page.GetParentWindow();
        if (parentWindow != null)
        {
            parentWindow.MaskVisible(_v);
        }
    }

    public static BaseMainWindow GetParentWindow(this Page _page)
    {
        try
        {
            BaseMainWindow parentWindow = (BaseMainWindow)_page.Tag;
            return parentWindow;
        }
        catch
        {
            return null;
        }
    }
}
