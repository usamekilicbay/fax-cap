using Assets.Scripts.Common.Types;
using FaxCap.Installer;
using FaxCap.UI.Screen;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace FaxCap.Card
{
    public class NavigationCard : CardBase, IPointerClickHandler
    {
        private NavigationFacade _navigationFacade;

        [Inject]
        public void Construct(NavigationFacade navigationFacade)
        {
            _navigationFacade = navigationFacade;
        }

        protected override void Awake()
        {
            base.Awake();

            Setup();
        }

        protected override void Start()
        {
            // ...
        }

        protected override void Setup()
        {
            ShowFrontSide();
            
            base.Setup();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (movementAxis != MovementAxis.Initial)
                return;

            _navigationFacade.GameManager.StartRun();
            UpdateUI(_navigationFacade.GameScreen);
        }

        public override void UpdateCard()
        {
            throw new System.NotImplementedException();
        }

        protected override void SwipeLeft()
        {
            base.SwipeLeft();

            if (isFrontSideShown)
                UpdateUI(_navigationFacade.SettingsScreen);
            else
                UpdateUI(_navigationFacade.SettingsScreen);
        }

        protected override void SwipeUp()
        {
            base.SwipeUp();

            if (isFrontSideShown)
                UpdateUI(_navigationFacade.LeaderboardScreen);
            else
                UpdateUI(_navigationFacade.SettingsScreen);
        }

        protected override void SwipeRight()
        {
            base.SwipeRight();

            if (isFrontSideShown)
                UpdateUI(_navigationFacade.ProfileScreen);
            else
                UpdateUI(_navigationFacade.SendQuestionScreen);
        }

        protected override void SwipeDown()
        {
            base.SwipeDown();

            FlipCard();

            _navigationFacade.HomeScreen.UpdateNavigationUI(!isFrontSideShown);
        }

        private void UpdateUI(UIScreenBase screen)
        {
            _navigationFacade.UIManager.ShowScreen(screen);
        }

    }
}
