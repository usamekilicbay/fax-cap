using FaxCap.Card;
using FaxCap.Manager;
using FaxCap.UI.Dialog;
using FaxCap.UI.Screen;
using UnityEngine;
using Zenject;

namespace FaxCap.Installer
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameObject questionCardPrefab;

        public override void InstallBindings()
        {
            Container
                .Bind<GameManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ConfigurationManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<DeckManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<QuestionManager>()
                .To<QuestionManager>()
                .AsSingle();

            Container
                .Bind<ICurrencyManager>()
                .To<CurrencyManager>()
                .AsSingle();

            Container
                .Bind<ScoreManager>()
                .To<ScoreManager>()
                .AsSingle();

            Container
                .Bind<LevelManager>()
                .To<LevelManager>()
                .AsSingle();

            Container
                .Bind<ProgressManager>()
                .To<ProgressManager>()
                .AsSingle();

            Container
                .Bind<NavigationFacade>()
                .AsSingle();

            Container
                .Bind<CardFacade>()
                .AsSingle();

            Container
                .BindFactory<QuestionCard, QuestionCard.Factory>()
                .FromComponentInNewPrefab(questionCardPrefab);

            #region UI

            Container
                .Bind<UIManagerBase>()
                .FromComponentInHierarchy()
                .AsSingle();

            var screens = FindObjectsOfType<UIScreenBase>(true);
            foreach (var screen in screens)
                Container.Bind(screen.GetType()).FromComponentsInHierarchy().AsSingle();

            var dialogs = FindObjectsOfType<UIDialogBase>(true);
            foreach (var dialog in dialogs)
                Container.Bind(dialog.GetType()).FromComponentsInHierarchy().AsSingle();

            #endregion
        }
    }
}
