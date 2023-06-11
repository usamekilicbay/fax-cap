using FaxCap.Manager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.UI.Screen
{
    public class UIHomeScreen : UIScreenBase
    {
        [SerializeField] private Button startRunButton;

        private GameManager _gameManager;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(GameManager gameManager,
            UIGameScreen gameScreen)
        {
            _gameManager = gameManager;
            _uiGameScreen = gameScreen;
        }

        private void Awake()
        {
            startRunButton.onClick
                .AddListener(StartItemCollectRun);
        }

        private void StartItemCollectRun()
        {
            _gameManager.StartItemCollectRun();
            uiManager.ShowScreen(_uiGameScreen);
        }
    }
}
