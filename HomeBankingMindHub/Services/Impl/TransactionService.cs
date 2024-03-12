using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public void createTransaction(TransferDTO transferDTO)
        {
            Account accountOrigen = _accountRepository.FindByNumber(transferDTO.fromAccountNumber);
            Account accountDestino = _accountRepository.FindByNumber(transferDTO.toAccountNumber);

            Transaction newTransactionOrigen = new Transaction(accountOrigen, transferDTO, TransactionType.DEBIT);
            Transaction newTransactionDestino = new Transaction(accountDestino, transferDTO, TransactionType.CREDIT);

            _transactionRepository.Save(newTransactionOrigen);
            _transactionRepository.Save(newTransactionDestino);

            accountOrigen.Balance -= transferDTO.amount;
            accountDestino.Balance += transferDTO.amount;

            _accountRepository.Save(accountOrigen);
            _accountRepository.Save(accountDestino);
        }
        public void createTransactionLoan(Account account, LoanApplicationDTO loanApplicationDTO, Loan loan)
        {
            Transaction newTransaction = new Transaction(account, loanApplicationDTO, loan);
            
            _transactionRepository.Save(newTransaction);

            account.Balance += loanApplicationDTO.Amount;

            _accountRepository.Save(account);
        }

    }
}
