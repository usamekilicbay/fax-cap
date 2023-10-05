using FaxCap.Common.Abstract;
using FaxCap.UI.Screen;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace FaxCap.Manager
{
    public class LevelManager : ICompletable
    {
        public int TempExp { get; private set; } = 0;
        public int TempRequiredExp { get; private set; } = 0;
        public int TempLevel { get; private set; } = 1;
        public int Exp { get; private set; } = 0;
        public int RequiredExp { get; private set; } = 0;
        public int Level { get; private set; } = 0;

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
            TempExp += expPoints;

            // Check if player level up
            while (TempExp >= TempRequiredExp)
                LevelUp();
        }

        // Level up
        public void LevelUp()
        {
            TempLevel++;

            TempExp -= TempRequiredExp;
            TempRequiredExp = GetRequiredExp(TempLevel);

            Debug.Log("Level Up! You reached level " + Level);
        }

        public async Task Complete(bool isSuccessful = true)
        {
            Exp = TempExp;
            RequiredExp = TempRequiredExp;
            Level = TempLevel;

            await _resultScreen.UpdateLevel(Exp, Level);
        }
    }
}
