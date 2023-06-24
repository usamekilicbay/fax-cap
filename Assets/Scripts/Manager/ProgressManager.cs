using FaxCap.UI.Screen;
using Zenject;

namespace FaxCap.Manager
{
    public class ProgressManager
    {
        private const int _milestone = 3;
        private int _progress;

        private UIGameScreen _gameScreen;

        [Inject]
        public void Construct(UIGameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }

        public void Progress()
        {
            _progress++;

            if (HasReachedToMilestone())
            {
                Milestone();
            }
        }

        private void Milestone()
        {
            _gameScreen.UpdateProgressBar(_progress);
            _progress = 0;
        }

        public bool HasReachedToMilestone()
            => _progress == _milestone;
    }
}
