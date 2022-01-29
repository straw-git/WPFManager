

namespace DemoPlugin
{
    public class WindowGlobal
    {
        public static MainWindow MainWindow;

        public static void EnableMainWindow(bool _enable)
        {
            MainWindow.IsEnabled = _enable;
        }

        public static void MaskVisible(bool _visible)
        {
            MainWindow.IsMaskVisible = _visible;
        }
    }
}
