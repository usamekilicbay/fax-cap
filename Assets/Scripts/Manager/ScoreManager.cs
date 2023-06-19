using FaxCap.Common.Abstract;
using FaxCap.Common.Constant;
using FaxCap.UI.Screen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class ScoreManager : IRenewable
    {
        private Dictionary<string, List<int>> _scoreStorage = new();

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

        public void AddScore(float replyTimeSpan, bool isDoubleScore)
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

            _scoreStorage[Key.Score.Question].Add(tempScore);

            _uiGameScreen.UpdateScoreText(_score);
        }

        public void ResetScore()
        {
            _scoreStorage[Key.Score.Question].Clear();
        }

        public void UpdateScore()
        {
            var scoreKeys = _scoreStorage.Keys;

            var scoreCounter = 0;

            foreach (var key in scoreKeys)
            {
                var scores = _scoreStorage[key];

                foreach (var score in scores)
                {
                    scoreCounter += score;
                    // TODO: Call score text particle spawn and text update here
                }
                
                _score += scoreCounter;
            }
        }

        private bool IsPerfectReplyTime(float replyTimeSpan)
            => replyTimeSpan >= _perfectScoreTimeSpan;

        public void Renew()
        {
            ResetScore();
        }
    }
}
