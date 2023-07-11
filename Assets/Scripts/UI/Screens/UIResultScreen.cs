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

            _scoreRectTransform = scoreText.transform.parent.GetComponent<RectTransform>();
            _scoreTextInitialSize = scoreText.rectTransform.sizeDelta;
        }

        public void UpdateQuestionText(int questionCount)
        {
            questionCountText.SetText($"Questions: {questionCount}");
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
                //await GenerateScoreTextAsync(score);
                GenerateScoreTextAsync(score);
                var currentScore = totalScore;
                totalScore += score;

                await AnimateScore(currentScore, totalScore);
            }
        }

        public void ShowScoreBestLabel()
        {
            scoreBestLabel.enabled = true;
        }

        public void ShowQuestionCounterBestLabel()
        {
            questionCounterBestLabel.enabled = true;
        }

        public void ShowComboBestLabel()
        {
            comboBestLabel.enabled = true;
        }

        private async Task AnimateScore(int currentScore, int targetScore)
        {
            var duration = 0.3f;

            var scoreUpdateTween = DOTween.To(() => currentScore, x => currentScore = x, targetScore, duration);
            scoreUpdateTween.SetEase(Ease.Linear);
            scoreUpdateTween.Pause();

            var targetSize = _scoreTextInitialSize * 2f;

            var scoreSizeTween = scoreText.rectTransform.DOSizeDelta(targetSize, duration);
            scoreSizeTween.SetEase(Ease.InBack);
            scoreSizeTween.SetLoops(2, LoopType.Yoyo);
            scoreSizeTween.Pause();

            var sequence = DOTween.Sequence();
            sequence.Append(scoreUpdateTween);
            sequence.Join(scoreSizeTween);
            sequence.OnUpdate(() => scoreText.SetText(currentScore.ToString()));
            sequence.OnComplete(() => scoreText.SetText(targetScore.ToString()));
            sequence.Restart();

            await sequence.AsyncWaitForCompletion();
        }

        public void UpdateHighScoreText(int highScore)
        {
            questionCounterBestLabel.enabled = true;
            questionCounterBestLabel.SetText($"High Score");
            //highScoreText.SetText($"High Score: {highScore}");
        }

        public void UpdateComboText(int combo)
        {
            comboText.SetText($"{combo}");
        }

        public async void UpdateLevel(float exp, int currentLevel)
        {
            await UpdateExpBar(exp, currentLevel);
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

        public void UpdateLevelTexts(int currentLevel)
        {
            currentLevelText.SetText($"Level: {currentLevel}");
            nextLevelText.SetText($"Level: {currentLevel + 1}");
        }

        //public async Task GenerateScoreTextAsync(int score)
        public void GenerateScoreTextAsync(int score)
        {
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
            //await sequence.AsyncWaitForCompletion();
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
            questionCounterBestLabel.enabled = false;
        }
    }
}
