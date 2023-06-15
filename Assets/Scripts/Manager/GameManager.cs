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
        private QuestionManager _questionManager;
        private UIHomeScreen _uiHomeScreen;
        private UIGameScreen _uiGameScreen;
        private UIResultScreen _uiResultScreen;

        [Inject]
        public void Construct(UIManagerBase uiManager,
            ICurrencyManager currencyManager,
            DeckManager deckManager,
            QuestionManager questionManager,
            UIHomeScreen uiHomeScreen,
            UIGameScreen uiGameScreen,
            UIResultScreen uiResultScreen)
        {
            _uiManager = uiManager;
            _currencyManager = currencyManager;
            _deckManager = deckManager;
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
        }
    }
}
