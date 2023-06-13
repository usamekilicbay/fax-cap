using FaxCap.Entity;
using FaxCap.UI.Screen;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class QuestionManager : MonoBehaviour
    {
        private List<Question> _questions = new();
        private Question _question;
        private int questionCount;

        public const float ReplyTimerLimit = 5f;

        private ScoreManager _scoreManager;
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
            _questions.Clear();

            var animalsCategory = new Category("Animals");
            var spaceCategory = new Category("Space");
            var chemistryCategory = new Category("Chemistry");
            var mathCategory = new Category("Math");
            var biologyCategory = new Category("Biology");
            var geographyCategory = new Category("Geography");
            var foodCategory = new Category("Food");
            var natureCategory = new Category("Nature");

            _questions.Add(new Question("Are dogs mammals?", true, animalsCategory));
            _questions.Add(new Question("Is the Earth flat?", false, spaceCategory));
            _questions.Add(new Question("Is water wet?", true, chemistryCategory));
            _questions.Add(new Question("Can birds fly?", true, biologyCategory));
            _questions.Add(new Question("Is the sun a star?", true, spaceCategory));
            _questions.Add(new Question("Is 1 + 1 equal to 3?", false, mathCategory));
            _questions.Add(new Question("Do humans have five fingers on each hand?", true, biologyCategory));
            _questions.Add(new Question("Is the capital of France London?", false, geographyCategory));
            _questions.Add(new Question("Is chocolate a vegetable?", false, foodCategory));
            _questions.Add(new Question("Do plants require sunlight to grow?", true, natureCategory));
        }

        public void AssignNewQuestion()
        {
            var randomIndex = Random.Range(0, _questions.Count);
            _question = _questions[randomIndex];

            UpdateQuestionCount();
        }

        private void UpdateQuestionCount()
        {
            questionCount++;
            _uiGameScreen.UpdateQuestionCountText(questionCount);
        }

        public string GetQuestionText()
            => _question.QuestionText;

        public bool CheckAnswer(bool answer)
            => _question.Answer == answer;
    }
}
