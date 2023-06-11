using FaxCap.Manager;
using Zenject;

namespace FaxCap
{
    public class CardFacade
    {
        public DeckManager DeckManager { get; private set; }
        public ICurrencyManager CurrencyManager { get; private set; }

        [Inject]
        public void Construct(DeckManager deckManager,
            ICurrencyManager currencyManager)
        {
            DeckManager = deckManager;
            CurrencyManager = currencyManager;
        }
    }
}
