using FaxCap.UI.Screen;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class GameManager : MonoBehaviour
    {
        private UIManagerBase _uiManager;
        private ICurrencyManager _currencyManager;
        private DeckManager _deckManager;
        private LevelManager _levelManager;
        private QuestionManager _questionManager;
        private ScoreManager _scoreManager;
        private ProgressManager _progressManager;
        private UIHomeScreen _uiHomeScreen;
        private UIGameScreen _uiGameScreen;
        private UIResultScreen _uiResultScreen;

        [Inject]
        public void Construct(UIManagerBase uiManager,
            ICurrencyManager currencyManager,
            DeckManager deckManager,
            ScoreManager scoreManager,
            LevelManager levelManager,
            ProgressManager progressManager,
            QuestionManager questionManager,
            UIHomeScreen uiHomeScreen,
            UIGameScreen uiGameScreen,
            UIResultScreen uiResultScreen)
        {
            _uiManager = uiManager;
            _currencyManager = currencyManager;
            _deckManager = deckManager;
            _levelManager = levelManager;
            _scoreManager = scoreManager;
            _progressManager = progressManager;
            _questionManager = questionManager;
            _uiHomeScreen = uiHomeScreen;
            _uiGameScreen = uiGameScreen;
            _uiResultScreen = uiResultScreen;
        }

        //TODO: Development purpose
        private void Update()
        {
            if (Input.GetMouseButton(2))
                Time.timeScale = 0.3f;
            else if (Input.GetMouseButtonUp(2))
                Time.timeScale = 1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _uiManager.ShowScreen(_uiGameScreen);
                _deckManager.Renew();
            }
        }

        public void StartRun()
        {
            _progressManager.Renew();
            _scoreManager.Renew();
            _questionManager.StartRun();
            _deckManager.Renew();
        }

        public async Task CompleteRun(bool isSuccessful = true)
        {
            await _deckManager.Complete(isSuccessful);

            await Task.Delay(2000);

            _uiManager.ShowScreen(_uiResultScreen);

            if (isSuccessful)
                await _progressManager.Complete(isSuccessful);

            await _levelManager.Complete(isSuccessful);
            await _questionManager.Complete(isSuccessful);
            await _scoreManager.Complete(isSuccessful);
        }
    }
}
