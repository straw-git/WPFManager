

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Client.Animation
{
    public class Close_WH : WindowAnimation<Window>
    {
        private bool isCloseApp = false;

        public Close_WH(Window _window, bool _closeApp = false) : base(_window)
        {
            isCloseApp = _closeApp;
        }

        public override void StartAnimation()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation dh = new DoubleAnimation();
            DoubleAnimation dw = new DoubleAnimation();
            dh.Duration = dw.Duration = sb.Duration = new Duration(new TimeSpan(0, 0, 1));
            dh.To = dw.To = 0;
            Storyboard.SetTarget(dh, window);
            Storyboard.SetTarget(dw, window);
            Storyboard.SetTargetProperty(dh, new PropertyPath("Height", new object[] { }));
            Storyboard.SetTargetProperty(dw, new PropertyPath("Width", new object[] { }));
            sb.Children.Add(dh);
            sb.Children.Add(dw);
            sb.Completed += AnimationCompleted;
            sb.Begin();
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (isCloseApp)
            {
                Application.Current.Shutdown();
            }
            else
            {
                window.Close();
            }
        }
    }
}
