using DG.Tweening;
using FaxCap.Common.Abstract;
using FaxCap.Manager;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.UI.Screen
{
    public class UIGameScreen : UIScreenBase, IRenewable
    {
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboCounterText;
        [Space(10)]
        [Header("Progress")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private Image progressFill;
        [SerializeField] private Image progressIcon;
        [Space(10)]
        [Header("Time")]
        [SerializeField] private Slider timeBar;
        [SerializeField] private Image timeFill;

        public Color BackgroundInitialColor { get; private set; }
        public Color BackgroundCurrentColor { get; private set; }

        private Vector3 _scoreTextInitialSize;
        private Vector3 _progressIconInitialSize;
        private Vector3 _comboCounterTextInitialSize;
        private Tween _scoreScaleTween;
        private Tween _progressIconScaleTween;
        private Tween _comboCounterScaleTween;

        #region DI

        private ConfigurationManager _configurationManager;

        [Inject]
        public void Construct(ConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        #endregion

        private void Awake()
        {
            Setup();

            _scoreTextInitialSize = scoreText.rectTransform.sizeDelta;
            _progressIconInitialSize = progressIcon.rectTransform.sizeDelta;
            _comboCounterTextInitialSize = comboCounterText.rectTransform.sizeDelta;

            AnimationSetup();
        }

        private void Setup()
        {
            BackgroundInitialColor = background.color;
            progressBar.maxValue = _configurationManager.GameConfigs.ProgressMilestone;
        }

        private void AnimationSetup()
        {
            _scoreScaleTween = scoreText.rectTransform.DOSizeDelta(_scoreTextInitialSize * 2f, 0.2f);
            _scoreScaleTween.SetLoops(2, LoopType.Yoyo);
            _scoreScaleTween.SetAutoKill(false);
            _scoreScaleTween.Pause();

            _comboCounterScaleTween = comboCounterText.rectTransform
                .DOSizeDelta(_comboCounterTextInitialSize, 0.4f);
            _comboCounterScaleTween.SetEase(Ease.InBack);
            _comboCounterScaleTween.SetAutoKill(false);
            _comboCounterScaleTween.Pause();
            _comboCounterScaleTween.SetDelay(0.3f);

            var progressIconAnimDuration = 0.5f;

            _progressIconScaleTween = progressIcon.rectTransform
                .DOSizeDelta(_progressIconInitialSize * 2f, progressIconAnimDuration);
            _progressIconScaleTween.OnStepComplete(async () =>
            {
                if (_progressIconScaleTween.CompletedLoops() == 1)
                {
                    var progressIconRotateTween = progressIcon.rectTransform.DORotate(new Vector3(0f, 180f, 0f), progressIconAnimDuration);

                    await progressIconRotateTween.AsyncWaitForCompletion();
                }
            });

            _progressIconScaleTween.SetLoops(2, LoopType.Yoyo);
            _progressIconScaleTween.SetAutoKill(false);
            _progressIconScaleTween.Pause();
        }

        #region Remove

        int i;
        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                i++;
                UpdateComboCounterText(i);
            }
        }

        #endregion

        public void UpdateBackgroundColor(Color color)
        {
            BackgroundCurrentColor = color;
            background.color = BackgroundCurrentColor;
        }

        public void ResetBackgroundColor()
        {
            BackgroundCurrentColor = BackgroundInitialColor;
            background.DOColor(BackgroundCurrentColor, 0.5f);
        }

        public void UpdateScoreText(int score)
        {
            scoreText.SetText($"{score}");

            PlayScoreTextScaleTween();
        }

        public void UpdateComboCounterText(int comboCount)
        {
            if (comboCount == 1)
            {
                comboCounterText.gameObject.SetActive(false);
                return;
            }

            comboCounterText.gameObject.SetActive(true);
            comboCounterText.SetText($"X{comboCount}");

            PlayComboCounterScaleTween();
        }

        public void UpdateTimeBar(float timeSpan)
        {
            timeBar.value = timeSpan;
            float normalizedTime = timeSpan / QuestionManager.ReplyTimerLimit;
            timeFill.color = Color.Lerp(Color.red, Color.green, normalizedTime);
        }

        public async Task UpdateProgressBar(float progress)
        {
            var tween = progressBar.DOValue(progress, 1f);
            await tween.AsyncWaitForCompletion();

            if (progressBar.value == progressBar.maxValue)
                await PlayProgressIconScaleTween();

            if (progressBar.value == progressBar.minValue)
                progressIcon.rectTransform.DORotate(Vector3.zero, 0.5f);
        }

        public override Task Show()
        {
            Renew();

            return base.Show();
        }

        public override Task Hide()
        {
            return base.Hide();
        }

        public void Renew()
        {
            background.color = BackgroundInitialColor;
            UpdateScoreText(0);
            UpdateComboCounterText(0);
            UpdateProgressBar(0);
        }

        private void PlayScoreTextScaleTween()
        {
            var tween = _scoreScaleTween;

            if (tween == null)
                return;

            if (tween.IsPlaying())
                tween.Pause();

            scoreText.rectTransform.sizeDelta = _scoreTextInitialSize;
            tween.Restart();
        }

        private async Task PlayProgressIconScaleTween()
        {
            var tween = _progressIconScaleTween;

            if (tween == null)
                return;

            if (tween.IsPlaying())
                tween.Pause();

            progressIcon.rectTransform.sizeDelta = _progressIconInitialSize;
            tween.Restart();

            await tween.AsyncWaitForCompletion();
        }

        private void PlayComboCounterScaleTween()
        {
            var tween = _comboCounterScaleTween;

            if (tween == null)
                return;

            if (tween.IsPlaying())
                tween.Pause();

            comboCounterText.rectTransform.sizeDelta = _comboCounterTextInitialSize * 5;
            tween.Restart();
        }
    }
}
