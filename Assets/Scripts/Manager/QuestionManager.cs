using FaxCap.UI.Screen;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class QuestionManager : MonoBehaviour
    {
        private Dictionary<string, bool> questions = new();
        private string question;
        private bool _answer;
        private int questionCount;

        public const float ReplyTimerLimit = 5f;

        private ScoreManager scoreManager;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(UIGameScreen uiGameScreen)
        {
            _uiGameScreen = uiGameScreen;
        }

        private void Start()
        {
            GenerateQuestions();
            AssignNewQuestion();
            UpdateQuestionCount();
        }

        private void GenerateQuestions()
        {
            questions.Clear();

            questions.Add("Are dogs mammals?", true);
            questions.Add("Is the Earth flat?", false);
            questions.Add("Is water wet?", true);
            questions.Add("Can birds fly?", true);
            questions.Add("Is the sun a star?", true);
            questions.Add("Is 1 + 1 equal to 3?", false);
            questions.Add("Do humans have five fingers on each hand?", true);
            questions.Add("Is the capital of France London?", false);
            questions.Add("Is chocolate a vegetable?", false);
            questions.Add("Do plants require sunlight to grow?", true);
        }

        public void AssignNewQuestion()
        {
            var keys = new List<string>(questions.Keys);
            var randomIndex = Random.Range(0, keys.Count);
            question = keys[randomIndex];
            _answer = questions[question];
            UpdateQuestionCount();
        }

        private void UpdateQuestionCount()
        {
            questionCount++;
            _uiGameScreen.UpdateQuestionCountText(questionCount);
        }

        public string GetQuestionText()
            => question;

        public bool CheckAnswer(bool answer)
            => _answer == answer;
    }
}
