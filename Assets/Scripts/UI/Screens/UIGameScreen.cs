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

        public Queue<float> ProgressQueue { get; private set; }

        private Vector3 _scoreTextInitialSize;
        private Vector3 _progressIconInitialSize;
        private Vector3 _comboCounterTextInitialSize;
        private Tween _scoreScaleTween;
        private Tween _progressIconScaleTween;
        private Tween _comboCounterScaleDownTween;
        private Sequence _comboCounterSequence;

        #region DI

        private ConfigurationManager _configurationManager;
        private CameraManager _cameraManager;
        private AudioManager _audioManager;

        [Inject]
        public void Construct(ConfigurationManager configurationManager,
            CameraManager cameraManager,
            AudioManager audioManager)
        {
            _configurationManager = configurationManager;
            _cameraManager = cameraManager;
            _audioManager = audioManager;
        }

        #endregion

        private void Awake()
        {
            Setup();
        }

        #region Remove

        int i;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
                UpdateComboCounterText(++i);
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

        public async void UpdateComboCounterText(int comboCount)
        {
            if (comboCount < 2)
            {
                comboCounterText.SetText($"X{comboCount}");
                comboCounterText.gameObject.SetActive(false);
                return;
            }

            comboCounterText.gameObject.SetActive(true);
            comboCounterText.SetText($"X{comboCount}");

            await PlayComboCounterScaleTween();

            _cameraManager.ShakeCamera();

            _audioManager.PlayAudio(_audioManager.ScoreSfxs);
        }

        public void UpdateTimeBar(float timeSpan)
        {
            timeBar.value = timeSpan;
            float normalizedTime = timeSpan / QuestionManager.ReplyTimerLimit;
            timeFill.color = Color.Lerp(Color.red, Color.green, normalizedTime);
        }

        public async Task UpdateProgressBar(float progress)
        {
            ProgressQueue.Enqueue(progress);

            if (ProgressQueue.Count > 1)
                return;

            float target = progress;
            var tween = progressBar.DOValue(target, 1f);
            await tween.AsyncWaitForCompletion();

            if (progressBar.value == progressBar.maxValue)
            {
                await PlayProgressIconScaleTween();
                var resetTween = progressBar.DOValue(0, 1f);
                await resetTween.AsyncWaitForCompletion();
            }

            if (progressBar.value == progressBar.minValue)
            {
                var progressIconResetTween = progressIcon.rectTransform.DORotate(Vector3.zero, 0.5f);
                await progressIconResetTween.AsyncWaitForCompletion();
            }

            ProgressQueue.Dequeue();

            if (ProgressQueue.Any())
            {
                target = ProgressQueue.Dequeue();
                await UpdateProgressBar(target);
            }
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

        #region Setup

        private void Setup()
        {
            BackgroundInitialColor = background.color;
            progressBar.maxValue = _configurationManager.GameConfigs.ProgressMilestone;

            _scoreTextInitialSize = scoreText.rectTransform.sizeDelta;
            _progressIconInitialSize = progressIcon.rectTransform.sizeDelta;
            _comboCounterTextInitialSize = comboCounterText.rectTransform.sizeDelta;

            _comboCounterSequence = DOTween.Sequence();
            ProgressQueue = new Queue<float>();

            SetupProgressAnimation();
            SetupComboCounterAnimation();
            SetupScoreAnimation();
        }

        #region Animation Setup

        private void SetupProgressAnimation()
        {
            var progressIconAnimDuration = 0.5f;

            _progressIconScaleTween = progressIcon.rectTransform
                .DOSizeDelta(_progressIconInitialSize * 2f, progressIconAnimDuration);
            _progressIconScaleTween.OnStart(() =>
            {
                progressIcon.rectTransform.sizeDelta = _progressIconInitialSize;
            });
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

        private void SetupComboCounterAnimation()
        {
            var duration = 0.2f;

            _comboCounterScaleDownTween = comboCounterText.rectTransform
                .DOSizeDelta(_comboCounterTextInitialSize * 0.7f, duration);
            //_comboCounterScaleDownTween.SetEase(Ease.InBack);
            _comboCounterScaleDownTween.SetAutoKill(false);
            _comboCounterScaleDownTween.Pause();

            var comboCounterScaleResetTween = comboCounterText.rectTransform
                .DOSizeDelta(_comboCounterTextInitialSize, 0.2f);
            comboCounterScaleResetTween.SetEase(Ease.InBack);
            comboCounterScaleResetTween.SetAutoKill(false);
            comboCounterScaleResetTween.Pause();

            var comboCounterColorTween = comboCounterText.DOFade(1f, duration);
            comboCounterColorTween.SetAutoKill(false);
            comboCounterColorTween.Pause();

            _comboCounterSequence.OnStart(() =>
            {
                comboCounterText.rectTransform.sizeDelta = _comboCounterTextInitialSize * 20;
                var color = comboCounterText.color;
                color.a = 0.5f;
                comboCounterText.color = color;
            });
            _comboCounterSequence.Append(_comboCounterScaleDownTween);
            _comboCounterSequence.Join(comboCounterColorTween);
            _comboCounterSequence.OnComplete(() => comboCounterText.rectTransform
                .DOSizeDelta(_comboCounterTextInitialSize, 0.1f));
            //_comboCounterSequence.SetDelay(0.2f);
            _comboCounterSequence.SetAutoKill(false);
        }

        private void SetupScoreAnimation()
        {
            _scoreScaleTween = scoreText.rectTransform.DOSizeDelta(_scoreTextInitialSize * 2f, 0.2f);
            _scoreScaleTween.SetLoops(2, LoopType.Yoyo);
            _scoreScaleTween.SetAutoKill(false);
            _scoreScaleTween.Pause();
        }

        #endregion
        #endregion

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

            tween.Restart();

            await tween.AsyncWaitForCompletion();
        }

        private async Task PlayComboCounterScaleTween()
        {
            var sequence = _comboCounterSequence;

            if (sequence == null)
                return;

            if (sequence.IsPlaying())
                sequence.Pause();

            ResetComboCounter();
            sequence.Restart();

            await sequence.AsyncWaitForCompletion();
        }

        private void ResetComboCounter()
        {
            comboCounterText.rectTransform.sizeDelta = _comboCounterTextInitialSize * 20;
            var color = comboCounterText.color;
            color.a = 0.5f;
            comboCounterText.color = color;
        }
    }
}
