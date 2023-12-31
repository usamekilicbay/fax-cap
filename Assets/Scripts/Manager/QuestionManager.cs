using FaxCap.Common.Abstract;
using FaxCap.Entity;
using FaxCap.UI.Screen;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class QuestionManager : IRenewable, ICompletable
    {
        private List<Question> _questions = new();
        private Question _question;
        private int questionCount;

        public const float ReplyTimerLimit = 5f;

        private UIGameScreen _uiGameScreen;
        private UIResultScreen _resultScreen;

        [Inject]
        public void Construct(UIGameScreen uiGameScreen,
            UIResultScreen resultScreen)
        {
            _uiGameScreen = uiGameScreen;
            _resultScreen = resultScreen;

            GenerateQuestions();
        }

        public void StartRun()
        {
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
            //_uiGameScreen.UpdateQuestionCounterText(questionCount);
        }

        public string GetQuestionText()
            => _question.QuestionText;

        public bool CheckAnswer(bool answer)
            => _question.Answer == answer;

        public void Renew()
        {
            questionCount = 0;
        }

        public async Task Complete(bool isSuccessful = true)
        {
            await _resultScreen.UpdateQuestionText(questionCount);
        }
    }
}
