using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class CurrencyManager : ICurrencyManager
    {
        private readonly int _startMoney = 100000;

        public static int CurrentMoney { get; private set; }

        private UIGameScreen _uiGameScreen;

        [Inject]
        public void Construct(UIGameScreen uiGameScreen)
        {
            _uiGameScreen = uiGameScreen;
            CurrentMoney = _startMoney;
        }

        public bool BuyItem(int amount)
        {
            if (CurrentMoney < amount)
            {
                Debug.LogWarning($"Not enough money! Required: ${amount}, Owned: ${CurrentMoney}");
                return false;
            }

            CurrentMoney -= amount;
            _uiGameScreen.UpdateScoreText(CurrentMoney);

            return true;
        }

        public void SellItem(int amount)
        {
            Debug.LogWarning($"Item sold for: ${amount}, Owned: ${CurrentMoney}");

            CurrentMoney += amount;
            _uiGameScreen.UpdateScoreText(CurrentMoney);
        }
    }
}
