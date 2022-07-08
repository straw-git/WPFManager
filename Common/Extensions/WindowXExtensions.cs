
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

public static class WindowXExtensions
{
    public static void SetDataGridMouseWheel(this WindowX window, DataGrid _grid)
    {
        window.Closed += Window_Closed;
        allDataGrids.Add(window.Title, _grid);
        _grid.PreviewMouseWheel += DataGrid_PreviewMouseWheel;
    }

    public static void UseCloseAnimation(this WindowX window, bool closeApp = false, bool messageBox = false)
    {
        useMessageBox = messageBox;

        window.Closing += Window_Closing;
        window.Tag = closeApp;
    }

    private static WindowX currCloseWindow = null;
    private static bool useMessageBox = false;

    private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        currCloseWindow = sender as WindowX;
        e.Cancel = true;
        // base.OnClosing(e);

        if (useMessageBox)
        {
            var result = MessageBoxX.Show("是否退出应用", "退出提醒", System.Windows.Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                useMessageBox = false;
                return;
            }
        }

        Storyboard sb = new Storyboard();
        DoubleAnimation dh = new DoubleAnimation();
        dh.Duration = sb.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        dh.To = 0;
        Storyboard.SetTarget(dh, currCloseWindow);
        Storyboard.SetTargetProperty(dh, new PropertyPath("Height", new object[] { }));
        sb.Children.Add(dh);
        sb.Completed += AnimationCompleted;
        sb.Begin();
    }

    private static void AnimationCompleted(object sender, EventArgs e)
    {
        if (currCloseWindow == null) return;

        currCloseWindow.Closing -= Window_Closing;
        currCloseWindow.Close();

        if (Convert.ToBoolean(currCloseWindow.Tag))
        {
            Application.Current.Shutdown();
        }

        currCloseWindow = null;
    }

    public static Dictionary<string, DataGrid> allDataGrids = new Dictionary<string, DataGrid>();

    private static void Window_Closed(object sender, System.EventArgs e)
    {
        WindowX window = sender as WindowX;
        allDataGrids[window.Title].PreviewMouseWheel -= DataGrid_PreviewMouseWheel;
        window.Closed -= Window_Closed;
        allDataGrids.Remove(window.Title);

    }

    private static void DataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        DataGrid _grid = sender as DataGrid;
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
        eventArg.Source = sender;
        _grid.RaiseEvent(eventArg);
    }
}
