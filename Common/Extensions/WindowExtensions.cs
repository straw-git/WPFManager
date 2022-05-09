using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

public static class WindowExtensions
{
    public static void SetDataGridMouseWheel(this Window window, DataGrid _grid)
    {
        window.Closed += Window_Closed;
        allDataGrids.Add(window.Title, _grid);
        _grid.PreviewMouseWheel += DataGrid_PreviewMouseWheel;
    }

    public static void UseCloseAnimation(this Window window, bool closeApp = false)
    {
        window.Closing += Window_Closing;
        window.Tag = closeApp;
    }

    /// <summary>
    /// 扩展Windows的Log
    /// </summary>
    /// <param name="_window"></param>
    /// <param name="_logStr"></param>
    public static void Log(this Window _window, string _logStr)
    {
        MainWindowGlobal.MainWindow.WriteInfoOnBottom(_logStr);
    }

    private static Window currCloseWindow = null;

    private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        currCloseWindow = sender as Window;
        e.Cancel = true;
        // base.OnClosing(e);

        Storyboard sb = new Storyboard();
        DoubleAnimation dh = new DoubleAnimation();
        dh.Duration = sb.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
        dh.To = 0;
        Storyboard.SetTarget(dh, currCloseWindow);
        Storyboard.SetTargetProperty(dh, new PropertyPath("Height", new object[] { }));
        sb.Children.Add(dh);
        sb.Completed += AnimationCompleted;
        sb.Begin();
    }

    private static void AnimationCompleted(object sender, EventArgs e)
    {
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
        Window window = sender as Window;
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
