
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        public void createTransaction(TransferDTO transferDTO);
        public void createTransactionLoan(Account account, LoanApplicationDTO loanApplicationDTO, Loan loan);
    }
}
