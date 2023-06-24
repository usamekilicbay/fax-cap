using FaxCap.UI.Screen;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _uiManager.ShowScreen(_uiGameScreen);
                _deckManager.StartRun();
            }
        }

        public void StartRun()
        {

            _questionManager.StartRun();
            _deckManager.StartRun();
        }

        public void CompleteRun()
        {
            _deckManager.CompleteRun();
            _uiManager.ShowScreen(_uiResultScreen);
            _scoreManager.Complete();
            _levelManager.Complete();
        }
    }
}
