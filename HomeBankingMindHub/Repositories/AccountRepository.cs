using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(client => client.Transactions)
                .ToList();
        }

        public Account FindById(long id)
        {
            return FindByCondition(client => client.Id == id)
                .Include(client => client.Transactions)
                .FirstOrDefault();
        }

    }
}
