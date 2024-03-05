using HomeBankingMindHub.dtos;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult GetLoans()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                var loansDTO = new List<LoanDTO>();

                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loansDTO.Add(newLoanDTO);
                }
                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult PostLoans([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                if (loanApplicationDTO == null)
                {
                    return Forbid();
                }
                //obtener info cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                if (loanApplicationDTO.LoanId == 0 || loanApplicationDTO.Amount == 0 || loanApplicationDTO.Payments == null || loanApplicationDTO.ToAccountNumber == null)
                {
                    return StatusCode(403, "Hay datos nulos");
                }

                if (loanApplicationDTO.Amount < 0)
                {
                    return StatusCode(403, "El monto debe ser positivo");
                }

                Loan loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                List<int> cuotasDisponibles = loan.Payments.Split(',').Select(int.Parse).ToList();
                int cuotaSeleccionada;
                if(int.TryParse(loanApplicationDTO.Payments, out cuotaSeleccionada))
                {
                    if (!cuotasDisponibles.Contains(cuotaSeleccionada))
                        {
                        return StatusCode(403, "Cuota seleccionada no disponible para este prestamo");
                        }
                }

                if(loan.MaxAmount < loanApplicationDTO.Amount)
                {
                    return StatusCode(403, $"El monto del prestamo no puede ser mayor a {loan.MaxAmount}");
                }

                if (!_accountRepository.ExistsByNumber(loanApplicationDTO.ToAccountNumber))
                {
                    return StatusCode(403, "Cuenta destino inexistente");
                }

                Client client = _clientRepository.FindByEmail(email);
                if(!client.Accounts.Any(account => account.Number == loanApplicationDTO.ToAccountNumber))
                {
                    return StatusCode(403, "La cuenta destino no le pertenece");
                }

                ClientLoan newClientLoan = new ClientLoan
                {
                    Amount = loanApplicationDTO.Amount + (loanApplicationDTO.Amount * 0.2),
                    Payments = loanApplicationDTO.Payments,
                    ClientId = client.Id,
                    LoanId = loanApplicationDTO.LoanId,        
                };

                _clientLoanRepository.Save(newClientLoan);

                Account account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                long cuentaDestino = account.Id;

                Transaction newTransaction = new Transaction
                {
                    AccountId = cuentaDestino,
                    Type = TransactionType.CREDIT,
                    Amount = loanApplicationDTO.Amount,
                    Description = $"{loan.Name} loan approved", 
                    Date = DateTime.Now,
                };

                _transactionRepository.Save(newTransaction);

                account.Balance += loanApplicationDTO.Amount;

                _accountRepository.Save(account);
                
                return StatusCode(201, "Prestamo realizado");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
