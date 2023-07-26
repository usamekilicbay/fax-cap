using Assets.Scripts.Common.Animations;
using DG.Tweening;
using FaxCap.Common.Abstract;
using FaxCap.Manager;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.UI.Screen
{
    public class UIResultScreen : UIScreenBase, IRenewable
    {
        [SerializeField] private Button homeButton;
        [Header("Level")]
        [SerializeField] private Slider expBar;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI nextLevelText;
        [SerializeField] private TextMeshProUGUI expText;
        [Space(10)]
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreBestLabel;
        [SerializeField] private TextMeshProUGUI scoreText;
        [Space(10)]
        [Header("Question Count")]
        [SerializeField] private TextMeshProUGUI questionCounterBestLabel;
        [SerializeField] private TextMeshProUGUI questionCountText;
        [Space(10)]
        [Header("Combo")]
        [SerializeField] private TextMeshProUGUI comboBestLabel;
        [SerializeField] private TextMeshProUGUI comboText;
        [Space(10)]
        [Header("Effects")]
        [SerializeField] private RectTransform scoreAnimationTextSpawnPoint;
        [SerializeField] private GameObject scoreAnimationTextPrefab;

        private RectTransform _scoreRectTransform;
        private Vector2 _scoreTextInitialSize;
        private Vector2 _questionCountTextInitialSize;
        private Vector2 _comboTextInitialSize;

        private GameManager _gameManager;
        private UIHomeScreen _uiHomeScreen;
        private UIGameScreen _uiGameScreen;
        private ScoreManager _scoreManager;
        private PlayerStatsManager _playerStatsManager;
        private LevelManager _levelManager;

        [Inject]
        public void Construct(GameManager gameManager,
            UIHomeScreen homeScreen,
            UIGameScreen gameScreen,
            ScoreManager scoreManager,
            PlayerStatsManager playerStatsManager,
            LevelManager levelManager)
        {
            _gameManager = gameManager;
            _uiGameScreen = gameScreen;
            _uiHomeScreen = homeScreen;
            _scoreManager = scoreManager;
            _playerStatsManager = playerStatsManager;
            _levelManager = levelManager;
        }

        private void Awake()
        {
            homeButton.onClick
                .AddListener(GoToHomeScreen);

            Setup();

            //StartTheDance();
        }

        public async Task UpdateQuestionText(int questionCount)
        {
            ToggleQuestionCounterBestLabel(_playerStatsManager.HasExceededBestQuestionCount(questionCount));

            await questionCountText
                .AnimateTextt(0, 20, _questionCountTextInitialSize, _questionCountTextInitialSize * 2f, 1f);

            //questionCountText.SetText($"Questions: {questionCount}");
        }

        public void UpdateScoreText(int score)
        {
            scoreText.SetText($"{score}");
        }

        public async Task UpdateScoreTextAsync(IReadOnlyCollection<int> scores)
        {
            var totalScore = 0;

            foreach (var score in scores)
            {
                //GenerateScoreTextAsync(score);
                await GenerateScoreTextAsync(score);
                var currentScore = totalScore;
                totalScore += score;

                await scoreText.AnimateTextt(currentScore, totalScore, _scoreTextInitialSize,
                    _scoreTextInitialSize * 2f, 0.3f);
            }
        }

        public void UpdateBestScoreText(int bestScore)
        {
            questionCounterBestLabel.enabled = true;
            questionCounterBestLabel.SetText($"Best Score");
            //highScoreText.SetText($"High Score: {highScore}");
        }

        public async Task UpdateComboText(int comboCount)
        {
            ToggleComboBestLabel(_playerStatsManager.HasExceededBestCombo(comboCount));

            await comboText.AnimateTextt(0, comboCount, _comboTextInitialSize, _comboTextInitialSize * 2f, 0.3f);

            //comboText.SetText($"{combo}");
        }

        public async Task UpdateLevel(float exp, int currentLevel)
        {
            expBar.value = 0f;
            expBar.maxValue = _levelManager.GetRequiredExp(currentLevel);
            UpdateLevelTexts(currentLevel - 1);
            await UpdateExpBar(exp, currentLevel);
        }

        public void UpdateLevelTexts(int currentLevel)
        {
            currentLevelText.SetText($"Level: {currentLevel}");
            nextLevelText.SetText($"Level: {currentLevel + 1}");
        }

        public async Task GenerateScoreTextAsync(int score)
        //public void GenerateScoreTextAsync(int score)
        {
            ToggleScoreBestLabel(_playerStatsManager.HasExceedBestScore(1));

            var duration = 0.5f;

            var targetPos = scoreText.rectTransform.position;
            var leftBorder = scoreText.rectTransform.rect.xMin;
            var rightBorder = scoreText.rectTransform.rect.xMax;

            var spawnPos = new Vector2
            {
                x = Random.Range(leftBorder, rightBorder),
                y = 0f
            };

            var scoreAnimationText = Instantiate(scoreAnimationTextPrefab, scoreAnimationTextSpawnPoint)
                .GetComponent<TextMeshProUGUI>();

            scoreAnimationText.rectTransform.anchoredPosition = spawnPos;

            scoreAnimationText.SetText(score.ToString());

            var sequence = DOTween.Sequence();
            sequence.Append(scoreAnimationText.rectTransform.DOMoveY(targetPos.y, duration));
            sequence.Join(scoreAnimationText.DOFade(0, duration));
            await sequence.AsyncWaitForCompletion();
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
            questionCounterBestLabel.enabled = false;

            UpdateLevel(_levelManager.Exp, _levelManager.Level);
        }

        private void Setup()
        {
            _scoreRectTransform = scoreText.transform.parent.GetComponent<RectTransform>();
            _scoreTextInitialSize = scoreText.rectTransform.sizeDelta;
            _questionCountTextInitialSize = questionCountText.rectTransform.sizeDelta;
            _comboTextInitialSize = comboText.rectTransform.sizeDelta;
        }

        private void GoToHomeScreen()
        {
            uiManager.ShowScreen(_uiHomeScreen);
        }

        private async Task UpdateExpBar(float exp, int currentLevel)
        {
            var requiredExp = expBar.maxValue - expBar.value;
            var target = requiredExp < exp
                ? exp
                : expBar.maxValue;

            var remainingExp = exp - requiredExp;
            var level = currentLevel;

            if (requiredExp < exp)
            {
                target = expBar.maxValue;

                var expBarFillTween = expBar.DOValue(target, 0.5f);
                expBarFillTween.OnUpdate(UpdateExpText);
                expBarFillTween.OnComplete(async () =>
                {
                    UpdateLevelTexts(level);
                    level++;
                    await UpdateExpBar(remainingExp, level);
                });
            }
            else
            {
                var expBarFillTween = expBar.DOValue(target, 0.5f);
                expBarFillTween.OnUpdate(UpdateExpText);

                await expBarFillTween.AsyncWaitForCompletion();
            }
        }

        private void UpdateExpText()
        {
            expText.SetText($"{expBar.value}/{expBar.maxValue}");
        }

        private void ToggleScoreBestLabel(bool isEnabled)
        {
            scoreBestLabel.enabled = isEnabled;
        }

        private void ToggleQuestionCounterBestLabel(bool isEnabled)
        {
            questionCounterBestLabel.enabled = isEnabled;
        }

        private void ToggleComboBestLabel(bool isEnabled)
        {
            comboBestLabel.enabled = isEnabled;
        }
    }
}
