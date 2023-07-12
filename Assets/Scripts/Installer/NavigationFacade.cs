using FaxCap.Manager;
using FaxCap.UI.Screen;
using Zenject;

namespace FaxCap.Installer
{
    public class NavigationFacade
    {
        public GameManager GameManager {  get; private set; }
        public UIManagerBase UIManager { get; private set; }
        public UIHomeScreen HomeScreen { get; private set; }
        public UIGameScreen GameScreen { get; private set; }
        public UIScreenBase SettingsScreen { get; private set; }
        public UIScreenBase LeaderboardScreen { get; private set; }
        public UIScreenBase ProfileScreen { get; private set; }
        public UIScreenBase SendQuestionScreen { get; private set; }

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            GameManager  = diContainer.Resolve<GameManager>();
            UIManager = diContainer.Resolve<UIManagerBase>();
            HomeScreen = diContainer.Resolve<UIHomeScreen>();
            GameScreen = diContainer.Resolve<UIGameScreen>();
            SettingsScreen= diContainer.Resolve<UIHomeScreen>();
            LeaderboardScreen = diContainer.Resolve<UIHomeScreen>();
            ProfileScreen = diContainer.Resolve<UIHomeScreen>();
            SendQuestionScreen = diContainer.Resolve<UIHomeScreen>();
        }
    }
}
