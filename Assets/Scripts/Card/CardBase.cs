using DG.Tweening;
using FaxCap.Common.Types;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using Zenject;

namespace FaxCap.Card
{
    public abstract class CardBase : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] protected GameObject frontSide;
        [SerializeField] protected GameObject backSide;

        protected RectTransform cardTransform;

        protected bool isFrontSideShown;
        private Vector3 _defaultScale;

        protected bool isTimerStart;

        public float replyTimer = 5f;

        public bool isDone = false;

        private Vector2 initialPosition;
        private Quaternion initialRotation;
        private bool isDragging = false;

        public CardType CardType;

        private readonly float _swipeThreshold = 100f; // Minimum distance to trigger swipe
        private readonly float _movementThreshold = 50f; // Minimum distance to move the card
        private readonly float _rotationFactor = 0.2f; // Rotation factor for swipe effect
        private readonly float _movementSpeed = 5f; // Speed at which the card moves back to the center
        private readonly float _maxRotationAngle = 30f; // Maximum rotation angle
        private readonly Range _horizontalMovementRange = new() { Min = -180f, Max = 180f }; // Movement range limits
        private readonly Range _verticalMovementRange = new() { Min = -500f, Max = 500f }; // Movement range limits
        private readonly float _rotationSpeed = 5f; // Adjust the rotation speed as needed
        private bool _isHorizontalDrag = false; // Flag to lock the initial movement direction
        private bool _isHorizontalDragLocked = false; // Flag to lock the initial movement direction

        public class Range
        {
            public float Min { get; set; }
            public float Max { get; set; }
        }

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
            // ...
        }

        protected virtual void Start()
        {
            // ...
        }

        protected virtual void Update()
        {
            // ...
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate the distance of the drag
            float dragDistanceX = eventData.position.x - eventData.pressPosition.x;
            float dragDistanceY = eventData.position.y - eventData.pressPosition.y;

            // Determine the dominant axis based on the absolute drag distances
            bool isHorizontalDrag = Mathf.Abs(dragDistanceX) >= Mathf.Abs(dragDistanceY);

            // Check if dragging has started in the current direction
            if (!isDragging)
            {
                isDragging = true;
                _isHorizontalDragLocked = isHorizontalDrag; // Lock the direction of movement
            }

            // Move the card horizontally based on the drag distance, within the movement range limits
            if (_isHorizontalDragLocked)
            {
                float targetX = Mathf.Clamp(initialPosition.x + dragDistanceX, _horizontalMovementRange.Min, _horizontalMovementRange.Max);
                cardTransform.anchoredPosition = new Vector2(targetX, initialPosition.y);

                // Rotate back to initial rotation and lean to the left side if moving towards the right beyond the initial position
                if (targetX > initialPosition.x)
                {
                    float leanRotationAngle = Mathf.Lerp(0f, -_maxRotationAngle, Mathf.InverseLerp(initialPosition.x, _horizontalMovementRange.Max, targetX));
                    cardTransform.rotation = Quaternion.Euler(0f, 0f, leanRotationAngle);
                }
                else if (targetX < initialPosition.x)
                {
                    float leanRotationAngle = Mathf.Lerp(0f, _maxRotationAngle, Mathf.InverseLerp(initialPosition.x, _horizontalMovementRange.Min, targetX));
                    cardTransform.rotation = Quaternion.Euler(0f, 0f, leanRotationAngle);
                }
            }
            // Move the card vertically based on the drag distance
            else if (!_isHorizontalDragLocked)
            {
                float targetY = Mathf.Clamp(initialPosition.y + dragDistanceY, _horizontalMovementRange.Min, _horizontalMovementRange.Max);
                cardTransform.anchoredPosition = new Vector2(initialPosition.x, targetY);
                cardTransform.rotation = initialRotation;
            }

            // Calculate the normalized position within the movement range
            float targetXNormalized = Mathf.InverseLerp(_horizontalMovementRange.Min, _horizontalMovementRange.Max, cardTransform.anchoredPosition.x);
            float targetYNormalized = Mathf.InverseLerp(_horizontalMovementRange.Min, _horizontalMovementRange.Max, cardTransform.anchoredPosition.y);

            // Smoothly interpolate the background color between red (left side), white (midpoint), and green (right side)
            Color targetColor = Color.Lerp(Color.red, Color.green, _isHorizontalDragLocked ? targetXNormalized : targetYNormalized);
            targetColor = Color.Lerp(Color.white, targetColor, Mathf.Abs((_isHorizontalDragLocked ? targetXNormalized : targetYNormalized) - 0.5f) * 2f);
            gameScreen.UpdateBackgroundColor(targetColor);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            float posX = cardTransform.anchoredPosition.x;
            float posY = cardTransform.anchoredPosition.y;

            // If the drag distance does not exceed the threshold, reset the card position and the background color
            if (Mathf.Abs(posX) < _swipeThreshold
                && Mathf.Abs(posY) < _swipeThreshold)
            {
                ResetCard();
                _isHorizontalDragLocked = false; // Reset the direction locking
                return;
            }

            if (_isHorizontalDragLocked)
            {
                if (posX > 0f)
                    SwipeRight();
                else if (posX < 0f)
                    SwipeLeft();
            }
            else
            {
                if (posY > 0f)
                    SwipeUp();
                else if (posY < 0f)
                    SwipeDown();
            }

            // Reset the direction locking
            _isHorizontalDragLocked = false;
        }

        private IEnumerator RotateCardToInitialRotation()
        {
            Quaternion startRotation = cardTransform.rotation;
            Quaternion targetRotation = initialRotation;

            float rotationTime = 0f;
            float rotationDuration = 0.5f; // Adjust the duration as needed

            while (rotationTime < rotationDuration)
            {
                rotationTime += Time.deltaTime;
                float t = rotationTime / rotationDuration;
                cardTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            cardTransform.rotation = targetRotation;
        }

        #endregion

        protected virtual void Setup()
        {
            cardTransform = GetComponent<RectTransform>();
            initialPosition = cardTransform.anchoredPosition;
            initialRotation = cardTransform.rotation;
        }

        protected void FlipCard()
        {
            var targetAngle = cardTransform.rotation.eulerAngles + Vector3.up * 90;
            var duration = 0.5f;

            var sequence = DOTween.Sequence();
            sequence.Append(cardTransform.DORotate(targetAngle, duration));
            sequence.Join(canvasGroup.DOFade(0f, duration));
            sequence.OnComplete(UpdateShownSide);
        }

        private void UpdateShownSide()
        {
            if (isFrontSideShown)
                ShowBackSide();
            else
                ShowFrontSide();

            var duration = 0.5f;
            var sequence = DOTween.Sequence();
            sequence.Append(cardTransform.DORotate(Vector3.zero, duration));
            sequence.Join(canvasGroup.DOFade(1f, duration));
            sequence.OnComplete(() => isTimerStart = true);
        }

        protected void ShowFrontSide()
        {
            frontSide.SetActive(true);
            backSide.SetActive(false);
            isFrontSideShown = true;
        }

        protected void ShowBackSide()
        {
            frontSide.SetActive(false);
            backSide.SetActive(true);
            isFrontSideShown = false;
        }

        protected virtual void SwipeLeft()
        {
            cardTransform.DOAnchorPosX(-1200f, 1f)
                 .OnComplete(VanishCard);
        }

        protected virtual void SwipeUp()
        {
            // TODO: Complete run without fail
        }

        protected virtual void SwipeRight()
        {
            cardTransform.DOAnchorPosX(1200f, 1f)
                .OnComplete(VanishCard);
        }

        protected virtual void SwipeDown()
        {
            // TODO: Complete run without fail
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
