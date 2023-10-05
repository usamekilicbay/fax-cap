using FaxCap.Common.Types;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace FaxCap.Card
{
    public class QuestionCard : CardBase
    {
        [Header("Front Side")]
        [SerializeField] private Image frontArtwork;
        [SerializeField] private Image topBar;
        [SerializeField] private TextMeshProUGUI questionText;
        [Header("Time")]
        [SerializeField] private Slider timeBar;
        [SerializeField] private Image timeFill;
        [Space(10)]
        [Header("Back Side")]
        [SerializeField] private Image backArtwork;
        [SerializeField] private Image categoryIcon;
        [Space(10)]
        [Header("Effects")]
        [SerializeField] private AudioClip[] sfxs;
        [SerializeField] Sprite[] cardBackArtworks;

        protected CardType cardType;

        private AudioSource _audioSource;

        private GameManager _gameManager;
        private ScoreManager _scoreManager;
        private ProgressManager _progressManager;
        private QuestionManager _questionManager;
        private UIGameScreen _gameScreen;
        private ConfigurationManager _configurationManager;

        [Inject]
        public void Construct(GameManager gameManager,
            ScoreManager scoreManager,
            ProgressManager progressManager,
            UIGameScreen gameScreen,
            QuestionManager questionManager,
            ConfigurationManager configurationManager)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _progressManager = progressManager;
            _gameScreen = gameScreen;
            _questionManager = questionManager;
            _configurationManager = configurationManager;
        }

        protected override void Awake()
        {
            base.Awake();

            Setup();
        }

        protected override void Start()
        {
            FlipCard();

            replyTimer = _configurationManager.GameConfigs.DefaultAnswerPeriod;
            timeBar.maxValue = _configurationManager.GameConfigs.DefaultAnswerPeriod;
            timeBar.value = _configurationManager.GameConfigs.DefaultAnswerPeriod;
        }

        protected override void Update()
        {
            base.Update();

            if (!isTimerStart)
                return;

            replyTimer -= Time.deltaTime;
            UpdateTimeBar(replyTimer);

            if (replyTimer <= 0f)
                GameOver();
        }

        protected override void Setup()
        {
            _audioSource = GetComponent<AudioSource>();

            ShowBackSide();

            base.Setup();
        }

        public void UpdateTimeBar(float timeSpan)
        {
            timeBar.value = timeSpan;
            //float normalizedTime = timeSpan / QuestionManager.ReplyTimerLimit;
            //timeFill.color = Color.Lerp(Color.red, Color.green, normalizedTime);
        }

        protected override async Task SwipeLeft()
        {
            Debug.Log("Swipe left");
            Answer(false);

            await base.SwipeLeft();
        }

        protected override async Task SwipeRight()
        {
            Debug.Log("Swipe right");
            Answer(true);

            await base.SwipeRight();
        }

        protected override async Task SwipeDown()
        {
            Debug.Log("Swipe down");
            GameOver(true);

            await base.SwipeDown();
        }

        private void Answer(bool answer)
        {
            gameScreen.ResetBackgroundColor();

            isTimerStart = false;

            var wrongAnswer = !_questionManager.CheckAnswer(answer);

            if (wrongAnswer)
            {
                GameOver();
                return;
            }

            deckManager.SpawnCard();

            _audioSource.PlayOneShot(sfxs[Random.Range(0, sfxs.Length)]);

            var isDoublePointCard = cardType == CardType.DoublePoint;
            _scoreManager.AddScore(replyTimer, isDoublePointCard);
            _progressManager.Progress();
        }

        // TODO: Not sure if deck manager should fill this part
        public override void UpdateCard()
        {
            questionText.text = _questionManager.GetQuestionText();
            backArtwork.sprite = cardBackArtworks[Random.Range(0, cardBackArtworks.Length)];
        }

        private void GameOver(bool isSuccesful = false)
        {
            isTimerStart = false;
            _gameManager.CompleteRun(isSuccesful);
        }

        public class Factory : PlaceholderFactory<QuestionCard>
        {
            // ...
        }
    }
}
