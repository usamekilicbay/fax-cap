using FaxCap.Manager;
using TMPro;
using UnityEngine;
using Zenject;

namespace FaxCap.UI.Screen
{
    public class UIHomeScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI leftNavigationText;
        [SerializeField] private TextMeshProUGUI topNavigationText;
        [SerializeField] private TextMeshProUGUI rightNavigationText;
        [SerializeField] private TextMeshProUGUI bottomNavigationText;

        private const string _leftNavigationFrontSideName = "";
        private const string _topNavigationFrontSideName = "";
        private const string _rightNavigationFrontSideName = "";
        private const string _bottomNavigationFrontSideName = "";
        private const string _leftNavigationBackSideName = "";
        private const string _topNavigationBackSideName = "";
        private const string _rightNavigationBackSideName = "";
        private const string _bottomNavigationBackSideName = "";

        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(GameManager gameManager,
            UIGameScreen gameScreen)
        {
            _uiGameScreen = gameScreen;
        }

        public void UpdateNavigationUI(bool isFrontSideActive = true)
        {
            UpdateNavigationTexts(isFrontSideActive);
        }

        private void UpdateNavigationTexts(bool isFrontSideActive = true)
        {
            var leftNavigationName = isFrontSideActive
                ? _leftNavigationFrontSideName
                : _leftNavigationBackSideName;

            var topNavigationName = isFrontSideActive
                ? _topNavigationFrontSideName
                : _topNavigationBackSideName;

            var rightNavigationName = isFrontSideActive
                ? _rightNavigationFrontSideName
                : _rightNavigationBackSideName;

            var bottomNavigationName = isFrontSideActive
                ? _bottomNavigationFrontSideName
                : _bottomNavigationBackSideName;

            leftNavigationText.SetText(leftNavigationName.ToString());
            topNavigationText.SetText(topNavigationName.ToString());
            rightNavigationText.SetText(rightNavigationName.ToString());
            bottomNavigationText.SetText(bottomNavigationName.ToString());
        }
    }
}
