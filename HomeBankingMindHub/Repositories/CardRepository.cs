using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositorioContext) : base(repositorioContext)
        {

        }
        public IEnumerable<Card> GetAllCards()
        {
            return FindAll().ToList();
        }

        public bool ExistsByCardHolder(string cardHolder)
        {
            return FindByCondition(card => card.CardHolder == cardHolder).Any();
        }
        
        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
