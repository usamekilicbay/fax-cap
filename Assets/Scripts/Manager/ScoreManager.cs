using FaxCap.Common.Abstract;
using FaxCap.Common.Constant;
using FaxCap.UI.Screen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class ScoreManager : IRenewable, ICompletable
    {
        private readonly Dictionary<string, List<int>> _scoreStorage = new();

        private const int _reqularQuestionScore = 10;
        private const int _perfectScore = 10;

        private int _score;
        private int _comboCounter = 1;
        private const float _perfectScoreTimeSpan = 0f;

        private UIGameScreen _gameScreen;
        private UIResultScreen _resultScreen;

        [Inject]
        public void Construct(UIGameScreen gameScreen,
            UIResultScreen resultScreen)
        {
            _gameScreen = gameScreen;
            _resultScreen = resultScreen;

            Setup();
        }

        private void Setup()
        {
            _scoreStorage.Add(Key.Score.Question, new List<int>());
        }

        public void AddScore(float replyTimeSpan, bool isDoubleScore)
        {
            var tempScore = _reqularQuestionScore;

            if (IsPerfectReplyTime(replyTimeSpan))
            {
                tempScore = _perfectScore;
                _comboCounter++;
                Debug.Log(_comboCounter);
                _gameScreen.UpdateComboCounterText(_comboCounter);
            }
            else
                _comboCounter = 1;

            tempScore *= isDoubleScore
                ? 2
                : 1;

            _scoreStorage[Key.Score.Question].Add(tempScore);

            _gameScreen.UpdateScoreText(tempScore);
        }

        public void ResetScore()
        {
            var keys = _scoreStorage.Keys.ToList();

            keys.ForEach(x => _scoreStorage[Key.Score.Question].Clear());
        }

        public async void UpdateScore()
        {
            var scoreKeys = _scoreStorage.Keys;

            foreach (var key in scoreKeys)
            {
                var scores = _scoreStorage[key];

                await _resultScreen.UpdateScoreTextAsync(scores);
                _score += scores.Sum();
            }
        }

        private bool IsPerfectReplyTime(float replyTimeSpan)
            => replyTimeSpan >= _perfectScoreTimeSpan;

        public void Renew()
        {
            ResetScore();
        }

        public void Complete()
        {
            UpdateScore();
        }
    }
}
