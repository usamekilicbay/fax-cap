using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class ScoreManager
    {
        private const int _reqularQuestionScore = 10;
        private const int _doubleQuestionScore = 10;

        private float _doubleScoreTimeSpan = 4f;
        private int _score;

        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(UIGameScreen uiGameScreen)
        {
            _uiGameScreen = uiGameScreen;
        }

        public void IncreaseScore(float replyTimeSpan, bool isDoubleScore)
        {
            var tempscore = replyTimeSpan < _doubleScoreTimeSpan
                ? _reqularQuestionScore
                : _doubleQuestionScore;

            tempscore *= isDoubleScore
                ? 2
                : 1;

            _score += tempscore;

            _uiGameScreen.UpdateScoreText(_score);
        }
    }
}
