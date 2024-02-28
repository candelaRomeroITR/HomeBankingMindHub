using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        bool ExistsByCardHolder(string cardHolder);
    }
}
