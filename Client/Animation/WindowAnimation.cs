

using Panuon.UI.Silver;

namespace Client.Animation
{

    public delegate void AnimationEventHandler(object sender);

    public class WindowAnimation<T>
    {
        internal T window;
        public AnimationEventHandler OnAnimationCompleted;
        public WindowAnimation(T _window)
        {
            window = _window;

            StartAnimation();
        }

        public virtual void StartAnimation() { }
    }
}
