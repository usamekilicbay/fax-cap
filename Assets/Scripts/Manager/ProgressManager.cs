using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class ProgressManager : MonoBehaviour
    {
        private const int _milestone = 3;
        private int progress;

        private UIGameScreen _gameScreen;

        [Inject]
        public void Construct(UIGameScreen gameScreen)
        {
            _gameScreen = gameScreen;
        }

        public void Progress()
        {
            progress++;

            if (HasReachedToMilestone())
            {
                Milestone();
            }
        }

        private void Milestone()
        {
            _gameScreen.UpdateProgress(progress);
            progress = 0;
        }

        public bool HasReachedToMilestone()
            => progress == _milestone;
    }
}
