using FaxCap.Common.Abstract;
using FaxCap.UI.Screen;
using System.Threading.Tasks;
using Zenject;

namespace FaxCap.Manager
{
    public class PlayerStatsManager : ICompletable
    {
        private SaveManager _saveManager;
        private UIResultScreen _resultScreen;

        [Inject]
        public void Construct(SaveManager saveManager, UIResultScreen resultScreen)
        {
            _saveManager = saveManager;
            _resultScreen = resultScreen;
        }

        public bool HasExceedBestScore(int currentScore)
            => currentScore > 0;

        public bool HasExceededBestCombo(int currentCombo)
            => currentCombo > 0;

        public bool HasExceededBestQuestionCount(int currentQuestionCount)
            => currentQuestionCount > 0;

        public async Task Complete(bool isSuccessful = true)
        {
            await _resultScreen.UpdateQuestionText(13);
            await _resultScreen.UpdateComboText(4);
        }
    }
}
