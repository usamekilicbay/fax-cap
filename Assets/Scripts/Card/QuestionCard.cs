using FaxCap.Common.Types;
using FaxCap.Entity;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace FaxCap.Card
{
    public class QuestionCard : CardBase, IDragHandler, IEndDragHandler
    {
        [Header("Front")]
        [SerializeField] private Image frontArtwork;
        [SerializeField] private Image topBar;
        [SerializeField] private Image categoryIcon;
        [SerializeField] private TextMeshProUGUI questionText;
        [Space(10)]
        [Header("Back")]
        [SerializeField] private Image backArtwork;
        [Space(10)]
        [SerializeField] private AudioClip[] sfxs;

        private AudioSource _audioSource;

        private GameManager _gameManager;
        private ScoreManager _scoreManager;
        private QuestionManager _questionManager;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(GameManager gameManager,
            ScoreManager scoreManager,
            UIGameScreen uiGameScreen,
            QuestionManager questionManager)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _uiGameScreen = uiGameScreen;
            _questionManager = questionManager;
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (replyTimer < 0f)
                return;

            replyTimer -= Time.deltaTime;
            _uiGameScreen.UpdateTimerBar(replyTimer);
        }

        protected override void SwipeLeft()
        {
            Debug.Log("Swipe left");
            Answer(false);

            base.SwipeLeft();
        }

        protected override void SwipeRight()
        {
            Debug.Log("Swipe right");
            Answer(true);

            base.SwipeRight();
        }

        private void Answer(bool answer)
        {
            _audioSource.PlayOneShot(sfxs[Random.Range(0, sfxs.Length)]);

            var falseAnswer = !_questionManager.CheckAnswer(answer);

            if (falseAnswer)
                _gameManager.CompleteRun();

            var isDoublePointCard = CardType == CardType.DoublePoint;
            _scoreManager.IncreaseScore(replyTimer, isDoublePointCard);
            _questionManager.AssignNewQuestion();
        }

        // TODO: Not sure if deck manager should fill this part
        public override void UpdateCard()
        {
            questionText.text = _questionManager.GetQuestionText();
        }

        public class Factory : PlaceholderFactory<QuestionCard>
        {
            ///
        }
    }
}
