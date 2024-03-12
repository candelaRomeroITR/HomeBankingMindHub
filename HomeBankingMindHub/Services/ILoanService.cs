using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        public List<LoanDTO> getAllLoans();
        public Loan getLoanById(long id);
        public ClientLoan createNewClientLoan(Client client, LoanApplicationDTO loanApplicationDTO);
    }
}
