using DG.Tweening;
using FaxCap.Common.Abstract;
using FaxCap.Manager;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [Space(10)]
        [Header("Time")]
        [SerializeField] private Slider timeBar;
        [SerializeField] private Image timeFill;

        private Color _backgroundInitialColor;
        private Vector3 _scoreTextInitialSize;
        private Vector3 _comboCounterTextInitialSize;
        private Tween _scoreScaleTween;
        private Tween _comboCounterScaleTween;

        private void Awake()
        {
            Setup();

            _scoreTextInitialSize = scoreText.rectTransform.sizeDelta;
            _comboCounterTextInitialSize = comboCounterText.rectTransform.sizeDelta;

            AnimationSetup();
        }

        private void Setup()
        {
            _backgroundInitialColor = background.color;
            progressBar.maxValue = 10;
            //timeBar.maxValue = 10;
        }

        private void AnimationSetup()
        {
            _scoreScaleTween = scoreText.rectTransform.DOSizeDelta(_scoreTextInitialSize * 2f, 0.2f);
            _scoreScaleTween.SetLoops(2, LoopType.Yoyo);
            _scoreScaleTween.SetAutoKill(false);
            _scoreScaleTween.Pause();

            _comboCounterScaleTween = comboCounterText.rectTransform.DOSizeDelta(_comboCounterTextInitialSize, 0.4f);
            _comboCounterScaleTween.SetEase(Ease.InBack);
            _comboCounterScaleTween.SetAutoKill(false);
            _comboCounterScaleTween.Pause();
            _comboCounterScaleTween.SetDelay(0.3f);
        }

        int i;
        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                i++;
                UpdateComboCounterText(i);
            }
        }

        public void UpdateBackgroundColor(Color color)
        {
            background.color = color;
        }

        public void UpdateScoreText(int score)
        {
            scoreText.SetText($"{score}");

            PlayScoreScaleTween();
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

        public void UpdateProgressBar(int progress)
        {
            progressBar.DOValue(progress, 0.5f);
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
            background.color = _backgroundInitialColor;
            UpdateScoreText(0);
            UpdateComboCounterText(1);
            UpdateProgressBar(0);
        }

        private void PlayScoreScaleTween()
        {
            var tween = _scoreScaleTween;

            if (tween.IsPlaying())
                tween.Pause();

            scoreText.rectTransform.sizeDelta = _scoreTextInitialSize;
            tween.Restart();
        }

        private void PlayComboCounterScaleTween()
        {
            var tween = _comboCounterScaleTween;

            if (tween.IsPlaying())
                tween.Pause();

            comboCounterText.rectTransform.sizeDelta = _comboCounterTextInitialSize * 5;
            tween.Restart();
        }
    }
}
