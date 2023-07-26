using FaxCap.Common.Abstract;
using FaxCap.Common.Constant;
using FaxCap.UI.Screen;
using System.Threading.Tasks;
using Zenject;

namespace FaxCap.Manager
{
    public class ProgressManager : IRenewable, ICompletable
    {
        private int _progress;

        private UIGameScreen _gameScreen;
        private UIResultScreen _resultScreen;
        private ConfigurationManager _configurationManager;

        [Inject]
        public void Construct(UIGameScreen gameScreen,
            UIResultScreen resultScreen,
            ConfigurationManager configurationManager)
        {
            _gameScreen = gameScreen;
            _resultScreen = resultScreen;
            _configurationManager = configurationManager;
        }

        public async void Progress()
        {
            _progress++;
            _gameScreen.UpdateProgressBar(_progress);

            if (HasReachedToMilestone())
                Milestone();
        }

        public int RemainingToMilestone()
            => _configurationManager.GameConfigs.ProgressMilestone - _progress;

        public bool HasReachedToMilestone()
           => _progress == _configurationManager.GameConfigs.ProgressMilestone;

        public async Task Complete(bool isSuccessful = true)
        {
            _progress = 0;
        }

        public void Renew()
        {
            _progress = 0;
        }

        private void Milestone()
        {
            _progress = 0;
        }
    }
}
