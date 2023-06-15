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
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboCounterText;
        [SerializeField] private Slider timerIndicator;
        [SerializeField] private Image timerFill;

        private Vector3 _scoreTextInitialSize;
        private Vector3 _comboCounterTextInitialSize;
        private Tween _scoreScaleTween;
        private Tween _comboCounterScaleTween;

        private void Awake()
        {
            _scoreTextInitialSize = scoreText.rectTransform.sizeDelta;
            _comboCounterTextInitialSize = comboCounterText.rectTransform.sizeDelta;

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
            if (Input.GetMouseButtonDown(1))
            {
                i++;
                UpdateComboCounterText(i);
            }
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

        public void UpdateTimerBar(float timeSpan)
        {
            timerIndicator.value = timeSpan;
            float normalizedTime = timeSpan / QuestionManager.ReplyTimerLimit;
            timerFill.color = Color.Lerp(Color.red, Color.green, normalizedTime);
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
            UpdateScoreText(0);
            UpdateComboCounterText(1);
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
