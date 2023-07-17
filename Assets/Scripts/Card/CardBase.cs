using Assets.Scripts.Common.Types;
using DG.Tweening;
using FaxCap.Configs;
using FaxCap.Manager;
using FaxCap.UI.Screen;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.Card
{
    public abstract class CardBase : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] protected GameObject frontSide;
        [SerializeField] protected GameObject backSide;
        [SerializeField] private CardMovementConfigs cardMovementConfigs;

        protected RectTransform cardTransform;

        protected bool isFrontSideShown;

        protected bool isTimerStart;

        public float replyTimer = 5f;

        public bool isDone = false;

        private Vector2 _initialPosition;
        private Quaternion _initialRotation;

        protected MovementAxis movementAxis = MovementAxis.Initial; // Flag to lock the initial movement direction
        protected MovementDirection allowedMovementDirections;
        private bool _isInThresholdCircle = true; // Flag to lock the initial movement direction

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
#if UNITY_EDITOR
            if (!_isThresholdDebugRingCreated)
                CreateCircle();
#endif
        }

        protected virtual void Start()
        {
            // ...
        }

        protected virtual void Update()
        {
#if UNITY_EDITOR
            UpdateDebugRingColor();
#endif
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate the distance of the drag
            float dragDistanceX = eventData.position.x - eventData.pressPosition.x;
            float dragDistanceY = eventData.position.y - eventData.pressPosition.y;

            var distance = Vector2.Distance(cardTransform.anchoredPosition, _initialPosition);

            _isInThresholdCircle = distance < cardMovementConfigs.ThresholdRadius;

            // Check if the card has moved outside the threshold circle
            if (_isInThresholdCircle)
            {
                movementAxis = MovementAxis.Neutral;

                float targetX = Mathf.Clamp(_initialPosition.x + dragDistanceX, cardMovementConfigs.MinHorizontalRange, cardMovementConfigs.MaxHorizontalRange);
                float targetY = Mathf.Clamp(_initialPosition.y + dragDistanceY, cardMovementConfigs.MinVerticalRange, cardMovementConfigs.MaxVerticalRange);
                cardTransform.anchoredPosition = new Vector2(targetX, targetY);

                if (targetX > _initialPosition.x)
                {
                    float leanRotationAngle = Mathf.Lerp(0f, -cardMovementConfigs.MaxRotationAngle, Mathf.InverseLerp(_initialPosition.x, cardMovementConfigs.MaxHorizontalRange, targetX));
                    cardTransform.rotation = Quaternion.Euler(0f, 0f, leanRotationAngle);
                }
                else if (targetX < _initialPosition.x)
                {
                    float leanRotationAngle = Mathf.Lerp(0f, cardMovementConfigs.MaxRotationAngle, Mathf.InverseLerp(_initialPosition.x, cardMovementConfigs.MinHorizontalRange, targetX));
                    cardTransform.rotation = Quaternion.Euler(0f, 0f, leanRotationAngle);
                }

                return;
            }

            var posX = cardTransform.anchoredPosition.x;
            var posY = cardTransform.anchoredPosition.y;

            movementAxis = Mathf.Abs(posX) >= Mathf.Abs(posY)
                ? MovementAxis.Horizontal
                : MovementAxis.Vertical;

            // Move the card horizontally based on the drag distance, within the movement range limits
            if (movementAxis == MovementAxis.Horizontal)
            {
                float targetX = Mathf.Clamp(_initialPosition.x + dragDistanceX, cardMovementConfigs.MinHorizontalRange, cardMovementConfigs.MaxHorizontalRange);
                cardTransform.anchoredPosition = new Vector2(targetX, _initialPosition.y);

                // Rotate back to initial rotation and lean to the left side if moving towards the right beyond the initial position
                if (targetX > _initialPosition.x)
                {
                    float leanRotationAngle = Mathf.Lerp(0f, -cardMovementConfigs.MaxRotationAngle, Mathf.InverseLerp(_initialPosition.x, cardMovementConfigs.MaxHorizontalRange, targetX));
                    cardTransform.rotation = Quaternion.Euler(0f, 0f, leanRotationAngle);
                }
                else if (targetX < _initialPosition.x)
                {
                    float leanRotationAngle = Mathf.Lerp(0f, cardMovementConfigs.MaxRotationAngle, Mathf.InverseLerp(_initialPosition.x, cardMovementConfigs.MinHorizontalRange, targetX));
                    cardTransform.rotation = Quaternion.Euler(0f, 0f, leanRotationAngle);
                }
            }
            // Move the card vertically based on the drag distance
            else if (movementAxis == MovementAxis.Vertical)
            {
                float targetY = Mathf.Clamp(_initialPosition.y + dragDistanceY, cardMovementConfigs.MinHorizontalRange, cardMovementConfigs.MaxHorizontalRange);
                cardTransform.anchoredPosition = new Vector2(_initialPosition.x, targetY);
                cardTransform.rotation = _initialRotation;
            }

            // Calculate the normalized position within the movement range
            float targetXNormalized = Mathf.InverseLerp(cardMovementConfigs.MinHorizontalRange, cardMovementConfigs.MaxHorizontalRange, cardTransform.anchoredPosition.x);
            float targetYNormalized = Mathf.InverseLerp(cardMovementConfigs.MinHorizontalRange, cardMovementConfigs.MaxHorizontalRange, cardTransform.anchoredPosition.y);

            // Smoothly interpolate the background color between red (left side), white (midpoint), and green (right side)
            Color targetColor = Color.Lerp(Color.red, Color.green, movementAxis == MovementAxis.Horizontal ? targetXNormalized : targetYNormalized);
            targetColor = Color.Lerp(Color.white, targetColor, Mathf.Abs((movementAxis == MovementAxis.Horizontal ? targetXNormalized : targetYNormalized) - 0.5f) * 2f);
            gameScreen.UpdateBackgroundColor(targetColor);
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            _isInThresholdCircle = true;

            float posX = cardTransform.anchoredPosition.x;
            float posY = cardTransform.anchoredPosition.y;

            // If the drag distance does not exceed the threshold, reset the card position and the background color
            if (IsInTheThreshold())
            {
                if (IsCardInTheInitialPosition())
                {
                    movementAxis = MovementAxis.Initial;
                    return;
                }

                ResetCard();
                movementAxis = MovementAxis.Neutral; // Reset the direction locking
                return;
            }

            if (movementAxis == MovementAxis.Horizontal)
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
            movementAxis = MovementAxis.Neutral; // Reset the direction locking
        }

        #endregion

        protected virtual void Setup()
        {
            cardTransform = GetComponent<RectTransform>();
            _initialPosition = cardTransform.anchoredPosition;
            _initialRotation = cardTransform.rotation;
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

        protected virtual void SwipeRight()
        {
            cardTransform.DOAnchorPosX(1200f, 1f)
                .OnComplete(VanishCard);
        }

        protected virtual void SwipeUp()
        {
            // ...
        }

        protected virtual void SwipeDown()
        {
            // ...
        }

        private void ResetCard()
        {
            var duration = 0.5f;
            var color = gameScreen.BackgroundCurrentColor;
            var targetColor = gameScreen.BackgroundInitialColor;

            // Reset card position and rotation
            var colorTransitionTween = DOTween.To(() => color, x => color = x, targetColor, duration);
            colorTransitionTween.OnUpdate(() => gameScreen.UpdateBackgroundColor(color));
            colorTransitionTween.Pause();

            var sequence = DOTween.Sequence();
            sequence.Append(cardTransform.DOAnchorPos(_initialPosition, duration));
            sequence.Join(cardTransform.DORotateQuaternion(_initialRotation, duration));
            sequence.Join(colorTransitionTween);
            sequence.OnComplete(() =>
            {
                movementAxis = MovementAxis.Initial;
            });
        }

        private void VanishCard()
        {
            Destroy(gameObject);
        }

        public abstract void UpdateCard();

        #region Position Check

        private bool IsCardInTheInitialPosition()
        {
            return cardTransform.anchoredPosition == _initialPosition;
        }

        private bool IsInTheThreshold()
        {
            return IsInTheHorizontalThrehshold()
                && IsInTheVerticalThreshold();
        }

        private bool IsInTheHorizontalThrehshold()
        {
            float posX = cardTransform.anchoredPosition.x;

            return Mathf.Abs(posX) < cardMovementConfigs.ThresholdRadius;
        }

        private bool IsInTheVerticalThreshold()
        {
            float posY = cardTransform.anchoredPosition.y;

            return Mathf.Abs(posY) < cardMovementConfigs.ThresholdRadius;
        }


        #endregion

        #region Debug

        private static bool _isThresholdDebugRingCreated = false;
        private static Image _thresholdDebugRing;

        private void CreateCircle()
        {
            _isThresholdDebugRingCreated = true;

            var circleObject = new GameObject(GetThresholdDebugRingName());
            circleObject.transform.SetParent(transform.root, false);

            var rectTransform = circleObject.AddComponent<RectTransform>();

            // Set the position and size of the circle to be centered on the screen
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;

            var ringImage = circleObject.AddComponent<Image>();
            ringImage.color = Color.red;

            // Create a ring texture dynamically
            var ringTexture = GenerateRingTexture((int)cardMovementConfigs.ThresholdRadius * 2, 10f); // Adjust the thickness as needed

            // Create a sprite with the ring texture
            var ringSprite = Sprite.Create(ringTexture, new Rect(0, 0, ringTexture.width, ringTexture.height), Vector2.one * 0.5f);
            ringImage.sprite = ringSprite;

            float diameter = cardMovementConfigs.ThresholdRadius * 2f;
            rectTransform.sizeDelta = new Vector2(diameter, diameter);

            ringImage.raycastTarget = false;

            _thresholdDebugRing = ringImage;
        }


        private Texture2D GenerateRingTexture(int size, float thickness)
        {
            var texture = new Texture2D(size, size);
            var pixels = new Color[size * size];

            // Calculate the radius and center of the ring
            var outerRadius = size * 0.5f;
            var innerRadius = outerRadius - thickness;
            var center = new Vector2(outerRadius, outerRadius);

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    // Calculate the distance from the center of the ring
                    var distance = Vector2.Distance(center, new Vector2(x, y));

                    // Check if the pixel is within the ring
                    if (distance <= outerRadius && distance >= innerRadius)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private void UpdateDebugRingColor()
        {
            if (_thresholdDebugRing == null)
                return;

            _thresholdDebugRing.color = IsInTheThreshold()
                ? Color.red
                : Color.green;
        }

        private static string GetThresholdDebugRingName()
            => nameof(_thresholdDebugRing).ToUpper();

        #endregion
    }
}
