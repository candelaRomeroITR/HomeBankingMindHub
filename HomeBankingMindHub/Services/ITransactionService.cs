using System.Transactions;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        public Transaction createTransaction(Transaction transaction);
    }
}
