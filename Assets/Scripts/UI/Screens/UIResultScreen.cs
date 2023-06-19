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
        [Space(10)]
        [SerializeField] private TextMeshProUGUI questionCountText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [Header("Prefab")]
        [SerializeField] private GameObject scoreTextPrefab;

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
        }

        private void GoToHomeScreen()
        {
            uiManager.ShowScreen(_uiHomeScreen);
        }

        public void UpdateQuestionText(int questionCount)
        {
            questionCountText.SetText($"Questions: {questionCount}");
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

        private void AnimateScore(int currentScore, int targetScore)
        {
            DOTween.To(() => currentScore, x => currentScore = x, targetScore, 0.3f)
                .SetEase(Ease.Linear)
                .OnUpdate(() => scoreText.SetText(currentScore.ToString()))
                .OnComplete(() => scoreText.SetText(targetScore.ToString()));
        }

        public void UpdateHighScoreText(int highScore)
        {
            highScoreText.enabled = true;
            highScoreText.SetText($"High Score");
            //highScoreText.SetText($"High Score: {highScore}");
        }

        public void UpdateLevelBar(float exp)
        {
            levelBar.DOValue(exp, 0.5f);
        }

        public void UpdateLevelTexts(int currentLevel)
        {
            currentLevelText.SetText($"Level: {currentLevel}");
            nextLevelText.SetText($"Level: {currentLevel + 1}");
        }

        private async Task GenerateScoreTextAsync(int score)
        {
            var scoreText = Instantiate(scoreTextPrefab)
                .GetComponent<TextMeshProUGUI>();

            scoreText.SetText(score.ToString());

            var scoreTextTween = scoreText.rectTransform.DOAnchorPos(scoreText.rectTransform.anchoredPosition, 0.3f);
            scoreTextTween.OnUpdate(() => scoreText.DOFade(0, 0.3f));
            await scoreTextTween.AsyncWaitForCompletion();
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
            highScoreText.enabled = false;
        }
    }
}
