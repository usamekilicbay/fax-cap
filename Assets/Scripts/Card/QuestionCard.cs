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
        [SerializeField] private TextMeshProUGUI questionText;
        [Header("Time")]
        [SerializeField] private Slider timeBar;
        [SerializeField] private Image timeFill;
        [Space(10)]
        [Header("Back")]
        [SerializeField] private Image backArtwork;
        [SerializeField] private Image categoryIcon;
        [Space(10)]
        [Header("Effects")]
        [SerializeField] private AudioClip[] sfxs;
        [SerializeField] Sprite[] cardBackArtworks;

        private AudioSource _audioSource;

        private const float _replyTimeLimit = 500f;

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
            timeBar.maxValue = _replyTimeLimit;
            timeBar.value = _replyTimeLimit;
        }

        protected override void Update()
        {
            if (!isTimerStart)
                return;

            replyTimer -= Time.deltaTime;
            UpdateTimeBar(replyTimer);

            if (replyTimer <= 0f)
                GameOver();
        }

        public void UpdateTimeBar(float timeSpan)
        {
            timeBar.value = timeSpan;
            //float normalizedTime = timeSpan / QuestionManager.ReplyTimerLimit;
            //timeFill.color = Color.Lerp(Color.red, Color.green, normalizedTime);
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

            var wrongAnswer = !_questionManager.CheckAnswer(answer);

            if (wrongAnswer)
            {
                GameOver();
                return;
            }

            deckManager.SpawnCard();

            _audioSource.PlayOneShot(sfxs[Random.Range(0, sfxs.Length)]);

            var isDoublePointCard = CardType == CardType.DoublePoint;
            _scoreManager.AddScore(replyTimer, isDoublePointCard);
        }

        // TODO: Not sure if deck manager should fill this part
        public override void UpdateCard()
        {
            questionText.text = _questionManager.GetQuestionText();
            backArtwork.sprite = cardBackArtworks[Random.Range(0, cardBackArtworks.Length)];
        }

        private void GameOver()
        {
            isTimerStart = false;
            _gameManager.CompleteRun();
        }

        public class Factory : PlaceholderFactory<QuestionCard>
        {
            ///
        }
    }
}
