using Assets.Scripts.Common.Extensions;
using FaxCap.Common.Abstract;
using FaxCap.Common.Constant;
using FaxCap.UI.Screen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private int _comboCounter = 0;
        private int _bestCombo = 0;
        private const float _perfectScoreTimeSpan = 0f;

        private UIGameScreen _gameScreen;
        private UIResultScreen _resultScreen;
        private ProgressManager _progressManager;

        [Inject]
        public void Construct(UIGameScreen gameScreen,
            UIResultScreen resultScreen,
            ProgressManager progressManager)
        {
            _gameScreen = gameScreen;
            _resultScreen = resultScreen;
            _progressManager = progressManager;

            Setup();
        }

        public void AddScore(float replyTimeSpan, bool isDoubleScore = false)
        {
            var tempScore = _reqularQuestionScore;

            if (IsPerfectReplyTime(replyTimeSpan))
            {
                tempScore = _perfectScore;
                _comboCounter++;
                
                if (_comboCounter > _bestCombo)
                    _bestCombo = _comboCounter;

                Debug.Log($"X{_comboCounter}");
                _gameScreen.UpdateComboCounterText(_comboCounter);
            }
            else
                _comboCounter = 1;

            tempScore *= isDoubleScore
                ? 2
                : 1;

            tempScore *= _comboCounter > 1
                ? (int)(_comboCounter + _comboCounter * 0.3f)
                : 1;

            _scoreStorage[Key.Score.Question].Add(tempScore);

            _gameScreen.UpdateScoreText(tempScore);
        }

        public void ResetScore()
        {
            var keys = _scoreStorage.Keys.ToList();

            keys.ForEach(x => _scoreStorage[Key.Score.Question].Clear());
        }

        public async Task UpdateScoreAsync(bool isSucessful = true)
        {
            var scoreKeys = _scoreStorage.Keys;

            if (!isSucessful)
            {
                var wastedCount = _progressManager.RemainingToMilestone();
                scoreKeys.ToList().RemoveLast(wastedCount);
            }

            foreach (var key in scoreKeys)
            {
                var scores = _scoreStorage[key];

                await _resultScreen.UpdateScoreTextAsync(scores);
                _score += scores.Sum();
            }
        }

        public void Renew()
        {
            _comboCounter = 0;
            ResetScore();
        }

        public async Task Complete(bool isSuccessful = true)
        {
            await _resultScreen.UpdateComboText(15);
            await UpdateScoreAsync(true);

            //UpdateScore(isSuccessful);
        }

        private void Setup()
        {
            _scoreStorage.Add(Key.Score.Question, new List<int>());
        }

        private bool IsPerfectReplyTime(float replyTimeSpan)
           => replyTimeSpan >= _perfectScoreTimeSpan;
    }
}
