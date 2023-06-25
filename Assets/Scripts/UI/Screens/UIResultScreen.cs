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
        [SerializeField] private Slider levelBar;
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
                await GenerateScoreTextAsync(score);
                totalScore += score;

                // TODO: Update score with animation!!

                scoreText.SetText($"{totalScore}");
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

        private void AnimateScore(int currentScore, int targetScore)
        {
            DOTween.To(() => currentScore, x => currentScore = x, targetScore, 0.3f)
                .SetEase(Ease.Linear)
                .OnUpdate(() => scoreText.SetText(currentScore.ToString()))
                .OnComplete(() => scoreText.SetText(targetScore.ToString()));
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

        public void UpdateLevelBar(float exp)
        {
            levelBar.DOValue(exp, 0.5f)
                .OnUpdate(UpdateExpText);
        }

        private void UpdateExpText()
        {
            expText.SetText($"{levelBar.value}/{levelBar.maxValue}");
        }

        public void UpdateLevelTexts(int currentLevel)
        {
            currentLevelText.SetText($"Level: {currentLevel}");
            nextLevelText.SetText($"Level: {currentLevel + 1}");
        }

        public async Task GenerateScoreTextAsync(int score)
        {
            var duration = 1f;

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
