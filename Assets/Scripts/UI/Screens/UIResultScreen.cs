using FaxCap.Common.Abstract;
using FaxCap.Manager;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.UI.Screen
{
    public class UIResultScreen : UIScreenBase, IRenewable
    {
        [SerializeField] private Button homeButton;

        private GameManager _gameManager;
        private UIHomeScreen _uiHomeScreen;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(GameManager gameManager,
            UIHomeScreen homeScreen,
            UIGameScreen gameScreen)
        {
            _gameManager = gameManager;
            _uiGameScreen = gameScreen;
            _uiHomeScreen = homeScreen;
        }

        private void Awake()
        {
            homeButton.onClick
                .AddListener(GoToHomeScreen);
        }

        private void GoToHomeScreen()
        {
            uiManager.ShowScreen(_uiHomeScreen);
        }


        public override Task Show()
        {
            return base.Show();
        }

        public override Task Hide()
        {
            Renew();

            return base.Hide();
        }

        public void Renew()
        {
        }
    }
}
