using Assets.Scripts.Common.Types;
using DG.Tweening;
using FaxCap.Installer;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace FaxCap.Card
{
    [RequireComponent(typeof(RectTransform))]
    public class NavigationCard : CardBase, IPointerClickHandler
    {
        private RectTransform _cardTransform;

        private Vector2 _initialSize;
        private const float _pulseScaleFactor = 1.03f;
        private const float _pulseDuration = 2f;

        private NavigationFacade _navigationFacade;
        private AudioManager _audioManager;

        [Inject]
        public void Construct(NavigationFacade navigationFacade,
            AudioManager audioManager)
        {
            _navigationFacade = navigationFacade;
            _audioManager = audioManager;
        }

        protected override void Awake()
        {
            base.Awake();

            Setup();
        }

        protected override void Start()
        {
            //PulseAnimation();
        }

        private void PulseAnimation()
        {
            // Complete any previous tweens on the object before starting a new one
            _cardTransform.DOKill();

            // Use DOPunchScale to create a pulse animation
            _cardTransform.DOSizeDelta(_initialSize * _pulseScaleFactor, _pulseDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(-1, LoopType.Yoyo);
        }

        protected override void Setup()
        {
            _cardTransform = GetComponent<RectTransform>();
            _initialSize = _cardTransform.sizeDelta;
            ShowFrontSide();

            base.Setup();
        }

        public async void OnPointerClick(PointerEventData eventData)
        {
            if (movementAxis != MovementAxis.Initial)
                return;
            
            _audioManager.PlayAudio(_audioManager.CardSfxs);

            cardTransform.sizeDelta = _initialSize;
            var sizeTween = cardTransform.DOSizeDelta(_initialSize * 0.95f, 0.5f);
            sizeTween.SetEase(Ease.InOutBounce);
            await sizeTween.AsyncWaitForCompletion();

            _navigationFacade.GameManager.StartRun();
            UpdateUI(_navigationFacade.GameScreen);

            cardTransform.sizeDelta = _initialSize;
        }

        public override void UpdateCard()
        {
            throw new System.NotImplementedException();
        }

        protected override async Task SwipeLeft()
        {
            await base.SwipeLeft();

            if (isFrontSideShown)
                UpdateUI(_navigationFacade.SettingsScreen);
            else
                UpdateUI(_navigationFacade.SettingsScreen);
        }

        protected override async Task SwipeUp()
        {
            await base.SwipeUp();

            if (isFrontSideShown)
                UpdateUI(_navigationFacade.LeaderboardScreen);
            else
                UpdateUI(_navigationFacade.SettingsScreen);
        }

        protected override async Task SwipeRight()
        {
            await base.SwipeRight();

            if (isFrontSideShown)
                UpdateUI(_navigationFacade.ProfileScreen);
            else
                UpdateUI(_navigationFacade.SendQuestionScreen);
        }

        protected override async Task SwipeDown()
        {
            await base.SwipeDown();

            FlipCard();

            _navigationFacade.HomeScreen.UpdateNavigationUI(!isFrontSideShown);
        }

        private void UpdateUI(UIScreenBase screen)
        {
            _navigationFacade.UIManager.ShowScreen(screen);
        }
    }
}
