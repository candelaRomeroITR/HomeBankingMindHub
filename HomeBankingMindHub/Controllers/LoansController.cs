using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private ILoanService _loanService;
        private IAccountService _accountService;
        private IClientService _clientService;
        private ITransactionService _transactionService;

        public LoansController(ILoanService loanService, IAccountService accountService, 
            IClientService clientService, ITransactionService transactionService)
        {
            _loanService = loanService;
            _accountService = accountService;
            _clientService = clientService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult GetLoans()
        {
            try
            {
                return Ok(_loanService.getAllLoans());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult PostLoans([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            using(var scope = new TransactionScope())
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

                        Loan loan = _loanService.getLoanById(loanApplicationDTO.LoanId);
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

                        if (!_accountService.accountExistsByNumber(loanApplicationDTO.ToAccountNumber))
                        {
                            return StatusCode(403, "Cuenta destino inexistente");
                        }

                        Client client = _clientService.getClientByEmail(email);
                        if(!client.Accounts.Any(account => account.Number == loanApplicationDTO.ToAccountNumber))
                        {
                            return StatusCode(403, "La cuenta destino no le pertenece");
                        }

                        ClientLoan newClientLoan = _loanService.createNewClientLoan(client, loanApplicationDTO);

                        Account account = _accountService.getAccountByNumber(loanApplicationDTO.ToAccountNumber);
                        long cuentaDestino = account.Id;

                        _transactionService.createTransactionLoan(account, loanApplicationDTO, loan);
                     
                        scope.Complete();
                        return Ok();

                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }
            }
            
    }
}
