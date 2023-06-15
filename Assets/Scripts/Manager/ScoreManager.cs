using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class ScoreManager
    {
        private const int _reqularQuestionScore = 10;
        private const int _perfectScore = 10;

        private int _score;
        private int _comboCounter = 1;
        private const float _perfectScoreTimeSpan = 0f;

        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(UIGameScreen uiGameScreen)
        {
            _uiGameScreen = uiGameScreen;
        }

        public void IncreaseScore(float replyTimeSpan, bool isDoubleScore)
        {
            var tempScore = _reqularQuestionScore;

            if (IsPerfectReplyTime(replyTimeSpan))
            {
                tempScore = _perfectScore;
                _comboCounter++;
                Debug.Log(_comboCounter);
                _uiGameScreen.UpdateComboCounterText(_comboCounter);
            }
            else
                _comboCounter = 1;

            tempScore *= isDoubleScore
                ? 2
                : 1;

            _score += tempScore;

            _uiGameScreen.UpdateScoreText(_score);
        }

        private bool IsPerfectReplyTime(float replyTimeSpan)
        {
            return replyTimeSpan >= _perfectScoreTimeSpan;
        }
    }
}
