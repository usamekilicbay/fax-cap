using FaxCap.Common.Constant;
using FaxCap.UI.Screen;
using Zenject;

namespace FaxCap.Manager
{
    public class ProgressManager
    {
        private int _progress;

        private UIGameScreen _gameScreen;
        private ConfigurationManager _configurationManager;

        [Inject]
        public void Construct(UIGameScreen gameScreen,
            ConfigurationManager configurationManager)
        {
            _gameScreen = gameScreen;
            _configurationManager = configurationManager;
        }

        public async void Progress()
        {
            _progress++;
            await _gameScreen.UpdateProgressBar(_progress);

            if (HasReachedToMilestone())
            {
                Milestone();
                await _gameScreen.UpdateProgressBar(_progress);
            }
        }

        private void Milestone()
        {
            _progress = 0;
        }

        public bool HasReachedToMilestone()
            => _progress == _configurationManager.GameConfigs.ProgressMilestone;
    }
}
