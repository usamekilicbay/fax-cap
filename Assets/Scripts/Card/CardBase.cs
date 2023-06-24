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
    public abstract class CardBase : MonoBehaviour
    {
        [SerializeField] private GameObject frontSide;
        [SerializeField] private GameObject backSide;
        [SerializeField] private CanvasGroup canvasGroup;

        protected RectTransform cardTransform;

        private bool _isBackSideShown = true;
        private Vector3 _defaultScale;

        protected bool isTimerStart;

        public float replyTimer = 5f;

        public bool isDone = false;

        private Vector2 initialPosition;
        private Quaternion initialRotation;
        private bool isDragging = false;

        public CardType CardType;

        public float swipeThreshold = 100f; // Minimum distance to trigger swipe
        public float rotationFactor = 0.2f; // Rotation factor for swipe effect
        public float movementSpeed = 5f; // Speed at which the card moves back to the center
        public float maxRotationAngle = 30f; // Maximum rotation angle
        public Vector2 movementRange = new(-200f, 200f); // Movement range limits

        public bool IsUsed { get; protected set; }

        #region Dependency Injection

        protected DeckManager deckManager;
        protected ICurrencyManager currencyManager;
        protected UIGameScreen gameScreen;

        [Inject]
        public void Construct(CardFacade cardFacade,
            UIGameScreen gameScreen)
        {
            deckManager = cardFacade.DeckManager;
            currencyManager = cardFacade.CurrencyManager;
            this.gameScreen = gameScreen;
        }

        #endregion

        #region Unity

        protected virtual void Awake()
        {
            Setup();
        }

        protected virtual void Start()
        {
            FlipCard();
        }

        protected virtual void Update()
        {
            if (isDone)
                return;

            // If the card is not being dragged and it's not in the initial position,
            // gradually move it back to the center position
            //if (!isDragging && cardTransform.anchoredPosition != initialPosition)
            //{
            //    cardTransform.anchoredPosition = Vector2.Lerp(cardTransform.anchoredPosition, initialPosition, movementSpeed * Time.deltaTime);
            //    cardTransform.rotation = Quaternion.Lerp(cardTransform.rotation, initialRotation, movementSpeed * Time.deltaTime);
            //}
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate the distance of the drag
            float dragDistance = eventData.position.x - eventData.pressPosition.x + initialPosition.x;
            //float dragDistance = eventData.position.x - eventData.pressPosition.x;

            // Apply the rotation effect based on the drag distance, even if it's below the rotation threshold
            float rotationAngle = Mathf.Clamp(dragDistance * rotationFactor, -maxRotationAngle, maxRotationAngle);
            cardTransform.rotation = initialRotation * Quaternion.Euler(0f, 0f, -rotationAngle);

            // Move the card horizontally based on the drag distance, within the movement range limits
            float targetX = Mathf.Clamp(initialPosition.x + dragDistance, movementRange.x, movementRange.y);
            cardTransform.anchoredPosition = new Vector2(targetX, initialPosition.y);

            // Calculate the normalized position within the movement range
            float normalizedPosition = Mathf.InverseLerp(movementRange.x, movementRange.y, targetX);

            // Smoothly interpolate the background color between red (left side), white (midpoint), and green (right side)
            Color targetColor = Color.Lerp(Color.red, Color.green, normalizedPosition);
            // Smoothly interpolate to white at the midpoint
            targetColor = Color.Lerp(gameScreen.BackgroundInitialColor, targetColor, Mathf.Abs(normalizedPosition - 0.5f) * 2f);
            gameScreen.UpdateBackgroundColor(targetColor);

            isDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            // Calculate the distance of the drag
            float dragDistance = eventData.position.x - eventData.pressPosition.x;

            // If the drag distance does not exceed the threshold, call the reset the card positon and the background color
            if (Mathf.Abs(dragDistance) < swipeThreshold)
            {
                // Reset card position and rotation if the drag distance is below the threshold
                ResetCard();
                return;
            }

            isDone = true;

            if (dragDistance > 0f)
                SwipeRight();
            else
                SwipeLeft();
        }

        #endregion

        private void Setup()
        {
            frontSide.SetActive(false);
            backSide.SetActive(true);
            //backSide.transform.rotation = Quaternion.Euler(Vector3.up * 180);
            //_defaultScale = transform.localScale;
            //transform.localScale = _defaultScale * 0.7f;
            //transform.DOScale(_defaultScale, 0.5f);
            //var cloakColor = cloakRenderer.color;
            //cloakColor.a = 0;
            //cloakRenderer.color = cloakColor;

            cardTransform = GetComponent<RectTransform>();
            initialPosition = cardTransform.anchoredPosition;
            initialRotation = cardTransform.rotation;
        }

        private void FlipCard()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(cardTransform.DORotate(cardTransform.rotation.eulerAngles + Vector3.up * 90, 0.5f));
            sequence.Join(canvasGroup.DOFade(0f, 0.5f));
            sequence.OnComplete(UpdateShownSide);
        }

        private void UpdateShownSide()
        {
            if (_isBackSideShown)
            {
                frontSide.SetActive(true);
                backSide.SetActive(false);
                _isBackSideShown = false;
            }
            else
            {
                frontSide.SetActive(false);
                backSide.SetActive(true);
                _isBackSideShown = true;
            }

            var sequence = DOTween.Sequence();
            sequence.Append(cardTransform.DORotate(Vector3.zero, 0.5f));
            sequence.Join(canvasGroup.DOFade(1f, 0.5f));
            sequence.OnComplete(() => isTimerStart = true);
        }

        protected virtual void SwipeRight()
        {
            cardTransform.DOAnchorPosX(1200f, 1f)
                .OnComplete(VanishCard);
        }

        protected virtual void SwipeLeft()
        {
            cardTransform.DOAnchorPosX(-1200f, 1f)
                 .OnComplete(VanishCard);
        }

        private void ResetCard()
        {
            var duration = 0.5f;
            var color = gameScreen.BackgroundCurrentColor;
            var targetColor = gameScreen.BackgroundInitialColor;

            // Reset card position and rotation
            var sequence = DOTween.Sequence();
            sequence.Append(cardTransform.DOAnchorPos(initialPosition, duration));
            sequence.Join(cardTransform.DORotateQuaternion(initialRotation, duration));
            sequence.Join(DOTween.To(() => color, x => color = x, targetColor, duration)
                .OnUpdate(() =>
                {
                    gameScreen.UpdateBackgroundColor(color);
                }));
        }


        private void VanishCard()
        {
            Destroy(gameObject);
            //cloakRenderer.DOFade(1, 0.5f)
            //    .OnComplete(() => Destroy(gameObject));
        }

        public abstract void UpdateCard();
    }
}
