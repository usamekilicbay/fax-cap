using FaxCap.Card;
using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] private RectTransform cardSpawnPoint;

        private CardBase _currentCard;

        #region Dependency Injection

        private DiContainer _diContainer;
        private GameManager _gameManager;
        private QuestionManager _questionManager;
        private QuestionCard.Factory _questionCardFactory;
        private UIManagerBase _uiManager;
        private UIHomeScreen _uiHomeScreen;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(GameManager gameManager,
            DiContainer diContainer,
            QuestionCard.Factory itemCardFactory)
        {
            _gameManager = gameManager;
            _diContainer = diContainer;
            _questionCardFactory = itemCardFactory;
            _questionManager = _diContainer.Resolve<QuestionManager>();
            _uiManager = _diContainer.Resolve<UIManagerBase>();
            _uiHomeScreen = _diContainer.Resolve<UIHomeScreen>();
            _uiGameScreen = _diContainer.Resolve<UIGameScreen>();
        }

        #endregion

        public void StartRun()
        {
            SpawnCard();
        }

        public void SpawnCard()
        {
            _questionManager.AssignNewQuestion();
            var card = _questionCardFactory.Create();
            card.transform.SetParent(cardSpawnPoint);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.UpdateCard();
            _currentCard = card;
        }

        public void CompleteRun()
        {
            if (!_currentCard.IsUsed)
                Destroy(_currentCard.gameObject);
        }
    }
}
