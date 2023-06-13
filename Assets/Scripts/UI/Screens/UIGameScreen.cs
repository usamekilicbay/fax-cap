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
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI questionCountText;
        [SerializeField] private Slider timerIndicator;
        [SerializeField] private Image timerFill;

        public void UpdateScoreText(int score)
        {
            scoreText.SetText($"Score: {score}");
        }

        public void UpdateQuestionCountText(int questionCount)
        {
            scoreText.SetText($"Question: {questionCount}");
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
            UpdateQuestionCountText(1);
        }
    }
}
