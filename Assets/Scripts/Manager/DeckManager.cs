using FaxCap.Card;
using FaxCap.Common.Abstract;
using FaxCap.UI.Screen;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class DeckManager : MonoBehaviour, IRenewable, ICompletable
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

        public void SpawnCard()
        {
            _questionManager.AssignNewQuestion();
            var card = _questionCardFactory.Create();
            card.transform.SetParent(cardSpawnPoint);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.UpdateCard();
            _currentCard = card;
        }

        public void Renew()
        {
            if (_currentCard != null)
                Destroy(_currentCard.gameObject);

            SpawnCard();
        }

        public async Task Complete(bool isSuccessful = true)
        {
            Destroy(_currentCard?.gameObject);
        }
    }
}
