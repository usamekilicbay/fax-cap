using FaxCap.Common.Types;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace FaxCap.Card
{
    public class QuestionCard : CardBase, IDragHandler, IEndDragHandler
    {
        [Header("Front")]
        [SerializeField] private Image frontArtwork;
        [SerializeField] private Image topBar;
        [SerializeField] private Image categoryIcon;
        [SerializeField] private TextMeshProUGUI questionText;
        [Space(10)]
        [Header("Back")]
        [SerializeField] private Image backArtwork;
        [Space(10)]
        [SerializeField] private AudioClip[] sfxs;
        [SerializeField] Sprite[] cardBackArtworks;

        private AudioSource _audioSource;

        private const float _replyTimeLimit = 5f;

        private GameManager _gameManager;
        private ScoreManager _scoreManager;
        private QuestionManager _questionManager;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(GameManager gameManager,
            ScoreManager scoreManager,
            UIGameScreen uiGameScreen,
            QuestionManager questionManager)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _uiGameScreen = uiGameScreen;
            _questionManager = questionManager;
        }

        protected override void Awake()
        {
            base.Awake();

            _audioSource = GetComponent<AudioSource>();
        }

        protected override void Start()
        {
            base.Start();

            replyTimer = _replyTimeLimit;
        }

        protected override void Update()
        {
            if (!isTimerStart)
                return;

            replyTimer -= Time.deltaTime;
            _uiGameScreen.UpdateTimerBar(replyTimer);

            if (replyTimer <= 0f)
                _gameManager.CompleteRun();
        }

        protected override void SwipeLeft()
        {
            Debug.Log("Swipe left");
            Answer(false);

            base.SwipeLeft();
        }

        protected override void SwipeRight()
        {
            Debug.Log("Swipe right");
            Answer(true);

            base.SwipeRight();
        }

        private void Answer(bool answer)
        {
            isTimerStart = false;

            deckManager.SpawnCard();

            _audioSource.PlayOneShot(sfxs[Random.Range(0, sfxs.Length)]);

            var falseAnswer = !_questionManager.CheckAnswer(answer);

            if (falseAnswer)
                _gameManager.CompleteRun();

            var isDoublePointCard = CardType == CardType.DoublePoint;
            _scoreManager.IncreaseScore(replyTimer, isDoublePointCard);
        }

        // TODO: Not sure if deck manager should fill this part
        public override void UpdateCard()
        {
            questionText.text = _questionManager.GetQuestionText();
            backArtwork.sprite = cardBackArtworks[Random.Range(0, cardBackArtworks.Length)];
        }

        private void OnDestroy()
        {
            isTimerStart = false;
        }

        public class Factory : PlaceholderFactory<QuestionCard>
        {
            ///
        }
    }
}
