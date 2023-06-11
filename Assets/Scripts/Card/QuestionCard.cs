using DG.Tweening;
using FaxCap.Common.Types;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.Card
{
    public class QuestionCard : CardBase, IDragHandler, IEndDragHandler
    {
        private TextMeshProUGUI questionText;
        private AudioSource _audioSource;

        [SerializeField] private AudioClip[] sfxs;

        private ScoreManager _scoreManager;
        private QuestionManager _questionManager;
        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(ScoreManager scoreManager,
            UIGameScreen uiGameScreen,
            QuestionManager questionManager)
        {
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
            if (replyTimer > 0f)
            {
                replyTimer -= Time.deltaTime;

                _uiGameScreen.UpdateTimerBar(replyTimer);
            }
        }

        protected override void SwipeLeft()
        {
            Debug.Log("Swipe left");

            _audioSource.PlayOneShot(sfxs[Random.Range(0, sfxs.Length)]);


            if (_questionManager.CheckAnswer(false))
                _scoreManager.IncreaseScore(replyTimer, CardType == CardType.DoublePoint);

            _questionManager.AssignNewQuestion();

            base.SwipeLeft();
        }

        protected override void SwipeRight()
        {
            Debug.Log("Swipe right");

            _audioSource.PlayOneShot(sfxs[Random.Range(0, sfxs.Length)]);

            if (_questionManager.CheckAnswer(true))
                _scoreManager.IncreaseScore(replyTimer, CardType == CardType.DoublePoint);

            _questionManager.AssignNewQuestion();

            base.SwipeRight();
        }

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
