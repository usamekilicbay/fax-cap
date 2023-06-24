using FaxCap.Common.Abstract;
using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class LevelManager : ICompletable
    {
        public int Level { get; private set; } = 1;
        public int Exp { get; private set; } = 0;
        public int RequiredExp { get; private set; } = 100;

        private UIResultScreen _resultScreen;

        [Inject]
        public void Construct(UIResultScreen resultScreen)
        {
            _resultScreen = resultScreen;
        }

        // Formula to calculate required experience
        public int GetRequiredExp(int level)
        {
            // Example formula: 100 + (level - 1) * 50
            var requiredExp = 100 + (level - 1) * 50;

            return requiredExp;
        }

        // Add experience points
        public void AddExp(int expPoints)
        {
            Exp += expPoints;

            // Check if player level up
            while (Exp >= RequiredExp)
                LevelUp();
        }

        // Level up
        public void LevelUp()
        {
            Level++;
            Exp -= RequiredExp;
            RequiredExp = GetRequiredExp(Level);

            Debug.Log("Level Up! You reached level " + Level);
        }

        public void Complete()
        {
            _resultScreen.UpdateLevelBar(Exp);
            _resultScreen.UpdateLevelTexts(Level);
        }
    }
}
