using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;

namespace HomeBankingMindHub.Services.Impl
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        public LoanService(ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository)
        {
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
        }
        public Loan getLoanById(long id)
        {
            return _loanRepository.FindById(id);
        }
        public List<LoanDTO> getAllLoans()
        {
            var loans = _loanRepository.GetAllLoans();
            var listLoanDTO = new List<LoanDTO>();

            foreach (Loan loan in loans)
            {
                var newLoanDTO = new LoanDTO(loan);
                listLoanDTO.Add(newLoanDTO);
            }
            
            return listLoanDTO;
        }
        public Loan getLoanByEmail(long id)
        {
            return _loanRepository.FindById(id);
        }
        public ClientLoan createNewClientLoan(Client client, LoanApplicationDTO loanApplicationDTO)
        {
            ClientLoan newClientLoan = new ClientLoan(loanApplicationDTO, client);

            _clientLoanRepository.Save(newClientLoan);
            
            return newClientLoan;
        }


    }
}
